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

 * http://www.apache.org/licenses/LICENSE-2.0

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
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using YAF.Core.Membership;

/// <summary>
/// The asp net users manager.
/// </summary>
public class AspNetUsersManager : UserManager<AspNetUsers>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetUsersManager"/> class.
    /// </summary>
    /// <param name="store">
    /// The store.
    /// </param>
    /// <param name="optionsAccessor">
    /// The options Accessor.
    /// </param>
    /// <param name="passwordHasher">
    /// The password Hasher.
    /// </param>
    /// <param name="userValidators">
    /// The user Validators.
    /// </param>
    /// <param name="passwordValidators">
    /// The password Validators.
    /// </param>
    /// <param name="keyNormalizer">
    /// The key Normalizer.
    /// </param>
    /// <param name="errors">
    /// The errors.
    /// </param>
    /// <param name="services">
    /// The services.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public AspNetUsersManager(
        IUserStore<AspNetUsers> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<AspNetUsers> passwordHasher,
        IEnumerable<IUserValidator<AspNetUsers>> userValidators,
        IEnumerable<IPasswordValidator<AspNetUsers>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
            ILogger<UserManager<AspNetUsers>> logger)
        : base(
            store,
            optionsAccessor,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger)
    {
        this.PasswordHasher = new SQLPasswordHasher();
    }

    /// <summary>
    /// The users.
    /// </summary>
    public virtual IQueryable<AspNetUsers> AspNetUsers => base.Users;

    /// <summary>
    /// Remove a user from a role.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="role">
    /// The role.
    /// </param>
    /// <returns>
    /// The <see cref="Microsoft.AspNetCore.Identity.IdentityResult"/>.
    /// </returns>
    public IdentityResult RemoveFromRole(AspNetUsers user, string role)
    {
        return this.RemoveFromRoleAsync(user, role).Result;
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public IdentityResult Delete(AspNetUsers user)
    {
        return this.DeleteAsync(user).Result;
    }

    /// <summary>
    /// Find a user by id
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// The <see cref="AspNetUsers"/>.
    /// </returns>
    public AspNetUsers FindById(string userId)
    {
        return this.FindByIdAsync(userId).Result;
    }

    /// <summary>
    /// Returns the roles for the user
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="IList"/>.
    /// </returns>
    public IList<string> GetRoles(AspNetUsers user)
    {
        return this.GetRolesAsync(user).Result;
    }

    /// <summary>
    /// Returns true if the user is in the specified role
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="role">
    /// The role.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool IsInRole(AspNetUsers user, string role)
    {
        return this.IsInRoleAsync(user, role).Result;
    }

    /// <summary>
    /// Add a user to a role
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="roleName">
    /// The role Name.
    /// </param>
    public void AddToRole(AspNetUsers user, string roleName)
    { 
        this.AddToRoleAsync(user, roleName);
    }

    /// <summary>
    /// Create a user with the given password
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public IdentityResult Create(AspNetUsers user, string password)
    {
        return this.CreateAsync(user, password).Result;
    }

    /// <summary>
    /// Update a user
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public IdentityResult Update(AspNetUsers user)
    {
        return this.UpdateAsync(user).Result;
    }

    /// <summary>
    /// Confirm the user's email with confirmation token
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="token">
    /// The token.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public IdentityResult ConfirmEmail(AspNetUsers user, string token)
    {
        return this.ConfirmEmailAsync(user, token).Result;
    }

    /// <summary>
    /// The find by name.
    /// </summary>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <returns>
    /// The <see cref="AspNetUsers"/>.
    /// </returns>
    public AspNetUsers FindByName(string userName)
    {
        return this.FindByNameAsync(userName).Result;
    }

    /// <summary>
    /// The find by email.
    /// </summary>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <returns>
    /// The <see cref="AspNetUsers"/>.
    /// </returns>
    public AspNetUsers FindByEmail(string email)
    {
        return this.FindByEmailAsync(email).Result;
    }

    /// <summary>
    /// Generate a password reset token for the user using the UserTokenProvider
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GeneratePasswordResetToken(AspNetUsers user)
    {
        return this.GeneratePasswordResetTokenAsync(user).Result;
    }

    /// <summary>
    /// Reset a user's password using a reset password token
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="token">
    /// The token.
    /// </param>
    /// <param name="newPassword">
    /// The new Password.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public IdentityResult ResetPassword(AspNetUsers user, string token, string newPassword)
    {
        return this.ResetPasswordAsync(user, token, newPassword).Result;
    }

    /// <summary>
    /// Get the email confirmation token for the user
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GenerateEmailConfirmationResetToken(AspNetUsers user)
    {
        return this.GenerateEmailConfirmationTokenAsync(user).Result;
    }

    /// <summary>
    /// Change a user password
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="currentPassword">
    /// The current Password.
    /// </param>
    /// <param name="newPassword">
    /// The new Password.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public IdentityResult ChangePassword(AspNetUsers user, string currentPassword, string newPassword)
    {
        return this.ChangePasswordAsync(user, currentPassword, newPassword).Result;
    }

    /// <summary>
    /// The add login.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="login">
    /// The login.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public IdentityResult AddLogin(AspNetUsers user, UserLoginInfo login)
    {
        return this.AddLoginAsync(user, login).Result;
    }
}