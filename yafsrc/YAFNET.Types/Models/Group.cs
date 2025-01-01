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
/// A class which represents the Group table.
/// </summary>
[Serializable]
[UniqueConstraint(nameof(BoardID), nameof(Name))]
public class Group : IEntity, IHaveID, IHaveBoardID
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [AutoIncrement]
    [Alias("GroupID")]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the board id.
    /// </summary>
    [References(typeof(Board))]
    [Required]
    public int BoardID { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the group flags.
    /// </summary>
    [Ignore]
    public GroupFlags GroupFlags
    {
        get => new(this.Flags);

        set => this.Flags = value.BitValue;
    }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    [Required]
    [Default(0)]
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the style.
    /// </summary>
    [Index]
    [StringLength(255)]
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    [Required]
    [Index]
    [Default(0)]
    public short SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [StringLength(128)]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the usr sig chars.
    /// </summary>
    [Required]
    [Default(0)]
    public int UsrSigChars { get; set; }

    /// <summary>
    /// Gets or sets the user sig bb codes.
    /// </summary>
    [StringLength(255)]
    public string UsrSigBBCodes { get; set; }

    /// <summary>
    /// Gets or sets the user albums.
    /// </summary>
    [Required]
    [Default(0)]
    public int UsrAlbums { get; set; }

    /// <summary>
    /// Gets or sets the user album images.
    /// </summary>
    [Required]
    [Default(0)]
    public int UsrAlbumImages { get; set; }
}