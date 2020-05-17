/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Linq;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin Page for manually user registration
    /// </summary>
    public partial class RegisterUser : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handles the Click event of the ForumRegister control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ForumRegisterClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Page.Validate();

            if (!this.Page.IsValid)
            {
                return;
            }

            var newEmail = this.Email.Text.Trim();
            var newUsername = this.UserName.Text.Trim();

            if (!ValidationHelper.IsValidEmail(newEmail))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_INVALID_MAIL"), MessageTypes.danger);

                return;
            }

            if (this.Get<IAspNetUsersHelper>().UserExists(this.UserName.Text.Trim(), newEmail))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_NAME_EXISTS"), MessageTypes.danger);
                return;
            }

            // setup their initial profile information
            var userProfile = new ProfileInfo
            {
                Location = this.Location.Text.Trim(), Homepage = this.HomePage.Text.Trim()
            };

            var user = new AspNetUsers
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationId = this.Get<BoardSettings>().ApplicationId,
                UserName = newUsername,
                LoweredUserName = newUsername,
                Email = newEmail,
                IsApproved = !this.Get<BoardSettings>().EmailVerification,
                EmailConfirmed = !this.Get<BoardSettings>().EmailVerification,

                Profile_Birthday = userProfile.Birthday,
                Profile_Blog = userProfile.Blog,
                Profile_Gender = userProfile.Gender,
                Profile_GoogleId = userProfile.GoogleId,
                Profile_Homepage = userProfile.Homepage,
                Profile_ICQ = userProfile.ICQ,
                Profile_Facebook = userProfile.Facebook,
                Profile_FacebookId = userProfile.FacebookId,
                Profile_Twitter = userProfile.Twitter,
                Profile_TwitterId = userProfile.TwitterId,
                Profile_Interests = userProfile.Interests,
                Profile_Location = userProfile.Location,
                Profile_Country = userProfile.Country,
                Profile_Region = userProfile.Region,
                Profile_City = userProfile.City,
                Profile_Occupation = userProfile.Occupation,
                Profile_RealName = userProfile.RealName,
                Profile_Skype = userProfile.Skype,
                Profile_XMPP = userProfile.XMPP,
                Profile_LastSyncedWithDNN = userProfile.LastSyncedWithDNN
            };

            var result = this.Get<IAspNetUsersHelper>().Create(user, this.Password.Text.Trim());

            if (!result.Succeeded)
            {
                // error of some kind
                this.PageContext.AddLoadMessage(result.Errors.FirstOrDefault(), MessageTypes.danger);
                return;
            }

            // setup initial roles (if any) for this user
              AspNetRolesHelper.SetupUserRoles(BoardContext.Current.PageBoardID, user);

              // create the user in the YAF DB as well as sync roles...
              var userId = AspNetRolesHelper.CreateForumUser(user, BoardContext.Current.PageBoardID);

              var autoWatchTopicsEnabled = this.Get<BoardSettings>().DefaultNotificationSetting
                  .Equals(UserNotificationSetting.TopicsIPostToOrSubscribeTo);

              // save the time zone...
              this.GetRepository<User>().Save(
                  this.Get<IAspNetUsersHelper>().GetUserIDFromProviderUserKey(user.Id),
                  this.PageContext.PageBoardID,
                  null,
                  null,
                  null,
                  this.TimeZones.SelectedValue,
                  null,
                  null,
                  null,
                  false);

              if (this.Get<BoardSettings>().EmailVerification)
              {
                  this.Get<ISendNotification>().SendVerificationEmail(user, newEmail, userId, newUsername);
              }

              this.GetRepository<User>().SaveNotification(
                  this.Get<IAspNetUsersHelper>().GetUserIDFromProviderUserKey(user.Id),
                  true,
                  autoWatchTopicsEnabled,
                  this.Get<BoardSettings>().DefaultNotificationSetting.ToInt(),
                  this.Get<BoardSettings>().DefaultSendDigestEmail);

              // success
              this.PageContext.AddLoadMessage(
                  this.GetTextFormatted("MSG_CREATED", this.UserName.Text.Trim()),
                  MessageTypes.success);

              BuildLink.Redirect(ForumPages.Admin_RegisterUser);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.TimeZones.DataSource = StaticDataHelper.TimeZones();
            this.DataBind();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), BuildLink.GetLink(ForumPages.Admin_Users));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_REGUSER", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_USERS", "TITLE")} - {this.GetText("ADMIN_REGUSER", "TITLE")}";
        }

        /// <summary>
        /// Handles the Click event of the cancel control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Admin_Users);
        }

        #endregion
    }
}