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
///     A class which represents the PollVote table.
/// </summary>
[Serializable]
public class PollVote : IEntity, IHaveID
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [Alias("PollVoteID")]
    [AutoIncrement]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the poll id.
    /// </summary>
    [References(typeof(Poll))]
    [Required]
    [Index]
    public int PollID { get; set; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [Index]
    public int? UserID { get; set; }

    /// <summary>
    /// Gets or sets the choice id.
    /// </summary>
    public int? ChoiceID { get; set; }
}