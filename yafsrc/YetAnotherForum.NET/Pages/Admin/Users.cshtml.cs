
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

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Modals;
using YAF.Types.Models;
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
            // deleting yourself isn't an option
            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_USERS", "MSG_SELF_DELETE"),
                MessageTypes.danger);
        }

        // get user(s) we are about to delete
        var userToDelete = this.Get<IAspNetUsersHelper>().GetBoardUser(
            id,
            this.PageBoardContext.PageBoardID, true);

        if (userToDelete.Item1.UserFlags.IsGuest)
        {
            // we cannot delete guest
            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_USERS", "MSG_DELETE_GUEST"),
                MessageTypes.danger);
        }

        if (userToDelete.Item4.IsAdmin > 0 || userToDelete.Item1.UserFlags.IsHostAdmin)
        {
            // admin are not deletable either
            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_USERS", "MSG_DELETE_ADMIN"),
                MessageTypes.danger);
        }

        // all is good, user can be deleted
        await this.Get<IAspNetUsersHelper>().DeleteUserAsync(id);

        // rebind data
        this.BindData();

        return this.Page();
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

        if (flag.IsApproved)
        {
            return string.Empty;
        }

        if (flag.IsApproved && flag.IsDeleted)
        {
            return $"""<span class="badge text-bg-warning">{this.GetText("ADMIN_EDITUSER","DISABLED")}</span>""";
        }

        return $"""<span class="badge text-bg-danger">{this.GetText("NOT_APPROVED")}</span>""";
    }

    /// <summary>
    /// Initializes dropdown with options to filter results by date.
    /// </summary>
    protected void InitSinceDropdown()
    {
        var lastVisit = this.Get<ISessionService>().LastVisit;

        this.SinceList = [
            new SelectListItem(
                this.GetTextFormatted(
                    "last_visit",
                    this.Get<IDateTimeService>().FormatDateTime(
                        lastVisit.HasValue && lastVisit.Value
                        != DateTimeHelper.SqlDbMinTime()
                            ? lastVisit.Value
                            : DateTime.UtcNow)),
                "0"),
            // negative values for hours backward

            new SelectListItem("Last hour", "-1"),
            new SelectListItem("Last two hours", "-2"),
            // positive values for days backward
            new SelectListItem("Last day", "1"),
            new SelectListItem("Last two days", "2"),
            new SelectListItem("Last week", "7"),
            new SelectListItem("Last two weeks", "14"),
            new SelectListItem("Last month", "31"),
            // all time (i.e. no filter)
            new SelectListItem("All time", "9999")
        ];
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    public void OnGet()
    {
        this.Input = new UsersInputModel();

        // initialize since filter items
        this.InitSinceDropdown();

        // set since filter to last item "All time"
        this.Input.Since = "9999";

        // get list of user groups for filtering
        var groups = this.GetRepository<Group>().List(boardId: this.PageBoardContext.PageBoardID);

        groups.Insert(0, new Group { Name = this.GetText("FILTER_NO"), ID = 0 });

        this.Groups = new SelectList(groups, nameof(Group.ID), nameof(Group.Name));

        // get list of user ranks for filtering
        var ranks = this.GetRepository<Rank>().GetByBoardId().OrderBy(r => r.SortOrder).ToList();

        // add empty for no filtering
        ranks.Insert(0, new Rank { Name = this.GetText("FILTER_NO"), ID = 0 });

        this.Ranks = new SelectList(ranks, nameof(Rank.ID), nameof(Rank.Name));

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
    public async Task OnPostLockAccountsAsync()
    {
        await this.Get<IAspNetUsersHelper>().LockInactiveAccountsAsync(DateTime.UtcNow.AddYears(-this.Input.YearsOld));

        this.BindData();
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
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        // default since date is now
         DateTime? sinceDate = null;

         // default since option is "since last visit"
         var sinceValue = 0;

         // is any "since"option selected
         if (this.Input.Since.IsSet())
         {
             // get selected value
             sinceValue = int.Parse(this.Input.Since);

             sinceDate = sinceValue switch {
                 > 0 => DateTime.UtcNow - TimeSpan.FromDays(sinceValue),
                 < 0 => DateTime.UtcNow + TimeSpan.FromHours(sinceValue),
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
             this.Input.Rank == 0 ? null : this.Input.Rank);

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