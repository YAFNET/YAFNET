/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
using System.Threading.Tasks;

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
    /// The method to return latest topic content to display in a feed.
    /// </summary>
    /// <param name="text">An active topic first message content/partial content.</param>
    /// <param name="messageId">The Message Id</param>
    /// <param name="messageAuthorUserId">The Message Author User Id</param>
    /// <returns>
    /// An Html formatted first message content string.
    /// </returns>
    public async Task<string> GetPostLatestContentAsync(
        string text,
        int messageId,
        int messageAuthorUserId)
    {
        text = await this.Get<IFormatMessage>().FormatSyndicationMessageAsync(
            text,
            messageId,
            messageAuthorUserId);

        return text;
    }

    /// <summary>
    /// The method creates Syndication Feed for topics in a forum.
    /// </summary>
    /// <returns>
    /// The <see cref="FeedItem"/>.
    /// </returns>
    public async Task<FeedItem> GetPostLatestFeedAsync()
    {
        var syndicationItems = new List<SyndicationItem>();

        var urlAlphaNum = FormatUrlForFeed(this.Get<BoardInfo>().ForumBaseUrl);

        var feed = new FeedItem(this.Get<ILocalization>().GetText("ACTIVE_DISCUSSIONS"), RssFeeds.LatestPosts, urlAlphaNum);

        var topics = this.GetRepository<Topic>().RssLatest(
            BoardContext.Current.PageBoardID,
            BoardContext.Current.BoardSettings.ActiveDiscussionsCount <= 50
                ? BoardContext.Current.BoardSettings.ActiveDiscussionsCount
                : 50,
            BoardContext.Current.PageUserID);

        foreach (var topic in topics)
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
                await this.GetPostLatestContentAsync(
                    topic.Item1.MessageText,
                    topic.Item1.ID,
                    topic.Item1.UserID),
                null,
                this.Get<ILinkBuilder>().GetAbsoluteLink(
                    ForumPages.Posts,
                    new { t = topic.Item2.ID, name = topic.Item2.TopicName }),
                $"urn:{urlAlphaNum}:ft{RssFeeds.LatestPosts}:tid{topic.Item2.ID}:mid{topic.Item2.LastMessageID}:{BoardContext.Current.PageBoardID}"
                    .Unidecode(),
                lastPosted,
                feed);

            feed.Items = syndicationItems;
        }

        return feed;
    }

    /// <summary>
    /// The method creates the SyndicationFeed for posts.
    /// </summary>
    /// <param name="topicId">
    /// The TopicID
    /// </param>
    /// <returns>
    /// The <see cref="FeedItem"/>.
    /// </returns>
    public async Task<FeedItem> GetPostsFeedAsync(int topicId)
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

        var urlAlphaNum = FormatUrlForFeed(this.Get<BoardInfo>().ForumBaseUrl);

        var feed = new FeedItem(
            $"{this.Get<ILocalization>().GetText("PROFILE", "TOPIC")}{BoardContext.Current.PageTopic.TopicName} - {BoardContext.Current.BoardSettings.PostsPerPage}",
            RssFeeds.Posts,
            urlAlphaNum);

        foreach (var row in posts)
        {
            var posted = row.Edited + this.Get<IDateTimeService>().TimeOffset;

            if (syndicationItems.Count <= 0)
            {
                feed.Authors.Add(SyndicationItemExtensions.NewSyndicationPerson(string.Empty, row.UserID, null, null));
                feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTimeService>().TimeOffset;
            }

            feed.Contributors.Add(SyndicationItemExtensions.NewSyndicationPerson(string.Empty, row.UserID, null, null));

            syndicationItems.AddSyndicationItem(
                row.Topic,
                await this.Get<IFormatMessage>().FormatSyndicationMessageAsync(
                    row.Message,
                    row.MessageID,
                    row.UserID),
                null,
                this.Get<ILinkBuilder>().GetAbsoluteLink(
                    ForumPages.Posts,
                    new { m = row.MessageID, name = row.Topic, t = row.TopicID }),
                $"urn:{urlAlphaNum}:ft{RssFeeds.Posts}:meid{row.MessageID}:{BoardContext.Current.PageBoardID}"
                    .Unidecode(),
                posted,
                feed);
        }

        feed.Items = syndicationItems;

        return feed;
    }

    /// <summary>
    /// The method creates SyndicationFeed for topics in a forum.
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <returns>
    /// The <see cref="FeedItem"/>.
    /// </returns>
    public async Task<FeedItem> GetTopicsFeedAsync(int forumId)
    {
        var syndicationItems = new List<SyndicationItem>();

        var topics = this.GetRepository<Topic>().RssList(
            forumId,
            BoardContext.Current.PageUserID,
            BoardContext.Current.BoardSettings.TopicsFeedItemsCount);

        var urlAlphaNum = FormatUrlForFeed(this.Get<BoardInfo>().ForumBaseUrl);

        var feed = new FeedItem(
            $"{this.Get<ILocalization>().GetText("DEFAULT", "FORUM")}:{BoardContext.Current.PageForum.Name}",
            RssFeeds.Topics,
            urlAlphaNum);

        foreach (var topic in topics)
        {
            var lastPosted = topic.LastPosted + this.Get<IDateTimeService>().TimeOffset;

            if (syndicationItems.Count <= 0)
            {
                feed.Authors.Add(
                    SyndicationItemExtensions.NewSyndicationPerson(string.Empty, topic.LastUserID.Value, null, null));
                feed.LastUpdatedTime = DateTime.UtcNow + this.Get<IDateTimeService>().TimeOffset;
            }

            feed.Contributors.Add(
                SyndicationItemExtensions.NewSyndicationPerson(string.Empty, topic.LastUserID.Value, null, null));

            var postLink = this.Get<ILinkBuilder>().GetAbsoluteLink(
                ForumPages.Posts,
                new { m = topic.LastMessageID.Value, name = topic.Topic, t = topic.TopicID });

            var content = await this.GetPostLatestContentAsync(
                              topic.LastMessage,
                              topic.LastMessageID.Value,
                              topic.LastUserID.Value);

            syndicationItems.AddSyndicationItem(
                topic.Topic,
                content,
                null,
                postLink,
                $"urn:{urlAlphaNum}:ft{RssFeeds.Topics}:tid{topic.TopicID}:lmid{topic.LastMessageID}:{BoardContext.Current.PageBoardID}"
                    .Unidecode(),
                lastPosted.Value,
                feed);
        }

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

        if (formattedUrl.EndsWith('/'))
        {
            formattedUrl = formattedUrl[..^1];
        }

        return formattedUrl;
    }
}