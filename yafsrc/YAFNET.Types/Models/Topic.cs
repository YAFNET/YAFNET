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

namespace YAF.Types.Models;

/// <summary>
/// A class which represents the Topic table.
/// </summary>
[Serializable]
public class Topic : IEntity, IHaveID
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [AutoIncrement]
    [Alias("TopicID")]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the forum.
    /// </summary>
    /// <value>The forum.</value>
    [Reference]
    public Forum Forum { get; set; }

    /// <summary>
    /// Gets or sets the forum id.
    /// </summary>
    [References(typeof(Forum))]
    [Required]
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [References(typeof(User))]
    [Required]
    [Index]
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    [StringLength(255)]
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the posted.
    /// </summary>
    [Required]
    public DateTime Posted { get; set; }

    /// <summary>
    /// Gets or sets the topic name.
    /// </summary>
    [Required]
    [Alias("Topic")]
    [StringLength(100)]
    public string TopicName { get; set; }

    /// <summary>
    /// Gets or sets the views.
    /// </summary>
    [Required]
    public int Views { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    [Required]
    public short Priority { get; set; }

    /// <summary>
    /// Gets or sets the poll id.
    /// </summary>
    public int? PollID { get; set; }

    /// <summary>
    /// Gets or sets the topic moved id.
    /// </summary>
    [References(typeof(Topic))]
    public int? TopicMovedID { get; set; }

    /// <summary>
    /// Gets or sets the last posted.
    /// </summary>
    [Index]
    public DateTime? LastPosted { get; set; }

    /// <summary>
    /// Gets or sets the last message id.
    /// </summary>
    public int? LastMessageID { get; set; }

    /// <summary>
    /// Gets or sets the last user id.
    /// </summary>
    [References(typeof(User))]
    public int? LastUserID { get; set; }

    /// <summary>
    /// Gets or sets the last user name.
    /// </summary>
    [StringLength(255)]
    public string LastUserName { get; set; }

    /// <summary>
    /// Gets or sets the number of posts.
    /// </summary>
    [Required]
    public int NumPosts { get; set; }

    /// <summary>
    /// Gets or sets the topic flags.
    /// </summary>
    [Ignore]
    public TopicFlags TopicFlags
    {
        get => new(this.Flags);

        set => this.Flags = value.BitValue;
    }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    [Required]
    [Index]
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the answer message id.
    /// </summary>
    public int? AnswerMessageId { get; set; }

    /// <summary>
    /// Gets or sets the last message flags.
    /// </summary>
    public int? LastMessageFlags { get; set; }

    /// <summary>
    /// Gets or sets the topic image.
    /// </summary>
    [StringLength(255)]
    public string TopicImage { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [StringLength(255)]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [StringLength(255)]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the styles.
    /// </summary>
    [StringLength(255)]
    public string Styles { get; set; }

    /// <summary>
    /// Gets or sets the link date.
    /// </summary>
    public DateTime? LinkDate { get; set; }

    /// <summary>
    /// Gets or sets the user display name.
    /// </summary>
    [StringLength(255)]
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the last user display name.
    /// </summary>
    [StringLength(255)]
    public string LastUserDisplayName { get; set; }
}