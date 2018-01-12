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

namespace YAF.Pages
{
    // YAF.Pages
    #region

    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Members List Page
    /// </summary>
    public partial class members : ForumPage
    {
        #region Fields
        /// <summary>
        /// The _userListDataTable.
        /// </summary>
        private DataTable _userListDataTable;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "members" /> class.
        /// </summary>
        public members()
            : base("MEMBERS")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the Click event of the Search control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void Search_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Handles the Click event of the Reset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void Reset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-direct to self.
            YafBuildLink.Redirect(ForumPages.members);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the avatar Url for the user
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="avatarString">The avatar string.</param>
        /// <param name="hasAvatarImage">if set to <c>true</c> [has avatar image].</param>
        /// <param name="email">The email.</param>
        /// <returns>Returns the File Url</returns>
        protected string GetAvatarUrlFileName(int userId, string avatarString, bool hasAvatarImage, string email)
        {
            var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userId, avatarString, hasAvatarImage, email);

            return avatarUrl.IsNotSet()
                       ? "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot)
                       : avatarUrl;
        }

        /// <summary>
        /// protects from script in "location" field
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The get string safely.
        /// </returns>
        protected string GetStringSafely(object value)
        {
            return value == null ? string.Empty : this.HtmlEncode(value.ToString());
        }

