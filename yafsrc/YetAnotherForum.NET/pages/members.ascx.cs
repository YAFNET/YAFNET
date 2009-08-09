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

using System;
using System.Data;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for members.
	/// </summary>
	public partial class members : YAF.Classes.Core.ForumPage
	{

		public members()
			: base("MEMBERS")
		{
		}

		/// <summary>
		/// Called when the page loads
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
				PageLinks.AddLink(GetText("TITLE"), "");

				SetSort("Name", true);

				UserName.Text = GetText("username");
				Rank.Text = GetText("rank");
				Joined.Text = GetText("joined");
				Posts.Text = GetText("posts");
				Location.Text = GetText("location");

				BindData();
			}
		}

		/// <summary>
		/// protects from script in "location" field	    
		/// </summary>
		/// <param name="svalue"></param>
		/// <returns></returns>
		protected string GetStringSafely(object svalue)
		{
			return HtmlEncode(svalue.ToString());
		}

		/// <summary>
		/// Helper function for setting up the current sort on the memberlist view
		/// </summary>
		/// <param name="field"></param>
		/// <param name="asc"></param>
		private void SetSort(string field, bool asc)
		{
			if (ViewState["SortField"] != null && (string)ViewState["SortField"] == field)
			{
				ViewState["SortAscending"] = !(bool)ViewState["SortAscending"];
			}
			else
			{
				ViewState["SortField"] = field;
				ViewState["SortAscending"] = asc;
			}
		}

		protected void UserName_Click(object sender, System.EventArgs e)
		{
			SetSort("Name", true);
			BindData();
		}

		protected void Joined_Click(object sender, System.EventArgs e)
		{
			SetSort("Joined", true);
			BindData();
		}

		protected void Posts_Click(object sender, System.EventArgs e)
		{
			SetSort("NumPosts", false);
			BindData();
		}

		protected void Rank_Click(object sender, System.EventArgs e)
		{
			SetSort("RankName", true);
			BindData();
		}

		protected void Pager_PageChange(object sender, EventArgs e)
		{
			BindData();
		}

		private void BindData()
		{
			Pager.PageSize = 20;

			// get the user list
			DataTable userListDataTable = YAF.Classes.Data.DB.user_list(PageContext.PageBoardID, null, true);

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

			char selectedLetter = AlphaSort1.CurrentLetter;

			// handle dataview filtering
			if (selectedLetter != char.MinValue)
			{
				if (selectedLetter == '#')
				{
					string filter = string.Empty;
					foreach (char letter in GetText("LANGUAGE", "CHARSET"))
					{
						if (filter == string.Empty)
							filter = string.Format("Name not like '{0}%'", letter);
						else
							filter += string.Format("and Name not like '{0}%'", letter);
					}
					userListDataView.RowFilter = filter;
				}
				else
				{
					userListDataView.RowFilter = string.Format("Name like '{0}%'", selectedLetter);
				}
			}

			Pager.Count = userListDataView.Count;

			// create paged data source for the memberlist
			userListDataView.Sort = String.Format("{0} {1}", ViewState["SortField"], (bool)ViewState["SortAscending"] ? "asc" : "desc");
			PagedDataSource pds = new PagedDataSource();
			pds.DataSource = userListDataView;
			pds.AllowPaging = true;
			pds.CurrentPageIndex = Pager.CurrentPageIndex;
			pds.PageSize = Pager.PageSize;

			MemberList.DataSource = pds;
			DataBind();

			// handle the sort fields at the top
			// TODO: make these "sorts" into controls
			SortUserName.Visible = (string)ViewState["SortField"] == "Name";
			SortUserName.Src = GetThemeContents("SORT", (bool)ViewState["SortAscending"] ? "ASCENDING" : "DESCENDING");
			SortRank.Visible = (string)ViewState["SortField"] == "RankName";
			SortRank.Src = SortUserName.Src;
			SortJoined.Visible = (string)ViewState["SortField"] == "Joined";
			SortJoined.Src = SortUserName.Src;
			SortPosts.Visible = (string)ViewState["SortField"] == "NumPosts";
			SortPosts.Src = SortUserName.Src;
		}
	}
}
