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
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink HomeLink2;
		protected System.Web.UI.HtmlControls.HtmlGenericControl NavLinks;
		protected System.Web.UI.HtmlControls.HtmlGenericControl NavLinks2;
		protected System.Web.UI.WebControls.HyperLink CategoryLink;
		protected System.Web.UI.HtmlControls.HtmlGenericControl Welcome;
		protected System.Web.UI.WebControls.Label activeinfo;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				TimeNow.Text = String.Format(GetText("default_current_time"),FormatTime(DateTime.Now));
				TimeLastVisit.Text = String.Format(GetText("default_last_visit"),FormatDateTime(DateTime.Parse(Session["lastvisit"].ToString())));

				if(PageCategoryID!=0) {
					HomeLink.NavigateUrl = BaseDir;
					HomeLink.Text = ForumName;
					CategoryLink.NavigateUrl = String.Format("default.aspx?c={0}",PageCategoryID);
					CategoryLink.Text = PageCategoryName;
					NavLinks2.Visible = false;
					Welcome.Visible = false;
				} else {
					HomeLink2.NavigateUrl = BaseDir;
					HomeLink2.Text = ForumName;
					NavLinks.Visible = false;
				}

				DataSet ds = DB.ds_forumlayout(PageUserID,PageCategoryID);
				CategoryList.DataSource = ds.Tables["yaf_Category"];

				// Active users
				// Call this before forum_stats to clean up active users
				ActiveList.DataSource = DB.active_list(null);

				// Forum statistics
				DataRow stats = DB.stats();
				
				Stats.Text = String.Format(CustomCulture,GetText("default_stats_posts"),stats["posts"],stats["topics"],stats["forums"]);
				Stats.Text += "<br/>";
				
				if(!stats.IsNull("LastPost")) 
				{
					Stats.Text += String.Format(GetText("default_stats_lastpost"),
						FormatDateTime((DateTime)stats["LastPost"]),
						String.Format("<a href=\"profile.aspx?u={0}\">{1}</a>",stats["LastUserID"],stats["LastUser"])
					);
					Stats.Text += "<br/>";
				}
				
				Stats.Text += String.Format(GetText("default_stats_members"),stats["members"]);
				Stats.Text += "<br/>";

				Stats.Text += String.Format(GetText("default_stats_lastmember"),
					String.Format("<a href=\"profile.aspx?u={0}\">{1}</a>",stats["LastMemberID"],stats["LastMember"])
				);
				Stats.Text += "<br/>";

				activeinfo.Text = String.Format(CustomCulture,"{0:N0} <a href=\"activeusers.aspx\">active users</a> - {1:N0} members and {2:N0} guests.",stats["ActiveUsers"],stats["ActiveMembers"],stats["ActiveGuests"]);

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
					minipost = ThemeFile("icon_newest_reply.gif");
				else
					minipost = ThemeFile("icon_latest_reply.gif");
				
				return String.Format("{0}<br/>{1}<br/>{2}&nbsp;<a href=\"posts.aspx?m={4}#{4}\"><img src='{3}'></a>",
					FormatDateTime((DateTime)row["LastPosted"]),
					String.Format(GetText("default_in"),String.Format("<a href=\"posts.aspx?t={0}\">{1}</a>",row["LastTopicID"],row["LastTopicName"])),
					String.Format(GetText("default_by"),String.Format("<a href=\"profile.aspx?u={0}\">{1}</a>",row["LastUserID"],row["LastUser"])),
					minipost,
					row["LastMessageID"]
				);
			}
			else
				return GetText("default_no_posts");
		}

		protected string GetForumIcon(object lastPosted,object Locked,object oPostAccess,object oReplyAccess,object oReadAccess) 
		{
			try 
			{
				if((bool)Locked)
					return ThemeFile("topic_lock.png");

				if(DateTime.Parse(lastPosted.ToString()) > (DateTime)Session["lastvisit"])
					return ThemeFile("topic_new.png");
				else
					return ThemeFile("topic.png");
			}
			catch(Exception) 
			{
				return ThemeFile("topic.png");
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
