/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web.Security;
using YAF.Classes.Core;
using YAF.Providers.Utils;

namespace YAF.Providers.Roles
{
  /// <summary>
  /// The yaf role provider.
  /// </summary>
  public class YafRoleProvider : RoleProvider
  {
    /// <summary>
    /// The conn str app key name.
    /// </summary>
    public static string ConnStrAppKeyName = "YafRolesConnectionString";

    /// <summary>
    /// The _app name.
    /// </summary>
    private string _appName;

    /// <summary>
    /// The _conn str name.
    /// </summary>
    private string _connStrName;

    /// <summary>
    /// The _provider localization.
    /// </summary>
    private YafLocalization _providerLocalization;

    #region Override Public Properties

    /// <summary>
    /// Gets or sets ApplicationName.
    /// </summary>
    public override string ApplicationName
    {
      get
      {
        return this._appName;
      }

      set
      {
        if (value != this._appName)
        {
          this._appName = value;

          // clear the cache for obvious reasons...
          ClearUserRoleCache();
        }
      }
    }

    #endregion

    /// <summary>
    /// Gets UserRoleCache.
    /// </summary>
    private Dictionary<string, StringCollection> UserRoleCache
    {
      get
      {
        string key = GenerateCacheKey("UserRoleDictionary");
        return YafContext.Current.Cache.GetItem(key, 999, () => new Dictionary<string, StringCollection>());
      }
    }

    /// <summary>
    /// The delete from role cache if exists.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    private void DeleteFromRoleCacheIfExists(string key)
    {
      if (UserRoleCache.ContainsKey(key))
      {
        UserRoleCache.Remove(key);
      }
    }

    /// <summary>
    /// The clear user role cache.
    /// </summary>
    private void ClearUserRoleCache()
    {
      string key = GenerateCacheKey("UserRoleDictionary");
      YafContext.Current.Cache.Remove(key);
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
      return String.Format("YafRoleProvider-{0}-{1}", name, ApplicationName);
    }

    #region Overriden Public Methods

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
      this._connStrName = Transform.ToString(config["connectionStringName"] ?? String.Empty);

      // is the connection string set?
      if (!String.IsNullOrEmpty(this._connStrName))
      {
        string connStr = ConfigurationManager.ConnectionStrings[this._connStrName].ConnectionString;

        // set the app variable...
        if (YafContext.Application[ConnStrAppKeyName] == null)
        {
          YafContext.Application.Add(ConnStrAppKeyName, connStr);
        }
        else
        {
          YafContext.Application[ConnStrAppKeyName] = connStr;
        }
      }

      base.Initialize(name, config);

      // application name
      this._appName = config["applicationName"];
      if (string.IsNullOrEmpty(this._appName))
      {
        this._appName = "YetAnotherForum";
      }

      this._providerLocalization = new YafLocalization();
    }

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
      // Loop through username
      foreach (string username in usernames)
      {
        // Loop through roles
        foreach (string roleName in roleNames)
        {
          DB.Current.AddUserToRole(ApplicationName, username, roleName);
        }

        // invalidate the cache for this user...
        DeleteFromRoleCacheIfExists(username.ToLower());
      }
    }

    /// <summary>
    /// Creates a role
    /// </summary>
    /// <param name="roleName">
    /// </param>
    public override void CreateRole(string roleName)
    {
      if (String.IsNullOrEmpty(roleName))
      {
        ExceptionReporter.ThrowArgument("ROLES", "ROLENAMEBLANK");
      }

      DB.Current.CreateRole(ApplicationName, roleName);
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
      int returnValue = DB.Current.DeleteRole(ApplicationName, roleName, throwOnPopulatedRole);

      ClearUserRoleCache();

      // zero means there were no complications...
      if (returnValue == 0)
      {
        return true;
      }

      // it failed for some reason...
      return false;
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
      if (String.IsNullOrEmpty(roleName))
      {
        ExceptionReporter.ThrowArgument("ROLES", "ROLENAMEBLANK");
      }

      // Roles
      DataTable users = DB.Current.FindUsersInRole(ApplicationName, roleName);
      var usernames = new StringCollection();
      foreach (DataRow user in users.Rows)
      {
        usernames.Add(Transform.ToString(user["Username"]));
      }

      return Transform.ToStringArray(usernames);
    }

    /// <summary>
    /// Grabs all the roles from the DB
    /// </summary>
    /// <returns>
    /// </returns>
    public override string[] GetAllRoles()
    {
      // get all roles...
      DataTable roles = DB.Current.GetRoles(ApplicationName, null);

      // make a string collection to store the role list...
      var roleNames = new StringCollection();

      foreach (DataRow row in roles.Rows)
      {
        roleNames.Add(Transform.ToString(row["RoleName"]));
      }

      return Transform.ToStringArray(roleNames); // return as a string array
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
      if (String.IsNullOrEmpty(username))
      {
        ExceptionReporter.ThrowArgument("ROLES", "USERNAMEBLANK");
      }

      StringCollection roleNames = null;

      // get the users's collection from the dictionary
      if (!UserRoleCache.ContainsKey(username.ToLower()))
      {
        roleNames = new StringCollection();
        DataTable roles = DB.Current.GetRoles(ApplicationName, username);
        foreach (DataRow dr in roles.Rows)
        {
          roleNames.Add(Transform.ToString(dr["Rolename"])); // add rolename to collection
        }

        // add it to the dictionary cache...
        UserRoleCache[username.ToLower()] = roleNames;
      }
      else
      {
        roleNames = UserRoleCache[username.ToLower()];
      }

      return Transform.ToStringArray(roleNames); // return as a string array
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
      if (String.IsNullOrEmpty(roleName))
      {
        ExceptionReporter.ThrowArgument("ROLES", "ROLENAMEBLANK");
      }

      DataTable users = DB.Current.FindUsersInRole(ApplicationName, roleName);
      var userNames = new StringCollection();
      
      foreach (DataRow dr in users.Rows)
      {
        userNames.Add(Transform.ToString(dr["Username"]));
      }

      return Transform.ToStringArray(userNames);
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
      DataTable roles = DB.Current.IsUserInRole(ApplicationName, username, roleName);

      if (roles.Rows.Count > 0)
      {
        return true;
      }
      else
      {
        return false;
      }
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
      foreach (string username in usernames)
      {
        // Loop through roles
        foreach (string roleName in roleNames)
        {
          DB.Current.RemoveUserFromRole(ApplicationName, username, roleName); // Remove role

          // invalidate cache for this user...
          DeleteFromRoleCacheIfExists(username.ToLower());
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
      object exists = DB.Current.GetRoleExists(ApplicationName, roleName);

      // if there are any rows then this role exists...
      if (Convert.ToInt32(exists) > 0)
      {
        return true;
      }

      // doesn't exist
      return false;
    }

    #endregion
  }
}