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

using System.Collections.Specialized;

using YAF.Types.Models;

/// <summary>
/// Primary administrator interface for groups/roles editing.
/// </summary>
public partial class Groups : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Groups"/> class. 
    /// </summary>
    public Groups()
        : base("ADMIN_GROUPS", ForumPages.Admin_Groups)
    {
    }

    /// <summary>
    ///   Temporary storage of un-linked provider roles.
    /// </summary>
    private readonly StringCollection availableRoles = new();

    /// <summary>
    /// Format string color.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Set values  are rendered green if true, and if not red
    /// </returns>
    protected string GetItemColorString(string item)
    {
        // show enabled flag red
        return item.IsSet() ? "badge bg-success" : "badge bg-danger";
    }

    /// <summary>
    /// Format access mask setting color formatting.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Set access mask flags  are rendered green if true, and if not red
    /// </returns>
    protected string GetItemColor(bool enabled)
    {
        // show enabled flag red
        return enabled ? "badge bg-success" : "badge bg-danger";
    }

    /// <summary>
    /// Get a user friendly item name.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Item Name.
    /// </returns>
    protected string GetItemName(bool enabled)
    {
        return enabled ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // forum index
        this.PageBoardContext.PageLinks.AddRoot();

        // admin index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        // roles
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_GROUPS", "TITLE"), string.Empty);
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
    [NotNull]
    protected string GetLinkedStatus([NotNull] Group currentRow)
    {
        // check whether role is Guests role, which can't be linked
        return currentRow.GroupFlags.IsGuest
                   ? this.GetText("ADMIN_GROUPS", "UNLINKABLE")
                   : this.GetText("ADMIN_GROUPS", "LINKED");
    }

    /// <summary>
    /// Handles click on new role button
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void NewGroupClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        // redirect to new role page
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditGroup);
    }

    /// <summary>
    /// Handles page load event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        // this needs to be done just once, not during post-backs
        if (this.IsPostBack)
        {
            return;
        }

        // sync roles just in case...
        this.Get<IAspNetRolesHelper>().SyncRoles(this.PageBoardContext.PageBoardID);

        // bind data
        this.BindData();
    }

    /// <summary>
    /// Handles provider roles adding/deleting.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RoleListNetItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
        // detect which command are we handling
        switch (e.CommandName)
        {
            case "add":

                // save role and get its ID
                const int InitialPMessages = 0;

                var groupFlags = new GroupFlags();

                var groupId = this.GetRepository<Group>().Save(
                    null,
                    this.PageBoardContext.PageBoardID,
                    e.CommandArgument.ToString(),
                    groupFlags,
                    1,
                    InitialPMessages,
                    null,
                    100,
                    null,
                    0,
                    null,
                    0,
                    0);

                // redirect to newly created role
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditGroup, new { i = groupId });
                break;
            case "delete":

                // delete role from provider data
                this.Get<IAspNetRolesHelper>().DeleteRole(e.CommandArgument.ToString());

                // re-bind data
                this.BindData();
                break;
        }
    }

    /// <summary>
    /// Handles role editing/deletion buttons.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RoleListYafItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
        // detect which command are we handling
        switch (e.CommandName)
        {
            case "edit":

                // go to role editing page
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditGroup, new { i = e.CommandArgument });
                break;
            case "delete":

                var groupId = e.CommandArgument.ToType<int>();

                // delete role
                this.GetRepository<GroupMedal>().Delete(g => g.GroupID == groupId);
                this.GetRepository<ForumAccess>().Delete(g => g.GroupID == groupId);
                this.GetRepository<UserGroup>().Delete(g => g.GroupID == groupId);
                this.GetRepository<Group>().Delete(g => g.ID == groupId);

                // re-bind data
                this.BindData();
                break;
        }
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
        // list roles of this board
        var groups = this.GetRepository<Group>().List(boardId: this.PageBoardContext.PageBoardID);

        // set repeater data-source
        this.RoleListYaf.DataSource = groups;

        // clear cached list of roles
        this.availableRoles.Clear();

        // get all provider roles
        (from role in this.Get<IAspNetRolesHelper>().GetAllRoles()
         let rows = groups.Select(g => g.Name == role)
         where groups.Count == 0
         select role).ForEach(role1 => this.availableRoles.Add(role1));

        // check if there are any roles for syncing
        if (this.availableRoles.Count > 0 && !Config.IsDotNetNuke)
        {
            // make it data-source
            this.RoleListNet.DataSource = this.availableRoles;
        }
        else
        {
            // no data-source for provider roles
            this.RoleListNet.DataSource = null;
        }

        // bind data to controls
        this.DataBind();
    }
}