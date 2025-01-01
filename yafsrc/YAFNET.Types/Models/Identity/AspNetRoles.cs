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

namespace YAF.Types.Models.Identity;

using ServiceStack.Model;

/// <summary>
/// The asp net roles.
/// </summary>
public class AspNetRoles : AspNetRoles<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetRoles"/> class.
    /// </summary>
    public AspNetRoles()
    {
        this.Id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetRoles"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    public AspNetRoles(string name)
        : base(Guid.NewGuid().ToString(), name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetRoles"/> class.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    public AspNetRoles(string id, string name)
        : base(id, name)
    {
    }
}

/// <summary>
/// The asp net roles.
/// </summary>
/// <typeparam name="TKey">
/// </typeparam>
public class AspNetRoles<TKey> : IEntity, IHasId<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetRoles{TKey}"/> class.
    /// Default constructor for Role
    /// </summary>
    public AspNetRoles()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetRoles{TKey}"/> class.
    /// Constructor that takes names as argument
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    public AspNetRoles(string name)
        : this()
    {
        this.Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetRoles{TKey}"/> class.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    public AspNetRoles(TKey id, string name) : this(name)
    {
        this.Id = id;
    }

    /// <summary>
    /// Gets or sets the Role ID
    /// </summary>
    [StringLength(56)]
    public TKey Id { get; set; }

    /// <summary>
    /// Gets or sets the Role name
    /// </summary>
    [StringLength(50)]
    [Index(Unique = true)]
    [Required]
    public string Name { get; set; }
}