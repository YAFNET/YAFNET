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
	public class YafRoleProvider : RoleProvider
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
		public override void Initialize( string name, NameValueCollection config )
		{
			// verify that the configuration section was properly passed
			if ( config == null )
				throw new ArgumentNullException( "config" );

			base.Initialize( name, config );

			// application name
			_appName = config ["applicationName"];
			if ( string.IsNullOrEmpty( _appName ) )
				_appName = "YetAnotherForum";
		}

		public override void AddUsersToRoles( string [] usernames, string [] roleNames )
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		public override void CreateRole( string roleName )
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		/// <summary>
		/// Deletes a role
		/// </summary>
		/// <param name="roleName"></param>
		/// <param name="throwOnPopulatedRole"></param>
		/// <returns></returns>
		public override bool DeleteRole( string roleName, bool throwOnPopulatedRole )
		{
			int returnValue = YAFProviders.Roles.DB.role_delete( this.ApplicationName, roleName, throwOnPopulatedRole );

			// zero means there were no complications...
			if ( returnValue == 0 ) return true;
			
			// it failed for some reason...
			return false;
		}

		public override string [] FindUsersInRole( string roleName, string usernameToMatch )
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		/// <summary>
		/// Grabs all the roles from the DB
		/// </summary>
		/// <returns></returns>
		public override string [] GetAllRoles()
		{
			// get all roles...
			DataTable roles = YAFProviders.Roles.DB.role_list( this.ApplicationName, null );

			// make a string collection to store the role list...
			StringCollection sc = new StringCollection();

			foreach ( DataRow row in roles.Rows )
			{
				sc.Add( row ["Name"].ToString() );
			}

			// return as a string array
			String [] strReturn = new String [sc.Count];
			sc.CopyTo( strReturn, 0 );

			return strReturn;			
		}

		public override string [] GetRolesForUser( string username )
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		public override string [] GetUsersInRole( string roleName )
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		public override bool IsUserInRole( string username, string roleName )
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		public override void RemoveUsersFromRoles( string [] usernames, string [] roleNames )
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		public override bool RoleExists( string roleName )
		{
			// get this role...
			DataTable roles = YAFProviders.Roles.DB.role_list( this.ApplicationName, roleName );

			// if there are any rows then this role exists...
			if ( roles.Rows.Count > 0 )
			{
				return true;
			}

			// doesn't exist
			return false;
		}

		#endregion
	}
}
