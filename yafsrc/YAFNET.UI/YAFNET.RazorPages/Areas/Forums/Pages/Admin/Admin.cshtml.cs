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
using System.Linq;
using System.Threading.Tasks;

using FarsiLibrary.Utils;

using Microsoft.AspNetCore.Mvc.Rendering;

using MimeKit;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The Admin Index Page.
/// </summary>
public class AdminModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public AdminInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the user list.
    /// </summary>
    /// <value>The user list.</value>
    [BindProperty]
    public IList<User> UserList { get; set; }

    /// <summary>
    /// Gets or sets the active user list.
    /// </summary>
    /// <value>The active user list.</value>
    [BindProperty]
    public List<ActiveUser> ActiveUserList { get; set; }

    /// <summary>
    /// Gets or sets the update highlight.
    /// </summary>
    /// <value>The update highlight.</value>
    [TempData]
    public string UpdateHighlight { get; set; }

    /// <summary>
    /// Gets or sets the boards.
    /// </summary>
    /// <value>The boards.</value>
    public SelectList Boards { get; set; }

    /// <summary>
    /// Gets or sets the pageSize List.
    /// </summary>
    public SelectList UnverifiedPageSizeList { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminModel"/> class.
    /// </summary>
    public AdminModel()
        : base("ADMIN_ADMIN", ForumPages.Admin_Admin)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
    }

    /// <summary>
    /// Resend user approve email.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="p">The p.</param>
    /// <param name="p2">The p2.</param>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostResendEmailAsync(int id, int p, int p2)
    {
        var unApprovedUsers = this.GetRepository<User>().GetUnApprovedUsers(this.PageBoardContext.PageBoardID);

        var userUnApproved = unApprovedUsers.Find(x => x.ID == id);

        var checkMail = await this.GetRepository<CheckEmail>()
            .GetSingleAsync(mail => mail.Email == userUnApproved.Email);

        if (checkMail != null)
        {
            var verifyEmail = new TemplateEmail("VERIFYEMAIL");

            var subject = this.Get<ILocalization>().GetTextFormatted(
                "VERIFICATION_EMAIL_SUBJECT",
                this.PageBoardContext.BoardSettings.Name);

            verifyEmail.TemplateParams["{link}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
                ForumPages.Account_Approve,
                new {code = checkMail.Hash});
            verifyEmail.TemplateParams["{key}"] = checkMail.Hash;
            verifyEmail.TemplateParams["{forumname}"] = this.PageBoardContext.BoardSettings.Name;
            verifyEmail.TemplateParams["{forumlink}"] = this.Get<LinkBuilder>().ForumUrl;

            await verifyEmail.SendEmailAsync(new MailboxAddress(userUnApproved.DisplayOrUserName(), checkMail.Email), subject);

            this.BindData(p, p2);

            return this.PageBoardContext.Notify(this.GetText("ADMIN_ADMIN", "MSG_MESSAGE_SEND"), MessageTypes.success);
        }

        var userFound = this.Get<IUserDisplayName>().FindUserContainsName(userUnApproved.Name).FirstOrDefault();

        var user = await this.Get<IAspNetUsersHelper>().GetUserByNameAsync(userFound!.Name);

        await this.Get<ISendNotification>().SendVerificationEmailAsync(user, userUnApproved.Email, userFound.ID);

        this.BindData(p, p2);

        return this.Page();
    }

    /// <summary>
    /// Delete unapproved user
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="p">The p.</param>
    /// <param name="p2">The p2.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task OnPostDeleteAsync(int id, int p, int p2)
    {
        await this.Get<IAspNetUsersHelper>().DeleteUserAsync(id);

        this.BindData(p, p2);
    }

    /// <summary>
    /// Approve user.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="p">The p.</param>
    /// <param name="p2">The p2.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task OnPostApproveAsync(int id, int p, int p2)
    {
        await this.Get<IAspNetUsersHelper>().ApproveUserAsync(id);

        this.BindData(p, p2);
    }

    /// <summary>
    /// Delete all unapproved user(s).
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="p2">The p2.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task OnPostDeleteAllAsync(int p, int p2)
    {
        var daysValueAll = this.Input.DaysOld;

        await this.Get<IAspNetUsersHelper>().DeleteAllUnapprovedAsync(DateTime.UtcNow.AddDays(-daysValueAll.ToType<int>()));

        this.BindData(p, p2);
    }

    /// <summary>
    /// Approve all unapproved user(s).
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="p2">The p2.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task OnPostApproveAllAsync(int p, int p2)
    {
        await this.Get<IAspNetUsersHelper>().ApproveAllAsync();

        this.BindData(p, p2);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet(int p, int p2)
    {
        this.BindData(p, p2);
    }

    /// <summary>
    /// Called when [post].
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="p2">The p2.</param>
    public void OnPost(int p, int p2)
    {
        this.BindData(p, p2);
    }

    /// <summary>
    /// Shows the upgrade message.
    /// </summary>
    private void ShowUpgradeMessage()
    {
        try
        {
            var version = this.Get<IDataCache>().GetOrSet(
                "LatestVersion",
                () => this.Get<ILatestInformationService>().GetLatestVersionAsync().Result,
                TimeSpan.FromDays(1));

            var latestVersion = (DateTime)version.VersionDate;

            if (latestVersion.ToUniversalTime().Date <= this.Get<BoardInfo>().AppVersionDate.ToUniversalTime().Date)
            {
                return;
            }

            // updateLink
            this.UpdateHighlight = version.UpgradeUrl;
        }
        catch (Exception)
        {
            this.UpdateHighlight = null;
        }
    }

    /// <summary>
    /// Binds the active user data.
    /// </summary>
    /// <param name="p">
    /// The page index.
    /// </param>
    private void BindActiveUserData(int p)
    {
        var activeUsers = this.GetRepository<Active>().ListUsersPaged(
            this.PageBoardContext.PageUserID,
            true,
            true,
            p,
            this.Size);

        this.ActiveUserList = activeUsers;
    }

    /// <summary>
    /// Bind list of boards to drop down
    /// </summary>
    private void BindBoardsList()
    {
        // only if user is host admin, otherwise boards' list is hidden
        if (!this.PageBoardContext.PageUser.UserFlags.IsHostAdmin)
        {
            return;
        }

        var boards = this.GetRepository<Board>().GetAll();

        this.Boards = new SelectList(boards, nameof(Board.ID), nameof(Board.Name), this.PageBoardContext.PageBoardID);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData(int p, int p2)
    {
        this.Input = new AdminInputModel {
                                        SelectedBoardId = this.PageBoardContext.PageBoardID,
                                        UnverifiedPageSize = this.PageBoardContext.PageUser.PageSize
                                    };

        this.PageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.UnverifiedPageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.BindBoardsList();

        this.ShowUpgradeMessage();

        this.BindUnverifiedUsers(p2);

        // get stats for current board, selected board or all boards (see function)
        var data = this.GetRepository<Board>().Stats(this.Input.SelectedBoardId);

        this.Input.NumCategories = data.Categories;
        this.Input.NumForums = data.Forums;
        this.Input.NumTopics = $"{data.Topics:N0}";
        this.Input.NumPosts = $"{data.Posts:N0}";
        this.Input.NumUsers = $"{data.Users:N0}";

        var span = DateTime.UtcNow - data.BoardStart;
        double days = span.Days;

        this.Input.BoardStart = this.Get<IDateTimeService>().FormatDateTimeTopic(
            this.PageBoardContext.BoardSettings.UseFarsiCalender
                ? PersianDateConverter.ToPersianDate(data.BoardStart)
                : data.BoardStart);

        this.Input.BoardStartAgo = data.BoardStart;

        if (days < 1)
        {
            days = 1;
        }

        this.Input.DayPosts = $"{data.Posts / days:N2}";
        this.Input.DayTopics = $"{data.Topics / days:N2}";
        this.Input.DayUsers = $"{data.Users / days:N2}";

        try
        {
            this.Input.DBSize = $"{this.Get<IDbAccess>().GetDatabaseSize()} MB";
        }
        catch (Exception)
        {
            this.Input.DBSize = this.GetText("ADMIN_ADMIN", "ERROR_DATABASESIZE");
        }

        this.BindActiveUserData(p);
    }

    /// <summary>
    /// Bind unverified users.
    /// </summary>
    /// <param name="p2">
    /// The page index.
    /// </param>
    private void BindUnverifiedUsers(int p2)
    {
        var unverifiedUsers = this.GetRepository<User>().GetUnApprovedUsers(this.PageBoardContext.PageBoardID);

        var pager = new Paging {
                                   CurrentPageIndex = p2 - 1,
                                   PageSize = this.Input.UnverifiedPageSize,
                                   Count = unverifiedUsers.Count
                               };

        this.Input.UnverifiedCount = unverifiedUsers.Count;

        // bind list
        this.UserList = unverifiedUsers.GetPaged(pager);
    }
}