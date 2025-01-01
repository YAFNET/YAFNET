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

#pragma warning disable S1125
namespace YAF.Core.Model;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

using Microsoft.AspNetCore.Hosting;

using YAF.Types.Models;
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
    public static void Promote(this IRepository<User> repository, int userId)
    {
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
                     x.MinPosts <= rankInfo.Item1.NumPosts).MaxBy(x => x.MinPosts);
        }
        else
        {
            // -- See if user got enough posts for next ladder group
            newRank = BoardContext.Current.GetRepository<Rank>().Get(
                x => x.BoardID == rankInfo.Item2.BoardID && (x.Flags & 2) == 2 &&
                     x.MinPosts <= rankInfo.Item1.NumPosts && x.MinPosts == rankInfo.Item2.MinPosts).MaxBy(x => x.MinPosts);
        }

        if (newRank != null)
        {
            repository.UpdateOnly(() => new User { RankID = newRank.ID }, u => u.ID == userId);
        }
    }

    /// <summary>
    /// Update all User Styles for the Board.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    public static void UpdateStyles(this IRepository<User> repository, int boardId)
    {
        var users = repository.Get(u => u.BoardID == boardId);

        users.ForEach(
            user => repository.UpdateStyle(user.ID));
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
    public static void UpdateStyle(this IRepository<User> repository, int userId)
    {
        var groupStyle = BoardContext.Current.GetRepository<UserGroup>().GetGroupStyleForUser(userId);
        var rankStyle = BoardContext.Current.GetRepository<Rank>().GetRankStyleForUser(userId);

        if (groupStyle.IsSet())
        {
            repository.UpdateOnly(() => new User { UserStyle = groupStyle }, u => u.ID == userId);
        }
        else
        {
            if (rankStyle.IsSet())
            {
                repository.UpdateOnly(() => new User { UserStyle = rankStyle }, u => u.ID == userId);
            }
        }
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
    /// <returns>
    /// The <see cref="long"/>.
    /// </returns>
    public static long BoardMembers(this IRepository<User> repository, int boardId)
    {
        return repository.Count(u => u.BoardID == boardId && (u.Flags & 4) != 4 && (u.Flags & 32) != 32 && (u.Flags & 2) == 2);
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
    /// <returns>
    /// The <see cref="User"/>.
    /// </returns>
    public static User Latest(this IRepository<User> repository, int boardId)
    {
        return repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    expression.Where<User>(u => (u.Flags & 4) != 4 && (u.Flags & 2) == 2 && u.BoardID == boardId);

                    expression.OrderByDescending<User>(u => u.Joined).Take(1);

                    return db.Connection.Single(expression);
                });
    }

    /// <summary>
    /// List the reporters.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// Returns the List of Message Reporters
    /// </returns>
    public static List<Tuple<MessageReportedAudit, User>> MessageReporters(
        this IRepository<User> repository,
        int messageId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<MessageReportedAudit>();

        expression.Join<User>((m, u) => u.ID == m.UserID)
            .Where<MessageReportedAudit>(m => m.MessageID == messageId);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<MessageReportedAudit, User>(expression));
    }

    /// <summary>
    /// List the reporters.
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
    /// <returns>
    /// The List of Message Reporters
    /// </returns>
    public static List<Tuple<User, MessageReportedAudit>> MessageReporter(
        this IRepository<User> repository,
        int messageId,
        int userId)
    {
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
    /// Returns the Last Active Members
    /// </returns>
    public static List<LastActive> LastActive(
        this IRepository<User> repository,
        int boardId,
        int guestUserId,
        DateTime startDate,
        int displayNumber)
    {
        return repository.DbAccess.Execute(
            db =>
                {
                    var provider = OrmLiteConfig.DialectProvider;
                    var expression = provider.SqlExpression<User>();

                    expression.CustomJoin(
                            $"""
                              inner join (select m.{expression.Column<Message>(x => x.UserID)} as ID,
                                                                              Count(m.{expression.Column<Message>(x => x.UserID)}) as {provider.GetQuotedName("NumOfPosts")}
                                                                        from {expression.Table<Message>()} m
                                                                        where m.{expression.Column<Message>(x => x.Posted)} >= {OrmLiteConfig.DialectProvider.GetQuotedValue(startDate, startDate.GetType())}
                                                                        and (m.{expression.Column<Message>(x => x.Flags)} & 16) = 16
                                                                        and (m.{expression.Column<Message>(x => x.Flags)} & 8) != 8
                                                                        group by m.{expression.Column<Message>(x => x.UserID)}
                                                                      ) as counter on {expression.Column<User>(u => u.ID, true)} = counter.ID
                             """)
                        .Where<User>(u => u.BoardID == boardId && u.ID != guestUserId).Select(
                            $"""
                             counter.ID,
                                                         {expression.Column<User>(u => u.Name, true)},
                                                         {expression.Column<User>(u => u.DisplayName, true)},
                                                         {expression.Column<User>(u => u.Suspended, true)},
                                                         {expression.Column<User>(u => u.UserStyle, true)},
                                                         counter.{provider.GetQuotedName("NumOfPosts")}
                             """).Take(displayNumber);

                    return db.Connection
                        .Select<LastActive>(expression).OrderByDescending(x => x.NumOfPosts).ToList();
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
        int userId,
        int? fromUserId,
        int points)
    {
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
        int userId,
        int? fromUserId,
        int points)
    {
        repository.UpdateAdd(() => new User { Points = -points }, u => u.ID == userId);

        if (fromUserId.HasValue)
        {
            BoardContext.Current.GetRepository<ReputationVote>().UpdateOrAdd(fromUserId.Value, userId);
        }
    }

    /// <summary>
    /// Update the User from the Edit User Page
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <param name="rankId">
    /// The rank id.
    /// </param>
    public static void AdminSave(
        this IRepository<User> repository,
        int boardId,
        int userId,
        int flags,
        int rankId)
    {
        repository.UpdateOnly(
            () => new User
                      {
                          BoardID = boardId,
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
    public static void Approve(this IRepository<User> repository, int userId)
    {
        repository.Approve(repository.GetById(userId));
    }

    /// <summary>
    /// Approves the User
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    public static void Approve(this IRepository<User> repository, User user)
    {
        var userFlags = user.UserFlags;

        if (userFlags.IsApproved)
        {
            return;
        }

        userFlags.IsApproved = true;

        repository.UpdateOnly(() => new User { Flags = userFlags.BitValue }, u => u.ID == user.ID);

        // Send welcome mail/pm to user
        BoardContext.Current.Get<ISendNotification>().SendUserWelcomeNotificationAsync(user);
    }

    /// <summary>
    /// The user AspNet.
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
    /// <param name="displayName">
    /// The display Name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    /// <param name="pageSize">
    /// The default page size.
    /// </param>
    /// <param name="isApproved">
    /// The is approved.
    /// </param>
    /// <param name="existingUser">
    /// The existing User.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static int AspNet(
        this IRepository<User> repository,
        int boardId,
        string userName,
        string displayName,
        string email,
        string providerUserKey,
        int pageSize,
        bool isApproved,
        User existingUser = null)
    {
        var approvedFlag = 0;
        int userId;

        if (isApproved)
        {
            approvedFlag = 2;
        }

        User user;

        if (existingUser == null)
        {
            user = repository.GetSingle(
                u => u.BoardID == boardId && (u.ProviderUserKey == providerUserKey || u.Name == userName));
        }
        else
        {
            user = existingUser;
        }

        var updateExisting = false;

        if (user != null)
        {
            userId = user.ID;
            var flags = user.UserFlags;

            if (isApproved && !flags.IsApproved)
            {
                flags.IsApproved = true;

                updateExisting = true;
            }

            if (displayName.IsNotSet())
            {
                displayName = user.DisplayName;

                updateExisting = true;
            }

            if (updateExisting)
            {
                repository.UpdateOnly(
                    () => new User
                              {
                                  DisplayName = displayName,
                                  Email = email,
                                  Flags = flags.BitValue
                              },
                    u => u.ID == user.ID);
            }
        }
        else
        {
            var rankId = BoardContext.Current.GetRepository<Rank>()
                .GetSingle(r => r.BoardID == boardId && (r.Flags & 1) == 1).ID;

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
                        Email = email,
                        Joined = DateTime.UtcNow,
                        LastVisit = DateTime.UtcNow,
                        NumPosts = 0,
                        TimeZone = TimeZoneInfo.Local.Id,
                        Flags = approvedFlag,
                        ProviderUserKey = providerUserKey,
                        PageSize = pageSize
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
    /// <param name="user">
    /// The user that will be deleted
    /// </param>
    public static void Delete(this IRepository<User> repository, User user)
    {
        var guestUserId = BoardContext.Current.GuestUserID;

        if (guestUserId == user.ID)
        {
            return;
        }

        BoardContext.Current.GetRepository<Message>().UpdateOnly(
            () => new Message { UserName = user.Name, UserDisplayName = user.DisplayName, UserID = guestUserId },
            u => u.UserID == user.ID);
        BoardContext.Current.GetRepository<Topic>().UpdateOnly(
            () => new Topic { UserName = user.Name, UserDisplayName = user.DisplayName, UserID = guestUserId },
            u => u.UserID == user.ID);
        BoardContext.Current.GetRepository<Topic>().UpdateOnly(
            () => new Topic { LastUserName = user.Name, LastUserDisplayName = user.DisplayName, LastUserID = guestUserId },
            u => u.LastUserID == user.ID);
        BoardContext.Current.GetRepository<Forum>().UpdateOnly(
            () => new Forum { LastUserName = user.Name, LastUserDisplayName = user.DisplayName, LastUserID = guestUserId },
            u => u.LastUserID == user.ID);

        BoardContext.Current.GetRepository<Active>().Delete(x => x.UserID == user.ID);
        BoardContext.Current.GetRepository<Activity>().Delete(x => x.FromUserID == user.ID || x.UserID == user.ID);
        BoardContext.Current.GetRepository<EventLog>().Delete(x => x.UserID == user.ID);
        BoardContext.Current.GetRepository<PrivateMessage>().Delete(x => x.FromUserId == user.ID);
        BoardContext.Current.GetRepository<PrivateMessage>().Delete(x => x.ToUserId == user.ID);

        // legacy private messages
        if (BoardContext.Current.GetRepository<UserPMessage>().TableExists())
        {
            BoardContext.Current.GetRepository<UserPMessage>().Delete(x => x.UserID == user.ID);
            BoardContext.Current.GetRepository<PMessage>().Delete(x => x.FromUserID == user.ID);
        }

        BoardContext.Current.GetRepository<Thanks>().Delete(x => x.ThanksFromUserID == user.ID || x.ThanksToUserID == user.ID);
        BoardContext.Current.GetRepository<Buddy>().Delete(x => x.FromUserID == user.ID);
        BoardContext.Current.GetRepository<Buddy>().Delete(x => x.ToUserID == user.ID);
        BoardContext.Current.GetRepository<Attachment>().Delete(x => x.UserID == user.ID);
        BoardContext.Current.GetRepository<CheckEmail>().Delete(x => x.UserID == user.ID);
        BoardContext.Current.GetRepository<WatchTopic>().Delete(x => x.UserID == user.ID);
        BoardContext.Current.GetRepository<WatchForum>().Delete(x => x.UserID == user.ID);
        BoardContext.Current.GetRepository<TopicReadTracking>().Delete(x => x.UserID == user.ID);
        BoardContext.Current.GetRepository<ForumReadTracking>().Delete(x => x.UserID == user.ID);

        // -- Delete user albums
        var albums = BoardContext.Current.GetRepository<UserAlbum>().ListByUser(user.ID);

        albums.ForEach(
            album =>
            {
                BoardContext.Current.GetRepository<UserAlbumImage>().Delete(x => x.AlbumID == album.ID);
            });

        BoardContext.Current.GetRepository<UserAlbum>().Delete(x => x.UserID == user.ID);

        BoardContext.Current.GetRepository<ReputationVote>().Delete(x => x.ReputationFromUserID == user.ID);
        BoardContext.Current.GetRepository<ReputationVote>().Delete(x => x.ReputationToUserID == user.ID);
        BoardContext.Current.GetRepository<UserGroup>().Delete(x => x.UserID == user.ID);

        BoardContext.Current.GetRepository<UserForum>().Delete(x => x.UserID == user.ID);
        BoardContext.Current.GetRepository<IgnoreUser>().Delete(x => x.UserID == user.ID);
        BoardContext.Current.GetRepository<AdminPageUserAccess>().Delete(x => x.UserID == user.ID);

        BoardContext.Current.GetRepository<ProfileCustom>().Delete(x => x.UserID == user.ID);

        repository.DeleteById(user.ID);

        repository.FireDeleted(user.ID);
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
    public static void DeleteAvatar(this IRepository<User> repository, int userId)
    {
        var user = repository.GetById(userId);

        // Delete File if Avatar was uploaded
        if (user.Avatar.IsSet() && user.Avatar.StartsWith('/')
                                && !user.Avatar.Contains(BoardContext.Current.Get<BoardFolders>().Avatars))
        {
            var filePath =
                $"{BoardContext.Current.Get<IWebHostEnvironment>().WebRootPath}{user.Avatar.Replace("/", Path.DirectorySeparatorChar.ToString())}";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

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
    public static void DeleteOld(this IRepository<User> repository, int boardId, int days)
    {
        var users = repository.Get(u => u.BoardID == boardId && (u.Flags & 2) != 2)
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
    /// Gets all Users that Watch that topic or the Forum
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
    /// <returns>
    /// List of the Users
    /// </returns>
    public static List<User> WatchMailList(
        this IRepository<User> repository,
        int topicId,
        int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<WatchTopic>();

        expression.Join<User>((a, b) => b.ID == a.UserID).Where<WatchTopic, User>(
            (a, b) => b.ID != userId && b.NotificationType != 10 &&
                      a.TopicID == topicId && (b.Flags & 2) == 2 && (b.Flags & 4) != 4 && (b.Flags & 32) != 32).Select<User>(x => x);

        var expression2 = OrmLiteConfig.DialectProvider.SqlExpression<WatchForum>();

        expression2.Join<User>((a, b) => b.ID == a.UserID).Join<Topic>((a, c) => c.ForumID == a.ForumID)
            .Where<WatchForum, User, Topic>(
                (a, b, c) => b.ID != userId && b.NotificationType != 10 &&
                             c.ID == topicId && (b.Flags & 2) == 2 && (b.Flags & 4) != 4 && (b.Flags & 32) != 32).Select<User>(x => x);

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
    /// Returns all Users Emails from the Group
    /// </returns>
    public static List<string> GroupEmails(this IRepository<User> repository, int groupId)
    {
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
    public static User GetUserByProviderKey(this IRepository<User> repository, int? boardId, string providerUserKey)
    {
        return boardId == null
                   ? repository.GetSingle(u => u.ProviderUserKey == providerUserKey)
                   : repository.GetSingle(u => u.BoardID == boardId && u.ProviderUserKey == providerUserKey);
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
    /// <returns>
    /// Returns the Stats.
    /// </returns>
    public static dynamic MaxAlbumData(
        this IRepository<User> repository,
        int userId,
        int boardId)
    {
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
    /// <param name="userId">
    /// The User Id
    /// </param>
    /// <param name="boardId">
    /// The Board Id
    /// </param>
    /// <returns>
    /// Returns the Stats
    /// </returns>
    public static dynamic SignatureData(
        this IRepository<User> repository,
        int userId,
        int boardId)
    {
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

        try
        {
            data.UsrSigChars = groupMax.usrSigChars > rankMax.usrSigChars ? groupMax.usrSigChars : rankMax.usrSigChars;
        }
        catch (Exception)
        {
            data.UsrSigChars = 0;
        }

        if (rankMax.usrSigBBCodes.IsSet() && groupMax.usrSigBBCodes.IsSet())
        {
            data.UsrSigBBCodes = groupMax.usrSigBBCodes.Length > rankMax.usrSigBBCodes.Length ? groupMax.usrSigBBCodes : rankMax.usrSigBBCodes;
        }
        else
        {
            data.UsrSigBBCodes = groupMax.usrSigBBCodes.IsSet() ? groupMax.usrSigBBCodes : string.Empty;
        }

        return data;
    }

    /// <summary>
    /// To return a rather rarely updated active user data
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The UserID. It is always should have a positive &gt; 0 value.
    /// </param>
    /// <param name="boardId">
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
    /// <returns>
    /// The <see cref="UserLazyData"/>.
    /// </returns>
    public static UserLazyData LazyData(
        this IRepository<User> repository,
        int userId,
        int boardId,
        bool showPendingBuddies,
        bool showUnreadPMs,
        bool showUserAlbums)
    {
        return repository.DbAccess.Execute(
            db =>
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                expression.Where<User>(u => u.ID == userId);

                // -- moderate Posts
                var moderatePostsExpression = OrmLiteConfig.DialectProvider.SqlExpression<Message>()
                    .Join<Topic>((a, b) => b.ID == a.TopicID).Join<Topic, Forum>((b, c) => c.ID == b.ForumID)
                    .Join<Forum, Category>((c, d) => d.ID == c.CategoryID)
                    .Join<Topic, ActiveAccess>((b, access) => access.ForumID == b.ForumID);

                moderatePostsExpression.Where<Message, Topic, Forum, Category, ActiveAccess>(
                    (a, b, c, d, access) =>
                        (a.Flags & 128) == 128 && (a.Flags & 8) != 8 && (b.Flags & 8) != 8 &&
                        d.BoardID == boardId && access.ModeratorAccess && access.UserID == userId ||
                        (a.Flags & 16) != 16 && (a.Flags & 8) != 8 && (b.Flags & 8) != 8 &&
                        d.BoardID == boardId && (d.Flags & 1) == 1 && access.ModeratorAccess &&
                        access.UserID == userId);

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

                // -- count Watch Topics
                var countWatchTopicsExpression = OrmLiteConfig.DialectProvider.SqlExpression<Activity>();

                countWatchTopicsExpression.Where(
                    a => a.UserID == userId && a.Notification &&
                         ((a.Flags & 8192) == 8192 || (a.Flags & 16384) == 16384));

                var countWatchTopicsSql = countWatchTopicsExpression.Select(Sql.Count("1"))
                    .ToMergedParamsSelectStatement();

                var countUnreadSql = "0";

                if (showUnreadPMs)
                {
                    // -- count Unread
                    var countUnreadExpression = OrmLiteConfig.DialectProvider.SqlExpression<PrivateMessage>();

                    countUnreadExpression.Where(
                        x => x.ToUserId == userId && (x.Flags & 1) != 1);

                    countUnreadSql = countUnreadExpression.Select(Sql.Count("1"))
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

                var hasBuddiesSql = hasBuddiesExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                // -- has Private Messages
                var hasPmsExpression = OrmLiteConfig.DialectProvider.SqlExpression<PrivateMessage>();

                hasPmsExpression.Where(x =>
                        x.FromUserId == userId && (x.Flags & 2) != 2 || x.ToUserId == userId && (x.Flags & 4) != 4)
                    .Limit(1);

                var hasPmsSql = hasPmsExpression.Select(Sql.Count("1")).ToMergedParamsSelectStatement();

                expression.Take(1).Select<User>(
                    a => new {
                        a.ProviderUserKey,
                        a.Suspended,
                        a.SuspendedReason,
                        TimeZoneUser = a.TimeZone,
                        IsGuest =
                            Sql.Custom<bool>(
                                $"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<User>(x => x.Flags, true)}&4")})"),
                        ModeratePosts = Sql.Custom($"({moderatePostsSql})"),
                        WatchTopic = Sql.Custom($"({countWatchTopicsSql})"),
                        ReceivedThanks = Sql.Custom($"({countThanksSql})"),
                        Mention = Sql.Custom($"({countMentionSql})"),
                        Quoted = Sql.Custom($"({countQuotedSql})"),
                        UnreadPrivate = Sql.Custom($"({countUnreadSql})"),
                        PendingBuddies = Sql.Custom($"({countBuddiesSql})"),
                        LastPendingBuddies = Sql.Custom($"({lastBuddySql})"),
                        NumAlbums = Sql.Custom($"({countAlbumsSql})"),
                        UserHasBuddies =
                            Sql.Custom(
                                $"sign({OrmLiteConfig.DialectProvider.IsNullFunction(hasBuddiesSql, 0)})"),
                        UserHasPrivateConversations = Sql.Custom(
                            $"sign({OrmLiteConfig.DialectProvider.IsNullFunction(hasPmsSql, 0)})")
                    });

                return db.Connection.Single<UserLazyData>(expression);
            });
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
    /// <param name="activity">
    /// The activity.
    /// </param>
    /// <param name="pageSize">
    /// The page Size.
    /// </param>
    public static void Save(
        this IRepository<User> repository,
        int userId,
        string timeZone,
        string languageFile,
        string culture,
        string themeFile,
        bool hideUser,
        bool activity,
        int pageSize)
    {
        var user = repository.GetById(userId);

        var flags = user.UserFlags;

        // -- set user dirty
        flags.IsDirty = true;
        flags.IsActiveExcluded = hideUser;

        repository.UpdateOnly(
            () => new User
                      {
                          Activity = activity,
                          TimeZone = timeZone,
                          LanguageFile = languageFile,
                          ThemeFile = themeFile,
                          Culture = culture,
                          Flags = flags.BitValue,
                          PageSize = pageSize
                      },
            u => u.ID == userId);
    }

    /// <summary>
    /// The update display name.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="displayName">
    /// The display name.
    /// </param>
    public static void UpdateDisplayName(
        this IRepository<User> repository,
        User user,
        string displayName)
    {
        var updateDisplayName = false;

        var oldDisplayName = user.DisplayName;

        if (displayName.IsNotSet())
        {
            displayName = user.DisplayName;
        }
        else
        {
            updateDisplayName = displayName != oldDisplayName;
        }

        if (!updateDisplayName)
        {
            return;
        }

        repository.UpdateOnly(
            () => new User
                      {
                          DisplayName = displayName
                      },
            u => u.ID == user.ID);

        // -- here we sync a new display name everywhere
        BoardContext.Current.GetRepository<Forum>().UpdateOnly(
            () => new Forum { LastUserDisplayName = displayName },
            x => x.LastUserID == user.ID &&
                 (x.LastUserDisplayName == null || x.LastUserDisplayName == oldDisplayName));

        BoardContext.Current.GetRepository<Topic>().UpdateOnly(
            () => new Topic { LastUserDisplayName = displayName },
            x => x.LastUserID == user.ID &&
                 (x.LastUserDisplayName == null || x.LastUserDisplayName == oldDisplayName));

        BoardContext.Current.GetRepository<Topic>().UpdateOnly(
            () => new Topic { UserDisplayName = displayName },
            x => x.UserID == user.ID &&
                 (x.UserDisplayName == null || x.UserDisplayName == oldDisplayName));

        BoardContext.Current.GetRepository<Message>().UpdateOnly(
            () => new Message { UserDisplayName = displayName },
            x => x.UserID == user.ID &&
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
        int userId,
        string avatarUrl,
        Stream stream,
        string avatarImageType)
    {
        if (avatarUrl == null)
        {
            byte[] data = null;

            if (stream != null)
            {
                data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.ReadExactly(data, 0, stream.Length.ToType<int>());
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
        int userId,
        bool autoWatchTopics,
        int? notificationType,
        bool dailyDigest)
    {
        repository.UpdateOnly(
            () => new User
                      {
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
    /// <param name="boardId">
    /// The board identifier.
    /// </param>
    /// <returns>
    /// Returns List with all Admin. Users
    /// </returns>
    public static List<User> ListAdmins(
        this IRepository<User> repository,
        int? boardId = null)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

        expression.Join<VAccess>((u, v) => v.UserID == u.ID).Where<VAccess, User>(
            (v, u) => u.BoardID == (boardId ?? repository.BoardID) && (u.Flags & 4) != 4 && v.IsAdmin > 0 &&
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
    /// Returns all Unapproved Users
    /// </returns>
    public static List<User> GetUnApprovedUsers(this IRepository<User> repository, int boardId)
    {
        return repository.Get(u => u.BoardID == boardId && (u.Flags & 2) != 2 && (u.Flags & 32) != 32);
    }

    /// <summary>
    /// Saves the signature.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="signature">The signature.</param>
    public static void SaveSignature(
        this IRepository<User> repository,
        int userId,
        string signature)
    {
        repository.UpdateOnly(() => new User { Signature = signature }, u => u.ID == userId);
    }

    /// <summary>
    /// Sets the points.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="points">The points.</param>
    public static void SetPoints(this IRepository<User> repository, int userId, int points)
    {
        repository.UpdateOnly(() => new User { Points = points }, u => u.ID == userId);
    }

    /// <summary>
    /// Suspends or Un-suspend the User
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="suspend">The suspend.</param>
    /// <param name="suspendReason">The suspend reason.</param>
    /// <param name="suspendBy">The suspend by.</param>
    public static void Suspend(
        this IRepository<User> repository,
        int userId,
        DateTime? suspend = null,
        string suspendReason = null,
        int suspendBy = 0)
    {
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
    public static void UpdateBlockFlags(this IRepository<User> repository, int userId, int flags)
    {
        repository.UpdateOnly(() => new User { BlockFlags = flags }, u => u.ID == userId);
    }

    /// <summary>
    /// Gets the list of recently (last 24 hours) logged in users.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <returns>
    /// The list of users.
    /// </returns>
    public static List<ActiveUser> GetRecentUsers(
        this IRepository<User> repository)
    {
        var timeSinceLastLogin = DateTime.UtcNow.AddMinutes(0 - 60 * 24 * 30);

        var users = repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    expression.Where<User>(
                        u => (u.Flags & 2) == 2 && u.BoardID == repository.BoardID && (u.Flags & 4) != 4 &&
                             timeSinceLastLogin < u.LastVisit);

                    expression.OrderBy(u => u.LastVisit);

                    expression.Select<User>(
                        u => new
                                 {
                                     UserID = u.ID,
                                     UserName = u.Name,
                                     UserDisplayName = u.DisplayName,
                                     IsCrawler = 0,
                                     UserCount = 1,
                                     IsActiveExcluded = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<User>(x => x.Flags, true)}&16")})"),
                                     IsGuest = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<User>(x => x.Flags, true)}&4")})"),
                                     u.UserStyle,
                                     u.Suspended,
                                     u.LastVisit
                                 });

                    return db.Connection.Select<ActiveUser>(expression);
                });

        return users;
    }

    /// <summary>
    /// Gets the forum moderators.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <returns>
    /// Returns the List of Forum Moderators
    /// </returns>
    public static List<SimpleModerator> GetForumModerators(
        this IRepository<User> repository)
    {
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
                                                 CategoryID = Sql.Custom("NULL"),
                                                 CategoryName = Sql.Custom("NULL"),
                                                 a.ForumID,
                                                 ForumName = f.Name,
                                                 f.ParentID,
                                                 ModeratorID = a.GroupID,
                                                 b.Name,
                                                 Email = b.Name,
                                                 ModeratorBlockFlags = 0,
                                                 Avatar = b.Name,
                                                 AvatarImage = Sql.Custom("NULL"),
                                                 DisplayName = b.Name,
                                                 b.Style,
                                                 IsGroup = 1,
                                                 Suspended = Sql.Custom("NULL")
                                             });

                    var expression2 = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    expression2
                        .Join<VaccessGroup>((usr, access) => access.UserID == usr.ID)
                        .Join<VaccessGroup, Forum>((access, f) => f.ID == access.ForumID)
                        .Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                        .Where<VaccessGroup, User>((x, u) => x.ModeratorAccess > 0 && u.BoardID == repository.BoardID)
                        .Select<User, VaccessGroup, Forum, Category>(
                            (usr, access, f, c) => new
                                                       {
                                                           CategoryID = c.ID,
                                                           CategoryName = c.Name,
                                                           access.ForumID,
                                                           ForumName = f.Name,
                                                           f.ParentID,
                                                           ModeratorID = usr.ID,
                                                           usr.Name,
                                                           usr.Email,
                                                           ModeratorBlockFlags = usr.BlockFlags,
                                                           usr.Avatar,
                                                           AvtatarImage = usr.AvatarImage,
                                                           usr.DisplayName,
                                                           Style = usr.UserStyle,
                                                           IsGroup = 0,
                                                           usr.Suspended
                                                       });

                    return db.Connection.Select<SimpleModerator>(
                            $"{expression.ToMergedParamsSelectStatement()} union all {expression2.ToMergedParamsSelectStatement()}")
                        .OrderByDescending(x => x.IsGroup).ThenBy(x => x.Name).ToList();
                });
    }
}