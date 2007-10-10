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

namespace YAFProviders.Roles
{
	public static class DB
	{
		static public DataTable role_list( object appName, object roleName )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "prov_role_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "AppName", appName );
				cmd.Parameters.AddWithValue( "RoleName", roleName );
				return DBAccess.GetData( cmd );
			}
		}

		static public int role_delete( object appName, object roleName, object deleteOnlyIfRoleIsEmpty )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "prov_role_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "AppName", appName );
				cmd.Parameters.AddWithValue( "RoleName", roleName );
				cmd.Parameters.AddWithValue( "DeleteOnlyIfRoleIsEmpty", deleteOnlyIfRoleIsEmpty );

				SqlParameter p = new SqlParameter( "ReturnValue", SqlDbType.Int );
				p.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.Add( p );

				DBAccess.ExecuteNonQuery( cmd );

				return Convert.ToInt32(cmd.Parameters ["ReturnValue"].Value);
			}
		}
		/*
		static public DataTable group_member( object boardID, object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "group_member" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.GetData( cmd );
			}
		}
		static public long group_save( object groupID, object boardID, object name, object isAdmin, object isGuest, object isStart, object isModerator, object accessMaskID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "group_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "IsAdmin", isAdmin );
				cmd.Parameters.AddWithValue( "IsGuest", isGuest );
				cmd.Parameters.AddWithValue( "IsStart", isStart );
				cmd.Parameters.AddWithValue( "IsModerator", isModerator );
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				return long.Parse( DBAccess.ExecuteScalar( cmd ).ToString() );
			}
		}
		*/
	}
}
