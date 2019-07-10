/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Utilities;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Login Box
    /// </summary>
    public partial class LoginBox : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Handles the Authenticate event of the Login1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AuthenticateEventArgs"/> instance containing the event data.</param>
        protected void Login1_Authenticate([NotNull] object sender, [NotNull] AuthenticateEventArgs e)
        {
            e.Authenticated = false;

            var realUserName = this.GetValidUsername(username: this.Login1.UserName, password: this.Login1.Password);

            if (!realUserName.IsSet())
            {
                return;
            }

            this.Login1.UserName = realUserName;
            e.Authenticated = true;
        }

        /// <summary>
        /// Gets the valid username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The get valid login.
        /// </returns>
        protected virtual string GetValidUsername(string username, string password)
        {
            if (username.Contains(value: "@") && this.Get<MembershipProvider>().RequiresUniqueEmail)
            {
                // attempt Email Login
                var realUsername = this.Get<MembershipProvider>().GetUserNameByEmail(email: username);

                if (realUsername.IsSet() && this.Get<MembershipProvider>()
                        .ValidateUser(username: realUsername, password: password))
                {
                    return realUsername;
                }
            }

            // Standard user name login
            if (this.Get<MembershipProvider>().ValidateUser(username: username, password: password))
            {
                return username;
            }

            // display name login...
            if (!this.Get<YafBoardSettings>().EnableDisplayName)
            {
                return null;
            }

            // Display name login
            var id = this.Get<IUserDisplayName>().GetId(name: username);

            if (id.HasValue)
            {
                // get the username associated with this id...
                var realUsername = UserMembershipHelper.GetUserNameFromID(userID: id.Value);

                // validate again...
                if (this.Get<MembershipProvider>().ValidateUser(username: realUsername, password: password))
                {
                    return realUsername;
                }
            }

            // no valid login -- return null
            return null;
        }

        /// <summary>
        /// Handles the LoginError event of the Login1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Login1_LoginError([NotNull] object sender, [NotNull] EventArgs e)
        {
            var emptyFields = false;

            var userName = this.Login1.FindControlAs<TextBox>(id: "UserName");
            var password = this.Login1.FindControlAs<TextBox>(id: "Password");

            if (userName.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(
                    message: this.GetText(page: "REGISTER", tag: "NEED_USERNAME"),
                    script: "LoginBox",
                    messageType: MessageTypes.danger);
                emptyFields = true;
            }

            if (password.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(
                    message: this.GetText(page: "REGISTER", tag: "NEED_PASSWORD"),
                    script: "LoginBox",
                    messageType: MessageTypes.danger);
                emptyFields = true;
            }

            if (!emptyFields)
            {
                this.PageContext.AddLoadMessage(
                    message: this.Login1.FailureText,
                    script: "LoginBox",
                    messageType: MessageTypes.danger);
            }
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and YAF JS...
            YafContext.Current.PageElements.RegisterJsBlock(
                name: "yafmodaldialogJs",
                script: JavaScriptBlocks.LoginBoxLoadJs(openLink: ".LoginLink", dialogId: "#LoginBox"));

            base.OnPreRender(e: e);
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

            this.Login1.MembershipProvider = Config.MembershipProvider;

            // Login1.CreateUserText = "Sign up for a new account.";
            // Login1.CreateUserUrl = YafBuildLink.GetLink( ForumPages.register );
            this.Login1.PasswordRecoveryText = this.GetText(tag: "lostpassword");
            this.Login1.PasswordRecoveryUrl = YafBuildLink.GetLink(page: ForumPages.recoverpassword);
            this.Login1.FailureText = this.GetText(tag: "password_error");

            this.Login1.DestinationPageUrl = this.Page.Request.RawUrl;

            // localize controls
            var rememberMe = this.Login1.FindControlAs<CheckBox>(id: "RememberMe");
            var userName = this.Login1.FindControlAs<TextBox>(id: "UserName");
            var password = this.Login1.FindControlAs<TextBox>(id: "Password");
            var forumLogin = this.Login1.FindControlAs<Button>(id: "LoginButton");
            var passwordRecovery = this.Login1.FindControlAs<LinkButton>(id: "PasswordRecovery");

            var faceBookHolder = this.Login1.FindControlAs<PlaceHolder>(id: "FaceBookHolder");
            var facebookRegister = this.Login1.FindControlAs<LinkButton>(id: "FacebookRegister");

            var twitterHolder = this.Login1.FindControlAs<PlaceHolder>(id: "TwitterHolder");
            var twitterRegister = this.Login1.FindControlAs<LinkButton>(id: "TwitterRegister");

            var googleHolder = this.Login1.FindControlAs<PlaceHolder>(id: "GoogleHolder");
            var googleRegister = this.Login1.FindControlAs<LinkButton>(id: "GoogleRegister");

            userName.Focus();

            if (rememberMe != null)
            {
                rememberMe.Text = this.GetText(tag: "auto");
            }

            if (forumLogin != null)
            {
                forumLogin.Text = this.GetText(tag: "forum_login");

                var script =
                    $"if(event.which || event.keyCode){{if ((event.which == 13) || (event.keyCode == 13)) {{document.getElementById('{forumLogin.ClientID}').click();return false;}}}} else {{return true}}; ";

                userName.Attributes.Add(key: "onkeydown", value: script);

                password.Attributes.Add(key: "onkeydown", value: script);
            }

            if (passwordRecovery != null)
            {
                passwordRecovery.Text = this.GetText(tag: "lostpassword");
            }

            if (this.Get<YafBoardSettings>().AllowSingleSignOn)
            {
                faceBookHolder.Visible = Config.FacebookAPIKey.IsSet() && Config.FacebookSecretKey.IsSet();
                twitterHolder.Visible = Config.TwitterConsumerKey.IsSet() && Config.TwitterConsumerSecret.IsSet();
                googleHolder.Visible = Config.GoogleClientID.IsSet() && Config.GoogleClientSecret.IsSet();

                if (twitterHolder.Visible)
                {
                    twitterRegister.Visible = true;
                    twitterRegister.Text = this.GetTextFormatted(tag: "AUTH_CONNECT", "Twitter");
                    twitterRegister.ToolTip = this.GetTextFormatted(tag: "AUTH_CONNECT_HELP", "Twitter");
                }

                if (faceBookHolder.Visible)
                {
                    facebookRegister.Visible = true;
                    facebookRegister.Text = this.GetTextFormatted(tag: "AUTH_CONNECT", "Facebook");
                    facebookRegister.ToolTip = this.GetTextFormatted(tag: "AUTH_CONNECT_HELP", "Facebook");
                }

                if (googleHolder.Visible)
                {
                    googleRegister.Visible = true;
                    googleRegister.Text = this.GetTextFormatted(tag: "AUTH_CONNECT", "Google");
                    googleRegister.ToolTip = this.GetTextFormatted(tag: "AUTH_CONNECT_HELP", "Google");
                }
            }

            this.DataBind();
        }

        /// <summary>
        /// Called when Password Recovery is Clicked
        /// </summary>
        /// <param name="sender">
        /// standard event object sender
        /// </param>
        /// <param name="e">
        /// event args
        /// </param>
        protected void PasswordRecovery_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(page: ForumPages.recoverpassword);
        }

        #endregion

        /// <summary>
        /// Handles the LoggedIn event of the Login1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Login1_LoggedIn(object sender, EventArgs e)
        {
            this.Get<IRaiseEvent>()
                .Raise(eventObject: new SuccessfulUserLoginEvent(userId: this.PageContext.PageUserID));

            this.GetRepository<User>().UpdateAuthServiceStatus(
                userId: this.PageContext.PageUserID,
                authService: AuthService.none);
        }

        /// <summary>
        /// Redirects to the Facebook login/register page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void FacebookRegisterClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(page: ForumPages.login, format: "auth={0}", AuthService.facebook);
        }

        /// <summary>
        /// Redirects to the Twitter login/register page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void TwitterRegisterClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(page: ForumPages.login, format: "auth={0}", AuthService.twitter);
        }

        /// <summary>
        /// Redirects to the Google login/register page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GoogleRegisterClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(page: ForumPages.login, format: "auth={0}", AuthService.google);
        }
    }
}