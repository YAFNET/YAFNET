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

namespace YAF.Types.Interfaces;

using YAF.Types.Models;

/// <summary>
/// Activity Stream Interface
/// </summary>
public interface IActivityStream
{
    // void AddAlbumImageToStream(int forumID, long topicID, int messageID, string topicTitle, string message);

    /// <summary>
    /// Adds the New Topic to the User's ActivityStream
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
    void AddTopicToStream(User user, int topicId, int messageId, string topicTitle, string message);

    /// <summary>
    /// Adds the Watch Reply to the User's ActivityStream
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
    /// The from User Id.
    /// </param>
    void AddWatchReplyToStream(User user, int topicId, int messageId, string topicTitle, string message, int fromUserId);

    /// <summary>
    /// Adds the New Topic to the User's ActivityStream
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
    /// The from User Id.
    /// </param>
    void AddWatchTopicToStream(User user, int topicId, int messageId, string topicTitle, string message, int fromUserId);

    /// <summary>
    /// Adds the Reply to the User's ActivityStream
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
    void AddReplyToStream(User user, int topicId, int messageId, string topicTitle, string message);

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
    void AddMentionToStream(int userId, int topicId, int messageId, int fromUserId);

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
    void AddQuotingToStream(int userId, int topicId, int messageId, int fromUserId);

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
    void AddThanksReceivedToStream(int userId, int topicId, int messageId, int fromUserId);

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
    void AddThanksGivenToStream(int userId, int topicId, int messageId, int fromUserId);

    /// <summary>
    /// Adds the become friends to stream.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="fromUserId">From user identifier.</param>
    void AddBecomeFriendsToStream(int userId, int fromUserId);
}