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

namespace YAF.Pages.Profile;

using System.ComponentModel.DataAnnotations;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Interfaces.Identity;

using DataType = System.ComponentModel.DataAnnotations.DataType;

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
    public InputModel Input { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        if (!(this.PageBoardContext.IsAdmin || this.PageBoardContext.IsForumModerator))
        {
            // Not accessible...
            return this.Get<LinkBuilder>().AccessDenied();
        }

        return this.Page();
    }

    /// <summary>
    /// Change Password
    /// </summary>
    public void OnPost()
    {
        if (!this.ModelState.IsValid)
        {
            return;
        }

        var result = this.Get<IAspNetUsersHelper>().ChangePassword(
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

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the confirm new password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}