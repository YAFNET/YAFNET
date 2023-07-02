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

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Authorization;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
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
    public InputModel Input { get; set; }

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
    public async Task OnGetAsync(string code = null)
    {
        this.Input = new InputModel();

        if (code.IsSet())
        {
            this.Input.Key = code;
            await this.ValidateKeyAsync();
        }
        else
        {
            this.ErrorMessage = this.GetText("email_verify_failed");
        }
    }

    /// <summary>
    /// The on post async.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task OnPostAsync()
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
        var userEmail = this.GetRepository<CheckEmail>().Update(this.Input.Key);

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

            // tell the provider to update...
            await this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);

            this.GetRepository<User>().Approve(userEmail.UserID);

            this.GetRepository<CheckEmail>().DeleteById(userEmail.ID);

            // automatically log in created users
            await this.Get<IAspNetUsersHelper>().SignInAsync(user);

            // now redirect to main site...
            this.PageBoardContext.SessionNotify(this.GetText("EMAIL_VERIFIED"), MessageTypes.info);

            // default redirect -- because if may not want to redirect to login.
            return this.Get<LinkBuilder>().Redirect(ForumPages.Index);
        }

        this.ErrorMessage = result.Errors.FirstOrDefault()?.Description;

        return this.Page();
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }
    }
}