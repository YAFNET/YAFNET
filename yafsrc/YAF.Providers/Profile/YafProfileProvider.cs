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
using System.Data;
using System.Web;
using System.Web.Profile;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Configuration;
using YAF.Providers.Utils;

namespace YAF.Providers.Profile
{
	/// <summary>
	/// YAF Custom Profile Provider
	/// </summary>
	public class YafProfileProvider : ProfileProvider
	{
		static public string ConnStrAppKeyName = "YafProfileConnectionString";

		private string _appName, _connStrName;
		private bool _propertiesSetup = false;
		private System.Collections.Generic.List<SettingsPropertyColumn> _settingsColumnsList = new System.Collections.Generic.List<SettingsPropertyColumn>();

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

		Dictionary<string, SettingsPropertyValueCollection> UserProfileCache
		{
			get
			{
				string key = GenerateCacheKey( "UserProfileDictionary" );

				// get the roles collection...
				Dictionary<string, SettingsPropertyValueCollection> userProfileDic = System.Web.HttpContext.Current.Cache[key] as Dictionary<string, SettingsPropertyValueCollection>;

				if ( userProfileDic == null )
				{
					// make sure it exists in the cache...
					userProfileDic = new Dictionary<string, SettingsPropertyValueCollection>();
					System.Web.HttpContext.Current.Cache[key] = userProfileDic;
				}

				return userProfileDic;
			}
		}

		private void DeleteFromProfileCacheIfExists( string key )
		{
			if ( UserProfileCache.ContainsKey( key ) )
			{
				UserProfileCache.Remove( key );
			}
		}

		private void ClearUserProfileCache()
		{
			string key = GenerateCacheKey( "UserProfileDictionary" );
			System.Web.HttpContext.Current.Cache[key] = null;
		}

		private string GenerateCacheKey( string name )
		{
			return String.Format( "YafProfileProvider-{0}-{1}", name, this.ApplicationName );
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
				throw new ArgumentNullException( "config" );

			// Connection String Name
			_connStrName = Utils.Transform.ToString( config["connectionStringName"] ?? String.Empty );

			// application name
			_appName = config["applicationName"];
			if ( string.IsNullOrEmpty( _appName ) )
				_appName = "YetAnotherForum";

			// is the connection string set?
			if ( !String.IsNullOrEmpty( _connStrName ) )
			{
				string connStr = ConfigurationManager.ConnectionStrings[_connStrName].ConnectionString;

				// set the app variable...
				if ( HttpContext.Current.Application[ConnStrAppKeyName] == null )
				{
					HttpContext.Current.Application.Add( ConnStrAppKeyName, connStr );
				}
				else
				{
					HttpContext.Current.Application[ConnStrAppKeyName] = connStr;
				}
			}

			base.Initialize( name, config );
		}

		protected void LoadFromPropertyCollection( SettingsPropertyCollection collection )
		{
			if ( !_propertiesSetup )
			{
				// clear it out just in case something is still in there...
				_settingsColumnsList.Clear();

				// validiate all the properties and populate the internal settings collection
				foreach ( SettingsProperty property in collection )
				{
					SqlDbType dbType;
					int size;

					// parse custom provider data...
					GetDbTypeAndSizeFromString( property.Attributes["CustomProviderData"].ToString(), out dbType, out size );

					// default the size to 256 if no size is specified
					if ( dbType == SqlDbType.NVarChar && size == -1 )
					{
						size = 256;
					}
					_settingsColumnsList.Add( new SettingsPropertyColumn( property, dbType, size ) );
				}

				// sync profile table structure with the db...
				DataTable structure = DB.Current.GetProfileStructure();

				// verify all the columns are there...
				foreach ( SettingsPropertyColumn column in _settingsColumnsList )
				{
					// see if this column exists
					if ( !structure.Columns.Contains( column.Settings.Name ) )
					{
						// if not, create it...
						DB.Current.AddProfileColumn( column.Settings.Name, column.DataType, column.Size );
					}
				}

				// it's setup now...
				_propertiesSetup = true;
			}
		}

		protected void LoadFromPropertyValueCollection( SettingsPropertyValueCollection collection )
		{
			if ( !_propertiesSetup )
			{
				// clear it out just in case something is still in there...
				_settingsColumnsList.Clear();

				// validiate all the properties and populate the internal settings collection
				foreach ( SettingsPropertyValue value in collection )
				{
					SqlDbType dbType;
					int size;

					// parse custom provider data...
					GetDbTypeAndSizeFromString( value.Property.Attributes["CustomProviderData"].ToString(), out dbType, out size );

					// default the size to 256 if no size is specified
					if ( dbType == SqlDbType.NVarChar && size == -1 )
					{
						size = 256;
					}
					_settingsColumnsList.Add( new SettingsPropertyColumn( value.Property, dbType, size ) );
				}

				// sync profile table structure with the db...
				DataTable structure = DB.Current.GetProfileStructure();

				// verify all the columns are there...
				foreach ( SettingsPropertyColumn column in _settingsColumnsList )
				{
					// see if this column exists
					if ( !structure.Columns.Contains( column.Settings.Name ) )
					{
						// if not, create it...
						DB.Current.AddProfileColumn( column.Settings.Name, column.DataType, column.Size );
					}
				}

				// it's setup now...
				_propertiesSetup = true;
			}
		}

