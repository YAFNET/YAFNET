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

namespace YAF.Pages.Profile;

using System.Linq;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Interfaces.Identity;

/// <summary>
/// The Change Password Page.
/// </summary>
public class ChangePasswordModel : ProfilePage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "ChangePasswordModel" /> class.
    /// </summary>
    public ChangePasswordModel()
        : base("CHANGE_PASSWORD", ForumPages.Profile_ChangePassword)
    {
    }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public ChangePasswordInputModel Input { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(
            this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"));
    }

    /// <summary>
    /// Change Password
    /// </summary>
    public async Task OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return;
        }

        var result = await this.Get<IAspNetUsersHelper>().ChangePasswordAsync(
                         this.PageBoardContext.MembershipUser,
                         this.Input.Password,
                         this.Input.NewPassword);

        if (result.Succeeded)
        {
            this.PageBoardContext.Notify(this.GetText("CHANGE_SUCCESS"), MessageTypes.success);
        }
        else
        {
            this.PageBoardContext.Notify(result.Errors.FirstOrDefault()!.Description, MessageTypes.danger);
        }
    }
}