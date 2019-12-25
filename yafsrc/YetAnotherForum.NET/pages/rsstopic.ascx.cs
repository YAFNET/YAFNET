/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Syndication;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Utils.Helpers.StringUtils;

    #endregion

    /// <summary>
    /// Generates the RSS Feeds.
    /// </summary>
    public partial class rsstopic : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "rsstopic" /> class.
        /// </summary>
        public rsstopic()
            : base("RSSTOPIC")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Put user code to initialize the page here 
            if (!(this.Get<YafBoardSettings>().ShowRSSLink || this.Get<YafBoardSettings>().ShowAtomLink))
            {
                YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
            }

            // Atom feed as variable
            var atomFeedByVar = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ft")
                                 == YafSyndicationFormats.Atom.ToInt().ToString();

            YafSyndicationFeed feed = null;

            // var syndicationItems = new List<SyndicationItem>();
            var lastPostIcon = string.Empty;
            var lastPostName = this.GetText("DEFAULT", "GO_LAST_POST");

            YafRssFeeds feedType;

            try
            {
                feedType = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("pg").ToEnum<YafRssFeeds>(true);
            }
            catch
            {
                // default to Forum Feed.
                feedType = YafRssFeeds.Forum;
            }

            switch (feedType)
            {
                    // Latest posts feed
                case YafRssFeeds.LatestPosts:
                    if (
                        !(this.Get<YafBoardSettings>().ShowActiveDiscussions
                          && this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostLatestFeedAccess)))
                    {
                        YafBuildLink.AccessDenied();
                    }

                    this.GetPostLatestFeed(ref feed, feedType, atomFeedByVar, lastPostIcon, lastPostName);
                    break;

                    // Latest Announcements feed
                case YafRssFeeds.LatestAnnouncements:
                    if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ForumFeedAccess))
                    {
                        YafBuildLink.AccessDenied();
                    }

                    this.GetLatestAnnouncementsFeed(ref feed, feedType, atomFeedByVar);
                    break;

                    // Posts Feed
                case YafRssFeeds.Posts:
                    if (
                        !(this.PageContext.ForumReadAccess
                          && this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostsFeedAccess)))
                    {
                        YafBuildLink.AccessDenied();
                    }

                    if (this.Get<HttpRequestBase>().QueryString.Exists("t"))
                    {
                        if (int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t"), out var topicId))
                        {
                            this.GetPostsFeed(ref feed, feedType, atomFeedByVar, topicId);
                        }
                    }

                    break;

                    // Forum Feed
                case YafRssFeeds.Forum:
                    if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ForumFeedAccess))
                    {
                        YafBuildLink.AccessDenied();
                    }

                    int? categoryId = null;

                    if (this.Get<HttpRequestBase>().QueryString.Exists("c"))
                    {
                        categoryId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("c");
                    }

                    this.GetForumFeed(ref feed, feedType, atomFeedByVar, categoryId);
                    break;

                    // Topics Feed
                case YafRssFeeds.Topics:
                    if (
                        !(this.PageContext.ForumReadAccess
                          && this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().TopicsFeedAccess)))
                    {
                        YafBuildLink.AccessDenied();
                    }

                    if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
                    {
                        if (int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"), out var forumId))
                        {
                            this.GetTopicsFeed(ref feed, feedType, atomFeedByVar, lastPostIcon, lastPostName, forumId);
                        }
                    }

                    break;

                    // Active Topics
                case YafRssFeeds.Active:
                    if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ActiveTopicFeedAccess))
                    {
                        YafBuildLink.AccessDenied();
                    }

                    object categoryActiveId = null;
                    if (this.Get<HttpRequestBase>().QueryString.Exists("f")
                        &&
                        int.TryParse(
                            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"), out var categoryActiveIntId))
                    {
                        categoryActiveId = categoryActiveIntId;
                    }

                    this.GetActiveFeed(ref feed, feedType, atomFeedByVar, lastPostIcon, lastPostName, categoryActiveId);

                    break;
                case YafRssFeeds.Favorite:
                    if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().FavoriteTopicFeedAccess))
                    {
                        YafBuildLink.AccessDenied();
                    }

                    object categoryFavId = null;
                    if (this.Get<HttpRequestBase>().QueryString.Exists("f")
                        &&
                        int.TryParse(
                            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"), out var categoryFavIntId))
                    {
                        categoryFavId = categoryFavIntId;
                    }

                    this.GetFavoriteFeed(ref feed, feedType, atomFeedByVar, lastPostIcon, lastPostName, categoryFavId);
                    break;
                default:
                    YafBuildLink.AccessDenied();
                    break;
            }

            // update the feed with the item list... 
            // the list should be added after all other feed properties are set
            if (feed != null)
            {
                var writer = new XmlTextWriter(this.Get<HttpResponseBase>().OutputStream, Encoding.UTF8);
                writer.WriteStartDocument();

                // write the feed to the response writer);
                if (!atomFeedByVar)
                {
                    var rssFormatter = new Rss20FeedFormatter(feed);
                    rssFormatter.WriteTo(writer);
                    this.Get<HttpResponseBase>().ContentType = "application/rss+xml";
                }
                else
                {
                    var atomFormatter = new Atom10FeedFormatter(feed);
                    atomFormatter.WriteTo(writer);

                    this.Get<HttpResponseBase>().ContentType = "application/atom+xml";
                }

                writer.WriteEndDocument();
                writer.Close();

                this.Get<HttpResponseBase>().ContentEncoding = Encoding.UTF8;
                this.Get<HttpResponseBase>().Cache.SetCacheability(HttpCacheability.Public);

                this.Get<HttpResponseBase>().End();
            }
            else
            {
                YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
            }
        }

        /// <summary>
        /// Format the Url to an URN compatible string
        /// </summary>
        /// <param name="inputUrl">
        /// Input Url to format
        /// </param>
        /// <returns>
        /// Formatted url
        /// </returns>
        [NotNull]
        private static string FormatUrlForFeed([NotNull] string inputUrl)
        {
            var formatedUrl = inputUrl;

            if (formatedUrl.Contains(@"http://www."))
            {
                formatedUrl = formatedUrl.Replace("http://www.", string.Empty);
            }
            else if (formatedUrl.Contains(@"http://"))
            {
                formatedUrl = formatedUrl.Replace("http://", string.Empty);
            }

            formatedUrl = formatedUrl.Replace(".", "-").Replace("/", "-");

            if (formatedUrl.EndsWith("/"))
            {
                formatedUrl = formatedUrl.Remove(formatedUrl.Length - 1);
            }

            return formatedUrl;
        }

        /// <summary>
        /// The method to return latest topic content to display in a feed.
        /// </summary>
        /// <param name="link">A link to an active topic.</param>
        /// <param name="imgUrl">A latest topic icon Url.</param>
        /// <param name="imgAlt">A latest topic icon Alt text.</param>
        /// <param name="linkName">A latest topic displayed link name</param>
        /// <param name="text">An active topic first message content/partial content.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="altItem">The alt Item.</param>
        /// <returns>
        /// An Html formatted first message content string.
        /// </returns>
        private static string GetPostLatestContent(
            [NotNull] string link,
            [NotNull] string imgUrl,
            [NotNull] string imgAlt,
            [NotNull] string linkName,
            [NotNull] string text,
            int flags,
            bool altItem)
        {
            text = YafContext.Current.Get<IFormatMessage>().FormatSyndicationMessage(
                text, new MessageFlags(flags), altItem, 4000);

            return
                string
                    .Format(@"{0}<table><tr><td><a href=""{1}"" ><img src=""{2}"" alt =""{3}"" title =""{3}"" /></a></td></tr><table>", text, link, imgUrl, imgAlt, linkName);
        }

        /// <summary>
        /// The method creates YafSyndicationFeed for Active topics.
        /// </summary>
        /// <param name="feed">The YafSyndicationFeed.</param>
        /// <param name="feedType">The FeedType.</param>
        /// <param name="atomFeedByVar">The Atom feed checker.</param>
        /// <param name="lastPostIcon">The icon for last post link.</param>
        /// <param name="lastPostName">The last post name.</param>
        /// <param name="categoryActiveId">The category active id.</param>
        private void GetActiveFeed(
            [NotNull] ref YafSyndicationFeed feed,
            YafRssFeeds feedType,
            bool atomFeedByVar,
            [NotNull] string lastPostIcon,
            [NotNull] string lastPostName,
            [NotNull] object categoryActiveId)
        {
            var syndicationItems = new List<SyndicationItem>();
            var toActDate = DateTime.UtcNow;
            var toActText = this.GetText("MYTOPICS", "LAST_MONTH");

            if (this.Get<HttpRequestBase>().QueryString.Exists("txt"))
            {
                toActText =
                    this.Server.UrlDecode(
                        this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("txt")));
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("d"))
            {
                if (
                    !DateTime.TryParse(
                        this.Server.UrlDecode(
                            this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("d"))),
                        out toActDate))
                {
                    toActDate = Convert.ToDateTime(this.Get<IDateTime>().FormatDateTimeShort(DateTime.UtcNow))
                                + TimeSpan.FromDays(-31);
                    toActText = this.GetText("MYTOPICS", "LAST_MONTH");
                }
                else
                {
                    // To limit number of feeds items by timespan if we are getting an unreasonable time                
                    if (toActDate < DateTime.UtcNow + TimeSpan.FromDays(-31))
                    {
                        toActDate = DateTime.UtcNow + TimeSpan.FromDays(-31);
                        toActText = this.GetText("MYTOPICS", "LAST_MONTH");
                    }
                }
            }

            var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);
            var feedNameAlphaNum = new Regex(@"[^A-Za-z0-9]", RegexOptions.IgnoreCase).Replace(
                toActText, string.Empty);
            feed = new YafSyndicationFeed(
                $"{this.GetText("MYTOPICS", "ACTIVETOPICS")} - {toActText}",
                feedType,
                atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                urlAlphaNum);

            using (
                var dt =
                    this.TabledCleanUpByDate(
                        this.GetRepository<Topic>().ActiveAsDataTable(
                            this.PageContext.PageBoardID,
                            categoryActiveId.ToType<int>(),
                            this.PageContext.PageUserID,
                            toActDate,
                            DateTime.UtcNow,

                            // page index in db which is returned back  is +1 based!
                            0,

                            // set the page size here
                            20,
                            false,
                            this.Get<YafBoardSettings>().UseReadTrackingByDatabase),
                        "LastPosted",
                        toActDate))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (!row["TopicMovedID"].IsNullOrEmptyDBField())
                    {
                        continue;
                    }

                    var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty, row["UserID"].ToType<long>(), null, null));
                        feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;
                    }

                    feed.Contributors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(
                            string.Empty, row["LastUserID"].ToType<long>(), null, null));

                    var messageLink = YafBuildLink.GetLinkNotEscaped(
                        ForumPages.posts, true, "m={0}#post{0}", row["LastMessageID"]);
                    syndicationItems.AddSyndicationItem(
                        row["Subject"].ToString(),
                        GetPostLatestContent(
                            messageLink,
                            lastPostIcon,
                            lastPostName,
                            lastPostName,
                            string.Empty,
                            !row["LastMessageFlags"].IsNullOrEmptyDBField() ? row["LastMessageFlags"].ToType<int>() : 22,
                            false),
                        null,
                        messageLink,
                        $"urn:{urlAlphaNum}:ft{feedNameAlphaNum}:st{feedType}:span{(atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt())}:ltid{row["LinkTopicID"]}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                            .Unidecode(),
                        lastPosted,
                        feed);
                }

                feed.Items = syndicationItems;
            }
        }

        /// <summary>
        /// The method creates YafSyndicationFeed for Favorite topics.
        /// </summary>
        /// <param name="feed">The YafSyndicationFeed.</param>
        /// <param name="feedType">The FeedType.</param>
        /// <param name="atomFeedByVar">The Atom feed checker.</param>
        /// <param name="lastPostIcon">The icon for last post link.</param>
        /// <param name="lastPostName">The last post name.</param>
        /// <param name="categoryActiveId">The category active id.</param>
        private void GetFavoriteFeed(
            [NotNull] ref YafSyndicationFeed feed,
            YafRssFeeds feedType,
            bool atomFeedByVar,
            [NotNull] string lastPostIcon,
            [NotNull] string lastPostName,
            [NotNull] object categoryActiveId)
        {
            var syndicationItems = new List<SyndicationItem>();

            DateTime toFavDate;

            var toFavText = this.GetText("MYTOPICS", "LAST_MONTH");

            if (this.Get<HttpRequestBase>().QueryString.Exists("txt"))
            {
                toFavText =
                    this.Server.UrlDecode(
                        this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("txt")));
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("d"))
            {
                if (
                    !DateTime.TryParse(
                        this.Server.UrlDecode(
                            this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("d"))),
                        out toFavDate))
                {
                    toFavDate = PageContext.CurrentUserData.Joined ?? DateTimeHelper.SqlDbMinTime() + TimeSpan.FromDays(2);
                    toFavText = this.GetText("MYTOPICS", "SHOW_ALL");
                }
            }
            else
            {
                toFavDate = PageContext.CurrentUserData.Joined ?? DateTimeHelper.SqlDbMinTime() + TimeSpan.FromDays(2);
                toFavText = this.GetText("MYTOPICS", "SHOW_ALL");
            }

            using (
                var dt =
                    this.TabledCleanUpByDate(
                        this.GetRepository<FavoriteTopic>().Details(
                            categoryActiveId.ToType<int?>(),
                            this.PageContext.PageUserID,
                            toFavDate,
                            DateTime.UtcNow,

                            // page index in db is 1 based!
                            0,

                            // set the page size here
                            20,
                            false,
                            false),
                        "LastPosted",
                        toFavDate))
            {
                var urlAlphaNum = FormatUrlForFeed(YafForumInfo.ForumBaseUrl);
                var feedNameAlphaNum =
                    new Regex(@"[^A-Za-z0-9]", RegexOptions.IgnoreCase)
                        .Replace(toFavText, string.Empty);
                feed = new YafSyndicationFeed(
                    $"{this.GetText("MYTOPICS", "FAVORITETOPICS")} - {toFavText}",
                    feedType,
                    atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                    urlAlphaNum);

                foreach (DataRow row in dt.Rows)
                {
                    if (!row["TopicMovedID"].IsNullOrEmptyDBField())
                    {
                        continue;
                    }

                    var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty, row["UserID"].ToType<long>(), null, null));
                        feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;
                    }

                    feed.Contributors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(
                            string.Empty, row["LastUserID"].ToType<long>(), null, null));

                    syndicationItems.AddSyndicationItem(
                        row["Subject"].ToString(),
                        GetPostLatestContent(
                            YafBuildLink.GetLinkNotEscaped(
                                ForumPages.posts, true, "m={0}#post{0}", row["LastMessageID"]),
                            lastPostIcon,
                            lastPostName,
                            lastPostName,
                            string.Empty,
                            !row["LastMessageFlags"].IsNullOrEmptyDBField() ? row["LastMessageFlags"].ToType<int>() : 22,
                            false),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["LinkTopicID"]),
                        $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt())}:span{feedNameAlphaNum}:ltid{row["LinkTopicID"].ToType<int>()}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                            .Unidecode(),
                        lastPosted,
                        feed);
                }

                feed.Items = syndicationItems;
            }
        }

        /// <summary>
        /// The method creates YAF SyndicationFeed for forums in a category.
        /// </summary>
        /// <param name="feed">
        /// The YAF Syndication Feed.
        /// </param>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        private void GetForumFeed(
            [NotNull] ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar, [NotNull] int? categoryId)
        {
            var syndicationItems = new List<SyndicationItem>();

            using (var dt = this.GetRepository<Forum>().ListReadAsDataTable(this.PageContext.PageBoardID, this.PageContext.PageUserID, categoryId, null, false, false))
            {
                var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed = new YafSyndicationFeed(
                    this.GetText("DEFAULT", "FORUM"),
                    feedType,
                    atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                    urlAlphaNum);

                foreach (DataRow row in dt.Rows)
                {
                    if (row["TopicMovedID"].IsNullOrEmptyDBField() && row["LastPosted"].IsNullOrEmptyDBField())
                    {
                        continue;
                    }

                    var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        if (row["LastUserID"].IsNullOrEmptyDBField() || row["LastUserID"].IsNullOrEmptyDBField())
                        {
                            break;
                        }

                        feed.Authors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty, row["LastUserID"].ToType<long>(), null, null));

                        feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;

                        // Alternate Link
                        // feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.topics, true))));
                    }

                    if (!row["LastUserID"].IsNullOrEmptyDBField())
                    {
                        feed.Contributors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty, row["LastUserID"].ToType<long>(), null, null));
                    }

                    syndicationItems.AddSyndicationItem(
                        row["Forum"].ToString(),
                        this.HtmlEncode(row["Description"].ToString()),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.topics, true, "f={0}", row["ForumID"]),
                        $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt())}:fid{row["ForumID"]}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                            .Unidecode(),
                        lastPosted,
                        feed);
                }

                feed.Items = syndicationItems;
            }
        }

        /// <summary>
        /// The method creates YafSyndicationFeed for topic announcements.
        /// </summary>
        /// <param name="feed">
        /// The YafSyndicationFeed.
        /// </param>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        private void GetLatestAnnouncementsFeed(
            [NotNull] ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar)
        {
            var syndicationItems = new List<SyndicationItem>();

            var dt = this.GetRepository<Topic>().AnnouncementsAsDataTable(this.PageContext.PageBoardID, 10, this.PageContext.PageUserID);
            var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

            feed = new YafSyndicationFeed(
                this.GetText("POSTMESSAGE", "ANNOUNCEMENT"),
                feedType,
                atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                urlAlphaNum);

            foreach (DataRow row in dt.Rows)
            {
                // don't render moved topics
                if (!row["TopicMovedID"].IsNullOrEmptyDBField())
                {
                    continue;
                }

                var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

                if (syndicationItems.Count <= 0)
                {
                    feed.Authors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(
                            string.Empty, row["UserID"].ToType<long>(), null, null));
                    feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;
                }

                feed.Contributors.Add(
                    SyndicationItemExtensions.NewSyndicationPerson(
                        string.Empty, row["LastUserID"].ToType<long>(), null, null));

                syndicationItems.AddSyndicationItem(
                    row["Topic"].ToString(),
                    row["Message"].ToString(),
                    null,
                    YafBuildLink.GetLinkNotEscaped(
                        ForumPages.posts, true, "t={0}", this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t")),
                    $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt())}:tid{this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t")}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                        .Unidecode(),
                    lastPosted,
                    feed);
            }

            feed.Items = syndicationItems;
        }

        /// <summary>
        /// The helper function gets media enclosure links for a post
        /// </summary>
        /// <param name="messageId">
        /// The MessageId with attached files.
        /// </param>
        /// <returns>
        /// The get media links.
        /// </returns>
        [NotNull]
        private List<SyndicationLink> GetMediaLinks(int messageId)
        {
            var attachementLinks = new List<SyndicationLink>();
            var attachments = this.GetRepository<Attachment>().Get(a => a.MessageID == messageId);

            if (attachments.Any())
            {
                attachementLinks.AddRange(
                    from Attachment attachment in attachments
                    where attachment.FileName.IsSet()
                    select
                        new SyndicationLink(
                        new Uri(
                            $"{YafForumInfo.ForumBaseUrl}{YafForumInfo.ForumClientFileRoot.TrimStart('/')}resource.ashx?a={attachment.ID}&b={this.PageContext.PageBoardID}"),
                        "enclosure",
                        attachment.FileName,
                        attachment.ContentType,
                        attachment.Bytes.ToType<long>()));
            }


            return attachementLinks;
        }

        /// <summary>
        /// The method creates YafSyndicationFeed for topics in a forum.
        /// </summary>
        /// <param name="feed">
        /// The YafSyndicationFeed.
        /// </param>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="lastPostIcon">
        /// The icon for last post link.
        /// </param>
        /// <param name="lastPostName">
        /// The last post name.
        /// </param>
        private void GetPostLatestFeed(
            [NotNull] ref YafSyndicationFeed feed,
            YafRssFeeds feedType,
            bool atomFeedByVar,
            [NotNull] string lastPostIcon,
            [NotNull] string lastPostName)
        {
            var syndicationItems = new List<SyndicationItem>();

            using (
                var dataTopics = this.GetRepository<Topic>().RssLatestAsDataTable(
                    this.PageContext.PageBoardID,
                    this.Get<YafBoardSettings>().ActiveDiscussionsCount <= 50
                        ? this.Get<YafBoardSettings>().ActiveDiscussionsCount
                        : 50,
                    this.PageContext.PageUserID,
                    this.Get<YafBoardSettings>().UseStyledNicks,
                    this.Get<YafBoardSettings>().NoCountForumsInActiveDiscussions))
            {
                var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed = new YafSyndicationFeed(
                    this.GetText("ACTIVE_DISCUSSIONS"),
                    feedType,
                    atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                    urlAlphaNum);
                var altItem = false;
                foreach (DataRow row in dataTopics.Rows)
                {
                    // don't render moved topics
                    if (row["TopicMovedID"].IsNullOrEmptyDBField())
                    {
                        var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;
                        if (syndicationItems.Count <= 0)
                        {
                            feed.LastUpdatedTime = lastPosted + this.Get<IDateTime>().TimeOffset;
                            feed.Authors.Add(
                                SyndicationItemExtensions.NewSyndicationPerson(
                                    string.Empty,
                                    row["UserID"].ToType<long>(),
                                    row["UserName"].ToString(),
                                    row["UserDisplayName"].ToString()));
                        }

                        feed.Contributors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty,
                                row["LastUserID"].ToType<long>(),
                                row["LastUserName"].ToString(),
                                row["LastUserDisplayName"].ToString()));

                        var messageLink = YafBuildLink.GetLinkNotEscaped(
                            ForumPages.posts, true, "m={0}#post{0}", row["LastMessageID"]);

                        syndicationItems.AddSyndicationItem(
                            row["Topic"].ToString(),
                            GetPostLatestContent(
                                messageLink,
                                lastPostIcon,
                                lastPostName,
                                lastPostName,
                                row["LastMessage"].ToString(),
                                !row["LastMessageFlags"].IsNullOrEmptyDBField()
                                    ? row["LastMessageFlags"].ToType<int>()
                                    : 22,
                                altItem),
                            null,
                            YafBuildLink.GetLinkNotEscaped(
                                ForumPages.posts, true, "t={0}", row["TopicID"].ToType<int>()),
                            $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt())}:tid{row["TopicID"]}:mid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                                .Unidecode(),
                            lastPosted,
                            feed);
                    }

                    altItem = !altItem;
                }

                feed.Items = syndicationItems;
            }
        }

        /// <summary>
        /// The method creates YafSyndicationFeed for posts.
        /// </summary>
        /// <param name="feed">
        /// The YafSyndicationFeed.
        /// </param>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="topicId">
        /// The TopicID
        /// </param>
        private void GetPostsFeed(
            [NotNull] ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar, int topicId)
        {
            var syndicationItems = new List<SyndicationItem>();

            var showDeleted = false;
            var userId = 0;

            if (this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
            {
                showDeleted = true;
            }

            if (!showDeleted
                &&
                (this.Get<YafBoardSettings>().ShowDeletedMessages
                 && !this.Get<YafBoardSettings>().ShowDeletedMessagesToAll || this.PageContext.IsAdmin
                 || this.PageContext.IsForumModerator))
            {
                userId = this.PageContext.PageUserID;
            }

            using (
                var dt = this.GetRepository<Message>().PostListAsDataTable(
                    topicId,
                    this.PageContext.PageUserID,
                    userId,
                    0,
                    showDeleted,
                    true,
                    false,
                    DateTimeHelper.SqlDbMinTime(),
                    DateTime.UtcNow,
                    DateTimeHelper.SqlDbMinTime(),
                    DateTime.UtcNow,
                    0,
                    this.Get<YafBoardSettings>().PostsPerPage,
                    2,
                    0,
                    0,
                    false,
                    -1))
            {
                // convert to linq...
                var rowList = dt.AsEnumerable();

                // last page posts
                var dataRows = rowList.Take(this.Get<YafBoardSettings>().PostsPerPage);

                var altItem = false;

                var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed =
                    new YafSyndicationFeed(
                        $"{this.GetText("PROFILE", "TOPIC")}{this.PageContext.PageTopicName} - {this.Get<YafBoardSettings>().PostsPerPage}",
                        feedType,
                        atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                        urlAlphaNum);

                foreach (var row in dataRows)
                {
                    var posted = Convert.ToDateTime(row["Edited"]) + this.Get<IDateTime>().TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty, row["UserID"].ToType<long>(), null, null));
                        feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;
                    }

                    List<SyndicationLink> attachementLinks = null;

                    // if the user doesn't have download access we simply don't show enclosure links.
                    if (this.PageContext.ForumDownloadAccess)
                    {
                        attachementLinks = this.GetMediaLinks(row["MessageID"].ToType<int>());
                    }

                    feed.Contributors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(
                            string.Empty, row["UserID"].ToType<long>(), null, null));

                    syndicationItems.AddSyndicationItem(
                        row["Topic"].ToString(),
                        this.Get<IFormatMessage>().FormatSyndicationMessage(
                            row["Message"].ToString(), new MessageFlags(row["Flags"]), altItem, 4000),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}&find=lastpost", row["MessageID"]),
                        $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt())}:meid{row["MessageID"]}:{this.PageContext.PageBoardID}"
                            .Unidecode(),
                        posted,
                        feed,
                        attachementLinks);

                    // used to format feeds
                    altItem = !altItem;
                }

                feed.Items = syndicationItems;
            }
        }

        /// <summary>
        /// The method creates YAF SyndicationFeed for topics in a forum.
        /// </summary>
        /// <param name="feed">
        /// The YAF SyndicationFeed.
        /// </param>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="lastPostIcon">
        /// The icon for last post link.
        /// </param>
        /// <param name="lastPostName">
        /// The last post name.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        private void GetTopicsFeed(
            [NotNull] ref YafSyndicationFeed feed,
            YafRssFeeds feedType,
            bool atomFeedByVar,
            [NotNull] string lastPostIcon,
            [NotNull] string lastPostName,
            int forumId)
        {
            var syndicationItems = new List<SyndicationItem>();

            // vzrus changed to separate DLL specific code
            using (var dt = this.GetRepository<Topic>().RssListAsDataTable(forumId, this.Get<YafBoardSettings>().TopicsFeedItemsCount))
            {
                var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed = new YafSyndicationFeed(
                    $"{this.GetText("DEFAULT", "FORUM")}:{this.PageContext.PageForumName}",
                    feedType,
                    atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                    urlAlphaNum);

                foreach (DataRow row in dt.Rows)
                {
                    var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty, row["LastUserID"].ToType<long>(), null, null));
                        feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;

                        // Alternate Link
                        // feed.Links.Add(new SyndicationLink(new Uri(YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true))));
                    }

                    feed.Contributors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(
                            string.Empty, row["LastUserID"].ToType<long>(), null, null));

                    syndicationItems.AddSyndicationItem(
                        row["Topic"].ToString(),
                        GetPostLatestContent(
                            YafBuildLink.GetLinkNotEscaped(
                                ForumPages.posts, true, "m={0}#post{0}", row["LastMessageID"]),
                            lastPostIcon,
                            lastPostName,
                            lastPostName,
                            row["LastMessage"].ToString(),
                            !row["LastMessageFlags"].IsNullOrEmptyDBField() ? row["LastMessageFlags"].ToType<int>() : 22,
                            false),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["TopicID"]),
                        $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt())}:tid{row["TopicID"]}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                            .Unidecode(),
                        lastPosted,
                        feed);
                }

                feed.Items = syndicationItems;
            }
        }

        /// <summary>
        /// Tableds the clean up by date.
        /// TODO: general clean-up after complex subquiries paging - a repeating code
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <param name="fieldName">
        /// Name of the field.
        /// </param>
        /// <param name="cleanToDate">
        /// The clean to date.
        /// </param>
        /// <returns>
        /// The tabled clean up by date.
        /// </returns>
        private DataTable TabledCleanUpByDate(DataTable dt, string fieldName, DateTime cleanToDate)
        {
            if (dt == null || !dt.HasRows())
            {
                return dt;
            }

            var topicsNew = dt.Copy();

            topicsNew.Rows.Cast<DataRow>()
                .Where(
                    thisTableRow => thisTableRow[fieldName] != DBNull.Value
                                    && thisTableRow[fieldName].ToType<DateTime>() <= cleanToDate)
                .ForEach(thisTableRow => thisTableRow.Delete());

            // styled nicks
            topicsNew.AcceptChanges();
            return topicsNew;
        }

        #endregion
    }
}