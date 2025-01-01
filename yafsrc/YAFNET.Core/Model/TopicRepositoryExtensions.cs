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
using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using YAF.Types.Constants;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
///     The Topic repository extensions.
/// </summary>
public static class TopicRepositoryExtensions
{
    /// <summary>
    /// Get the Topic From Message.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// The <see cref="Topic"/>.
    /// </returns>
    public static Topic GetTopicFromMessage(this IRepository<Topic> repository, int messageId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        expression.Join<Topic>((m, t) => t.ID == m.TopicID).Where<Message>(m => m.ID == messageId).Take(1);

        return repository.DbAccess.Execute(db => db.Connection.Single<Topic>(expression));
    }

    /// <summary>
    /// Attach a Poll to a Topic
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="pollId">
    /// The poll id.
    /// </param>
    public static void AttachPoll(this IRepository<Topic> repository, int topicId, int pollId)
    {
        repository.UpdateOnly(() => new Topic { PollID = pollId }, t => t.ID == topicId);
    }

    /// <summary>
    /// Sets the answer message.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="topicId">The topic identifier.</param>
    /// <param name="messageId">The message identifier.</param>
    public static void SetAnswerMessage(
        this IRepository<Topic> repository,
        int topicId,
        int messageId)
    {
        repository.UpdateOnly(() => new Topic { AnswerMessageId = messageId }, t => t.ID == topicId);
    }

    /// <summary>
    /// Removes answer message.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="topicId">The topic identifier.</param>
    public static void RemoveAnswerMessage(this IRepository<Topic> repository, int topicId)
    {
        repository.UpdateOnly(() => new Topic { AnswerMessageId = null }, t => t.ID == topicId);
    }

    /// <summary>
    /// Gets the answer message.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="topicId">The topic identifier.</param>
    /// <returns>Returns the Answer Message identifier</returns>
    public static int? GetAnswerMessage(this IRepository<Topic> repository, int topicId)
    {
        var topic = repository.GetById(topicId);

        return topic?.AnswerMessageId;
    }

    /// <summary>
    /// Locks/Unlock the topic.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="topicId">The topic identifier.</param>
    /// <param name="flags">The topic flags.</param>
    public static void Lock(this IRepository<Topic> repository, int topicId, int flags)
    {
        repository.UpdateOnly(() => new Topic { Flags = flags }, t => t.ID == topicId);
    }

    /// <summary>
    /// Returns topics which have no replies.
    /// </summary>
    /// <param name="repository">
    /// The repository.
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
    /// <param name="findLastRead">
    /// Indicates if the Table should contain the last Access Date
    /// </param>
    /// <returns>
    /// Returns the List with the Topics
    /// </returns>
    public static List<PagedTopic> ListUnansweredPaged(
        this IRepository<Topic> repository,
        int pageUserId,
        DateTime sinceDate,
        DateTime toDate,
        int pageIndex,
        int pageSize,
        bool findLastRead)
    {
        return repository.ListPaged(
            pageUserId,
            pageIndex,
            pageSize,
            findLastRead,
            (t, x, c) => x.UserID == pageUserId && x.ReadAccess && (t.Flags & 8) != 8 && t.TopicMovedID == null &&
                         t.LastPosted != null && t.LastPosted > sinceDate && t.LastPosted < toDate &&
                         t.NumPosts == 1);
    }

    /// <summary>
    /// Lists the Active Topics
    /// </summary>
    /// <param name="repository">
    /// The repository.
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
    /// <param name="findLastRead">
    /// Indicates if the Table should contain the last Access Date
    /// </param>
    /// <returns>
    /// Returns the Active Topics
    /// </returns>
    public static List<PagedTopic> ListActivePaged(
        this IRepository<Topic> repository,
        int pageUserId,
        DateTime sinceDate,
        DateTime toDate,
        int pageIndex,
        int pageSize,
        bool findLastRead)
    {
        return repository.ListPaged(
            pageUserId,
            pageIndex,
            pageSize,
            findLastRead,
            (t, x, c) => x.UserID == pageUserId && x.ReadAccess &&
                         (t.Flags & 8) != 8 && t.TopicMovedID == null && t.LastPosted != null && t.LastPosted > sinceDate && t.LastPosted < toDate);
    }

