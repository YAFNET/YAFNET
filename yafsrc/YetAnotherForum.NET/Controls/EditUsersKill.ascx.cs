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

using YAF.Types.Models;

namespace YAF.Controls;

using YAF.Core.Services.CheckForSpam;
using YAF.Types.Models.Identity;

using ListItem = ListItem;

/// <summary>
/// The edit users kill.
/// </summary>
public partial class EditUsersKill : BaseUserControl
{
    /// <summary>
    ///   The _all posts by user.
    /// </summary>
    private IOrderedEnumerable<Message> allPostsByUser;

    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    [NotNull]
    public Tuple<User, AspNetUsers, Rank, vaccess> User { get; set; }

    /// <summary>
    ///   Gets AllPostsByUser.
    /// </summary>
    public IOrderedEnumerable<Message> AllPostsByUser =>
        this.allPostsByUser ??= this.GetRepository<Message>().GetAllUserMessages(this.User.Item1.ID);

    /// <summary>
    ///   Gets IPAddresses.
    /// </summary>
    [NotNull]
    public List<string> IPAddresses
    {
        get
        {
            var list = this.AllPostsByUser.Select(m => m.IP).OrderBy(x => x).Distinct().ToList();

            if (list.Count.Equals(0))
            {
                list.Add(this.User.Item1.IP);
            }

            return list;
        }
    }

    /// <summary>
    /// Gets or sets the current user.
    /// </summary>
    /// <value>
    /// The current user.
    /// </value>
    private User CurrentUser { get; set; }

    /// <summary>
    /// Kills the PageUser
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Kill_OnClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        // Ban User Email?
        if (this.BanEmail.Checked)
        {
            this.GetRepository<BannedEmail>().Save(
                null,
                this.User.Item1.Email,
                $"Email was reported by: {this.PageBoardContext.PageUser.DisplayOrUserName()}");
        }

        // Ban User IP?
        if (this.BanIps.Checked && this.IPAddresses.Any())
        {
            this.BanUserIps();
        }

        // Ban User IP?
        if (this.BanName.Checked)
        {
            this.GetRepository<BannedName>().Save(
                null,
                this.User.Item1.Name,
                $"Name was reported by: {this.PageBoardContext.PageUser.DisplayOrUserName()}");
        }

        this.DeleteAllUserMessages();

        if (this.ReportUser.Checked && this.PageBoardContext.BoardSettings.StopForumSpamApiKey.IsSet() &&
            this.IPAddresses.Any())
        {
            try
            {
                var stopForumSpam = new StopForumSpam();

                if (stopForumSpam.ReportUserAsBot(this.IPAddresses.FirstOrDefault(), this.User.Item1.Email, this.User.Item1.Name))
                {
                    this.GetRepository<Registry>().IncrementReportedSpammers();

                    this.Logger.Log(
                        this.PageBoardContext.PageUserID,
                        "User Reported to StopForumSpam.com",
                        $"User (Name:{this.User.Item1.Name}/ID:{this.User.Item1.ID}/IP:{this.IPAddresses.FirstOrDefault()}/Email:{this.User.Item1.Email}) Reported to StopForumSpam.com by {this.PageBoardContext.PageUser.DisplayOrUserName()}",
                        EventLogTypes.SpamBotReported);
                }
            }
            catch (Exception exception)
            {
                this.PageBoardContext.Notify(
                    this.GetText("ADMIN_EDITUSER", "BOT_REPORTED_FAILED"),
                    MessageTypes.danger);

                this.Logger.Log(
                    this.PageBoardContext.PageUserID,
                    $"User (Name{this.User.Item1.Name}/ID:{this.User.Item1.ID}) Report to StopForumSpam.com Failed",
                    exception);
            }
        }

        switch (this.SuspendOrDelete.SelectedValue)
        {
            case "delete":
                if (this.User.Item1.ID > 0)
                {
                    // we are deleting user
                    if (this.PageBoardContext.PageUserID == this.User.Item1.ID)
                    {
                        // deleting yourself isn't an option
                        this.PageBoardContext.Notify(
                            this.GetText("ADMIN_USERS", "MSG_SELF_DELETE"),
                            MessageTypes.danger);
                        return;
                    }

                    // get user(s) we are about to delete
                    if (this.User.Item1.UserFlags.IsGuest)
                    {
                        // we cannot delete guest
                        this.PageBoardContext.Notify(
                            this.GetText("ADMIN_USERS", "MSG_DELETE_GUEST"),
                            MessageTypes.danger);
                        return;
                    }

                    if (this.User.Item4.IsAdmin == 1 ||
                        this.User.Item1.UserFlags.IsHostAdmin)
                    {
                        // admin are not deletable either
                        this.PageBoardContext.Notify(
                            this.GetText("ADMIN_USERS", "MSG_DELETE_ADMIN"),
                            MessageTypes.danger);

                        return;
                    }

                    // all is good, user can be deleted
                    this.Get<IAspNetUsersHelper>().DeleteUser(this.User.Item1.ID);

                    this.PageBoardContext.LoadMessage.AddSession(
                        this.GetTextFormatted("MSG_USER_KILLED", this.User.Item1.Name),
                        MessageTypes.success);

                    this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Users);
                }

