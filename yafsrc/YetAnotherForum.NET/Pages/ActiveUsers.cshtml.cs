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

namespace YAF.Pages;

using System.Collections.Generic;

using Core.Extensions;
using Core.Model;
using Core.Services;

using Types.Extensions;
using Types.Models;
using Types.Objects.Model;
using Types.Interfaces;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Helpers;

/// <summary>
/// The Cookies model.
/// </summary>
public class ActiveUsersModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "ActiveUsersModel" /> class.
    /// </summary>
    public ActiveUsersModel()
        : base("ACTIVEUSERS", ForumPages.ActiveUsers)
    {
    }

    /// <summary>
    /// Gets or sets the user list.
    /// </summary>
    public List<ActiveUser> UserList { get; set; }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="v">
    /// The v.
    /// </param>
    public IActionResult OnGet(int? v = null)
    {
        return this.BindData(v);
    }

    /// <summary>
    /// Called when [post].
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPost(int? v = null)
    {
        return !this.PageBoardContext.UploadAccess ? this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied) : this.BindData(v);
    }

    /// <summary>
    /// Removes from the DataView all users but guests.
    /// </summary>
    /// <param name="activeUsers">
    /// The active users.
    /// </param>
    private static void RemoveAllButGuests(ref List<ActiveUser> activeUsers)
    {
        if (activeUsers.Count == 0)
        {
            return;
        }

        // remove non-guest users...
        activeUsers.RemoveAll(row => !row.IsGuest);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    /// <param name="v">
    /// The version.
    /// </param>
    private IActionResult BindData(int? v = null)
    {
        int version;

        if (v.HasValue && this.Get<IPermissions>()
                .Check(this.PageBoardContext.BoardSettings.ActiveUsersViewPermissions))
        {
            version = v.Value;
        }
        else
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        List<ActiveUser> activeUsers;

        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        switch (version)
        {
            case 0:

                // Show all users
                activeUsers = this.GetActiveUsersData(
                    true,
                    this.PageBoardContext.BoardSettings.ShowGuestsInDetailedActiveList);

                if (activeUsers != null)
                {
                    this.RemoveHiddenUsers(ref activeUsers);
                }

                break;
            case 1:

                // Show members
                activeUsers = this.GetActiveUsersData(false, false);

                if (activeUsers != null)
                {
                    this.RemoveHiddenUsers(ref activeUsers);
                }

                break;
            case 2:

                // Show guests
                activeUsers = this.GetActiveUsersData(
                    true,
                    this.PageBoardContext.BoardSettings.ShowCrawlersInActiveList);

                if (activeUsers != null)
                {
                    RemoveAllButGuests(ref activeUsers);
                }

                break;
            case 3:

                // Show hidden
                if (this.PageBoardContext.IsAdmin)
                {
                    activeUsers = this.GetActiveUsersData(false, false);
                    if (activeUsers != null)
                    {
                        this.RemoveAllButHiddenUsers(ref activeUsers);
                    }
                }
                else
                {
                    return this.Get<LinkBuilder>().AccessDenied();
                }

                break;
            default:
                return this.Get<LinkBuilder>().AccessDenied();
        }

        if (activeUsers.NullOrEmpty())
        {
            return this.Page();
        }

        this.UserList = activeUsers;

        return this.Page();
    }

    /// <summary>
    /// Gets active user(s) data for a page user
    /// </summary>
    /// <param name="showGuests">
    /// The show guests.
    /// </param>
    /// <param name="showCrawlers">
    /// The show crawlers.
    /// </param>
    /// <returns>
    /// Returns the Active user list
    /// </returns>
    private List<ActiveUser> GetActiveUsersData(bool showGuests, bool showCrawlers)
    {
        var activeUsers = this.GetRepository<Active>().ListUsersPaged(
            this.PageBoardContext.PageUserID,
            showGuests,
            showCrawlers,
            this.PageBoardContext.PageIndex == 0 ? 1 : this.PageBoardContext.PageIndex, this.Size);

        return activeUsers;
    }

    /// <summary>
    /// Removes from the DataView all users but hidden.
    /// </summary>
    /// <param name="activeUsers">
    /// The active users.
    /// </param>
    private void RemoveAllButHiddenUsers(ref List<ActiveUser> activeUsers)
    {
        if (activeUsers.Count == 0)
        {
            return;
        }

        // remove hidden users...
        activeUsers.RemoveAll(
            row => !row.IsActiveExcluded && this.PageBoardContext.PageUserID != row.UserID);
    }

    /// <summary>
    /// Removes hidden users.
    /// </summary>
    /// <param name="activeUsers">
    /// The active users.
    /// </param>
    private void RemoveHiddenUsers(ref List<ActiveUser> activeUsers)
    {
        if (activeUsers.Count == 0)
        {
            return;
        }

        // remove hidden users...
        activeUsers.RemoveAll(
            row => row.IsActiveExcluded && !this.PageBoardContext.IsAdmin &&
                   this.PageBoardContext.PageUserID != row.UserID);
    }
}