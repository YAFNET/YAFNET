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
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf
{
	/// <summary>
	/// Summary description for topics.
	/// </summary>
	public class topics : BasePage
	{
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.DropDownList ShowList;
		protected System.Web.UI.WebControls.Repeater TopicList;
		protected System.Web.UI.WebControls.Repeater Announcements;
		protected System.Web.UI.WebControls.LinkButton NewTopic2;
		protected System.Web.UI.WebControls.LinkButton NewTopic1;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink CategoryLink;
		protected System.Web.UI.WebControls.Label PageTitle;
		protected System.Web.UI.WebControls.DropDownList ForumJump;
		protected System.Web.UI.WebControls.DropDownList DropDownList1;
		protected System.Web.UI.WebControls.HyperLink ForumLink;
		private DataRow forum;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks1;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks2;
		protected System.Web.UI.WebControls.LinkButton WatchForum;
		protected System.Web.UI.HtmlControls.HtmlTableCell AccessCell;
		protected LinkButton moderate1, moderate2;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.QueryString["f"] == null)
				Response.Redirect(BaseDir);

			if(!ForumReadAccess)
				Response.Redirect(BaseDir);

			forum = Data.ForumInfo(PageForumID);

			PageTitle.Text = (string)forum["Name"];
			HomeLink.NavigateUrl = BaseDir;
			HomeLink.Text = ForumName;
			CategoryLink.NavigateUrl = String.Format("default.aspx?c={0}",PageCategoryID);
			CategoryLink.Text = PageCategoryName;
			ForumLink.NavigateUrl = String.Format("topics.aspx?f={0}",PageForumID);
			ForumLink.Text = PageForumName;

			if(!IsPostBack) 
				BindData();

			if(!ForumPostAccess) {
				NewTopic1.Visible = false;
				NewTopic2.Visible = false;
			}

			if(!ForumModeratorAccess) {
				moderate1.Visible = false;
				moderate2.Visible = false;
			}

			System.Text.StringBuilder tmp = new System.Text.StringBuilder();
			tmp.AppendFormat("You <b>{0}</b> post new topics in this forum.<br>",ForumPostAccess?"can":"cannot");
			tmp.AppendFormat("You <b>{0}</b> reply to topics in this forum.<br>",ForumReplyAccess?"can":"cannot");
			tmp.AppendFormat("You <b>{0}</b> delete your posts in this forum.<br>",ForumDeleteAccess?"can":"cannot");
			tmp.AppendFormat("You <b>{0}</b> edit your posts in this forum.<br>",ForumEditAccess?"can":"cannot");
			tmp.AppendFormat("You <b>{0}</b> create polls in this forum.<br>",ForumPollAccess?"can":"cannot");
			tmp.AppendFormat("You <b>{0}</b> vote in polls in this forum.<br>",ForumVoteAccess?"can":"cannot");
			AccessCell.InnerHtml = tmp.ToString();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			moderate1.Click += new EventHandler(moderate_Click);
			moderate2.Click += new EventHandler(moderate_Click);
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
				Response.Redirect(String.Format("moderate.aspx?f={0}",PageForumID));
		}

		private void ShowList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BindData();
		}

		protected string FormatLastPost(System.Data.DataRowView row) 
		{
			if(row["LastPosted"].ToString().Length>0) {
				string minipost;
				if(DateTime.Parse(row["LastPosted"].ToString()) > (DateTime)Session["lastvisit"])
					minipost = ThemeFile("icon_newest_reply.gif");
				else
					minipost = ThemeFile("icon_latest_reply.gif");
				return String.Format(CustomCulture,"{0}<br />by <a href=\"profile.aspx?u={1}\">{2}</a>&nbsp;<a href=\"posts.aspx?m={4}#{4}\"><img border=0 src='{3}'></a>", 
					FormatDateTime((DateTime)row["LastPosted"]), 
					row["LastUserID"], 
					row["LastUserName"], 
					minipost, 
					row["LastMessageID"]
				);
			} else
				return "No Posts";
		}

		protected string GetTopicImage(object o) {
			DataRowView row = (DataRowView)o;
			object lastPosted = row["LastPosted"];
			object isLocked = row["IsLocked"];
			try {
				bool bIsLocked = (bool)isLocked || (bool)forum["Locked"];

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

		private void BindData() 
		{
			PagedDataSource pds = new PagedDataSource();
			pds.AllowPaging = true;

			using(SqlCommand cmd = new SqlCommand("yaf_topic_list")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",PageForumID);
				cmd.Parameters.Add("@UserID",PageUserID);
				cmd.Parameters.Add("@Announcement",true);
				Announcements.DataSource = DataManager.GetData(cmd);
			}

			if(ShowList.SelectedIndex==0) 
			{
				pds.DataSource = Data.GetTopics(int.Parse(Request.QueryString["f"]),PageUserID,false).DefaultView;
			} 
			else 
			{
				DateTime date = DateTime.Now;
				switch(ShowList.SelectedIndex) 
				{
					case 1:
						date -= TimeSpan.FromDays(7);
						break;
					case 2:
						date -= TimeSpan.FromDays(14);
						break;
					case 3:
						date -= TimeSpan.FromDays(31);
						break;
					case 4:
						date -= TimeSpan.FromDays(2*31);
						break;
					case 5:
						date -= TimeSpan.FromDays(6*31);
						break;
					case 6:
						date -= TimeSpan.FromDays(365);
						break;
				}
				pds.DataSource = Data.GetTopics(int.Parse(Request.QueryString["f"]),PageUserID,false,date).DefaultView;

			}

			pds.PageSize = 15;
			if(Request.QueryString["p"] != null)
				pds.CurrentPageIndex = int.Parse(Request.QueryString["p"]);
			else
				pds.CurrentPageIndex = 0;

			TopicList.DataSource = pds;

			DataBind();

			if(pds.PageCount>1) {
				PageLinks1.InnerHtml = String.Format("{0} Pages:",pds.PageCount);
				for(int i=0;i<pds.PageCount;i++) {
					if(i==pds.CurrentPageIndex) {
						PageLinks1.InnerHtml += String.Format(" [{0}]",i+1);
					} else {
						PageLinks1.InnerHtml += String.Format(" <a href=\"topics.aspx?f={2}&p={1}\">{0}</a>",i+1,i,PageForumID);
					}
				}
				PageLinks2.InnerHtml = PageLinks1.InnerHtml;
			} else {
				PageLinks1.Visible = false;
				PageLinks2.Visible = false;
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

		private void NewTopic_Click(object sender, System.EventArgs e) {
			if((bool)forum["Locked"]) {
				AddLoadMessage("The forum is closed.");
				return;
			}

			int ForumID = int.Parse(Request.QueryString["f"]);
			if(!ForumPostAccess) {
				AddLoadMessage("You don't have access to post new topics in this forum.");
				return;
			}
			
			Response.Redirect(String.Format("postmessage.aspx?f={0}",ForumID));
		}

		private void WatchForum_Click(object sender, System.EventArgs e) {
			if(!ForumReadAccess)
				return;

			if(IsGuest) {
				AddLoadMessage("You must be registered to watch forums.");
				return;
			}

			using(SqlCommand cmd = new SqlCommand("yaf_watchforum_add")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",PageUserID);
				cmd.Parameters.Add("@ForumID",PageForumID);
				DataManager.ExecuteNonQuery(cmd);
			}
			AddLoadMessage("You will now be notified when new posts arrive in this forum.");
		}
	}
}
