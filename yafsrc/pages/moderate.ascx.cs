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

namespace yaf.pages
{
	/// <summary>
	/// Summary description for moderate.
	/// </summary>
	public class moderate0 : ForumPage
	{
		protected Repeater topiclist, UserList;
		protected controls.PageLinks PageLinks;
		protected LinkButton AddUser;

		public moderate0() : base("MODERATE")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!ForumModeratorAccess)
				Data.AccessDenied();

			if(!IsPostBack) 
			{
				AddUser.Text = GetText("INVITE");

				if(ForumControl.LockedForum==0)
				{
					PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
					PageLinks.AddLink(PageCategoryName,Forum.GetLink(Pages.forum,"c={0}",PageCategoryID));
				}
				PageLinks.AddForumLinks(PageForumID);
				PageLinks.AddLink(GetText("TITLE"),"");
			}
			BindData();
		}

		private void AddUser_Click(object sender, System.EventArgs e)
		{
			Forum.Redirect(Pages.mod_forumuser,"f={0}",PageForumID);
		}

		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')",GetText("confirm_delete"));
		}

		protected void DeleteUser_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')","Remove this user from this forum?");
		}

		private void BindData() 
		{
			topiclist.DataSource = DB.topic_list(PageForumID,-1,null,0,999999);
			UserList.DataSource = DB.userforum_list(null,PageForumID);
			DataBind();
		}

		private void topiclist_ItemCommand(object sender,RepeaterCommandEventArgs e) {
			if(e.CommandName=="delete") {
				DB.topic_delete(e.CommandArgument);
				AddLoadMessage(GetText("deleted"));
				BindData();
			}
		}

		private void UserList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "edit":
					Forum.Redirect(Pages.mod_forumuser,"f={0}&u={1}",PageForumID,e.CommandArgument);
					break;
				case "remove":
					DB.userforum_delete(e.CommandArgument,PageForumID);
					BindData();
					break;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			topiclist.ItemCommand += new RepeaterCommandEventHandler(topiclist_ItemCommand);
			UserList.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.UserList_ItemCommand);
			AddUser.Click += new EventHandler(AddUser_Click);
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
