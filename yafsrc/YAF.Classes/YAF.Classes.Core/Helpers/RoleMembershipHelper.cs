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
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Security;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	public static class RoleMembershipHelper
	{
		public static void CreateRole( string roleName )
		{
			YafContext.Current.CurrentRoles.CreateRole( roleName );
		}

		public static void DeleteRole( string roleName, bool throwOnPopulatedRole )
		{
			YafContext.Current.CurrentRoles.DeleteRole( roleName, throwOnPopulatedRole );
		}

		public static string[] GetAllRoles()
		{
			return YafContext.Current.CurrentRoles.GetAllRoles();
		}

		public static bool RoleExists( string roleName )
		{
			return YafContext.Current.CurrentRoles.RoleExists( roleName );
		}

		public static bool IsUserInRole( string username, string role )
		{
			return YafContext.Current.CurrentRoles.IsUserInRole( username, role );
		}

		public static void AddUserToRole( string username, string role )
		{
			YafContext.Current.CurrentRoles.AddUsersToRoles( new string[] { username }, new string[] { role } );
		}

		public static void RemoveUserFromRole( string username, string role )
		{
			YafContext.Current.CurrentRoles.RemoveUsersFromRoles( new string[] { username }, new string[] { role } );
		}

		public static string[] GetRolesForUser( string username )
		{
			return YafContext.Current.CurrentRoles.GetRolesForUser( username );
		}

		/// <summary>
		/// Takes all YAF users and creates them in the Membership Provider
		/// </summary>
		/// <param name="pageBoardID"></param>
		public static void SyncUsers( int pageBoardID )
		{
			// first sync unapproved users...
			using ( DataTable dt = DB.user_list( pageBoardID, DBNull.Value, false ) )
			{
				MigrateUsersFromDT( pageBoardID, false, dt );
			}
			// then sync approved users...
			using ( DataTable dt = DB.user_list( pageBoardID, DBNull.Value, true ) )
			{
				MigrateUsersFromDT( pageBoardID, true, dt );
			}
		}

		static private void MigrateUsersFromDT( int pageBoardID, bool approved, DataTable dt )
		{
			// is this the Yaf membership provider?
			bool isYafProvider = ( YafContext.Current.CurrentMembership.GetType().Name == "YafMembershipProvider" );
			bool isLegacyYafDB = dt.Columns.Contains( "Location" );

			foreach ( DataRow row in dt.Rows )
			{
				// make sure this thread isn't aborted...
				if ( !Thread.CurrentThread.IsAlive ) break;

				// skip the guest user
				if ( (int)row["IsGuest"] > 0 )
					continue;

				string name = row["Name"].ToString().Trim();
				string email = row["Email"].ToString().ToLower().Trim();

				// clean up the name by removing commas...
				name = name.Replace( ",", "" );

				// verify this user & email are not empty
				if ( !String.IsNullOrEmpty( name ) && !String.IsNullOrEmpty( email ) )
				{
					MembershipUser user = UserMembershipHelper.GetUser( name, false );

					if ( user == null )
					{
						MembershipCreateStatus status = MigrateCreateUser( name, email, "Your email in all lower case", email, approved, out user );

						if ( status != MembershipCreateStatus.Success )
						{
							DB.eventlog_create( 0, "MigrateUsers", string.Format( "Failed to create user {0}: {1}", name, status ) );
						}
						else
						{
							// update the YAF table with the ProviderKey -- update the provider table if this is the YAF provider...
							DB.user_migrate( row["UserID"], user.ProviderUserKey, isYafProvider );

							user.Comment = "Migrated from YetAnotherForum.NET";
							Membership.UpdateUser( user );

							if ( !isYafProvider )
							{
								/* Email generated password to user
								System.Text.StringBuilder msg = new System.Text.StringBuilder();
								msg.AppendFormat( "Hello {0}.\r\n\r\n", name );
								msg.AppendFormat( "Here is your new password: {0}\r\n\r\n", password );
								msg.AppendFormat( "Visit {0} at {1}", ForumName, ForumURL );

								YAF.Classes.Data.DB.mail_create( ForumEmail, user.Email, "Forum Upgrade", msg.ToString() );
								*/
							}
						}

						if ( isLegacyYafDB )
						{
							// copy profile data over...
							YafUserProfile userProfile = YafUserProfile.GetProfile( name );
							if ( dt.Columns.Contains( "AIM" ) && row["AIM"] != DBNull.Value ) userProfile.AIM = row["AIM"].ToString();
							if ( dt.Columns.Contains( "YIM" ) && row["YIM"] != DBNull.Value ) userProfile.YIM = row["YIM"].ToString();
							if ( dt.Columns.Contains( "MSN" ) && row["MSN"] != DBNull.Value ) userProfile.MSN = row["MSN"].ToString();
							if ( dt.Columns.Contains( "ICQ" ) && row["ICQ"] != DBNull.Value ) userProfile.ICQ = row["ICQ"].ToString();
							if ( dt.Columns.Contains( "RealName" ) && row["RealName"] != DBNull.Value ) userProfile.RealName = row["RealName"].ToString();
							if ( dt.Columns.Contains( "Occupation" ) && row["Occupation"] != DBNull.Value ) userProfile.Occupation = row["Occupation"].ToString();
							if ( dt.Columns.Contains( "Location" ) && row["Location"] != DBNull.Value ) userProfile.Location = row["Location"].ToString();
							if ( dt.Columns.Contains( "Homepage" ) && row["Homepage"] != DBNull.Value ) userProfile.Homepage = row["Homepage"].ToString();
							if ( dt.Columns.Contains( "Interests" ) && row["Interests"] != DBNull.Value ) userProfile.Interests = row["Interests"].ToString();
							if ( dt.Columns.Contains( "Weblog" ) && row["Weblog"] != DBNull.Value ) userProfile.Blog = row["Weblog"].ToString();
							if ( dt.Columns.Contains( "Gender" ) && row["Gender"] != DBNull.Value ) userProfile.Gender = Convert.ToInt32( row["Gender"] );
							userProfile.Save();
						}
					}
					else
					{
						// just update the link just in case...
						DB.user_migrate( row["UserID"], user.ProviderUserKey, false );
					}

					// setup roles for this user...
					using ( DataTable dtGroups = DB.usergroup_list( row["UserID"] ) )
					{
						foreach ( DataRow rowGroup in dtGroups.Rows )
						{
							AddUserToRole( user.UserName, rowGroup["Name"].ToString() );
						}
					}
				}
			}
		}

		static private MembershipCreateStatus MigrateCreateUser( string name, string email, string question, string answer, bool approved, out MembershipUser user )
		{
			string password;
			MembershipCreateStatus status;

			// create a new user and generate a password.
			int retry = 0;
			do
			{
				password = Membership.GeneratePassword( 7 + retry, 1 + retry );
				user = YafContext.Current.CurrentMembership.CreateUser( name, password, email, question, answer, approved, null, out status );
			}
			while ( status == MembershipCreateStatus.InvalidPassword && ++retry < 10 );

			return status;
		}

		/// <summary>
		/// Sets up the user roles from the "start" settings for a given group/role
		/// </summary>
		/// <param name="pageBoardID">Current BoardID</param>
		/// <param name="userName"></param>
		static public void SetupUserRoles( int pageBoardID, string userName )
		{
			using ( DataTable dt = YAF.Classes.Data.DB.group_list( pageBoardID, DBNull.Value ) )
			{
				foreach ( DataRow row in dt.Rows )
				{
					GroupFlags roleFlags = new GroupFlags( row["Flags"] );
					// see if the "Is Start" flag is set for this group and NOT the "Is Guest" flag (those roles aren't synced)
					if ( roleFlags.IsStart && !roleFlags.IsGuest )
					{
						// add the user to this role in membership
						string roleName = row["Name"].ToString();

						if ( !String.IsNullOrEmpty( roleName ) )
							AddUserToRole( userName, roleName );
					}
				}
			}
		}

		/// <summary>
		/// Syncs the ASP.NET roles with YAF group based on YAF (not bi-directional)
		/// </summary>
		/// <param name="pageBoardID"></param>
		static public void SyncRoles( int pageBoardID )
		{
			// get all the groups in YAF DB and create them if they do not exist as a role in membership
			using ( DataTable dt = YAF.Classes.Data.DB.group_list( pageBoardID, DBNull.Value ) )
			{
				foreach ( DataRow row in dt.Rows )
				{
					string name = (string)row["Name"];
					GroupFlags roleFlags = new GroupFlags( row["Flags"] );

					// testing if this role is a "Guest" role...
					// if it is, we aren't syncing it.
					if ( !String.IsNullOrEmpty( name ) && !roleFlags.IsGuest && !RoleExists( name ) )
					{
						CreateRole( name );
					}
				}

				/* get all the roles and create them in the YAF DB if they do not exist
				foreach ( string role in Roles.GetAllRoles() )
				{
				  int nGroupID = 0;
				  string filter = string.Format( "Name='{0}'", role );
				  DataRow [] rows = dt.Select( filter );

				  if ( rows.Length == 0 )
				  {
					// sets new roles to default "Read Only" access
					nGroupID = ( int ) YAF.Classes.Data.DB.group_save( DBNull.Value, pageBoardID, role, false, false, false, false, 1 );
				  }
				  else
				  {
					nGroupID = ( int ) rows [0] ["GroupID"];
				  }
				}
						*/
			}
		}

		/// <summary>
		/// Creates the user in the YAF DB from the ASP.NET Membership user information.
		/// Also copies the Roles as groups into YAF DB for the current user
		/// </summary>
		/// <param name="user">Current Membership User</param>
		/// <param name="pageBoardID">Current BoardID</param>
		/// <returns>Returns the UserID of the user if everything was successful. Otherwise, null.</returns>
		public static int? CreateForumUser( MembershipUser user, int pageBoardID )
		{
			int? userID = null;

			try
			{
				userID = DB.user_aspnet( pageBoardID, user.UserName, user.Email, user.ProviderUserKey, user.IsApproved );

				foreach ( string role in GetRolesForUser( user.UserName ) )
				{
					DB.user_setrole( pageBoardID, user.ProviderUserKey, role );
				}
				//YAF.Classes.Data.DB.eventlog_create(DBNull.Value, user, string.Format("Created forum user {0}", user.UserName));
			}
			catch ( Exception x )
			{
				DB.eventlog_create( DBNull.Value, "CreateForumUser", x );
			}

			return userID;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="pageBoardID"></param>
		/// <returns></returns>
		public static bool DidCreateForumUser( MembershipUser user, int pageBoardID )
		{
			int? userID = CreateForumUser( user, pageBoardID );
			return ( userID == null ) ? false : true;
		}

		/// <summary>
		/// Updates the information in the YAF DB from the ASP.NET Membership user information.
		/// Called once per session for a user to sync up the data
		/// </summary>
		/// <param name="user">Current Membership User</param>
		/// <param name="pageBoardID">Current BoardID</param>
		public static void UpdateForumUser( MembershipUser user, int pageBoardID )
		{
			if ( user == null ) // Check to make sure its not a guest
			{
				return;
			}

			int nUserID = DB.user_aspnet( pageBoardID, user.UserName, user.Email, user.ProviderUserKey, user.IsApproved );
			// get user groups...
			DataTable groupTable = DB.group_member( pageBoardID, nUserID );
			string[] roles = GetRolesForUser( user.UserName );

			// add groups...
			foreach ( string role in roles )
			{
				if ( !GroupInGroupTable( role, groupTable ) )
				{
					// add the role...
					DB.user_setrole( pageBoardID, user.ProviderUserKey, role );
				}
			}
			// remove groups...
			foreach ( DataRow row in groupTable.Rows )
			{
				if ( !RoleInRoleArray( row["Name"].ToString(), roles ) )
				{
					// remove since there is no longer an association in the membership...
					DB.usergroup_save( nUserID, row["GroupID"], 0 );
				}
			}
		}

		public static bool GroupInGroupTable( string groupName, DataTable groupTable )
		{
			foreach ( DataRow row in groupTable.Rows )
			{
				if ( row["Name"].ToString() == groupName )
				{
					if ( row["Member"].ToString() == "1" )
						return true;
				}
			}

			return false;
		}

		public static bool RoleInRoleArray( string roleName, string[] roleArray )
		{
			foreach ( string role in roleArray )
			{
				if ( role == roleName )
					return true;
			}

			return false;
		}
	}
}
