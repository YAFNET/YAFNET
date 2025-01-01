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

namespace YAF.Types.Models;

/// <summary>
/// A class which represents the Buddy table.
/// </summary>
[Serializable]
[UniqueConstraint(nameof(FromUserID), nameof(ToUserID))]
public class Buddy : IEntity, IHaveID
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [AutoIncrement]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets from user identifier.
    /// </summary>
    /// <value>
    /// From user identifier.
    /// </value>
    [References(typeof(User))]
    [Required]
    [Index]
    public int FromUserID { get; set; }

    /// <summary>
    /// Gets or sets to user identifier.
    /// </summary>
    /// <value>
    /// To user identifier.
    /// </value>
    [Required]
    [Index]
    public int ToUserID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Buddy"/> is approved.
    /// </summary>
    /// <value>
    ///   <c>true</c> if approved; otherwise, <c>false</c>.
    /// </value>
    [Required]
    public bool Approved { get; set; }

    /// <summary>
    /// Gets or sets the requested.
    /// </summary>
    /// <value>
    /// The requested.
    /// </value>
    [Required]
    public DateTime Requested { get; set; }
}