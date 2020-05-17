/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
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

namespace YAF.Core.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;

    using YAF.Core.Context;
    using YAF.Core.Membership;
    using YAF.Types.Models.Identity;

    using Startup = YAF.Core.Context.Start.Startup;

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
        public AspNetUsersManager(
            IUserStore<AspNetUsers> store)
            : base(store)
        {
            this.UserValidator = new UserValidator<AspNetUsers>(this)
            {
                AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = true
            };

            this.PasswordHasher = new SQLPasswordHasher();

            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = BoardContext.Current.BoardSettings.MinRequiredPasswordLength, RequireNonLetterOrDigit = true
            };

            /*manager.RegisterTwoFactorProvider("E-Mail-Code", new EmailTokenProvider<AspNetUsers>
            {
                Subject = "Sicherheitscode",
                BodyFormat = "Ihr Sicherheitscode lautet {0}"
            });*/

            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;

            var dataProtectionProvider = Startup.DataProtectionProvider;

            this.UserTokenProvider =
                new DataProtectorTokenProvider<AspNetUsers>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    TokenLifespan = TimeSpan.FromHours(3)
                };
        }

        /// <summary>
        /// The users.
        /// </summary>
        public virtual IQueryable<AspNetUsers> Users => this.Users;

        /// <summary>
        /// Remove a user from a role.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        /// <returns>
        /// The <see cref="IdentityResult"/>.
        /// </returns>
        public IdentityResult RemoveFromRole(string userId, string role)
        {
            return this.RemoveFromRoleAsync(userId, role).Result;
        }

        /// <summary>
        /// Creates a ClaimsIdentity representing the user
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="authenticationType">
        /// The authentication Type.
        /// </param>
        /// <returns>
        /// The <see cref="ClaimsIdentity"/>.
        /// </returns>
        public ClaimsIdentity CreateIdentity(AspNetUsers user, string authenticationType)
        {
            return this.CreateIdentityAsync(user, authenticationType).Result;
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
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public IList<string> GetRoles(string userId)
        {
            return this.GetRolesAsync(userId).Result;
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
            return this.IsInRoleAsync(user.Id, role).Result;
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
            this.AddToRoleAsync(user.Id, roleName);
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
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="IdentityResult"/>.
        /// </returns>
        public IdentityResult ConfirmEmail(string userId, string token)
        {
            return this.ConfirmEmailAsync(userId, token).Result;
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
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GeneratePasswordResetToken(string userId)
        {
            return this.GeneratePasswordResetTokenAsync(userId).Result;
        }

        /// <summary>
        /// Reset a user's password using a reset password token
        /// </summary>
        /// <param name="userId">
        /// The user Id.
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
        public IdentityResult ResetPassword(string userId, string token, string newPassword)
        {
            return this.ResetPasswordAsync(userId, token, newPassword).Result;
        }

        /// <summary>
        /// Get the email confirmation token for the user
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GenerateEmailConfirmationResetToken(string userId)
        {
            return this.GenerateEmailConfirmationTokenAsync(userId).Result;
        }

        /// <summary>
        /// Change a user password
        /// </summary>
        /// <param name="userId">
        /// The user Id.
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
        public IdentityResult ChangePassword(string userId, string currentPassword, string newPassword)
        {
            return this.ChangePasswordAsync(userId, currentPassword, newPassword).Result;
        }
    }
}
