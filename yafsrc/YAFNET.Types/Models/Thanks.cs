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

using YAF.Types.Constants;

namespace YAF.Types.Models;

/// <summary>
///     A class which represents the Thanks table in the Database.
/// </summary>
[Serializable]
public class Thanks : IEntity, IHaveID
{
    /// <summary>
    ///     Gets or sets the Thanks id.
    /// </summary>
    [AutoIncrement]
    [Alias("ThanksID")]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the thanks from user id.
    /// </summary>
    [References(typeof(User))]
    [Required]
    [Index]
    public int ThanksFromUserID { get; set; }

    /// <summary>
    /// Gets or sets the thanks to user id.
    /// </summary>
    [Required]
    [Index]
    public int ThanksToUserID { get; set; }

    /// <summary>
    /// Gets or sets the message id.
    /// </summary>
    [Required]
    [Index]
    public int MessageID { get; set; }

    /// <summary>
    /// Gets or sets the thanks date.
    /// </summary>
    [Required]
    public DateTime ThanksDate { get; set; }

    /// <summary>
    /// Gets or sets the reaction (see <see cref="ReactionType"/>), the thanks given before the reactions are a love.
    /// </summary>
    [Required]
    [Default(2)]
    public int Reaction { get; set; } = (int)ReactionType.Love;
}