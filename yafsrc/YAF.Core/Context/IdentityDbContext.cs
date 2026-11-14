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

namespace YAF.Core.Context;

using System;

using YAF.Types.Models.Identity;

/// <summary>
/// The identity db context.
/// </summary>
public class IdentityDbContext : IdentityDbContext<AspNetUsers<string>, AspNetRoles<string>>
{
}

/// <summary>
/// The identity db context.
/// </summary>
/// <typeparam name="TUser">
/// </typeparam>
/// <typeparam name="TRole">
/// </typeparam>
public class IdentityDbContext<TUser, TRole> : IdentityDbContext<TUser, TRole, string, string>
    where TUser : AspNetUsers<string>
    where TRole : AspNetRoles<string>
{
}

/// <summary>
/// The identity db context.
/// </summary>
/// <typeparam name="TUser">
/// </typeparam>
/// <typeparam name="TRole">
/// </typeparam>
/// <typeparam name="TKey">
/// </typeparam>
/// <typeparam name="TRoleKey">
/// </typeparam>
#pragma warning disable S2326 // the type parameters are part of the public API (used in the constraints)
public class IdentityDbContext<TUser, TRole, TKey, TRoleKey> : IDisposable
#pragma warning restore S2326
    where TUser : AspNetUsers<TKey>
    where TRole : AspNetRoles<TRoleKey>
{
    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources (there are none for now).
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release managed resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        // no resources to release
    }
}