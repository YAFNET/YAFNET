/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Pages.Account;

using System.Net.Mail;

using YAF.Core.Identity.Owin;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Web.Controls;

using Config = YAF.Configuration.Config;

/// <summary>
/// The Forum Login Page.
/// </summary>
public partial class Login : AccountPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="Login" /> class.
    /// </summary>
    public Login()
        : base("LOGIN", ForumPages.Account_Login)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.Get<HttpRequestBase>().QueryString.Exists("auth")
            && this.PageBoardContext.BoardSettings.AllowSingleSignOn)
        {
            this.HandleExternalLogin();
        }

        if (this.IsPostBack)
        {
            return;
        }

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "loadLoginValidatorFormJs",
            JavaScriptBlocks.FormValidatorJs(this.LoginButton.ClientID));

        this.UserName.Focus();

        this.RememberMe.Text = this.GetText("auto");

        this.Password.Attributes.Add(
            "onkeydown",
            JavaScriptBlocks.ClickOnEnterJs(this.LoginButton.ClientID));

        if (this.PageBoardContext.IsGuest && !this.PageBoardContext.BoardSettings.DisableRegistrations && !Config.IsAnyPortal)
        {
            this.RegisterLink.Visible = true;
        }

        this.OpenAuthProvidersHolder.Visible = this.PageBoardContext.BoardSettings.AllowSingleSignOn;

        this.DataBind();
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("title"));
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
        this.Get<LinkBuilder>().Redirect(ForumPages.Account_ForgotPassword);
    }

    /// <summary>
    /// Redirects to the Register Page
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void RegisterLinkClick(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(
            this.PageBoardContext.BoardSettings.ShowRulesForRegistration ? ForumPages.RulesAndPrivacy : ForumPages.Account_Register);
    }

    /// <summary>
    /// The sign in.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SignIn(object sender, EventArgs e)
    {
        if (!this.Page.IsValid)
        {
            return;
        }

        var user = this.Get<IAspNetUsersHelper>().ValidateUser(this.UserName.Text.Trim());

        if (user == null)
        {
            this.PageBoardContext.Notify(this.GetText("PASSWORD_ERROR"), MessageTypes.danger);
            return;
        }

        if (!user.IsApproved)
        {
            var yafUser = this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(user.Id);

            // Ignore Deleted User
            if (yafUser.UserFlags.IsDeleted)
            {
                return;
            }

            this.ResendConfirm.CommandArgument = $"{user.Email};{user.UserName}";
            this.NotApprovedHolder.Visible = true;
            return;
        }

        // Valid user, verify password
        var result = (PasswordVerificationResult)this.Get<IAspNetUsersHelper>().IPasswordHasher
            .VerifyHashedPassword(user.PasswordHash, this.Password.Text);

        switch (result)
        {
            case PasswordVerificationResult.Success:
                this.UserAuthenticated(user);
                break;
            case PasswordVerificationResult.SuccessRehashNeeded:
                user.PasswordHash = this.Get<IAspNetUsersHelper>().IPasswordHasher.HashPassword(this.Password.Text);
                this.UserAuthenticated(user);
                break;
            default:
                {
                    // Lockout for 15 minutes if more than 10 failed attempts
                    user.AccessFailedCount++;
                    if (user.AccessFailedCount >= 10)
                    {
                        user.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(15);

                        this.Logger.Info(
                            $"User: {user.UserName} has reached the Limit of 10 failed login attempts and is locked out until {user.LockoutEndDateUtc}");

                        this.PageBoardContext.LoadMessage.AddSession(
                            this.GetText("LOGIN", "ERROR_LOCKEDOUT"),
                            MessageTypes.danger);
                    }

                    this.Get<IAspNetUsersHelper>().Update(user);

                    this.Logger.Log(
                        $"Login Failure for User {this.UserName.Text.Trim()} with the IP Address {this.Get<HttpRequestBase>().GetUserRealIPAddress()}",
                        EventLogTypes.LoginFailure,
                        null,
                        $"Login Failure: {this.UserName.Text.Trim()}");

                    this.PageBoardContext.Notify(this.GetText("PASSWORD_ERROR"), MessageTypes.danger);
                    break;
                }
        }
    }

    /// <summary>
    /// Handles the OnClick event of the ResendConfirm control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void ResendConfirmClick(object sender, EventArgs e)
    {
        var commandArgument = sender.ToType<ThemeButton>().CommandArgument.Split(';');

        var checkMail = this.GetRepository<CheckEmail>().ListTyped(commandArgument[0]).FirstOrDefault();

        if (checkMail == null)
        {
            return;
        }

        var verifyEmail = new TemplateEmail("VERIFYEMAIL");

        var subject = this.Get<ILocalization>()
            .GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", this.PageBoardContext.BoardSettings.Name);

        verifyEmail.TemplateParams["{link}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
            ForumPages.Account_Approve,
            new { code = checkMail.Hash });
        verifyEmail.TemplateParams["{key}"] = checkMail.Hash;
        verifyEmail.TemplateParams["{forumname}"] = this.PageBoardContext.BoardSettings.Name;
        verifyEmail.TemplateParams["{forumlink}"] = this.Get<LinkBuilder>().ForumUrl;

        verifyEmail.SendEmail(new MailAddress(checkMail.Email, commandArgument[1]), subject);

        this.PageBoardContext.Notify(
            this.GetText("LOGIN", "MSG_MESSAGE_SEND"),
            MessageTypes.success);
    }

    /// <summary>
    /// The user authenticated.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    private void UserAuthenticated(AspNetUsers user)
    {
        this.Get<IAspNetUsersHelper>().SignIn(user, this.RememberMe.Checked);

        if (this.Get<HttpRequestBase>().QueryString.Exists("ReturnUrl"))
        {
            var returnUrl = this.HtmlEncode(this.Server.UrlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("ReturnUrl")));

            this.Page.Response.Redirect(returnUrl);
        }
        else
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Board);
        }
    }

    /// <summary>
    ///  handle external login if provider exist in Query String
    /// </summary>
    private void HandleExternalLogin()
    {
        var providerName = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<string>("auth");

        var loginAuth = (AuthService)Enum.Parse(typeof(AuthService), providerName, true);

        var message = string.Empty;

        switch (loginAuth)
        {
            case AuthService.facebook:
                {
                    var facebookAuth = new Facebook();
                    facebookAuth.LoginOrCreateUser(out message);
                }

                break;
            case AuthService.google:
                {
                    var googleAuth = new Google();
                    googleAuth.LoginOrCreateUser(out message);
                }

                break;
            /* case AuthService.github:
                 {
                     var gitHubAccountAuth = new GitHub();
                     gitHubAccountAuth.LoginOrCreateUser(out message);
                 }

                 break;*/
        }

        if (message.IsSet())
        {
            this.PageBoardContext.Notify(message, MessageTypes.warning);
        }
        else
        {
            this.Get<IAspNetUsersHelper>().SignInExternal();
        }
    }
}