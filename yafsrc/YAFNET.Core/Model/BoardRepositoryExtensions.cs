/* Yet Another Forum.NET
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

namespace YAF.Core.Model;

using System;

using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
///     The board repository extensions.
/// </summary>
public static class BoardRepositoryExtensions
{
    /// <summary>
    /// Creates the new Board.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardName">
    /// The board name.
    /// </param>
    /// <param name="boardEmail">
    /// The board Email.
    /// </param>
    /// <param name="culture">
    /// The culture.
    /// </param>
    /// <param name="languageFile">
    /// The language file.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="userEmail">
    /// The user email.
    /// </param>
    /// <param name="userKey">
    /// The user key.
    /// </param>
    /// <param name="isHostAdmin">
    /// The is host admin.
    /// </param>
    /// <param name="rolePrefix">
    /// The role prefix.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static int Create(
        this IRepository<Board> repository,
        string boardName,
        string boardEmail,
        string culture,
        string languageFile,
        string userName,
        string userEmail,
        string userKey,
        bool isHostAdmin,
        string rolePrefix)
    {
        // -- Board
        var newBoardId = repository.Insert(new Board { Name = boardName });

        BoardContext.Current.GetRepository<Registry>().Save("culture", culture);
        BoardContext.Current.GetRepository<Registry>().Save("language", languageFile);

        // -- Rank
        var rankIdAdmin = BoardContext.Current.GetRepository<Rank>().Insert(
            new Rank
                {
                    BoardID = newBoardId,
                    Name = "Administration",
                    Flags = 0,
                    MinPosts = 0,
                    Style = "color: #811334",
                    SortOrder = 0
                });

        var rankIdGuest = BoardContext.Current.GetRepository<Rank>().Insert(
            new Rank
                {
                    BoardID = newBoardId,
                    Name = "Guest",
                    Flags = 0,
                    MinPosts = 0,
                    SortOrder = 100
                });

        BoardContext.Current.GetRepository<Rank>().Insert(
            new Rank
                {
                    BoardID = newBoardId,
                    Name = "Newbie",
                    Flags = 1,
                    MinPosts = 3,
                    SortOrder = 3
                });

        BoardContext.Current.GetRepository<Rank>().Insert(
            new Rank
                {
                    BoardID = newBoardId,
                    Name = "Member",
                    Flags = 2,
                    MinPosts = 10,
                    SortOrder = 2
                });

        BoardContext.Current.GetRepository<Rank>().Insert(
            new Rank
                {
                    BoardID = newBoardId,
                    Name = "Advanced Member",
                    Flags = 2,
                    MinPosts = 30,
                    SortOrder = 1
                });

        // -- AccessMask
        var adminAccessFlag = new AccessFlags
                                  {
                                      DeleteAccess = true,
                                      EditAccess = true,
                                      ModeratorAccess = true,
                                      PollAccess = true,
                                      PostAccess = true,
                                      PriorityAccess = true,
                                      ReadAccess = true,
                                      ReplyAccess = true,
                                      VoteAccess = true
                                  };

        var accessMaskIdAdmin = BoardContext.Current.GetRepository<AccessMask>().Insert(
            new AccessMask { BoardID = newBoardId, Name = "Admin Access", Flags = adminAccessFlag.BitValue, SortOrder = 4 });

        BoardContext.Current.GetRepository<AccessMask>().Insert(
            new AccessMask { BoardID = newBoardId, Name = "Moderator Access", Flags = adminAccessFlag.BitValue, SortOrder = 3 });

        var accessMaskIdMember = BoardContext.Current.GetRepository<AccessMask>().Insert(
            new AccessMask { BoardID = newBoardId, Name = "Member Access", Flags = 423, SortOrder = 2 });

        var accessMaskIdReadOnly = BoardContext.Current.GetRepository<AccessMask>().Insert(
            new AccessMask { BoardID = newBoardId, Name = "Read Only Access", Flags = 1, SortOrder = 1 });

        BoardContext.Current.GetRepository<AccessMask>().Insert(
            new AccessMask { BoardID = newBoardId, Name = "No Access", Flags = 0, SortOrder = 0 });

        // -- Group
        var adminGroupFlag = new GroupFlags
                                 {
                                     IsAdmin = true,
                                     AllowUpload = true,
                                     AllowDownload = true
                                 };

        var groupIdAdmin = BoardContext.Current.GetRepository<Group>().Insert(
            new Group
                {
                    BoardID = newBoardId,
                    Name = $"{rolePrefix}Administrators",
                    Flags = adminGroupFlag.BitValue,
                    Style = "color: red",
                    SortOrder = 0,
                    UsrSigChars = 256,
                    UsrSigBBCodes = "URL,IMG,SPOILER,QUOTE",
                    UsrAlbums = 10,
                    UsrAlbumImages = 120
                });

        var guestGroupFlag = new GroupFlags
                                 {
                                     IsGuest = true
                                 };

        var groupIdGuest = BoardContext.Current.GetRepository<Group>().Insert(
            new Group
                {
                    BoardID = newBoardId,
                    Name = "Guests",
                    Flags = guestGroupFlag.BitValue,
                    Style = "font-style: italic; font-weight: bold; color: #0c7333",
                    SortOrder = 1,
                    UsrSigChars = 0,
                    UsrSigBBCodes = null,
                    UsrAlbums = 0,
                    UsrAlbumImages = 0
                });

        var memberGroupFlag = new GroupFlags
                                  {
                                      AllowDownload = true,
                                      IsStart = true
                                  };

        var groupIdMember = BoardContext.Current.GetRepository<Group>().Insert(
            new Group
                {
                    BoardID = newBoardId,
                    Name = $"{rolePrefix}Registered Users",
                    Flags = memberGroupFlag.BitValue,
                    SortOrder = 2,
                    UsrSigChars = 128,
                    UsrSigBBCodes = "URL,IMG,SPOILER,QUOTE",
                    UsrAlbums = 5,
                    UsrAlbumImages = 30
                });

        // -- User (GUEST)
        var userIdGuest = BoardContext.Current.GetRepository<User>().Insert(
            new User
                {
                    BoardID = newBoardId,
                    RankID = rankIdGuest,
                    Name = "Guest",
                    DisplayName = "Guest",
                    Joined = DateTime.Now,
                    LastVisit = DateTime.Now,
                    NumPosts = 0,
                    TimeZone = TimeZoneInfo.Local.Id,
                    Email = boardEmail,
                    Flags = 6,
                    PageSize = 5
                });

        var userFlags = 2;

        if (isHostAdmin)
        {
            userFlags = 3;
        }

        // -- User (ADMIN)
        var adminUser = new User
                            {
                                BoardID = newBoardId,
                                RankID = rankIdAdmin,
                                Name = userName,
                                DisplayName = userName,
                                ProviderUserKey = userKey,
                                Joined = DateTime.Now,
                                LastVisit = DateTime.Now,
                                NumPosts = 0,
                                TimeZone = TimeZoneInfo.Local.Id,
                                Email = userEmail,
                                Flags = userFlags,
                                PageSize = 5
                            };

        adminUser.ID = BoardContext.Current.GetRepository<User>().Insert(adminUser);

        // -- UserGroup
        BoardContext.Current.GetRepository<UserGroup>().Insert(
            new UserGroup { UserID = adminUser.ID, GroupID = groupIdAdmin });

        BoardContext.Current.GetRepository<UserGroup>().Insert(
            new UserGroup { UserID = userIdGuest, GroupID = groupIdGuest });

        var categoryFlags = new CategoryFlags {IsActive = true};

        // -- Category
        var categoryId = BoardContext.Current.GetRepository<Category>().Insert(
            new Category { BoardID = newBoardId, Name = "Test Category", SortOrder = 1, Flags = categoryFlags.BitValue});

        // -- Forum
        var forum = new Forum
                        {
                            CategoryID = categoryId,
                            Name = "Test Forum",
                            Description = "A test forum",
                            SortOrder = 1,
                            NumTopics = 0,
                            NumPosts = 0,
                            Flags = 4
                        };

        forum.ID = BoardContext.Current.GetRepository<Forum>().Insert(
            forum);

        // -- ForumAccess
        BoardContext.Current.GetRepository<ForumAccess>().Insert(
            new ForumAccess { GroupID = groupIdAdmin, ForumID = forum.ID, AccessMaskID = accessMaskIdAdmin });

        BoardContext.Current.GetRepository<ForumAccess>().Insert(
            new ForumAccess { GroupID = groupIdGuest, ForumID = forum.ID, AccessMaskID = accessMaskIdReadOnly });

        BoardContext.Current.GetRepository<ForumAccess>().Insert(
            new ForumAccess { GroupID = groupIdMember, ForumID = forum.ID, AccessMaskID = accessMaskIdMember });

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateUserStylesEvent(newBoardId));

        repository.FireNew(newBoardId);

        // -- Create Welcome Topic
        var messageFlags = new MessageFlags
                               {
                                   IsHtml = false, IsBBCode = true, IsPersistent = true, IsApproved = true
                               };

        BoardContext.Current.GetRepository<Topic>().SaveNew(
            forum,
            "🎉 Hello from YAF.NET",
            string.Empty,
            string.Empty,
            "Welcome to the New YAF.NET Installation",
            """
            😎 Thank you for installing YetAnotherForum.

            If you have any questions use our [url=https://yetanotherforum.net/forum/]Support Forum[/url] or if you found any issues or have problems use the GitHub Issue tracker on our [url=https://github.com/YAFNET/YAFNET/issues]Project Site[/url]
            """,
            adminUser,
            0,
            userName,
            userName,
            BoardContext.Current.Get<IHttpContextAccessor>().HttpContext.GetUserRealIPAddress(),
            DateTime.UtcNow,
            messageFlags,
            out _);

        return newBoardId;
    }

    /// <summary>
    /// Gets the post stats.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="showNoCountPosts">
    /// The show no count posts.
    /// </param>
    /// <returns>
    /// The <see cref="BoardStat"/>.
    /// </returns>
    public static BoardStat PostStats(this IRepository<Board> repository, int boardId, bool showNoCountPosts)
    {
        var data = repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

                    expression.Join<Topic>((a, b) => b.ID == a.TopicID).Join<Topic, Forum>((b, c) => c.ID == b.ForumID)
                        .Join<Forum, Category>((c, d) => d.ID == c.CategoryID).Join<User>((a, e) => e.ID == a.UserID);

                    expression.Where<Message, Topic, Forum, Category>(
                        (a, b, c, d) => (a.Flags & 16) == 16 && (b.Flags & 8) != 8 && d.BoardID == boardId && (d.Flags & 1) == 1);

                    if (!showNoCountPosts)
                    {
                        expression.And<Forum>(c => (c.Flags & 4) != 4);
                    }

                    expression.OrderByDescending(a => a.Posted).Limit(1);

                    // -- count Posts
                    var countPostsExpression = db.Connection.From<Message>();

                    countPostsExpression.Join<Topic>((a, b) => b.ID == a.TopicID)
                        .Join<Topic, Forum>((b, c) => c.ID == b.ForumID)
                        .Join<Forum, Category>((b, c) => c.ID == b.CategoryID);

                    countPostsExpression.Where<Category>(c => c.BoardID == boardId && (c.Flags & 1) == 1);

                    var countPostsSql = countPostsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Topics
                    var countTopicsExpression = db.Connection.From<Topic>();

                    countTopicsExpression.Join<Forum>((a, b) => b.ID == a.ForumID)
                        .Join<Forum, Category>((b, c) => c.ID == b.CategoryID);

                    countTopicsExpression.Where<Category>(c => c.BoardID == boardId && (c.Flags & 1) == 1);

                    var countTopicsSql = countTopicsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Forums
                    var countForumsExpression = db.Connection.From<Forum>();

                    countForumsExpression.Join<Category>((a, b) => b.ID == a.CategoryID);

                    countForumsExpression.Where<Category>(x => x.BoardID == boardId);

                    var countForumsSql = countForumsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    expression.Take(1).Select<Message, User>(
                        (a, e) => new
                                      {
                                          Posts = Sql.Custom<int>($"({countPostsSql})"),
                                          Topics = Sql.Custom<int>($"({countTopicsSql})"),
                                          Forums = Sql.Custom<int>($"({countForumsSql})"),
                                          LastPost = a.Posted,
                                          LastUserID = a.UserID,
                                          LastUser = e.Name,
                                          LastUserDisplayName = e.DisplayName,
                                          LastUserStyle = e.UserStyle,
                                          LastUserSuspended = e.Suspended
                                      });

                    return db.Connection.Single<BoardStat>(expression);
                });

        return data ?? repository.Stats(boardId);
    }

    /// <summary>
    /// Save Board Settings (Name, Culture and Language File)
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="boardId">The board id.</param>
    /// <param name="name">The name.</param>
    /// <param name="languageFile">The language file.</param>
    /// <param name="culture">The culture.</param>
    public static void Save(
        this IRepository<Board> repository,
        int boardId,
        string name,
        string languageFile,
        string culture)
    {
        BoardContext.Current.GetRepository<Registry>().Save("culture", culture, boardId);
        BoardContext.Current.GetRepository<Registry>().Save("language", languageFile, boardId);

        repository.UpdateOnly(() => new Board { Name = name }, board => board.ID == boardId);

        repository.FireUpdated(boardId);
    }

    /// <summary>
    /// The stats.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <returns>
    /// The <see cref="BoardStat"/>.
    /// </returns>
    public static BoardStat Stats(this IRepository<Board> repository, int boardId)
    {
        return repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    expression.Where(u => u.BoardID == boardId);

                    // -- count Posts
                    var countPostsExpression = db.Connection.From<Message>();

                    countPostsExpression.Join<Topic>((a, b) => b.ID == a.TopicID && (a.Flags & 8) != 8)
                        .Join<Topic, Forum>((b, c) => c.ID == b.ForumID)
                        .Join<Forum, Category>((b, c) => c.ID == b.CategoryID);

                    countPostsExpression.Where<Category>(c => c.BoardID == boardId && (c.Flags & 1) == 1);

                    var countPostsSql = countPostsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Topics
                    var countTopicsExpression = db.Connection.From<Topic>();

                    countTopicsExpression.Join<Forum>((a, b) => b.ID == a.ForumID && (a.Flags & 8) != 8 && a.NumPosts > 0)
                        .Join<Forum, Category>((b, c) => c.ID == b.CategoryID && (c.Flags & 1) == 1);

                    countTopicsExpression.Where<Category>(c => c.BoardID == boardId);

                    var countTopicsSql = countTopicsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Forums
                    var countForumsExpression = db.Connection.From<Forum>();

                    countForumsExpression.Join<Category>((a, b) => b.ID == a.CategoryID);

                    countForumsExpression.Where<Category>(x => x.BoardID == boardId);

                    var countForumsSql = countForumsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Users
                    var countUsersExpression = expression;

                    countUsersExpression.Where(u => (u.Flags & 2) == 2 && u.BoardID == boardId);

                    var countUsersSql = countUsersExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Categories
                    var countCategoriesExpression = db.Connection.From<Category>();

                    countCategoriesExpression.Where<Category>(x => x.BoardID == boardId);

                    var countCategoriesSql = countCategoriesExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    expression.Take(1).Select<User>(
                        x => new
                                 {
                                     Categories = Sql.Custom<int>($"({countCategoriesSql})"),
                                     Posts = Sql.Custom<int>($"({countPostsSql})"),
                                     Forums = Sql.Custom<int>($"({countForumsSql})"),
                                     Topics = Sql.Custom<int>($"({countTopicsSql})"),
                                     Users = Sql.Custom<int>($"({countUsersSql})"),
                                     BoardStart = Sql.Min(x.Joined)
                                 });

                    return db.Connection.Single<BoardStat>(expression);
                });
    }

    /// <summary>
    /// The delete board.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    public static void DeleteBoard(this IRepository<Board> repository, int boardId)
    {
        // --- Delete all forums of the board
        var forums = BoardContext.Current.GetRepository<Forum>().ListAll(boardId);

        forums.ForEach(f => BoardContext.Current.GetRepository<Forum>().Delete(f.Item2.ID));

        // --- Delete user(s)
        var users = BoardContext.Current.GetRepository<User>().Get(u => u.BoardID == boardId);

        users.ForEach(u => BoardContext.Current.GetRepository<User>().Delete(u));

        // --- Delete Group
        var groups = BoardContext.Current.GetRepository<Group>().Get(g => g.BoardID == boardId);

        groups.ForEach(
            g =>
                {
                    BoardContext.Current.GetRepository<GroupMedal>().Delete(x => x.GroupID == g.ID);
                    BoardContext.Current.GetRepository<ForumAccess>().Delete(x => x.GroupID == g.ID);
                    BoardContext.Current.GetRepository<UserGroup>().Delete(x => x.GroupID == g.ID);
                    BoardContext.Current.GetRepository<Group>().Delete(x => x.ID == g.ID);
                });

        BoardContext.Current.GetRepository<Category>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<ActiveAccess>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<Active>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<Rank>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<AccessMask>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<BBCode>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<Medal>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<ReplaceWords>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<SpamWords>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<BannedIP>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<BannedName>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<BannedEmail>().Delete(x => x.BoardID == boardId);
        BoardContext.Current.GetRepository<Registry>().Delete(x => x.BoardID == boardId);
        repository.Delete(x => x.ID == boardId);
    }
}