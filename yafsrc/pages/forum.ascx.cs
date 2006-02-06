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
	/// Summary description for _default.
	/// </summary>
	public class forum : ForumPage
	{
		protected System.Web.UI.WebControls.Label TimeNow;
		protected System.Web.UI.WebControls.Label Stats;
		protected System.Web.UI.WebControls.Repeater CategoryList;
		protected System.Web.UI.WebControls.Label TimeLastVisit;
		protected System.Web.UI.WebControls.Repeater ActiveList, LatestPosts;
		protected System.Web.UI.WebControls.HyperLink UnreadMsgs;
		protected System.Web.UI.HtmlControls.HtmlGenericControl Welcome;
		protected System.Web.UI.WebControls.Label activeinfo;
		protected LinkButton MarkAll;
		protected controls.PageLinks PageLinks;
	
		public forum() : base("DEFAULT")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				if (IsPrivate && (User==null || (User!=null && !User.IsAuthenticated)))
				{
					Forum.Redirect(Pages.login,"ReturnUrl={0}",Request.RawUrl);
				}

				TimeNow.Text = String.Format(GetText("CURRENT_TIME"),FormatTime(DateTime.Now));
				TimeLastVisit.Text = String.Format(GetText("last_visit"),FormatDateTime(Mession.LastVisit));
				MarkAll.Text = GetText("MARKALL");

				if(UnreadPrivate>0) 
				{
					UnreadMsgs.Visible = true;
					UnreadMsgs.NavigateUrl = Forum.GetLink(Pages.cp_inbox);
					if(UnreadPrivate==1)
						UnreadMsgs.Text = String.Format(GetText("unread1"),UnreadPrivate);
					else
						UnreadMsgs.Text = String.Format(GetText("unread0"),UnreadPrivate);
				}

				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				if(PageCategoryID!=0) 
				{
					PageLinks.AddLink(PageCategoryName,Forum.GetLink(Pages.forum,"c={0}",PageCategoryID));
					Welcome.Visible = false;
				}

				BindData();
			}
		}

		private void MarkAll_Click(object sender, System.EventArgs e)
		{
			Mession.LastVisit = DateTime.Now;
			BindData();
		}

		private void BindData() 
		{
			DataSet ds = DB.board_layout(PageBoardID,PageUserID,PageCategoryID,null);
			CategoryList.DataSource = ds.Tables["yaf_Category"];

			// Active users
			// Call this before forum_stats to clean up active users
			ActiveList.DataSource = DB.active_list(PageBoardID,null);

			// Latest forum posts
			// Shows the latest n number of posts on the main forum list page
			LatestPosts.DataSource = DB.topic_latest(PageBoardID,7,PageUserID);

			// Forum statistics
			string key = string.Format("BoardStats.{0}",PageBoardID);
			DataRow stats = (DataRow)Cache[key];
			if(stats==null) 
			{
				stats = DB.board_poststats(PageBoardID);
				Cache.Insert(key,stats,null,DateTime.Now.AddMinutes(15),TimeSpan.Zero);
			}
				
			Stats.Text = String.Format(GetText("stats_posts"),stats["posts"],stats["topics"],stats["forums"]);
			Stats.Text += "<br/>";
				
			if(!stats.IsNull("LastPost")) 
			{
				Stats.Text += String.Format(GetText("stats_lastpost"),
					FormatDateTimeTopic((DateTime)stats["LastPost"]),
					String.Format("<a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.profile,"u={0}",stats["LastUserID"]),Server.HtmlEncode(stats["LastUser"].ToString()))
				);
				Stats.Text += "<br/>";
			}
				
			Stats.Text += String.Format(GetText("stats_members"),stats["members"]);
			Stats.Text += "<br/>";

			Stats.Text += String.Format(GetText("stats_lastmember"),
				String.Format("<a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.profile,"u={0}",stats["LastMemberID"]),Server.HtmlEncode(stats["LastMember"].ToString()))
				);
			Stats.Text += "<br/>";

			DataRow activeStats = DB.active_stats(PageBoardID);
			activeinfo.Text = String.Format("<a href=\"{3}\">{0}</a> - {1}, {2}.",
				String.Format(GetText((int)activeStats["ActiveUsers"]==1 ? "ACTIVE_USERS_COUNT1" : "ACTIVE_USERS_COUNT2"),activeStats["ActiveUsers"]),
				String.Format(GetText((int)activeStats["ActiveMembers"]==1 ? "ACTIVE_USERS_MEMBERS1" : "ACTIVE_USERS_MEMBERS2"),activeStats["ActiveMembers"]),
				String.Format(GetText((int)activeStats["ActiveGuests"]==1 ? "ACTIVE_USERS_GUESTS1" : "ACTIVE_USERS_GUESTS2"),activeStats["ActiveGuests"]),
				Forum.GetLink(Pages.activeusers)
				);

			activeinfo.Text += "<br/>" + string.Format(GetText("MAX_ONLINE"),BoardSettings.MaxUsers,FormatDateTimeTopic(BoardSettings.MaxUsersWhen));

			DataBind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			MarkAll.Click += new EventHandler(MarkAll_Click);
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
				if(DateTime.Parse(row["LastPosted"].ToString()) > Mession.LastVisit)
					minipost = GetThemeContents("ICONS","ICON_NEWEST");
				else
					minipost = GetThemeContents("ICONS","ICON_LATEST");
				
				return String.Format("{0}<br/>{1}<br/>{2}&nbsp;<a title=\"{4}\" href=\"{5}\"><img src='{3}'></a>",
					FormatDateTimeTopic(Convert.ToDateTime(row["LastPosted"])),
					String.Format(GetText("in"),String.Format("<a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.posts,"t={0}",row["LastTopicID"]),row["LastTopicName"])),
					String.Format(GetText("by"),String.Format("<a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.profile,"u={0}",row["LastUserID"]),row["LastUser"])),
					minipost,
					GetText("GO_LAST_POST"),
					Forum.GetLink(Pages.posts,"m={0}#{0}",row["LastMessageID"])					
					);
			}
			else
				return GetText("NO_POSTS");
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

		protected string GetForumIcon(object o) 
		{
			DataRow		row			= (DataRow)o;
			bool		locked		= (bool)row["Locked"];
			DateTime	lastRead	= GetForumRead((int)row["ForumID"]);
			DateTime	lastPosted	= row["LastPosted"]!=DBNull.Value ? (DateTime)row["LastPosted"] : lastRead;

			string		img, imgTitle;
			
			try 
			{
				if(locked) 
				{
					img = GetThemeContents("ICONS","FORUM_LOCKED");
					imgTitle = GetText("ICONLEGEND","Forum_Locked");
					} 
				else if(lastPosted > lastRead)
				{
					img = GetThemeContents("ICONS","FORUM_NEW");
					imgTitle = GetText("ICONLEGEND","New_Posts");
				}
				else
				{
					img = GetThemeContents("ICONS","FORUM");
					imgTitle = GetText("ICONLEGEND","No_New_Posts");
				}
			}
			catch(Exception) 
			{
				img = GetThemeContents("ICONS","FORUM");
				imgTitle = GetText("ICONLEGEND","No_New_Posts");
			}

			return String.Format("<img src=\"{0}\" title=\"{1}\"/>",img,imgTitle);
		}

		protected void ModeratorList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			//AddLoadMessage("TODO: Fix this");
			//TODO: Show moderators
		}
	}
}
