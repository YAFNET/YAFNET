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
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// Primary administrator interface for groups/roles editing.
/// </summary>
public class GroupsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the role list yaf.
    /// </summary>
    /// <value>The role list yaf.</value>
    [BindProperty]
    public IList<Group> RoleListYaf { get; set; }

    /// <summary>
    /// Gets or sets the role list net.
    /// </summary>
    /// <value>The role list net.</value>
    [BindProperty]
    public StringCollection RoleListNet { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupsModel"/> class.
    /// </summary>
    public GroupsModel()
        : base("ADMIN_GROUPS", ForumPages.Admin_Groups)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        // roles
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_GROUPS", "TITLE"), string.Empty);
    }

    /// <summary>
    ///   Temporary storage of un-linked provider roles.
    /// </summary>
    private readonly StringCollection availableRoles = [];

    /// <summary>
    /// Format string color.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Set values  are rendered green if true, and if not red
    /// </returns>
    public string GetItemColorString(string item)
    {
        // show enabled flag red
        return item.IsSet() ? "badge text-bg-success" : "badge text-bg-danger";
    }

    /// <summary>
    /// Get status of provider role VS YAF roles.
    /// </summary>
    /// <param name="currentRow">
    /// Data row which contains data about role.
    /// </param>
    /// <returns>
    /// String "Linked" when role is linked to YAF roles, "Un-linkable" otherwise.
    /// </returns>
    public string GetLinkedStatus(Group currentRow)
    {
        // check whether role is Guests role, which can't be linked
        return currentRow.GroupFlags.IsGuest
                   ? this.GetText("ADMIN_GROUPS", "UNLINKABLE")
                   : this.GetText("ADMIN_GROUPS", "LINKED");
    }

    /// <summary>
    /// Handles page load event.
    /// </summary>
    public async Task OnGetAsync()
    {
        // sync roles just in case...
        await this.Get<IAspNetRolesHelper>().SyncRolesAsync(this.PageBoardContext.PageBoardID);

        // bind data
        this.BindData();
    }

    /// <summary>
    /// Called when [post add].
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>Microsoft.AspNetCore.Mvc.IActionResult.</returns>
    public async Task<IActionResult> OnPostAddAsync(string role)
    {
        // save role and get its ID
        var groupFlags = new GroupFlags();

        var groupId = await this.GetRepository<Group>().SaveAsync(
            null,
            this.PageBoardContext.PageBoardID,
            role,
            groupFlags,
            1,
            null,
            100,
            null,
            0,
            null,
            0,
            0);

        // redirect to newly created role
        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditGroup, new { i = groupId });
    }

    /// <summary>
    /// Called when [post delete net].
    /// </summary>
    /// <param name="role">The role.</param>
    public void OnPostDeleteNet(string role)
    {
        // delete role from provider data
        this.Get<IAspNetRolesHelper>().DeleteRoleAsync(role);

        // re-bind data
        this.BindData();
    }

    public void OnPostDelete(int id)
    {
        var groupId = id;

        // delete role
        this.GetRepository<GroupMedal>().Delete(g => g.GroupID == groupId);
        this.GetRepository<ForumAccess>().Delete(g => g.GroupID == groupId);
        this.GetRepository<UserGroup>().Delete(g => g.GroupID == groupId);
        this.GetRepository<Group>().Delete(g => g.ID == groupId);

        // re-bind data
        this.BindData();
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
        // list roles of this board
        var groups = this.GetRepository<Group>().List(boardId: this.PageBoardContext.PageBoardID);

        // set repeater data-source
        this.RoleListYaf = groups;

        // clear cached list of roles
        this.availableRoles.Clear();

        // get all provider roles
        var roles = this.Get<IAspNetRolesHelper>().GetAllRoles();

        foreach (var role in roles.Where(role =>
                     groups.All(g => !string.Equals(g.Name, role, StringComparison.CurrentCultureIgnoreCase))))
        {
            this.availableRoles.Add(role);
        }

        // check if there are any roles for syncing
        // make it data-source
        this.RoleListNet = this.availableRoles.Count > 0 ? this.availableRoles :
                               // no data-source for provider roles
                               null;
    }
}