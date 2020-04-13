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
    using System.Data;
    using System.Web;
    using System.Web.Security;

    using YAF.Core.BasePages;
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
    /// The User Account Verification Page.
    /// </summary>
    public partial class Approve : ForumPage
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
            var userRow = this.GetRepository<CheckEmail>().Update(this.key.Text);
            var userEmail = userRow["Email"].ToString();

            var keyVerified = !userRow["ProviderUserKey"].IsNullOrEmptyDBField();

            this.Approved.Visible = keyVerified;
            this.Error.Visible = !keyVerified;

            if (!keyVerified)
            {
                return;
            }

            // approve and update e-mail in the membership as well...
            var user = UserMembershipHelper.GetMembershipUserByKey(userRow["ProviderUserKey"]);

            if (!user.IsApproved)
            {
                user.IsApproved = true;

                // Send welcome mail/pm to user
                this.Get<ISendNotification>().SendUserWelcomeNotification(user, userRow.Field<int>("UserID"));
            }

            // update the email if anything was returned...
            if (user.Email != userEmail && userEmail != string.Empty)
            {
                user.Email = userEmail;
            }

            // tell the provider to update...
            this.Get<MembershipProvider>().UpdateUser(user);

            // now redirect to main site...
            this.PageContext.LoadMessage.AddSession(this.GetText("EMAIL_VERIFIED"), MessageTypes.info);

            // default redirect -- because if may not want to redirect to login.
            BuildLink.Redirect(ForumPages.forum);
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

            if (this.Get<HttpRequestBase>().QueryString.Exists("k"))
            {
                this.key.Text = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("k");
                this.ValidateKey_Click(sender, e);
            }
            else
            {
                this.Approved.Visible = false;
                this.Error.Visible = true;
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