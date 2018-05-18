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
    using System.Globalization;
    using System.IO;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Core.Tasks;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Admin Members Page.
    /// </summary>
    public partial class users : AdminPage
    {
        #region Public Methods

        /// <summary>
        /// Redirects to the Create User Page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void NewUserClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // redirect to create new user page
            YafBuildLink.Redirect(ForumPages.admin_reguser);
        }

        /// <summary>
        /// The user list_ item command.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        public void UserListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":

                    // we are going to edit user - redirect to edit page
                    YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", e.CommandArgument);
                    break;
                case "delete":

                    // we are deleting user
                    if (this.PageContext.PageUserID == int.Parse(e.CommandArgument.ToString()))
                    {
                        // deleting yourself isn't an option
                        this.PageContext.AddLoadMessage(
                            this.GetText("ADMIN_USERS", "MSG_SELF_DELETE"),
                            MessageTypes.danger);
                        return;
                    }

                    // get user(s) we are about to delete
                    using (
                        var dataTable = LegacyDb.user_list(this.PageContext.PageBoardID, e.CommandArgument, DBNull.Value))
                    {
                        // examine each if he's possible to delete
                        foreach (DataRow row in dataTable.Rows)
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
                    UserMembershipHelper.DeleteUser(e.CommandArgument.ToType<int>());

                    // rebind data
                    this.BindData();

                    // quit case
                    break;
            }
        }

        /// <summary>
        /// The search_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void SearchClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Export all Users as CSV
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ExportUsersCsvClick(object sender, EventArgs e)
        {
            this.ExportAllUsers("csv");
        }

        /// <summary>
        /// Export all Users as XML
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ExportUsersXmlClick(object sender, EventArgs e)
        {
            this.ExportAllUsers("xml");
        }

        /// <summary>
        /// Gets the suspended string.
        /// </summary>
        /// <param name="suspendedUntil">The suspended until.</param>
        /// <returns>Returns the suspended string</returns>
        protected string GetSuspendedString(string suspendedUntil)
        {
            return suspendedUntil.IsNotSet()
                       ? this.GetText("COMMON", "NO")
                       : this.GetTextFormatted(
                           "USERSUSPENDED",
                           this.Get<IDateTime>().FormatDateTime(suspendedUntil.ToType<DateTime>()));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlockStartup(
                "BlockUIExecuteJs",
                JavaScriptBlocks.BlockUIExecuteJs("SyncUsersMessage", this.SyncUsers.ClientID));

            base.OnPreRender(e);
        }

        /// <summary>
        /// The bit set.
        /// </summary>
        /// <param name="_o">
        /// The _o.
        /// </param>
        /// <param name="bitmask">
        /// The bitmask.
        /// </param>
        /// <returns>
        /// The bit set boolean value.
        /// </returns>
        protected bool BitSet([NotNull] object _o, int bitmask)
        {
            var i = (int)_o;
            return (i & bitmask) != 0;
        }

        /// <summary>
        /// Creates navigation page links on top of forum (breadcrumbs).
        /// </summary>
        protected override void CreatePageLinks()
        {
            // link to board index
            this.PageLinks.AddRoot();

            // link to administration index
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                YafBuildLink.GetLink(ForumPages.admin_admin));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_USERS", "TITLE"));
        }

        /// <summary>
        /// Initializes dropdown with options to filter results by date.
        /// </summary>
        protected void InitSinceDropdown()
        {
            var lastVisit = this.Get<IYafSession>().LastVisit;

            // value 0, for since last visted
            this.Since.Items.Add(
                new ListItem(
                    this.GetTextFormatted(
                        "last_visit",
                        this.Get<IDateTime>()
                            .FormatDateTime(
                                lastVisit.HasValue && lastVisit.Value != DateTimeHelper.SqlDbMinTime()
                                    ? lastVisit.Value
                                    : DateTime.UtcNow)),
                    "0"));

            // negative values for hours backward
            this.Since.Items.Add(new ListItem("Last hour", "-1"));
            this.Since.Items.Add(new ListItem("Last two hours", "-2"));

            // positive values for days backward
            this.Since.Items.Add(new ListItem("Last day", "1"));
            this.Since.Items.Add(new ListItem("Last two days", "2"));
            this.Since.Items.Add(new ListItem("Last week", "7"));
            this.Since.Items.Add(new ListItem("Last two weeks", "14"));
            this.Since.Items.Add(new ListItem("Last month", "31"));

            // all time (i.e. no filter)
            this.Since.Items.Add(new ListItem("All time", "9999"));
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            if (Config.IsAnyPortal)
            {
                this.ImportUsers.Visible = false;
                this.SyncUsers.Visible = false;
            }

            // intialize since filter items
            this.InitSinceDropdown();

            // set since filter to last item "All time"
            this.Since.SelectedIndex = this.Since.Items.Count - 1;
            this.LoadingImage.ImageUrl = YafForumInfo.GetURLToContent("images/loader.gif");

            // get list of user groups for filtering
            var groups = this.GetRepository<Group>().List(boardId: this.PageContext.PageBoardID);

            groups.Insert(0, new Group { Name = this.GetText("FILTER_NO"), ID = 0 });

            this.group.DataTextField = "Name";
            this.group.DataValueField = "ID";

            this.group.DataBind();

            // get list of user ranks for filtering
            using (var dt = LegacyDb.rank_list(this.PageContext.PageBoardID, null))
            {
                // add empty for for no filtering
                var newRow = dt.NewRow();
                newRow["Name"] = this.GetText("FILTER_NO");
                newRow["RankID"] = 0;
                dt.Rows.InsertAt(newRow, 0);

                this.rank.DataSource = dt;
                this.rank.DataTextField = "Name";
                this.rank.DataValueField = "RankID";
                this.rank.DataBind();
            }

            // TODO : page size difinable?
            this.PagerTop.PageSize = 25;

            // Hide "New User" & Sync Button on DotNetNuke
            this.ImportAndSyncHolder.Visible = !Config.IsDotNetNuke;

            this.BindData();
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTopPageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// The sync users_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SyncUsersClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // start...
            SyncMembershipUsersTask.Start(this.PageContext.PageBoardID);

            // enable timer...
            this.UpdateStatusTimer.Enabled = true;
        }

        /// <summary>
        /// The update status timer_ tick.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UpdateStatusTimerTick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // see if the migration is done...
            if (this.Get<ITaskModuleManager>().IsTaskRunning(SyncMembershipUsersTask.TaskName))
            {
                // continue...
                return;
            }

            this.UpdateStatusTimer.Enabled = false;

            // done here...
            YafBuildLink.Redirect(ForumPages.admin_users);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.SearchResults.Visible = true;

            // default since date is now
            var sinceDate = DateTime.UtcNow;

            // default since option is "since last visit"
            var sinceValue = 0;

            // is any "since"option selected
            if (this.Since.SelectedItem != null)
            {
                // get selected value
                sinceValue = int.Parse(this.Since.SelectedItem.Value);

                // sinceDate = DateTime.UtcNow;
                // no need to do it again (look above)
                // decrypt selected option
                if (sinceValue == 9999)
                {
                    // all
                    // get all, from the beginning
                    sinceDate = DateTimeHelper.SqlDbMinTime();
                }
                else if (sinceValue > 0)
                {
                    // days
                    // get posts newer then defined number of days
                    sinceDate = DateTime.UtcNow - TimeSpan.FromDays(sinceValue);
                }
                else if (sinceValue < 0)
                {
                    // hours
                    // get posts newer then defined number of hours
                    sinceDate = DateTime.UtcNow + TimeSpan.FromHours(sinceValue);
                }
            }

            // we want to filter topics since last visit
            if (sinceValue == 0)
            {
                sinceDate = this.Get<IYafSession>().LastVisit ?? DateTime.UtcNow;
            }

            // we are going to page results
            var pds = new PagedDataSource { AllowPaging = true, PageSize = this.PagerTop.PageSize };

            // page size defined by pager's size

            // get users, eventually filter by groups or ranks
            using (
                var dt = LegacyDb.user_list(
                    this.PageContext.PageBoardID,
                    null,
                    null,
                    this.group.SelectedIndex <= 0 ? null : this.group.SelectedValue,
                    this.rank.SelectedIndex <= 0 ? null : this.rank.SelectedValue,
                    false))
            {
                using (var dv = dt.DefaultView)
                {
                    // filter by name or email
                    if (this.name.Text.Trim().Length > 0 || (this.Email.Text.Trim().Length > 0))
                    {
                        dv.RowFilter =
                            "(Name LIKE '%{0}%' OR DisplayName LIKE '%{0}%') AND Email LIKE '%{1}%'".FormatWith(
                                this.name.Text.Trim(),
                                this.Email.Text.Trim());
                    }

                    // filter by date of registration
                    if (sinceValue != 9999)
                    {
                        dv.RowFilter += "{1}Joined > '{0}'".FormatWith(
                            sinceDate.ToString(CultureInfo.InvariantCulture),
                            dv.RowFilter.IsNotSet() ? string.Empty : " AND ");
                    }

                    // show only suspended ?
                    if (this.SuspendedOnly.Checked)
                    {
                        dv.RowFilter += "{0}Suspended is not null".FormatWith(
                            dv.RowFilter.IsNotSet() ? string.Empty : " AND ");
                    }

                    // set pager and data source
                    this.PagerTop.Count = dv.Count;
                    pds.DataSource = dv;

                    // page to render
                    pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;

                    // if we are above total number of pages, select last
                    if (pds.CurrentPageIndex >= pds.PageCount)
                    {
                        pds.CurrentPageIndex = pds.PageCount - 1;
                    }

                    // bind list
                    this.UserList.DataSource = pds;
                    this.UserList.DataBind();
                }
            }
        }

        /// <summary>
        /// Export All Users
        /// </summary>
        /// <param name="type">
        /// The export format type.
        /// </param>
        private void ExportAllUsers(string type)
        {
            var usersList = LegacyDb.user_list(this.PageContext.PageBoardID, null, true);

            usersList.DataSet.DataSetName = "YafUserList";

            usersList.TableName = "YafUser";

            usersList.Columns.Remove("AvatarImage");
            usersList.Columns.Remove("AvatarImageType");

            usersList.Columns.Remove("ProviderUserKey");
            usersList.Columns.Remove("Password");
            usersList.Columns.Remove("Joined");
            usersList.Columns.Remove("LastVisit");
            usersList.Columns.Remove("IP");
            usersList.Columns.Remove("NumPosts");
            usersList.Columns.Remove("RankID");
            usersList.Columns.Remove("Flags");
            usersList.Columns.Remove("Points");
            usersList.Columns.Remove("IsApproved");
            usersList.Columns.Remove("IsActiveExcluded");
            usersList.Columns.Remove("IsCaptchaExcluded");
            usersList.Columns.Remove("IsDirty");
            usersList.Columns.Remove("Style");
            usersList.Columns.Remove("IsAdmin");
            usersList.Columns.Remove("IsGuest1");
            usersList.Columns.Remove("IsHostAdmin");

            // Add Profile Columns
            usersList.Columns.Add("RealName");
            usersList.Columns.Add("BlogServiceUrl");
            usersList.Columns.Add("Blog");
            usersList.Columns.Add("Gender");
            usersList.Columns.Add("MSN");
            usersList.Columns.Add("Birthday");
            usersList.Columns.Add("BlogServiceUsername");
            usersList.Columns.Add("BlogServicePassword");
            usersList.Columns.Add("AIM");
            usersList.Columns.Add("GoogleTalk");
            usersList.Columns.Add("Location");
            usersList.Columns.Add("Country");
            usersList.Columns.Add("Region");
            usersList.Columns.Add("City");
            usersList.Columns.Add("Interests");
            usersList.Columns.Add("Homepage");
            usersList.Columns.Add("Skype");
            usersList.Columns.Add("ICQ");
            usersList.Columns.Add("XMPP");
            usersList.Columns.Add("YIM");
            usersList.Columns.Add("Occupation");
            usersList.Columns.Add("Twitter");
            usersList.Columns.Add("TwitterId");
            usersList.Columns.Add("Facebook");
            usersList.Columns.Add("FacebookId");
            usersList.Columns.Add("Google");
            usersList.Columns.Add("GoogleId");

            usersList.Columns.Add("Roles");

            usersList.AcceptChanges();

            foreach (DataRow user in usersList.Rows)
            {
                var userProfile = YafUserProfile.GetProfile((string)user["Name"]);

                // Add Profile Fields to User List Table.
                user["RealName"] = userProfile.RealName;
                user["Blog"] = userProfile.Blog;
                user["Gender"] = userProfile.Gender;
                user["MSN"] = userProfile.MSN;
                user["Birthday"] = userProfile.Birthday;
                user["BlogServiceUsername"] = userProfile.BlogServiceUsername;
                user["BlogServicePassword"] = userProfile.BlogServicePassword;
                user["AIM"] = userProfile.AIM;
                user["Google"] = userProfile.Google;
                user["GoogleId"] = userProfile.GoogleId;
                user["Location"] = userProfile.Location;
                user["Country"] = userProfile.Country;
                user["Region"] = userProfile.Region;
                user["City"] = userProfile.City;
                user["Interests"] = userProfile.Interests;
                user["Homepage"] = userProfile.Homepage;
                user["Skype"] = userProfile.Skype;
                user["ICQ"] = userProfile.ICQ;
                user["XMPP"] = userProfile.XMPP;
                user["YIM"] = userProfile.YIM;
                user["Occupation"] = userProfile.Occupation;
                user["Twitter"] = userProfile.Twitter;
                user["TwitterId"] = userProfile.TwitterId;
                user["Facebook"] = userProfile.Facebook;
                user["FacebookId"] = userProfile.FacebookId;

                user["Roles"] = this.Get<RoleProvider>().GetRolesForUser((string)user["Name"]).ToDelimitedString(",");

                usersList.AcceptChanges();
            }

            switch (type)
            {
                case "xml":
                    this.ExportAsXml(usersList);
                    break;
                case "csv":
                    this.ExportAsCsv(usersList);
                    break;
            }
        }

        /// <summary>
        /// Export As CSV
        /// </summary>
        /// <param name="usersList">
        /// The users list.
        /// </param>
        private void ExportAsCsv(DataTable usersList)
        {
            this.Response.ContentType = "application/vnd.csv";

            this.Response.AppendHeader(
                "Content-Disposition",
                "attachment; filename=YafUsersExport-{0}.csv".FormatWith(
                    HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))));

            var sw = new StreamWriter(this.Response.OutputStream);

            // Write Column Headers
            var columnCount = usersList.Columns.Count;

            for (var i = 0; i < columnCount; i++)
            {
                sw.Write(usersList.Columns[i]);

                if (i < columnCount - 1)
                {
                    sw.Write(",");
                }
            }

            sw.Write(sw.NewLine);

            foreach (DataRow dr in usersList.Rows)
            {
                for (var i = 0; i < columnCount; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        sw.Write(dr[i].ToString());
                    }

                    if (i < columnCount - 1)
                    {
                        sw.Write(",");
                    }
                }

                sw.Write(sw.NewLine);
            }

            sw.Close();

            this.Response.Flush();
            this.Response.End();
        }

        /// <summary>
        /// Export As Xml
        /// </summary>
        /// <param name="usersList">
        /// The users list.
        /// </param>
        private void ExportAsXml(DataTable usersList)
        {
            this.Response.ContentType = "text/xml";

            this.Response.AppendHeader(
                "Content-Disposition",
                "attachment; filename=YafUsersExport-{0}.xml".FormatWith(
                    HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HHmm"))));

            usersList.DataSet.WriteXml(this.Response.OutputStream);

            this.Response.Flush();
            this.Response.End();
        }

        #endregion
    }
}