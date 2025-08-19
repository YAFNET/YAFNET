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

namespace YAF.Pages.Admin.EditUser;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services.CheckForSpam;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Class UsersKillModel.
/// Implements the <see cref="YAF.Core.BasePages.AdminPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.AdminPage" />
public class UsersKillModel : AdminPage
{
    /// <summary>
    ///   The _all posts by user.
    /// </summary>
    private IOrderedEnumerable<Message> allPostsByUser;

    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    public User EditUser { get; set; }

    /// <summary>
    /// Gets or sets the suspend or delete list.
    /// </summary>
    /// <value>The suspend or delete list.</value>
    [BindProperty]
    public List<SelectListItem> SuspendOrDeleteList { get; set; }

    /// <summary>
    ///   Gets AllPostsByUser.
    /// </summary>
    public IOrderedEnumerable<Message> AllPostsByUser =>
        this.allPostsByUser ??= this.GetRepository<Message>().GetAllUserMessages(this.EditUser.ID);

    /// <summary>
    ///   Gets the IPAddresses.
    /// </summary>
    public List<string> IpAddresses {
        get {
            var list = this.AllPostsByUser.Select(m => m.IP).OrderBy(x => x).Distinct().ToList();

            if (list.Count.Equals(0))
            {
                list.Add(this.EditUser.IP);
            }

            return list;
        }
    }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersKillInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersKillModel"/> class.
    /// </summary>
    public UsersKillModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.Input = new UsersKillInputModel
        {
                         UserId = userId
                     };

        this.BindData(userId);

