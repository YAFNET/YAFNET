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

namespace yaf
{
	/// <summary>
	/// Summary description for movetopic.
	/// </summary>
	public class movetopic : BasePage
	{
		protected System.Web.UI.WebControls.Button Move;
		protected System.Web.UI.WebControls.DropDownList ForumList;
		protected controls.PageLinks PageLinks;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.QueryString["t"] == null || !ForumModeratorAccess)
				Data.AccessDenied();

			if(!IsPostBack) {
				PageLinks.AddLink(ForumName,BaseDir);
				PageLinks.AddLink(PageCategoryName,String.Format("{0}?c={1}",BaseDir,PageCategoryID));
				PageLinks.AddLink(PageForumName,String.Format("topics.aspx?f={0}",PageForumID));
				PageLinks.AddLink(PageTopicName,String.Format("posts.aspx?t={0}",PageTopicID));

				Move.Text = GetText("move");

				ForumList.DataSource = DB.forum_listread(PageUserID,null);
				DataBind();
				ForumList.Items.FindByValue(PageForumID.ToString()).Selected = true;
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

		private void Move_Click(object sender, System.EventArgs e) {
			DB.topic_move(PageTopicID,ForumList.SelectedValue,ShowMovedTopics);
			Response.Redirect(String.Format("topics.aspx?f={0}",PageForumID));
		}
	}
}
