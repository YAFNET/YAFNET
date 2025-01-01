﻿/* Yet Another Forum.NET
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

namespace YAF.Controls;

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models.Identity;
using YAF.Types.Models;

/// <summary>
/// The edit users groups.
/// </summary>
public partial class EditUsersGroups : BaseUserControl
{
    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    
    public Tuple<User, AspNetUsers, Rank, VAccess> User { get; set; }
	
    /// <summary>
    /// Handles page load event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // this needs to be done just once, not during post-backs
        if (this.IsPostBack)
        {
            return;
        }

        this.SendEmail.Text = this.GetText("ADMIN_EDITUSER", "SEND_EMAIL");

        // bind data
        this.BindData();
    }

    /// <summary>
    /// Handles click on save button.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Save_Click(object sender, EventArgs e)
    {
        var addedRoles = new List<string>();
        var removedRoles = new List<string>();

        // go through all roles displayed on page
        for (var i = 0; i < this.UserGroups.Items.Count; i++)
        {
            // get current item
            var item = this.UserGroups.Items[i];

            // get role ID from it
            var roleID = int.Parse(item.FindControlAs<Label>("GroupID").Text);

            // get role name
            var roleName = this.GetRepository<Group>().GetById(roleID).Name;

            // is user supposed to be in that role?
            var isChecked = item.FindControlAs<CheckBox>("GroupMember").Checked;

            // save user in role
            this.GetRepository<UserGroup>().AddOrRemove(this.User.Item1.ID, roleID, isChecked);

            // update roles if this user isn't the guest
            if (this.Get<IAspNetUsersHelper>().IsGuestUser(this.User.Item1.ID))
            {
                continue;
            }

            // add/remove user from roles in membership provider
            if (isChecked && !this.Get<IAspNetRolesHelper>().IsUserInRole(this.User.Item2, roleName))
            {
                this.Get<IAspNetRolesHelper>().AddUserToRole(this.User.Item2, roleName);

                addedRoles.Add(roleName);
            }
            else if (!isChecked && this.Get<IAspNetRolesHelper>().IsUserInRole(this.User.Item2, roleName))
            {
                this.Get<IAspNetRolesHelper>().RemoveUserFromRole(this.User.Item2.Id, roleName);

                removedRoles.Add(roleName);
            }
        }

        this.Get<IRaiseEvent>().Raise(new UpdateUserStyleEvent(this.User.Item1.ID));

        if (this.SendEmail.Checked)
        {
            // send notification to user
            if (addedRoles.Any())
            {
                this.Get<ISendNotification>().SendRoleAssignmentNotification(this.User.Item2, addedRoles);
            }

            if (removedRoles.Any())
            {
                this.Get<ISendNotification>().SendRoleUnAssignmentNotification(this.User.Item2, removedRoles);
            }
        }

        // clear the cache for this user...
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.User.Item1.ID));

        this.BindData();
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
        // get user roles
        this.UserGroups.DataSource = this.GetRepository<Group>().Member(
            this.PageBoardContext.PageBoardID,
            this.User.Item1.ID);

        // bind data to controls
        this.DataBind();
    }
}