/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Security;
    using YAF.Classes;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The role membership helper.
    /// </summary>
    public static class RoleMembershipHelper
    {
        #region Public Methods

        /// <summary>
        /// The add user to role.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        public static void AddUserToRole([NotNull] string username, [NotNull] string role)
        {
            YafContext.Current.Get<RoleProvider>().AddUsersToRoles(new[] { username }, new[] { role });
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
        public static int? CreateForumUser([NotNull] MembershipUser user, int pageBoardID)
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
        public static int? CreateForumUser([NotNull] MembershipUser user, [NotNull] string displayName, int pageBoardID)
        {
            int? userID = null;

            try
            {
                userID = YafContext.Current.GetRepository<User>().Aspnet(pageBoardID, user.UserName, displayName, user.Email, user.ProviderUserKey, user.IsApproved);

                GetRolesForUser(user.UserName).ForEach(
                    role => YafContext.Current.GetRepository<User>().SetRole(
                        pageBoardID,
                        user.ProviderUserKey.ToString(),
                        role));
            }
            catch (Exception x)
            {
                YafContext.Current.Get<ILogger>().Error(x, "Error in CreateForumUser");
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
            YafContext.Current.Get<RoleProvider>().CreateRole(roleName);
        }

        /// <summary>
        /// The delete role.
        /// </summary>
        /// <param name="roleName">
        /// The role name.
        /// </param>
        /// <param name="throwOnPopulatedRole">
        /// The throw on populated role.
        /// </param>
        public static void DeleteRole([NotNull] string roleName, bool throwOnPopulatedRole)
        {
            YafContext.Current.Get<RoleProvider>().DeleteRole(roleName, throwOnPopulatedRole);
        }

        /// <summary>
        /// Check if the forum user was created.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pageBoardID">The page board ID.</param>
        /// <returns>
        /// The did create forum user.
        /// </returns>
        public static bool DidCreateForumUser([NotNull] MembershipUser user, int pageBoardID)
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
        public static string[] GetAllRoles()
        {
            return YafContext.Current.Get<RoleProvider>().GetAllRoles();
        }

        /// <summary>
        /// Gets the roles for the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        /// Returns all Roles
        /// </returns>
        public static string[] GetRolesForUser([NotNull] string username)
        {
            return YafContext.Current.Get<RoleProvider>().GetRolesForUser(username);
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
            return
                groupTable.AsEnumerable()
                    .Any(row => row["Member"].ToType<int>() == 1 && row["Name"].ToType<string>() == groupName);
        }

        /// <summary>
        /// Determines whether [is user in role] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="role">The role.</param>
        /// <returns>
        /// The is user in role.
        /// </returns>
        public static bool IsUserInRole([NotNull] string username, [NotNull] string role)
        {
            return YafContext.Current.Get<RoleProvider>().IsUserInRole(username, role);
        }

        /// <summary>
        /// The remove user from role.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        public static void RemoveUserFromRole([NotNull] string username, [NotNull] string role)
        {
            YafContext.Current.Get<RoleProvider>().RemoveUsersFromRoles(new[] { username }, new[] { role });
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
            return YafContext.Current.Get<RoleProvider>().RoleExists(roleName);
        }

        /// <summary>
        /// Sets up the user roles from the "start" settings for a given group/role
        /// </summary>
        /// <param name="pageBoardID">Current BoardID</param>
        /// <param name="userName">Name of the user.</param>
        public static void SetupUserRoles(int pageBoardID, [NotNull] string userName)
        {
            foreach (
                var roleName in from @group in YafContext.Current.GetRepository<Group>().List(boardId: pageBoardID)
                                let roleFlags = new GroupFlags(@group.Flags)
                                where roleFlags.IsStart && !roleFlags.IsGuest
                                select @group.Name
                                into roleName
                                where roleName.IsSet()
                                where !IsUserInRole(userName, roleName)
                                select roleName)
            {
                AddUserToRole(userName, roleName);
            }
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
            int totalRecords;

            // get all users in membership...
            var users =
                YafContext.Current.Get<MembershipProvider>()
                    .GetAllUsers(0, 999999, out totalRecords)
                    .Cast<MembershipUser>()
                    .Where(u => u != null && u.Email.IsSet());

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
            foreach (
                var name in from @group in YafContext.Current.GetRepository<Group>().List(boardId: pageBoardID)
                            let name = @group.Name
                            let roleFlags = new GroupFlags(@group.Flags)
                            where name.IsSet() && !roleFlags.IsGuest && !RoleExists(name)
                            select name)
            {
                CreateRole(name);
            }
        }

        /// <summary>
        /// Takes all YAF users and creates them in the Membership Provider
        /// </summary>
        /// <param name="pageBoardID">The page board ID.</param>
        public static void SyncUsers(int pageBoardID)
        {
            // first sync unapproved users...
            using (var dt = YafContext.Current.GetRepository<User>().ListAsDataTable(pageBoardID, null, false))
            {
                MigrateUsersFromDataTable(false, dt);
            }

            // then sync approved users...
            using (var dt = YafContext.Current.GetRepository<User>().ListAsDataTable(pageBoardID, null, true))
            {
                MigrateUsersFromDataTable(true, dt);
            }
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
        public static int? UpdateForumUser([NotNull] MembershipUser user, int pageBoardID, string[] roles = null)
        {
            if (user == null)
            {
                // Check to make sure its not a guest
                return null;
            }

            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

            if (userId == UserMembershipHelper.GuestUserId)
            {
                return userId;
            }

            if (user.ProviderUserKey == null)
            {
                // problem -- log and move on...
                YafContext.Current.Get<ILogger>()
                    .Log(
                        userId,
                        "UpdateForumUser",
                        $"Null User Provider Key for UserName {user.UserName}. Please check your provider key settings for your ASP.NET membership provider.");

                return userId;
            }

            // is this a new user?
            var isNewUser = userId <= 0;

            userId = YafContext.Current.GetRepository<User>().Aspnet(
                pageBoardID,
                user.UserName,
                null,
                user.Email,
                user.ProviderUserKey,
                user.IsApproved);

            // get user groups...
            var groupTable = YafContext.Current.GetRepository<Group>().MemberAsDataTable(pageBoardID, userId);
            var userRoles = GetRolesForUser(user.UserName);

            if (Config.IsDotNetNuke && roles != null)
            {
                userRoles = roles;
            }

            if (Config.IsMojoPortal)
            {
                var roles1 = userRoles.Where(t => !string.IsNullOrEmpty(t))
                    .Aggregate(string.Empty, (current, t) => current.Trim() + "," + t.Trim());
                userRoles = roles1.Trim(',').Split(',');
            }

            // add groups...
            foreach (var role in userRoles.Where(role => !GroupInGroupTable(role, groupTable)))
            {
                // add the role...
                YafContext.Current.GetRepository<User>().SetRole(pageBoardID, user.ProviderUserKey.ToString(), role);
            }

            // remove groups...
            foreach (var row in
                groupTable.AsEnumerable().Where(row => !userRoles.Contains(row["Name"].ToString())))
            {
                // remove since there is no longer an association in the membership...
                YafContext.Current.GetRepository<UserGroup>().Save(userId, row["GroupID"], 0);
            }

            if (!isNewUser || userId <= 0)
            {
                return userId;
            }

            try
            {
                var defaultNotificationSetting =
                    YafContext.Current.Get<YafBoardSettings>().DefaultNotificationSetting;

                var defaultSendDigestEmail = YafContext.Current.Get<YafBoardSettings>().DefaultSendDigestEmail;

                // setup default notifications...
                var autoWatchTopicsEnabled = defaultNotificationSetting
                                              == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

                // save the settings...
                YafContext.Current.GetRepository<User>().SaveNotification(
                    userId,
                    true,
                    autoWatchTopicsEnabled,
                    defaultNotificationSetting,
                    defaultSendDigestEmail);
            }
            catch (Exception ex)
            {
                YafContext.Current.Get<ILogger>()
                    .Log(
                        userId,
                        "UpdateForumUser",
                        $"Failed to save default notifications for new user: {ex}");
            }

            return userId;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Migrates the create user.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="question">The question.</param>
        /// <param name="answer">The answer.</param>
        /// <param name="approved">The approved.</param>
        /// <param name="user">The user.</param>
        /// <returns>
        /// The migrate create user.
        /// </returns>
        private static MembershipCreateStatus MigrateCreateUser(
            [NotNull] string name,
            [NotNull] string email,
            [NotNull] string question,
            [NotNull] string answer,
            bool approved,
            [NotNull] out MembershipUser user)
        {
            MembershipCreateStatus status;

            // create a new user and generate a password.
            var retry = 0;
            do
            {
                var password = Membership.GeneratePassword(7 + retry, 1 + retry);
                user = YafContext.Current.Get<MembershipProvider>()
                    .CreateUser(name, password, email, question, answer, approved, null, out status);
            }
            while (status == MembershipCreateStatus.InvalidPassword && ++retry < 10);

            return status;
        }

        /// <summary>
        /// The migrate users from <paramref name="dataTable"/>.
        /// </summary>
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="dataTable">
        /// The dataTable.
        /// </param>
        private static void MigrateUsersFromDataTable(bool approved, [NotNull] DataTable dataTable)
        {
            // is this the Yaf membership provider?
            var isYafProvider = YafContext.Current.Get<MembershipProvider>().Name == "YafMembershipProvider";
            var isLegacyYafDB = dataTable.Columns.Contains("Location");

            var userRows = dataTable.AsEnumerable();

            Parallel.ForEach(
                userRows,
                row =>
                    {
                        // skip the guest user
                        if (row.Field<bool>("IsGuest"))
                        {
                            return;
                        }

                        // validate that name and email are available...
                        if (row["Name"].IsNullOrEmptyDBField() || row["Email"].IsNullOrEmptyDBField())
                        {
                            return;
                        }

                        var name = row.Field<string>("Name").Trim();
                        var email = row.Field<string>("Email").ToLower().Trim();

                        // clean up the name by removing commas...
                        name = name.Replace(",", string.Empty);

                        // verify this user & email are not empty
                        if (!name.IsSet() || !email.IsSet())
                        {
                            return;
                        }

                        var user = UserMembershipHelper.GetUser(name, false);

                        if (user == null)
                        {
                            var status = MigrateCreateUser(
                                name,
                                email,
                                "Your email in all lower case",
                                email,
                                approved,
                                out user);

                            if (status != MembershipCreateStatus.Success)
                            {
                                YafContext.Current.Get<ILogger>()
                                    .Log(0, "MigrateUsers", $"Failed to create user {name}: {status}");
                            }
                            else
                            {
                                // update the YAF table with the ProviderKey -- update the provider table if this is the YAF provider...
                                YafContext.Current.GetRepository<User>().Migrate(row["UserID"].ToType<int>(), user.ProviderUserKey.ToString(), isYafProvider);

                                user.Comment = "Migrated from YetAnotherForum.NET";

                                YafContext.Current.Get<MembershipProvider>().UpdateUser(user);

                                if (!isYafProvider)
                                {
                                    /* Email generated password to user
                            System.Text.StringBuilder msg = new System.Text.StringBuilder();
                            msg.AppendFormat( "Hello {0}.\r\n\r\n", name );
                            msg.AppendFormat( "Here is your new password: {0}\r\n\r\n", password );
                            msg.AppendFormat( "Visit {0} at {1}", ForumName, ForumURL );

                            YAF.Classes.Data.DB.mail_create( ForumEmail, user.Email, "Forum Upgrade", msg.ToString() );
                            */
                                }
                            }

                            if (isLegacyYafDB)
                            {
                                // copy profile data over...
                                var userProfile = YafUserProfile.GetProfile(name);

                                if (dataTable.Columns.Contains("ICQ") && !row["ICQ"].IsNullOrEmptyDBField())
                                {
                                    userProfile.ICQ = row["ICQ"].ToString();
                                }

                                if (dataTable.Columns.Contains("RealName") && !row["RealName"].IsNullOrEmptyDBField())
                                {
                                    userProfile.RealName = row["RealName"].ToString();
                                }

                                if (dataTable.Columns.Contains("Occupation")
                                    && !row["Occupation"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Occupation = row["Occupation"].ToString();
                                }

                                if (dataTable.Columns.Contains("Location") && !row["Location"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Location = row["Location"].ToString();
                                }

                                if (dataTable.Columns.Contains("Homepage") && !row["Homepage"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Homepage = row["Homepage"].ToString();
                                }

                                if (dataTable.Columns.Contains("Interests") && !row["Interests"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Interests = row["Interests"].ToString();
                                }

                                if (dataTable.Columns.Contains("Weblog") && !row["Weblog"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Blog = row["Weblog"].ToString();
                                }

                                if (dataTable.Columns.Contains("Gender") && !row["Gender"].IsNullOrEmptyDBField())
                                {
                                    userProfile.Gender = row["Gender"].ToType<int>();
                                }

                                userProfile.Save();
                            }
                        }
                        else
                        {
                            // just update the link just in case...
                            YafContext.Current.GetRepository<User>().Migrate(row["UserID"].ToType<int>(), user.ProviderUserKey.ToString(), false);
                        }

                        // setup roles for this user...
                        using (var dtGroups = YafContext.Current.GetRepository<UserGroup>().ListAsDataTable(row["UserID"]))
                        {
                            foreach (DataRow rowGroup in dtGroups.Rows)
                            {
                                AddUserToRole(user.UserName, rowGroup["Name"].ToString());
                            }
                        }
                    });
        }

        #endregion
    }
}