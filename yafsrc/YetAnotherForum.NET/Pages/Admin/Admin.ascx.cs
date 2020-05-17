/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net.Mail;
    using System.Web.UI.WebControls;

    using FarsiLibrary.Utils;

    using YAF.Classes;
    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin Index Page.
    /// </summary>
    public partial class Admin : AdminPage
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
                    BuildLink.Redirect(ForumPages.Admin_EditUser, "u={0}", e.CommandArgument);
                    break;
                case "resendEmail":
                    var commandArgument = e.CommandArgument.ToString().Split(';');

                    var checkMail = this.GetRepository<CheckEmail>().ListTyped(commandArgument[0]).FirstOrDefault();

                    if (checkMail != null)
                    {
                        var verifyEmail = new TemplateEmail("VERIFYEMAIL");

                        var subject = this.Get<ILocalization>()
                            .GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", this.Get<BoardSettings>().Name);

                        verifyEmail.TemplateParams["{link}"] = BuildLink.GetLinkNotEscaped(
                            ForumPages.Account_Approve,
                            true,
                            "code={0}",
                            checkMail.Hash);
                        verifyEmail.TemplateParams["{key}"] = checkMail.Hash;
                        verifyEmail.TemplateParams["{forumname}"] = this.Get<BoardSettings>().Name;
                        verifyEmail.TemplateParams["{forumlink}"] = BoardInfo.ForumURL;

                        verifyEmail.SendEmail(new MailAddress(checkMail.Email, commandArgument[1]), subject);

                        this.PageContext.AddLoadMessage(
                            this.GetText("ADMIN_ADMIN", "MSG_MESSAGE_SEND"),
                            MessageTypes.success);
                    }
                    else
                    {
                        var userFound = this.Get<IUserDisplayName>().Find(commandArgument[1]).FirstOrDefault();

                        var user = this.Get<IAspNetUsersHelper>().GetUserByName(userFound.Name);

                        this.Get<ISendNotification>().SendVerificationEmail(user, commandArgument[0], userFound.ID);
                    }

                    break;
                case "delete":
                    if (!Config.IsAnyPortal)
                    {
                        this.Get<IAspNetUsersHelper>().DeleteUser(e.CommandArgument.ToType<int>());
                    }

                    this.GetRepository<User>().Delete(e.CommandArgument.ToType<int>());

                    this.BindData();
                    break;
                case "approve":
                    this.Get<IAspNetUsersHelper>().ApproveUser(e.CommandArgument.ToType<int>());
                    this.BindData();
                    break;
                case "deleteall":

                    // vzrus: Should not delete the whole providers portal data? Under investigation.
                    var daysValueAll =
                        this.PageContext.CurrentForumPage.FindControlRecursiveAs<TextBox>("DaysOld").Text.Trim();
                    if (!ValidationHelper.IsValidInt(daysValueAll))
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetText("ADMIN_ADMIN", "MSG_VALID_DAYS"),
                            MessageTypes.warning);
                        return;
                    }

                    if (!Config.IsAnyPortal)
                    {
                        this.Get<IAspNetUsersHelper>().DeleteAllUnapproved(System.DateTime.UtcNow.AddDays(-daysValueAll.ToType<int>()));
                    }
                    else
                    {
                        this.GetRepository<User>().DeleteOld(this.PageContext.PageBoardID, daysValueAll.ToType<int>());
                    }

                    this.BindData();
                    break;
                case "approveall":
                    this.Get<IAspNetUsersHelper>().ApproveAll();

                    // vzrus: Should delete users from send email list
                    this.GetRepository<User>().ApproveAll(this.PageContext.PageBoardID);
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
                $"<a target=\"_top\" href=\"{BuildLink.GetLink(ForumPages.Topics, "f={0}&name={1}", forumId, forumName)}\">{forumName}</a>";
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
                $"<a target=\"_top\" href=\"{BuildLink.GetLink(ForumPages.Posts, "t={0}", topicId)}\">{topicName}</a>";
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

            this.BoardStatsSelect.Visible = this.PageContext.IsHostAdmin;

            // bind data
            this.BindBoardsList();

            this.BindData();

            this.ShowUpgradeMessage();
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
            var latestInfo = new LatestInformationService().GetLatestVersion();

            if (latestInfo == null || BitConverter.ToInt64(latestInfo, 0) <= BitConverter.ToInt64(BoardInfo.AppVersionCode, 0))
            {
                return;
            }

            // updateLink
            this.UpdateHightlight.Visible = true;
        }

        /// <summary>
        /// Binds the active user data.
        /// </summary>
        private void BindActiveUserData()
        {
            var activeUsers = this.GetActiveUsersData(true, true);

            if (activeUsers.HasRows())
            {
                BoardContext.Current.PageElements.RegisterJsBlock(
                    "ActiveUsersTablesorterLoadJs",
                    JavaScriptBlocks.LoadTableSorter("#ActiveUsers", "sortList: [[0,0]]", "#ActiveUsersPager"));
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

            var boards = this.GetRepository<Board>().GetAll();

            // add row for "all boards" (null value)
            boards.Insert(0, new Board { ID = -1, Name = this.GetText("ADMIN_ADMIN", "ALL_BOARDS") });

            // set data source
            this.BoardStatsSelect.DataSource = boards;
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
                var unverifiedUsers = this.GetRepository<User>().ListAsDataTable(this.PageContext.PageBoardID, null, false);

                if (unverifiedUsers.HasRows())
                {
                    BoardContext.Current.PageElements.RegisterJsBlock(
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

            this.NumPosts.Text = $"{row["NumPosts"]:N0}";
            this.NumTopics.Text = $"{row["NumTopics"]:N0}";
            this.NumUsers.Text = $"{row["NumUsers"]:N0}";

            var span = System.DateTime.UtcNow - (System.DateTime)row["BoardStart"];
            double days = span.Days;

            this.BoardStart.Text = this.Get<IDateTime>().FormatDateTimeTopic(
                this.Get<BoardSettings>().UseFarsiCalender
                    ? PersianDateConverter.ToPersianDate((System.DateTime)row["BoardStart"])
                    : row["BoardStart"]);

            this.BoardStartAgo.Text = new DisplayDateTime
            {
                                              DateTime = (System.DateTime)row["BoardStart"], Format = DateTimeFormat.BothTopic
                                          }.RenderToString();

            if (days < 1)
            {
                days = 1;
            }

            this.DayPosts.Text = $"{row["NumPosts"].ToType<int>() / days:N2}";
            this.DayTopics.Text = $"{row["NumTopics"].ToType<int>() / days:N2}";
            this.DayUsers.Text = $"{row["NumUsers"].ToType<int>() / days:N2}";

            try
            {
                this.DBSize.Text = $"{this.Get<IDbFunction>().GetDBSize()} MB";
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
            var activeUsers = this.GetRepository<Active>()
                .ListUserAsDataTable(
                    this.PageContext.PageUserID,
                    showGuests,
                    showCrawlers,
                    this.PageContext.BoardSettings.ActiveListTime,
                    this.PageContext.BoardSettings.UseStyledNicks);

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
            // check dropdown only if user is host admin
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