/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Security;

namespace YAFProviders.Passthru
{
  /// <summary>
  /// The yaf roles pass thru.
  /// </summary>
  internal class YAFRolesPassThru : RoleProvider
  {
    /// <summary>
    /// The _real provider.
    /// </summary>
    private RoleProvider _realProvider;

    /// <summary>
    /// Gets or sets ApplicationName.
    /// </summary>
    public override string ApplicationName
    {
      get
      {
        return this._realProvider.ApplicationName;
      }

      set
      {
        this._realProvider.ApplicationName = value;
      }
    }

    /// <summary>
    /// The initialize.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="config">
    /// The config.
    /// </param>
    /// <exception cref="ProviderException">
    /// </exception>
    public override void Initialize(string name, NameValueCollection config)
    {
      string realProviderName = config["passThru"];


      if (realProviderName == null || realProviderName.Length < 1)
      {
        throw new ProviderException("Pass Thru provider name has not been specified in the web.config");
      }

      // Remove passThru configuration attribute
      config.Remove("passThru");

      // Check for further attributes
      if (config.Count > 0)
      {
        // Throw Provider error as no more attributes were expected
        throw new ProviderException("Unrecognised Attribute on the Roles PassThru Provider");
      }

      // Initialise the "Real" roles provider
      this._realProvider = Roles.Providers[realProviderName];
    }

    /// <summary>
    /// The add users to roles.
    /// </summary>
    /// <param name="usernames">
    /// The usernames.
    /// </param>
    /// <param name="roleNames">
    /// The role names.
    /// </param>
    public override void AddUsersToRoles(string[] usernames, string[] roleNames)
    {
      this._realProvider.AddUsersToRoles(usernames, roleNames);
    }

    /// <summary>
    /// The create role.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    public override void CreateRole(string roleName)
    {
      this._realProvider.CreateRole(roleName);
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
    /// <returns>
    /// The delete role.
    /// </returns>
    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
      return this._realProvider.DeleteRole(roleName, throwOnPopulatedRole);
    }

    /// <summary>
    /// The find users in role.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <param name="usernameToMatch">
    /// The username to match.
    /// </param>
    /// <returns>
    /// </returns>
    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      return this._realProvider.FindUsersInRole(roleName, usernameToMatch);
    }

    /// <summary>
    /// The get all roles.
    /// </summary>
    /// <returns>
    /// </returns>
    public override string[] GetAllRoles()
    {
      return this._realProvider.GetAllRoles();
    }

    /// <summary>
    /// The get roles for user.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <returns>
    /// </returns>
    public override string[] GetRolesForUser(string username)
    {
      return this._realProvider.GetRolesForUser(username);
    }

    /// <summary>
    /// The get users in role.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <returns>
    /// </returns>
    public override string[] GetUsersInRole(string roleName)
    {
      return GetUsersInRole(roleName);
    }

    /// <summary>
    /// The is user in role.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <returns>
    /// The is user in role.
    /// </returns>
    public override bool IsUserInRole(string username, string roleName)
    {
      return IsUserInRole(username, roleName);
    }

    /// <summary>
    /// The remove users from roles.
    /// </summary>
    /// <param name="usernames">
    /// The usernames.
    /// </param>
    /// <param name="roleNames">
    /// The role names.
    /// </param>
    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
    {
      this._realProvider.RemoveUsersFromRoles(usernames, roleNames);
    }

    /// <summary>
    /// The role exists.
    /// </summary>
    /// <param name="roleName">
    /// The role name.
    /// </param>
    /// <returns>
    /// The role exists.
    /// </returns>
    public override bool RoleExists(string roleName)
    {
      return this._realProvider.RoleExists(roleName);
    }
  }
}