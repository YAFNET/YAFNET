/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using YAF.Classes.Utils;
using YAF.Providers.Utils;

namespace YAF.Providers.Roles
{
	public class YafRoleProvider : RoleProvider
	{
		private string _appName;
		private YafLocalization _providerLocalization;

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

		Dictionary<string, StringCollection> UserRoleCache
		{
			get
			{
				string key = GenerateCacheKey( "UserRoleDictionary" );

				// get the roles collection...
				Dictionary<string, StringCollection> userRoleDic = System.Web.HttpContext.Current.Cache [key] as Dictionary<string, StringCollection>;

				if ( userRoleDic == null )
				{
					// make sure it exists in the cache...
					userRoleDic = new Dictionary<string, StringCollection>();
					System.Web.HttpContext.Current.Cache [key] = userRoleDic;
				}

				return userRoleDic;
			}
		}

		private void DeleteFromRoleCacheIfExists(string key)
		{
			if ( UserRoleCache.ContainsKey( key ) )
			{
				UserRoleCache.Remove(key);
			}
		}

		private void ClearUserRoleCache()
		{
			string key = GenerateCacheKey( "UserRoleDictionary" );
			HttpContext.Current.Cache.Remove( key );
		}

		private string GenerateCacheKey( string name )
		{
			return String.Format( "YafRoleProvider-{0}-{1}", name, this.ApplicationName );
		}

		#region Overriden Public Methods

		/// <summary>
		/// Sets up the profile providers
		/// </summary>
		/// <param name="name"></param>
		/// <param name="config"></param>
		public override void Initialize( string name, NameValueCollection config )
		{
			// verify that the configuration section was properly passed
			if ( config == null )
				ExceptionReporter.ThrowArgument( "ROLES", "CONFIGNOTFOUND" );

			base.Initialize( name, config );

			// application name
			_appName = config ["applicationName"];
			if ( string.IsNullOrEmpty( _appName ) )
				_appName = "YetAnotherForum";

			_providerLocalization = new YafLocalization();
		}

		/// <summary>
		/// Adds a list of users to a list of groups
		/// </summary>
		/// <param name="usernames">List of Usernames</param>
		/// <param name="roleNames">List of Rolenames</param>
		/// <returns></returns>
		public override void AddUsersToRoles( string [] usernames, string [] roleNames )
		{
			// Loop through username
			foreach ( string username in usernames )
			{
				// Loop through roles
				foreach ( string roleName in roleNames )
				{
					DB.AddUserToRole( this.ApplicationName, username, roleName );
				}
				// invalidate the cache for this user...
				DeleteFromRoleCacheIfExists( username.ToLower() );
			}
		}

		/// <summary>
		/// Creates a role
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public override void CreateRole( string roleName )
		{
			if ( String.IsNullOrEmpty( roleName ) )
				ExceptionReporter.ThrowArgument( "ROLES", "ROLENAMEBLANK" );

			DB.CreateRole( this.ApplicationName, roleName );
		}

		/// <summary>
		/// Deletes a role
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="throwOnPopulatedRole"></param>
		/// <returns>True or False</returns>
		public override bool DeleteRole( string roleName, bool throwOnPopulatedRole )
		{
			int returnValue = DB.DeleteRole( this.ApplicationName, roleName, throwOnPopulatedRole );

			ClearUserRoleCache();

			// zero means there were no complications...
			if ( returnValue == 0 ) return true;

			// it failed for some reason...
			return false;
		}

		/// <summary>
		/// Adds a list of users to a list of groups
		/// </summary>
		/// <param name="roleName">Rolename</param>
		/// <param name="usernameToMatch">like Username used in search</param>
		/// <returns>List of Usernames</returns>
		public override string [] FindUsersInRole( string roleName, string usernameToMatch )
		{
			if ( String.IsNullOrEmpty( roleName ) )
				ExceptionReporter.ThrowArgument( "ROLES", "ROLENAMEBLANK" );
			// Roles
			DataTable users = DB.FindUsersInRole( this.ApplicationName, roleName );
			StringCollection usernames = new StringCollection(); ;
			foreach ( DataRow user in users.Rows )
			{
				usernames.Add( Utils.Transform.ToString( user ["Username"] ) );
			}
			return Utils.Transform.ToStringArray( usernames );
		}

