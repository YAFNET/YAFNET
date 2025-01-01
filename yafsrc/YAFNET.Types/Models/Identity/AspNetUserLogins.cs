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

/// <summary>
/// The asp net user logins.
/// </summary>
public class AspNetUserLogins : AspNetUserLogins<string>;

/// <summary>
/// The asp net user logins.
/// </summary>
/// <typeparam name="TKey">
/// </typeparam>
[CompositePrimaryKey(nameof(UserId), nameof(LoginProvider), nameof(ProviderKey))]
public class AspNetUserLogins<TKey> : IEntity
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [StringLength(56)]
    [Index(NonClustered = true)]
    [ForeignKey(typeof(AspNetUsers), OnDelete = "CASCADE")]
    [Required]
    public TKey UserId { get; set; }

    /// <summary>
    /// Gets or sets the login provider.
    /// </summary>
    [StringLength(128)]
    [Required]
    public string LoginProvider { get; set; }

    /// <summary>
    /// Gets or sets the provider key.
    /// </summary>
    [StringLength(128)]
    [Required]
    public string ProviderKey { get; set; }
}