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

namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

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

            var realUserName = this.GetValidUsername(this.Login1.UserName, this.Login1.Password);

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
            string realUsername;
            if (username.Contains("@") && this.Get<MembershipProvider>().RequiresUniqueEmail)
            {
                // attempt Email Login
                realUsername = this.Get<MembershipProvider>().GetUserNameByEmail(username);

                if (realUsername.IsSet() && this.Get<MembershipProvider>().ValidateUser(realUsername, password))
                {
                    return realUsername;
                }
            }

            // Standard user name login
            if (this.Get<MembershipProvider>().ValidateUser(username, password))
            {
                return username;
            }

            // display name login...
            if (!this.Get<BoardSettings>().EnableDisplayName)
            {
                return null;
            }

            // Display name login
            var id = this.Get<IUserDisplayName>().GetId(username);

            if (!id.HasValue)
            {
                return null;
            }

            // get the username associated with this id...
            realUsername = UserMembershipHelper.GetUserNameFromID(id.Value);

            // validate again...
            return this.Get<MembershipProvider>().ValidateUser(realUsername, password) ? realUsername : null;
        }

        /// <summary>
        /// Handles the LoginError event of the Login1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Login1_LoginError([NotNull] object sender, [NotNull] EventArgs e)
        {
            var emptyFields = false;

            var userName = this.Login1.FindControlAs<TextBox>("UserName");
            var password = this.Login1.FindControlAs<TextBox>("Password");

            if (userName.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("REGISTER", "NEED_USERNAME"),
                    "LoginBox",
                    MessageTypes.danger);
                emptyFields = true;
            }

            if (password.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("REGISTER", "NEED_PASSWORD"),
                    "LoginBox",
                    MessageTypes.danger);
                emptyFields = true;
            }

            if (!emptyFields)
            {
                this.PageContext.AddLoadMessage(
                    this.Login1.FailureText,
                    "LoginBox",
                    MessageTypes.danger);
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
            BoardContext.Current.PageElements.RegisterJsBlock(
                "yafmodaldialogJs",
                JavaScriptBlocks.LoginBoxLoadJs(".LoginLink", "#LoginBox"));

            base.OnPreRender(e);
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
            // Login1.CreateUserUrl = BuildLink.GetLink( ForumPages.register );
            this.Login1.PasswordRecoveryText = this.GetText("lostpassword");
            this.Login1.PasswordRecoveryUrl = BuildLink.GetLink(ForumPages.RecoverPassword);
            this.Login1.FailureText = this.GetText("password_error");

            this.Login1.DestinationPageUrl = this.Page.Request.RawUrl;

            // localize controls
            var rememberMe = this.Login1.FindControlAs<CheckBox>("RememberMe");
            var userName = this.Login1.FindControlAs<TextBox>("UserName");
            var password = this.Login1.FindControlAs<TextBox>("Password");
            var forumLogin = this.Login1.FindControlAs<Button>("LoginButton");
            var registerLink = this.Login1.FindControlAs<ThemeButton>("RegisterLink");
            var passwordRecovery = this.Login1.FindControlAs<LinkButton>("PasswordRecovery");

            var singleSignOnHolder = this.Login1.FindControlAs<PlaceHolder>("SingleSignOnHolder");

            var faceBookHolder = this.Login1.FindControlAs<PlaceHolder>("FaceBookHolder");
            var facebookRegister = this.Login1.FindControlAs<ThemeButton>("FacebookRegister");

            var twitterHolder = this.Login1.FindControlAs<PlaceHolder>("TwitterHolder");
            var twitterRegister = this.Login1.FindControlAs<ThemeButton>("TwitterRegister");

            var googleHolder = this.Login1.FindControlAs<PlaceHolder>("GoogleHolder");
            var googleRegister = this.Login1.FindControlAs<ThemeButton>("GoogleRegister");

            userName.Focus();

            if (rememberMe != null)
            {
                rememberMe.Text = this.GetText("auto");
            }

            if (forumLogin != null)
            {
                forumLogin.Text = this.GetText("forum_login");

                var script =
                    $@"if(event.which || event.keyCode){{if ((event.which == 13) || (event.keyCode == 13)) {{
                          document.getElementById('{forumLogin.ClientID}').click();return false;}}}} else {{return true}}; ";

                userName.Attributes.Add("onkeydown", script);

                password.Attributes.Add("onkeydown", script);
            }

            if (passwordRecovery != null)
            {
                passwordRecovery.Text = this.GetText("lostpassword");
            }

            if (this.Get<BoardSettings>().AllowSingleSignOn)
            {
                faceBookHolder.Visible = Config.FacebookAPIKey.IsSet() && Config.FacebookSecretKey.IsSet();
                twitterHolder.Visible = Config.TwitterConsumerKey.IsSet() && Config.TwitterConsumerSecret.IsSet();
                googleHolder.Visible = Config.GoogleClientID.IsSet() && Config.GoogleClientSecret.IsSet();

                if (twitterHolder.Visible)
                {
                    twitterRegister.Visible = true;
                    twitterRegister.Text = this.GetTextFormatted("AUTH_CONNECT", "Twitter");
                    twitterRegister.TitleLocalizedTag = "AUTH_CONNECT_HELP";
                    twitterRegister.ParamTitle0 = "Twitter";
                }

                if (faceBookHolder.Visible)
                {
                    facebookRegister.Visible = true;
                    facebookRegister.Text = this.GetTextFormatted("AUTH_CONNECT", "Facebook");
                    facebookRegister.TitleLocalizedTag = "AUTH_CONNECT_HELP";
                    facebookRegister.ParamTitle0 = "Facebook";
                }

                if (googleHolder.Visible)
                {
                    googleRegister.Visible = true;
                    googleRegister.Text = this.GetTextFormatted("AUTH_CONNECT", "Google");
                    googleRegister.TitleLocalizedTag = "AUTH_CONNECT_HELP";
                    googleRegister.ParamTitle0 = "Google";
                }

                singleSignOnHolder.Visible = twitterHolder.Visible || faceBookHolder.Visible || googleHolder.Visible;
            }
            else
            { 
                singleSignOnHolder.Visible = false; 
            }

            if (this.PageContext.IsGuest && !this.Get<BoardSettings>().DisableRegistrations && !Config.IsAnyPortal)
            {
                registerLink.Visible = true;
                registerLink.Text = this.GetText("register_instead");
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
            BuildLink.Redirect(ForumPages.RecoverPassword);
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
                .Raise(new SuccessfulUserLoginEvent(this.PageContext.PageUserID));

            this.GetRepository<User>().UpdateAuthServiceStatus(
                this.PageContext.PageUserID,
                AuthService.none);
        }

        /// <summary>
        /// Redirects to the Facebook login/register page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void FacebookRegisterClick(object sender, EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login, "auth={0}", AuthService.facebook);
        }

        /// <summary>
        /// Redirects to the Twitter login/register page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void TwitterRegisterClick(object sender, EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login, "auth={0}", AuthService.twitter);
        }

        /// <summary>
        /// Redirects to the Google login/register page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GoogleRegisterClick(object sender, EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login, "auth={0}", AuthService.google);
        }

        /// <summary>
        /// Redirects to the Register Page
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void RegisterLink_Click(object sender, EventArgs e)
        {
            BuildLink.Redirect(
                this.Get<BoardSettings>().ShowRulesForRegistration ? ForumPages.RulesAndPrivacy : ForumPages.Register);
        }
    }
}