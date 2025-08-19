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

using System.Collections.Generic;
using System.Threading.Tasks;

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
    /// <param name="boardDescription">
    /// Description of the board.
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
    /// The username.
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
    public async static Task<int> CreateAsync(
        this IRepository<Board> repository,
        string boardName,
        string boardDescription,
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
        var newBoardId = await repository.InsertAsync(new Board { Name = boardName, Description = boardDescription });

        BoardContext.Current.GetRepository<Registry>().Save("culture", culture);
        BoardContext.Current.GetRepository<Registry>().Save("language", languageFile);

        // -- Rank
        var rankIdAdmin = await BoardContext.Current.GetRepository<Rank>().InsertAsync(
            new Rank
                {
                    BoardID = newBoardId,
                    Name = "Administration",
                    Flags = 0,
                    MinPosts = 0,
                    Style = "color: #811334",
                    SortOrder = 0
                });

        var rankIdGuest = await BoardContext.Current.GetRepository<Rank>().InsertAsync(
            new Rank
                {
                    BoardID = newBoardId,
                    Name = "Guest",
                    Flags = 0,
                    MinPosts = 0,
                    SortOrder = 100
                });

        await BoardContext.Current.GetRepository<Rank>().InsertAsync(
            new Rank
                {
                    BoardID = newBoardId,
                    Name = "Newbie",
                    Flags = 1,
                    MinPosts = 3,
                    SortOrder = 3
                });

        await BoardContext.Current.GetRepository<Rank>().InsertAsync(
            new Rank
                {
                    BoardID = newBoardId,
                    Name = "Member",
                    Flags = 2,
                    MinPosts = 10,
                    SortOrder = 2
                });

        await BoardContext.Current.GetRepository<Rank>().InsertAsync(
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

        var accessMaskIdAdmin = await BoardContext.Current.GetRepository<AccessMask>().InsertAsync(
            new AccessMask { BoardID = newBoardId, Name = "Admin Access", Flags = adminAccessFlag.BitValue, SortOrder = 4 });

        await BoardContext.Current.GetRepository<AccessMask>().InsertAsync(
            new AccessMask { BoardID = newBoardId, Name = "Moderator Access", Flags = adminAccessFlag.BitValue, SortOrder = 3 });

        var accessMaskIdMember = await BoardContext.Current.GetRepository<AccessMask>().InsertAsync(
            new AccessMask { BoardID = newBoardId, Name = "Member Access", Flags = 423, SortOrder = 2 });

        var accessMaskIdReadOnly = await BoardContext.Current.GetRepository<AccessMask>().InsertAsync(
            new AccessMask { BoardID = newBoardId, Name = "Read Only Access", Flags = 1, SortOrder = 1 });

        await BoardContext.Current.GetRepository<AccessMask>().InsertAsync(
            new AccessMask { BoardID = newBoardId, Name = "No Access", Flags = 0, SortOrder = 0 });

        // -- Group
        var adminGroupFlag = new GroupFlags
                                 {
                                     IsAdmin = true,
                                     AllowUpload = true,
                                     AllowDownload = true
                                 };

        var groupIdAdmin = await BoardContext.Current.GetRepository<Group>().InsertAsync(
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

        var groupIdGuest = await BoardContext.Current.GetRepository<Group>().InsertAsync(
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

        var groupIdMember = await BoardContext.Current.GetRepository<Group>().InsertAsync(
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
        var userIdGuest = await BoardContext.Current.GetRepository<User>().InsertAsync(
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

        adminUser.ID = await BoardContext.Current.GetRepository<User>().InsertAsync(adminUser);

        // -- UserGroup
        await BoardContext.Current.GetRepository<UserGroup>().InsertAsync(
            new UserGroup { UserID = adminUser.ID, GroupID = groupIdAdmin });

        await BoardContext.Current.GetRepository<UserGroup>().InsertAsync(
            new UserGroup { UserID = userIdGuest, GroupID = groupIdGuest });

        var categoryFlags = new CategoryFlags {IsActive = true};

        // -- Category
        var categoryId = await BoardContext.Current.GetRepository<Category>().InsertAsync(
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

        forum.ID = await BoardContext.Current.GetRepository<Forum>().InsertAsync(
            forum);

        // -- ForumAccess
        await BoardContext.Current.GetRepository<ForumAccess>().InsertAsync(
            new ForumAccess { GroupID = groupIdAdmin, ForumID = forum.ID, AccessMaskID = accessMaskIdAdmin });

        await BoardContext.Current.GetRepository<ForumAccess>().InsertAsync(
            new ForumAccess { GroupID = groupIdGuest, ForumID = forum.ID, AccessMaskID = accessMaskIdReadOnly });

        await BoardContext.Current.GetRepository<ForumAccess>().InsertAsync(
            new ForumAccess { GroupID = groupIdMember, ForumID = forum.ID, AccessMaskID = accessMaskIdMember });

        await BoardContext.Current.Get<IRaiseEventAsync>().RaiseAsync(new UpdateUserStylesEvent(newBoardId));

        // -- Create Welcome Topic
        var messageFlags = new MessageFlags
                               {
                                   IsHtml = false, IsBBCode = true, IsPersistent = true, IsApproved = true
                               };

        await BoardContext.Current.GetRepository<Topic>().SaveNewAsync(
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
            messageFlags);

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
    public async static Task<BoardStat> PostStatsAsync(this IRepository<Board> repository, int boardId, bool showNoCountPosts)
    {
        var data = await repository.DbAccess.ExecuteAsync(
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
                    var countPostsExpression = db.From<Message>();

                    countPostsExpression.Join<Topic>((a, b) => b.ID == a.TopicID)
                        .Join<Topic, Forum>((b, c) => c.ID == b.ForumID)
                        .Join<Forum, Category>((b, c) => c.ID == b.CategoryID);

                    countPostsExpression.Where<Category>(c => c.BoardID == boardId && (c.Flags & 1) == 1);

                    var countPostsSql = countPostsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Topics
                    var countTopicsExpression = db.From<Topic>();

                    countTopicsExpression.Join<Forum>((a, b) => b.ID == a.ForumID)
                        .Join<Forum, Category>((b, c) => c.ID == b.CategoryID);

                    countTopicsExpression.Where<Category>(c => c.BoardID == boardId && (c.Flags & 1) == 1);

                    var countTopicsSql = countTopicsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Forums
                    var countForumsExpression = db.From<Forum>();

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

                    return db.SingleAsync<BoardStat>(expression);
                });

        return data ?? await repository.StatsAsync(boardId);
    }

    /// <summary>
    /// Save Board Settings (Name, Culture and Language File)
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="boardId">The board id.</param>
    /// <param name="name">The name.</param>
    /// <param name="description">Description of the board.</param>
    /// <param name="languageFile">The language file.</param>
    /// <param name="culture">The culture.</param>
    public static void Save(
        this IRepository<Board> repository,
        int boardId,
        string name,
        string description,
        string languageFile,
        string culture)
    {
        BoardContext.Current.GetRepository<Registry>().Save("culture", culture, boardId);
        BoardContext.Current.GetRepository<Registry>().Save("language", languageFile, boardId);

        repository.UpdateOnly(() => new Board { Name = name, Description = description }, board => board.ID == boardId);
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
    public static Task<BoardStat> StatsAsync(this IRepository<Board> repository, int boardId)
    {
        return repository.DbAccess.ExecuteAsync(
            db =>
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                expression.Where(u => u.BoardID == boardId);

                // -- count Posts
                var countPostsExpression = db.From<Message>();

                countPostsExpression.Join<Topic>((a, b) => b.ID == a.TopicID && (a.Flags & 8) != 8)
                    .Join<Topic, Forum>((b, c) => c.ID == b.ForumID)
                    .Join<Forum, Category>((b, c) => c.ID == b.CategoryID);

                countPostsExpression.Where<Category>(c => c.BoardID == boardId && (c.Flags & 1) == 1);

                var countPostsSql = countPostsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                // -- count Topics
                var countTopicsExpression = db.From<Topic>();

                countTopicsExpression.Join<Forum>((a, b) => b.ID == a.ForumID && (a.Flags & 8) != 8 && a.NumPosts > 0)
                    .Join<Forum, Category>((b, c) => c.ID == b.CategoryID && (c.Flags & 1) == 1);

                countTopicsExpression.Where<Category>(c => c.BoardID == boardId);

                var countTopicsSql = countTopicsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                // -- count Forums
                var countForumsExpression = db.From<Forum>();

                countForumsExpression.Join<Category>((a, b) => b.ID == a.CategoryID);

                countForumsExpression.Where<Category>(x => x.BoardID == boardId);

                var countForumsSql = countForumsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                // -- count Users
                var countUsersExpression = expression;

                countUsersExpression.Where(u => (u.Flags & 2) == 2 && u.BoardID == boardId);

                var countUsersSql = countUsersExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                // -- count Categories
                var countCategoriesExpression = db.From<Category>();

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

                return db.SingleAsync<BoardStat>(expression);
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
    public async static Task DeleteBoardAsync(this IRepository<Board> repository, int boardId)
    {
        // --- Delete all forums of the board
        var forums = BoardContext.Current.GetRepository<Forum>().ListAll(boardId);

        foreach (var f in forums)
        {
            await BoardContext.Current.GetRepository<Forum>().DeleteAsync(f.Item2.ID);
        }

        // --- Delete user(s)
        var users = await BoardContext.Current.GetRepository<User>().GetAsync(u => u.BoardID == boardId);

        foreach (var user in users)
        {
            await BoardContext.Current.GetRepository<User>().DeleteAsync(user);
        }

        // --- Delete Group
        var groups = await BoardContext.Current.GetRepository<Group>().GetAsync(g => g.BoardID == boardId);

        foreach (var groupId in groups.Select(g => g.ID))
        {
            await BoardContext.Current.GetRepository<GroupMedal>().DeleteAsync(x => x.GroupID == groupId);
            await BoardContext.Current.GetRepository<ForumAccess>().DeleteAsync(x => x.GroupID == groupId);
            await BoardContext.Current.GetRepository<UserGroup>().DeleteAsync(x => x.GroupID == groupId);
            await BoardContext.Current.GetRepository<Group>().DeleteAsync(x => x.ID == groupId);
        }

        await BoardContext.Current.GetRepository<Category>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<ActiveAccess>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<Active>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<Rank>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<AccessMask>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<BBCode>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<Medal>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<ReplaceWords>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<SpamWords>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<BannedIP>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<BannedName>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<BannedEmail>().DeleteAsync(x => x.BoardID == boardId);
        await BoardContext.Current.GetRepository<Registry>().DeleteAsync(x => x.BoardID == boardId);

        await repository.DeleteAsync(x => x.ID == boardId);
    }

    /// <summary>
    /// Gets all existing board ids.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <returns>System.Threading.Tasks.Task&lt;System.Collections.Generic.List&lt;System.Int32&gt;&gt;.</returns>
    public static Task<List<int>> GetAllBoardIdsAsync(
            this IRepository<Board> repository)
    {
        return repository.DbAccess.ExecuteAsync(
            db =>
            {
                return db.ColumnAsync<int>(db.From<Board>().Select(x => x.ID));
            });
    }
}