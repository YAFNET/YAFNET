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

namespace yaf
{
	/// <summary>
	/// Summary description for members.
	/// </summary>
	public class members : BasePage
	{
		protected System.Web.UI.WebControls.Repeater MemberList;
		protected System.Web.UI.WebControls.HyperLink HomeLink, ThisLink;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks1;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks2;
		protected LinkButton UserName,Joined,Posts, GoPage, Rank;
		protected HtmlImage SortUserName, SortRank, SortJoined, SortPosts;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!User.Identity.IsAuthenticated)
				Response.Redirect(String.Format("login.aspx?ReturnUrl={0}",Request.RawUrl));

			if(!IsPostBack) 
			{
				HomeLink.Text = ForumName;
				HomeLink.NavigateUrl = BaseDir;
				ThisLink.NavigateUrl = Request.RawUrl;
				ThisLink.Text = GetText("title");

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

		private void GoPage_Click(object sender, System.EventArgs e) 
		{
			ViewState["PageNo"] = int.Parse(Request.Form["__EVENTARGUMENT"]);
			BindData();
		}

		private void BindData() 
		{
			int CurrentPage = 0;
			if(ViewState["PageNo"]!=null)
				CurrentPage = (int)ViewState["PageNo"];

			PagedDataSource pds = new PagedDataSource();
			DataView dv = DB.user_list(null,true).DefaultView;
			dv.Sort = String.Format("{0} {1}",ViewState["SortField"],(bool)ViewState["SortAscending"] ? "asc" : "desc");
			pds.DataSource = dv;
			pds.AllowPaging = true;
			pds.CurrentPageIndex = CurrentPage;
			pds.PageSize = 20;
			
			MemberList.DataSource = pds;
			DataBind();

			if(pds.PageCount>1) 
			{
				PageLinks1.InnerHtml = String.Format(GetText("pages"),pds.PageCount);
				for(int i=0;i<pds.PageCount;i++) 
				{
					if(i==pds.CurrentPageIndex) 
					{
						PageLinks1.InnerHtml += String.Format(" [{0}]",i+1);
					} 
					else 
					{
						PageLinks1.InnerHtml += String.Format(" <a href=\"javascript:__doPostBack('GoPage','{1}')\">{0}</a>",i+1,i);
					}
				}
				PageLinks2.InnerHtml = PageLinks1.InnerHtml;
			} 
			else 
			{
				PageLinks1.Visible = false;
				PageLinks2.Visible = false;
			}
			DataBind();
			SortUserName.Visible = (string)ViewState["SortField"] == "Name";
			SortUserName.Src = ThemeFile((bool)ViewState["SortAscending"] ? "sort_up.gif" : "sort_down.gif");
			SortRank.Visible = (string)ViewState["SortField"] == "RankName";
			SortRank.Src = ThemeFile((bool)ViewState["SortAscending"] ? "sort_up.gif" : "sort_down.gif");
			SortJoined.Visible = (string)ViewState["SortField"] == "Joined";
			SortJoined.Src = ThemeFile((bool)ViewState["SortAscending"] ? "sort_up.gif" : "sort_down.gif");
			SortPosts.Visible = (string)ViewState["SortField"] == "NumPosts";
			SortPosts.Src = ThemeFile((bool)ViewState["SortAscending"] ? "sort_up.gif" : "sort_down.gif");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			this.UserName.Click += new EventHandler(this.UserName_Click);
			this.Joined.Click += new EventHandler(this.Joined_Click);
			this.Posts.Click += new EventHandler(this.Posts_Click);
			this.Rank.Click += new EventHandler(this.Rank_Click);
			this.GoPage.Click += new EventHandler(this.GoPage_Click);
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
