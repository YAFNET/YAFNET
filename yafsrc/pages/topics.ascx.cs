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
	/// Summary description for topics.
	/// </summary>
	public class topics : ForumPage
	{
		protected System.Web.UI.WebControls.DropDownList ShowList;
		protected System.Web.UI.WebControls.Repeater TopicList;
		protected System.Web.UI.WebControls.Repeater Announcements;
		protected System.Web.UI.WebControls.LinkButton NewTopic2;
		protected System.Web.UI.WebControls.LinkButton NewTopic1;
		protected System.Web.UI.WebControls.Label PageTitle;
		protected System.Web.UI.WebControls.DropDownList ForumJump;
		protected System.Web.UI.WebControls.DropDownList DropDownList1;
		private DataRow forum;
		protected System.Web.UI.WebControls.LinkButton WatchForum;
		protected LinkButton moderate1, moderate2, MarkRead;
		protected controls.PageLinks PageLinks;
		protected controls.Pager Pager;

		public topics() : base("TOPICS")
		{
		}

		private void topics_Unload(object sender, System.EventArgs e)
		{
			if(Mession.UnreadTopics==0) 
				SetForumRead(PageForumID,DateTime.Now);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Mession.UnreadTopics = 0;
			if(!IsPostBack) 
			{
				PageLinks.AddLink(Config.ForumSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(PageCategoryName,Forum.GetLink(Pages.forum,"c={0}",PageCategoryID));
				PageLinks.AddLink(PageForumName,Forum.GetLink(Pages.topics,"f={0}",PageForumID));

				WatchForum.Text = GetText("watchforum");
				moderate1.Text = GetThemeContents("BUTTONS","MODERATE");
				moderate1.ToolTip = "Moderate this forum";
				moderate2.Text = moderate1.Text;
				moderate2.ToolTip = moderate1.ToolTip;
				MarkRead.Text = GetText("MARKREAD");

				NewTopic1.Text = GetThemeContents("BUTTONS","NEWTOPIC");
				NewTopic1.ToolTip = "Post new topic";
				NewTopic2.Text = NewTopic1.Text;
				NewTopic2.ToolTip = NewTopic1.ToolTip;

				ShowList.Items.Add(new ListItem(GetText("all"),"0"));
				ShowList.Items.Add(new ListItem(GetText("last_day"),"1"));
				ShowList.Items.Add(new ListItem(GetText("last_two_days"),"2"));
				ShowList.Items.Add(new ListItem(GetText("last_week"),"3"));
				ShowList.Items.Add(new ListItem(GetText("last_two_weeks"),"4"));
				ShowList.Items.Add(new ListItem(GetText("last_month"),"5"));
				ShowList.Items.Add(new ListItem(GetText("last_two_months"),"6"));
				ShowList.Items.Add(new ListItem(GetText("last_six_months"),"7"));
				ShowList.Items.Add(new ListItem(GetText("last_year"),"8"));

				try 
				{
					ShowList.SelectedIndex = Mession.ShowList;
				}
				catch(Exception) 
				{
					ShowList.SelectedIndex = 8;
				}
			}

			if(Request.QueryString["f"] == null)
				Data.AccessDenied();

			if(!ForumReadAccess)
				Data.AccessDenied();

			using(DataTable dt = DB.forum_list(PageForumID))
				forum = dt.Rows[0];

			PageTitle.Text = (string)forum["Name"];

			BindData();	// Always because of yaf:TopicLine

			if(!ForumPostAccess) {
				NewTopic1.Visible = false;
				NewTopic2.Visible = false;
			}

			if(!ForumModeratorAccess) {
				moderate1.Visible = false;
				moderate2.Visible = false;
			}
		}

		private void MarkRead_Click(object sender, System.EventArgs e) 
		{
			SetForumRead(PageForumID,DateTime.Now);
			BindData();
		}

		private void Pager_PageChange(object sender, EventArgs e)
		{
			BindData();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			this.Unload += new EventHandler(topics_Unload);
			moderate1.Click += new EventHandler(moderate_Click);
			moderate2.Click += new EventHandler(moderate_Click);
			ShowList.SelectedIndexChanged += new EventHandler(ShowList_SelectedIndexChanged);
			MarkRead.Click += new EventHandler(MarkRead_Click);
			Pager.PageChange += new EventHandler(Pager_PageChange);
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
			this.NewTopic1.Click += new System.EventHandler(this.NewTopic_Click);
			this.NewTopic2.Click += new System.EventHandler(this.NewTopic_Click);
			this.WatchForum.Click += new System.EventHandler(this.WatchForum_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void moderate_Click(object sender,EventArgs e) {
			if(ForumModeratorAccess)
				Forum.Redirect(Pages.moderate,"f={0}",PageForumID);
		}

		private void ShowList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Mession.ShowList = ShowList.SelectedIndex;
			BindData();
		}

		private void BindData() 
		{
			Pager.PageSize = 15;

			DataTable dt = DB.topic_list(PageForumID,1,null,0,10);
			int nPageSize = System.Math.Max(5,Pager.PageSize - dt.Rows.Count);
			Announcements.DataSource = dt;

			int nCurrentPageIndex = Pager.CurrentPageIndex;

			DataTable dtTopics;
			if(ShowList.SelectedIndex==0) 
			{
				dtTopics = DB.topic_list(PageForumID,0,null,nCurrentPageIndex*nPageSize,nPageSize);
			} 
			else 
			{
				DateTime date = DateTime.Now;
				switch(ShowList.SelectedIndex) 
				{
					case 1:
						date -= TimeSpan.FromDays(1);
						break;
					case 2:
						date -= TimeSpan.FromDays(2);
						break;
					case 3:
						date -= TimeSpan.FromDays(7);
						break;
					case 4:
						date -= TimeSpan.FromDays(14);
						break;
					case 5:
						date -= TimeSpan.FromDays(31);
						break;
					case 6:
						date -= TimeSpan.FromDays(2*31);
						break;
					case 7:
						date -= TimeSpan.FromDays(6*31);
						break;
					case 8:
						date -= TimeSpan.FromDays(365);
						break;
				}
				dtTopics = DB.topic_list(PageForumID,0,date,nCurrentPageIndex*nPageSize,nPageSize);
			}
			int nRowCount = 0;
			if(dtTopics.Rows.Count>0) nRowCount = (int)dtTopics.Rows[0]["RowCount"];
			int nPageCount = (nRowCount + nPageSize - 1) / nPageSize;

			TopicList.DataSource = dtTopics;

			DataBind();

			Pager.Count = nRowCount;
		}

		private void NewTopic_Click(object sender, System.EventArgs e) {
			if((bool)forum["Locked"]) {
				AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
				return;
			}

			int ForumID = int.Parse(Request.QueryString["f"]);
			if(!ForumPostAccess)
				Data.AccessDenied(/*"You don't have access to post new topics in this forum."*/);
			
			Forum.Redirect(Pages.postmessage,"f={0}",ForumID);
		}

		private void WatchForum_Click(object sender, System.EventArgs e) {
			if(!ForumReadAccess)
				return;

			if(IsGuest) {
				AddLoadMessage(GetText("WARN_LOGIN_FORUMWATCH"));
				return;
			}

			DB.watchforum_add(PageUserID,PageForumID);
			AddLoadMessage(GetText("INFO_WATCH_FORUM"));
		}
	}
}
