﻿/* Yet Another Forum.NET
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
namespace YAF.Types.Models;

/// <summary>
///     A class which represents the yaf_vaccess views.
/// </summary>
[Serializable]
[Alias("vaccess")]
public class VAccess : IEntity
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [Required]
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the forum id.
    /// </summary>
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is admin.
    /// </summary>
    public int? IsAdmin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is forum moderator.
    /// </summary>
    public int? IsForumModerator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is moderator.
    /// </summary>
    public int? IsModerator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether read access.
    /// </summary>
    public int? ReadAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether post access.
    /// </summary>
    public int? PostAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether reply access.
    /// </summary>
    public int? ReplyAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether priority access.
    /// </summary>
    public int? PriorityAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether poll access.
    /// </summary>
    public int? PollAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether vote access.
    /// </summary>
    public int? VoteAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether moderator access.
    /// </summary>
    public int? ModeratorAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether edit access.
    /// </summary>
    public int? EditAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether delete access.
    /// </summary>
    public int? DeleteAccess { get; set; }
}