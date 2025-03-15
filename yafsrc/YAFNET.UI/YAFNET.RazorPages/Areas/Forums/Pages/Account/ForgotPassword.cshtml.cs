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

using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Authorization;

using YAF.Core.Extensions;
using YAF.Types.Interfaces.Identity;

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
    /// Initializes a new instance of the <see cref="ForgotPasswordModel"/> class.
    /// </summary>
    public ForgotPasswordModel()
        : base("RECOVER_PASSWORD", ForumPages.Account_ForgotPassword)
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
    /// The on post async.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        var user = this.Input.UserName.Contains('@')
                       ? await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(this.Input.UserName)
                       : await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(this.Input.UserName);

        if (user is null)
        {
            return this.PageBoardContext.Notify(this.GetText("USERNAME_FAILURE"), MessageTypes.danger);
        }

        // verify the user is approved, etc...
        if (!user.IsApproved)
        {
            // explain they are not approved yet...
            return this.PageBoardContext.Notify(this.GetTextFormatted("ACCOUNT_NOT_APPROVED_VERIFICATION", user.Email), MessageTypes.warning);
        }

        var code = HttpUtility.UrlEncode(
            await this.Get<IAspNetUsersHelper>().GeneratePasswordResetTokenAsync(user),
            Encoding.UTF8);

        await this.Get<ISendNotification>().SendPasswordResetAsync(user, code);

        this.PageBoardContext.SessionNotify(this.GetText("SUCCESS"), MessageTypes.success);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
    }
}