		private bool GetDbTypeAndSizeFromString( string providerData, out SqlDbType dbType, out int size )
		{
			size = -1;
			dbType = SqlDbType.NVarChar;

			if ( String.IsNullOrEmpty( providerData ) )
			{
				return false;
			}

			// split the data
			string[] chunk = providerData.Split( new char[] { ';' } );

			// first item is the column name...
			string columnName = chunk[0];

			// get the datatype and ignore case...
			dbType = (SqlDbType)Enum.Parse( typeof( SqlDbType ), chunk[1], true );

			if ( chunk.Length > 2 )
			{
				// handle size...
				if ( !Int32.TryParse( chunk[2], out size ) )
				{
					throw new ArgumentException( "Unable to parse as integer: " + chunk[2] );
				}
			}

			return true;
		}

		public override int DeleteInactiveProfiles( ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate )
		{
			if ( authenticationOption == ProfileAuthenticationOption.Anonymous )
			{
				ExceptionReporter.ThrowArgument( "PROFILE", "NOANONYMOUS" );
			}

			// just clear the whole thing...
			ClearUserProfileCache();

			return DB.Current.DeleteInactiveProfiles( this.ApplicationName, userInactiveSinceDate );
		}

		public override int DeleteProfiles( string[] usernames )
		{
			if ( usernames == null || usernames.Length < 1 )
			{
				return 0;
			}

			// make single string of usernames...
			StringBuilder userNameBuilder = new StringBuilder();
			bool bFirst = true;

			for ( int i = 0; i < usernames.Length; i++ )
			{
				string username = usernames[i].Trim();

				if ( username.Length > 0 )
				{
					if ( !bFirst ) userNameBuilder.Append( "," ); else bFirst = false;
					userNameBuilder.Append( username );

					// delete this user from the cache if they are in there...
					DeleteFromProfileCacheIfExists( username.ToLower() );
				}
			}

			// call the DB...
			return DB.Current.DeleteProfiles( this.ApplicationName, userNameBuilder.ToString() );
		}

		public override int DeleteProfiles( ProfileInfoCollection profiles )
		{
			if ( profiles == null )
			{
				ExceptionReporter.ThrowArgumentNull( "PROFILE", "PROFILESNULL" );
			}

			if ( profiles.Count < 1 )
			{
				ExceptionReporter.ThrowArgument( "PROFILE", "PROFILESEMPTY" );
			}

			string[] usernames = new string[profiles.Count];

			int index = 0;
			foreach ( ProfileInfo profile in profiles )
			{
				usernames[index++] = profile.UserName;
			}

			return DeleteProfiles( usernames );
		}

		public override ProfileInfoCollection FindInactiveProfilesByUserName( ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords )
		{
			return GetProfileAsCollection( authenticationOption, pageIndex, pageSize, usernameToMatch, userInactiveSinceDate, out totalRecords );
		}

		public override ProfileInfoCollection FindProfilesByUserName( ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords )
		{
			return GetProfileAsCollection( authenticationOption, pageIndex, pageSize, usernameToMatch, null, out totalRecords );
		}

		public override ProfileInfoCollection GetAllInactiveProfiles( ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords )
		{
			return GetProfileAsCollection( authenticationOption, pageIndex, pageSize, null, userInactiveSinceDate, out totalRecords );
		}

		public override ProfileInfoCollection GetAllProfiles( ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords )
		{
			return GetProfileAsCollection( authenticationOption, pageIndex, pageSize, null, null, out totalRecords );
		}

		public override int GetNumberOfInactiveProfiles( ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate )
		{
			if ( authenticationOption == ProfileAuthenticationOption.Anonymous )
			{
				ExceptionReporter.ThrowArgument( "PROFILE", "NOANONYMOUS" );
			}

			return DB.Current.GetNumberInactiveProfiles( this.ApplicationName, userInactiveSinceDate );
		}

