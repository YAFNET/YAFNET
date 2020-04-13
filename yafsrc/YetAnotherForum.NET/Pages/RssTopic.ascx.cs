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
    using YAF.Core.BasePages;
    using YAF.Core.Context;
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
    public partial class RssTopic : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RssTopic" /> class.
        /// </summary>
        public RssTopic()
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
            if (!(this.Get<BoardSettings>().ShowRSSLink || this.Get<BoardSettings>().ShowAtomLink))
            {
                BuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
            }

            // Atom feed as variable
            var atomFeedByVar = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("type")
                                == SyndicationFormats.Atom.ToInt().ToString();

            YafSyndicationFeed feed = null;

            // var syndicationItems = new List<SyndicationItem>();
            var lastPostName = this.GetText("DEFAULT", "GO_LAST_POST");

            RssFeeds feedType;

            try
            {
                feedType = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("feed").ToEnum<RssFeeds>(true);
            }
            catch
            {
                // default to Forum Feed.
                feedType = RssFeeds.Forum;
            }

            switch (feedType)
            {
                // Latest posts feed
                case RssFeeds.LatestPosts:
                    if (!(this.Get<BoardSettings>().ShowActiveDiscussions && this.Get<IPermissions>()
                              .Check(this.Get<BoardSettings>().PostLatestFeedAccess)))
                    {
                        BuildLink.AccessDenied();
                    }

                    feed = this.GetPostLatestFeed(feedType, atomFeedByVar, lastPostName);
                    break;

                // Latest Announcements feed
                case RssFeeds.LatestAnnouncements:
                    if (!this.Get<IPermissions>().Check(this.Get<BoardSettings>().ForumFeedAccess))
                    {
                        BuildLink.AccessDenied();
                    }

                    feed = this.GetLatestAnnouncementsFeed(feedType, atomFeedByVar);
                    break;

                // Posts Feed
                case RssFeeds.Posts:
                    if (!(this.PageContext.ForumReadAccess
                          && this.Get<IPermissions>().Check(this.Get<BoardSettings>().PostsFeedAccess)))
                    {
                        BuildLink.AccessDenied();
                    }

                    if (this.Get<HttpRequestBase>().QueryString.Exists("t"))
                    {
                        var topicId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("t");

                        feed = this.GetPostsFeed(feedType, atomFeedByVar, topicId.Value);
                    }

                    break;

                // Forum Feed
                case RssFeeds.Forum:
                    if (!this.Get<IPermissions>().Check(this.Get<BoardSettings>().ForumFeedAccess))
                    {
                        BuildLink.AccessDenied();
                    }

                    int? categoryId = null;

                    if (this.Get<HttpRequestBase>().QueryString.Exists("c"))
                    {
                        categoryId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("c");
                    }

                    feed = this.GetForumFeed(feedType, atomFeedByVar, categoryId);
                    break;

                // Topics Feed
                case RssFeeds.Topics:
                    if (!(this.PageContext.ForumReadAccess
                          && this.Get<IPermissions>().Check(this.Get<BoardSettings>().TopicsFeedAccess)))
                    {
                        BuildLink.AccessDenied();
                    }

                    if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
                    {
                        var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("f");

                        feed = this.GetTopicsFeed(feedType, atomFeedByVar, lastPostName, forumId.Value);
                    }

                    break;

                // Active Topics
                case RssFeeds.Active:
                    if (!this.Get<IPermissions>().Check(this.Get<BoardSettings>().ActiveTopicFeedAccess))
                    {
                        BuildLink.AccessDenied();
                    }

                    int? categoryActiveId = null;

                    if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
                    {
                        categoryActiveId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("f");
                    }

                    feed = this.GetActiveFeed(feedType, atomFeedByVar, lastPostName, categoryActiveId);

                    break;
                case RssFeeds.Favorite:
                    if (!this.Get<IPermissions>().Check(this.Get<BoardSettings>().FavoriteTopicFeedAccess))
                    {
                        BuildLink.AccessDenied();
                    }

                    int? categoryFavId = null;

                    if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
                    {
                        categoryFavId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("f");
                    }

                    feed = this.GetFavoriteFeed(feedType, atomFeedByVar, lastPostName, categoryFavId);
                    break;
                default:
                    BuildLink.AccessDenied();
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
                BuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
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
            var formattedUrl = inputUrl;

            if (formattedUrl.Contains(@"http://www."))
            {
                formattedUrl = formattedUrl.Replace("http://www.", string.Empty);
            }
            else if (formattedUrl.Contains(@"http://"))
            {
                formattedUrl = formattedUrl.Replace("http://", string.Empty);
            }

            formattedUrl = formattedUrl.Replace(".", "-").Replace("/", "-");

            if (formattedUrl.EndsWith("/"))
            {
                formattedUrl = formattedUrl.Remove(formattedUrl.Length - 1);
            }

            return formattedUrl;
        }

        /// <summary>
        /// The method to return latest topic content to display in a feed.
        /// </summary>
        /// <param name="link">A link to an active topic.</param>
        /// <param name="linkName">A latest topic displayed link name</param>
        /// <param name="text">An active topic first message content/partial content.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="altItem">The alt Item.</param>
        /// <returns>
        /// An Html formatted first message content string.
        /// </returns>
        private static string GetPostLatestContent(
            [NotNull] string link,
            [NotNull] string linkName,
            [NotNull] string text,
            int flags,
            bool altItem)
        {
            text = BoardContext.Current.Get<IFormatMessage>().FormatSyndicationMessage(
                text,
                new MessageFlags(flags),
                altItem,
                4000);

            return $@"{text}<a href=""{link}"" >{linkName}</a>";
        }

        /// <summary>
        /// The method creates SyndicationFeed for Active topics.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="lastPostName">
        /// The last post name.
        /// </param>
        /// <param name="categoryActiveId">
        /// The category active id.
        /// </param>
        /// <returns>
        /// The <see cref="YafSyndicationFeed"/>.
        /// </returns>
        private YafSyndicationFeed GetActiveFeed(
            RssFeeds feedType,
            bool atomFeedByVar,
            [NotNull] string lastPostName,
            [NotNull] int? categoryActiveId)
        {
            var syndicationItems = new List<SyndicationItem>();
            var toActDate = DateTime.UtcNow;
            var toActText = this.GetText("MYTOPICS", "LAST_MONTH");

            if (this.Get<HttpRequestBase>().QueryString.Exists("txt"))
            {
                toActText = this.Server.UrlDecode(
                    this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("txt")));
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("d"))
            {
                if (!DateTime.TryParse(
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
            var feedNameAlphaNum = new Regex(@"[^A-Za-z0-9]", RegexOptions.IgnoreCase).Replace(toActText, string.Empty);
            var feed = new YafSyndicationFeed(
                $"{this.GetText("MYTOPICS", "ACTIVETOPICS")} - {toActText}",
                feedType,
                atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt(),
                urlAlphaNum);

            using (var dt = TabledCleanUpByDate(
                this.GetRepository<Topic>().ActiveAsDataTable(
                    this.PageContext.PageBoardID,
                    categoryActiveId,
                    this.PageContext.PageUserID,
                    toActDate,
                    DateTime.UtcNow,
                    0, // page index in db which is returned back  is +1 based!
                    20, // set the page size here
                    false,
                    this.Get<BoardSettings>().UseReadTrackingByDatabase),
                "LastPosted",
                toActDate))
            {
                dt.Rows.Cast<DataRow>().Where(row => !row["TopicMovedID"].IsNullOrEmptyDBField()).ForEach(
                    row =>
                        {
                            var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

                            if (syndicationItems.Count <= 0)
                            {
                                feed.Authors.Add(
                                    SyndicationItemExtensions.NewSyndicationPerson(
                                        string.Empty,
                                        row["UserID"].ToType<long>(),
                                        null,
                                        null));
                                feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;
                            }

                            feed.Contributors.Add(
                                SyndicationItemExtensions.NewSyndicationPerson(
                                    string.Empty,
                                    row["LastUserID"].ToType<long>(),
                                    null,
                                    null));

                            var messageLink = BuildLink.GetLinkNotEscaped(
                                ForumPages.Posts,
                                true,
                                "m={0}#post{0}",
                                row["LastMessageID"]);
                            syndicationItems.AddSyndicationItem(
                                row["Subject"].ToString(),
                                GetPostLatestContent(
                                    messageLink,
                                    lastPostName,
                                    lastPostName,
                                    !row["LastMessageFlags"].IsNullOrEmptyDBField()
                                        ? row["LastMessageFlags"].ToType<int>()
                                        : 22,
                                    false),
                                null,
                                messageLink,
                                $"urn:{urlAlphaNum}:ft{feedNameAlphaNum}:st{feedType}:span{(atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt())}:ltid{row["LinkTopicID"]}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                                    .Unidecode(),
                                lastPosted,
                                feed);
                        });

                feed.Items = syndicationItems;
            }

            return feed;
        }

        /// <summary>
        /// method the SyndicationFeed for Favorite topics.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="lastPostName">
        /// The last post name.
        /// </param>
        /// <param name="categoryActiveId">
        /// The category active id.
        /// </param>
        /// <returns>
        /// The <see cref="YafSyndicationFeed"/>.
        /// </returns>
        private YafSyndicationFeed GetFavoriteFeed(
            RssFeeds feedType,
            bool atomFeedByVar,
            [NotNull] string lastPostName,
            [NotNull] object categoryActiveId)
        {
            YafSyndicationFeed feed;
            var syndicationItems = new List<SyndicationItem>();

            DateTime toFavDate;

            var toFavText = this.GetText("MYTOPICS", "LAST_MONTH");

            if (this.Get<HttpRequestBase>().QueryString.Exists("txt"))
            {
                toFavText = this.Server.UrlDecode(
                    this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("txt")));
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("d"))
            {
                if (!DateTime.TryParse(
                        this.Server.UrlDecode(
                            this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("d"))),
                        out toFavDate))
                {
                    toFavDate = PageContext.CurrentUserData.Joined
                                ?? DateTimeHelper.SqlDbMinTime() + TimeSpan.FromDays(2);
                    toFavText = this.GetText("MYTOPICS", "SHOW_ALL");
                }
            }
            else
            {
                toFavDate = PageContext.CurrentUserData.Joined ?? DateTimeHelper.SqlDbMinTime() + TimeSpan.FromDays(2);
                toFavText = this.GetText("MYTOPICS", "SHOW_ALL");
            }

            using (var dt = TabledCleanUpByDate(
                this.GetRepository<FavoriteTopic>().Details(
                    categoryActiveId.ToType<int?>(),
                    this.PageContext.PageUserID,
                    toFavDate,
                    DateTime.UtcNow,
                    0, // page index in db is 1 based!
                    20, // set the page size here
                    false,
                    false),
                "LastPosted",
                toFavDate))
            {
                var urlAlphaNum = FormatUrlForFeed(BoardInfo.ForumBaseUrl);
                var feedNameAlphaNum =
                    new Regex(@"[^A-Za-z0-9]", RegexOptions.IgnoreCase).Replace(toFavText, string.Empty);

                feed = new YafSyndicationFeed(
                    $"{this.GetText("MYTOPICS", "FAVORITETOPICS")} - {toFavText}",
                    feedType,
                    atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt(),
                    urlAlphaNum);

                dt.Rows.Cast<DataRow>().Where(row => !row["TopicMovedID"].IsNullOrEmptyDBField()).ForEach(
                    row =>
                        {
                            var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

                            if (syndicationItems.Count <= 0)
                            {
                                feed.Authors.Add(
                                    SyndicationItemExtensions.NewSyndicationPerson(
                                        string.Empty,
                                        row["UserID"].ToType<long>(),
                                        null,
                                        null));
                                feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;
                            }

                            feed.Contributors.Add(
                                SyndicationItemExtensions.NewSyndicationPerson(
                                    string.Empty,
                                    row["LastUserID"].ToType<long>(),
                                    null,
                                    null));

                            syndicationItems.AddSyndicationItem(
                                row["Subject"].ToString(),
                                GetPostLatestContent(
                                    BuildLink.GetLinkNotEscaped(
                                        ForumPages.Posts,
                                        true,
                                        "m={0}#post{0}",
                                        row["LastMessageID"]),
                                    lastPostName,
                                    lastPostName,
                                    !row["LastMessageFlags"].IsNullOrEmptyDBField()
                                        ? row["LastMessageFlags"].ToType<int>()
                                        : 22,
                                    false),
                                null,
                                BuildLink.GetLinkNotEscaped(ForumPages.Posts, true, "t={0}", row["LinkTopicID"]),
                                $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt())}:span{feedNameAlphaNum}:ltid{row["LinkTopicID"].ToType<int>()}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                                    .Unidecode(),
                                lastPosted,
                                feed);
                        });

                feed.Items = syndicationItems;
            }

            return feed;
        }

        /// <summary>
        /// The method creates YAF SyndicationFeed for forums in a category.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <returns>
        /// The <see cref="YafSyndicationFeed"/>.
        /// </returns>
        private YafSyndicationFeed GetForumFeed(RssFeeds feedType, bool atomFeedByVar, [NotNull] int? categoryId)
        {
            YafSyndicationFeed feed;

            var syndicationItems = new List<SyndicationItem>();

            using (var dt = this.GetRepository<Forum>().ListReadAsDataTable(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                categoryId,
                null,
                false,
                false))
            {
                var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed = new YafSyndicationFeed(
                    this.GetText("DEFAULT", "FORUM"),
                    feedType,
                    atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt(),
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
                                string.Empty,
                                row["LastUserID"].ToType<long>(),
                                null,
                                null));

                        feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;

                        // Alternate Link
                        // feed.Links.Add(new SyndicationLink(new Uri(BuildLink.GetLinkNotEscaped(ForumPages.topics, true))));
                    }

                    if (!row["LastUserID"].IsNullOrEmptyDBField())
                    {
                        feed.Contributors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty,
                                row["LastUserID"].ToType<long>(),
                                null,
                                null));
                    }

                    syndicationItems.AddSyndicationItem(
                        row["Forum"].ToString(),
                        this.HtmlEncode(row["Description"].ToString()),
                        null,
                        BuildLink.GetLinkNotEscaped(ForumPages.topics, true, "f={0}", row["ForumID"]),
                        $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt())}:fid{row["ForumID"]}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                            .Unidecode(),
                        lastPosted,
                        feed);
                }

                feed.Items = syndicationItems;
            }

            return feed;
        }

        /// <summary>
        /// The method creates SyndicationFeed for topic announcements.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <returns>
        /// The <see cref="YafSyndicationFeed"/>.
        /// </returns>
        private YafSyndicationFeed GetLatestAnnouncementsFeed(RssFeeds feedType, bool atomFeedByVar)
        {
            var syndicationItems = new List<SyndicationItem>();

            var dt = this.GetRepository<Topic>().AnnouncementsAsDataTable(
                this.PageContext.PageBoardID,
                10,
                this.PageContext.PageUserID);
            var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

            var feed = new YafSyndicationFeed(
                this.GetText("POSTMESSAGE", "ANNOUNCEMENT"),
                feedType,
                atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt(),
                urlAlphaNum);

            dt.Rows.Cast<DataRow>().Where(row => !row["TopicMovedID"].IsNullOrEmptyDBField()).ForEach(
                row =>
                    {
                        var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

                        if (syndicationItems.Count <= 0)
                        {
                            feed.Authors.Add(
                                SyndicationItemExtensions.NewSyndicationPerson(
                                    string.Empty,
                                    row["UserID"].ToType<long>(),
                                    null,
                                    null));
                            feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;
                        }

                        feed.Contributors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty,
                                row["LastUserID"].ToType<long>(),
                                null,
                                null));

                        syndicationItems.AddSyndicationItem(
                            row["Topic"].ToString(),
                            row["Message"].ToString(),
                            null,
                            BuildLink.GetLinkNotEscaped(
                                ForumPages.Posts,
                                true,
                                "t={0}",
                                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t")),
                            $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt())}:tid{this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t")}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                                .Unidecode(),
                            lastPosted,
                            feed);
                    });

            feed.Items = syndicationItems;

            return feed;
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
            var attachmentLinks = new List<SyndicationLink>();
            var attachments = this.GetRepository<Attachment>().Get(a => a.MessageID == messageId);

            if (attachments.Any())
            {
                attachmentLinks.AddRange(
                    from Attachment attachment in attachments
                    where attachment.FileName.IsSet()
                    select new SyndicationLink(
                        new Uri(
                            $"{BoardInfo.ForumBaseUrl}{BoardInfo.ForumClientFileRoot.TrimStart('/')}resource.ashx?a={attachment.ID}&b={this.PageContext.PageBoardID}"),
                        "enclosure",
                        attachment.FileName,
                        attachment.ContentType,
                        attachment.Bytes.ToType<long>()));
            }

            return attachmentLinks;
        }

        /// <summary>
        /// The method creates Syndication Feed for topics in a forum.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="lastPostName">
        /// The last post name.
        /// </param>
        /// <returns>
        /// The <see cref="YafSyndicationFeed"/>.
        /// </returns>
        private YafSyndicationFeed GetPostLatestFeed(
            RssFeeds feedType,
            bool atomFeedByVar,
            [NotNull] string lastPostName)
        {
            var syndicationItems = new List<SyndicationItem>();

            var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

            var feed = new YafSyndicationFeed(
                this.GetText("ACTIVE_DISCUSSIONS"),
                feedType,
                atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt(),
                urlAlphaNum);

            using (var dataTopics = this.GetRepository<Topic>().RssLatestAsDataTable(
                this.PageContext.PageBoardID,
                this.Get<BoardSettings>().ActiveDiscussionsCount <= 50
                    ? this.Get<BoardSettings>().ActiveDiscussionsCount
                    : 50,
                this.PageContext.PageUserID,
                this.Get<BoardSettings>().UseStyledNicks,
                this.Get<BoardSettings>().NoCountForumsInActiveDiscussions))
            {
                var altItem = false;

                dataTopics.Rows.Cast<DataRow>().Where(row => !row["TopicMovedID"].IsNullOrEmptyDBField()).ForEach(
                    row =>
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

                            var messageLink = BuildLink.GetLinkNotEscaped(
                                ForumPages.Posts,
                                true,
                                "m={0}#post{0}",
                                row["LastMessageID"]);

                            syndicationItems.AddSyndicationItem(
                                row["Topic"].ToString(),
                                GetPostLatestContent(
                                    messageLink,
                                    lastPostName,
                                    row["LastMessage"].ToString(),
                                    !row["LastMessageFlags"].IsNullOrEmptyDBField()
                                        ? row["LastMessageFlags"].ToType<int>()
                                        : 22,
                                    altItem),
                                null,
                                BuildLink.GetLinkNotEscaped(
                                    ForumPages.Posts,
                                    true,
                                    "t={0}",
                                    row["TopicID"].ToType<int>()),
                                $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt())}:tid{row["TopicID"]}:mid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                                    .Unidecode(),
                                lastPosted,
                                feed);

                            altItem = !altItem;

                            feed.Items = syndicationItems;
                        });

                return feed;
            }
        }

        /// <summary>
        /// The method creates the SyndicationFeed for posts.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="topicId">
        /// The TopicID
        /// </param>
        /// <returns>
        /// The <see cref="YafSyndicationFeed"/>.
        /// </returns>
        private YafSyndicationFeed GetPostsFeed(RssFeeds feedType, bool atomFeedByVar, int topicId)
        {
            YafSyndicationFeed feed;

            var syndicationItems = new List<SyndicationItem>();

            var showDeleted = false;
            var userId = 0;

            if (this.Get<BoardSettings>().ShowDeletedMessagesToAll)
            {
                showDeleted = true;
            }

            if (!showDeleted
                && (this.Get<BoardSettings>().ShowDeletedMessages && !this.Get<BoardSettings>().ShowDeletedMessagesToAll
                    || this.PageContext.IsAdmin || this.PageContext.IsForumModerator))
            {
                userId = this.PageContext.PageUserID;
            }

            using (var dt = this.GetRepository<Message>().PostListAsDataTable(
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
                this.Get<BoardSettings>().PostsPerPage,
                2,
                0,
                0,
                false,
                -1))
            {
                // convert to linq...
                var rowList = dt.AsEnumerable();

                // last page posts
                var dataRows = rowList.Take(this.Get<BoardSettings>().PostsPerPage);

                var altItem = false;

                var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed = new YafSyndicationFeed(
                    $"{this.GetText("PROFILE", "TOPIC")}{this.PageContext.PageTopicName} - {this.Get<BoardSettings>().PostsPerPage}",
                    feedType,
                    atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt(),
                    urlAlphaNum);

                dataRows.ForEach(
                    row =>
                        {
                            var posted = Convert.ToDateTime(row["Edited"]) + this.Get<IDateTime>().TimeOffset;

                            if (syndicationItems.Count <= 0)
                            {
                                feed.Authors.Add(
                                    SyndicationItemExtensions.NewSyndicationPerson(
                                        string.Empty,
                                        row["UserID"].ToType<long>(),
                                        null,
                                        null));
                                feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;
                            }

                            List<SyndicationLink> attachmentLinks = null;

                            // if the user doesn't have download access we simply don't show enclosure links.
                            if (this.PageContext.ForumDownloadAccess)
                            {
                                attachmentLinks = this.GetMediaLinks(row["MessageID"].ToType<int>());
                            }

                            feed.Contributors.Add(
                                SyndicationItemExtensions.NewSyndicationPerson(
                                    string.Empty,
                                    row["UserID"].ToType<long>(),
                                    null,
                                    null));

                            syndicationItems.AddSyndicationItem(
                                row["Topic"].ToString(),
                                this.Get<IFormatMessage>().FormatSyndicationMessage(
                                    row["Message"].ToString(),
                                    new MessageFlags(row["Flags"]),
                                    altItem,
                                    4000),
                                null,
                                BuildLink.GetLinkNotEscaped(
                                    ForumPages.Posts,
                                    true,
                                    "m={0}&find=lastpost",
                                    row["MessageID"]),
                                $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt())}:meid{row["MessageID"]}:{this.PageContext.PageBoardID}"
                                    .Unidecode(),
                                posted,
                                feed,
                                attachmentLinks);

                            // used to format feeds
                            altItem = !altItem;
                        });

                feed.Items = syndicationItems;
            }

            return feed;
        }

        /// <summary>
        /// The method creates SyndicationFeed for topics in a forum.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="atomFeedByVar">
        /// The Atom feed checker.
        /// </param>
        /// <param name="lastPostName">
        /// The last post name.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <returns>
        /// The <see cref="YafSyndicationFeed"/>.
        /// </returns>
        private YafSyndicationFeed GetTopicsFeed(
            RssFeeds feedType,
            bool atomFeedByVar,
            [NotNull] string lastPostName,
            int forumId)
        {
            YafSyndicationFeed feed;
            var syndicationItems = new List<SyndicationItem>();

            // vzrus changed to separate DLL specific code
            using (var dt = this.GetRepository<Topic>().RssListAsDataTable(
                forumId,
                this.Get<BoardSettings>().TopicsFeedItemsCount))
            {
                var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed = new YafSyndicationFeed(
                    $"{this.GetText("DEFAULT", "FORUM")}:{this.PageContext.PageForumName}",
                    feedType,
                    atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt(),
                    urlAlphaNum);

                dt.Rows.Cast<DataRow>().ForEach(
                    row =>
                        {
                            var lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

                            if (syndicationItems.Count <= 0)
                            {
                                feed.Authors.Add(
                                    SyndicationItemExtensions.NewSyndicationPerson(
                                        string.Empty,
                                        row["LastUserID"].ToType<long>(),
                                        null,
                                        null));
                                feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTime>().TimeOffset;

                                // Alternate Link
                                // feed.Links.Add(new SyndicationLink(new Uri(BuildLink.GetLinkNotEscaped(ForumPages.Posts, true))));
                            }

                            feed.Contributors.Add(
                                SyndicationItemExtensions.NewSyndicationPerson(
                                    string.Empty,
                                    row["LastUserID"].ToType<long>(),
                                    null,
                                    null));

                            syndicationItems.AddSyndicationItem(
                                row["Topic"].ToString(),
                                GetPostLatestContent(
                                    BuildLink.GetLinkNotEscaped(
                                        ForumPages.Posts,
                                        true,
                                        "m={0}#post{0}",
                                        row["LastMessageID"]),
                                    lastPostName,
                                    row["LastMessage"].ToString(),
                                    !row["LastMessageFlags"].IsNullOrEmptyDBField()
                                        ? row["LastMessageFlags"].ToType<int>()
                                        : 22,
                                    false),
                                null,
                                BuildLink.GetLinkNotEscaped(ForumPages.Posts, true, "t={0}", row["TopicID"]),
                                $"urn:{urlAlphaNum}:ft{feedType}:st{(atomFeedByVar ? SyndicationFormats.Atom.ToInt() : SyndicationFormats.Rss.ToInt())}:tid{row["TopicID"]}:lmid{row["LastMessageID"]}:{this.PageContext.PageBoardID}"
                                    .Unidecode(),
                                lastPosted,
                                feed);
                        });

                feed.Items = syndicationItems;
            }

            return feed;
        }

        /// <summary>
        /// Cleanup Tables by Date.
        /// TODO: general clean-up after complex sub-queries paging - a repeating code
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
        private static DataTable TabledCleanUpByDate(DataTable dt, string fieldName, DateTime cleanToDate)
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