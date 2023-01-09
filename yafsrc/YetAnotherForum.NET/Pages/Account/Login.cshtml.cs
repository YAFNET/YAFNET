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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Account;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using MimeKit;

using YAF.Core.Extensions;
using YAF.Core.Identity.Owin;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

using DataType = System.ComponentModel.DataAnnotations.DataType;
using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;

/// <summary>
/// The login model.
/// </summary>
[AllowAnonymous]
public class LoginModel : AccountPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginModel"/> class.
    /// </summary>
    public LoginModel()
        : base("LOGIN", ForumPages.Account_Login)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [TempData]
    public string ErrorMessage { get; set; }

    [TempData]
    public bool ShowNotApproved { get; set; }

    [TempData]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("LOGIN","TITLE"));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task OnGetAsync()
    {
        if (this.ErrorMessage.IsSet())
        {
            this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handle External Login
    /// </summary>
    /// <param name="auth">
    /// The provider name.
    /// </param>
    /// <param name="handler">
    /// The handler.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> OnGetCallbackAsync(string auth = null, string handler = null)
    {
        if (auth.IsSet() && handler.IsSet() && this.PageBoardContext.BoardSettings.AllowSingleSignOn)
        {
            return await this.HandleExternalLoginAsync(auth);
        }

        return this.Get<ISessionService>().InfoMessage.IsSet() ? this.PageBoardContext.Notify(this.Get<ISessionService>().InfoMessage, MessageTypes.danger) : this.Page();
    }

    /// <summary>
    /// The on post async.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var user = this.Get<IAspNetUsersHelper>().ValidateUser(this.Input.UserName.Trim());

        if (user == null)
        {
            return this.PageBoardContext.Notify(this.GetText("PASSWORD_ERROR"), MessageTypes.danger);
        }

        if (!user.IsApproved)
        {
            var yafUser = this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(user.Id);

            // Ignore Deleted User
            if (yafUser.UserFlags.IsDeleted)
            {
                return this.Page();
            }

            this.Email = user.Email;
            this.ShowNotApproved = true;

            return this.PageBoardContext.Notify(this.GetText("LOGIN","ACCOUNT_NOT_APPROVED"), MessageTypes.warning);
        }

        // Valid user, verify password
        var result = this.Get<IAspNetUsersHelper>().IPasswordHasher
            .VerifyHashedPassword(user, user.PasswordHash, this.Input.Password);

        switch (result)
        {
            case PasswordVerificationResult.Success:
                await this.Get<IAspNetUsersHelper>().SignInAsync(user, this.Input.RememberMe);

               return this.Get<LinkBuilder>().Redirect(ForumPages.Index);

            case PasswordVerificationResult.SuccessRehashNeeded:
                user.PasswordHash = this.Get<IAspNetUsersHelper>().IPasswordHasher
                    .HashPassword(user, this.Input.Password);

                await this.Get<IAspNetUsersHelper>().SignInAsync(user, this.Input.RememberMe);

                return this.Get<LinkBuilder>().Redirect(ForumPages.Index);

            default:
            {
                // Lockout for 15 minutes if more than 10 failed attempts
                user.AccessFailedCount++;
                if (user.AccessFailedCount >= 10)
                {
                    user.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(15);

                    this.Get<ILogger<LoginModel>>().Info(
                        $"User: {user.UserName} has reached the Limit of 10 failed login attempts and is locked out until {user.LockoutEndDateUtc}");

                    return this.PageBoardContext.Notify(this.GetText("LOGIN", "ERROR_LOCKEDOUT"), MessageTypes.danger);
                }

                this.Get<IAspNetUsersHelper>().Update(user);

                this.Get<ILogger<LoginModel>>().Log(
                    null,
                    $"Login Failure: {this.Input.UserName.Trim()}",
                    $"Login Failure for User: {this.Input.UserName.Trim()} with the IP Address: {this.Request.HttpContext.Connection.LocalIpAddress}",
                    EventLogTypes.LoginFailure);

                return this.PageBoardContext.Notify(this.GetText("PASSWORD_ERROR"), MessageTypes.danger);
            }
        }
    }

    /// <summary>
    /// Go to Register Page.
    /// </summary>
    public IActionResult OnPostRegister()
    {
        return this.Get<LinkBuilder>().Redirect(
            this.PageBoardContext.BoardSettings.ShowRulesForRegistration
                ? ForumPages.Privacy
                : ForumPages.Account_Register);
    }

    /// <summary>
    /// External Login
    /// </summary>
    /// <param name="auth">
    /// The Authentication Provider.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<ActionResult> OnPostAuthAsync(string auth)
    {
        var redirectUrl = this.Get<LinkBuilder>().GetLink(
            ForumPages.Account_Login,
            new { auth, handler = "Callback" });

        var properties = this.Get<SignInManager<AspNetUsers>>()
            .ConfigureExternalAuthenticationProperties(auth, redirectUrl);

        ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault |
                                               SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        return Task.FromResult<ActionResult>(new ChallengeResult(auth, properties));
    }

    /// <summary>
    /// Handles the OnClick event of the ResendConfirm control.
    /// </summary>
    protected Task<ActionResult> OnPostResendConfirmAsync()
    {
        if (this.Email == null)
        {
            this.ShowNotApproved = false;
            this.Email = null;

            return Task.FromResult<ActionResult>(this.Page());
        }

        var checkMail = this.GetRepository<CheckEmail>().ListTyped(this.Email).FirstOrDefault();

        if (checkMail == null)
        {
            this.ShowNotApproved = false;
            this.Email = null;

            return Task.FromResult<ActionResult>(this.Page());
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

        verifyEmail.SendEmail(new MailboxAddress(this.Input.UserName, checkMail.Email), subject);

        return Task.FromResult<ActionResult>(this.PageBoardContext.Notify(
            this.GetText("LOGIN", "MSG_MESSAGE_SEND"),
            MessageTypes.success));
    }

    /// <summary>
    /// handle external login if provider exist in Query String
    /// </summary>
    /// <param name="auth">
    /// The Provider Name.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    private async Task<IActionResult> HandleExternalLoginAsync(string auth)
    {
        var loginAuth = auth.ToEnum<AuthService>();

        var message = string.Empty;

        AspNetUsers user = null;

        switch (loginAuth)
        {
            case AuthService.twitter:
            {
                var twitterAuth = new Twitter();
                user = twitterAuth.LoginOrCreateUser(out message);
            }

                break;
            case AuthService.facebook:
            {
                var facebookAuth = new Facebook();
                user = facebookAuth.LoginOrCreateUser(out message);
            }

                break;
            case AuthService.google:
            {
                var googleAuth = new Google();
                user = googleAuth.LoginOrCreateUser(out message);
            }

                break;
        }

        if (message.IsSet() && user != null)
        {
            return this.PageBoardContext.Notify(message, MessageTypes.warning);
        }
        else
        {
            return await this.Get<IAspNetUsersHelper>().SignInExternalAsync(user);
        }
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember me.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}