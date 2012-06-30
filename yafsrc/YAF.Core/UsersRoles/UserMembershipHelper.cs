/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Core
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Services;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Utils.Structures;

    /// <summary>
    /// This is a stop-gap class to help with syncing operations
    /// with users/membership.
    /// </summary>
    public static partial class UserMembershipHelper
    {
        #region Properties

        /// <summary>
        /// Gets the guest user id for the current board.
        /// </summary>
        /// <exception cref="NoValidGuestUserForBoardException">No Valid Guest User Exception</exception>
        public static int GuestUserId
        {
            get
            {
                int? guestUserID = YafContext.Current.Get<IDataCache>().GetOrSet(
                    Constants.Cache.GuestUserID,
                    () =>
                        {
                            // get the guest user for this board...
                            guestUserID = LegacyDb.user_guest(YafContext.Current.PageBoardID);

                            if (!guestUserID.HasValue)
                            {
                                //// attempt to fix the guest user by re-associating them with the guest group...
                                // FixGuestUserForBoard(YafContext.Current.PageBoardID);

                                // attempt to get the guestUser again...
                                guestUserID = LegacyDb.user_guest(YafContext.Current.PageBoardID);
                            }

                            if (!guestUserID.HasValue)
                            {
                                // failure...
                                throw new NoValidGuestUserForBoardException(
                                    "Could not locate the guest user for the board id {0}. You might have deleted the guest group or removed the guest user."
                                        .FormatWith(YafContext.Current.PageBoardID));
                            }

                            return guestUserID.Value;
                        });

                return guestUserID ?? -1;
            }
        }

        /*public static void FixGuestUserForBoard(int boardId)
        //{
        //  // find the most likely guest user...
        //  var users = DB.UserFind(boardId, false, null, null, null, null, null);
        //  var guestGroup = DB.group_list(boardId, null).AsEnumerable().Where(x => x.Field<int>("Flags").Equals(2));

        //  if (users.Any(x => x.IsGuest) && guestGroup.Any())
        //  {
        //    // add guest user to guest group...
        //    DB.usergroup_save(users.First(), guestGroup.First().Field<int>("GroupID"), 1);
        //  }
        }*/

        /// <summary>
        /// Gets the Username of the Guest user for the current board.
        /// </summary>
        public static string GuestUserName
        {
            get
            {
                return
                    LegacyDb.user_list(YafContext.Current.PageBoardID, GuestUserId, true).GetFirstRowColumnAsValue<string>("Name", null);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// For the admin fuction: approve all users. Approves all
        /// users waiting for approval 
        /// </summary>
        public static void ApproveAll()
        {
            int exitCount = 1;
            int pageCount = 0;

            // get all users...
            // vzrus: we should do it by portions for large forums
            while (exitCount > 0)
            {
                MembershipUserCollection allUsers = GetAllUsers(pageCount, out exitCount, 1000);

                // iterate through each one...
                foreach (MembershipUser user in allUsers.Cast<MembershipUser>().Where(user => !user.IsApproved))
                {
                    // approve this user...
                    user.IsApproved = true;
                    YafContext.Current.Get<MembershipProvider>().UpdateUser(user);
                    int id = GetUserIDFromProviderUserKey(user.ProviderUserKey);
                    if (id > 0)
                    {
                        LegacyDb.user_approve(id);
                    }
                }

                pageCount++;
            }
        }

        /// <summary>
        /// Approves the user.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <returns>
        /// The approve user.
        /// </returns>
        public static bool ApproveUser(int userID)
        {
            object providerUserKey = GetProviderUserKeyFromID(userID);

            if (providerUserKey != null)
            {
                MembershipUser user =
                    GetUser(ObjectExtensions.ConvertObjectToType(providerUserKey, Config.ProviderKeyType));
                if (!user.IsApproved)
                {
                    user.IsApproved = true;
                }

                YafContext.Current.Get<MembershipProvider>().UpdateUser(user);
                LegacyDb.user_approve(userID);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifies that the the user no longer has a cache...
        /// </summary>
        /// <param name="userId">The user id.</param>
        public static void ClearCacheForUserId(long userId)
        {
            YafContext.Current.Get<IUserDisplayName>().Clear((int)userId);
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.UserListForID.FormatWith(userId));

            var cache = YafContext.Current.Get<IDataCache>().GetOrSet(
                Constants.Cache.UserSignatureCache, () => new MostRecentlyUsed(250), TimeSpan.FromMinutes(10));

            // remove from the the signature cache...
            cache.Remove((int)userId);

            // Clearing cache with old Active User Lazy Data ...
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userId));
        }

        /// <summary>
        /// The delete all unapproved.
        /// </summary>
        /// <param name="createdCutoff">
        /// The created cutoff.
        /// </param>
        public static void DeleteAllUnapproved(DateTime createdCutoff)
        {
            int exitCount = 1;
            int pageCount = 0;

            // get all users...
            // vzrus: we should do it by portions for large forums
            while (exitCount > 0)
            {
                MembershipUserCollection allUsers = GetAllUsers(pageCount, out exitCount, 1000);

                // iterate through each one...
                foreach (MembershipUser user in
                    allUsers.Cast<MembershipUser>().Where(user => !user.IsApproved && user.CreationDate < createdCutoff))
                {
                    // delete this user...
                    LegacyDb.user_delete(GetUserIDFromProviderUserKey(user.ProviderUserKey));
                    YafContext.Current.Get<MembershipProvider>().DeleteUser(user.UserName, true);
                    YafContext.Current.Get<ILogger>().UserDeleted(YafContext.Current.PageUserID, "UserMembershipHelper.DeleteAllUnapproved", "User {0} was deleted by user id {1} as unapproved.".FormatWith(user.UserName, YafContext.Current.PageUserID));
                }

                pageCount++;
            }
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <returns>
        /// Returns if Deleting was successfully
        /// </returns>
        public static bool DeleteUser(int userID)
        {
            string userName = GetUserNameFromID(userID);

            if (userName != string.Empty)
            {
                // Delete the images/albums both from database and physically.
                string sUpDir =
                    HttpContext.Current.Server.MapPath(
                        string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

                using (DataTable dt = LegacyDb.album_list(userID, null))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        YafAlbum.Album_Image_Delete(sUpDir, dr["AlbumID"], userID, null);
                    }
                }

                YafContext.Current.Get<MembershipProvider>().DeleteUser(userName, true);
                LegacyDb.user_delete(userID);
                YafContext.Current.Get<ILogger>().UserDeleted(YafContext.Current.PageUserID, "UserMembershipHelper.DeleteUser", "User {0} was deleted by user id {1}.".FormatWith(userName, YafContext.Current.PageUserID));
                
                // clear the cache
                YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
                YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
                YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersDisplayNameCollection);

                return true;
            }

            return false;
        }

        /// <summary>
        /// The find users by email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// Returns the Collection of founded Users
        /// </returns>
        public static MembershipUserCollection FindUsersByEmail(string email)
        {
            int totalRecords;
            return YafContext.Current.Get<MembershipProvider>().FindUsersByEmail(email, 0, 999999, out totalRecords);
        }

        /// <summary>
        /// The find users by name.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// Returns the Collection of founded Users
        /// </returns>
        public static MembershipUserCollection FindUsersByName(string username)
        {
            int totalRecords;
            return YafContext.Current.Get<MembershipProvider>().FindUsersByName(username, 0, 999999, out totalRecords);
        }

        /// <summary>
        /// The get all users.
        /// </summary>
        /// <param name="pageCount">
        /// The page count.
        /// </param>
        /// <param name="exitCount">
        /// The exit count.
        /// </param>
        /// <param name="userNumber">
        /// The user number.
        /// </param>
        /// <returns>
        /// Returns Collection of All Users
        /// </returns>
        public static MembershipUserCollection GetAllUsers(int pageCount, out int exitCount, int userNumber)
        {
            int totalRecords;
            MembershipUserCollection muc = YafContext.Current.Get<MembershipProvider>().GetAllUsers(
                pageCount, 1000, out totalRecords);
            exitCount = totalRecords;
            return muc;
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>
        /// The get all users.
        /// </returns>
        public static MembershipUserCollection GetAllUsers()
        {
            int userCount;
            return GetAllUsers(0, out userCount, 9999);
        }

        /// <summary>
        /// Gets the membership user by id.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <returns>
        /// The get membership user by id.
        /// </returns>
        public static MembershipUser GetMembershipUserById(long? userID)
        {
            return userID.HasValue ? GetMembershipUserById(userID.Value) : null;
        }

        /// <summary>
        /// get the membership user from the userID
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <returns>
        /// The get membership user by id.
        /// </returns>
        public static MembershipUser GetMembershipUserById(long userID)
        {
            object providerUserKey = GetProviderUserKeyFromID(userID);

            return providerUserKey != null ? GetMembershipUserByKey(providerUserKey) : null;
        }

        /// <summary>
        /// get the membership user from the providerUserKey
        /// </summary>
        /// <param name="providerUserKey">
        /// The provider user key.
        /// </param>
        /// <returns>
        /// The get membership user by key.
        /// </returns>
        public static MembershipUser GetMembershipUserByKey(object providerUserKey)
        {
            // convert to provider type...
            return GetUser(ObjectExtensions.ConvertObjectToType(providerUserKey, Config.ProviderKeyType));
        }

        /// <summary>
        /// get the membership user from the userName
        /// </summary>
        /// <param name="userName">
        /// Name of the user.
        /// </param>
        /// <returns>
        /// The get membership user by name.
        /// </returns>
        public static MembershipUser GetMembershipUserByName(string userName)
        {
            return GetUser(userName);
        }

        /// <summary>
        /// Gets the user provider key from the UserID for a user
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>
        /// The get provider user key from id.
        /// </returns>
        public static object GetProviderUserKeyFromID(long userID)
        {
            object providerUserKey = null;

            DataRow row = GetUserRowForID(userID);
            if (row != null)
            {
                if (row["ProviderUserKey"] != DBNull.Value)
                {
                    providerUserKey = row["ProviderUserKey"];
                }
            }

            return providerUserKey;
        }

        /// <summary>
        /// Method returns MembershipUser 
        /// </summary>
        /// <returns>
        /// Returns MembershipUser 
        /// </returns>
        public static MembershipUser GetUser()
        {
            return GetUser(false);
        }

        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        /// Returns MembershipUser
        /// </returns>
        public static MembershipUser GetUser(string username)
        {
            return GetUser(username, false);
        }

        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="updateOnlineStatus">if set to <c>true</c> [update online status].</param>
        /// <returns>
        /// Returns MembershipUser
        /// </returns>
        public static MembershipUser GetUser(string username, bool updateOnlineStatus)
        {
            return YafContext.Current.Get<MembershipProvider>().GetUser(username, updateOnlineStatus);
        }

        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="providerKey">The provider key.</param>
        /// <returns>
        /// Returns MembershipUser
        /// </returns>
        public static MembershipUser GetUser(object providerKey)
        {
            return YafContext.Current.Get<MembershipProvider>().GetUser(providerKey, false);
        }

        /// <summary>
        /// Method returns Application Name 
        /// </summary>
        /// <returns>
        /// Returns Application Name 
        /// </returns>
        public static string ApplicationName()
        {
            return YafContext.Current.Get<MembershipProvider>().ApplicationName;
        }

        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="providerKey">The provider key.</param>
        /// <param name="updateOnlineStatus">if set to <c>true</c> [update online status].</param>
        /// <returns>
        /// Returns MembershipUser
        /// </returns>
        public static MembershipUser GetUser(object providerKey, bool updateOnlineStatus)
        {
            return YafContext.Current.Get<MembershipProvider>().GetUser(providerKey, updateOnlineStatus);
        }

        /// <summary>
        /// Method which returns MembershipUser
        /// </summary>
        /// <param name="updateOnlineStatus">if set to <c>true</c> [update online status].</param>
        /// <returns>
        /// Returns MembershipUser
        /// </returns>
        public static MembershipUser GetUser(bool updateOnlineStatus)
        {
            return YafContext.Current.Get<HttpContextBase>().User != null
                   && YafContext.Current.Get<HttpContextBase>().User.Identity.IsAuthenticated
                       ? GetUser(HttpContext.Current.User.Identity.Name, updateOnlineStatus)
                       : null;
        }

        /// <summary>
        /// Get the UserID from the ProviderUserKey
        /// </summary>
        /// <param name="providerUserKey">The provider user key.</param>
        /// <returns>
        /// The get user id from provider user key.
        /// </returns>
        public static int GetUserIDFromProviderUserKey(object providerUserKey)
        {
            int userID = LegacyDb.user_get(YafContext.Current.PageBoardID, providerUserKey.ToString());
            return userID;
        }

        /// <summary>
        /// Gets the user name from the UesrID
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>
        /// The get user name from id.
        /// </returns>
        public static string GetUserNameFromID(long userID)
        {
            string userName = string.Empty;

            DataRow row = GetUserRowForID(userID, true);
            if (row != null)
            {
                if (!row["Name"].IsNullOrEmptyDBField())
                {
                    userName = row["Name"].ToString();
                }
            }

            return userName;
        }

        /// <summary>
        /// Gets the user name from the UesrID
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>
        /// The get user name from id.
        /// </returns>
        public static string GetDisplayNameFromID(long userID)
        {
            string displayName = string.Empty;

            DataRow row = GetUserRowForID(userID, true);
            if (row != null)
            {
                if (!row["DisplayName"].IsNullOrEmptyDBField())
                {
                    displayName = row["DisplayName"].ToString();
                }
            }

            return displayName;
        }

        /// <summary>
        /// Helper function that gets user data from the DB (or cache)
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="allowCached">
        /// if set to <c>true</c> [allow cached].
        /// </param>
        /// <returns>
        /// The get user row for id.
        /// </returns>
        public static DataRow GetUserRowForID(long userID, bool allowCached)
        {
            if (!allowCached)
            {
                return LegacyDb.user_list(YafContext.Current.PageBoardID, userID, DBNull.Value).GetFirstRow();
            }

            // get the item cached...
            return
                YafContext.Current.Get<IDataCache>().GetOrSet(
                    Constants.Cache.UserListForID.FormatWith(userID),
                    () => LegacyDb.user_list(YafContext.Current.PageBoardID, userID, DBNull.Value),
                    TimeSpan.FromMinutes(5)).GetFirstRow();
        }

        /// <summary>
        /// Gets the user row for ID.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <param name="allowCached">The allow cached.</param>
        /// <returns>
        /// The get user row for id.
        /// </returns>
        public static DataRow GetUserRowForID(int userID, bool allowCached)
        {
            return GetUserRowForID((long)userID, allowCached);
        }

        /// <summary>
        /// Default allows the user row to be cached (mostly used for Provider key and UserID which never change)
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <returns>
        /// The get user row for id.
        /// </returns>
        public static DataRow GetUserRowForID(long userID)
        {
            return GetUserRowForID(userID, true);
        }

        /// <summary>
        /// Gets the user row for ID.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <returns>
        /// The get user row for id.
        /// </returns>
        public static DataRow GetUserRowForID(int userID)
        {
            return GetUserRowForID((long)userID);
        }

        /// <summary>
        /// Simply tells you if the userID passed is the Guest user
        /// for the current board
        /// </summary>
        /// <param name="userID">
        /// ID of user to lookup
        /// </param>
        /// <returns>
        /// true if the userid is a guest user
        /// </returns>
        public static bool IsGuestUser(object userID)
        {
            return userID == null || userID is DBNull || IsGuestUser((int)userID);
        }

        /// <summary>
        /// Simply tells you if the userID passed is the Guest user
        /// for the current board
        /// </summary>
        /// <param name="userID">
        /// ID of user to lookup
        /// </param>
        /// <returns>
        /// true if the userid is a guest user
        /// </returns>
        public static bool IsGuestUser(int userID)
        {
            return GuestUserId == userID;
        }

        /// <summary>
        /// Helper function to update a user's email address.
        /// Syncs with both the YAF DB and Membership Provider.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="newEmail">The new email.</param>
        /// <returns>
        /// The update email.
        /// </returns>
        public static bool UpdateEmail(int userID, string newEmail)
        {
            object providerUserKey = GetProviderUserKeyFromID(userID);

            if (providerUserKey != null)
            {
                MembershipUser user =
                    GetUser(ObjectExtensions.ConvertObjectToType(providerUserKey, Config.ProviderKeyType));

                user.Email = newEmail;

                YafContext.Current.Get<MembershipProvider>().UpdateUser(user);

                LegacyDb.user_aspnet(
                    YafContext.Current.PageBoardID, user.UserName, null, newEmail, user.ProviderUserKey, user.IsApproved);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks Membership Provider to see if a user
        /// with the username and email passed exists.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="email">The email.</param>
        /// <returns>
        /// true if they exist
        /// </returns>
        public static bool UserExists(string userName, string email)
        {
            bool exists = false;

            if (userName != null)
            {
                if (FindUsersByName(userName).Count > 0)
                {
                    exists = true;
                }
            }
            else if (email != null)
            {
                if (FindUsersByEmail(email).Count > 0)
                {
                    exists = true;
                }
            }

            return exists;
        }

        #endregion
    }
}