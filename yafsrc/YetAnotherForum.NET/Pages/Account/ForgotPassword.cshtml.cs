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

using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The recover Password Page.
/// </summary>
/// <summary>
/// The login model.
/// </summary>
[AllowAnonymous]
public class ForgotPasswordModel : AccountPage
{
    /// <summary>
    /// How many emails (password reset or verification) an account gets in the time window,
    /// the page can be used by anybody (otherwise the inbox of a user could be flooded).
    /// </summary>
    private const int MaxEmailsPerWindow = 3;

    /// <summary>
    /// The time window of the email limit.
    /// </summary>
    private readonly static TimeSpan EmailWindow = TimeSpan.FromHours(1);

    /// <summary>
    /// The memory cache that keeps the sent emails of the accounts.
    /// </summary>
    private readonly IMemoryCache memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordModel"/> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache.</param>
    public ForgotPasswordModel(IMemoryCache memoryCache)
        : base("RECOVER_PASSWORD", ForumPages.Account_ForgotPassword)
    {
        this.memoryCache = memoryCache;
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public ForgotPasswordInputModel Input { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("RECOVER_PASSWORD","TITLE"));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        return this.Page();
    }

    /// <summary>
    /// Sends the password reset email (or a new verification email when the email is not verified yet).
    /// The page can be used by anybody: the answer is the same for every entry, it does not tell
    /// which user names or email addresses are registered.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        var userName = this.Input?.UserName?.Trim();

        var user = userName.IsNotSet()
                       ? null
                       : userName.Contains('@')
                           ? await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(userName)
                           : await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(userName);

        // over the limit nothing is sent (otherwise the inbox of the user could be flooded)
        if (user is not null
            && SendLimitHelper.TryCount(this.memoryCache, $"AccountEmail_{user.Id}", MaxEmailsPerWindow, EmailWindow))
        {
            if (user.IsApproved)
            {
                var code = HttpUtility.UrlEncode(
                    await this.Get<IAspNetUsersHelper>().GeneratePasswordResetTokenAsync(user),
                    Encoding.UTF8);

                await this.Get<ISendNotification>().SendPasswordResetAsync(user, code);
            }
            else
            {
                await this.SendNewVerificationEmailAsync(user);
            }
        }

        this.PageBoardContext.SessionNotify(this.GetText("SUCCESS_IF_EXISTS"), MessageTypes.success);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
    }

    /// <summary>
    /// Sends a new verification email when the email of the user is not verified yet
    /// (users waiting for the approval of an admin, e.g. possible spam bots, get no email).
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The <see cref="Task"/>.</returns>
    private async Task SendNewVerificationEmailAsync(AspNetUsers user)
    {
        var forumUser = await this.GetRepository<User>().GetUserByProviderKeyAsync(this.PageBoardContext.PageBoardID, user.Id);

        if (forumUser is null
            || !await this.GetRepository<CheckEmail>().ExistsAsync(c => c.UserID == forumUser.ID))
        {
            return;
        }

        // the new code replaces the old one
        await this.GetRepository<CheckEmail>().DeleteAsync(c => c.UserID == forumUser.ID);

        await this.Get<ISendNotification>().SendVerificationEmailAsync(user, user.Email, forumUser.ID);
    }
}