        /// <summary>
        /// Get all users from user_list for this board.
        /// </summary>
        /// <param name="literals">
        /// The literals.
        /// </param>
        /// <param name="lastUserId">
        /// The last User Id.
        /// </param>
        /// <param name="specialSymbol">
        /// The special Symbol.
        /// </param>
        /// <param name="totalCount">
        /// The total Count.
        /// </param>
        /// <returns>
        /// The Members List
        /// </returns>
        protected DataTable GetUserList(string literals, int lastUserId, bool specialSymbol, out int totalCount)
        {
            this._userListDataTable = LegacyDb.user_listmembers(
                this.PageContext.PageBoardID,
                null,
                true,
                this.Group.SelectedIndex <= 0 ? null : this.Group.SelectedValue,
                this.Ranks.SelectedIndex <= 0 ? null : this.Ranks.SelectedValue,
                this.Get<YafBoardSettings>().UseStyledNicks,
                lastUserId,
                literals,
                specialSymbol,
                specialSymbol,
                this.Pager.CurrentPageIndex,
                this.Pager.PageSize,
                this.ViewState["SortNameField"].ToType<int?>(),
                this.ViewState["SortRankNameField"].ToType<int?>(),
                this.ViewState["SortJoinedField"].ToType<int?>(),
                this.ViewState["SortNumPostsField"].ToType<int?>(),
                this.ViewState["SortLastVisitField"].ToType<int?>(),
                this.NumPostsTB.Text.Trim().IsSet() ? this.NumPostsTB.Text.Trim().ToType<int>() : 0,
                this.NumPostDDL.SelectedIndex < 0 ? 3 : (this.NumPostsTB.Text.Trim().IsSet() ? this.NumPostDDL.SelectedValue.ToType<int>() : 0));

            if (this.Get<YafBoardSettings>().UseStyledNicks)
            {
                this.Get<IStyleTransform>().DecodeStyleByTable(this._userListDataTable);
            }

            if (this._userListDataTable.HasRows())
            {
                // commits the deletes to the table
                totalCount = (int)this._userListDataTable.Rows[0]["TotalCount"];
            }
            else
            {
                totalCount = 0;
            }

            return this._userListDataTable;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.DefaultButton = this.SearchByUserName.UniqueID;

            this.UserSearchName.Focus();

            if (this.IsPostBack)
            {
                return;
            }

            this.ViewState["SortNameField"] = 1;
            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.PageLinks.AddRoot().AddLink(this.GetText("TITLE"));

            //// this.SetSort("Name", true);

            this.UserName.Text = this.GetText("username");
            this.Rank.Text = this.GetText("rank");
            this.Joined.Text = this.GetText("joined");
            this.Posts.Text = this.GetText("posts");
            this.LastVisitLB.Text = this.GetText("members", "lastvisit");
       
            using (var dt = this.Get<IDbFunction>().GetAsDataTable(cdb => cdb.group_list(this.PageContext.PageBoardID, null)))
            {
                // add empty item for no filtering
                var newRow = dt.NewRow();
                newRow["Name"] = this.GetText("ALL");
                newRow["GroupID"] = DBNull.Value;
                dt.Rows.InsertAt(newRow, 0);

                var guestRows = dt.Select("Name='Guests'");

                if (guestRows.Length > 0)
                {
                    foreach (var row in guestRows)
                    {
                        row.Delete();
                    }
                }

                // commits the deletes to the table
                dt.AcceptChanges();

                this.Group.DataSource = dt;
                this.Group.DataTextField = "Name";
                this.Group.DataValueField = "GroupID";
                this.Group.DataBind();
            }

            this.NumPostDDL.Items.Add(new ListItem(this.GetText("MEMBERS", "NUMPOSTSEQUAL"), "1"));
            this.NumPostDDL.Items.Add(new ListItem(this.GetText("MEMBERS", "NUMPOSTSLESSOREQUAL"), "2"));
            this.NumPostDDL.Items.Add(new ListItem(this.GetText("MEMBERS", "NUMPOSTSMOREOREQUAL"), "3"));
           
            this.NumPostDDL.DataBind();

            // get list of user ranks for filtering
            using (var dt = this.Get<IDbFunction>().GetAsDataTable(cdb => cdb.rank_list(this.PageContext.PageBoardID, null)))
            {
                // add empty for for no filtering
                var newRow = dt.NewRow();
                newRow["Name"] = this.GetText("ALL");
                newRow["RankID"] = DBNull.Value;
                dt.Rows.InsertAt(newRow, 0);

                var guestRows = dt.Select("Name='Guest'");

                if (guestRows.Length > 0)
                {
                    foreach (var row in guestRows)
                    {
                        row.Delete();
                    }
                }

                // commits the deletes to the table
                dt.AcceptChanges();

                this.Ranks.DataSource = dt;
                this.Ranks.DataTextField = "Name";
                this.Ranks.DataValueField = "RankID";
                this.Ranks.DataBind();
            }

            this.BindData();
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
        protected void Pager_PageChange(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The joined_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Joined_Click(object sender, EventArgs e)
        {
            this.SetSort("Joined");

            this.ViewState["SortNameField"] = 0;
            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The LastVisitLB Click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void LastVisitLB_Click(object sender, EventArgs e)
        {
            this.SetSort("LastVisit");

            this.ViewState["SortNameField"] = 0;
            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The posts_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Posts_Click(object sender, EventArgs e)
        {
            this.SetSort("NumPosts");

            this.ViewState["SortNameField"] = 0;
            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The rank_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Rank_Click(object sender, EventArgs e)
        {
            this.SetSort("RankName");

            this.ViewState["SortNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The user name_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void UserName_Click(object sender, EventArgs e)
        {
            this.SetSort("Name");

            this.ViewState["SortRankNameField"] = 0;
            this.ViewState["SortJoinedField"] = 0;
            this.ViewState["SortNumPostsField"] = 0;
            this.ViewState["SortLastVisitField"] = 0;

            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.Pager.PageSize = this.Get<YafBoardSettings>().MemberListPageSize;
            var selectedCharLetter = this.AlphaSort1.CurrentLetter;

            // get the user list...
            int totalCount;

            var selectedLetter = this.UserSearchName.Text.IsSet() ? this.UserSearchName.Text.Trim() : selectedCharLetter.ToString();
            
            int numpostsTb;

            if (this.NumPostsTB.Text.Trim().IsSet() &&
                (!int.TryParse(this.NumPostsTB.Text.Trim(), out numpostsTb) || numpostsTb < 0 || numpostsTb > int.MaxValue))
            {
                this.PageContext.AddLoadMessage(this.GetText("MEMBERS", "INVALIDPOSTSVALUE"));
                return;
            }

            if (this.NumPostsTB.Text.Trim().IsNotSet())
            {
                this.NumPostsTB.Text = "0";
                this.NumPostDDL.SelectedValue = "3";
            }

            // get the user list...
            this._userListDataTable = this.GetUserList(
                selectedLetter,
                0,
                this.UserSearchName.Text.IsNotSet() || (selectedCharLetter == char.MinValue && selectedCharLetter == '#'),
                out totalCount);
            
            this.Pager.Count = totalCount;
            this.MemberList.DataSource = this._userListDataTable;
            this.DataBind();

            // handle the sort fields at the top
            // TODO: make these "sorts" into controls
            // this.SortAscendingName = (string)this.ViewState["SortField"] == nameField;
            // this.SortAscendingName.Value = "Name";
            switch (this.ViewState["SortNameField"].ToType<int?>())
            {
                case 1:
                    this.SortUserName.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortUserName.Visible = true;
                    break;
                case 2:
                     this.SortUserName.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortUserName.Visible = true;
                    break;
                default:
                    this.ViewState["SortNameField"] = 0;
                    this.SortUserName.Visible = false;
                    break;
            }

            switch (this.ViewState["SortRankNameField"].ToType<int?>())
            {
                case 1:
                    this.SortRank.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortRank.Visible = true;
                    break;
                case 2:
                     this.SortRank.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortRank.Visible = true;
                    break;
                default:
                    this.ViewState["SortRankNameField"] = 0;
                    this.SortRank.Visible = false;
                    break;
            }

            switch (this.ViewState["SortJoinedField"].ToType<int?>())
            {
                case 1:
                    this.SortJoined.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortJoined.Visible = true;
                    break;
                case 2:
                     this.SortJoined.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortJoined.Visible = true;
                    break;
                default:
                    this.ViewState["SortJoinedField"] = 0;
                    this.SortJoined.Visible = false;
                    break;
            }

            switch (this.ViewState["SortNumPostsField"].ToType<int?>())
            {
                case 1:
                    this.SortPosts.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortPosts.Visible = true;
                    break;
                case 2:
                     this.SortPosts.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortPosts.Visible = true;
                    break;
                default:
                    this.ViewState["SortNumPostsField"] = 0;
                    this.SortPosts.Visible = false;
                    break;
            }

            switch (this.ViewState["SortLastVisitField"].ToType<int?>())
            {
                case 1:
                    this.SortLastVisit.Src = this.GetThemeContents(
                   "SORT", "ASCENDING");
                     this.SortLastVisit.Visible = true;
                    break;
                case 2:
                     this.SortLastVisit.Src = this.GetThemeContents(
                   "SORT", "DESCENDING");
                     this.SortLastVisit.Visible = true;
                    break;
                default:
                    this.ViewState["SortLastVisitField"] = 0;
                    this.SortLastVisit.Visible = false;
                    break;
            } 
        }

        /// <summary>
        /// Helper function for setting up the current sort on 
        /// the member list view
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        private void SetSort(string field)
        {
            switch (field)
            {
                case "Name":
                    this.ViewState["SortNameField"] = this.ViewState["SortNameField"] == null ? 0 : (this.ViewState["SortNameField"].ToType<int>() == 1 ? 2 : 1);
                    break;
                case "RankName":
                    this.ViewState["SortRankNameField"] = this.ViewState["SortRankNameField"] == null ? 0 : (this.ViewState["SortRankNameField"].ToType<int>() == 1 ? 2 : 1);
                    break;
                case "Joined":
                    this.ViewState["SortJoinedField"] = this.ViewState["SortJoinedField"] == null ? 0 : (this.ViewState["SortJoinedField"].ToType<int>() == 1 ? 2 : 1);
                    break;
                case "NumPosts":
                    this.ViewState["SortNumPostsField"] = this.ViewState["SortNumPostsField"] == null ? 0 : (this.ViewState["SortNumPostsField"].ToType<int>() == 1 ? 2 : 1);
                    break;
                case "LastVisit":
                    this.ViewState["SortLastVisitField"] = this.ViewState["SortLastVisitField"] == null ? 0 : (this.ViewState["SortLastVisitField"].ToType<int>() == 1 ? 2 : 1);
                    break;
            }
        }
        #endregion
    }
}