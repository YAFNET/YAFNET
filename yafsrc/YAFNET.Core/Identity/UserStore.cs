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

namespace YAF.Core.Identity;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

/// <summary>
/// The user store.
/// </summary>
public class UserStore : IUserEmailStore<AspNetUsers>,
                         IUserPhoneNumberStore<AspNetUsers>,
                         IUserTwoFactorStore<AspNetUsers>,
                         IUserPasswordStore<AspNetUsers>,
                         IUserRoleStore<AspNetUsers>,
                         IQueryableUserStore<AspNetUsers>,
                         IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserStore"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public UserStore(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; }

    /// <summary>
    /// The users.
    /// </summary>
    public virtual IQueryable<AspNetUsers> Users => this.GetRepository<AspNetUsers>().GetAll().AsQueryable();

    /// <summary>
    /// The add login async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="login">
    /// The login.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task AddLoginAsync(AspNetUsers user, UserLoginInfo login)
    {
        var userLogin = new AspNetUserLogins
                            {
                                UserId = user.Id, ProviderKey = login.ProviderKey, LoginProvider = login.LoginProvider
                            };

        return this.GetRepository<AspNetUserLogins>().InsertAsync(userLogin, false);
    }

    /// <summary>
    /// The find async.
    /// </summary>
    /// <param name="login">
    /// The login.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async virtual Task<AspNetUsers> FindAsync(UserLoginInfo login)
    {
        var userLogin = await this.GetRepository<AspNetUserLogins>().GetSingleAsync(
                            i => i.LoginProvider == login.LoginProvider && i.ProviderKey == login.ProviderKey);

        if (userLogin == null)
        {
            return null;
        }

        return await this.GetRepository<AspNetUsers>().GetSingleAsync(u => u.Id == userLogin.UserId);
    }

    /// <summary>
    /// The get logins async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async virtual Task<IList<UserLoginInfo>> GetLoginsAsync(AspNetUsers user)
    {
        var logins = await this.GetRepository<AspNetUserLogins>().GetAsync(l => l.UserId == user.Id);

        IList<UserLoginInfo> result =
            [.. logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, user.UserName))];
        return result;
    }

    /// <summary>
    /// The remove login async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="login">
    /// The login.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task RemoveLoginAsync(AspNetUsers user, UserLoginInfo login)
    {
        return Task.FromResult(
            this.GetRepository<AspNetUserLogins>().DeleteAsync(
                l => l.UserId == user.Id && l.ProviderKey == login.ProviderKey
                                         && l.LoginProvider == login.LoginProvider));
    }

    /// <summary>
    /// The create async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IdentityResult> CreateAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await this.GetRepository<AspNetUsers>().InsertAsync(user, false, token: cancellationToken);

        return IdentityResult.Success;
    }

    /// <summary>
    /// The delete async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IdentityResult> DeleteAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await this.GetRepository<AspNetUsers>().DeleteAsync(u => u.Id == user.Id, token: cancellationToken);

        return IdentityResult.Success;
    }

    /// <summary>
    /// The find by id async.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<AspNetUsers> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return this.GetRepository<AspNetUsers>().GetSingleAsync(
            u => u.Id == userId,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// The find by name async.
    /// </summary>
    /// <param name="normalizedUserName">
    /// The user name.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<AspNetUsers> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return this.GetRepository<AspNetUsers>().GetSingleAsync(
            u => u.UserName.ToLower(CultureInfo.InvariantCulture) == normalizedUserName.ToLower(CultureInfo.InvariantCulture),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// The update async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IdentityResult> UpdateAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await this.UpdateUserAsync(user, cancellationToken);

        return IdentityResult.Success;
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public virtual void Dispose()
    {
    }

    /// <summary>
    /// The add claim async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="claim">
    /// The claim.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task AddClaimAsync(AspNetUsers user, Claim claim)
    {
        var userClaim =
            new AspNetUserClaims { UserId = user.Id, ClaimType = claim.ValueType, ClaimValue = claim.Value };

        this.UpdateUserAsync(user);

        return this.GetRepository<AspNetUserClaims>().InsertAsync(userClaim, false);
    }

    /// <summary>
    /// The get claims async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async virtual Task<IList<Claim>> GetClaimsAsync(AspNetUsers user)
    {
        var claims = await this.GetRepository<AspNetUserClaims>().GetAsync(l => l.UserId == user.Id);

        IList<Claim> result = [.. claims.Select(c => new Claim(c.ClaimType, c.ClaimValue))];
        return result;
    }

    /// <summary>
    /// The remove claim async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="claim">
    /// The claim.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task RemoveClaimAsync(AspNetUsers user, Claim claim)
    {
        var result = this.GetRepository<AspNetUserClaims>().DeleteAsync(
            c => c.UserId == user.Id && c.ClaimValue == claim.Value && c.ClaimType == claim.Type);

        this.UpdateUserAsync(user);

        return Task.FromResult(result);
    }

    /// <summary>
    /// The add to role async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task AddToRoleAsync(AspNetUsers user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = await this.GetRepository<AspNetRoles>().GetSingleAsync(
                       r => r.Name == roleName,
                       cancellationToken: cancellationToken);

        if (role == null)
        {
            return;
        }

        var newUserRole = new AspNetUserRoles { RoleId = role.Id, UserId = user.Id };

        await this.GetRepository<AspNetUserRoles>().InsertAsync(newUserRole, false, token: cancellationToken);

        await this.UpdateUserAsync(user, cancellationToken);
    }

    /// <summary>
    /// The get roles async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IList<string>> GetRolesAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userRoles = await this.GetRepository<AspNetUserRoles>().GetAsync(
                            r => r.UserId == user.Id,
                            cancellationToken: cancellationToken);

        var rolesSelected = userRoles.Select(r => r.RoleId).AsEnumerable().ToList();

        var roles = await this.GetRepository<AspNetRoles>().GetAsync(
                        r => rolesSelected.Contains(r.Id),
                        cancellationToken: cancellationToken);

        return [.. roles.Select(r => r.Name)];
    }

    /// <summary>
    /// The is in role async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<bool> IsInRoleAsync(AspNetUsers user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = await this.GetRepository<AspNetRoles>().GetSingleAsync(
                       r => r.Name == roleName,
                       cancellationToken: cancellationToken);

        var isInRole = false;

        if (role != null)
        {
            isInRole = this.GetRepository<AspNetUserRoles>().Count(r => r.RoleId == role.Id && r.UserId == user.Id) > 0;
        }

        return isInRole;
    }

    /// <summary>
    /// Get users in role as an asynchronous operation.
    /// </summary>
    /// <param name="roleName">The name of the role whose membership should be returned.</param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A Task&lt;IList`1&gt; representing the asynchronous operation.</returns>
    public async Task<IList<AspNetUsers>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await this.Get<AspNetRolesHelper>().GetUsersInRoleAsync(roleName);
    }

    /// <summary>
    /// The remove from role async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task RemoveFromRoleAsync(AspNetUsers user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = await this.GetRepository<AspNetRoles>().GetSingleAsync(
                       r => r.Name == roleName,
                       cancellationToken: cancellationToken);

        if (role == null)
        {
            return;
        }

        await this.GetRepository<AspNetUserRoles>().DeleteAsync(
            r => r.UserId == user.Id && r.RoleId == role.Id,
            token: cancellationToken);

        await this.UpdateUserAsync(user, cancellationToken);
    }

    /// <summary>
    /// The get password hash async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<string> GetPasswordHashAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    /// <summary>
    /// The has password async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<bool> HasPasswordAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash.IsSet());
    }

    /// <summary>
    /// The set password hash async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="passwordHash">
    /// The password hash.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task SetPasswordHashAsync(AspNetUsers user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        this.UpdateUserAsync(user, cancellationToken);

        return Task.FromResult(0);
    }

    /// <summary>
    /// The get security stamp async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task<string> GetSecurityStampAsync(AspNetUsers user)
    {
        return Task.FromResult(user.SecurityStamp);
    }

    /// <summary>
    /// The set security stamp async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="stamp">
    /// The stamp.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task SetSecurityStampAsync(AspNetUsers user, string stamp)
    {
        user.SecurityStamp = stamp;
        return this.UpdateUserAsync(user);
    }

    /// <summary>
    /// The find by email async.
    /// </summary>
    /// <param name="normalizedEmail">
    /// The email.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<AspNetUsers> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return this.GetRepository<AspNetUsers>().GetSingleAsync(
            u => u.Email.ToLower(CultureInfo.InvariantCulture) == normalizedEmail.ToLower(CultureInfo.InvariantCulture),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// The get email async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<string> GetEmailAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    /// <summary>
    /// The get email confirmed async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<bool> GetEmailConfirmedAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    /// <summary>
    /// The set email async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task SetEmailAsync(AspNetUsers user, string email, CancellationToken cancellationToken)
    {
        user.Email = email;

        return this.UpdateUserAsync(user, cancellationToken);
    }

    /// <summary>
    /// The set email confirmed async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="confirmed">
    /// The confirmed.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task SetEmailConfirmedAsync(AspNetUsers user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;

        return this.UpdateUserAsync(user, cancellationToken);
    }

    /// <summary>
    /// The get phone number async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<string> GetPhoneNumberAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumber);
    }

    /// <summary>
    /// The get phone number confirmed async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<bool> GetPhoneNumberConfirmedAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    /// <summary>
    /// The set phone number async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="phoneNumber">
    /// The phone number.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task SetPhoneNumberAsync(AspNetUsers user, string phoneNumber, CancellationToken cancellationToken)
    {
        user.PhoneNumber = phoneNumber;

        return this.UpdateUserAsync(user, cancellationToken);
    }

    /// <summary>
    /// The set phone number confirmed async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="confirmed">
    /// The confirmed.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task SetPhoneNumberConfirmedAsync(AspNetUsers user, bool confirmed, CancellationToken cancellationToken)
    {
        user.PhoneNumberConfirmed = confirmed;

        return this.UpdateUserAsync(user, cancellationToken);
    }

    /// <summary>
    /// The get two factor enabled async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<bool> GetTwoFactorEnabledAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.TwoFactorEnabled);
    }

    /// <summary>
    /// The set two factor enabled async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task SetTwoFactorEnabledAsync(AspNetUsers user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;

        return this.UpdateUserAsync(user, cancellationToken);
    }

    /// <summary>
    /// The get access failed count async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task<int> GetAccessFailedCountAsync(AspNetUsers user)
    {
        return Task.FromResult(user.AccessFailedCount);
    }

    /// <summary>
    /// The get lockout enabled async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task<bool> GetLockoutEnabledAsync(AspNetUsers user)
    {
        return Task.FromResult(user.LockoutEnabled);
    }

    /// <summary>
    /// The get lockout end date async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(AspNetUsers user)
    {
        return Task.FromResult(
            user.LockoutEndDateUtc.HasValue
                ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                : new DateTimeOffset());
    }

    /// <summary>
    /// The increment access failed count async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async virtual Task<int> IncrementAccessFailedCountAsync(AspNetUsers user)
    {
        user.AccessFailedCount++;

        await this.UpdateUserAsync(user);

        return user.AccessFailedCount;
    }

    /// <summary>
    /// The reset access failed count async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task ResetAccessFailedCountAsync(AspNetUsers user)
    {
        user.AccessFailedCount = 0;
        return this.UpdateUserAsync(user);
    }

    /// <summary>
    /// The set lockout enabled async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task SetLockoutEnabledAsync(AspNetUsers user, bool enabled)
    {
        user.LockoutEnabled = enabled;
        return this.UpdateUserAsync(user);
    }

    /// <summary>
    /// The set lockout end date async.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="lockoutEnd">
    /// The lockout end.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public virtual Task SetLockoutEndDateAsync(AspNetUsers user, DateTimeOffset lockoutEnd)
    {
        user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
        return this.UpdateUserAsync(user);
    }

    /// <summary>
    /// Gets the user identifier for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose identifier should be retrieved.</param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user" />.</returns>
    public Task<string> GetUserIdAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    /// <summary>
    /// Gets the user name for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose name should be retrieved.</param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the name for the specified <paramref name="user" />.</returns>
    public Task<string> GetUserNameAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    /// <summary>
    /// Gets the normalized user name for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose normalized name should be retrieved.</param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the normalized user name for the specified <paramref name="user" />.</returns>
    public Task<string> GetNormalizedUserNameAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.LoweredUserName);
    }

    /// <summary>
    /// Sets the given normalized name for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose name should be set.</param>
    /// <param name="normalizedName">The normalized name to set.</param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    public Task SetNormalizedUserNameAsync(AspNetUsers user, string normalizedName, CancellationToken cancellationToken)
    {
        user.LoweredUserName = normalizedName;
        return Task.FromResult(0);
    }

    /// <summary>
    /// Sets the given <paramref name="userName" /> for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose name should be set.</param>
    /// <param name="userName">The user name to set.</param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    public Task SetUserNameAsync(AspNetUsers user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.FromResult(0);
    }

    /// <summary>
    /// Returns the normalized email for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose email address to retrieve.</param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The task object containing the results of the asynchronous lookup operation, the normalized email address if any associated with the specified user.</returns>
    public Task<string> GetNormalizedEmailAsync(AspNetUsers user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.LoweredEmail);
    }

    /// <summary>
    /// Sets the normalized email for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose email address to set.</param>
    /// <param name="normalizedEmail">The normalized email to set for the specified <paramref name="user" />.</param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public Task SetNormalizedEmailAsync(AspNetUsers user, string normalizedEmail, CancellationToken cancellationToken)
    {
        user.LoweredEmail = normalizedEmail;
        return Task.FromResult(0);
    }

    /// <summary>
    /// The update user.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    private Task UpdateUserAsync(AspNetUsers user, CancellationToken cancellationToken = default)
    {
        return this.GetRepository<AspNetUsers>().UpdateAsync(user, token: cancellationToken);
    }
}