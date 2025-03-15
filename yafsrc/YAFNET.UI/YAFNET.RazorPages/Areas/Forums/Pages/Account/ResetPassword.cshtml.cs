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
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;

/// <summary>
/// The recover Password Page.
/// </summary>
/// <summary>
/// The login model.
/// </summary>
[AllowAnonymous]
public class ResetPasswordModel : AccountPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordModel"/> class.
    /// </summary>
    public ResetPasswordModel()
        : base("ACCOUNT_RESEST_PASSWORD", ForumPages.Account_ResetPassword)
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
    public ResetPasswordInputModel Input { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ACCOUNT_RESEST_PASSWORD", "TITLE"));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="code">
    /// The code.
    /// </param>
    public IActionResult OnGet(string code)
    {
        return code.IsSet() ? this.Page() : this.Get<ILinkBuilder>().AccessDenied();
    }

    /// <summary>
    /// The on post async.
    /// </summary>
    /// <param name="code">
    /// The code.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> OnPostAsync(string code)
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var user = await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(this.Input.Email);

        if (user is null)
        {
            return this.PageBoardContext.Notify(this.GetText("USERNAME_FAILURE"), MessageTypes.danger);
        }

        var result = await this.Get<IAspNetUsersHelper>()
            .ResetPasswordAsync(user, HttpUtility.UrlDecode(code, Encoding.UTF8), this.Input.Password);

        if (!result.Succeeded)
        {
            return this.PageBoardContext.Notify(result.Errors.FirstOrDefault()?.Description, MessageTypes.danger);
        }

        // Get User again to get updated Password Hash
        user = await this.Get<IAspNetUsersHelper>().GetUserAsync(user.Id);

        await this.Get<IAspNetUsersHelper>().SignInAsync(user);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
    }
}