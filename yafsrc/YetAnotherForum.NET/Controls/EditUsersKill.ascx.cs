/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Services.CheckForSpam;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;

    using DateTime = System.DateTime;
    using ListItem = System.Web.UI.WebControls.ListItem;

    #endregion

    /// <summary>
    /// The edit users kill.
    /// </summary>
    public partial class EditUsersKill : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _all posts by user.
        /// </summary>
        private IOrderedEnumerable<Tuple<Message, Topic>> allPostsByUser;

        /// <summary>
        /// The user.
        /// </summary>
        private Tuple<User, AspNetUsers, Rank, vaccess> user;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets AllPostsByUser.
        /// </summary>
        public IOrderedEnumerable<Tuple<Message, Topic>> AllPostsByUser =>
            this.allPostsByUser ??= this.GetRepository<Message>().GetAllUserMessages(this.CurrentUserId);

        /// <summary>
        ///   Gets IPAddresses.
        /// </summary>
        [NotNull]
        public List<string> IPAddresses
        {
            get
            {
                var list = this.AllPostsByUser.Select(m => m.Item1.IP).OrderBy(x => x).Distinct().ToList();

                if (list.Count.Equals(0))
                {
                    list.Add(this.User.Item1.IP);
                }

                return list;
            }
        }

        /// <summary>
        /// Gets the User Data.
        /// </summary>
        [NotNull]
        private Tuple<User, AspNetUsers, Rank, vaccess> User => this.user ??= this.GetRepository<User>().GetBoardUser(this.CurrentUserId);

        /// <summary>
        ///   Gets CurrentUserID.
        /// </summary>
        protected int CurrentUserId => this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        /// <value>
        /// The current user.
        /// </value>
        private User CurrentUser { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Kills the User
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
                    $"Email was reported by: {this.PageContext.User.DisplayOrUserName()}");
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
                    $"Name was reported by: {this.PageContext.User.DisplayOrUserName()}");
            }

            this.DeleteAllUserMessages();

            if (this.ReportUser.Checked && this.PageContext.BoardSettings.StopForumSpamApiKey.IsSet() &&
                this.IPAddresses.Any())
            {
                try
                {
                    var stopForumSpam = new StopForumSpam();

                    if (stopForumSpam.ReportUserAsBot(this.IPAddresses.FirstOrDefault(), this.User.Item1.Email, this.User.Item1.Name))
                    {
                        this.GetRepository<Registry>().IncrementReportedSpammers();

                        this.Logger.Log(
                            this.PageContext.PageUserID,
                            "User Reported to StopForumSpam.com",
                            $"User (Name:{this.User.Item1.Name}/ID:{this.CurrentUserId}/IP:{this.IPAddresses.FirstOrDefault()}/Email:{this.User.Item1.Email}) Reported to StopForumSpam.com by {this.PageContext.User.DisplayOrUserName()}",
                            EventLogTypes.SpamBotReported);
                    }
                }
                catch (Exception exception)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITUSER", "BOT_REPORTED_FAILED"),
                        MessageTypes.danger);

                    this.Logger.Log(
                        this.PageContext.PageUserID,
                        $"User (Name{this.User.Item1.Name}/ID:{this.CurrentUserId}) Report to StopForumSpam.com Failed",
                        exception);
                }
            }

            switch (this.SuspendOrDelete.SelectedValue)
            {
                case "delete":
                    if (this.CurrentUserId > 0)
                    {
                        // we are deleting user
                        if (this.PageContext.PageUserID == this.CurrentUserId)
                        {
                            // deleting yourself isn't an option
                            this.PageContext.AddLoadMessage(
                                this.GetText("ADMIN_USERS", "MSG_SELF_DELETE"),
                                MessageTypes.danger);
                            return;
                        }

                        // get user(s) we are about to delete
                        if (this.User.Item1.IsGuest.Value)
                        {
                            // we cannot delete guest
                            this.PageContext.AddLoadMessage(
                                this.GetText("ADMIN_USERS", "MSG_DELETE_GUEST"),
                                MessageTypes.danger);
                            return;
                        }

                        if (!this.User.Item4.IsAdmin &&
                            !this.User.Item1.UserFlags.IsHostAdmin)
                        {
                            return;
                        }

                        // admin are not deletable either
                        this.PageContext.AddLoadMessage(
                            this.GetText("ADMIN_USERS", "MSG_DELETE_ADMIN"),
                            MessageTypes.danger);


                        // all is good, user can be deleted
                        this.Get<IAspNetUsersHelper>().DeleteUser(this.CurrentUserId);

                        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Users);
                    }

                    break;
                case "suspend":
                    if (this.CurrentUserId > 0)
                    {
                        this.GetRepository<User>().Suspend(
                            this.CurrentUserId,
                            DateTime.UtcNow.AddYears(5));
                    }

                    break;
            }

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted("MSG_USER_KILLED", this.User.Item1.Name),
                MessageTypes.success);

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
            var allIps = this.GetRepository<BannedIP>().Get(x => x.BoardID == this.PageContext.PageBoardID)
                .Select(x => x.Mask).ToList();

            // ban user ips...
            var name = this.User.Item1.DisplayOrUserName();

            this.IPAddresses.Except(allIps).ToList().Where(i => i.IsSet()).ForEach(
                ip =>
                {
                    var linkUserBan = this.Get<ILocalization>().GetTextFormatted(
                        "ADMIN_EDITUSER",
                        "LINK_USER_BAN",
                        this.CurrentUserId,
                        this.Get<LinkBuilder>().GetUserProfileLink(this.CurrentUserId, name),
                        this.HtmlEncode(name));

                    this.GetRepository<BannedIP>().Save(null, ip, linkUserBan, this.PageContext.PageUserID);
                });
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.ViewPostsLink.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                ForumPages.Search,
                "postedby={0}",
                !this.User.Item1.IsGuest.Value
                    ? this.User.Item1.DisplayOrUserName()
                    : this.Get<IAspNetUsersHelper>().GuestUserName);

            this.ReportUserRow.Visible = this.PageContext.BoardSettings.StopForumSpamApiKey.IsSet();

            // load ip address history for user...
            this.IPAddresses.Take(5).ForEach(
                ipAddress => this.IpAddresses.Text +=
                                 $@"<a href=""{string.Format(this.PageContext.BoardSettings.IPInfoPageURL, ipAddress)}""
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
                this.CurrentUserId);

            if (this.CurrentUser.Suspended.HasValue)
            {
                this.SuspendedTo.Visible = true;

                // is user suspended?
                this.SuspendedTo.Text =
                    $"<div class=\"alert alert-info\" role=\"alert\">{this.GetText("PROFILE", "ENDS")} {this.Get<IDateTime>().FormatDateTime(this.CurrentUser.Suspended)}</div>";
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
                    x.Item2.ForumID,
                    x.Item2.ID,
                    x.Item1.ID,
                    true,
                    string.Empty,
                    true,
                    true,
                    true));
        }

        #endregion
    }
}