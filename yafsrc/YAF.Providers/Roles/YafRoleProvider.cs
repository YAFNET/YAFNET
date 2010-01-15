// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="YafRoleProvider.cs">
//   
// </copyright>
// <summary>
//   The yaf role provider.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Providers.Roles
{
  using System;
  using System.Collections.Specialized;
  using System.Configuration;
  using System.Data;
  using System.Web.Security;

  using YAF.Classes.Core;
  using YAF.Classes.Pattern;
  using YAF.Providers.Utils;

  /// <summary>
  /// The yaf role provider.
  /// </summary>
  public class YafRoleProvider : RoleProvider
  {
    #region Constants and Fields

    /// <summary>
    /// The conn str app key name.
    /// </summary>
    private static string _connStrAppKeyName = "YafRolesConnectionString";

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

    #endregion

    #region Properties

    /// <summary>
    /// Gets the Connection String App Key Name.
    /// </summary>
    public static string ConnStrAppKeyName
    {
      get
      {
        return _connStrAppKeyName;
      }
    }

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
          this.ClearUserRoleCache();
        }
      }
    }

    /// <summary>
    /// Gets UserRoleCache.
    /// </summary>
    private ThreadSafeDictionary<string, StringCollection> UserRoleCache
    {
      get
      {
        string key = this.GenerateCacheKey("UserRoleDictionary");
        return YafContext.Current.Cache.GetItem(key, 999, () => new ThreadSafeDictionary<string, StringCollection>());
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
      // Loop through username
      foreach (string username in usernames)
      {
        // Loop through roles
        foreach (string roleName in roleNames)
        {
          DB.Current.AddUserToRole(this.ApplicationName, username, roleName);
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
      if (String.IsNullOrEmpty(roleName))
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
      int returnValue = DB.Current.DeleteRole(this.ApplicationName, roleName, throwOnPopulatedRole);

      this.ClearUserRoleCache();

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
      DataTable users = DB.Current.FindUsersInRole(this.ApplicationName, roleName);
      var usernames = new StringCollection();
      foreach (DataRow user in users.Rows)
      {
        usernames.Add(user["Username"].ToStringDBNull());
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
      DataTable roles = DB.Current.GetRoles(this.ApplicationName, null);

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
      if (String.IsNullOrEmpty(username))
      {
        ExceptionReporter.ThrowArgument("ROLES", "USERNAMEBLANK");
      }

      StringCollection roleNames = null;

      // get the users's collection from the dictionary
      if (!this.UserRoleCache.ContainsKey(username.ToLower()))
      {
        roleNames = new StringCollection();

        DataTable roles = DB.Current.GetRoles(this.ApplicationName, username);

        foreach (DataRow dr in roles.Rows)
        {
          roleNames.Add(dr["Rolename"].ToStringDBNull()); // add rolename to collection
        }

        // add it to the dictionary cache...
        this.UserRoleCache.MergeSafe(username.ToLower(), roleNames);
      }
      else
      {
        roleNames = this.UserRoleCache[username.ToLower()];
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

      DataTable users = DB.Current.FindUsersInRole(this.ApplicationName, roleName);

      var userNames = new StringCollection();

      foreach (DataRow dr in users.Rows)
      {
        userNames.Add(dr["Username"].ToStringDBNull());
      }

      return Transform.ToStringArray(userNames);
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
      DataTable roles = DB.Current.IsUserInRole(this.ApplicationName, username, roleName);

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
      object exists = DB.Current.GetRoleExists(this.ApplicationName, roleName);

      // if there are any rows then this role exists...
      if (Convert.ToInt32(exists) > 0)
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
      string key = this.GenerateCacheKey("UserRoleDictionary");
      YafContext.Current.Cache.Remove(key);
    }

    /// <summary>
    /// The delete from role cache if exists.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    private void DeleteFromRoleCacheIfExists(string key)
    {
      this.UserRoleCache.RemoveSafe(key);
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
      return String.Format("YafRoleProvider-{0}-{1}", name, this.ApplicationName);
    }

    #endregion
  }
}