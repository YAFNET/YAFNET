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
using System.Data.SqlClient;
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
		protected Label SortOrder, PageNo;

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
			SortOrder.Text = "Name asc";
			BindData();
		}

		private void Joined_Click(object sender, System.EventArgs e) 
		{
			SortOrder.Text = "Joined asc";
			BindData();
		}

		private void Posts_Click(object sender, System.EventArgs e) 
		{
			SortOrder.Text = "NumPosts desc";
			BindData();
		}

		private void Rank_Click(object sender, System.EventArgs e) 
		{
			SortOrder.Text = "RankName asc";
			BindData();
		}

		private void GoPage_Click(object sender, System.EventArgs e) 
		{
			PageNo.Text = Request.Form["__EVENTARGUMENT"];
			BindData();
		}

		private void BindData() 
		{
			int CurrentPage = 0;
			if(PageNo.Text.Length>0)
				CurrentPage = int.Parse(PageNo.Text);

			PagedDataSource pds = new PagedDataSource();
			using(SqlCommand cmd = new SqlCommand("yaf_user_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",null);
				cmd.Parameters.Add("@Approved",true);
				DataView dv = DataManager.GetData(cmd).DefaultView;
				if(SortOrder.Text.Length>0)
					dv.Sort = SortOrder.Text;
				pds.DataSource = dv;
			}
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
						//PageLinks1.InnerHtml += String.Format(" <a href=\"members.aspx?p={1}\">{0}</a>",i+1,i);
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
