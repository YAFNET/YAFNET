/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models.Identity;

/// <summary>
/// The login model.
/// </summary>
[AllowAnonymous]
public class AuthorizeModel : AccountPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeModel"/> class.
    /// </summary>
    public AuthorizeModel()
        : base("LOGIN", ForumPages.Account_Authorize)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Gets or sets the validation code.
    /// </summary>
    /// <value>The validation code.</value>
    [Required]
    [BindProperty]
    public string ValidationCode { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("LOGIN", "TITLE"));
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    public IActionResult OnGet()
    {
        var user = this.GetPendingUser();

        return user == null ? this.Get<ILinkBuilder>().Redirect(ForumPages.Account_Login) : this.Page();
    }

    /// <summary>
    /// Validate 2FA Code and if correct log user in
    /// </summary>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        var pendingUser = this.GetPendingUser();

        // the current lockout state of the user
        var user = pendingUser is null ? null : await this.Get<IAspNetUsersHelper>().GetUserAsync(pendingUser.Id);

        if (user is null)
        {
            this.Get<ISessionService>().ClearPageData();

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Account_Login);
        }

        // Locked out after too many failed attempts (password or code), don't even check the code
        if (user.LockoutEndDateUtc > DateTime.UtcNow)
        {
            return this.LockedOut();
        }

        if (!this.Get<ITwoFactorAuthService>().ValidatePin(user.MobilePIN, this.ValidationCode?.Trim() ?? string.Empty))
        {
            // wrong codes count like wrong passwords, otherwise the code could be guessed
            user.AccessFailedCount++;

            if (user.AccessFailedCount >= 10)
            {
                user.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(15);

                await this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);

                this.Get<ILogger<AuthorizeModel>>().Info(
                    $"User: {user.UserName} has reached the Limit of 10 failed login attempts and is locked out until {user.LockoutEndDateUtc}");

                return this.LockedOut();
            }

            await this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);

            return this.PageBoardContext.Notify(
                    this.GetText("EDIT_SETTINGS", "WRONG_CODE"),
                    MessageTypes.warning);
        }

        this.Get<ISessionService>().ClearPageData();

        // the "remember me" choice of the login (a session cookie without it)
        var rememberMe = this.HttpContext.Session.GetData<bool>(Constants.TwoFactorRememberMeSessionKey);

        this.HttpContext.Session.Remove(Constants.TwoFactorRememberMeSessionKey);

        return await this.Get<IAspNetUsersHelper>().SignInAsync(user, rememberMe);
    }

    /// <summary>
    /// Gets the user that entered the password and waits for the code.
    /// </summary>
    /// <returns>The user or null if there is none (other page data or an expired session).</returns>
    private AspNetUsers GetPendingUser()
    {
        try
        {
            return this.Get<ISessionService>().GetPageData<AspNetUsers>();
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// The user has to log in again after the lockout.
    /// </summary>
    /// <returns>IActionResult.</returns>
    private IActionResult LockedOut()
    {
        this.Get<ISessionService>().ClearPageData();

        return this.PageBoardContext.SessionNotify(
            this.GetText("LOGIN", "ERROR_LOCKEDOUT"),
            MessageTypes.danger,
            ForumPages.Account_Login);
    }
}