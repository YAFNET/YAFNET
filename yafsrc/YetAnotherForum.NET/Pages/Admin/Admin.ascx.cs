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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Linq;
    using System.Net.Mail;
    using System.Web.UI.WebControls;

    using FarsiLibrary.Utils;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
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
                    this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, "u={0}", e.CommandArgument);
                    break;
                case "resendEmail":
                    var commandArgument = e.CommandArgument.ToString().Split(';');

                    var checkMail = this.GetRepository<CheckEmail>().ListTyped(commandArgument[0]).FirstOrDefault();

                    if (checkMail != null)
                    {
                        var verifyEmail = new TemplateEmail("VERIFYEMAIL");

                        var subject = this.Get<ILocalization>()
                            .GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", this.PageContext.BoardSettings.Name);

                        verifyEmail.TemplateParams["{link}"] = this.Get<LinkBuilder>().GetLink(
                            ForumPages.Account_Approve,
                            true,
                            "code={0}",
                            checkMail.Hash);
                        verifyEmail.TemplateParams["{key}"] = checkMail.Hash;
                        verifyEmail.TemplateParams["{forumname}"] = this.PageContext.BoardSettings.Name;
                        verifyEmail.TemplateParams["{forumlink}"] = this.Get<LinkBuilder>().ForumUrl;

                        verifyEmail.SendEmail(new MailAddress(checkMail.Email, commandArgument[1]), subject);

                        this.PageContext.AddLoadMessage(
                            this.GetText("ADMIN_ADMIN", "MSG_MESSAGE_SEND"),
                            MessageTypes.success);
                    }
                    else
                    {
                        var userFound = this.Get<IUserDisplayName>().FindUserContainsName(commandArgument[1]).FirstOrDefault();

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
                        this.Get<IAspNetUsersHelper>().DeleteAllUnapproved(DateTime.UtcNow.AddDays(-daysValueAll.ToType<int>()));
                    }
                    else
                    {
                        this.GetRepository<User>().DeleteOld(this.PageContext.PageBoardID, daysValueAll.ToType<int>());
                    }

                    this.BindData();
                    break;
                case "approveall":
                    this.Get<IAspNetUsersHelper>().ApproveAll();

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
                $"<a target=\"_top\" href=\"{this.Get<LinkBuilder>().GetForumLink(forumId.ToType<int>(), forumName.ToString())}\">{forumName}</a>";
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
        protected string FormatTopicLink([NotNull] object topicId, [NotNull] string topicName)
        {
            if (topicId.ToString() == string.Empty || topicName.IsNotSet())
            {
                return string.Empty;
            }

            return
                $"<a target=\"_top\" href=\"{this.Get<LinkBuilder>().GetLink(ForumPages.Posts, "t={0}&name={1}", topicId, topicName)}\">{topicName}</a>";
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

            this.PageSize.DataSource = StaticDataHelper.PageEntries();
            this.PageSize.DataTextField = "Name";
            this.PageSize.DataValueField = "Value";
            this.PageSize.DataBind();

            this.PageSizeUnverified.DataSource = StaticDataHelper.PageEntries();
            this.PageSizeUnverified.DataTextField = "Name";
            this.PageSizeUnverified.DataValueField = "Value";
            this.PageSizeUnverified.DataBind();

            this.BoardStatsSelect.Visible = this.PageContext.User.UserFlags.IsHostAdmin;

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
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTopChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindActiveUserData();
        }

        /// <summary>
        /// The page size on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindActiveUserData();
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerUnverifiedChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindUnverifiedUsers();
        }

        /// <summary>
        /// The page size on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageSizeUnverifiedSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindUnverifiedUsers();
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
                    () => this.Get<ILatestInformation>().GetLatestVersion(),
                    TimeSpan.FromDays(1));

                var latestVersion = (DateTime)version.VersionDate;

                if (latestVersion.ToUniversalTime() <= BoardInfo.AppVersionDate.ToUniversalTime())
                {
                    return;
                }

                // updateLink
                this.UpdateHightlight.Visible = true;
                this.UpdateLinkHighlight.NavigateUrl = version.UpgradeUrl;
            }
            catch (Exception)
            {
                this.UpdateHightlight.Visible = false;
            }
        }

        /// <summary>
        /// Binds the active user data.
        /// </summary>
        private void BindActiveUserData()
        {
            this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

            var activeUsers = this.GetRepository<Active>().ListUsersPaged(
                this.PageContext.PageUserID,
                true,
                true,
                this.PageContext.BoardSettings.ActiveListTime,
                this.PagerTop.CurrentPageIndex,
                this.PagerTop.PageSize);

            this.ActiveList.DataSource = activeUsers;
            this.ActiveList.DataBind();
        }

        /// <summary>
        /// Bind list of boards to drop down
        /// </summary>
        private void BindBoardsList()
        {
            // only if user is host admin, otherwise boards' list is hidden
            if (!this.PageContext.User.UserFlags.IsHostAdmin)
            {
                return;
            }

            var boards = this.GetRepository<Board>().GetAll();

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
            this.BindUnverifiedUsers();

            // get stats for current board, selected board or all boards (see function)
            var data = this.GetRepository<Board>().Stats(this.GetSelectedBoardId());

            this.NumPosts.Text = $"{data.Posts:N0}";
            this.NumTopics.Text = $"{data.Topics:N0}";
            this.NumUsers.Text = $"{data.Users:N0}";

            var span = DateTime.UtcNow - data.BoardStart;
            double days = span.Days;

            this.BoardStart.Text = this.Get<IDateTimeService>().FormatDateTimeTopic(
                this.PageContext.BoardSettings.UseFarsiCalender
                    ? PersianDateConverter.ToPersianDate(data.BoardStart)
                    : data.BoardStart);

            this.BoardStartAgo.Text = new DisplayDateTime
            {
                DateTime = data.BoardStart, Format = DateTimeFormat.BothTopic
            }.RenderToString();

            if (days < 1)
            {
                days = 1;
            }

            this.DayPosts.Text = $"{data.Posts / days:N2}";
            this.DayTopics.Text = $"{data.Topics / days:N2}";
            this.DayUsers.Text = $"{data.Users / days:N2}";

            try
            {
                this.DBSize.Text = $"{this.Get<IDbAccess>().GetDatabaseSize()} MB";
            }
            catch (Exception)
            {
                this.DBSize.Text = this.GetText("ADMIN_ADMIN", "ERROR_DATABASESIZE");
            }

            this.BindActiveUserData();

            this.DataBind();
        }

        /// <summary>
        /// Bind unverified users.
        /// </summary>
        private void BindUnverifiedUsers()
        {
            this.UnverifiedUsersHolder.Visible = !Config.IsDotNetNuke;

            if (!this.UnverifiedUsersHolder.Visible)
            {
                return;
            }

            this.PagerUnverified.PageSize = this.PageSizeUnverified.SelectedValue.ToType<int>();

            var unverifiedUsers = this.GetRepository<User>().UnApprovedUsers(this.PageContext.PageBoardID)
                .GetPaged(this.PagerUnverified);

            // bind list
            this.UserList.DataSource = unverifiedUsers;
            this.UserList.DataBind();

            if (this.UserList.Items.Count == 0)
            {
                this.NoInfo.Visible = true;
            }
        }

        /// <summary>
        /// Gets board ID for which to show statistics.
        /// </summary>
        /// <returns>
        /// Returns ID of selected board (for host admin), ID of current board (for admin), null if all boards is selected.
        /// </returns>
        private int GetSelectedBoardId()
        {
            // check dropdown only if user is host admin
            return !this.PageContext.User.UserFlags.IsHostAdmin
                ? this.PageContext.PageBoardID
                : this.BoardStatsSelect.SelectedValue.ToType<int>();
        }

        #endregion
    }
}