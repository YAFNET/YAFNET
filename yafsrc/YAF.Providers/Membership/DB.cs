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
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using YAF.Classes.Data;

namespace YAF.Providers.Membership
{
	public class DB
	{
		public static void UpgradeMembership( int previousVersion, int newVersion )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_upgrade" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@PreviousVersion", previousVersion );
				cmd.Parameters.AddWithValue( "@NewVersion", newVersion );

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}		

		}

		public static void ChangePassword( string appName, string username, string newPassword, string newSalt, int passwordFormat, string newPasswordAnswer )
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

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		public static void ChangePasswordQuestionAndAnswer( string appName, string username, string passwordQuestion, string passwordAnswer )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_changepasswordquestionandanswer" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Username", username );
				cmd.Parameters.AddWithValue( "@PasswordQuestion", passwordQuestion );
				cmd.Parameters.AddWithValue( "@PasswordAnswer", passwordAnswer );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		public static void CreateUser( string appName, string username, string password, string passwordSalt, int passwordFormat, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey )
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
				YafDBAccess.Current.ExecuteNonQuery( cmd );
				//Retrieve Output Parameters
				providerUserKey = paramUserKey.Value;

			}
		}

		public static void DeleteUser( string appName, string username, bool deleteAllRelatedData )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_deleteuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Username", username );
				cmd.Parameters.AddWithValue( "@DeleteAllRelated", deleteAllRelatedData );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		public static DataTable FindUsersByEmail( string appName, string emailToMatch, int pageIndex, int pageSize )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_findusersbyemail" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@EmailAddress", emailToMatch );
				cmd.Parameters.AddWithValue( "@PageIndex", pageIndex );
				cmd.Parameters.AddWithValue( "@PageSize", pageSize );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		public static DataTable FindUsersByName( string appName, string usernameToMatch, int pageIndex, int pageSize )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_findusersbyname" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Username", usernameToMatch );
				cmd.Parameters.AddWithValue( "@PageIndex", pageIndex );
				cmd.Parameters.AddWithValue( "@PageSize", pageSize );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		public static DataTable GetAllUsers( string appName, int pageIndex, int pageSize )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getallusers" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@PageIndex", pageIndex );
				cmd.Parameters.AddWithValue( "@PageSize", pageSize );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		public static int GetNumberOfUsersOnline( string appName, int TimeWindow )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getnumberofusersonline" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@TimeWindow", TimeWindow );
				cmd.Parameters.AddWithValue( "@CurrentTimeUtc", DateTime.UtcNow );
				SqlParameter p = new SqlParameter( "ReturnValue", SqlDbType.Int );
				p.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.Add( p );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
				return Convert.ToInt32( cmd.Parameters ["ReturnValue"].Value );
			}
		}

		public static DataRow GetUser( string appName, object providerUserKey, string userName, bool userIsOnline )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@UserName", userName );
				cmd.Parameters.AddWithValue( "@UserKey", providerUserKey );
				cmd.Parameters.AddWithValue( "@UserIsOnline", userIsOnline );
				using ( DataTable dt = YafDBAccess.Current.GetData( cmd ) )
				{
					if ( dt.Rows.Count > 0 )
						return dt.Rows [0];
					else
						return null;
				}
			}

		}

		public static DataTable GetUserPasswordInfo( string appName, string username, bool updateUser )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Username", username );
				cmd.Parameters.AddWithValue( "@UserIsOnline", updateUser );
				return YafDBAccess.Current.GetData( cmd );
			}

		}

		public static DataTable GetUserNameByEmail( string appName, string email )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_getusernamebyemail" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@Email", email );
				return YafDBAccess.Current.GetData( cmd );
			}
		}


		public static void ResetPassword( string appName, string userName, string password, string passwordSalt, int passwordFormat, int maxInvalidPasswordAttempts, int passwordAttemptWindow )
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

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}

		}

		public static void UnlockUser( string appName, string userName )
		{
			using ( SqlCommand cmd = new SqlCommand( YafDBAccess.GetObjectName( "prov_unlockuser" ) ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ApplicationName", appName );
				// Nonstandard args
				cmd.Parameters.AddWithValue( "@UserName", userName );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		public static int UpdateUser( object appName, MembershipUser user, bool requiresUniqueEmail )
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

				YafDBAccess.Current.ExecuteNonQuery( cmd ); // Execute Non SQL Query
				return Convert.ToInt32( p.Value ); // Return
			}
		}

	}
}
