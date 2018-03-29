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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Admin Page for manually user registration
    /// </summary>
    public partial class reguser : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handles the Click event of the ForumRegister control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ForumRegisterClick([NotNull] object sender, [NotNull] EventArgs e)
        {
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

            MembershipCreateStatus status;
            var user = this.Get<MembershipProvider>().CreateUser(
                newUsername,
                this.Password.Text.Trim(),
                newEmail,
                this.Question.Text.Trim(),
                this.Answer.Text.Trim(),
                !this.Get<YafBoardSettings>().EmailVerification,
                null,
                out status);

            if (status != MembershipCreateStatus.Success)
            {
                // error of some kind
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_REGUSER", "MSG_ERROR_CREATE").FormatWith(status),
                    MessageTypes.danger);
                return;
            }

            // setup inital roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, newUsername);

            // create the user in the YAF DB as well as sync roles...
            var userId = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

            // create profile
            var userProfile = YafUserProfile.GetProfile(newUsername);

            // setup their inital profile information
            userProfile.Location = this.Location.Text.Trim();
            userProfile.Homepage = this.HomePage.Text.Trim();
            userProfile.Save();

            var autoWatchTopicsEnabled = this.Get<YafBoardSettings>().DefaultNotificationSetting
                .Equals(UserNotificationSetting.TopicsIPostToOrSubscribeTo);

            // save the time zone...
            LegacyDb.user_save(
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
                null,
                null,
                this.Get<YafBoardSettings>().DefaultNotificationSetting,
                autoWatchTopicsEnabled,
                null,
                null,
                null);

            if (this.Get<YafBoardSettings>().EmailVerification)
            {
                this.Get<ISendNotification>().SendVerificationEmail(user, newEmail, userId, newUsername);
            }

            LegacyDb.user_savenotification(
                UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey),
                true,
                autoWatchTopicsEnabled,
                this.Get<YafBoardSettings>().DefaultNotificationSetting,
                this.Get<YafBoardSettings>().DefaultSendDigestEmail);

            // success
            this.PageContext.AddLoadMessage(
                this.GetText("ADMIN_REGUSER", "MSG_CREATED").FormatWith(this.UserName.Text.Trim()));
            YafBuildLink.Redirect(ForumPages.admin_reguser);
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
                YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_users));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_REGUSER", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_USERS", "TITLE"),
                this.GetText("ADMIN_REGUSER", "TITLE"));
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
            YafBuildLink.Redirect(ForumPages.admin_users);
        }

        #endregion
    }
}