/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.UsersRoles
{
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Security;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    /// <summary>
    /// This is a stop-gap class to help with syncing operations
    /// with users/membership.
    /// </summary>
    public static class UserMembershipHelper
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
                int? guestUserID = BoardContext.Current.Get<IDataCache>().GetOrSet(
                    Constants.Cache.GuestUserID,
                    () =>
                        {
                            // get the guest user for this board...
                            guestUserID =
                                BoardContext.Current.GetRepository<User>().GetGuestUserId(BoardContext.Current.PageBoardID)
                                ?? BoardContext.Current.GetRepository<User>()
                                    .GetGuestUserId(BoardContext.Current.PageBoardID);

                            if (!guestUserID.HasValue)
                            {
                                // failure...
                                throw new NoValidGuestUserForBoardException(
                                    $"Could not locate the guest user for the board id {BoardContext.Current.PageBoardID}. You might have deleted the guest group or removed the guest user.");
                            }

                            return guestUserID.Value;
                        });

                return guestUserID ?? -1;
            }
        }

        /// <summary>
        /// Gets the Username of the Guest user for the current board.
        /// </summary>
        public static string GuestUserName =>
            BoardContext.Current.GetRepository<User>().ListAsDataTable(BoardContext.Current.PageBoardID, GuestUserId, true)
                .GetFirstRowColumnAsValue<string>("Name", null);

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
                allUsers.Cast<MembershipUser>().Where(user => !user.IsApproved).ForEach(
                    user =>
                        {
                            // approve this user...
                            user.IsApproved = true;
                            BoardContext.Current.Get<MembershipProvider>().UpdateUser(user);
                            var id = GetUserIDFromProviderUserKey(user.ProviderUserKey);
                            if (id > 0)
                            {
                                BoardContext.Current.GetRepository<User>().Approve(id);
                            }
                        });

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

            BoardContext.Current.Get<MembershipProvider>().UpdateUser(user);
            BoardContext.Current.GetRepository<User>().Approve(userID);

            return true;
        }

        /// <summary>
        /// Deletes all Unapproved Users older then Cut Off DateTime
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
                allUsers.Cast<MembershipUser>().Where(user => !user.IsApproved && user.CreationDate < createdCutoff)
                    .ForEach(
                        user =>
                            {
                                // delete this user...
                                BoardContext.Current.GetRepository<User>()
                                    .Delete(GetUserIDFromProviderUserKey(user.ProviderUserKey));
                                BoardContext.Current.Get<MembershipProvider>().DeleteUser(user.UserName, true);

                                if (BoardContext.Current.Get<BoardSettings>().LogUserDeleted)
                                {
                                    BoardContext.Current.Get<ILogger>().Log(
                                        BoardContext.Current.PageUserID,
                                        "UserMembershipHelper.DeleteAllUnapproved",
                                        $"User {user.UserName} was deleted by user id {BoardContext.Current.PageUserID} as unapproved.",
                                        EventLogTypes.UserDeleted);
                                }
                            });

                pageCount++;
            }
        }

        /// <summary>
        /// De-active all User accounts which are not active for x years
        /// </summary>
        /// <param name="createdCutoff">
        /// The created cutoff.
        /// </param>
        public static void LockInactiveAccounts(DateTime createdCutoff)
        {
            var allUsers = GetAllUsers();

            // iterate through each one...
            allUsers.Cast<MembershipUser>().Where(user => !user.IsApproved && user.LastActivityDate < createdCutoff)
                .ForEach(
                    user =>
                        {
                            // Set user to un-approve...
                            user.IsApproved = false;

                            BoardContext.Current.Get<MembershipProvider>().UpdateUser(user);
                        });
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
            var uploadFolderPath = HttpContext.Current.Server.MapPath(
                string.Concat(BaseUrlBuilder.ServerFileRoot, BoardFolders.Current.Uploads));

            var dt = BoardContext.Current.GetRepository<UserAlbum>().ListByUser(userID);

            dt.ForEach(dr => BoardContext.Current.Get<IAlbum>().AlbumImageDelete(uploadFolderPath, dr.ID, userID, null));

            // Check if there are any avatar images in the uploads folder
            if (!BoardContext.Current.Get<BoardSettings>().UseFileTable
                && BoardContext.Current.Get<BoardSettings>().AvatarUpload)
            {
                string[] imageExtensions = { "jpg", "jpeg", "gif", "png", "bmp" };

                imageExtensions.ForEach(
                    extension =>
                        {
                            if (File.Exists(Path.Combine(uploadFolderPath, $"{userID}.{extension}")))
                            {
                                File.Delete(Path.Combine(uploadFolderPath, $"{userID}.{extension}"));
                            }
                        });
            }

            BoardContext.Current.Get<MembershipProvider>().DeleteUser(userName, true);
            BoardContext.Current.GetRepository<User>().Delete(userID);

            if (BoardContext.Current.Get<BoardSettings>().LogUserDeleted)
            {
                BoardContext.Current.Get<ILogger>().Log(
                    BoardContext.Current.PageUserID,
                    "UserMembershipHelper.DeleteUser",
                    $"User {userName} was deleted by {(isBotAutoDelete ? "the automatic spam check system" : BoardContext.Current.PageUserName)}.",
                    EventLogTypes.UserDeleted);
            }

            // clear the cache
            BoardContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
            BoardContext.Current.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
            BoardContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersDisplayNameCollection);

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
            // Update Anti SPAM Stats
            BoardContext.Current.GetRepository<Registry>().IncrementBannedUsers();

            // Ban IP ?
            if (BoardContext.Current.Get<BoardSettings>().BanBotIpOnDetection)
            {
                BoardContext.Current.GetRepository<BannedIP>().Save(
                    null,
                    userIpAddress,
                    $"A spam Bot who was trying to register was banned by IP {userIpAddress}",
                    userID);

                // Clear cache
                BoardContext.Current.Get<IDataCache>().Remove(Constants.Cache.BannedIP);

                if (BoardContext.Current.Get<BoardSettings>().LogBannedIP)
                {
                    BoardContext.Current.Get<ILogger>().Log(
                        userID,
                        "IP BAN of Bot",
                        $"A spam Bot who was banned by IP {userIpAddress}",
                        EventLogTypes.IpBanSet);
                }
            }

            // Ban Name ?
            BoardContext.Current.GetRepository<BannedName>().Save(
                null,
                user.UserName,
                "Name was reported by the automatic spam system.");

            // Ban User Email?
            BoardContext.Current.GetRepository<BannedEmail>().Save(
                null,
                user.Email,
                "Email was reported by the automatic spam system.");

            // Delete the images/albums both from database and physically.
            var uploadDir = HttpContext.Current.Server.MapPath(
                string.Concat(BaseUrlBuilder.ServerFileRoot, BoardFolders.Current.Uploads));

            var dt = BoardContext.Current.GetRepository<UserAlbum>().ListByUser(userID);

            dt.ForEach(dr => BoardContext.Current.Get<IAlbum>().AlbumImageDelete(uploadDir, dr.ID, userID, null));

            // delete posts...
            var messageIds = BoardContext.Current.GetRepository<Message>().GetAllUserMessages(userID).Select(m => m.ID)
                .Distinct().ToList();

            messageIds.ForEach(x => BoardContext.Current.GetRepository<Message>().Delete(x, true, string.Empty, 1, true));

            BoardContext.Current.Get<MembershipProvider>().DeleteUser(user.UserName, true);
            BoardContext.Current.GetRepository<User>().Delete(userID);

            if (BoardContext.Current.Get<BoardSettings>().LogUserDeleted)
            {
                BoardContext.Current.Get<ILogger>().Log(
                    BoardContext.Current.PageUserID,
                    "UserMembershipHelper.DeleteUser",
                    $"User {user.UserName} was deleted by the automatic spam check system.",
                    EventLogTypes.UserDeleted);
            }

            // clear the cache
            BoardContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
            BoardContext.Current.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
            BoardContext.Current.Get<IDataCache>().Remove(Constants.Cache.UsersDisplayNameCollection);

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
            return BoardContext.Current.Get<MembershipProvider>().FindUsersByEmail(email, 0, 999999, out _);
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
            return BoardContext.Current.Get<MembershipProvider>().FindUsersByName(username, 0, 999999, out _);
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
            var muc = BoardContext.Current.Get<MembershipProvider>().GetAllUsers(pageCount, 1000, out var totalRecords);
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
            return GetAllUsers(0, out _, 9999);
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
            return GetProviderUserKeyFromID(userID, BoardContext.Current.PageBoardID);
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
            return BoardContext.Current.Get<MembershipProvider>().GetUser(username, updateOnlineStatus);
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
            return BoardContext.Current.Get<MembershipProvider>().GetUser(providerKey, false);
        }

        /// <summary>
        /// Method returns Application Name
        /// </summary>
        /// <returns>
        /// Returns Application Name
        /// </returns>
        public static string ApplicationName()
        {
            return BoardContext.Current.Get<MembershipProvider>().ApplicationName;
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
            return BoardContext.Current.Get<MembershipProvider>().GetUser(providerKey, updateOnlineStatus);
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
            return BoardContext.Current.Get<HttpContextBase>().User != null
                   && BoardContext.Current.Get<HttpContextBase>().User.Identity.IsAuthenticated
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
            return BoardContext.Current.GetRepository<User>().GetUserId(
                BoardContext.Current.PageBoardID,
                providerUserKey.ToString());
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
            return GetUserRowForID(userID, BoardContext.Current.PageBoardID, allowCached);
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
            var boardId = boardID ?? BoardContext.Current.PageBoardID;

            if (!allowCached)
            {
                return BoardContext.Current.GetRepository<User>()
                    .ListAsDataTable(boardId, userID.ToType<int>(), DBNull.Value).GetFirstRow();
            }

            // get the item cached...
            return BoardContext.Current.Get<IDataCache>().GetOrSet(
                string.Format(Constants.Cache.UserListForID, userID),
                () => BoardContext.Current.GetRepository<User>()
                    .ListAsDataTable(boardId, userID.ToType<int>(), DBNull.Value),
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

            BoardContext.Current.Get<MembershipProvider>().UpdateUser(user);

            BoardContext.Current.GetRepository<User>().Aspnet(
                BoardContext.Current.PageBoardID,
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