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
using System.Globalization;

namespace yaf.admin {
	/// <summary>
	/// Summary description for settings.
	/// </summary>
	public class settings : BasePage {
		protected System.Web.UI.WebControls.Button Save;
		protected System.Web.UI.WebControls.Label SQLVersion;
		protected System.Web.UI.WebControls.DropDownList TimeZones;
		protected System.Web.UI.WebControls.TextBox ForumSmtpServer;
		protected System.Web.UI.WebControls.TextBox ForumEmailEdit;
		protected System.Web.UI.WebControls.TextBox Name;
		protected System.Web.UI.WebControls.CheckBox EmailVerification, ShowMoved;
	
		private void Page_Load(object sender, System.EventArgs e) {
			if(!IsAdmin) Response.Redirect(BaseDir);

			TopMenu = false;

			if(!IsPostBack)
				BindData();
		}

		private void BindData() {
			DataRow row;
			TimeZones.DataSource = Data.TimeZones();
			using(DataTable dt = DataManager.GetData("yaf_system_list",CommandType.StoredProcedure)) {
				row = dt.Rows[0];
			}
			DataBind();
			SQLVersion.Text = (string)row["SQLVersion"];
			TimeZones.Items.FindByValue(row["TimeZone"].ToString()).Selected = true;
			Name.Text = (string)row["Name"];
			ForumSmtpServer.Text = (string)row["SmtpServer"];
			ForumEmailEdit.Text = (string)row["ForumEmail"];
			EmailVerification.Checked = (bool)row["EmailVerification"];
			ShowMoved.Checked = (bool)row["ShowMoved"];
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Save_Click(object sender, System.EventArgs e) {
			if(!IsValid) return;

			using(SqlCommand cmd = new SqlCommand("yaf_system_save")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@Name",Name.Text);
				cmd.Parameters.Add("@TimeZone",TimeZones.SelectedItem.Value);
				cmd.Parameters.Add("@SmtpServer",ForumSmtpServer.Text);
				cmd.Parameters.Add("@ForumEmail",ForumEmailEdit.Text);
				cmd.Parameters.Add("@EmailVerification",EmailVerification.Checked);
				cmd.Parameters.Add("@ShowMoved",ShowMoved.Checked);
				DataManager.ExecuteNonQuery(cmd);
			}
			Response.Redirect("main.aspx");
		}
	}
}
