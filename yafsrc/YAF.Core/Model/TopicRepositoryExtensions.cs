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
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects;

    #endregion

    /// <summary>
    ///     The Topic repository extensions.
    /// </summary>
    public static class TopicRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Sets the answer message.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void SetAnswerMessage(this IRepository<Topic> repository, [NotNull] int topicId, [NotNull] int messageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Topic { AnswerMessageId = messageId }, t => t.ID == topicId);
        }

        /// <summary>
        /// Removes answer message.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        public static void RemoveAnswerMessage(this IRepository<Topic> repository, [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Topic { AnswerMessageId = null }, t => t.ID == topicId);
        }

        /// <summary>
        /// Gets the answer message.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <returns>Returns the Answer Message identifier</returns>
        public static int? GetAnswerMessage(this IRepository<Topic> repository, [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var topic = repository.GetById(topicId);

            return topic?.AnswerMessageId;
        }

        /// <summary>
        /// Locks/Unlock the topic.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="flags">The topic flags.</param>
        public static void LockTopic(this IRepository<Topic> repository, [NotNull] int topicId, [NotNull] int flags)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Topic { Flags = flags }, t => t.ID == topicId);
        }

        /// <summary>
        /// The unanswered as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param>
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should contain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Topics Unanswered
        /// </returns>
        public static DataTable UnansweredAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [CanBeNull] object categoryId,
            [NotNull] int pageUserId,
            [NotNull] DateTime sinceDate,
            [NotNull] DateTime toDate,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [NotNull] bool useStyledNicks,
            [CanBeNull] bool findLastRead = false)
        {
            return repository.DbFunction.GetData.topic_unanswered(
                BoardID: boardId,
                CategoryID: categoryId,
                PageUserID: pageUserId,
                SinceDate: sinceDate,
                ToDate: toDate,
                PageIndex: pageIndex,
                PageSize: pageSize,
                StyledNicks: useStyledNicks,
                FindLastRead: findLastRead);
        }

        /// <summary>
        /// Returns Topics Unread by a user
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param>
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should contain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Topics Unread be a PageUserId
        /// </returns>
        public static DataTable ActiveAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [CanBeNull] object categoryId,
            [NotNull] int pageUserId,
            [NotNull] DateTime sinceDate,
            [NotNull] DateTime toDate,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [NotNull] bool useStyledNicks,
            [CanBeNull] bool findLastRead = false)
        {
            return repository.DbFunction.GetData.topic_active(
                BoardID: boardId,
                CategoryID: categoryId,
                PageUserID: pageUserId,
                SinceDate: sinceDate,
                ToDate: toDate,
                PageIndex: pageIndex,
                PageSize: pageSize,
                StyledNicks: useStyledNicks,
                FindLastRead: findLastRead);
        }

        /// <summary>
        /// Returns Topics Unread by a user
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param>
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should contain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Topics Unread be a PageUserId
        /// </returns>
        public static DataTable UnreadAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [CanBeNull] object categoryId,
            [NotNull] int pageUserId,
            [NotNull] DateTime sinceDate,
            [NotNull] DateTime toDate,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [NotNull] bool useStyledNicks,
            [CanBeNull] bool findLastRead = false)
        {
            return repository.DbFunction.GetData.topic_unread(
                BoardID: boardId,
                CategoryID: categoryId,
                PageUserID: pageUserId,
                SinceDate: sinceDate,
                ToDate: toDate,
                PageIndex: pageIndex,
                PageSize: pageSize,
                StyledNicks: useStyledNicks,
                FindLastRead: findLastRead);
        }

        /// <summary>
        /// Gets all topics where the page User id has posted
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param>
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should contain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the User Topics
        /// </returns>
        public static DataTable ByUserAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [CanBeNull] object categoryId,
            [NotNull] int pageUserId,
            [NotNull] DateTime sinceDate,
            [NotNull] DateTime toDate,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [NotNull] bool useStyledNicks,
            [CanBeNull] bool findLastRead = false)
        {
            return repository.DbFunction.GetData.topics_byuser(
                BoardID: boardId,
                CategoryID: categoryId,
                PageUserID: pageUserId,
                SinceDate: sinceDate,
                ToDate: toDate,
                PageIndex: pageIndex,
                PageSize: pageSize,
                StyledNicks: useStyledNicks,
                FindLastRead: findLastRead);
        }

        /// <summary>
        /// The topic_announcements.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The number of posts to retrieve.
        /// </param>
        /// <param name="pageUserId">
        /// The page User Id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable AnnouncementsAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [NotNull] int numOfPostsToRetrieve,
            [NotNull] int pageUserId)
        {
            return repository.DbFunction.GetData.topic_announcements(
                BoardID: boardId,
                NumPosts: numOfPostsToRetrieve,
                PageUserID: pageUserId);
        }

        /// <summary>
        /// The topic_create_by_message.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="newTopicSubject">
        /// The new topic subject.
        /// </param>
        /// <returns>
        /// Returns the new Topic ID
        /// </returns>
        public static long CreateByMessage(
            this IRepository<Topic> repository, [NotNull] int messageId, [NotNull] int forumId, [NotNull] string newTopicSubject)
        {
            return long.Parse(repository.DbFunction.GetData.topic_create_by_message(
                MessageID: messageId,
                ForumID: forumId,
                Subject: newTopicSubject,
                UTCTIMESTAMP: DateTime.UtcNow).GetFirstRow()["TopicID"].ToString());
        }

        /// <summary>
        /// Get the Latest Topics for the RSS Feed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The number of posts to retrieve.
        /// </param>
        /// <param name="pageUserId">
        /// The page UserId id.
        /// </param>
        /// <param name="useStyledNicks">
        /// If true returns string for userID style.
        /// </param>
        /// <param name="showNoCountPosts">
        /// The show No Count Posts.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable RssLatestAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [NotNull] int numOfPostsToRetrieve,
            [NotNull] int pageUserId,
            bool useStyledNicks,
            bool showNoCountPosts)
        {
            return repository.DbFunction.GetData.rss_topic_latest(
                BoardID: boardId,
                NumPosts: numOfPostsToRetrieve,
                PageUserID: pageUserId,
                StyledNicks: useStyledNicks,
                ShowNoCountPosts: showNoCountPosts);
        }

        /// <summary>
        /// Gets all Topics for an RSS Feed of specified forum id.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumId">The forum id.</param>
        /// <param name="topicLimit">The topic limit.</param>
        /// <returns>
        /// Returns a DataTable with the Topics of a Forum
        /// </returns>
        public static DataTable RssListAsDataTable(
            this IRepository<Topic> repository, int forumId, int topicLimit)
        {
            return repository.DbFunction.GetData.rsstopic_list(ForumID: forumId, TopicLimit: topicLimit);
        }

        /// <summary>
        /// Get the Latest Topics
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The number of posts to retrieve.
        /// </param>
        /// <param name="pageUserId">
        /// The page UserId id.
        /// </param>
        /// <param name="useStyledNicks">
        /// If true returns string for userID style.
        /// </param>
        /// <param name="showNoCountPosts">
        /// The show No Count Posts.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should contain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the DataTable with the Latest Topics
        /// </returns>
        public static DataTable LatestAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [NotNull] int numOfPostsToRetrieve,
            [NotNull] int pageUserId,
            bool useStyledNicks,
            bool showNoCountPosts,
            [CanBeNull] bool findLastRead)
        {
            return repository.DbFunction.GetData.topic_latest(
                BoardID: boardId,
                NumPosts: numOfPostsToRetrieve,
                PageUserID: pageUserId,
                StyledNicks: useStyledNicks,
                ShowNoCountPosts: showNoCountPosts,
                FindLastRead: findLastRead);
        }

        /// <summary>
        /// Get the Latest Topics for the specified category
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The number of posts to retrieve.
        /// </param>
        /// <param name="pageUserId">
        /// The page UserId id.
        /// </param>
        /// <param name="useStyledNicks">
        /// If true returns string for userID style.
        /// </param>
        /// <param name="showNoCountPosts">
        /// The show No Count Posts.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should contain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the DataTable with the Latest Topics
        /// </returns>
        public static DataTable LatestInCategoryAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [NotNull] int categoryId,
            [NotNull] int numOfPostsToRetrieve,
            [NotNull] int pageUserId,
            bool useStyledNicks,
            bool showNoCountPosts,
            [CanBeNull] bool findLastRead)
        {
            return repository.DbFunction.GetData.topic_latest_in_category(
                BoardID: boardId,
                categoryId: categoryId,
                NumPosts: numOfPostsToRetrieve,
                PageUserID: pageUserId,
                StyledNicks: useStyledNicks,
                ShowNoCountPosts: showNoCountPosts,
                FindLastRead: findLastRead);
        }

        /// <summary>
        /// The topic_list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param>
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// To return style for user nicks in topic_list.
        /// </param>
        /// <param name="showMoved">
        /// The show Moved.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should contain the last Access Date
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable ListAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int forumId,
            [CanBeNull] int? userId,
            [CanBeNull] DateTime? sinceDate,
            [CanBeNull] DateTime? toDate,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [NotNull] bool useStyledNicks,
            [NotNull] bool showMoved,
            [CanBeNull] bool findLastRead)
        {
            return repository.DbFunction.GetData.topic_list(
                ForumID: forumId,
                UserID: userId,
                Date: sinceDate,
                ToDate: toDate,
                PageIndex: pageIndex,
                PageSize: pageSize,
                StyledNicks: useStyledNicks,
                ShowMoved: showMoved,
                FindLastRead: findLastRead);
        }

        /// <summary>
        /// The topic_list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param>
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// To return style for user nicks in topic_list.
        /// </param>
        /// <param name="showMoved">
        /// The show Moved.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Contain the last Access Date
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable AnnouncementsAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int forumId,
            [CanBeNull] int? userId,
            [CanBeNull] DateTime? sinceDate,
            [CanBeNull] DateTime? toDate,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [NotNull] bool useStyledNicks,
            [NotNull] bool showMoved,
            [CanBeNull] bool findLastRead)
        {
            return repository.DbFunction.GetData.announcements_list(
                ForumID: forumId,
                UserID: userId,
                Date: sinceDate,
                ToDate: toDate,
                PageIndex: pageIndex,
                PageSize: pageSize,
                StyledNicks: useStyledNicks,
                ShowMoved: showMoved,
                FindLastRead: findLastRead);
        }

        /// <summary>
        /// The topic_save.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="styles">
        /// The styles.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="ip">
        /// The IP Address.
        /// </param>
        /// <param name="posted">
        /// The posted.
        /// </param>
        /// <param name="blogPostId">
        /// The blog Post Id.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="topicTags">
        /// The topic Tags.
        /// </param>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <returns>
        /// Returns the Topic ID
        /// </returns>
        public static long Save(
            this IRepository<Topic> repository,
            [NotNull] int forumId,
            [NotNull] string subject,
            [CanBeNull] string status,
            [CanBeNull] string styles,
            [CanBeNull] string description,
            [NotNull] string message,
            [NotNull] int userId,
            [NotNull] int priority,
            [NotNull] string userName,
            [NotNull] string ip,
            [NotNull] DateTime posted,
            [NotNull] string blogPostId,
            [NotNull] int flags,
            [CanBeNull] string topicTags,
            ref long messageId)
        {
            var dt = repository.DbFunction.GetData.topic_save(
                ForumID: forumId,
                Subject: subject,
                Description: description,
                Status: status,
                Styles: styles,
                UserID: userId,
                Message: message,
                Priority: priority,
                UserName: userName,
                IP: ip,
                Posted: posted,
                BlogPostID: blogPostId,
                Flags: flags,
                UTCTIMESTAMP: DateTime.UtcNow);

            messageId = long.Parse(dt.Rows[0]["MessageID"].ToString());

            long topicId = long.Parse(dt.Rows[0]["TopicID"].ToString());

            // Add to search index
            var newMessage = new SearchMessage
                                 {
                                     MessageId = messageId.ToType<int>(),
                                     Message = message,
                                     Flags = flags,
                                     Posted = posted.ToString(CultureInfo.InvariantCulture),
                                     UserName = BoardContext.Current.User.UserName,
                                     UserDisplayName = BoardContext.Current.CurrentUserData.DisplayName,
                                     UserStyle = BoardContext.Current.UserStyle,
                                     UserId = BoardContext.Current.PageUserID,
                                     TopicId = topicId.ToType<int>(),
                                     Topic = subject,
                                     TopicTags = topicTags,
                                     ForumId = BoardContext.Current.PageForumID,
                                     ForumName = BoardContext.Current.PageForumName,
                                     Description = string.Empty
                                 };

            BoardContext.Current.Get<ISearch>().AddSearchIndexItem(newMessage);

            return topicId;
        }

        /// <summary>
        /// The topic_move.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="showMoved">
        /// The show moved.
        /// </param>
        /// <param name="linkDays">
        /// The link Days.
        /// </param>
        public static void MoveTopic(this IRepository<Topic> repository, [NotNull] int topicId, [NotNull] int forumId, [NotNull] bool showMoved, [NotNull] int linkDays)
        {
            repository.DbFunction.Scalar.topic_move(
                TopicID: topicId,
                ForumID: forumId,
                ShowMoved: showMoved,
                LinkDays: linkDays,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// The prune.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="days">
        /// The days.
        /// </param>
        /// <param name="permDelete">
        /// The perm delete.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Prune(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [NotNull] int forumId,
            [NotNull] int days,
            [NotNull] bool permDelete)
        {
            return (int)repository.DbFunction.Scalar.topic_prune(
                BoardID: boardId,
                ForumID: forumId,
                Days: days,
                PermDelete: permDelete,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// The topic_delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        public static void Delete(this IRepository<Topic> repository, [NotNull] int topicID)
        {
            repository.Delete(topicID, false);
        }

        /// <summary>
        /// The topic_delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <param name="eraseTopic">
        /// The erase topic.
        /// </param>
        public static void Delete(this IRepository<Topic> repository, [NotNull] int topicId, [NotNull] bool eraseTopic)
        {
            repository.DeleteAttachments(topicId);

            BoardContext.Current.Get<ISearch>().DeleteSearchIndexRecordByTopicId(topicId);

            BoardContext.Current.Get<ILogger>().Log(
                BoardContext.Current.PageUserName,
                "YAF",
                BoardContext.Current.Get<ILocalization>().GetTextFormatted("DELETED_TOPIC", topicId),
                EventLogTypes.Information);

            repository.DbFunction.Scalar.topic_delete(TopicID: topicId, EraseTopic: eraseTopic);
        }
        
        /// <summary>
        /// The check for duplicate topic.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicSubject">
        /// The topic subject.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool CheckForDuplicateTopic(this IRepository<Topic> repository, [NotNull] string topicSubject)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var topic = repository.GetSingle(t => t.TopicName.Contains(topicSubject) && t.TopicMovedID == null);

            return topic != null;
        }

        /// <summary>
        /// The find next topic.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="currentTopic">
        /// The current topic.
        /// </param>
        /// <returns>
        /// The <see cref="Topic"/>.
        /// </returns>
        public static Topic FindNextTopic(this IRepository<Topic> repository, [NotNull] Topic currentTopic)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Get(
                    t => t.LastPosted > currentTopic.LastPosted && t.ForumID == currentTopic.ForumID
                                                                && !t.TopicFlags.IsDeleted && t.TopicMovedID == null)
                .OrderBy(t => t.LastPosted).FirstOrDefault();
        }

        /// <summary>
        /// The find previous topic.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="currentTopic">
        /// The current topic.
        /// </param>
        /// <returns>
        /// The <see cref="Topic"/>.
        /// </returns>
        public static Topic FindPreviousTopic(this IRepository<Topic> repository, [NotNull] Topic currentTopic)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Get(
                    t => t.LastPosted < currentTopic.LastPosted && t.ForumID == currentTopic.ForumID
                                                                && !t.TopicFlags.IsDeleted && t.TopicMovedID == null)
                .OrderByDescending(t => t.LastPosted).FirstOrDefault();
        }

        /// <summary>
        /// The simple list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="startId">
        /// The start id.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Topic> SimpleList(this IRepository<Topic> repository, [CanBeNull] int startId = 0, [CanBeNull] int limit = 500)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Get(t => t.ID >= limit && t.ID < startId + limit).OrderBy(t => t.ID).ToList();
        }

        /// <summary>
        /// The simple list as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="startId">
        /// The start id.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable SimpleListAsDataTable(this IRepository<Topic> repository, [CanBeNull] int startId = 0, [CanBeNull] int limit = 500)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.topic_simplelist(StartID: startId, Limit: limit);
        }

        /// <summary>
        /// Un-encode All Topics and Subjects
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="decodeTopicFunc">
        /// The decode topic function
        /// </param>
        public static void UnEncodeAllTopicsSubjects(this IRepository<Topic> repository, [NotNull] Func<string, string> decodeTopicFunc)
        {
            var topics = repository.SimpleList(0, 99999999);

            topics.Where(t => t.TopicName.IsSet()).ForEach(
                topic =>
                    {
                        try
                        {
                            var decodedTopic = decodeTopicFunc(topic.TopicName);

                            if (!decodedTopic.Equals(topic.TopicName))
                            {
                                // un-encode it and update.
                                repository.UpdateOnly(
                                    () => new Topic { TopicName = decodedTopic },
                                    t => t.ID == topic.ID);
                            }
                        }
                        catch
                        {
                            // soft-fail...
                        }
                    });
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
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Tuple<Forum, Topic>> GetDeletedTopics(this IRepository<Topic> repository, [NotNull] int boardId, [CanBeNull] string filter)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

            if (filter.IsSet())
            {
                expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                    .Join<Topic>((f, t) => t.ForumID == f.ID).Where<Topic, Category>(
                        (t, category) => category.BoardID == boardId && t.IsDeleted == true && t.TopicName.Contains(filter)).Select();
            }
            else
            {
                expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                    .Join<Topic>((f, t) => t.ForumID == f.ID).Where<Topic, Category>(
                        (t, category) => category.BoardID == boardId && t.IsDeleted == true).Select();
            }
            
            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Forum, Topic>(expression));
        }

        #endregion

        /// <summary>
        /// The topic_delete attachments.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        private static void DeleteAttachments(this IRepository<Topic> repository, [NotNull] object topicID)
        {
            var topics = repository.DbFunction.GetData.topic_listmessages(TopicID: topicID);

            foreach (DataRow row in topics.Rows)
            {
                repository.DeleteRecursively(
                    row["MessageID"].ToType<int>(),
                    true,
                    string.Empty,
                    0,
                    true,
                    false);
            }
        }

        /// <summary>
        /// The message_delete recursively.
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
            this IRepository<Topic> repository,
            [NotNull] int messageID,
            bool isModeratorChanged,
            [NotNull] string deleteReason,
            int isDeleteAction,
            bool deleteLinked,
            bool eraseMessages)
        {
            var useFileTable = BoardContext.Current.Get<BoardSettings>().UseFileTable;

            if (deleteLinked)
            {
                // Delete replies
                var replies = BoardContext.Current.GetRepository<Message>().Get(m => m.ReplyTo == messageID).Select(x => x.ID);

                replies.ForEach(
                    replyId => repository.DeleteRecursively(
                        replyId,
                        isModeratorChanged,
                        deleteReason,
                        isDeleteAction,
                        true,
                        eraseMessages));
            }

            // If the files are actually saved in the Hard Drive
            if (!useFileTable)
            {
                BoardContext.Current.GetRepository<Attachment>().DeleteByMessageId(messageID);
            }

            // Ederon : erase message for good
            if (eraseMessages)
            {
                // Delete Message from Search Index
                BoardContext.Current.Get<ISearch>().DeleteSearchIndexRecordByMessageId(messageID);

                repository.DbFunction.Scalar.message_delete(
                    MessageID: messageID,
                    EraseMessage: true);
            }
            else
            {
                // Delete Message
                // un-delete function added
                repository.DbFunction.Scalar.message_deleteundelete(
                    MessageID: messageID,
                    isModeratorChanged: isModeratorChanged,
                    DeleteReason: deleteReason,
                    isDeleteAction: isDeleteAction);
            }
        }
    }
}