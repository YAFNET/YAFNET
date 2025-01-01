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

using ServiceStack.DataAnnotations;

namespace YAF.Types.Objects.Model;

/// <summary>
/// The paged topic.
/// </summary>
[Serializable]
public class PagedTopic
{
    /// <summary>
    /// Gets or sets the forum name.
    /// </summary>
    public string ForumName { get; set; }

    /// <summary>
    /// Gets or sets the forum id.
    /// </summary>
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets the topic id.
    /// </summary>
    public int TopicID { get; set; }

    /// <summary>
    /// Gets or sets the posted.
    /// </summary>
    public DateTime Posted { get; set; }

    /// <summary>
    /// Gets or sets the link topic id.
    /// </summary>
    public int LinkTopicID { get; set; }

    /// <summary>
    /// Gets or sets the topic moved id.
    /// </summary>
    public int? TopicMovedID { get; set; }

    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the styles.
    /// </summary>
    public string Styles { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the starter.
    /// </summary>
    public string Starter { get; set; }

    /// <summary>
    /// Gets or sets the starter display.
    /// </summary>
    public string StarterDisplay { get; set; }

    /// <summary>
    /// Gets or sets the starter style.
    /// </summary>
    public string StarterStyle { get; set; }

    /// <summary>
    /// Gets or sets the replies.
    /// </summary>
    public int Replies { get; set; }

    /// <summary>
    /// Gets or sets the views.
    /// </summary>
    public int Views { get; set; }

    /// <summary>
    /// Gets or sets the last posted.
    /// </summary>
    public DateTime? LastPosted { get; set; }

    /// <summary>
    /// Gets or sets the last user name.
    /// </summary>
    public string LastUserName { get; set; }

    /// <summary>
    /// Gets or sets the last user display name.
    /// </summary>
    public string LastUserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the last user id.
    /// </summary>
    public int? LastUserID { get; set; }

    /// <summary>
    /// Gets or sets the last user suspended.
    /// </summary>
    public DateTime? LastUserSuspended { get; set; }

    /// <summary>
    /// Gets or sets the last user style.
    /// </summary>
    public string LastUserStyle { get; set; }

    /// <summary>
    /// Gets or sets the last message id.
    /// </summary>
    public int? LastMessageID { get; set; }

    /// <summary>
    /// Gets or sets the last message flags.
    /// </summary>
    public int? LastMessageFlags { get; set; }

    /// <summary>
    /// Gets or sets the link date.
    /// </summary>
    public DateTime? LinkDate { get; set; }

    /// <summary>
    /// Gets or sets the topic flags.
    /// </summary>
    public int TopicFlags { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets the poll id.
    /// </summary>
    public int? PollID { get; set; }

    /// <summary>
    /// Gets or sets the forum flags.
    /// </summary>
    public int ForumFlags { get; set; }

    /// <summary>
    /// Gets or sets the starter suspended.
    /// </summary>
    public DateTime? StarterSuspended { get; set; }

    /// <summary>
    /// Gets or sets the topic image.
    /// </summary>
    public string TopicImage { get; set; }

    /// <summary>
    /// Gets or sets the total rows.
    /// </summary>
    public int TotalRows { get; set; }

    /// <summary>
    /// Gets or sets the last topic access.
    /// </summary>
    public DateTime? LastTopicAccess { get; set; }

    /// <summary>
    /// Gets or sets the last forum access.
    /// </summary>
    public DateTime? LastForumAccess { get; set; }

    /// <summary>
    /// Gets or sets the number of deleted posts.
    /// </summary>
    public int NumPostsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the number of posts.
    /// </summary>
    public int NumPosts { get; set; }

    /// <summary>
    /// Gets or sets the last message.
    /// </summary>
    public string LastMessage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="PagedTopic"/> is selected.
    /// </summary>
    /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
    [Ignore]
    public bool Selected { get; set; }
}