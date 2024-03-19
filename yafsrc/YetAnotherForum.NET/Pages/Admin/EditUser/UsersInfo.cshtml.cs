/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2024 Ingo Herbote
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
using YAF.Core.Services;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

public class UsersInfoModel : AdminPage
{
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

    public UsersInfoModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.Input = new UsersInfoInputModel();

        this.BindData(userId);

        return this.Page();
    }

    /// <summary>
    /// Updates the User Info
    /// </summary>
    public async Task<IActionResult> OnPostSaveAsync(int userId)
    {
        // Update the Membership
        if (!this.Input.IsGuestX)
        {
            var aspNetUser = await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(this.Input.Name);

            // Update IsApproved if user is not already approved
            aspNetUser.IsApproved = !aspNetUser.IsApproved ? this.Input.IsApproved : aspNetUser.IsApproved;

            //set input variable to current value so that flags are properly updated.
            this.Input.IsApproved = aspNetUser.IsApproved;

            await this.Get<IAspNetUsersHelper>().UpdateUserAsync(aspNetUser);
        }
        else
        {
            if (!this.Input.IsApproved)
            {
                return this.PageBoardContext.Notify(
                    this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_GUEST_APPROVED"),
                    MessageTypes.success);
            }
        }

        var userFlags = new UserFlags
        {
            IsHostAdmin = this.Input.IsHostAdminX,
            IsGuest = this.Input.IsGuestX,
            IsActiveExcluded = this.Input.IsExcludedFromActiveUsers,
            IsApproved = this.Input.IsApproved,
            Moderated = this.Input.Moderated
        };

        this.GetRepository<User>().AdminSave(
            this.PageBoardContext.PageBoardID,
            userId,
            this.Input.Name,
            this.Input.DisplayName,
            userFlags.BitValue,
            this.Input.RankID);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { userId, tab = "View1" });
    }

    /// <summary>
    /// Approve the User
    /// </summary>
    public async Task OnPostApproveUserAsync(int id)
    {
        await this.Get<IAspNetUsersHelper>().ApproveUserAsync(id);

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Users);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData(int userId)
    {
        this.EditUser =
            this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] as
                Tuple<User, AspNetUsers, Rank, VAccess>;

        if (this.EditUser is null)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            return;
        }

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
        this.Input.IsFacebookUser = this.EditUser.Item2.Profile_FacebookId.IsSet();
        this.Input.IsGoogleUser = this.EditUser.Item2.Profile_GoogleId.IsSet();
        this.Input.LastVisit = this.EditUser.Item1.LastVisit.ToString(CultureInfo.InvariantCulture);
        this.Input.RankID = this.EditUser.Item1.RankID;
    }
}