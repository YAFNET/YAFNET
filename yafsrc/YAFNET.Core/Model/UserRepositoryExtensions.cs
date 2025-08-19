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
    public async static Task PromoteAsync(this IRepository<User> repository, int userId)
    {
        // -- Get user and rank information
        var rankInfo = await BoardContext.Current.GetRepository<Rank>().GetUserAndRankAsync(userId);

        if (!rankInfo.Item2.RankFlags.IsLadder)
        {
            // -- If user isn't member of a ladder rank, exit
            return;
        }

        Rank newRank;

        // -- does user have rank from his board?
        if (rankInfo.Item2.BoardID != rankInfo.Item1.BoardID)
        {
            // -- get the highest rank user can get
            var result = await BoardContext.Current.GetRepository<Rank>().GetAsync(x =>
                x.BoardID == rankInfo.Item2.BoardID && (x.Flags & 2) == 2 &&
                x.MinPosts <= rankInfo.Item1.NumPosts);

            newRank = result.MaxBy(x => x.MinPosts);
        }
        else
        {
            // -- See if user got enough posts for next ladder group
            var result = await BoardContext.Current.GetRepository<Rank>().GetAsync(x =>
                x.BoardID == rankInfo.Item2.BoardID && (x.Flags & 2) == 2 &&
                x.MinPosts <= rankInfo.Item1.NumPosts && x.MinPosts == rankInfo.Item2.MinPosts);

            newRank = result.MaxBy(x => x.MinPosts);
        }

        if (newRank != null)
        {
            await repository.UpdateOnlyAsync(() => new User { RankID = newRank.ID }, u => u.ID == userId);
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
    public async static Task UpdateStylesAsync(this IRepository<User> repository, int boardId)
    {
        var users = await repository.GetAsync(u => u.BoardID == boardId);

        foreach (var user in users)
        {
            await repository.UpdateStyleAsync(user.ID);
        }
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
    public async static Task UpdateStyleAsync(this IRepository<User> repository, int userId)
    {
        var groupStyle = await BoardContext.Current.GetRepository<UserGroup>().GetGroupStyleForUserAsync(userId);
        var rankStyle = await BoardContext.Current.GetRepository<Rank>().GetRankStyleForUserAsync(userId);

        if (groupStyle.IsSet())
        {
            await repository.UpdateOnlyAsync(() => new User { UserStyle = groupStyle }, u => u.ID == userId);
        }
        else
        {
            if (rankStyle.IsSet())
            {
                await repository.UpdateOnlyAsync(() => new User { UserStyle = rankStyle }, u => u.ID == userId);
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
    public static Task<long> BoardMembersAsync(this IRepository<User> repository, int boardId)
    {
        return repository.CountAsync(u =>
            u.BoardID == boardId && (u.Flags & 4) != 4 && (u.Flags & 32) != 32 && (u.Flags & 2) == 2);
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
    public static Task<User> LatestAsync(this IRepository<User> repository, int boardId)
    {
        return repository.DbAccess.ExecuteAsync(db =>
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            expression.Where<User>(u => (u.Flags & 4) != 4 && (u.Flags & 2) == 2 && u.BoardID == boardId);

            expression.OrderByDescending<User>(u => u.Joined).Take(1);

            return db.SingleAsync(expression);
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
    public static Task<List<Tuple<MessageReportedAudit, User>>> MessageReportersAsync(
        this IRepository<User> repository,
        int messageId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<MessageReportedAudit>();

        expression.Join<User>((m, u) => u.ID == m.UserID)
            .Where<MessageReportedAudit>(m => m.MessageID == messageId);

        return repository.DbAccess.ExecuteAsync(db => db.SelectMultiAsync<MessageReportedAudit, User>(expression));
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
    public static Task<List<Tuple<User, MessageReportedAudit>>> MessageReporterAsync(
        this IRepository<User> repository,
        int messageId,
        int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

        expression.Join<MessageReportedAudit>((user, m) => m.UserID == userId && m.MessageID == messageId)
            .Where<User>(user => user.ID == userId);

        return repository.DbAccess.ExecuteAsync(db => db.SelectMultiAsync<User, MessageReportedAudit>(expression));
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
    public async static Task<List<LastActive>> LastActiveAsync(
        this IRepository<User> repository,
        int boardId,
        int guestUserId,
        DateTime startDate,
        int displayNumber)
    {
        return (await repository.DbAccess.ExecuteAsync(db =>
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

            return db.SelectAsync<LastActive>(expression);
        })).OrderByDescending(x => x.NumOfPosts).ToList();
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
    public async static Task AddPointsAsync(
        this IRepository<User> repository,
        int userId,
        int? fromUserId,
        int points)
    {
        await repository.UpdateAddAsync(() => new User { Points = points }, u => u.ID == userId);

        if (fromUserId.HasValue)
        {
            await BoardContext.Current.GetRepository<ReputationVote>().UpdateOrAddAsync(fromUserId.Value, userId);
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
    public async static Task RemovePointsAsync(
        this IRepository<User> repository,
        int userId,
        int? fromUserId,
        int points)
    {
        await repository.UpdateAddAsync(() => new User { Points = -points }, u => u.ID == userId);

        if (fromUserId.HasValue)
        {
            await BoardContext.Current.GetRepository<ReputationVote>().UpdateOrAddAsync(fromUserId.Value, userId);
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
    public static Task AdminSaveAsync(
        this IRepository<User> repository,
        int boardId,
        int userId,
        int flags,
        int rankId)
    {
        return repository.UpdateOnlyAsync(
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
    public async static Task ApproveAsync(this IRepository<User> repository, int userId)
    {
        await repository.ApproveAsync(await repository.GetByIdAsync(userId));
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
    public async static Task ApproveAsync(this IRepository<User> repository, User user)
    {
        var userFlags = user.UserFlags;

        if (userFlags.IsApproved)
        {
            return;
        }

        userFlags.IsApproved = true;

        await repository.UpdateOnlyAsync(() => new User { Flags = userFlags.BitValue }, u => u.ID == user.ID);

        // Send welcome mail/pm to user
        await BoardContext.Current.Get<ISendNotification>().SendUserWelcomeNotificationAsync(user);
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
    /// The username.
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
    public async static Task<int> AspNetAsync(
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
            user = await repository.GetSingleAsync(u =>
                u.BoardID == boardId && (u.ProviderUserKey == providerUserKey || u.Name == userName));
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
                await repository.UpdateOnlyAsync(
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
            var rankId = (await BoardContext.Current.GetRepository<Rank>()
                .GetSingleAsync(r => r.BoardID == boardId && (r.Flags & 1) == 1)).ID;

            if (displayName.IsNotSet())
            {
                displayName = userName;
            }

            userId = await repository.InsertAsync(
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
    public async static Task DeleteAsync(this IRepository<User> repository, User user)
    {
        var guestUserId = BoardContext.Current.GuestUserID;

        if (guestUserId == user.ID)
        {
            return;
        }

        await BoardContext.Current.GetRepository<Message>().UpdateOnlyAsync(
            () => new Message { UserName = user.Name, UserDisplayName = user.DisplayName, UserID = guestUserId },
            u => u.UserID == user.ID);
        await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
            () => new Topic { UserName = user.Name, UserDisplayName = user.DisplayName, UserID = guestUserId },
            u => u.UserID == user.ID);
        await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
            () => new Topic
                { LastUserName = user.Name, LastUserDisplayName = user.DisplayName, LastUserID = guestUserId },
            u => u.LastUserID == user.ID);
        await BoardContext.Current.GetRepository<Forum>().UpdateOnlyAsync(
            () => new Forum
                { LastUserName = user.Name, LastUserDisplayName = user.DisplayName, LastUserID = guestUserId },
            u => u.LastUserID == user.ID);

        await BoardContext.Current.GetRepository<Active>().DeleteAsync(x => x.UserID == user.ID);
        await BoardContext.Current.GetRepository<Activity>().DeleteAsync(x => x.FromUserID == user.ID || x.UserID == user.ID);
        await BoardContext.Current.GetRepository<EventLog>().DeleteAsync(x => x.UserID == user.ID);
        await BoardContext.Current.GetRepository<PrivateMessage>().DeleteAsync(x => x.FromUserId == user.ID);
        await BoardContext.Current.GetRepository<PrivateMessage>().DeleteAsync(x => x.ToUserId == user.ID);

        // legacy private messages
        if (await BoardContext.Current.GetRepository<UserPMessage>().TableExistsAsync())
        {
            await BoardContext.Current.GetRepository<UserPMessage>().DeleteAsync(x => x.UserID == user.ID);
            await BoardContext.Current.GetRepository<PMessage>().DeleteAsync(x => x.FromUserID == user.ID);
        }

        await BoardContext.Current.GetRepository<Thanks>()
            .DeleteAsync(x => x.ThanksFromUserID == user.ID || x.ThanksToUserID == user.ID);
        await BoardContext.Current.GetRepository<Buddy>().DeleteAsync(x => x.FromUserID == user.ID);
        await BoardContext.Current.GetRepository<Buddy>().DeleteAsync(x => x.ToUserID == user.ID);
        await BoardContext.Current.GetRepository<Attachment>().DeleteAsync(x => x.UserID == user.ID);
        await BoardContext.Current.GetRepository<CheckEmail>().DeleteAsync(x => x.UserID == user.ID);
        await BoardContext.Current.GetRepository<WatchTopic>().DeleteAsync(x => x.UserID == user.ID);
        await BoardContext.Current.GetRepository<WatchForum>().DeleteAsync(x => x.UserID == user.ID);
        await BoardContext.Current.GetRepository<TopicReadTracking>().DeleteAsync(x => x.UserID == user.ID);
        await BoardContext.Current.GetRepository<ForumReadTracking>().DeleteAsync(x => x.UserID == user.ID);

        // -- Delete user albums
        var albums = BoardContext.Current.GetRepository<UserAlbum>().ListByUser(user.ID);

        foreach (var album in albums)
        {
            await BoardContext.Current.GetRepository<UserAlbumImage>().DeleteAsync(x => x.AlbumID == album.ID);
        }

        await BoardContext.Current.GetRepository<UserAlbum>().DeleteAsync(x => x.UserID == user.ID);

        await BoardContext.Current.GetRepository<ReputationVote>().DeleteAsync(x => x.ReputationFromUserID == user.ID);
        await BoardContext.Current.GetRepository<ReputationVote>().DeleteAsync(x => x.ReputationToUserID == user.ID);
        await BoardContext.Current.GetRepository<UserGroup>().DeleteAsync(x => x.UserID == user.ID);

        await BoardContext.Current.GetRepository<UserForum>().DeleteAsync(x => x.UserID == user.ID);
        await BoardContext.Current.GetRepository<IgnoreUser>().DeleteAsync(x => x.UserID == user.ID);
        await BoardContext.Current.GetRepository<AdminPageUserAccess>().DeleteAsync(x => x.UserID == user.ID);

        await BoardContext.Current.GetRepository<ProfileCustom>().DeleteAsync(x => x.UserID == user.ID);

        await repository.DeleteByIdAsync(user.ID);
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
    public async static Task DeleteAvatarAsync(this IRepository<User> repository, int userId)
    {
        var user = await repository.GetByIdAsync(userId);

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

        await repository.UpdateOnlyAsync(
            () => new User { AvatarImage = null, Avatar = null, AvatarImageType = null },
            u => u.ID == userId);
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
    public async static Task<List<User>> WatchMailListAsync(
        this IRepository<User> repository,
        int topicId,
        int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<WatchTopic>();

        expression.Join<User>((a, b) => b.ID == a.UserID).Where<WatchTopic, User>((a, b) => b.ID != userId &&
                b.NotificationType != 10 &&
                a.TopicID == topicId && (b.Flags & 2) == 2 && (b.Flags & 4) != 4 && (b.Flags & 32) != 32)
            .Select<User>(x => x);

        var expression2 = OrmLiteConfig.DialectProvider.SqlExpression<WatchForum>();

        expression2.Join<User>((a, b) => b.ID == a.UserID).Join<Topic>((a, c) => c.ForumID == a.ForumID)
            .Where<WatchForum, User, Topic>((a, b, c) => b.ID != userId && b.NotificationType != 10 &&
                                                         c.ID == topicId && (b.Flags & 2) == 2 && (b.Flags & 4) != 4 &&
                                                         (b.Flags & 32) != 32).Select<User>(x => x);

        return
        [
            .. (await repository.DbAccess.ExecuteAsync(db => db.SelectAsync<User>(
                    $"{expression.ToMergedParamsSelectStatement()} UNION ALL {expression2.ToMergedParamsSelectStatement()}")))
                .DistinctBy(x => x.ID)
        ];
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
    public static Task<List<string>> GroupEmailsAsync(this IRepository<User> repository, int groupId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

        expression.Join<UserGroup>((u, b) => b.UserID == u.ID).Join<UserGroup, Group>((b, c) => c.ID == b.GroupID)
            .Where<User, UserGroup, Group>((a, b, c) => b.GroupID == groupId && (c.Flags & 2) == 0 && a.Email != null)
            .Select(u => new { u.Email });

        return repository.DbAccess.ExecuteAsync(db => db.SqlListAsync<string>(expression));
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
    public async static Task<User> GetUserByProviderKeyAsync(this IRepository<User> repository, int? boardId, string providerUserKey)
    {
        return boardId == null
            ? await repository.GetSingleAsync(u => u.ProviderUserKey == providerUserKey)
            : await repository.GetSingleAsync(u => u.BoardID == boardId && u.ProviderUserKey == providerUserKey);
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
    public async static Task<dynamic> MaxAlbumDataAsync(
        this IRepository<User> repository,
        int userId,
        int boardId)
    {
        var groupMax = await repository.DbAccess.ExecuteAsync(db => db.SingleAsync<(int maxAlbum, int maxAlbumImages)>(
            db.From<User>().Join<UserGroup>((a, b) => b.UserID == a.ID)
                .Join<UserGroup, Group>((b, c) => c.ID == b.GroupID)
                .Where(a => a.ID == userId && a.BoardID == boardId).Select<Group>(c =>
                    new { MaxAlbum = Sql.Max(c.UsrAlbums), MaxAlbumImages = Sql.Max(c.UsrAlbumImages) })));

        var rankMax = await repository.DbAccess.ExecuteAsync(db => db.SingleAsync<(int maxAlbum, int maxAlbumImages)>(
            db.From<User>().Join<Rank>((a, b) => b.ID == a.RankID)
                .Where(a => a.ID == userId && a.BoardID == boardId).Select<Rank>(b =>
                    new { MaxAlbum = b.UsrAlbums, MaxAlbumImages = b.UsrAlbumImages })));

        dynamic data = new ExpandoObject();

        data.UserAlbum = groupMax.maxAlbum > rankMax.maxAlbum ? groupMax.maxAlbum : rankMax.maxAlbum;
        data.UserAlbumImages = groupMax.maxAlbumImages > rankMax.maxAlbumImages
            ? groupMax.maxAlbumImages
            : rankMax.maxAlbumImages;

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
    public async static Task<dynamic> SignatureDataAsync(
        this IRepository<User> repository,
        int userId,
        int boardId)
    {
        var groupMax = await repository.DbAccess.ExecuteAsync(db => db.SingleAsync<(string usrSigBBCodes, int usrSigChars)>(
            db.From<User>().Join<UserGroup>((a, b) => b.UserID == a.ID)
                .Join<UserGroup, Group>((b, c) => c.ID == b.GroupID)
                .Where(a => a.ID == userId && a.BoardID == boardId).Select<Group>(c =>
                    new { UsrSigBBCodes = Sql.Max(c.UsrSigBBCodes), UsrSigChars = Sql.Max(c.UsrSigChars) })));

        var rankMax = await repository.DbAccess.ExecuteAsync(db => db.SingleAsync<(string usrSigBBCodes, int usrSigChars)>(
            db.From<User>().Join<Rank>((a, b) => b.ID == a.RankID)
                .Where(a => a.ID == userId && a.BoardID == boardId)
                .Select<Rank>(b => new { b.UsrSigBBCodes, b.UsrSigChars })));

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
            data.UsrSigBBCodes = groupMax.usrSigBBCodes.Length > rankMax.usrSigBBCodes.Length
                ? groupMax.usrSigBBCodes
                : rankMax.usrSigBBCodes;
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
    public static Task<UserLazyData> LazyDataAsync(
        this IRepository<User> repository,
        int userId,
        int boardId,
        bool showPendingBuddies,
        bool showUnreadPMs,
        bool showUserAlbums)
    {
        return repository.DbAccess.ExecuteAsync(db =>
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            expression.Where<User>(u => u.ID == userId);

            // -- moderate Posts
            var moderatePostsExpression = OrmLiteConfig.DialectProvider.SqlExpression<Message>()
                .Join<Topic>((a, b) => b.ID == a.TopicID).Join<Topic, Forum>((b, c) => c.ID == b.ForumID)
                .Join<Forum, Category>((c, d) => d.ID == c.CategoryID)
                .Join<Topic, ActiveAccess>((b, access) => access.ForumID == b.ForumID);

            moderatePostsExpression.Where<Message, Topic, Forum, Category, ActiveAccess>((a, b, c, d, access) =>
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

            countThanksExpression.Where(a => a.UserID == userId && (a.Flags & 1024) == 1024 && a.Notification);

            var countThanksSql = countThanksExpression.Select(Sql.Count("1"))
                .ToMergedParamsSelectStatement();

            // -- count Mention
            var countMentionExpression = OrmLiteConfig.DialectProvider.SqlExpression<Activity>();

            countMentionExpression.Where(a => a.UserID == userId && (a.Flags & 512) == 512 && a.Notification);

            var countMentionSql = countMentionExpression.Select(Sql.Count("1"))
                .ToMergedParamsSelectStatement();

            // -- count Quoted
            var countQuotedExpression = OrmLiteConfig.DialectProvider.SqlExpression<Activity>();

            countQuotedExpression.Where(a => a.UserID == userId && (a.Flags & 1024) == 1024 && a.Notification);

            var countQuotedSql = countQuotedExpression.Select(Sql.Count("1"))
                .ToMergedParamsSelectStatement();

            // -- count Watch Topics
            var countWatchTopicsExpression = OrmLiteConfig.DialectProvider.SqlExpression<Activity>();

            countWatchTopicsExpression.Where(a => a.UserID == userId && a.Notification &&
                                                  ((a.Flags & 8192) == 8192 || (a.Flags & 16384) == 16384));

            var countWatchTopicsSql = countWatchTopicsExpression.Select(Sql.Count("1"))
                .ToMergedParamsSelectStatement();

            var countUnreadSql = "0";

            if (showUnreadPMs)
            {
                // -- count Unread
                var countUnreadExpression = OrmLiteConfig.DialectProvider.SqlExpression<PrivateMessage>();

                countUnreadExpression.Where(x => x.ToUserId == userId && (x.Flags & 1) != 1);

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

            expression.Take(1).Select<User>(a => new
            {
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

            return db.SingleAsync<UserLazyData>(expression);
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
    /// <param name="flags">
    /// The user flags.
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
    public static Task SaveAsync(
        this IRepository<User> repository,
        int userId,
        UserFlags flags,
        string timeZone,
        string languageFile,
        string culture,
        string themeFile,
        bool hideUser,
        bool activity,
        int pageSize)
    {
        // -- set user dirty
        flags.IsDirty = true;
        flags.IsActiveExcluded = hideUser;

        return repository.UpdateOnlyAsync(
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
    public async static Task UpdateDisplayNameAsync(
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

        await repository.UpdateOnlyAsync(
            () => new User
            {
                DisplayName = displayName
            },
            u => u.ID == user.ID);

        // -- here we sync a new display name everywhere
        await BoardContext.Current.GetRepository<Forum>().UpdateOnlyAsync(
            () => new Forum { LastUserDisplayName = displayName },
            x => x.LastUserID == user.ID &&
                 (x.LastUserDisplayName == null || x.LastUserDisplayName == oldDisplayName));

        await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
            () => new Topic { LastUserDisplayName = displayName },
            x => x.LastUserID == user.ID &&
                 (x.LastUserDisplayName == null || x.LastUserDisplayName == oldDisplayName));

        await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
            () => new Topic { UserDisplayName = displayName },
            x => x.UserID == user.ID &&
                 (x.UserDisplayName == null || x.UserDisplayName == oldDisplayName));

        await BoardContext.Current.GetRepository<Message>().UpdateOnlyAsync(
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
    public async static Task SaveAvatarAsync(
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
                await stream.ReadExactlyAsync(data, 0, stream.Length.ToType<int>());
            }

            await repository.UpdateOnlyAsync(
                () => new User { Avatar = avatarUrl, AvatarImage = data, AvatarImageType = avatarImageType },
                u => u.ID == userId);
        }
        else
        {
            await repository.UpdateOnlyAsync(
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
    public static Task SaveNotificationAsync(
        this IRepository<User> repository,
        int userId,
        bool autoWatchTopics,
        int? notificationType,
        bool dailyDigest)
    {
        return repository.UpdateOnlyAsync(
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
    public static Task<List<User>> ListAdminsAsync(
        this IRepository<User> repository,
        int? boardId = null)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

        expression.Join<VAccess>((u, v) => v.UserID == u.ID).Where<VAccess, User>((v, u) =>
            u.BoardID == (boardId ?? repository.BoardID) && (u.Flags & 4) != 4 && v.IsAdmin > 0 &&
            v.ForumID == 0).OrderBy<User>(u => u.DisplayName);

        return repository.DbAccess.ExecuteAsync(db => db.SelectAsync(expression));
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
    public static Task<List<User>> GetUnApprovedUsersAsync(this IRepository<User> repository, int boardId)
    {
        return repository.GetAsync(u => u.BoardID == boardId && (u.Flags & 2) != 2 && (u.Flags & 32) != 32);
    }

    /// <summary>
    /// Saves the signature.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="signature">The signature.</param>
    public static Task SaveSignatureAsync(
        this IRepository<User> repository,
        int userId,
        string signature)
    {
        return repository.UpdateOnlyAsync(() => new User { Signature = signature }, u => u.ID == userId);
    }

    /// <summary>
    /// Sets the points.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="points">The points.</param>
    public static Task SetPointsAsync(this IRepository<User> repository, int userId, int points)
    {
        return repository.UpdateOnlyAsync(() => new User { Points = points }, u => u.ID == userId);
    }

    /// <summary>
    /// Suspends or Un-suspend the User
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="suspend">The suspend date and time.</param>
    /// <param name="suspendReason">The suspend reason.</param>
    /// <param name="suspendBy">Suspend by User Id.</param>
    public static Task SuspendAsync(
        this IRepository<User> repository,
        int userId,
        DateTime? suspend = null,
        string suspendReason = null,
        int suspendBy = 0)
    {
        return repository.UpdateOnlyAsync(
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
    public static Task UpdateBlockFlagsAsync(this IRepository<User> repository, int userId, int flags)
    {
        return repository.UpdateOnlyAsync(() => new User { BlockFlags = flags }, u => u.ID == userId);
    }

    /// <summary>
    /// Gets the list of recently (last 24 hours) logged-in users.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <returns>
    /// The list of users.
    /// </returns>
    public async static Task<List<ActiveUser>> GetRecentUsersAsync(
        this IRepository<User> repository)
    {
        var timeSinceLastLogin = DateTime.UtcNow.AddMinutes(0 - 60 * 24 * 30);

        var users = await repository.DbAccess.ExecuteAsync(db =>
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            expression.Where<User>(u => (u.Flags & 2) == 2 && u.BoardID == repository.BoardID && (u.Flags & 4) != 4 &&
                                        timeSinceLastLogin < u.LastVisit);

            expression.OrderBy(u => u.LastVisit);

            expression.Select<User>(u => new
            {
                UserID = u.ID,
                UserName = u.Name,
                UserDisplayName = u.DisplayName,
                IsCrawler = 0,
                UserCount = 1,
                IsActiveExcluded =
                    Sql.Custom<bool>(
                        $"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<User>(x => x.Flags, true)}&16")})"),
                IsGuest = Sql.Custom<bool>(
                    $"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<User>(x => x.Flags, true)}&4")})"),
                u.UserStyle,
                u.Suspended,
                u.LastVisit
            });

            return db.SelectAsync<ActiveUser>(expression);
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
    public async static Task<List<SimpleModerator>> GetForumModeratorsAsync(
        this IRepository<User> repository)
    {
        var results = await repository.DbAccess.ExecuteAsync(db =>
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

                expression.Join<ForumAccess>((f, a) => a.ForumID == f.ID)
                    .Join<ForumAccess, Group>((a, b) => b.ID == a.GroupID)
                    .Join<ForumAccess, AccessMask>((a, c) => c.ID == a.AccessMaskID)
                    .Where<Group, AccessMask>((b, c) => b.BoardID == repository.BoardID && (c.Flags & 64) != 0)
                    .Select<Forum, ForumAccess, Group>((f, a, b) => new
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
                    .Select<User, VaccessGroup, Forum, Category>((usr, access, f, c) => new
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

                return db.SelectAsync<SimpleModerator>(
                    $"{expression.ToMergedParamsSelectStatement()} union all {expression2.ToMergedParamsSelectStatement()}");
            });

            return [.. results.OrderByDescending(x => x.IsGroup).ThenBy(x => x.Name)];
    }

    /// <summary>
    /// Gets the registered users by month.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="boardId">The board identifier.</param>
    /// <returns>System.Collections.Generic.List&lt;YAF.Types.Models.User&gt;.</returns>
    public static Task<List<User>> GetRegisteredUsersByMonthAsync(this IRepository<User> repository, int boardId)
    {
        return repository.GetAsync(a => a.BoardID == boardId && a.Joined > DateTime.UtcNow - TimeSpan.FromDays(730));
    }
}