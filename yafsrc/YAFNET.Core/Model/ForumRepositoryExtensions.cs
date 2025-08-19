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

using System.Threading.Tasks;

namespace YAF.Core.Model;

using System;
using System.Collections.Generic;
using System.Web;

using YAF.Types.Constants;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The Forum Repository Extensions
/// </summary>
public static class ForumRepositoryExtensions
{
    /// <summary>
    /// Saves a Forum or if forumId is null creates a new Forum
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="categoryId">
    /// The category id.
    /// </param>
    /// <param name="parentId">
    /// The parent id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="sortOrder">
    /// The sort order.
    /// </param>
    /// <param name="locked">
    /// The locked.
    /// </param>
    /// <param name="hidden">
    /// The hidden.
    /// </param>
    /// <param name="isTest">
    /// The is test.
    /// </param>
    /// <param name="moderated">
    /// The moderated.
    /// </param>
    /// <param name="moderatedPostCount">
    /// The moderated post count.
    /// </param>
    /// <param name="isModeratedNewTopicOnly">
    /// The is moderated new topic only.
    /// </param>
    /// <param name="remoteUrl">
    /// The remote url.
    /// </param>
    /// <param name="themeUrl">
    /// The theme url.
    /// </param>
    /// <param name="imageUrl">
    /// The image url.
    /// </param>
    /// <param name="styles">
    /// The styles.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static int Save(
        this IRepository<Forum> repository,
        int? forumId,
        int categoryId,
        int? parentId,
        string name,
        string description,
        int sortOrder,
        bool locked,
        bool hidden,
        bool isTest,
        bool moderated,
        int? moderatedPostCount,
        bool isModeratedNewTopicOnly,
        string remoteUrl,
        string themeUrl,
        string imageUrl,
        string styles)
    {
        if (parentId is 0)
        {
            parentId = null;
        }

        if (remoteUrl.IsNotSet())
        {
            remoteUrl = null;
        }

        var flags = new ForumFlags
                        {
                            IsLocked = locked, IsHidden = hidden, IsTest = isTest, IsModerated = moderated
                        };

        if (!forumId.HasValue)
        {
            var newForumId = repository.Insert(
                new Forum
                    {
                        ParentID = parentId,
                        Name = name,
                        Description = description,
                        SortOrder = sortOrder,
                        CategoryID = categoryId,
                        RemoteURL = remoteUrl,
                        ThemeURL = themeUrl,
                        ImageURL = imageUrl,
                        Styles = styles,
                        Flags = flags.BitValue,
                        ModeratedPostCount = moderatedPostCount,
                        IsModeratedNewTopicOnly = isModeratedNewTopicOnly
                    });

            return newForumId;
        }

        repository.UpdateOnly(
            () => new Forum
                      {
                          ParentID = parentId,
                          Name = name,
                          Description = description,
                          SortOrder = sortOrder,
                          CategoryID = categoryId,
                          RemoteURL = remoteUrl,
                          ThemeURL = themeUrl,
                          ImageURL = imageUrl,
                          Styles = styles,
                          Flags = flags.BitValue,
                          ModeratedPostCount = moderatedPostCount,
                          IsModeratedNewTopicOnly = isModeratedNewTopicOnly
                      },
            f => f.ID == forumId);

        return forumId.Value;
    }

    /// <summary>
    /// Checks if selected forum has already sub forums
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool IsParentsChecker(this IRepository<Forum> repository, int forumId)
    {
        return repository.Exists(f => f.ParentID == forumId);
    }

    /// <summary>
    /// Lists all forums by Board Id
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <returns>
    /// Returns all Forums for the selected Board Id
    /// </returns>
    public static List<Tuple<Category,Forum>> ListAll(
        this IRepository<Forum> repository,
        int boardId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Category>();

        expression.LeftJoin<Category, Forum>((category, forum) => category.ID == forum.CategoryID)
            .Where<Forum, Category>((forum, category) => category.BoardID == boardId)
            .OrderBy<Category>(c => c.SortOrder).ThenBy<Forum>(f => f.SortOrder).ThenBy<Category>(c => c.ID)
            .ThenBy<Forum>(f => f.ID);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Category, Forum>(expression));
    }

    /// <summary>
    /// Lists all forums accessible to a user
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// Returns all forums accessible to a user
    /// </returns>
    public static List<Tuple<Forum, Category, ActiveAccess>> ListAllWithAccess(
        this IRepository<Forum> repository,
        int boardId,
        int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

        expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
            .Join<ActiveAccess>((forum, active) => active.ForumID == forum.ID)
            .Where<Forum, Category, ActiveAccess>(
                (forum, category, active) =>
                    active.UserID == userId && category.BoardID == boardId && active.ReadAccess && forum.RemoteURL == null && (category.Flags & 1) == 1)
            .OrderBy<Category>(c => c.SortOrder).ThenBy<Forum>(f => f.SortOrder).ThenBy<Category>(c => c.ID)
            .ThenBy<Forum>(f => f.ID);

        return repository.DbAccess.Execute(
            db => db.Connection.SelectMulti<Forum, Category, ActiveAccess>(expression));
    }

    /// <summary>
    /// Lists all forums within a given category
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="categoryId">
    /// The category ID.
    /// </param>
    /// <returns>
    /// Returns Sorted Forums
    /// </returns>
    public static List<ForumSorted> ListAllFromCategory(
        this IRepository<Forum> repository,
        int categoryId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

        expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
            .Where<Forum, Category>(
                (forum, category) => category.BoardID == repository.BoardID && category.ID == categoryId && (category.Flags & 1) == 1
                                     && forum.ParentID == null).OrderBy<Forum>(f => f.SortOrder)
            .ThenBy<Forum>(f => f.ID);

        var list = repository.DbAccess.Execute(db => db.Connection.Select(expression));

        var sortedList = new List<ForumSorted>();

        list.ForEach(
            forum =>
                {
                    // import the row into the destination
                    var item = new ForumSorted { ForumID = forum.ID, Forum = forum.Name, Icon = "comments" };

                    sortedList.Add(item);
                });

        return sortedList;
    }

    /// <summary>
    /// Gets the forum list all sorted, with Access and paged, by search term
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="searchTerm">
    /// Filter Forums by Search Term
    /// </param>
    /// <returns>
    /// Returns all Sorted Forums
    /// </returns>
    public static List<SelectGroup> ListAllSorted(
        this IRepository<Forum> repository,
        int boardId,
        int userId,
        string searchTerm)
    {
        var list = BoardContext.Current.Get<IDataCache>().GetOrSet(
            string.Format(Constants.Cache.ForumJump, BoardContext.Current.PageUserID.ToString()),
            () => repository.ListAllWithAccess(boardId, userId),
            TimeSpan.FromMinutes(5));

        return repository.SortList(list.Where(x =>
            x.Item1.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)));
    }

    /// <summary>
    /// Gets the forum list all sorted, with Access and paged, by search term
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="forumId">
    /// Filter Forums by forum Id
    /// </param>
    /// <returns>
    /// Return the selected forum as Select Group
    /// </returns>
    public static List<SelectGroup> ListAllSorted(
        this IRepository<Forum> repository,
        int boardId,
        int userId,
        int forumId)
    {
        var list = BoardContext.Current.Get<IDataCache>().GetOrSet(
            string.Format(Constants.Cache.ForumJump, BoardContext.Current.PageUserID.ToString()),
            () => repository.ListAllWithAccess(boardId, userId),
            TimeSpan.FromMinutes(5));

        return repository.SortList(list.Where(x => x.Item1.ID == forumId));
    }

    /// <summary>
    /// Gets the forum list all sorted, with Access and paged.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="pageIndex">
    /// The page index
    /// </param>
    /// <param name="pageSize">
    /// The page size
    /// </param>
    /// <param name="pager">
    /// the pager
    /// </param>
    /// <returns>
    /// Returns Paged list of forums
    /// </returns>
    public static List<SelectGroup> ListAllSorted(
        this IRepository<Forum> repository,
        int boardId,
        int userId,
        int pageIndex,
        int pageSize,
        out Paging pager)
    {
        pager = new Paging { CurrentPageIndex = pageIndex, PageSize = pageSize };

        var forums = BoardContext.Current.Get<IDataCache>().GetOrSet(
            string.Format(Constants.Cache.ForumJump, BoardContext.Current.PageUserID.ToString()),
            () => repository.ListAllWithAccess(boardId, userId),
            TimeSpan.FromMinutes(5));

        var list = forums.GetPaged(pager);

        return repository.SortList(list);
    }

    /// <summary>
    /// Lists all forums
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The Board ID
    /// </param>
    /// <param name="userId">
    /// The user ID.
    /// </param>
    /// <param name="categoryId">
    /// The category ID.
    /// </param>
    /// <param name="parentId">
    /// The Parent ID.
    /// </param>
    /// <param name="findLastRead">
    /// Indicates if the Table should Contain the last Access Date
    /// </param>
    /// <param name="pageIndex">
    /// The Current Page Index
    /// </param>
    /// <param name="pageSize">
    /// The Number of items to retrieve.
    /// </param>
    /// <returns>
    /// Returns List of Forum Read
    /// </returns>
    public static List<ForumRead> ListRead(
        this IRepository<Forum> repository,
        int boardId,
        int userId,
        int? categoryId,
        int? parentId,
        bool findLastRead,
        int pageIndex,
        int pageSize)
    {
        return repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<Category>();

                    var countViewsExpression = db.Connection.From<Active>(db.Connection.TableAlias("x"));
                    countViewsExpression.Where(
                        $"""
                         x.{countViewsExpression.Column<Active>(x => x.ForumID)}=
                                                            {expression.Column<Forum>(x => x.ID, true)}
                         """);
                    var countViewsSql = countViewsExpression.Select(Sql.Count("1")).ToSelectStatement();

                    var lastTopicAccessSql = "NULL";
                    var lastForumAccessSql = "NULL";

                    if (findLastRead)
                    {
                        var topicAccessExpression =
                            db.Connection.From<TopicReadTracking>(db.Connection.TableAlias("y"));
                        topicAccessExpression.Where(
                            $"""
                             y.{topicAccessExpression.Column<TopicReadTracking>(y => y.TopicID)}={expression.Column<Topic>(x => x.ID, true)}
                                                                 and y.{topicAccessExpression.Column<TopicReadTracking>(y => y.UserID)}={userId}
                             """);
                        lastTopicAccessSql = topicAccessExpression.Select(
                                $" {topicAccessExpression.Column<TopicReadTracking>(x => x.LastAccessDate)}")
                            .ToSelectStatement();

                        var forumAccessExpression =
                            db.Connection.From<ForumReadTracking>(db.Connection.TableAlias("x"));
                        forumAccessExpression.Where(
                            $"""
                             x.{forumAccessExpression.Column<ForumReadTracking>(x => x.ForumID)}={expression.Column<Topic>(x => x.ForumID, true)}
                                                                 and x.{forumAccessExpression.Column<ForumReadTracking>(x => x.UserID)}={userId}
                             """);
                        lastForumAccessSql = forumAccessExpression.Select(
                                $" {forumAccessExpression.Column<ForumReadTracking>(x => x.LastAccessDate)}")
                            .ToSelectStatement();
                    }

                    expression.Join<Forum>((c, f) => c.ID == f.CategoryID)
                        .Join<Forum, ActiveAccess>((f, x) => x.ForumID == f.ID).CustomJoin(
                            $"""
                              left outer join {expression.Table<Topic>()} on {expression.Column<Topic>(t => t.ID, true)} =
                                                                                                            {expression.Column<Forum>(f => f.LastTopicID, true)}
                             """)
                        .CustomJoin(
                            $" left outer join {expression.Table<User>()} on {expression.Column<User>(x => x.ID, true)} = {expression.Column<Topic>(t => t.LastUserID, true)} ")
                        .Where<Forum, Category, ActiveAccess>(
                            (forum, category, x) => category.BoardID == boardId && (category.Flags & 1) == 1 && x.UserID == userId && x.ReadAccess);

                    // -- count sub-forums
                    var countSubForumsExpression = db.Connection.From<Forum>(db.Connection.TableAlias("sub"));

                    countSubForumsExpression.Where(
                        $"sub.{countSubForumsExpression.Column<Forum>(x => x.ParentID)}={expression.Column<Forum>(x => x.ID, true)}");
                    var countSubForumsSql = countSubForumsExpression.Select(Sql.Count("1")).ToSelectStatement();

                    if (categoryId.HasValue)
                    {
                        expression.And<Category>(a => a.ID == categoryId.Value);
                    }

                    if (parentId.HasValue)
                    {
                        expression.And<Forum>(f => f.ParentID == parentId.Value);
                    }

                    // -- count total
                    var countTotalExpression = expression;

                    var countTotalSql = countTotalExpression
                        .Select(Sql.Count($"{countTotalExpression.Column<Forum>(x => x.ID, true)}")).ToSelectStatement();

                    expression.OrderBy<Category>(a => a.SortOrder).ThenBy<Forum>(b => b.SortOrder).Page(pageIndex + 1, pageSize);

                    expression.Select<Category, Forum, ActiveAccess, Topic, User>(
                        (a, b, x, t, lastUser) => new
                                                      {
                                                          CategoryID = a.ID,
                                                          Category = a.Name,
                                                          a.CategoryImage,
                                                          ForumID = b.ID,
                                                          Forum = b.Name,
                                                          b.Description,
                                                          b.ImageURL,
                                                          b.Styles,
                                                          b.ParentID,
                                                          Topics = b.NumTopics,
                                                          Posts = b.NumPosts,
                                                          t.LastPosted,
                                                          t.LastMessageID,
                                                          t.LastMessageFlags,
                                                          t.LastUserID,
                                                          LastUser = lastUser.Name,
                                                          LastUserDisplayName = lastUser.DisplayName,
                                                          LastUserSuspended = lastUser.Suspended,
                                                          LastTopicID = t.ID,
                                                          t.TopicMovedID,
                                                          LastTopicName = t.TopicName,
                                                          LastTopicStyles = t.Styles,
                                                          b.Flags,
                                                          Viewing = Sql.Custom($"({countViewsSql})"),
                                                          b.RemoteURL,
                                                          x.ReadAccess,
                                                          Style = lastUser.UserStyle,
                                                          LastForumAccess = Sql.Custom($"({lastForumAccessSql})"),
                                                          LastTopicAccess = Sql.Custom($"({lastTopicAccessSql})"),
                                                          SubForums = Sql.Custom($"({countSubForumsSql})"),
                                                          Total = Sql.Custom($"({countTotalSql})")
                                                      });

                    return db.Connection.Select<ForumRead>(expression);
                });
    }

    /// <summary>
    /// Deletes a forum
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum ID.
    /// </param>
    /// <returns>
    /// Indicate that forum has been deleted
    /// </returns>
    public async static Task<bool> DeleteAsync(this IRepository<Forum> repository, int forumId)
    {
        if (await repository.ExistsAsync(f => f.ParentID == forumId))
        {
            await repository.UpdateOnlyAsync(() => new Forum { ParentID = null }, f => f.ParentID == forumId);
        }

        await repository.UpdateOnlyAsync(() => new Forum { LastMessageID = null, LastTopicID = null }, f => f.ID == forumId);

        await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
            () => new Topic { LastMessageID = null },
            f => f.ID == forumId);

        await BoardContext.Current.GetRepository<Active>().DeleteAsync(x => x.ForumID == forumId);

        await BoardContext.Current.GetRepository<WatchForum>().DeleteAsync(x => x.ForumID == forumId);
        await BoardContext.Current.GetRepository<ForumReadTracking>().DeleteAsync(x => x.ForumID == forumId);

        // --- Delete topics, messages and attachments
        var topics = await BoardContext.Current.GetRepository<Topic>().GetAsync(g => g.ForumID == forumId);

        foreach (var topicId in topics.Select(t => t.ID))
        {
            await BoardContext.Current.GetRepository<WatchTopic>().DeleteAsync(x => x.TopicID == topicId);

            await BoardContext.Current.GetRepository<Topic>().DeleteAsync(forumId, topicId, true);
        }

        await BoardContext.Current.GetRepository<ForumAccess>().DeleteAsync(x => x.ForumID == forumId);
        await BoardContext.Current.GetRepository<UserForum>().DeleteAsync(x => x.ForumID == forumId);
        await BoardContext.Current.GetRepository<Forum>().DeleteByIdAsync(forumId);

        return true;
    }

    /// <summary>
    /// Deletes a Forum and Moves the Content to a new Forum
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="oldForumId">
    /// The Current Forum ID
    /// </param>
    /// <param name="newForumId">
    /// The New Forum ID
    /// </param>
    /// <returns>
    /// Indicates that forum has been deleted
    /// </returns>
    public async static Task<bool> MoveAsync(this IRepository<Forum> repository, int oldForumId, int newForumId)
    {
        if (await repository.ExistsAsync(f => f.ParentID == oldForumId))
        {
            await repository.UpdateOnlyAsync(() => new Forum { ParentID = null }, f => f.ParentID == oldForumId);
        }

        await BoardContext.Current.GetRepository<Forum>().UpdateOnlyAsync(
            () => new Forum { LastMessageID = null, LastTopicID = null },
            f => f.ID == oldForumId);
        await BoardContext.Current.GetRepository<Active>().UpdateOnlyAsync(
            () => new Active { ForumID = newForumId },
            f => f.ForumID == oldForumId);

        await BoardContext.Current.GetRepository<WatchForum>().DeleteAsync(x => x.ForumID == oldForumId);
        await BoardContext.Current.GetRepository<ForumReadTracking>().DeleteAsync(x => x.ForumID == oldForumId);

        // -- Move topics, messages and attachments
        var topics = await BoardContext.Current.GetRepository<Topic>().GetAsync(t => t.ForumID == oldForumId);

        foreach (var topic in topics)
        {
            await BoardContext.Current.GetRepository<Topic>().MoveAsync(topic, oldForumId, newForumId, false, 0);
        }

        await BoardContext.Current.GetRepository<ForumAccess>().DeleteAsync(x => x.ForumID == oldForumId);

        await BoardContext.Current.GetRepository<UserForum>().UpdateOnlyAsync(
            () => new UserForum { ForumID = newForumId },
            f => f.ForumID == oldForumId);

        await BoardContext.Current.GetRepository<Forum>().DeleteAsync(x => x.ID == oldForumId);

        return true;
    }

    /// <summary>
    /// Return admin view of Categories with Forums/Sub-forums ordered accordingly.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The User ID
    /// </param>
    /// <param name="boardId">
    /// The Board ID
    /// </param>
    /// <returns>
    /// Returns the Moderator List for the Board
    /// </returns>
    public static List<ModerateForum> ModerateList(
        this IRepository<Forum> repository,
        int userId,
        int boardId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Category>();

        var forums = repository.DbAccess.Execute(
            db =>
                {
                    expression.Join<Forum>((c, f) => c.ID == f.CategoryID)
                        .Join<Forum, ActiveAccess>((f, x) => x.ForumID == f.ID);

                    expression.Where<Category, ActiveAccess>(
                        (c, x) => c.BoardID == boardId && x.ModeratorAccess && x.UserID == userId);

                    expression.OrderBy<Category>(c => c.SortOrder).ThenBy<Forum>(f => f.SortOrder);

                    // -- count unapproved posts
                    var countMessagesExpression = db.Connection.From<Message>(db.Connection.TableAlias("m"));
                    countMessagesExpression.Join<Topic>(
                        (m, t) => Sql.TableAlias(t.ID, "t") == Sql.TableAlias(m.TopicID, "m"),
                        db.Connection.TableAlias("t"));

                    countMessagesExpression.Where(
                        $"""
                         (m.{countMessagesExpression.Column<Message>(x => x.Flags)} & 16) != 16
                                                             and (m.{countMessagesExpression.Column<Message>(x => x.Flags)} & 8) != 8
                                                             and (t.{countMessagesExpression.Column<Topic>(x => x.Flags)} & 8) != 8
                                                             and t.{countMessagesExpression.Column<Topic>(x => x.ForumID)}=
                                                             {expression.Column<Forum>(x => x.ID, true)}
                         """);
                    var countMessagesSql = countMessagesExpression
                        .Select(Sql.Count(countMessagesExpression.Column<Message>(x => x.ID))).ToSelectStatement();

                    // -- count reported posts
                    var countReportedExpression = db.Connection.From<Message>(db.Connection.TableAlias("m"));
                    countReportedExpression.Join<Topic>(
                        (m, t) => Sql.TableAlias(t.ID, "t") == Sql.TableAlias(m.TopicID, "m"),
                        db.Connection.TableAlias("t"));

                    countReportedExpression.Where(
                        $"""
                         (m.{countReportedExpression.Column<Message>(x => x.Flags)} & 128) = 128
                                                             and (m.{countMessagesExpression.Column<Message>(x => x.Flags)} & 8) != 8
                                                             and (t.{countMessagesExpression.Column<Topic>(x => x.Flags)} & 8) != 8
                                                             and t.{countReportedExpression.Column<Topic>(x => x.ForumID)}=
                                                             {expression.Column<Forum>(x => x.ID, true)}
                         """);
                    var countReportedSql = countReportedExpression
                        .Select(Sql.Count(countReportedExpression.Column<Message>(x => x.ID))).ToSelectStatement();

                    expression.Select<Category, Forum>(
                        (c, f) => new
                                      {
                                          Category = c.Name,
                                          f.CategoryID,
                                          f.Name,
                                          f.ParentID,
                                          ForumID = f.ID,
                                          ReportedCount = Sql.Custom($"({countReportedSql})"),
                                          MessageCount = Sql.Custom($"({countMessagesSql})")
                                      });

                    return db.Connection.Select<ModerateForum>(expression);
                });

        // Remove all forums with no reports. Would be better to do it in query...
        forums.RemoveAll(f => f.MessageCount == 0 && f.ReportedCount == 0);

        return forums;
    }

    /// <summary>
    /// Updates the Forum Stats (Posts and Topics Count).
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="forumId">The forum identifier.</param>
    public static void UpdateStats(this IRepository<Forum> repository, int forumId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

        expression.Join<Forum>((t, f) => f.ID == t.ForumID)
            .Where<Topic, Forum>((t, f) => (f.ID == forumId || f.ParentID == forumId) && (t.Flags & 8) != 8 && t.NumPosts > 0).Take(1)
            .Select<Topic, Forum>(
                (t, f) => new { PostsCount = Sql.Sum(t.NumPosts), TopicsCount = Sql.Count(t.ID) });

        var (postsCount, topicsCount) = repository.DbAccess
            .Execute(db => db.Connection.Single<(int postsCount, int topicsCount)>(expression));

        repository.UpdateOnly(
            () => new Forum { NumPosts = postsCount, NumTopics = topicsCount },
            f => f.ID == forumId);
    }

    /// <summary>
    /// Updates the Forum Last Post.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="forumId">The forum identifier.</param>
    public static void UpdateLastPost(this IRepository<Forum> repository, int forumId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

        expression.Join<Message>((t, m) => m.TopicID == t.ID)
            .Where<Topic, Message>((t, m) => t.ForumID == forumId && (t.Flags & 8) != 8 && (m.Flags & 24) == 16)
            .OrderByDescending<Message>(m => m.Posted).Take(1);

        var message = repository.DbAccess.Execute(db => db.Connection.Single<Message>(expression));

        if (message != null)
        {
            repository.UpdateOnly(
                () => new Forum
                          {
                              LastPosted = message.Posted,
                              LastTopicID = message.TopicID,
                              LastMessageID = message.ID,
                              LastUserID = message.UserID,
                              LastUserName = message.UserName,
                              LastUserDisplayName = message.UserDisplayName
                          },
                f => f.ID == forumId);
        }
        else
        {
            repository.UpdateOnly(
                () => new Forum
                          {
                              LastPosted = null,
                              LastTopicID = null,
                              LastMessageID = null,
                              LastUserID = null,
                              LastUserName = null,
                              LastUserDisplayName = null
                          },
                f => f.ID == forumId);
        }
    }

    /// <summary>
    /// Re-Order all Forums By Name Ascending
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forums">
    /// The List of forums to be sorted
    /// </param>
    public static void ReOrderAllAscending(this IRepository<Forum> repository, List<Forum> forums)
    {
        short sortOrder = 0;

        forums.OrderBy(x => x.Name).ForEach(
            forum =>
                {
                    repository.UpdateOnly(
                        () => new Forum
                                  {
                                      SortOrder = sortOrder
                                  },
                        f => f.ID == forum.ID);

                    sortOrder++;
                });
    }

    /// <summary>
    /// Re-Order all Forums By Name Descending
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forums">
    /// The List of forums to be sorted
    /// </param>
    public static void ReOrderAllDescending(this IRepository<Forum> repository, List<Forum> forums)
    {
        short sortOrder = 0;

        forums.OrderByDescending(x => x.Name).ForEach(
            forum =>
                {
                    repository.UpdateOnly(
                        () => new Forum
                                  {
                                      SortOrder = sortOrder
                                  },
                        f => f.ID == forum.ID);

                    sortOrder++;
                });
    }

    /// <summary>
    /// The SortList.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="listSource">
    /// The list source.
    /// </param>
    /// <returns>
    /// Returns the Sorted Moderator List
    /// </returns>
    public static List<ForumSorted> SortModeratorList(
        this IRepository<Forum> repository,
        IEnumerable<ModeratorsForums> listSource)
    {
        return [.. listSource.Select(
            forum => new ForumSorted
                         {
                             Category = forum.CategoryName,
                             Forum = forum.ParentID.HasValue
                                         ? $" - {HttpUtility.HtmlEncode(forum.ForumName)}"
                                         : HttpUtility.HtmlEncode(forum.ForumName),
                             ForumID = forum.ForumID,
                             Icon = "comments",
                             ForumLink = BoardContext.Current.Get<ILinkBuilder>().GetForumLink(forum.ForumID, forum.ForumName)
                         })];
    }

    /// <summary>
    /// The SortList.
    /// </summary>
    /// <param name="_">
    /// The repository.
    /// </param>
    /// <param name="listSource">
    /// The list source.
    /// </param>
    /// <returns>
    /// Returns the Sorted List
    /// </returns>
    private static List<SelectGroup> SortList(
        this IRepository<Forum> _,
        IEnumerable<Tuple<Forum, Category, ActiveAccess>> listSource)
    {
        var enumerable = listSource.ToList();
        var categories = enumerable.Select(x => x.Item2).DistinctBy(x => x.ID);

        var listDestination = new List<SelectGroup>();

        categories.ForEach(
            category =>
                {
                    var forumsByCategory = enumerable.Select(x => x.Item1).Where(x => x.CategoryID == category.ID);

                    var selectGroup = new SelectGroup
                                          {
                                              id = -category.ID,
                                              text = category.Name,
                                              children = [.. forumsByCategory.Select(
                                                  forum => new SelectOptions
                                                               {
                                                                   id = forum.ID.ToString(),
                                                                   text = forum.ParentID.HasValue ? $" - {HttpUtility.HtmlEncode(forum.Name)}"
                                                                              : HttpUtility.HtmlEncode(forum.Name),
                                                                   url = BoardContext.Current.Get<ILinkBuilder>().GetForumLink(forum)
                                                               })]
                    };

                    listDestination.Add(selectGroup);
                });

        return listDestination;
    }
}