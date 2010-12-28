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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Data;
  using System.Linq;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
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
    public void search_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // re-bind data
      this.BindData(true);
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
    protected string GetStringSafely([NotNull] object svalue)
    {
      return svalue == null ? string.Empty : this.HtmlEncode(svalue.ToString());
    }

    /// <summary>
    /// Get all users from user_list for this board.
    /// </summary>
    /// <returns>
    /// The Members List
    /// </returns>
    [NotNull]
    protected DataTable GetUserList()
    {
      DataTable userListDataTable = DB.user_list(
        this.PageContext.PageBoardID, 
        null, 
        true, 
        this.group.SelectedIndex <= 0 ? null : this.group.SelectedValue, 
        this.Ranks.SelectedIndex <= 0 ? null : this.Ranks.SelectedValue, 
        YafContext.Current.BoardSettings.UseStyledNicks);

      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        new StyleTransform(YafContext.Current.Theme).DecodeStyleByTable(ref userListDataTable, false);
      }

      // select only the guest user (if one exists)
      DataRow[] guestRows = userListDataTable.Select("IsGuest > 0");

      if (guestRows.Length > 0)
      {
        foreach (DataRow row in guestRows)
        {
          row.Delete();
        }

        // commits the deletes to the table
        userListDataTable.AcceptChanges();
      }

      return userListDataTable;
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
    protected void Joined_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.Page.Form.DefaultButton = this.search.UniqueID;

      this.search.Focus();

      if (this.IsPostBack)
      {
        return;
      }

      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
      this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

      this.SetSort("Name", true);

      this.UserName.Text = this.GetText("username");
      this.Rank.Text = this.GetText("rank");
      this.Joined.Text = this.GetText("joined");
      this.Posts.Text = this.GetText("posts");
      this.Location.Text = this.GetText("location");

      using (DataTable dt = DB.group_list(this.PageContext.PageBoardID, null))
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

        this.group.DataSource = dt;
        this.group.DataTextField = "Name";
        this.group.DataValueField = "GroupID";
        this.group.DataBind();
      }

      // get list of user ranks for filtering
      using (DataTable dt = DB.rank_list(this.PageContext.PageBoardID, null))
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
    protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void Posts_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void Rank_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void UserName_Click([NotNull] object sender, [NotNull] EventArgs e)
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
      this.Pager.PageSize = 20;

      // get the user list...
      DataTable userListDataTable = this.GetUserList();

      // get the view from the datatable
      DataView userListDataView = userListDataTable.DefaultView;

      string nameField = this.PageContext.BoardSettings.EnableDisplayName ? "DisplayName" : "Name";

      if (!isSearch)
      {
        char selectedLetter = this.AlphaSort1.CurrentLetter;

        // handle dataview filtering
        if (selectedLetter != char.MinValue)
        {
          if (selectedLetter == '#')
          {
            string filter = string.Empty;

            foreach (char letter in this.GetText("LANGUAGE", "CHARSET").Where(letter => letter != '/'))
            {
              if (filter == string.Empty)
              {
                filter = "{0} not like '{1}%'".FormatWith(nameField, letter);
              }
              else
              {
                filter += "and {0} not like '{1}%'".FormatWith(nameField, letter);
              }
            }

            userListDataView.RowFilter = filter;
          }
          else
          {
            userListDataView.RowFilter = "{0} like '{1}%'".FormatWith(nameField, selectedLetter);
          }
        }
      }
      else
      {
        // filter by name or email
        if (this.name.Text.Trim().Length > 0)
        {
          userListDataView.RowFilter =
            "(Name LIKE '%{0}%' OR DisplayName LIKE '%{0}%')".FormatWith(this.name.Text.Trim());
        }
      }

      this.Pager.Count = userListDataView.Count;

      // create paged data source for the memberlist
      userListDataView.Sort = "{0} {1}".FormatWith(
        this.ViewState["SortField"], (bool)this.ViewState["SortAscending"] ? "asc" : "desc");
      var pds = new PagedDataSource
        {
          DataSource = userListDataView, 
          AllowPaging = true, 
          CurrentPageIndex = this.Pager.CurrentPageIndex, 
          PageSize = this.Pager.PageSize
        };

      this.MemberList.DataSource = pds;
      this.DataBind();

      // handle the sort fields at the top
      // TODO: make these "sorts" into controls
      this.SortUserName.Visible = (string)this.ViewState["SortField"] == nameField;
      this.SortUserName.Src = this.GetThemeContents(
        "SORT", (bool)this.ViewState["SortAscending"] ? "ASCENDING" : "DESCENDING");
      this.SortRank.Visible = (string)this.ViewState["SortField"] == "RankName";
      this.SortRank.Src = this.SortUserName.Src;
      this.SortJoined.Visible = (string)this.ViewState["SortField"] == "Joined";
      this.SortJoined.Src = this.SortUserName.Src;
      this.SortPosts.Visible = (string)this.ViewState["SortField"] == "NumPosts";
      this.SortPosts.Src = this.SortUserName.Src;
    }

    /// <summary>
    /// Helper function for setting up the current sort on the memberlist view
    /// </summary>
    /// <param name="field">
    /// </param>
    /// <param name="asc">
    /// </param>
    private void SetSort([NotNull] string field, bool asc)
    {
      if (this.ViewState["SortField"] != null && (string)this.ViewState["SortField"] == field)
      {
        this.ViewState["SortAscending"] = !(bool)this.ViewState["SortAscending"];
      }
      else
      {
        this.ViewState["SortField"] = field;
        this.ViewState["SortAscending"] = asc;
      }
    }

    #endregion
  }
}