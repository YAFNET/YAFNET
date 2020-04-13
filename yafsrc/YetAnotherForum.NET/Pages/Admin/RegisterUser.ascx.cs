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
    using System.Web.Security;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
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

            if (UserMembershipHelper.UserExists(this.UserName.Text.Trim(), newEmail))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_REGUSER", "MSG_NAME_EXISTS"), MessageTypes.danger);
                return;
            }

            var user = this.Get<MembershipProvider>().CreateUser(
                newUsername,
                this.Password.Text.Trim(),
                newEmail,
                this.Question.Text.Trim(),
                this.Answer.Text.Trim(),
                !this.Get<BoardSettings>().EmailVerification,
                null,
                out var status);

            if (status != MembershipCreateStatus.Success)
            {
                // error of some kind
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("MSG_ERROR_CREATE", status),
                    MessageTypes.danger);
                return;
            }

            // setup initial roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(BoardContext.Current.PageBoardID, newUsername);

            // create the user in the YAF DB as well as sync roles...
            var userId = RoleMembershipHelper.CreateForumUser(user, BoardContext.Current.PageBoardID);

            // create profile
            var userProfile = Utils.UserProfile.GetProfile(newUsername);

            // setup their initial profile information
            userProfile.Location = this.Location.Text.Trim();
            userProfile.Homepage = this.HomePage.Text.Trim();
            userProfile.Save();

            var autoWatchTopicsEnabled = this.Get<BoardSettings>().DefaultNotificationSetting
                .Equals(UserNotificationSetting.TopicsIPostToOrSubscribeTo);

            // save the time zone...
            this.GetRepository<User>().Save(
                UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey),
                this.PageContext.PageBoardID,
                null,
                null,
                null,
                this.TimeZones.SelectedValue,
                null,
                null,
                null,
                null,
                this.Get<BoardSettings>().DefaultNotificationSetting,
                autoWatchTopicsEnabled,
                null,
                null,
                null);

            if (this.Get<BoardSettings>().EmailVerification)
            {
                this.Get<ISendNotification>().SendVerificationEmail(user, newEmail, userId, newUsername);
            }

            this.GetRepository<User>().SaveNotification(
                UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey),
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