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
/// A class which represents the MessageHistory table.
/// </summary>
[Serializable]
[CompositePrimaryKey(nameof(MessageID), nameof(Edited))]
public class MessageHistory : IEntity
{
    /// <summary>
    /// Gets or sets the message id.
    /// </summary>
    [References(typeof(Message))]
    [Required]
    public int MessageID { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    [CustomField(OrmLiteVariables.MaxTextUnicode)]
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the IP Address.
    /// </summary>
    [Required]
    [StringLength(39)]
    public string IP { get; set; }

    /// <summary>
    /// Gets or sets the edited.
    /// </summary>
    [Required]
    public DateTime Edited { get; set; }

    /// <summary>
    /// Gets or sets the edited by.
    /// </summary>
    public int? EditedBy { get; set; }

    /// <summary>
    /// Gets or sets the edit reason.
    /// </summary>
    [StringLength(100)]
    public string EditReason { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is moderator changed.
    /// </summary>
    [Required]
    [Default(typeof(bool), "0")]
    public bool IsModeratorChanged { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    [Required]
    [Default(23)]
    public int Flags { get; set; }
}