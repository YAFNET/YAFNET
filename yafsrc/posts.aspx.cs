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

namespace yaf
{
	/// <summary>
	/// Summary description for posts.
	/// </summary>
	public class posts : BasePage
	{
		protected System.Web.UI.WebControls.HyperLink ForumLink;
		protected System.Web.UI.WebControls.LinkButton NewTopic1;
		protected System.Web.UI.WebControls.Repeater MessageList;
		protected System.Web.UI.WebControls.LinkButton PostReplyLink1;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink CategoryLink;
		protected System.Web.UI.WebControls.Repeater Poll;

		private DataRow forum, topic;
		protected System.Web.UI.WebControls.LinkButton PrevTopic;
		protected System.Web.UI.WebControls.LinkButton NextTopic;
		protected System.Web.UI.WebControls.LinkButton PrintTopic;
		protected System.Web.UI.WebControls.LinkButton EmailTopic;
		protected System.Web.UI.WebControls.LinkButton DeleteTopic1;
		protected System.Web.UI.WebControls.LinkButton LockTopic1;
		protected System.Web.UI.WebControls.LinkButton UnlockTopic1;
		protected System.Web.UI.WebControls.Label TopicTitle;
		protected System.Web.UI.WebControls.HyperLink TopicLink;
		private DataTable dtPoll;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks1;
		protected System.Web.UI.WebControls.LinkButton PostReplyLink2;
		protected System.Web.UI.WebControls.LinkButton NewTopic2;
		protected System.Web.UI.WebControls.LinkButton DeleteTopic2;
		protected System.Web.UI.WebControls.LinkButton LockTopic2;
		protected System.Web.UI.WebControls.LinkButton UnlockTopic2;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks2;
		protected System.Web.UI.WebControls.LinkButton TrackTopic;
		protected System.Web.UI.WebControls.LinkButton MoveTopic1;
		protected System.Web.UI.WebControls.LinkButton MoveTopic2;
		private FormatMsg fmt;
		protected System.Web.UI.HtmlControls.HtmlTableCell AccessCell;

		private void Page_Load(object sender, System.EventArgs e)
		{
			//if(Request.QueryString["t"] == null)
			//	Response.Redirect(BaseDir);

			using(DataTable dt = DB.forum_list(PageForumID))
				forum = dt.Rows[0];
			topic = DB.topic_info(PageTopicID);

			if(!ForumReadAccess)
				Response.Redirect(BaseDir);

			HomeLink.NavigateUrl = BaseDir;
			HomeLink.Text = ForumName;
			CategoryLink.NavigateUrl = String.Format("default.aspx?c={0}",PageCategoryID);
			CategoryLink.Text = PageCategoryName;
			ForumLink.NavigateUrl = String.Format("topics.aspx?f={0}",forum["ForumID"]);
			ForumLink.Text = (string)forum["Name"];
			TopicLink.NavigateUrl = String.Format("posts.aspx?t={0}",topic["TopicID"]);
			TopicLink.Text = (string)topic["Topic"];
			TopicTitle.Text = (string)topic["Topic"];

			if(!ForumPostAccess) {
				NewTopic1.Visible = false;
				NewTopic2.Visible = false;
			}
			
			if(!ForumReplyAccess) {
				PostReplyLink1.Visible = false;
				PostReplyLink2.Visible = false;
			}

			if(ForumModeratorAccess) {
				MoveTopic1.Visible = true;
				MoveTopic2.Visible = true;
			} else {
				MoveTopic1.Visible = false;
				MoveTopic2.Visible = false;
			}

			BindData();

			System.Text.StringBuilder tmp = new System.Text.StringBuilder();
			tmp.Append(GetText(ForumPostAccess ? "can_post" : "cannot_post"));
			tmp.Append("<br/>");
			tmp.Append(GetText(ForumReplyAccess ? "can_reply" : "cannot_reply"));
			tmp.Append("<br/>");
			tmp.Append(GetText(ForumDeleteAccess ? "can_delete" : "cannot_delete"));
			tmp.Append("<br/>");
			tmp.Append(GetText(ForumEditAccess ? "can_edit" : "cannot_edit"));
			tmp.Append("<br/>");
			tmp.Append(GetText(ForumPollAccess ? "can_poll" : "cannot_poll"));
			tmp.Append("<br/>");
			tmp.Append(GetText(ForumVoteAccess ? "can_vote" : "cannot_vote"));
			tmp.Append("<br/>");
			AccessCell.InnerHtml = tmp.ToString();
		}

