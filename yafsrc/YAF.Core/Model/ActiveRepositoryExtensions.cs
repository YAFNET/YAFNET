/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Collections.Generic;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The active repository extensions.
    /// </summary>
    public static class ActiveRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Lists the forum.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<dynamic> ListForum(this IRepository<Active> repository, [NotNull] int forumId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<Active>();

                    var countExpression = db.Connection.From<Active>(db.Connection.TableAlias("ac"));
                    countExpression.Where(
                        $@"ac.{countExpression.Column<Active>(ac => ac.UserID)}={expression.Column<Active>(a => a.UserID, true)}
                                    and ac.{countExpression.Column<Active>(ac => ac.ForumID)}={forumId}");
                    var countSql = countExpression.Select(Sql.Count($"{countExpression.Column<Active>(x => x.UserID)}"))
                        .ToSelectStatement();

                    expression.Join<User>((a, u) => u.ID == a.UserID).Where(a => a.ForumID == forumId)
                        .Select<Active, User>(
                            (a, b) => new
                            {
                                a.UserID,
                                UserName = b.Name,
                                UserDisplayName = b.DisplayName,
                                b.IsActiveExcluded,
                                IsCrawler = a.Flags & 8,
                                b.UserStyle,
                                b.Suspended,
                                UserCount = Sql.Custom($"({countSql})"),
                                a.Browser
                            }).GroupBy<Active, User>(
                            (a, u) => new
                            {
                                a.UserID,
                                u.DisplayName,
                                u.Name,
                                u.IsActiveExcluded,
                                u.ID,
                                u.UserStyle,
                                u.Suspended,
                                a.Flags,
                                a.Browser
                            }).OrderBy<User>(u => u.Name);

                    return db.Connection.Select<object>(expression);
                });
        }

        /// <summary>
        /// Lists the topic.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<dynamic> ListTopic(this IRepository<Active> repository, [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(db =>
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Active>();

                var countExpression = db.Connection.From<Active>(db.Connection.TableAlias("ac"));
                countExpression.Where(
                    $@"ac.{countExpression.Column<Active>(ac => ac.UserID)}={expression.Column<Active>(a => a.UserID, true)}
                                    and ac.{countExpression.Column<Active>(ac => ac.TopicID)}={topicId}");
                var countSql = countExpression.Select(Sql.Count($"{countExpression.Column<Active>(x => x.UserID)}"))
                    .ToSelectStatement();

                expression.Join<User>((a, u) => u.ID == a.UserID).Where(a => a.TopicID == topicId).Select<Active, User>(
                    (a, b) => new
                    {
                        a.UserID,
                        UserName = b.Name,
                        UserDisplayName = b.DisplayName,
                        b.IsActiveExcluded,
                        IsCrawler = a.Flags & 8,
                        b.UserStyle,
                        b.Suspended,
                        UserCount = Sql.Custom($"({countSql})"),
                        a.Browser
                    }).GroupBy<Active, User>(
                    (a, u) => new
                    {
                        a.UserID,
                        u.DisplayName,
                        u.Name,
                        u.IsActiveExcluded,
                        u.ID,
                        u.UserStyle,
                        u.Suspended,
                        a.Flags,
                        a.Browser
                    }).OrderBy<User>(u => u.Name);

                return db.Connection.Select<object>(expression);
            });
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="showGuests">
        /// The show Guests.
        /// </param>
        /// <param name="showCrawlers">
        /// The show crawlers. 
        /// </param>
        /// <param name="activeTime">
        /// The active time. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<dynamic> List(
            this IRepository<Active> repository,
            [NotNull] bool showGuests,
            [NotNull] bool showCrawlers,
            [NotNull] int activeTime,
            [CanBeNull] int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.DeleteActive(activeTime);

            // -- we don't delete guest access
            BoardContext.Current.GetRepository<ActiveAccess>().Delete(activeTime);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            return repository.DbAccess.Execute(
                db =>
                {
                    expression.Join<Rank>((u, r) => r.ID == u.RankID).Join<Active>((u, a) => a.UserID == u.ID);

                    if (showGuests)
                    {
                        expression.Where<User, Active>(
                            (u, a) => a.BoardID == (boardId ?? repository.BoardID));
                    }
                    else
                    {
                        if (showCrawlers)
                        {
                            expression.Where<User, Active>(
                                (u, a) => a.BoardID == (boardId ?? repository.BoardID) &&
                                          ((u.Flags & 4) == 4 || (u.Flags & 8) == 8));
                        }
                        else
                        {
                            expression.Where<User, Active>(
                                (u, a) => a.BoardID == (boardId ?? repository.BoardID) && u.IsGuest == false);
                        }
                    }

                    expression.OrderByDescending<Active>(a => a.LastActive);

                    var forumExpression = db.Connection.From<Forum>(db.Connection.TableAlias("f"));
                    forumExpression.Where(
                        $"f.{forumExpression.Column<Forum>(f => f.ID)}={expression.Column<Active>(a => a.ForumID, true)}");
                    var forumSql = forumExpression.Select($"{forumExpression.Column<Forum>(x => x.Name)}")
                        .ToSelectStatement();

                    var topicExpression = db.Connection.From<Topic>(db.Connection.TableAlias("t"));
                    topicExpression.Where(
                        $"t.{topicExpression.Column<Topic>(f => f.ID)}={expression.Column<Active>(a => a.TopicID, true)}");
                    var topicSql = topicExpression
                        .Select($"{topicExpression.Column<Topic>(x => x.TopicName)}").ToSelectStatement();

                    expression.Select<User, Rank, Active>(
                        (u, r, a) => new
                        {
                            a.UserID,
                            UserName = u.Name,
                            UserDisplayName = u.DisplayName,
                            a.IP,
                            a.SessionID,
                            a.ForumID,
                            a.TopicID,
                            ForumName = Sql.Custom($"({forumSql})"),
                            TopicName = Sql.Custom($"({topicSql})"),
                            u.IsGuest,
                            IsCrawler = a.Flags & 8,
                            u.IsActiveExcluded,
                            u.UserStyle,
                            u.Suspended,
                            UserCount = 1,
                            a.Login,
                            a.LastActive,
                            a.Location,
                            Active = Sql.Custom(
                                $"DATEDIFF(minute,{expression.Column<Active>(x => x.Login, true)}, {expression.Column<Active>(x => x.LastActive, true)})"),
                            a.Browser,
                            a.Platform,
                            a.ForumPage
                        });

                    return db.Connection.Select<dynamic>(expression);
                });
        }

        /// <summary>
        /// Lists the active user.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="showGuests">
        /// The show Guests.
        /// </param>
        /// <param name="showCrawlers">
        /// The show crawlers.
        /// </param>
        /// <param name="activeTime">
        /// The active time.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<dynamic> ListUsers(
            this IRepository<Active> repository,
            [NotNull] int userId,
            [NotNull] bool showGuests,
            [NotNull] bool showCrawlers,
            [NotNull] int activeTime,
            [CanBeNull] int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            return repository.DbAccess.Execute(
                db =>
                {
                    expression.Join<Rank>((u, r) => r.ID == u.RankID)
                        .Join<Active>((u, a) => a.UserID == u.ID)
                        .Join<Active, ActiveAccess>((a, x) => x.ForumID == (a.ForumID != null ? a.ForumID : 0));

                    if (showGuests)
                    {
                        expression.Where<User, Active, ActiveAccess>(
                            (u, a, x) => a.BoardID == (boardId ?? repository.BoardID) && x.UserID == userId);
                    }
                    else
                    {
                        if (showCrawlers)
                        {
                            expression.Where<User, Active, ActiveAccess>(
                                (u, a, x) => a.BoardID == (boardId ?? repository.BoardID) &&
                                          ((u.Flags & 4) == 4 || (u.Flags & 8) == 8) && x.UserID == userId);
                        }
                        else
                        {
                            expression.Where<User, Active, ActiveAccess>(
                                (u, a, x) => a.BoardID == (boardId ?? repository.BoardID) && u.IsGuest == false && x.UserID == userId);
                        }
                    }

                    expression.OrderByDescending<Active>(a => a.LastActive);

                    var forumExpression = db.Connection.From<Forum>(db.Connection.TableAlias("f"));
                    forumExpression.Where(
                        $"f.{forumExpression.Column<Forum>(f => f.ID)}={expression.Column<Active>(a => a.ForumID, true)}");
                    var forumSql = forumExpression.Select(Sql.Count($"{forumExpression.Column<Forum>(x => x.Name)}"))
                        .ToSelectStatement();

                    var topicExpression = db.Connection.From<Topic>(db.Connection.TableAlias("t"));
                    topicExpression.Where(
                        $"t.{topicExpression.Column<Topic>(f => f.ID)}={expression.Column<Active>(a => a.TopicID, true)}");
                    var topicSql = topicExpression
                        .Select(Sql.Count($"{topicExpression.Column<Topic>(x => x.TopicName)}")).ToSelectStatement();

                    expression.Select<User, Rank, Active, ActiveAccess>(
                        (u, r, a, x) => new
                        {
                            a.UserID,
                            UserName = u.Name,
                            UserDisplayName = u.DisplayName,
                            a.IP,
                            a.SessionID,
                            a.ForumID,
                            HasForumAccess = x.ReadAccess,
                            a.TopicID,
                            ForumName = Sql.Custom($"({forumSql})"),
                            TopicName = Sql.Custom($"({topicSql})"),
                            u.IsGuest,
                            IsCrawler = a.Flags & 8,
                            u.IsActiveExcluded,
                            u.UserStyle,
                            u.Suspended,
                            UserCount = 1,
                            a.Login,
                            a.LastActive,
                            a.Location,
                            Active = Sql.Custom(
                                $"DATEDIFF(minute,{expression.Column<Active>(ac => ac.Login, true)}, {expression.Column<Active>(ac => ac.LastActive, true)})"),
                            a.Browser,
                            a.Platform,
                            ForumPage = a.ForumPage == null ? "MAINPAGE" : a.ForumPage
                        });

                    return db.Connection.Select<dynamic>(expression);
                });
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
        /// The <see cref="dynamic"/>.
        /// </returns>
        public static dynamic Stats(this IRepository<Active> repository, int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<Active>();

                    expression.Join<User>((x, u) => u.ID == x.UserID);

                    expression.Where<User>(
                        u => u.BoardID == boardId && u.IsActiveExcluded == false);

                    // -- count Members
                    var countMembersExpression = OrmLiteConfig.DialectProvider.SqlExpression<Active>();

                    countMembersExpression.Join<User>((x, u) => u.ID == x.UserID);

                    countMembersExpression.Where<Active, User>(
                        (x, u) => x.BoardID == boardId && u.IsActiveExcluded == false && u.IsApproved == true);

                    var countMembersSql = countMembersExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Guests
                    var countGuestsExpression = OrmLiteConfig.DialectProvider.SqlExpression<Active>();

                    countGuestsExpression.Join<User>((x, u) => u.ID == x.UserID);

                    countGuestsExpression.Where<Active, User>(
                        (x, u) => x.BoardID == boardId && u.IsGuest == true);

                    var countGuestsSql = countMembersExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    // -- count Hidden
                    var countHiddenExpression = OrmLiteConfig.DialectProvider.SqlExpression<Active>();

                    countHiddenExpression.Join<User>((x, u) => u.ID == x.UserID);

                    countHiddenExpression.Where<Active, User>(
                        (x, u) => x.BoardID == boardId && u.IsApproved == true && u.IsActiveExcluded == true);

                    var countHiddenSql = countHiddenExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                    expression.Select<Active>(
                        x => new
                        {
                            ActiveUsers = Sql.Count("1"),
                            ActiveMembers = Sql.Custom($"({countMembersSql})"),
                            ActiveGuests = Sql.Custom($"({countGuestsSql})"),
                            ActiveHidden = Sql.Custom($"({countHiddenSql})")
                        });

                    return db.Connection.Select<dynamic>(expression);
                }).FirstOrDefault();
        }

        /// <summary>
        /// Delete all old
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="activeTime">
        /// The active Time.
        /// </param>
        private static void DeleteActive(
            this IRepository<Active> repository,
            [NotNull] int activeTime)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.DbAccess.Execute(db =>
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Active>();

                expression.Where(
                    $"DATEDIFF(minute, {expression.Column<Active>(x => x.LastActive, true)}, GETUTCDATE()) > {activeTime}");

                return db.Connection.Delete(expression);
            });
        }

        #endregion
    }
}