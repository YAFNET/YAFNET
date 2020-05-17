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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Net.Mail;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Send Email from one user to the user
    /// </summary>
    public partial class Email : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Email" /> class.
        /// </summary>
        public Email()
            : base("IM_EMAIL")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets UserID.
        /// </summary>
        public int UserId =>
            Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.User == null || !this.Get<BoardSettings>().AllowEmailSending)
            {
                BuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            // get user data...
            var user = this.Get<IAspNetUsersHelper>().GetMembershipUserById(this.UserId);

            if (user == null)
            {
                // No such user exists
                BuildLink.AccessDenied();
            }
            else
            {
                if (user.IsApproved == false)
                {
                    BuildLink.AccessDenied();
                }

                var displayName = this.Get<IAspNetUsersHelper>().GetDisplayNameFromID(this.UserId);

                this.PageLinks.AddRoot();
                this.PageLinks.AddLink(
                    this.PageContext.BoardSettings.EnableDisplayName ? displayName : user.UserName,
                    BuildLink.GetLink(
                        ForumPages.UserProfile,
                        "u={0}&name={1}",
                        this.UserId,
                        this.PageContext.BoardSettings.EnableDisplayName ? displayName : user.UserName));
                this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
            }
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {

        }

        /// <summary>
        /// Handles the Click event of the Send control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Send_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            try
            {
                // get "to" user...
                var toUser = this.Get<IAspNetUsersHelper>().GetMembershipUserById(this.UserId);

                // send it...
                this.Get<ISendMail>().Send(
                    new MailAddress(this.PageContext.User.Email, this.PageContext.User.UserName),
                    new MailAddress(toUser.Email.Trim(), toUser.UserName.Trim()),
                    new MailAddress(this.PageContext.BoardSettings.ForumEmail, this.PageContext.BoardSettings.Name),
                    this.Subject.Text.Trim(),
                    this.Body.Text.Trim());

                // redirect to profile page...
                BuildLink.Redirect(ForumPages.UserProfile, false, "u={0}", this.UserId);
            }
            catch (Exception x)
            {
                this.Logger.Log(this.PageContext.PageUserID, this, x);

                this.PageContext.AddLoadMessage(
                    this.PageContext.IsAdmin ? x.Message : this.GetText("ERROR"),
                    MessageTypes.danger);
            }
        }

        #endregion
    }
}