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
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration.Provider;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using YAF.Classes.Data;

namespace YAF.Providers.Profile
{
	public static class DB
	{
		static public DataSet GetProfiles( object appName, object pageIndex, object pageSize, object userNameToMatch, object inactiveSinceDate )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "prov_profile_getprofiles" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ApplicationName", appName );
				cmd.Parameters.AddWithValue( "PageIndex", pageIndex );
				cmd.Parameters.AddWithValue( "PageSize", pageSize );
				cmd.Parameters.AddWithValue( "UserNameToMatch", userNameToMatch );
				cmd.Parameters.AddWithValue( "InactiveSinceDate", inactiveSinceDate );
				return DBAccess.GetDataset( cmd );
			}
		}

		static public DataTable GetProfileStructure()
		{
			string sql = String.Format( @"SELECT TOP 1 * FROM {0}", DBAccess.GetObjectName( "prov_Profile" ) );

			using ( SqlCommand cmd = new SqlCommand( sql ) )
			{
				cmd.CommandType = CommandType.Text;
				return DBAccess.GetData( cmd );
			}
		}

		static public void AddProfileColumn( string Name, SqlDbType columnType, int size )
		{
			// get column type...
			string type = columnType.ToString();

			if ( size > 0 )
			{
				type += "(" + size.ToString() + ")";
			}

			string sql = String.Format( "ALTER TABLE {0} ADD [{1}] {2} NULL", DBAccess.GetObjectName( "prov_Profile" ), Name, type );

			using ( SqlCommand cmd = new SqlCommand( sql ) )
			{
				cmd.CommandType = CommandType.Text;
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public object GetProviderUserKey( object appName, object username )
		{
			DataRow row = YAF.Providers.Membership.DB.GetUser( appName.ToString(), null, username.ToString(), false );

			if ( row != null )
			{
				return row ["UserID"];
			}

			return null;
		}

		static public void SetProfileProperties( object appName, object userID, System.Configuration.SettingsPropertyValueCollection values, System.Collections.Generic.List<SettingsPropertyColumn> settingsColumnsList )
		{
			using ( SqlCommand cmd = new SqlCommand() )
			{
				string table = YAF.Classes.Data.DBAccess.GetObjectName( "prov_Profile" );

				StringBuilder sqlCommand = new StringBuilder( "IF EXISTS (SELECT 1 FROM " ).Append( table );
				sqlCommand.Append( " WHERE UserId = @UserID) " );
				cmd.Parameters.AddWithValue( "@UserID", userID );

				// Build up strings used in the query
				StringBuilder columnStr = new StringBuilder();
				StringBuilder valueStr = new StringBuilder();
				StringBuilder setStr = new StringBuilder();
				int count = 0;

				foreach ( SettingsPropertyColumn column in settingsColumnsList )
				{
					// only write if it's dirty
					if ( values [column.Settings.Name].IsDirty )
					{
						columnStr.Append( ", " );
						valueStr.Append( ", " );
						columnStr.Append( column.Settings.Name );
						string valueParam = "@Value" + count;
						valueStr.Append( valueParam );
						cmd.Parameters.AddWithValue( valueParam, values [column.Settings.Name].PropertyValue );

						if ( column.DataType != SqlDbType.Timestamp )
						{
							if ( count > 0 )
							{
								setStr.Append( "," );
							}
							setStr.Append( column.Settings.Name );
							setStr.Append( "=" );
							setStr.Append( valueParam );
						}
						count++;
					}
				}

				columnStr.Append( ",LastUpdatedDate " );
				valueStr.Append( ",@LastUpdatedDate" );
				setStr.Append( ",LastUpdatedDate=@LastUpdatedDate" );
				cmd.Parameters.AddWithValue( "@LastUpdatedDate", DateTime.UtcNow );

				sqlCommand.Append( "BEGIN UPDATE " ).Append( table ).Append( " SET " ).Append( setStr.ToString() );
				sqlCommand.Append( " WHERE UserId = '" ).Append( userID.ToString() ).Append( "'" );

				sqlCommand.Append( " END ELSE BEGIN INSERT " ).Append( table ).Append( " (UserId" ).Append( columnStr.ToString() );
				sqlCommand.Append( ") VALUES ('" ).Append( userID.ToString() ).Append( "'" ).Append( valueStr.ToString() ).Append( ") END" );

				cmd.CommandText = sqlCommand.ToString();
				cmd.CommandType = CommandType.Text;

				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public int DeleteProfiles( object appName, object userNames )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "prov_profile_deleteprofiles" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ApplicationName", appName );
				cmd.Parameters.AddWithValue( "UserNames", userNames );
				return Convert.ToInt32( DBAccess.ExecuteScalar( cmd ) );
			}
		}

		static public int DeleteInactiveProfiles( object appName, object inactiveSinceDate )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "prov_profile_deleteinactive" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ApplicationName", appName );
				cmd.Parameters.AddWithValue( "InactiveSinceDate", inactiveSinceDate );
				return Convert.ToInt32( DBAccess.ExecuteScalar( cmd ) );
			}
		}

		static public int GetNumberInactiveProfiles( object appName, object inactiveSinceDate )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "prov_profile_getnumberinactiveprofiles" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ApplicationName", appName );
				cmd.Parameters.AddWithValue( "InactiveSinceDate", inactiveSinceDate );
				return Convert.ToInt32( DBAccess.ExecuteScalar( cmd ) );
			}
		}

		/*
		public static void ValidateAddColumnInProfile( string columnName, SqlDbType columnType )
		{
			SqlCommand cmd = new SqlCommand( sprocName );
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue( "@ApplicationName", appName );
			cmd.Parameters.AddWithValue( "@Username", username );
			cmd.Parameters.AddWithValue( "@IsUserAnonymous", isAnonymous );

			return cmd;
		}
		*/
	}
}
