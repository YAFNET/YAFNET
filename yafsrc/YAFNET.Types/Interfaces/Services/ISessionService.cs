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

namespace YAF.Types.Interfaces.Services;

using System.Collections;
using System.Collections.Generic;

using Objects;
using Objects.Model;

/// <summary>
/// The YAF Session Interface
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Gets or sets the index of the board forums.
    /// </summary>
    /// <value>The index of the board forums.</value>
    public int BoardForumsIndex { get; set; }

    /// <summary>
    /// Gets or sets the mods.
    /// </summary>
    /// <value>The mods.</value>
    public List<SimpleModerator> Mods { get; set; }

    /// <summary>
    /// Gets or sets the forums.
    /// </summary>
    /// <value>The forums.</value>
    public List<ForumRead> Forums { get; set; }

    /// <summary>
    /// Gets or sets the multi quote ids.
    /// </summary>
    /// <value>
    /// The multi quote ids.
    /// </value>
    List<MultiQuote> MultiQuoteIds { get; set; }

    /// <summary>
    ///   Gets or sets ForumRead.
    /// </summary>
    Hashtable ForumRead { get; set; }

    /// <summary>
    ///   Gets or sets LastPm.
    /// </summary>
    DateTime LastPendingBuddies { get; set; }

    /// <summary>
    ///   Gets or sets LastVisit.
    /// </summary>
    DateTime? LastVisit { get; set; }

    /// <summary>
    ///   Gets PanelState.
    /// </summary>
    IPanelSessionState PanelState { get; }

    /// <summary>
    /// Gets the page data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    public T GetPageData<T>();

    /// <summary>
    /// Sets the page data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newData">The new data.</param>
    public void SetPageData<T>(T newData);

    /// <summary>
    /// Clears the page data.
    /// </summary>
    public void ClearPageData();

    /// <summary>
    ///   Gets or sets TopicRead.
    /// </summary>
    Hashtable TopicRead { get; set; }

    /// <summary>
    /// Gets or sets the info message.
    /// </summary>
    string InfoMessage { get; set; }

    /// <summary>
    /// Gets the last time the forum was read.
    /// </summary>
    /// <param name="forumId">
    /// This is the ID of the forum you wish to get the last read date from.
    /// </param>
    /// <returns>
    /// A DateTime object of when the forum was last read.
    /// </returns>
    DateTime GetForumRead(int forumId);

    /// <summary>
    /// Returns the last time that the topic Id was read.
    /// </summary>
    /// <param name="topicId">
    /// The topicID you wish to find the DateTime object for.
    /// </param>
    /// <returns>
    /// The DateTime object from the topic Id.
    /// </returns>
    DateTime GetTopicRead(int topicId);

    /// <summary>
    /// Sets the time that the forum was read.
    /// </summary>
    /// <param name="forumId">
    /// The forum Id that was read.
    /// </param>
    /// <param name="date">
    /// The DateTime you wish to set the read to.
    /// </param>
    void SetForumRead(int forumId, DateTime date);

    /// <summary>
    /// Sets the time that the <paramref name="topicId"/> was read.
    /// </summary>
    /// <param name="topicId">
    /// The topic Id that was read.
    /// </param>
    /// <param name="date">
    /// The DateTime you wish to set the read to.
    /// </param>
    void SetTopicRead(int topicId, DateTime date);
}