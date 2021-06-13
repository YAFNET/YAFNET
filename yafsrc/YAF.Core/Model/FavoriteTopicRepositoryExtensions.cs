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

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects.Model;

    /// <summary>
    ///     The favorite topic repository extensions.
    /// </summary>
    public static class FavoriteTopicRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The count.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Count(this IRepository<FavoriteTopic> repository, [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.Count(f => f.TopicID == topicId).ToType<int>();
        }

        /// <summary>
        /// The favorite remove.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool DeleteByUserAndTopic(
            this IRepository<FavoriteTopic> repository,
            [NotNull] int userId,
            [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository);

            var count = repository.DbAccess.Execute(
                db => db.Connection.Delete<FavoriteTopic>(x => x.UserID == userId && x.TopicID == topicId));
            if (count > 0)
            {
                repository.FireDeleted();
            }

            return count > 0;
        }

        /// <summary>
        /// Gets the Paged List of Favorite Topics.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="sinceDate">
        /// The since date.
        /// </param>
        /// <param name="toDate">
        /// The to date.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="findLastRead">
        /// The find last read.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<PagedTopic> ListPaged(
            this IRepository<FavoriteTopic> repository,
            [NotNull] int userId,
            [NotNull] DateTime sinceDate,
            [NotNull] DateTime toDate,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [NotNull] bool findLastRead)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

            return repository.DbAccess.Execute(
                db =>
                {
                    expression.Join<User>((t, u) => u.ID == t.UserID).Join<Forum>((t, f) => f.ID == t.ForumID)
                        .Join<Forum, ActiveAccess>((f, x) => x.ForumID == f.ID)
                        .Join<Forum, Category>((f, c) => c.ID == f.CategoryID)
                        .Join<FavoriteTopic>((t, z) => z.TopicID == t.ID && z.UserID == userId);

                    expression.Where<Topic, ActiveAccess, Category>(
                        (t, x, c) => x.UserID == userId && x.ReadAccess && (t.Flags & 8) != 8 &&
                                     t.TopicMovedID == null && t.LastPosted != null && t.LastPosted > sinceDate &&
                                     t.LastPosted < toDate);

                    // -- count total
                    var countTotalExpression = db.Connection.From<Topic>();

                    countTotalExpression.Where<Topic, ActiveAccess, Category>(
                        (t, x, c) => x.UserID == userId && x.ReadAccess && (t.Flags & 8) != 8 &&
                                     t.TopicMovedID == null && t.LastPosted != null && t.LastPosted > sinceDate &&
                                     t.LastPosted < toDate);

                    expression.OrderByDescending<Topic>(t => t.LastPosted).Page(pageIndex + 1, pageSize);

                    var countTotalSql = countTotalExpression
                        .Select(Sql.Count($"{countTotalExpression.Column<Topic>(x => x.ID)}")).ToSelectStatement();

                    // -- Count favorite
                    var countFavoriteExpression = db.Connection.From<FavoriteTopic>(db.Connection.TableAlias("f"));
                    countFavoriteExpression.Where(
                        $@"f.{countFavoriteExpression.Column<FavoriteTopic>(f => f.TopicID)}=
                                    {OrmLiteConfig.DialectProvider.IsNullFunction(expression.Column<Topic>(x => x.TopicMovedID, true),expression.Column<Topic>(x => x.ID, true))}");
                    var countFavoriteSql = countFavoriteExpression.Select(Sql.Count("1")).ToSelectStatement();

                    // -- count deleted posts
                    var countDeletedExpression = db.Connection.From<Message>(db.Connection.TableAlias("mes"));
                    countDeletedExpression.Where(
                        $@"mes.{countDeletedExpression.Column<Message>(x => x.TopicID)}={expression.Column<Topic>(x => x.ID, true)}
                                    and (mes.{countDeletedExpression.Column<Message>(x => x.Flags)} & 8) = 8
                                    and mes.{countDeletedExpression.Column<Message>(x => x.UserID)}={userId}");
                    var countDeletedSql = countDeletedExpression.Select(Sql.Count("1")).ToSelectStatement();

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
                                $"{topicAccessExpression.Column<TopicReadTracking>(x => x.LastAccessDate)}").Limit(1)
                            .ToSelectStatement();

                        var forumAccessExpression =
                            db.Connection.From<ForumReadTracking>(db.Connection.TableAlias("x"));
                        forumAccessExpression.Where(
                            $@"x.{forumAccessExpression.Column<ForumReadTracking>(x => x.ForumID)}={expression.Column<Topic>(x => x.ForumID, true)}
                                    and x.{forumAccessExpression.Column<ForumReadTracking>(x => x.UserID)}={userId}");
                        lastForumAccessSql = forumAccessExpression.Select(
                                $"{forumAccessExpression.Column<ForumReadTracking>(x => x.LastAccessDate)}").Limit(1)
                            .ToSelectStatement();
                    }

                    // -- last user
                    var lastUserNameExpression = db.Connection.From<User>(db.Connection.TableAlias("usr"));
                    lastUserNameExpression.Where(
                        $@"usr.{lastUserNameExpression.Column<User>(x => x.ID)}=
                                   {expression.Column<Topic>(x => x.LastUserID, true)}");
                    var lastUserNameSql = lastUserNameExpression.Select(
                        $"{lastUserNameExpression.Column<User>(x => x.Name)}").Limit(1).ToSelectStatement();

                    var lastUserDisplayNameSql = lastUserNameExpression.Select(
                        $"{lastUserNameExpression.Column<User>(x => x.DisplayName)}").Limit(1).ToSelectStatement();

                    var lastUserSuspendedSql = lastUserNameExpression.Select(
                        $"{lastUserNameExpression.Column<User>(x => x.Suspended)}").Limit(1).ToSelectStatement();

                    var lastUserStyleSql = lastUserNameExpression.Select(
                        $"{lastUserNameExpression.Column<User>(x => x.UserStyle)}").Limit(1).ToSelectStatement();

                    // -- first message
                    var firstMessageExpression = db.Connection.From<Message>(db.Connection.TableAlias("fm"));
                    firstMessageExpression.Where(
                        $@"fm.{firstMessageExpression.Column<Message>(x => x.TopicID)}=
                                   {OrmLiteConfig.DialectProvider.IsNullFunction(expression.Column<Topic>(x => x.TopicMovedID, true),expression.Column<Topic>(x => x.ID, true))}
                                   and fm.{firstMessageExpression.Column<Message>(x => x.Position)} = 0");
                    var firstMessageSql = firstMessageExpression.Select(
                        $"{firstMessageExpression.Column<Message>(x => x.MessageText)}").Limit(1).ToSelectStatement();

                    expression.Select<Topic, User, Forum>(
                        (c, b, d) => new
                        {
                            ForumID = d.ID,
                            TopicID = c.ID,
                            c.Posted,
                            LinkTopicID = c.TopicMovedID != null ? c.TopicMovedID : c.ID,
                            c.TopicMovedID,
                            FavoriteCount = Sql.Custom($"({countFavoriteSql})"),
                            Subject = c.TopicName,
                            c.Description,
                            c.Status,
                            c.Styles,
                            c.UserID,
                            Starter = c.UserName != null ? c.UserName : b.Name,
                            StarterDisplay = c.UserDisplayName != null ? c.UserDisplayName : b.DisplayName,
                            Replies = c.NumPosts - 1,
                            NumPostsDeleted = Sql.Custom($"({countDeletedSql})"),
                            c.Views,
                            c.LastPosted,
                            c.LastUserID,
                            LastUserName = Sql.Custom($"({lastUserNameSql})"),
                            LastUserDisplayName = Sql.Custom($"({lastUserDisplayNameSql})"),
                            LastUserSuspended = Sql.Custom($"({lastUserSuspendedSql})"),
                            LastUserStyle = Sql.Custom($"({lastUserStyleSql})"),
                            c.LastMessageFlags,
                            c.LastMessageID,
                            LastTopicID = c.ID,
                            c.LinkDate,
                            TopicFlags = c.Flags,
                            c.Priority,
                            c.PollID,
                            ForumFlags = d.Flags,
                            FirstMessage = Sql.Custom($"({firstMessageSql})"),
                            StarterStyle = b.UserStyle,
                            StarterSuspended = b.Suspended,
                            LastForumAccess = Sql.Custom($"({lastForumAccessSql})"),
                            LastTopicAccess = Sql.Custom($"({lastTopicAccessSql})"),
                            c.TopicImage,
                            PageIndex = pageIndex,
                            TotalRows = Sql.Custom($"({countTotalSql})")
                        });

                    return db.Connection.Select<PagedTopic>(expression);
                });
        }

        #endregion
    }
}