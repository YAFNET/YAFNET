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
using System.Web;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Pattern;

namespace YAF.Providers.Membership
{
	public class YafMembershipDBConnManager : YafDBConnManager
	{
		public override string ConnectionString
		{
			get
			{

				if ( YafContext.Application[YafMembershipProvider.ConnStrAppKeyName] != null )
				{
					return YafContext.Application[YafMembershipProvider.ConnStrAppKeyName] as string;
				}

				return Config.ConnectionString;
			}
		}
	}

	public class DB
	{
		private YafDBAccess _dbAccess = new YafDBAccess();

		public static DB Current
		{
			get
			{
				return PageSingleton<DB>.Instance;
			}
		}

		public DB()
		{
			_dbAccess.SetConnectionManagerAdapter<YafMembershipDBConnManager>();
		}

		public void UpgradeMembership( int previousVersion, int newVersion )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_upgrade" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@PreviousVersion", previousVersion );
				cmd.Parameters.AddWithValue( "@NewVersion", newVersion );

				_dbAccess.ExecuteNonQuery( cmd );
			}
		}

		public void ChangePassword( string appName, string username, string newPassword, string newSalt, int passwordFormat, string newPasswordAnswer )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_changepassword" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Username", username );
				cmd.Parameters.AddWithValue( "@Password", newPassword );
				cmd.Parameters.AddWithValue( "@PasswordSalt", newSalt );
				cmd.Parameters.AddWithValue( "@PasswordFormat", passwordFormat );
				cmd.Parameters.AddWithValue( "@PasswordAnswer", newPasswordAnswer );

				_dbAccess.ExecuteNonQuery( cmd );
			}
		}

		public void ChangePasswordQuestionAndAnswer( string appName, string username, string passwordQuestion, string passwordAnswer )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_changepasswordquestionandanswer" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Username", username );
				cmd.Parameters.AddWithValue( "@PasswordQuestion", passwordQuestion );
				cmd.Parameters.AddWithValue( "@PasswordAnswer", passwordAnswer );
				_dbAccess.ExecuteNonQuery( cmd );
			}
		}

		public void CreateUser( string appName, string username, string password, string passwordSalt, int passwordFormat, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_createuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Input Parameters
				cmd.Parameters.AddWithValue( "Username", username );
				cmd.Parameters.AddWithValue( "Password", password );
				cmd.Parameters.AddWithValue( "PasswordSalt", passwordSalt );
				cmd.Parameters.AddWithValue( "PasswordFormat", passwordFormat );
				cmd.Parameters.AddWithValue( "Email", email );
				cmd.Parameters.AddWithValue( "PasswordQuestion", passwordQuestion );
				cmd.Parameters.AddWithValue( "PasswordAnswer", passwordAnswer );
				cmd.Parameters.AddWithValue( "IsApproved", isApproved );
				// Input Output Parameters
				SqlParameter paramUserKey = new SqlParameter( "UserKey", SqlDbType.UniqueIdentifier );
				paramUserKey.Direction = ParameterDirection.InputOutput;
				paramUserKey.Value = providerUserKey;
				cmd.Parameters.Add( paramUserKey );

				//Execute
				_dbAccess.ExecuteNonQuery( cmd );
				//Retrieve Output Parameters
				providerUserKey = paramUserKey.Value;

			}
		}

		public void DeleteUser( string appName, string username, bool deleteAllRelatedData )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_deleteuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Username", username );
				cmd.Parameters.AddWithValue( "@DeleteAllRelated", deleteAllRelatedData );
				_dbAccess.ExecuteNonQuery( cmd );
			}
		}

		public DataTable FindUsersByEmail( string appName, string emailToMatch, int pageIndex, int pageSize )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_findusersbyemail" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@EmailAddress", emailToMatch );
				cmd.Parameters.AddWithValue( "@PageIndex", pageIndex );
				cmd.Parameters.AddWithValue( "@PageSize", pageSize );
				return _dbAccess.GetData( cmd );
			}
		}

		public DataTable FindUsersByName( string appName, string usernameToMatch, int pageIndex, int pageSize )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_findusersbyname" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Username", usernameToMatch );
				cmd.Parameters.AddWithValue( "@PageIndex", pageIndex );
				cmd.Parameters.AddWithValue( "@PageSize", pageSize );
				return _dbAccess.GetData( cmd );
			}
		}

		public DataTable GetAllUsers( string appName, int pageIndex, int pageSize )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getallusers" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@PageIndex", pageIndex );
				cmd.Parameters.AddWithValue( "@PageSize", pageSize );
				return _dbAccess.GetData( cmd );
			}
		}

		public int GetNumberOfUsersOnline( string appName, int timeWindow )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getnumberofusersonline" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@TimeWindow", timeWindow );
				cmd.Parameters.AddWithValue( "@CurrentTimeUtc", DateTime.UtcNow );
				SqlParameter p = new SqlParameter( "ReturnValue", SqlDbType.Int );
				p.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.Add( p );
				_dbAccess.ExecuteNonQuery( cmd );
				return Convert.ToInt32( cmd.Parameters["ReturnValue"].Value );
			}
		}

		public DataRow GetUser( string appName, object providerUserKey, string userName, bool userIsOnline )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@UserName", userName );
				cmd.Parameters.AddWithValue( "@UserKey", providerUserKey );
				cmd.Parameters.AddWithValue( "@UserIsOnline", userIsOnline );
				using ( DataTable dt = _dbAccess.GetData( cmd ) )
				{
					if ( dt.Rows.Count > 0 )
						return dt.Rows[0];
					else
						return null;
				}
			}
		}

		public DataTable GetUserPasswordInfo( string appName, string username, bool updateUser )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Username", username );
				cmd.Parameters.AddWithValue( "@UserIsOnline", updateUser );
				return _dbAccess.GetData( cmd );
			}
		}

		public DataTable GetUserNameByEmail( string appName, string email )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getusernamebyemail" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Email", email );
				return _dbAccess.GetData( cmd );
			}
		}

		public void ResetPassword( string appName, string userName, string password, string passwordSalt, int passwordFormat, int maxInvalidPasswordAttempts, int passwordAttemptWindow )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_resetpassword" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@UserName", userName );
				cmd.Parameters.AddWithValue( "@Password", password );
				cmd.Parameters.AddWithValue( "@PasswordSalt", passwordSalt );
				cmd.Parameters.AddWithValue( "@PasswordFormat", passwordFormat );
				cmd.Parameters.AddWithValue( "@MaxInvalidAttempts", maxInvalidPasswordAttempts );
				cmd.Parameters.AddWithValue( "@PasswordAttemptWindow", passwordAttemptWindow );
				cmd.Parameters.AddWithValue( "@CurrentTimeUtc", DateTime.UtcNow );

				_dbAccess.ExecuteNonQuery( cmd );
			}
		}

		public void UnlockUser( string appName, string userName )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_unlockuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@UserName", userName );
				_dbAccess.ExecuteNonQuery( cmd );
			}
		}

		public int UpdateUser( object appName, MembershipUser user, bool requiresUniqueEmail )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_updateuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "UserKey", user.ProviderUserKey );
				cmd.Parameters.AddWithValue( "UserName", user.UserName );
				cmd.Parameters.AddWithValue( "Email", user.Email );
				cmd.Parameters.AddWithValue( "Comment", user.Comment );
				cmd.Parameters.AddWithValue( "IsApproved", user.IsApproved );
				cmd.Parameters.AddWithValue( "LastLogin", user.LastLoginDate );
				cmd.Parameters.AddWithValue( "LastActivity", user.LastActivityDate.ToUniversalTime() );
				cmd.Parameters.AddWithValue( "UniqueEmail", requiresUniqueEmail );
				// Add Return Value
				SqlParameter p = new SqlParameter( "ReturnValue", SqlDbType.Int );
				p.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.Add( p );

				_dbAccess.ExecuteNonQuery( cmd ); // Execute Non SQL Query
				return Convert.ToInt32( p.Value ); // Return
			}
		}
	}
}
