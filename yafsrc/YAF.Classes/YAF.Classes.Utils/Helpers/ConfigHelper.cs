/* Yet Another Forum.net
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
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace YAF.Classes.Utils
{
	public class ConfigHelper
	{
		private AspNetHostingPermissionLevel? _trustLevel = null;
		public AspNetHostingPermissionLevel TrustLevel
		{
			get
			{
				if ( !_trustLevel.HasValue )
				{
					_trustLevel = General.GetCurrentTrustLevel();
				}

				return _trustLevel.Value;
			}
		}

		public T GetConfigSection<T>( string sectionName ) where T : class
		{
			T section = WebConfigurationManager.GetWebApplicationSection( sectionName ) as T;
			return section;
		}

		public NameValueCollection AppSettings
		{
			get
			{
				return WebConfigurationManager.AppSettings;
			}
		}

		public string GetConfigValueAsString( string keyName )
		{
			if ( this.TrustLevel == AspNetHostingPermissionLevel.High )
			{
				foreach ( string key in AppSettingsFull.Settings.AllKeys )
				{
					if ( key.Equals( keyName, StringComparison.CurrentCultureIgnoreCase ) )
					{
						return AppSettingsFull.Settings[key].Value;
					}
				}
			}
			else
			{
				foreach ( string key in AppSettings.AllKeys )
				{
					if ( key.Equals( keyName, StringComparison.CurrentCultureIgnoreCase ) )
					{
						return AppSettings[key];
					}
				}
			}

			return null;
		}

		private Configuration _webConfig = null;
		public Configuration WebConfigFull
		{
			get
			{
				if ( _webConfig == null )
					_webConfig = WebConfigurationManager.OpenWebConfiguration( "~/" );

				return _webConfig;
			}
		}

		private AppSettingsSection _appSettingsFull = null;
		public AppSettingsSection AppSettingsFull
		{
			get
			{
				if ( _appSettingsFull == null )
				{
					_appSettingsFull = GetConfigSectionFull<AppSettingsSection>( "appSettings" );
				}

				return _appSettingsFull;
			}
		}

		[AspNetHostingPermission( SecurityAction.Demand, Level=AspNetHostingPermissionLevel.High )]
		public T GetConfigSectionFull<T>( string sectionName ) where T : class
		{
			ConfigurationSection section = WebConfigFull.GetSection( sectionName );
			if ( section is T )
			{
				return section as T;
			}

			return null;
		}

		[AspNetHostingPermission( SecurityAction.Demand, Level=AspNetHostingPermissionLevel.High )]
		public bool WriteConnectionString( string keyName, string keyValue, string providerValue )
		{
			ConnectionStringsSection connStrings = GetConfigSectionFull<ConnectionStringsSection>( "connectionStrings" );

			if ( connStrings == null )
			{
				return false;
			}

			bool writtenSuccessfully = false;
			try
			{
				if ( connStrings.ConnectionStrings[keyName] != null )
				{
					connStrings.ConnectionStrings.Remove( keyName );
				}

				connStrings.ConnectionStrings.Add( new ConnectionStringSettings( keyName, keyValue, providerValue ) );

				WebConfigFull.Save( ConfigurationSaveMode.Modified );

				writtenSuccessfully = true;
			}
			catch
			{
				writtenSuccessfully = false;
			}

			return writtenSuccessfully;
		}

		[AspNetHostingPermission( SecurityAction.Demand, Level=AspNetHostingPermissionLevel.High )]
		public bool WriteAppSetting( string keyName, string keyValue )
		{
			bool writtenSuccessfully = false;

			try
			{
				if ( AppSettingsFull.Settings[keyName] != null )
				{
					AppSettingsFull.Settings.Remove( keyName );
				}

				AppSettingsFull.Settings.Add( keyName, keyValue );

				WebConfigFull.Save( ConfigurationSaveMode.Modified );

				writtenSuccessfully = true;
			}
			catch
			{
				writtenSuccessfully = false;
			}

			return writtenSuccessfully;
		}
	}
}
