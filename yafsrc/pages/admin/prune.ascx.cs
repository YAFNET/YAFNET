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
	/// Summary description for prune.
	/// </summary>
	public class prune : AdminPage
	{
		protected DropDownList forumlist;
		protected TextBox days;
		protected Button commit;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				days.Text = "60";
				BindData();
			}
		}

		private void BindData() {
			forumlist.DataSource = DB.forum_listread(PageBoardID,PageInfo.PageUserID,null,null);
			forumlist.DataValueField = "ForumID";
			forumlist.DataTextField = "Forum";
			DataBind();
			forumlist.Items.Insert(0,new ListItem("All Forums","0"));
		}

		private void commit_Click(object sender,EventArgs e) {
			int Count = DB.topic_prune(forumlist.SelectedValue,days.Text);
			AddLoadMessage(String.Format("{0} topic(s) deleted.",Count));
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			commit.Click += new EventHandler(commit_Click);
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
