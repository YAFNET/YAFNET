/* Yet Another Forum.NET
 * Copyright (C) 2006-2007 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration.Provider;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;

using YAF.Classes.Data;

namespace YAF.Providers.Roles
{
    public class DB
    {
        /// <summary>
        /// Database Action - Add User to Role
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <param name="userName">User Name</param>
        /// <param name="roleNames">Role Name</param>
        /// <returns></returns>
        public static void AddUserToRole(object appName, object userName, object roleName)
        {
            using (SqlCommand cmd =new SqlCommand("yafprov_role_addusertorole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("Username", userName);
                cmd.Parameters.AddWithValue("RoleName", roleName);
                DBAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Database Action - Create Role
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <param name="roleNames">Role Name</param>
        /// <returns></returns>
        public static void CreateRole(object appName, object roleName)
        {
            using (SqlCommand cmd =new SqlCommand("yafprov_role_createrole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("RoleName", roleName);
                DBAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Database Action - Delete Role
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <param name="roleNames">Role Name</param>
        /// <returns>Status as integer</returns>
        public static int DeleteRole(object appName, object roleName, object deleteOnlyIfRoleIsEmpty)
        {
            using (SqlCommand cmd =new SqlCommand("yafprov_role_deleterole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("RoleName", roleName);
                cmd.Parameters.AddWithValue("DeleteOnlyIfRoleIsEmpty", deleteOnlyIfRoleIsEmpty);

                SqlParameter p = new SqlParameter("ReturnValue", SqlDbType.Int);
                p.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(p);

                DBAccess.ExecuteNonQuery(cmd);

                return Convert.ToInt32(cmd.Parameters["ReturnValue"].Value);
            }
        }

        /// <summary>
        /// Database Action - Find Users in Role
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <param name="roleNames">Role Name</param>
        /// <returns>Datatable containing User Information</returns>
        public static DataTable FindUsersInRole(object appName, object roleName)
        {
            using (SqlCommand cmd =new SqlCommand("yafprov_role_findusersinrole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("RoleName", roleName);
                return DBAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// Database Action - Get Roles
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <param name="roleNames">Role Name</param>
        /// <returns>Database containing Role Information</returns>
        public static DataTable GetRoles(object appName, object roleName)
        {
            using (SqlCommand cmd =new SqlCommand("yafprov_role_getroles"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("RoleName", roleName);
                return DBAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// Database Action - Add User to Role
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <param name="userName">User Name</param>
        /// <param name="roleNames">Role Name</param>
        /// <returns>DataTable with user information</returns>
        public static DataTable IsUserInRole(object appName, object username, object roleName)
        {
            using (SqlCommand cmd =new SqlCommand("yafprov_role_isuserinrole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("Username", username);
                cmd.Parameters.AddWithValue("RoleName", roleName);
                return DBAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// Database Action - Remove User From Role
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <param name="userName">User Name</param>
        /// <param name="roleNames">Role Name</param>
        /// <returns></returns>
        public static void RemoveUserFromRole(object appName, string username, string roleName)
        {
            using (SqlCommand cmd =new SqlCommand("yafprov_role_removeuserfromrole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("Username", username);
                cmd.Parameters.AddWithValue("RoleName", roleName);
                DBAccess.ExecuteNonQuery(cmd);
            }

        }
    }
}
