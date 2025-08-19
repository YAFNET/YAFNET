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

namespace YAF.Core.Helpers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Core.Identity;
using YAF.Core.Model;
using YAF.Types.Models;
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
    /// The users.
    /// </summary>
    public IQueryable<AspNetRoles> Roles => this.Get<AspNetRoleManager>().AspNetRoles;

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
    ///     Current Membership User
    /// </param>
    /// <param name="pageBoardId">
    ///     Current BoardID
    /// </param>
    /// <returns>
    /// Returns the UserID of the user if everything was successful. Otherwise, null.
    /// </returns>
    public Task<int?> CreateForumUserAsync(AspNetUsers user, int pageBoardId)
    {
        return this.Get<IAspNetRolesHelper>().CreateForumUserAsync(user, user.UserName, pageBoardId);
    }

    /// <summary>
    /// Creates the user in the YAF DB from the ASP.NET Membership user information.
    ///   Also copies the Roles as groups into YAF DB for the current user
    /// </summary>
    /// <param name="user">
    ///     Current Membership User
    /// </param>
    /// <param name="displayName">
    ///     The display Name.
    /// </param>
    /// <param name="pageBoardId">
    ///     Current BoardID
    /// </param>
    /// <returns>
    /// Returns the UserID of the user if everything was successful. Otherwise, null.
    /// </returns>
    public async Task<int?> CreateForumUserAsync(AspNetUsers user, string displayName, int pageBoardId)
    {
        int? userId = null;

        try
        {
            userId = await this.GetRepository<User>().AspNetAsync(
                pageBoardId,
                user.UserName,
                displayName,
                user.Email,
                user.Id,
                this.Get<BoardSettings>().PageSizeDefault,
                user.IsApproved);

            var roles = await this.Get<IAspNetRolesHelper>().GetRolesForUserAsync(user);

            foreach (var role in roles)
            {
                this.GetRepository<UserGroup>().SetRole(pageBoardId, userId.Value, role);
            }

            if (this.Get<BoardSettings>().UseStyledNicks)
            {
                await this.Get<IRaiseEventAsync>().RaiseAsync(new UpdateUserStyleEvent(userId.Value));
            }
        }
        catch (Exception x)
        {
            this.Get<ILogger<AspNetRolesHelper>>().Error(x, "Error in CreateForumUser");
        }

        return userId;
    }

    /// <summary>
    /// The create role.
    /// </summary>
    /// <param name="roleName">
    ///     The role name.
    /// </param>
    public Task CreateRoleAsync(string roleName)
    {
        var role = new AspNetRoles { Name = roleName };

        return this.Get<IAspNetRoleManager>().CreateRoleAsync(role);
    }

    /// <summary>
    /// The delete role.
    /// </summary>
    /// <param name="roleName">
    ///     The role name.
    /// </param>
    public async Task DeleteRoleAsync(string roleName)
    {
        var role = await this.Get<IAspNetRoleManager>().FindByRoleNameAsync(roleName);

        await this.Get<IAspNetRoleManager>().DeleteRoleAsync(role);
    }

    /// <summary>
    /// Check if the forum user was created.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="pageBoardId">The page board ID.</param>
    /// <returns>
    /// The did create forum user.
    /// </returns>
    public bool DidCreateForumUser(AspNetUsers user, int pageBoardId)
    {
        var userId = this.Get<IAspNetRolesHelper>().CreateForumUserAsync(user, pageBoardId);
        return userId != null;
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
    ///     The user.
    /// </param>
    /// <returns>
    /// Returns all Roles
    /// </returns>
    public Task<IList<string>> GetRolesForUserAsync(AspNetUsers user)
    {
        return this.Get<IAspNetRoleManager>().GetRolesAsync(user);
    }

    /// <summary>
    /// The get users in role.
    /// </summary>
    /// <param name="roleName">
    ///     The role name.
    /// </param>
    /// <returns>
    /// Returns all users in that Role
    /// </returns>
    public async Task<List<AspNetUsers>> GetUsersInRoleAsync(string roleName)
    {
        var role = await this.Get<IAspNetRoleManager>().FindByRoleNameAsync(roleName);

        var users = await this.GetRepository<AspNetUserRoles>().GetAsync(r => r.RoleId == role.Id);

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
    /// Roles the exists.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <returns>
    /// The role exists.
    /// </returns>
    public Task<bool> RoleNameExistsAsync(string roleName)
    {
        return this.Get<IAspNetRoleManager>().RoleNameExistsAsync(roleName);
    }

    /// <summary>
    /// Sets up the user roles from the "start" settings for a given group/role
    /// </summary>
    /// <param name="pageBoardId">
    ///     Current BoardID
    /// </param>
    /// <param name="user">
    ///     The user.
    /// </param>
    public async Task SetupUserRolesAsync(int pageBoardId, AspNetUsers user)
    {
        var groups = await this.GetRepository<Group>()
            .GetAsync(g => g.BoardID == pageBoardId && (g.Flags & 2) != 2 && (g.Flags & 4) == 4);

        foreach (var name in groups.Select(group => group.Name))
        {
            if (name.IsSet() && !await this.Get<IAspNetUsersHelper>().IsUserInRoleAsync(user, name))
            {
                this.Get<IAspNetRolesHelper>().AddUserToRole(user, name);
            }
        }
    }

    /// <summary>
    /// Syncs the ASP.NET roles with YAF group based on YAF (not bi-directional)
    /// </summary>
    /// <param name="pageBoardId">The page board ID.</param>
    public async Task SyncRolesAsync(int pageBoardId)
    {
        var groups = await this.GetRepository<Group>().GetAsync(g => g.BoardID == pageBoardId && (g.Flags & 2) != 2);

        var groupsNames =
            groups.Select(g => g.Name);

        // get all the groups in YAF DB and create them if they do not exist as a role in membership
        foreach (var roleName in groupsNames.ToList())
        {
            if (!await this.Get<IAspNetRolesHelper>().RoleNameExistsAsync(roleName))
            {
                await this.Get<IAspNetRolesHelper>().CreateRoleAsync(roleName);
            }
        }
    }
}