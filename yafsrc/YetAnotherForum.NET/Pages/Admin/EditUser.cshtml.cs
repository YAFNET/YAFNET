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

using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using System.Globalization;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The Admin edit user page.
/// </summary>
public class EditUserModel : AdminPage
{
    /// <summary>
    /// The current culture information
    /// </summary>
    private CultureInfo currentCultureInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditUserModel"/> class.
    /// </summary>
    public EditUserModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    /// <summary>
    /// Gets or sets the last tab.
    /// </summary>
    /// <value>The last tab.</value>
    [BindProperty]
    public string LastTab { get; set; } = "View1";

    /// <summary>
    /// Gets or sets the edit user.
    /// </summary>
    /// <value>The edit user.</value>
    [BindProperty]
    public Tuple<User, AspNetUsers, Rank, VAccess> EditUser { get; set; }

    /// <summary>
    /// Gets or sets the edit user rank.
    /// </summary>
    /// <value>The edit user rank.</value>
    [BindProperty]
    public Rank EditUserRank { get; set; }

    /// <summary>
    /// Gets the current Culture information.
    /// </summary>
    /// <value>
    /// The current Culture information.
    /// </value>
    public CultureInfo CurrentCultureInfo {
        get {
            if (this.currentCultureInfo != null)
            {
                return this.currentCultureInfo;
            }

            this.currentCultureInfo = CultureInfoHelper.GetCultureByUser(
                this.PageBoardContext.BoardSettings,
                this.EditUser.Item1);

            return this.currentCultureInfo;
        }
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_USERS", "TITLE"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Users));
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="u">The user id.</param>
    /// <param name="tab">The tab.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnGetAsync(int u, string tab = null)
    {
        if (tab.IsSet())
        {
            this.LastTab = tab;
        }

        var editUser = await this.Get<IAspNetUsersHelper>().GetBoardUserAsync(u, includeNonApproved: true);

        this.Get<IDataCache>().Set(string.Format(Constants.Cache.EditUser, u), editUser);

        if (editUser is null)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // do admin permission check...
        if (!this.PageBoardContext.PageUser.UserFlags.IsHostAdmin && editUser.Item1.UserFlags.IsHostAdmin)
        {
            // user is not host admin and is attempted to edit host admin account...
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.EditUser = editUser;

        // current page label (no link)
        var userName = this.HtmlEncode(editUser.Item1.DisplayOrUserName());

        var header = string.Format(this.GetText("ADMIN_EDITUSER", "TITLE"), userName);

        this.PageBoardContext.PageLinks.AddLink(header, string.Empty);

        return this.Page();
    }
}