/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using MimeKit;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

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

    /// <summary>
    /// Gets or sets a value indicating whether [show not approved].
    /// </summary>
    /// <value><c>true</c> if [show not approved]; otherwise, <c>false</c>.</value>
    [TempData]
    public bool ShowNotApproved { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>The email.</value>
    [TempData]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public LoginInputModel Input { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("LOGIN", "TITLE"));
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

        this.Input = new LoginInputModel();

        return Task.CompletedTask;
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

        var user = await this.Get<IAspNetUsersHelper>().ValidateUserAsync(this.Input.UserName.Trim());

        if (user is null)
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

            return this.PageBoardContext.Notify(this.GetText("LOGIN", "ACCOUNT_NOT_APPROVED"), MessageTypes.warning);
        }

        // Valid user, verify password
        var result = this.Get<IAspNetUsersHelper>().PasswordHasher
            .VerifyHashedPassword(user, user.PasswordHash, this.Input.Password);

        switch (result)
        {
            case PasswordVerificationResult.Success:
                return await this.SignInAsync(user);

            case PasswordVerificationResult.SuccessRehashNeeded:
                user.PasswordHash = this.Get<IAspNetUsersHelper>().PasswordHasher
                    .HashPassword(user, this.Input.Password);

                return await this.SignInAsync(user);

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

                await this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);

                this.Get<ILogger<LoginModel>>().Log(
                    null,
                    $"Login Failure: {this.Input.UserName.Trim()}",
                    $"Login Failure for User: {this.Input.UserName.Trim()} with the IP Address: {this.Request.GetUserRealIPAddress()}",
                    EventLogTypes.LoginFailure);

                return this.PageBoardContext.Notify(this.GetText("PASSWORD_ERROR"), MessageTypes.danger);
            }
        }
    }

    /// <summary>
    /// Sign in user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> SignInAsync(AspNetUsers user)
    {
        if (!user.TwoFactorEnabled)
        {
            return await this.Get<IAspNetUsersHelper>().SignInAsync(user, this.Input.RememberMe);
        }

        this.Get<ISessionService>().SetPageData(user);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Account_Authorize);
    }

    /// <summary>
    /// Go to Register Page.
    /// </summary>
    public IActionResult OnPostRegister()
    {
        return this.Get<ILinkBuilder>().Redirect(
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
        var redirectUrl = this.Get<ILinkBuilder>().GetLink(ForumPages.Account_Login, new { auth, handler = "Callback" });

        var properties = this.Get<SignInManager<AspNetUsers>>()
            .ConfigureExternalAuthenticationProperties(auth, redirectUrl);

        return Task.FromResult<ActionResult>(new ChallengeResult(auth, properties));
    }

    /// <summary>
    /// Handles the OnClick event of the ResendConfirm control.
    /// </summary>
    async protected Task<ActionResult> OnPostResendConfirmAsync()
    {
        if (this.Email.IsNotSet())
        {
            this.ShowNotApproved = false;
            this.Email = null;

            return this.Page();
        }

        var checkMail = await this.GetRepository<CheckEmail>().GetSingleAsync(mail => mail.Email == this.Email);

        if (checkMail is null)
        {
            this.ShowNotApproved = false;
            this.Email = null;

            return this.Page();
        }

        var verifyEmail = new TemplateEmail("VERIFYEMAIL");

        var subject = this.Get<ILocalization>().GetTextFormatted(
            "VERIFICATION_EMAIL_SUBJECT",
            this.PageBoardContext.BoardSettings.Name);

        verifyEmail.TemplateParams["{link}"] = this.Get<ILinkBuilder>().GetAbsoluteLink(
            ForumPages.Account_Approve,
            new { code = checkMail.Hash });
        verifyEmail.TemplateParams["{key}"] = checkMail.Hash;
        verifyEmail.TemplateParams["{forumname}"] = this.PageBoardContext.BoardSettings.Name;
        verifyEmail.TemplateParams["{forumlink}"] = this.Get<ILinkBuilder>().ForumUrl;

        await verifyEmail.SendEmailAsync(new MailboxAddress(this.Input.UserName, checkMail.Email), subject);

        return this.PageBoardContext.Notify(this.GetText("LOGIN", "MSG_MESSAGE_SEND"), MessageTypes.success);
    }
}