		private ProfileInfoCollection GetProfileAsCollection( ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, object userNameToMatch, object inactiveSinceDate, out int totalRecords )
		{
			if ( authenticationOption == ProfileAuthenticationOption.Anonymous )
			{
				ExceptionReporter.ThrowArgument( "PROFILE", "NOANONYMOUS" );
			}
			if ( pageIndex < 0 )
			{
				ExceptionReporter.ThrowArgument( "PROFILE", "PAGEINDEXTOOSMALL" );
			}
			if ( pageSize < 1 )
			{
				ExceptionReporter.ThrowArgument( "PROFILE", "PAGESIZETOOSMALL" );
			}

			// get all the profiles...
			DataSet allProfilesDS = DB.Current.GetProfiles( this.ApplicationName, pageIndex, pageSize, userNameToMatch, inactiveSinceDate );

			// create an instance for the profiles...
			ProfileInfoCollection profiles = new ProfileInfoCollection();

			DataTable allProfilesDT = allProfilesDS.Tables[0];
			DataTable profilesCountDT = allProfilesDS.Tables[1];

			foreach ( DataRow profileRow in allProfilesDT.Rows )
			{
				string username;
				DateTime lastActivity;
				DateTime lastUpdated = DateTime.UtcNow;

				username = profileRow["Username"].ToString();
				lastActivity = DateTime.SpecifyKind( Convert.ToDateTime( profileRow["LastActivity"] ), DateTimeKind.Utc );
				lastUpdated = DateTime.SpecifyKind( Convert.ToDateTime( profileRow["LastUpdated"] ), DateTimeKind.Utc );

				profiles.Add( new ProfileInfo( username, false, lastActivity, lastUpdated, 0 ) );
			}

			// get the first record which is the count...
			totalRecords = Convert.ToInt32( profilesCountDT.Rows[0][0] );

			return profiles;
		}

		public override System.Configuration.SettingsPropertyValueCollection GetPropertyValues( System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyCollection collection )
		{
			SettingsPropertyValueCollection settingPropertyCollection = new SettingsPropertyValueCollection();

			if ( collection == null || collection.Count < 1 || context == null )
				return settingPropertyCollection;

			string username = context["UserName"].ToString();

			if ( String.IsNullOrEmpty( username ) )
				return settingPropertyCollection;

			// this provider doesn't support anonymous users
			if ( !Convert.ToBoolean( context["IsAuthenticated"] ) )
			{
				ExceptionReporter.ThrowArgument( "PROFILE", "NOANONYMOUS" );
			}

			// load the property collection (sync profile class)
			LoadFromPropertyCollection( collection );

			// see if it's cached...
			if ( UserProfileCache.ContainsKey( username.ToLower() ) )
			{
				// just use the cached version...
				return UserProfileCache[username.ToLower()];
			}
			else
			{
				// transfer properties regardless...
				foreach ( SettingsProperty prop in collection )
				{
					settingPropertyCollection.Add( new SettingsPropertyValue( prop ) );
				}

				// get this profile from the DB
				DataSet profileDS = DB.Current.GetProfiles( this.ApplicationName, 0, 1, username, null );
				DataTable profileDT = profileDS.Tables[0];

				if ( profileDT.Rows.Count > 0 )
				{
					DataRow row = profileDT.Rows[0];
					// load the data into the collection...
					foreach ( SettingsPropertyValue prop in settingPropertyCollection )
					{
						object val = row[prop.Name];
						//Only initialize a SettingsPropertyValue for non-null values
						if ( !( val is DBNull || val == null ) )
						{
							prop.PropertyValue = val;
							prop.IsDirty = false;
							prop.Deserialized = true;
						}
					}
				}
				// save this collection to the cache
				UserProfileCache.Add( username.ToLower(), settingPropertyCollection );
			}

			return settingPropertyCollection;
		}

		public override void SetPropertyValues( System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyValueCollection collection )
		{
			string username = (string)context["UserName"];

			if ( username == null || username.Length < 1 || collection.Count < 1 )
				return;

			// this provider doesn't support anonymous users
			if ( !Convert.ToBoolean( context["IsAuthenticated"] ) )
			{
				ExceptionReporter.ThrowArgument( "PROFILE", "NOANONYMOUS" );
			}

			bool itemsToSave = false;

			// First make sure we have at least one item to save
			foreach ( SettingsPropertyValue pp in collection )
			{
				if ( pp.IsDirty )
				{
					itemsToSave = true;
					break;
				}
			}

			if ( !itemsToSave )
				return;

			// load the data for the configuration
			LoadFromPropertyValueCollection( collection );

			object userID = DB.Current.GetProviderUserKey( this.ApplicationName, username );
			if ( userID != null )
			{
				// start saving...
				DB.Current.SetProfileProperties( this.ApplicationName, userID, collection, _settingsColumnsList );
				// erase from the cache
				DeleteFromProfileCacheIfExists( username.ToLower() );
			}
		}

		#endregion
	}

	public class SettingsPropertyColumn
	{
		public SqlDbType DataType;
		public SettingsProperty Settings;
		public int Size;

		public SettingsPropertyColumn()
		{
			// empty for default constructor...
		}

		public SettingsPropertyColumn( SettingsProperty settings, SqlDbType dataType, int size )
		{
			DataType = dataType;
			Settings = settings;
			Size = size;
		}
	}
}