    /// <summary>
    /// List all Topics Watched by a user
    /// </summary>
    /// <param name="repository">
    /// The repository.
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
    /// <param name="findLastRead">
    /// Indicates if the Table should contain the last Access Date
    /// </param>
    /// <returns>
    /// Returns the List with the Topics Watched by a user
    /// </returns>
    public static List<PagedTopic> ListWatchedPaged(
        this IRepository<Topic> repository,
        int userId,
        DateTime sinceDate,
        DateTime toDate,
        int pageIndex,
        int pageSize,
        bool findLastRead = false)
    {
        Expression<Func<Topic, bool>> whereCriteria = t =>
            (t.Flags & 8) != 8 && t.TopicMovedID == null && t.LastPosted != null && t.LastPosted > sinceDate
            && t.LastPosted < toDate;

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<WatchTopic>();

        return repository.DbAccess.Execute(
            db =>
                {
                    expression.Join<User>((t, u) => u.ID == t.UserID)
                        .Join<Topic>((a, b) => b.ID == a.TopicID).Join<Topic, Forum>((t, f) => f.ID == t.ForumID)
                        .Where<WatchTopic>(b => b.UserID == userId);

                    expression.Where(whereCriteria);

                    // -- count total
                    var countTotalExpression = db.Connection.From<Topic>().Join<Forum>((t, f) => f.ID == t.ForumID)
                        .Join<Forum, ActiveAccess>((f, x) => x.ForumID == f.ID)
                        .Join<Forum, Category>((f, c) => c.ID == f.CategoryID);

                    countTotalExpression.Where(whereCriteria);

                    expression.OrderByDescending<Topic>(t => t.LastPosted).Page(pageIndex + 1, pageSize);

                    var countTotalSql = countTotalExpression
                        .Select(Sql.Count($"{countTotalExpression.Column<Topic>(x => x.ID)}")).ToSelectStatement();

                    // -- count deleted posts
                    var countDeletedExpression = db.Connection.From<Message>(db.Connection.TableAlias("mes"));
                    countDeletedExpression.Where(
                        $"""
                         mes.{countDeletedExpression.Column<Message>(x => x.TopicID)}={expression.Column<Topic>(x => x.ID, true)}
                                                             and (mes.{countDeletedExpression.Column<Message>(x => x.Flags)} & 8) = 8
                                                             and mes.{countDeletedExpression.Column<Message>(x => x.UserID)}={userId}
                         """);
                    var countDeletedSql = countDeletedExpression.Select(Sql.Count("1")).ToSelectStatement();

                    var lastTopicAccessSql = "NULL";
                    var lastForumAccessSql = "NULL";

                    if (findLastRead)
                    {
                        var topicAccessExpression =
                            db.Connection.From<TopicReadTracking>(db.Connection.TableAlias("y"));
                        topicAccessExpression.Where(
                            $"""
                             y.{topicAccessExpression.Column<TopicReadTracking>(y => y.TopicID)}={expression.Column<Topic>(x => x.ID, true)}
                                                                 and y.{topicAccessExpression.Column<TopicReadTracking>(y => y.UserID)}={userId}
                             """);
                        lastTopicAccessSql = topicAccessExpression.Select(
                                $"{topicAccessExpression.Column<TopicReadTracking>(x => x.LastAccessDate)}").Limit(1)
                            .ToSelectStatement();

                        var forumAccessExpression =
                            db.Connection.From<ForumReadTracking>(db.Connection.TableAlias("x"));
                        forumAccessExpression.Where(
                            $"""
                             x.{forumAccessExpression.Column<ForumReadTracking>(x => x.ForumID)}={expression.Column<Topic>(x => x.ForumID, true)}
                                                                 and x.{forumAccessExpression.Column<ForumReadTracking>(x => x.UserID)}={userId}
                             """);
                        lastForumAccessSql = forumAccessExpression.Select(
                                $"{forumAccessExpression.Column<ForumReadTracking>(x => x.LastAccessDate)}").Limit(1)
                            .ToSelectStatement();
                    }

                    // -- last user
                    var lastUserNameExpression = db.Connection.From<User>(db.Connection.TableAlias("usr"));
                    lastUserNameExpression.Where(
                        $"""
                         usr.{lastUserNameExpression.Column<User>(x => x.ID)}=
                                                            {expression.Column<Topic>(x => x.LastUserID, true)}
                         """);
                    var lastUserNameSql = lastUserNameExpression.Select(
                        $"{lastUserNameExpression.Column<User>(x => x.Name)}").Limit(1).ToSelectStatement();

                    var lastUserDisplayNameSql = lastUserNameExpression.Select(
                        $"{lastUserNameExpression.Column<User>(x => x.DisplayName)}").Limit(1).ToSelectStatement();

                    var lastUserSuspendedSql = lastUserNameExpression.Select(
                        $"{lastUserNameExpression.Column<User>(x => x.Suspended)}").Limit(1).ToSelectStatement();

                    var lastUserStyleSql = lastUserNameExpression.Select(
                        $"{lastUserNameExpression.Column<User>(x => x.UserStyle)}").Limit(1).ToSelectStatement();

                    // -- last message
                    var lastMessageExpression = db.Connection.From<Message>(db.Connection.TableAlias("fm"));
                    lastMessageExpression.Where(
                        $"""
                         fm.{lastMessageExpression.Column<Message>(x => x.ID)}=
                         {OrmLiteConfig.DialectProvider.IsNullFunction(expression.Column<Topic>(x => x.TopicMovedID, true), expression.Column<Topic>(x => x.LastMessageID, true))}
                         """);

                    var lastMessageSql = lastMessageExpression.Select(
                        $"{lastMessageExpression.Column<Message>(x => x.MessageText)}").Limit(1).ToSelectStatement();

                    expression.Select<Topic, User, Forum>(
                        (c, b, d) => new
                                         {
                                             ForumID = d.ID,
                                             ForumName = d.Name,
                                             TopicID = c.ID,
                                             c.Posted,
                                             LinkTopicID = c.TopicMovedID ?? c.ID,
                                             c.TopicMovedID,
                                             Subject = c.TopicName,
                                             c.Description,
                                             c.Status,
                                             c.Styles,
                                             c.UserID,
                                             Starter = c.UserName ?? b.Name,
                                             StarterDisplay =
                                                 c.UserDisplayName ?? b.DisplayName,
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
                                             LastMessage = Sql.Custom($"({lastMessageSql})"),
                                             StarterStyle = b.UserStyle,
                                             StarterSuspended = b.Suspended,
                                             LastForumAccess = Sql.Custom($"({lastForumAccessSql})"),
                                             LastTopicAccess = Sql.Custom($"({lastTopicAccessSql})"),
                                             c.TopicImage,
                                             TotalRows = Sql.Custom($"({countTotalSql})")
                                         });

                    return db.Connection.Select<PagedTopic>(expression);
                });
    }

