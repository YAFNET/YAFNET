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
namespace YAF.Types.Interfaces
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using YAF.Types.Objects;

    /// <summary>
    /// The YAF Session Interface
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Gets or sets Twitter Token.
        /// </summary>
        string TwitterToken { get; set; }

        /// <summary>
        /// Gets or sets Twitter Token Secret.
        /// </summary>
        string TwitterTokenSecret { get; set; }

        /// <summary>
        /// Gets or sets the multi quote ids.
        /// </summary>
        /// <value>
        /// The multi quote ids.
        /// </value>
        List<MultiQuote> MultiQuoteIds { get; set; }

        /// <summary>
        /// Gets or sets the user activity page size.
        /// </summary>
        int? UserActivityPageSize { get; set; }

        /// <summary>
        ///   Gets or sets Unread Topic Since.
        /// </summary>
        int? UnreadTopicSince { get; set; }

        /// <summary>
        ///   Gets or sets User Topic Since.
        /// </summary>
        int? UserTopicSince { get; set; }

        /// <summary>
        ///   Gets or sets ActiveTopicSince.
        /// </summary>
        int? ActiveTopicSince { get; set; }

        /// <summary>
        ///   Gets or sets UnansweredTopicSince.
        /// </summary>
        int? UnansweredTopicSince { get; set; }

        /// <summary>
        ///   Gets or sets FavoriteTopicSince.
        /// </summary>
        int? FavoriteTopicSince { get; set; }

        /// <summary>
        ///   Gets or sets ForumRead.
        /// </summary>
        Hashtable ForumRead { get; set; }

        /// <summary>
        ///   Gets or sets LastPm.
        /// </summary>
        DateTime LastPendingBuddies { get; set; }

        /// <summary>
        ///   Gets or sets LastPm.
        /// </summary>
        DateTime LastPm { get; set; }

        /// <summary>
        ///   Gets or sets LastPost.
        /// </summary>
        DateTime LastPost { get; set; }

        /// <summary>
        ///   Gets or sets LastVisit.
        /// </summary>
        DateTime? LastVisit { get; set; }

        /// <summary>
        ///   Gets PanelState.
        /// </summary>
        [NotNull]
        IPanelSessionState PanelState { get; }

        /// <summary>
        ///   Gets or sets ShowList.
        /// </summary>
        int ShowList { get; set; }

        /// <summary>
        ///   Gets or sets TopicRead.
        /// </summary>
        Hashtable TopicRead { get; set; }

        /// <summary>
        ///   Gets or sets UnreadTopics.
        /// </summary>
        int UnreadTopics { get; set; }

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
}