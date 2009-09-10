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
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace YAF.Classes.Utils
{
	public class ConfigHelper
	{
		private Configuration _webConfig = null;
		public Configuration WebConfig
		{
			get
			{
				if ( _webConfig == null )
					_webConfig = WebConfigurationManager.OpenWebConfiguration( "~/" );

				return _webConfig;
			}
		}

		private AppSettingsSection _appSettings = null;
		public AppSettingsSection AppSettings
		{
			get
			{
				if ( _appSettings == null )
				{
					_appSettings = GetConfigSection<AppSettingsSection>( "appSettings" );
				}

				return _appSettings;
			}
		}

		public T GetConfigSection<T>( string sectionName ) where T : class
		{
			ConfigurationSection section = WebConfig.GetSection( sectionName );
			if ( section is T )
			{
				return section as T;
			}

			return null;
		}

		public string GetConfigValueAsString( string keyName )
		{
			foreach ( string key in AppSettings.Settings.AllKeys )
			{
				if ( key.Equals( keyName, StringComparison.CurrentCultureIgnoreCase ) )
				{
					return AppSettings.Settings[key].Value;
				}
			}

			return null;
		}

		public bool WriteConnectionString( string keyName, string keyValue, string providerValue )
		{
			ConnectionStringsSection connStrings = GetConfigSection<ConnectionStringsSection>( "connectionStrings" );

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

				WebConfig.Save( ConfigurationSaveMode.Modified );

				writtenSuccessfully = true;
			}
			catch
			{
				writtenSuccessfully = false;
			}

			return writtenSuccessfully;
		}

		public bool WriteAppSetting( string keyName, string keyValue )
		{
			bool writtenSuccessfully = false;

			try
			{
				if ( AppSettings.Settings[keyName] != null )
				{
					AppSettings.Settings.Remove( keyName );
				}

				AppSettings.Settings.Add( keyName, keyValue );

				WebConfig.Save( ConfigurationSaveMode.Modified );

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
