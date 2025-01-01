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
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANYc
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Identity.Owin;

using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// Google Single Sign On Class
/// </summary>
public class Google : IAuthBase, IHaveServiceLocator
{
    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Logins the or create user.
    /// </summary>
    /// <returns>
    /// The <see cref="AspNetUsers"/>.
    /// </returns>
    public async Task<(string Message, AspNetUsers User)> LoginOrCreateUserAsync()
    {
        if (!this.Get<BoardSettings>().AllowSingleSignOn)
        {
            return (this.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED"), null);
        }

        var loginInfo = await this.Get<SignInManager<AspNetUsers>>().GetExternalLoginInfoAsync();

        if (loginInfo == null)
        {
            return (this.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLE_FAILED3"), null);
        }

        // Get Values
        var email = loginInfo.Principal.FindFirst(ClaimTypes.Email).Value;
        var name = loginInfo.Principal.FindFirst(ClaimTypes.Name).Value;
        var googleUserId = loginInfo.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

        if (email.IsNotSet())
        {
            return (this.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLE_FAILED3"), null);
        }

        // Check if user exists
        var existingUser = await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(email);

        if (existingUser == null)
        {
            // Create new PageUser
            return await this.CreateGoogleUserAsync(name, email, googleUserId);
        }

        if (existingUser.Profile_GoogleId == googleUserId)
        {
            return (string.Empty, existingUser);
        }

        return (this.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLE_FAILED3"), null);
    }

    /// <summary>
    /// Creates the Google user
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="googleUserId">
    /// The google PageUser Id.
    /// </param>
    /// <returns>
    /// Returns if the login was successfully or not
    /// </returns>
    private async Task<(string Message, AspNetUsers User)> CreateGoogleUserAsync(string name, string email, string googleUserId)
    {
        if (this.Get<BoardSettings>().DisableRegistrations)
        {
            return (this.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED"), null);
        }

        // Check if user name is null
        var userName = name;
        var displayName = userName;

        userName = displayName.Replace(" ", ".");

        var pass = PasswordGenerator.GeneratePassword(true, true, true, true, false, 16);

        var user = new AspNetUsers {
            Id = Guid.NewGuid().ToString(),
            ApplicationId = this.Get<BoardSettings>().ApplicationId,
            UserName = userName,
            LoweredUserName = userName.ToLower(),
            Email = email,
            LoweredEmail = email.ToLower(),
            IsApproved = true,
            EmailConfirmed = true,
            Profile_RealName = name,
            Profile_GoogleId = googleUserId
        };

        var result = await this.Get<IAspNetUsersHelper>().CreateUserAsync(user, pass);

        if (!result.Succeeded)
        {
            // error of some kind
            return (result.Errors.FirstOrDefault()?.Description, null);
        }

        // setup initial roles (if any) for this user
        await this.Get<IAspNetRolesHelper>().SetupUserRolesAsync(BoardContext.Current.PageBoardID, user);

        // create the user in the YAF DB as well as sync roles...
        var userId = await this.Get<IAspNetRolesHelper>().CreateForumUserAsync(user, displayName, BoardContext.Current.PageBoardID);

        if (userId == null)
        {
            // something is seriously wrong here -- redirect to failure...
            return (this.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED"), null);
        }

        // send user register notification to the user...
        await this.Get<ISendNotification>().SendRegistrationNotificationToUserAsync(
            user,
            pass,
            "NOTIFICATION_ON_GOOGLE_REGISTER");

        if (this.Get<BoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
        {
            // send user register notification to the following admin users...
            await this.Get<ISendNotification>().SendRegistrationNotificationEmailAsync(user, userId.Value);
        }

        var autoWatchTopicsEnabled = this.Get<BoardSettings>().DefaultNotificationSetting
                                     == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

        // save the settings...
        this.GetRepository<User>().SaveNotification(
            userId.Value,
            autoWatchTopicsEnabled,
            this.Get<BoardSettings>().DefaultNotificationSetting.ToInt(),
            this.Get<BoardSettings>().DefaultSendDigestEmail);

        this.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId.Value));

        return (string.Empty, user);
    }
}