/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Types.Interfaces.Identity;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using YAF.Types.Exceptions;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects.Model;

/// <summary>
/// The AspNetUsersHelper interface.
/// </summary>
public interface IAspNetUsersHelper
{
    /// <summary>
    /// Gets the users.
    /// </summary>
    IQueryable<AspNetUsers> Users { get; }

    /// <summary>Gets the hash/verify passwords</summary>
    IPasswordHasher<AspNetUsers> PasswordHasher { get; }

    /// <summary>
    /// Enable 2FA for the ASP net user
    /// </summary>
    /// <param name="aspNetUser">The ASP net user.</param>
    /// <param name="twoFactorKey">The two factor key.</param>
    /// <returns>Task.</returns>
    Task SetTwoFactorEnabledAsync(AspNetUsers aspNetUser, string twoFactorKey);

    /// <summary>
    /// Disable 2FA for the ASP net user
    /// </summary>
    /// <param name="aspNetUser">The ASP net user.</param>
    /// <param name="twoFactorKey">The two factor key.</param>
    /// <returns>Task.</returns>
    Task SetTwoFactorDisabledAsync(AspNetUsers aspNetUser, string twoFactorKey);

    /// <summary>
    /// Gets the guest user for the current board.
    /// </summary>
    /// <param name="boardId">
    ///     The board Id.
    /// </param>
    /// <exception cref="NoValidGuestUserForBoardException">
    /// No Valid Guest User Exception
    /// </exception>
    /// <returns>
    /// The <see cref="User"/>.
    /// </returns>
    User GuestUser(int boardId);

    /// <summary>
    /// Gets the guest user for the current board.
    /// </summary>
    /// <param name="boardId">
    ///     The board Id.
    /// </param>
    /// <exception cref="NoValidGuestUserForBoardException">
    /// No Valid Guest User Exception
    /// </exception>
    /// <returns>
    /// The <see cref="User"/>.
    /// </returns>
    Task<User> GuestUserAsync(int boardId);

    /// <summary>
    /// For the admin function: approve all users. Approves all
    /// users waiting for approval
    /// </summary>
    Task ApproveAllAsync();

    /// <summary>
    /// Approves the user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>
    /// The approve user.
    /// </returns>
    Task<bool> ApproveUserAsync(int userId);

    /// <summary>
    /// Deletes all Unapproved Users older then Cut Off DateTime
    /// </summary>
    /// <param name="createdCutoff">
    ///     The created cutoff.
    /// </param>
    Task DeleteAllUnapprovedAsync(DateTime createdCutoff);

    /// <summary>
    /// De-active all User accounts which are not active for x years
    /// </summary>
    /// <param name="createdCutoff">
    ///     The created cutoff.
    /// </param>
    Task LockInactiveAccountsAsync(DateTime createdCutoff);

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="isBotAutoDelete">if set to <c>true</c> [is bot automatic delete].</param>
    /// <returns>
    /// Returns if Deleting was successfully
    /// </returns>
    Task<bool> DeleteUserAsync(int userId, bool isBotAutoDelete = false);

    /// <summary>
    /// Deletes and ban's the user.
    /// </summary>
    /// <param name="user">The board user.</param>
    /// <param name="aspNetUser">The MemberShip User.</param>
    /// <param name="userIpAddress">The user's IP address.</param>
    /// <returns>
    /// Returns if Deleting was successfully
    /// </returns>
    Task<bool> DeleteAndBanUserAsync(User user, AspNetUsers aspNetUser, string userIpAddress);

    /// <summary>
    /// The find users by email.
    /// </summary>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <returns>
    /// Returns the Collection of founded Users
    /// </returns>
    IQueryable<AspNetUsers> FindUsersByEmail(string email);

    /// <summary>
    /// The find users by name.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <returns>
    /// Returns the Collection of founded Users
    /// </returns>
    IQueryable<AspNetUsers> FindUsersByName(string username);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>
    /// The get all users.
    /// </returns>
    IQueryable<AspNetUsers> GetAllUsers();

    /// <summary>
    /// get the membership user from the userID
    /// </summary>
    /// <param name="userId">
    ///     The user identifier.
    /// </param>
    /// <returns>
    /// The get membership user by id.
    /// </returns>
    Task<AspNetUsers> GetMembershipUserByIdAsync(int userId);

    /// <summary>
    /// Method returns MembershipUser
    /// </summary>
    /// <returns>
    /// Returns MembershipUser
    /// </returns>
    AspNetUsers GetUser();

    /// <summary>
    /// Method returns MembershipUser
    /// </summary>
    /// <returns>
    /// Returns MembershipUser
    /// </returns>
    Task<AspNetUsers> GetUserAsync();

    /// <summary>
    /// Method returns MembershipUser
    /// </summary>
    /// <param name="providerKey">The provider key.</param>
    /// <returns>
    /// Returns MembershipUser
    /// </returns>
    Task<AspNetUsers> GetUserAsync(object providerKey);

    /// <summary>
    /// Method returns MembershipUser
    /// </summary>
    /// <param name="username">The username.</param>
    /// <returns>
    /// Returns MembershipUser
    /// </returns>
    Task<AspNetUsers> GetUserByNameAsync(string username);

    /// <summary>
    /// Method returns MembershipUser
    /// </summary>
    /// <param name="email">
    ///     The email.
    /// </param>
    /// <returns>
    /// The <see cref="AspNetUsers"/>.
    /// </returns>
    Task<AspNetUsers> GetUserByEmailAsync(string email);

