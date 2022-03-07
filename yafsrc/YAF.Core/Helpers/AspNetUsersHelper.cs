/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;

    using ServiceStack.OrmLite;

    using System.Linq.Expressions;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Identity;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;
    using YAF.Types.Objects.Model;

    using Constants = YAF.Types.Constants.Constants;

    /// <summary>
    /// This is a stop-gap class to help with syncing operations
    /// with users/membership.
    /// </summary>
    public class AspNetUsersHelper : IAspNetUsersHelper, IHaveServiceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetUsersHelper"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public AspNetUsersHelper([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; protected set; }

        /// <summary>
        /// The users.
        /// </summary>
        public IQueryable<AspNetUsers> Users => this.Get<AspNetUsersManager>().AspNetUsers;

        /// <summary>Used to hash/verify passwords</summary>et
        public IPasswordHasher IPasswordHasher => this.Get<AspNetUsersManager>().PasswordHasher;

        /// <summary>
        /// Gets the guest user id for the current board.
        /// </summary>
        /// <exception cref="NoValidGuestUserForBoardException">No Valid Guest User Exception</exception>
        public int GuestUserId
        {
            get
            {
                var guestUserID = this.Get<IDataCache>().GetOrSet(
                    Constants.Cache.GuestUserID,
                    () =>
                    {
                        // get the guest user for this board...
                        var guestUser = this.GetRepository<User>().GetGuestUser(BoardContext.Current.PageBoardID);

                        if (guestUser == null)
                        {
                            // failure...
                            throw new NoValidGuestUserForBoardException(
                                $@"Could not locate the guest user for the board id {BoardContext.Current.PageBoardID}. 
                                          You might have deleted the guest group or removed the guest user.");
                        }

                        return guestUser.ID;
                    });

                return guestUserID;
            }
        }

        /// <summary>
        /// Gets the Username of the Guest user for the current board.
        /// </summary>
        public string GuestUserName =>
            this.GetRepository<User>().GetById(BoardContext.Current.GuestUserID).Name;

        #endregion

        #region Public Methods

        /// <summary>
        /// For the admin function: approve all users. Approves all
        /// users waiting for approval
        /// </summary>
        public void ApproveAll()
        {
            // get all users...
            var allUsers = this.Get<IAspNetUsersHelper>().GetAllUsers();

            // iterate through each one...
            allUsers.Where(user => !user.IsApproved).ForEach(
                user =>
                {
                    // approve this user...
                    user.IsApproved = true;
                    this.Get<AspNetUsersManager>().Update(user);
                    var checkUser  = this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(user.Id);

                    var id = checkUser?.ID ?? 0;

                    if (id <= 0)
                    {
                        return;
                    }

                    this.GetRepository<User>().Approve(checkUser);

                    var checkEmail = this.GetRepository<CheckEmail>().GetSingle(m => m.UserID == id);

                    if (checkEmail != null)
                    {
                        this.GetRepository<CheckEmail>().DeleteById(checkEmail.ID);
                    }
                });
        }

        /// <summary>
        /// Approves the user.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <returns>
        /// The approve user.
        /// </returns>
        public bool ApproveUser(int userID)
        {
            var yafUser = this.GetRepository<User>().GetById(userID);

            if (yafUser?.ProviderUserKey == null)
            {
                return false;
            }

            var user = this.Get<IAspNetUsersHelper>().GetUser(yafUser.ProviderUserKey);

            if (!user.IsApproved)
            {
                user.IsApproved = true;
            }

            this.Get<AspNetUsersManager>().Update(user);

            this.GetRepository<User>().Approve(yafUser);

            var checkEmail = this.GetRepository<CheckEmail>().GetSingle(m => m.UserID == userID);

            if (checkEmail != null)
            {
                this.GetRepository<CheckEmail>().DeleteById(checkEmail.ID);
            }

            return true;
        }

        /// <summary>
        /// Deletes all Unapproved Users older then Cut Off DateTime
        /// </summary>
        /// <param name="createdCutoff">
        /// The created cutoff.
        /// </param>
        public void DeleteAllUnapproved(DateTime createdCutoff)
        {
            // get all users...
            var allUsers = this.Get<IAspNetUsersHelper>().GetAllUsers();

            // iterate through each one...
            allUsers.Where(user => !user.IsApproved && user.CreateDate < createdCutoff).ForEach(
                user =>
                {
                    // delete this user...
                    this.GetRepository<User>().Delete(this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(user.Id).ID);
                    this.Get<AspNetUsersManager>().Delete(user);

                    if (this.Get<BoardSettings>().LogUserDeleted)
                    {
                        this.Get<ILoggerService>().UserDeleted(
                            BoardContext.Current.PageUser.ID,
                            $"User {user.UserName} was deleted by user id {BoardContext.Current.PageUserID} as unapproved.");
                    }
                });
        }

        /// <summary>
        /// De-active all User accounts which are not active for x years
        /// </summary>
        /// <param name="createdCutoff">
        /// The created cutoff.
        /// </param>
        public void LockInactiveAccounts(DateTime createdCutoff)
        {
            var allUsers = this.Get<IAspNetUsersHelper>().GetAllUsers();

            // iterate through each one...
            allUsers.Where(user => !user.IsApproved && user.LastActivityDate < createdCutoff).ForEach(
                user =>
                {
                    // Set user to un-approve...
                    user.IsApproved = false;

                    this.Get<AspNetUsersManager>().Update(user);
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
        public bool DeleteUser(int userID, bool isBotAutoDelete = false)
        {
            var user = this.GetRepository<User>().GetById(userID);

            if (user == null)
            {
                return false;
            }

            // Delete the images/albums both from database and physically.
            var uploadFolderPath = HttpContext.Current.Server.MapPath(
                string.Concat(BaseUrlBuilder.ServerFileRoot, this.Get<BoardFolders>().Uploads));

            var dt = this.GetRepository<UserAlbum>().ListByUser(userID);

            dt.ForEach(dr => this.Get<IAlbum>().AlbumImageDelete(uploadFolderPath, dr.ID, userID, null));

            // Check if there are any avatar images in the uploads folder
            if (!this.Get<BoardSettings>().UseFileTable && this.Get<BoardSettings>().AvatarUpload)
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

            var aspNetUser = this.Get<IAspNetUsersHelper>().GetUser(user.ProviderUserKey);

            if (aspNetUser != null)
            {
                this.Get<AspNetUsersManager>().Delete(aspNetUser);
            }

            this.GetRepository<User>().Delete(userID);

            if (this.Get<BoardSettings>().LogUserDeleted)
            {
                this.Get<ILoggerService>().UserDeleted(
                    BoardContext.Current.PageUser.ID,
                    $"User {user.DisplayOrUserName()} was deleted by {(isBotAutoDelete ? "the automatic spam check system" : BoardContext.Current.PageUser.DisplayOrUserName())}.");
            }

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
        public bool DeleteAndBanUser(int userID, AspNetUsers user, string userIpAddress)
        {
            // Update Anti SPAM Stats
            this.GetRepository<Registry>().IncrementBannedUsers();

            // Ban IP ?
            if (this.Get<BoardSettings>().BanBotIpOnDetection)
            {
                this.Get<IRaiseEvent>().Raise(new BanUserEvent(userID, user.UserName, user.Email, userIpAddress));
            }

            // Delete the images/albums both from database and physically.
            var uploadDir = HttpContext.Current.Server.MapPath(
                string.Concat(BaseUrlBuilder.ServerFileRoot, this.Get<BoardFolders>().Uploads));

            var dt = this.GetRepository<UserAlbum>().ListByUser(userID);

            dt.ForEach(dr => this.Get<IAlbum>().AlbumImageDelete(uploadDir, dr.ID, userID, null));

            // delete posts...
            var messages = this.GetRepository<Message>().GetAllUserMessages(userID).Distinct()
                .ToList();

            messages.ForEach(
                x => this.GetRepository<Message>().Delete(
                    x.Topic.ForumID,
                    x.TopicID,
                    x.ID,
                    true,
                    string.Empty,
                    true,
                    true));

            this.Get<AspNetUsersManager>().Delete(user);
            this.GetRepository<User>().Delete(userID);

            if (this.Get<BoardSettings>().LogUserDeleted)
            {
                this.Get<ILoggerService>().UserDeleted(
                    BoardContext.Current.PageUser.ID,
                    $"User {user.UserName} was deleted by the automatic spam check system.");
            }

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
        public IQueryable<AspNetUsers> FindUsersByEmail(string email)
        {
            return this.Get<AspNetUsersManager>().AspNetUsers.Where(u => u.Email == email);
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
        public IQueryable<AspNetUsers> FindUsersByName(string username)
        {
            return this.Get<AspNetUsersManager>().AspNetUsers.Where(u => u.UserName == username);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>
        /// The get all users.
        /// </returns>
        public IQueryable<AspNetUsers> GetAllUsers()
        {
            return this.Get<AspNetUsersManager>().AspNetUsers;
        }

        /// <summary>
        /// get the membership user from the userID
        /// </summary>
        /// <param name="userID">
        /// The user identifier.
        /// </param>
        /// <returns>
        /// The get membership user by id.
        /// </returns>
        public AspNetUsers GetMembershipUserById(int userID)
        {
            var providerUserKey = this.Get<IAspNetUsersHelper>().GetUserProviderKeyFromUserID(userID);

            return providerUserKey != null
                ? this.Get<IAspNetUsersHelper>().GetUser(providerUserKey)
                : null;
        }

        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <returns>
        /// Returns MembershipUser
        /// </returns>
        public AspNetUsers GetUser()
        {
            return this.Get<HttpContextBase>().User != null && this.Get<HttpContextBase>().User.Identity.IsAuthenticated
                ? this.Get<IAspNetUsersHelper>().GetUserByName(HttpContext.Current.User.Identity.Name)
                : null;
        }

        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        /// Returns MembershipUser
        /// </returns>
        public AspNetUsers GetUserByName(string username)
        {
            return this.Get<AspNetUsersManager>().FindByName(username);
        }

        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="AspNetUsers"/>.
        /// </returns>
        public AspNetUsers GetUserByEmail(string email)
        {
            return this.Get<AspNetUsersManager>().FindByEmail(email);
        }

        /// <summary>
        /// Method returns MembershipUser
        /// </summary>
        /// <param name="providerKey">The provider key.</param>
        /// <returns>
        /// Returns MembershipUser
        /// </returns>
        public AspNetUsers GetUser(object providerKey)
        {
            return this.Get<AspNetUsersManager>().FindById(providerKey.ToString());
        }

        /// <summary>
        /// Get the User from the ProviderUserKey
        /// </summary>
        /// <param name="providerUserKey">The provider user key.</param>
        /// <param name="currentBoard">
        /// Get user from Current board, or all boards
        /// </param>
        /// <returns>
        /// The get user from provider user key.
        /// </returns>
        public User GetUserFromProviderUserKey(string providerUserKey, bool currentBoard = true)
        {
            return this.GetRepository<User>().GetUserByProviderKey(currentBoard ? BoardContext.Current.PageBoardID : null, providerUserKey);
        }

        /// <summary>
        /// Helper function that gets user data from the DB (or cache)
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The get user row for id.
        /// </returns>
        public string GetUserProviderKeyFromUserID(int userId)
        {
            return this.GetRepository<User>().GetById(userId).ProviderUserKey;
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
        public bool IsGuestUser(int userID)
        {
            return BoardContext.Current.GuestUserID == userID;
        }

        /// <summary>
        /// Helper function to update a user's email address.
        /// Syncs with both the YAF DB and Membership Provider.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="newEmail">
        /// The new email.
        /// </param>
        /// <returns>
        /// The update email.
        /// </returns>
        public bool UpdateEmail(AspNetUsers user, string newEmail)
        {
            user.Email = newEmail;

            this.Get<AspNetUsersManager>().Update(user);

            this.GetRepository<User>().AspNet(
                BoardContext.Current.PageBoardID,
                user.UserName,
                null,
                newEmail,
                user.Id,
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
        public bool UserExists(string userName, string email)
        {
            var exists = false;

            if (userName != null)
            {
                if (this.Get<IAspNetUsersHelper>().FindUsersByName(userName).Any())
                {
                    exists = true;
                }
            }
            else if (email != null && this.Get<IAspNetUsersHelper>().FindUsersByEmail(email).Any())
            {
                exists = true;
            }

            return exists;
        }

        /// <summary>
        /// The sign out.
        /// </summary>
        public void SignOut()
        {
            this.Get<IAuthenticationManager>().SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        /// <summary>
        /// The sign in.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="isPersistent">
        /// The is persistent.
        /// </param>
        public void SignIn(AspNetUsers user, bool isPersistent = true)
        {
            this.Get<IAuthenticationManager>().SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var userIdentity = this.Get<IAspNetUsersHelper>()
                .CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

            this.Get<IAuthenticationManager>().SignIn(
                new AuthenticationProperties { IsPersistent = isPersistent },
                userIdentity);

            user.AccessFailedCount = 0;
            user.LockoutEndDateUtc = null;

            this.Get<IAspNetUsersHelper>().Update(user);

            this.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(BoardContext.Current.PageUserID));
        }

        /// <summary>
        /// The sign in external.
        /// </summary>
        public void SignInExternal()
        {
            var loginInfo = this.Get<IAuthenticationManager>().GetExternalLoginInfo();

            var user = loginInfo.Login.LoginProvider.Equals("Twitter")
                ? this.Get<IAspNetUsersHelper>().GetUserByName(loginInfo.DefaultUserName)
                : this.Get<IAspNetUsersHelper>().GetUserByEmail(loginInfo.Email);

            this.Get<IAspNetUsersHelper>().AddLogin(user.Id, loginInfo.Login);

            // Login
            this.Get<IAspNetUsersHelper>().SignIn(user);

            this.Get<LinkBuilder>().Redirect(ForumPages.Board);
        }

        /// <summary>
        /// The remove from role.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        /// <returns>
        /// The <see cref="IdentityResult"/>.
        /// </returns>
        public IdentityResult RemoveFromRole(string userId, string role)
        {
            return this.Get<AspNetUsersManager>().RemoveFromRole(userId, role);
        }

        /// <summary>
        /// Creates a ClaimsIdentity representing the user
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="authenticationType">
        /// The authentication Type.
        /// </param>
        /// <returns>
        /// The <see cref="ClaimsIdentity"/>.
        /// </returns>
        public ClaimsIdentity CreateIdentity(AspNetUsers user, string authenticationType)
        {
            return this.Get<AspNetUsersManager>().CreateIdentity(user, authenticationType);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="IdentityResult"/>.
        /// </returns>
        public IdentityResult Delete(AspNetUsers user)
        {
            return this.Get<AspNetUsersManager>().Delete(user);
        }

        /// <summary>
        /// Returns the roles for the user
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public IList<string> GetRoles(string userId)
        {
            return this.Get<AspNetUsersManager>().GetRoles(userId);
        }

        /// <summary>
        /// Returns true if the user is in the specified role
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="role">
        /// The role.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsInRole(AspNetUsers user, string role)
        {
            return this.Get<AspNetUsersManager>().IsInRole(user.Id, role);
        }

        /// <summary>
        /// Add a user to a role
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="roleName">
        /// The role Name.
        /// </param>
        public void AddToRole(AspNetUsers user, string roleName)
        {
            this.Get<AspNetUsersManager>().AddToRole(user.Id, roleName);
        }

        /// <summary>
        /// Create a user with the given password
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="IdentityResult"/>.
        /// </returns>
        public IdentityResult Create(AspNetUsers user, string password)
        {
            return this.Get<AspNetUsersManager>().Create(user, password);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="IdentityResult"/>.
        /// </returns>
        public IdentityResult Update(AspNetUsers user)
        {
            return this.Get<AspNetUsersManager>().Update(user);
        }

        /// <summary>
        /// Confirm the user's email with confirmation token
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="IdentityResult"/>.
        /// </returns>
        public IdentityResult ConfirmEmail(string userId, string token)
        {
            return this.Get<AspNetUsersManager>().ConfirmEmail(userId, token);
        }

        /// <summary>
        /// Generate a password reset token for the user using the UserTokenProvider
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GeneratePasswordResetToken(string userId)
        {
            return this.Get<AspNetUsersManager>().GeneratePasswordResetToken(userId);
        }

        /// <summary>
        /// Reset a user's password using a reset password token
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="newPassword">
        /// The new Password.
        /// </param>
        /// <returns>
        /// The <see cref="IdentityResult"/>.
        /// </returns>
        public IdentityResult ResetPassword(string userId, string token, string newPassword)
        {
            return this.Get<AspNetUsersManager>().ResetPassword(userId, token, newPassword);
        }

        /// <summary>
        /// Get the email confirmation token for the user
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GenerateEmailConfirmationResetToken(string userId)
        {
            return this.Get<AspNetUsersManager>().GenerateEmailConfirmationToken(userId);
        }

        /// <summary>
        /// Change a user password
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="currentPassword">
        /// The current Password.
        /// </param>
        /// <param name="newPassword">
        /// The new Password.
        /// </param>
        /// <returns>
        /// The <see cref="IdentityResult"/>.
        /// </returns>
        public IdentityResult ChangePassword(string userId, string currentPassword, string newPassword)
        {
            return this.Get<AspNetUsersManager>().ChangePassword(userId, currentPassword, newPassword);
        }

        /// <summary>
        /// The add login.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="login">
        /// The login.
        /// </param>
        public void AddLogin(string userId, UserLoginInfo login)
        {
            CodeContracts.VerifyNotNull(userId);
            CodeContracts.VerifyNotNull(login);

            if (this.GetRepository<AspNetUserLogins>().GetSingle(l => l.UserId == userId) != null)
            {
                return;
            }

            var userLogin = new AspNetUserLogins
            {
                UserId = userId, ProviderKey = login.ProviderKey, LoginProvider = login.LoginProvider
            };

            this.GetRepository<AspNetUserLogins>().Insert(userLogin);
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="userName">
        /// The user Name.
        /// </param>
        /// <returns>
        /// The <see cref="AspNetUsers"/>.
        /// </returns>
        public AspNetUsers ValidateUser(string userName)
        {
            if (userName.Contains("@"))
            {
                // attempt Email Login
                var realUser = BoardContext.Current.Get<IAspNetUsersHelper>().GetUserByEmail(userName);

                if (realUser != null)
                {
                    return realUser;
                }
            }

            var user = this.Get<IAspNetUsersHelper>().GetUserByName(userName);

            // Standard user name login
            if (user != null)
            {
                return user;
            }

            // display name login...
            if (!this.Get<BoardSettings>().EnableDisplayName)
            {
                return null;
            }

            // Display name login
            var realUsername = this.GetRepository<User>().GetSingle(u => u.DisplayName == userName);

            if (realUsername == null)
            {
                return null;
            }

            // get the username associated with this id...
            user = this.Get<IAspNetUsersHelper>().GetUser(realUsername.ProviderUserKey);

            // validate again...
            return user;

            // no valid login -- return null
        }

        /// <summary>
        /// Gets the board user by Id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="includeNonApproved">
        /// Include not approved user
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        public Tuple<User, AspNetUsers, Rank, vaccess> GetBoardUser(
            [NotNull] int userId,
            [CanBeNull] int? boardId = null,
            bool includeNonApproved = false)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            expression.LeftJoin<vaccess>((u, v) => v.UserID == u.ID).LeftJoin<AspNetUsers>((u, a) => a.Id == u.ProviderUserKey)
                .Join<Rank>((u, r) => r.ID == u.RankID);

            if (includeNonApproved)
            {
                expression.Where<vaccess, User>(
                    (v, u) => u.ID == userId && u.BoardID == (boardId ?? this.GetRepository<User>().BoardID));
            }
            else
            {
                expression.Where<vaccess, User>(
                    (v, u) => u.ID == userId && u.BoardID == (boardId ?? this.GetRepository<User>().BoardID) &&
                              (u.Flags & 2) == 2);
            }

            return this.GetRepository<User>().DbAccess
                .Execute(db => db.Connection.SelectMulti<User, AspNetUsers, Rank, vaccess>(expression))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the users paged.
        /// </summary>
        /// <param name="boardId">The board identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <param name="joinedDate">The joined date.</param>
        /// <param name="onlySuspended">if set to <c>true</c> [only suspended].</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="rankId">The rank identifier.</param>
        /// <returns>
        /// Returns the board users.
        /// </returns>
        public List<PagedUser> GetUsersPaged(
            [NotNull] int? boardId,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [CanBeNull] string name,
            [CanBeNull] string email,
            [CanBeNull] DateTime? joinedDate,
            [NotNull] bool onlySuspended,
            [CanBeNull] int? groupId,
            [CanBeNull] int? rankId)
        {
            return this.GetRepository<User>().DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    Expression<Func<User, bool>> whereCriteria = u => u.BoardID == (boardId ?? this.GetRepository<User>().BoardID);

                    // -- count total
                    var countTotalExpression = db.Connection.From<User>();

                    expression.Join<AspNetUsers>((u, a) => a.Id == u.ProviderUserKey)
                        .Join<Rank>((u, r) => r.ID == u.RankID);

                    countTotalExpression.Where(whereCriteria);

                    expression.Where(whereCriteria);

                    // filter by name
                    if (name.IsSet())
                    {
                        countTotalExpression.And<User>(u => u.Name.Contains(name) || u.DisplayName.Contains(name));

                        expression.And<User>(u => u.Name.Contains(name) || u.DisplayName.Contains(name));
                    }

                    // filter by email
                    if (email.IsSet())
                    {
                        countTotalExpression.And<User>(u => u.Email.Contains(email));

                        expression.And<User>(u => u.Email.Contains(email));
                    }

                    // filter by date of registration
                    if (joinedDate.HasValue)
                    {
                        countTotalExpression.And<User>(u => u.Joined > joinedDate.Value);

                        expression.And<User>(u => u.Joined > joinedDate.Value);
                    }

                    // show only suspended ?
                    if (onlySuspended)
                    {
                        countTotalExpression.And<User>(u => u.Suspended != null);

                        expression.And<User>(u => u.Suspended != null);
                    }

                    // filter by rank
                    if (rankId.HasValue)
                    {
                        countTotalExpression.And<User>(u => u.RankID == rankId.Value);

                        expression.And<User>(u => u.RankID == rankId.Value);
                    }

                    // filter by group
                    if (groupId.HasValue)
                    {
                        countTotalExpression.UnsafeAnd(
                            $@"exists(select 1 from {countTotalExpression.Table<UserGroup>()} x 
                                               where x.{countTotalExpression.Column<UserGroup>(x => x.UserID)} = {countTotalExpression.Column<User>(x => x.ID, true)} 
                                               and x.{countTotalExpression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})");

                        expression.UnsafeAnd(
                            $@"exists(select 1 from {expression.Table<UserGroup>()} x 
                                               where x.{expression.Column<UserGroup>(x => x.UserID)} = {expression.Column<User>(x => x.ID, true)} 
                                               and x.{expression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})");
                    }

                    var countTotalSql = countTotalExpression
                        .Select(Sql.Count($"{countTotalExpression.Column<User>(x => x.ID)}")).ToSelectStatement();

                    expression.Select<User, AspNetUsers, Rank>(
                        (u, a, r) => new {
                            UserID = u.ID,
                            u.Name,
                            u.DisplayName,
                            u.Flags,
                            u.Suspended,
                            u.UserStyle,
                            u.Avatar,
                            u.AvatarImage,
                            u.Email,
                            u.Joined,
                            u.LastVisit,
                            u.NumPosts,
                            IsGuest = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<User>(x => x.Flags, true)}&4")})"),
                            a.Profile_GoogleId,
                            a.Profile_FacebookId,
                            a.Profile_TwitterId,
                            RankName = r.Name,
                            TotalRows = Sql.Custom($"({countTotalSql})")
                        });

                    expression.OrderBy(u => u.Name);

                    // Set Paging
                    expression.Page(pageIndex + 1, pageSize);

                    return db.Connection.Select<PagedUser>(expression);
                });
        }

        /// <summary>
        /// List Members Paged
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <param name="rankId">
        /// The rank id.
        /// </param>
        /// <param name="startLetter">
        /// The start letter.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="sortName">
        /// The sort Name.
        /// </param>
        /// <param name="sortRank">
        /// The sort Rank.
        /// </param>
        /// <param name="sortJoined">
        /// The sort Joined.
        /// </param>
        /// <param name="sortPosts">
        /// The sort Posts.
        /// </param>
        /// <param name="sortLastVisit">
        /// The sort Last Visit.
        /// </param>
        /// <param name="numPosts">
        /// The number of Posts.
        /// </param>
        /// <param name="numPostCompare">
        /// The number of Post Compare.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PagedUser> ListMembersPaged(
            [CanBeNull] int? boardId,
            [CanBeNull] int? groupId,
            [CanBeNull] int? rankId,
            [NotNull] char startLetter,
            [CanBeNull] string name,
            [CanBeNull] int pageIndex,
            [CanBeNull] int pageSize,
            [CanBeNull] int? sortName,
            [CanBeNull] int? sortRank,
            [CanBeNull] int? sortJoined,
            [CanBeNull] int? sortPosts,
            [CanBeNull] int? sortLastVisit,
            [CanBeNull] int? numPosts,
            [NotNull] int numPostCompare)
        {
            return this.GetRepository<User>().DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    Expression<Func<User, bool>> whereCriteria = u => u.BoardID == (boardId ?? this.GetRepository<User>().BoardID) && (u.Flags & 2) == 2;

                    // -- count total
                    var countTotalExpression = db.Connection.From<User>();

                    expression.Join<AspNetUsers>((u, a) => a.Id == u.ProviderUserKey)
                        .Join<Rank>((u, r) => r.ID == u.RankID);

                    countTotalExpression.Where(whereCriteria);

                    expression.Where(whereCriteria);

                    if (startLetter == char.MinValue)
                    {
                        // filter by name
                        if (name.IsSet())
                        {
                            countTotalExpression.And<User>(u => u.Name.Contains(name) || u.DisplayName.Contains(name));

                            expression.And<User>(u => u.Name.Contains(name) || u.DisplayName.Contains(name));
                        }
                    }
                    else
                    {
                        countTotalExpression.And<User>(
                            u => u.Name.StartsWith(startLetter.ToString()) ||
                                 u.DisplayName.StartsWith(startLetter.ToString()));

                        expression.And<User>(u => u.Name.StartsWith(startLetter.ToString()) ||
                                                  u.DisplayName.StartsWith(startLetter.ToString()));
                    }

                    // Remove Guests
                    expression.And<User>(u => (u.Flags & 4) != 4);

                    // filter by posts
                    if (numPosts.HasValue)
                    {
                        switch (numPostCompare)
                        {
                            case 1:
                                countTotalExpression.And<User>(u => u.NumPosts == numPosts.Value);

                                expression.And<User>(u => u.NumPosts == numPosts.Value);
                                break;
                            case 2:
                                countTotalExpression.And<User>(u => u.NumPosts <= numPosts.Value);

                                expression.And<User>(u => u.NumPosts <= numPosts.Value);
                                break;
                            case 3:
                                countTotalExpression.And<User>(u => u.NumPosts >= numPosts.Value);

                                expression.And<User>(u => u.NumPosts >= numPosts.Value);
                                break;
                        }
                    }

                    // filter by rank
                    if (rankId.HasValue)
                    {
                        countTotalExpression.And<User>(u => u.RankID == rankId.Value);

                        expression.And<User>(u => u.RankID == rankId.Value);
                    }

                    // filter by group
                    if (groupId.HasValue)
                    {
                        countTotalExpression.UnsafeAnd(
                            $@"exists(select 1 from {countTotalExpression.Table<UserGroup>()} x 
                                               where x.{countTotalExpression.Column<UserGroup>(x => x.UserID)} = {countTotalExpression.Column<User>(x => x.ID, true)} 
                                               and x.{countTotalExpression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})");

                        expression.UnsafeAnd(
                            $@"exists(select 1 from {expression.Table<UserGroup>()} x 
                                               where x.{expression.Column<UserGroup>(x => x.UserID)} = {expression.Column<User>(x => x.ID, true)} 
                                               and x.{expression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})");
                    }

                    var countTotalSql = countTotalExpression
                        .Select(Sql.Count($"{countTotalExpression.Column<User>(x => x.ID)}")).ToSelectStatement();

                    expression.Select<User, AspNetUsers, Rank>(
                        (u, a, r) => new {
                            UserID = u.ID,
                            u.Name,
                            u.DisplayName,
                            u.Flags,
                            u.Suspended,
                            u.UserStyle,
                            u.Avatar,
                            u.AvatarImage,
                            u.Email,
                            u.Joined,
                            u.LastVisit,
                            u.NumPosts,
                            IsGuest = Sql.Custom<bool>($"({OrmLiteConfig.DialectProvider.ConvertFlag($"{expression.Column<User>(x => x.Flags, true)}&4")})"),
                            a.Profile_GoogleId,
                            a.Profile_FacebookId,
                            a.Profile_TwitterId,
                            RankName = r.Name,
                            TotalRows = Sql.Custom($"({countTotalSql})")
                        });

                    // Set Sorting
                    if (sortName.HasValue)
                    {
                        if (sortName.Value == 1)
                        {
                            expression.OrderBy(u => u.Name);
                        }
                        else
                        {
                            expression.OrderByDescending(u => u.Name);
                        }
                    }

                    if (sortRank is > 0)
                    {
                        if (sortRank.Value == 1)
                        {
                            expression.OrderBy<Rank>(r => r.Name);
                        }
                        else
                        {
                            expression.OrderByDescending<Rank>(r => r.Name);
                        }
                    }

                    if (sortJoined is > 0)
                    {
                        if (sortJoined.Value == 1)
                        {
                            expression.OrderBy(u => u.Joined);
                        }
                        else
                        {
                            expression.OrderByDescending(u => u.Joined);
                        }
                    }

                    if (sortLastVisit is > 0)
                    {
                        if (sortLastVisit.Value == 1)
                        {
                            expression.OrderBy(u => u.LastVisit);
                        }
                        else
                        {
                            expression.OrderByDescending(u => u.LastVisit);
                        }
                    }

                    if (sortPosts is > 0)
                    {
                        if (sortPosts.Value == 1)
                        {
                            expression.OrderBy(u => u.NumPosts);
                        }
                        else
                        {
                            expression.OrderByDescending(u => u.NumPosts);
                        }
                    }

                    // Set Paging
                    expression.Page(pageIndex + 1, pageSize);

                    return db.Connection.Select<PagedUser>(expression);
                });
        }

        #endregion
    }
}