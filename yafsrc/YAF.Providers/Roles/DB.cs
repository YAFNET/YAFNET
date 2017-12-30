/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

namespace YAF.Providers.Roles
{
    #region Using

    using System;
    using System.Data;
    using System.Data.SqlClient;

    using YAF.Classes.Data;
    using YAF.Classes.Pattern;
    using YAF.Providers.Utils;
    using YAF.Types.Interfaces.Data;

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
            : base(YafRoleProvider.ConnStrAppKeyName)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Current.
        /// </summary>
        public static DB Current
        {
            get { return PageSingleton<DB>.Instance; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Database Action - Add User to Role
        /// </summary>
        /// <param name="appName">
        /// Application Name
        /// </param>
        /// <param name="userName">
        /// User Name
        /// </param>
        /// <param name="roleName">
        /// Role Name
        /// </param>
        public void AddUserToRole(object appName, object userName, object roleName)
        {
            using (var cmd = new SqlCommand(DbHelpers.GetObjectName("prov_role_addusertorole")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("Rolename", roleName);
                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Database Action - Create Role
        /// </summary>
        /// <param name="appName">
        /// Application Name
        /// </param>
        /// <param name="roleName">
        /// Role Name
        /// </param>
        public void CreateRole(object appName, object roleName)
        {
            using (var cmd = new SqlCommand(DbHelpers.GetObjectName("prov_role_createrole")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("Rolename", roleName);
                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Database Action - Delete Role
        /// </summary>
        /// <param name="appName">
        /// Application Name
        /// </param>
        /// <param name="roleName">
        /// Role Name
        /// </param>
        /// <param name="deleteOnlyIfRoleIsEmpty">
        /// The delete Only If Role Is Empty.
        /// </param>
        /// <returns>
        /// Status as integer
        /// </returns>
        public int DeleteRole(object appName, object roleName, object deleteOnlyIfRoleIsEmpty)
        {
            using (var cmd = new SqlCommand(DbHelpers.GetObjectName("prov_role_deleterole")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("Rolename", roleName);
                cmd.Parameters.AddWithValue("DeleteOnlyIfRoleIsEmpty", deleteOnlyIfRoleIsEmpty);

                var p = new SqlParameter("ReturnValue", SqlDbType.Int);
                p.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(p);

                this.DbAccess.ExecuteNonQuery(cmd);

                return Convert.ToInt32(cmd.Parameters["ReturnValue"].Value);
            }
        }

        /// <summary>
        /// Database Action - Find Users in Role
        /// </summary>
        /// <param name="appName">
        /// Application Name
        /// </param>
        /// <param name="roleName">
        /// Role Name
        /// </param>
        /// <returns>
        /// Datatable containing User Information
        /// </returns>
        public DataTable FindUsersInRole(object appName, object roleName)
        {
            using (var cmd = new SqlCommand(DbHelpers.GetObjectName("prov_role_findusersinrole")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("Rolename", roleName);
                return this.DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// Database Action - Get Role Exists
        /// </summary>
        /// <param name="appName">
        /// Application Name
        /// </param>
        /// <param name="roleName">
        /// Role Name
        /// </param>
        /// <returns>
        /// Database containing Role Information
        /// </returns>
        public object GetRoleExists(object appName, object roleName)
        {
            using (var cmd = new SqlCommand(DbHelpers.GetObjectName("prov_role_exists")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("RoleName", roleName);
                return this.DbAccess.ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// Database Action - Get Roles
        /// </summary>
        /// <param name="appName">
        /// Application Name
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// Database containing Role Information
        /// </returns>
        public DataTable GetRoles(object appName, object username)
        {
            using (var cmd = new SqlCommand(DbHelpers.GetObjectName("prov_role_getroles")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("UserName", username);
                return this.DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// Database Action - Add User to Role
        /// </summary>
        /// <param name="appName">
        /// Application Name
        /// </param>
        /// <param name="userName">
        /// User Name
        /// </param>
        /// <param name="roleName">
        /// Role Name
        /// </param>
        /// <returns>
        /// DataTable with user information
        /// </returns>
        public DataTable IsUserInRole(object appName, object userName, object roleName)
        {
            using (var cmd = new SqlCommand(DbHelpers.GetObjectName("prov_role_isuserinrole")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("Rolename", roleName);
                return this.DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// Database Action - Remove User From Role
        /// </summary>
        /// <param name="appName">
        /// Application Name
        /// </param>
        /// <param name="userName">
        /// User Name
        /// </param>
        /// <param name="roleName">
        /// Role Name
        /// </param>
        public void RemoveUserFromRole(object appName, string userName, string roleName)
        {
            using (var cmd = new SqlCommand(DbHelpers.GetObjectName("prov_role_removeuserfromrole")))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("Rolename", roleName);
                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        #endregion
    }
}