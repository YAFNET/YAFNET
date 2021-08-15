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
    using System.Globalization;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Types.Objects.Model;

    /// <summary>
    ///     The Message repository extensions.
    /// </summary>
    public static class MessageRepositoryExtensions
    {
        #region Public Methods and Operators

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
            [NotNull] int topicId,
            [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

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
        public static Tuple<Topic, Message, User, Forum> GetMessage(
            this IRepository<Message> repository,
            [NotNull] int messageId)
        {
            CodeContracts.VerifyNotNull(repository);

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
            [NotNull] int messageId,
            [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

            expression.Join<Message>((t, m) => m.TopicID == t.ID).Join<Message, User>((m, u) => u.ID == m.UserID)
                .Join<Forum>((t, f) => f.ID == t.ForumID).LeftJoin<ActiveAccess>((t, x) => x.ForumID == t.ForumID)
                .Where<Message, ActiveAccess>((m, x) => m.ID == messageId && x.UserID == userId && x.ReadAccess);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Topic, Message, User, Forum>(expression))
                .FirstOrDefault();
        }

        /// <summary>
        /// Get Deleted Topics
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
        public static List<Tuple<Forum, Topic, Message>> GetDeletedMessages(
            this IRepository<Message> repository,
            [NotNull] int boardId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Join<Topic>((f, t) => t.ForumID == f.ID).Join<Topic, Message>((t, m) => m.TopicID == t.ID)
                .Where<Message, Category>((m, category) => category.BoardID == boardId && (m.Flags & 8) == 8);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Forum, Topic, Message>(expression));
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
        /// The <see cref="List"/>.
        /// </returns>
        public static List<PagedMessage> PostListPaged(
            this IRepository<Message> repository,
            [NotNull] int topicId,
            [NotNull] int userId,
            [NotNull] bool updateViewCount,
            [NotNull] bool showDeleted,
            [NotNull] DateTime sincePostedDate,
            [NotNull] DateTime toPostedDate,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [NotNull] int messagePosition)
        {
            CodeContracts.VerifyNotNull(repository);

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
                    var totalPages = totalRows / pageSize;

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

                    // -- Count favorite
                    var reputationExpression = db.Connection.From<ReputationVote>(db.Connection.TableAlias("x"));
                    reputationExpression.Where(
                        $@"x.{reputationExpression.Column<ReputationVote>(x => x.ReputationToUserID)}={expression.Column<User>(b => b.ID, true)}
                                        and x.{reputationExpression.Column<ReputationVote>(x => x.ReputationFromUserID)}={userId}");
                    var reputationSql = reputationExpression
                        .Select($"{reputationExpression.Column<ReputationVote>(x => x.VoteDate)}").Limit(1)
                        .ToSelectStatement();

                    var isThankByUserExpression = db.Connection.From<Thanks>(db.Connection.TableAlias("ta"));
                    isThankByUserExpression.Where(
                        $@"ta.{isThankByUserExpression.Column<Thanks>(x => x.ThanksFromUserID)}={userId}
                                    and ta.{isThankByUserExpression.Column<Thanks>(x => x.MessageID)}={expression.Column<Message>(x => x.ID, true)}");
                    var isThankByUserSql = isThankByUserExpression.Select(Sql.Count("1")).ToSelectStatement();

                    var thanksCountExpression = db.Connection.From<Thanks>(db.Connection.TableAlias("ta"));
                    thanksCountExpression.Where(
                        $@"ta.{thanksCountExpression.Column<Thanks>(x => x.MessageID)}={expression.Column<Message>(x => x.ID, true)}");
                    var thanksCountSql = thanksCountExpression
                        .Select(Sql.Count($"{thanksCountExpression.Column<Thanks>(x => x.ID)}")).ToSelectStatement();

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
                            m.ReferenceMessageId,
                            UserName = m.UserName != null ? m.UserName : b.Name,
                            DisplayName = m.UserDisplayName != null ? m.UserDisplayName : b.DisplayName,
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
                            Edited = m.Edited != null ? m.Edited : m.Posted,
                            HasAvatarImage = b.AvatarImage != null ? true : false,
                            IsThankedByUser = Sql.Custom($"({isThankByUserSql})"),
                            ThanksNumber = Sql.Custom($"({thanksCountSql})"),
                            PageIndex = pageIndex,
                            TotalRows = totalRows
                        });

                    return db.Connection.Select<PagedMessage>(expression);
                });
        }

        /// <summary>
        /// The last posts.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static List<Tuple<Message, User>> LastPosts(this IRepository<Message> repository, [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository);

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
        public static IOrderedEnumerable<Tuple<Message, Topic>> GetAllUserMessages(
            this IRepository<Message> repository,
            int userId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

            expression.Join<Topic>((m, t) => t.ID == m.TopicID).Where<Message>(m => m.UserID == userId);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Message, Topic>(expression))
                .OrderByDescending(m => m.Item1.Posted);
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
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Tuple<Message, Topic, User>> GetAllUserMessagesWithAccess(
            this IRepository<Message> repository,
            [NotNull] int boardId,
            [NotNull] int userId,
            [NotNull] int pageUserId,
            [NotNull] int count)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

            expression.Join<User>((a, b) => b.ID == a.UserID)
                .Join<Topic>((a, c) => c.ID == a.TopicID)
                .Join<Topic, Forum>((c, d) => d.ID == c.ForumID)
                .Join<Forum, Category>((d, e) => e.ID == d.CategoryID)
                .Join<Forum, ActiveAccess>((forum, access) => access.ForumID == forum.ID)
                .Where<Topic, Message, ActiveAccess, Category>(
                    (topic, message, x, e) => message.UserID == userId && x.UserID == pageUserId && x.ReadAccess &&
                                              e.BoardID == boardId && (topic.Flags & 8) != 8 &&
                                              (message.Flags & 8) != 8).OrderByDescending<Message>(x => x.Posted)
                .Take(count);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Message, Topic, User>(expression));
        }

        /// <summary>
        /// Gets all messages by forum as Typed Search Message List.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <returns>
        /// Returns all Messages as Typed Search Message List
        /// </returns>
        public static IEnumerable<SearchMessage> GetAllSearchMessagesByForum(
            this IRepository<Message> repository,
            [NotNull] int forumId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

            expression.Join<Topic>((forum, topic) => topic.ForumID == forum.ID)
                .Join<Topic, Message>((topic, message) => message.TopicID == topic.ID)
                .Join<Message, User>((message, user) => user.ID == message.UserID).Where<Forum, Topic, Message>(
                    (forum, topic, message) => forum.ID == forumId && (topic.Flags & 8) != 8 &&
                                               (message.Flags & 8) != 8 && (message.Flags & 16) == 16 &&
                                               topic.TopicMovedID == null).OrderByDescending<Message>(x => x.Posted);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Forum, Topic, Message, User>(expression))
                .ConvertAll(x => new SearchMessage(x));
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
        public static void Approve(this IRepository<Message> repository, [NotNull] int messageId, [NotNull] int forumId)
        {
            CodeContracts.VerifyNotNull(repository);

            var message = BoardContext.Current.GetRepository<Topic>().GetTopicWithMessage(messageId);

            var flags = message.Item1.MessageFlags;

            flags.IsApproved = true;

            // -- update Message table, set message flag to approved
            repository.UpdateFlags(messageId, flags.BitValue);

            if (!BoardContext.Current.GetRepository<Forum>().Exists(f => f.ID == forumId && (f.Flags & 4) == 4))
            {
                // -- update User table to increase post count
                BoardContext.Current.GetRepository<User>().UpdateAdd(
                    () => new User { NumPosts = 1 },
                    u => u.ID == message.Item1.UserID);

                BoardContext.Current.GetRepository<User>().Promote(message.Item1.UserID);
            }

            // -- update Forum table with last topic/post info
            BoardContext.Current.GetRepository<Forum>().UpdateOnly(
                () => new Forum
                {
                    LastPosted = message.Item1.Posted,
                    LastTopicID = message.Item1.TopicID,
                    LastMessageID = message.Item1.ID,
                    LastUserID = message.Item1.UserID,
                    LastUserName = message.Item1.UserName,
                    LastUserDisplayName = message.Item1.UserDisplayName
                },
                x => x.ID == message.Item2.ForumID);

            var numPostsCount = repository.Count(m => m.TopicID == message.Item1.TopicID && (m.Flags & 8) != 8)
                .ToType<int>();

            // -- update Topic table with info about last post in topic
            BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                () => new Topic
                {
                    LastPosted = message.Item1.Posted,
                    LastMessageID = message.Item1.ID,
                    LastUserID = message.Item1.UserID,
                    LastUserName = message.Item1.UserName,
                    LastUserDisplayName = message.Item1.UserDisplayName,
                    LastMessageFlags = flags.BitValue,
                    NumPosts = numPostsCount
                },
                x => x.ID == message.Item1.TopicID);

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
            [NotNull] int messageId,
            [NotNull] int flags)
        {
            CodeContracts.VerifyNotNull(repository);

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
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <param name="isModeratorChanged">
        /// The is moderator changed.
        /// </param>
        /// <param name="deleteReason">
        /// The delete reason.
        /// </param>
        /// <param name="isDeleteAction">
        /// The is delete action.
        /// </param>
        /// <param name="deleteLinked">
        /// The delete linked.
        /// </param>
        public static void Delete(
            this IRepository<Message> repository,
            [NotNull] int forumId,
            [NotNull] int topicId,
            [NotNull] int messageId,
            [NotNull] bool isModeratorChanged,
            [NotNull] string deleteReason,
            [NotNull] bool isDeleteAction,
            [NotNull] bool deleteLinked)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.Delete(
                forumId,
                topicId,
                messageId,
                isModeratorChanged,
                deleteReason,
                isDeleteAction,
                deleteLinked,
                true);

            repository.FireDeleted(messageId);
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
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <param name="isModeratorChanged">
        /// The is moderator changed.
        /// </param>
        /// <param name="deleteReason">
        /// The delete reason.
        /// </param>
        /// <param name="isDeleteAction">
        /// The is delete action.
        /// </param>
        /// <param name="deleteLinked">
        /// The delete linked.
        /// </param>
        /// <param name="eraseMessage">
        /// The erase message.
        /// </param>
        public static void Delete(
            this IRepository<Message> repository,
            [NotNull] int forumId,
            [NotNull] int topicId,
            [NotNull] int messageId,
            [NotNull] bool isModeratorChanged,
            [NotNull] string deleteReason,
            [NotNull] bool isDeleteAction,
            [NotNull] bool deleteLinked,
            [NotNull] bool eraseMessage)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.DeleteRecursively(
                messageId,
                isModeratorChanged,
                deleteReason,
                isDeleteAction,
                deleteLinked,
                eraseMessage);

            BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateTopicLastPostEvent(forumId, topicId));

            BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forumId));
        }

        /// <summary>
        /// message move function
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <param name="moveToTopicId">
        /// The move To Topic Id.
        /// </param>
        /// <param name="moveAll">
        /// The move all.
        /// </param>
        public static void Move(
            this IRepository<Message> repository,
            [NotNull] int messageId,
            [NotNull] int moveToTopicId,
            [NotNull] bool moveAll)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.Move(messageId, moveToTopicId);

            if (moveAll)
            {
                // moveAll=true anyway
                // it's in charge of moving answers of moved post
                var replies = repository.Get(m => m.ReplyTo == messageId).Select(x => x.ID);

                replies.ForEach(replyId => repository.MoveRecursively(replyId, moveToTopicId));
            }

            var newForumId = BoardContext.Current.GetRepository<Topic>().GetById(moveToTopicId).ForumID;
            var oldTopic = BoardContext.Current.GetRepository<Topic>().GetTopicFromMessage(messageId);

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
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [NotNull]
        public static List<Tuple<Message, User>> Replies(this IRepository<Message> repository, [NotNull] int messageId)
        {
            CodeContracts.VerifyNotNull(repository);

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

            expression.Join<User>((message, user) => user.ID == message.UserID).Where<Message>(
                m => (m.Flags & 16) == 16 && m.ReplyTo == messageId);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Message, User>(expression));
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
        /// <returns>
        /// The <see cref="(int MessagePosition, int MessageID)"/>.
        /// </returns>
        public static (int MessagePosition, int MessageID) FindUnread(
            this IRepository<Message> repository,
            [NotNull] int topicId,
            [CanBeNull] int? messageId,
            [NotNull] DateTime lastRead,
            [NotNull] bool showDeleted)
        {
            CodeContracts.VerifyNotNull(repository);

            var minDateTime = DateTimeHelper.SqlDbMinTime().AddYears(-1);

            (int MessagePosition, int MessageID) message = new(0, 0);

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

                        expression.OrderByDescending(m => m.Posted);

                        expression.Limit(1);

                        expression.Select(m => new { MessagePosition = m.Position, MessageID = m.ID });

                        return db.Connection.Select<(int MessagePosition, int MessageID)>(expression).FirstOrDefault();
                    });

                if (message.MessageID > 0)
                {
                    return message;
                }
            }

            // -- if value > yaf db min value (1-1-1903) we are looking for first unread
            if (lastRead > minDateTime)
            {
                // -- a message with the id was not found or we are looking for first unread or last post
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

                        expression.OrderByDescending(m => m.Posted);

                        expression.Limit(1);

                        expression.Select(m => new { MessagePosition = m.Position, MessageID = m.ID });

                        return db.Connection.Select<(int MessagePosition, int MessageID)>(expression).FirstOrDefault();

                    });

                if (message.MessageID > 0)
                {
                    return message;
                }
            }

            // -- if first unread was not found or we looking for last posted
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

                    expression.OrderByDescending(m => m.Posted);

                    expression.Limit(1);

                    expression.Select(m => new { MessagePosition = m.Position, MessageID = m.ID });

                    return db.Connection.Select<(int MessagePosition, int MessageID)>(expression).FirstOrDefault();

                });

            return message;
        }

        /// <summary>
        /// Copy current Message text over reported Message text.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageFlag">
        /// The message flag.
        /// </param>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        public static void ReportResolve(
            this IRepository<Message> repository,
            [NotNull] int messageFlag,
            [NotNull] int messageId,
            [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository);

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
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="userId">
        /// The user Id.
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
        public static int SaveNew(
            this IRepository<Message> repository,
            [NotNull] int forumId,
            [NotNull] int topicId,
            [NotNull] int userId,
            [NotNull] string message,
            [NotNull] string guestUserName,
            [NotNull] string ipAddress,
            [NotNull] DateTime posted,
            [CanBeNull] int? replyTo,
            [NotNull] MessageFlags flags)
        {
            CodeContracts.VerifyNotNull(repository);

            int position;

            var maxPosition = repository.DbAccess.Execute(
                db => db.Connection.Scalar<Message, int?>(m => Sql.Max(m.Position), m => m.TopicID == topicId));

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
                    u => u.ID == userId);
            }

            var replaceName = BoardContext.Current.IsGuest ? guestUserName : BoardContext.Current.User.DisplayName;

            var newMessageId = repository.Insert(
                new Message
                {
                    UserID = userId,
                    MessageText = message,
                    TopicID = topicId,
                    Posted = posted,
                    UserName = guestUserName,
                    UserDisplayName = replaceName,
                    IP = ipAddress,
                    ReplyTo = replyTo,
                    Position = position,
                    Indent = 0,
                    Flags = flags.BitValue,
                    ExternalMessageId = null,
                    ReferenceMessageId = null
                });

            // Add to search index
            var newMessage = new SearchMessage
            {
                MessageId = newMessageId,
                Message = message,
                Flags = flags.BitValue,
                Posted = posted.ToString(CultureInfo.InvariantCulture),
                UserName = BoardContext.Current.MembershipUser.UserName,
                UserDisplayName = BoardContext.Current.User.DisplayName,
                UserStyle = BoardContext.Current.User.UserStyle,
                UserId = BoardContext.Current.PageUserID,
                TopicId = BoardContext.Current.PageTopicID,
                Topic = BoardContext.Current.PageTopicName,
                ForumId = forumId,
                TopicTags = string.Empty,
                ForumName = BoardContext.Current.PageForumName,
                Description = string.Empty
            };

            BoardContext.Current.Get<ISearch>().AddSearchIndexItem(newMessage);

            if (flags.IsApproved)
            {
                repository.Approve(newMessageId, forumId);
                BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forumId));
            }

            repository.FireNew(newMessageId);

            return newMessageId;
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
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Tuple<Topic, Message, User>> Unapproved(
            this IRepository<Message> repository,
            [NotNull] int forumId)
        {
            CodeContracts.VerifyNotNull(repository);

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
        /// <param name="originalMessage">
        /// The original Message.
        /// </param>
        /// <param name="editedBy">
        /// UserId of who edited the message.
        /// </param>
        public static void Update(
            this IRepository<Message> repository,
            [CanBeNull] short? priority,
            [NotNull] string message,
            [CanBeNull] string description,
            [CanBeNull] string status,
            [CanBeNull] string styles,
            [CanBeNull] string subject,
            [NotNull] string reasonOfEdit,
            [NotNull] bool isModeratorChanged,
            [NotNull] bool overrideApproval,
            [NotNull] Tuple<Topic, Message, User, Forum> originalMessage,
            [NotNull] int editedBy)
        {
            CodeContracts.VerifyNotNull(repository);

            if (overrideApproval || !originalMessage.Item4.ForumFlags.IsModerated)
            {
                originalMessage.Item2.MessageFlags.IsApproved = true;
            }

            if (BoardContext.Current.GetRepository<MessageHistory>()
                .Exists(m => m.MessageID == originalMessage.Item2.ID))
            {
                // -- insert current message variant - use OriginalMessage in future
                BoardContext.Current.GetRepository<MessageHistory>().Insert(
                    new MessageHistory
                    {
                        MessageID = originalMessage.Item2.ID,
                        Message = originalMessage.Item2.MessageText,
                        IP = originalMessage.Item2.IP,
                        Edited = DateTime.UtcNow,
                        EditedBy = editedBy,
                        EditReason = reasonOfEdit,
                        IsModeratorChanged = originalMessage.Item2.IsModeratorChanged.Value,
                        Flags = originalMessage.Item2.Flags
                    });
            }
            else
            {
                // -- save original message in the history if this is the first edit
                BoardContext.Current.GetRepository<MessageHistory>().Insert(
                    new MessageHistory
                    {
                        MessageID = originalMessage.Item2.ID,
                        Message = originalMessage.Item2.MessageText,
                        IP = originalMessage.Item2.IP,
                        Edited = originalMessage.Item2.Posted,
                        EditedBy = originalMessage.Item2.UserID,
                        EditReason = null,
                        IsModeratorChanged = originalMessage.Item2.IsModeratorChanged.Value,
                        Flags = originalMessage.Item2.Flags
                    });
            }

            repository.UpdateOnly(
                () => new Message
                {
                    MessageText = message,
                    Edited = DateTime.UtcNow,
                    EditedBy = editedBy,
                    Flags = originalMessage.Item2.MessageFlags.BitValue,
                    IsModeratorChanged = isModeratorChanged,
                    EditReason = reasonOfEdit
                },
                m => m.ID == originalMessage.Item2.ID);

            if (priority.HasValue)
            {
                BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                    () => new Topic { Priority = priority.Value },
                    t => t.ID == originalMessage.Item1.ID);
            }

            if (subject.IsSet())
            {
                BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                    () => new Topic
                    {
                        TopicName = subject, Description = description, Status = status, Styles = styles
                    },
                    t => t.ID == originalMessage.Item1.ID);
            }

            // Update Search index Item
            var updateMessage = new SearchMessage
            {
                MessageId = originalMessage.Item2.ID,
                Message = message,
                Flags = originalMessage.Item1.Flags,
                Posted = originalMessage.Item1.Posted.ToString(CultureInfo.InvariantCulture),
                UserName = originalMessage.Item4.Name,
                UserDisplayName = originalMessage.Item3.DisplayName,
                UserStyle = originalMessage.Item3.UserStyle,
                UserId = originalMessage.Item4.ID,
                TopicId = originalMessage.Item2.ID,
                Topic = originalMessage.Item1.TopicName,
                ForumId = originalMessage.Item3.ID,
                ForumName = originalMessage.Item3.Name,
                Description = description
            };

            BoardContext.Current.Get<ISearch>().UpdateSearchIndexItem(updateMessage, true);

            if (originalMessage.Item4.ForumFlags.IsModerated)
            {
                // If forum is moderated, make sure last post pointers are correct
                BoardContext.Current.Get<IRaiseEvent>().Raise(
                    new UpdateTopicLastPostEvent(originalMessage.Item4.ID, originalMessage.Item1.ID));
            }
        }

        #endregion

        /// <summary>
        /// Delete all messages recursively.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <param name="isModeratorChanged">
        /// The is moderator changed.
        /// </param>
        /// <param name="deleteReason">
        /// The delete reason.
        /// </param>
        /// <param name="isDeleteAction">
        /// The is delete action.
        /// </param>
        /// <param name="deleteLinked">
        /// The delete linked.
        /// </param>
        /// <param name="eraseMessages">
        /// The erase messages.
        /// </param>
        private static void DeleteRecursively(
            this IRepository<Message> repository,
            [NotNull] int messageID,
            [NotNull] bool isModeratorChanged,
            [NotNull] string deleteReason,
            [NotNull] bool isDeleteAction,
            [NotNull] bool deleteLinked,
            [NotNull] bool eraseMessages)
        {
            CodeContracts.VerifyNotNull(repository);

            var useFileTable = BoardContext.Current.BoardSettings.UseFileTable;

            if (deleteLinked)
            {
                // Delete replies
                var replies = repository.Get(m => m.ReplyTo == messageID).Select(x => x.ID).ToList();

                if (replies.Any())
                {
                    replies.ForEach(
                        replyId => repository.DeleteRecursively(
                            replyId,
                            isModeratorChanged,
                            deleteReason,
                            isDeleteAction,
                            true,
                            eraseMessages));
                }
            }

            // If the files are actually saved in the Hard Drive
            if (!useFileTable)
            {
                BoardContext.Current.GetRepository<Attachment>().DeleteByMessageId(messageID);
            }

            // Ederon : erase message for good
            if (eraseMessages)
            {
                BoardContext.Current.Get<ILoggerService>().Log(
                    BoardContext.Current.PageUserID,
                    "YAF",
                    BoardContext.Current.Get<ILocalization>().GetTextFormatted("DELETED_MESSAGE", messageID),
                    EventLogTypes.Information);
            }

            repository.Delete(messageID, isModeratorChanged, deleteReason, eraseMessages, isDeleteAction);

            // Delete Message from Search Index
            if (isDeleteAction)
            {
                BoardContext.Current.Get<ISearch>().DeleteSearchIndexRecordByMessageId(messageID);
            }
        }

        /// <summary>
        /// moves answers of moved post
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="moveToTopicId">
        /// The move to topic.
        /// </param>
        private static void MoveRecursively(
            this IRepository<Message> repository,
            [NotNull] int messageId,
            [NotNull] int moveToTopicId)
        {
            CodeContracts.VerifyNotNull(repository);

            var replies = repository.Get(m => m.ReplyTo == messageId).Select(x => x.ID);

            replies.ForEach(replyId => repository.MoveRecursively(replyId, moveToTopicId));

            repository.Move(messageId, moveToTopicId);
        }

        /// <summary>
        /// Execute the actual message delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="isModeratorChanged">
        /// The is Moderator Changed.
        /// </param>
        /// <param name="deleteReason">
        /// The delete Reason.
        /// </param>
        /// <param name="eraseMessage">
        /// The erase Message.
        /// </param>
        /// <param name="isDeleteAction">
        /// The is Delete Action.
        /// </param>
        private static void Delete(
            this IRepository<Message> repository,
            [NotNull] int messageId,
            [NotNull] bool isModeratorChanged,
            [NotNull] string deleteReason,
            [NotNull] bool eraseMessage,
            [NotNull] bool isDeleteAction)
        {
            CodeContracts.VerifyNotNull(repository);

            // -- Find TopicID and ForumID
            var message = BoardContext.Current.GetRepository<Topic>().GetTopicWithMessage(messageId);

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
                x => x.LastMessageID == messageId);

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
                x => x.LastMessageID == messageId);

            // -- should it be physically deleter or not?
            if (eraseMessage)
            {
                BoardContext.Current.GetRepository<Attachment>().Delete(x => x.MessageID == messageId);
                BoardContext.Current.GetRepository<Activity>().Delete(x => x.MessageID == messageId);
                BoardContext.Current.GetRepository<MessageReportedAudit>().Delete(x => x.MessageID == messageId);
                BoardContext.Current.GetRepository<MessageReported>().Delete(x => x.ID == messageId);
                BoardContext.Current.GetRepository<Thanks>().Delete(x => x.MessageID == messageId);
                BoardContext.Current.GetRepository<MessageHistory>().Delete(x => x.MessageID == messageId);

                // -- update message positions inside the topic
                repository.UpdateAdd(
                    () => new Message { Position = -1 },
                    x => x.TopicID == message.Item1.TopicID && x.Posted > message.Item1.Posted && x.ID == messageId);

                // -- update ReplyTo
                var replyMessage = repository.GetSingle(
                    x => x.TopicID == message.Item1.TopicID && x.Position == 0 && x.ID != messageId);

                if (replyMessage != null)
                {
                    repository.UpdateOnly(
                        () => new Message { ReplyTo = replyMessage.ID },
                        x => x.TopicID == message.Item1.TopicID && x.ID == messageId);

                    // -- fix Reply To if equal with MessageID
                    repository.UpdateOnly(
                        () => new Message { ReplyTo = null },
                        x => x.TopicID == message.Item1.TopicID && x.ID == replyMessage.ID);
                }

                // -- finally delete the message we want to delete
                repository.DeleteById(messageId);

                if (repository.Count(x => x.TopicID == message.Item1.TopicID && (x.Flags & 8) != 8) == 0)
                {
                    BoardContext.Current.GetRepository<Topic>().Delete(
                        message.Item2.ForumID,
                        message.Item1.TopicID,
                        true);
                }
            }
            else
            {
                var flags = message.Item1.MessageFlags;

                flags.IsDeleted = isDeleteAction;

                // -- "Delete" it only by setting deleted flag message
                repository.UpdateOnly(
                    () => new Message
                    {
                        IsModeratorChanged = isModeratorChanged, DeleteReason = deleteReason, Flags = flags.BitValue
                    },
                    x => x.TopicID == message.Item1.TopicID && x.ID == messageId);
            }

            // -- update user post count
            if (!BoardContext.Current.GetRepository<Forum>()
                .Exists(f => f.ID == message.Item2.ForumID && (f.Flags & 4) != 4))
            {
                var postCount = repository.Count(
                    x => x.UserID == message.Item1.UserID && (x.Flags & 8) != 8 && (x.Flags & 16) == 16).ToType<int>();

                BoardContext.Current.GetRepository<User>().UpdateOnly(
                    () => new User { NumPosts = postCount },
                    u => u.ID == message.Item1.UserID);
            }

            // -- update topic Post Count
            BoardContext.Current.GetRepository<Topic>().UpdateAdd(
                () => new Topic { NumPosts = -1 },
                x => x.ID == message.Item1.TopicID);
        }

        /// <summary>
        /// Execute the actual message move.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="moveToTopicId">
        /// The move To Topic Id.
        /// </param>
        private static void Move(
            this IRepository<Message> repository,
            [NotNull] int messageId,
            [NotNull] int moveToTopicId)
        {
            CodeContracts.VerifyNotNull(repository);

            // -- Find TopicID and ForumID
            var message = BoardContext.Current.GetRepository<Topic>().GetTopicWithMessage(messageId);

            int? replyToId = repository.GetSingle(x => x.Position == 0 && x.TopicID == moveToTopicId).ID;

            int position;

            try
            {
                position = repository.DbAccess.Execute(
                    db => db.Connection.Scalar<Message, int>(
                        x => Sql.Max(x.Position) + 1,
                        x => x.TopicID == moveToTopicId && x.Posted < message.Item1.Posted));
            }
            catch (Exception)
            {
                position = 0;
            }

            repository.UpdateAdd(
                () => new Message { Position = 1 },
                x => x.TopicID == moveToTopicId && x.Posted > message.Item1.Posted);

            repository.UpdateAdd(
                () => new Message { Position = -1 },
                x => x.TopicID == message.Item1.TopicID && x.Posted > message.Item1.Posted);

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
                x => x.LastMessageID == messageId);

            BoardContext.Current.GetRepository<Forum>().UpdateOnly(
                () => new Forum
                {
                    LastPosted = null,
                    LastMessageID = null,
                    LastUserID = null,
                    LastUserName = null,
                    LastUserDisplayName = null
                },
                x => x.LastMessageID == messageId);

            if (position == 0)
            {
                repository.UpdateOnly(
                    () => new Message { ReplyTo = messageId },
                    x => x.TopicID == moveToTopicId && x.ReplyTo == null);

                replyToId = null;
            }

            repository.UpdateOnly(
                () => new Message { TopicID = moveToTopicId, ReplyTo = replyToId, Position = position },
                x => x.ID == messageId);

            // -- Delete topic if there are no more messages
            if (repository.Count(x => x.TopicID == message.Item1.TopicID && (x.Flags & 8) != 8) == 0)
            {
                BoardContext.Current.GetRepository<Topic>().Delete(message.Item2.ForumID, message.Item1.TopicID, true);
            }

            // -- update topic Post Count
            BoardContext.Current.GetRepository<Topic>().UpdateAdd(
                () => new Topic { NumPosts = -1 },
                x => x.ID == message.Item1.TopicID);

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
}