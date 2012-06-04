/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Providers.Roles
{
  #region Using

	using System;
	using System.Data;
	using System.Data.SqlClient;

	using YAF.Classes;
	using YAF.Classes.Data;
	using YAF.Classes.Pattern;
	using YAF.Core;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	#endregion

  /// <summary>
  /// The yaf roles db conn manager.
  /// </summary>
  public class MsSqlRolesDbConnectionProvider : MsSqlDbConnectionProvider
  {
    #region Properties

    /// <summary>
    ///   Gets ConnectionString.
    /// </summary>
    public override string ConnectionString
    {
      get
      {
        if (YafContext.Application[YafRoleProvider.ConnStrAppKeyName] != null)
        {
          return YafContext.Application[YafRoleProvider.ConnStrAppKeyName] as string;
        }

        return Config.ConnectionString;
      }
    }

    #endregion
  }

  /// <summary>
  /// The db.
  /// </summary>
  public class DB
  {
    #region Constants and Fields

    /// <summary>
    ///   The _db access.
    /// </summary>
		private readonly IDbAccess _dbAccess = new MsSqlDbAccess(new MsSqlRolesDbConnectionProvider());

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "DB" /> class.
    /// </summary>
    public DB()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Current.
    /// </summary>
    public static DB Current
    {
      get
      {
        return PageSingleton<DB>.Instance;
      }
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
      using (var cmd = this._dbAccess.GetCommand("prov_role_addusertorole"))
      {
        cmd.AddParam("ApplicationName", appName);
        cmd.AddParam("Username", userName);
        cmd.AddParam("RoleName", roleName);

        this._dbAccess.ExecuteNonQuery(cmd);
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
      using (var cmd = this._dbAccess.GetCommand("prov_role_createrole"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.AddParam("ApplicationName", appName);
        cmd.AddParam("RoleName", roleName);
        this._dbAccess.ExecuteNonQuery(cmd);
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
      using (var cmd = this._dbAccess.GetCommand("prov_role_deleterole"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.AddParam("ApplicationName", appName);
        cmd.AddParam("RoleName", roleName);
        cmd.AddParam("DeleteOnlyIfRoleIsEmpty", deleteOnlyIfRoleIsEmpty);

				cmd.CreateOutputParameter("ReturnValue", DbType.Int32, direction: ParameterDirection.ReturnValue);

        this._dbAccess.ExecuteNonQuery(cmd);

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
      using (var cmd = this._dbAccess.GetCommand("prov_role_findusersinrole"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.AddParam("ApplicationName", appName);
        cmd.AddParam("RoleName", roleName);
        return this._dbAccess.GetData(cmd);
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
      using (var cmd = this._dbAccess.GetCommand("prov_role_exists"))
      {
        cmd.AddParam("ApplicationName", appName);
        cmd.AddParam("RoleName", roleName);
        return this._dbAccess.ExecuteScalar(cmd);
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
      using (var cmd = this._dbAccess.GetCommand("prov_role_getroles"))
      {
        cmd.AddParam("ApplicationName", appName);
        cmd.AddParam("Username", username);
        return this._dbAccess.GetData(cmd);
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
      using (var cmd = this._dbAccess.GetCommand("prov_role_isuserinrole"))
      {
        cmd.AddParam("ApplicationName", appName);
        cmd.AddParam("Username", userName);
        cmd.AddParam("RoleName", roleName);
        return this._dbAccess.GetData(cmd);
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
      using (var cmd = this._dbAccess.GetCommand("prov_role_removeuserfromrole"))
      {
        cmd.AddParam("ApplicationName", appName);
        cmd.AddParam("Username", userName);
        cmd.AddParam("RoleName", roleName);
        this._dbAccess.ExecuteNonQuery(cmd);
      }
    }

    #endregion
  }
}