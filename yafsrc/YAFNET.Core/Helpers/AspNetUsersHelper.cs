/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using Microsoft.AspNetCore.Authentication;

namespace YAF.Core.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using YAF.Core.Identity;
using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

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
    public AspNetUsersHelper(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; protected set; }

    /// <summary>
    /// The users.
    /// </summary>
    public IQueryable<AspNetUsers> Users => this.Get<AspNetUsersManager>().AspNetUsers;

    /// <summary>Used to hash/verify passwords</summary>et
    public IPasswordHasher<AspNetUsers> PasswordHasher => this.Get<AspNetUsersManager>().PasswordHasher;

    /// <summary>
    /// Enable 2FA for the ASP net user
    /// </summary>
    /// <param name="aspNetUser">The ASP net user.</param>
    /// <param name="twoFactorKey">The two factor key.</param>
    /// <returns>Task.</returns>
    public Task SetTwoFactorEnabledAsync(AspNetUsers aspNetUser, string twoFactorKey)
    {
        aspNetUser.TwoFactorEnabled = true;

        aspNetUser.MobilePIN = twoFactorKey;

        return this.Get<AspNetUsersManager>().UpdateUsrAsync(aspNetUser);
    }

    /// <summary>
    /// Disable 2FA for the ASP net user
    /// </summary>
    /// <param name="aspNetUser">The ASP net user.</param>
    /// <param name="twoFactorKey">The two factor key.</param>
    /// <returns>Task.</returns>
    public Task SetTwoFactorDisabledAsync(AspNetUsers aspNetUser, string twoFactorKey)
    {
        aspNetUser.TwoFactorEnabled = false;

        aspNetUser.MobilePIN = string.Empty;

        return this.Get<AspNetUsersManager>().UpdateUsrAsync(aspNetUser);
    }

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
    public User GuestUser(int boardId)
    {
        var guestUser = this.Get<IDataCache>().GetOrSet(
            Constants.Cache.GuestUser, () =>
            {
                var guest = this.GetRepository<User>().Get(u => u.BoardID == boardId && (u.Flags & 4) == 4);

                var guestUser = guest.FirstOrDefault() ?? throw new NoValidGuestUserForBoardException(
                    $"""
                     Could not locate the guest user for the board id {BoardContext.Current.PageBoardID}.
                                                               You might have deleted the guest group or removed the guest user.
                     """);
                if (guest.Count > 1)
                {
                    throw new ArgumentException(
                        $"Found {guest.Count} possible guest users. There should be one and only one user marked as guest.");
                }

                return guestUser;
            });

        return guestUser;
    }

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
    public async Task<User> GuestUserAsync(int boardId)
    {
        var guestUser = await this.Get<IDataCache>().GetOrSetAsync(
            Constants.Cache.GuestUser, async () =>
                {
                    var guest = await this.GetRepository<User>().GetAsync(u => u.BoardID == boardId && (u.Flags & 4) == 4);

                    var guestUser = guest.FirstOrDefault() ?? throw new NoValidGuestUserForBoardException(
                                        $"""
                                         Could not locate the guest user for the board id {BoardContext.Current.PageBoardID}.
                                                                                   You might have deleted the guest group or removed the guest user.
                                         """);
                    if (guest.Count > 1)
                    {
                        throw new ArgumentException(
                            $"Found {guest.Count} possible guest users. There should be one and only one user marked as guest.");
                    }

                    return guestUser;
                });

        return guestUser;
    }

    /// <summary>
    /// For the admin function: approve all users. Approves all
    /// users waiting for approval
    /// </summary>
    public async Task ApproveAllAsync()
    {
        // get all users...
        var allUsers = this.Get<IAspNetUsersHelper>().GetAllUsers();

        // iterate through each one...
        foreach (var user in allUsers.Where(user => !user.IsApproved))
        {
            // approve this user...
            user.IsApproved = true;
            await this.Get<AspNetUsersManager>().UpdateUsrAsync(user);
            var checkUser = await this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKeyAsync(user.Id);

            var id = checkUser?.ID ?? 0;

            if (id <= 0)
            {
                return;
            }

            await this.GetRepository<User>().ApproveAsync(checkUser);

            var checkEmail = await this.GetRepository<CheckEmail>().GetSingleAsync(m => m.UserID == id);

            if (checkEmail != null)
            {
                await this.GetRepository<CheckEmail>().DeleteByIdAsync(checkEmail.ID);
            }
        }
    }

    /// <summary>
    /// Approves the user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>
    /// The approve user.
    /// </returns>
    public async Task<bool> ApproveUserAsync(int userId)
    {
        var yafUser = await this.GetRepository<User>().GetByIdAsync(userId);

        if (yafUser?.ProviderUserKey == null)
        {
            return false;
        }

        var user = await this.Get<IAspNetUsersHelper>().GetUserAsync(yafUser.ProviderUserKey);

        if (!user.IsApproved)
        {
            user.IsApproved = true;
        }

        await this.Get<AspNetUsersManager>().UpdateUsrAsync(user);

        await this.GetRepository<User>().ApproveAsync(yafUser);

        var checkEmail = await this.GetRepository<CheckEmail>().GetSingleAsync(m => m.UserID == userId);

        if (checkEmail != null)
        {
            await this.GetRepository<CheckEmail>().DeleteByIdAsync(checkEmail.ID);
        }

        return true;
    }

    /// <summary>
    /// Deletes all Unapproved Users older than Cut Off DateTime
    /// </summary>
    /// <param name="createdCutoff">
    ///     The created cutoff.
    /// </param>
    public async Task DeleteAllUnapprovedAsync(DateTime createdCutoff)
    {
        // get all users...
        var allUsers = this.Get<IAspNetUsersHelper>().GetAllUsers();

        // iterate through each one...
        foreach (var user in allUsers.Where(user => !user.IsApproved && user.CreateDate < createdCutoff))
        {
            var yafUser = await this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKeyAsync(user.Id);

            if (yafUser != null)
            {
                // delete this user...
                await this.GetRepository<User>().DeleteAsync(yafUser);
            }

            await this.Get<AspNetUsersManager>().DeleteUserAsync(user);

            if (this.Get<BoardSettings>().LogUserDeleted)
            {
                this.Get<ILogger<AspNetUsersHelper>>().UserDeleted(
                    BoardContext.Current.PageUser.ID,
                    $"User {user.UserName} was deleted by user id {BoardContext.Current.PageUserID} as unapproved.");
            }
        }
    }

    /// <summary>
    /// De-active all PageUser accounts which are not active for x years
    /// </summary>
    /// <param name="createdCutoff">
    ///     The created cutoff.
    /// </param>
    public async Task LockInactiveAccountsAsync(DateTime createdCutoff)
    {
        var allUsers = await this.GetRepository<User>().GetByBoardIdAsync();

        // iterate through each one...
        allUsers.Where(user => (user.Flags & 4) != 4 && (user.Flags & 2) == 2 && (user.Flags & 32) != 32 && user.LastVisit < createdCutoff)
            .ForEach(
                user =>
                {
                    var flags = user.UserFlags;

                    // Set user to un-approve...
                    flags.IsApproved = false;
                    flags.IsDeleted = true;

                    this.GetRepository<User>().UpdateOnly(
                        () => new User
                                  {
                                      Flags = flags.BitValue
                                  },
                        u => u.ID == user.ID);
                });
    }

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="isBotAutoDelete">if set to <c>true</c> [is bot automatic delete].</param>
    /// <returns>
    /// Returns if Deleting was successfully
    /// </returns>
    public async Task<bool> DeleteUserAsync(int userId, bool isBotAutoDelete = false)
    {
        var user = await this.GetRepository<User>().GetByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var webRootPath = this.Get<IWebHostEnvironment>().WebRootPath;

        // Delete the images/albums both from database and physically.
        var uploadFolderPath = Path.Combine(webRootPath, this.Get<BoardFolders>().Uploads);

        var dt = this.GetRepository<UserAlbum>().ListByUser(userId);

        dt.ForEach(dr => this.Get<IAlbum>().AlbumImageDeleteAsync(uploadFolderPath, dr.ID, userId));

        // Check if there are any avatar images in the uploads folder
        if (!this.Get<BoardSettings>().UseFileTable && this.Get<BoardSettings>().AvatarUpload)
        {
            var imageExtensions = StaticDataHelper.ImageFormats();

            imageExtensions.ForEach(
                extension =>
                    {
                        if (File.Exists(Path.Combine(uploadFolderPath, $"{userId}.{extension}")))
                        {
                            File.Delete(Path.Combine(uploadFolderPath, $"{userId}.{extension}"));
                        }
                    });
        }

        var aspNetUser = await this.Get<IAspNetUsersHelper>().GetUserAsync(user.ProviderUserKey);

        if (aspNetUser != null)
        {
            await this.Get<AspNetUsersManager>().DeleteUserAsync(aspNetUser);
        }

        await this.GetRepository<User>().DeleteAsync(user);

        if (this.Get<BoardSettings>().LogUserDeleted)
        {
            this.Get<ILogger<AspNetUsersHelper>>().UserDeleted(
                BoardContext.Current.PageUser.ID,
                $"User {user.DisplayOrUserName()} was deleted by {(isBotAutoDelete ? "the automatic spam check system" : BoardContext.Current.PageUser.DisplayOrUserName())}.");
        }

        return true;
    }

    /// <summary>
    /// Deletes and ban's the user.
    /// </summary>
    /// <param name="user">The board user.</param>
    /// <param name="aspNetUser">The MemberShip User.</param>
    /// <param name="userIpAddress">The user's IP address.</param>
    /// <returns>
    /// Returns if Deleting was successfully
    /// </returns>
    public async Task<bool> DeleteAndBanUserAsync(User user, AspNetUsers aspNetUser, string userIpAddress)
    {
        // Update Anti SPAM Stats
        this.GetRepository<Registry>().IncrementBannedUsers();

        // Ban IP ?
        if (this.Get<BoardSettings>().BanBotIpOnDetection)
        {
            this.Get<IRaiseEvent>().Raise(new BanUserEvent(user.ID, aspNetUser.UserName, user.Email, userIpAddress));
        }

        var webRootPath = this.Get<IWebHostEnvironment>().WebRootPath;

        // Delete the images/albums both from database and physically.
        var uploadDir = Path.Combine(webRootPath, this.Get<BoardFolders>().Uploads);

        var dt = this.GetRepository<UserAlbum>().ListByUser(user.ID);

        dt.ForEach(dr => this.Get<IAlbum>().AlbumImageDeleteAsync(uploadDir, dr.ID, user.ID));

        // delete posts...
        var messages = this.GetRepository<Message>().GetAllUserMessages(user.ID).Distinct()
            .ToList();

        foreach (var x in messages)
        {
            await this.GetRepository<Message>().DeleteAsync(
                x.Topic.ForumID,
                x.TopicID,
                x,
                true,
                string.Empty,
                true,
                true);
        }

        await this.Get<AspNetUsersManager>().DeleteUserAsync(aspNetUser);
        await this.GetRepository<User>().DeleteAsync(user);

        if (this.Get<BoardSettings>().LogUserDeleted)
        {
            this.Get<ILogger<AspNetUsersHelper>>().UserDeleted(
                BoardContext.Current.PageUser.ID,
                $"User {aspNetUser.UserName} was deleted by the automatic spam check system.");
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
    /// <param name="userId">
    ///     The user identifier.
    /// </param>
    /// <returns>
    /// The get membership user by id.
    /// </returns>
    public async Task<AspNetUsers> GetMembershipUserByIdAsync(int userId)
    {
        var providerUserKey = this.Get<IAspNetUsersHelper>().GetUserProviderKeyFromUserId(userId);

        return providerUserKey != null
                   ? await this.Get<IAspNetUsersHelper>().GetUserAsync(providerUserKey)
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
        var httpContext = this.Get<IHttpContextAccessor>().HttpContext;

        if (httpContext?.User.Identity == null)
        {
            return null;
        }

        return httpContext.User.Identity.IsAuthenticated
            ? this.GetRepository<AspNetUsers>().GetSingle(
                u => u.UserName.ToLower(CultureInfo.InvariantCulture) ==
                     httpContext.User.Identity.Name.ToLower(CultureInfo.InvariantCulture))
            : null;
    }

    /// <summary>
    /// Method returns MembershipUser
    /// </summary>
    /// <returns>
    /// Returns MembershipUser
    /// </returns>
    public async Task<AspNetUsers> GetUserAsync()
    {
        var httpContext = this.Get<IHttpContextAccessor>().HttpContext;

        if (httpContext?.User.Identity == null)
        {
            return null;
        }

        return httpContext.User.Identity.IsAuthenticated
                   ? await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(httpContext.User.Identity.Name)
                   : null;
    }

    /// <summary>
    /// Method returns MembershipUser
    /// </summary>
    /// <param name="providerKey">The provider key.</param>
    /// <returns>
    /// Returns MembershipUser
    /// </returns>
    public Task<AspNetUsers> GetUserAsync(object providerKey)
    {
        return this.Get<AspNetUsersManager>().FindUserByIdAsync(providerKey.ToString());
    }

    /// <summary>
    /// Method returns MembershipUser
    /// </summary>
    /// <param name="username">The username.</param>
    /// <returns>
    /// Returns MembershipUser
    /// </returns>
    public Task<AspNetUsers> GetUserByNameAsync(string username)
    {
        return this.Get<AspNetUsersManager>().FindUserByNameAsync(username);
    }

    /// <summary>
    /// Method returns MembershipUser
    /// </summary>
    /// <param name="email">
    ///     The email.
    /// </param>
    /// <returns>
    /// The <see cref="AspNetUsers"/>.
    /// </returns>
    public Task<AspNetUsers> GetUserByEmailAsync(string email)
    {
        return this.Get<AspNetUsersManager>().FindUserByEmailAsync(email);
    }

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
    public Task<User> GetUserFromProviderUserKeyAsync(string providerUserKey, bool currentBoard = true)
    {
        return this.GetRepository<User>().GetUserByProviderKeyAsync(currentBoard ? BoardContext.Current.PageBoardID : null, providerUserKey);
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
    public string GetUserProviderKeyFromUserId(int userId)
    {
        return this.GetRepository<User>().GetById(userId).ProviderUserKey;
    }

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
    public bool IsGuestUser(int userId)
    {
        return BoardContext.Current.GuestUserID == userId;
    }

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
    public async Task<bool> UpdateEmailAsync(AspNetUsers user, string newEmail)
    {
        user.Email = newEmail;

        await this.Get<AspNetUsersManager>().UpdateUsrAsync(user);

        await this.GetRepository<User>().AspNetAsync(
            BoardContext.Current.PageBoardID,
            user.UserName,
            null,
            newEmail,
            user.Id,
            this.Get<BoardSettings>().PageSizeDefault,
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
    public Task SignOutAsync()
    {
        return this.Get<SignInManager<AspNetUsers>>().SignOutAsync();
    }

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
    public async Task<IActionResult> SignInAsync(AspNetUsers user, bool isPersistent = true)
    {
        await this.Get<SignInManager<AspNetUsers>>().SignOutAsync();

        var authenticationProperties = new AuthenticationProperties() { IsPersistent = isPersistent };

        if (isPersistent)
        {
            authenticationProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1);
        }

        await this.Get<SignInManager<AspNetUsers>>().SignInAsync(user, authenticationProperties);

        user.AccessFailedCount = 0;
        user.LockoutEndDateUtc = null;

        await this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);

        this.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(BoardContext.Current.PageUserID));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
    }

    /// <summary>
    /// The sign in external.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> SignInExternalAsync(AspNetUsers user)
    {
        var info = await this.Get<SignInManager<AspNetUsers>>().GetExternalLoginInfoAsync();

        if (info == null)
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Account_Login);
        }

        await this.Get<SignInManager<AspNetUsers>>().SignOutAsync();

        // Sign in the user with this external login provider if the user already has a login.
        await this.Get<SignInManager<AspNetUsers>>().SignInAsync(user, true);

        user.AccessFailedCount = 0;
        user.LockoutEndDateUtc = null;

        await this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);

        this.Get<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(BoardContext.Current.PageUserID));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
    }

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
    public Task<IdentityResult> RemoveUserFromRoleAsync(AspNetUsers user, string role)
    {
        return this.Get<AspNetUsersManager>().RemoveUserFromRoleAsync(user, role);
    }

    /// <summary>
    /// Returns the roles for the user
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// Returns the roles for the user
    /// </returns>
    public Task<IList<string>> GetUserRolesAsync(AspNetUsers user)
    {
        return this.Get<AspNetUsersManager>().GetUserRolesAsync(user);
    }

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
    public Task<bool> IsUserInRoleAsync(AspNetUsers user, string role)
    {
        return this.Get<AspNetUsersManager>().IsUserInRoleAsync(user, role);
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
        this.Get<AspNetUsersManager>().AddToRole(user, roleName);
    }

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
    public Task<IdentityResult> CreateUserAsync(AspNetUsers user, string password)
    {
        return this.Get<AspNetUsersManager>().CreateUserAsync(user, password);
    }

    /// <summary>
    /// Update a user
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// The <see cref="IdentityResult"/>.
    /// </returns>
    public Task<IdentityResult> UpdateUserAsync(AspNetUsers user)
    {
        return this.Get<AspNetUsersManager>().UpdateUsrAsync(user);
    }

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
    public Task<IdentityResult> ConfirmUserEmailAsync(AspNetUsers user, string token)
    {
        return this.Get<AspNetUsersManager>().ConfirmUserEmailAsync(user, token);
    }

    /// <summary>
    /// Generate a password reset token for the user using the UserTokenProvider
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public Task<string> GeneratePasswordResetTokenAsync(AspNetUsers user)
    {
        return this.Get<AspNetUsersManager>().GeneratePasswordResTokenAsync(user);
    }

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
    public Task<IdentityResult> ResetPasswordAsync(AspNetUsers user, string token, string newPassword)
    {
        return this.Get<AspNetUsersManager>().ResetUserPasswordAsync(user, token, newPassword);
    }

    /// <summary>
    /// Get the email confirmation token for the user
    /// </summary>
    /// <param name="user">
    ///     The user.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public Task<string> GenerateEmailConfirmationResetTokenAsync(AspNetUsers user)
    {
        return this.Get<AspNetUsersManager>().GenerateEmailConfirmationResTokenAsync(user);
    }

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
    public Task<IdentityResult> ChangePasswordAsync(AspNetUsers user, string currentPassword, string newPassword)
    {
        return this.Get<AspNetUsersManager>().ChangeUserPasswordAsync(user, currentPassword, newPassword);
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
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(login);

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
    ///     The user Name.
    /// </param>
    /// <returns>
    /// The <see cref="AspNetUsers"/>.
    /// </returns>
    public async Task<AspNetUsers> ValidateUserAsync(string userName)
    {
        if (userName.Contains('@'))
        {
            // attempt Email Login
            var realUser = await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(userName);

            if (realUser != null)
            {
                return realUser;
            }
        }

        var user = await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(userName);

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
        var realUsername = await this.GetRepository<User>().GetSingleAsync(u => u.DisplayName == userName && (u.Flags & 4) != 4);

        if (realUsername == null)
        {
            return null;
        }

        // get the username associated with this id...
        user = await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(realUsername.Name);

        // validate again...
        return user;

        // no valid login -- return null
    }

    /// <summary>
    /// Gets the board user by Id.
    /// </summary>
    /// <param name="userId">
    ///     The user id.
    /// </param>
    /// <param name="boardId">
    ///     The board id.
    /// </param>
    /// <param name="includeNonApproved"></param>
    /// <returns>
    /// The <see cref="Tuple"/>.
    /// </returns>
    public async Task<Tuple<User, AspNetUsers, Rank, VAccess>> GetBoardUserAsync(int userId,
        int? boardId = null,
        bool includeNonApproved = false)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

        expression.LeftJoin<VAccess>((u, v) => v.UserID == u.ID).LeftJoin<AspNetUsers>((u, a) => a.Id == u.ProviderUserKey)
            .Join<Rank>((u, r) => r.ID == u.RankID);

        if (includeNonApproved)
        {
            expression.Where<VAccess, User>(
                (v, u) => u.ID == userId && u.BoardID == (boardId ?? this.GetRepository<User>().BoardID));
        }
        else
        {
            expression.Where<VAccess, User>(
                (v, u) => u.ID == userId && u.BoardID == (boardId ?? this.GetRepository<User>().BoardID) &&
                          (u.Flags & 2) == 2);
        }

        var users = await this.GetRepository<User>().DbAccess
            .ExecuteAsync(db => db.SelectMultiAsync<User, AspNetUsers, Rank, VAccess>(expression));

        return users.FirstOrDefault();
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
        bool includeGuests = true)
    {
        return this.GetRepository<User>().DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

                    Expression<Func<User, bool>> whereCriteria =
                        includeGuests
                            ? u => u.BoardID == (boardId ?? this.GetRepository<User>().BoardID)
                                   && (u.Flags & 2) == 2
                            : u => u.BoardID == (boardId ?? this.GetRepository<User>().BoardID)
                                   && (u.Flags & 2) == 2 && (u.Flags & 4) != 4;

                    // -- count total
                    var countTotalExpression = db.Connection.From<User>();

                    expression.Join<Rank>((u, r) => r.ID == u.RankID)
                        .LeftJoin<AspNetUsers>((u, a) => a.Id == u.ProviderUserKey);

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
                            $"""
                             exists(select 1 from {countTotalExpression.Table<UserGroup>()} x
                                                                            where x.{countTotalExpression.Column<UserGroup>(x => x.UserID)} = {countTotalExpression.Column<User>(x => x.ID, true)}
                                                                            and x.{countTotalExpression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})
                             """);

                        expression.UnsafeAnd(
                            $"""
                             exists(select 1 from {expression.Table<UserGroup>()} x
                                                                            where x.{expression.Column<UserGroup>(x => x.UserID)} = {expression.Column<User>(x => x.ID, true)}
                                                                            and x.{expression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})
                             """);
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
    /// Returns paged members list
    /// </returns>
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
        int numPostCompare)
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

                    // Remove Guests amd deleted
                    expression.And<User>(u => (u.Flags & 4) != 4 && (u.Flags & 32) != 32);

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
                            $"""
                             exists(select 1 from {countTotalExpression.Table<UserGroup>()} x
                                                                            where x.{countTotalExpression.Column<UserGroup>(x => x.UserID)} = {countTotalExpression.Column<User>(x => x.ID, true)}
                                                                            and x.{countTotalExpression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})
                             """);

                        expression.UnsafeAnd(
                            $"""
                             exists(select 1 from {expression.Table<UserGroup>()} x
                                                                            where x.{expression.Column<UserGroup>(x => x.UserID)} = {expression.Column<User>(x => x.ID, true)}
                                                                            and x.{expression.Column<UserGroup>(x => x.GroupID)} = {groupId.Value})
                             """);
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
                                                 RankName = r.Name,
                                                 TotalRows = Sql.Custom($"({countTotalSql})")
                                             });

                    // Set Sorting
                    if (sortName is > 0)
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
}