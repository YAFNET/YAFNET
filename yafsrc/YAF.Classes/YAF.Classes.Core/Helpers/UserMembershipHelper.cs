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
	/// <summary>
	/// This is a stop-gap class to help with syncing operations
	/// with users/membership.
	/// </summary>
	public static partial class UserMembershipHelper
	{
        /// <summary>
        /// Method returns MembershipUser 
        /// </summary>      
        /// <returns>Returns MembershipUser </returns>
		public static MembershipUser GetUser()
		{
			return GetUser( false );
		}
        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns MembershipUser</returns>
		public static MembershipUser GetUser( string username )
		{
			return GetUser( username, false );
		}
        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="username"></param>
        /// <param name="updateOnlineStatus"></param>
        /// <returns>Returns MembershipUser</returns>
		public static MembershipUser GetUser( string username, bool updateOnlineStatus )
		{
			return YafContext.Current.CurrentMembership.GetUser( username, updateOnlineStatus );
		}
        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="providerKey"></param>
        /// <returns>Returns MembershipUser</returns>
		public static MembershipUser GetUser( object providerKey )
		{
			return YafContext.Current.CurrentMembership.GetUser( providerKey, false );
		}
        /// <summary>
        /// Method returns MembershipUser 
        /// </summary>
        /// <param name="providerKey"></param>
        /// <param name="updateOnlineStatus"></param>
        /// <returns>Returns MembershipUser</returns>
		public static MembershipUser GetUser( object providerKey, bool updateOnlineStatus )
		{
			return YafContext.Current.CurrentMembership.GetUser( providerKey, updateOnlineStatus );
		}
        /// <summary>
        /// Method which returns MembershipUser
        /// </summary>
        /// <param name="updateOnlineStatus"></param>
        /// <returns>Returns MembershipUser</returns>
		public static MembershipUser GetUser( bool updateOnlineStatus )
		{
			if ( HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated )
			{
				return GetUser( HttpContext.Current.User.Identity.Name, updateOnlineStatus );
			}

			return null;
		}

        public static MembershipUserCollection GetAllUsers(int pageCount, out int exitCount, int userNumber)
        {
            int totalRecords;
            MembershipUserCollection muc = YafContext.Current.CurrentMembership.GetAllUsers(pageCount, 1000, out totalRecords);
            exitCount = totalRecords;
            return muc;
        }

		public static MembershipUserCollection GetAllUsers()
		{
			int userCount;
            return GetAllUsers(0,out userCount,9999);
		}

		public static MembershipUserCollection FindUsersByName( string username )
		{
			int totalRecords;
			return YafContext.Current.CurrentMembership.FindUsersByName( username, 1, 999999, out totalRecords );
		}

		public static MembershipUserCollection FindUsersByEmail( string email )
		{
			int totalRecords;
			return YafContext.Current.CurrentMembership.FindUsersByEmail( email, 1, 999999, out totalRecords );
		}

		/// <summary>
		/// Verifies that the the user no longer has a cache...
		/// </summary>
		/// <param name="userId"></param>
		public static void ClearCacheForUserId( long userId )
		{
			string cacheKey = string.Format( "UserListForID{0}", userId );
			YafContext.Current.Cache.Remove( YafCache.GetBoardCacheKey( cacheKey ) );
		}

		/// <summary>
		/// Helper function that gets user data from the DB (or cache)
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="allowCached"></param>
		/// <returns></returns>
		public static DataRow GetUserRowForID( long userID, bool allowCached )
		{
			if ( !allowCached ) return DB.user_list( YafContext.Current.PageBoardID, userID, DBNull.Value ).GetFirstRow();

			// get the item cached...
			string cacheKey = YafCache.GetBoardCacheKey( string.Format( "UserListForID{0}", userID ) );

			DataRow userRow = YafContext.Current.Cache.GetItem<DataRow>( cacheKey, 5,
			                                                             () =>
			                                                             DB.user_list( YafContext.Current.PageBoardID, userID,
			                                                                           DBNull.Value ).GetFirstRow() );
			return userRow;
		}

		public static DataRow GetUserRowForID( int userID, bool allowCached )
		{
			return GetUserRowForID( (long)userID, allowCached );
		}

		/// <summary>
		/// Default allows the user row to be cached (mostly used for Provider key and UserID which never change)
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static DataRow GetUserRowForID( long userID )
		{
			return GetUserRowForID( userID, true );
		}

		public static DataRow GetUserRowForID( int userID )
		{
			return GetUserRowForID( (long)userID );
		}

		/// <summary>
		/// Gets the user provider key from the UserID for a user
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static object GetProviderUserKeyFromID( long userID )
		{
			object providerUserKey = null;

			DataRow row = GetUserRowForID( userID );
			if ( row != null )
			{
				if ( row["ProviderUserKey"] != DBNull.Value )
					providerUserKey = row["ProviderUserKey"];
			}

			return providerUserKey;
		}

		/// <summary>
		/// Gets the user name from the UesrID
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static string GetUserNameFromID( long userID )
		{
			string userName = string.Empty;

			DataRow row = GetUserRowForID( userID );
			if ( row != null )
			{
				if ( row["Name"] != DBNull.Value )
					userName = row["Name"].ToString();
			}

			return userName;
		}

		/// <summary>
		/// Get the UserID from the ProviderUserKey
		/// </summary>
		/// <param name="providerUserKey"></param>
		/// <returns></returns>
		public static int GetUserIDFromProviderUserKey( object providerUserKey )
		{
			int userID = DB.user_get( YafContext.Current.PageBoardID, providerUserKey );
			return userID;
		}

		/// <summary>
		/// get the membership user from the providerUserKey
		/// </summary>
		/// <param name="providerUserKey"></param>
		/// <returns></returns>
		public static MembershipUser GetMembershipUserByKey( object providerUserKey )
		{
			// convert to provider type...
			return GetUser( TypeHelper.ConvertObjectToType( providerUserKey, Config.ProviderKeyType ) );
		}

		public static MembershipUser GetMembershipUserById( long? userID )
		{
			if ( userID.HasValue ) return GetMembershipUserById( userID.Value );
			return null;
		}

		/// <summary>
		/// get the membership user from the userID
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public static MembershipUser GetMembershipUserById( long userID )
		{
			object providerUserKey = GetProviderUserKeyFromID( userID );
			if ( providerUserKey != null )
				return GetMembershipUserByKey( providerUserKey );

			return null;
		}

		/// <summary>
		/// get the membership user from the userName
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public static MembershipUser GetMembershipUserByName( string userName )
		{
			return GetUser( userName );
		}

		/// <summary>
		/// Helper function to update a user's email address.
		/// Syncs with both the YAF DB and Membership Provider.
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="newEmail"></param>
		/// <returns></returns>
		public static bool UpdateEmail( int userID, string newEmail )
		{
			object providerUserKey = GetProviderUserKeyFromID( userID );

			if ( providerUserKey != null )
			{
				MembershipUser user = GetUser( TypeHelper.ConvertObjectToType( providerUserKey, Config.ProviderKeyType ) );
				user.Email = newEmail;
				YafContext.Current.CurrentMembership.UpdateUser( user );
				DB.user_aspnet( YafContext.Current.PageBoardID, user.UserName, newEmail, user.ProviderUserKey, user.IsApproved );
				return true;
			}

			return false;
		}

		public static bool DeleteUser( int userID )
		{
			string userName = GetUserNameFromID( userID );

			if ( userName != string.Empty )
			{
                // Delete the images/albums both from database and physically.
                string sUpDir = HttpContext.Current.Server.MapPath(String.Concat(UrlBuilder.FileRoot, YafBoardFolders.Current.Uploads));
                using (DataTable dt = DB.album_list(userID, null))
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        YafAlbum.Album_Image_Delete(sUpDir,dr["AlbumID"],userID,null);
                    }
                }
				YafContext.Current.CurrentMembership.DeleteUser( userName, true );
                DB.user_delete(userID);
				return true;
			}

			return false;
		}

		public static bool ApproveUser( int userID )
		{
			object providerUserKey = GetProviderUserKeyFromID( userID );

			if ( providerUserKey != null )
			{
				MembershipUser user = GetUser( TypeHelper.ConvertObjectToType( providerUserKey, Config.ProviderKeyType ) );
				if ( !user.IsApproved ) user.IsApproved = true;
				YafContext.Current.CurrentMembership.UpdateUser( user );
				DB.user_approve( userID );

				return true;
			}

			return false;
		}

		public static void DeleteAllUnapproved( DateTime createdCutoff )
		{
			int exitCount = 1;
            int pageCount = 0;
			// get all users...
            //vzrus: we should do it by portions for large forums
            while (exitCount > 0)
            {
                MembershipUserCollection allUsers = GetAllUsers(pageCount, out exitCount, 1000);


			// iterate through each one...
			foreach ( MembershipUser user in allUsers )
			{
				if ( !user.IsApproved && user.CreationDate < createdCutoff )
				{
					// delete this user...
					DB.user_delete( GetUserIDFromProviderUserKey( user.ProviderUserKey ) );
					YafContext.Current.CurrentMembership.DeleteUser( user.UserName, true );
				}
			}
            pageCount++;
            }
		}

		/// <summary>
		/// For the admin fuction: approve all users. Approves all
		/// users waiting for approval 
		/// </summary>
		public static void ApproveAll()
		{
            int exitCount = 1;
            int pageCount = 0;
			// get all users...
            //vzrus: we should do it by portions for large forums
            while (exitCount > 0)
            {
                MembershipUserCollection allUsers = GetAllUsers(pageCount, out exitCount, 1000);

                // iterate through each one...
                foreach (MembershipUser user in allUsers)
                {
                    if (!user.IsApproved)
                    {
                        // approve this user...
                        user.IsApproved = true;
                        YafContext.Current.CurrentMembership.UpdateUser(user);
                        int id = GetUserIDFromProviderUserKey(user.ProviderUserKey);
                        if (id > 0)
                            DB.user_approve(id);
                    }
                }
                pageCount++;
            }
		}

		/// <summary>
		/// Checks Membership Provider to see if a user
		/// with the username and email passed exists.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="email"></param>
		/// <returns>true if they exist</returns>
		public static bool UserExists( string userName, string email )
		{
			bool exists = false;

			if ( userName != null )
			{
				if ( FindUsersByName( userName ).Count > 0 )
				{
					exists = true;
				}
			}
			else if ( email != null )
			{
				if ( FindUsersByEmail( email ).Count > 0 )
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
		public static bool IsGuestUser( object userID )
		{
			if ( userID == null || userID is DBNull )
			{
				// if supplied userID is null,user is guest
				return true;
			}
			else
			{
				// otherwise evaluate him
				return IsGuestUser( (int)userID );
			}
		}

		/// <summary>
		/// Simply tells you if the userID passed is the Guest user
		/// for the current board
		/// </summary>
		/// <param name="userID">ID of user to lookup</param>
		/// <returns>true if the userid is a guest user</returns>
		public static bool IsGuestUser( int userID )
		{
			return GuestUserId == userID;
		}

	
		/// <summary>
		/// Gets the guest user id for the current board.
		/// </summary>
		public static int GuestUserId
		{
			get
			{
				int guestUserID = -1;
				// obtain board specific cache key
				string cacheKey = YafCache.GetBoardCacheKey( Constants.Cache.GuestUserID );

				// check if there is value cached
				if ( YafContext.Current.Cache[cacheKey] == null )
				{
					// get the guest user for this board...
					guestUserID = DB.user_guest( YafContext.Current.PageBoardID );
					// cache it
					YafContext.Current.Cache[cacheKey] = guestUserID;
				}
				else
				{
					// retrieve guest user id from cache
					guestUserID = Convert.ToInt32( YafContext.Current.Cache[cacheKey] );
				}

				return guestUserID;
			}
		}

		/// <summary>
		/// Username of the Guest user for the current board.
		/// </summary>
		public static string GuestUserName
		{
			get
			{
				return DB.user_list( YafContext.Current.PageBoardID, GuestUserId, true ).GetFirstRowColumnAsValue<string>( "Name", null );
			}
		}
	}
}
