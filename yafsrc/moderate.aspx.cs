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
	/// Summary description for moderate.
	/// </summary>
	public class moderate0 : BasePage
	{
		protected HyperLink HomeLink, CategoryLink, ForumLink, ModLink;
		protected Repeater topiclist;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!ForumModeratorAccess)
				Data.AccessDenied();

			HomeLink.NavigateUrl = BaseDir;
			HomeLink.Text = ForumName;
			CategoryLink.NavigateUrl = String.Format("default.aspx?c={0}",PageCategoryID);
			CategoryLink.Text = PageCategoryName;
			ForumLink.NavigateUrl = String.Format("topics.aspx?f={0}",PageForumID);
			ForumLink.Text = PageForumName;
			ModLink.NavigateUrl = Request.RawUrl;
			ModLink.Text = GetText("title");

			if(!IsPostBack)
				BindData();
		}

		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')",GetText("confirm_delete"));
		}

		private void BindData() 
		{
			topiclist.DataSource = DB.topic_list(PageForumID,PageUserID,-1,null);
			DataBind();
		}

		private void topiclist_ItemCommand(object sender,RepeaterCommandEventArgs e) {
			if(e.CommandName=="delete") {
				DB.topic_delete(e.CommandArgument);
				AddLoadMessage(GetText("deleted"));
				BindData();
			}
		}

		#region Code copied from topics.aspx
		// Copied from topics.aspx
		protected string GetTopicImage(object o) {
			DataRowView row = (DataRowView)o;
			object lastPosted = row["LastPosted"];
			object isLocked = row["IsLocked"];
			try {
				bool bIsLocked = (bool)isLocked /*|| (bool)forum["Locked"]*/;

				if(row["TopicMovedID"].ToString().Length>0)
					return ThemeFile("topic_moved.png");

				if(row["Priority"].ToString() == "1")
					return ThemeFile("topic_sticky.png");

				if(row["Priority"].ToString() == "2")
					return ThemeFile("topic_announce.png");

				if(DateTime.Parse(lastPosted.ToString()) > (DateTime)Session["lastvisit"]) {
					if(bIsLocked)
						return ThemeFile("topic_lock_new.png");
					else
						return ThemeFile("topic_new.png");
				}
				else {
					if(bIsLocked)
						return ThemeFile("topic_lock.png");
					else
						return ThemeFile("topic.png");
				}
			}
			catch(Exception) {
				return ThemeFile("topic.png");
			}
		}
		protected string GetPriorityMessage(DataRowView row) {
			if(row["TopicMovedID"].ToString().Length>0)
				return "[ Moved ] ";

			if(row["PollID"].ToString()!="")
				return "[ Poll ] ";

			switch(int.Parse(row["Priority"].ToString())) {
				case 1:
					return "[ Sticky ] ";
				case 2:
					return "[ Announcement ] ";
				default:
					return "";
			}
		}
		protected string FormatLastPost(System.Data.DataRowView row) 
		{
			if(row["LastPosted"].ToString().Length>0) 
			{
				string minipost;
				if(DateTime.Parse(row["LastPosted"].ToString()) > (DateTime)Session["lastvisit"])
					minipost = ThemeFile("icon_newest_reply.gif");
				else
					minipost = ThemeFile("icon_latest_reply.gif");
				
				string by = String.Format(GetText("by"),String.Format("<a href=\"profile.aspx?u={0}\">{1}</a>&nbsp;<a href=\"posts.aspx?m={3}#{3}\"><img border=0 src='{2}'></a>",
					row["LastUserID"], 
					row["LastUserName"], 
					minipost, 
					row["LastMessageID"]
					));
				return String.Format(CustomCulture,"{0}<br />{1}", 
					FormatDateTime((DateTime)row["LastPosted"]),
					by
					);
			} 
			else
				return GetText("no_posts");
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			topiclist.ItemCommand += new RepeaterCommandEventHandler(topiclist_ItemCommand);
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
