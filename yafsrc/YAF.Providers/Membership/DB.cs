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

namespace YAF.Providers.Membership
{
    #region Using

    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web.Security;

    using YAF.Configuration.Pattern;
    using YAF.Core.Data;
    using YAF.Providers.Utils;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Extensions.Data;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The db.
    /// </summary>
    public class DB : BaseProviderDb
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DB" /> class.
        /// </summary>
        public DB()
            : base(YafMembershipProvider.ConnStrAppKeyName)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Current.
        /// </summary>
        public static DB Current => PageSingleton<DB>.Instance;

        #endregion

        #region Public Methods

        /// <summary>
        /// The change password.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <param name="newSalt">
        /// The new salt.
        /// </param>
        /// <param name="passwordFormat">
        /// The password format.
        /// </param>
        /// <param name="newPasswordAnswer">
        /// The new password answer.
        /// </param>
        public void ChangePassword([NotNull] string appName, [NotNull] string username, [NotNull] string newPassword,
            [NotNull] string newSalt, int passwordFormat, [NotNull] string newPasswordAnswer)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_changepassword")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", username);
                cmd.Parameters.AddWithValue("@Password", newPassword);
                cmd.Parameters.AddWithValue("@PasswordSalt", newSalt);
                cmd.Parameters.AddWithValue("@PasswordFormat", passwordFormat);
                cmd.Parameters.AddWithValue("@PasswordAnswer", newPasswordAnswer);

                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The change password question and answer.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="passwordQuestion">
        /// The password question.
        /// </param>
        /// <param name="passwordAnswer">
        /// The password answer.
        /// </param>
        public void ChangePasswordQuestionAndAnswer([NotNull] string appName, [NotNull] string username,
            [NotNull] string passwordQuestion, [NotNull] string passwordAnswer)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_changepasswordquestionandanswer")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", username);
                cmd.Parameters.AddWithValue("@PasswordQuestion", passwordQuestion);
                cmd.Parameters.AddWithValue("@PasswordAnswer", passwordAnswer);
                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The create user.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="passwordSalt">
        /// The password salt.
        /// </param>
        /// <param name="passwordFormat">
        /// The password format.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="passwordQuestion">
        /// The password question.
        /// </param>
        /// <param name="passwordAnswer">
        /// The password answer.
        /// </param>
        /// <param name="isApproved">
        /// The is approved.
        /// </param>
        /// <param name="providerUserKey">
        /// The provider user key.
        /// </param>
        public void CreateUser([NotNull] string appName, [NotNull] string username, [NotNull] string password,
            [NotNull] string passwordSalt,
            int passwordFormat, [NotNull] string email, [NotNull] string passwordQuestion,
            [NotNull] string passwordAnswer,
            bool isApproved, [NotNull] object providerUserKey)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_createuser")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Input Parameters
                cmd.Parameters.AddWithValue("Username", username);
                cmd.Parameters.AddWithValue("Password", password);
                cmd.Parameters.AddWithValue("PasswordSalt", passwordSalt);
                cmd.Parameters.AddWithValue("PasswordFormat", passwordFormat);
                cmd.Parameters.AddWithValue("Email", email);
                cmd.Parameters.AddWithValue("PasswordQuestion", passwordQuestion);
                cmd.Parameters.AddWithValue("PasswordAnswer", passwordAnswer);
                cmd.Parameters.AddWithValue("IsApproved", isApproved);
                cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);

                // Input Output Parameters
                var paramUserKey = new SqlParameter("UserKey", SqlDbType.UniqueIdentifier)
                                       {
                                           Direction = ParameterDirection.InputOutput, Value = providerUserKey
                                       };
                cmd.Parameters.Add(paramUserKey);

                // Execute
                this.DbAccess.ExecuteNonQuery(cmd);

                // Retrieve Output Parameters
                providerUserKey = paramUserKey.Value;
            }
        }

        /// <summary>
        /// The delete user.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="deleteAllRelatedData">
        /// The delete all related data.
        /// </param>
        public void DeleteUser([NotNull] string appName, [NotNull] string username, bool deleteAllRelatedData)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_deleteuser")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", username);
                cmd.Parameters.AddWithValue("@DeleteAllRelated", deleteAllRelatedData);
                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The find users by email.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="emailToMatch">
        /// The email to match.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// </returns>
        public DataTable FindUsersByEmail([NotNull] string appName, [NotNull] string emailToMatch, int pageIndex,
            int pageSize)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_findusersbyemail")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@EmailAddress", emailToMatch);
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return this.DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// The find users by name.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="usernameToMatch">
        /// The username to match.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// </returns>
        public DataTable FindUsersByName([NotNull] string appName, [NotNull] string usernameToMatch, int pageIndex,
            int pageSize)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_findusersbyname")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", usernameToMatch);
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return this.DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// The get all users.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// </returns>
        public DataTable GetAllUsers([NotNull] string appName, int pageIndex, int pageSize)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_getallusers")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return this.DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// The get number of users online.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="timeWindow">
        /// The time window.
        /// </param>
        /// <returns>
        /// The get number of users online.
        /// </returns>
        public int GetNumberOfUsersOnline([NotNull] string appName, int timeWindow)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_getnumberofusersonline")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@TimeWindow", timeWindow);
                cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);
                var p = new SqlParameter("ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
                cmd.Parameters.Add(p);
                this.DbAccess.ExecuteNonQuery(cmd);
                return cmd.Parameters["ReturnValue"].Value.ToType<int>();
            }
        }

        /// <summary>
        /// The get user.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="providerUserKey">
        /// The provider user key.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="userIsOnline">
        /// The user is online.
        /// </param>
        /// <returns>
        /// </returns>
        public DataRow GetUser([NotNull] string appName, [NotNull] object providerUserKey, [NotNull] string userName,
            bool userIsOnline)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_getuser")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@UserKey", providerUserKey);
                cmd.Parameters.AddWithValue("@UserIsOnline", userIsOnline);
                cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);

                using (var dt = this.DbAccess.GetData(cmd))
                {
                    return dt.GetFirstRow();
                }
            }
        }

        /// <summary>
        /// The get user name by email.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// </returns>
        public DataTable GetUserNameByEmail([NotNull] string appName, [NotNull] string email)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_getusernamebyemail")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@Email", email);
                return this.DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// The get user password info.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="updateUser">
        /// The update user.
        /// </param>
        /// <returns>
        /// </returns>
        public DataTable GetUserPasswordInfo([NotNull] string appName, [NotNull] string username, bool updateUser)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_getuser")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", username);
                cmd.Parameters.AddWithValue("@UserIsOnline", updateUser);
                cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);
                return this.DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// The reset password.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="passwordSalt">
        /// The password salt.
        /// </param>
        /// <param name="passwordFormat">
        /// The password format.
        /// </param>
        /// <param name="maxInvalidPasswordAttempts">
        /// The max invalid password attempts.
        /// </param>
        /// <param name="passwordAttemptWindow">
        /// The password attempt window.
        /// </param>
        public void ResetPassword([NotNull] string appName, [NotNull] string userName, [NotNull] string password,
            [NotNull] string passwordSalt,
            int passwordFormat,
            int maxInvalidPasswordAttempts,
            int passwordAttemptWindow)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_resetpassword")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
                cmd.Parameters.AddWithValue("@PasswordFormat", passwordFormat);
                cmd.Parameters.AddWithValue("@MaxInvalidAttempts", maxInvalidPasswordAttempts);
                cmd.Parameters.AddWithValue("@PasswordAttemptWindow", passwordAttemptWindow);
                cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);

                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The unlock user.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        public void UnlockUser([NotNull] string appName, [NotNull] string userName)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_unlockuser")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", userName);
                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The update user.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="requiresUniqueEmail">
        /// The requires unique email.
        /// </param>
        /// <returns>
        /// The update user.
        /// </returns>
        public int UpdateUser([NotNull] object appName, [NotNull] MembershipUser user, bool requiresUniqueEmail)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_updateuser")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);

                // Nonstandard args
                cmd.Parameters.AddWithValue("UserKey", user.ProviderUserKey);
                cmd.Parameters.AddWithValue("UserName", user.UserName);
                cmd.Parameters.AddWithValue("Email", user.Email);
                cmd.Parameters.AddWithValue("Comment", user.Comment);
                cmd.Parameters.AddWithValue("IsApproved", user.IsApproved);
                cmd.Parameters.AddWithValue("LastLogin", user.LastLoginDate);
                cmd.Parameters.AddWithValue("LastActivity", user.LastActivityDate.ToUniversalTime());
                cmd.Parameters.AddWithValue("UniqueEmail", requiresUniqueEmail);

                // Add Return Value
                var p = new SqlParameter("ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
                cmd.Parameters.Add(p);

                this.DbAccess.ExecuteNonQuery(cmd); // Execute Non SQL Query
                return p.Value.ToType<int>(); // Return
            }
        }

        /// <summary>
        /// The upgrade membership.
        /// </summary>
        /// <param name="previousVersion">
        /// The previous version.
        /// </param>
        /// <param name="newVersion">
        /// The new version.
        /// </param>
        public void UpgradeMembership(int previousVersion, int newVersion)
        {
            using (var cmd = new SqlCommand(CommandTextHelpers.GetObjectName("prov_upgrade")))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Nonstandard args
                cmd.Parameters.AddWithValue("@PreviousVersion", previousVersion);
                cmd.Parameters.AddWithValue("@NewVersion", newVersion);
                cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);
                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        #endregion
    }
}