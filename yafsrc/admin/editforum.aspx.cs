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

namespace yaf.admin
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class editforum : BasePage {
		protected System.Web.UI.WebControls.TextBox Name;
		protected System.Web.UI.WebControls.TextBox Description;
		protected System.Web.UI.WebControls.TextBox SortOrder;
		protected System.Web.UI.WebControls.Button Save;
		protected System.Web.UI.WebControls.Button Cancel;
		protected System.Web.UI.WebControls.CheckBox Locked;
		protected System.Web.UI.WebControls.DropDownList CategoryList;
		protected System.Web.UI.WebControls.Repeater AccessList;
		protected System.Web.UI.WebControls.CheckBox IsTest;
		protected System.Web.UI.WebControls.Label ForumNameTitle;
		protected System.Web.UI.WebControls.CheckBox HideNoAccess;
	
		private void Page_Load(object sender, System.EventArgs e) {
			if(!IsAdmin) Response.Redirect(BaseDir);

			TopMenu = false;

			if(!IsPostBack) {
				BindData();
				if(Request.QueryString["f"] != null) {
					using(DataTable dt = DB.forum_list(Request.QueryString["f"])) 
					{
						DataRow row = dt.Rows[0];
						Name.Text = (string)row["Name"];
						Description.Text = (string)row["Description"];
						SortOrder.Text = row["SortOrder"].ToString();
						HideNoAccess.Checked = (bool)row["Hidden"];
						Locked.Checked = (bool)row["Locked"];
						IsTest.Checked = (bool)row["IsTest"];
						ForumNameTitle.Text = Name.Text;

						CategoryList.Items.FindByValue(row["CategoryID"].ToString()).Selected = true;
					}
				}
			}
		}

		private void BindData() {
			CategoryList.DataSource = DB.category_list(null);
			if(Request.QueryString["f"] != null)
				AccessList.DataSource = DB.forumaccess_list(Request.QueryString["f"]);

			DataBind();
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

		private void Save_Click(object sender, System.EventArgs e)
		{
			if(IsValid) 
			{
				// Forum
				long ForumID = 0;
				if(Request.QueryString["f"] != null) ForumID = long.Parse(Request.QueryString["f"]);
				
				ForumID = DB.forum_save(ForumID,CategoryList.SelectedValue,Name.Text,Description.Text,SortOrder.Text,Locked.Checked,HideNoAccess.Checked,IsTest.Checked);

				// Access
				if(Request.QueryString["f"] != null) 
				{
					for(int i=0;i<AccessList.Items.Count;i++) 
					{
						RepeaterItem item = AccessList.Items[i];
						int GroupID = int.Parse(((Label)item.FindControl("GroupID")).Text);
						DB.forumaccess_save(ForumID,GroupID,
							((CheckBox)item.FindControl("ReadAccess")).Checked,
							((CheckBox)item.FindControl("PostAccess")).Checked,
							((CheckBox)item.FindControl("ReplyAccess")).Checked,
							((CheckBox)item.FindControl("PriorityAccess")).Checked,
							((CheckBox)item.FindControl("PollAccess")).Checked,
							((CheckBox)item.FindControl("VoteAccess")).Checked,
							((CheckBox)item.FindControl("ModeratorAccess")).Checked,
							((CheckBox)item.FindControl("EditAccess")).Checked,
							((CheckBox)item.FindControl("DeleteAccess")).Checked,
							((CheckBox)item.FindControl("UploadAccess")).Checked
						);
					}
					Response.Redirect("forums.aspx");
				}

				// Done
				Response.Redirect(String.Format("editforum.aspx?f={0}",ForumID));
			}
		}

		private void Cancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("forums.aspx");
		}

	}
}
