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

using System.Globalization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Class UsersInfoModel.
/// Implements the <see cref="YAF.Core.BasePages.AdminPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.AdminPage" />
public class UsersInfoModel : AdminPage
{
    /// <summary>
    /// Gets or sets the ranks.
    /// </summary>
    /// <value>The ranks.</value>
    public SelectList Ranks { get; set; }

    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    public Tuple<User, AspNetUsers, Rank, VAccess> EditUser { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersInfoInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersInfoModel"/> class.
    /// </summary>
    public UsersInfoModel()
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

        this.Input = new UsersInfoInputModel();

        return this.BindData(userId);
    }

    /// <summary>
    /// Updates the User Info
    /// </summary>
    public IActionResult OnPostSave(int userId)
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] is not
            Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = userId
                });
        }

        this.EditUser = user;

        var userFlags = this.EditUser.Item1.UserFlags;

        userFlags.IsHostAdmin = this.Input.IsHostAdminX;
        userFlags.IsGuest = this.Input.IsGuestX;
        userFlags.IsActiveExcluded = this.Input.IsExcludedFromActiveUsers;
        userFlags.Moderated = this.Input.Moderated;

        this.GetRepository<User>().AdminSave(
            this.PageBoardContext.PageBoardID,
            userId,
            userFlags.BitValue,
            this.Input.RankID);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = userId, tab = "View1" });
    }

    /// <summary>
    /// Approve the User
    /// </summary>
    public async Task OnPostApproveUserAsync(int id)
    {
        await this.Get<IAspNetUsersHelper>().ApproveUserAsync(id);

        this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Users);
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

        this.EditUser = user;

        this.Ranks = new SelectList(this.GetRepository<Rank>().GetByBoardId(), nameof(Rank.ID), nameof(Rank.Name));

        this.Input.Name = this.EditUser.Item1.Name;
        this.Input.DisplayName = this.EditUser.Item1.DisplayName;
        this.Input.Email = this.EditUser.Item1.Email;
        this.Input.IsHostAdminX = this.EditUser.Item1.UserFlags.IsHostAdmin;
        this.Input.IsApproved = this.EditUser.Item1.UserFlags.IsApproved;
        this.Input.IsGuestX = this.EditUser.Item1.UserFlags.IsGuest;
        this.Input.IsExcludedFromActiveUsers = this.EditUser.Item1.UserFlags.IsActiveExcluded;
        this.Input.Moderated = this.EditUser.Item1.UserFlags.Moderated;
        this.Input.Joined = this.EditUser.Item1.Joined.ToString(CultureInfo.InvariantCulture);
        this.Input.LastVisit = this.EditUser.Item1.LastVisit.ToString(CultureInfo.InvariantCulture);
        this.Input.RankID = this.EditUser.Item1.RankID;

        return this.Page();
    }
}