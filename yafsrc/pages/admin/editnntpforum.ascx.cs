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

namespace yaf.pages.admin
{
	/// <summary>
	/// Summary description for editgroup.
	/// </summary>
	public class editnntpforum : AdminPage
	{
		protected DropDownList NntpServerID, ForumID;
		protected TextBox GroupName;
		protected Button Save, Cancel;
		protected controls.PageLinks PageLinks;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(Config.BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("NNTP Forums",Forum.GetLink(Pages.admin_nntpforums));
				
				BindData();
				if(Request.QueryString["s"] != null) 
				{
					using(DataTable dt = DB.nntpforum_list(PageBoardID,null,Request.QueryString["s"]))
					{
						DataRow row = dt.Rows[0];
						NntpServerID.Items.FindByValue(row["NntpServerID"].ToString()).Selected = true;
						GroupName.Text = row["GroupName"].ToString();
						ForumID.Items.FindByValue(row["ForumID"].ToString()).Selected = true;
					}
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
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
			this.Save.Click += new System.EventHandler(this.Save_Click);
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void BindData() {
			NntpServerID.DataSource = DB.nntpserver_list(PageBoardID,null);
			NntpServerID.DataValueField = "NntpServerID";
			NntpServerID.DataTextField = "Name";
			ForumID.DataSource = DB.forum_list(PageBoardID,null);
			ForumID.DataValueField = "ForumID";
			ForumID.DataTextField = "Name";
			DataBind();
		}

		private void Cancel_Click(object sender, System.EventArgs e)
		{
			Forum.Redirect(Pages.admin_nntpforums);
		}

		private void Save_Click(object sender, System.EventArgs e)
		{
			object nntpForumID = null;
			if(Request.QueryString["s"]!=null) nntpForumID = Request.QueryString["s"];
			DB.nntpforum_save(nntpForumID,NntpServerID.SelectedValue,GroupName.Text,ForumID.SelectedValue);
			Forum.Redirect(Pages.admin_nntpforums);
		}
	}
}