		/// <summary>
		/// Grabs all the roles from the DB
		/// </summary>
		/// <returns></returns>
		public override string [] GetAllRoles()
		{
			// get all roles...
			DataTable roles = DB.GetRoles( this.ApplicationName, null );

			// make a string collection to store the role list...
			StringCollection roleNames = new StringCollection();

			foreach ( DataRow row in roles.Rows )
			{
				roleNames.Add( Utils.Transform.ToString( row ["RoleName"] ) );
			}

			return Utils.Transform.ToStringArray( roleNames );  // return as a string array
		}

		/// <summary>
		/// Grabs all the roles from the DB
		/// </summary>
		/// <returns></returns>
		public override string [] GetRolesForUser( string username )
		{
			if ( String.IsNullOrEmpty( username ) )
				ExceptionReporter.ThrowArgument( "ROLES", "USERNAMEBLANK" );

			StringCollection roleNames = null;

			// get the users's collection from the dictionary
			if ( !UserRoleCache.ContainsKey( username.ToLower() ) )
			{
				roleNames = new StringCollection();
				DataTable roles = DB.GetRoles( this.ApplicationName, username );
				foreach ( DataRow dr in roles.Rows )
				{
					roleNames.Add( Utils.Transform.ToString( dr ["Rolename"] ) ); // add rolename to collection
				}
				// add it to the dictionary cache...
				UserRoleCache [username.ToLower()] = roleNames;
			}
			else
			{
				roleNames = UserRoleCache[username.ToLower()];
			}

			return Utils.Transform.ToStringArray( roleNames ); // return as a string array
		}

		/// <summary>
		/// Gets a list of usernames in a a particular role
		/// </summary>
		/// <param name="roleName">Rolename</param>
		/// <returns>List of Usernames</returns>
		public override string [] GetUsersInRole( string roleName )
		{
			if ( String.IsNullOrEmpty( roleName ) )
				ExceptionReporter.ThrowArgument( "ROLES", "ROLENAMEBLANK" );

			DataTable users = DB.FindUsersInRole( this.ApplicationName, roleName );
			StringCollection userNames = new StringCollection(); ;
			foreach ( DataRow dr in users.Rows )
			{
				userNames.Add( Utils.Transform.ToString( dr ["Username"] ) );
			}
			return Utils.Transform.ToStringArray( userNames );

		}

		/// <summary>
		/// Check to see if user belongs to a role
		/// </summary>
		/// <param name="usernames">Username</param>
		/// <param name="roleNames">Rolename</param>
		/// <returns>True/False</returns>
		public override bool IsUserInRole( string username, string roleName )
		{
			DataTable roles = DB.IsUserInRole( this.ApplicationName, username, roleName );

			if ( roles.Rows.Count > 0 )
				return true;
			else
				return false;
		}

		/// <summary>
		/// Remove Users From Roles
		/// </summary>
		/// <param name="usernames">Usernames</param>
		/// <param name="roleNames">Rolenames</param>
		public override void RemoveUsersFromRoles( string [] usernames, string [] roleNames )
		{
			// Loop through username
			foreach ( string username in usernames )
			{
				// Loop through roles
				foreach ( string roleName in roleNames )
				{
					DB.RemoveUserFromRole( this.ApplicationName, username, roleName ); // Remove role
					// invalidate cache for this user...
					DeleteFromRoleCacheIfExists( username.ToLower() );
				}
			}
		}

		/// <summary>
		/// Check to see if a role exists
		/// </summary>
		/// <param name="roleName">Rolename</param>
		/// <returns>True/False</returns>
		public override bool RoleExists( string roleName )
		{
			// get this role...
			object exists = DB.GetRoleExists( this.ApplicationName, roleName );

			// if there are any rows then this role exists...
			if ( Convert.ToInt32( exists ) > 0 )
			{
				return true;
			}

			// doesn't exist
			return false;
		}

		#endregion
	}
}
