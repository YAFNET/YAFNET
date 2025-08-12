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

namespace YAF.Types.Objects.Model;

/// <summary>
/// Class ActiveUser.
/// </summary>
public class ActiveUser
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the display name of the user.
    /// </summary>
    /// <value>The display name of the user.</value>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is active excluded.
    /// </summary>
    /// <value><c>true</c> if this instance is active excluded; otherwise, <c>false</c>.</value>
    public bool IsActiveExcluded { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    /// <value>The flags.</value>
    public int Flags { get; set; }

    /// <summary>
    /// Gets the active flags.
    /// </summary>
    /// <value>The active flags.</value>
    [Ignore]
    public ActiveFlags ActiveFlags => new(this.Flags);

    /// <summary>
    /// Gets or sets the user style.
    /// </summary>
    /// <value>The user style.</value>
    public string UserStyle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is guest.
    /// </summary>
    /// <value><c>true</c> if this instance is guest; otherwise, <c>false</c>.</value>
    public bool IsGuest { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    /// <value>The suspended.</value>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the user count.
    /// </summary>
    /// <value>The user count.</value>
    public int UserCount { get; set; }

    /// <summary>
    /// Gets or sets the browser.
    /// </summary>
    /// <value>The browser.</value>
    public string Browser { get; set; }

    /// <summary>
    /// Gets or sets the last visit.
    /// </summary>
    /// <value>The last visit.</value>
    public DateTime LastVisit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance has forum access.
    /// </summary>
    /// <value><c>true</c> if this instance has forum access; otherwise, <c>false</c>.</value>
    public bool HasForumAccess { get; set; }

    /// <summary>
    /// Gets or sets the forum page.
    /// </summary>
    /// <value>The forum page.</value>
    public string ForumPage { get; set; }

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    /// <value>The location.</value>
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets the referer.
    /// </summary>
    /// <value>The referer.</value>
    public string Referer { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    /// <value>The country.</value>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the forum identifier.
    /// </summary>
    /// <value>The forum identifier.</value>
    public int? ForumID { get; set; }

    /// <summary>
    /// Gets or sets the name of the forum.
    /// </summary>
    /// <value>The name of the forum.</value>
    public string ForumName { get; set; }

    /// <summary>
    /// Gets or sets the topic identifier.
    /// </summary>
    /// <value>The topic identifier.</value>
    public int? TopicID { get; set; }

    /// <summary>
    /// Gets or sets the name of the topic.
    /// </summary>
    /// <value>The name of the topic.</value>
    public string TopicName { get; set; }

    /// <summary>
    /// Gets or sets the login.
    /// </summary>
    /// <value>The login.</value>
    public DateTime Login { get; set; }

    /// <summary>
    /// Gets or sets the last active.
    /// </summary>
    /// <value>The last active.</value>
    public DateTime LastActive { get; set; }

    /// <summary>
    /// Gets or sets the active.
    /// </summary>
    /// <value>The active.</value>
    public int Active { get; set; }

    /// <summary>
    /// Gets or sets the platform.
    /// </summary>
    /// <value>The platform.</value>
    public string Platform { get; set; }

    /// <summary>
    /// Gets or sets the user agent.
    /// </summary>
    /// <value>The user agent.</value>
    public string UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the ip.
    /// </summary>
    /// <value>The ip.</value>
    public string IP { get; set; }

    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    /// <value>The session identifier.</value>
    public string SessionID { get; set; }
}