		protected void DeleteMessage_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')",GetText("confirm_deletemessage"));
		}

		protected void DeleteTopic_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')",GetText("confirm_deletetopic"));
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
			using(DataTable dt = DB.forum_list(PageForumID))
				forum = dt.Rows[0];

			topic = DB.topic_info(PageTopicID);
			if(topic==null)
				Response.Redirect(String.Format("topics.aspx?f={0}",PageForumID));

				PagedDataSource pds = new PagedDataSource();
				pds.AllowPaging = true;
				pds.PageSize = 20;
				using(DataTable dt = DB.post_list(PageTopicID,PageUserID,1)) 
				{
					pds.DataSource = dt.DefaultView;
					if(Request.QueryString["m"] != null) 
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

					if(Request.QueryString["p"] != null)
						pds.CurrentPageIndex = int.Parse(Request.QueryString["p"]);

					if(pds.CurrentPageIndex>=pds.PageCount) pds.CurrentPageIndex = pds.PageCount - 1;
				
					MessageList.DataSource = pds;

			if(pds.PageCount>1) 
			{
				PageLinks1.InnerHtml = String.Format("{0} Pages:",pds.PageCount);
				for(int i=0;i<pds.PageCount;i++) 
				{
					if(i==pds.CurrentPageIndex) 
					{
						PageLinks1.InnerHtml += String.Format(" [{0}]",i+1);
					} 
					else 
					{
						PageLinks1.InnerHtml += String.Format(" <a href=\"posts.aspx?t={2}&p={1}\">{0}</a>",i+1,i,PageTopicID);
					}
				}
				PageLinks2.InnerHtml = PageLinks1.InnerHtml;
			} 
			else 
			{
				PageLinks1.Visible = false;
				PageLinks2.Visible = false;
			}

			if(topic["PollID"].ToString() != "") {
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


		private void DeleteTopic_Click(object sender, System.EventArgs e)
		{
			if(!ForumModeratorAccess) {
				AddLoadMessage("You don't have access to delete topics.");
				return;
			}

			DB.topic_delete(PageTopicID);
			Response.Redirect(String.Format("topics.aspx?f={0}",PageForumID));
		}

		private void LockTopic_Click(object sender, System.EventArgs e)
		{
			DB.topic_lock(PageTopicID,true);
			BindData();
			AddLoadMessage("Topic was locked.");
		}

		private void UnlockTopic_Click(object sender, System.EventArgs e)
		{
			DB.topic_lock(PageTopicID,false);
			BindData();
			AddLoadMessage("Topic was unlocked.");
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
					Response.Redirect(String.Format("postmessage.aspx?t={0}&f={1}&q={2}",PageTopicID,PageForumID,e.CommandArgument));
					break;
				case "Delete":
					if((bool)topic["IsLocked"]) {
						AddLoadMessage("The topic is closed.");
						return;
					}

					if((bool)forum["Locked"]) {
						AddLoadMessage("The forum is closed.");
						return;
					}

					if(!ForumDeleteAccess) {
						AddLoadMessage("You don't have access to delete posts in this forum.");
						return;
					}

					// Check that non-moderators only delete messages they have written
					if(!ForumModeratorAccess) {
						using(DataTable dt = DB.message_list(e.CommandArgument)) {
							if((int)dt.Rows[0]["UserID"] != PageUserID) {
								AddLoadMessage("You didn't post this message.");
								return;
							}
						}
					}

					DB.message_delete(e.CommandArgument);
					BindData();
					AddLoadMessage("Message was deleted.");
					break;
				case "Edit":
					if((bool)topic["IsLocked"]) {
						AddLoadMessage("The topic is closed.");
						return;
					}

					if((bool)forum["Locked"]) {
						AddLoadMessage("The forum is closed.");
						return;
					}

					if(!ForumEditAccess) {
						AddLoadMessage("You don't have access to edit posts in this forum.");
						return;
					}

					// Check that non-moderators only edit messages they have written
					if(!ForumModeratorAccess) {
						using(DataTable dt = DB.message_list(e.CommandArgument)) {
							if((int)dt.Rows[0]["UserID"] != PageUserID) {
								AddLoadMessage("You didn't post this message.");
								return;
							}
						}
					}

					Response.Redirect(String.Format("postmessage.aspx?m={0}",e.CommandArgument));
/*
					using(SqlCommand cmd = new SqlCommand("yaf_message_list")) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@MessageID",e.CommandArgument);
						DataTable dt = DataManager.GetData(cmd);
						if(dt.Rows.Count==1) {
							if((int)dt.Rows[0]["UserID"]==PageUserID)
								Response.Redirect(String.Format("postmessage.aspx?m={0}",e.CommandArgument));
						}
					}
					*/
					break;
				case "PM":
					if(!User.Identity.IsAuthenticated) {
						AddLoadMessage("You must be logged in to send private messages.");
						return;
					}

					Response.Redirect(String.Format("pmessage.aspx?u={0}",e.CommandArgument));
					break;
			}
		
		}

		private void Poll_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			if(e.CommandName=="vote" && ForumVoteAccess) {
				string cookie = String.Format("poll#{0}#{1}",topic["PollID"],PageUserID);
			
				if(Request.Cookies[cookie] != null) {
					AddLoadMessage("You have already voted.");
					return;
				}

				if((bool)topic["IsLocked"]) {
					AddLoadMessage("The topic is closed.");
					return;
				}

				DB.choice_vote(e.CommandArgument);
				HttpCookie c = new HttpCookie(cookie,e.CommandArgument.ToString());
				c.Expires = DateTime.Now + TimeSpan.FromDays(365);
				Response.Cookies.Add(c);
				AddLoadMessage("Thank you for your vote!");
				BindData();
			}
		}

