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

namespace YAF.Types.Interfaces.Services;

/// <summary>
/// Read Tracking Interface for the Current User
/// </summary>
public interface IReadTrackCurrentUser
{
    /// <summary>
    /// Gets the Global Last Read DateTime a user Reads a topic or marks a forum as read
    /// </summary>
    /// <value>
    /// The last read.
    /// </value>
    DateTime LastRead { get; }

    /// <summary>
    /// Returns the last time that the forum was read or marked as Read.
    /// </summary>
    /// <param name="forumId">The forum Id of the Forum</param>
    /// <param name="readTimeOverride">The read time override.</param>
    /// <returns>
    /// Returns the DateTime object from the Forum Id.
    /// </returns>
    DateTime GetForumRead(int forumId, DateTime? readTimeOverride);

    /// <summary>
    /// Returns the last time that the Topic was read.
    /// </summary>
    /// <param name="topicId">The topic Id you wish to find the DateTime object for.</param>
    /// <param name="readTimeOverride">The read time override.</param>
    /// <returns>
    /// Returns the DateTime object from the topic Id.
    /// </returns>
    DateTime GetTopicRead(int topicId, DateTime? readTimeOverride);

    /// <summary>
    /// Add Or Update The Forum Read DateTime
    /// </summary>
    /// <param name="forumId">
    /// The forum Id of the Forum.
    /// </param>
    void SetForumRead(int forumId);

    /// <summary>
    /// Add Or Update The topic Read DateTime
    /// </summary>
    /// <param name="topicId">
    /// The topic id to mark read.
    /// </param>
    void SetTopicRead(int topicId);
}