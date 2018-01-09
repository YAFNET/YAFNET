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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net.Mail;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using FarsiLibrary.Utils;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Admin Index Page.
    /// </summary>
    public partial class admin : AdminPage
    {
        #region Public Methods

        /// <summary>
        /// Loads the Board Stats for the Selected Board
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void BoardStatsSelectChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Handles the ItemCommand event of the UserList control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        public void UserListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", e.CommandArgument);
                    break;
                case "resendEmail":
                    var commandArgument = e.CommandArgument.ToString().Split(';');

                    var checkMail = this.GetRepository<CheckEmail>().ListTyped(commandArgument[0]).FirstOrDefault();

                    if (checkMail != null)
                    {
                        var verifyEmail = new YafTemplateEmail("VERIFYEMAIL");

                        var subject = this.Get<ILocalization>()
                            .GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", this.Get<YafBoardSettings>().Name);

                        verifyEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(
                            ForumPages.approve,
                            true,
                            "k={0}",
                            checkMail.Hash);
                        verifyEmail.TemplateParams["{key}"] = checkMail.Hash;
                        verifyEmail.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;
                        verifyEmail.TemplateParams["{forumlink}"] = YafForumInfo.ForumURL;

                        verifyEmail.SendEmail(new MailAddress(checkMail.Email, commandArgument[1]), subject, true);

                        this.PageContext.AddLoadMessage(this.GetText("ADMIN_ADMIN", "MSG_MESSAGE_SEND"));
                    }
                    else
                    {
                        var userFound = this.Get<IUserDisplayName>().Find(commandArgument[1]).FirstOrDefault();

                        var user = this.Get<MembershipProvider>().GetUser(userFound.Value, false);

                        this.Get<ISendNotification>().SendVerificationEmail(user, commandArgument[0], userFound.Key);
                    }

                    break;
                case "delete":
                    var daysValue =
                        this.PageContext.CurrentForumPage.FindControlRecursiveAs<TextBox>("DaysOld").Text.Trim();
                    if (!ValidationHelper.IsValidInt(daysValue))
                    {
                        this.PageContext.AddLoadMessage(this.GetText("ADMIN_ADMIN", "MSG_VALID_DAYS"));
                        return;
                    }

                    if (!Config.IsAnyPortal)
                    {
                        UserMembershipHelper.DeleteUser(e.CommandArgument.ToType<int>());
                    }

                    LegacyDb.user_delete(e.CommandArgument);

                    this.BindData();
                    break;
                case "approve":
                    UserMembershipHelper.ApproveUser(e.CommandArgument.ToType<int>());
                    this.BindData();
                    break;
                case "deleteall":

                    // vzrus: Should not delete the whole providers portal data? Under investigation.
                    var daysValueAll =
                        this.PageContext.CurrentForumPage.FindControlRecursiveAs<TextBox>("DaysOld").Text.Trim();
                    if (!ValidationHelper.IsValidInt(daysValueAll))
                    {
                        this.PageContext.AddLoadMessage(this.GetText("ADMIN_ADMIN", "MSG_VALID_DAYS"));
                        return;
                    }

                    if (!Config.IsAnyPortal)
                    {
                        UserMembershipHelper.DeleteAllUnapproved(DateTime.UtcNow.AddDays(-daysValueAll.ToType<int>()));
                    }

                    LegacyDb.user_deleteold(this.PageContext.PageBoardID, daysValueAll.ToType<int>());
                    this.BindData();
                    break;
                case "approveall":
                    UserMembershipHelper.ApproveAll();

                    // vzrus: Should delete users from send email list
                    LegacyDb.user_approveall(this.PageContext.PageBoardID);
                    this.BindData();
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the forum link.
        /// </summary>
        /// <param name="forumId">
        /// The forum ID.
        /// </param>
        /// <param name="forumName">
        /// Name of the forum.
        /// </param>
        /// <returns>
        /// The format forum link.
        /// </returns>
        protected string FormatForumLink([NotNull] object forumId, [NotNull] object forumName)
        {
            if (forumId.ToString() == string.Empty || forumName.ToString() == string.Empty)
            {
                return string.Empty;
            }

            return
                "<a target=\"_top\" href=\"{0}\">{1}</a>".FormatWith(
                    YafBuildLink.GetLink(ForumPages.topics, "f={0}&name={1}", forumId, forumName), forumName);
        }

        /// <summary>
        /// Formats the topic link.
        /// </summary>
        /// <param name="topicId">
        /// The topic ID.
        /// </param>
        /// <param name="topicName">
        /// Name of the topic.
        /// </param>
        /// <returns>
        /// The format topic link.
        /// </returns>
        protected string FormatTopicLink([NotNull] object topicId, [NotNull] object topicName)
        {
            if (topicId.ToString() == string.Empty || topicName.ToString() == string.Empty)
            {
                return string.Empty;
            }

            return
                "<a target=\"_top\" href=\"{0}\">{1}</a>".FormatWith(
                    YafBuildLink.GetLink(ForumPages.posts, "t={0}", topicId), topicName);
        }

        /// <summary>
        /// Sets the location.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Returns the Location</returns>
        protected string SetLocation([NotNull] string userName)
        {
            string location;

            try
            {
                location = YafUserProfile.GetProfile(userName).Location;

                if (location.IsNotSet())
                {
                    location = "-";
                }
            }
            catch (Exception)
            {
                location = "-";
            }

            return this.HtmlEncode(this.Get<IBadWordReplace>().Replace(location));
        }

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            // bind data
            this.BindBoardsList();

            this.BindData();

            try
            {
                this.ShowUpgradeMessage();
            }
            catch (Exception)
            {
                this.UpdateHightlight.Visible = false;
                this.UpdateWarning.Visible = false;
            }
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddRoot();

            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);

            this.Page.Header.Title = this.GetText("ADMIN_ADMIN", "Administration");
        }

        /// <summary>
        /// Shows the upgrade message.
        /// </summary>
        private void ShowUpgradeMessage()
        {
            var latestInfo = new LatestInformationService().GetLatestVersionInformation();

            if (latestInfo == null || latestInfo.Version <= YafForumInfo.AppVersionCode)
            {
                return;
            }

            // updateLink
            var updateLink = new Action<HyperLink>(
                link =>
                {
                    link.Text = latestInfo.Message;
                    link.NavigateUrl = latestInfo.Link;
                });

            if (latestInfo.IsWarning)
            {
                this.UpdateWarning.Visible = true;
                updateLink(this.UpdateLinkWarning);
            }
            else
            {
                this.UpdateHightlight.Visible = true;
                updateLink(this.UpdateLinkHighlight);
            }
        }

        /// <summary>
        /// Binds the active user data.
        /// </summary>
        private void BindActiveUserData()
        {
            var activeUsers = this.GetActiveUsersData(true, true);

            if (activeUsers.HasRows())
            {
                YafContext.Current.PageElements.RegisterJsBlock(
                    "ActiveUsersTablesorterLoadJs",
                    JavaScriptBlocks.LoadTableSorter(
                        "#ActiveUsers",
                        "sortList: [[0,0]]",
                        "#ActiveUsersPager"));
            }

            this.ActiveList.DataSource = activeUsers;
            this.ActiveList.DataBind();
        }

        /// <summary>
        /// Bind list of boards to drop down
        /// </summary>
        private void BindBoardsList()
        {
            // only if user is host admin, otherwise boards' list is hidden
            if (!this.PageContext.IsHostAdmin)
            {
                return;
            }

            var table = this.GetRepository<Board>().List();

            // add row for "all boards" (null value)
            var row = table.NewRow();

            row["BoardID"] = -1;
            row["Name"] = this.GetText("ADMIN_ADMIN", "ALL_BOARDS");

            table.Rows.InsertAt(row, 0);

            // set datasource
            this.BoardStatsSelect.DataSource = table;
            this.BoardStatsSelect.DataBind();

            // select current board as default
            this.BoardStatsSelect.SelectedIndex =
                this.BoardStatsSelect.Items.IndexOf(
                    this.BoardStatsSelect.Items.FindByValue(this.PageContext.PageBoardID.ToString()));
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.UnverifiedUsersHolder.Visible = !Config.IsDotNetNuke;

            if (this.UnverifiedUsersHolder.Visible)
            {
                var unverifiedUsers = LegacyDb.user_list(this.PageContext.PageBoardID, null, false);

                if (unverifiedUsers.HasRows())
                {
                    YafContext.Current.PageElements.RegisterJsBlock(
                        "UnverifiedUserstablesorterLoadJs",
                        JavaScriptBlocks.LoadTableSorter(
                            "#UnverifiedUsers",
                            "headers: { 4: { sorter: false }},sortList: [[3,1],[0,0]]",
                            "#UnverifiedUsersPager"));
                }

                // bind list
                this.UserList.DataSource = unverifiedUsers;
                this.UserList.DataBind();
            }

            // get stats for current board, selected board or all boards (see function)
            var row = this.GetRepository<Board>().Stats(this.GetSelectedBoardId());

            this.NumPosts.Text = "{0:N0}".FormatWith(row["NumPosts"]);
            this.NumTopics.Text = "{0:N0}".FormatWith(row["NumTopics"]);
            this.NumUsers.Text = "{0:N0}".FormatWith(row["NumUsers"]);

            var span = DateTime.UtcNow - (DateTime)row["BoardStart"];
            double days = span.Days;

            this.BoardStart.Text =
                this.GetText("ADMIN_ADMIN", "DAYS_AGO").FormatWith(
                    this.Get<YafBoardSettings>().UseFarsiCalender
                        ? PersianDateConverter.ToPersianDate((DateTime)row["BoardStart"])
                        : row["BoardStart"],
                    days);

            if (days < 1)
            {
                days = 1;
            }

            this.DayPosts.Text = "{0:N2}".FormatWith(row["NumPosts"].ToType<int>() / days);
            this.DayTopics.Text = "{0:N2}".FormatWith(row["NumTopics"].ToType<int>() / days);
            this.DayUsers.Text = "{0:N2}".FormatWith(row["NumUsers"].ToType<int>() / days);

            try
            {
                this.DBSize.Text = "{0} MB".FormatWith(this.Get<IDbFunction>().GetDBSize());
            }
            catch (SqlException)
            {
                this.DBSize.Text = this.GetText("ADMIN_ADMIN", "ERROR_DATABASESIZE");
            }

            this.BindActiveUserData();

            this.DataBind();
        }

        /// <summary>
        /// Gets active user data Table data for a page user
        /// </summary>
        /// <param name="showGuests">
        /// The show guests.
        /// </param>
        /// <param name="showCrawlers">
        /// The show crawlers.
        /// </param>
        /// <returns>
        /// A DataTable
        /// </returns>
        private DataTable GetActiveUsersData(bool showGuests, bool showCrawlers)
        {
            // vzrus: Here should not be a common cache as it's should be individual for each user because of ActiveLocationcontrol to hide unavailable places.
            var activeUsers = this.GetRepository<Active>()
                .ListUser(
                    userID: this.PageContext.PageUserID,
                    guests: showGuests,
                    showCrawlers: showCrawlers,
                    activeTime: this.PageContext.BoardSettings.ActiveListTime,
                    styledNicks: this.PageContext.BoardSettings.UseStyledNicks);

            return activeUsers;
        }

        /// <summary>
        /// Gets board ID for which to show statistics.
        /// </summary>
        /// <returns>
        /// Returns ID of selected board (for host admin), ID of current board (for admin), null if all boards is selected.
        /// </returns>
        private int? GetSelectedBoardId()
        {
            // check dropdown only if user is hostadmin
            if (!this.PageContext.IsHostAdmin)
            {
                return this.PageContext.PageBoardID;
            }

            // -1 means all boards are selected
            return this.BoardStatsSelect.SelectedValue == "-1" ? (int?)null : this.BoardStatsSelect.SelectedValue.ToType<int>();
        }

        #endregion
    }
}