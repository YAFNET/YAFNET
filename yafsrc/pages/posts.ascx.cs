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
using System.Text.RegularExpressions;

namespace yaf.pages
{
	/// <summary>
	/// Summary description for posts.
	/// </summary>
	public class posts : ForumPage
	{
		protected System.Web.UI.WebControls.LinkButton NewTopic1;
		protected System.Web.UI.WebControls.Repeater MessageList;
		protected System.Web.UI.WebControls.LinkButton PostReplyLink1;
		protected System.Web.UI.WebControls.Repeater Poll;
		protected controls.PageLinks PageLinks;
		protected controls.Pager Pager;

		private DataRow forum, topic;
		protected System.Web.UI.WebControls.LinkButton PrevTopic;
		protected System.Web.UI.WebControls.LinkButton NextTopic;
		protected System.Web.UI.WebControls.LinkButton PrintTopic;
		protected System.Web.UI.WebControls.LinkButton EmailTopic;
		protected System.Web.UI.WebControls.LinkButton DeleteTopic1;
		protected System.Web.UI.WebControls.LinkButton LockTopic1;
		protected System.Web.UI.WebControls.LinkButton UnlockTopic1;
		protected System.Web.UI.WebControls.Label TopicTitle;
		private DataTable dtPoll;
		protected System.Web.UI.WebControls.LinkButton PostReplyLink2;
		protected System.Web.UI.WebControls.LinkButton NewTopic2;
		protected System.Web.UI.WebControls.LinkButton DeleteTopic2;
		protected System.Web.UI.WebControls.LinkButton LockTopic2;
		protected System.Web.UI.WebControls.LinkButton UnlockTopic2;
		protected System.Web.UI.WebControls.LinkButton TrackTopic;
		protected System.Web.UI.WebControls.LinkButton MoveTopic1;
		protected System.Web.UI.WebControls.LinkButton MoveTopic2;

		public posts() : base("POSTS")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			topic = DB.topic_info(PageTopicID);
			using(DataTable dt = DB.forum_list(PageBoardID,PageForumID))
				forum = dt.Rows[0];

			if(!ForumReadAccess)
				Data.AccessDenied();

			if(!IsPostBack) 
			{
				PageLinks.AddLink(Config.BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(PageCategoryName,Forum.GetLink(Pages.forum,"c={0}",PageCategoryID));
				PageLinks.AddLink(PageForumName,Forum.GetLink(Pages.topics,"f={0}",PageForumID));
				PageLinks.AddLink(PageTopicName,Forum.GetLink(Pages.posts,"t={0}",PageTopicID));
				TopicTitle.Text = (string)topic["Topic"];

				if(!ForumPostAccess) 
				{
					NewTopic1.Visible = false;
					NewTopic2.Visible = false;
				}
			
				if(!ForumReplyAccess) 
				{
					PostReplyLink1.Visible = false;
					PostReplyLink2.Visible = false;
				}

				if(ForumModeratorAccess) 
				{
					MoveTopic1.Visible = true;
					MoveTopic2.Visible = true;
				} 
				else 
				{
					MoveTopic1.Visible = false;
					MoveTopic2.Visible = false;
				}

				PostReplyLink1.Text = GetThemeContents("BUTTONS","POSTREPLY");
				PostReplyLink1.ToolTip = "Post reply";
				PostReplyLink2.Text = PostReplyLink1.Text;
				PostReplyLink2.ToolTip = PostReplyLink1.ToolTip;
				NewTopic1.Text = GetThemeContents("BUTTONS","NEWTOPIC");
				NewTopic1.ToolTip = "Post new topic";
				NewTopic2.Text = NewTopic1.Text;
				NewTopic2.ToolTip = NewTopic1.ToolTip;
				DeleteTopic1.Text = GetThemeContents("BUTTONS","DELETETOPIC");
				DeleteTopic1.ToolTip = "Delete this topic";
				DeleteTopic2.Text = DeleteTopic1.Text;
				DeleteTopic2.ToolTip = DeleteTopic1.ToolTip;
				LockTopic1.Text = GetThemeContents("BUTTONS","LOCKTOPIC");
				LockTopic1.ToolTip = "Lock this topic";
				LockTopic2.Text = LockTopic1.Text;
				LockTopic2.ToolTip = LockTopic1.ToolTip;
				UnlockTopic1.Text = GetThemeContents("BUTTONS","UNLOCKTOPIC");
				UnlockTopic1.ToolTip = "Unlock this topic";
				UnlockTopic2.Text = UnlockTopic1.Text;
				UnlockTopic2.ToolTip = UnlockTopic1.ToolTip;
				MoveTopic1.Text = GetThemeContents("BUTTONS","MOVETOPIC");
				MoveTopic1.ToolTip = "Move this topic";
				MoveTopic2.Text = MoveTopic1.Text;
				MoveTopic2.ToolTip = MoveTopic1.ToolTip;

				BindData();
			}
			/// Mark topic read
			SetTopicRead(PageTopicID,DateTime.Now);
		}

