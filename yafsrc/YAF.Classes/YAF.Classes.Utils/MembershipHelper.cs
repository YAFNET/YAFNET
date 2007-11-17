/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
using System.Data;
using System.Text;
using System.Security;
using System.Web.Security;
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
	public static class RoleMembershipHelper
	{
		/// <summary>
		/// Takes all YAF users and creates them in the Membership Provider
		/// </summary>
		/// <param name="pageBoardID"></param>
		public static void SyncUsers( int pageBoardID )
		{
			// first sync unapproved users...
			using (DataTable dt = YAF.Classes.Data.DB.user_list(pageBoardID, DBNull.Value, false))
			{
				MigrateUsersFromDT( pageBoardID, false, dt );
			}
			// then sync approved users...
			using ( DataTable dt = YAF.Classes.Data.DB.user_list( pageBoardID, DBNull.Value, true ) )
			{
				MigrateUsersFromDT( pageBoardID, true, dt );
			}
		}

		static private void MigrateUsersFromDT( int pageBoardID, bool approved, DataTable dt )
		{
			// is this the Yaf membership provider?
			bool isYafProvider = ( Membership.Provider.GetType().Name == "YafMembershipProvider" );
			bool isLegacyYafDB = dt.Columns.Contains( "Location" );

			foreach ( DataRow row in dt.Rows )
			{
				// skip the guest user
				if ( ( int )row ["IsGuest"] > 0 )
					continue;

				string name = row ["Name"].ToString();
				string email = row ["Email"].ToString().ToLower();

				MembershipUser user = Membership.GetUser( name );

				if ( user == null )
				{
					MembershipCreateStatus status = MigrateCreateUser( name, email, "Your email in all lower case", email, approved, out user );

					if ( status != MembershipCreateStatus.Success )
					{
						throw new ApplicationException( string.Format( "Failed to create user {0}: {1}", name, status ) );
					}
					else
					{
						// update the YAF table with the ProviderKey -- update the provider table if this is the YAF provider...
						YAF.Classes.Data.DB.user_migrate( row ["UserID"], user.ProviderUserKey, isYafProvider );

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
						YafUserProfile userProfile = YafContext.Current.GetProfile( name );
						if ( dt.Columns.Contains( "AIM" ) && row ["AIM"] != DBNull.Value ) userProfile.AIM = row ["AIM"].ToString();
						if ( dt.Columns.Contains( "YIM" ) && row ["YIM"] != DBNull.Value ) userProfile.YIM = row ["YIM"].ToString();
						if ( dt.Columns.Contains( "MSN" ) && row ["MSN"] != DBNull.Value ) userProfile.MSN = row ["MSN"].ToString();
						if ( dt.Columns.Contains( "ICQ" ) && row ["ICQ"] != DBNull.Value ) userProfile.ICQ = row ["ICQ"].ToString();
						if ( dt.Columns.Contains( "RealName" ) && row ["RealName"] != DBNull.Value ) userProfile.RealName = row ["RealName"].ToString();
						if ( dt.Columns.Contains( "Occupation" ) && row ["Occupation"] != DBNull.Value ) userProfile.Occupation = row ["Occupation"].ToString();
						if ( dt.Columns.Contains( "Location" ) && row ["Location"] != DBNull.Value ) userProfile.Location = row ["Location"].ToString();
						if ( dt.Columns.Contains( "Homepage" ) && row ["Homepage"] != DBNull.Value ) userProfile.Homepage = row ["Homepage"].ToString();
						if ( dt.Columns.Contains( "Interests" ) && row ["Interests"] != DBNull.Value ) userProfile.Interests = row ["Interests"].ToString();
						if ( dt.Columns.Contains( "Weblog" ) && row ["Weblog"] != DBNull.Value ) userProfile.Blog = row ["Weblog"].ToString();
						if ( dt.Columns.Contains( "Gender" ) && row ["Gender"] != DBNull.Value ) userProfile.Gender = Convert.ToInt32( row ["Gender"] );
					}
				}
				else
				{
					// just update the link just in case...
					YAF.Classes.Data.DB.user_migrate( row ["UserID"], user.ProviderUserKey, false );
				}

				// setup roles for this user...
				using ( DataTable dtGroups = YAF.Classes.Data.DB.usergroup_list( row ["UserID"] ) )
				{
					foreach ( DataRow rowGroup in dtGroups.Rows )
					{
						Roles.AddUserToRole( user.UserName, rowGroup ["Name"].ToString() );
					}
				}
			}
		}

		static private MembershipCreateStatus MigrateCreateUser(string name, string email, string question, string answer, bool approved, out MembershipUser user)
		{
			string password;
			MembershipCreateStatus status;

			// create a new user and generate a password.
			int retry = 0;
			do
			{
				password = Membership.GeneratePassword( 7 + retry, 1 + retry );
				user = Membership.CreateUser( name, password, email, question, answer, approved, out status );
			}
			while ( status == MembershipCreateStatus.InvalidPassword && ++retry < 10 );

			return status;
		}

		static private bool BitSet(object o, int bitmask)
		{
			int i = (int)o;
			return (i & bitmask) != 0;
		}

		/// <summary>
		/// Sets up the user roles from the "start" settings for a given group/role
		/// </summary>
		/// <param name="PageBoardID">Current BoardID</param>
		/// <param name="userName"></param>
		static public void SetupUserRoles(int pageBoardID, string userName)
		{
			using (DataTable dt = YAF.Classes.Data.DB.group_list(pageBoardID, DBNull.Value))
			{
				foreach (DataRow row in dt.Rows)
				{
					// see if the "Is Start" flag is set for this group and NOT the "Is Guest" flag (those roles aren't synced)
					if (BitSet(row["Flags"], 4) && !BitSet(row["Flags"], 2))
					{
						// add the user to this role in membership
						string roleName = row["Name"].ToString();
						Roles.AddUserToRole(userName, roleName);
					}
				}
			}
		}

		/// <summary>
		/// Syncs the ASP.NET roles with YAF group based on YAF (not bi-directional)
		/// </summary>
		/// <param name="PageBoardID"></param>
		static public void SyncRoles(int pageBoardID)
		{
			// get all the groups in YAF DB and create them if they do not exist as a role in membership
			using (DataTable dt = YAF.Classes.Data.DB.group_list(pageBoardID, DBNull.Value))
			{
				foreach (DataRow row in dt.Rows)
				{
					string name = (string)row["Name"];

					// bitset is testing if this role is a "Guest" role...
					// if it is, we aren't syncing it.
					if (!BitSet(row["Flags"], 2) && !Roles.RoleExists(name))
					{
						Roles.CreateRole(name);
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
		public static int? CreateForumUser(MembershipUser user, int pageBoardID)
		{
			int? userID = null;

			try
			{
				userID = YAF.Classes.Data.DB.user_aspnet(pageBoardID, user.UserName, user.Email, user.ProviderUserKey, user.IsApproved);

				foreach (string role in Roles.GetRolesForUser(user.UserName))
				{
					YAF.Classes.Data.DB.user_setrole(pageBoardID, user.ProviderUserKey, role);
				}

				YAF.Classes.Data.DB.eventlog_create(DBNull.Value, user, string.Format("Created forum user {0}", user.UserName));
			}
			catch (Exception x)
			{
				YAF.Classes.Data.DB.eventlog_create(DBNull.Value, "CreateForumUser", x);
			}

			return userID;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="pageBoardID"></param>
		/// <returns></returns>
		public static bool DidCreateForumUser(MembershipUser user, int pageBoardID)
		{
			int? userID = CreateForumUser(user, pageBoardID);
			return (userID == null) ? false : true;
		}

		/// <summary>
		/// Updates the information in the YAF DB from the ASP.NET Membership user information.
		/// Called once per session for a user to sync up the data
		/// </summary>
		/// <param name="user">Current Membership User</param>
		/// <param name="pageBoardID">Current BoardID</param>
		public static void UpdateForumUser(MembershipUser user, int pageBoardID)
		{
			int nUserID = YAF.Classes.Data.DB.user_aspnet(pageBoardID, user.UserName, user.Email, user.ProviderUserKey, user.IsApproved);
			YAF.Classes.Data.DB.user_setrole(pageBoardID, user.ProviderUserKey, DBNull.Value);
			foreach (string role in Roles.GetRolesForUser(user.UserName))
			{
				YAF.Classes.Data.DB.user_setrole(pageBoardID, user.ProviderUserKey, role);
			}
		}
	}

	/// <summary>
	/// This is a stop-gap class to help with syncing operations
	/// with users/membership.
	/// </summary>
	public static class UserMembershipHelper
	{
		/// <summary>
		/// Helper function that gets user data from the DB (or cache)
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static DataRow GetUserRowForID(int userID, bool allowCached)
		{
			string cacheKey = string.Format("UserListForID{0}", userID);
			DataRow userRow = YafCache.Current [YafCache.GetBoardCacheKey( cacheKey )] as DataRow;

			if ( userRow == null || !allowCached)
			{
				DataTable dt = YAF.Classes.Data.DB.user_list(YafContext.Current.PageBoardID, userID, DBNull.Value);
				if (dt.Rows.Count == 1)
				{
					userRow = dt.Rows[0];
					// cache it
					YafCache.Current[YafCache.GetBoardCacheKey(cacheKey)] = userRow;
				}
			}

			return userRow;
		}

		/// <summary>
		/// Default allows the user row to be cached (mostly used for Provider key and UserID which never change)
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static DataRow GetUserRowForID(int userID)
		{
			return GetUserRowForID(userID, true);
		}

		/// <summary>
		/// Gets the user provider key from the UserID for a user
		/// </summary>
		/// <param name="UserID"></param>
		/// <returns></returns>
		public static object GetProviderUserKeyFromID(int userID)
		{
			object providerUserKey = null;
			DataRow row = GetUserRowForID(userID);

			if (row != null)
			{
				if (row["ProviderUserKey"] != DBNull.Value)
					providerUserKey = row["ProviderUserKey"];
			}

			return providerUserKey;
		}

		/// <summary>
		/// Gets the user name from the UesrID
		/// </summary>
		/// <param name="UserID"></param>
		/// <returns></returns>
		public static string GetUserNameFromID(int userID)
		{
			string userName = string.Empty;

			DataRow row = GetUserRowForID(userID);

			if (row != null)
			{
				if (row["Name"] != DBNull.Value)
					userName = row["Name"].ToString();
			}

			return userName;
		}

		/// <summary>
		/// Get the UserID from the ProviderUserKey
		/// </summary>
		/// <param name="providerUserKey"></param>
		/// <returns></returns>
		public static int GetUserIDFromProviderUserKey(object providerUserKey)
		{
			int userID = DB.user_get(YafContext.Current.PageBoardID, providerUserKey);
			return userID;
		}

		/// <summary>
		/// get the membership user from the providerUserKey
		/// </summary>
		/// <param name="providerUserKey"></param>
		/// <returns></returns>
		public static MembershipUser GetMembershipUser(object providerUserKey)
		{
			return Membership.GetUser(providerUserKey);
		}

		/// <summary>
		/// get the membership user from the userID
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static MembershipUser GetMembershipUser(int userID)
		{
			object providerUserKey = GetProviderUserKeyFromID(userID);
			if (providerUserKey != null)
				return GetMembershipUser(providerUserKey);

			return null;
		}

		/// <summary>
		/// get the membership user from the userName
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static MembershipUser GetMembershipUser(string userName)
		{
			return Membership.GetUser(userName);
		}

		/// <summary>
		/// Helper function to update a user's email address.
		/// Syncs with both the YAF DB and Membership Provider.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="newEmail"></param>
		/// <returns></returns>
		public static bool UpdateEmail(int userID, string newEmail)
		{
			object providerUserKey = GetProviderUserKeyFromID(userID);

			if (providerUserKey != null)
			{
				MembershipUser user = Membership.GetUser(providerUserKey);
				user.Email = newEmail;
				Membership.UpdateUser(user);
				DB.user_aspnet(YafContext.Current.PageBoardID, user.UserName, newEmail, user.ProviderUserKey, user.IsApproved);
				return true;
			}

			return false;
		}

		public static bool DeleteUser(int userID)
		{
			string userName = GetUserNameFromID(userID);

			if (userName != string.Empty)
			{
				DB.user_delete(userID);
				Membership.DeleteUser(userName, true);

				return true;
			}

			return false;
		}

		public static bool ApproveUser(int userID)
		{
			object providerUserKey = GetProviderUserKeyFromID(userID);

			if (providerUserKey != null)
			{
				MembershipUser user = Membership.GetUser(providerUserKey);
				if (!user.IsApproved) user.IsApproved = true;
				Membership.UpdateUser(user);
				DB.user_approve(userID);

				return true;
			}

			return false;
		}


		public static void DeleteAllUnapproved()
		{
			// get all users...
			MembershipUserCollection allUsers = Membership.GetAllUsers();

			// iterate through each one...
			foreach (MembershipUser user in allUsers)
			{
				if (!user.IsApproved)
				{
					// delete this user...
					DB.user_delete(GetUserIDFromProviderUserKey(user.ProviderUserKey));
					Membership.DeleteUser(user.UserName, true);
				}
			}
		}

		/// <summary>
		/// For the admin fuction: approve all users. Approves all
		/// users waiting for approval 
		/// </summary>
		public static void ApproveAll()
		{
			// get all users...
			MembershipUserCollection allUsers = Membership.GetAllUsers();

			// iterate through each one...
			foreach (MembershipUser user in allUsers)
			{
				if (!user.IsApproved)
				{
					// approve this user...
					user.IsApproved = true;
					Membership.UpdateUser(user);
					DB.user_approve(GetUserIDFromProviderUserKey(user.ProviderUserKey));
				}
			}
		}

		/// <summary>
		/// Checks Membership Provider to see if a user
		/// with the username and email passed exists.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="email"></param>
		/// <returns>true if they exist</returns>
		public static bool UserExists(string userName, string email)
		{
			bool exists = false;

			if (userName != null)
			{
				if (Membership.FindUsersByName(userName).Count > 0)
				{
					exists = true;
				}
			}
			else if (email != null)
			{
				if (Membership.FindUsersByEmail(email).Count > 0)
				{
					exists = true;
				}
			}

			return exists;
		}


		/// <summary>
		/// Simply tells you if the userID passed is the Guest user
		/// for the current board
		/// </summary>
		/// <param name="userID">ID of user to lookup</param>
		/// <returns>true if the userid is a guest user</returns>
		public static bool IsGuestUser(object userID)
		{
			if (userID == null || userID is DBNull)
			{
				// if supplied userID is null,user is guest
				return true;
			}
			else
			{
				// otherwise evaluate him
				return IsGuestUser((int)userID);
			}
		}
		/// <summary>
		/// Simply tells you if the userID passed is the Guest user
		/// for the current board
		/// </summary>
		/// <param name="userID">ID of user to lookup</param>
		/// <returns>true if the userid is a guest user</returns>
		public static bool IsGuestUser(int userID)
		{
			int guestUserID = -1;
			// obtain board specific cache key
			string cacheKey = YafCache.GetBoardCacheKey(Constants.Cache.GuestUserID);

			// check if there is value cached
			if ( YafCache.Current [cacheKey] == null )
			{
				// get the guest user for this board...
				guestUserID = DB.user_guest(YafContext.Current.PageBoardID);
				// cache it
				YafCache.Current[cacheKey] = guestUserID;
			}
			else
			{
				// retrieve guest user id from cache
				guestUserID = Convert.ToInt32(YafCache.Current[cacheKey]);
			}

			// compare user id from parameter with guest user id
			return (userID == guestUserID);
		}
	}
}
