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

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for members.
	/// </summary>
	public partial class boards : AdminPage
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsHostAdmin)
				Data.AccessDenied();

			if(!IsPostBack) 
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink( ForumPages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink( ForumPages.admin_admin));
				PageLinks.AddLink("Boards","");

				BindData();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			this.List.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.List_ItemCommand);
			this.New.Click += new EventHandler(New_Click);
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			base.OnInit(e);
		}
		#endregion
	
		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this board?')";
		}

		private void BindData() 
		{
			List.DataSource = YAF.Classes.Data.DB.board_list(null);
			DataBind();
		}

		private void New_Click(object sender,EventArgs e) 
		{
			Forum.Redirect( ForumPages.admin_editboard);
		}

		private void List_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			switch(e.CommandName) {
				case "edit":
					Forum.Redirect( ForumPages.admin_editboard,"b={0}",e.CommandArgument);
					break;
				case "delete":
					YAF.Classes.Data.DB.board_delete(e.CommandArgument);
					BindData();
					break;
			}
		}
	}
}