    /// <summary>
    /// Gets all topics where the page User id has posted
    /// </summary>
    /// <param name="repository">
    /// The repository.
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
    /// <param name="findLastRead">
    /// Indicates if the Table should contain the last Access Date
    /// </param>
    /// <returns>
    /// Returns the List with the User Topics
    /// </returns>
    public static List<PagedTopic> ListByUserPaged(
        this IRepository<Topic> repository,
        int pageUserId,
        DateTime sinceDate,
        DateTime toDate,
        int pageIndex,
        int pageSize,
        bool findLastRead = false)
    {
        return repository.ListPaged(
            pageUserId,
            pageIndex,
            pageSize,
            findLastRead,
            (t, x, c) => x.UserID == pageUserId && x.ReadAccess && (t.Flags & 8) != 8 && t.TopicMovedID == null &&
                         t.LastPosted != null && t.LastPosted > sinceDate && t.LastPosted < toDate &&
                         t.UserID == pageUserId);
    }

    /// <summary>
    /// Create New Topic By Message.
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
        this IRepository<Topic> repository,
        int messageId,
        int forumId,
        string newTopicSubject)
    {
        var message = BoardContext.Current.GetRepository<Message>().GetById(messageId);

        var topic = new Topic
                        {
                            ForumID = forumId,
                            TopicName = newTopicSubject,
                            UserID = message.UserID,
                            Posted = message.Posted,
                            Views = 0,
                            Priority = 0,
                            PollID = null,
                            UserName = null,
                            NumPosts = 0
                        };

        return repository.Insert(topic);
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
    /// <returns>
    /// Returns the Latest Topics for the RSS Feed.
    /// </returns>
    public static List<Tuple<Message, Topic, User>> RssLatest(
        this IRepository<Topic> repository,
        int boardId,
        int numOfPostsToRetrieve,
        int pageUserId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        expression.Join<Topic>((m, t) => m.ID == t.LastMessageID).Join<Topic, User>((t, u) => u.ID == t.LastUserID)
            .Join<Topic, Forum>((c, d) => d.ID == c.ForumID).Join<Forum, Category>((d, e) => e.ID == d.CategoryID)
            .Join<Forum, ActiveAccess>((d, x) => x.ForumID == d.ID)
            .Where<Topic, Message, ActiveAccess, Category>(
                (topic, message, x, e) => e.BoardID == boardId && (e.Flags & 1) == 1 && topic.TopicMovedID == null &&
                                          x.UserID == pageUserId && x.ReadAccess && (topic.Flags & 8) != 8 &&
                                          (message.Flags & 8) != 8 && topic.LastPosted != null)
            .OrderByDescending<Message>(x => x.Posted).Take(numOfPostsToRetrieve);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Message, Topic, User>(expression));
    }

    /// <summary>
    /// Gets all Topics for an RSS Feed of specified forum id.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="pageUserId">
    /// The page User Id.
    /// </param>
    /// <param name="topicLimit">
    /// The topic limit.
    /// </param>
    /// <returns>
    /// Returns all Topics for an RSS Feed of specified forum id.
    /// </returns>
    public static List<LatestTopic> RssList(
        this IRepository<Topic> repository,
        int forumId,
        int pageUserId,
        int topicLimit)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

        return repository.DbAccess.Execute(
            db =>
                {
                    expression.Join<Forum>((t, f) => f.ID == t.ForumID).Join<Message>((t, m) => m.ID == t.LastMessageID)
                        .Join<Forum, Category>((f, c) => c.ID == f.CategoryID)
                        .Join<Forum, ActiveAccess>((f, x) => x.ForumID == f.ID);

                    expression.Where<Topic, Forum, ActiveAccess, Category>(
                        (topic, f, x, c) => c.BoardID == repository.BoardID && (c.Flags & 1) == 1 && topic.TopicMovedID == null &&
                                            x.UserID == pageUserId && x.ReadAccess && (topic.Flags & 8) != 8 &&
                                            topic.LastPosted != null && topic.NumPosts > 0 && f.ID == forumId);

                    expression.OrderByDescending<Topic>(t => t.LastPosted);

                    expression.Take(topicLimit).Select<Topic, Message, Forum>(
                        (t, m, f) => new
                                         {
                                             Topic = t.TopicName,
                                             TopicID = t.ID,
                                             Forum = f.Name,
                                             LastPosted = t.LastPosted ?? t.Posted,
                                             LastUserID = t.LastUserID ?? t.UserID,
                                             t.LastMessageID,
                                             LastMessage = m.MessageText,
                                             LastMessageFlags = m.Flags
                                         });

                    return db.Connection.Select<LatestTopic>(expression);
                });
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
    /// <param name="categoryId">
    /// The category Id.
    /// </param>
    /// <param name="numOfPostsToRetrieve">
    /// The number of posts to retrieve.
    /// </param>
    /// <param name="pageUserId">
    /// The page UserId id.
    /// </param>
    /// <param name="showNoCountPosts">
    /// The show No Count Posts.
    /// </param>
    /// <param name="findLastRead">
    /// Indicates if the Table should contain the last Access Date
    /// </param>
    /// <param name="sortOrder">
    /// The sort Order 0 == LastPosted, 1 == Views, 2 == Number of Posts.
    /// </param>
    /// <returns>
    /// The List of Latest Topics
    /// </returns>
    public static List<LatestTopic> Latest(
        this IRepository<Topic> repository,
        int boardId,
        int categoryId,
        int numOfPostsToRetrieve,
        int pageUserId,
        bool showNoCountPosts,
        bool findLastRead,
        int sortOrder = 0)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

        return repository.DbAccess.Execute(
            db =>
                {
                    expression.Join<Message>((t, m) => m.ID == t.LastMessageID).Join<Forum>((t, f) => f.ID == t.ForumID)
                        .Join<User>((t, u) => u.ID == t.UserID)
                        .Join<User>(
                            (t, lastUser) => Sql.TableAlias(lastUser.ID, "lastUser") == t.LastUserID,
                            db.Connection.TableAlias("lastUser")).Join<Forum, Category>((f, c) => c.ID == f.CategoryID)
                        .Join<Forum, VAccess>((f, x) => x.ForumID == f.ID);

                    if (showNoCountPosts)
                    {
                        expression.Where<Topic, Forum, VAccess, Category>(
                            (topic, f, x, c) => c.BoardID == boardId && (c.Flags & 1) == 1 && topic.TopicMovedID == null &&
                                                x.UserID == pageUserId && x.ReadAccess > 0 && (topic.Flags & 8) != 8 &&
                                                topic.LastPosted != null);
                    }
                    else
                    {
                        expression.Where<Topic, Forum, VAccess, Category>(
                            (topic, f, x, c) => c.BoardID == boardId && (c.Flags & 1) == 1 && topic.TopicMovedID == null &&
                                                x.UserID == pageUserId && x.ReadAccess > 0 && (topic.Flags & 8) != 8 &&
                                                topic.LastPosted != null && (f.Flags & 4) != 4);
                    }

                    if (categoryId > 0)
                    {
                        expression.And<Category>(c => c.ID == categoryId);
                    }

                    switch (sortOrder)
                    {
                        case 0:
                            expression.OrderByDescending<Topic>(t => t.LastPosted);
                            break;
                        case 1:
                            expression.OrderByDescending<Topic>(t => t.Views);
                            break;
                        case 2:
                            expression.OrderByDescending<Topic>(t => t.NumPosts);
                            break;
                    }

                    if (findLastRead)
                    {
                        var forumReadTrackExpression =
                            db.Connection.From<ForumReadTracking>(db.Connection.TableAlias("x"));
                        forumReadTrackExpression.Where(
                            $"""
                             x.{forumReadTrackExpression.Column<ForumReadTracking>(x => x.ForumID)}={expression.Column<Forum>(f => f.ID, true)}
                                                                     and x.{forumReadTrackExpression.Column<ForumReadTracking>(x => x.UserID)}={pageUserId}
                             """);
                        var forumReadTrackSql = forumReadTrackExpression
                            .Select($"{forumReadTrackExpression.Column<ForumReadTracking>(x => x.LastAccessDate)}")
                            .Limit(1).ToSelectStatement();

                        var topicReadTrackExpression =
                            db.Connection.From<TopicReadTracking>(db.Connection.TableAlias("x"));
                        topicReadTrackExpression.Where(
                            $"""
                             x.{topicReadTrackExpression.Column<TopicReadTracking>(x => x.TopicID)}={expression.Column<Topic>(t => t.ID, true)}
                                                                    and x.{topicReadTrackExpression.Column<TopicReadTracking>(x => x.UserID)}={pageUserId}
                             """);
                        var topicReadTrackSql = topicReadTrackExpression
                            .Select($"{topicReadTrackExpression.Column<TopicReadTracking>(x => x.LastAccessDate)}")
                            .Limit(1).ToSelectStatement();

                        expression.Take(numOfPostsToRetrieve).Select<Topic, Message, Forum, User, User>(
                            (t, m, f, u, lastUser) => new
                                                          {
                                                              t.LastPosted,
                                                              t.ForumID,
                                                              Forum = f.Name,
                                                              t.TopicName,
                                                              t.Status,
                                                              t.Styles,
                                                              TopicID = t.ID,
                                                              t.TopicMovedID,
                                                              t.UserID,
                                                              UserName = t.UserName ?? u.Name,
                                                              UserDisplayName = t.UserDisplayName ?? u.Name,
                                                              t.LastMessageID,
                                                              t.LastMessageFlags,
                                                              t.LastUserID,
                                                              t.NumPosts,
                                                              t.Views,
                                                              t.Posted,
                                                              LastMessage = m.MessageText,
                                                              LastUserName = Sql.TableAlias(lastUser.Name, "lastUser"),
                                                              LastUserDisplayName = Sql.TableAlias(lastUser.DisplayName, "lastUser"),
                                                              LastUserStyle = Sql.TableAlias(lastUser.UserStyle, "lastUser"),
                                                              LastUserSuspended = Sql.TableAlias(lastUser.Suspended, "lastUser"),
                                                              LastForumAccess = Sql.Custom($"({forumReadTrackSql})"),
                                                              LastTopicAccess = Sql.Custom($"({topicReadTrackSql})")
                                                          });
                    }
                    else
                    {
                        expression.Take(numOfPostsToRetrieve).Select<Topic, Message, Forum, User, User>(
                            (t, m, f, u, lastUser) => new
                                                          {
                                                              t.LastPosted,
                                                              t.ForumID,
                                                              Forum = f.Name,
                                                              t.TopicName,
                                                              t.Status,
                                                              t.Styles,
                                                              TopicID = t.ID,
                                                              t.TopicMovedID,
                                                              t.UserID,
                                                              UserName = t.UserName ?? u.Name,
                                                              UserDisplayName = t.UserDisplayName ?? u.Name,
                                                              t.LastMessageID,
                                                              t.LastMessageFlags,
                                                              t.LastUserID,
                                                              t.NumPosts,
                                                              t.Views,
                                                              t.Posted,
                                                              LastMessage = m.MessageText,
                                                              LastUserName = Sql.TableAlias(lastUser.Name, "lastUser"),
                                                              LastUserDisplayName = Sql.TableAlias(lastUser.DisplayName, "lastUser"),
                                                              LastUserStyle = Sql.TableAlias(lastUser.UserStyle, "lastUser"),
                                                              LastUserSuspended = Sql.TableAlias(lastUser.Suspended, "lastUser"),
                                                              LastForumAccess = Sql.Custom("(NULL)"),
                                                              LastTopicAccess = Sql.Custom("(NULL)")
                                                          });
                    }

                    return db.Connection.Select<LatestTopic>(expression);
                });
    }

    /// <summary>
    /// Gets the paged Topic List
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
    /// <param name="pageIndex">
    /// The page Index.
    /// </param>
    /// <param name="pageSize">
    /// The page Size.
    /// </param>
    /// <param name="findLastRead">
    /// Indicates if the list should contain the last Access Date
    /// </param>
    /// <returns>
    /// Returns the paged Topic List
    /// </returns>
    public static List<PagedTopic> ListPaged(
        this IRepository<Topic> repository,
        int forumId,
        int userId,
        DateTime? sinceDate,
        int pageIndex,
        int pageSize,
        bool findLastRead)
    {
        return repository.ListPaged(
            userId,
            pageIndex,
            pageSize,
            findLastRead,
            (t, x, c) => t.ForumID == forumId && t.Priority == 0 && t.LastPosted >= sinceDate && (t.Flags & 8) != 8
                         && x.UserID == userId && x.ReadAccess && (t.TopicMovedID != null || t.NumPosts > 0));
    }

    /// <summary>
    /// Gets the paged Topic List
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="pageIndex">
    /// The page Index.
    /// </param>
    /// <param name="pageSize">
    /// The page Size.
    /// </param>
    /// <param name="findLastRead">
    /// Indicates if the list should contain the last Access Date
    /// </param>
    /// <param name="whereCriteria">
    /// The where Criteria.
    /// </param>
    /// <returns>
    /// Returns the paged Topic List
    /// </returns>
    public static List<PagedTopic> ListPaged(
        this IRepository<Topic> repository,
        int userId,
        int pageIndex,
        int pageSize,
        bool findLastRead,
        Expression<Func<Topic, ActiveAccess, Category, bool>> whereCriteria)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

        return repository.DbAccess.Execute(
            db =>
                {
                    expression.Join<User>((t, u) => u.ID == t.UserID).Join<Forum>((t, f) => f.ID == t.ForumID)
                        .Join<Forum, ActiveAccess>((f, x) => x.ForumID == f.ID)
                        .Join<Forum, Category>((f, c) => c.ID == f.CategoryID);

                    expression.Where(whereCriteria);

                    // -- count total
                    var countTotalExpression = db.Connection.From<Topic>().Join<Forum>((t, f) => f.ID == t.ForumID)
                        .Join<Forum, ActiveAccess>((f, x) => x.ForumID == f.ID)
                        .Join<Forum, Category>((f, c) => c.ID == f.CategoryID);

                    countTotalExpression.Where(whereCriteria);

                    expression.OrderByDescending<Topic>(t => t.LastPosted).Page(pageIndex + 1, pageSize);

                    var countTotalSql = countTotalExpression
                        .Select(Sql.Count($"{countTotalExpression.Column<Topic>(x => x.ID)}")).ToSelectStatement();

                    // -- count deleted posts
                    var countDeletedExpression = db.Connection.From<Message>(db.Connection.TableAlias("mes"));
                    countDeletedExpression.Where(
                        $"""
                         mes.{countDeletedExpression.Column<Message>(x => x.TopicID)}={expression.Column<Topic>(x => x.ID, true)}
                                                             and (mes.{countDeletedExpression.Column<Message>(x => x.Flags)} & 8) = 8
                                                             and mes.{countDeletedExpression.Column<Message>(x => x.UserID)}={userId}
                         """);
                    var countDeletedSql = countDeletedExpression.Select(Sql.Count("1")).ToSelectStatement();

                    var lastTopicAccessSql = "NULL";
                    var lastForumAccessSql = "NULL";

                    if (findLastRead)
                    {
                        var topicAccessExpression =
                            db.Connection.From<TopicReadTracking>(db.Connection.TableAlias("y"));
                        topicAccessExpression.Where(
                            $"""
                             y.{topicAccessExpression.Column<TopicReadTracking>(y => y.TopicID)}={expression.Column<Topic>(x => x.ID, true)}
                                                                 and y.{topicAccessExpression.Column<TopicReadTracking>(y => y.UserID)}={userId}
                             """);
                        lastTopicAccessSql = topicAccessExpression.Select(
                                $"{topicAccessExpression.Column<TopicReadTracking>(x => x.LastAccessDate)}").Limit(1)
                            .ToSelectStatement();

                        var forumAccessExpression =
                            db.Connection.From<ForumReadTracking>(db.Connection.TableAlias("x"));
                        forumAccessExpression.Where(
                            $"""
                             x.{forumAccessExpression.Column<ForumReadTracking>(x => x.ForumID)}={expression.Column<Topic>(x => x.ForumID, true)}
                                                                 and x.{forumAccessExpression.Column<ForumReadTracking>(x => x.UserID)}={userId}
                             """);
                        lastForumAccessSql = forumAccessExpression.Select(
                                $"{forumAccessExpression.Column<ForumReadTracking>(x => x.LastAccessDate)}").Limit(1)
                            .ToSelectStatement();
                    }

                    // -- last user
                    var lastUserNameExpression = db.Connection.From<User>(db.Connection.TableAlias("usr"));
                    lastUserNameExpression.Where(
                        $"""
                         usr.{lastUserNameExpression.Column<User>(x => x.ID)}=
                                                            {expression.Column<Topic>(x => x.LastUserID, true)}
                         """);
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
                        $"""
                         fm.{firstMessageExpression.Column<Message>(x => x.TopicID)}=
                                                            {OrmLiteConfig.DialectProvider.IsNullFunction(expression.Column<Topic>(x => x.TopicMovedID, true),expression.Column<Topic>(x => x.ID, true))}
                                                            and fm.{firstMessageExpression.Column<Message>(x => x.Position)} = 0
                         """);
                    var firstMessageSql = firstMessageExpression.Select(
                        $"{firstMessageExpression.Column<Message>(x => x.MessageText)}").Limit(1).ToSelectStatement();

                    expression.Select<Topic, User, Forum>(
                        (c, b, d) => new
                                         {
                                             ForumID = d.ID,
                                             TopicID = c.ID,
                                             c.Posted,
                                             LinkTopicID = c.TopicMovedID ?? c.ID,
                                             c.TopicMovedID,
                                             Subject = c.TopicName,
                                             c.Description,
                                             c.Status,
                                             c.Styles,
                                             c.UserID,
                                             Starter = c.UserName ?? b.Name,
                                             StarterDisplay = c.UserDisplayName ?? b.DisplayName,
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
                                             TotalRows = Sql.Custom($"({countTotalSql})")
                                         });

                    return db.Connection.Select<PagedTopic>(expression);
                });
    }

    /// <summary>
    /// Get the Paged Announcements Topics
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
    /// <param name="pageIndex">
    /// The page Index.
    /// </param>
    /// <param name="pageSize">
    /// The page Size.
    /// </param>
    /// <param name="findLastRead">
    /// Indicates if the list should Contain the last Access Date
    /// </param>
    /// <returns>
    /// Returns the Paged Announcements Topics
    /// </returns>
    public static List<PagedTopic> ListAnnouncementsPaged(
        this IRepository<Topic> repository,
        int forumId,
        int userId,
        int pageIndex,
        int pageSize,
        bool findLastRead)
    {
        return repository.ListPaged(
            userId,
            pageIndex,
            pageSize,
            findLastRead,
            (t, x, c) => t.ForumID == forumId && t.Priority > 0 && (t.Flags & 8) != 8 &&
                         (t.TopicMovedID != null || t.NumPosts > 0) && x.UserID == userId && x.ReadAccess);
    }

    /// <summary>
    /// The topic_save.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forum">
    /// The forum.
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
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="priority">
    /// The priority.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="userDisplayName">
    /// The user Display Name.
    /// </param>
    /// <param name="ip">
    /// The IP Address.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <param name="newMessage">
    /// The new message.
    /// </param>
    /// <returns>
    /// Returns the Topic
    /// </returns>
    public static Topic SaveNew(
        this IRepository<Topic> repository,
        Forum forum,
        string subject,
        string status,
        string styles,
        string description,
        string message,
        User user,
        short priority,
        string userName,
        string userDisplayName,
        string ip,
        DateTime posted,
        MessageFlags flags,
        out Message newMessage)
    {
        var topicFlags = new TopicFlags { IsPersistent = flags.IsPersistent };

        var topic = new Topic
                        {
                            ForumID = forum.ID,
                            TopicName = subject,
                            UserID = user.ID,
                            Posted = posted,
                            Views = 0,
                            Priority = priority,
                            UserName = userName,
                            UserDisplayName = userDisplayName,
                            NumPosts = 0,
                            Description = description,
                            Status = status,
                            Styles = styles,
                            Flags = topicFlags.BitValue
        };

        topic.ID = repository.Insert(topic);

        newMessage = BoardContext.Current.GetRepository<Message>().SaveNew(
            forum,
            topic,
            user,
            message,
            userName,
            ip,
            posted,
            null,
            flags);

        if (flags.IsApproved)
        {
            repository.FireNew(topic.ID);
        }

        return topic;
    }

    /// <summary>
    /// Move the Topic.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="oldForumId">
    /// The old Forum Id.
    /// </param>
    /// <param name="newForumId">
    /// The new Forum Id.
    /// </param>
    /// <param name="showMoved">
    /// The show moved.
    /// </param>
    /// <param name="linkDays">
    /// The link Days.
    /// </param>
    public static void Move(
        this IRepository<Topic> repository,
        int topicId,
        int oldForumId,
        int newForumId,
        bool showMoved,
        int linkDays)
    {
        var topic = repository.GetById(topicId);

        repository.Move(topic, oldForumId, newForumId, showMoved, linkDays);
    }

    /// <summary>
    /// Move the Topic.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <param name="oldForumId">
    /// The old Forum Id.
    /// </param>
    /// <param name="newForumId">
    /// The new Forum Id.
    /// </param>
    /// <param name="showMoved">
    /// The show moved.
    /// </param>
    /// <param name="linkDays">
    /// The link Days.
    /// </param>
    public static void Move(
        this IRepository<Topic> repository,
        Topic topic,
        int oldForumId,
        int newForumId,
        bool showMoved,
        int linkDays)
    {
        if (showMoved)
        {
            // -- delete an old link if exists
            repository.Delete(t => t.TopicMovedID == topic.ID);

            var linkDate = DateTime.UtcNow.AddDays(linkDays);

            // -- create a moved message
            repository.Insert(
                new Topic
                    {
                        ForumID = topic.ForumID,
                        UserID = topic.UserID,
                        UserName = topic.UserName,
                        UserDisplayName = topic.UserDisplayName,
                        Posted = topic.Posted,
                        TopicName = topic.TopicName,
                        Views = 0,
                        Flags = topic.Flags,
                        Priority = topic.Priority,
                        PollID = topic.PollID,
                        TopicMovedID = topic.ID,
                        LastPosted = topic.LastPosted,
                        NumPosts = 0,
                        LinkDate = linkDate
                    });
        }

        // -- move the topic
        repository.UpdateOnly(() => new Topic { ForumID = newForumId }, t => t.ID == topic.ID);

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(newForumId));
        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(oldForumId));
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
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public static int Prune(
        this IRepository<Topic> repository,
        int boardId,
        int forumId,
        int days)
    {
        var topics = repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<Topic>();

                    expression.Join<Forum>((t, f) => f.ID == t.ForumID).Join<Forum, Category>((f, c) => c.ID == f.CategoryID);

                    if (forumId == 0)
                    {
                        expression.Where<Topic, Forum, Category>(
                            (t, f, c) => c.BoardID == boardId && (c.Flags & 1) == 1 && t.Priority == 0);
                    }
                    else
                    {
                        expression.Where<Topic, Forum, Category>(
                            (t, f, c) => c.BoardID == boardId && (c.Flags & 1) == 1 && f.ID == forumId && t.Priority == 0);
                    }

                    expression.And(
                        $"""
                         ({expression.Column<Topic>(x => x.Flags, true)} & 512) = 0
                                                                and {OrmLiteConfig.DialectProvider.DateDiffFunction("dd",
                                                                    expression.Column<Topic>(x => x.LastPosted, true),
                                                                    OrmLiteConfig.DialectProvider.GetUtcDateFunction())} > {days}
                         """);

                    return db.Connection.Select(expression);
                });

        return topics.Count;
    }

    /// <summary>
    /// Delete Topic
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
    public static void Delete(this IRepository<Topic> repository, int forumId, int topicId)
    {
        repository.Delete(forumId, topicId, false);
    }

    /// <summary>
    /// Delete Topic
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="eraseTopic">
    /// The erase topic.
    /// </param>
    public static void Delete(
        this IRepository<Topic> repository,
        int forumId,
        int topicId,
        bool eraseTopic)
    {
        var topic = repository.GetById(topicId);

        BoardContext.Current.GetRepository<Forum>().UpdateOnly(
            () => new Forum
                      {
                          LastPosted = null,
                          LastTopicID = null,
                          LastMessageID = null,
                          LastUserID = null,
                          LastUserName = null,
                          LastUserDisplayName = null
                      },
            x => x.LastTopicID == topicId);

        BoardContext.Current.GetRepository<Active>().UpdateOnly(
            () => new Active { TopicID = null },
            t => t.TopicID == topicId);

        // -- delete messages and topics
        if (eraseTopic)
        {
            repository.Delete(x => x.TopicMovedID == topicId);

            BoardContext.Current.GetRepository<Message>().UpdateOnly(
                () => new Message { ReplyTo = null },
                t => t.TopicID == topicId);

            // -- remove all messages
            var messages = BoardContext.Current.GetRepository<Message>().Get(x => x.TopicID == topicId);

            messages.ForEach(
                x => BoardContext.Current.GetRepository<Message>().Delete(
                    forumId,
                    topicId,
                    x,
                    false,
                    string.Empty,
                    true,
                    true,
                    true));

            BoardContext.Current.GetRepository<TopicTag>().Delete(x => x.TopicID == topicId);
            BoardContext.Current.GetRepository<Activity>().Delete(x => x.TopicID == topicId);
            BoardContext.Current.GetRepository<WatchTopic>().Delete(x => x.TopicID == topicId);
            BoardContext.Current.GetRepository<TopicReadTracking>().Delete(x => x.TopicID == topicId);
            BoardContext.Current.GetRepository<Topic>().Delete(x => x.TopicMovedID == topicId);
            BoardContext.Current.GetRepository<Topic>().Delete(x => x.ID == topicId);
        }
        else
        {
            var flags = topic.TopicFlags;

            flags.IsDeleted = true;

            repository.UpdateOnly(() => new Topic { Flags = flags.BitValue }, x => x.TopicMovedID == topicId);

            repository.UpdateOnly(() => new Topic { Flags = flags.BitValue }, x => x.ID == topicId);

            BoardContext.Current.GetRepository<Message>().DbAccess.Execute(
                db =>
                    {
                        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

                        return db.Connection.ExecuteSql(
                            $" update {expression.Table<Message>()} set Flags = Flags | 8 where TopicID = {topicId}");
                    });
        }

        BoardContext.Current.Get<ISearch>().DeleteSearchIndexRecordByTopicId(topicId);

        if (BoardContext.Current.CurrentForumPage != null)
        {
            BoardContext.Current.Get<ILogger<IRepository<Topic>>>().Log(
                BoardContext.Current.PageUserID,
                "YAF",
                BoardContext.Current.Get<ILocalization>().GetTextFormatted("DELETED_TOPIC", topicId),
                EventLogTypes.Information);
        }

        repository.FireDeleted(topicId);

        BoardContext.Current.Get<IRaiseEvent>().Raise(new UpdateForumStatsEvent(forumId));
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
    public static bool CheckForDuplicate(this IRepository<Topic> repository, string topicSubject)
    {
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
    public static Topic FindNext(this IRepository<Topic> repository, Topic currentTopic)
    {
        return repository.Get(
            t => t.LastPosted > currentTopic.LastPosted && t.ForumID == currentTopic.ForumID &&
                 (t.Flags & 8) != 8 && t.TopicMovedID == null).MinBy(t => t.LastPosted);
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
    public static Topic FindPrevious(this IRepository<Topic> repository, Topic currentTopic)
    {
        return repository.Get(
            t => t.LastPosted < currentTopic.LastPosted && t.ForumID == currentTopic.ForumID &&
                 (t.Flags & 8) != 8 && t.TopicMovedID == null).MaxBy(t => t.LastPosted);
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
    /// Returns Topic List
    /// </returns>
    public static List<Topic> SimpleList(
        this IRepository<Topic> repository,
        int startId = 0,
        int limit = 500)
    {
        return [.. repository.Get(t => t.ID >= limit && t.ID < startId + limit).OrderBy(t => t.ID)];
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
    public static void UnEncodeAllTopicsSubjects(
        this IRepository<Topic> repository,
        Func<string, string> decodeTopicFunc)
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
                            repository.UpdateOnly(() => new Topic { TopicName = decodedTopic }, t => t.ID == topic.ID);
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
    /// Returns the list of Deleted Topics
    /// </returns>
    public static List<Tuple<Forum, Topic>> GetDeletedTopics(
        this IRepository<Topic> repository,
        int boardId,
        string filter)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

        if (filter.IsSet())
        {
            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Join<Topic>((f, t) => t.ForumID == f.ID).Where<Topic, Category>(
                    (t, category) =>
                        category.BoardID == boardId && (category.Flags & 1) == 1 && (t.Flags & 8) == 8 && t.TopicName.Contains(filter));
        }
        else
        {
            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Join<Topic>((f, t) => t.ForumID == f.ID).Where<Topic, Category>(
                    (t, category) => category.BoardID == boardId && (category.Flags & 1) == 1 && (t.Flags & 8) == 8);
        }

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Forum, Topic>(expression));
    }

    /// <summary>
    /// Get Deleted Topics paged.
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
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    public static List<PagedTopic> GetDeletedTopicsPaged(
        this IRepository<Topic> repository,
        int boardId,
        string filter,
        int pageIndex,
        int pageSize)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Forum>();

        if (filter.IsSet())
        {
            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Join<Topic>((f, t) => t.ForumID == f.ID).Where<Topic, Category>(
                    (t, category) =>
                        category.BoardID == boardId && (category.Flags & 1) == 1 && (t.Flags & 8) == 8 && t.TopicName.Contains(filter));
        }
        else
        {
            expression.Join<Forum, Category>((forum, category) => category.ID == forum.CategoryID)
                .Join<Topic>((f, t) => t.ForumID == f.ID).Where<Topic, Category>(
                    (t, category) => category.BoardID == boardId && (category.Flags & 1) == 1 && (t.Flags & 8) == 8);
        }

        // -- count total
        var countTotalExpression = expression;

        var countTotalSql = countTotalExpression
            .Select(Sql.Count($"{countTotalExpression.Column<Topic>(x => x.ID)}")).ToSelectStatement();

        expression.Select<Forum, Topic>(
            (forum, topic) => new {
                ForumID = forum.ID,
                ForumName = forum.Name,
                TopicID = topic.ID,
                topic.Posted,
                LinkTopicID = topic.TopicMovedID ?? topic.ID,
                topic.TopicMovedID,
                Subject = topic.TopicName,
                topic.Description,
                topic.Status,
                topic.Styles,
                topic.UserID,
                Replies = topic.NumPosts - 1,
                topic.Views,
                topic.LastPosted,
                topic.LastUserID,
                topic.LastMessageFlags,
                topic.LastMessageID,
                LastTopicID = topic.ID,
                topic.LinkDate,
                TopicFlags = topic.Flags,
                topic.Priority,
                topic.PollID,
                ForumFlags = forum.Flags,
                topic.TopicImage,
                topic.NumPosts,
                TotalRows = Sql.Custom($"({countTotalSql})")
            });

        // Set Paging
        expression.Page(pageIndex + 1, pageSize);

        return repository.DbAccess.Execute(db => db.Connection.Select<PagedTopic>(expression));
    }

    /// <summary>
    /// Updates the Forum Last Post.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    public static void UpdateLastPost(
        this IRepository<Topic> repository,
        int topicId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Message>();

        expression.Join<Topic>((m, t) => t.ID == m.TopicID).Where<Message>(m => (m.Flags & 24) == 16 && m.TopicID == topicId)
            .OrderByDescending<Message>(m => m.Posted);

        var message = repository.DbAccess.Execute(db => db.Connection.Single(expression));

        if (message == null)
        {
            // Don't update if there are no more messages
            return;
        }

        repository.UpdateOnly(
            () => new Topic
                      {
                          LastPosted = message.Posted,
                          LastMessageID = message.ID,
                          LastUserID = message.UserID,
                          LastUserName = message.UserName,
                          LastUserDisplayName = message.UserDisplayName,
                          LastMessageFlags = message.Flags
                      },
            t => t.ID == topicId);
    }

    /// <summary>
    /// Get Topic with References
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <returns>
    /// Returns topic with references
    /// </returns>
    public static Topic GetTopic(
        this IRepository<Topic> repository,
        int topicId)
    {
        return repository.DbAccess.Execute(db => db.Connection.LoadSelect<Topic>(t => t.ID == topicId)).FirstOrDefault();
    }
}