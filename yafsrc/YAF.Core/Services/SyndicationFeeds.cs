/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services.Syndication;
    using YAF.Core.Utilities.StringUtils;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The Syndication feeds.
    /// </summary>
    public class SyndicationFeeds : IHaveServiceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyndicationFeeds"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public SyndicationFeeds(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        /// <summary>
        ///     Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        /// Gets the Feed
        /// </summary>
        public void GetFeed()
        {
            FeedItem feed = null;

            var lastPostName = this.Get<ILocalization>().GetText("DEFAULT", "GO_LAST_POST");

            RssFeeds feedType;

            try
            {
                feedType = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("feed").ToEnum<RssFeeds>(true);
            }
            catch
            {
                // default to Forum Feed.
                feedType = RssFeeds.LatestPosts;
            }

            switch (feedType)
            {
                // Latest posts feed
                case RssFeeds.LatestPosts:
                    if (!(BoardContext.Current.BoardSettings.ShowActiveDiscussions && this.Get<IPermissions>()
                        .Check(BoardContext.Current.BoardSettings.PostLatestFeedAccess)))
                    {
                        this.Get<LinkBuilder>().AccessDenied();
                    }

                    feed = this.GetPostLatestFeed(feedType, lastPostName);
                    break;

                // Posts Feed
                case RssFeeds.Posts:
                    if (!(BoardContext.Current.ForumReadAccess && this.Get<IPermissions>()
                        .Check(BoardContext.Current.BoardSettings.PostsFeedAccess)))
                    {
                        this.Get<LinkBuilder>().AccessDenied();
                    }

                    if (this.Get<HttpRequestBase>().QueryString.Exists("t"))
                    {
                        var topicId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("t");

                        feed = this.GetPostsFeed(feedType, topicId.Value);
                    }

                    break;

                // Topics Feed
                case RssFeeds.Topics:
                    if (!(BoardContext.Current.ForumReadAccess && this.Get<IPermissions>()
                        .Check(BoardContext.Current.BoardSettings.TopicsFeedAccess)))
                    {
                        this.Get<LinkBuilder>().AccessDenied();
                    }

                    if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
                    {
                        var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("f");

                        feed = this.GetTopicsFeed(feedType, lastPostName, forumId.Value);
                    }

                    break;

                case RssFeeds.Favorite:
                    if (!this.Get<IPermissions>().Check(BoardContext.Current.BoardSettings.FavoriteTopicFeedAccess))
                    {
                        this.Get<LinkBuilder>().AccessDenied();
                    }

                    feed = this.GetFavoriteFeed(feedType, lastPostName);
                    break;
                default:
                    this.Get<LinkBuilder>().AccessDenied();
                    break;
            }

            // update the feed with the item list...
            // the list should be added after all other feed properties are set
            if (feed != null)
            {
                var writer = new XmlTextWriter(this.Get<HttpResponseBase>().OutputStream, Encoding.UTF8);
                writer.WriteStartDocument();

                // write the feed to the response writer
                var atomFormatter = new Atom10FeedFormatter(feed);
                atomFormatter.WriteTo(writer);

                this.Get<HttpResponseBase>().ContentType = "application/atom+xml";

                writer.WriteEndDocument();
                writer.Close();

                this.Get<HttpResponseBase>().ContentEncoding = Encoding.UTF8;
                this.Get<HttpResponseBase>().Cache.SetCacheability(HttpCacheability.Public);

                this.Get<HttpResponseBase>().End();
            }
            else
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
            }
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
        public string GetPostLatestContent(
            [NotNull] string link,
            [NotNull] string linkName,
            [NotNull] string text,
            [NotNull] int flags,
            [NotNull] bool altItem)
        {
            text = this.Get<IFormatMessage>().FormatSyndicationMessage(text, new MessageFlags(flags), altItem, 4000);

            return $@"{text}<a href=""{link}"" >{linkName}</a>";
        }

        /// <summary>
        /// method the SyndicationFeed for Favorite topics.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="lastPostName">
        /// The last post name.
        /// </param>
        /// <returns>
        /// The <see cref="FeedItem"/>.
        /// </returns>
        public FeedItem GetFavoriteFeed([NotNull] RssFeeds feedType, [NotNull] string lastPostName)
        {
            var syndicationItems = new List<SyndicationItem>();

            var toFavDate = BoardContext.Current.User.Joined;

            var toFavText = this.Get<ILocalization>().GetText("MYTOPICS", "SHOW_ALL");

            var list = this.GetRepository<Types.Models.FavoriteTopic>().ListPaged(
                BoardContext.Current.PageUserID,
                toFavDate,
                DateTime.UtcNow,
                0,
                20,
                false);

            var urlAlphaNum = FormatUrlForFeed(BoardInfo.ForumBaseUrl);
            var feedNameAlphaNum = new Regex(@"[^A-Za-z0-9]", RegexOptions.IgnoreCase).Replace(toFavText, string.Empty);

            var feed = new FeedItem(
                $"{this.Get<ILocalization>().GetText("MYTOPICS", "FAVORITETOPICS")} - {toFavText}",
                feedType,
                urlAlphaNum);

            list.ForEach(
                t =>
                {
                    var lastPosted = t.LastPosted.Value + this.Get<IDateTimeService>().TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(string.Empty, t.UserID, null, null));
                        feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTimeService>().TimeOffset;
                    }

                    feed.Contributors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(string.Empty, t.LastUserID.Value, null, null));

                    syndicationItems.AddSyndicationItem(
                        t.Subject,
                        this.GetPostLatestContent(
                            this.Get<LinkBuilder>().GetLink(ForumPages.Posts, true, "m={0}&name={1}", t.LastMessageID, t.Subject),
                            lastPostName,
                            lastPostName,
                            t.LastMessageFlags ?? 22,
                            false),
                        null,
                        this.Get<LinkBuilder>().GetLink(ForumPages.Posts, true, "t={0}&name={1}", t.LinkTopicID, t.Subject),
                        $"urn:{urlAlphaNum}:ft{feedType}:span{feedNameAlphaNum}:ltid{t.LinkTopicID}:lmid{t.LastMessageID}:{BoardContext.Current.PageBoardID}"
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
        public List<SyndicationLink> GetMediaLinks(int messageId)
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
                            $"{BoardInfo.ForumBaseUrl}{BoardInfo.ForumClientFileRoot.TrimStart('/')}resource.ashx?a={attachment.ID}&b={BoardContext.Current.PageBoardID}"),
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
        /// <param name="lastPostName">
        /// The last post name.
        /// </param>
        /// <returns>
        /// The <see cref="FeedItem"/>.
        /// </returns>
        public FeedItem GetPostLatestFeed([NotNull] RssFeeds feedType, [NotNull] string lastPostName)
        {
            var syndicationItems = new List<SyndicationItem>();

            var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

            var feed = new FeedItem(this.Get<ILocalization>().GetText("ACTIVE_DISCUSSIONS"), feedType, urlAlphaNum);

            var topics = this.GetRepository<Topic>().RssLatest(
                BoardContext.Current.PageBoardID,
                BoardContext.Current.BoardSettings.ActiveDiscussionsCount <= 50
                    ? BoardContext.Current.BoardSettings.ActiveDiscussionsCount
                    : 50,
                BoardContext.Current.PageUserID);

            var altItem = false;

            topics.ForEach(
                topic =>
                {
                    var lastPosted = topic.Item2.LastPosted.Value + this.Get<IDateTimeService>().TimeOffset;
                    if (syndicationItems.Count <= 0)
                    {
                        feed.LastUpdatedTime = lastPosted + this.Get<IDateTimeService>().TimeOffset;
                        feed.Authors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty,
                                topic.Item2.UserID,
                                topic.Item2.UserName,
                                topic.Item2.UserDisplayName));
                    }

                    feed.Contributors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(
                            string.Empty,
                            topic.Item2.LastUserID.Value,
                            topic.Item2.LastUserName,
                            topic.Item2.LastUserDisplayName));

                    var messageLink = this.Get<LinkBuilder>().GetLink(
                        ForumPages.Posts,
                        true,
                        "m={0}&name={1}",
                        topic.Item2.LastMessageID,
                        topic.Item2.TopicName);

                    syndicationItems.AddSyndicationItem(
                        topic.Item2.TopicName,
                        this.GetPostLatestContent(
                            messageLink,
                            lastPostName,
                            topic.Item1.MessageText,
                            topic.Item2.LastMessageFlags ?? 22,
                            altItem),
                        null,
                        this.Get<LinkBuilder>().GetLink(
                            ForumPages.Posts,
                            true,
                            "t={0}&name={1}",
                            topic.Item2.ID,
                            topic.Item2.TopicName),
                        $"urn:{urlAlphaNum}:ft{feedType}:tid{topic.Item2.ID}:mid{topic.Item2.LastMessageID}:{BoardContext.Current.PageBoardID}"
                            .Unidecode(),
                        lastPosted,
                        feed);

                    altItem = !altItem;

                    feed.Items = syndicationItems;
                });

            return feed;
        }

        /// <summary>
        /// The method creates the SyndicationFeed for posts.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="topicId">
        /// The TopicID
        /// </param>
        /// <returns>
        /// The <see cref="FeedItem"/>.
        /// </returns>
        public FeedItem GetPostsFeed([NotNull] RssFeeds feedType, [NotNull] int topicId)
        {
            var syndicationItems = new List<SyndicationItem>();

            var showDeleted = BoardContext.Current.BoardSettings.ShowDeletedMessagesToAll;

            var posts = this.GetRepository<Message>().PostListPaged(
                topicId,
                BoardContext.Current.PageUserID,
                false,
                showDeleted,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                0,
                BoardContext.Current.BoardSettings.PostsPerPage,
                -1);

            var altItem = false;

            var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

            var feed = new FeedItem(
                $"{this.Get<ILocalization>().GetText("PROFILE", "TOPIC")}{BoardContext.Current.PageTopicName} - {BoardContext.Current.BoardSettings.PostsPerPage}",
                feedType,
                urlAlphaNum);

            posts.ForEach(
                row =>
                {
                    var posted = row.Edited + this.Get<IDateTimeService>().TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(string.Empty, row.UserID, null, null));
                        feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTimeService>().TimeOffset;
                    }

                    List<SyndicationLink> attachmentLinks = null;

                    // if the user doesn't have download access we simply don't show enclosure links.
                    if (BoardContext.Current.ForumDownloadAccess)
                    {
                        attachmentLinks = this.GetMediaLinks(row.MessageID);
                    }

                    feed.Contributors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(string.Empty, row.UserID, null, null));

                    syndicationItems.AddSyndicationItem(
                        row.Topic,
                        this.Get<IFormatMessage>().FormatSyndicationMessage(
                            row.Message,
                            new MessageFlags(row.Flags),
                            altItem,
                            4000),
                        null,
                        this.Get<LinkBuilder>().GetLink(
                            ForumPages.Posts,
                            true,
                            "m={0}&name={1}&find=lastpost",
                            row.MessageID,
                            row.Topic),
                        $"urn:{urlAlphaNum}:ft{feedType}:meid{row.MessageID}:{BoardContext.Current.PageBoardID}"
                            .Unidecode(),
                        posted,
                        feed,
                        attachmentLinks);

                    // used to format feeds
                    altItem = !altItem;
                });

            feed.Items = syndicationItems;

            return feed;
        }

        /// <summary>
        /// The method creates SyndicationFeed for topics in a forum.
        /// </summary>
        /// <param name="feedType">
        /// The FeedType.
        /// </param>
        /// <param name="lastPostName">
        /// The last post name.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <returns>
        /// The <see cref="FeedItem"/>.
        /// </returns>
        public FeedItem GetTopicsFeed([NotNull] RssFeeds feedType, [NotNull] string lastPostName, [NotNull] int forumId)
        {
            var syndicationItems = new List<SyndicationItem>();

            var topics = this.GetRepository<Topic>().RssList(
                forumId,
                BoardContext.Current.PageUserID,
                BoardContext.Current.BoardSettings.TopicsFeedItemsCount);

            var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

            var feed = new FeedItem(
                $"{this.Get<ILocalization>().GetText("DEFAULT", "FORUM")}:{BoardContext.Current.PageForumName}",
                feedType,
                urlAlphaNum);

            topics.ForEach(
                topic =>
                {
                    var lastPosted = topic.LastPosted + this.Get<IDateTimeService>().TimeOffset;

                    if (syndicationItems.Count <= 0)
                    {
                        feed.Authors.Add(
                            SyndicationItemExtensions.NewSyndicationPerson(
                                string.Empty,
                                topic.LastUserID.Value,
                                null,
                                null));
                        feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTimeService>().TimeOffset;
                    }

                    feed.Contributors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(
                            string.Empty,
                            topic.LastUserID.Value,
                            null,
                            null));

                    var postLink = this.Get<LinkBuilder>().GetLink(
                        ForumPages.Posts,
                        true,
                        "m={0}&name={1}",
                        topic.LastMessageID.Value,
                        topic.Topic);

                    var content = this.GetPostLatestContent(
                        postLink,
                        lastPostName,
                        topic.LastMessage,
                        topic.LastMessageFlags.Value,
                        false);

                    syndicationItems.AddSyndicationItem(
                        topic.Topic,
                        content,
                        null,
                        postLink,
                        $"urn:{urlAlphaNum}:ft{feedType}:tid{topic.TopicID}:lmid{topic.LastMessageID}:{BoardContext.Current.PageBoardID}"
                            .Unidecode(),
                        lastPosted.Value,
                        feed);
                });

            feed.Items = syndicationItems;

            return feed;
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

            /*if (formattedUrl.Contains(@"http://www."))
            {
                formattedUrl = formattedUrl.Replace("http://www.", string.Empty);
            }
            else if (formattedUrl.Contains(@"http://"))
            {
                formattedUrl = formattedUrl.Replace("http://", string.Empty);
            }*/

            formattedUrl = formattedUrl.Replace(".", "-").Replace("/", "-");

            if (formattedUrl.EndsWith("/"))
            {
                formattedUrl = formattedUrl.Remove(formattedUrl.Length - 1);
            }

            return formattedUrl;
        }
    }
}