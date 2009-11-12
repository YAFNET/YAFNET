/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
  using System;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for members.
  /// </summary>
  public partial class members : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="members"/> class.
    /// </summary>
    public members()
      : base("MEMBERS")
    {
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
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(GetText("TITLE"), string.Empty);

        SetSort("Name", true);

        this.UserName.Text = GetText("username");
        this.Rank.Text = GetText("rank");
        this.Joined.Text = GetText("joined");
        this.Posts.Text = GetText("posts");
        this.Location.Text = GetText("location");

        BindData();
      }
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
      return svalue == null ? string.Empty : HtmlEncode(svalue.ToString());
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
      if (ViewState["SortField"] != null && (string) ViewState["SortField"] == field)
      {
        ViewState["SortAscending"] = !(bool) ViewState["SortAscending"];
      }
      else
      {
        ViewState["SortField"] = field;
        ViewState["SortAscending"] = asc;
      }
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
      SetSort("Name", true);
      BindData();
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
      SetSort("Joined", true);
      BindData();
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
      SetSort("NumPosts", false);
      BindData();
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
      SetSort("RankName", true);
      BindData();
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
      BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.Pager.PageSize = 20;

      // get the user list
      DataTable userListDataTable = DB.user_list(PageContext.PageBoardID, null, true);

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

      // get the view from the datatable
      DataView userListDataView = userListDataTable.DefaultView;

      char selectedLetter = this.AlphaSort1.CurrentLetter;

      // handle dataview filtering
      if (selectedLetter != char.MinValue)
      {
        if (selectedLetter == '#')
        {
          string filter = string.Empty;
          foreach (char letter in GetText("LANGUAGE", "CHARSET"))
          {
            if (filter == string.Empty)
            {
              filter = string.Format("Name not like '{0}%'", letter);
            }
            else
            {
              filter += string.Format("and Name not like '{0}%'", letter);
            }
          }

          userListDataView.RowFilter = filter;
        }
        else
        {
          userListDataView.RowFilter = string.Format("Name like '{0}%'", selectedLetter);
        }
      }

      this.Pager.Count = userListDataView.Count;

      // create paged data source for the memberlist
      userListDataView.Sort = String.Format("{0} {1}", ViewState["SortField"], (bool) ViewState["SortAscending"] ? "asc" : "desc");
      var pds = new PagedDataSource();
      pds.DataSource = userListDataView;
      pds.AllowPaging = true;
      pds.CurrentPageIndex = this.Pager.CurrentPageIndex;
      pds.PageSize = this.Pager.PageSize;

      this.MemberList.DataSource = pds;
      DataBind();

      // handle the sort fields at the top
      // TODO: make these "sorts" into controls
      this.SortUserName.Visible = (string) ViewState["SortField"] == "Name";
      this.SortUserName.Src = GetThemeContents("SORT", (bool) ViewState["SortAscending"] ? "ASCENDING" : "DESCENDING");
      this.SortRank.Visible = (string) ViewState["SortField"] == "RankName";
      this.SortRank.Src = this.SortUserName.Src;
      this.SortJoined.Visible = (string) ViewState["SortField"] == "Joined";
      this.SortJoined.Src = this.SortUserName.Src;
      this.SortPosts.Visible = (string) ViewState["SortField"] == "NumPosts";
      this.SortPosts.Src = this.SortUserName.Src;
    }
  }
}