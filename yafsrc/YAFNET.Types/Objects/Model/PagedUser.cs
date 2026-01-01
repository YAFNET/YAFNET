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
/// The paged user.
/// </summary>
public class PagedUser
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the user style.
    /// </summary>
    public string UserStyle { get; set; }

    /// <summary>
    /// Gets or sets the avatar.
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the avatar image.
    /// </summary>
    public byte[] AvatarImage { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the joined.
    /// </summary>
    public DateTime Joined { get; set; }

    /// <summary>
    /// Gets or sets the last visit.
    /// </summary>
    public DateTime LastVisit { get; set; }

    /// <summary>
    /// Gets or sets the number of posts.
    /// </summary>
    public int NumPosts { get; set; }

    /// <summary>
    /// Gets or sets the is guest.
    /// </summary>
    public bool? IsGuest { get; set; }

    /// <summary>
    /// Gets or sets the rank name.
    /// </summary>
    public string RankName { get; set; }

    /// <summary>
    /// Gets or sets the total rows.
    /// </summary>
    public int TotalRows { get; set; }
}