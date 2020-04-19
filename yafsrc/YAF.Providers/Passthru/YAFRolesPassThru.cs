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

namespace YAFProviders.Passthru
{
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Web.Security;

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
      get => this._realProvider.ApplicationName;

      set => this._realProvider.ApplicationName = value;
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
      var realProviderName = config["passThru"];


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
      return this.GetUsersInRole(roleName);
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
      return this.IsUserInRole(username, roleName);
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