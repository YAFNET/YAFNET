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

using ServiceStack.DataAnnotations;

namespace YAF.Types.Objects.Model;

/// <summary>
/// The forum read.
/// </summary>
[Serializable]
public class ForumRead
{
    /// <summary>
    /// Gets or sets the category id.
    /// </summary>
    public int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the category image.
    /// </summary>
    public string CategoryImage { get; set; }

    /// <summary>
    /// Gets or sets the forum id.
    /// </summary>
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets the forum.
    /// </summary>
    public string Forum { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the image url.
    /// </summary>
    public string ImageURL { get; set; }

    /// <summary>
    /// Gets or sets the styles.
    /// </summary>
    public string Styles { get; set; }

    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    public int? ParentID { get; set; }

    /// <summary>
    /// Gets or sets the topics.
    /// </summary>
    public int Topics { get; set; }

    /// <summary>
    /// Gets or sets the posts.
    /// </summary>
    public int Posts { get; set; }

    /// <summary>
    /// Gets or sets the last posted.
    /// </summary>
    public DateTime? LastPosted { get; set; }

    /// <summary>
    /// Gets or sets the last message id.
    /// </summary>
    public int? LastMessageID { get; set; }

    /// <summary>
    /// Gets or sets the last user id.
    /// </summary>
    public int? LastUserID { get; set; }

    /// <summary>
    /// Gets or sets the last message flags.
    /// </summary>
    public int? LastMessageFlags { get; set; }

    /// <summary>
    /// Gets or sets the last user name.
    /// </summary>
    public string LastUser { get; set; }

    /// <summary>
    /// Gets or sets the last user display name.
    /// </summary>
    public string LastUserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the last user suspended.
    /// </summary>
    public DateTime? LastUserSuspended { get; set; }

    /// <summary>
    /// Gets or sets the last user style.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets the last topic id.
    /// </summary>
    public int LastTopicID { get; set; }

    /// <summary>
    /// Gets or sets the topic moved id.
    /// </summary>
    public int? TopicMovedID { get; set; }

    /// <summary>
    /// Gets or sets the last topic name.
    /// </summary>
    public string LastTopicName { get; set; }

    /// <summary>
    /// Gets or sets the last topic styles.
    /// </summary>
    public string LastTopicStyles { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the forum flags.
    /// </summary>
    [Ignore]
    public ForumFlags ForumFlags
    {
        get => new(this.Flags);

        set => this.Flags = value.BitValue;
    }

    /// <summary>
    /// Gets or sets the viewing.
    /// </summary>
    public int Viewing { get; set; }

    /// <summary>
    /// Gets or sets the remote url.
    /// </summary>
    public string RemoteURL { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether read access.
    /// </summary>
    public bool ReadAccess { get; set; }

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
    /// Gets or sets the first message.
    /// </summary>
    public string FirstMessage { get; set; }

    /// <summary>
    /// Gets or sets the sub forums.
    /// </summary>
    /// <value>The sub forums.</value>
    public int SubForums { get; set; }

    /// <summary>
    /// Gets or sets the total.
    /// </summary>
    /// <value>The total.</value>
    public int Total { get; set; }
}