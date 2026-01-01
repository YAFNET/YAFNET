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

namespace YAF.Types.Models;

/// <summary>
/// A class which represents the DeviceSubscription table.
/// </summary>
[Serializable]
[CompositePrimaryKey(nameof(UserID), nameof(Device))]
public class DeviceSubscription : IEntity, IHaveBoardID
{
    /// <summary>
    /// Gets or sets the device (stripped user agent).
    /// </summary>
    /// <value>
    /// The device.
    /// </value>
    [Required]
    public string Device { get; set; }

    /// <summary>
    /// Gets or sets the user agent.
    /// </summary>
    /// <value>
    /// The user agent.
    /// </value>
    [Required]
    public string UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [References(typeof(User))]
    [Required]
    [Index]
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the board id.
    /// </summary>
    [References(typeof(Board))]
    [Required]
    [Index]
    public int BoardID { get; set; }

    /// <summary>
    /// Gets or sets the end point.
    /// </summary>
    /// <value>
    /// The end point.
    /// </value>
    [Required]
    public string EndPoint { get; set; }

    /// <summary>
    /// Gets or sets the P256DH.
    /// </summary>
    /// <value>
    /// The P256DH.
    /// </value>
    [Required]
    public string P256dh { get; set; }

    /// <summary>
    /// Gets or sets the authentication.
    /// </summary>
    /// <value>
    /// The authentication.
    /// </value>
    [Required]
    public string Auth { get; set; }
}