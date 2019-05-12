/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;

    using YAF.Classes;
    using YAF.Core.Extensions;
    using YAF.Types;
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
        /// Gets the similar topics.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="searchInput">The search input.</param>
        /// <returns>Get List of similar topics</returns>
        public static List<SearchMessage> GetSimilarTopics(this IRepository<Topic> repository, [NotNull] int userId, [NotNull] string searchInput)
        {
            return YafContext.Current.Get<ISearch>().SearchSimilar(userId, searchInput, "Topic");
        }
        

        /// <summary>
        /// Sets the answer message.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void SetAnswerMessage(this IRepository<Topic> repository, [NotNull] int topicId, [NotNull] int messageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Topic { AnswerMessageId = messageId }, where: t => t.ID == topicId);
        }

        /// <summary>
        /// Removes answer message.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        public static void RemoveAnswerMessage(this IRepository<Topic> repository, [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Topic { AnswerMessageId = null }, where: t => t.ID == topicId);
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

            var topic = repository.GetSingle(t => t.ID == topicId);

            return topic?.AnswerMessageId;
        }

        /// <summary>
        /// Lock's/Unlock's the topic.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="flags">The topic flags.</param>
        public static void LockTopic(this IRepository<Topic> repository, [NotNull] int topicId, [NotNull] int flags)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Topic { Flags = flags }, where: t => t.ID == topicId);
        }

        /// The topic_unanswered
        /// </summary>
        /// <param name="boardID">
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
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Topics Unanswered
        /// </returns>
        public static DataTable UnansweredAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [CanBeNull] int? categoryId,
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
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Topics Unread be a PageUserId
        /// </returns>
        public static DataTable ActiveAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [CanBeNull] int? categoryId,
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
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Topics Unread be a PageUserId
        /// </returns>
        public static DataTable UnreadAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [CanBeNull] int? categoryId,
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
        /// Gets all topics where the pageUserid has posted
        /// </summary>
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
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the User Topics
        /// </returns>
        public static DataTable ByUserAsDataTable(
            this IRepository<Topic> repository,
            [NotNull] int boardId,
            [CanBeNull] int? categoryId,
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
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The num of posts to retrieve.
        /// </param>
        /// <param name="pageUserId">
        /// The page User Id.
        /// </param>
        /// <returns>
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
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="newTopicSubj">
        /// The new topic subj.
        /// </param>
        /// <returns>
        /// The topic_create_by_message.
        /// </returns>
        public static long CreateByMessage(
            this IRepository<Topic> repository, [NotNull] int messageId, [NotNull] int forumId, [NotNull] string newTopicSubj)
        {
            return repository.DbFunction.GetData.topic_create_by_message(
                MessageID: messageId,
                ForumID: forumId,
                Subject: newTopicSubj,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// The rss_topic_latest.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The num of posts to retrieve.
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
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The num of posts to retrieve.
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
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returnst the DataTable with the Latest Topics
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
        /// <param name="boardID">The board id.</param>
        /// <param name="categoryID">The category identifier.</param>
        /// <param name="numOfPostsToRetrieve">The num of posts to retrieve.</param>
        /// <param name="pageUserId">The page UserId id.</param>
        /// <param name="useStyledNicks">If true returns string for userID style.</param>
        /// <param name="showNoCountPosts">The show No Count Posts.</param>
        /// <param name="findLastRead">Indicates if the Table should Countain the last Access Date</param>
        /// <returns>
        /// Returnst the DataTable with the Latest Topics
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
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="announcement">
        /// The announcement.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="useStyledNicks">
        /// To return style for user nicks in topic_list.
        /// </param>
        /// <param name="showMoved">
        /// The show Moved.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
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
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="announcement">
        /// The announcement.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="useStyledNicks">
        /// To return style for user nicks in topic_list.
        /// </param>
        /// <param name="showMoved">
        /// The show Moved.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable AnnouncementsAsDataTable(this IRepository<Topic> repository,
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
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <param name="posted">
        /// The posted.
        /// </param>
        /// <param name="blogPostID">
        /// The blog post id.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="messageID">
        /// The message id.
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
            return long.Parse(dt.Rows[0]["TopicID"].ToString());
        }

        /// <summary>
        /// The topic_move.
        /// </summary>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="showMoved">
        /// The show moved.
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
        /// The topic_prune.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="days">
        /// The days.
        /// </param>
        /// <param name="permDelete">
        /// The perm delete.
        /// </param>
        /// <returns>
        /// The topic_prune.
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
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="eraseTopic">
        /// The erase topic.
        /// </param>
        public static void Delete(this IRepository<Topic> repository, [NotNull] int topicID, [NotNull] bool eraseTopic)
        {
            repository.DeleteAttachments(topicID);

            repository.DbFunction.Scalar.topic_delete(TopicID: topicID, EraseTopic: eraseTopic);
        }

        public static bool CheckForDuplicateTopic(this IRepository<Topic> repository, [NotNull] string topicSubject)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var topic = repository.GetSingle(t => t.TopicName.Contains(topicSubject) && t.TopicMovedID== null);

            return topic != null;
        }

        public static Topic FindNextTopic(this IRepository<Topic> repository, [NotNull] Topic currentTopic)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Get(
                    t => t.LastPosted > currentTopic.LastPosted && t.ForumID == currentTopic.ForumID
                                                                && !t.TopicFlags.IsDeleted && t.TopicMovedID == null)
                .OrderBy(t => t.LastPosted).FirstOrDefault();
        }

        public static Topic FindPreviousTopic(this IRepository<Topic> repository, [NotNull] Topic currentTopic)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Get(
                    t => t.LastPosted < currentTopic.LastPosted && t.ForumID == currentTopic.ForumID
                                                                && !t.TopicFlags.IsDeleted && t.TopicMovedID == null)
                .OrderByDescending(t => t.LastPosted).FirstOrDefault();
        }

        public static List<Topic> SimpleList(this IRepository<Topic> repository, [CanBeNull] int StartId = 0, [CanBeNull] int Limit = 500)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Get(t => t.ID >= Limit && t.ID < StartId + Limit).OrderBy(t => t.ID).ToList();
        }

        public static DataTable SimpleListAsDataTable(this IRepository<Topic> repository, [CanBeNull] int startId = 0, [CanBeNull] int limit = 500)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.topic_simplelist(StartID: startId, Limit: limit);
        }

        /// <summary>
        /// The unencode_all_topics_subjects.
        /// </summary>
        /// <param name="decodeTopicFunc">
        /// The decode topic func.
        /// </param>
        public static void UnencodeAllTopicsSubjects(this IRepository<Topic> repository, [NotNull] Func<string, string> decodeTopicFunc)
        {
            var topics = repository.SimpleList(0, 99999999);

            foreach (var topic in topics.Where(t => t.TopicName.IsSet()))
            {
                try
                {
                    var decodedTopic = decodeTopicFunc(topic.TopicName);

                    if (!decodedTopic.Equals(topic.TopicName))
                    {
                        // unencode it and update.
                        repository.UpdateOnly(() => new Topic { TopicName = decodedTopic }, t => t.ID == topic.ID);
                    }
                }
                catch
                {
                    // soft-fail...
                }
            }
        }

        #endregion

        /// <summary>
        /// The topic_delete attachments.
        /// </summary>
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
        /// Delete message and all subsequent releated messages to that ID
        /// </summary>
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
        /// <param name="DeleteLinked">
        /// The delete linked.
        /// </param>
        /// <param name="isLinked">
        /// The is linked.
        /// </param>
        private static void DeleteRecursively(
            this IRepository<Topic> repository,
            [NotNull] int messageID,
            bool isModeratorChanged,
            [NotNull] string deleteReason,
            int isDeleteAction,
            bool DeleteLinked,
            bool isLinked)
        {
            repository.DeleteRecursively(
                messageID,
                isModeratorChanged,
                deleteReason,
                isDeleteAction,
                DeleteLinked,
                isLinked,
                false);
        }

        /// <summary>
        /// The message_delete recursively.
        /// </summary>
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
        /// <param name="isLinked">
        /// The is linked.
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
            bool isLinked,
            bool eraseMessages)
        {
            var useFileTable = YafContext.Current.Get<YafBoardSettings>().UseFileTable;

            if (deleteLinked)
            { 
                // Delete replies
                var replies = YafContext.Current.GetRepository<Message>().Get(m => m.ReplyTo == messageID).Select(x => x.ID);

                foreach (var replyId in replies)
                {
                    repository.DeleteRecursively(
                        replyId,
                        isModeratorChanged,
                        deleteReason,
                        isDeleteAction,
                        true,
                        true,
                        eraseMessages);
                }
            }

            // If the files are actually saved in the Hard Drive
            if (!useFileTable)
            {
                var attachments = repository.DbFunction.GetData.attachment_list(MessageID: messageID);

                var uploadDir =
                    HostingEnvironment.MapPath(
                        string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

                foreach (DataRow row in attachments.Rows)
                {
                    try
                    {
                        var fileName = "{0}/{1}.{2}.yafupload".FormatWith(uploadDir, messageID, row["FileName"]);

                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                    }
                    catch
                    {
                        // error deleting that file...
                    }
                }
            }

            // Ederon : erase message for good
            if (eraseMessages)
            {
                repository.DbFunction.Scalar.message_delete(
                    MessageID: messageID,
                    EraseMessage: eraseMessages);
            }
            else
            {
                // Delete Message
                // undelete function added
                repository.DbFunction.Scalar.message_deleteundelete(
                    MessageID: messageID,
                    isModeratorChanged: isModeratorChanged,
                    DeleteReason: deleteReason,
                    isDeleteAction: isDeleteAction);
            }
        }
    }
}