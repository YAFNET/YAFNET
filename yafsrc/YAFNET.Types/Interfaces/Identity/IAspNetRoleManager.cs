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

namespace YAF.Types.Interfaces.Identity;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using YAF.Types.Models.Identity;

/// <summary>
/// The AspNetRoleManager interface.
/// </summary>
public interface IAspNetRoleManager
{
    /// <summary>
    /// Gets the roles.
    /// </summary>
    IQueryable<AspNetRoles> AspNetRoles { get; }

    /// <summary>
    /// The get roles.
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// Returns List of Role names
    /// </returns>
    Task<IList<string>> GetRolesAsync(AspNetUsers user);

    /// <summary>
    /// The find by name.
    /// </summary>
    /// <param name="roleName">
    ///     The role name.
    /// </param>
    /// <returns>
    /// The <see cref="AspNetRoles"/>.
    /// </returns>
    Task<AspNetRoles> FindByRoleNameAsync(string roleName);

    /// <summary>
    /// Create a role
    /// </summary>
    /// <param name="role">
    ///     The role.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    Task<IdentityResult> CreateRoleAsync(AspNetRoles role);

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="role">
    ///     The role.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    Task<IdentityResult> DeleteRoleAsync(AspNetRoles role);

    /// <summary>
    /// Returns true if the role exists
    /// </summary>
    /// <param name="roleName">
    ///     The role Name.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    Task<bool> RoleNameExistsAsync(string roleName);
}