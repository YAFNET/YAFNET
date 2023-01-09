/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Core.Identity;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using YAF.Types.Attributes;

/// <summary>
/// The role store.
/// </summary>
public class RoleStore : IQueryableRoleStore<AspNetRoles>,
                         IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleStore"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public RoleStore([NotNull] IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; }

    /// <summary>
    /// The roles.
    /// </summary>
    public virtual IQueryable<AspNetRoles> Roles => this.GetRepository<AspNetRoles>().GetAll().AsQueryable();

    /// <summary>
    /// The create async.
    /// </summary>
    /// <param name="role">
    /// The role.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IdentityResult> CreateAsync([NotNull]AspNetRoles role, CancellationToken cancellationToken)
    {
        CodeContracts.VerifyNotNull(role);

        cancellationToken.ThrowIfCancellationRequested();

        await Task.FromResult(this.GetRepository<AspNetRoles>().Insert(role, false));

        return IdentityResult.Success;
    }

    /// <summary>
    /// The update async.
    /// </summary>
    /// <param name="role">
    /// The role.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IdentityResult> UpdateAsync([NotNull] AspNetRoles role, CancellationToken cancellationToken)
    {
        CodeContracts.VerifyNotNull(role);

        cancellationToken.ThrowIfCancellationRequested();

        await Task.FromResult(this.GetRepository<AspNetRoles>().Update(role));

        return IdentityResult.Success;
    }

    /// <summary>
    /// The delete async.
    /// </summary>
    /// <param name="role">
    /// The role.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<IdentityResult> DeleteAsync([NotNull]AspNetRoles role, CancellationToken cancellationToken)
    {
        CodeContracts.VerifyNotNull(role);

        cancellationToken.ThrowIfCancellationRequested();

        this.GetRepository<AspNetRoles>().Delete(r => r.Id == role.Id);

        return Task.FromResult(IdentityResult.Success);
    }

    /// <summary>
    /// The find by id async.
    /// </summary>
    /// <param name="roleId">
    /// The role id.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<AspNetRoles> FindByIdAsync([NotNull]string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await Task.FromResult(this.GetRepository<AspNetRoles>().GetSingle(r => r.Id == roleId));
    }

    /// <summary>
    /// The find by name async.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<AspNetRoles> FindByNameAsync([NotNull]string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await Task.FromResult(this.GetRepository<AspNetRoles>().GetSingle(r => r.Name == roleName));
    }

    public Task<string> GetRoleIdAsync(AspNetRoles role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id);
    }

    public Task<string> GetRoleNameAsync(AspNetRoles role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetRoleNameAsync(AspNetRoles role, string roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;
        return Task.FromResult(0);
    }

    public Task<string> GetNormalizedRoleNameAsync(AspNetRoles role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetNormalizedRoleNameAsync(AspNetRoles role, string normalizedName, CancellationToken cancellationToken)
    {
        role.Name = normalizedName;
        return Task.FromResult(0);
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public virtual void Dispose()
    {
        // No resource to dispose for now!
    }
}