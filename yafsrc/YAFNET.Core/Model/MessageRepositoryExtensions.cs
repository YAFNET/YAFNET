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
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using YAF.Types.Constants;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
///     The Message repository extensions.
/// </summary>
public static class MessageRepositoryExtensions
{
    /// <summary>
    /// Checks if the User has replied to the specific topic.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// Returns if true or not
    /// </returns>
    public static bool RepliedTopic(
        this IRepository<Message> repository,
        int topicId,
        int userId)
    {
        return repository.Count(m => m.TopicID == topicId && m.UserID == userId) > 0;
    }

    /// <summary>
    /// The get message.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// The <see cref="Tuple"/>.
    /// </returns>
    public static Message GetMessage(
        this IRepository<Message> repository,
        int messageId)
    {
        return repository.DbAccess.Execute(db => db.Connection.LoadSingleById<Message>(messageId));
    }

    /// <summary>
    /// The get message.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// The <see cref="Tuple"/>.
    /// </returns>
    public static Tuple<Topic, Message, User, Forum> GetMessageAsTuple(
        this IRepository<Message> repository,
        int messageId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

        expression.Join<Message>((t, m) => m.TopicID == t.ID).Join<Message, User>((m, u) => u.ID == m.UserID)
            .Join<Forum>((t, f) => f.ID == t.ForumID).Where<Message>(m => m.ID == messageId);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Topic, Message, User, Forum>(expression))
            .FirstOrDefault();
    }

    /// <summary>
    /// Gets the message with access.
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
    /// The <see cref="Tuple"/>.
    /// </returns>
    public static Tuple<Topic, Message, User, Forum> GetMessageWithAccess(
        this IRepository<Message> repository,
        int messageId,
        int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

        expression.Join<Message>((t, m) => m.TopicID == t.ID).Join<Message, User>((m, u) => u.ID == m.UserID)
            .Join<Forum>((t, f) => f.ID == t.ForumID).LeftJoin<ActiveAccess>((t, x) => x.ForumID == t.ForumID)
            .Where<Message, ActiveAccess>((m, x) => m.ID == messageId && x.UserID == userId && x.ReadAccess);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Topic, Message, User, Forum>(expression))
            .FirstOrDefault();
    }

    /// <summary>
    /// Gets the deleted posts.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <returns>
    /// List of deleted posts.
    /// </returns>
    public static List<Tuple<Forum, Topic, Message>> GetDeletedMessages(
        this IRepository<Message> repository,
        int boardId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

        expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
            .Join<Topic>((f, t) => t.ForumID == f.ID).Join<Topic, Message>((t, m) => m.TopicID == t.ID)
            .Where<Message, Category>((m, category) => category.BoardID == boardId && (m.Flags & 8) == 8 && (category.Flags & 1) == 1);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Forum, Topic, Message>(expression));
    }

    /// <summary>
    /// Get the (paged) deleted posts.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <returns>
    /// List of deleted Topics
    /// </returns>
    public static List<PagedMessage> GetDeletedMessagesPaged(
        this IRepository<Message> repository,
        int boardId,
        int pageIndex,
        int pageSize)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

        expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
            .Join<Topic>((f, t) => t.ForumID == f.ID).Join<Topic, Message>((t, m) => m.TopicID == t.ID)
            .Where<Message, Category>((m, category) => category.BoardID == boardId && (m.Flags & 8) == 8 && (category.Flags & 1) == 1);

        // -- count total
        var countTotalExpression = expression;

        var countTotalSql = countTotalExpression
            .Select(Sql.Count($"{countTotalExpression.Column<Message>(x => x.ID)}")).ToSelectStatement();

        expression.Select<Forum, Topic, Message>(
            (g, d, m) => new {
                TopicID = d.ID,
                Topic = d.TopicName,
                d.Priority,
                d.Description,
                d.Status,
                d.Styles,
                d.PollID,
                TopicOwnerID = d.UserID,
                d.AnswerMessageId,
                TopicFlags = d.Flags,
                ForumFlags = g.Flags,
                ForumName = g.Name,
                MessageID = m.ID,
                m.Posted,
                Message = m.MessageText,
                m.IP,
                m.Flags,
                m.EditReason,
                m.IsModeratorChanged,
                IsDeleted =
                    Sql.Custom<bool>(
                        $"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<Message>(x => x.Flags, true)}&8")})"),
                m.Position,
                m.DeleteReason,
                m.ExternalMessageId,
                d.Views,
                d.ForumID,
                Edited = m.Edited ?? m.Posted,
                TotalRows = Sql.Custom($"({countTotalSql})")
            });

        // Set Paging
        expression.Page(pageIndex + 1, pageSize);

        return repository.DbAccess.Execute(db => db.Connection.Select<PagedMessage>(expression));
    }

    /// <summary>
    /// Gets the Post List Paged
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="updateViewCount">
    /// The update view count.
    /// </param>
    /// <param name="showDeleted">
    /// The show deleted.
    /// </param>
    /// <param name="sincePostedDate">
    /// The posted date.
    /// </param>
    /// <param name="toPostedDate">
    /// The to Posted Date.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The Page size.
    /// </param>
    /// <param name="messagePosition">
    /// The message Position.
    /// </param>
    /// <returns>
    /// Returns the Paged Post List
    /// </returns>
    public static List<PagedMessage> PostListPaged(
        this IRepository<Message> repository,
        int topicId,
        int userId,
        bool updateViewCount,
        bool showDeleted,
        DateTime sincePostedDate,
        DateTime toPostedDate,
        int pageIndex,
        int pageSize,
        int messagePosition)
    {
        if (updateViewCount)
        {
            BoardContext.Current.GetRepository<Topic>().UpdateAdd(
                () => new Topic { Views = 1 },
                t => t.ID == topicId);
        }

        // Get Total Rows
        var totalRows = GetMessageCount(topicId, showDeleted, sincePostedDate, toPostedDate);

        if (messagePosition > 0)
        {
            // Change index
            pageIndex = messagePosition / pageSize;

            if (pageIndex > 0)
            {
                pageIndex++;
            }
        }

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        return repository.DbAccess.Execute(
            db =>
                {
                    expression.Join<User>((m, u) => u.ID == m.UserID).Join<Topic>((m, t) => t.ID == m.TopicID)
                        .Join<Topic, Forum>((t, f) => f.ID == t.ForumID)
                        .Join<Forum, Category>((f, c) => c.ID == f.CategoryID)
                        .Join<User, Rank>((u, r) => r.ID == u.RankID);

                    expression.Where<Message>(
                        m => m.TopicID == topicId && (m.Flags & 16) == 16 && m.Posted >= sincePostedDate &&
                             m.Posted <= toPostedDate);

                    if (!showDeleted)
                    {
                        expression.And(m => (m.Flags & 8) != 8);
                    }

                    var pageIndexCheck = pageIndex + 1;
                    var totalPages = IPagerExtensions.PageCount(totalRows, pageSize);

                    if (totalPages == 0)
                    {
                        totalPages = 1;
                    }

                    if (pageIndexCheck > totalPages)
                    {
                        // Fix if position is wrong
                        expression.OrderBy<Message>(m => m.Posted).Page(totalPages, pageSize);
                    }
                    else
                    {
                        expression.OrderBy<Message>(m => m.Posted).Page(pageIndex + 1, pageSize);
                    }

                    var reputationExpression = db.Connection.From<ReputationVote>(db.Connection.TableAlias("x"));
                    reputationExpression.Where(
                        $"""
                         x.{reputationExpression.Column<ReputationVote>(x => x.ReputationToUserID)}={expression.Column<User>(b => b.ID, true)}
                                                                 and x.{reputationExpression.Column<ReputationVote>(x => x.ReputationFromUserID)}={userId}
                         """);
                    var reputationSql = reputationExpression
                        .Select($"{reputationExpression.Column<ReputationVote>(x => x.VoteDate)}").Limit(1)
                        .ToSelectStatement();

                    var isThankByUserExpression = db.Connection.From<Thanks>(db.Connection.TableAlias("ta"));
                    isThankByUserExpression.Where(
                        $"""
                         ta.{isThankByUserExpression.Column<Thanks>(x => x.ThanksFromUserID)}={userId}
                                                             and ta.{isThankByUserExpression.Column<Thanks>(x => x.MessageID)}={expression.Column<Message>(x => x.ID, true)}
                         """);
                    var isThankByUserSql = isThankByUserExpression.Select(Sql.Count("1")).ToSelectStatement();

                    var thanksCountExpression = db.Connection.From<Thanks>(db.Connection.TableAlias("ta"));
                    thanksCountExpression.Where(
                        $"ta.{thanksCountExpression.Column<Thanks>(x => x.MessageID)}={expression.Column<Message>(x => x.ID, true)}");
                    var thanksCountSql = thanksCountExpression
                        .Select(Sql.Count($"{thanksCountExpression.Column<Thanks>(x => x.ID)}")).ToSelectStatement();

#pragma warning disable IDE0075
                    expression.Select<Message, User, Topic, Forum, Category, Rank>(
                        (m, b, d, g, h, c) => new
                                                  {
                                                      TopicID = d.ID,
                                                      Topic = d.TopicName,
                                                      d.Priority,
                                                      d.Description,
                                                      d.Status,
                                                      d.Styles,
                                                      d.PollID,
                                                      TopicOwnerID = d.UserID,
                                                      d.AnswerMessageId,
                                                      TopicFlags = d.Flags,
                                                      ForumFlags = g.Flags,
                                                      ForumName = g.Name,
                                                      MessageID = m.ID,
                                                      m.Posted,
                                                      m.MessageText,
                                                      UserID = b.ID,
                                                      m.IP,
                                                      m.Flags,
                                                      m.EditReason,
                                                      m.IsModeratorChanged,
                                                      IsDeleted =
                                                          Sql.Custom<bool>(
                                                              $"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<Message>(x => x.Flags, true)}&8")})"),
                                                      m.Position,
                                                      m.DeleteReason,
                                                      m.ExternalMessageId,
                                                      UserName = m.UserName ?? b.Name,
                                                      DisplayName = m.UserDisplayName ?? b.DisplayName,
                                                      b.BlockFlags,
                                                      b.Suspended,
                                                      b.Joined,
                                                      b.Avatar,
                                                      b.Signature,
                                                      Posts = b.NumPosts,
                                                      b.Points,
                                                      ReputationVoteDate =
                                                          Sql.Custom<DateTime>(
                                                              $"{OrmLiteConfig.DialectProvider.IsNullFunction(reputationSql, "null")}"),
                                                      IsGuest =
                                                          Sql.Custom<bool>(
                                                              $"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<User>(x => x.Flags, true)}&4")})"),
                                                      d.Views,
                                                      d.ForumID,
                                                      RankName = c.Name,
                                                      RankStyle = c.Style,
                                                      Style = b.UserStyle,
                                                      Edited = m.Edited ?? m.Posted,
                                                      HasAvatarImage = b.AvatarImage != null ? true : false,
                                                      IsThankedByUser = Sql.Custom($"({isThankByUserSql})"),
                                                      ThanksNumber = Sql.Custom($"({thanksCountSql})"),
                                                      PageIndex = pageIndex,
                                                      TotalRows = totalRows
                                                  });
