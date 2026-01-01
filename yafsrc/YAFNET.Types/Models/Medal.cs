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
///     A class which represents the Medal table.
/// </summary>
[Serializable]
public class Medal : IEntity, IHaveBoardID, IHaveID
{
    /// <summary>
    /// Gets BoardId.
    /// </summary>
    /// <value>The board identifier.</value>
    [Required]
    public int BoardID { get; set; }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    /// <value>The identifier.</value>
    [Alias("MedalID")]
    [AutoIncrement]
    [PrimaryKey]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    [CustomField(OrmLiteVariables.MaxTextUnicode)]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    [Required]
    [StringLength(100)]
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    /// <value>The category.</value>
    [StringLength(50)]
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the medal URL.
    /// </summary>
    /// <value>The medal URL.</value>
    [Required]
    [StringLength(250)]
    public string MedalURL { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    /// <value>The flags.</value>
    [Required]
    [Default(0)]
    public int Flags { get; set; }

    /// <summary>
    /// Gets or sets the medal flags.
    /// </summary>
    /// <value>The medal flags.</value>
    [Ignore]
    public MedalFlags MedalFlags
    {
        get => new(this.Flags);

        set => this.Flags = value.BitValue;
    }
}