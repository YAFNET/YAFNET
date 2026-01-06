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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects.Model;

/// <summary>
/// The User Profile Page.
/// </summary>
public class UserProfileModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "UserProfileModel" /> class.
    /// </summary>
    public UserProfileModel()
        : base("PROFILE", ForumPages.UserProfile)
    {
    }

    /// <summary>
    /// Gets or sets the combined user.
    /// </summary>
    /// <value>The combined user.</value>
    public Tuple<User, AspNetUsers, Rank, VAccess> CombinedUser { get; set; }

    /// <summary>
    /// Gets or sets the custom profile.
    /// </summary>
    /// <value>The custom profile.</value>
    public List<Tuple<ProfileCustom, ProfileDefinition>> CustomProfile { get; set; }

    /// <summary>
    /// Gets or sets the groups.
    /// </summary>
    /// <value>The groups.</value>
    public List<Group> Groups { get; set; }

    /// <summary>
    /// Gets or sets the last posts.
    /// </summary>
    /// <value>The last posts.</value>
    public List<Tuple<Message, Topic, User>> LastPosts { get; set; }

    /// <summary>
    /// Gets or sets the friends.
    /// </summary>
    /// <value>The friends.</value>
    public List<BuddyUser> Friends { get; set; }

    /// <summary>
    /// Gets or sets the thanks.
    /// </summary>
    /// <value>The thanks.</value>
    public (int Posts, string ThanksReceived) Thanks { get; set; }

    /// <summary>
    /// Gets or sets the stats.
    /// </summary>
    /// <value>The stats.</value>
    [BindProperty] public string Stats { get; set; }

    /// <summary>
    /// Gets or sets the medals.
    /// </summary>
    /// <value>The medals.</value>
    [BindProperty] public string Medals { get; set; }

    /// <summary>
    /// Gets or sets the blog URL.
    /// </summary>
    /// <value>The blog URL.</value>
    [BindProperty] public string BlogUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show add buddy link].
    /// </summary>
    /// <value><c>true</c> if [show add buddy link]; otherwise, <c>false</c>.</value>
    [BindProperty] public bool ShowAddBuddyLink { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show remove buddy link].
    /// </summary>
    /// <value><c>true</c> if [show remove buddy link]; otherwise, <c>false</c>.</value>
    [BindProperty] public bool ShowRemoveBuddyLink { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show pm link].
    /// </summary>
    /// <value><c>true</c> if [show pm link]; otherwise, <c>false</c>.</value>
    [BindProperty] public bool ShowPmLink { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show email link].
    /// </summary>
    /// <value><c>true</c> if [show email link]; otherwise, <c>false</c>.</value>
    [BindProperty] public bool ShowEmailLink { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show social media card].
    /// </summary>
    /// <value><c>true</c> if [show social media card]; otherwise, <c>false</c>.</value>
    [BindProperty] public bool ShowSocialMediaCard { get; set; }

    /// <summary>
    /// Gets or sets the suspend reason.
    /// </summary>
    /// <value>The suspend reason.</value>
    [BindProperty] public string SuspendReason { get; set; }

    /// <summary>
    /// Gets or sets the suspend count.
    /// </summary>
    /// <value>The suspend count.</value>
    [BindProperty] public int SuspendCount { get; set; }

    /// <summary>
    /// Gets or sets the suspend unit.
    /// </summary>
    /// <value>The suspend unit.</value>
    [BindProperty] public string SuspendUnit { get; set; }

    /// <summary>
    /// Gets the suspend units.
    /// </summary>
    /// <value>The suspend units.</value>
    public List<SelectListItem> SuspendUnits =>
    [
        new(this.GetText("PROFILE", "DAYS"), "1"),
        new(this.GetText("PROFILE", "HOURS"), "2"),
        new(this.GetText("PROFILE", "MINUTES"), "3")
    ];

    /// <summary>
    /// add page links.
    /// </summary>
    /// <param name="userDisplayName">
    /// The user display name.
    /// </param>
    private void AddPageLinks(string userDisplayName)
    {
        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("MEMBERS"),
            this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.MembersListViewPermissions)
                ? this.Get<ILinkBuilder>().GetLink(ForumPages.Members)
                : null);
        this.PageBoardContext.PageLinks.AddLink(userDisplayName, string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(int u)
    {
        return u == 0
            ?
            // No such user exists
            this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid)
            : await this.BindDataAsync(u);
    }

    /// <summary>
    /// Remove active suspension.
    /// </summary>
    /// <param name="u">The u.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    public async Task OnPostRemoveSuspensionAsync(int u)
    {
        await this.BindDataAsync(u);

        // un-suspend user
        await this.GetRepository<User>().SuspendAsync(u);

        if (this.PageBoardContext.BoardSettings.LogUserSuspendedUnsuspended)
        {
            this.Get<ILogger<UserProfileModel>>().Log(
                this.PageBoardContext.PageUserID,
                "YAF.Controls.EditUsersSuspend",
                $"User {this.CombinedUser.Item1.DisplayOrUserName()} was unsuspended by {this.CombinedUser.Item1.DisplayOrUserName()}.",
                EventLogTypes.UserUnsuspended);
        }

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(u));

        await this.Get<ISendNotification>().SendUserSuspensionEndedNotificationAsync(
            this.CombinedUser.Item1.Email,
            this.CombinedUser.Item1.DisplayOrUserName());
    }

    /// <summary>
    /// Suspends the user.
    /// </summary>
    /// <param name="u">The u.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    public async Task OnPostSuspendAsync(int u)
    {
        await this.BindDataAsync(u);

        var access = await this.GetRepository<VAccess>().GetSingleAsync(v => v.UserID == u);

        // is user to be suspended admin?
        if (access.IsAdmin > 0)
        {
            // tell user he can't suspend admin
            this.PageBoardContext.Notify(this.GetText("PROFILE", "ERROR_ADMINISTRATORS"), MessageTypes.danger);
            return;
        }

        // is user to be suspended forum moderator, while user suspending him is not admin?
        if (!this.PageBoardContext.IsAdmin && access.IsForumModerator > 0)
        {
            // tell user he can't suspend forum moderator when he's not admin
            this.PageBoardContext.Notify(this.GetText("PROFILE", "ERROR_FORUMMODERATORS"), MessageTypes.danger);
            return;
        }

        var isGuest = this.CombinedUser.Item1.UserFlags.IsGuest;

        // verify the user isn't guest...
        if (isGuest)
        {
            this.PageBoardContext.Notify(this.GetText("PROFILE", "ERROR_GUESTACCOUNT"), MessageTypes.danger);
        }

        // time until when user is suspended
        var suspend = this.Get<IDateTimeService>().GetUserDateTime(
            DateTime.UtcNow,
            this.CombinedUser.Item1.TimeZoneInfo);

        // number inserted by suspending user
        var count = this.SuspendCount;

        // what time units are used for suspending
        suspend = this.SuspendUnit switch
        {
            // days
            "1" =>
                // add user inserted suspension time to current time
                suspend.AddDays(count),
            // hours
            "2" =>
                // add user inserted suspension time to current time
                suspend.AddHours(count),
            // minutes
            "3" =>
                // add user inserted suspension time to current time
                suspend.AddHours(count),
            _ => suspend
        };

        // suspend user by calling appropriate method
        await this.GetRepository<User>().SuspendAsync(u, suspend, this.SuspendReason.Trim(), this.PageBoardContext.PageUserID);

        this.Get<ILogger<UserProfileModel>>().Log(
            this.PageBoardContext.PageUserID,
            "YAF.Controls.EditUsersSuspend",
            $"User {this.CombinedUser.Item1.DisplayOrUserName()} was suspended by {this.PageBoardContext.PageUser.DisplayOrUserName()} until: {suspend} (UTC)",
            EventLogTypes.UserSuspended);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(u));

        await this.Get<ISendNotification>().SendUserSuspensionNotificationAsync(
            suspend,
            this.SuspendReason.Trim(),
            this.CombinedUser.Item1.Email,
            this.CombinedUser.Item1.DisplayOrUserName());

        this.SuspendReason = string.Empty;

        await this.BindDataAsync(u);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private async Task<IActionResult> BindDataAsync(int userId)
    {
        this.CombinedUser = await this.Get<IAspNetUsersHelper>().GetBoardUserAsync(userId);

        if (this.CombinedUser is null || this.CombinedUser.Item1.ID == 0)
        {
            // No such user exists or this is a nntp user ("0")
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // populate user information controls...
        // Is BuddyList feature enabled?
        if (this.PageBoardContext.BoardSettings.EnableBuddyList && !this.PageBoardContext.IsGuest)
        {
            this.SetupBuddyList();
        }

        // Show User Medals
        if (this.PageBoardContext.BoardSettings.ShowMedals)
        {
            this.Medals = this.Get<IUserMedalService>().GetUserMedals(userId);
        }

        this.AddPageLinks(this.CombinedUser.Item1.DisplayOrUserName());

        this.SetupUserStatistics();

        this.SetupUserLinks();

        this.Groups = this.GetRepository<UserGroup>().List(this.CombinedUser.Item1.ID);

        this.Thanks = this.GetRepository<Thanks>().ThanksToUser(this.CombinedUser.Item1.ID);

        this.LastPosts = this.GetRepository<Message>().GetAllUserMessagesWithAccess(
            this.PageBoardContext.PageBoardID,
            userId,
            this.PageBoardContext.PageUserID,
            10);

        // select hours
        this.SuspendUnit = "1";

        // default number of hours to suspend user for
        this.SuspendCount = 2;

        return this.Page();
    }

    /// <summary>
    /// The setup buddy list.
    /// </summary>
    private void SetupBuddyList()
    {
        if (this.CombinedUser.Item1.ID == this.PageBoardContext.PageUserID)
        {
            this.ShowAddBuddyLink = false;
            this.ShowRemoveBuddyLink = false;
        }
        else
        {
            if (this.Get<IFriends>().IsBuddy(this.CombinedUser.Item1.ID))
            {
                this.ShowAddBuddyLink = false;
                this.ShowRemoveBuddyLink = true;
            }
            else
            {
                this.ShowAddBuddyLink = true;
                this.ShowRemoveBuddyLink = false;
            }

            if (this.CombinedUser.Item1.Block.BlockFriendRequests)
            {
                this.ShowAddBuddyLink = false;
                this.ShowRemoveBuddyLink = false;
            }
        }

        this.Friends = this.GetRepository<Buddy>().GetAllFriends(this.CombinedUser.Item1.ID);
    }

    /// <summary>
    /// The setup user links.
    /// </summary>
    private void SetupUserLinks()
    {
        if (this.CombinedUser.Item2.Profile_Blog.IsSet())
        {
            var link = this.CombinedUser.Item2.Profile_Blog.Replace("\"", string.Empty);

            if (!link.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
            {
                link = $"https://{link}";
            }

            this.BlogUrl = link;
        }

        this.ShowSocialMediaCard = this.BlogUrl.IsSet();

        this.CustomProfile = this.DataCache.GetOrSet(
            string.Format(Constants.Cache.UserCustomProfileData, this.CombinedUser.Item1.ID),
            () => this.GetRepository<ProfileCustom>().ListByUser(this.CombinedUser.Item1.ID));

        if (this.CombinedUser.Item1.ID == this.PageBoardContext.PageUserID)
        {
            return;
        }

        var isFriend = this.Get<IFriends>().IsBuddy(this.CombinedUser.Item1.ID);

        this.ShowPmLink = !this.CombinedUser.Item1.UserFlags.IsGuest &&
                          this.PageBoardContext.BoardSettings.AllowPrivateMessages;

        if (this.ShowPmLink)
        {
            if (this.CombinedUser.Item1.Block.BlockPMs)
            {
                this.ShowPmLink = false;
            }

            if (this.PageBoardContext.IsAdmin || isFriend)
            {
                this.ShowPmLink = true;
            }
        }

        // email link
        this.ShowEmailLink = !this.CombinedUser.Item1.UserFlags.IsGuest &&
                             this.PageBoardContext.BoardSettings.AllowEmailSending;

        if (!this.ShowEmailLink)
        {
            return;
        }

        if (this.CombinedUser.Item1.Block.BlockEmails && !this.PageBoardContext.IsAdmin)
        {
            this.ShowEmailLink = false;
        }

        if (this.PageBoardContext.IsAdmin || isFriend)
        {
            this.ShowEmailLink = true;
        }
    }

    /// <summary>
    /// The setup user statistics.
    /// </summary>
    private void SetupUserStatistics()
    {
        this.Stats = this.CombinedUser.Item1.NumPosts.ToString();
    }
}