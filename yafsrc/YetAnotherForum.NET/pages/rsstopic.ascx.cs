/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
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
            bool atomFeedByVar = this.Request.QueryString.GetFirstOrDefault("ft")
                                 == YafSyndicationFormats.Atom.ToInt().ToString();

            YafSyndicationFeed feed = null;

            // var syndicationItems = new List<SyndicationItem>();
            string lastPostIcon = BaseUrlBuilder.BaseUrl
                                  + this.PageContext.CurrentForumPage.GetThemeContents("ICONS", "ICON_NEWEST");
            string lastPostName = this.GetText("DEFAULT", "GO_LAST_POST");

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

                    if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t") != null)
                    {
                        int topicId;
                        if (int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t"), out topicId))
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

                    object categoryId = null;

                    if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("c") != null)
                    {
                        int icategoryId;
                        if (int.TryParse(
                            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("c"), out icategoryId))
                        {
                            categoryId = icategoryId;
                        }
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

                    if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f") != null)
                    {
                        int forumId;
                        if (int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"), out forumId))
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

                    int categoryActiveIntId;
                    object categoryActiveId = null;
                    if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f") != null
                        &&
                        int.TryParse(
                            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"), out categoryActiveIntId))
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

                    int categoryFavIntId;
                    object categoryFavId = null;
                    if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f") != null
                        &&
                        int.TryParse(
                            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"), out categoryFavIntId))
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
                var writer = new XmlTextWriter(this.Response.OutputStream, Encoding.UTF8);
                writer.WriteStartDocument();

                // write the feed to the response writer);
                if (!atomFeedByVar)
                {
                    var rssFormatter = new Rss20FeedFormatter(feed);
                    rssFormatter.WriteTo(writer);
                    this.Response.ContentType = "application/rss+xml";
                }
                else
                {
                    var atomFormatter = new Atom10FeedFormatter(feed);
                    atomFormatter.WriteTo(writer);

                    this.Response.ContentType = "application/atom+xml";
                }

                writer.WriteEndDocument();
                writer.Close();

                this.Response.ContentEncoding = Encoding.UTF8;
                this.Response.Cache.SetCacheability(HttpCacheability.Public);

                this.Response.End();
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
            string formatedUrl = inputUrl;

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
        /// <param name="link">A linkt to an active topic.</param>
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
                @"{0}<table><tr><td><a href=""{1}"" ><img src=""{2}"" alt =""{3}"" title =""{3}"" /></a></td></tr><table>"
                    .FormatWith(text, link, imgUrl, imgAlt, linkName);
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
            DateTime toActDate = DateTime.UtcNow;
            string toActText = this.GetText("MYTOPICS", "LAST_MONTH");

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("txt") != null)
            {
                toActText =
                    this.Server.UrlDecode(
                        this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("txt")));
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("d") != null)
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

            string urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);
            string feedNameAlphaNum = new Regex(@"[^A-Za-z0-9]", RegexOptions.IgnoreCase).Replace(
                toActText, string.Empty);
            feed = new YafSyndicationFeed(
                this.GetText("MYTOPICS", "ACTIVETOPICS") + " - " + toActText,
                feedType,
                atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                urlAlphaNum);

            using (
                DataTable dt =
                    this.TabledCleanUpByDate(
                        LegacyDb.topic_active(
                            this.PageContext.PageBoardID,
                            categoryActiveId,
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

                    DateTime lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

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

                    string messageLink = YafBuildLink.GetLinkNotEscaped(
                        ForumPages.posts, true, "m={0}&find=lastpost", row["LastMessageID"]);
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
                        "urn:{0}:ft{1}:st{2}:span{3}:ltid{4}:lmid{5}:{6}".FormatWith(
                            urlAlphaNum,
                            feedNameAlphaNum,
                            feedType,
                            atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                            row["LinkTopicID"],
                            row["LastMessageID"],
                            this.PageContext.PageBoardID).Unidecode(),
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

            string toFavText = this.GetText("MYTOPICS", "LAST_MONTH");

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("txt") != null)
            {
                toFavText =
                    this.Server.UrlDecode(
                        this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("txt")));
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("d") != null)
            {
                if (
                    !DateTime.TryParse(
                        this.Server.UrlDecode(
                            this.Server.HtmlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("d"))),
                        out toFavDate))
                {
                    toFavDate = this.PageContext.CurrentUserData.Joined == null
                                    ? DateTime.MinValue + TimeSpan.FromDays(2)
                                    : (DateTime)this.PageContext.CurrentUserData.Joined;
                    toFavText = this.GetText("MYTOPICS", "SHOW_ALL");
                }
            }
            else
            {
                toFavDate = this.PageContext.CurrentUserData.Joined == null
                                ? DateTime.MinValue + TimeSpan.FromDays(2)
                                : (DateTime)this.PageContext.CurrentUserData.Joined;
                toFavText = this.GetText("MYTOPICS", "SHOW_ALL");
            }

            using (
                DataTable dt =
                    this.TabledCleanUpByDate(
                        LegacyDb.topic_favorite_details(
                            this.PageContext.PageBoardID,
                            categoryActiveId,
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
                string urlAlphaNum = FormatUrlForFeed(YafForumInfo.ForumBaseUrl);
                string feedNameAlphaNum = new Regex(@"[^A-Za-z0-9]", RegexOptions.IgnoreCase).Replace(
                    toFavText, string.Empty);
                feed =
                    new YafSyndicationFeed(
                        "{0} - {1}".FormatWith(this.GetText("MYTOPICS", "FAVORITETOPICS"), toFavText),
                        feedType,
                        atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                        urlAlphaNum);

                foreach (DataRow row in dt.Rows)
                {
                    if (!row["TopicMovedID"].IsNullOrEmptyDBField())
                    {
                        continue;
                    }

                    DateTime lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

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
                                ForumPages.posts, true, "m={0}&find=lastpost", row["LastMessageID"]),
                            lastPostIcon,
                            lastPostName,
                            lastPostName,
                            string.Empty,
                            !row["LastMessageFlags"].IsNullOrEmptyDBField() ? row["LastMessageFlags"].ToType<int>() : 22,
                            false),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["LinkTopicID"]),
                        "urn:{0}:ft{1}:st{2}:span{3}:ltid{4}:lmid{5}:{6}".FormatWith(
                            urlAlphaNum,
                            feedType,
                            atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                            feedNameAlphaNum,
                            row["LinkTopicID"].ToType<int>(),
                            row["LastMessageID"],
                            this.PageContext.PageBoardID).Unidecode(),
                        lastPosted,
                        feed);
                }

                feed.Items = syndicationItems;
            }
        }

        /// <summary>
        /// The method creates YafSyndicationFeed for forums in a category.
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
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        private void GetForumFeed(
            [NotNull] ref YafSyndicationFeed feed, YafRssFeeds feedType, bool atomFeedByVar, [NotNull] object categoryId)
        {
            var syndicationItems = new List<SyndicationItem>();
            using (
                DataTable dt = LegacyDb.forum_listread(
                    this.PageContext.PageBoardID, this.PageContext.PageUserID, categoryId, null, false, false))
            {
                string urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

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

                    DateTime lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

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
                        "urn:{0}:ft{1}:st{2}:fid{3}:lmid{4}:{5}".FormatWith(
                            urlAlphaNum,
                            feedType,
                            atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                            row["ForumID"],
                            row["LastMessageID"],
                            this.PageContext.PageBoardID).Unidecode(),
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
            using (
                DataTable dt = LegacyDb.topic_announcements(
                    this.PageContext.PageBoardID, 10, this.PageContext.PageUserID))
            {
                string urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

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

                    DateTime lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

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
                        "urn:{0}:ft{1}:st{2}:tid{3}:lmid{4}:{5}".FormatWith(
                            urlAlphaNum,
                            feedType,
                            atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t"),
                            row["LastMessageID"],
                            this.PageContext.PageBoardID).Unidecode(),
                        lastPosted,
                        feed);
                }

                feed.Items = syndicationItems;
            }
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
            using (var attList = this.GetRepository<Attachment>().List(messageId, null, this.PageContext.PageBoardID, 0, 1000))
            {
                if (attList.Rows.Count > 0)
                {
                    attachementLinks.AddRange(
                        from DataRow attachLink in attList.Rows
                        where !attachLink["FileName"].IsNullOrEmptyDBField()
                        select
                            new SyndicationLink(
                            new Uri(
                            "{0}{1}resource.ashx?a={2}".FormatWith(
                                YafForumInfo.ForumBaseUrl, YafForumInfo.ForumClientFileRoot.TrimStart('/'), attachLink["AttachmentID"])),
                            "enclosure",
                            attachLink["FileName"].ToString(),
                            attachLink["ContentType"].ToString(),
                            attachLink["Bytes"].ToType<long>()));
                }
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
                DataTable dataTopics = LegacyDb.rss_topic_latest(
                    this.PageContext.PageBoardID,
                    this.Get<YafBoardSettings>().ActiveDiscussionsCount <= 50
                        ? this.Get<YafBoardSettings>().ActiveDiscussionsCount
                        : 50,
                    this.PageContext.PageUserID,
                    this.Get<YafBoardSettings>().UseStyledNicks,
                    this.Get<YafBoardSettings>().NoCountForumsInActiveDiscussions))
            {
                string urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed = new YafSyndicationFeed(
                    this.GetText("ACTIVE_DISCUSSIONS"),
                    feedType,
                    atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                    urlAlphaNum);
                bool altItem = false;
                foreach (DataRow row in dataTopics.Rows)
                {
                    // don't render moved topics
                    if (row["TopicMovedID"].IsNullOrEmptyDBField())
                    {
                        DateTime lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;
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

                        string messageLink = YafBuildLink.GetLinkNotEscaped(
                            ForumPages.posts, true, "m={0}&find=lastpost", row["LastMessageID"]);

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
                            "urn:{0}:ft{1}:st{2}:tid{3}:mid{4}:{5}".FormatWith(
                                urlAlphaNum,
                                feedType,
                                atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                                row["TopicID"],
                                row["LastMessageID"],
                                this.PageContext.PageBoardID).Unidecode(),
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

            bool showDeleted = false;
            int userId = 0;

            if (this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
            {
                showDeleted = true;
            }

            if (!showDeleted
                &&
                ((this.Get<YafBoardSettings>().ShowDeletedMessages
                  && !this.Get<YafBoardSettings>().ShowDeletedMessagesToAll) || this.PageContext.IsAdmin
                 || this.PageContext.IsForumModerator))
            {
                userId = this.PageContext.PageUserID;
            }

            using (
                DataTable dt = LegacyDb.post_list(
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

                string urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed =
                    new YafSyndicationFeed(
                        "{0}{1} - {2}".FormatWith(
                            this.GetText("PROFILE", "TOPIC"),
                            this.PageContext.PageTopicName,
                            this.Get<YafBoardSettings>().PostsPerPage),
                        feedType,
                        atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                        urlAlphaNum);

                foreach (var row in dataRows)
                {
                    DateTime posted = Convert.ToDateTime(row["Edited"]) + this.Get<IDateTime>().TimeOffset;

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
                        "urn:{0}:ft{1}:st{2}:meid{3}:{4}".FormatWith(
                            urlAlphaNum,
                            feedType,
                            atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                            row["MessageID"],
                            this.PageContext.PageBoardID).Unidecode(),
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
            using (DataTable dt = LegacyDb.rsstopic_list(forumId, this.Get<YafBoardSettings>().TopicsFeedItemsCount))
            {
                string urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

                feed = new YafSyndicationFeed(
                    "{0}:{1}".FormatWith(this.GetText("DEFAULT", "FORUM"), this.PageContext.PageForumName),
                    feedType,
                    atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                    urlAlphaNum);

                foreach (DataRow row in dt.Rows)
                {
                    DateTime lastPosted = Convert.ToDateTime(row["LastPosted"]) + this.Get<IDateTime>().TimeOffset;

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
                                ForumPages.posts, true, "m={0}&find=lastpost", row["LastMessageID"]),
                            lastPostIcon,
                            lastPostName,
                            lastPostName,
                            string.Empty,
                            !row["LastMessageFlags"].IsNullOrEmptyDBField() ? row["LastMessageFlags"].ToType<int>() : 22,
                            false),
                        null,
                        YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", row["TopicID"]),
                        "urn:{0}:ft{1}:st{2}:tid{3}:lmid{4}:{5}".FormatWith(
                            urlAlphaNum,
                            feedType,
                            atomFeedByVar ? YafSyndicationFormats.Atom.ToInt() : YafSyndicationFormats.Rss.ToInt(),
                            row["TopicID"],
                            row["LastMessageID"],
                            this.PageContext.PageBoardID).Unidecode(),
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
            if (dt == null || dt.Rows.Count <= 0)
            {
                return dt;
            }

            DataTable topicsNew = dt.Copy();
            foreach (
                DataRow thisTableRow in
                    topicsNew.Rows.Cast<DataRow>().Where(
                        thisTableRow =>
                        thisTableRow[fieldName] != DBNull.Value
                        && thisTableRow[fieldName].ToType<DateTime>() <= cleanToDate))
            {
                thisTableRow.Delete();
            }

            // styled nicks
            topicsNew.AcceptChanges();
            return topicsNew;
        }

        #endregion
    }
}