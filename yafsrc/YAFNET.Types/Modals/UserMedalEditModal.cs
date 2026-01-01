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

namespace YAF.Types.Modals;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Class UserMedalEditModal.
/// </summary>
public class UserMedalEditModal
{
    /// <summary>
    /// Gets or sets the medal identifier.
    /// </summary>
    /// <value>The medal identifier.</value>
    public int MedalId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    public int UserID { get; set; }

    //[Required]
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the user message.
    /// </summary>
    /// <value>The user message.</value>
    [StringLength(100)]
    public string UserMessage { get; set; }

    /// <summary>
    /// Gets or sets the user sort order.
    /// </summary>
    /// <value>The user sort order.</value>
    public int UserSortOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [user hide].
    /// </summary>
    /// <value><c>true</c> if [user hide]; otherwise, <c>false</c>.</value>
    public bool UserHide { get; set; }

    /// <summary>
    /// Gets or sets the name of the medal.
    /// </summary>
    /// <value>The name of the medal.</value>
    public string MedalName { get; set; }

    /// <summary>
    /// Gets or sets the request verification token.
    /// </summary>
    /// <value>The request verification token.</value>
    public string __RequestVerificationToken { get; set; }
}