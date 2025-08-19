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

namespace YAF.Pages.Admin.EditUser;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Class UsersSuspendModel.
/// Implements the <see cref="YAF.Core.BasePages.AdminPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.AdminPage" />
public class UsersSuspendModel : AdminPage
{
    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    public Tuple<User, AspNetUsers, Rank, VAccess> EditUser { get; set; }

    /// <summary>
    /// Gets or sets the suspend units.
    /// </summary>
    /// <value>The suspend units.</value>
    [BindProperty]
    public List<SelectListItem> SuspendUnits { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersSuspendInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersSuspendModel"/> class.
    /// </summary>
    public UsersSuspendModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.Input = new UsersSuspendInputModel {
            UserId = userId
        };

        return this.BindData(userId);
    }

    /// <summary>
    /// Removes suspension from a user.
    /// </summary>
    public async Task<IActionResult> OnPostRemoveSuspensionAsync()
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] is not
            Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = this.Input.UserId
                });
        }

        this.EditUser = user;

        // un-suspend user
        await this.GetRepository<User>().SuspendAsync(this.Input.UserId);

        if (this.PageBoardContext.BoardSettings.LogUserSuspendedUnsuspended)
        {
            this.Get<ILogger<UsersSuspendModel>>().Log(
                this.PageBoardContext.PageUserID,
                "YAF.Controls.EditUsersSuspend",
                $"User {this.EditUser.Item1.DisplayOrUserName()} was un-suspended by {this.PageBoardContext.PageUser.DisplayOrUserName()}.",
                EventLogTypes.UserUnsuspended);
        }

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

        await this.Get<ISendNotification>().SendUserSuspensionEndedNotificationAsync(
            this.EditUser.Item1.Email,
            this.EditUser.Item1.DisplayOrUserName());

        this.PageBoardContext.SessionNotify(
            $"User {this.EditUser.Item1.DisplayOrUserName()} was un-suspended by {this.PageBoardContext.PageUser.DisplayOrUserName()}.",
            MessageTypes.success);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
    }

    /// <summary>
    /// Suspends a user when clicked.
    /// </summary>
    public async Task<IActionResult> OnPostSuspendAsync()
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] is not
            Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = this.Input.UserId
                });
        }

        this.EditUser = user;

        // is user to be suspended admin?
        if (this.EditUser.Item4.IsAdmin > 0)
        {
            // tell user he can't suspend admin
            this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "ERROR_ADMINISTRATORS"), MessageTypes.danger);

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
        }

        // is user to be suspended forum moderator, while user suspending him is not admin?
        if (!this.PageBoardContext.IsAdmin && this.EditUser.Item4.IsForumModerator > 0)
        {
            // tell user he can't suspend forum moderator when he's not admin
            this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "ERROR_FORUMMODERATORS"), MessageTypes.danger);

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
        }

        // verify the user isn't guest...
        if (this.EditUser.Item1.UserFlags.IsGuest)
        {
            this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "ERROR_GUESTACCOUNT"), MessageTypes.danger);

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
        }

        // time until when user is suspended
        var suspend = this.Get<IDateTimeService>().GetUserDateTime(DateTime.UtcNow, this.EditUser.Item1.TimeZoneInfo);

        // number inserted by suspending user
        var count = this.Input.SuspendCount;

        // what time units are used for suspending
        suspend = this.Input.SuspendUnit switch
        {
            // days
            1 =>
                // add user inserted suspension time to current time
                suspend.AddDays(count),
            // hours
            2 =>
                // add user inserted suspension time to current time
                suspend.AddHours(count),
            // minutes
            3 =>
                // add user inserted suspension time to current time
                suspend.AddHours(count),
            _ => suspend
        };

        // suspend user by calling appropriate method
        await this.GetRepository<User>().SuspendAsync(
            this.Input.UserId,
            suspend,
            this.Input.SuspendedReason,
            this.PageBoardContext.PageUserID);

        this.Get<ILogger<UsersSuspendModel>>().Log(
            this.PageBoardContext.PageUserID,
            "YAF.Controls.EditUsersSuspend",
            $"User {this.EditUser.Item1.DisplayOrUserName()} was suspended by {this.PageBoardContext.PageUser.DisplayOrUserName()} until: {suspend} (UTC)",
            EventLogTypes.UserSuspended);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

        await this.Get<ISendNotification>().SendUserSuspensionNotificationAsync(
            suspend,
            this.Input.SuspendedReason,
            this.EditUser.Item1.Email,
            this.EditUser.Item1.DisplayOrUserName());

        this.Input.SuspendedReason = string.Empty;

        // re-bind data
        this.BindData(this.Input.UserId);

        this.PageBoardContext.SessionNotify(
            $"User {this.EditUser.Item1.DisplayOrUserName()} was suspended by {this.PageBoardContext.PageUser.DisplayOrUserName()} until: {suspend} (UTC)",
            MessageTypes.success);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private IActionResult BindData(int userId)
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] is not Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = userId
                });
        }

        this.SuspendUnits = [
            new SelectListItem(this.GetText("PROFILE", "DAYS"), "1"),
            new SelectListItem(this.GetText("PROFILE", "HOURS"), "2"),
            new SelectListItem(this.GetText("PROFILE", "MINUTES"), "3")
        ];

        this.EditUser =
            user;

        return this.Page();
    }
}