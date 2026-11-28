
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

namespace YAF.Pages.Admin;

using System.Net;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects.Model;

/// <summary>
/// Admin Members Page.
/// </summary>
public class UsersModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the since list.
    /// </summary>
    /// <value>The since list.</value>
    public List<SelectListItem> SinceList { get; set; }

    /// <summary>
    /// Gets or sets the groups.
    /// </summary>
    /// <value>The groups.</value>
    public SelectList Groups { get; set; }

    /// <summary>
    /// Gets or sets the ranks.
    /// </summary>
    /// <value>The ranks.</value>
    public SelectList Ranks { get; set; }

    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty]
    public List<PagedUser> List { get; set; }

    /// <summary>
    /// Gets or sets the ids of the users that are selected for a bulk action (only of the shown page).
    /// </summary>
    [BindProperty]
    public List<int> SelectedUsers { get; set; } = [];

    /// <summary>
    /// Gets or sets the number of time units of the bulk suspension.
    /// </summary>
    [BindProperty]
    public int BulkSuspendCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the time unit of the bulk suspension (1 days, 2 hours, 3 minutes).
    /// </summary>
    [BindProperty]
    public int BulkSuspendUnit { get; set; } = 1;

    /// <summary>
    /// Gets or sets the reason of the bulk suspension.
    /// </summary>
    [BindProperty]
    public string BulkSuspendReason { get; set; }

    /// <summary>
    /// Gets or sets the role of the bulk role actions.
    /// </summary>
    [BindProperty]
    public int BulkRole { get; set; }

    /// <summary>
    /// Gets or sets the roles of the board (bulk role actions).
    /// </summary>
    public SelectList BulkRoles { get; set; }

    /// <summary>
    /// Gets or sets the time units of the bulk suspension.
    /// </summary>
    public List<SelectListItem> SuspendUnits { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersModel"/> class.
    /// </summary>
    public UsersModel()
        : base("ADMIN_USERS", ForumPages.Admin_Users)
    {
    }

    /// <summary>
    /// Creates navigation page links on top of forum (breadcrumbs).
    /// </summary>
    public override void CreatePageLinks()
    {
        // link to administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// On post delete as an asynchronous operation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        // we are deleting user
        if (this.PageBoardContext.PageUserID == id)
        {
            // deleting yourself isn't an option (the page is shown again with the list)
            this.BindData();

            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_USERS", "MSG_SELF_DELETE"),
                MessageTypes.danger);
        }

        // get user(s) we are about to delete
        var userToDelete = await this.Get<IAspNetUsersHelper>().GetBoardUserAsync(
            id,
            this.PageBoardContext.PageBoardID, true);

        if (userToDelete is null)
        {
            // a user of another board or a user that was deleted already (the page was sent again): the list
            this.BindData();

            return this.Page();
        }

        if (userToDelete.Item1.UserFlags.IsGuest)
        {
            // we cannot delete guest
            this.BindData();

            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_USERS", "MSG_DELETE_GUEST"),
                MessageTypes.danger);
        }

        if (userToDelete.Item4.IsAdmin > 0 || userToDelete.Item1.UserFlags.IsHostAdmin)
        {
            // admin are not deletable either
            this.BindData();

            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_USERS", "MSG_DELETE_ADMIN"),
                MessageTypes.danger);
        }

        // all is good, user can be deleted
        await this.Get<IAspNetUsersHelper>().DeleteUserAsync(id);

        // the list is shown again with the filter (the notification shows the message as html)
        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetTextFormatted("USER_DELETED", WebUtility.HtmlEncode(userToDelete.Item1.DisplayOrUserName())),
            MessageTypes.success);
    }

    /// <summary>
    /// Checks if a user of the list can be selected for the bulk actions (not the guest, a host admin or yourself).
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>True if the user can be selected.</returns>
    public bool CanSelect(PagedUser user)
    {
        return user.IsGuest != true && !new UserFlags(user.Flags).IsHostAdmin
                                     && user.UserID != this.PageBoardContext.PageUserID;
    }

    /// <summary>
    /// Approves the selected users.
    /// </summary>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostBulkApproveAsync()
    {
        var (users, skipped) = await this.GetSelectedUsersAsync(false);
        var count = 0;

        foreach (var user in users.Where(u => !u.Item1.UserFlags.IsApproved))
        {
            if (await this.Get<IAspNetUsersHelper>().ApproveUserAsync(user.Item1.ID))
            {
                count++;
            }
        }

        return this.BulkDone(this.GetTextFormatted("BULK_APPROVED", count), skipped);
    }

    /// <summary>
    /// Suspends the selected users (not the admins, the moderators only by an admin).
    /// </summary>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostBulkSuspendAsync()
    {
        var (users, skipped) = await this.GetSelectedUsersAsync(true);

        // at most 100 years like the suspension of a user, a huge value would fail
        var unit = this.BulkSuspendUnit is >= 1 and <= 3 ? this.BulkSuspendUnit : 1;
        var count = Math.Clamp(
            this.BulkSuspendCount,
            1,
            unit switch
            {
                2 => 36500 * 24,
                3 => 36500 * 24 * 60,
                _ => 36500
            });

        var suspend = unit switch
        {
            2 => DateTime.UtcNow.AddHours(count),
            3 => DateTime.UtcNow.AddMinutes(count),
            _ => DateTime.UtcNow.AddDays(count)
        };

        var reason = this.BulkSuspendReason?.Trim();

        foreach (var user in users.Select(u => u.Item1))
        {
            await this.GetRepository<User>().SuspendAsync(user.ID, suspend, reason, this.PageBoardContext.PageUserID);

            if (this.PageBoardContext.BoardSettings.LogUserSuspendedUnsuspended)
            {
                this.Get<ILogger<UsersModel>>().Log(
                    this.PageBoardContext.PageUserID,
                    this,
                    $"User {user.DisplayOrUserName()} was suspended by {this.PageBoardContext.PageUser.DisplayOrUserName()} until: {suspend} (UTC)",
                    EventLogTypes.UserSuspended);
            }

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(user.ID));

            await this.Get<ISendNotification>().SendUserSuspensionNotificationAsync(
                this.Get<IDateTimeService>().GetUserDateTime(suspend, user.TimeZoneInfo),
                reason,
                user);
        }

        return this.BulkDone(this.GetTextFormatted("BULK_SUSPENDED", users.Count), skipped);
    }

    /// <summary>
    /// Removes the suspension of the selected users.
    /// </summary>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostBulkUnsuspendAsync()
    {
        var (users, skipped) = await this.GetSelectedUsersAsync(false);
        var count = 0;

        foreach (var user in users.Select(u => u.Item1).Where(u => u.Suspended.HasValue))
        {
            await this.GetRepository<User>().SuspendAsync(user.ID);

            if (this.PageBoardContext.BoardSettings.LogUserSuspendedUnsuspended)
            {
                this.Get<ILogger<UsersModel>>().Log(
                    this.PageBoardContext.PageUserID,
                    this,
                    $"User {user.DisplayOrUserName()} was un-suspended by {this.PageBoardContext.PageUser.DisplayOrUserName()}.",
                    EventLogTypes.UserUnsuspended);
            }

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(user.ID));

            await this.Get<ISendNotification>().SendUserSuspensionEndedNotificationAsync(user);

            count++;
        }

        return this.BulkDone(this.GetTextFormatted("BULK_UNSUSPENDED", count), skipped);
    }

    /// <summary>
    /// Adds the selected users to the role.
    /// </summary>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public Task<IActionResult> OnPostBulkAddRoleAsync()
    {
        return this.BulkRoleAsync(true);
    }

    /// <summary>
    /// Removes the selected users from the role (a user keeps at least one role).
    /// </summary>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public Task<IActionResult> OnPostBulkRemoveRoleAsync()
    {
        return this.BulkRoleAsync(false);
    }

    /// <summary>
    /// Deletes the selected users (not the admins), their posts are kept like the delete of a user.
    /// </summary>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostBulkDeleteAsync()
    {
        var (users, skipped) = await this.GetSelectedUsersAsync(true);
        var count = 0;

        foreach (var user in users)
        {
            if (await this.Get<IAspNetUsersHelper>().DeleteUserAsync(user.Item1.ID))
            {
                count++;
            }
        }

        return this.BulkDone(this.GetTextFormatted("BULK_DELETED", count), skipped);
    }

    /// <summary>
    /// The search_ click.
    /// </summary>
    public void OnPostSearch()
    {
        // re-bind data
        this.BindData();
    }

    /// <summary>
    /// Export all Users as XML
    /// </summary>
    public IActionResult OnPostExportUsersXml()
    {
        return this.ExportAllUsers();
    }

    /// <summary>
    /// Gets the suspended string.
    /// </summary>
    /// <param name="suspendedUntil">The suspended until.</param>
    /// <returns>Returns the suspended string</returns>
    public string GetSuspendedString(string suspendedUntil)
    {
        return suspendedUntil.IsNotSet()
                   ? this.GetText("COMMON", "NO")
                   : this.GetTextFormatted(
                       "USERSUSPENDED",
                       this.Get<IDateTimeService>().FormatDateTime(suspendedUntil.ToType<DateTime>()));
    }

    /// <summary>
    /// Get Label if user is Unapproved or Disabled (Soft-Deleted)
    /// </summary>
    /// <param name="userFlag">
    /// The user Flag.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetIsUserDisabledLabel(int userFlag)
    {
        var flag = new UserFlags(userFlag);

        // disabled (soft deleted) users first, they can be approved or not
        if (flag.IsDeleted)
        {
            return $"""<span class="badge text-bg-warning">{this.GetText("ADMIN_EDITUSER","DISABLED")}</span>""";
        }

        if (flag.IsApproved)
        {
            return string.Empty;
        }

        return $"""<span class="badge text-bg-danger">{this.GetText("NOT_APPROVED")}</span>""";
    }

    /// <summary>
    /// Initializes dropdown with options to filter results by date.
    /// </summary>
    protected void InitSinceDropdown()
    {
        var lastVisit = this.Get<ISessionService>().LastVisit;

        // the texts of my topics (the same filter), they were English text in the code and the last visit had no date
        this.SinceList = [
            new SelectListItem(
                string.Format(
                    this.GetText("MYTOPICS", "LAST_VISIT"),
                    this.Get<IDateTimeService>().FormatDateTime(
                        lastVisit.HasValue && lastVisit.Value
                        != DateTimeHelper.SqlDbMinTime()
                            ? lastVisit.Value
                            : DateTime.UtcNow)),
                "0"),
            // negative values for hours backward
            new SelectListItem(this.GetText("MYTOPICS", "LAST_HOUR"), "-1"),
            new SelectListItem(this.GetText("MYTOPICS", "LAST_TWO_HOURS"), "-2"),
            // positive values for days backward
            new SelectListItem(this.GetText("MYTOPICS", "LAST_DAY"), "1"),
            new SelectListItem(this.GetText("MYTOPICS", "LAST_TWO_DAYS"), "2"),
            new SelectListItem(this.GetText("MYTOPICS", "LAST_WEEK"), "7"),
            new SelectListItem(this.GetText("MYTOPICS", "LAST_TWO_WEEKS"), "14"),
            new SelectListItem(this.GetText("MYTOPICS", "LAST_MONTH"), "31"),
            // all time (i.e. no filter)
            new SelectListItem(this.GetText("MYTOPICS", "SHOW_ALL"), "9999")
        ];
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="g">The group to filter by (the members of a group of the groups page).</param>
    /// <param name="r">The rank to filter by (the users of a rank of the ranks page).</param>
    public void OnGet(int? g = null, int? r = null)
    {
        this.Input = new UsersInputModel
        {
            Since = "9999",

            // only a group of the board
            Group = g is > 0 && this.GetRepository<Group>().Exists(x => x.ID == g && x.BoardID == this.PageBoardContext.PageBoardID)
                        ? g.Value
                        : 0,

            // only a rank of the board
            Rank = r is > 0 && this.GetRepository<Rank>().Exists(x => x.ID == r && x.BoardID == this.PageBoardContext.PageBoardID)
                       ? r.Value
                       : 0
        };

        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// The lock accounts click.
    /// </summary>
    public async Task<IActionResult> OnPostLockAccountsAsync()
    {
        // at least one year, 0 or a negative value would lock (almost) all users
        var years = Math.Clamp(this.Input.YearsOld, 1, 100);

        var lockedCount = await this.Get<IAspNetUsersHelper>().LockInactiveAccountsAsync(DateTime.UtcNow.AddYears(-years));

        this.Get<ILogger<UsersModel>>().Log(
            this.PageBoardContext.PageUserID,
            this,
            $"{lockedCount} inactive account(s) (no visit for more than {years} year(s)) were locked by {this.PageBoardContext.PageUser.DisplayOrUserName()}",
            EventLogTypes.UserSuspended);

        this.BindData();

        return this.PageBoardContext.Notify(this.GetTextFormatted("LOCKED_ACCOUNTS", lockedCount), MessageTypes.success);
    }

    /// <summary>
    /// Called when [get import].
    /// </summary>
    /// <returns>Microsoft.AspNetCore.Mvc.PartialViewResult.</returns>
    public PartialViewResult OnGetImport()
    {
        return new PartialViewResult
               {
                   ViewName = "Dialogs/UsersImport",
                   ViewData = new ViewDataDictionary<ImportModal>(
                       this.ViewData,
                       new ImportModal())
               };
    }

    /// <summary>
    /// Adds the selected users to the role or removes them from it.
    /// </summary>
    /// <param name="add">Add (true) or remove (false).</param>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    private async Task<IActionResult> BulkRoleAsync(bool add)
    {
        // only a role of the board (the id is sent by the client)
        var role = this.BulkRole > 0 ? await this.GetRepository<Group>().GetByIdAsync(this.BulkRole) : null;

        if (role is null || role.BoardID != this.PageBoardContext.PageBoardID)
        {
            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("ADMIN_USERS", "BULK_NO_ROLE"), MessageTypes.warning);
        }

        var (users, skipped) = await this.GetSelectedUsersAsync(false);
        var count = 0;

        foreach (var user in users)
        {
            var memberships = this.GetRepository<Group>().Member(this.PageBoardContext.PageBoardID, user.Item1.ID)
                .Where(g => g.IsMember).ToList();
            var isMember = memberships.Exists(g => g.GroupID == role.ID);

            if (isMember == add)
            {
                // nothing to change
                continue;
            }

            if (!add && memberships.Count <= 1)
            {
                // a user keeps at least one role
                skipped++;
                continue;
            }

            this.GetRepository<UserGroup>().AddOrRemove(user.Item1.ID, role.ID, add);

            if (add)
            {
                if (!await this.Get<IAspNetUsersHelper>().IsUserInRoleAsync(user.Item2, role.Name))
                {
                    this.Get<IAspNetRolesHelper>().AddUserToRole(user.Item2, role.Name);
                }
            }
            else if (await this.Get<IAspNetUsersHelper>().IsUserInRoleAsync(user.Item2, role.Name))
            {
                await this.Get<IAspNetUsersHelper>().RemoveUserFromRoleAsync(user.Item2, role.Name);
            }

            await this.Get<IRaiseEventAsync>().RaiseAsync(new UpdateUserStyleEvent(user.Item1.ID));
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(user.Item1.ID));

            count++;
        }

        var name = WebUtility.HtmlEncode(role.Name);

        return this.BulkDone(
            add
                ? this.GetTextFormatted("BULK_ROLE_ADDED", count, name)
                : this.GetTextFormatted("BULK_ROLE_REMOVED", count, name),
            skipped);
    }

    /// <summary>
    /// Gets the selected users of the board that can be changed: not the guest, a host admin or yourself, the ids
    /// are sent by the client.
    /// </summary>
    /// <param name="skipAdmins">Skip the admins and (if the current user is no admin) the moderators, e.g. for the
    /// delete and the suspension like for a single user.</param>
    /// <returns>The users and the number of the skipped users.</returns>
    private async Task<(List<Tuple<User, AspNetUsers, Rank, VAccess>> Users, int Skipped)> GetSelectedUsersAsync(bool skipAdmins)
    {
        var users = new List<Tuple<User, AspNetUsers, Rank, VAccess>>();
        var skipped = 0;

        foreach (var id in (this.SelectedUsers ?? []).Distinct())
        {
            var user = id == this.PageBoardContext.PageUserID
                           ? null
                           : await this.Get<IAspNetUsersHelper>().GetBoardUserAsync(id, this.PageBoardContext.PageBoardID, true);

            if (user is null || user.Item1.UserFlags.IsGuest || user.Item1.UserFlags.IsHostAdmin
                || skipAdmins && (user.Item4.IsAdmin > 0
                                  || !this.PageBoardContext.IsAdmin && user.Item4.IsForumModerator > 0))
            {
                skipped++;
                continue;
            }

            users.Add(user);
        }

        return (users, skipped);
    }

    /// <summary>
    /// Shows the list again with the message of a bulk action (and the number of skipped users).
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="skipped">The number of skipped users.</param>
    /// <returns>The <see cref="IActionResult"/>.</returns>
    private IActionResult BulkDone(string message, int skipped)
    {
        if (skipped > 0)
        {
            message = $"{message} {this.GetTextFormatted("BULK_SKIPPED", skipped)}";
        }

        // the selection is not kept (the list can be another one)
        this.ModelState.Clear();
        this.SelectedUsers = [];

        this.BindData();

        return this.PageBoardContext.Notify(message, skipped > 0 ? MessageTypes.warning : MessageTypes.success);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        // initialize since filter items
        this.InitSinceDropdown();

        // get list of user groups for filtering
        var groups = this.GetRepository<Group>().List(boardId: this.PageBoardContext.PageBoardID);

        groups.Insert(0, new Group { Name = this.GetText("FILTER_NO"), ID = 0 });

        this.Groups = new SelectList(groups, nameof(Group.ID), nameof(Group.Name));

        // the roles of the bulk actions (without the "no filter" item)
        this.BulkRoles = new SelectList(groups.Where(g => g.ID > 0), nameof(Group.ID), nameof(Group.Name));

        this.SuspendUnits = [
            new SelectListItem(this.GetText("PROFILE", "DAYS"), "1"),
            new SelectListItem(this.GetText("PROFILE", "HOURS"), "2"),
            new SelectListItem(this.GetText("PROFILE", "MINUTES"), "3")
        ];

        // get list of user ranks for filtering
        var ranks = this.GetRepository<Rank>().GetByBoardId().OrderBy(r => r.SortOrder).ToList();

        // add empty for no filtering
        ranks.Insert(0, new Rank { Name = this.GetText("FILTER_NO"), ID = 0 });

        this.Ranks = new SelectList(ranks, nameof(Rank.ID), nameof(Rank.Name));

        // default since date is now
        DateTime? sinceDate = null;

         // default since option is "since last visit"
         var sinceValue = 0;

         // is any "since"option selected
         if (this.Input.Since.IsSet())
         {
             // get selected value (sent by the client, a value that is not a number shows all users)
             sinceValue = int.TryParse(this.Input.Since, NumberStyles.Integer, CultureInfo.InvariantCulture, out var since)
                              ? since
                              : 9999;

             sinceDate = sinceValue switch {
                 > 0 => DateTime.UtcNow.AddDays(-Math.Min(sinceValue, 36500)),
                 < 0 => DateTime.UtcNow.AddHours(Math.Max(sinceValue, -876000)),
                 _ => null
             };
         }

         // we want to filter topics since last visit
         if (sinceValue == 0)
         {
             sinceDate = this.Get<ISessionService>().LastVisit ?? DateTime.UtcNow;
         }

         // get users, eventually filter by groups or ranks
         var users = this.Get<IAspNetUsersHelper>().GetUsersPaged(
             this.PageBoardContext.PageBoardID,
             this.PageBoardContext.PageIndex,
             this.Size,
             this.Input.Name,
             this.Input.Email,
             sinceDate,
             this.Input.SuspendedOnly,
             this.Input.Group == 0 ? null : this.Input.Group,
             this.Input.Rank == 0 ? null : this.Input.Rank,
             includeNonApproved: true);

         // bind list
         this.List = users;
    }

    /// <summary>
    /// Export All Users
    /// </summary>
    private FileContentResult ExportAllUsers()
    {
        var usersList = this.GetRepository<User>().GetByBoardId(this.PageBoardContext.PageBoardID);

        return this.ExportAsXml(usersList);
    }

    /// <summary>
    /// Export As Xml
    /// </summary>
    /// <param name="usersList">
    /// The users list.
    /// </param>
    private FileContentResult ExportAsXml(IList<User> usersList)
    {
        var writer = new System.Xml.Serialization.XmlSerializer(typeof(List<User>));
        var stream = new MemoryStream();
        writer.Serialize(stream, usersList);

        var fileName = $"YafUsersExport-{HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))}.xml";

        return this.File(stream.ToArray(), "application/xml", fileName);
    }
}