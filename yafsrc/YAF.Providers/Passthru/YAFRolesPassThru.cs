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
using System.Text;
using System.Web;
using System.Configuration.Provider;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;

namespace YAFProviders.Passthru
{
	class YAFRolesPassThru : System.Web.Security.RoleProvider
	{

		System.Web.Security.RoleProvider _realProvider;

		public override void Initialize( string name, NameValueCollection config )
		{
			string realProviderName = config ["passThru"];


			if ( realProviderName == null || realProviderName.Length < 1 )
				throw new ProviderException( "Pass Thru provider name has not been specified in the web.config" );

			// Remove passThru configuration attribute
			config.Remove( "passThru" );

			// Check for further attributes
			if ( config.Count > 0 )
			{
				// Throw Provider error as no more attributes were expected
				throw new ProviderException( "Unrecognised Attribute on the Roles PassThru Provider" );
			}

			// Initialise the "Real" roles provider
			_realProvider = System.Web.Security.Roles.Providers [realProviderName];
		}

		public override void AddUsersToRoles( string [] usernames, string [] roleNames )
		{
			_realProvider.AddUsersToRoles( usernames, roleNames );
		}

		public override string ApplicationName
		{
			get
			{
				return _realProvider.ApplicationName;
			}
			set
			{
				_realProvider.ApplicationName = value;
			}
		}

		public override void CreateRole( string roleName )
		{
			_realProvider.CreateRole( roleName );

		}

		public override bool DeleteRole( string roleName, bool throwOnPopulatedRole )
		{
			return _realProvider.DeleteRole( roleName, throwOnPopulatedRole );
		}

		public override string [] FindUsersInRole( string roleName, string usernameToMatch )
		{
			return _realProvider.FindUsersInRole( roleName, usernameToMatch );
		}

		public override string [] GetAllRoles()
		{
			return _realProvider.GetAllRoles();
		}

		public override string [] GetRolesForUser( string username )
		{
			return _realProvider.GetRolesForUser( username );
		}

		public override string [] GetUsersInRole( string roleName )
		{
			return GetUsersInRole( roleName );
		}

		public override bool IsUserInRole( string username, string roleName )
		{
			return IsUserInRole( username, roleName );
		}

		public override void RemoveUsersFromRoles( string [] usernames, string [] roleNames )
		{
			_realProvider.RemoveUsersFromRoles( usernames, roleNames );
		}

		public override bool RoleExists( string roleName )
		{
			return _realProvider.RoleExists( roleName );
		}
	}
}
