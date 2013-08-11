/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Björnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Controls
{
    // YAF.Pages
    #region Using

    using System;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
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
            if (username.Contains("@") && this.Get<MembershipProvider>().RequiresUniqueEmail)
            {
                // attempt Email Login
                string realUsername = this.Get<MembershipProvider>().GetUserNameByEmail(username);

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
            if (this.Get<YafBoardSettings>().EnableDisplayName)
            {
                // Display name login
                var id = this.Get<IUserDisplayName>().GetId(username);

                if (id.HasValue)
                {
                    // get the username associated with this id...
                    string realUsername = UserMembershipHelper.GetUserNameFromID(id.Value);

                    // validate again...
                    if (this.Get<MembershipProvider>().ValidateUser(realUsername, password))
                    {
                        return realUsername;
                    }
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
            bool emptyFields = false;

            var userName = this.Login1.FindControlAs<TextBox>("UserName");
            var password = this.Login1.FindControlAs<TextBox>("Password");

            if (userName.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("REGISTER", "NEED_USERNAME"));
                emptyFields = true;
            }

            if (password.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("REGISTER", "NEED_PASSWORD"));
                emptyFields = true;
            }

            if (!emptyFields)
            {
                this.PageContext.AddLoadMessage(this.Login1.FailureText);
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
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJsResourceInclude("yafmodaldialog", "js/jquery.yafmodaldialog.js");
            YafContext.Current.PageElements.RegisterJsBlock(
              "yafmodaldialogJs", JavaScriptBlocks.YafModalDialogLoadJs(".LoginLink", "#LoginBox"));
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.yafmodaldialog.css");

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
            // Login1.CreateUserUrl = YafBuildLink.GetLink( ForumPages.register );
            this.Login1.PasswordRecoveryText = this.GetText("lostpassword");
            this.Login1.PasswordRecoveryUrl = YafBuildLink.GetLink(ForumPages.recoverpassword);
            this.Login1.FailureText = this.GetText("password_error");

            this.Login1.DestinationPageUrl =
                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ReturnUrl").IsSet()
                    ? this.Server.UrlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ReturnUrl"))
                    : YafBuildLink.GetLink(ForumPages.forum);

            // localize controls
            var rememberMe = this.Login1.FindControlAs<CheckBox>("RememberMe");
            var userName = this.Login1.FindControlAs<TextBox>("UserName");
            var password = this.Login1.FindControlAs<TextBox>("Password");
            var forumLogin = this.Login1.FindControlAs<Button>("LoginButton");
            var passwordRecovery = this.Login1.FindControlAs<LinkButton>("PasswordRecovery");

            var faceBookHolder = this.Login1.FindControlAs<PlaceHolder>("FaceBookHolder");
            var facebookRegister = this.Login1.FindControlAs<LinkButton>("FacebookRegister");
            
            var twitterHolder = this.Login1.FindControlAs<PlaceHolder>("TwitterHolder");
            var twitterRegister = this.Login1.FindControlAs<LinkButton>("TwitterRegister");

            var googleHolder = this.Login1.FindControlAs<PlaceHolder>("GoogleHolder");
            var googleRegister = this.Login1.FindControlAs<LinkButton>("GoogleRegister");

            userName.Focus();

            /*
                RequiredFieldValidator usernameRequired = ( RequiredFieldValidator ) Login1.FindControl( "UsernameRequired" );
                RequiredFieldValidator passwordRequired = ( RequiredFieldValidator ) Login1.FindControl( "PasswordRequired" );

                usernameRequired.ToolTip = usernameRequired.ErrorMessage = GetText( "REGISTER", "NEED_USERNAME" );
                passwordRequired.ToolTip = passwordRequired.ErrorMessage = GetText( "REGISTER", "NEED_PASSWORD" );
                */
            if (rememberMe != null)
            {
                rememberMe.Text = this.GetText("auto");
            }

            if (forumLogin != null)
            {
                forumLogin.Text = this.GetText("forum_login");
            }

            if (passwordRecovery != null)
            {
                passwordRecovery.Text = this.GetText("lostpassword");
            }

            userName.Attributes.Add(
                "onkeydown",
                "if(event.which || event.keyCode){{if ((event.which == 13) || (event.keyCode == 13)) {{document.getElementById('{0}').click();return false;}}}} else {{return true}}; "
                    .FormatWith(forumLogin.ClientID));

            password.Attributes.Add(
                "onkeydown",
                "if(event.which || event.keyCode){{if ((event.which == 13) || (event.keyCode == 13)) {{document.getElementById('{0}').click();return false;}}}} else {{return true}}; "
                    .FormatWith(forumLogin.ClientID));

            if (this.Get<YafBoardSettings>().AllowSingleSignOn)
            {
                faceBookHolder.Visible = Config.FacebookAPIKey.IsSet() && Config.FacebookSecretKey.IsSet();
                twitterHolder.Visible = Config.TwitterConsumerKey.IsSet() && Config.TwitterConsumerSecret.IsSet();
                googleHolder.Visible = Config.GoogleClientID.IsSet() && Config.GoogleClientSecret.IsSet();

                if (twitterHolder.Visible)
                {
                    twitterRegister.Visible = true;
                    twitterRegister.Text = this.GetTextFormatted("AUTH_CONNECT", "Twitter");
                    twitterRegister.ToolTip = this.GetTextFormatted("AUTH_CONNECT_HELP", "Twitter");
                }

                if (faceBookHolder.Visible)
                {
                    facebookRegister.Visible = true;
                    facebookRegister.Text = this.GetTextFormatted("AUTH_CONNECT", "Facebook");
                    facebookRegister.ToolTip = this.GetTextFormatted("AUTH_CONNECT_HELP", "Facebook");
                }

                if (googleHolder.Visible)
                {
                    googleRegister.Visible = true;
                    googleRegister.Text = this.GetTextFormatted("AUTH_CONNECT", "Google");
                    googleRegister.ToolTip = this.GetTextFormatted("AUTH_CONNECT_HELP", "Google");
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
            YafBuildLink.Redirect(ForumPages.recoverpassword);
        }

        #endregion

        /// <summary>
        /// Handles the LoggedIn event of the Login1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Login1_LoggedIn(object sender, EventArgs e)
        {
            this.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(this.PageContext.PageUserID));

            LegacyDb.user_update_single_sign_on_status(this.PageContext.PageUserID, AuthService.none);
        }

        /// <summary>
        /// Redirects to the Facebook login/register page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void FacebookRegisterClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.login, "auth={0}", AuthService.facebook);
        }

        /// <summary>
        /// Redirects to the Twitter login/register page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void TwitterRegisterClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.login, "auth={0}", AuthService.twitter);
        }

        /// <summary>
        /// Redirects to the Google login/register page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GoogleRegisterClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.login, "auth={0}", AuthService.google);
        }
    }
}