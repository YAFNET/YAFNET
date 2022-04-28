/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

using Microsoft.Owin.Security;

using YAF.Core.Model;
using YAF.Types.Constants;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

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
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// Returns if Login was successful or not
    /// </returns>
    public bool LoginOrCreateUser(out string message)
    {
        message = string.Empty;

        if (!this.Get<BoardSettings>().AllowSingleSignOn)
        {
            message = this.Get<ILocalization>().GetText("LOGIN", "SSO_DEACTIVATED");

            return false;
        }

        var loginInfo = this.Get<IAuthenticationManager>().GetExternalLoginInfo();

        // Get Values
        var email = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:google:email").Value;
        var name = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:google:name").Value;
        var googleUserId = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:google:id").Value;

        if (email.IsNotSet())
        {
            message = this.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLE_FAILED3");

            return false;
        }

        // Check if user exists
        var existingUser = this.Get<IAspNetUsersHelper>().GetUserByEmail(email);

        if (existingUser == null)
        {
            // Create new User
            return this.CreateGoogleUser(name, email, googleUserId, out message);
        }

        if (existingUser.Profile_GoogleId == googleUserId)
        {
            message = string.Empty;
            return true;
        }

        message = this.Get<ILocalization>().GetText("LOGIN", "SSO_GOOGLE_FAILED3");

        return false;
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
    /// The google User Id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// Returns if the login was successfully or not
    /// </returns>
    private bool CreateGoogleUser(string name, string email, string googleUserId, out string message)
    {
        if (this.Get<BoardSettings>().DisableRegistrations)
        {
            message = this.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
            return false;
        }

        // Check if user name is null
        var userName = name;
        var displayName = userName;

        userName = displayName.Replace(" ", ".");

        var pass = PasswordGenerator.GeneratePassword(true, true, true, true, false, 16);

        var user = new AspNetUsers
                       {
                           Id = Guid.NewGuid().ToString(),
                           ApplicationId = this.Get<BoardSettings>().ApplicationId,
                           UserName = userName,
                           LoweredUserName = userName.ToLower(),
                           Email = email,
                           IsApproved = true,
                           EmailConfirmed = true,
                           Profile_RealName = name,
                           Profile_GoogleId = googleUserId
                       };

        var result = this.Get<IAspNetUsersHelper>().Create(user, pass);

        if (!result.Succeeded)
        {
            // error of some kind
            message = result.Errors.FirstOrDefault();
            return false;
        }

        // setup initial roles (if any) for this user
        this.Get<IAspNetRolesHelper>().SetupUserRoles(BoardContext.Current.PageBoardID, user);

        // create the user in the YAF DB as well as sync roles...
        var userID = this.Get<IAspNetRolesHelper>().CreateForumUser(user, displayName, BoardContext.Current.PageBoardID);

        if (userID == null)
        {
            // something is seriously wrong here -- redirect to failure...
            message = this.Get<ILocalization>().GetText("LOGIN", "SSO_FAILED");
            return false;
        }

        // send user register notification to the user...
        this.Get<ISendNotification>().SendRegistrationNotificationToUser(
            user,
            pass,
            "NOTIFICATION_ON_GOOGLE_REGISTER");

        if (this.Get<BoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
        {
            // send user register notification to the following admin users...
            this.Get<ISendNotification>().SendRegistrationNotificationEmail(user, userID.Value);
        }

        var autoWatchTopicsEnabled = this.Get<BoardSettings>().DefaultNotificationSetting
                                     == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

        // save the settings...
        this.GetRepository<User>().SaveNotification(
            userID.Value,
            true,
            autoWatchTopicsEnabled,
            this.Get<BoardSettings>().DefaultNotificationSetting.ToInt(),
            this.Get<BoardSettings>().DefaultSendDigestEmail);

        // save avatar
        /*this.GetRepository<User>().SaveAvatar(
            userID.Value,
            $"https://graph.facebook.com/{facebookUserId}/picture",
            null,
            null);

        BoardContext.Current.GetRepository<User>().SaveAvatar(BoardContext.Current.PageUserID, googleUser.ProfileImage, null, null);*/

        this.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userID.Value));

        message = string.Empty;

        return true;
    }
}