		protected void DeleteMessage_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')",GetText("confirm_deletemessage"));
		}

		protected void DeleteTopic_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')",GetText("confirm_deletetopic"));
		}

		private void Pager_PageChange(object sender, EventArgs e)
		{
			BindData();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
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
			this.Poll.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.Poll_ItemCommand);
			this.PostReplyLink1.Click += new System.EventHandler(this.PostReplyLink_Click);
			this.NewTopic1.Click += new System.EventHandler(this.NewTopic_Click);
			this.DeleteTopic1.Click += new System.EventHandler(this.DeleteTopic_Click);
			this.LockTopic1.Click += new System.EventHandler(this.LockTopic_Click);
			this.UnlockTopic1.Click += new System.EventHandler(this.UnlockTopic_Click);
			this.TrackTopic.Click += new System.EventHandler(this.TrackTopic_Click);
			this.MessageList.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.MessageList_ItemCommand);
			this.PostReplyLink2.Click += new System.EventHandler(this.PostReplyLink_Click);
			this.NewTopic2.Click += new System.EventHandler(this.NewTopic_Click);
			this.DeleteTopic2.Click += new System.EventHandler(this.DeleteTopic_Click);
			this.LockTopic2.Click += new System.EventHandler(this.LockTopic_Click);
			this.UnlockTopic2.Click += new System.EventHandler(this.UnlockTopic_Click);
			this.MoveTopic1.Click += new System.EventHandler(this.MoveTopic_Click);
			this.MoveTopic2.Click += new System.EventHandler(this.MoveTopic_Click);
			this.PrevTopic.Click += new System.EventHandler(this.PrevTopic_Click);
			this.NextTopic.Click += new System.EventHandler(this.NextTopic_Click);
			this.PrintTopic.Click += new System.EventHandler(this.PrintTopic_Click);
			this.EmailTopic.Click += new System.EventHandler(this.EmailTopic_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void BindData() 
		{
			Pager.PageSize = 20;
			if(topic==null)
				Forum.Redirect(Pages.topics,"f={0}",PageForumID);

			PagedDataSource pds = new PagedDataSource();
			pds.AllowPaging = true;
			pds.PageSize = Pager.PageSize;

			using(DataTable dt = DB.post_list(PageTopicID,1)) 
			{
				Pager.Count = dt.Rows.Count;
				pds.DataSource = dt.DefaultView;
				int nFindMessage = 0;
				try
				{
					if(Request.QueryString["m"]!=null)
					{
						// Show this message
						nFindMessage = int.Parse(Request.QueryString["m"]);
					}
					else if(Request.QueryString["find"]!=null && Request.QueryString["find"].ToLower()=="unread")
					{
						// Find next unread
						using(DataTable dtUnread = DB.message_findunread(PageTopicID,Mession.LastVisit))
						{
							foreach(DataRow row in dtUnread.Rows)
							{
								nFindMessage = (int)row["MessageID"];
								break;
							}
						}
					}
				}
				catch(Exception)
				{
				}

				if(nFindMessage>0) 
				{
					// Find correct page for message
					long nMessageID = long.Parse(Request.QueryString["m"]);
					for(int nRow=0;nRow<dt.Rows.Count;nRow++) 
					{
						if((int)dt.Rows[nRow]["MessageID"] == nMessageID) 
						{
							pds.CurrentPageIndex = nRow / pds.PageSize;
							break;
						}
					}
				}
			}

			pds.CurrentPageIndex = Pager.CurrentPageIndex;

			if(pds.CurrentPageIndex>=pds.PageCount) pds.CurrentPageIndex = pds.PageCount - 1;
				
			MessageList.DataSource = pds;

			if(topic["PollID"]!=DBNull.Value) 
			{
				Poll.Visible = true;
				dtPoll = DB.poll_stats(topic["PollID"]);
				Poll.DataSource = dtPoll;
			}
			
			if(!ForumModeratorAccess) 
			{
				LockTopic1.Visible = false;
				UnlockTopic1.Visible = false;
				DeleteTopic1.Visible = false;
				LockTopic2.Visible = false;
				UnlockTopic2.Visible = false;
				DeleteTopic2.Visible = false;
			} 
			else 
			{
				LockTopic1.Visible = !(bool)topic["IsLocked"];
				UnlockTopic1.Visible = !LockTopic1.Visible;
				LockTopic2.Visible = !(bool)topic["IsLocked"];
				UnlockTopic2.Visible = !LockTopic2.Visible;
			}

			DataBind();
		}

		protected bool CanVote
		{
			get
			{
				string cookie = String.Format("poll#{0}#{1}",topic["PollID"],PageUserID);
				return ForumVoteAccess && Request.Cookies[cookie] == null;
			}
		}

		private void DeleteTopic_Click(object sender, System.EventArgs e)
		{
			if(!ForumModeratorAccess)
				Data.AccessDenied(/*"You don't have access to delete topics."*/);

			DB.topic_delete(PageTopicID);
			Forum.Redirect(Pages.topics,"f={0}",PageForumID);
		}

		private void LockTopic_Click(object sender, System.EventArgs e)
		{
			DB.topic_lock(PageTopicID,true);
			BindData();
			AddLoadMessage(GetText("INFO_TOPIC_LOCKED"));
		}

		private void UnlockTopic_Click(object sender, System.EventArgs e)
		{
			DB.topic_lock(PageTopicID,false);
			BindData();
			AddLoadMessage(GetText("INFO_TOPIC_UNLOCKED"));
		}


		private void MessageList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "Quote":
					if((bool)topic["IsLocked"]) {
						AddLoadMessage("The topic is closed.");
						return;
					}

					if((bool)forum["Locked"]) {
						AddLoadMessage("The forum is closed.");
						return;
					}
					
					if(!ForumReplyAccess) {
						AddLoadMessage("You don't have access to reply to posts in this forum.");
						return;
					}
					Forum.Redirect(Pages.postmessage,"t={0}&f={1}&q={2}",PageTopicID,PageForumID,e.CommandArgument);
					break;
				case "Delete":
					if((bool)topic["IsLocked"]) {
						AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
						return;
					}

					if((bool)forum["Locked"]) {
						AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
						return;
					}

					if(!ForumDeleteAccess)
						Data.AccessDenied(/*"You don't have access to delete posts in this forum."*/);

					// Check that non-moderators only delete messages they have written
					if(!ForumModeratorAccess) {
						using(DataTable dt = DB.message_list(e.CommandArgument)) {
							if((int)dt.Rows[0]["UserID"] != PageUserID)
								Data.AccessDenied(/*"You didn't post this message."*/);
						}
					}

					DB.message_delete(e.CommandArgument);
					BindData();
					AddLoadMessage(GetText("INFO_MESSAGE_DELETED"));
					break;
				case "Attach":
					if((bool)topic["IsLocked"]) 
					{
						AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
						return;
					}

					if((bool)forum["Locked"]) 
					{
						AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
						return;
					}

					if(!ForumUploadAccess) 
						Data.AccessDenied(/*"You don't have access to attach files in this forum."*/);

					// Check that non-moderators only edit messages they have written
					if(!ForumModeratorAccess) 
					{
						using(DataTable dt = DB.message_list(e.CommandArgument)) 
						{
							if((int)dt.Rows[0]["UserID"] != PageUserID) 
								Data.AccessDenied(/*"You didn't post this message."*/);
						}
					}

					Forum.Redirect(Pages.attachments,"m={0}",e.CommandArgument);
					break;
				case "Edit":
					if((bool)topic["IsLocked"]) {
						AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
						return;
					}

					if((bool)forum["Locked"]) {
						AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
						return;
					}

					if(!ForumEditAccess)
						Data.AccessDenied(/*"You don't have access to edit posts in this forum."*/);

					// Check that non-moderators only edit messages they have written
					if(!ForumModeratorAccess) {
						using(DataTable dt = DB.message_list(e.CommandArgument)) {
							if((int)dt.Rows[0]["UserID"] != PageUserID)
								Data.AccessDenied(/*"You didn't post this message."*/);
						}
					}

					Forum.Redirect(Pages.postmessage,"m={0}",e.CommandArgument);
					break;
				case "PM":
					if(!User.IsAuthenticated) {
						AddLoadMessage(GetText("WARN_PMLOGIN"));
						return;
					}

					Forum.Redirect(Pages.pmessage,"u={0}",e.CommandArgument);
					break;
			}
		
		}

		private void Poll_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			if(e.CommandName=="vote" && ForumVoteAccess) {
				string cookie = String.Format("poll#{0}#{1}",topic["PollID"],PageUserID);

				if(Request.Cookies[cookie] != null) 
				{
					AddLoadMessage(GetText("WARN_ALREADY_VOTED"));
					return;
				}

				if((bool)topic["IsLocked"]) {
					AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
					return;
				}

				DB.choice_vote(e.CommandArgument);
				HttpCookie c = new HttpCookie(cookie,e.CommandArgument.ToString());
				c.Expires = DateTime.Now.AddYears(1);
				Response.Cookies.Add(c);
				AddLoadMessage(GetText("INFO_VOTED"));
				BindData();
			}
		}

		protected string GetPollQuestion() {
			return (string)dtPoll.Rows[0]["Question"];
		}

		private void PostReplyLink_Click(object sender, System.EventArgs e) {
			if((bool)topic["IsLocked"]) {
				AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
				return;
			}

			if((bool)forum["Locked"]) {
				AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
				return;
			}
					
			Forum.Redirect(Pages.postmessage,"t={0}&f={1}",PageTopicID,PageForumID);
		}

		private void NewTopic_Click(object sender, System.EventArgs e) {
			if((bool)forum["Locked"]) {
				AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
				return;
			}
			Forum.Redirect(Pages.postmessage,"f={0}",PageForumID);
		}

		protected string FormatUserBox(System.Data.DataRowView row) {
			string html = "";

			// Avatar
			if(Config.BoardSettings.AvatarUpload && row["HasAvatarImage"]!=null && long.Parse(row["HasAvatarImage"].ToString())>0) 
			{
				html += String.Format("<img src='{1}image.aspx?u={0}'><br clear=\"all\"/>",row["UserID"],ForumRoot);
			} 
			else if(Config.BoardSettings.AvatarRemote && row["Avatar"].ToString().Length>0) 
			{
				//html += String.Format("<img src='{0}'><br clear=\"all\"/>",row["Avatar"]);
				html += String.Format("<img src='{3}image.aspx?url={0}&width={1}&height={2}'><br clear=\"all\"/>",
					Server.UrlEncode(row["Avatar"].ToString()),
					Config.BoardSettings.AvatarWidth,
					Config.BoardSettings.AvatarHeight,
					ForumRoot
				);
			}

			// Rank Image
			if(row["RankImage"].ToString().Length>0)
				html += String.Format("<img align=left src=\"{0}images/ranks/{1}\"/><br clear=\"all\"/>",ForumRoot,row["RankImage"]);

			// Rank
			html += String.Format("{0}: {1}<br clear=\"all\"/>",GetText("rank"),row["RankName"]);

			// Groups
			if(Config.BoardSettings.ShowGroups) 
			{
				using(DataTable dt = DB.usergroup_list(PageBoardID,row["UserID"])) 
				{
					html += String.Format("{0}: ",GetText("groups"));
					bool bFirst = true;
					foreach(DataRow grp in dt.Rows) 
					{
						if(bFirst) 
						{
							html += grp["Name"].ToString();
							bFirst = false;
						} 
						else 
						{
							html += String.Format(", {0}",grp["Name"]);
						}
					}
					html += "<br/>";
				}
			}

			// Extra row
			html += "<br/>";

			// Joined
			html += String.Format("{0}: {1}<br/>",GetText("joined"),FormatDateShort((DateTime)row["Joined"]));

			// Posts
			html += String.Format("{0}: {1:N0}<br/>",GetText("posts"),row["Posts"]);

			// Location
			if(row["Location"].ToString().Length>0)
				html += String.Format("{0}: {1}<br/>",GetText("location"),row["Location"]);

			return html;
		}

		private void TrackTopic_Click(object sender, System.EventArgs e) {
			if(IsGuest) {
				AddLoadMessage(GetText("WARN_WATCHLOGIN"));
				return;
			}

			DB.watchtopic_add(PageUserID,PageTopicID);
			AddLoadMessage(GetText("INFO_WATCH_TOPIC"));
		}
		
		private void MoveTopic_Click(object sender, System.EventArgs e) {
			if(!ForumModeratorAccess)
				Data.AccessDenied(/*"You are not a forum moderator."*/);

			Forum.Redirect(Pages.movetopic,"t={0}",PageTopicID);
		}

		private void PrevTopic_Click(object sender, System.EventArgs e) {
			using(DataTable dt = DB.topic_findprev(PageTopicID)) {
				if(dt.Rows.Count==0) {
					AddLoadMessage(GetText("INFO_NOMORETOPICS"));
					return;
				}
				Forum.Redirect(Pages.posts,"t={0}",dt.Rows[0]["TopicID"]);
			}
		}

		private void NextTopic_Click(object sender, System.EventArgs e) {
			using(DataTable dt = DB.topic_findnext(PageTopicID)) {
				if(dt.Rows.Count==0) {
					AddLoadMessage(GetText("INFO_NOMORETOPICS"));
					return;
				}
				Forum.Redirect(Pages.posts,"t={0}",dt.Rows[0]["TopicID"]);
			}
		}
		private void EmailTopic_Click(object sender, System.EventArgs e) {
			if(!User.IsAuthenticated) {
				AddLoadMessage(GetText("WARN_EMAILLOGIN"));
				return;
			}
			Forum.Redirect(Pages.emailtopic,"t={0}",PageTopicID);
		}
		private void PrintTopic_Click(object sender, System.EventArgs e) {
			Forum.Redirect(Pages.printtopic,"t={0}",PageTopicID);
		}

		protected string FormatBody(object o) {
			DataRowView row = (DataRowView)o;
			string html = row["Message"].ToString();
			bool isHtml = html.IndexOf('<')>=0;
		
			if(long.Parse(row["HasAttachments"].ToString())>0) 
			{
				html += String.Format("<p><b class='smallfont'>{0}</b><br/>",GetText("ATTACHMENTS"));
				string stats = GetText("ATTACHMENTINFO");
				using(DataTable dt = DB.attachment_list(row["MessageID"],null)) 
				{
					foreach(DataRow dr in dt.Rows) 
					{
						int kb = (1023 + (int)dr["Bytes"]) / 1024;
						html += String.Format("<a href=\"{0}image.aspx?a={1}\">{2}</a> <span class='smallfont'>- {3}</span><br/>",ForumRoot,dr["AttachmentID"],dr["FileName"],String.Format(stats,kb,dr["Downloads"]));
					}
				}
				html += "</p>";
			}
			
			if(row["Signature"] != DBNull.Value)
				html += "<br/><hr noshade/>" + FormatMsg.ForumCodeToHtml(this,row["Signature"].ToString());

			if(!isHtml)
				html = FormatMsg.ForumCodeToHtml(this,html);

			return FormatMsg.FetchURL(this,html);
		}

		protected bool CanEditPost(DataRowView row) 
		{
			return ((int)row["UserID"]==PageUserID || ForumModeratorAccess) && ForumEditAccess;
		}

		protected bool CanAttach(DataRowView row) 
		{
			return ((int)row["UserID"]==PageUserID || ForumModeratorAccess) && ForumUploadAccess;
		}

		protected bool CanDeletePost(DataRowView row) 
		{
			return ((int)row["UserID"]==PageUserID || ForumModeratorAccess) && ForumDeleteAccess;
		}

		protected int VoteWidth(object o) 
		{
			DataRowView row = (DataRowView)o;
			return (int)row["Stats"] * 80 / 100;
		}
	}
}
