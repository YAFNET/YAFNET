/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.pages
{
	/// <summary>
	/// Summary description for members.
	/// </summary>
	public class members : ForumPage
	{
		protected Repeater MemberList;
		protected LinkButton UserName,Joined,Posts, Rank;
		protected HtmlImage SortUserName, SortRank, SortJoined, SortPosts;
		protected controls.PageLinks PageLinks;
		protected controls.Pager Pager;

		public members() : base("MEMBERS")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!User.IsAuthenticated)
			{
				if(User.CanLogin)
					Forum.Redirect(Pages.login,"ReturnUrl={0}",Request.RawUrl);
				else
					Forum.Redirect(Pages.forum);
			}

			if(!IsPostBack) 
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(GetText("TITLE"),Request.RawUrl);

				SetSort("Name",true);

				UserName.Text = GetText("username");
				Rank.Text = GetText("rank");
				Joined.Text = GetText("joined");
				Posts.Text = GetText("posts");

				BindData();
			}
		}

		private void SetSort(string field,bool asc) 
		{
			if(ViewState["SortField"]!=null && (string)ViewState["SortField"] == field) 
			{
				ViewState["SortAscending"] = !(bool)ViewState["SortAscending"];
			}
			else 
			{
				ViewState["SortField"] = field;
				ViewState["SortAscending"] = asc;
			}
		}

		private void UserName_Click(object sender, System.EventArgs e) 
		{
			SetSort("Name",true);
			BindData();
		}

		private void Joined_Click(object sender, System.EventArgs e) 
		{
			SetSort("Joined",true);
			BindData();
		}

		private void Posts_Click(object sender, System.EventArgs e) 
		{
			SetSort("NumPosts",false);
			BindData();
		}

		private void Rank_Click(object sender, System.EventArgs e) 
		{
			SetSort("RankName",true);
			BindData();
		}

		private void Pager_PageChange(object sender, EventArgs e)
		{
			BindData();
		}

		private void BindData() 
		{
			Pager.PageSize = 20;

			DataView dv = DB.user_list(PageBoardID,null,true).DefaultView;
			Pager.Count = dv.Count;

			dv.Sort = String.Format("{0} {1}",ViewState["SortField"],(bool)ViewState["SortAscending"] ? "asc" : "desc");
			PagedDataSource pds = new PagedDataSource();
			pds.DataSource = dv;
			pds.AllowPaging = true;
			pds.CurrentPageIndex = Pager.CurrentPageIndex;
			pds.PageSize = Pager.PageSize;
			
			MemberList.DataSource = pds;
			DataBind();

			SortUserName.Visible = (string)ViewState["SortField"] == "Name";
			SortUserName.Src = GetThemeContents("SORT",(bool)ViewState["SortAscending"] ? "ASCENDING" : "DESCENDING");
			SortRank.Visible = (string)ViewState["SortField"] == "RankName";
			SortRank.Src = SortUserName.Src;
			SortJoined.Visible = (string)ViewState["SortField"] == "Joined";
			SortJoined.Src = SortUserName.Src;
			SortPosts.Visible = (string)ViewState["SortField"] == "NumPosts";
			SortPosts.Src = SortUserName.Src;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			this.UserName.Click += new EventHandler(this.UserName_Click);
			this.Joined.Click += new EventHandler(this.Joined_Click);
			this.Posts.Click += new EventHandler(this.Posts_Click);
			this.Rank.Click += new EventHandler(this.Rank_Click);
			this.Pager.PageChange += new EventHandler(Pager_PageChange);
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
