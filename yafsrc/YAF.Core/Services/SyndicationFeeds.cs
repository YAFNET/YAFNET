﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Services;

using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

using YAF.Core.Model;
using YAF.Core.Services.Syndication;
using YAF.Core.Utilities.StringUtils;
using YAF.Types.Constants;
using YAF.Types.Models;

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

                feed = this.GetPostLatestFeed(feedType);
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

                    feed = this.GetTopicsFeed(feedType, forumId.Value);
                }

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
    /// <param name="text">An active topic first message content/partial content.</param>
    /// <param name="messageId">The Message Id</param>
    /// <param name="messageAuthorUserId">The Message Author User Id</param>
    /// <param name="flags">The flags.</param>
    /// <returns>
    /// An Html formatted first message content string.
    /// </returns>
    public string GetPostLatestContent(
        string text,
        int messageId,
        int messageAuthorUserId,
        int flags)
    {
        text = this.Get<IFormatMessage>().FormatSyndicationMessage(
            text,
            messageId,
            messageAuthorUserId,
            new MessageFlags(flags));

        return text;
    }

    /// <summary>
    /// The method creates Syndication Feed for topics in a forum.
    /// </summary>
    /// <param name="feedType">
    /// The FeedType.
    /// </param>
    /// <returns>
    /// The <see cref="FeedItem"/>.
    /// </returns>
    public FeedItem GetPostLatestFeed(RssFeeds feedType)
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

                    syndicationItems.AddSyndicationItem(
                        topic.Item2.TopicName,
                        this.GetPostLatestContent(
                            topic.Item1.MessageText,
                            topic.Item1.ID,
                            topic.Item1.UserID,
                            topic.Item2.LastMessageFlags ?? 22),
                        null,
                        this.Get<LinkBuilder>().GetAbsoluteLink(
                            ForumPages.Posts,
                            new { t = topic.Item2.ID, name = topic.Item2.TopicName }),
                        $"urn:{urlAlphaNum}:ft{feedType}:tid{topic.Item2.ID}:mid{topic.Item2.LastMessageID}:{BoardContext.Current.PageBoardID}"
                            .Unidecode(),
                        lastPosted,
                        feed);

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
    public FeedItem GetPostsFeed(RssFeeds feedType, int topicId)
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

        var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

        var feed = new FeedItem(
            $"{this.Get<ILocalization>().GetText("PROFILE", "TOPIC")}{BoardContext.Current.PageTopic.TopicName} - {BoardContext.Current.BoardSettings.PostsPerPage}",
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

                    feed.Contributors.Add(
                        SyndicationItemExtensions.NewSyndicationPerson(string.Empty, row.UserID, null, null));

                    syndicationItems.AddSyndicationItem(
                        row.Topic,
                        this.Get<IFormatMessage>().FormatSyndicationMessage(
                            row.Message,
                            row.MessageID,
                            row.UserID,
                            new MessageFlags(row.Flags)),
                        null,
                        this.Get<LinkBuilder>().GetAbsoluteLink(
                            ForumPages.Posts,
                            new {m = row.MessageID, name = row.Topic, find = "lastpost"}),
                        $"urn:{urlAlphaNum}:ft{feedType}:meid{row.MessageID}:{BoardContext.Current.PageBoardID}"
                            .Unidecode(),
                        posted,
                        feed);
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
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <returns>
    /// The <see cref="FeedItem"/>.
    /// </returns>
    public FeedItem GetTopicsFeed(RssFeeds feedType, int forumId)
    {
        var syndicationItems = new List<SyndicationItem>();

        var topics = this.GetRepository<Topic>().RssList(
            forumId,
            BoardContext.Current.PageUserID,
            BoardContext.Current.BoardSettings.TopicsFeedItemsCount);

        var urlAlphaNum = FormatUrlForFeed(BaseUrlBuilder.BaseUrl);

        var feed = new FeedItem(
            $"{this.Get<ILocalization>().GetText("DEFAULT", "FORUM")}:{BoardContext.Current.PageForum.Name}",
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

                    var postLink = this.Get<LinkBuilder>().GetAbsoluteLink(
                        ForumPages.Posts,
                        new { m = topic.LastMessageID.Value, name = topic.Topic });

                    var content = this.GetPostLatestContent(
                        topic.LastMessage,
                        topic.LastMessageID.Value,
                        topic.LastUserID.Value,
                        topic.LastMessageFlags.Value);

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
    private static string FormatUrlForFeed(string inputUrl)
    {
        var formattedUrl = inputUrl;

        formattedUrl = formattedUrl.Replace(".", "-").Replace("/", "-");

        if (formattedUrl.EndsWith("/"))
        {
            formattedUrl = formattedUrl.Remove(formattedUrl.Length - 1);
        }

        return formattedUrl;
    }
}