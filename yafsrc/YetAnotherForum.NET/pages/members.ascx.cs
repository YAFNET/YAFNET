/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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

using YAF.Types;
using YAF.Types.Constants;
using YAF.Types.Interfaces;

namespace YAF.Pages
{
    // YAF.Pages
    #region

    using System;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Pattern;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Summary description for members.
    /// </summary>
    public partial class members : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "members" /> class.
        /// </summary>
        public members()
            : base("MEMBERS")
        {
        }

        #endregion

        #region Fields
        /// <summary>
        /// The _userListDataTable.
        /// </summary>
        private DataTable _userListDataTable;

        /// <summary>
        /// The _sortName.
        /// </summary>
        private bool _sortName = true;

        /// <summary>
        /// The _sortRank.
        /// </summary>
        private bool _sortRank;

        /// <summary>
        /// The _sortPosts.
        /// </summary>
        private bool _sortPosts;

        /// <summary>
        /// The _sortJoined.
        /// </summary>
        private bool _sortJoined; 
        #endregion

        #region Public Methods

        /// <summary>
        /// The search_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void Search_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-bind data
            this.BindData(true);
        }

        /// <summary>
        /// The search_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void Reset_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // re-direct to self.
            YafBuildLink.Redirect(ForumPages.members);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get avatar url from id.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// The get avatar url from id.
        /// </returns>
        protected string GetAvatarUrlFromID(int userID)
        {
            string avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userID);

            if (avatarUrl.IsNotSet())
            {
                avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
            }

            return avatarUrl;
        }

        /// <summary>
        /// protects from script in "location" field
        /// </summary>
        /// <param name="svalue">
        /// </param>
        /// <returns>
        /// The get string safely.
        /// </returns>
        protected string GetStringSafely(object svalue)
        {
            return svalue == null ? string.Empty : this.HtmlEncode(svalue.ToString());
        }

        /// <summary>
        /// Get all users from user_list for this board.
        /// </summary>
        /// <returns>
        /// The Members List
        /// </returns>
        protected DataTable GetUserList(string literals, int lastUserId, bool specialSymbol, out int totalCount)
        {
            _userListDataTable =  LegacyDb.user_listmembers(
                PageContext.PageBoardID,
                null,
                true,
                this.Group.SelectedIndex <= 0 ? null : this.Group.SelectedValue,
                this.Ranks.SelectedIndex <= 0 ? null : this.Ranks.SelectedValue,
                YafContext.Current.BoardSettings.UseStyledNicks,
                lastUserId,
                literals,
                specialSymbol,
                !specialSymbol ? false : true,
                this.Pager.CurrentPageIndex,
                Pager.PageSize,
                _sortName,
                _sortRank,
                _sortJoined,
                _sortPosts);
            if (YafContext.Current.BoardSettings.UseStyledNicks)
            {
                new StyleTransform(YafContext.Current.Theme).DecodeStyleByTable(ref _userListDataTable, false);
            }

            if (_userListDataTable.Rows.Count > 0)
            {
                // commits the deletes to the table
                totalCount = (int)_userListDataTable.Rows[0]["TotalCount"];
            }
            else
            {
                totalCount = 0;
            }
            return _userListDataTable;
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
            this.SetSort("Joined", true);
            this.BindData(false);
        }

        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.DefaultButton = this.SearchByUserName.UniqueID;

            this.SearchByUserName.Focus();

            if (this.IsPostBack)
            {
                return;
            }
            else
            {
                this.SetSort("Name", true);
            }

            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            // this.SetSort("Name", true);

            this.UserName.Text = this.GetText("username");
            this.Rank.Text = this.GetText("rank");
            this.Joined.Text = this.GetText("joined");
            this.Posts.Text = this.GetText("posts");
            this.Location.Text = this.GetText("location");

            using (DataTable dt = LegacyDb.group_list(this.PageContext.PageBoardID, null))
            {
                // add empty item for no filtering
                DataRow newRow = dt.NewRow();
                newRow["Name"] = this.GetText("ALL");
                newRow["GroupID"] = DBNull.Value;
                dt.Rows.InsertAt(newRow, 0);

                DataRow[] guestRows = dt.Select("Name='Guests'");

                if (guestRows.Length > 0)
                {
                    foreach (DataRow row in guestRows)
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

            // get list of user ranks for filtering
            using (DataTable dt = LegacyDb.rank_list(this.PageContext.PageBoardID, null))
            {
                // add empty for for no filtering
                DataRow newRow = dt.NewRow();
                newRow["Name"] = this.GetText("ALL");
                newRow["RankID"] = DBNull.Value;
                dt.Rows.InsertAt(newRow, 0);

                DataRow[] guestRows = dt.Select("Name='Guest'");

                if (guestRows.Length > 0)
                {
                    foreach (DataRow row in guestRows)
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

            this.BindData(false);
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
            this.BindData(false);
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
            this.SetSort("NumPosts", false);
            this.BindData(false);
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
            this.SetSort("RankName", true);
            this.BindData(false);
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
            this.SetSort("Name", true);
            this.BindData(false);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        /// <param name="isSearch">
        /// The search.
        /// </param>
        private void BindData(bool isSearch)
        {
            this.Pager.PageSize = PageContext.BoardSettings.MemberListPageSize;
            char selectedCharLetter = this.AlphaSort1.CurrentLetter;
            string selectedLetter;

            // get the user list...
            int totalCount = 0;
            if (this.UserSearchName.Text.IsSet())
            {
                selectedLetter = this.UserSearchName.Text.Trim();
            }
            else
            {
                selectedLetter = selectedCharLetter.ToString();
            }

            // get the user list...
            _userListDataTable = this.GetUserList(selectedLetter, 0, (this.UserSearchName.Text.IsNotSet()) || (selectedCharLetter == char.MinValue && selectedCharLetter == '#'), out  totalCount);
            
            // get the view from the datatable
            DataView userListDataView = _userListDataTable.DefaultView;
            string nameField = this.PageContext.BoardSettings.EnableDisplayName ? "DisplayName" : "Name";
            
            this.Pager.Count = totalCount;
            this.MemberList.DataSource = _userListDataTable;
            this.DataBind();

            // handle the sort fields at the top
            // TODO: make these "sorts" into controls
            // this.SortAscendingName = (string)this.ViewState["SortField"] == nameField;
            /*  this.SortAscendingName.Value = "Name";
            this.SortUserName.Src = this.GetThemeContents(
                "SORT", ascM ? "ASCENDING" : "DESCENDING");
            this.SortUserName.Visible = true;
            this.SortUserName.Src = this.GetThemeContents(
                "SORT", (bool)this.ViewState["SortAscending"] ? "ASCENDING" : "DESCENDING"); */
            /*  this.SortRank.Visible = (string)this.ViewState["SortField"] == "RankName";
            this.SortRank.Src = this.SortUserName.Src;
            this.SortJoined.Visible = (string)this.ViewState["SortField"] == "Joined";
            this.SortJoined.Src = this.SortUserName.Src;
            this.SortPosts.Visible = (string)this.ViewState["SortField"] == "NumPosts";
            this.SortPosts.Src = this.SortUserName.Src; */
            this.SortPosts.Visible = false;
            this.SortJoined.Visible = false;
            this.SortRank.Visible = false;
            this.SortUserName.Visible = false;
        }

        /// <summary>
        /// Get Theme Contents
        /// </summary>
        /// <param name="page">
        /// The Localization Page.
        /// </param>
        /// <param name="tag">
        /// The Localisation Page Tag.
        /// </param>
        /// <returns>
        /// Returns Theme Content.
        /// </returns>
        protected string GetThemeContents([NotNull] string page, [NotNull] string tag)
        {
            return YafContext.Current.Theme.GetItem(page, tag);
        }

        /// <summary>
        /// Helper function for setting up the current sort on the memberlist view
        /// </summary>
        /// <param name="field">
        /// </param>
        /// <param name="asc">
        /// </param>
        private void SetSort(string field, bool asc)
        {
          
        switch (field)
       {
            case "Name":
               _sortName = true;
             //   _sortName = !_sortName;
             //  this.SortUserName.Src = GetThemeContents("SORT", (bool) asc ? "ASCENDING" : "DESCENDING");
             //   this.SortUserName.Visible = (string) field == "Name";
                break;
           case "RankName":
                _sortRank = true;
              //  this.SortRank.Src = GetThemeContents("SORT", (bool) asc ? "ASCENDING" : "DESCENDING");
             //   this.SortRank.Visible = (string) field == "RankName";
                break;
            case "Joined":
                _sortJoined = true;
                break;
            case "NumPosts":
                _sortPosts = true;
                break;
            default: break;
        }  

     
         /*  this.SortRank.Src = this.SortUserName.Src;
             this.SortJoined.Visible = (string) ViewState["SortField"] == "Joined";
             this.SortJoined.Src = this.SortUserName.Src;
             this.SortPosts.Visible = (string) ViewState["SortField"] == "NumPosts";
             this.SortPosts.Src = this.SortUserName.Src; 
             this.SortAscendingName.Value = (!this.SortAscendingName.Value.ToType<bool>()).ToString();
             if (ViewState["SortField"] != null && (string) ViewState["SortField"] == field)
             {
             ViewState["SortAscending"] = !(bool) ViewState["SortAscending"];
             }
             {
             ViewState["SortField"] = field;
             if (ViewState["SortAscending"] == null)
             {ViewState["SortAscending"] = asc;
             }
           } */

        }

        #endregion
    }
}