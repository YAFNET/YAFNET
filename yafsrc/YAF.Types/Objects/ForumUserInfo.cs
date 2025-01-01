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

namespace YAF.Types.Objects;

/// <summary>
/// The Forum User Info
/// </summary>
[Serializable]
public class ForumUserInfo
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the real name.
    /// </summary>
    /// <value>
    /// The real name.
    /// </value>
    public string RealName { get; set; }

    /// <summary>
    /// Gets or sets the avatar.
    /// </summary>
    /// <value>
    /// The avatar.
    /// </value>
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the interests.
    /// </summary>
    /// <value>
    /// The interests.
    /// </value>
    public string Interests { get; set; }

    /// <summary>
    /// Gets or sets the home page.
    /// </summary>
    /// <value>
    /// The home page.
    /// </value>
    public string HomePage { get; set; }

    /// <summary>
    /// Gets or sets the profile link.
    /// </summary>
    /// <value>
    /// The profile link.
    /// </value>
    public string ProfileLink { get; set; }

    /// <summary>
    /// Gets or sets the posts.
    /// </summary>
    /// <value>
    /// The posts.
    /// </value>
    public string Posts { get; set; }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    /// <value>
    /// The points.
    /// </value>
    public string Points { get; set; }

    /// <summary>
    /// Gets or sets the tank.
    /// </summary>
    /// <value>
    /// The tank.
    /// </value>
    public string Rank { get; set; }

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    /// <value>
    /// The location.
    /// </value>
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets the joined.
    /// </summary>
    /// <value>
    /// The joined.
    /// </value>
    public string Joined { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ForumUserInfo" /> is online.
    /// </summary>
    /// <value>
    ///   <c>true</c> if online; otherwise, <c>false</c>.
    /// </value>
    public bool Online { get; set; }

    /// <summary>
    /// Gets or sets the action buttons.
    /// </summary>
    /// <value>
    /// The action buttons.
    /// </value>
    public string ActionButtons { get; set; }
}