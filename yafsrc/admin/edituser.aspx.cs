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
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.admin
{
	/// <summary>
	/// Summary description for edituser.
	/// </summary>
	public class edituser : BasePage
	{
		protected System.Web.UI.WebControls.TextBox Name;
		protected System.Web.UI.WebControls.TextBox Email;
		protected System.Web.UI.WebControls.TextBox Joined;
		protected System.Web.UI.WebControls.TextBox LastVisit;
		protected System.Web.UI.WebControls.DropDownList GroupList;
		protected System.Web.UI.WebControls.Button Save;
		protected System.Web.UI.WebControls.Button Cancel;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsAdmin) Response.Redirect(BaseDir);
			TopMenu = false;
			if(!IsPostBack) {
				BindData();
				using(SqlCommand cmd = new SqlCommand("yaf_user_list")) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@UserID",Request.QueryString["u"]);
					using(DataTable dt = DataManager.GetData(cmd)) {
						DataRow row = dt.Rows[0];
						Name.Text = (string)row["Name"];
						Email.Text = row["Email"].ToString();
						Joined.Text = row["Joined"].ToString();
						LastVisit.Text = row["LastVisit"].ToString();
						GroupList.Items.FindByValue(row["GroupID"].ToString()).Selected = true;
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
			GroupList.DataSource = DataManager.GetData("yaf_group_list",CommandType.StoredProcedure);
			DataBind();
		}

		private void Cancel_Click(object sender, System.EventArgs e) {
			Response.Redirect("users.aspx");
		}

		private void Save_Click(object sender, System.EventArgs e) {
			using(SqlCommand cmd = new SqlCommand("yaf_user_adminsave")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",Request.QueryString["u"]);
				cmd.Parameters.Add("@GroupID",GroupList.SelectedItem.Value);
				cmd.Parameters.Add("@Name",Name.Text);
				DataManager.ExecuteNonQuery(cmd);
			}
			Response.Redirect("users.aspx");
		}
	}
}
