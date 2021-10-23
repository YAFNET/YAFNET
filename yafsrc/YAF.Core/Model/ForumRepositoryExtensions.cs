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

namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects.Model;

    /// <summary>
    /// The Forum Repository Extensions
    /// </summary>
    public static class ForumRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Saves a Forum or if forumId is null creates a new Forum
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <param name="parentID">
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
        /// <param name="remoteURL">
        /// The remote url.
        /// </param>
        /// <param name="themeURL">
        /// The theme url.
        /// </param>
        /// <param name="imageURL">
        /// The image url.
        /// </param>
        /// <param name="styles">
        /// The styles.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Save(
            [NotNull] this IRepository<Forum> repository,
            [CanBeNull] int? forumID,
            [NotNull] int categoryID,
            [CanBeNull] int? parentID,
            [NotNull] string name,
            [NotNull] string description,
            [NotNull] int sortOrder,
            [NotNull] bool locked,
            [NotNull] bool hidden,
            [NotNull] bool isTest,
            [NotNull] bool moderated,
            [CanBeNull] int? moderatedPostCount,
            [NotNull] bool isModeratedNewTopicOnly,
            [CanBeNull] string remoteURL,
            [CanBeNull] string themeURL,
            [CanBeNull] string imageURL,
            [CanBeNull] string styles)
        {
            CodeContracts.VerifyNotNull(repository);

            if (parentID is 0)
            {
                parentID = null;
            }

            var flags = new ForumFlags
            {
                IsLocked = locked, IsHidden = hidden, IsTest = isTest, IsModerated = moderated
            };

            if (!forumID.HasValue)
            {
                var newForumId = repository.Insert(
                    new Forum
                    {
                        ParentID = parentID,
                        Name = name,
                        Description = description,
                        SortOrder = sortOrder,
                        CategoryID = categoryID,
                        RemoteURL = remoteURL,
                        ThemeURL = themeURL,
                        ImageURL = imageURL,
                        Styles = styles,
                        Flags = flags.BitValue,
                        ModeratedPostCount = moderatedPostCount,
                        IsModeratedNewTopicOnly = isModeratedNewTopicOnly
                    });

                repository.FireNew(newForumId);

                return newForumId;
            }

            repository.UpdateOnly(
                () => new Forum
                {
                    ParentID = parentID,
                    Name = name,
                    Description = description,
                    SortOrder = sortOrder,
                    CategoryID = categoryID,
                    RemoteURL = remoteURL,
                    ThemeURL = themeURL,
                    ImageURL = imageURL,
                    Styles = styles,
                    Flags = flags.BitValue,
                    ModeratedPostCount = moderatedPostCount,
                    IsModeratedNewTopicOnly = isModeratedNewTopicOnly
                },
                f => f.ID == forumID);

            repository.FireUpdated(forumID.Value);

            // empty out access table(s)
            BoardContext.Current.GetRepository<Active>().DeleteAll();
            BoardContext.Current.GetRepository<ActiveAccess>().DeleteAll();

            return forumID.Value;
        }

        /// <summary>
        /// The method returns an integer value for a  found parent forum
        ///   if a forum is a parent of an existing child to avoid circular dependency
        ///   while creating a new forum
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="parentId">
        /// The parent Id.
        /// </param>
        public static bool IsParentsChecker([NotNull] this IRepository<Forum> repository, [NotNull] int forumId, [NotNull] int parentId)
        {
            CodeContracts.VerifyNotNull(repository);

            if (repository.Exists(f => f.ParentID == forumId))
            {
                // Forum Is already a Parent
                return true;
            }

            // Checks if Parent Forum is parent or child
            return repository.Exists(f => f.ParentID == parentId && f.ID != forumId) ||
                   repository.Exists(f => f.ID == parentId && f.ParentID != null);
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
        public static List<Tuple<Forum, Category>> ListAll(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Where<Forum, Category>((forum, category) => category.BoardID == boardId)
                .OrderBy<Category>(c => c.SortOrder).ThenBy<Forum>(f => f.SortOrder).ThenBy<Category>(c => c.ID)
                .ThenBy<Forum>(f => f.ID);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Forum, Category>(expression));
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
        public static List<Tuple<Forum, Category, ActiveAccess>> ListAllWithAccess(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardId,
            [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Join<ActiveAccess>((forum, active) => active.ForumID == forum.ID)
                .Where<Forum, Category, ActiveAccess>(
                    (forum, category, active) =>
                        active.UserID == userId && category.BoardID == boardId && active.ReadAccess)
                .OrderBy<Category>(c => c.SortOrder).ThenBy<Forum>(f => f.SortOrder).ThenBy<Category>(c => c.ID)
                .ThenBy<Forum>(f => f.ID).Select<Forum, Category, Active>(
                    (forum, category, active) => new
                    {
                        CategoryID = category.ID,
                        Category = category.Name,
                        ForumID = forum.ID,
                        Forum = forum.Name,
                        Indent = 0,
                        forum.ParentID
                    });

            return repository.DbAccess.Execute(
                db => db.Connection.SelectMulti<Forum, Category, ActiveAccess>(expression));
        }

        /// <summary>
        /// Lists all forums within a given subcategory
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="categoryId">
        /// The category ID.
        /// </param>
        public static List<ForumSorted> ListAllFromCategory(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int categoryId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.ListAllFromCategory(categoryId, true);
        }

        /// <summary>
        /// Lists all forums within a given subcategory
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="categoryId">
        /// The category ID.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty First Row.
        /// </param>
        public static List<ForumSorted> ListAllFromCategory(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int categoryId,
            bool emptyFirstRow)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Where<Forum, Category>(
                    (forum, category) => category.BoardID == repository.BoardID && category.ID == categoryId);

            var list = repository.DbAccess.Execute(db => db.Connection.Select(expression));

            return repository.SortList(list, 0, 0, emptyFirstRow);
        }

        /// <summary>
        /// Gets the forum list all sorted.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        public static List<ForumSorted> ListAllSorted(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int userID)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.ListAllSorted(boardID, userID, false);
        }

        /// <summary>
        /// Gets the forum list all sorted.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty first row.
        /// </param>
        [NotNull]
        public static List<ForumSorted> ListAllSorted(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardID,
            [NotNull] int userID,
            bool emptyFirstRow)
        {
            CodeContracts.VerifyNotNull(repository);

            var list = repository.ListAllWithAccess(boardID, userID);

            return repository.SortList(list, 0, 0, 0, emptyFirstRow);
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
        public static List<ForumRead> ListRead(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardId,
            [NotNull] int userId,
            [CanBeNull] int? categoryId,
            [CanBeNull] int? parentId,
            [NotNull] bool findLastRead)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<Category>();

                    var countViewsExpression = db.Connection.From<Active>(db.Connection.TableAlias("x"));
                    countViewsExpression.Where(
                        $@"x.{countViewsExpression.Column<Active>(x => x.ForumID)}=
                                   {expression.Column<Forum>(x => x.ID, true)}");
                    var countViewsSql = countViewsExpression.Select(Sql.Count("1")).ToSelectStatement();

                    var lastTopicAccessSql = "NULL";
                    var lastForumAccessSql = "NULL";

                    if (findLastRead)
                    {
                        var topicAccessExpression =
                            db.Connection.From<TopicReadTracking>(db.Connection.TableAlias("y"));
                        topicAccessExpression.Where(
                            $@"y.{topicAccessExpression.Column<TopicReadTracking>(y => y.TopicID)}={expression.Column<Topic>(x => x.ID, true)}
                                    and y.{topicAccessExpression.Column<TopicReadTracking>(y => y.UserID)}={userId}");
                        lastTopicAccessSql = topicAccessExpression.Select(
                                $" {topicAccessExpression.Column<TopicReadTracking>(x => x.LastAccessDate)}")
                            .ToSelectStatement();

                        var forumAccessExpression =
                            db.Connection.From<ForumReadTracking>(db.Connection.TableAlias("x"));
                        forumAccessExpression.Where(
                            $@"x.{forumAccessExpression.Column<ForumReadTracking>(x => x.ForumID)}={expression.Column<Topic>(x => x.ForumID, true)}
                                    and x.{forumAccessExpression.Column<ForumReadTracking>(x => x.UserID)}={userId}");
                        lastForumAccessSql = forumAccessExpression.Select(
                                $" {forumAccessExpression.Column<ForumReadTracking>(x => x.LastAccessDate)}")
                            .ToSelectStatement();
                    }

                    expression.Join<Forum>((c, f) => c.ID == f.CategoryID)
                        .Join<Forum, ActiveAccess>((f, x) => x.ForumID == f.ID).CustomJoin(
                            $@" left outer join {expression.Table<Topic>()} on {expression.Column<Topic>(t => t.ID, true)} =
                                                                               {expression.Column<Forum>(f => f.LastTopicID, true)} ")
                        .CustomJoin(
                            $@" left outer join {expression.Table<User>()} on {expression.Column<User>(x => x.ID, true)} = {expression.Column<Topic>(t => t.LastUserID, true)} ")
                        .Where<Forum, Category, ActiveAccess>(
                            (forum, category, x) => category.BoardID == boardId && x.UserID == userId && x.ReadAccess);

                    if (categoryId.HasValue)
                    {
                        expression.And<Category>(a => a.ID == categoryId.Value);
                    }

                    if (parentId.HasValue)
                    {
                        expression.And<Forum>(f => f.ParentID == parentId.Value);
                    }

                    expression.OrderBy<Category>(a => a.SortOrder).ThenBy<Forum>(b => b.SortOrder);

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
                        });

                    return db.Connection.Select<ForumRead>(expression);
                });
        }

        /// <summary>
        /// Gets a list of topics in a forum
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The Board ID
        /// </param>
        /// <param name="forumId">
        /// The forum ID.
        /// </param>
        public static List<Forum> List(
            [NotNull] this IRepository<Forum> repository,
            [NotNull] int boardId,
            [CanBeNull] int? forumId)
        {
            CodeContracts.VerifyNotNull(repository);

            if (forumId is 0)
            {
                forumId = null;
            }

            if (forumId.HasValue)
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                    .Where<Forum, Category>(
                        (forum, category) => category.BoardID == boardId && forum.ID == forumId.Value);

                return repository.DbAccess.Execute(db => db.Connection.Select(expression));
            }
            else
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                    .Where<Forum, Category>((forum, category) => category.BoardID == boardId).OrderBy(f => f.SortOrder);

                return repository.DbAccess.Execute(db => db.Connection.Select(expression));
            }
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
        public static bool Delete(this IRepository<Forum> repository, [NotNull] int forumId)
        {
            CodeContracts.VerifyNotNull(repository);

            if (repository.Exists(f => f.ParentID == forumId))
            {
                return false;
            }

            repository.UpdateOnly(() => new Forum { LastMessageID = null, LastTopicID = null }, f => f.ID == forumId);

            BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                () => new Topic { LastMessageID = null },
                f => f.ID == forumId);

            BoardContext.Current.GetRepository<Active>().Delete(x => x.ForumID == forumId);

            BoardContext.Current.GetRepository<WatchForum>().Delete(x => x.ForumID == forumId);
            BoardContext.Current.GetRepository<ForumReadTracking>().Delete(x => x.ForumID == forumId);


            // --- Delete topics, messages and attachments
            var topics = BoardContext.Current.GetRepository<Topic>().Get(g => g.ForumID == forumId);

            topics.ForEach(
                t =>
                {
                    BoardContext.Current.GetRepository<WatchTopic>().Delete(x => x.TopicID == t.ID);
                    BoardContext.Current.GetRepository<NntpTopic>().Delete(x => x.TopicID == t.ID);

                    BoardContext.Current.GetRepository<Topic>().Delete(forumId, t.ID, true);
                });

            BoardContext.Current.GetRepository<NntpForum>().Delete(x => x.ForumID == forumId);
            BoardContext.Current.GetRepository<ForumAccess>().Delete(x => x.ForumID == forumId);
            BoardContext.Current.GetRepository<UserForum>().Delete(x => x.ForumID == forumId);
            BoardContext.Current.GetRepository<Forum>().DeleteById(forumId);

            repository.FireDeleted(forumId);

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
        public static bool Move(this IRepository<Forum> repository, [NotNull] int oldForumId, [NotNull] int newForumId)
        {
            CodeContracts.VerifyNotNull(repository);

            if (repository.Exists(f => f.ParentID == oldForumId))
            {
                return false;
            }

            BoardContext.Current.GetRepository<Forum>().UpdateOnly(
                () => new Forum { LastMessageID = null, LastTopicID = null },
                f => f.ID == oldForumId);
            BoardContext.Current.GetRepository<Active>().UpdateOnly(
                () => new Active { ForumID = newForumId },
                f => f.ForumID == oldForumId);
            BoardContext.Current.GetRepository<NntpForum>().UpdateOnly(
                () => new NntpForum { ForumID = newForumId },
                f => f.ForumID == oldForumId);
            BoardContext.Current.GetRepository<WatchForum>().UpdateOnly(
                () => new WatchForum { ForumID = newForumId },
                f => f.ForumID == oldForumId);
            BoardContext.Current.GetRepository<ForumReadTracking>().UpdateOnly(
                () => new ForumReadTracking { ForumID = newForumId },
                f => f.ForumID == oldForumId);

            // -- Move topics, messages and attachments
            var topics = BoardContext.Current.GetRepository<Topic>().Get(t => t.ForumID == oldForumId);

            topics.ForEach(
                topic => BoardContext.Current.GetRepository<Topic>().Move(topic.ID, oldForumId, newForumId, false, 0));

            BoardContext.Current.GetRepository<ForumAccess>().Delete(x => x.ForumID == oldForumId);

            BoardContext.Current.GetRepository<UserForum>().UpdateOnly(
                () => new UserForum { ForumID = newForumId },
                f => f.ForumID == oldForumId);

            BoardContext.Current.GetRepository<Forum>().Delete(x => x.ID == oldForumId);

            return true;
        }

        /// <summary>
        /// Gets all Forums sorted by category
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Forum> GetByCategorySorted(this IRepository<Forum> repository, [NotNull] int categoryId)
        {
            CodeContracts.VerifyNotNull(repository);

            var forums = repository.Get(f => f.CategoryID == categoryId);

            var forumsSorted = new List<Forum>();

            ForumListSortBasic(forums, forumsSorted, 0, 0);

            return forumsSorted;
        }

        /// <summary>
        /// Return admin view of Categories with Forums/Sub-forums ordered accordingly.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        ///  The User ID
        /// </param>
        /// <param name="boardId">
        ///  The Board ID
        /// </param>
        [NotNull]
        public static List<ModerateForum> ModerateList(
            this IRepository<Forum> repository,
            [NotNull] int userId,
            [NotNull] int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

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
                        $@"(m.{countMessagesExpression.Column<Message>(x => x.Flags)} & 16) != 16
                                    and (m.{countMessagesExpression.Column<Message>(x => x.Flags)} & 8) != 8
                                    and (t.{countMessagesExpression.Column<Topic>(x => x.Flags)} & 8) != 8
                                    and t.{countMessagesExpression.Column<Topic>(x => x.ForumID)}=
                                    {expression.Column<Forum>(x => x.ID, true)}");
                    var countMessagesSql = countMessagesExpression
                        .Select(Sql.Count(countMessagesExpression.Column<Message>(x => x.ID))).ToSelectStatement();

                    // -- count reported posts
                    var countReportedExpression = db.Connection.From<Message>(db.Connection.TableAlias("m"));
                    countReportedExpression.Join<Topic>(
                        (m, t) => Sql.TableAlias(t.ID, "t") == Sql.TableAlias(m.TopicID, "m"),
                        db.Connection.TableAlias("t"));

                    countReportedExpression.Where(
                        $@"(m.{countReportedExpression.Column<Message>(x => x.Flags)} & 128) = 128
                                    and (m.{countMessagesExpression.Column<Message>(x => x.Flags)} & 8) != 8
                                    and (t.{countMessagesExpression.Column<Topic>(x => x.Flags)} & 8) != 8
                                    and t.{countReportedExpression.Column<Topic>(x => x.ForumID)}=
                                    {expression.Column<Forum>(x => x.ID, true)}");
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
        public static void UpdateStats([NotNull] this IRepository<Forum> repository, [NotNull] int forumId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

            expression.Join<Forum>((t, f) => f.ID == t.ForumID)
                .Where<Topic, Forum>((t, f) => (f.ID == forumId || f.ParentID == forumId) && (t.Flags & 8) != 8)
                .Select<Topic, Forum>(
                    (t, f) => new { PostsCount = Sql.Sum(t.NumPosts), TopicsCount = Sql.Count(t.ID), });

            var (postsCount, topicsCount) = repository.DbAccess
                .Execute(db => db.Connection.Select<(int postsCount, int topicsCount)>(expression)).FirstOrDefault();

            repository.UpdateOnly(
                () => new Forum { NumPosts = postsCount, NumTopics = topicsCount },
                f => f.ID == forumId);
        }

        /// <summary>
        /// Updates the Forum Last Post.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="forumId">The forum identifier.</param>
        public static void UpdateLastPost([NotNull] this IRepository<Forum> repository, [NotNull] int forumId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

            expression.Join<Message>((t, m) => m.TopicID == t.ID)
                .Where<Topic, Message>((t, m) => t.ForumID == forumId && (t.Flags & 8) != 8 && (m.Flags & 24) == 16)
                .OrderByDescending<Message>(m => m.Posted);

            var message = repository.DbAccess.Execute(db => db.Connection.Select<Message>(expression)).FirstOrDefault();

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

        #endregion

        /// <summary>
        /// The SortList.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="parentId">
        /// The parent Id.
        /// </param>
        /// <param name="startingIndent">
        /// The starting indent.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty first row.
        /// </param>
        [NotNull]
        private static List<ForumSorted> SortList(
            this IRepository<Forum> repository,
            [NotNull] List<Forum> listSource,
            [NotNull] int parentId,
            [NotNull] int startingIndent,
            [NotNull] bool emptyFirstRow)
        {
            CodeContracts.VerifyNotNull(repository);

            var listDestination = new List<ForumSorted>();

            if (emptyFirstRow)
            {
                var blankRow = new ForumSorted
                {
                    ForumID = 0,
                    Forum = BoardContext.Current.Get<ILocalization>().GetText("NONE"),
                    Icon = string.Empty
                };

                listDestination.Add(blankRow);
            }

            repository.SortListRecursive(listSource, listDestination, parentId, startingIndent);

            return listDestination;
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
        /// <param name="parentId">
        /// The parent id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="startingIndent">
        /// The starting indent.
        /// </param>
        /// <param name="emptyFirstRow">
        /// The empty first row.
        /// </param>
        [NotNull]
        private static List<ForumSorted> SortList(
            this IRepository<Forum> repository,
            [NotNull] IEnumerable<Tuple<Forum, Category, ActiveAccess>> listSource,
            [NotNull] int parentId,
            [NotNull] int categoryId,
            [NotNull] int startingIndent,
            [NotNull] bool emptyFirstRow)
        {
            CodeContracts.VerifyNotNull(repository);

            var listDestination = new List<ForumSorted>();

            if (emptyFirstRow)
            {
                var blankRow = new ForumSorted
                {
                    ForumID = 0,
                    Forum = BoardContext.Current.Get<ILocalization>().GetText("NONE"),
                    Category = string.Empty,
                    Icon = string.Empty
                };

                listDestination.Add(blankRow);
            }

            repository.SortListRecursive(listSource, listDestination, parentId, categoryId, startingIndent);

            return listDestination;
        }

        /// <summary>
        /// The SortListRecursive.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="listDestination">
        /// The list destination.
        /// </param>
        /// <param name="parentId">
        /// The parent id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="currentIndent">
        /// The current indent.
        /// </param>
        private static void SortListRecursive(
            this IRepository<Forum> repository,
            [NotNull] IEnumerable<Tuple<Forum, Category, ActiveAccess>> listSource,
            [NotNull] ICollection<ForumSorted> listDestination,
            [NotNull] int parentId,
            [NotNull] int categoryId,
            [NotNull] int currentIndent)
        {
            CodeContracts.VerifyNotNull(repository);

            foreach (var (item1, item2, _) in listSource)
            {
                // see if this is a root-forum
                item1.ParentID ??= 0;

                if (item1.ParentID != parentId)
                {
                    continue;
                }

                if (item2.ID != categoryId)
                {
                    categoryId = item2.ID;

                    listDestination.Add(
                        new ForumSorted
                        {
                            ForumID = -categoryId,
                            Forum = $"{item2.Name}",
                            Category = $"{item2.Name}",
                            Icon = "folder"
                        });
                }

                var indent = string.Empty;

                for (var j = 0; j < currentIndent; j++)
                {
                    indent += "-";
                }

                // import the row into the destination
                var newRow = new ForumSorted
                {
                    ForumID = item1.ID,
                    Forum = $" {indent} {item1.Name}",
                    Category = $"{item2.Name}",
                    Icon = "comments"
                };


                listDestination.Add(newRow);

                // recurse through the list...
                repository.SortListRecursive(listSource, listDestination, item1.ID, categoryId, currentIndent + 1);
            }
        }

        /// <summary>
        /// The SortListRecursive.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="listDestination">
        /// The list destination.
        /// </param>
        /// <param name="parentId">
        /// The parent id.
        /// </param>
        /// <param name="currentIndent">
        /// The current indent.
        /// </param>
        private static void SortListRecursive(
            this IRepository<Forum> repository,
            [NotNull] List<Forum> listSource,
            [NotNull] ICollection<ForumSorted> listDestination,
            [NotNull] int parentId,
            [NotNull] int currentIndent)
        {
            listSource.ForEach(
                forum =>
                {
                    // see if this is a root-forum
                    forum.ParentID ??= 0;

                    if (forum.ParentID != parentId)
                    {
                        return;
                    }

                    var indent = string.Empty;

                    for (var j = 0; j < currentIndent; j++)
                    {
                        indent += "--";
                    }

                    // import the row into the destination
                    var newRow = new ForumSorted
                    {
                        ForumID = forum.ID, Forum = $" -{indent} {forum.Name}", Icon = "comments"
                    };

                    listDestination.Add(newRow);

                    // recurse through the list...
                    repository.SortListRecursive(listSource, listDestination, forum.ID, currentIndent + 1);
                });
        }

        /// <summary>
        /// Basic Sorting for the Forum List
        /// </summary>
        /// <param name="listSource">
        /// The list source.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="parentId">
        /// The parent Id.
        /// </param>
        /// <param name="currentLevel">
        /// The current level.
        /// </param>
        private static void ForumListSortBasic(
            [NotNull] List<Forum> listSource,
            [NotNull] ICollection<Forum> list,
            [NotNull] int parentId,
            [NotNull] int currentLevel)
        {
            listSource.ForEach(
                row =>
                {
                    row.ParentID ??= 0;

                    if (row.ParentID != parentId)
                    {
                        return;
                    }

                    var indent = string.Empty;
                    var intentIndex = currentLevel.ToType<int>();

                    for (var j = 0; j < intentIndex; j++)
                    {
                        indent += "--";
                    }

                    row.Name = $" -{indent} {row.Name}";
                    list.Add(row);
                    ForumListSortBasic(listSource, list, row.ID, currentLevel + 1);
                });
        }
    }
}