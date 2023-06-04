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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

public class UsersSuspendModel : AdminPage
{
    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    [BindProperty]
    public Tuple<User, AspNetUsers, Rank, VAccess> EditUser { get; set; }

    [BindProperty]
    public List<SelectListItem> SuspendUnits { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    public UsersSuspendModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.Input = new InputModel
                     {
                         UserId = userId
                     };

        this.BindData(userId);

        return this.Page();
    }

    /// <summary>
    /// Removes suspension from a user.
    /// </summary>
    public IActionResult OnPostRemoveSuspension()
    {
        this.EditUser =
            this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] as
                Tuple<User, AspNetUsers, Rank, VAccess>;

        // un-suspend user
        this.GetRepository<User>().Suspend(this.Input.UserId);

        if (this.PageBoardContext.BoardSettings.LogUserSuspendedUnsuspended)
        {
            this.Get<ILogger<UsersSuspendModel>>().Log(
                this.PageBoardContext.PageUserID,
                "YAF.Controls.EditUsersSuspend",
                $"User {this.EditUser.Item1.DisplayOrUserName()} was un-suspended by {this.PageBoardContext.PageUser.DisplayOrUserName()}.",
                EventLogTypes.UserUnsuspended);
        }

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

        this.Get<ISendNotification>().SendUserSuspensionEndedNotification(
            this.EditUser.Item1.Email,
            this.EditUser.Item1.DisplayOrUserName());

        this.PageBoardContext.SessionNotify(
            $"User {this.EditUser.Item1.DisplayOrUserName()} was un-suspended by {this.PageBoardContext.PageUser.DisplayOrUserName()}.",
            MessageTypes.success);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
    }

    /// <summary>
    /// Suspends a user when clicked.
    /// </summary>
    public IActionResult OnPostSuspend()
    {
        this.EditUser =
            this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] as
                Tuple<User, AspNetUsers, Rank, VAccess>;

        // is user to be suspended admin?
        if (this.EditUser.Item4.IsAdmin > 0)
        {
            // tell user he can't suspend admin
            this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "ERROR_ADMINISTRATORS"), MessageTypes.danger);

            return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
        }

        // is user to be suspended forum moderator, while user suspending him is not admin?
        if (!this.PageBoardContext.IsAdmin && this.EditUser.Item4.IsForumModerator > 0)
        {
            // tell user he can't suspend forum moderator when he's not admin
            this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "ERROR_FORUMMODERATORS"), MessageTypes.danger);

            return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
        }

        // verify the user isn't guest...
        if (this.EditUser.Item1.UserFlags.IsGuest)
        {
            this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "ERROR_GUESTACCOUNT"), MessageTypes.danger);

            return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
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
        this.GetRepository<User>().Suspend(
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

        this.Get<ISendNotification>().SendUserSuspensionNotification(
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

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View8" });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData(int userId)
    {
        this.SuspendUnits = new List<SelectListItem> {
                                                         new(this.GetText("PROFILE", "DAYS"), "1"),
                                                         new(this.GetText("PROFILE", "HOURS"), "2"),
                                                         new(this.GetText("PROFILE", "MINUTES"), "3")
                                                     };

        this.EditUser =
            this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] as
                Tuple<User, AspNetUsers, Rank, VAccess>;
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        public int UserId { get; set; }

        public string SuspendedReason { get; set; }

        public int SuspendCount { get; set; } = 2;

        public int SuspendUnit { get; set; } = 2;
    }
}