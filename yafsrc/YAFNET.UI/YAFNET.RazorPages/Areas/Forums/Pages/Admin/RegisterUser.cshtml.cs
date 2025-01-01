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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Admin;

using System.Linq;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The Admin Page for manually user registration
/// </summary>
public class RegisterUserModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public RegisterUserInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserModel"/> class.
    /// </summary>
    public RegisterUserModel()
        : base("ADMIN_REGUSER", ForumPages.Admin_RegisterUser)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_USERS", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Users));

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_REGUSER", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Click event of the ForumRegister control.
    /// </summary>
    public async Task<IActionResult> OnPostForumRegisterAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var newEmail = this.Input.Email.Trim();
        var newUsername = this.Input.UserName.Trim();

        if (!ValidationHelper.IsValidEmail(newEmail))
        {
            return this.PageBoardContext.Notify(this.GetText("ADMIN_REGUSER", "MSG_INVALID_MAIL"), MessageTypes.danger);
        }

        if (this.Get<IAspNetUsersHelper>().UserExists(this.Input.UserName.Trim(), newEmail))
        {
            return this.PageBoardContext.Notify(this.GetText("ADMIN_REGUSER", "MSG_NAME_EXISTS"), MessageTypes.danger);
        }

        var user = new AspNetUsers {
            Id = Guid.NewGuid().ToString(),
            ApplicationId = this.PageBoardContext.BoardSettings.ApplicationId,
            UserName = newUsername,
            LoweredUserName = newUsername.ToLower(),
            Email = newEmail,
            LoweredEmail = newEmail.ToLower(),
            IsApproved = false,
            EmailConfirmed = false
        };

        var result = await this.Get<IAspNetUsersHelper>().CreateUserAsync(user, this.Input.Password);

        if (!result.Succeeded)
        {
            // error of some kind
            return this.PageBoardContext.Notify(result.Errors.FirstOrDefault()?.Description, MessageTypes.danger);
        }

        // setup initial roles (if any) for this user
        await this.Get<IAspNetRolesHelper>().SetupUserRolesAsync(this.PageBoardContext.PageBoardID, user);

        // create the user in the YAF DB as well as sync roles...
        var userId = await this.Get<IAspNetRolesHelper>().CreateForumUserAsync(user, this.PageBoardContext.PageBoardID);

        var autoWatchTopicsEnabled = this.PageBoardContext.BoardSettings.DefaultNotificationSetting
            .Equals(UserNotificationSetting.TopicsIPostToOrSubscribeTo);

        await this.Get<ISendNotification>().SendVerificationEmailAsync(user, newEmail, userId.Value, newUsername);

        this.GetRepository<User>().SaveNotification(
            this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(user.Id).ID,
            autoWatchTopicsEnabled,
            this.PageBoardContext.BoardSettings.DefaultNotificationSetting.ToInt(),
            this.PageBoardContext.BoardSettings.DefaultSendDigestEmail);

        // success
        this.PageBoardContext.SessionNotify(
            this.GetTextFormatted("MSG_CREATED", this.Input.UserName),
            MessageTypes.success);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Users);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.Input = new RegisterUserInputModel();
    }
}