                break;
            case "suspend":
                if (this.User.Item1.ID > 0)
                {
                    this.GetRepository<User>().Suspend(
                        this.User.Item1.ID,
                        DateTime.UtcNow.AddYears(5));
                }

                break;
        }

        // update the displayed data...
        this.BindData();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        // this needs to be done just once, not during post-backs
        if (this.IsPostBack)
        {
            return;
        }

        this.SuspendOrDelete.Items.Add(new ListItem(this.GetText("ADMIN_EDITUSER", "DELETE_ACCOUNT"), "delete"));
        this.SuspendOrDelete.Items.Add(
            new ListItem(this.GetText("ADMIN_EDITUSER", "SUSPEND_ACCOUNT_USER"), "suspend"));

        this.SuspendOrDelete.Items[0].Selected = true;

        // bind data
        this.BindData();
    }

    /// <summary>
    /// Bans the user IP Addresses.
    /// </summary>
    private void BanUserIps()
    {
        var allIps = this.GetRepository<BannedIP>().Get(x => x.BoardID == this.PageBoardContext.PageBoardID)
            .Select(x => x.Mask).ToList();

        // ban user ips...
        var name = this.User.Item1.DisplayOrUserName();

        this.IPAddresses.Except(allIps).ToList().Where(i => i.IsSet()).ForEach(
            ip =>
                {
                    var linkUserBan = this.Get<ILocalization>().GetTextFormatted(
                        "ADMIN_EDITUSER",
                        "LINK_USER_BAN",
                        this.User.Item1.ID,
                        this.Get<LinkBuilder>().GetUserProfileLink(this.User.Item1.ID, name),
                        this.HtmlEncode(name));

                    this.GetRepository<BannedIP>().Save(null, ip, linkUserBan, this.PageBoardContext.PageUserID);
                });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.ViewPostsLink.NavigateUrl = this.Get<LinkBuilder>().GetLink(
            ForumPages.Search,
            new
                {
                    postedby = !this.User.Item1.UserFlags.IsGuest
                                   ? this.User.Item1.DisplayOrUserName()
                                   : this.Get<IAspNetUsersHelper>().GuestUser(this.PageBoardContext.PageBoardID).Name
                });

        this.ReportUserRow.Visible = this.PageBoardContext.BoardSettings.StopForumSpamApiKey.IsSet();

        // load ip address history for user...
        this.IPAddresses.Take(5).ForEach(
            ipAddress => this.IpAddresses.Text +=
                             $@"<a href=""{string.Format(this.PageBoardContext.BoardSettings.IPInfoPageURL, ipAddress)}""
                                       target=""_blank""
                                       title=""{this.GetText("COMMON", "TT_IPDETAILS")}"">
                                       {ipAddress}
                                    </a>
                                    <br />");

        // if no ip disable BanIp checkbox
        if (!this.IPAddresses.Any())
        {
            this.BanIps.Checked = false;
            this.BanIps.Enabled = false;
            this.ReportUserRow.Visible = false;
        }

        // show post count...
        this.PostCount.Text = this.AllPostsByUser.Count().ToString();

        // get user's info
        this.CurrentUser = this.GetRepository<User>().GetById(
            this.User.Item1.ID);

        if (this.CurrentUser.Suspended.HasValue)
        {
            this.SuspendedTo.Visible = true;

            // is user suspended?
            this.SuspendedTo.Text =
                $"<div class=\"alert alert-info\" role=\"alert\">{this.GetText("PROFILE", "ENDS")} {this.Get<IDateTimeService>().FormatDateTime(this.CurrentUser.Suspended.Value)}</div>";
        }

        this.DataBind();
    }

    /// <summary>
    /// Deletes all user messages.
    /// </summary>
    private void DeleteAllUserMessages()
    {
        // delete posts...
        var messages = this.AllPostsByUser.Distinct().ToList();

        messages.ForEach(
            x => this.GetRepository<Message>().Delete(
                x.Topic.ForumID,
                x.Topic.ID,
                x.ID,
                true,
                string.Empty,
                true,
                true,
                true));
    }
}