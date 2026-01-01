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

namespace YAF.Types.Objects.Model;

/// <summary>
/// The page load.
/// </summary>
public class PageLoad
{
    /// <summary>
    /// Gets or sets a value indicating whether active update.
    /// </summary>
    public bool ActiveUpdate { get; set; }

    /// <summary>
    /// Gets or sets the previous visit.
    /// </summary>
    public DateTime? PreviousVisit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is moderator any.
    /// </summary>
    public bool IsModeratorAny { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is crawler.
    /// </summary>
    public bool IsCrawler { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether delete access.
    /// </summary>
    public bool DeleteAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether download access.
    /// </summary>
    public bool DownloadAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether edit access.
    /// </summary>
    public bool EditAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is admin.
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is forum moderator.
    /// </summary>
    public bool IsForumModerator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is guest x.
    /// </summary>
    public bool IsGuestX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is moderator.
    /// </summary>
    public bool IsModerator { get; set; }

    /// <summary>
    /// Gets or sets the last active.
    /// </summary>
    public DateTime? LastActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether moderator access.
    /// </summary>
    public bool ModeratorAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether poll access.
    /// </summary>
    public bool PollAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether post access.
    /// </summary>
    public bool PostAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether priority access.
    /// </summary>
    public bool PriorityAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether read access.
    /// </summary>
    public bool ReadAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether reply access.
    /// </summary>
    public bool ReplyAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether upload access.
    /// </summary>
    public bool UploadAccess { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether vote access.
    /// </summary>
    public bool VoteAccess { get; set; }

    /// <summary>
    /// Gets or sets the guest user id.
    /// </summary>
    public int GuestUserID { get; set; }

    /// <summary>
    /// Gets or sets the last posted.
    /// </summary>
    /// <value>The last posted.</value>
    public DateTime? LastPosted { get; set; }
}