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
/// Class GroupMember.
/// </summary>
public class GroupMember
{
    /// <summary>
    /// Gets or sets the group identifier.
    /// </summary>
    /// <value>The group identifier.</value>
    public int GroupID { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    public int? UserID { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is member.
    /// </summary>
    /// <value><c>true</c> if this instance is member; otherwise, <c>false</c>.</value>
    [Ignore]
    public bool IsMember => this.UserID.HasValue;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="GroupMember"/> is selected.
    /// </summary>
    /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
    [Ignore]
    public bool Selected { get; set; }
}