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

namespace YAF.Core.Services
{
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
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
        public ActivityStream([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; protected set; }

        /// <summary>
        /// Adds the New Topic to the User's ActivityStream
        /// </summary>
        /// <param name="forumID">The forum unique identifier.</param>
        /// <param name="topicID">The topic unique identifier.</param>
        /// <param name="messageID">The message unique identifier.</param>
        /// <param name="topicTitle">The topic title.</param>
        /// <param name="message">The message.</param>
        public void AddTopicToStream(int forumID, long topicID, int messageID, string topicTitle, string message)
        {
            var flags = new ActivityFlags { CreatedTopic = true };

            var activity = new Activity
            {
                                   Flags = flags.BitValue,
                                   TopicID = topicID.ToType<int>(),
                                   MessageID = messageID,
                                   UserID = forumID,
                                   Notification = false,
                                   Created = System.DateTime.UtcNow
                               };

            this.GetRepository<Activity>().Insert(activity);
        }

        /// <summary>
        /// Adds the Reply to the User's ActivityStream
        /// </summary>
        /// <param name="forumID">The forum unique identifier.</param>
        /// <param name="topicID">The topic unique identifier.</param>
        /// <param name="messageID">The message unique identifier.</param>
        /// <param name="topicTitle">The topic title.</param>
        /// <param name="message">The message.</param>
        public void AddReplyToStream(int forumID, long topicID, int messageID, string topicTitle, string message)
        {
            var flags = new ActivityFlags { CreatedReply = true };

            var activity = new Activity
            {
                                   Flags = flags.BitValue,
                                   TopicID = topicID.ToType<int>(),
                                   MessageID = messageID,
                                   UserID = forumID,
                                   Notification = false,
                                   Created = System.DateTime.UtcNow
            };

            this.GetRepository<Activity>().Insert(activity);
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
                                   Created = System.DateTime.UtcNow
            };

            this.GetRepository<Activity>().Insert(activity);

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
                                   Created = System.DateTime.UtcNow
                               };

            this.GetRepository<Activity>().Insert(activity);

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
                                   Created = System.DateTime.UtcNow
            };

            this.GetRepository<Activity>().Insert(activity);

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
                                   Created = System.DateTime.UtcNow
            };

            this.GetRepository<Activity>().Insert(activity);
        }
    }
}