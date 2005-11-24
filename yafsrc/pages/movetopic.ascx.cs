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
	/// Summary description for movetopic.
	/// </summary>
	public class movetopic : ForumPage
	{
		protected System.Web.UI.WebControls.Button Move;
		protected System.Web.UI.WebControls.DropDownList ForumList;
		protected controls.PageLinks PageLinks;
	
		public movetopic() : base("MOVETOPIC")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.QueryString["t"] == null || !ForumModeratorAccess)
				Data.AccessDenied();

			if(!IsPostBack)
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(PageCategoryName,Forum.GetLink(Pages.forum,"c={0}",PageCategoryID));
				PageLinks.AddForumLinks(PageForumID);
				PageLinks.AddLink(PageTopicName,Forum.GetLink(Pages.posts,"t={0}",PageTopicID));

				Move.Text = GetText("move");

				ForumList.DataSource = DB.forum_listall(PageBoardID,PageUserID);
				
				DataBind();
				
				System.Web.UI.WebControls.ListItem pageItem = ForumList.Items.FindByValue(PageForumID.ToString());
				if (pageItem != null) pageItem.Selected = true;
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
			this.Move.Click += new System.EventHandler(this.Move_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Move_Click(object sender, System.EventArgs e)
		{
			// only move if it's a destination is a different forum.
			if (Convert.ToInt32(ForumList.SelectedValue) != PageForumID)
			{
				DB.topic_move(PageTopicID,ForumList.SelectedValue,BoardSettings.ShowMoved);
			}
			Forum.Redirect(Pages.topics,"f={0}",PageForumID);
		}
	}
}
