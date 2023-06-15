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
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

public class UsersGroupsModel : AdminPage
{
    [BindProperty]
    public List<GroupMember> UserGroups { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersGroupsModel"/> class.
    /// </summary>
    public UsersGroupsModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.Input = new InputModel();

        this.BindData(userId);

        return this.Page();
    }

    public IActionResult OnPostSave()
    {
        var addedRoles = new List<string>();
        var removedRoles = new List<string>();

        var user = this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] as Tuple<User, AspNetUsers, Rank, VAccess>;

        // go through all roles displayed on page
        foreach (var item in this.UserGroups)
        {
            // get role ID from it
            var roleId = item.GroupID;

            // get role name
            var roleName = this.GetRepository<Group>().GetById(roleId).Name;

            // is user supposed to be in that role?
            var isChecked = item.Selected;

            // save user in role
            this.GetRepository<UserGroup>().AddOrRemove(this.Input.UserId, roleId, isChecked);

            // update roles if this user isn't the guest
            if (this.Get<IAspNetUsersHelper>().IsGuestUser(this.Input.UserId))
            {
                continue;
            }

            // add/remove user from roles in membership provider
            if (isChecked && !this.Get<IAspNetRolesHelper>().IsUserInRole(user.Item2, roleName))
            {
                this.Get<IAspNetRolesHelper>().AddUserToRole(user.Item2, roleName);

                addedRoles.Add(roleName);
            }
            else if (!isChecked && this.Get<IAspNetRolesHelper>().IsUserInRole(user.Item2, roleName))
            {
                this.Get<IAspNetRolesHelper>().RemoveUserFromRole(user.Item2, roleName);

                removedRoles.Add(roleName);
            }
        }

        this.Get<IRaiseEvent>().Raise(new UpdateUserStyleEvent(this.Input.UserId));

        if (this.Input.SendEmail)
        {
            // send notification to user
            if (addedRoles.Any())
            {
                this.Get<ISendNotification>().SendRoleAssignmentNotification(user.Item2, addedRoles);
            }

            if (removedRoles.Any())
            {
                this.Get<ISendNotification>().SendRoleUnAssignmentNotification(user.Item2, removedRoles);
            }
        }

        // clear the cache for this user...
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View2" });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData(int userId)
    {
        this.Input = new InputModel {
                                        UserId = userId
                                    };

        // get user roles
        var roles = this.GetRepository<Group>().Member(this.PageBoardContext.PageBoardID, userId);
        
        foreach (var role in roles.Where(role => role.IsMember))
        {
            role.Selected = true;
        }

        this.UserGroups = roles;
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        public int UserId { get; set; }

        public bool SendEmail { get; set; }
    }
}