#pragma warning restore IDE0075

                    return db.Connection.Select<PagedMessage>(expression);
                });
    }

    /// <summary>
    /// Gets the 10 Newest Posts of a Topic
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    public static List<Tuple<Message, User>> LastPosts(this IRepository<Message> repository, int topicId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        expression.Join<Message, User>((m, u) => m.UserID == u.ID)
            .Where<Message>(m => m.TopicID == topicId && (m.Flags & 24) == 16).OrderByDescending(m => m.Posted)
            .Take(10);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Message, User>(expression));
    }

    /// <summary>
    /// Gets all the post by a user.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// Returns all the post by a user.
    /// </returns>
    public static IOrderedEnumerable<Message> GetAllUserMessages(
        this IRepository<Message> repository,
        int userId)
    {
        return repository.DbAccess.Execute(db => db.Connection.LoadSelect<Message>(m => m.UserID == userId)
            .OrderByDescending(m => m.Posted));
    }

    /// <summary>
    /// Gets all the post by a user. (With Read Access of current Page User)
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
    /// <param name="pageUserId">
    /// The page User Id.
    /// </param>
    /// <param name="count">
    /// The count.
    /// </param>
    public static List<Tuple<Message, Topic, User>> GetAllUserMessagesWithAccess(
        this IRepository<Message> repository,
        int boardId,
        int userId,
        int pageUserId,
        int count)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        expression.Join<User>((a, b) => b.ID == a.UserID)
            .Join<Topic>((a, c) => c.ID == a.TopicID)
            .Join<Topic, Forum>((c, d) => d.ID == c.ForumID)
            .Join<Forum, Category>((d, e) => e.ID == d.CategoryID)
            .Join<Forum, ActiveAccess>((forum, access) => access.ForumID == forum.ID)
            .Where<Topic, Message, ActiveAccess, Category>(
                (topic, message, x, e) => message.UserID == userId && x.UserID == pageUserId && x.ReadAccess &&
                                          e.BoardID == boardId && (e.Flags & 1) == 1 && (topic.Flags & 8) != 8 &&
                                          (message.Flags & 8) != 8 && (message.Flags & 16) == 16).OrderByDescending<Message>(x => x.Posted)
            .Take(count);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Message, Topic, User>(expression));
    }

    /// <summary>
    /// Gets all messages by forum as Typed Search Message List.
    /// </summary>
    /// <param name="repository">
    ///     The repository.
    /// </param>
    /// <param name="forumId">
    ///     The forum Id.
    /// </param>
    /// <param name="topicTags"></param>
    /// <returns>
    /// Returns all Messages as Typed Search Message List
    /// </returns>
    public static IEnumerable<SearchMessage> GetAllSearchMessagesByForum(this IRepository<Message> repository,
        int forumId, List<Tag> topicTags)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

        expression.Join<Topic>((forum, topic) => topic.ForumID == forum.ID)
            .Join<Topic, Message>((topic, message) => message.TopicID == topic.ID)
            .Join<Message, User>((message, user) => user.ID == message.UserID).Where<Forum, Topic, Message>(
                (forum, topic, message) => forum.ID == forumId && (topic.Flags & 8) != 8 &&
                                           (message.Flags & 8) != 8 && (message.Flags & 16) == 16 &&
                                           topic.TopicMovedID == null).OrderByDescending<Message>(x => x.Posted);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Forum, Topic, Message, User>(expression))
            .ConvertAll(x => new SearchMessage(x, topicTags));
    }

    /// <summary>
    /// Approves the message.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message identifier.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    public static void Approve(this IRepository<Message> repository, int messageId, int forumId)
    {
        var message = repository.GetMessage(messageId);

        var flags = message.MessageFlags;

        flags.IsApproved = true;

        // -- update Message table, set message flag to approved
        repository.UpdateFlags(messageId, flags.BitValue);

        if (!BoardContext.Current.GetRepository<Forum>().Exists(f => f.ID == forumId && (f.Flags & 4) == 4))
        {
            // -- update User table to increase post count
            BoardContext.Current.GetRepository<User>().UpdateAdd(
                () => new User { NumPosts = 1 },
                u => u.ID == message.UserID);

            BoardContext.Current.GetRepository<User>().Promote(message.UserID);
        }

        // -- update Forum table with last topic/post info
        BoardContext.Current.GetRepository<Forum>().UpdateOnly(
            () => new Forum
                      {
                          LastPosted = message.Posted,
                          LastTopicID = message.TopicID,
                          LastMessageID = message.ID,
                          LastUserID = message.UserID,
                          LastUserName = message.UserName,
                          LastUserDisplayName = message.UserDisplayName
                      },
            x => x.ID == message.Topic.ForumID);

        var numPostsCount = repository.Count(m => m.TopicID == message.TopicID && (m.Flags & 8) != 8)
            .ToType<int>();

        // -- update Topic table with info about last post in topic
        BoardContext.Current.GetRepository<Topic>().UpdateOnly(
            () => new Topic
                      {
                          LastPosted = message.Posted,
                          LastMessageID = message.ID,
                          LastUserID = message.UserID,
                          LastUserName = message.UserName,
                          LastUserDisplayName = message.UserDisplayName,
                          LastMessageFlags = flags.BitValue,
                          NumPosts = numPostsCount
                      },
            x => x.ID == message.TopicID);

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forumId));

        repository.FireUpdated(messageId);
    }

    /// <summary>
    /// Updates the flags.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="flags">The flags.</param>
    public static void UpdateFlags(
        this IRepository<Message> repository,
        int messageId,
        int flags)
    {
        repository.UpdateOnly(() => new Message { Flags = flags }, u => u.ID == messageId);
    }

    /// <summary>
    /// Delete Message(s)
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is moderator changed.
    /// </param>
    /// <param name="deleteReason">
    /// The delete reason.
    /// </param>
    /// <param name="deleteLinked">
    /// The delete linked.
    /// </param>
    /// <param name="eraseMessage">
    /// Delete Message from Db, only set IsDeleted flag
    /// </param>
    /// <param name="isTopicDeleteAction">
    /// Indicator if we delete the entire topic
    /// </param>
    public static void Delete(
        this IRepository<Message> repository,
        int forumId,
        int topicId,
        Message message,
        bool isModeratorChanged,
        string deleteReason,
        bool deleteLinked,
        bool eraseMessage,
        bool isTopicDeleteAction = false)
    {
        var forum = BoardContext.Current.GetRepository<Forum>().GetById(forumId);

        repository.DeleteRecursively(
            forum,
            topicId,
            message,
            isModeratorChanged,
            deleteReason,
            deleteLinked,
            eraseMessage,
            isTopicDeleteAction);

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateTopicLastPostEvent(forumId, topicId));

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forumId));
    }

    /// <summary>
    /// Restore Message(s)
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public static void Restore(
        this IRepository<Message> repository,
        int forumId,
        int topicId,
        Message message)
    {
        repository.RestoreRecursively(
            forumId,
            topicId,
            message);

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateTopicLastPostEvent(forumId, topicId));

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forumId));
    }

    /// <summary>
    /// message move function
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="oldTopic">
    /// The old topic.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="moveToTopicId">
    /// The move To Topic Id.
    /// </param>
    /// <param name="moveAll">
    /// The move all.
    /// </param>
    public static void Move(
        this IRepository<Message> repository,
        Topic oldTopic,
        Message message,
        int moveToTopicId,
        bool moveAll)
    {
        repository.Move(message, moveToTopicId);

        if (moveAll)
        {
            // moveAll=true anyway
            // it's in charge of moving answers of moved post
            var replies = repository.Get(m => m.ReplyTo == message.ID);

            replies.ForEach(reply => repository.MoveRecursively(reply, moveToTopicId));
        }

        var newForumId = BoardContext.Current.GetRepository<Topic>().GetById(moveToTopicId).ForumID;

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateTopicLastPostEvent(newForumId, moveToTopicId));
        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateTopicLastPostEvent(oldTopic.ForumID, oldTopic.ID));

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(newForumId));
        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(oldTopic.ForumID));
    }

    /// <summary>
    /// gets list of replies to message
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    public static List<Message> Replies(this IRepository<Message> repository, int messageId)
    {
        return repository.DbAccess.Execute(
            db => db.Connection.LoadSelect<Message>(m => (m.Flags & 16) == 16 && m.ReplyTo == messageId));
    }

    /// <summary>
    /// Finds the first Unread or Newest Message
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <param name="lastRead">
    /// The last read.
    /// </param>
    /// <param name="showDeleted">
    /// The show Deleted.
    /// </param>
    public static (int MessagePosition, int MessageID) FindUnread(
        this IRepository<Message> repository,
        int topicId,
        int? messageId,
        DateTime lastRead,
        bool showDeleted)
    {
        var minDateTime = DateTimeHelper.SqlDbMinTime().AddYears(-1);

        (int MessagePosition, int MessageID) message;

        if (messageId.HasValue)
        {
            message = repository.DbAccess.Execute(
                db =>
                    {
                        var expression = db.Connection.From<Message>();

                        if (showDeleted)
                        {
                            expression.Where<Message>(
                                m => m.ID == messageId && m.TopicID == topicId && (m.Flags & 16) == 16);
                        }
                        else
                        {
                            expression.Where<Message>(
                                m => m.ID == messageId && m.TopicID == topicId && (m.Flags & 16) == 16 &&
                                     (m.Flags & 8) != 8);
                        }

                        expression.OrderBy(m => m.Posted);

                        expression.Limit(1);

                        expression.Select(m => new { MessagePosition = m.Position, MessageID = m.ID });

                        return db.Connection.Single<(int MessagePosition, int MessageID)>(expression);
                    });

            if (message.MessageID > 0)
            {
                return message;
            }
        }

        // -- if value > yaf db min value (1-1-1903) we are looking for first unread
        if (lastRead > minDateTime)
        {
            // -- a message with the id was not found, or we are looking for first unread or last post
            message = repository.DbAccess.Execute(
                db =>
                    {
                        var expression = db.Connection.From<Message>();

                        if (showDeleted)
                        {
                            expression.Where<Message>(
                                m => m.TopicID == topicId && (m.Flags & 16) == 16 && m.Posted > lastRead);
                        }
                        else
                        {
                            expression.Where<Message>(
                                m => m.TopicID == topicId && (m.Flags & 16) == 16 && (m.Flags & 8) != 8 &&
                                     m.Posted > lastRead);
                        }

                        expression.OrderBy(m => m.Posted);

                        expression.Limit(1);

                        expression.Select(m => new { MessagePosition = m.Position, MessageID = m.ID });

                        return db.Connection.Single<(int MessagePosition, int MessageID)>(expression);
                    });

            if (message.MessageID > 0)
            {
                return message;
            }
        }

        // -- if first unread was not found, or we're looking for last posted
        message = repository.DbAccess.Execute(
            db =>
                {
                    var expression = db.Connection.From<Message>();

                    if (showDeleted)
                    {
                        expression.Where<Message>(m => m.TopicID == topicId && (m.Flags & 16) == 16);
                    }
                    else
                    {
                        expression.Where<Message>(
                            m => m.TopicID == topicId && (m.Flags & 16) == 16 && (m.Flags & 8) != 8);
                    }

                    if (lastRead > minDateTime)
                    {
                        expression.OrderByDescending(m => m.Posted);
                    }
                    else
                    {
                        expression.OrderBy(m => m.Posted);
                    }

                    expression.Limit(1);

                    expression.Select(m => new { MessagePosition = m.Position, MessageID = m.ID });

                    return db.Connection.Single<(int MessagePosition, int MessageID)>(expression);
                });

        return message;
    }

    /// <summary>
    /// Copy current Message text over reported Message text.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    public static void ReportResolve(
        this IRepository<Message> repository,
        int messageId,
        int userId)
    {
        BoardContext.Current.GetRepository<MessageReported>().UpdateOnly(
            () => new MessageReported { Resolved = true, ResolvedBy = userId, ResolvedDate = DateTime.UtcNow },
            m => m.ID == messageId);

        var flags = new MessageFlags(
                        repository.DbAccess.Execute(
                            db => db.Connection.Scalar<Message, int>(m => m.Flags, m => m.ID == messageId)))
                        {
                            IsReported = false
                        };

        repository.UpdateFlags(messageId, flags.BitValue);

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));
    }

    /// <summary>
    /// Saves the new Message
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="guestUserName">
    /// The guest user name.
    /// </param>
    /// <param name="ipAddress">
    /// The IP Address.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    /// <param name="replyTo">
    /// The reply to.
    /// </param>
    /// <param name="flags">
    /// The Message flags.
    /// </param>
    /// <returns>
    /// Returns the Message ID
    /// </returns>
    public static Message SaveNew(
        this IRepository<Message> repository,
        Forum forum,
        Topic topic,
        User user,
        string message,
        string guestUserName,
        string ipAddress,
        DateTime posted,
        int? replyTo,
        MessageFlags flags)
    {
        int position;

        var maxPosition = repository.DbAccess.Execute(
            db => db.Connection.Scalar<Message, int?>(m => Sql.Max(m.Position), m => m.TopicID == topic.ID));

        if (maxPosition.HasValue)
        {
            // Normal Reply
            position = maxPosition.Value + 1;
        }
        else
        {
            // New Topic
            position = 0;
        }

        // Add points to Users total reputation points
        if (!BoardContext.Current.IsGuest)
        {
            BoardContext.Current.GetRepository<User>().UpdateAdd(
                () => new User { Points = 3 },
                u => u.ID == user.ID);
        }

        var replaceName = BoardContext.Current.IsGuest ? guestUserName : BoardContext.Current.PageUser.DisplayName;

        var newMessage = new Message
                             {
                                 UserID = user.ID,
                                 MessageText = message,
                                 TopicID = topic.ID,
                                 Posted = posted,
                                 UserName = guestUserName,
                                 UserDisplayName = replaceName,
                                 IP = ipAddress,
                                 ReplyTo = replyTo,
                                 Position = position,
                                 Indent = 0,
                                 Flags = flags.BitValue,
                                 ExternalMessageId = null
                             };

        var newMessageId = repository.Insert(
            newMessage);

        newMessage.ID = newMessageId;

        // Add to search index
        var newSearchMessage = new SearchMessage
                                   {
                                       MessageId = newMessageId,
                                       Message = message,
                                       Flags = flags.BitValue,
                                       Posted = posted.ToString(CultureInfo.InvariantCulture),
                                       UserName = user.Name,
                                       UserDisplayName = user.DisplayName,
                                       UserStyle = user.UserStyle,
                                       UserId = user.ID,
                                       TopicId = topic.ID,
                                       Topic = topic.TopicName,
                                       ForumId = forum.ID,
                                       TopicTags = string.Empty,
                                       ForumName = forum.Name,
                                       Description = string.Empty
                                   };

        try
        {
            BoardContext.Current.Get<ISearch>().AddSearchIndexItem(newSearchMessage);
        }
        catch (Exception ex)
        {
            BoardContext.Current.Get<ILogger<IRepository<Message>>>().Error(ex, ex.Message);
        }

        if (flags.IsApproved)
        {
            repository.Approve(newMessageId, forum.ID);
            BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forum.ID));
        }

        repository.FireNew(newMessageId);

        return newMessage;
    }

    /// <summary>
    /// The message_unapproved.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    public static List<Tuple<Topic, Message, User>> Unapproved(
        this IRepository<Message> repository,
        int forumId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

        expression.Join<Message>((topic, message) => message.TopicID == topic.ID)
            .Join<Message, User>((message, user) => message.UserID == user.ID).Where<Topic, Message>(
                (topic, message) => topic.ForumID == forumId && (message.Flags & 16) != 16 &&
                                    (topic.Flags & 8) != 8 && (message.Flags & 8) != 8);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Topic, Message, User>(expression));
    }

    /// <summary>
    /// Updating the Message (Post)
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="priority">
    /// The priority.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="status">
    /// The status.
    /// </param>
    /// <param name="styles">
    /// The styles.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="reasonOfEdit">
    /// The reason of edit.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is moderator changed.
    /// </param>
    /// <param name="overrideApproval">
    /// The override approval.
    /// </param>
    /// <param name="topic">
    /// The Topic
    /// </param>
    /// <param name="originalMessage">
    /// The original Message.
    /// </param>
    /// <param name="originalMessageUser">
    /// The Original Message User.
    /// </param>
    /// <param name="editedBy">
    /// UserId of who edited the message.
    /// </param>
    /// <param name="forum">
    /// The Forum.
    /// </param>
    public static void Update(
        this IRepository<Message> repository,
        short? priority,
        string message,
        string description,
        string status,
        string styles,
        string subject,
        string reasonOfEdit,
        bool isModeratorChanged,
        bool overrideApproval,
        Topic topic,
        Message originalMessage,
        Forum forum,
        User originalMessageUser,
        int editedBy)
    {
        if (overrideApproval || !forum.ForumFlags.IsModerated)
        {
            originalMessage.MessageFlags.IsApproved = true;
        }

        if (BoardContext.Current.GetRepository<MessageHistory>()
            .Exists(m => m.MessageID == originalMessage.ID))
        {
            // -- insert current message variant - use OriginalMessage in future
            BoardContext.Current.GetRepository<MessageHistory>().Insert(
                new MessageHistory
                    {
                        MessageID = originalMessage.ID,
                        Message = originalMessage.MessageText,
                        IP = originalMessage.IP,
                        Edited = DateTime.UtcNow,
                        EditedBy = editedBy,
                        EditReason = reasonOfEdit,
                        IsModeratorChanged = originalMessage.IsModeratorChanged.Value,
                        Flags = originalMessage.Flags
                    });
        }
        else
        {
            // -- save original message in the history if this is the first edit
            BoardContext.Current.GetRepository<MessageHistory>().Insert(
                new MessageHistory
                    {
                        MessageID = originalMessage.ID,
                        Message = originalMessage.MessageText,
                        IP = originalMessage.IP,
                        Edited = originalMessage.Posted,
                        EditedBy = originalMessage.UserID,
                        EditReason = null,
                        IsModeratorChanged = originalMessage.IsModeratorChanged.Value,
                        Flags = originalMessage.Flags
                    });
        }

        repository.UpdateOnly(
            () => new Message
                      {
                          MessageText = message,
                          Edited = DateTime.UtcNow,
                          EditedBy = editedBy,
                          Flags = originalMessage.MessageFlags.BitValue,
                          IsModeratorChanged = isModeratorChanged,
                          EditReason = reasonOfEdit
                      },
            m => m.ID == originalMessage.ID);

        if (priority.HasValue)
        {
            BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                () => new Topic { Priority = priority.Value },
                t => t.ID == originalMessage.TopicID);
        }

        if (subject.IsSet())
        {
            BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                () => new Topic
                          {
                              TopicName = subject, Description = description, Status = status, Styles = styles
                          },
                t => t.ID == originalMessage.TopicID);
        }

        // Update Search index Item
        var updateMessage = new SearchMessage
                                {
                                    MessageId = originalMessage.ID,
                                    Message = message,
                                    Flags = originalMessage.Flags,
                                    Posted = originalMessage.Posted.ToString(CultureInfo.InvariantCulture),
                                    UserName = originalMessageUser.Name,
                                    UserDisplayName = originalMessageUser.DisplayName,
                                    UserStyle = originalMessageUser.UserStyle,
                                    UserId = originalMessage.UserID,
                                    TopicId = originalMessage.TopicID,
                                    Topic = subject.IsSet() ? subject  : topic.TopicName,
                                    ForumId = forum.ID,
                                    ForumName = forum.Name,
                                    Description = description
                                };

        BoardContext.Current.Get<ISearch>().UpdateSearchIndexItem(updateMessage, true);

        if (forum.ForumFlags.IsModerated)
        {
            // If forum is moderated, make sure last post pointers are correct
            BoardContext.Current.Get<IRaiseEvent>().Raise(
                new UpdateTopicLastPostEvent(forum.ID, originalMessage.TopicID));
        }
    }

    /// <summary>
    /// Gets the user last posted date time.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>System.Nullable&lt;System.DateTime&gt;.</returns>
    public static DateTime? GetUserLastPosted(
            this IRepository<Message> repository,
            int userId)
    {
        var lastPostedExpression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        lastPostedExpression.Where(a => a.UserID == userId).Select(x => x.Posted).OrderByDescending(x => x.Posted)
            .Take(1)
            .ToMergedParamsSelectStatement();

        return repository.DbAccess
            .Execute(db => db.Connection.Select<DateTime?>(lastPostedExpression))
            .FirstOrDefault();
    }

    /// <summary>
    /// Delete all messages recursively.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is moderator changed.
    /// </param>
    /// <param name="deleteReason">
    /// The delete reason.
    /// </param>
    /// <param name="deleteLinked">
    /// The delete linked.
    /// </param>
    /// <param name="eraseMessages">
    /// Delete Message from Db, only set IsDeleted flag
    /// </param>
    /// <param name="isTopicDeleteAction">
    /// Indicator if we delete the entire topic
    /// </param>
    private static void DeleteRecursively(
        this IRepository<Message> repository,
        Forum forum,
        int topicId,
        Message message,
        bool isModeratorChanged,
        string deleteReason,
        bool deleteLinked,
        bool eraseMessages,
        bool isTopicDeleteAction)
    {
        if (deleteLinked)
        {
            // Delete replies
            var replies = repository.Get(m => m.ReplyTo == message.ID).ToList();

            if (replies.HasItems())
            {
                replies.ForEach(
                    reply => repository.DeleteRecursively(
                        forum,
                        topicId,
                        reply,
                        isModeratorChanged,
                        deleteReason,
                        true,
                        eraseMessages,
                        isTopicDeleteAction));
            }
        }

        // Ederon : erase message for good
        if (eraseMessages && BoardContext.Current.CurrentForumPage != null)
        {
            BoardContext.Current.Get<ILogger<IRepository<Message>>>().Log(
                BoardContext.Current.PageUserID,
                "YAF",
                BoardContext.Current.Get<ILocalization>().GetTextFormatted("DELETED_MESSAGE", message.ID),
                EventLogTypes.Information);
        }

        repository.DeleteInternal(
            forum,
            topicId,
            message,
            isModeratorChanged,
            deleteReason,
            eraseMessages,
            isTopicDeleteAction);

        // Delete Message from Search Index
        BoardContext.Current.Get<ISearch>().DeleteSearchIndexRecordByMessageId(message.ID);
    }

    /// <summary>
    /// Restore all messages recursively.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    private static void RestoreRecursively(
        this IRepository<Message> repository,
        int forumId,
        int topicId,
        Message message)
    {
        // Restore replies
        var replies = repository.Get(m => m.ReplyTo == message.ID).ToList();

        if (replies.HasItems())
        {
            replies.ForEach(reply => repository.RestoreRecursively(forumId, topicId, reply));
        }

        repository.RestoreInternal(forumId,
            topicId,
            message);
    }

    /// <summary>
    /// moves answers of moved post
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="moveToTopicId">
    /// The move to topic.
    /// </param>
    private static void MoveRecursively(
        this IRepository<Message> repository,
        Message message,
        int moveToTopicId)
    {
        var replies = repository.Get(m => m.ReplyTo == message.ID);

        replies.ForEach(reply => repository.MoveRecursively(reply, moveToTopicId));

        repository.Move(message, moveToTopicId);
    }

    /// <summary>
    /// Execute the actual message delete.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forum">
    /// The forum Id.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is Moderator Changed.
    /// </param>
    /// <param name="deleteReason">
    /// The delete Reason.
    /// </param>
    /// <param name="eraseMessage">
    /// Delete Message from Db, only set IsDeleted flag
    /// </param>
    /// <param name="isTopicDeleteAction">
    /// Indicator if we delete the entire topic
    /// </param>
    private static void DeleteInternal(
        this IRepository<Message> repository,
        Forum forum,
        int topicId,
        Message message,
        bool isModeratorChanged,
        string deleteReason,
        bool eraseMessage,
        bool isTopicDeleteAction)
    {
        // -- Update LastMessageID in Topic
        BoardContext.Current.GetRepository<Topic>().UpdateOnly(
            () => new Topic
                      {
                          LastPosted = null,
                          LastMessageID = null,
                          LastUserID = null,
                          LastUserName = null,
                          LastUserDisplayName = null,
                          LastMessageFlags = null
                      },
            x => x.LastMessageID == message.ID);

        // -- Update LastMessageID in Forum
        BoardContext.Current.GetRepository<Forum>().UpdateOnly(
            () => new Forum
                      {
                          LastPosted = null,
                          LastMessageID = null,
                          LastUserID = null,
                          LastUserName = null,
                          LastUserDisplayName = null
                      },
            x => x.LastMessageID == message.ID);

        // -- should it be physically deleter or not?
        if (eraseMessage)
        {
            BoardContext.Current.GetRepository<Attachment>().Delete(x => x.MessageID == message.ID);
            BoardContext.Current.GetRepository<Activity>().Delete(x => x.MessageID == message.ID);
            BoardContext.Current.GetRepository<MessageReportedAudit>().Delete(x => x.MessageID == message.ID);
            BoardContext.Current.GetRepository<MessageReported>().Delete(x => x.ID == message.ID);
            BoardContext.Current.GetRepository<Thanks>().Delete(x => x.MessageID == message.ID);
            BoardContext.Current.GetRepository<MessageHistory>().Delete(x => x.MessageID == message.ID);

            // -- update message positions inside the topic
            if (!isTopicDeleteAction)
            {
                try
                {
                    repository.UpdateAdd(
                        () => new Message { Position = -1 },
                        x => x.TopicID == topicId && x.Posted > message.Posted && x.ID != message.ID);
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            // -- update ReplyTo
            var replyMessage = repository.GetSingle(
                x => x.TopicID == topicId && x.Position == 0 && x.ID != message.ID);

            if (replyMessage != null)
            {
                repository.UpdateOnly(
                    () => new Message { ReplyTo = replyMessage.ID },
                    x => x.TopicID == topicId && x.ID == message.ID);

                // -- fix Reply To if equal with MessageID
                repository.UpdateOnly(
                    () => new Message { ReplyTo = null },
                    x => x.TopicID == topicId && x.ID == replyMessage.ID);
            }

            // -- finally delete the message we want to delete
            repository.DeleteById(message.ID);

            if (repository.Count(x => x.TopicID == topicId && (x.Flags & 8) != 8) == 0)
            {
               BoardContext.Current.GetRepository<Topic>().Delete(
                    forum.ID,
                    topicId,
                    true);
            }
        }
        else
        {
            var flags = message.MessageFlags;

            flags.IsDeleted = true;

            // -- "Delete" it only by setting deleted flag message
            repository.UpdateOnly(
                () => new Message
                          {
                              IsModeratorChanged = isModeratorChanged, DeleteReason = deleteReason, Flags = flags.BitValue
                          },
                x => x.TopicID == topicId && x.ID == message.ID);
        }

        // -- update user post count
        if (!forum.ForumFlags.IsTest)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

            expression.Join<Topic>((m, t) => t.ID == m.TopicID);
            expression.Join<Topic, Forum>((t, f) => f.ID == t.ForumID);

            expression.Where<Message, Topic, Forum>(
                (m, t, f) => m.UserID == message.UserID && (m.Flags & 8) != 8 && (m.Flags & 16) == 16
                             && (f.Flags & 4) != 4);

           var postCount = repository.DbAccess.Execute(db => db.Connection.Count(expression)).ToType<int>();

            BoardContext.Current.GetRepository<User>().UpdateOnly(
                () => new User { NumPosts = postCount },
                u => u.ID == message.UserID);
        }

        try
        {
            // -- update topic Post Count
            if (!isTopicDeleteAction)
            {
                BoardContext.Current.GetRepository<Topic>().UpdateAdd(
                    () => new Topic {NumPosts = -1},
                    x => x.ID == topicId);
            }
        }
        catch (Exception)
        {
            // Ignore if Post count is wrong
        }
    }

    /// <summary>
    /// Execute the actual message delete.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    private static void RestoreInternal(
        this IRepository<Message> repository,
        int forumId,
        int topicId,
        Message message)
    {
        var flags = message.MessageFlags;

        flags.IsDeleted = false;

        // -- "Delete" it only by setting deleted flag message
        repository.UpdateOnly(
            () => new Message
                  {
                      IsModeratorChanged = false, DeleteReason = null, Flags = flags.BitValue
                  },
            x => x.TopicID == topicId && x.ID == message.ID);

        // -- update user post count
        if (!BoardContext.Current.GetRepository<Forum>()
                .Exists(f => f.ID == forumId && (f.Flags & 4) != 4))
        {
            var postCount = repository.Count(
                x => x.UserID == message.UserID && (x.Flags & 8) != 8 && (x.Flags & 16) == 16).ToType<int>();

            BoardContext.Current.GetRepository<User>().UpdateOnly(
                () => new User { NumPosts = postCount },
                u => u.ID == message.UserID);
        }

        try
        {
            // -- update topic Post Count
            BoardContext.Current.GetRepository<Topic>().UpdateAdd(
                () => new Topic { NumPosts = 1},
                x => x.ID == topicId);
        }
        catch (Exception)
        {
            // Ignore if Post count is wrong
        }
    }

    /// <summary>
    /// Execute the actual message move.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="moveToTopicId">
    /// The move To Topic Id.
    /// </param>
    private static void Move(
        this IRepository<Message> repository,
        Message message,
        int moveToTopicId)
    {
        var replyToId = repository.GetSingle(x => x.Position == 0 && x.TopicID == moveToTopicId)?.ID;

        int position;

        try
        {
            position = repository.DbAccess.Execute(
                db => db.Connection.Scalar<Message, int>(
                    x => Sql.Max(x.Position) + 1,
                    x => x.TopicID == moveToTopicId && x.Posted < message.Posted));
        }
        catch (Exception)
        {
            position = 0;
        }

        repository.UpdateAdd(
            () => new Message { Position = 1 },
            x => x.TopicID == moveToTopicId && x.Posted > message.Posted);

        repository.UpdateAdd(
            () => new Message { Position = -1 },
            x => x.TopicID == message.TopicID && x.Posted > message.Posted);

        // -- Update LastMessageID in Topic & Forum
        BoardContext.Current.GetRepository<Topic>().UpdateOnly(
            () => new Topic
                      {
                          LastPosted = null,
                          LastMessageID = null,
                          LastUserID = null,
                          LastUserName = null,
                          LastUserDisplayName = null,
                          LastMessageFlags = null
                      },
            x => x.LastMessageID == message.ID);

        BoardContext.Current.GetRepository<Forum>().UpdateOnly(
            () => new Forum
                      {
                          LastPosted = null,
                          LastMessageID = null,
                          LastUserID = null,
                          LastUserName = null,
                          LastUserDisplayName = null
                      },
            x => x.LastMessageID == message.ID);

        if (position == 0)
        {
            repository.UpdateOnly(
                () => new Message { ReplyTo = message.ID },
                x => x.TopicID == moveToTopicId && x.ReplyTo == null);

            replyToId = null;
        }

        repository.UpdateOnly(
            () => new Message { TopicID = moveToTopicId, ReplyTo = replyToId, Position = position },
            x => x.ID == message.ID);

        // -- Delete topic if there are no more messages
        if (repository.Count(x => x.TopicID == message.TopicID && (x.Flags & 8) != 8) == 0)
        {
            var forumId = BoardContext.Current.GetRepository<Topic>().GetById(message.TopicID).ForumID;

            BoardContext.Current.GetRepository<Topic>().Delete(forumId, message.TopicID, true);
        }

        // -- update topic Post Count
        BoardContext.Current.GetRepository<Topic>().UpdateAdd(
            () => new Topic { NumPosts = -1 },
            x => x.ID == message.TopicID);

        BoardContext.Current.GetRepository<Topic>().UpdateAdd(
            () => new Topic { NumPosts = 1 },
            x => x.ID == moveToTopicId);
    }

    private static int GetMessageCount(
        int topicId,
        bool showDeleted,
        DateTime sincePostedDate,
        DateTime toPostedDate)
    {
        // -- find total returned count
        return BoardContext.Current.GetRepository<Message>().DbAccess.Execute(
            db =>
                {
                    var countTotalExpression = db.Connection.From<Message>();

                    countTotalExpression.Where<Message>(
                        m => m.TopicID == topicId && (m.Flags & 16) == 16 && m.Posted >= sincePostedDate &&
                             m.Posted <= toPostedDate);

                    if (!showDeleted)
                    {
                        countTotalExpression.And(m => (m.Flags & 8) != 8);
                    }

                    countTotalExpression.Select(Sql.Count($"{countTotalExpression.Column<Message>(x => x.ID)}"));

                    return db.Connection.SqlScalar<int>(countTotalExpression);
                });
    }
}