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
	/// Summary description for _default.
	/// </summary>
	public class _default : BasePage
	{
		protected System.Web.UI.WebControls.Label TimeNow;
		protected System.Web.UI.WebControls.Label Stats;
		protected System.Web.UI.WebControls.Repeater CategoryList;
		protected System.Web.UI.WebControls.Label TimeLastVisit;
		protected System.Web.UI.WebControls.Repeater ForumList;
		protected System.Web.UI.WebControls.Repeater ActiveList;
		protected System.Web.UI.WebControls.HyperLink UnreadMsgs;
		protected System.Web.UI.HtmlControls.HtmlGenericControl Welcome;
		protected System.Web.UI.WebControls.Label activeinfo;
		protected controls.PageLinks PageLinks;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				TimeNow.Text = String.Format(GetText("current_time"),FormatTime(DateTime.Now));
				TimeLastVisit.Text = String.Format(GetText("last_visit"),FormatDateTime(DateTime.Parse(Session["lastvisit"].ToString())));

				if(UnreadPrivate>0) 
				{
					UnreadMsgs.Visible = true;
					UnreadMsgs.NavigateUrl = "cp_inbox.aspx";
					if(UnreadPrivate==1)
						UnreadMsgs.Text = String.Format(GetText("unread1"),UnreadPrivate);
					else
						UnreadMsgs.Text = String.Format(GetText("unread0"),UnreadPrivate);
				}

				PageLinks.AddLink(ForumName,BaseDir);
				if(PageCategoryID!=0) 
				{
					PageLinks.AddLink(PageCategoryName,String.Format("{0}?c={1}",BaseDir,PageCategoryID));
					Welcome.Visible = false;
				}

				DataSet ds = DB.ds_forumlayout(PageUserID,PageCategoryID);
				CategoryList.DataSource = ds.Tables["yaf_Category"];

				// Active users
				// Call this before forum_stats to clean up active users
				ActiveList.DataSource = DB.active_list(null);

				// Forum statistics
				DataRow stats = (DataRow)Cache["ForumStats"];
				if(stats==null) 
				{
					stats = DB.stats();
					Cache["ForumStats"] = stats;
				}
				
				Stats.Text = String.Format(GetText("stats_posts"),stats["posts"],stats["topics"],stats["forums"]);
				Stats.Text += "<br/>";
				
				if(!stats.IsNull("LastPost")) 
				{
					Stats.Text += String.Format(GetText("stats_lastpost"),
						FormatDateTime((DateTime)stats["LastPost"]),
						String.Format("<a href=\"profile.aspx?u={0}\">{1}</a>",stats["LastUserID"],stats["LastUser"])
					);
					Stats.Text += "<br/>";
				}
				
				Stats.Text += String.Format(GetText("stats_members"),stats["members"]);
				Stats.Text += "<br/>";

				Stats.Text += String.Format(GetText("stats_lastmember"),
					String.Format("<a href=\"profile.aspx?u={0}\">{1}</a>",stats["LastMemberID"],stats["LastMember"])
				);
				Stats.Text += "<br/>";

				DataRow activeStats = DB.active_stats();
				activeinfo.Text = String.Format("<a href=\"activeusers.aspx\">{0}</a> - {1}, {2}.",
					String.Format(GetText((int)activeStats["ActiveUsers"]==1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2"),activeStats["ActiveUsers"]),
					String.Format(GetText((int)activeStats["ActiveMembers"]==1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2"),activeStats["ActiveMembers"]),
					String.Format(GetText((int)activeStats["ActiveGuests"]==1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2"),activeStats["ActiveGuests"])
				);

				DataBind();
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		protected string FormatLastPost(System.Data.DataRow row) {
			if(!row.IsNull("LastPosted")) {
				string minipost;
				if(DateTime.Parse(row["LastPosted"].ToString()) > (DateTime)Session["lastvisit"])
					minipost = GetThemeContents("ICONS","ICON_NEWEST");
				else
					minipost = GetThemeContents("ICONS","ICON_LATEST");
				
				return String.Format("{0}<br/>{1}<br/>{2}&nbsp;<a href=\"posts.aspx?m={4}#{4}\"><img src='{3}'></a>",
					FormatDateTime((DateTime)row["LastPosted"]),
					String.Format(GetText("in"),String.Format("<a href=\"posts.aspx?t={0}\">{1}</a>",row["LastTopicID"],row["LastTopicName"])),
					String.Format(GetText("by"),String.Format("<a href=\"profile.aspx?u={0}\">{1}</a>",row["LastUserID"],row["LastUser"])),
					minipost,
					row["LastMessageID"]
				);
			}
			else
				return GetText("no_posts");
		}

		protected string GetViewing(object o) 
		{
			DataRow row = (DataRow)o;
			int nViewing = (int)row["Viewing"];
			if(nViewing>0)
				return "&nbsp;" + String.Format(GetText("VIEWING"),nViewing);
			else
				return "";
		}

		protected string GetForumIcon(object lastPosted,object Locked) 
		{
			try 
			{
				if((bool)Locked)
					return GetThemeContents("ICONS","FORUM_LOCKED");

				if(DateTime.Parse(lastPosted.ToString()) > (DateTime)Session["lastvisit"])
					return GetThemeContents("ICONS","FORUM_NEW");
				else
					return GetThemeContents("ICONS","FORUM");
			}
			catch(Exception) 
			{
				return GetThemeContents("ICONS","FORUM");
			}
		}
		protected void ForumList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			switch(e.CommandName) {
				case "forum":
					if(DB.user_access(PageUserID,e.CommandArgument))
						Response.Redirect(String.Format("topics.aspx?f={0}",e.CommandArgument));

					AddLoadMessage("You can't access that forum.");
					break;
			}
		}

		protected void ModeratorList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			//AddLoadMessage("TODO: Fix this");
			//TODO: Show moderators
		}
	}
}
