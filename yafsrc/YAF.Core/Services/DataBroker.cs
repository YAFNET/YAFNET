/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

using YAF.Core.Model;
using YAF.Types.Constants;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
///     Class used for multi-step DB operations so they can be cached, etc.
/// </summary>
public class DataBroker : IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataBroker"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="boardSettings">
    /// The board settings.
    /// </param>
    /// <param name="dataCache">
    /// The data cache.
    /// </param>
    public DataBroker(
        IServiceLocator serviceLocator,
        BoardSettings boardSettings,
        IDataCache dataCache)
    {
        this.ServiceLocator = serviceLocator;
        this.BoardSettings = boardSettings;
        this.DataCache = dataCache;
    }

    /// <summary>
    /// Gets or sets the board settings.
    /// </summary>
    /// <value>
    /// The board settings.
    /// </value>
    public BoardSettings BoardSettings { get; set; }

    /// <summary>
    ///     Gets or sets DataCache.
    /// </summary>
    public IDataCache DataCache { get; set; }

    /// <summary>
    ///     Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    ///     The user lazy data.
    /// </summary>
    /// <param name="userId"> The user ID. </param>
    /// <returns> Returns the Active User </returns>
    public UserLazyData ActiveUserLazyData(int userId)
    {
        // get a row with user lazy data...
        return
            this.DataCache.GetOrSet(
                string.Format(Constants.Cache.ActiveUserLazyData, userId),
                () =>
                    this.GetRepository<User>().LazyData(
                        userId,
                        BoardContext.Current.PageBoardID,
                        this.BoardSettings.EnableBuddyList,
                        this.BoardSettings.AllowPrivateMessages,
                        this.BoardSettings.EnableAlbum),
                TimeSpan.FromMinutes(this.BoardSettings.ActiveUserLazyDataCacheTimeout));
    }

    /// <summary>
    /// Returns the layout of the board
    /// </summary>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="pageIndex">
    /// The Current Page Index
    /// </param>
    /// <param name="pageSize">
    /// The Number of items to retrieve.
    /// </param>
    /// <param name="categoryId">
    /// The category Id.
    /// </param>
    /// <param name="parentId">
    /// The parent Id.
    /// </param>
    /// <returns>
    /// The <see cref="Tuple"/>.
    /// </returns>
    public Tuple<List<SimpleModerator>, List<ForumRead>> BoardLayout(
        [NotNull] int boardId,
        [NotNull] int userId,
        [NotNull] int pageIndex,
        [NotNull] int pageSize,
        [CanBeNull] int? categoryId,
        [CanBeNull] int? parentId)
    {
        if (categoryId is 0)
        {
            categoryId = null;
        }

        // get the cached version of forum moderators if it's valid
        var moderators = new List<SimpleModerator>();

        if (this.BoardSettings.ShowModeratorList)
        {
            moderators = this.GetModerators();
        }

        var forums = this.GetRepository<Forum>().ListRead(
            boardId,
            userId,
            categoryId,
            parentId,
            this.BoardSettings.UseReadTrackingByDatabase,
            pageIndex,
            pageSize);

        return new Tuple<List<SimpleModerator>, List<ForumRead>>(moderators, forums);
    }

    /// <summary>
    /// The Page Load as Data Row
    /// </summary>
    /// <param name="sessionId">
    /// The session id.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="userKey">
    /// The user key.
    /// </param>
    /// <param name="ip">
    /// The IP Address.
    /// </param>
    /// <param name="location">
    /// The location.
    /// </param>
    /// <param name="forumPage">
    /// The forum page name.
    /// </param>
    /// <param name="browser">
    /// The browser.
    /// </param>
    /// <param name="platform">
    /// The platform.
    /// </param>
    /// <param name="categoryId">
    /// The category Id.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <param name="isCrawler">
    /// The is Crawler.
    /// </param>
    /// <param name="doNotTrack">
    /// The do Not Track.
    /// </param>
    /// <returns>
    /// The <see cref="PageLoad"/>.
    /// </returns>
    public Tuple<PageLoad, User, Category, Forum, Topic, Message> GetPageLoad(
        [NotNull] string sessionId,
        [NotNull] int boardId,
        [CanBeNull] string userKey,
        [NotNull] string ip,
        [NotNull] string location,
        [NotNull] string forumPage,
        [NotNull] string browser,
        [NotNull] string platform,
        [CanBeNull] int? categoryId,
        [CanBeNull] int? forumId,
        [CanBeNull] int? topicId,
        [CanBeNull] int? messageId,
        [NotNull] bool isCrawler,
        [NotNull] bool doNotTrack)
    {
        while (true)
        {
            int userId;
            bool isGuest;
            DateTime? previousVisit = null;
            User currentUser;
            var activeUpdate = false;

            // -- set IsActiveNow ActiveFlag - it's a default
            var activeFlags = new ActiveFlags {IsActiveNow = true};

            // -- find a guest id should do it every time to be sure that guest access rights are in ActiveAccess table
            var guestUser = this.Get<IAspNetUsersHelper>().GuestUser(boardId);
                
            if (userKey == null)
            {
                currentUser = guestUser;

                // -- this is a guest
                userId = guestUser.ID;
                previousVisit = guestUser.LastVisit;
                isGuest = true;

                // -- set IsGuest ActiveFlag  1 | 2
                activeFlags.IsGuest = true;

                // -- crawlers are always guests
                if (isCrawler)
                {
                    // -- set IsCrawler ActiveFlag
                    activeFlags.IsCrawler = true;
                }
            }
            else
            {
                currentUser = this.GetRepository<User>()
                    .GetSingle(u => u.BoardID == boardId && u.ProviderUserKey == userKey);

                if (currentUser != null)
                {
                    userId = currentUser.ID;

                    isGuest = false;

                    // make sure that registered users are not crawlers
                    isCrawler = false;

                    // -- set IsRegistered ActiveFlag
                    activeFlags.IsRegistered = true;
                }
                else
                {
                    currentUser = guestUser;

                    // -- this is a guest
                    userId = guestUser.ID;
                    previousVisit = guestUser.LastVisit;
                    isGuest = true;

                    // -- set IsGuest ActiveFlag  1 | 2
                    activeFlags = 3;

                    // -- crawlers are always guests
                    if (isCrawler)
                    {
                        // -- set IsCrawler ActiveFlag
                        activeFlags.IsCrawler = true;
                    }
                }
            }

            // -- Check valid ForumID
            var forum = forumId is > 0 ? this.GetRepository<Forum>().GetById(forumId.Value) : null;

            if (forum == null)
            {
                forumId = null;
            }

            // -- Check valid CategoryID
            var category = categoryId is > 0 ? this.GetRepository<Category>().GetById(categoryId.Value) : null;

            if (category == null)
            {
                categoryId = null;
            }

            // -- Check valid MessageID
            if (messageId is 0)
            {
                messageId = null;
            }

            // -- Check valid TopicID
            if (topicId is 0)
            {
                topicId = null;
            }

            Topic topic = null;
            Message message = null;

            if (!this.GetRepository<ActiveAccess>().Exists(a => a.UserID == userId))
            {
                var accessList = this.GetRepository<vaccess>().Get(x => x.UserID == userId);

                var activeList = new List<ActiveAccess>();

                // -- update active access
                // -- ensure that access right are in place
                accessList.ForEach(
                    access => activeList.Add(
                        new ActiveAccess
                            {
                                UserID = userId,
                                BoardID = boardId,
                                ForumID = access.ForumID,
                                IsAdmin = access.IsAdmin > 0,
                                IsForumModerator = access.IsForumModerator > 0,
                                IsModerator = access.IsModerator > 0,
                                IsGuestX = isGuest,
                                LastActive = DateTime.UtcNow,
                                ReadAccess = access.ReadAccess > 0,
                                PostAccess = access.PostAccess > 0,
                                ReplyAccess = access.ReplyAccess > 0,
                                PriorityAccess = access.PriorityAccess > 0,
                                PollAccess = access.PollAccess > 0,
                                VoteAccess = access.VoteAccess > 0,
                                ModeratorAccess = access.ModeratorAccess > 0,
                                EditAccess = access.EditAccess > 0,
                                DeleteAccess = access.DeleteAccess > 0
                            }));

                this.GetRepository<ActiveAccess>().InsertAll(activeList);
            }

            // -- find missing ForumID/TopicID
            if (messageId.HasValue && (!forumId.HasValue || !categoryId.HasValue || !topicId.HasValue))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

                var id = messageId;
                expression.Join<Topic>((m, t) => t.ID == m.TopicID).Join<Topic, Forum>((t, f) => f.ID == t.ForumID)
                    .Join<Forum, Category>((f, c) => c.ID == f.CategoryID)
                    .Where<Message, Category>((m, c) => m.ID == id.Value && c.BoardID == boardId && (c.Flags & 1) == 1);

                var result = this.GetRepository<ActiveAccess>().DbAccess.Execute(
                    db => db.Connection.SelectMulti<Message, Topic, Forum, Category>(expression)).FirstOrDefault();

                if (result != null)
                {
                    message = result.Item1;

                    categoryId = result.Item3.CategoryID;
                    category = result.Item4;

                    forumId = result.Item3.ID;
                    forum = result.Item3;

                    topicId = result.Item1.TopicID;
                    topic = result.Item2;
                }
                else
                {
                    messageId = null;
                }
            }

            if (topicId.HasValue && (!categoryId.HasValue || !forumId.HasValue))
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

                var id = topicId;
                expression.Join<Forum>((t, f) => f.ID == t.ForumID)
                    .Join<Forum, Category>((f, c) => c.ID == f.CategoryID)
                    .Where<Topic, Category>((t, c) => t.ID == id.Value && c.BoardID == boardId && (c.Flags & 1) == 1)
                    .Select<Topic, Forum, Message>((t, f, m) => new { f.CategoryID, t.ForumID, });

                var result = this.GetRepository<ActiveAccess>().DbAccess
                    .Execute(db => db.Connection.SelectMulti<Topic, Forum, Category>(expression)).FirstOrDefault();

                if (result != null)
                {
                    categoryId = result.Item2.CategoryID;
                    category = result.Item3;

                    forumId = result.Item1.ForumID;
                    forum = result.Item2;

                    topic = result.Item1;
                }
                else
                {
                    topicId = null;
                }
            }

            if (forumId.HasValue && !categoryId.HasValue)
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                var id = forumId;
                expression.Join<Category>((f, c) => c.ID == f.CategoryID)
                    .Where<Forum, Category>((f, c) => f.ID == id.Value && c.BoardID == boardId);

                var result = this.GetRepository<ActiveAccess>().DbAccess
                    .Execute(db => db.Connection.Single<Category>(expression));

                if (result != null)
                {
                    category = result;
                }
                else
                {
                    forumId = null;
                }
            }

            // -- get previous visit
            if (!isGuest)
            {
                previousVisit = currentUser.LastVisit;

                if (forumId is null or 0)
                {
                    // -- update last visit
                    this.GetRepository<User>().UpdateOnly(
                        () => new User { LastVisit = DateTime.UtcNow, IP = ip },
                        u => u.ID == userId);
                }
            }

            if (!doNotTrack)
            {
                if (this.GetRepository<Active>().Exists(
                        x => x.BoardID == boardId && x.SessionID == sessionId ||
                             x.Browser == browser && (x.Flags & 8) == 8))
                {
                    if (!isCrawler)
                    {
                        // -- user is not a crawler - use his session id
                        this.GetRepository<Active>().UpdateOnly(
                            () => new Active
                                      {
                                          UserID = userId,
                                          IP = ip,
                                          LastActive = DateTime.UtcNow,
                                          Location = location,
                                          ForumID = forumId,
                                          TopicID = topicId,
                                          Browser = browser,
                                          Platform = platform,
                                          ForumPage = forumPage,
                                          Flags = activeFlags.BitValue
                                      },
                            a => a.SessionID == sessionId && a.BoardID == boardId);
                    }
                    else
                    {
                        // -- search crawler by other parameters then session id
                        this.GetRepository<Active>().UpdateOnly(
                            () => new Active
                                      {
                                          UserID = userId,
                                          IP = ip,
                                          LastActive = DateTime.UtcNow,
                                          Location = location,
                                          ForumID = forumId,
                                          TopicID = topicId,
                                          Browser = browser,
                                          Platform = platform,
                                          ForumPage = forumPage,
                                          Flags = activeFlags.BitValue
                                      },
                            a => a.Browser == browser && a.IP == ip && a.BoardID == boardId);
                    }
                }
                else
                {
                    // -- we set @ActiveFlags ready flags
                    this.GetRepository<Active>().Insert(
                        new Active
                            {
                                UserID = userId,
                                SessionID = sessionId,
                                BoardID = boardId,
                                IP = ip,
                                Login = DateTime.UtcNow,
                                LastActive = DateTime.UtcNow,
                                Location = location,
                                ForumID = forumId,
                                TopicID = topicId,
                                Browser = browser,
                                Platform = platform,
                                Flags = activeFlags.BitValue
                            });

                    // -- update max user stats
                    this.GetRepository<Registry>().UpdateMaxStats(boardId);

                    // -- parameter to update active users cache if this is a new user
                    if (!isGuest)
                    {
                        activeUpdate = true;
                    }
                }

                // -- remove duplicate users
                if (!isGuest)
                {
                    this.GetRepository<Active>().Delete(
                        x => x.UserID == userId && x.BoardID == boardId && x.SessionID != sessionId);
                }
            }

            // -- return information
            var pageLoad = this.GetRepository<ActiveAccess>().DbAccess.Execute(
                db =>
                    {
                        var expression = OrmLiteConfig.DialectProvider.SqlExpression<ActiveAccess>();

                        if (forumId.HasValue)
                        {
                            expression.Where<ActiveAccess>(x => x.UserID == userId && x.ForumID == forumId.Value);
                        }
                        else
                        {
                            expression.Where<ActiveAccess>(x => x.UserID == userId && x.ForumID == 0);
                        }

                        // -- is Moderator Any
                        var isModeratorAnyExpression = OrmLiteConfig.DialectProvider.SqlExpression<ActiveAccess>();

                        isModeratorAnyExpression.Where(x => x.UserID == userId && x.ModeratorAccess);

                        var isModeratorAnySql = isModeratorAnyExpression.Select(Sql.Count("1"))
                            .ToMergedParamsSelectStatement();

                        // -- Upload Access
                        var uploadAccessExpression = OrmLiteConfig.DialectProvider.SqlExpression<UserGroup>();

                        var uploadAccessSql = uploadAccessExpression
                            .Join<Group>((a, b) => b.ID == a.GroupID && (b.Flags & 64) == 64)
                            .Where<UserGroup>(a => a.UserID == userId).Select(Sql.Count("1"))
                            .ToMergedParamsSelectStatement();

                        // -- Download Access
                        var downloadAccessExpression = OrmLiteConfig.DialectProvider.SqlExpression<UserGroup>();

                        var downloadAccessSql = downloadAccessExpression
                            .Join<Group>((a, b) => b.ID == a.GroupID && (b.Flags & 128) == 128)
                            .Where<UserGroup>(a => a.UserID == userId).Select(Sql.Count("1"))
                            .ToMergedParamsSelectStatement();

                        expression.Select<ActiveAccess>(
                            x => new
                                     {
                                         ActiveUpdate = activeUpdate,
                                         PreviousVisit = previousVisit,
                                         x.BoardID,
                                         x.DeleteAccess,
                                         x.EditAccess,
                                         x.ForumID,
                                         x.IsAdmin,
                                         x.IsForumModerator,
                                         x.IsGuestX,
                                         x.LastActive,
                                         x.ModeratorAccess,
                                         x.PollAccess,
                                         x.PostAccess,
                                         x.PriorityAccess,
                                         x.ReadAccess,
                                         x.ReplyAccess,
                                         x.UserID,
                                         x.VoteAccess,
                                         IsModeratorAny =
                                             Sql.Custom(
                                                 $"sign({OrmLiteConfig.DialectProvider.IsNullFunction(isModeratorAnySql, 0)})"),
                                         IsCrawler = isCrawler,
                                         GuestUserId = guestUser.ID,
                                         UploadAccess =
                                             Sql.Custom(
                                                 $"sign({OrmLiteConfig.DialectProvider.IsNullFunction(uploadAccessSql, 0)})"),
                                         DownloadAccess = Sql.Custom(
                                             $"sign({OrmLiteConfig.DialectProvider.IsNullFunction(downloadAccessSql, 0)})")
                                     });

                        return db.Connection.Single<PageLoad>(expression);
                    });

            return Tuple.Create(pageLoad, currentUser, category, forum, topic, message);
        }
    }

    /// <summary>
    ///     Get a simple forum/topic listing.
    /// </summary>
    /// <param name="boardId"> The board Id. </param>
    /// <param name="userId"> The user Id. </param>
    /// <param name="timeFrame"> The time Frame. </param>
    /// <param name="maxCount"> The max Count. </param>
    /// <returns> The get simple forum topic. </returns>
    public List<SimpleForum> GetSimpleForumTopic(int boardId, int userId, DateTime timeFrame, int maxCount)
    {
        // If the user is not logged in (Active Access Table is empty), we need to make sure the Active Access Tables are set
        this.GetRepository<ActiveAccess>().InsertPageAccess(boardId, userId, false);

        var forumData = this.GetRepository<Forum>().ListAllWithAccess(boardId, userId)
            .Select(x => new SimpleForum { ForumID = x.Item1.ID, Name = x.Item1.Name }).ToList();

        // get topics for all forums...
        forumData.ForEach(forum =>
            {
                var forum1 = forum;

                // add topics
                var topics =
                    this.GetRepository<Topic>().ListPaged(
                        forum1.ForumID,
                        userId,
                        timeFrame,
                        0,
                        maxCount,
                        false);

                // filter first...
                forum.Topics =
                    topics.Where(x => x.LastPosted >= timeFrame)
                        .Select(x => this.LoadSimpleTopic(x, forum1))
                        .ToList();
            });

        return forumData;
    }

    /// <summary>
    ///     Get all moderators by Groups and User
    /// </summary>
    /// <returns> Returns the Moderator List </returns>
    public List<SimpleModerator> GetModerators()
    {
        return this.DataCache.GetOrSet(
            Constants.Cache.ForumModerators,
            () => this.GetRepository<User>().GetForumModerators(),
            TimeSpan.FromMinutes(this.Get<BoardSettings>().BoardModeratorsCacheTimeout));
    }

    /// <summary>
    /// The load simple topic.
    /// </summary>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <returns>
    /// Returns the simple topic.
    /// </returns>
    [NotNull]
    private SimpleTopic LoadSimpleTopic([NotNull] PagedTopic topic, [NotNull] SimpleForum forum)
    {
        CodeContracts.VerifyNotNull(forum);

        return new SimpleTopic
                   {
                       TopicID = topic.TopicID,
                       CreatedDate = topic.Posted,
                       Subject = topic.Subject,
                       StartedUserID = topic.UserID,
                       StartedUserName =
                           this.Get<BoardSettings>().EnableDisplayName ? topic.StarterDisplay : topic.Starter,
                       Replies = topic.Replies,
                       LastPostDate = topic.LastPosted.Value,
                       LastUserID = topic.LastUserID.Value,
                       LastUserName =
                           this.Get<BoardSettings>().EnableDisplayName ? topic.LastUserDisplayName : topic.LastUserName,
                       LastMessageID = topic.LastMessageID.Value,
                       FirstMessage = topic.FirstMessage,
                       LastMessage = this.GetRepository<Message>().GetById(topic.LastMessageID.Value).MessageText,
                       Forum = forum
                   };
    }
}