    /// <summary>
    /// Get the UserID from the ProviderUserKey
    /// </summary>
    /// <param name="providerUserKey">The provider user key.</param>
    /// <param name="currentBoard">
    ///     Get user from Current board, or all boards
    /// </param>
    /// <returns>
    /// The get user id from provider user key.
    /// </returns>
    Task<User> GetUserFromProviderUserKeyAsync(string providerUserKey, bool currentBoard = true);

    /// <summary>
    /// Helper function that gets user data from the DB (or cache)
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// The get user row for id.
    /// </returns>
    string GetUserProviderKeyFromUserId(int userId);

    /// <summary>
    /// Simply tells you if the user ID passed is the Guest user
    /// for the current board
    /// </summary>
    /// <param name="userId">
    /// ID of user to lookup
    /// </param>
    /// <returns>
    /// true if the user id is a guest user
    /// </returns>
    bool IsGuestUser(int userId);

    /// <summary>
    /// Helper function to update a user's email address.
    /// Syncs with both the YAF DB and Membership Provider.
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="newEmail">
    ///     The new email.
    /// </param>
    /// <returns>
    /// The update email.
    /// </returns>
    Task<bool> UpdateEmailAsync(AspNetUsers user, string newEmail);

    /// <summary>
    /// Checks Membership Provider to see if a user
    /// with the username and email passed exists.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="email">The email.</param>
    /// <returns>
    /// true if they exist
    /// </returns>
    bool UserExists(string userName, string email);

    /// <summary>
    /// The sign out.
    /// </summary>
    Task SignOutAsync();

    /// <summary>
    /// The sign in.
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="isPersistent">
    ///     The is persistent.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    Task<IActionResult> SignInAsync(AspNetUsers user, bool isPersistent = true);

    /// <summary>
    /// The sign in external.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    Task<IActionResult> SignInExternalAsync(AspNetUsers user);

    /// <summary>
    /// Returns the roles for the user
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// Returns the roles for the user
    /// </returns>
    Task<IList<string>> GetUserRolesAsync(AspNetUsers user);

    /// <summary>
    /// Returns true if the user is in the specified role
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="role">
    ///     The role.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    Task<bool> IsUserInRoleAsync(AspNetUsers user, string role);

    /// <summary>
    /// Add a user to a role
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="roleName">
    /// The role Name.
    /// </param>
    void AddToRole(AspNetUsers user, string roleName);

    /// <summary>
    /// The remove from role.
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="role">
    ///     The role.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    Task<IdentityResult> RemoveUserFromRoleAsync(AspNetUsers user, string role);

    /// <summary>
    /// Create a user with the given password
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="password">
    ///     The password.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    Task<IdentityResult> CreateUserAsync(AspNetUsers user, string password);

    /// <summary>
    /// Update a user
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    Task<IdentityResult> UpdateUserAsync(AspNetUsers user);

    /// <summary>
    /// Confirm the user's email with confirmation token
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="token">
    ///     The token.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    Task<IdentityResult> ConfirmUserEmailAsync(AspNetUsers user, string token);

    /// <summary>
    /// Generate a password reset token for the user using the UserTokenProvider
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    Task<string> GeneratePasswordResetTokenAsync(AspNetUsers user);

    /// <summary>
    /// Reset a user's password using a reset password token
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="token">
    ///     The token.
    /// </param>
    /// <param name="newPassword">
    ///     The new Password.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    Task<IdentityResult> ResetPasswordAsync(AspNetUsers user, string token, string newPassword);

    /// <summary>
    /// Get the email confirmation token for the user
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    Task<string> GenerateEmailConfirmationResetTokenAsync(AspNetUsers user);

    /// <summary>
    /// Change a user password
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <param name="currentPassword">
    ///     The current Password.
    /// </param>
    /// <param name="newPassword">
    ///     The new Password.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    Task<IdentityResult> ChangePasswordAsync(AspNetUsers user, string currentPassword, string newPassword);

    /// <summary>
    /// The add login.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="login">
    /// The login.
    /// </param>
    void AddLogin(string userId, UserLoginInfo login);

    /// <summary>
    /// Finds the user.
    /// </summary>
    /// <param name="userName">
    ///     The user Name.
    /// </param>
    /// <returns>
    /// The <see cref="AspNetUsers"/>.
    /// </returns>
    Task<AspNetUsers> ValidateUserAsync(string userName);

    /// <summary>
    /// Gets the board user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="boardId">The board identifier.</param>
    /// <param name="includeNonApproved">if set to <c>true</c> [include non approved].</param>
    /// <returns>Tuple&lt;User, AspNetUsers, Rank, VAccess&gt;.</returns>
    public Task<Tuple<User, AspNetUsers, Rank, VAccess>> GetBoardUserAsync(int userId,
        int? boardId = null,
        bool includeNonApproved = false);

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
    /// <param name="includeGuests">Include Guests ?!</param>
    /// <returns>
    /// Returns the board users.
    /// </returns>
    public List<PagedUser> GetUsersPaged(
        int? boardId,
        int pageIndex,
        int pageSize,
        string name,
        string email,
        DateTime? joinedDate,
        bool onlySuspended,
        int? groupId,
        int? rankId,
        bool includeGuests = true);

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
    public List<PagedUser> ListMembersPaged(
        int? boardId,
        int? groupId,
        int? rankId,
        char startLetter,
        string name,
        int pageIndex,
        int pageSize,
        int? sortName,
        int? sortRank,
        int? sortJoined,
        int? sortPosts,
        int? sortLastVisit,
        int? numPosts,
        int numPostCompare);

    /// <summary>
    /// Imports all AspNetUsers users in to the YAF DB, if they do not already exist.
    /// </summary>
    void ImportAllMembershipUsers();
}