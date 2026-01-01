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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// Interface for creating or editing user roles/groups.
/// </summary>
public class EditGroupModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditGroupInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the access masks list.
    /// </summary>
    [BindProperty]
    public List<(int ForumID, string ForumName, int? ParentID, int AccessMaskID)> AccessMasksList { get; set; }

    public Group Group { get; set; }

    public SelectList NewAccessMasks { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditGroupModel"/> class.
    /// </summary>
    public EditGroupModel()
        : base("ADMIN_EDITGROUP", ForumPages.Admin_EditGroup)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // admin index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_GROUPS", "TITLE"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Groups));

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITGROUP", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? i)
    {
        this.Input = new EditGroupInputModel();

        // bind data
        this.BindData(i);

        // is this editing of existing role or creation of new one?
        if (i is null or 0)
        {
            return this.Page();
        }

        this.Input.Id = i.Value;

        // get data about edited role
        this.Group = this.GetRepository<Group>().GetById(i.Value);

        if (this.Group is null)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // get role flags
        var flags = this.Group.GroupFlags;

        // set controls to role values
        this.Input.Name = this.Group.Name;

        this.Input.IsAdminX = flags.IsAdmin;

        this.Input.IsStartX = flags.IsStart;

        this.Input.IsModeratorX = flags.IsModerator;

        this.Input.UploadAccess = flags.AllowUpload;

        this.Input.DownloadAccess = flags.AllowDownload;

        this.Input.Style = this.Group.Style;

        this.Input.Priority = this.Group.SortOrder;

        this.Input.UsrAlbums = this.Group.UsrAlbums;

        this.Input.UsrAlbumImages = this.Group.UsrAlbumImages;

        this.Input.UsrSigChars = this.Group.UsrSigChars;

        this.Input.UsrSigBBCodes = this.Group.UsrSigBBCodes;

        this.Input.Description = this.Group.Description;

        this.Input.IsGuestX = flags.IsGuest;

        return this.Page();
    }

    /// <summary>
    /// Saves the click.
    /// </summary>
    public async Task<IActionResult> OnPostSaveAsync()
    {
        // Role
        int? roleId = null;

        // get role ID from page's parameter
        if (this.Input.Id > 0)
        {
            roleId = this.Input.Id;
        }

        // get new and old name
        var roleName = this.Input.Name;
        var oldRoleName = string.Empty;

        // if we are editing existing role, get it's original name
        if (roleId.HasValue)
        {
            // get the current role name in the DB
            var group = await this.GetRepository<Group>().GetByIdAsync(roleId.Value);

            oldRoleName = group.Name;
        }

        var groupFlags = new GroupFlags {
                                                IsGuest = this.Input.IsGuestX,
                                                IsAdmin = this.Input.IsAdminX,
                                                IsModerator = this.Input.IsModeratorX,
                                                IsStart = this.Input.IsStartX,
                                                AllowUpload = this.Input.UploadAccess,
                                                AllowDownload = this.Input.DownloadAccess
                                            };

        // save role and get its ID if it's new (if it's old role, we get it anyway)
        await this.GetRepository<Group>().SaveAsync(
            roleId,
            this.PageBoardContext.PageBoardID,
            roleName,
            groupFlags,
            this.Input.NewAccessMaskID,
            this.Input.Style,
            this.Input.Priority,
            this.Input.Description,
            this.Input.UsrSigChars,
            this.Input.UsrSigBBCodes,
            this.Input.UsrAlbums,
            this.Input.UsrAlbumImages);

        // see if need to rename an existing role...
        if (oldRoleName.IsSet() && roleName != oldRoleName &&
            await this.Get<IAspNetRolesHelper>().RoleNameExistsAsync(oldRoleName) &&
            !await this.Get<IAspNetRolesHelper>().RoleNameExistsAsync(roleName) && !this.Input.IsGuestX)
        {
            // transfer users in addition to changing the name of the role...
            var users = await this.Get<IAspNetRolesHelper>().GetUsersInRoleAsync(oldRoleName);

            // delete the old role...
            await this.Get<IAspNetRolesHelper>().DeleteRoleAsync(oldRoleName);

            // create new role...
            await this.Get<IAspNetRolesHelper>().CreateRoleAsync(roleName);

            if (users.Count != 0)
            {
                // put users into new role...
                users.ForEach(user => this.Get<IAspNetRolesHelper>().AddUserToRole(user, roleName));
            }
        }
        else if (!await this.Get<IAspNetRolesHelper>().RoleNameExistsAsync(roleName) && !this.Input.IsGuestX)
        {
            // if role doesn't exist in provider's data source, create it

            // simply create it
            await this.Get<IAspNetRolesHelper>().CreateRoleAsync(roleName);
        }

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Groups);
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    /// <param name="i"></param>
    private void BindData(int? i)
    {
        // set data source of access list (list of forums and role's access masks) if we are editing existing mask
        if (i.HasValue)
        {
            this.AccessMasksList = this.GetRepository<ForumAccess>()
                .ListByGroups(i.Value);
        }

        this.NewAccessMasks = new SelectList(
            this.GetRepository<AccessMask>().GetByBoardId(),
            nameof(AccessMask.ID),
            nameof(AccessMask.Name));
    }
}