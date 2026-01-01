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

namespace YAF.Types.Models;

using ServiceStack.DataAnnotations;

/// <summary>
/// A class which represents the yaf_PollGroupCluster table. Only used for Dropping
/// </summary>
[Serializable]
public class PollGroupCluster : IEntity, IHaveID
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>The identifier.</value>
    [Alias("PollGroupID")]
    [AutoIncrement]

    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    [Required]
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    /// <value>The flags.</value>
    [Required]
    [Default(0)]
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is bound.
    /// </summary>
    /// <value><c>null</c> if [is bound] contains no value, <c>true</c> if [is bound]; otherwise, <c>false</c>.</value>
    [Compute]
    public bool? IsBound { get; set; }
}