        return this.Page();
    }

    /// <summary>
    /// Kills the User
    /// </summary>
    public async Task<IActionResult> OnPostKillAsync()
    {
        var user =
            this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] as
                Tuple<User, AspNetUsers, Rank, VAccess>;

        this.EditUser = user.Item1;

        // Ban User Email?
        if (this.Input.BanEmail)
        {
            this.GetRepository<BannedEmail>().Save(
                null,
                user.Item1.Email,
                $"Email was reported by: {this.PageBoardContext.PageUser.DisplayOrUserName()}");
        }

        // Ban User IP?
        if (this.Input.BanIps && this.IpAddresses.Count != 0)
        {
            this.BanUserIps(user.Item1);
        }

        // Ban User IP?
        if (this.Input.BanName)
        {
            this.GetRepository<BannedName>().Save(
                null,
                user.Item1.Name,
                $"Name was reported by: {this.PageBoardContext.PageUser.DisplayOrUserName()}");
        }

        await this.DeleteAllUserMessagesAsync();

        if (this.Input.ReportUser && this.PageBoardContext.BoardSettings.StopForumSpamApiKey.IsSet() &&
            this.IpAddresses.Count != 0)
        {
            try
            {
                var stopForumSpam = new StopForumSpam();

                if (await stopForumSpam.ReportUserAsBotAsync(this.IpAddresses.FirstOrDefault(), user.Item1.Email, user.Item1.Name).ConfigureAwait(false))
                {
                    this.GetRepository<Registry>().IncrementReportedSpammers();

                    this.Get<ILogger<UsersKillModel>>().Log(
                        this.PageBoardContext.PageUserID,
                        "User Reported to StopForumSpam.com",
                        $"User (Name:{user.Item1.Name}/ID:{user.Item1.ID}/IP:{this.IpAddresses.FirstOrDefault()}/Email:{user.Item1.Email}) Reported to StopForumSpam.com by {this.PageBoardContext.PageUser.DisplayOrUserName()}",
                        EventLogTypes.SpamBotReported);
                }
            }
            catch (Exception exception)
            {
                this.Get<ILogger<UsersKillModel>>().Log(
                    this.PageBoardContext.PageUserID,
                    $"User (Name{user.Item1.Name}/ID:{user.Item1.ID}) Report to StopForumSpam.com Failed",
                    exception);

                this.PageBoardContext.SessionNotify(
                    this.GetText("ADMIN_EDITUSER", "BOT_REPORTED_FAILED"),
                    MessageTypes.danger);

                return this.Get<ILinkBuilder>().Redirect(
                    ForumPages.Admin_EditUser,
                    new { u = this.Input.UserId, tab = "View9" });
            }
        }

        switch (this.Input.SuspendOrDelete)
        {
            case "delete":
                if (user.Item1.ID > 0)
                {
                    // we are deleting user
                    if (this.PageBoardContext.PageUserID == user.Item1.ID)
                    {
                        // deleting yourself isn't an option
                        this.PageBoardContext.SessionNotify(
                            this.GetText("ADMIN_USERS", "MSG_SELF_DELETE"),
                            MessageTypes.danger);

                        return this.Get<ILinkBuilder>().Redirect(
                            ForumPages.Admin_EditUser,
                            new { u = this.Input.UserId, tab = "View9" });
                    }

                    // get user(s) we are about to delete
                    if (user.Item1.UserFlags.IsGuest)
                    {
                        // we cannot delete guest
                        this.PageBoardContext.SessionNotify(
                            this.GetText("ADMIN_USERS", "MSG_DELETE_GUEST"),
                            MessageTypes.danger);

                        return this.Get<ILinkBuilder>().Redirect(
                            ForumPages.Admin_EditUser,
                            new { u = this.Input.UserId, tab = "View9" });
                    }

                    if (user.Item1.UserFlags.IsHostAdmin)
                    {
                        // admin are not deletable either
                        this.PageBoardContext.SessionNotify(
                            this.GetText("ADMIN_USERS", "MSG_DELETE_ADMIN"),
                            MessageTypes.danger);

                        return this.Get<ILinkBuilder>().Redirect(
                            ForumPages.Admin_EditUser,
                            new {u = this.Input.UserId, tab = "View9"});
                    }

                    // all is good, user can be deleted
                    await this.Get<IAspNetUsersHelper>().DeleteUserAsync(this.Input.UserId);

                    this.PageBoardContext.SessionNotify(
                        this.GetTextFormatted("MSG_USER_KILLED", user.Item1.Name),
                        MessageTypes.success);

                    return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Users);
                }

                break;
            case "suspend":
                if (this.Input.UserId > 0)
                {
                    await this.GetRepository<User>().SuspendAsync(
                        this.Input.UserId,
                        DateTime.UtcNow.AddYears(5));
                }

                break;
        }

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View9" });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData(int userId)
    {
        var user =
            this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] as
                Tuple<User, AspNetUsers, Rank, VAccess>;

        this.EditUser = user.Item1;

        this.SuspendOrDeleteList = [
            new SelectListItem(
                this.GetText("ADMIN_EDITUSER", "DELETE_ACCOUNT"),
                "delete"),

            new SelectListItem(
                this.GetText(
                    "ADMIN_EDITUSER",
                    "SUSPEND_ACCOUNT_USER"),
                "suspend")
        ];
    }

    /// <summary>
    /// Deletes all user messages.
    /// </summary>
    private async Task DeleteAllUserMessagesAsync()
    {
        // delete posts...
        var messages = this.AllPostsByUser.Distinct().ToList();

        foreach (var x in messages)
        {
            await this.GetRepository<Message>().DeleteAsync(
                x.Topic.ForumID,
                x.Topic.ID,
                x,
                true,
                string.Empty,
                true,
                true);
        }
    }

    /// <summary>
    /// Bans the user IP Addresses.
    /// </summary>
    private void BanUserIps(User user)
    {
        var allIps = this.GetRepository<BannedIP>().Get(x => x.BoardID == this.PageBoardContext.PageBoardID)
            .Select(x => x.Mask).ToList();

        // ban user ips...
        var name = user.DisplayOrUserName();

        this.IpAddresses.Except(allIps).Where(i => i.IsSet()).ForEach(
            ip =>
            {
                var linkUserBan = this.Get<ILocalization>().GetTextFormatted(
                    "ADMIN_EDITUSER",
                    "LINK_USER_BAN",
                    user.ID,
                    this.Get<ILinkBuilder>().GetUserProfileLink(user.ID, name),
                    this.HtmlEncode(name));

                this.GetRepository<BannedIP>().Save(null, ip, linkUserBan, this.PageBoardContext.PageUserID);
            });
    }
}