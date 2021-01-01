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
    using System.Data.SqlClient;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;

    using ServiceStack.OrmLite;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;
    using YAF.Types.Objects;
    using YAF.Types.Objects.Model;

    /// <summary>
    /// The User Repository Extensions
    /// </summary>
    public static class UserRepositoryExtensions
    {
        /// <summary>
        /// Upgrade user, i.e. promote rank if conditions allow it
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public static void Promote(this IRepository<User> repository, [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            // -- Get user and rank information
            var rankInfo = BoardContext.Current.GetRepository<Rank>().GetUserAndRank(userId);

            if (!rankInfo.Item2.RankFlags.IsLadder)
            {
                // -- If user isn't member of a ladder rank, exit
                return;
            }

            Rank newRank;

            // -- does user have rank from his board?
            if (rankInfo.Item2.BoardID != rankInfo.Item1.BoardID)
            {
                // -- get highest rank user can get
                newRank = BoardContext.Current.GetRepository<Rank>().Get(
                    x => x.BoardID == rankInfo.Item2.BoardID && (x.Flags & 2) == 2 &&
                         x.MinPosts <= rankInfo.Item1.NumPosts).OrderByDescending(x => x.MinPosts).FirstOrDefault();
            }
            else
            {
                // -- See if user got enough posts for next ladder group
                newRank = BoardContext.Current.GetRepository<Rank>().Get(
                        x => x.BoardID == rankInfo.Item2.BoardID && (x.Flags & 2) == 2 &&
                             x.MinPosts <= rankInfo.Item1.NumPosts && x.MinPosts == rankInfo.Item2.MinPosts)
                    .OrderByDescending(x => x.MinPosts).FirstOrDefault();
            }

            if (newRank != null)
            {
                repository.UpdateOnly(() => new User { RankID = newRank.ID }, u => u.ID == userId);
            }
        }

        /// <summary>
        /// Update all User Styles for the the Board.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        public static void UpdateStyles(this IRepository<User> repository, [NotNull] int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    // TODO : Add typed for Update From Table
                    return db.Connection.ExecuteSql(
                        $@" update d 
                               set d.UserStyle = ISNULL((select top 1 f.Style FROM {expression.Table<UserGroup>()} e join {expression.Table<Group>()} f on f.GroupID=e.GroupID
                                     WHERE f.Style IS NOT NULL and e.UserID = d.UserID order by f.SortOrder),
                                    (SELECT TOP 1 r.Style FROM {expression.Table<Rank>()} r
                                    join {expression.Table<User>()} u on u.RankID = r.RankID
                                    where u.UserID = d.UserID ))
                               from  {expression.Table<User>()} d
		                       where d.BoardID = {boardId}");
                });
        }

        /// <summary>
        /// Update User style
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        public static void UpdateStyle(this IRepository<User> repository, [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    // TODO : Add typed for Update From Table
                    return db.Connection.ExecuteSql(
                        $@" update d 
                               set d.UserStyle = ISNULL((select top 1 f.Style FROM {expression.Table<UserGroup>()} e join {expression.Table<Group>()} f on f.GroupID=e.GroupID
                                     WHERE f.Style IS NOT NULL and e.UserID = d.UserID order by f.SortOrder),
                                    (SELECT TOP 1 r.Style FROM {expression.Table<Rank>()} r
                                    join {expression.Table<User>()} u on u.RankID = r.RankID
                                    where u.UserID = d.UserID ))
                               from  {expression.Table<User>()} d
		                       where d.UserID = {userId}");
                });
        }

        /// <summary>
        /// Gets the Members count by Board Id.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        public static long BoardMembers(this IRepository<User> repository, [NotNull] int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.Count(u => u.BoardID == boardId && u.IsGuest == false && u.IsApproved == true);
        }

        /// <summary>
        /// Gets the Latest User.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        public static User Latest(this IRepository<User> repository, [NotNull] int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    expression.Where<User>(u => u.IsGuest == false && u.IsApproved == true && u.BoardID == boardId);

                    expression.OrderByDescending<User>(u => u.Joined).Take(1);

                    return db.Connection.Select(expression);
                }).FirstOrDefault();
        }

        /// <summary>
        /// List the reporters as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        public static List<Tuple<MessageReportedAudit, User>> MessageReporters(
            this IRepository<User> repository,
            [NotNull] int messageId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<MessageReportedAudit>();

            expression.Join<User>((m, u) => u.ID == m.UserID)
                .Where<MessageReportedAudit>(m => m.MessageID == messageId);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<MessageReportedAudit, User>(expression));
        }

        /// <summary>
        /// List the reporters as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public static List<Tuple<User, MessageReportedAudit>> MessageReporter(
            this IRepository<User> repository,
            [NotNull] int messageId,
            [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            expression.Join<MessageReportedAudit>((user, m) => m.UserID == userId && m.MessageID == messageId)
                .Where<User>(user => user.ID == userId);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<User, MessageReportedAudit>(expression));
        }

        /// <summary>
        /// Get the Last Active Members
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="guestUserId">
        /// The guest User Id.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="displayNumber">
        /// The display number.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<dynamic> LastActive(
            this IRepository<User> repository,
            [NotNull] int boardId,
            [NotNull] int guestUserId,
            [NotNull] DateTime startDate,
            [NotNull] int displayNumber)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(
              db =>
              {
                  var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                  expression.CustomJoin(
                          $@" inner join (
                                           select m.UserID as ID, Count(m.UserID) as NumOfPosts 
                                           from {expression.Table<Message>()} m
                                           where m.Posted >= '{startDate.ToString(CultureInfo.InvariantCulture)}'
                                           group by m.UserID
                                         ) as counter on {expression.Column<User>(u => u.ID, true)} = counter.ID ")
                      .Where<User>((u) => u.BoardID == boardId && u.ID != guestUserId).Select(
                          $@"counter.[ID],
                            {expression.Column<User>(u => u.Name, true)}, 
                            {expression.Column<User>(u => u.DisplayName, true)},
                            {expression.Column<User>(u => u.Suspended, true)},
                            {expression.Column<User>(u => u.UserStyle, true)}, 
                            counter.NumOfPosts").Take(displayNumber);

                  return db.Connection
                      .Select<dynamic>(expression);
              });
        }

        /// <summary>
        /// Add Reputation Points to the specified user id.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        /// <param name="fromUserId">
        /// From user ID.
        /// </param>
        /// <param name="points">
        /// The points.
        /// </param>
        public static void AddPoints(
            this IRepository<User> repository,
            [NotNull] int userId,
            [CanBeNull] int? fromUserId,
            [NotNull] int points)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.UpdateAdd(() => new User { Points = points }, u => u.ID == userId);

            if (fromUserId.HasValue)
            {
                BoardContext.Current.GetRepository<ReputationVote>().UpdateOrAdd(fromUserId.Value, userId);
            }
        }

        /// <summary>
        /// Remove Reputation Points from the specified user id.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        /// <param name="fromUserId">
        /// From user ID.
        /// </param>
        /// <param name="points">
        /// The points.
        /// </param>
        public static void RemovePoints(
            this IRepository<User> repository,
            [NotNull] int userId,
            [CanBeNull] int? fromUserId,
            [NotNull] int points)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.UpdateAdd(() => new User { Points = -points }, u => u.ID == userId);

            if (fromUserId.HasValue)
            {
                BoardContext.Current.GetRepository<ReputationVote>().UpdateOrAdd(fromUserId.Value, userId);
            }
        }

        /// <summary>
        /// Update the Admin User
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="displayName">
        /// The display Name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="rankID">
        /// The rank id.
        /// </param>
        public static void AdminSave(
            this IRepository<User> repository,
            [NotNull] int boardId,
            [NotNull] int userId,
            [NotNull] string name,
            [NotNull] string displayName,
            [NotNull] string email,
            [NotNull] int flags,
            [NotNull] int rankId)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.UpdateOnly(
                () => new User
                {
                    BoardID = boardId,
                    Name = name,
                    DisplayName = displayName,
                    Email = email,
                    Flags = flags,
                    RankID = rankId
                },
                u => u.ID == userId);
        }

        /// <summary>
        /// Approves the User
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        public static void Approve(this IRepository<User> repository, [NotNull] int userId, [NotNull] string email)
        {
            CodeContracts.VerifyNotNull(repository);

            var userFlags = repository.GetById(userId).UserFlags;

            userFlags.IsApproved = true;

            repository.UpdateOnly(() => new User { Email = email, Flags = userFlags.BitValue }, u => u.ID == userId);
        }

        /// <summary>
        /// The user AspNet.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="displayName">
        /// The display Name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="providerUserKey">
        /// The provider user key.
        /// </param>
        /// <param name="isApproved">
        /// The is approved.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int AspNet(
            this IRepository<User> repository,
            [NotNull] int boardId,
            [NotNull] string userName,
            [CanBeNull] string displayName,
            [NotNull] string email,
            [NotNull] string providerUserKey,
            [NotNull] bool isApproved)
        {
            CodeContracts.VerifyNotNull(repository);

            var approvedFlag = 0;
            int userId;

            if (isApproved)
            {
                approvedFlag = 2;
            }

            var user =  repository.GetSingle(
                u => u.BoardID == boardId && (u.ProviderUserKey == providerUserKey || u.Name == userName));

            if (user != null)
            {
                userId = user.ID;
                var flags = user.UserFlags;

                if (isApproved)
                {
                    flags.IsApproved = true;
                }

                if (displayName.IsNotSet())
                {
                    displayName = user.DisplayName;
                }

                repository.UpdateOnly(
                    () => new User
                    {
                        DisplayName = displayName,
                        Email = email,
                        Flags = flags.BitValue
                    },
                    u => u.ID == user.ID);
            }
            else
            {
                var rankId = BoardContext.Current.GetRepository<Rank>()
                    .GetSingle(r => r.BoardID == boardId && (r.Flags & 2) == 2).ID;

                if (displayName.IsNotSet())
                {
                    displayName = userName;
                }

                userId = repository.Insert(
                    new User
                    {
                        BoardID = boardId,
                        RankID = rankId,
                        Name = userName,
                        DisplayName = displayName,
                        Password = "-",
                        Email = email,
                        Joined = DateTime.UtcNow,
                        LastVisit = DateTime.UtcNow,
                        NumPosts = 0,
                        TimeZone = TimeZoneInfo.Local.Id,
                        Flags = approvedFlag,
                        ProviderUserKey = providerUserKey
                    });
            }

            return userId;
        }

        /// <summary>
        /// Delete the User
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public static void Delete(this IRepository<User> repository, [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            var guestUserId = BoardContext.Current.Get<IAspNetUsersHelper>().GuestUserId;

            if (guestUserId == userId)
            {
                return;
            }

            var user = repository.GetById(userId);

            BoardContext.Current.GetRepository<Message>().UpdateOnly(
                () => new Message { UserName = user.Name, UserDisplayName = user.DisplayName, UserID = guestUserId },
                u => u.UserID == userId);
            BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                () => new Topic { UserName = user.Name, UserDisplayName = user.DisplayName, UserID = guestUserId },
                u => u.UserID == userId);
            BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                () => new Topic { LastUserName = user.Name, LastUserDisplayName = user.DisplayName, LastUserID = guestUserId },
                u => u.LastUserID == userId);
            BoardContext.Current.GetRepository<Forum>().UpdateOnly(
                () => new Forum { LastUserName = user.Name, LastUserDisplayName = user.DisplayName, LastUserID = guestUserId },
                u => u.LastUserID == userId);

            BoardContext.Current.GetRepository<Active>().Delete(x => x.UserID == userId);

            BoardContext.Current.GetRepository<EventLog>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<UserPMessage>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<Thanks>().Delete(x => x.ThanksFromUserID == userId || x.ThanksToUserID == userId);
            BoardContext.Current.GetRepository<FavoriteTopic>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<Buddy>().Delete(x => x.FromUserID == userId);
            BoardContext.Current.GetRepository<Buddy>().Delete(x => x.ToUserID == userId);

            // -- set messages as from guest so the User can be deleted
            BoardContext.Current.GetRepository<PMessage>().UpdateOnly(
                () => new PMessage { FromUserID = guestUserId },
                u => u.FromUserID == userId);

            BoardContext.Current.GetRepository<Attachment>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<CheckEmail>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<WatchTopic>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<WatchForum>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<TopicReadTracking>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<ForumReadTracking>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<UserAlbum>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<ReputationVote>().Delete(x => x.ReputationFromUserID == userId);
            BoardContext.Current.GetRepository<ReputationVote>().Delete(x => x.ReputationToUserID == userId);
            BoardContext.Current.GetRepository<UserGroup>().Delete(x => x.UserID == userId);

            BoardContext.Current.GetRepository<UserForum>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<IgnoreUser>().Delete(x => x.UserID == userId);
            BoardContext.Current.GetRepository<AdminPageUserAccess>().Delete(x => x.UserID == userId);

            repository.DeleteById(userId);

            repository.FireDeleted(userId);
        }

        /// <summary>
        /// Remove User Avatar
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public static void DeleteAvatar(this IRepository<User> repository, [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.UpdateOnly(
                () => new User { AvatarImage = null, Avatar = null, AvatarImageType = null },
                u => u.ID == userId);
        }

        /// <summary>
        /// Deletes all unapproved users older than x days
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="days">
        /// The days.
        /// </param>
        public static void DeleteOld(this IRepository<User> repository, [NotNull] int boardId, [NotNull] int days)
        {
            CodeContracts.VerifyNotNull(repository);

            var users = repository.Get(u => u.BoardID == boardId && u.IsApproved == false)
                .Where(u => (DateTime.UtcNow - u.Joined).Days > days);

            users.ForEach(
                u =>
                {
                    BoardContext.Current.GetRepository<EventLog>().Delete(x => x.UserID == u.ID);
                    BoardContext.Current.GetRepository<CheckEmail>().Delete(x => x.UserID == u.ID);
                    BoardContext.Current.GetRepository<UserGroup>().Delete(x => x.UserID == u.ID);
                    BoardContext.Current.GetRepository<User>().DeleteById(u.ID);
                });
        }

        /// <summary>
        /// The watch mail list as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public static List<User> WatchMailList(
            this IRepository<User> repository,
            [NotNull] int topicId,
            [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<WatchTopic>();

            expression.Join<User>((a, b) => b.ID == a.UserID).Where<WatchTopic, User>(
                (a, b) => b.ID == userId && b.NotificationType != 10 && b.NotificationType != 20 &&
                          a.TopicID == topicId && (a.LastMail == null || a.LastMail < b.LastVisit)).Select<User>(x => x);

            var expression2 = OrmLiteConfig.DialectProvider.SqlExpression<WatchForum>();

            expression2.Join<User>((a, b) => b.ID == a.UserID).Join<Topic>((a, c) => c.ForumID == a.ForumID)
                .Where<WatchForum, User, Topic>(
                    (a, b, c) => b.ID == userId && b.NotificationType != 10 && b.NotificationType != 20 &&
                                 c.ID == topicId && (a.LastMail == null || a.LastMail < b.LastVisit)).Select<User>(x => x);

            return repository.DbAccess.Execute(
                    db => db.Connection.Select<User>(
                        $"{expression.ToMergedParamsSelectStatement()} UNION ALL {expression2.ToMergedParamsSelectStatement()}"))
                .DistinctBy(x => x.ID).ToList();
        }

        /// <summary>
        /// Gets all Emails from User in Group
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> GroupEmails(this IRepository<User> repository, [NotNull] int groupId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            expression.Join<UserGroup>((u, b) => b.UserID == u.ID).Join<UserGroup, Group>((b, c) => c.ID == b.GroupID)
                .Where<User, UserGroup, Group>(
                    (a, b, c) => b.GroupID == groupId && (c.Flags & 2) == 0 && a.Email != null)
                .Select(u => new { u.Email });

            return repository.DbAccess.Execute(db => db.Connection.SqlList<string>(expression));
        }

        /// <summary>
        /// Gets the user id from the Provider User Key
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="providerUserKey">
        /// The provider user key.
        /// </param>
        /// <returns>
        /// Returns the User Id
        /// </returns>
        public static int GetUserId(this IRepository<User> repository, int boardId, [NotNull] string providerUserKey)
        {
            CodeContracts.VerifyNotNull(repository);

            var user = repository.GetSingle(u => u.BoardID == boardId && u.ProviderUserKey == providerUserKey);

            return user?.ID ?? 0;
        }

        /// <summary>
        /// Returns data about albums: allowed number of images and albums
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The userID
        /// </param>
        /// <param name="boardId">
        /// The boardID
        /// </param>
        public static dynamic MaxAlbumData(
            this IRepository<User> repository,
            [NotNull] int userId,
            [NotNull] int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

            var groupMax = repository.DbAccess.Execute(
                db => db.Connection.Single<(int maxAlbum, int maxAlbumImages)>(
                    db.Connection.From<User>().Join<UserGroup>((a, b) => b.UserID == a.ID)
                        .Join<UserGroup, Group>((b, c) => c.ID == b.GroupID)
                        .Where(a => a.ID == userId && a.BoardID == boardId).Select<Group>(
                            c => new { MaxAlbum = Sql.Max(c.UsrAlbums), MaxAlbumImages = Sql.Max(c.UsrAlbumImages) })));

            var rankMax = repository.DbAccess.Execute(
                db => db.Connection.Single<(int maxAlbum, int maxAlbumImages)>(
                    db.Connection.From<User>().Join<Rank>((a, b) => b.ID == a.RankID)
                        .Where(a => a.ID == userId && a.BoardID == boardId).Select<Rank>(
                            b => new { MaxAlbum = b.UsrAlbums, MaxAlbumImages = b.UsrAlbumImages })));

            dynamic data = new ExpandoObject();

            data.UserAlbum = groupMax.maxAlbum > rankMax.maxAlbum ? groupMax.maxAlbum : rankMax.maxAlbum;
            data.UserAlbumImages = groupMax.maxAlbumImages > rankMax.maxAlbumImages ? groupMax.maxAlbumImages : rankMax.maxAlbumImages;

            return data;
        }

        /// <summary>
        /// Returns data about allowed signature tags and character limits
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public static dynamic SignatureData(
            this IRepository<User> repository,
            [NotNull] int userId,
            [NotNull] int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

            var groupMax = repository.DbAccess.Execute(
                db => db.Connection.Single<(string usrSigBBCodes, int usrSigChars)>(
                    db.Connection.From<User>().Join<UserGroup>((a, b) => b.UserID == a.ID)
                        .Join<UserGroup, Group>((b, c) => c.ID == b.GroupID)
                        .Where(a => a.ID == userId && a.BoardID == boardId).Select<Group>(
                            c => new { UsrSigBBCodes = Sql.Max(c.UsrSigBBCodes), UsrSigChars = Sql.Max(c.UsrSigChars) })));

            var rankMax = repository.DbAccess.Execute(
                db => db.Connection.Single<(string usrSigBBCodes, int usrSigChars)>(
                    db.Connection.From<User>().Join<Rank>((a, b) => b.ID == a.RankID)
                        .Where(a => a.ID == userId && a.BoardID == boardId).Select<Rank>(
                            b => new { b.UsrSigBBCodes, b.UsrSigChars })));

            dynamic data = new ExpandoObject();

            data.UsrSigChars = groupMax.usrSigChars > rankMax.usrSigChars ? groupMax.usrSigChars : rankMax.usrSigChars;
            data.UsrSigBBCodes = groupMax.usrSigBBCodes.Length > rankMax.usrSigBBCodes.Length ? groupMax.usrSigBBCodes : rankMax.usrSigBBCodes;

            return data;
        }

        /// <summary>
        /// Gets the Guest User
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// Returns the Guest User Id
        /// </returns>
        public static User GetGuestUser(this IRepository<User> repository, [NotNull] int boardId) =>
            repository.GetSingle(u => u.BoardID == boardId && u.IsGuest == true);

        /// <summary>
        /// To return a rather rarely updated active user data
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The UserID. It is always should have a positive &gt; 0 value.
        /// </param>
        /// <param name="boardID">
        /// The board ID.
        /// </param>
        /// <param name="showPendingBuddies">
        /// The show Pending Buddies.
        /// </param>
        /// <param name="showUnreadPMs">
        /// The show Unread PMs.
        /// </param>
        /// <param name="showUserAlbums">
        /// The show User Albums.
        /// </param>
        /// <param name="styledNicks">
        /// If styles should be returned.
        /// </param>
        public static UserLazyData LazyData(
            this IRepository<User> repository,
            [NotNull] int userId,
            [NotNull] int boardId,
            [NotNull] bool showPendingBuddies,
            [NotNull] bool showUnreadPMs,
            [NotNull] bool showUserAlbums)
        {
            CodeContracts.VerifyNotNull(repository);

            var tries = 0;

            while (true)
            {
                try
                {
                    return repository.DbAccess.Execute(
                        db =>
                        {
                            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                            expression.Where<User>(u => u.ID == userId);

                            // -- moderate Posts 
                            var moderatePostsExpression = OrmLiteConfig.DialectProvider.SqlExpression<Message>()
                                .Join<Topic>((a, b) => b.ID == a.TopicID)
                                .Join<Topic, Forum>((b, c) => c.ID == b.ForumID)
                                .Join<Forum, Category>((c, d) => d.ID == c.CategoryID)
                                .Join<Topic, ActiveAccess>((b, access) => access.ForumID == b.ForumID);

                            moderatePostsExpression.Where<Message, Topic, Forum, Category, ActiveAccess>(
                                (a, b, c, d, access) =>
                                    (a.Flags & 128) == 128 && a.IsDeleted == false && b.IsDeleted == false &&
                                    d.BoardID == boardId && access.ModeratorAccess && access.UserID == userId ||
                                    a.IsApproved == false && a.IsDeleted == false && b.IsDeleted == false &&
                                    d.BoardID == boardId && access.ModeratorAccess && access.UserID == userId);

                            var moderatePostsSql = moderatePostsExpression.Select(Sql.Count("1"))
                                .ToMergedParamsSelectStatement();

                            var countAlbumsSql = "0";

                            if (showUserAlbums)
                            {
                                // -- count Albums
                                var countAlbumsExpression = OrmLiteConfig.DialectProvider.SqlExpression<UserAlbum>();

                                countAlbumsExpression.Where(u => u.UserID == userId);

                                countAlbumsSql = countAlbumsExpression.Select(Sql.Count("1"))
                                    .ToMergedParamsSelectStatement();
                            }

                            // -- count ReceivedThanks
                            var countThanksExpression = OrmLiteConfig.DialectProvider.SqlExpression<Activity>();

                            countThanksExpression.Where(
                                a => a.UserID == userId && (a.Flags & 1024) == 1024 && a.Notification);

                            var countThanksSql = countThanksExpression.Select(Sql.Count("1"))
                                .ToMergedParamsSelectStatement();

                            // -- count Mention
                            var countMentionExpression = OrmLiteConfig.DialectProvider.SqlExpression<Activity>();

                            countMentionExpression.Where(
                                a => a.UserID == userId && (a.Flags & 512) == 512 && a.Notification);

                            var countMentionSql = countMentionExpression.Select(Sql.Count("1"))
                                .ToMergedParamsSelectStatement();

                            // -- count Quoted
                            var countQuotedExpression = OrmLiteConfig.DialectProvider.SqlExpression<Activity>();

                            countQuotedExpression.Where(
                                a => a.UserID == userId && (a.Flags & 1024) == 1024 && a.Notification);

                            var countQuotedSql = countQuotedExpression.Select(Sql.Count("1"))
                                .ToMergedParamsSelectStatement();

                            var countUnreadSql = "0";
                            var lastUnreadSql = "null";

                            if (showUnreadPMs)
                            {
                                // -- count Unread
                                var countUnreadExpression = OrmLiteConfig.DialectProvider.SqlExpression<UserPMessage>();

                                countUnreadExpression.Where(
                                    x => x.UserID == userId && x.IsRead == false && x.IsDeleted == false &&
                                         x.IsArchived == false);

                                countUnreadSql = countUnreadExpression.Select(Sql.Count("1"))
                                    .ToMergedParamsSelectStatement();

                                // -- last Unread
                                var lastUnreadExpression = OrmLiteConfig.DialectProvider.SqlExpression<UserPMessage>()
                                    .Join<PMessage>((a, b) => b.ID == a.PMessageID)
                                    .OrderByDescending<PMessage>(x => x.Created).Limit(1);

                                lastUnreadExpression.Where(
                                    x => x.UserID == userId && x.IsRead == false && x.IsDeleted == false &&
                                         x.IsArchived == false);

                                lastUnreadSql = lastUnreadExpression
                                    .Select(lastUnreadExpression.Column<PMessage>(x => x.Created))
                                    .ToMergedParamsSelectStatement();
                            }

                            var countBuddiesSql = "0";
                            var lastBuddySql = "null";

                            if (showPendingBuddies)
                            {
                                // -- count Buddies
                                var countBuddiesExpression = OrmLiteConfig.DialectProvider.SqlExpression<Buddy>();

                                countBuddiesExpression.Where(x => x.ToUserID == userId && x.Approved == false);

                                countBuddiesSql = countBuddiesExpression.Select(Sql.Count("1"))
                                    .ToMergedParamsSelectStatement();

                                // -- last Buddy
                                var lastBuddyExpression = OrmLiteConfig.DialectProvider.SqlExpression<Buddy>()
                                    .OrderByDescending<Buddy>(x => x.Requested).Limit(1);

                                lastBuddyExpression.Where(x => x.ToUserID == userId && x.Approved == false);

                                lastBuddySql = lastBuddyExpression
                                    .Select(lastBuddyExpression.Column<Buddy>(x => x.Requested))
                                    .ToMergedParamsSelectStatement();
                            }

                            // -- has Buddies
                            var hasBuddiesExpression = OrmLiteConfig.DialectProvider.SqlExpression<Buddy>();

                            hasBuddiesExpression.Where(x => x.FromUserID == userId || x.ToUserID == userId).Limit(1);

                            var hasBuddiesSql = hasBuddiesExpression.Select(Sql.Count("1"))
                                .ToMergedParamsSelectStatement();


                            expression.Select<User>(
                                a => new
                                {
                                    a.ProviderUserKey,
                                    a.Suspended,
                                    a.SuspendedReason,
                                    TimeZoneUser = a.TimeZone,
                                    a.IsGuest,
                                    ModeratePosts = Sql.Custom($"({moderatePostsSql})"),
                                    ReceivedThanks = Sql.Custom($"({countThanksSql})"),
                                    Mention = Sql.Custom($"({countMentionSql})"),
                                    Quoted = Sql.Custom($"({countQuotedSql})"),
                                    UnreadPrivate = Sql.Custom($"({countUnreadSql})"),
                                    LastUnreadPm = Sql.Custom($"({lastUnreadSql})"),
                                    PendingBuddies = Sql.Custom($"({countBuddiesSql})"),
                                    LastPendingBuddies = Sql.Custom($"({lastBuddySql})"),
                                    NumAlbums = Sql.Custom($"({countAlbumsSql})"),
                                    UserHasBuddies = Sql.Custom($"sign(isnull(({hasBuddiesSql}),0))")
                                });

                            return db.Connection.Select<UserLazyData>(expression);
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
                        throw new ApplicationException($"Sql Exception with error number {x.Number} (Tries={tries}", x);
                    }
                }

                ++tries;
            }
        }

        /// <summary>
        /// List Members Paged
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
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <param name="rankId">
        /// The rank id.
        /// </param>
        /// <param name="useStyledNicks">
        /// Return style info.
        /// </param>
        /// <param name="literals">
        /// The literals.
        /// </param>
        /// <param name="exclude">
        /// The exclude.
        /// </param>
        /// <param name="beginsWith">
        /// The begins with.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="sortName">
        /// The sort Name.
        /// </param>
        /// <param name="sortRank">
        /// The sort Rank.
        /// </param>
        /// <param name="sortJoined">
        /// The sort Joined.
        /// </param>
        /// <param name="sortPosts">
        /// The sort Posts.
        /// </param>
        /// <param name="sortLastVisit">
        /// The sort Last Visit.
        /// </param>
        /// <param name="numPosts">
        /// The number of Posts.
        /// </param>
        /// <param name="numPostCompare">
        /// The number of Post Compare.
        /// </param>
        public static List<PagedUser> ListMembersPaged(
            this IRepository<User> repository,
            [CanBeNull] int? boardId,
            [CanBeNull] int? groupId,
            [CanBeNull] int? rankId,
            [NotNull] char startLetter,
            [CanBeNull] string name,
            [CanBeNull] int pageIndex,
            [CanBeNull] int pageSize,
            [CanBeNull] int? sortName,
            [CanBeNull] int? sortRank,
            [CanBeNull] int? sortJoined,
            [CanBeNull] int? sortPosts,
            [CanBeNull] int? sortLastVisit,
            [CanBeNull] int? numPosts,
            [NotNull] int numPostCompare)
        {
            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    Expression<Func<User, bool>> whereCriteria = u => u.BoardID == (boardId ?? repository.BoardID) && u.IsApproved == true;

                    // -- count total
                    var countTotalExpression = db.Connection.From<User>();

                    expression.Join<AspNetUsers>((u, a) => a.Id == u.ProviderUserKey)
                        .Join<Rank>((u, r) => r.ID == u.RankID);

                    countTotalExpression.Where(whereCriteria);

                    expression.Where(whereCriteria);

                    if (startLetter == char.MinValue)
                    {
                        // filter by name 
                        if (name.IsSet())
                        {
                            countTotalExpression.And<User>(u => u.Name.Contains(name) || u.DisplayName.Contains(name));

                            expression.And<User>(u => u.Name.Contains(name) || u.DisplayName.Contains(name));
                        }
                    }
                    else
                    {
                        countTotalExpression.And<User>(
                            u => u.Name.StartsWith(startLetter.ToString()) ||
                                 u.DisplayName.StartsWith(startLetter.ToString()));

                        expression.And<User>(u => u.Name.StartsWith(startLetter.ToString()) ||
                                                  u.DisplayName.StartsWith(startLetter.ToString()));
                    }
                    
                    // filter by posts
                    if (numPosts.HasValue)
                    {
                        switch (numPostCompare)
                        {
                            case 1:
                                countTotalExpression.And<User>(u => u.NumPosts == numPosts.Value);

                                expression.And<User>(u => u.NumPosts == numPosts.Value);
                                break;
                            case 2:
                                countTotalExpression.And<User>(u => u.NumPosts <= numPosts.Value);

                                expression.And<User>(u => u.NumPosts <= numPosts.Value);
                                break;
                            case 3:
                                countTotalExpression.And<User>(u => u.NumPosts >= numPosts.Value);

                                expression.And<User>(u => u.NumPosts >= numPosts.Value);
                                break;
                        }
                    }

                    // filter by rank
                    if (rankId.HasValue)
                    {
                        countTotalExpression.And<User>(u => u.RankID == rankId.Value);

                        expression.And<User>(u => u.RankID == rankId.Value);
                    }

                    // filter by group
                    if (groupId.HasValue)
                    {
                        countTotalExpression.UnsafeAnd(
                            $@"exists(select 1 from {countTotalExpression.Table<UserGroup>()} x 
                                               where x.{countTotalExpression.Column<UserGroup>(x => x.UserID)} = {countTotalExpression.Column<User>(x => x.ID, true)} 
                                               and x.{countTotalExpression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})");

                        expression.UnsafeAnd(
                            $@"exists(select 1 from {expression.Table<UserGroup>()} x 
                                               where x.{expression.Column<UserGroup>(x => x.UserID)} = {expression.Column<User>(x => x.ID, true)} 
                                               and x.{expression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})");
                    }

                    var countTotalSql = countTotalExpression
                        .Select(Sql.Count($"{countTotalExpression.Column<User>(x => x.ID)}")).ToSelectStatement();

                    expression.Select<User, AspNetUsers, Rank>(
                        (u, a, r) => new
                        {
                            UserID = u.ID,
                            u.Name,
                            u.DisplayName,
                            u.Flags,
                            u.Suspended,
                            u.UserStyle,
                            u.Avatar,
                            u.AvatarImage,
                            u.Email,
                            u.Joined,
                            u.LastVisit,
                            u.NumPosts,
                            u.IsGuest,
                            a.Profile_GoogleId,
                            a.Profile_FacebookId,
                            a.Profile_TwitterId,
                            RankName = r.Name,
                            TotalRows = Sql.Custom($"({countTotalSql})")
                        });

                    // Set Sorting
                    if (sortName.HasValue)
                    {
                        if (sortName.Value == 1)
                        {
                            expression.OrderBy(u => u.Name);
                        }
                        else
                        {
                            expression.OrderByDescending(u => u.Name);
                        }
                    }

                    if (sortRank.HasValue)
                    {
                        if (sortRank.Value == 1)
                        {
                            expression.OrderBy<Rank>(r => r.Name);
                        }
                        else
                        {
                            expression.OrderByDescending<Rank>(r => r.Name);
                        }
                    }

                    if (sortJoined.HasValue)
                    {
                        if (sortJoined.Value == 1)
                        {
                            expression.OrderBy(u => u.Joined);
                        }
                        else
                        {
                            expression.OrderByDescending(u => u.Joined);
                        }
                    }

                    if (sortLastVisit.HasValue)
                    {
                        if (sortLastVisit.Value == 1)
                        {
                            expression.OrderBy(u => u.LastVisit);
                        }
                        else
                        {
                            expression.OrderByDescending(u => u.LastVisit);
                        }
                    }

                    if (sortPosts.HasValue)
                    {
                        if (sortPosts.Value == 1)
                        {
                            expression.OrderBy(u => u.NumPosts);
                        }
                        else
                        {
                            expression.OrderByDescending(u => u.NumPosts);
                        }
                    }

                    // Set Paging
                    expression.Page(pageIndex + 1, pageSize);

                    return db.Connection.Select<PagedUser>(expression);
                });
        }

        /// <summary>
        /// Updates the NNTP User
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// Returns the User ID of the updated user.
        /// </returns>
        public static int UpdateNntpUser(
            this IRepository<User> repository,
            [NotNull] int boardId,
            [NotNull] string userName,
            [CanBeNull] string email)
        {
            CodeContracts.VerifyNotNull(repository);

            var user = repository.GetSingle(u => u.BoardID == boardId && u.Name == userName);

            repository.Save(user.ID, boardId, $"{userName} (NNTP)", null, email, null, null, null, null, true);

            return user.ID;
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="displayName">
        /// the display name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <param name="languageFile">
        /// The language file.
        /// </param>
        /// <param name="culture">
        /// the user culture
        /// </param>
        /// <param name="themeFile">
        /// The theme File.
        /// </param>
        /// <param name="hideUser">
        /// The hide User.
        /// </param>
        public static void Save(
            this IRepository<User> repository,
            [NotNull] int userId,
            [NotNull] int boardId,
            [NotNull] string userName,
            [CanBeNull] string displayName,
            [CanBeNull] string email,
            [CanBeNull] string timeZone,
            [CanBeNull] string languageFile,
            [CanBeNull] string culture,
            [CanBeNull] string themeFile,
            [NotNull] bool hideUser)
        {
            CodeContracts.VerifyNotNull(repository);

            var updateDisplayName = false;
            var user = repository.GetById(userId);

            var oldDisplayName = user.DisplayName;

            var flags = user.UserFlags;

            // -- set user dirty
            flags.IsDirty = true;
            flags.IsActiveExcluded = hideUser;

            if (displayName.IsNotSet())
            {
                displayName = user.DisplayName;
            }
            else
            {
                updateDisplayName = displayName != oldDisplayName;
            }

            if (email.IsNotSet())
            {
                email = user.Email;
            }

            repository.UpdateOnly(
                () => new User
                {
                    TimeZone = timeZone,
                    LanguageFile = languageFile,
                    ThemeFile = themeFile,
                    Culture = culture,
                    Flags = flags.BitValue,
                    DisplayName = displayName,
                    Email = email
                },
                u => u.ID == userId);

            if (!updateDisplayName)
            {
                return;
            }

            // -- here we sync a new display name everywhere
            BoardContext.Current.GetRepository<Forum>().UpdateOnly(
                () => new Forum { LastUserDisplayName = displayName },
                x => x.LastUserID == userId &&
                     (x.LastUserDisplayName == null || x.LastUserDisplayName == oldDisplayName));

            BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                () => new Topic { LastUserDisplayName = displayName },
                x => x.LastUserID == userId &&
                     (x.LastUserDisplayName == null || x.LastUserDisplayName == oldDisplayName));

            BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                () => new Topic { UserDisplayName = displayName },
                x => x.UserID == userId &&
                     (x.UserDisplayName == null || x.UserDisplayName == oldDisplayName));

            BoardContext.Current.GetRepository<Message>().UpdateOnly(
                () => new Message { UserDisplayName = displayName },
                x => x.UserID == userId &&
                     (x.UserDisplayName == null || x.UserDisplayName == oldDisplayName));
        }

        /// <summary>
        /// Save the User Avatar
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="avatarUrl">
        /// The avatar url.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="avatarImageType">
        /// The avatar image type.
        /// </param>
        public static void SaveAvatar(
            this IRepository<User> repository,
            [NotNull] int userId,
            [CanBeNull] string avatarUrl,
            [CanBeNull] Stream stream,
            [CanBeNull] string avatarImageType)
        {
            CodeContracts.VerifyNotNull(repository);

            if (avatarUrl == null)
            {
                byte[] data = null;

                if (stream != null)
                {
                    data = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(data, 0, stream.Length.ToType<int>());
                }

                repository.UpdateOnly(
                    () => new User { Avatar = avatarUrl, AvatarImage = data, AvatarImageType = avatarImageType },
                    u => u.ID == userId);
            }
            else
            {
                repository.UpdateOnly(
                    () => new User { Avatar = avatarUrl, AvatarImage = null, AvatarImageType = null },
                    u => u.ID == userId);
            }
        }

        /// <summary>
        /// Saves the notification type for a user
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="privateNotification">
        /// The pm Notification.
        /// </param>
        /// <param name="autoWatchTopics">
        /// The auto Watch Topics.
        /// </param>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="dailyDigest">
        /// The daily Digest.
        /// </param>
        public static void SaveNotification(
            this IRepository<User> repository,
            [NotNull] int userId,
            [NotNull] bool privateNotification,
            [NotNull] bool autoWatchTopics,
            [CanBeNull] int? notificationType,
            [NotNull] bool dailyDigest)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.UpdateOnly(
                () => new User
                {
                    PMNotification = privateNotification,
                    AutoWatchTopics = autoWatchTopics,
                    NotificationType = notificationType,
                    DailyDigest = dailyDigest
                },
                u => u.ID == userId);
        }

        /// <summary>
        /// Gets the List of Administrators
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use styled nicks.
        /// </param>
        /// <param name="boardId">
        /// The board identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<User> ListAdmins(
            this IRepository<User> repository,
            [NotNull] bool? useStyledNicks = null,
            [NotNull] int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            expression.Join<vaccess>((u, v) => v.UserID == u.ID).Where<vaccess, User>(
                (v, u) => u.BoardID == (boardId ?? repository.BoardID) && u.IsGuest == false && v.IsAdmin &&
                          v.ForumID == 0).OrderBy<User>(u => u.DisplayName);

            return repository.DbAccess.Execute(db => db.Connection.Select(expression));
        }

        /// <summary>
        /// Gets all Unapproved Users by Board Id.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<User> UnApprovedUsers(this IRepository<User> repository, [NotNull] int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.Get(u => u.BoardID == boardId && u.IsApproved == false);
        }

        /// <summary>
        /// Saves the signature.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="signature">The signature.</param>
        public static void SaveSignature(
            this IRepository<User> repository,
            [NotNull] int userId,
            [CanBeNull] string signature)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.UpdateOnly(() => new User { Signature = signature }, u => u.ID == userId);
        }

        /// <summary>
        /// Gets the user points.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Returns the user points</returns>
        public static int GetPoints(this IRepository<User> repository, [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.GetById(userId).Points;
        }

        /// <summary>
        /// Sets the points.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="points">The points.</param>
        public static void SetPoints(this IRepository<User> repository, [NotNull] int userId, [NotNull] int points)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.UpdateOnly(() => new User { Points = points }, u => u.ID == userId);
        }

        /// <summary>
        /// Suspends or Unsuspend the User
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="suspend">The suspend.</param>
        /// <param name="suspendReason">The suspend reason.</param>
        /// <param name="suspendBy">The suspend by.</param>
        public static void Suspend(
            this IRepository<User> repository,
            [NotNull] int userId,
            [NotNull] DateTime? suspend = null,
            [CanBeNull] string suspendReason = null,
            [NotNull] int suspendBy = 0)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.UpdateOnly(
                () => new User { Suspended = suspend, SuspendedReason = suspendReason, SuspendedBy = suspendBy },
                u => u.ID == userId);
        }

        /// <summary>
        /// Updates Block Flags for the User.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        public static void UpdateBlockFlags(this IRepository<User> repository, [NotNull] int userId, [NotNull] int flags)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.UpdateOnly(() => new User { BlockFlags = flags }, u => u.ID == userId);
        }

        /// <summary>
        /// Gets the board user by Id.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        public static Tuple<User, AspNetUsers, Rank, vaccess> GetBoardUser(
            this IRepository<User> repository,
            [NotNull] int userId,
            [CanBeNull] int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            expression.Join<vaccess>((u, v) => v.UserID == u.ID).Join<AspNetUsers>((u, a) => a.Id == u.ProviderUserKey)
                .Join<Rank>((u, r) => r.ID == u.RankID).Where<vaccess, User>(
                    (v, u) => u.ID == userId && u.BoardID == (boardId ?? repository.BoardID) && u.IsApproved == true);

            return repository.DbAccess
                .Execute(db => db.Connection.SelectMulti<User, AspNetUsers, Rank, vaccess>(expression))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the board users.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        public static List<PagedUser> GetUsersPaged(
            this IRepository<User> repository,
            [NotNull] int? boardId,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [CanBeNull] string name,
            [CanBeNull] string email,
            [CanBeNull] DateTime? joinedDate,
            [NotNull] bool onlySuspended,
            [CanBeNull] int? groupId,
            [CanBeNull] int? rankId)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    Expression<Func<User, bool>> whereCriteria = u => u.BoardID == (boardId ?? repository.BoardID) && u.IsApproved == true;

                    // -- count total
                    var countTotalExpression = db.Connection.From<User>();

                    expression.Join<AspNetUsers>((u, a) => a.Id == u.ProviderUserKey)
                        .Join<Rank>((u, r) => r.ID == u.RankID);

                    countTotalExpression.Where(whereCriteria);

                    expression.Where(whereCriteria);

                    // filter by name 
                    if (name.IsSet())
                    {
                        countTotalExpression.And<User>(u => u.Name.Contains(name) || u.DisplayName.Contains(name));

                        expression.And<User>(u => u.Name.Contains(name) || u.DisplayName.Contains(name));
                    }

                    // filter by email
                    if (email.IsSet())
                    {
                        countTotalExpression.And<User>(u => u.Email.Contains(email));

                        expression.And<User>(u => u.Email.Contains(email));
                    }

                    // filter by date of registration
                    if (joinedDate.HasValue)
                    {
                        countTotalExpression.And<User>(u => u.Joined > joinedDate.Value);

                        expression.And<User>(u => u.Joined > joinedDate.Value);
                    }

                    // show only suspended ?
                    if (onlySuspended)
                    {
                        countTotalExpression.And<User>(u => u.Suspended != null);

                        expression.And<User>(u => u.Suspended != null);
                    }

                    // filter by rank
                    if (rankId.HasValue)
                    {
                        countTotalExpression.And<User>(u => u.RankID == rankId.Value);

                        expression.And<User>(u => u.RankID == rankId.Value);
                    }

                    // filter by group
                    if (groupId.HasValue)
                    {
                        countTotalExpression.UnsafeAnd(
                            $@"exists(select 1 from {countTotalExpression.Table<UserGroup>()} x 
                                               where x.{countTotalExpression.Column<UserGroup>(x => x.UserID)} = {countTotalExpression.Column<User>(x => x.ID, true)} 
                                               and x.{countTotalExpression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})");

                        expression.UnsafeAnd(
                            $@"exists(select 1 from {expression.Table<UserGroup>()} x 
                                               where x.{expression.Column<UserGroup>(x => x.UserID)} = {expression.Column<User>(x => x.ID, true)} 
                                               and x.{expression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})");
                    }

                    var countTotalSql = countTotalExpression
                        .Select(Sql.Count($"{countTotalExpression.Column<User>(x => x.ID)}")).ToSelectStatement();

                    expression.Select<User, AspNetUsers, Rank>(
                        (u, a, r) => new
                        {
                            UserID = u.ID,
                            u.Name,
                            u.DisplayName,
                            u.Flags,
                            u.Suspended,
                            u.UserStyle,
                            u.Avatar,
                            u.AvatarImage,
                            u.Email,
                            u.Joined,
                            u.LastVisit,
                            u.NumPosts,
                            u.IsGuest,
                            a.Profile_GoogleId,
                            a.Profile_FacebookId,
                            a.Profile_TwitterId,
                            RankName = r.Name,
                            TotalRows = Sql.Custom($"({countTotalSql})")
                        });

                    expression.OrderBy(u => u.Name);

                    // Set Paging
                    expression.Page(pageIndex + 1, pageSize);

                    return db.Connection.Select<PagedUser>(expression);
                });
        }

        /// <summary>
        /// Get the list of recently logged in users.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="timeSinceLastLogin">
        /// The time since last login in minutes.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use Styled Nicks.
        /// </param>
        /// <returns>
        /// The list of users in Data table format.
        /// </returns>
        public static List<dynamic> GetRecentUsers(
            this IRepository<User> repository,
            [NotNull] int timeSinceLastLogin,
            [NotNull] bool useStyledNicks)
        {
            CodeContracts.VerifyNotNull(repository);

            var users = repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    expression.Where<User>(
                        u => u.IsApproved == true && u.BoardID == repository.BoardID && u.IsGuest == false);
                    expression.And(
                            $"(DATEADD(mi, 0 - {timeSinceLastLogin}, getdate()) < {expression.Column<User>(u => u.LastVisit, true)})")
                        .OrderBy(u => u.LastVisit);

                    expression.Select<User>(
                        u => new
                        {
                            u.ID,
                            UserName = u.Name,
                            UserDisplayName = u.DisplayName,
                            IsCrawler = 0,
                            UserCount = 1,
                            u.IsActiveExcluded,
                            u.UserStyle,
                            u.Suspended,
                            u.LastVisit
                        });

                    return db.Connection.Select<dynamic>(expression);
                });

            return users;
        }

        /// <summary>
        /// Gets the forum moderators as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use styled nicks.
        /// </param>
        public static List<SimpleModerator> GetForumModerators(
            this IRepository<User> repository)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                    expression.Join<ForumAccess>((f, a) => a.ForumID == f.ID)
                        .Join<ForumAccess, Group>((a, b) => b.ID == a.GroupID)
                        .Join<ForumAccess, AccessMask>((a, c) => c.ID == a.AccessMaskID)
                        .Where<Group, AccessMask>((b, c) => b.BoardID == repository.BoardID && (c.Flags & 64) != 0)
                        .Select<Forum, ForumAccess, Group>(
                            (f, a, b) => new 
                            {
                                a.ForumID,
                                ForumName = f.Name,
                                ModeratorID = a.GroupID,
                                b.Name,
                                Email = b.Name,
                                ModeratorBlockFlags = 0,
                                Avatar = b.Name,
                                AvatarImage = 0,
                                DisplayName = b.Name,
                                b.Style,
                                IsGroup = 1,
                                Suspended = 0
                            });

                    // TODO : Create a typed Version
                    var expression2 = @$"select
        ForumID = access.ForumID,
        ForumName = f.Name,
        ModeratorID = usr.UserID,
        Name = usr.Name,
        Email = usr.Email,
		ModeratorBlockFlags = usr.BlockFlags,
        Avatar = ISNULL(usr.Avatar, ''),
        AvatarImage = CAST((select count(1) from {expression.Table<User>()} x where x.UserID=usr.UserID and AvatarImage is not null)as bit),
        DisplayName = usr.DisplayName,
        Style = usr.UserStyle,
        IsGroup=0,
        Suspended = usr.Suspended
    from
        {expression.Table<User>()} usr
        INNER JOIN (
            select
                UserID				= a.UserID,
                ForumID				= x.ForumID,
                ModeratorAccess		= MAX(ModeratorAccess)
            from
                {expression.Table<vaccessfull>()} as x
                INNER JOIN {expression.Table<UserGroup>()} a  on a.UserID=x.UserID
                INNER JOIN {expression.Table<Group>()} b  on b.GroupID=a.GroupID
            WHERE
                b.BoardID = {repository.BoardID} and
		        ModeratorAccess <> 0
            GROUP BY a.UserID, x.ForumID
        ) access ON usr.UserID = access.UserID
        JOIN    {expression.Table<Forum>()} f
        ON f.ForumID = access.ForumID

        JOIN {expression.Table<Rank>()} r
        ON r.RankID = usr.RankID
    where
        access.ModeratorAccess<>0
    order by
        IsGroup desc,
        Name asc";

                    return db.Connection.Select<SimpleModerator>(
                        $"{expression.ToMergedParamsSelectStatement()} union all {expression2}");
                });
        }
    }
}