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

namespace YAF.Core.Services;

using System;

using YAF.Types.Models;

/// <summary>
/// The YAF Activity Stream.
/// </summary>
public class ActivityStream : IActivityStream, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityStream"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public ActivityStream(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; protected set; }

    /// <summary>
    /// Adds the New Topic to the PageUser's ActivityStream
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="topicId">
    /// The topic unique identifier.
    /// </param>
    /// <param name="messageId">
    /// The message unique identifier.
    /// </param>
    /// <param name="topicTitle">
    /// The topic title.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public void AddTopicToStream(User user, int topicId, int messageId, string topicTitle, string message)
    {
        var flags = new ActivityFlags { CreatedTopic = true };

        var activity = new Activity
                           {
                               Flags = flags.BitValue,
                               TopicID = topicId,
                               MessageID = messageId,
                               UserID = user.ID,
                               Notification = false,
                               Created = DateTime.UtcNow
                           };

        this.GetRepository<Activity>().Insert(activity);

        this.Get<IRaiseEvent>().Raise(new ActivityNotificationEvent(user.Name));
    }

    /// <summary>
    /// Adds the Reply to the PageUser's ActivityStream
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="topicId">
    /// The topic unique identifier.
    /// </param>
    /// <param name="messageId">
    /// The message unique identifier.
    /// </param>
    /// <param name="topicTitle">
    /// The topic title.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public void AddReplyToStream(User user, int topicId, int messageId, string topicTitle, string message)
    {
        var flags = new ActivityFlags { CreatedReply = true };

        var activity = new Activity
                           {
                               Flags = flags.BitValue,
                               TopicID = topicId,
                               MessageID = messageId,
                               UserID = user.ID,
                               Notification = false,
                               Created = DateTime.UtcNow
                           };

        this.GetRepository<Activity>().Insert(activity);

        this.Get<IRaiseEvent>().Raise(new ActivityNotificationEvent(user.Name));
    }

    /// <summary>
    /// Adds the New Watch Topic to the PageUser's ActivityStream
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="topicId">
    /// The topic unique identifier.
    /// </param>
    /// <param name="messageId">
    /// The message unique identifier.
    /// </param>
    /// <param name="topicTitle">
    /// The topic title.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="fromUserId">
    /// The from PageUser Id.
    /// </param>
    public void AddWatchTopicToStream(User user, int topicId, int messageId, string topicTitle, string message, int fromUserId)
    {
        var flags = new ActivityFlags { WatchForumReply = true };

        var activity = new Activity
                           {
                               Flags = flags.BitValue,
                               FromUserID = fromUserId,
                               TopicID = topicId,
                               MessageID = messageId,
                               UserID = user.ID,
                               Notification = true,
                               Created = DateTime.UtcNow
                           };

        this.GetRepository<Activity>().Insert(activity);

        this.Get<IRaiseEvent>().Raise(new ActivityNotificationEvent(user.Name));
    }

    /// <summary>
    /// Adds the Watch Reply to the PageUser's ActivityStream
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="topicId">
    /// The topic unique identifier.
    /// </param>
    /// <param name="messageId">
    /// The message unique identifier.
    /// </param>
    /// <param name="topicTitle">
    /// The topic title.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="fromUserId">
    /// The from PageUser Id.
    /// </param>
    public void AddWatchReplyToStream(User user, int topicId, int messageId, string topicTitle, string message, int fromUserId)
    {
        var flags = new ActivityFlags { WatchTopicReply = true };

        var activity = new Activity
                           {
                               Flags = flags.BitValue,
                               FromUserID = fromUserId,
                               TopicID = topicId,
                               MessageID = messageId,
                               UserID = user.ID,
                               Notification = true,
                               Created = DateTime.UtcNow
                           };

        this.GetRepository<Activity>().Insert(activity);

        this.Get<IRaiseEvent>().Raise(new ActivityNotificationEvent(user.Name));
    }

    /// <summary>
    /// Add Mention to Users Stream
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <param name="fromUserId">
    /// The from user id.
    /// </param>
    public void AddMentionToStream(int userId, int topicId, int messageId, int fromUserId)
    {
        var flags = new ActivityFlags { WasMentioned = true };

        var activity = new Activity
                           {
                               Flags = flags.BitValue,
                               FromUserID = fromUserId,
                               TopicID = topicId,
                               MessageID = messageId,
                               UserID = userId,
                               Notification = true,
                               Created = DateTime.UtcNow
                           };

        this.GetRepository<Activity>().Insert(activity);

        var user = this.GetRepository<User>().GetById(userId);

        this.Get<IRaiseEvent>().Raise(new ActivityNotificationEvent(user.Name));

        this.Get<IDataCache>().Remove(
            string.Format(Constants.Cache.ActiveUserLazyData, userId));
    }

    /// <summary>
    /// Add Quoting to Users Stream
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <param name="fromUserId">
    /// The from user id.
    /// </param>
    public void AddQuotingToStream(int userId, int topicId, int messageId, int fromUserId)
    {
        var flags = new ActivityFlags { WasQuoted = true };

        var activity = new Activity
                           {
                               Flags = flags.BitValue,
                               FromUserID = fromUserId,
                               TopicID = topicId,
                               MessageID = messageId,
                               UserID = userId,
                               Notification = true,
                               Created = DateTime.UtcNow
                           };

        this.GetRepository<Activity>().Insert(activity);

        var user = this.GetRepository<User>().GetById(userId);

        this.Get<IRaiseEvent>().Raise(new ActivityNotificationEvent(user.Name));

        this.Get<IDataCache>().Remove(
            string.Format(Constants.Cache.ActiveUserLazyData, userId));
    }

    /// <summary>
    /// The add thanks received to stream.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <param name="fromUserId">
    /// The from user id.
    /// </param>
    public void AddThanksReceivedToStream(int userId, int topicId, int messageId, int fromUserId)
    {
        var flags = new ActivityFlags { ReceivedThanks = true };

        var activity = new Activity
                           {
                               Flags = flags.BitValue,
                               FromUserID = fromUserId,
                               TopicID = topicId,
                               MessageID = messageId,
                               UserID = userId,
                               Notification = true,
                               Created = DateTime.UtcNow
                           };

        this.GetRepository<Activity>().Insert(activity);

        var user = this.GetRepository<User>().GetById(userId);

        this.Get<IRaiseEvent>().Raise(new ActivityNotificationEvent(user.Name));

        this.Get<IDataCache>().Remove(
            string.Format(Constants.Cache.ActiveUserLazyData, userId));
    }

    /// <summary>
    /// The add thanks given to stream.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <param name="fromUserId">
    /// The from user id.
    /// </param>
    public void AddThanksGivenToStream(int userId, int topicId, int messageId, int fromUserId)
    {
        var flags = new ActivityFlags { GivenThanks = true };

        var activity = new Activity
                           {
                               Flags = flags.BitValue,
                               FromUserID = fromUserId,
                               TopicID = topicId,
                               MessageID = messageId,
                               UserID = userId,
                               Notification = false,
                               Created = DateTime.UtcNow
                           };

        this.GetRepository<Activity>().Insert(activity);
    }

    /// <summary>
    /// Adds the become friends to stream.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="fromUserId">From user identifier.</param>
    public void AddBecomeFriendsToStream(int userId, int fromUserId)
    {
        var flags = new ActivityFlags { BecomeFriends = true };

        var activity = new Activity
                           {
                               Flags = flags.BitValue,
                               FromUserID = fromUserId,
                               UserID = userId,
                               Notification = false,
                               Created = DateTime.UtcNow
                           };

        this.GetRepository<Activity>().Insert(activity);
    }
}