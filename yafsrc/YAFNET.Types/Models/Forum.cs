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
/// A class which represents the Forum table.
/// </summary>
[Serializable]
[UniqueConstraint(nameof(CategoryID), nameof(Name))]
public class Forum : IEntity, IHaveID
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [AutoIncrement]
    [Alias("ForumID")]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    /// <value>The category.</value>
    [Reference]
    public Category Category { get; set; }

    /// <summary>
    /// Gets or sets the category id.
    /// </summary>
    [References(typeof(Category))]
    [Required]
    [Index]
    public int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    [References(typeof(Forum))]
    [Index]
    public int? ParentID { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [StringLength(255)]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    [Required]
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the last posted.
    /// </summary>
    public DateTime? LastPosted { get; set; }

    /// <summary>
    /// Gets or sets the last topic id.
    /// </summary>
    public int? LastTopicID { get; set; }

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
    /// Gets or sets the last user display name.
    /// </summary>
    [StringLength(255)]
    public string LastUserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the number of topics.
    /// </summary>
    [Required]
    public int NumTopics { get; set; }

    /// <summary>
    /// Gets or sets the number of posts.
    /// </summary>
    [Required]
    public int NumPosts { get; set; }

    /// <summary>
    /// Gets or sets the remote url.
    /// </summary>
    [StringLength(100)]
    public string RemoteURL { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    [Required]
    [Index]
    [Default(0)]
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
    /// Gets or sets the theme url.
    /// </summary>
    [StringLength(50)]
    public string ThemeURL { get; set; }

    /// <summary>
    /// Gets or sets the image url.
    /// </summary>
    [StringLength(128)]
    public string ImageURL { get; set; }

    /// <summary>
    /// Gets or sets the styles.
    /// </summary>
    [StringLength(255)]
    public string Styles { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public int? UserID { get; set; }

    /// <summary>
    /// Gets or sets the moderated post count.
    /// </summary>
    public int? ModeratedPostCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is moderated new topic only.
    /// </summary>
    [Default(typeof(bool), "0")]
    public bool IsModeratedNewTopicOnly { get; set; }
}