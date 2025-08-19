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

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Authorization;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// The User Account Verification Page.
/// </summary>
[AllowAnonymous]
public class ApproveModel : AccountPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApproveModel"/> class.
    /// </summary>
    public ApproveModel()
        : base("APPROVE", ForumPages.Account_Approve)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public ApproveInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [TempData]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="code">
    /// The code.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<IActionResult> OnGetAsync(string code)
    {
        this.Input = new ApproveInputModel();

        this.ErrorMessage = string.Empty;

        this.Input.Key = code;

        return this.ValidateKeyAsync();
    }

    /// <summary>
    /// The on post async.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<IActionResult> OnPostAsync()
    {
        return this.ValidateKeyAsync();
    }

    /// <summary>
    /// The validate key.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    private async Task<IActionResult> ValidateKeyAsync()
    {
        var userEmail = await this.GetRepository<CheckEmail>().UpdateAsync(this.Input.Key);

        if (userEmail is null)
        {
            this.ErrorMessage = this.GetText("email_verify_failed");

            return this.Page();
        }

        var user = await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(userEmail.Email);

        var result = await this.Get<IAspNetUsersHelper>().ConfirmUserEmailAsync(user, HttpUtility.UrlDecode(this.Input.Key, Encoding.UTF8));

        if (result.Succeeded)
        {
            user.IsApproved = true;
            user.EmailConfirmed = true;

            await this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);

            await this.GetRepository<User>().ApproveAsync(userEmail.UserID);

            await this.GetRepository<CheckEmail>().DeleteByIdAsync(userEmail.ID);

            this.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userEmail.UserID));

            await this.Get<IAspNetUsersHelper>().SignInAsync(user);

            this.PageBoardContext.SessionNotify(this.GetText("EMAIL_VERIFIED"), MessageTypes.info);

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
        }

        this.ErrorMessage = result.Errors.FirstOrDefault()?.Description;

        return this.Page();
    }
}