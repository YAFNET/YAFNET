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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Identity;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

/// <summary>
/// The asp net role manager.
/// </summary>
public class AspNetRoleManager : RoleManager<AspNetRoles>, IAspNetRoleManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetRoleManager"/> class.
    /// </summary>
    /// <param name="store">The persistence store the manager will operate over.</param>
    /// <param name="roleValidators">A collection of validators for roles.</param>
    /// <param name="keyNormalizer">The normalizer to use when normalizing role names to keys.</param>
    /// <param name="errors">The <see cref="T:Microsoft.AspNetCore.Identity.IdentityErrorDescriber" /> used to provider error messages.</param>
    /// <param name="logger">The logger used to log messages, warnings and errors.</param>
    public AspNetRoleManager(IRoleStore<AspNetRoles> store,
                             IEnumerable<IRoleValidator<AspNetRoles>> roleValidators,
                             ILookupNormalizer keyNormalizer,
                             IdentityErrorDescriber errors,
                             ILogger<RoleManager<AspNetRoles>> logger)
        : base(store, roleValidators, keyNormalizer, errors, logger)
    {
    }

    /// <summary>
    /// The roles.
    /// </summary>
    public virtual IQueryable<AspNetRoles> AspNetRoles => base.Roles;

    /// <summary>
    /// The get roles.
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// Returns List of Role names
    /// </returns>
    public async Task<IList<string>> GetRolesAsync(AspNetUsers user)
    {
        var roles = await BoardContext.Current.GetRepository<AspNetUserRoles>().GetAsync(r => r.UserId == user.Id);

        var rolesSelected = roles.Select(r => r.RoleId).AsEnumerable().ToList();

        var roleNames = await BoardContext.Current.GetRepository<AspNetRoles>()
                            .GetAsync(r => rolesSelected.Contains(r.Id));

        return [.. roleNames.Select(r => r.Name)];
    }

    /// <summary>
    /// The find by name.
    /// </summary>
    /// <param name="roleName">
    ///     The role name.
    /// </param>
    /// <returns>
    /// The <see cref="AspNetRoles"/>.
    /// </returns>
    public Task<AspNetRoles> FindByRoleNameAsync(string roleName)
    {
        return this.FindByNameAsync(roleName);
    }

    /// <summary>
    /// Create a role
    /// </summary>
    /// <param name="role">
    ///     The role.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public Task<IdentityResult> CreateRoleAsync(AspNetRoles role)
    {
        return this.CreateAsync(role);
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="role">
    ///     The role.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public Task<IdentityResult> DeleteRoleAsync(AspNetRoles role)
    {
        return this.DeleteAsync(role);
    }

    /// <summary>
    /// Returns true if the role exists
    /// </summary>
    /// <param name="roleName">
    ///     The role Name.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public Task<bool> RoleNameExistsAsync(string roleName)
    {
        return this.RoleExistsAsync(roleName);
    }
}