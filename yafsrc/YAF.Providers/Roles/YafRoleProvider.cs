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
using System.Data;
using System.Web;
using System.Web.Profile;
using System.Web.Hosting;
using System.Web.DataAccess;
using System.Web.Util;
using System.Web.Configuration;
using System.Web.Security;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Configuration.Provider;
using System.Configuration;
using System.Security;
using System.Security.Principal;
using System.Security.Permissions;
using System.Globalization;
using System.Runtime.Serialization;

using YAF.Classes.Data;

namespace YAF.Providers.Roles
{
    public class YAFRoleProvider : RoleProvider
    {
        private string _appName;

        #region Override Public Properties

        public override string ApplicationName
        {
            get
            {
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }
        #endregion

        #region Overriden Public Methods

        /// <summary>
        /// Sets up the profile providers
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // verify that the configuration section was properly passed
            if (config == null)
                throw new ArgumentNullException("config");

            base.Initialize(name, config);

            // application name
            _appName = config["applicationName"];
            if (string.IsNullOrEmpty(_appName))
                _appName = "YetAnotherForum";
        }

        /// <summary>
        /// Adds a list of users to a list of groups
        /// </summary>
        /// <param name="usernames">List of Usernames</param>
        /// <param name="roleNames">List of Rolenames</param>
        /// <returns></returns>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            // Loop through username
            foreach (string username in usernames)
            {
                // Loop through roles
                foreach (string roleName in roleNames)
                {
                    DB.AddUserToRole(this.ApplicationName, username, roleName); // Remove roll
                }
            }
        }

        /// <summary>
        /// Creates a role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public override void CreateRole(string roleName)
        {
            if (String.IsNullOrEmpty(roleName))
                throw new ArgumentException("Role Name cannot be blank");

            DB.CreateRole(this.ApplicationName, roleName);
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Deletes a role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="throwOnPopulatedRole"></param>
        /// <returns>True or False</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            int returnValue = DB.DeleteRole(this.ApplicationName, roleName, throwOnPopulatedRole);

            // zero means there were no complications...
            if (returnValue == 0) return true;

            // it failed for some reason...
            return false;
        }

        /// <summary>
        /// Adds a list of users to a list of groups
        /// </summary>
        /// <param name="roleName">Rolename</param>
        /// <param name="usernameToMatch">like Username used in search</param>
        /// <returns>List of Usernames</returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (String.IsNullOrEmpty(roleName))
                throw new ArgumentException("Role name cannot be null or empty");
            // Roles
            DataTable users = DB.FindUsersInRole(this.ApplicationName, roleName, usernameToMatch);
            string[] userList;
            foreach (DataRow user in users)
            {
                userList = user["Username"].ToString();
            }
        }

        /// <summary>
        /// Grabs all the roles from the DB
        /// </summary>
        /// <returns></returns>
        public override string[] GetAllRoles()
        {
            // get all roles...
            DataTable roles = YAFProviders.Roles.DB.GetRoles(this.ApplicationName, null);

            // make a string collection to store the role list...
            StringCollection sc = new StringCollection();

            foreach (DataRow row in roles.Rows)
            {
                sc.Add(row["Name"].ToString());
            }

            // return as a string array
            String[] strReturn = new String[sc.Count];
            sc.CopyTo(strReturn, 0);

            return strReturn;
        }

        /// <summary>
        /// Grabs all the roles from the DB
        /// </summary>
        /// <returns></returns>
        public override string[] GetRolesForUser(string username)
        {
            if (String.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty");

            DataTable roles = DB.GetRoles(this.ApplicationName, username);
            string[] roleNames;
            foreach (DataRow dr in roles)
            {
                roleNames = dr["Username"];
            }
        }

        /// <summary>
        /// Gets a list of usernames in a a particular role
        /// </summary>
        /// <param name="roleName">Rolename</param>
        /// <returns>List of Usernames</returns>
        public override string[] GetUsersInRole(string roleName)
        {
            if (String.IsNullOrEmpty(roleName))
                throw new ArgumentException("Role name cannot be null or empty");

            DataTable users = DB.GetUsersInRole(this.ApplicationName, roleName);
            string[] userNames;
            foreach (DataRow dr in users)
            {
                userNames = dr["RoleName"];
            }
        }

        /// <summary>
        /// Check to see if user belongs to a role
        /// </summary>
        /// <param name="usernames">Username</param>
        /// <param name="roleNames">Rolename</param>
        /// <returns>True/False</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            DataTable roles = DB.IsUserInRole(this.ApplicationName, username, roleName);

            if (roles.Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Remove Users From Roles
        /// </summary>
        /// <param name="usernames">Usernames</param>
        /// <param name="roleNames">Rolenames</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            // Loop through username
            foreach (string username in usernames)
            {
                // Loop through roles
                foreach (string roleName in roleNames)
                {
                    DB.RemoveUserFromRole(this.ApplicationName, username, roleName); // Remove roll
                }
            }
        }

        /// <summary>
        /// Check to see if a role exists
        /// </summary>
        /// <param name="roleName">Rolename</param>
        /// <returns>True/False</returns>
        public override bool RoleExists(string roleName)
        {
            // get this role...
            DataTable roles = DB.GetRoles(this.ApplicationName, roleName);

            // if there are any rows then this role exists...
            if (roles.Rows.Count > 0)
            {
                return true;
            }

            // doesn't exist
            return false;
        }

        #endregion
    }
}
