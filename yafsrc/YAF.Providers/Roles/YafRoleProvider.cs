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
namespace YAF.Providers.Roles
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Specialized;
    using System.Data;
    using System.Linq;
    using System.Web.Security;

    
    using YAF.Core.Context;
    using YAF.Providers.Utils;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
    /// The yaf role provider.
    /// </summary>
    public class YafRoleProvider : RoleProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The _app name.
        /// </summary>
        private string _appName;

        /// <summary>
        /// The _conn str name.
        /// </summary>
        private string _connStrName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Connection String App Key Name.
        /// </summary>
        public static string ConnStrAppKeyName { get; } = "YafRolesConnectionString";

        /// <summary>
        /// Gets or sets ApplicationName.
        /// </summary>
        public override string ApplicationName
        {
            get => this._appName;

            set
            {
                if (value != this._appName)
                {
                    this._appName = value;

                    // clear the cache for obvious reasons...
                    this.ClearUserRoleCache();
                }
            }
        }

        private ConcurrentDictionary<string, StringCollection> _userRoleCache;

        /// <summary>
        /// Gets UserRoleCache.
        /// </summary>
        private ConcurrentDictionary<string, StringCollection> UserRoleCache
        {
            get
            {
                var key = this.GenerateCacheKey("UserRoleDictionary");

                return this._userRoleCache ??
                       (this._userRoleCache =
                        BoardContext.Current.Get<IObjectStore>().GetOrSet(
                          key, () => new ConcurrentDictionary<string, StringCollection>()));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a list of users to a list of groups
        /// </summary>
        /// <param name="usernames">
        /// List of Usernames
        /// </param>
        /// <param name="roleNames">
        /// List of Rolenames
        /// </param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (usernames == null || usernames.Length == 0)
            {
                throw new ArgumentException("usernames is null or empty.", nameof(usernames));
            }

            if (roleNames == null || roleNames.Length == 0)
            {
                throw new ArgumentException("roleNames is null or empty.", nameof(roleNames));
            }

            // Loop through username
            foreach (var username in usernames)
            {
                var allRoles = this.GetAllRoles().ToList();

                // Loop through roles
                foreach (var roleName in roleNames)
                {
                    // only add user if this role actually exists...
                    if (roleName.IsSet() && allRoles.Contains(roleName))
                    {
                        DB.Current.AddUserToRole(this.ApplicationName, username, roleName);
                    }
                }

                // invalidate the cache for this user...
                this.DeleteFromRoleCacheIfExists(username.ToLower());
            }
        }

        /// <summary>
        /// Creates a role
        /// </summary>
        /// <param name="roleName">
        /// </param>
        public override void CreateRole(string roleName)
        {
            if (roleName.IsNotSet())
            {
                ExceptionReporter.ThrowArgument("ROLES", "ROLENAMEBLANK");
            }

            DB.Current.CreateRole(this.ApplicationName, roleName);
        }

        /// <summary>
        /// Deletes a role
        /// </summary>
        /// <param name="roleName">
        /// </param>
        /// <param name="throwOnPopulatedRole">
        /// </param>
        /// <returns>
        /// True or False
        /// </returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var returnValue = DB.Current.DeleteRole(this.ApplicationName, roleName, throwOnPopulatedRole);

            this.ClearUserRoleCache();

            // zero means there were no complications...
            return returnValue == 0;
        }

        /// <summary>
        /// Adds a list of users to a list of groups
        /// </summary>
        /// <param name="roleName">
        /// Rolename
        /// </param>
        /// <param name="usernameToMatch">
        /// like Username used in search
        /// </param>
        /// <returns>
        /// List of Usernames
        /// </returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (roleName.IsNotSet())
            {
                ExceptionReporter.ThrowArgument("ROLES", "ROLENAMEBLANK");
            }

            // Roles
            var users = DB.Current.FindUsersInRole(this.ApplicationName, roleName);
            var usernames = new StringCollection();
            foreach (DataRow user in users.Rows)
            {
                usernames.Add(user["Username"].ToStringDBNull());
            }

            return usernames.ToStringArray();
        }

        /// <summary>
        /// Grabs all the roles from the DB
        /// </summary>
        /// <returns>
        /// </returns>
        public override string[] GetAllRoles()
        {
            // get all roles...
            var roles = DB.Current.GetRoles(this.ApplicationName, null);

            // make a string collection to store the role list...
            var roleNames = new StringCollection();

            foreach (DataRow row in roles.Rows)
            {
                roleNames.Add(row["RoleName"].ToStringDBNull());
            }

            return roleNames.ToStringArray(); // return as a string array
        }

        /// <summary>
        /// Grabs all the roles from the DB
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// </returns>
        public override string[] GetRolesForUser(string username)
        {
            if (username.IsNotSet())
            {
                ExceptionReporter.ThrowArgument("ROLES", "USERNAMEBLANK");
            }

            StringCollection roleNames = null;

            // get the users's collection from the dictionary
            if (!this.UserRoleCache.ContainsKey(username.ToLower()))
            {
                roleNames = new StringCollection();

                var roles = DB.Current.GetRoles(this.ApplicationName, username);

                foreach (DataRow dr in roles.Rows)
                {
                    roleNames.Add(dr["Rolename"].ToStringDBNull()); // add rolename to collection
                }

                // add it to the dictionary cache...
                this.UserRoleCache.AddOrUpdate(username.ToLower(), (k) => roleNames, (s, v) => roleNames);
            }
            else
            {
                roleNames = this.UserRoleCache[username.ToLower()];
            }

            return roleNames.ToStringArray(); // return as a string array
        }

        /// <summary>
        /// Gets a list of usernames in a a particular role
        /// </summary>
        /// <param name="roleName">
        /// Rolename
        /// </param>
        /// <returns>
        /// List of Usernames
        /// </returns>
        public override string[] GetUsersInRole(string roleName)
        {
            if (roleName.IsNotSet())
            {
                ExceptionReporter.ThrowArgument("ROLES", "ROLENAMEBLANK");
            }

            var users = DB.Current.FindUsersInRole(this.ApplicationName, roleName);

            var userNames = new StringCollection();

            foreach (DataRow dr in users.Rows)
            {
                userNames.Add(dr["Username"].ToStringDBNull());
            }

            return userNames.ToStringArray();
        }

        /// <summary>
        /// Sets up the profile providers
        /// </summary>
        /// <param name="name">
        /// </param>
        /// <param name="config">
        /// </param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // verify that the configuration section was properly passed
            if (config == null)
            {
                ExceptionReporter.ThrowArgument("ROLES", "CONFIGNOTFOUND");
            }

            // Connection String Name
            this._connStrName = config["connectionStringName"].ToStringDBNull();

            ConnStringHelpers.TrySetProviderConnectionString(this._connStrName, ConnStrAppKeyName);

            base.Initialize(name, config);

            // application name
            this._appName = config["applicationName"];

            if (this._appName.IsNotSet())
            {
                this._appName = "YetAnotherForum";
            }
        }

        /// <summary>
        /// Check to see if user belongs to a role
        /// </summary>
        /// <param name="username">
        /// Username
        /// </param>
        /// <param name="roleName">
        /// Rolename
        /// </param>
        /// <returns>
        /// True/False
        /// </returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            var roles = DB.Current.IsUserInRole(this.ApplicationName, username, roleName);

            return roles.HasRows();
        }

        /// <summary>
        /// Remove Users From Roles
        /// </summary>
        /// <param name="usernames">
        /// Usernames
        /// </param>
        /// <param name="roleNames">
        /// Rolenames
        /// </param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            // Loop through username
            foreach (var username in usernames)
            {
                // Loop through roles
                foreach (var roleName in roleNames)
                {
                    DB.Current.RemoveUserFromRole(this.ApplicationName, username, roleName); // Remove role

                    // invalidate cache for this user...
                    this.DeleteFromRoleCacheIfExists(username.ToLower());
                }
            }
        }

        /// <summary>
        /// Check to see if a role exists
        /// </summary>
        /// <param name="roleName">
        /// Rolename
        /// </param>
        /// <returns>
        /// True/False
        /// </returns>
        public override bool RoleExists(string roleName)
        {
            // get this role...
            var exists = DB.Current.GetRoleExists(this.ApplicationName, roleName);

            // if there are any rows then this role exists...
            if (exists.ToType<int>() > 0)
            {
                return true;
            }

            // doesn't exist
            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The clear user role cache.
        /// </summary>
        private void ClearUserRoleCache()
        {
            var key = this.GenerateCacheKey("UserRoleDictionary");
            BoardContext.Current.Get<IObjectStore>().Remove(key);
        }

        /// <summary>
        /// The delete from role cache if exists.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        private void DeleteFromRoleCacheIfExists(string key)
        {
            this.UserRoleCache.TryRemove(key, out var collection);
        }

        /// <summary>
        /// The generate cache key.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The generate cache key.
        /// </returns>
        private string GenerateCacheKey(string name)
        {
            return $"YafRoleProvider-{name}-{this.ApplicationName}";
        }

        #endregion
    }
}