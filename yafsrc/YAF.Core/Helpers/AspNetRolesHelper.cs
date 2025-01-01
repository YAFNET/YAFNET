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

namespace YAF.Core.Helpers;

using System;
using System.Collections.Generic;

using YAF.Core.Model;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// The role membership helper.
/// </summary>
public class AspNetRolesHelper : IAspNetRolesHelper, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetRolesHelper"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public AspNetRolesHelper(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; protected set; }

    /// <summary>
    /// The add user to role.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="role">
    /// The role.
    /// </param>
    public void AddUserToRole(AspNetUsers user, string role)
    {
        this.Get<IAspNetUsersHelper>().AddToRole(user, role);
    }

    /// <summary>
    /// Creates the user in the YAF DB from the ASP.NET Membership user information.
    ///   Also copies the Roles as groups into YAF DB for the current user
    /// </summary>
    /// <param name="user">
    /// Current Membership User
    /// </param>
    /// <param name="pageBoardID">
    /// Current BoardID
    /// </param>
    /// <returns>
    /// Returns the UserID of the user if everything was successful. Otherwise, null.
    /// </returns>
    public int? CreateForumUser(AspNetUsers user, int pageBoardID)
    {
        return this.Get<IAspNetRolesHelper>().CreateForumUser(user, user.UserName, pageBoardID);
    }

    /// <summary>
    /// Creates the user in the YAF DB from the ASP.NET Membership user information.
    ///   Also copies the Roles as groups into YAF DB for the current user
    /// </summary>
    /// <param name="user">
    /// Current Membership User
    /// </param>
    /// <param name="displayName">
    /// The display Name.
    /// </param>
    /// <param name="pageBoardID">
    /// Current BoardID
    /// </param>
    /// <returns>
    /// Returns the UserID of the user if everything was successful. Otherwise, null.
    /// </returns>
    public int? CreateForumUser(AspNetUsers user, string displayName, int pageBoardID)
    {
        int? userId = null;

        try
        {
            userId = this.GetRepository<User>().AspNet(
                pageBoardID,
                user.UserName,
                displayName,
                user.Email,
                user.Id,
                this.Get<BoardSettings>().PageSizeDefault,
                user.IsApproved);

            this.Get<IAspNetRolesHelper>().GetRolesForUser(user).ForEach(
                role => this.GetRepository<UserGroup>().SetRole(pageBoardID, userId.Value, role));

            if (this.Get<BoardSettings>().UseStyledNicks)
            {
                this.Get<IRaiseEvent>().Raise(new UpdateUserStyleEvent(userId.Value));
            }
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Error(x, "Error in CreateForumUser");
        }

        return userId;
    }

    /// <summary>
    /// The create role.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    public void CreateRole(string roleName)
    {
        var role = new AspNetRoles { Name = roleName };

        this.Get<IAspNetRoleManager>().Create(role);
    }

    /// <summary>
    /// The delete role.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    public void DeleteRole(string roleName)
    {
        var role = this.Get<IAspNetRoleManager>().FindByName(roleName);

        this.Get<IAspNetRoleManager>().Delete(role);
    }

    /// <summary>
    /// Check if the forum user was created.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="pageBoardID">The page board ID.</param>
    /// <returns>
    /// The did create forum user.
    /// </returns>
    public bool DidCreateForumUser(AspNetUsers user, int pageBoardID)
    {
        var userID = this.Get<IAspNetRolesHelper>().CreateForumUser(user, pageBoardID);
        return userID != null;
    }

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <returns>
    /// Returns all Roles
    /// </returns>
    public List<string> GetAllRoles()
    {
        return [.. this.Get<IAspNetRoleManager>().AspNetRoles.Select(r => r.Name)];
    }

    /// <summary>
    /// Gets the roles for the user.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// Returns all Roles
    /// </returns>
    public IList<string> GetRolesForUser(AspNetUsers user)
    {
        return this.Get<IAspNetRoleManager>().GetRoles(user);
    }

    /// <summary>
    /// The get users in role.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <returns>
    /// The <see cref="List"/>.
    /// </returns>
    public List<AspNetUsers> GetUsersInRole(string roleName)
    {
        var role = this.Get<IAspNetRoleManager>().FindByName(roleName);

        var users = this.GetRepository<AspNetUserRoles>().Get(r => r.RoleId == role.Id);

        var userList = new List<AspNetUsers>();

        this.Get<IAspNetUsersHelper>().Users.ForEach(
            user =>
                {
                    if (users.Exists(u => u.UserId == user.Id))
                    {
                        userList.Add(user);
                    }
                });

        return userList;
    }

    /// <summary>
    /// Is Member of Group.
    /// </summary>
    /// <param name="groupName">
    /// The group name.
    /// </param>
    /// <param name="groups">
    /// The member Groups.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool IsMemberOfGroup(string groupName, List<GroupMember> groups)
    {
        return groups.Exists(
            row => row.IsMember && row.Name == groupName);
    }

    /// <summary>
    /// Determines whether [is user in role] [the specified username].
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="role">
    /// The role.
    /// </param>
    /// <returns>
    /// The is user in role.
    /// </returns>
    public bool IsUserInRole(AspNetUsers user, string role)
    {
        return this.Get<IAspNetUsersHelper>().IsInRole(user, role);
    }

    /// <summary>
    /// The remove user from role.
    /// </summary>
    /// <param name="userProviderKey">
    /// The user Provider Key.
    /// </param>
    /// <param name="role">
    /// The role.
    /// </param>
    public void RemoveUserFromRole(string userProviderKey, string role)
    {
        this.Get<IAspNetUsersHelper>().RemoveFromRole(userProviderKey, role);
    }

    /// <summary>
    /// Roles the exists.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <returns>
    /// The role exists.
    /// </returns>
    public bool RoleExists(string roleName)
    {
        return this.Get<IAspNetRoleManager>().RoleExists(roleName);
    }

    /// <summary>
    /// Sets up the user roles from the "start" settings for a given group/role
    /// </summary>
    /// <param name="pageBoardID">
    /// Current BoardID
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    public void SetupUserRoles(int pageBoardID, AspNetUsers user)
    {
        var groups = this.GetRepository<Group>()
            .Get(g => g.BoardID == pageBoardID && (g.Flags & 2) != 2 && (g.Flags & 4) == 4);

        (from @group in groups
         select @group.Name
         into roleName
         where roleName.IsSet()
         where !this.Get<IAspNetRolesHelper>().IsUserInRole(user, roleName)
         select roleName).ForEach(roleName => this.Get<IAspNetRolesHelper>().AddUserToRole(user, roleName));
    }

    /// <summary>
    /// Syncs the ASP.NET roles with YAF group based on YAF (not bi-directional)
    /// </summary>
    /// <param name="pageBoardID">The page board ID.</param>
    public void SyncRoles(int pageBoardID)
    {
        var groupsNames = this.GetRepository<Group>().Get(g => g.BoardID == pageBoardID && (g.Flags & 2) != 2)
            .Select(g => g.Name);

        // get all the groups in YAF DB and create them if they do not exist as a role in membership
        (from @group in groupsNames
         let name = @group
         where !this.Get<IAspNetRolesHelper>().RoleExists(name)
         select name).ForEach(this.Get<IAspNetRolesHelper>().CreateRole);
    }
}