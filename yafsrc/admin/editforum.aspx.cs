/* Copyright (C) 2003 Bjørnar Henden
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
using System.Drawing;
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
					using(SqlCommand cmd = new SqlCommand("yaf_forum_list")) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@ForumID",Request.QueryString["f"]);
						using(DataTable dt = DataManager.GetData(cmd)) {
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
		}

		private void BindData() {
			CategoryList.DataSource = DataManager.GetData("yaf_category_list",CommandType.StoredProcedure);
			if(Request.QueryString["f"] != null) {
				using(SqlCommand cmd = new SqlCommand("yaf_forumaccess_list")) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@ForumID",Request.QueryString["f"]);
					AccessList.DataSource = DataManager.GetData(cmd);
				}
			}
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
				int ForumID = 0;
				if(Request.QueryString["f"] != null) ForumID = int.Parse(Request.QueryString["f"]);
				
				SqlCommand cmd = new SqlCommand("yaf_forum_save");
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@CategoryID",CategoryList.SelectedItem.Value);
				cmd.Parameters.Add("@Name",Name.Text);
				cmd.Parameters.Add("@Description",Description.Text);
				cmd.Parameters.Add("@SortOrder",SortOrder.Text);
				cmd.Parameters.Add("@Locked",Locked.Checked);
				cmd.Parameters.Add("@Hidden",HideNoAccess.Checked);
				cmd.Parameters.Add("@IsTest",IsTest.Checked);
				
				ForumID = int.Parse(DataManager.ExecuteScalar(cmd).ToString());

				// Access
				if(Request.QueryString["f"] != null) 
				{
					for(int i=0;i<AccessList.Items.Count;i++) 
					{
						RepeaterItem item = AccessList.Items[i];
						int GroupID = int.Parse(((Label)item.FindControl("GroupID")).Text);
						using(SqlCommand cmd2 = new SqlCommand("yaf_forumaccess_save")) {
							cmd2.CommandType = CommandType.StoredProcedure;
							cmd2.Parameters.Add("@ForumID",ForumID);
							cmd2.Parameters.Add("@GroupID",GroupID);
							cmd2.Parameters.Add("@ReadAccess",((CheckBox)item.FindControl("ReadAccess")).Checked);
							cmd2.Parameters.Add("@PostAccess",((CheckBox)item.FindControl("PostAccess")).Checked);
							cmd2.Parameters.Add("@ReplyAccess",((CheckBox)item.FindControl("ReplyAccess")).Checked);
							cmd2.Parameters.Add("@PriorityAccess",((CheckBox)item.FindControl("PriorityAccess")).Checked);
							cmd2.Parameters.Add("@PollAccess",((CheckBox)item.FindControl("PollAccess")).Checked);
							cmd2.Parameters.Add("@VoteAccess",((CheckBox)item.FindControl("VoteAccess")).Checked);
							cmd2.Parameters.Add("@ModeratorAccess",((CheckBox)item.FindControl("ModeratorAccess")).Checked);
							cmd2.Parameters.Add("@EditAccess",((CheckBox)item.FindControl("EditAccess")).Checked);
							cmd2.Parameters.Add("@DeleteAccess",((CheckBox)item.FindControl("DeleteAccess")).Checked);
							DataManager.ExecuteNonQuery(cmd2);
						}
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
