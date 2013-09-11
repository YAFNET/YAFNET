/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.RegisterV2;
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
        public void BoardStatsSelect_Changed([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Handles the ItemCommand event of the UserList control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
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
                    this.Get<ILogger>()
                        .Log(
                            this.PageContext.PageUserID,
                            "YAF.Pages.Admin.admin",
                            "User {0} was deleted by {1}.".FormatWith(e.CommandArgument.ToType<int>(), this.PageContext.PageUserID),
                            EventLogTypes.UserDeleted);
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
        /// Adds Confirmation Dialog to the Approve All Button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ApproveAll_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((Button)sender).Text = this.GetText("ADMIN_ADMIN", "APROVE_ALL");
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_ADMIN", "CONFIRM_APROVE_ALL"));
        }

        /// <summary>
        /// Adds Confirmation Dialog to the Approve Button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Approve_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_ADMIN", "CONFIRM_APROVE"));
        }

        /// <summary>
        /// Adds Confirmation Dialog to the Delete All Button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteAll_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((Button)sender).Text = this.GetText("ADMIN_ADMIN", "DELETE_ALL");

            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_ADMIN", "CONFIRM_DELETE_ALL"));
        }

        /// <summary>
        /// Adds Confirmation Dialog to the Delete Button
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_ADMIN", "CONFIRM_DELETE"));
        }

        /// <summary>
        /// Formats the forum link.
        /// </summary>
        /// <param name="forumID">
        /// The forum ID.
        /// </param>
        /// <param name="forumName">
        /// Name of the forum.
        /// </param>
        /// <returns>
        /// The format forum link.
        /// </returns>
        protected string FormatForumLink([NotNull] object forumID, [NotNull] object forumName)
        {
            if (forumID.ToString() == string.Empty || forumName.ToString() == string.Empty)
            {
                return string.Empty;
            }

            return
                "<a target=\"_top\" href=\"{0}\">{1}</a>".FormatWith(
                    YafBuildLink.GetLink(ForumPages.topics, "f={0}", forumID), forumName);
        }

        /// <summary>
        /// Formats the topic link.
        /// </summary>
        /// <param name="topicID">
        /// The topic ID.
        /// </param>
        /// <param name="topicName">
        /// Name of the topic.
        /// </param>
        /// <returns>
        /// The format topic link.
        /// </returns>
        protected string FormatTopicLink([NotNull] object topicID, [NotNull] object topicName)
        {
            if (topicID.ToString() == string.Empty || topicName.ToString() == string.Empty)
            {
                return string.Empty;
            }

            return
                "<a target=\"_top\" href=\"{0}\">{1}</a>".FormatWith(
                    YafBuildLink.GetLink(ForumPages.posts, "t={0}", topicID), topicName);
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
                location = YafUserProfile.GetProfile(Eval("UserName").ToString()).Location;

                if (string.IsNullOrEmpty(location))
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
            // setup jQuery and YAF JS...
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJQueryUI();

            YafContext.Current.PageElements.RegisterJsResourceInclude(
                 "tablesorter", "js/jquery.tablesorter.js");

            YafContext.Current.PageElements.RegisterJsBlock(
                "spinnerJs",
                JavaScriptBlocks.LoadSpinnerWidget());

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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindActiveUserData();
        }

        /// <summary>
        /// Binds the active user data.
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

            DataTable dt = this.GetRepository<Board>().List();

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
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            var unverifiedUsers = LegacyDb.user_list(this.PageContext.PageBoardID, null, false);

            YafContext.Current.PageElements.RegisterJsBlock(
                "tablesorterLoadJs",
                JavaScriptBlocks.LoadTableSorter(
                    ".sortable",
                    unverifiedUsers.Rows.Count > 0 ? "headers: { 4: { sorter: false }},sortList: [[3,1],[0,0]]" : null));

            // bind list
            this.UserList.DataSource = unverifiedUsers;
            this.UserList.DataBind();

            // get stats for current board, selected board or all boards (see function)
            DataRow row = this.GetRepository<Board>().Stats(this.GetSelectedBoardID());

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
            DataTable activeUsers =
                this.Get<IDbFunction>().GetAsDataTable(
                    cdb => 
                    cdb.active_list_user(
                        this.PageContext.PageBoardID,
                        this.PageContext.PageUserID,
                        showGuests,
                        showCrawlers,
                        this.Get<YafBoardSettings>().ActiveListTime,
                        this.Get<YafBoardSettings>().UseStyledNicks));

            return activeUsers;
        }

        /// <summary>
        /// Gets board ID for which to show statistics.
        /// </summary>
        /// <returns>
        /// Returns ID of selected board (for host admin), ID of current board (for admin), null if all boards is selected.
        /// </returns>
        private int? GetSelectedBoardID()
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