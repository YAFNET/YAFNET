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
///     A class which represents the AccessMask table
/// </summary>
[Serializable]
public class AccessMask : IEntity, IHaveBoardID, IHaveID
{
    /// <summary>
    ///     Gets or sets the access mask id.
    /// </summary>
    [AutoIncrement]
    [Alias("AccessMaskID")]
    public int ID { get; set; }

    /// <summary>
    ///     Gets or sets the board id.
    /// </summary>
    [References(typeof(Board))]
    [Required]
    public int BoardID { get; set; }

    /// <summary>
    ///     Gets or sets the flags.
    /// </summary>
    [Required]
    [Default(0)]
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the access flags.
    /// </summary>
    [Ignore]
    public AccessFlags AccessFlags
    {
        get => new(this.Flags);

        set => this.Flags = value.BitValue;
    }

    /// <summary>
    ///     Gets or sets the name.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the sort order.
    /// </summary>
    [Required]
    [Default(0)]
    public short SortOrder { get; set; }
}