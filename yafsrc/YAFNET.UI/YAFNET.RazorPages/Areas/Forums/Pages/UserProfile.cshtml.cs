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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Flags;
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
    /// Gets or sets the facebook URL.
    /// </summary>
    /// <value>The facebook URL.</value>
    [BindProperty] public string FacebookUrl { get; set; }

    /// <summary>
    /// Gets or sets the skype URL.
    /// </summary>
    /// <value>The skype URL.</value>
    [BindProperty] public string SkypeUrl { get; set; }

    /// <summary>
    /// Gets or sets the blog URL.
    /// </summary>
    /// <value>The blog URL.</value>
    [BindProperty] public string BlogUrl { get; set; }

    /// <summary>
    /// Gets or sets the XMPP URL.
    /// </summary>
    /// <value>The XMPP URL.</value>
    [BindProperty] public string XmppUrl { get; set; }

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
                ? this.Get<LinkBuilder>().GetLink(ForumPages.Members)
                : null);
        this.PageBoardContext.PageLinks.AddLink(userDisplayName, string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet(int u)
    {
        return u == 0
            ?
            // No such user exists
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid)
            : this.BindData(u);
    }

    /// <summary>
    /// Remove active suspension.
    /// </summary>
    /// <param name="u">The u.</param>
    /// <returns>System.Threading.Tasks.Task.</returns>
    public Task OnPostRemoveSuspensionAsync(int u)
    {
        this.BindData(u);

        // un-suspend user
        this.GetRepository<User>().Suspend(u);

        if (this.PageBoardContext.BoardSettings.LogUserSuspendedUnsuspended)
        {
            this.Get<ILogger<UserProfileModel>>().Log(
                this.PageBoardContext.PageUserID,
                "YAF.Controls.EditUsersSuspend",
                $"User {this.CombinedUser.Item1.DisplayOrUserName()} was unsuspended by {this.CombinedUser.Item1.DisplayOrUserName()}.",
                EventLogTypes.UserUnsuspended);
        }

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(u));

        return this.Get<ISendNotification>().SendUserSuspensionEndedNotificationAsync(
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
        this.BindData(u);

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
        this.GetRepository<User>().Suspend(u, suspend, this.SuspendReason.Trim(), this.PageBoardContext.PageUserID);

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

        this.BindData(u);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private IActionResult BindData(int userId)
    {
        this.CombinedUser = this.Get<IAspNetUsersHelper>().GetBoardUser(userId);

        if (this.CombinedUser is null || this.CombinedUser.Item1.ID == 0)
        {
            // No such user exists or this is a nntp user ("0")
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
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
            this.ShowUserMedals();
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
    /// Show the user medals.
    /// </summary>
    private void ShowUserMedals()
    {
        var key = string.Format(Constants.Cache.UserMedals, this.CombinedUser.Item1.ID);

        // get the medals cached...
        var userMedals = this.DataCache.GetOrSet(
            key,
            () => this.GetRepository<Medal>().ListUserMedals(this.CombinedUser.Item1.ID),
            TimeSpan.FromMinutes(10));

        if (userMedals.Count == 0)
        {
            return;
        }

        var ribbonBar = new StringBuilder();
        var medals = new StringBuilder();

        userMedals.ForEach(
            medal =>
            {
                var flags = new MedalFlags(medal.Flags);

                // skip hidden medals
                if (flags.AllowHiding && medal.Hide)
                {
                    return;
                }

                var title = $"{medal.Name}{(flags.ShowMessage ? $": {medal.Message}" : string.Empty)}";

                ribbonBar.AppendFormat(
                    "<li class=\"list-inline-item\"><img src=\"/{2}/{0}\" alt=\"{1}\" title=\"{1}\" data-bs-toggle=\"tooltip\"></li>",
                    medal.MedalURL,
                    title,
                    this.Get<BoardFolders>().Medals);
            });

        this.Medals = $"<ul class=\"list-inline\">{ribbonBar}{medals}</ul>";
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

        if (this.CombinedUser.Item2.Profile_Facebook.IsSet())
        {
            this.FacebookUrl = ValidationHelper.IsNumeric(this.CombinedUser.Item2.Profile_Facebook)
                ? $"https://www.facebook.com/profile.php?id={this.CombinedUser.Item2.Profile_Facebook}"
                : this.CombinedUser.Item2.Profile_Facebook;
        }

        if (this.CombinedUser.Item2.Profile_XMPP.IsSet())
        {
            this.XmppUrl = this.Get<LinkBuilder>().GetLink(
                ForumPages.Jabber,
                new { u = this.CombinedUser.Item1.ID });
        }

        if (this.CombinedUser.Item2.Profile_Skype.IsSet())
        {
            this.SkypeUrl = $"skype:{this.CombinedUser.Item2.Profile_Skype}?call";
        }

        if (!this.SkypeUrl.IsSet() && !this.BlogUrl.IsSet() && !this.XmppUrl.IsSet() && !this.FacebookUrl.IsSet())
        {
            this.ShowSocialMediaCard = false;
        }
        else
        {
            this.ShowSocialMediaCard = true;
        }

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
        var allPosts = 0.0;

        var postsInBoard = this.GetRepository<Message>()
            .Count(m => (m.Flags & 16) == 16 && (m.Flags & 8) != 8);

        if (postsInBoard > 0)
        {
            allPosts = 100.0 * this.CombinedUser.Item1.NumPosts / postsInBoard;
        }

        var numberDays = DateTimeHelper.DateDiffDay(this.CombinedUser.Item1.Joined, DateTime.UtcNow) + 1;

        this.Stats =
            $"{this.CombinedUser.Item1.NumPosts:N0} [{this.GetTextFormatted("NUMALL", allPosts)} / {this.GetTextFormatted("NUMDAY", (double)this.CombinedUser.Item1.NumPosts / numberDays)}]";
    }
}