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
	/// Summary description for members.
	/// </summary>
	public class users : AdminPage
	{
		protected System.Web.UI.WebControls.Repeater UserList;
		protected controls.PageLinks PageLinks;
		protected LinkButton NewUser;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				PageLinks.AddLink(Config.BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("Users",Forum.GetLink(Pages.admin_users));

				BindData();
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
			this.UserList.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.UserList_ItemCommand);
			this.Load += new System.EventHandler(this.Page_Load);
			// Added BAI 07.01.2003
			this.NewUser.Click += new System.EventHandler(this.NewUser_Click);
			// END Added BAI 07.01.2003    
		}
		#endregion
	
		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this user?')";
		}

		private void BindData() 
		{
			UserList.DataSource = DB.user_list(PageBoardID,null,null);
			DataBind();
		}

		private void UserList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			switch(e.CommandName) {
				case "edit":
					Forum.Redirect(Pages.admin_edituser,"u={0}",e.CommandArgument);
					break;
				case "delete":
					if(PageUserID==int.Parse(e.CommandArgument.ToString()))
					{
						AddLoadMessage("You can't delete yourself.");
						return;
					}
					DB.user_delete(e.CommandArgument);
					BindData();
					break;
			}
		}
		// Added BAI 07.01.2003
		private void NewUser_Click(object sender, System.EventArgs e)
		{
			Forum.Redirect(Pages.admin_reguser);
		}
		// END Added BAI 07.01.2003		
	}
}
