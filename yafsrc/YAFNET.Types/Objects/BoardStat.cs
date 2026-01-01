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
/// Class BoardStat.
/// </summary>
public class BoardStat
{
    /// <summary>
    /// Gets or sets the categories.
    /// </summary>
    /// <value>The categories.</value>
    public int Categories { get; set; }

    /// <summary>
    /// Gets or sets the posts.
    /// </summary>
    /// <value>The posts.</value>
    public int Posts { get; set; }

    /// <summary>
    /// Gets or sets the topics.
    /// </summary>
    /// <value>The topics.</value>
    public int Topics { get; set; }

    /// <summary>
    /// Gets or sets the forums.
    /// </summary>
    /// <value>The forums.</value>
    public int Forums { get; set; }

    /// <summary>
    /// Gets or sets the last user style.
    /// </summary>
    /// <value>The last user style.</value>
    public DateTime? LastPost { get; set; }

    /// <summary>
    /// Gets or sets the last user identifier.
    /// </summary>
    /// <value>The last user identifier.</value>
    public int? LastUserID { get; set; }

    /// <summary>
    /// Gets or sets the last user.
    /// </summary>
    /// <value>The last user.</value>
    public string LastUser { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user display.
    /// </summary>
    /// <value>The last name of the user display.</value>
    public string LastUserDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the last user style.
    /// </summary>
    /// <value>The last user style.</value>
    public string LastUserStyle { get; set; }

    /// <summary>
    /// Gets or sets the last user suspended.
    /// </summary>
    /// <value>The last user suspended.</value>
    public DateTime? LastUserSuspended { get; set; }

    /// <summary>
    /// Gets or sets the users.
    /// </summary>
    /// <value>The users.</value>
    public int Users { get; set; }

    /// <summary>
    /// Gets or sets the board start.
    /// </summary>
    /// <value>The board start.</value>
    public DateTime BoardStart { get; set; }
}