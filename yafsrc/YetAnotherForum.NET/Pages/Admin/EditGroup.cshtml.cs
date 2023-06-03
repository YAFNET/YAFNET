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

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
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
    public InputModel Input { get; set; }

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
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Groups));

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITGROUP", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet(int? i)
    {
        this.Input = new InputModel();
        
        // bind data
        this.BindData(i);

        // is this editing of existing role or creation of new one?
        if (!i.HasValue)
        {
            return;
        }

        this.Input.Id = i.Value;

        // get data about edited role
        this.Group = this.GetRepository<Group>().GetById(i.Value);

        if (this.Group == null)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            return;
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

        this.Input.PMLimit = this.Group.PMLimit;

        this.Input.Style = this.Group.Style;

        this.Input.Priority = this.Group.SortOrder;

        this.Input.UsrAlbums = this.Group.UsrAlbums;

        this.Input.UsrAlbumImages = this.Group.UsrAlbumImages;

        this.Input.UsrSigChars = this.Group.UsrSigChars;

        this.Input.UsrSigBBCodes = this.Group.UsrSigBBCodes;

        this.Input.Description = this.Group.Description;

        this.Input.IsGuestX = flags.IsGuest;

        this.Get<ISessionService>().SetPageData(this.Group);
    }

    /// <summary>
    /// Saves the click.
    /// </summary>
    public IActionResult OnPostSave()
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
            var group = this.GetRepository<Group>().GetById(roleId.Value);

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
        this.GetRepository<Group>().Save(
            roleId,
            this.PageBoardContext.PageBoardID,
            roleName,
            groupFlags,
            this.Input.NewAccessMaskID,
            this.Input.PMLimit,
            this.Input.Style,
            this.Input.Priority,
            this.Input.Description,
            this.Input.UsrSigChars,
            this.Input.UsrSigBBCodes,
            this.Input.UsrAlbums,
            this.Input.UsrAlbumImages);

        // see if need to rename an existing role...
        if (oldRoleName.IsSet() && roleName != oldRoleName &&
            this.Get<IAspNetRolesHelper>().RoleExists(oldRoleName) &&
            !this.Get<IAspNetRolesHelper>().RoleExists(roleName) && !this.Input.IsGuestX)
        {
            // transfer users in addition to changing the name of the role...
            var users = this.Get<IAspNetRolesHelper>().GetUsersInRole(oldRoleName);

            // delete the old role...
            this.Get<IAspNetRolesHelper>().DeleteRole(oldRoleName);

            // create new role...
            this.Get<IAspNetRolesHelper>().CreateRole(roleName);

            if (users.Any())
            {
                // put users into new role...
                users.ForEach(user => this.Get<IAspNetRolesHelper>().AddUserToRole(user, roleName));
            }
        }
        else if (!this.Get<IAspNetRolesHelper>().RoleExists(roleName) && !this.Input.IsGuestX)
        {
            // if role doesn't exist in provider's data source, create it

            // simply create it
            this.Get<IAspNetRolesHelper>().CreateRole(roleName);
        }

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Groups);
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

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsGuestX { get; set; }

        public bool IsStartX { get; set; }

        public bool IsModeratorX { get; set; }

        public bool IsAdminX { get; set; }

        public bool UploadAccess { get; set; }

        public bool DownloadAccess { get; set; }

        public short Priority { get; set; }

        public int PMLimit { get; set; }

        public int UsrSigChars { get; set; } = 128;

        public string UsrSigBBCodes { get; set; }

        public int UsrAlbums { get; set; }

        public int UsrAlbumImages { get; set; }

        public string Style { get; set; }

        public int NewAccessMaskID { get; set; }
    }
}