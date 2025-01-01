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

namespace YAF.Types.Objects.Model;

/// <summary>
/// Class BuddyUser.
/// </summary>
public class BuddyUser
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    /// <value>The user identifier.</value>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the board identifier.
    /// </summary>
    /// <value>The board identifier.</value>
    public int BoardID { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>The display name.</value>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the joined.
    /// </summary>
    /// <value>The joined.</value>
    public DateTime Joined { get; set; }

    /// <summary>
    /// Gets or sets the number posts.
    /// </summary>
    /// <value>The number posts.</value>
    public int NumPosts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="BuddyUser"/> is approved.
    /// </summary>
    /// <value><c>true</c> if approved; otherwise, <c>false</c>.</value>
    public bool Approved { get; set; }

    /// <summary>
    /// Gets or sets the requested.
    /// </summary>
    /// <value>The requested.</value>
    public DateTime Requested { get; set; }

    /// <summary>
    /// Gets or sets the user style.
    /// </summary>
    /// <value>The user style.</value>
    public string UserStyle { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    /// <value>The suspended.</value>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the avatar.
    /// </summary>
    /// <value>The avatar.</value>
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the avatar image.
    /// </summary>
    /// <value>The avatar image.</value>
    public byte[] AvatarImage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="BuddyUser"/> is activity.
    /// </summary>
    /// <value><c>true</c> if activity; otherwise, <c>false</c>.</value>
    public bool Activity { get; set; }
}