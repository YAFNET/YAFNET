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
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks1;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks2;
		protected LinkButton UserName,Joined,Posts, GoPage, Rank;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!User.Identity.IsAuthenticated)
				Response.Redirect(String.Format("login.aspx?ReturnUrl={0}",Request.RawUrl));

			HomeLink.Text = ForumName;
			HomeLink.NavigateUrl = BaseDir;

			if(!IsPostBack)
				BindData();
		}

		private void UserName_Click(object sender, System.EventArgs e) 
		{
			ViewState["SortOrder"] = "Name asc";
			BindData();
		}

		private void Joined_Click(object sender, System.EventArgs e) 
		{
			ViewState["SortOrder"] = "Joined asc";
			BindData();
		}

		private void Posts_Click(object sender, System.EventArgs e) 
		{
			ViewState["SortOrder"] = "NumPosts desc";
			BindData();
		}

		private void Rank_Click(object sender, System.EventArgs e) 
		{
			ViewState["SortOrder"] = "RankName asc";
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
			if(ViewState["SortOrder"]!=null)
				dv.Sort = (string)ViewState["SortOrder"];
			pds.DataSource = dv;
			pds.AllowPaging = true;
			pds.CurrentPageIndex = CurrentPage;
			pds.PageSize = 20;
			
			MemberList.DataSource = pds;
			DataBind();

			if(pds.PageCount>1) 
			{
				PageLinks1.InnerHtml = String.Format("{0} Pages:",pds.PageCount);
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
