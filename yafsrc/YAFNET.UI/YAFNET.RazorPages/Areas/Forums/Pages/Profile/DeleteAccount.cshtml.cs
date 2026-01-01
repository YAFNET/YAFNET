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

using Microsoft.AspNetCore.Identity;

namespace YAF.Pages.Profile;

using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// User Page To Delete (deactivate) his account
/// </summary>
public class DeleteAccountModel : ProfilePage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "DeleteAccountModel" /> class.
    /// </summary>
    public DeleteAccountModel(SignInManager<AspNetUsers> signInManager)
        : base("DELETE_ACCOUNT", ForumPages.Profile_DeleteAccount)
    {
        this.signInManager = signInManager;
    }

    /// <summary>
    /// The _sign in manager.
    /// </summary>
    private readonly SignInManager<AspNetUsers> signInManager;

    /// <summary>
    /// Gets or sets the options.
    /// </summary>
    public string[] Options { get; set; } = ["suspend", "delete"];

    /// <summary>
    /// Gets or sets the option.
    /// </summary>
    [BindProperty]
    public string Option { get; set; } = "suspend";

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));

        this.PageBoardContext.PageLinks.AddLink(
            string.Format(this.GetText("DELETE_ACCOUNT", "TITLE"), this.PageBoardContext.BoardSettings.Name),
            string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        return this.PageBoardContext.PageUser.UserFlags.IsHostAdmin
            ? this.Get<ILinkBuilder>().AccessDenied()
            : this.Page();
    }

    /// <summary>
    /// Delete or Suspend User
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        switch (this.Option)
        {
            case "suspend":
            {
                // Suspend User for 30 Days
                // time until when user is suspended
                var suspend = this.Get<IDateTimeService>().GetUserDateTime(
                    DateTime.UtcNow,
                    this.PageBoardContext.TimeZoneInfoUser).AddDays(30);

                // suspend user by calling appropriate method
                await this.GetRepository<User>().SuspendAsync(
                    this.PageBoardContext.PageUserID,
                    suspend,
                    "User Suspended his own account",
                    this.PageBoardContext.PageUserID);

                var user = await this.GetRepository<User>().GetByIdAsync(
                    this.PageBoardContext.PageUserID);

                if (user != null)
                {
                    this.Get<ILogger<DeleteAccountModel>>().Log(
                        this.PageBoardContext.PageUserID,
                        this,
                        $"User {user.DisplayOrUserName()} Suspended his own account until: {suspend} (UTC)",
                        EventLogTypes.UserSuspended);

                    this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));
                }
            }

                break;
            case "delete":
            {
                // (Soft) Delete User
                var user = this.PageBoardContext.MembershipUser;

                // Update IsApproved
                user.IsApproved = false;

                await this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);

                var userFlags = this.PageBoardContext.PageUser.UserFlags;

                userFlags.IsDeleted = true;
                userFlags.IsApproved = false;

                await this.GetRepository<User>().UpdateOnlyAsync(
                    () => new User { Flags = userFlags.BitValue },
                    u => u.ID == this.PageBoardContext.PageUserID);

                // delete posts...
                var messages = this.GetRepository<Message>().GetAllUserMessages(this.PageBoardContext.PageUserID);

                foreach (var x in messages)
                {
                    await this.GetRepository<Message>().DeleteAsync(
                        x.Topic.ForumID,
                        x.TopicID,
                        x,
                        true,
                        string.Empty,
                        true,
                        true);
                }

                this.Get<ILogger<DeleteAccountModel>>().UserDeleted(
                    this.PageBoardContext.PageUserID,
                    $"User {this.PageBoardContext.PageUser.DisplayOrUserName()} Deleted his own account");
            }

                break;
        }

        await this.signInManager.SignOutAsync();

        await this.Get<IAspNetUsersHelper>().SignOutAsync();

        this.Get<IRaiseEvent>().Raise(new UserLogoutEvent(this.PageBoardContext.PageUserID));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
    }
}