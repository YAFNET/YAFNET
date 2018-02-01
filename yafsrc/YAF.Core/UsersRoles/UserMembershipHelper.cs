/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core
{
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types.Constants;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
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
                    LegacyDb.user_list(YafContext.Current.PageBoardID, GuestUserId, true)
                        .GetFirstRowColumnAsValue<string>("Name", null);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// For the admin function: approve all users. Approves all
        /// users waiting for approval
        /// </summary>
        public static void ApproveAll()
        {
            var exitCount = 1;
            var pageCount = 0;

            // get all users...
            // vzrus: we should do it by portions for large forums
            while (exitCount > 0)
            {
                var allUsers = GetAllUsers(pageCount, out exitCount, 1000);

                // iterate through each one...
                foreach (var user in allUsers.Cast<MembershipUser>().Where(user => !user.IsApproved))
                {
                    // approve this user...
                    user.IsApproved = true;
                    YafContext.Current.Get<MembershipProvider>().UpdateUser(user);
                    var id = GetUserIDFromProviderUserKey(user.ProviderUserKey);
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
            var providerUserKey = GetProviderUserKeyFromID(userID);

            if (providerUserKey == null)
            {
                return false;
            }

            var user = GetUser(ObjectExtensions.ConvertObjectToType(providerUserKey, Config.ProviderKeyType));
            if (!user.IsApproved)
            {
                user.IsApproved = true;
            }

            YafContext.Current.Get<MembershipProvider>().UpdateUser(user);
            LegacyDb.user_approve(userID);

            return true;
        }

        /// <summary>
        /// Verifies that the the user no longer has a cache...
        /// </summary>
        /// <param name="userId">The user id.</param>
        public static void ClearCacheForUserId(long userId)
        {
            YafContext.Current.Get<IUserDisplayName>().Clear((int)userId);
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.UserListForID.FormatWith(userId));

            var cache = YafContext.Current.Get<IDataCache>()
                .GetOrSet(Constants.Cache.UserSignatureCache, () => new MostRecentlyUsed(250), TimeSpan.FromMinutes(10));

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
            var exitCount = 1;
            var pageCount = 0;

            // get all users...
            // vzrus: we should do it by portions for large forums
            while (exitCount > 0)
            {
                var allUsers = GetAllUsers(pageCount, out exitCount, 1000);

                // iterate through each one...
                foreach (var user in
                    allUsers.Cast<MembershipUser>().Where(user => !user.IsApproved && user.CreationDate < createdCutoff))
                {
                    // delete this user...
                    LegacyDb.user_delete(GetUserIDFromProviderUserKey(user.ProviderUserKey));
                    YafContext.Current.Get<MembershipProvider>().DeleteUser(user.UserName, true);
                    YafContext.Current.Get<ILogger>()
                        .Log(
                            YafContext.Current.PageUserID,
                            "UserMembershipHelper.DeleteAllUnapproved",
                            "User {0} was deleted by user id {1} as unapproved.".FormatWith(
                                user.UserName,
                                YafContext.Current.PageUserID),
                            EventLogTypes.UserDeleted);
                }

                pageCount++;
            }
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <param name="isBotAutoDelete">if set to <c>true</c> [is bot automatic delete].</param>
        /// <returns>
        /// Returns if Deleting was successfully
        /// </returns>
        public static bool DeleteUser(int userID, bool isBotAutoDelete = false)
        {
            var userName = GetUserNameFromID(userID);

            if (userName.IsNotSet())
            {
                return false;
            }

            // Delete the images/albums both from database and physically.
            var uploadFolderPath =
                HttpContext.Current.Server.MapPath(
                    string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

            using (var dt = LegacyDb.album_list(userID, null))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    YafAlbum.Album_Image_Delete(uploadFolderPath, dr["AlbumID"], userID, null);
                }
            }

            // Check if there are any avatar images in the uploads folder
            if (!YafContext.Current.Get<YafBoardSettings>().UseFileTable
                && YafContext.Current.Get<YafBoardSettings>().AvatarUpload)
            {
                string[] imageExtensions = { "jpg", "jpeg", "gif", "png", "bmp" };

                foreach (var extension in imageExtensions)
                {
                    if (File.Exists(Path.Combine(uploadFolderPath, "{0}.{1}".FormatWith(userID, extension))))
                    {
                        File.Delete(Path.Combine(uploadFolderPath, "{0}.{1}".FormatWith(userID, extension)));
                    }
                }
            }

            YafContext.Current.Get<MembershipProvider>().DeleteUser(userName, true);
            LegacyDb.user_delete(userID);
            YafContext.Current.Get<ILogger>()
                .Log(
                    YafContext.Current.PageUserID,
                    "UserMembershipHelper.DeleteUser",
                    "User {0} was deleted by {1}.".FormatWith(
                        userName,
                        isBotAutoDelete ? "the automatic spam check system" : YafContext.Current.PageUserName),
                    EventLogTypes.UserDeleted);

            // clear the cache
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersDisplayNameCollection);

            return true;
        }

        /// <summary>
        /// Deletes and ban's the user.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <param name="user">The MemberShip User.</param>
        /// <param name="userIpAddress">The user's IP address.</param>
        /// <returns>
        /// Returns if Deleting was successfully
        /// </returns>
        public static bool DeleteAndBanUser(int userID, MembershipUser user, string userIpAddress)
        {
            // Ban IP ?
            if (YafContext.Current.Get<YafBoardSettings>().BanBotIpOnDetection)
            {
                YafContext.Current.GetRepository<BannedIP>()
                    .Save(
                        null,
                        userIpAddress,
                        "A spam Bot who was trying to register was banned by IP {0}".FormatWith(userIpAddress),
                        userID);

                // Clear cache
                YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.BannedIP);

                if (YafContext.Current.Get<YafBoardSettings>().LogBannedIP)
                {
                    YafContext.Current.Get<ILogger>()
                        .Log(
                            userID,
                            "IP BAN of Bot",
                            "A spam Bot who was banned by IP {0}".FormatWith(userIpAddress),
                            EventLogTypes.IpBanSet);
                }
            }

            // Ban Name ?
            YafContext.Current.GetRepository<BannedName>()
                .Save(null, user.UserName, "Name was reported by the automatic spam system.");

            // Ban User Email?
            YafContext.Current.GetRepository<BannedEmail>()
                .Save(null, user.Email, "Email was reported by the automatic spam system.");

            // Delete the images/albums both from database and physically.
            var uploadDir =
                HttpContext.Current.Server.MapPath(
                    string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

            using (var dt = LegacyDb.album_list(userID, null))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    YafAlbum.Album_Image_Delete(uploadDir, dr["AlbumID"], userID, null);
                }
            }

            // delete posts...
            var messageIds =
                (from m in LegacyDb.post_alluser_simple(
                               YafContext.Current.PageBoardID,
                               userID).AsEnumerable()
                 select m.Field<int>("MessageID")).Distinct().ToList();

            messageIds.ForEach(x => LegacyDb.message_delete(x, true, string.Empty, 1, true));

            YafContext.Current.Get<MembershipProvider>().DeleteUser(user.UserName, true);
            LegacyDb.user_delete(userID);
            YafContext.Current.Get<ILogger>()
                .Log(
                    YafContext.Current.PageUserID,
                    "UserMembershipHelper.DeleteUser",
                    "User {0} was deleted by the automatic spam check system.".FormatWith(user.UserName),
                    EventLogTypes.UserDeleted);

            // clear the cache
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
            YafContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersDisplayNameCollection);

            return true;
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
            var muc = YafContext.Current.Get<MembershipProvider>()
                .GetAllUsers(pageCount, 1000, out totalRecords);
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
            var providerUserKey = GetProviderUserKeyFromID(userID);

            return providerUserKey != null ? GetMembershipUserByKey(providerUserKey) : null;
        }

        /// <summary>
        /// get the membership user from the userID
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="boardId">The board identifier.</param>
        /// <returns>
        /// The get membership user by id.
        /// </returns>

        public static MembershipUser GetMembershipUserById(int userID, int boardId)
        {
            var providerUserKey = GetProviderUserKeyFromID(userID, boardId);

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
            return GetProviderUserKeyFromID(userID, YafContext.Current.PageBoardID);
        }

        /// <summary>
        /// Gets the user provider key from the UserID for a user
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="boardID">The board identifier.</param>
        /// <returns>
        /// The get provider user key from id.
        /// </returns>
        public static object GetProviderUserKeyFromID(long userID, int? boardID)
        {
            object providerUserKey = null;

            var row = GetUserRowForID(userID, boardID);

            if (row == null)
            {
                return null;
            }

            if (row["ProviderUserKey"] != DBNull.Value)
            {
                providerUserKey = row["ProviderUserKey"];
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
            var userID = LegacyDb.user_get(YafContext.Current.PageBoardID, providerUserKey.ToString());
            return userID;
        }

        /// <summary>
        /// Gets the user name from the UserID
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>
        /// The get user name from id.
        /// </returns>
        public static string GetUserNameFromID(long userID)
        {
            var userName = string.Empty;

            var row = GetUserRowForID(userID, true);

            if (row == null)
            {
                return userName;
            }

            if (!row["Name"].IsNullOrEmptyDBField())
            {
                userName = row["Name"].ToString();
            }

            return userName;
        }

        /// <summary>
        /// Gets the user name from the UserID
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>
        /// The get user name from id.
        /// </returns>
        public static string GetDisplayNameFromID(long userID)
        {
            var displayName = string.Empty;

            var row = GetUserRowForID(userID, true);

            if (row == null)
            {
                return displayName;
            }

            if (!row["DisplayName"].IsNullOrEmptyDBField())
            {
                displayName = row["DisplayName"].ToString();
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
            return GetUserRowForID(userID, YafContext.Current.PageBoardID, allowCached);
        }

        /// <summary>
        /// Helper function that gets user data from the DB (or cache)
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="boardID">The board identifier.</param>
        /// <param name="allowCached">if set to <c>true</c> [allow cached].</param>
        /// <returns>
        /// The get user row for id.
        /// </returns>
        public static DataRow GetUserRowForID(long userID, int? boardID, bool allowCached)
        {
            if (!boardID.HasValue)
            {
                boardID = YafContext.Current.PageBoardID;
            }

            if (!allowCached)
            {
                return LegacyDb.user_list(boardID, userID, DBNull.Value).GetFirstRow();
            }

            // get the item cached...
            return
                YafContext.Current.Get<IDataCache>()
                    .GetOrSet(
                        Constants.Cache.UserListForID.FormatWith(userID),
                        () => LegacyDb.user_list(boardID, userID, DBNull.Value),
                        TimeSpan.FromMinutes(5))
                    .GetFirstRow();
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
        /// Default allows the user row to be cached (mostly used for Provider key and UserID which never change)
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="boardID">The board identifier.</param>
        /// <returns>
        /// The get user row for id.
        /// </returns>
        public static DataRow GetUserRowForID(long userID, int? boardID)
        {
            return GetUserRowForID(userID, boardID, true);
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
        /// Simply tells you if the User ID passed is the Guest user
        /// for the current board
        /// </summary>
        /// <param name="userID">
        /// ID of user to lookup
        /// </param>
        /// <returns>
        /// true if the user id is a guest user
        /// </returns>
        public static bool IsGuestUser(object userID)
        {
            return userID == null || userID is DBNull || IsGuestUser((int)userID);
        }

        /// <summary>
        /// Simply tells you if the user ID passed is the Guest user
        /// for the current board
        /// </summary>
        /// <param name="userID">
        /// ID of user to lookup
        /// </param>
        /// <returns>
        /// true if the user id is a guest user
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
            var providerUserKey = GetProviderUserKeyFromID(userID);

            if (providerUserKey == null)
            {
                return false;
            }

            var user = GetUser(ObjectExtensions.ConvertObjectToType(providerUserKey, Config.ProviderKeyType));

            user.Email = newEmail;

            YafContext.Current.Get<MembershipProvider>().UpdateUser(user);

            LegacyDb.user_aspnet(
                YafContext.Current.PageBoardID,
                user.UserName,
                null,
                newEmail,
                user.ProviderUserKey,
                user.IsApproved);

            return true;
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
            var exists = false;

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