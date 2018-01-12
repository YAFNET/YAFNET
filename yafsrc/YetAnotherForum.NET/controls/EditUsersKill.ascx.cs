/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services.CheckForSpam;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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
        private DataTable allPostsByUser;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets AllPostsByUser.
        /// </summary>
        public DataTable AllPostsByUser
            =>
                this.allPostsByUser
                ?? (this.allPostsByUser =
                    LegacyDb.post_alluser_simple(this.PageContext.PageBoardID, this.CurrentUserId));

        /// <summary>
        ///   Gets IPAddresses.
        /// </summary>
        [NotNull]
        public List<string> IPAddresses
        {
            get
            {
                var list = this.AllPostsByUser.GetColumnAsList<string>("IP").OrderBy(x => x).Distinct().ToList();

                if (list.Count.Equals(0))
                {
                    list.Add(this.CurrentUserDataHelper.LastIP);
                }

                return list;
            }
        }

        /// <summary>
        /// Gets the current user data helper.
        /// </summary>
        /// <value>
        /// The current user data helper.
        /// </value>
        public CombinedUserDataHelper CurrentUserDataHelper
        {
            get
            {
                var user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserId);
                var currentUserId = this.CurrentUserId;

                return currentUserId != null
                           ? new CombinedUserDataHelper(user, currentUserId.Value.ToType<int>())
                           : null;
            }
        }

        /// <summary>
        ///   Gets CurrentUserID.
        /// </summary>
        protected long? CurrentUserId => this.PageContext.QueryIDs["u"];

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        /// <value>
        /// The current user.
        /// </value>
        private TypedUserList CurrentUser { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Kills the User
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Kill_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            var user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserId);

            // Ban User Email?
            if (this.BanEmail.Checked)
            {
                this.GetRepository<BannedEmail>()
                    .Save(
                        null,
                        user.Email,
                        "Email was reported by: {0}".FormatWith(
                            this.Get<YafBoardSettings>().EnableDisplayName
                                ? this.PageContext.CurrentUserData.DisplayName
                                : this.PageContext.CurrentUserData.UserName));
            }

            // Ban User IP?
            if (this.BanIps.Checked && this.IPAddresses.Any())
            {
                this.BanUserIps();
            }

            // Ban User IP?
            if (this.BanName.Checked)
            {
                this.GetRepository<BannedName>()
                    .Save(
                        null,
                        user.UserName,
                        "Name was reported by: {0}".FormatWith(
                            this.Get<YafBoardSettings>().EnableDisplayName
                                ? this.PageContext.CurrentUserData.DisplayName
                                : this.PageContext.CurrentUserData.UserName));
            }

            this.DeleteAllUserMessages();

            if (this.ReportUser.Checked && this.Get<YafBoardSettings>().StopForumSpamApiKey.IsSet()
                && this.IPAddresses.Any())
            {
                try
                {
                    var stopForumSpam = new StopForumSpam();

                    if (!stopForumSpam.ReportUserAsBot(this.IPAddresses.FirstOrDefault(), user.Email, user.UserName))
                    {
                        this.Logger.Log(
                            this.PageContext.PageUserID,
                            "User Reported to StopForumSpam.com",
                            "User (Name:{0}/ID:{1}/IP:{2}/Email:{3}) Reported to StopForumSpam.com by {4}".FormatWith(
                                user.UserName,
                                this.CurrentUserId,
                                this.IPAddresses.FirstOrDefault(),
                                user.Email,
                                this.Get<YafBoardSettings>().EnableDisplayName
                                    ? this.PageContext.CurrentUserData.DisplayName
                                    : this.PageContext.CurrentUserData.UserName),
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
                        "User (Name{0}/ID:{1}) Report to StopForumSpam.com Failed".FormatWith(
                            user.UserName,
                            this.CurrentUserId),
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
                        using (
                            var dt = LegacyDb.user_list(
                                this.PageContext.PageBoardID,
                                this.CurrentUserId,
                                DBNull.Value))
                        {
                            // examine each if he's possible to delete
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["IsGuest"].ToType<int>() > 0)
                                {
                                    // we cannot detele guest
                                    this.PageContext.AddLoadMessage(
                                        this.GetText("ADMIN_USERS", "MSG_DELETE_GUEST"),
                                        MessageTypes.danger);
                                    return;
                                }

                                if ((row["IsAdmin"] == DBNull.Value || row["IsAdmin"].ToType<int>() <= 0)
                                    && (row["IsHostAdmin"] == DBNull.Value || row["IsHostAdmin"].ToType<int>() <= 0))
                                {
                                    continue;
                                }

                                // admin are not deletable either
                                this.PageContext.AddLoadMessage(
                                    this.GetText("ADMIN_USERS", "MSG_DELETE_ADMIN"),
                                    MessageTypes.danger);
                                return;
                            }
                        }

                        // all is good, user can be deleted
                        UserMembershipHelper.DeleteUser(this.CurrentUserId.ToType<int>());

                        YafBuildLink.Redirect(ForumPages.admin_users);
                    }

                    break;
                case "suspend":
                    if (this.CurrentUserId > 0)
                    {
                        this.GetRepository<User>().Suspend(this.CurrentUserId.ToType<int>(), DateTime.UtcNow.AddYears(5));
                    }

                    break;
            }

            this.PageContext.AddLoadMessage(
                this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_USER_KILLED").FormatWith(user.UserName));

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
            // init ids...
            this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            // this needs to be done just once, not during postbacks
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
            var allIps = this.GetRepository<BannedIP>().Get(x => x.BoardID == this.PageContext.PageBoardID).Select(x => x.Mask).ToList();

            // ban user ips...
            var name = UserMembershipHelper.GetDisplayNameFromID(this.CurrentUserId?.ToType<int>() ?? -1);

            if (name.IsNotSet())
            {
                name = UserMembershipHelper.GetUserNameFromID(this.CurrentUserId?.ToType<int>() ?? -1);
            }

            foreach (var ip in this.IPAddresses.Except(allIps).ToList())
            {
                if (!ip.IsSet())
                {
                    continue;
                }

                var linkUserBan =
                    this.Get<ILocalization>()
                        .GetText("ADMIN_EDITUSER", "LINK_USER_BAN")
                        .FormatWith(
                            this.CurrentUserId,
                            YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", this.CurrentUserId, name),
                            this.HtmlEncode(name));

                this.GetRepository<BannedIP>().Save(null, ip, linkUserBan, this.PageContext.PageUserID);
            }

            // Clear cache
            this.Get<IDataCache>().Remove(Constants.Cache.BannedIP);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.ViewPostsLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.search,
                "postedby={0}",
                !this.CurrentUserDataHelper.IsGuest
                    ? (this.Get<YafBoardSettings>().EnableDisplayName
                           ? this.CurrentUserDataHelper.DisplayName
                           : this.CurrentUserDataHelper.UserName)
                    : UserMembershipHelper.GuestUserName);

            this.ReportUserRow.Visible = this.Get<YafBoardSettings>().StopForumSpamApiKey.IsSet();

            // load ip address history for user...
            foreach (var ipAddress in this.IPAddresses.Take(5))
            {
                this.IpAddresses.Text +=
                    "<a href=\"{0}\" target=\"_blank\" title=\"{1}\">{2}</a><br />".FormatWith(
                        this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(ipAddress),
                        this.GetText("COMMON", "TT_IPDETAILS"),
                        ipAddress);
            }

            // if no ip disable BanIp checkbox
            if (!this.IPAddresses.Any())
            {
                this.BanIps.Checked = false;
                this.BanIps.Enabled = false;
                this.ReportUserRow.Visible = false;
            }

            // show post count...
            this.PostCount.Text = this.AllPostsByUser.Rows.Count.ToString();

            // get user's info
            this.CurrentUser =
                LegacyDb.UserList(
                    this.PageContext.PageBoardID,
                    this.CurrentUserId.ToType<int?>(),
                    null,
                    null,
                    null,
                    false).FirstOrDefault();

            // there is no such user
            if (this.CurrentUser?.Suspended != null)
            {
                this.SuspendedTo.Visible = true;

                // is user suspended?
                this.SuspendedTo.Text = "({0} {1})".FormatWith(
                    this.GetText("PROFILE", "ENDS"),
                    this.Get<IDateTime>().FormatDateTime(this.CurrentUser.Suspended));
            }

            this.DataBind();
        }

        /// <summary>
        /// Deletes all user messages.
        /// </summary>
        private void DeleteAllUserMessages()
        {
            // delete posts...
            var messageIds =
                (from m in this.AllPostsByUser.AsEnumerable() select m.Field<int>("MessageID")).Distinct().ToList();

            messageIds.ForEach(x => LegacyDb.message_delete(x, true, string.Empty, 1, true, true));
        }

        #endregion
    }
}