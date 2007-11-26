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
			using ( SqlCommand cmd = DBAccess.GetCommand( DBAccess.GetObjectName( "prov_profile_getprofiles" ) ) )
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

		static public DataTable GetProfileStructure( object appName )
		{
			object applicationID = null;

			// get the AppID
			using ( SqlCommand cmd = DBAccess.GetCommand( DBAccess.GetObjectName( "prov_CreateApplication" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ApplicationName", appName);
				SqlParameter appID = new SqlParameter("ApplicationID", SqlDbType.UniqueIdentifier);
				appID.Direction = ParameterDirection.Output;
				cmd.Parameters.Add( appID );
				DBAccess.ExecuteNonQuery( cmd );

				applicationID = appID.Value;
			}

			string sql = String.Format( @"SELECT TOP 1 * FROM {0} WHERE ApplicationID = @ApplicationID", DBAccess.GetObjectName( "prov_Profile" ) );

			using ( SqlCommand cmd = DBAccess.GetCommand( sql ) )
			{
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue( "ApplicationID", applicationID );
				return DBAccess.GetData( cmd );
			}
		}

		static public void AddProfileColumn( string Name, SqlDbType columnType, int size)
		{
			// get column type...
			string type = columnType.ToString();

			if (size > 0)
			{
				type += "(" + size.ToString() + ")";
			}

			string sql = "alter table {0} add column {1} {2} NULL";

			using ( SqlCommand cmd = DBAccess.GetCommand( sql ) )
			{
				cmd.CommandType = CommandType.Text;
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public int DeleteProfiles( object appName, object userNames )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( DBAccess.GetObjectName( "prov_profile_deleteprofiles" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ApplicationName", appName );
				cmd.Parameters.AddWithValue( "UserNames", userNames );
				return Convert.ToInt32(DBAccess.ExecuteScalar( cmd ));
			}
		}

		static public int DeleteInactiveProfiles( object appName, object inactiveSinceDate )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( DBAccess.GetObjectName( "prov_profile_deleteinactive" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ApplicationName", appName );
				cmd.Parameters.AddWithValue( "InactiveSinceDate", inactiveSinceDate );
				return Convert.ToInt32( DBAccess.ExecuteScalar( cmd ) );
			}
		}

		static public int GetNumberInactiveProfiles( object appName, object inactiveSinceDate )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( DBAccess.GetObjectName( "prov_profile_getnumberinactiveprofiles" ) ) )
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
