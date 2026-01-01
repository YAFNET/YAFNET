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

namespace YAF.Types.Objects;

/// <summary>
/// The moderator.
/// </summary>
[Serializable]
public class SimpleModerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleModerator"/> class.
    /// </summary>
    public SimpleModerator()
    {
        this.UserBlockFlags = new UserBlockFlags(this.ModeratorBlockFlags);
    }

    /// <summary>
    ///   Gets or sets The Moderators Forums
    /// </summary>
    public ModeratorsForums[] ForumIDs { get; set; }

    /// <summary>
    /// Gets or sets ForumID.
    /// </summary>
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets the parent identifier.
    /// </summary>
    /// <value>The parent identifier.</value>
    public int? ParentID { get; set; }

    /// <summary>
    /// Gets or sets Forum Name.
    /// </summary>
    public string ForumName { get; set; }

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    /// <value>The category identifier.</value>
    public int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    /// <value>The name of the category.</value>
    public string CategoryName { get; set; }

    /// <summary>
    /// Gets or sets ModeratorID.
    /// </summary>
    public int ModeratorID { get; set; }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets Email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the block flags.
    /// </summary>
    public int ModeratorBlockFlags { get; set; }

    /// <summary>
    /// Gets or sets the user block flags.
    /// </summary>
    public UserBlockFlags UserBlockFlags { get; set; }

    /// <summary>
    /// Gets or sets Avatar.
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [avatar image].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [avatar image]; otherwise, <c>false</c>.
    /// </value>
    public byte[] AvatarImage { get; set; }

    /// <summary>
    /// Gets or sets Display Name.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets Style.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the selected forum id.
    /// </summary>
    [Ignore]
    public string SelectedForumId { get; set; }
}