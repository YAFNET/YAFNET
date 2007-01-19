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
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for mail.
	/// </summary>
	public partial class mail : YAF.Classes.Base.AdminPage
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(PageContext.BoardSettings.Name,YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
				PageLinks.AddLink("Administration",YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin));
				PageLinks.AddLink("Mail","");

				BindData();
			}
		}

		private void BindData() {
			ToList.DataSource = YAF.Classes.Data.DB.group_list(PageContext.PageBoardID,null);
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

		}
		#endregion

		protected void Send_Click(object sender, System.EventArgs e) {
			object GroupID = null;
			if(ToList.SelectedItem.Value!="0")
				GroupID = ToList.SelectedValue;

			using(DataTable dt = YAF.Classes.Data.DB.user_emails(PageContext.PageBoardID,GroupID)) 
			{
				foreach(DataRow row in dt.Rows)
					//  Build a MailMessage
					General.SendMail(PageContext.BoardSettings.ForumEmail,(string)row["Email"],Subject.Text,Body.Text);
			}
			Subject.Text = "";
			Body.Text = "";
			PageContext.AddLoadMessage("Mails sent.");
		}
	}
}
