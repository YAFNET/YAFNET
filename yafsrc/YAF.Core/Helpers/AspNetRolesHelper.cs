/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Helpers
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;

    #endregion

    /// <summary>
    /// The role membership helper.
    /// </summary>
    public static class AspNetRolesHelper
    {
        #region Public Methods

        /// <summary>
        /// The add user to role.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        public static void AddUserToRole([NotNull] AspNetUsers user, [NotNull] string role)
        {
            BoardContext.Current.Get<IAspNetUsersHelper>().AddToRole(user, role);
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
        public static int? CreateForumUser([NotNull] AspNetUsers user, int pageBoardID)
        {
            return CreateForumUser(user, user.UserName, pageBoardID);
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
        public static int? CreateForumUser([NotNull] AspNetUsers user, [NotNull] string displayName, int pageBoardID)
        {
            int? userID = null;

            try
            {
                userID = BoardContext.Current.GetRepository<User>().Aspnet(
                    pageBoardID,
                    user.UserName,
                    displayName,
                    user.Email,
                    user.Id,
                    user.IsApproved);

                GetRolesForUser(user).ForEach(
                    role => BoardContext.Current.GetRepository<User>().SetRole(
                        pageBoardID,
                        user.Id,
                        role));
            }
            catch (Exception x)
            {
                BoardContext.Current.Get<ILogger>().Error(x, "Error in CreateForumUser");
            }

            return userID;
        }

        /// <summary>
        /// The create role.
        /// </summary>
        /// <param name="roleName">
        /// The role name.
        /// </param>
        public static void CreateRole([NotNull] string roleName)
        {
            var role = new AspNetRoles { Name = roleName };

            BoardContext.Current.Get<IAspNetRoleManager>().Create(role);
        }

        /// <summary>
        /// The delete role.
        /// </summary>
        /// <param name="roleName">
        /// The role name.
        /// </param>
        public static void DeleteRole([NotNull] string roleName)
        {
            var role = BoardContext.Current.Get<IAspNetRoleManager>().FindByName(roleName);

            BoardContext.Current.Get<IAspNetRoleManager>().Delete(role);
        }

        /// <summary>
        /// Check if the forum user was created.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pageBoardID">The page board ID.</param>
        /// <returns>
        /// The did create forum user.
        /// </returns>
        public static bool DidCreateForumUser([NotNull] AspNetUsers user, int pageBoardID)
        {
            var userID = CreateForumUser(user, pageBoardID);
            return userID != null;
        }

        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <returns>
        /// Returns all Roles
        /// </returns>
        public static List<string> GetAllRoles()
        {
            return BoardContext.Current.Get<IAspNetRoleManager>().Roles.Select(r => r.Name).ToList();
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
        public static IList<string> GetRolesForUser([NotNull] AspNetUsers user)
        {
            return BoardContext.Current.Get<IAspNetRoleManager>().GetRoles(user);
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
        public static List<AspNetUsers> GetUsersInRole(string roleName)
        {
            var role = BoardContext.Current.Get<IAspNetRoleManager>().FindByName(roleName);

            var users = BoardContext.Current.GetRepository<AspNetUserRoles>().Get(r => r.RoleId == role.Id);

            var userList = new List<AspNetUsers>();

            BoardContext.Current.Get<IAspNetUsersHelper>().Users.ForEach(user =>
            {
                if (users.Any(u => u.UserId == user.Id))
                {
                    userList.Add(user);
                }
            });

            return userList;
        }

        /// <summary>
        /// Groups the in group table.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <param name="groupTable">The group table.</param>
        /// <returns>
        /// The group in group table.
        /// </returns>
        public static bool GroupInGroupTable([NotNull] string groupName, [NotNull] DataTable groupTable)
        {
            return groupTable.AsEnumerable().Any(
                row => row["Member"].ToType<int>() == 1 && row["Name"].ToType<string>() == groupName);
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
        public static bool IsUserInRole([NotNull] AspNetUsers user, [NotNull] string role)
        {
            return BoardContext.Current.Get<IAspNetUsersHelper>().IsInRole(user, role);
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
        public static void RemoveUserFromRole([NotNull] string userProviderKey, [NotNull] string role)
        {
            BoardContext.Current.Get<IAspNetUsersHelper>().RemoveFromRole(userProviderKey, role);
        }

        /// <summary>
        /// Roles the exists.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>
        /// The role exists.
        /// </returns>
        public static bool RoleExists([NotNull] string roleName)
        {
            return BoardContext.Current.Get<IAspNetRoleManager>().RoleExists(roleName);
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
        public static void SetupUserRoles(int pageBoardID, [NotNull] AspNetUsers user)
        {
            (from @group in BoardContext.Current.GetRepository<Group>().List(boardId: pageBoardID)
             let roleFlags = new GroupFlags(@group.Flags)
             where roleFlags.IsStart && !roleFlags.IsGuest
             select @group.Name
             into roleName
             where roleName.IsSet()
             where !IsUserInRole(user, roleName)
             select roleName).ForEach(roleName => AddUserToRole(user, roleName));
        }

        /// <summary>
        /// Goes through every membership user and manually "syncs" them to the forum.
        ///   Best for an existing membership structure -- will migrate all users at once 
        ///   rather then one at a time...
        /// </summary>
        /// <param name="pageBoardId">
        /// The page Board Id.
        /// </param>
        public static void SyncAllMembershipUsers(int pageBoardId)
        {
            // get all users in membership...
            var users = BoardContext.Current.Get<IAspNetUsersHelper>().Users.Where(u => u != null && u.Email.IsSet());

            // create/update users...
            Parallel.ForEach(users, user => UpdateForumUser(user, pageBoardId));
        }

        /// <summary>
        /// Syncs the ASP.NET roles with YAF group based on YAF (not bi-directional)
        /// </summary>
        /// <param name="pageBoardID">The page board ID.</param>
        public static void SyncRoles(int pageBoardID)
        {
            // get all the groups in YAF DB and create them if they do not exist as a role in membership
            (from @group in BoardContext.Current.GetRepository<Group>().List(boardId: pageBoardID)
             let name = @group.Name
             let roleFlags = new GroupFlags(@group.Flags)
             where name.IsSet() && !roleFlags.IsGuest && !RoleExists(name)
             select name).ForEach(CreateRole);
        }

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
        public static int? UpdateForumUser([NotNull] AspNetUsers user, int pageBoardID, string[] roles = null)
        {
            if (user == null)
            {
                // Check to make sure its not a guest
                return null;
            }

            var userId = BoardContext.Current.Get<IAspNetUsersHelper>().GetUserIDFromProviderUserKey(user.Id);

            if (userId == BoardContext.Current.Get<IAspNetUsersHelper>().GuestUserId)
            {
                return userId;
            }

            if (user.Id == null)
            {
                // problem -- log and move on...
                BoardContext.Current.Get<ILogger>().Log(
                    userId,
                    "UpdateForumUser",
                    $"Null User Provider Key for UserName {user.UserName}. Please check your provider key settings for your ASP.NET membership provider.");

                return userId;
            }

            // is this a new user?
            var isNewUser = userId <= 0;

            userId = BoardContext.Current.GetRepository<User>().Aspnet(
                pageBoardID,
                user.UserName,
                null,
                user.Email,
                user.Id,
                user.IsApproved);

            // get user groups...
            var groupTable = BoardContext.Current.GetRepository<Group>().MemberAsDataTable(pageBoardID, userId);
            var userRoles = GetRolesForUser(user);

            if (Config.IsDotNetNuke && roles != null)
            {
                userRoles = roles;
            }

            if (Config.IsMojoPortal)
            {
                var roles1 = userRoles.Where(t => t.IsSet()).Aggregate(
                    string.Empty,
                    (current, t) => $"{current.Trim()},{t.Trim()}");
                userRoles = roles1.Trim(',').Split(',');
            }

            // add groups...
            userRoles.Where(role => !GroupInGroupTable(role, groupTable)).ForEach(
                role => BoardContext.Current.GetRepository<User>().SetRole(
                    pageBoardID,
                    user.Id,
                    role));

            // remove groups...remove since there is no longer an association in the membership...
            groupTable.AsEnumerable().Where(row => !userRoles.Contains(row["Name"].ToString())).ForEach(
                row => BoardContext.Current.GetRepository<UserGroup>().Save(userId, row["GroupID"], 0));

            if (!isNewUser || userId <= 0)
            {
                return userId;
            }

            try
            {
                var defaultNotificationSetting = BoardContext.Current.Get<BoardSettings>().DefaultNotificationSetting;

                var defaultSendDigestEmail = BoardContext.Current.Get<BoardSettings>().DefaultSendDigestEmail;

                // setup default notifications...
                var autoWatchTopicsEnabled =
                    defaultNotificationSetting == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

                // save the settings...
                BoardContext.Current.GetRepository<User>().SaveNotification(
                    userId,
                    true,
                    autoWatchTopicsEnabled,
                    defaultNotificationSetting.ToInt(),
                    defaultSendDigestEmail);
            }
            catch (Exception ex)
            {
                BoardContext.Current.Get<ILogger>().Log(
                    userId,
                    "UpdateForumUser",
                    $"Failed to save default notifications for new user: {ex}");
            }

            return userId;
        }

        #endregion
    }
}