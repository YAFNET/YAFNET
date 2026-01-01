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

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class UsersInfoInputModel
{
    /// <summary>
    /// Gets or sets the rank identifier.
    /// </summary>
    /// <value>The rank identifier.</value>
    public int RankID { get; set; }

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
    /// Gets or sets the email.
    /// </summary>
    /// <value>The email.</value>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is host admin x.
    /// </summary>
    /// <value><c>true</c> if this instance is host admin x; otherwise, <c>false</c>.</value>
    public bool IsHostAdminX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is approved.
    /// </summary>
    /// <value><c>true</c> if this instance is approved; otherwise, <c>false</c>.</value>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is guest x.
    /// </summary>
    /// <value><c>true</c> if this instance is guest x; otherwise, <c>false</c>.</value>
    public bool IsGuestX { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is excluded from active users.
    /// </summary>
    /// <value><c>true</c> if this instance is excluded from active users; otherwise, <c>false</c>.</value>
    public bool IsExcludedFromActiveUsers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="UsersInfoInputModel"/> is moderated.
    /// </summary>
    /// <value><c>true</c> if moderated; otherwise, <c>false</c>.</value>
    public bool Moderated { get; set; }

    /// <summary>
    /// Gets or sets the joined.
    /// </summary>
    /// <value>The joined.</value>
    public string Joined { get; set; }

    /// <summary>
    /// Gets or sets the last visit.
    /// </summary>
    /// <value>The last visit.</value>
    public string LastVisit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [un disable user].
    /// </summary>
    /// <value><c>true</c> if [un disable user]; otherwise, <c>false</c>.</value>
    public bool UnDisableUser { get; set; } = true;
}