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
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Admin
{
    #region Using

    using YAF.Types.Models;
    using YAF.Types.Models.Identity;

    #endregion

    /// <summary>
    /// The Admin Page for manually user registration
    /// </summary>
    public partial class RegisterUser : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterUser"/> class. 
        /// </summary>
        public RegisterUser()
            : base("ADMIN_REGUSER", ForumPages.Admin_RegisterUser)
        {
        }

        #endregion

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
                this.PageBoardContext.Notify(this.GetText("ADMIN_REGUSER", "MSG_INVALID_MAIL"), MessageTypes.danger);

                return;
            }

            if (this.Get<IAspNetUsersHelper>().UserExists(this.UserName.Text.Trim(), newEmail))
            {
                this.PageBoardContext.Notify(this.GetText("ADMIN_REGUSER", "MSG_NAME_EXISTS"), MessageTypes.danger);
                return;
            }


            var user = new AspNetUsers
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationId = this.PageBoardContext.BoardSettings.ApplicationId,
                UserName = newUsername,
                LoweredUserName = newUsername,
                Email = newEmail,
                IsApproved = false,
                EmailConfirmed = false
            };

            var result = this.Get<IAspNetUsersHelper>().Create(user, this.Password.Text.Trim());

            if (!result.Succeeded)
            {
                // error of some kind
                this.PageBoardContext.Notify(result.Errors.FirstOrDefault(), MessageTypes.danger);
                return;
            }

            // setup initial roles (if any) for this user
            this.Get<IAspNetRolesHelper>().SetupUserRoles(this.PageBoardContext.PageBoardID, user);

            // create the user in the YAF DB as well as sync roles...
            var userId = this.Get<IAspNetRolesHelper>().CreateForumUser(user, this.PageBoardContext.PageBoardID);

            var autoWatchTopicsEnabled = this.PageBoardContext.BoardSettings.DefaultNotificationSetting
                .Equals(UserNotificationSetting.TopicsIPostToOrSubscribeTo);

            this.Get<ISendNotification>().SendVerificationEmail(user, newEmail, userId, newUsername);

            this.GetRepository<User>().SaveNotification(
                this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(user.Id).ID,
                true,
                autoWatchTopicsEnabled,
                this.PageBoardContext.BoardSettings.DefaultNotificationSetting.ToInt(),
                this.PageBoardContext.BoardSettings.DefaultSendDigestEmail);

            // success
            this.PageBoardContext.LoadMessage.AddSession(
                this.GetTextFormatted("MSG_CREATED", this.UserName.Text.Trim()),
                MessageTypes.success);

            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Users);
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

            this.DataBind();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddAdminIndex();

            this.PageLinks.AddLink(
                this.GetText("ADMIN_USERS", "TITLE"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Users));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_REGUSER", "TITLE"), string.Empty);
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
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Users);
        }

        #endregion
    }
}