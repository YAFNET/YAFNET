﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web;
    using System.Web.UI.WebControls;

    using FarsiLibrary;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.RegisterV2;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
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
        /// The board stats select_ changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void BoardStatsSelect_Changed([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// The user list_ item command.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void UserList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", e.CommandArgument);
                    break;
                case "delete":
                    string daysValue =
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
                    string daysValueAll =
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
        /// The approve all_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ApproveAll_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((Button)sender).Text = this.GetText("ADMIN_ADMIN", "APROVE_ALL");
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_ADMIN", "CONFIRM_APROVE_ALL"));
        }

        /// <summary>
        /// The approve_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Approve_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_ADMIN", "CONFIRM_APROVE"));
        }

        /// <summary>
        /// The delete all_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void DeleteAll_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((Button)sender).Text = this.GetText("ADMIN_ADMIN", "DELETE_ALL");

            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_ADMIN", "CONFIRM_DELETE_ALL"));
        }

        /// <summary>
        /// The delete_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_ADMIN", "CONFIRM_DELETE"));
        }

        /// <summary>
        /// Formats the forum link.
        /// </summary>
        /// <param name="ForumID">
        /// The forum ID.
        /// </param>
        /// <param name="ForumName">
        /// Name of the forum.
        /// </param>
        /// <returns>
        /// The format forum link.
        /// </returns>
        protected string FormatForumLink([NotNull] object ForumID, [NotNull] object ForumName)
        {
            if (ForumID.ToString() == string.Empty || ForumName.ToString() == string.Empty)
            {
                return string.Empty;
            }

            return
                "<a target=\"_top\" href=\"{0}\">{1}</a>".FormatWith(
                    YafBuildLink.GetLink(ForumPages.topics, "f={0}", ForumID), ForumName);
        }

        /// <summary>
        /// Formats the topic link.
        /// </summary>
        /// <param name="TopicID">
        /// The topic ID.
        /// </param>
        /// <param name="TopicName">
        /// Name of the topic.
        /// </param>
        /// <returns>
        /// The format topic link.
        /// </returns>
        protected string FormatTopicLink([NotNull] object TopicID, [NotNull] object TopicName)
        {
            if (TopicID.ToString() == string.Empty || TopicName.ToString() == string.Empty)
            {
                return string.Empty;
            }

            return
                "<a target=\"_top\" href=\"{0}\">{1}</a>".FormatWith(
                    YafBuildLink.GetLink(ForumPages.posts, "t={0}", TopicID), TopicName);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);

            this.Page.Header.Title = this.GetText("ADMIN_ADMIN", "Administration");

            // bind data
            this.BindBoardsList();

            this.BindData();

            var latestInfo =
                this.Get<HttpApplicationStateBase>()["YafRegistrationLatestInformation"] as LatestVersionInformation;

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

            // UpgradeNotice.Visible = install._default.GetCurrentVersion() < Data.AppVersion;
        }

        /// <summary>
        /// The pager_ page change.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindActiveUserData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindActiveUserData()
        {
            DataTable activeList = this.GetActiveUsersData(true, true);

            if (activeList == null || activeList.Rows.Count <= 0)
            {
                return;
            }

            var activeUsersView = activeList.DefaultView;
            this.Pager.PageSize = 50;

            var pds = new PagedDataSource { AllowPaging = true, PageSize = this.Pager.PageSize };
            this.Pager.Count = activeUsersView.Count;
            pds.DataSource = activeUsersView;
            pds.CurrentPageIndex = this.Pager.CurrentPageIndex;

            if (pds.CurrentPageIndex >= pds.PageCount)
            {
                pds.CurrentPageIndex = pds.PageCount - 1;
            }

            this.ActiveList.DataSource = pds;
            this.ActiveList.DataBind();
        }

        /// <summary>
        /// Bind list of boards to dropdown
        /// </summary>
        private void BindBoardsList()
        {
            // only if user is hostadmin, otherwise boards' list is hidden
            if (!this.PageContext.IsHostAdmin)
            {
                return;
            }

            DataTable dt = LegacyDb.board_list(null);

            // add row for "all boards" (null value)
            DataRow r = dt.NewRow();

            r["BoardID"] = -1;
            r["Name"] = this.GetText("ADMIN_ADMIN", "ALL_BOARDS");

            dt.Rows.InsertAt(r, 0);

            // set datasource
            this.BoardStatsSelect.DataSource = dt;
            this.BoardStatsSelect.DataBind();

            // select current board as default
            this.BoardStatsSelect.SelectedIndex =
                this.BoardStatsSelect.Items.IndexOf(
                    this.BoardStatsSelect.Items.FindByValue(this.PageContext.PageBoardID.ToString()));
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.UserList.DataSource = LegacyDb.user_list(this.PageContext.PageBoardID, null, false);

            // this.DataBind();

            // get stats for current board, selected board or all boards (see function)
            DataRow row = LegacyDb.board_stats(this.GetSelectedBoardID());

            this.NumPosts.Text = "{0:N0}".FormatWith(row["NumPosts"]);
            this.NumTopics.Text = "{0:N0}".FormatWith(row["NumTopics"]);
            this.NumUsers.Text = "{0:N0}".FormatWith(row["NumUsers"]);

            TimeSpan span = DateTime.UtcNow - (DateTime)row["BoardStart"];
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
                this.DBSize.Text = "{0} MB".FormatWith(LegacyDb.GetDBSize());
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
            DataTable activeUsers = LegacyDb.active_list_user(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                showGuests,
                showCrawlers,
                this.Get<YafBoardSettings>().ActiveListTime,
                this.Get<YafBoardSettings>().UseStyledNicks);

            // Set colorOnly parameter to false, as we get active users style from database        
            if (this.Get<YafBoardSettings>().UseStyledNicks)
            {
                this.Get<IStyleTransform>().DecodeStyleByTable(ref activeUsers, false);
            }

            return activeUsers;
        }

        /// <summary>
        /// Gets board ID for which to show statistics.
        /// </summary>
        /// <returns>
        /// Returns ID of selected board (for host admin), ID of current board (for admin), null if all boards is selected.
        /// </returns>
        private object GetSelectedBoardID()
        {
            // check dropdown only if user is hostadmin
            if (!this.PageContext.IsHostAdmin)
            {
                return this.PageContext.PageBoardID;
            }

            // -1 means all boards are selected
            return this.BoardStatsSelect.SelectedValue == "-1" ? null : this.BoardStatsSelect.SelectedValue;
        }

        #endregion
    }
}