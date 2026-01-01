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

namespace YAF.Types.Models;

/// <summary>
///     A class which represents the ActiveAccess table.
/// </summary>
[Serializable]
[CompositePrimaryKey(nameof(UserID), nameof(ForumID))]
public class ActiveAccess : IEntity, IHaveBoardID
{
    /// <summary>
    /// Gets or sets the board id.
    /// </summary>
    [Required]
    public int BoardID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether delete access.
    /// </summary>
    [Required]
    public bool DeleteAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether edit access.
    /// </summary>
    public bool EditAccess { get; set; }

    /// <summary>
    /// Gets or sets the forum id.
    /// </summary>
    [Required]
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is admin.
    /// </summary>
    [Required]
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is forum moderator.
    /// </summary>
    [Required]
    public bool IsForumModerator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is guest x.
    /// </summary>
    [Required]
    [Default(typeof(bool), "1")]
    public bool IsGuestX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is moderator.
    /// </summary>
    [Required]
    public bool IsModerator { get; set; }

    /// <summary>
    /// Gets or sets the last active.
    /// </summary>
    public DateTime? LastActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether moderator access.
    /// </summary>
    [Required]
    public bool ModeratorAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether poll access.
    /// </summary>
    [Required]
    public bool PollAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether post access.
    /// </summary>
    [Required]
    public bool PostAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether priority access.
    /// </summary>
    [Required]
    public bool PriorityAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether read access.
    /// </summary>
    [Required]
    public bool ReadAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether reply access.
    /// </summary>
    [Required]
    public bool ReplyAccess { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [Required]
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether vote access.
    /// </summary>
    [Required]
    public bool VoteAccess { get; set; }
}