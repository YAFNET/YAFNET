/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

namespace YAF.Types.Interfaces.Identity;

using System.Collections.Generic;

/// <summary>
/// The AspNetRolesHelper interface.
/// </summary>
public interface IAspNetRolesHelper
{
    /// <summary>
    /// The add user to role.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="role">
    /// The role.
    /// </param>
    void AddUserToRole(AspNetUsers user, string role);

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
    int? CreateForumUser(AspNetUsers user, int pageBoardID);

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
    int? CreateForumUser(AspNetUsers user, string displayName, int pageBoardID);

    /// <summary>
    /// The create role.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    void CreateRole(string roleName);

    /// <summary>
    /// The delete role.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    void DeleteRole(string roleName);

    /// <summary>
    /// Check if the forum user was created.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="pageBoardID">The page board ID.</param>
    /// <returns>
    /// The did create forum user.
    /// </returns>
    bool DidCreateForumUser(AspNetUsers user, int pageBoardID);

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <returns>
    /// Returns all Roles
    /// </returns>
    List<string> GetAllRoles();

    /// <summary>
    /// Gets the roles for the user.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// Returns all Roles
    /// </returns>
    IList<string> GetRolesForUser(AspNetUsers user);

    /// <summary>
    /// The get users in role.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <returns>
    /// The <see cref="List{T}"/>.
    /// </returns>
    List<AspNetUsers> GetUsersInRole(string roleName);

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
    bool IsMemberOfGroup(string groupName, List<GroupMember> groups);

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
    bool IsUserInRole(AspNetUsers user, string role);

    /// <summary>
    /// The remove user from role.
    /// </summary>
    /// <param name="userProviderKey">
    /// The user Provider Key.
    /// </param>
    /// <param name="role">
    /// The role.
    /// </param>
    void RemoveUserFromRole(string userProviderKey, string role);

    /// <summary>
    /// Roles the exists.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <returns>
    /// The role exists.
    /// </returns>
    bool RoleExists(string roleName);

    /// <summary>
    /// Sets up the user roles from the "start" settings for a given group/role
    /// </summary>
    /// <param name="pageBoardID">
    /// Current BoardID
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    void SetupUserRoles(int pageBoardID, AspNetUsers user);

    /// <summary>
    /// Syncs the ASP.NET roles with YAF group based on YAF (not bi-directional)
    /// </summary>
    /// <param name="pageBoardID">The page board ID.</param>
    void SyncRoles(int pageBoardID);
}