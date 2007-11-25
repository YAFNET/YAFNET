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
