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

namespace YAF.Pages.Account
{
    #region Using

    using System;
    using System.Linq;
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    using Constants = YAF.Types.Constants.Constants;

    #endregion

    /// <summary>
    /// The User Account Verification Page.
    /// </summary>
    public partial class Approve : AccountPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Approve" /> class.
        /// </summary>
        public Approve()
            : base("APPROVE")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether IsProtected.
        /// </summary>
        public override bool IsProtected => false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the Click event of the ValidateKey control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        public void ValidateKey_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var userEmail = this.GetRepository<CheckEmail>().Update(HttpUtility.UrlEncode(this.key.Text));

            if (userEmail == null)
            {
                this.Approved.Visible = false;
                this.ErrorAlert.Visible = true;
                this.ErrorMessage.Text = this.GetText("email_verify_failed");

                return;
            }

            var user = this.Get<IAspNetUsersHelper>().GetUserByEmail(userEmail.Email);

            var result = this.Get<IAspNetUsersHelper>().ConfirmEmail(user.Id, this.key.Text);

            if (result.Succeeded)
            {
                this.Approved.Visible = true;
                this.ErrorAlert.Visible = false;

                user.IsApproved = true;
                user.EmailConfirmed = true;

                // tell the provider to update...
                this.Get<IAspNetUsersHelper>().Update(user);

                this.GetRepository<User>().Approve(userEmail.UserID);

                // Send welcome mail/pm to user
                this.Get<ISendNotification>().SendUserWelcomeNotification(user, userEmail.UserID);

                this.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
                this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);

                // automatically log in created users
                this.Get<IAspNetUsersHelper>().SignIn(user);

                // now redirect to main site...
                this.PageContext.LoadMessage.AddSession(this.GetText("EMAIL_VERIFIED"), MessageTypes.info);

                // default redirect -- because if may not want to redirect to login.
                BuildLink.Redirect(ForumPages.Board);
            }
            else
            {
                this.Approved.Visible = false;
                this.ErrorAlert.Visible = true;

                this.ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("code"))
            {
                this.key.Text = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("code");
                this.ValidateKey_Click(sender, e);
            }
            else
            {
                this.Approved.Visible = false;
                this.ErrorAlert.Visible = true;
                this.ErrorMessage.Text = this.GetText("email_verify_failed");
            }
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
        }

        #endregion
    }
}