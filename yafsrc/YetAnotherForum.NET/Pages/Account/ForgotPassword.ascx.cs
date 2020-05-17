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
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The recover Password Page.
    /// </summary>
    public partial class ForgotPassword : AccountPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ForgotPassword" /> class.
        /// </summary>
        public ForgotPassword()
            : base("RECOVER_PASSWORD")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether IsProtected.
        /// </summary>
        public override bool IsProtected => false;

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                this.ContentBody.CssClass = "card-body was-validated";
                return;
            }

            this.DataBind();

            this.UserName.Focus();
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.GetText("TITLE"));
        }

        #endregion

        /// <summary>
        /// The forgot password click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ForgotPasswordClick(object sender, EventArgs e)
        {
            var user = this.UserName.Text.Contains("@")
                ? this.Get<IAspNetUsersHelper>().GetUserByEmail(this.UserName.Text)
                : this.Get<IAspNetUsersHelper>().GetUserByName(this.UserName.Text);

            if (user == null/* || !this.Get<IAspNetUsersManager>().IsEmailConfirmed(user.Id)*/)
            {
                this.PageContext.AddLoadMessage(this.GetText("USERNAME_FAILURE"), MessageTypes.danger);
                return;
            }

            // verify the user is approved, etc...
            if (!user.IsApproved)
            {
                // explain they are not approved yet...
                this.PageContext.LoadMessage.AddSession(this.GetText("ACCOUNT_NOT_APPROVED_VERIFICATION"), MessageTypes.warning);

                return;
            }

            var code = HttpUtility.UrlEncode(this.Get<IAspNetUsersHelper>().GeneratePasswordResetToken(user.Id));
           
            this.Get<ISendNotification>().SendPasswordReset(user, code);
            
            this.PageContext.LoadMessage.AddSession(this.GetText("SUCCESS"), MessageTypes.success);

            BuildLink.Redirect(ForumPages.Board);
        }
    }
}