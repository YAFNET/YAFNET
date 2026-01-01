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

namespace YAF.Types.Objects;

/// <summary>
/// The user lazy data.
/// </summary>
public class UserLazyData
{
    /// <summary>
    /// Gets or sets the provider user key.
    /// </summary>
    public string ProviderUserKey { get; set; }

    /// <summary>
    /// Gets or sets the suspended.
    /// </summary>
    public DateTime? Suspended { get; set; }

    /// <summary>
    /// Gets or sets the suspended reason.
    /// </summary>
    public string SuspendedReason { get; set; }

    /// <summary>
    /// Gets or sets the time zone user.
    /// </summary>
    public string TimeZoneUser { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is guest.
    /// </summary>
    public bool IsGuest { get; set; }

    /// <summary>
    /// Gets or sets the moderate posts.
    /// </summary>
    public int ModeratePosts { get; set; }

    /// <summary>
    /// Gets or sets the watch topic.
    /// </summary>
    public int WatchTopic { get; set; }

    /// <summary>
    /// Gets or sets the received thanks.
    /// </summary>
    public int ReceivedThanks { get; set; }

    /// <summary>
    /// Gets or sets the mention.
    /// </summary>
    public int Mention { get; set; }

    /// <summary>
    /// Gets or sets the quoted.
    /// </summary>
    public int Quoted { get; set; }

    /// <summary>
    /// Gets or sets the unread private.
    /// </summary>
    public int UnreadPrivate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance has private conversations.
    /// </summary>
    /// <value><c>true</c> if this instance has private conversations; otherwise, <c>false</c>.</value>
    public bool UserHasPrivateConversations { get; set; }

    /// <summary>
    /// Gets or sets the pending buddies.
    /// </summary>
    public int PendingBuddies { get; set; }

    /// <summary>
    /// Gets or sets the last pending buddies.
    /// </summary>
    public DateTime LastPendingBuddies { get; set; }

    /// <summary>
    /// Gets or sets the number of albums.
    /// </summary>
    public int NumAlbums { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether user has buddies.
    /// </summary>
    public bool UserHasBuddies { get; set; }
}