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
	/// Summary description for mail.
	/// </summary>
	public class mail : BasePage
	{
		protected System.Web.UI.WebControls.TextBox Subject;
		protected System.Web.UI.WebControls.DropDownList ToList;
		protected System.Web.UI.WebControls.Button Send;
		protected System.Web.UI.WebControls.TextBox Body;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsAdmin) Response.Redirect(BaseDir);
			TopMenu = false;
			if(!IsPostBack) BindData();
		}

		private void BindData() {
			ToList.DataSource = DataManager.GetData("yaf_group_list",CommandType.StoredProcedure);
			DataBind();

			ListItem item = new ListItem("All Users","0");
			ToList.Items.Insert(0,item);
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
			this.Send.Click += new System.EventHandler(this.Send_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Send_Click(object sender, System.EventArgs e) {
			using(SqlCommand cmd = new SqlCommand("yaf_user_emails")) {
				cmd.CommandType = CommandType.StoredProcedure;
				if(ToList.SelectedItem.Value!="0")
					cmd.Parameters.Add("@GroupID",ToList.SelectedItem.Value);

				using(DataTable dt = DataManager.GetData(cmd)) {
					for(int i=0;i<dt.Rows.Count;i++) {
						//  Build a MailMessage
						System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
						mailMessage.From = ForumEmail;
						mailMessage.To = (string)dt.Rows[i]["Email"];
						mailMessage.Subject = Subject.Text;
						mailMessage.BodyFormat = System.Web.Mail.MailFormat.Text;
						mailMessage.Body = Body.Text;

						System.Web.Mail.SmtpMail.SmtpServer = SmtpServer;
						System.Web.Mail.SmtpMail.Send(mailMessage);
					}
				}
			}
			Subject.Text = "";
			Body.Text = "";
			AddLoadMessage("Mails sent.");
		}
	}
}
