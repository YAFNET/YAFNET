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
	/// Summary description for mail.
	/// </summary>
	public class mail : AdminPage
	{
		protected System.Web.UI.WebControls.TextBox Subject;
		protected System.Web.UI.WebControls.DropDownList ToList;
		protected System.Web.UI.WebControls.Button Send;
		protected System.Web.UI.WebControls.TextBox Body;
		protected controls.PageLinks PageLinks;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(Config.BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("Mail",Forum.GetLink(Pages.admin_mail));

				BindData();
			}
		}

		private void BindData() {
			ToList.DataSource = DataProvider.group_list(PageBoardID,null);
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
			object GroupID = null;
			if(ToList.SelectedItem.Value!="0")
				GroupID = ToList.SelectedValue;

			using(DataTable dt = DataProvider.user_emails(PageBoardID,GroupID)) 
			{
				foreach(DataRow row in dt.Rows)
					//  Build a MailMessage
					Utils.SendMail(Config.BoardSettings.ForumEmail,(string)row["Email"],Subject.Text,Body.Text);
			}
			Subject.Text = "";
			Body.Text = "";
			AddLoadMessage("Mails sent.");
		}
	}
}