		protected string GetPollQuestion() {
			return (string)dtPoll.Rows[0]["Question"];
		}

		private void PostReplyLink_Click(object sender, System.EventArgs e) {
			if((bool)topic["IsLocked"]) {
				AddLoadMessage("The topic is closed.");
				return;
			}

			if((bool)forum["Locked"]) {
				AddLoadMessage("The forum is closed.");
				return;
			}
					
			Response.Redirect(String.Format("postmessage.aspx?t={0}&f={1}",PageTopicID,PageForumID));
		}

		private void NewTopic_Click(object sender, System.EventArgs e) {
			if((bool)forum["Locked"]) {
				AddLoadMessage("The forum is closed.");
				return;
			}
			Response.Redirect("postmessage.aspx?f=" + PageForumID);
		}

		protected string FormatUserBox(System.Data.DataRowView row) {
			string html = "";

			// Avatar
			if((bool)row["AvatarUpload"] && row["HasAvatarImage"]!=null && long.Parse(row["HasAvatarImage"].ToString())>0) 
			{
				html += String.Format("<img src='image.aspx?u={0}'><br clear=\"all\"/>",row["UserID"]);
			} 
			else if((bool)row["AvatarRemote"] && row["Avatar"].ToString().Length>0) 
			{
				//html += String.Format("<img src='{0}'><br clear=\"all\"/>",row["Avatar"]);
				html += String.Format("<img src='image.aspx?url={0}&width={1}&height={2}'><br clear=\"all\"/>",
					Server.UrlEncode(row["Avatar"].ToString()),
					row["AvatarWidth"],
					row["AvatarHeight"]
				);
			}

			// Rank Image
			if(row["RankImage"].ToString().Length>0)
				html += String.Format("<img align=left src=\"{0}images/ranks/{1}\"/><br clear=\"all\"/>",BaseDir,row["RankImage"]);

			// Rank
			html += String.Format("{0}: {1}<br clear=\"all\"/>",GetText("rank"),row["RankName"]);

			// Groups
			if(ShowGroups) 
			{
				using(DataTable dt = DB.usergroup_list(row["UserID"])) 
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
			html += String.Format(CustomCulture,"{0}: {1}<br/>",GetText("joined"),FormatDateShort((DateTime)row["Joined"]));

			// Posts
			html += String.Format(CustomCulture,"{0}: {1:N0}<br/>",GetText("posts"),row["Posts"]);

			// Location
			if(row["Location"].ToString().Length>0)
				html += String.Format("{0}: {1}<br/>",GetText("location"),row["Location"]);

			return html;
		}

		private void TrackTopic_Click(object sender, System.EventArgs e) {
			if(IsGuest) {
				AddLoadMessage("You must be registered to watch topics.");
				return;
			}

			DB.watchtopic_add(PageUserID,PageTopicID);
			AddLoadMessage("You will now be notified when new posts arrive in this topic.");
		}
		
		private void MoveTopic_Click(object sender, System.EventArgs e) {
			if(!ForumModeratorAccess) {
				AddLoadMessage("You are not a forum moderator.");
				return;
			}

			Response.Redirect(String.Format("movetopic.aspx?t={0}",PageTopicID));
		}

		private void PrevTopic_Click(object sender, System.EventArgs e) {
			using(DataTable dt = DB.topic_findprev(PageTopicID)) {
				if(dt.Rows.Count==0) {
					AddLoadMessage("No more topics in this forum.");
					return;
				}
				Response.Redirect(String.Format("posts.aspx?t={0}",dt.Rows[0]["TopicID"]));
			}
		}

		private void NextTopic_Click(object sender, System.EventArgs e) {
			using(DataTable dt = DB.topic_findnext(PageTopicID)) {
				if(dt.Rows.Count==0) {
					AddLoadMessage("No more topics in this forum.");
					return;
				}
				Response.Redirect(String.Format("posts.aspx?t={0}",dt.Rows[0]["TopicID"]));
			}
		}
		private void EmailTopic_Click(object sender, System.EventArgs e) {
			if(!User.Identity.IsAuthenticated) {
				AddLoadMessage("You must be registered to send emails.");
				return;
			}
			Response.Redirect(String.Format("emailtopic.aspx?t={0}",PageTopicID));
		}
		private void PrintTopic_Click(object sender, System.EventArgs e) {
			Response.Redirect(String.Format("printtopic.aspx?t={0}",PageTopicID));
		}

		protected string FormatBody(object o) {
			DataRowView row = (DataRowView)o;
			string html = row["Message"].ToString();
			bool isHtml = html.IndexOf('<')>=0;
		
			if(long.Parse(row["HasAttachments"].ToString())>0) 
			{
				string sUpDir = System.Configuration.ConfigurationSettings.AppSettings["uploaddir"];

				html += "<p><b>Attachments:</b><br/>";
				using(DataTable dt = DB.attachment_list(row["MessageID"])) 
				{
					foreach(DataRow dr in dt.Rows) 
					{
						html += String.Format("<a href=\"{0}{1}\">{1}</a><br/>",sUpDir,dr["FileName"]);
					}
				}
				html += "</p>";
			}
			
			if(fmt==null)
				fmt = new FormatMsg(this);

			if(row["Signature"].ToString().Length>0)
				html += "<br/><hr noshade/>" + fmt.FormatMessage(row["Signature"].ToString());


			if(isHtml)
				return html;

			return fmt.FormatMessage(html);
		}

		protected bool CanEditPost(DataRowView row) 
		{
			return ((int)row["UserID"]==PageUserID || ForumModeratorAccess) && ForumEditAccess;
		}

		protected bool CanDeletePost(DataRowView row) 
		{
			return ((int)row["UserID"]==PageUserID || ForumModeratorAccess) && ForumDeleteAccess;
		}
	}
}
