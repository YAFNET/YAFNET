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
    using System.Data.SqlClient;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Types.Objects.Model;

    #endregion

    /// <summary>
    ///     Class used for multi-step DB operations so they can be cached, etc.
    /// </summary>
    public class DataBroker : IHaveServiceLocator
    {
        #region Constructors and Destructors

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

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods and Operators

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
                this.BoardSettings.UseReadTrackingByDatabase);

            return new Tuple<List<SimpleModerator>, List<ForumRead>>(moderators, forums);
        }

        /// <summary>
        /// The Page Load as Data Row
        /// </summary>
        /// <param name="sessionID">The session id.</param>
        /// <param name="boardId">The board id.</param>
        /// <param name="userKey">The user key.</param>
        /// <param name="ip">The IP Address.</param>
        /// <param name="location">The location.</param>
        /// <param name="forumPage">The forum page name.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="categoryId">The category Id.</param>
        /// <param name="forumId">The forum Id.</param>
        /// <param name="topicId">The topic Id.</param>
        /// <param name="messageId">The message Id.</param>
        /// <param name="isCrawler">The is Crawler.</param>
        /// <param name="isMobileDevice">The browser is a mobile device.</param>
        /// <param name="doNotTrack">The do Not Track.</param>
        /// <returns>
        /// The <see cref="PageLoad" />.
        /// </returns>
        public PageLoad GetPageLoad(
            [NotNull] string sessionID,
            [NotNull] int boardId,
            [CanBeNull] string userKey,
            [NotNull] string ip,
            [NotNull] string location,
            [NotNull] string forumPage,
            [NotNull] string browser,
            [NotNull] string platform,
            [NotNull] int? categoryId,
            [NotNull] int? forumId,
            [NotNull] int? topicId,
            [NotNull] int? messageId,
            [NotNull] bool isCrawler,
            [NotNull] bool isMobileDevice,
            [NotNull] bool doNotTrack)
        {
            var tries = 0;
            while (true)
            {
                try
                {
                    int userId;
                    bool isGuest;
                    DateTime? previousVisit = null;
                    User currentUser = null;
                    var activeUpdate = false;

                    // -- set IsActiveNow ActiveFlag - it's a default
                    var activeFlags = 1;

                    // -- find a guest id should do it every time to be sure that guest access rights are in ActiveAccess table
                    var guest = this.GetRepository<User>().Get(u => u.BoardID == boardId && (u.Flags & 4) == 4);

                    if (guest == null)
                    {
                        throw new ApplicationException(
                            $"No candidates for a guest were found for the board {boardId}.");
                    }

                    if (guest.Count > 1)
                    {
                        throw new ApplicationException(
                            $"Found {guest.Count} possible guest users. There should be one and only one user marked as guest.");
                    }

                    if (userKey == null)
                    {
                        var guestUser = guest.FirstOrDefault();

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
                            activeFlags = activeFlags | 8;
                        }
                    }
                    else
                    {
                        currentUser = this.GetRepository<User>()
                            .GetSingle(u => u.BoardID == boardId && u.ProviderUserKey == userKey);
                        userId = currentUser.ID;

                        isGuest = false;

                        // make sure that registered users are not crawlers
                        isCrawler = false;

                        // -- set IsRegistered ActiveFlag
                        activeFlags = activeFlags | 4;
                    }

                    // -- Check valid ForumID
                    var forum = this.GetRepository<Forum>().GetById(forumId.Value);

                    if (forumId.HasValue && forum == null)
                    {
                        forumId = null;
                    }

                    // -- Check valid CategoryID
                    var category = this.GetRepository<Category>().GetById(categoryId.Value);

                    if (categoryId.HasValue && category == null)
                    {
                        categoryId = null;
                    }

                    // -- Check valid MessageID
                    var message = this.GetRepository<Message>().GetById(messageId.Value);

                    if (messageId.HasValue && message == null)
                    {
                        messageId = null;
                    }

                    // -- Check valid TopicID
                    var topic = this.GetRepository<Topic>().GetById(topicId.Value);

                    if (topicId.HasValue && topic == null)
                    {
                        topicId = null;
                    }

                    // -- find missing ForumID/TopicID
                    if (messageId.HasValue && (!forumId.HasValue || !categoryId.HasValue || !topicId.HasValue))
                    {
                        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

                        var id = messageId;
                        expression.Join<Topic>((m, t) => t.ID == m.TopicID)
                            .Join<Topic, Forum>((t, f) => f.ID == t.ForumID)
                            .Join<Forum, Category>((f, c) => c.ID == f.CategoryID)
                            .Where<Message, Category>((m, c) => m.ID == id.Value && c.BoardID == boardId);

                        var result = this.GetRepository<ActiveAccess>().DbAccess.Execute(
                                db => db.Connection.SelectMulti<Message, Topic, Forum, Category>(expression))
                            .FirstOrDefault();

                        categoryId = result.Item3.CategoryID;
                        category = result.Item4;

                        forumId = result.Item3.ID;
                        forum = result.Item3;

                        topicId = result.Item1.TopicID;
                        topic = result.Item2;
                    }

                    if (topicId.HasValue && (!categoryId.HasValue || !forumId.HasValue))
                    {
                        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

                        var id = topicId;
                        expression.Join<Forum>((t, f) => f.ID == t.ForumID)
                            .Join<Forum, Category>((f, c) => c.ID == f.CategoryID)
                            .Where<Topic, Category>((t, c) => t.ID == id.Value && c.BoardID == boardId)
                            .Select<Topic, Forum, Message>((t, f, m) => new { f.CategoryID, t.ForumID, });

                        var result = this.GetRepository<ActiveAccess>().DbAccess
                            .Execute(db => db.Connection.SelectMulti<Topic, Forum, Category>(expression))
                            .FirstOrDefault();

                        categoryId = result.Item2.CategoryID;
                        category = result.Item3;

                        forumId = result.Item1.ForumID;
                        forum = result.Item2;
                    }

                    if (forumId.HasValue && !categoryId.HasValue)
                    {
                        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                        var id = forumId;
                        expression.Join<Category>((f, c) => c.ID == f.CategoryID)
                            .Where<Forum, Category>((f, c) => f.ID == id.Value && c.BoardID == boardId);

                        category = this.GetRepository<ActiveAccess>().DbAccess
                            .Execute(db => db.Connection.Select<Category>(expression)).FirstOrDefault();
                        categoryId = category.ID;
                    }

                    var accessList = this.GetRepository<vaccess>().Get(x => x.UserID == userId);

                    if (!this.GetRepository<ActiveAccess>().Exists(a => a.UserID == userId))
                    {
                        accessList.ForEach(
                            access => this.GetRepository<ActiveAccess>().Insert(
                                new ActiveAccess
                                {
                                    UserID = userId,
                                    BoardID = boardId,
                                    ForumID = access.ForumID,
                                    IsAdmin = access.IsAdmin,
                                    IsForumModerator = access.IsForumModerator,
                                    IsModerator = access.IsModerator,
                                    IsGuestX = isGuest,
                                    LastActive = DateTime.UtcNow,
                                    ReadAccess = access.ReadAccess,
                                    PostAccess = access.PostAccess,
                                    ReplyAccess = access.ReplyAccess,
                                    PriorityAccess = access.PriorityAccess,
                                    PollAccess = access.PollAccess,
                                    VoteAccess = access.VoteAccess,
                                    ModeratorAccess = access.ModeratorAccess,
                                    EditAccess = access.EditAccess,
                                    DeleteAccess = access.DeleteAccess,
                                    UploadAccess = access.UploadAccess,
                                    DownloadAccess = access.DownloadAccess
                                }));
                    }

                    var idForum2 = forumId ?? 0;

                    if (this.GetRepository<ActiveAccess>().Exists(
                        x => x.UserID == userId && x.ForumID == idForum2 &&
                            idForum2 == 0 || x.ReadAccess))
                    {
                        // -- verify that there's not the sane session for other board and drop it if required. Test code for portals with many boards
                        this.GetRepository<Active>().Delete(
                            x => x.SessionID == sessionID && (x.BoardID != boardId || x.UserID != userId));

                        // -- get previous visit
                        if (!isGuest)
                        {
                            previousVisit = currentUser.LastVisit;

                            // -- update last visit
                            this.GetRepository<User>().UpdateOnly(
                                () => new User { LastVisit = DateTime.UtcNow, IP = ip },
                                u => u.ID == userId);
                        }
                    }

                    if (!doNotTrack)
                    {
                        if (this.GetRepository<Active>().Exists(
                            x => x.BoardID == boardId && x.SessionID == sessionID ||
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
                                        Flags = activeFlags
                                    },
                                    a => a.SessionID == sessionID && a.BoardID == boardId);
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
                                        Flags = activeFlags
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
                                    SessionID = sessionID,
                                    BoardID = boardId,
                                    IP = ip,
                                    Login = DateTime.UtcNow,
                                    LastActive = DateTime.UtcNow,
                                    Location = location,
                                    ForumID = forumId,
                                    TopicID = topicId,
                                    Browser = browser,
                                    Platform = platform,
                                    Flags = activeFlags
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
                                x => x.UserID == userId && x.BoardID == boardId && x.SessionID != sessionID);
                        }
                    }

                    // -- return information
                    return this.GetRepository<ActiveAccess>().DbAccess.Execute(
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
                                    x.DownloadAccess,
                                    x.PollAccess,
                                    x.PostAccess,
                                    x.PriorityAccess,
                                    x.ReadAccess,
                                    x.ReplyAccess,
                                    x.UploadAccess,
                                    x.UserID,
                                    x.VoteAccess,
                                    IsModeratorAny = Sql.Custom($"sign({OrmLiteConfig.DialectProvider.IsNullFunction(isModeratorAnySql, 0)})"),
                                    IsCrawler = isCrawler,
                                    IsMobileDevice = isMobileDevice,
                                    CategoryID = Sql.Custom($"{(category != null ? category.ID : 0)}"),
                                    CategoryName =
                                        Sql.Custom($"{(category != null ? $"'{category.Name}'" : "NULL")}"),
                                    ForumName = Sql.Custom($"{(forum != null ? $"'{forum.Name}'" : "NULL")}"),
                                    TopicID = Sql.Custom($"{(topic != null ? topic.ID : 0)}"),
                                    TopicName =
                                        Sql.Custom($"{(topic != null ? $"'{topic.TopicName}'" : "NULL")}"),
                                    ForumTheme = Sql.Custom($"{(forum != null && forum.ThemeURL.IsSet() ? $"'{forum.ThemeURL}'" : "NULL")}"),
                                    ParentForumID = Sql.Custom($"{(forum != null && forum.ParentID.HasValue ? $"'{forum.ParentID}'" : "NULL")}")
                                });

                            return db.Connection.Select<PageLoad>(expression);
                        }).FirstOrDefault();
                }
                catch (SqlException x)
                {
                    if (x.Number == 1205 && tries < 3)
                    {
                        // Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
                    }
                    else
                    {
                        throw new ApplicationException(
                            $"Sql Exception with error number {x.Number} (Tries={tries})",
                            x);
                    }
                }

                ++tries;
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
            var forumData = this.GetRepository<Forum>().ListAllWithAccess(boardId, userId)
                .Select(x => new SimpleForum { ForumID = x.Item1.ID, Name = x.Item1.Name }).ToList();

            if (forumData.Any())
            {
                // If the user is not logged in (Active Access Table is empty), we need to make sure the Active Access Tables are set
                this.GetRepository<ActiveAccess>().InsertPageAccess(boardId, userId, false);

                forumData = this.GetRepository<Forum>().ListAllWithAccess(boardId, userId)
                    .Select(x => new SimpleForum { ForumID = x.Item1.ID, Name = x.Item1.Name }).ToList();
            }

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
                        DateTime.UtcNow,
                        0,
                        maxCount,
                        false,
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

        #endregion

        #region Methods

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

        #endregion
    }
}