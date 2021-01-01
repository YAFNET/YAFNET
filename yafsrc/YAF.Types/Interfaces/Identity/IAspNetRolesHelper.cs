/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Types.Interfaces.Identity
{
    using System.Collections.Generic;

    using YAF.Types;
    using YAF.Types.Models.Identity;

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
        void AddUserToRole([NotNull] AspNetUsers user, [NotNull] string role);

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
        int? CreateForumUser([NotNull] AspNetUsers user, int pageBoardID);

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
        int? CreateForumUser([NotNull] AspNetUsers user, [NotNull] string displayName, int pageBoardID);

        /// <summary>
        /// The create role.
        /// </summary>
        /// <param name="roleName">
        /// The role name.
        /// </param>
        void CreateRole([NotNull] string roleName);

        /// <summary>
        /// The delete role.
        /// </summary>
        /// <param name="roleName">
        /// The role name.
        /// </param>
        void DeleteRole([NotNull] string roleName);

        /// <summary>
        /// Check if the forum user was created.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pageBoardID">The page board ID.</param>
        /// <returns>
        /// The did create forum user.
        /// </returns>
        bool DidCreateForumUser([NotNull] AspNetUsers user, int pageBoardID);

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
        IList<string> GetRolesForUser([NotNull] AspNetUsers user);

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
        /// <param name="memberGroups">
        /// The member Groups.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsMemberOfGroup([NotNull] string groupName, [NotNull] List<dynamic> groups);

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
        bool IsUserInRole([NotNull] AspNetUsers user, [NotNull] string role);

        /// <summary>
        /// The remove user from role.
        /// </summary>
        /// <param name="userProviderKey">
        /// The user Provider Key.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        void RemoveUserFromRole([NotNull] string userProviderKey, [NotNull] string role);

        /// <summary>
        /// Roles the exists.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>
        /// The role exists.
        /// </returns>
        bool RoleExists([NotNull] string roleName);

        /// <summary>
        /// Sets up the user roles from the "start" settings for a given group/role
        /// </summary>
        /// <param name="pageBoardID">
        /// Current BoardID
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        void SetupUserRoles(int pageBoardID, [NotNull] AspNetUsers user);

        /// <summary>
        /// Goes through every membership user and manually "syncs" them to the forum.
        ///   Best for an existing membership structure -- will migrate all users at once 
        ///   rather then one at a time...
        /// </summary>
        /// <param name="pageBoardId">
        /// The page Board Id.
        /// </param>
        void SyncAllMembershipUsers(int pageBoardId);

        /// <summary>
        /// Syncs the ASP.NET roles with YAF group based on YAF (not bi-directional)
        /// </summary>
        /// <param name="pageBoardID">The page board ID.</param>
        void SyncRoles(int pageBoardID);

        /// <summary>
        /// Updates the information in the YAF DB from the ASP.NET Membership user information.
        /// Called once per session for a user to sync up the data
        /// </summary>
        /// <param name="user">Current Membership User</param>
        /// <param name="pageBoardID">Current BoardID</param>
        /// <param name="roles">The DNN user roles.</param>
        /// <returns>
        /// The update forum user.
        /// </returns>
        int? UpdateForumUser([NotNull] AspNetUsers user, int pageBoardID, string[] roles = null);
    }
}