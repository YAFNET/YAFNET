/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
    /// <param name="repository">
    /// The repository.
    /// </param>
    extension(IRepository<Message> repository)
    {
        /// <summary>
        /// Checks if the User has replied to the specific topic.
        /// </summary>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns if true or not
        /// </returns>
        public bool RepliedTopic(int topicId,
            int userId)
        {
            return repository.Count(m => m.TopicID == topicId && m.UserID == userId) > 0;
        }

        /// <summary>
        /// The get message.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        public Message GetMessage(int messageId)
        {
            return repository.DbAccess.Execute(db => db.Connection.LoadSingleById<Message>(messageId));
        }

        /// <summary>
        /// The get message.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        public Tuple<Topic, Message, User, Forum> GetMessageAsTuple(int messageId)
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
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        public Tuple<Topic, Message, User, Forum> GetMessageWithAccess(int messageId,
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
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// List of deleted posts.
        /// </returns>
        public List<Tuple<Forum, Topic, Message>> GetDeletedMessages(int boardId)
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
        public List<PagedMessage> GetDeletedMessagesPaged(int boardId,
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

    /// <param name="repository">
    /// The repository.
    /// </param>
    extension(IRepository<Message> repository)
    {
        /// <summary>
        /// Gets the 10 Newest Posts of a Topic
        /// </summary>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        public List<Tuple<Message, User>> LastPosts(int topicId)
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
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// Returns all the post by a user.
        /// </returns>
        public IOrderedEnumerable<Message> GetAllUserMessages(int userId)
        {
            return repository.DbAccess.Execute(db => db.Connection.LoadSelect<Message>(m => m.UserID == userId)
                .OrderByDescending(m => m.Posted));
        }

        /// <summary>
        /// Gets all the post by a user. (With Read Access of current Page User)
        /// </summary>
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
        public List<Tuple<Message, Topic, User>> GetAllUserMessagesWithAccess(int boardId,
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
        /// <param name="forumId">
        ///     The forum Id.
        /// </param>
        /// <param name="topicTags"></param>
        /// <returns>
        /// Returns all Messages as Typed Search Message List
        /// </returns>
        public IEnumerable<SearchMessage> GetAllSearchMessagesByForum(int forumId, List<Tag> topicTags)
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
        /// <param name="messageId">
        /// The message identifier.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        public async Task ApproveAsync(int messageId, int forumId)
        {
            var message = repository.GetMessage(messageId);

            var flags = message.MessageFlags;

            flags.IsApproved = true;

            // -- update Message table, set message flag to approved
            await repository.UpdateFlagsAsync(messageId, flags.BitValue);

            if (!await BoardContext.Current.GetRepository<Forum>().ExistsAsync(f => f.ID == forumId && (f.Flags & 4) == 4))
            {
                // -- update User table to increase post count
                await BoardContext.Current.GetRepository<User>().UpdateAddAsync(
                    () => new User { NumPosts = 1 },
                    u => u.ID == message.UserID);

                await BoardContext.Current.GetRepository<User>().PromoteAsync(message.UserID);
            }

            // -- update Forum table with last topic/post info
            await BoardContext.Current.GetRepository<Forum>().UpdateOnlyAsync(
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

            var numPostsCount = (await repository.CountAsync(m => m.TopicID == message.TopicID && (m.Flags & 8) != 8))
                .ToType<int>();

            // -- update Topic table with info about last post in topic
            await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
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
        }

        /// <summary>
        /// Updates the flags.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="flags">The flags.</param>
        public Task UpdateFlagsAsync(int messageId,
            int flags)
        {
            return repository.UpdateOnlyAsync(() => new Message { Flags = flags }, u => u.ID == messageId);
        }

        /// <summary>
        /// Delete Message(s)
        /// </summary>
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
        public async Task DeleteAsync(int forumId,
            int topicId,
            Message message,
            bool isModeratorChanged,
            string deleteReason,
            bool deleteLinked,
            bool eraseMessage,
            bool isTopicDeleteAction = false)
        {
            var forum = await BoardContext.Current.GetRepository<Forum>().GetByIdAsync(forumId);

            await repository.DeleteRecursivelyAsync(
                forum,
                topicId,
                message,
                isModeratorChanged,
                deleteReason,
                deleteLinked,
                eraseMessage,
                isTopicDeleteAction);

            await BoardContext.Current.Get<IRaiseEventAsync>().RaiseAsync(new UpdateTopicLastPostEvent(forumId, topicId));

            BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forumId));
        }

        /// <summary>
        /// Restore Message(s)
        /// </summary>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public async Task RestoreAsync(int forumId,
            int topicId,
            Message message)
        {
            await repository.RestoreRecursivelyAsync(
                forumId,
                topicId,
                message);

            await BoardContext.Current.Get<IRaiseEventAsync>().RaiseAsync(new UpdateTopicLastPostEvent(forumId, topicId));

            BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forumId));
        }

        /// <summary>
        /// message move function
        /// </summary>
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
        public async Task MoveAsync(Topic oldTopic,
            Message message,
            int moveToTopicId,
            bool moveAll)
        {
            await repository.MoveAsync(message, moveToTopicId);

            if (moveAll)
            {
                // moveAll=true anyway
                // it's in charge of moving answers of moved post
                var replies = await repository.GetAsync(m => m.ReplyTo == message.ID);

                foreach (var reply in replies)
                {
                    await repository.MoveRecursivelyAsync(reply, moveToTopicId);
                }
            }

            var topic = await BoardContext.Current.GetRepository<Topic>().GetByIdAsync(moveToTopicId);

            var newForumId = topic.ForumID;

            await BoardContext.Current.Get<IRaiseEventAsync>().RaiseAsync(new UpdateTopicLastPostEvent(newForumId, moveToTopicId));
            await BoardContext.Current.Get<IRaiseEventAsync>().RaiseAsync(new UpdateTopicLastPostEvent(oldTopic.ForumID, oldTopic.ID));

            BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(newForumId));
            BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(oldTopic.ForumID));
        }

        /// <summary>
        /// gets list of replies to message
        /// </summary>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        public List<Message> Replies(int messageId)
        {
            return repository.DbAccess.Execute(
                db => db.Connection.LoadSelect<Message>(m => (m.Flags & 16) == 16 && m.ReplyTo == messageId));
        }

        /// <summary>
        /// Finds the first Unread or Newest Message
        /// </summary>
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
        public (int MessagePosition, int MessageID) FindUnread(int topicId,
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
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        public async Task ReportResolveAsync(int messageId,
            int userId)
        {
            await BoardContext.Current.GetRepository<MessageReported>().UpdateOnlyAsync(
                () => new MessageReported { Resolved = true, ResolvedBy = userId, ResolvedDate = DateTime.UtcNow },
                m => m.ID == messageId);

            var flags = new MessageFlags(
                await repository.DbAccess.ExecuteAsync(
                    db => db.ScalarAsync<Message, int>(m => m.Flags, m => m.ID == messageId)))
            {
                IsReported = false
            };

            await repository.UpdateFlagsAsync(messageId, flags.BitValue);

            BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));
        }

        /// <summary>
        /// Saves the new Message
        /// </summary>
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
        public async Task<Message> SaveNewAsync(Forum forum,
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
                await BoardContext.Current.GetRepository<User>().UpdateAddAsync(
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

            var newMessageId = await repository.InsertAsync(
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
                await repository.ApproveAsync(newMessageId, forum.ID);
                BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forum.ID));
            }

            return newMessage;
        }

        /// <summary>
        /// The message_unapproved.
        /// </summary>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        public Task<List<Tuple<Topic, Message, User>>> UnapprovedAsync(int forumId)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

            expression.Join<Message>((topic, message) => message.TopicID == topic.ID)
                .Join<Message, User>((message, user) => message.UserID == user.ID).Where<Topic, Message>(
                    (topic, message) => topic.ForumID == forumId && (message.Flags & 16) != 16 &&
                                        (topic.Flags & 8) != 8 && (message.Flags & 8) != 8);

            return repository.DbAccess.ExecuteAsync(db => db.SelectMultiAsync<Topic, Message, User>(expression));
        }

        /// <summary>
        /// Updating the Message (Post)
        /// </summary>
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
        public async Task UpdateAsync(short? priority,
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

            if (await BoardContext.Current.GetRepository<MessageHistory>()
                    .ExistsAsync(m => m.MessageID == originalMessage.ID))
            {
                // -- insert current message variant - use OriginalMessage in future
                await BoardContext.Current.GetRepository<MessageHistory>().InsertAsync(
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
                await BoardContext.Current.GetRepository<MessageHistory>().InsertAsync(
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

            await repository.UpdateOnlyAsync(
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
                await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
                    () => new Topic { Priority = priority.Value },
                    t => t.ID == originalMessage.TopicID);
            }

            if (subject.IsSet())
            {
                await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
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
                await BoardContext.Current.Get<IRaiseEventAsync>().RaiseAsync(
                    new UpdateTopicLastPostEvent(forum.ID, originalMessage.TopicID));
            }
        }

        /// <summary>
        /// Gets the user last posted date time.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>System.Nullable&lt;System.DateTime&gt;.</returns>
        public DateTime? GetUserLastPosted(int userId)
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
        private async Task DeleteRecursivelyAsync(Forum forum,
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
                var replies = await repository.GetAsync(m => m.ReplyTo == message.ID);

                if (replies.HasItems())
                {
                    foreach (var reply in replies)
                    {
                        await repository.DeleteRecursivelyAsync(
                            forum,
                            topicId,
                            reply,
                            isModeratorChanged,
                            deleteReason,
                            true,
                            eraseMessages,
                            isTopicDeleteAction);
                    }
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

            await repository.DeleteInternalAsync(
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
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private async Task RestoreRecursivelyAsync(int forumId,
            int topicId,
            Message message)
        {
            // Restore replies
            var replies = (await repository.GetAsync(m => m.ReplyTo == message.ID)).ToList();

            if (replies.HasItems())
            {
                foreach (var reply in replies)
                {
                    await repository.RestoreRecursivelyAsync(forumId, topicId, reply);
                }
            }

            await repository.RestoreInternalAsync(forumId,
                topicId,
                message);
        }

        /// <summary>
        /// moves answers of moved post
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="moveToTopicId">
        /// The move to topic.
        /// </param>
        private async Task MoveRecursivelyAsync(Message message,
            int moveToTopicId)
        {
            var replies = await repository.GetAsync(m => m.ReplyTo == message.ID);

            foreach (var reply in replies)
            {
                await repository.MoveRecursivelyAsync(reply, moveToTopicId);
            }

            await repository.MoveAsync(message, moveToTopicId);
        }

        /// <summary>
        /// Execute the actual message delete.
        /// </summary>
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
        private async Task DeleteInternalAsync(Forum forum,
            int topicId,
            Message message,
            bool isModeratorChanged,
            string deleteReason,
            bool eraseMessage,
            bool isTopicDeleteAction)
        {
            // -- Update LastMessageID in Topic
            await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
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
            await BoardContext.Current.GetRepository<Forum>().UpdateOnlyAsync(
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
                await BoardContext.Current.GetRepository<Attachment>().DeleteAsync(x => x.MessageID == message.ID);
                await BoardContext.Current.GetRepository<Activity>().DeleteAsync(x => x.MessageID == message.ID);
                await BoardContext.Current.GetRepository<MessageReportedAudit>().DeleteAsync(x => x.MessageID == message.ID);
                await BoardContext.Current.GetRepository<MessageReported>().DeleteAsync(x => x.ID == message.ID);
                await BoardContext.Current.GetRepository<Thanks>().DeleteAsync(x => x.MessageID == message.ID);
                await BoardContext.Current.GetRepository<MessageHistory>().DeleteAsync(x => x.MessageID == message.ID);

                // -- update message positions inside the topic
                if (!isTopicDeleteAction)
                {
                    try
                    {
                        await repository.UpdateAddAsync(
                            () => new Message { Position = -1 },
                            x => x.TopicID == topicId && x.Posted > message.Posted && x.ID != message.ID);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                // -- update ReplyTo
                var replyMessage = await repository.GetSingleAsync(
                    x => x.TopicID == topicId && x.Position == 0 && x.ID != message.ID);

                if (replyMessage != null)
                {
                    await repository.UpdateOnlyAsync(
                        () => new Message { ReplyTo = replyMessage.ID },
                        x => x.TopicID == topicId && x.ID == message.ID);

                    // -- fix Reply To if equal with MessageID
                    await repository.UpdateOnlyAsync(
                        () => new Message { ReplyTo = null },
                        x => x.TopicID == topicId && x.ID == replyMessage.ID);
                }

                // -- finally delete the message we want to delete
                await repository.DeleteByIdAsync(message.ID);

                if (await repository.CountAsync(x => x.TopicID == topicId && (x.Flags & 8) != 8) == 0)
                {
                    await BoardContext.Current.GetRepository<Topic>().DeleteAsync(
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
                await repository.UpdateOnlyAsync(
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

                await BoardContext.Current.GetRepository<User>().UpdateOnlyAsync(
                    () => new User { NumPosts = postCount },
                    u => u.ID == message.UserID);
            }

            try
            {
                // -- update topic Post Count
                if (!isTopicDeleteAction)
                {
                    await BoardContext.Current.GetRepository<Topic>().UpdateAddAsync(
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
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private async Task RestoreInternalAsync(int forumId,
            int topicId,
            Message message)
        {
            var flags = message.MessageFlags;

            flags.IsDeleted = false;

            // -- "Delete" it only by setting deleted flag message
            await repository.UpdateOnlyAsync(
                () => new Message
                {
                    IsModeratorChanged = false, DeleteReason = null, Flags = flags.BitValue
                },
                x => x.TopicID == topicId && x.ID == message.ID);

            // -- update user post count
            if (!await BoardContext.Current.GetRepository<Forum>()
                    .ExistsAsync(f => f.ID == forumId && (f.Flags & 4) != 4))
            {
                var postCount = (await repository.CountAsync(
                    x => x.UserID == message.UserID && (x.Flags & 8) != 8 && (x.Flags & 16) == 16)).ToType<int>();

                await BoardContext.Current.GetRepository<User>().UpdateOnlyAsync(
                    () => new User { NumPosts = postCount },
                    u => u.ID == message.UserID);
            }

            try
            {
                // -- update topic Post Count
                await BoardContext.Current.GetRepository<Topic>().UpdateAddAsync(
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
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="moveToTopicId">
        /// The move To Topic Id.
        /// </param>
        private async Task MoveAsync(Message message,
            int moveToTopicId)
        {
            var moveToMessage = await repository.GetSingleAsync(x => x.Position == 0 && x.TopicID == moveToTopicId);

            var replyToId = moveToMessage?.ID;

            int position;

            try
            {
                position = await repository.DbAccess.ExecuteAsync(
                    db => db.ScalarAsync<Message, int>(
                        x => Sql.Max(x.Position) + 1,
                        x => x.TopicID == moveToTopicId && x.Posted < message.Posted));
            }
            catch (Exception)
            {
                position = 0;
            }

            await repository.UpdateAddAsync(
                () => new Message { Position = 1 },
                x => x.TopicID == moveToTopicId && x.Posted > message.Posted);

            await repository.UpdateAddAsync(
                () => new Message { Position = -1 },
                x => x.TopicID == message.TopicID && x.Posted > message.Posted);

            // -- Update LastMessageID in Topic & Forum
            await BoardContext.Current.GetRepository<Topic>().UpdateOnlyAsync(
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

            await BoardContext.Current.GetRepository<Forum>().UpdateOnlyAsync(
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
                await repository.UpdateOnlyAsync(
                    () => new Message { ReplyTo = message.ID },
                    x => x.TopicID == moveToTopicId && x.ReplyTo == null);

                replyToId = null;
            }

            await repository.UpdateOnlyAsync(
                () => new Message { TopicID = moveToTopicId, ReplyTo = replyToId, Position = position },
                x => x.ID == message.ID);

            // -- Delete topic if there are no more messages
            if (await repository.CountAsync(x => x.TopicID == message.TopicID && (x.Flags & 8) != 8) == 0)
            {
                var topic = await BoardContext.Current.GetRepository<Topic>().GetByIdAsync(message.TopicID);
                var forumId = topic.ForumID;

                await BoardContext.Current.GetRepository<Topic>().DeleteAsync(forumId, message.TopicID, true);
            }

            // -- update topic Post Count
            await BoardContext.Current.GetRepository<Topic>().UpdateAddAsync(
                () => new Topic { NumPosts = -1 },
                x => x.ID == message.TopicID);

            await BoardContext.Current.GetRepository<Topic>().UpdateAddAsync(
                () => new Topic { NumPosts = 1 },
                x => x.ID == moveToTopicId);
        }
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