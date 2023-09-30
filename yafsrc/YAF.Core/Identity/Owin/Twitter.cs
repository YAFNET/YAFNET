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
/// Twitter Single Sign On Class
/// </summary>
public class Twitter : IAuthBase, IHaveServiceLocator
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
        var name = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:twitter:name").Value;
        var email = $"{name}@twitter.com";
        var twitterUserId = loginInfo.ExternalIdentity.Claims.First(c => c.Type == "urn:twitter:id").Value;

        // Check if user exists
        var existingUser = this.Get<IAspNetUsersHelper>().GetUserByName(name);

        if (existingUser == null)
        {
            // Create new User
            return this.CreateTwitterUser(name, email, twitterUserId, out message);
        }

        if (existingUser.Profile_TwitterId == twitterUserId)
        {
            message = string.Empty;
            return true;
        }

        message = this.Get<ILocalization>().GetText("LOGIN", "SSO_TWITTER_FAILED3");

        return false;
    }

    /// <summary>
    /// Creates the twitter user
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="twitterUserId">
    /// The twitter User Id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// Returns if the login was successfully or not
    /// </returns>
    private bool CreateTwitterUser(string name, string email, string twitterUserId, out string message)
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
                           Profile_Twitter = userName,
                           Profile_TwitterId = twitterUserId,
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
        this.SendRegistrationMessageToTwitterUser(user, pass, userID.Value);

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

        this.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userID.Value));

        message = string.Empty;

        return true;
    }

    /// <summary>
    /// Send an Private Message to the Newly Created User with
    /// his Account Info (Pass, Security Question and Answer)
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="pass">
    /// The pass.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    private void SendRegistrationMessageToTwitterUser(
        AspNetUsers user,
        string pass,
        int userId)
    {
        var subject = string.Format(
            BoardContext.Current.Get<ILocalization>().GetText("COMMON", "NOTIFICATION_ON_NEW_FACEBOOK_USER_SUBJECT"),
            BoardContext.Current.BoardSettings.Name);

        var notifyUser = new TemplateEmail("NOTIFICATION_ON_TWITTER_REGISTER")
                             {
                                 TemplateParams =
                                     {
                                         ["{user}"] = user.UserName,
                                         ["{email}"] = user.Email,
                                         ["{pass}"] = pass,
                                         ["{forumname}"] = BoardContext.Current.BoardSettings.Name
                                     }
                             };

        var emailBody = notifyUser.ProcessTemplate("NOTIFICATION_ON_TWITTER_REGISTER");

        var messageFlags = new MessageFlags { IsHtml = false, IsBBCode = true };

        var hostUser = this.GetRepository<User>()
            .Get(u => u.BoardID == BoardContext.Current.PageBoardID && (u.Flags & 1) == 1)
            .FirstOrDefault();

        // Send Message also as DM to Twitter.
        this.GetRepository<PMessage>().SendMessage(hostUser.ID, userId, subject, emailBody, messageFlags.BitValue, -1);
    }
}