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
	/// Summary description for postmessage.
	/// </summary>
	public class postmessage : BasePage
	{
		protected rte.rte Message;
		protected System.Web.UI.WebControls.TextBox Subject;
		protected System.Web.UI.WebControls.Button PostReply;
		protected System.Web.UI.WebControls.Label Title;
		protected System.Web.UI.HtmlControls.HtmlTableRow SubjectRow;
		protected System.Web.UI.WebControls.Button Preview;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink CategoryLink;
		protected System.Web.UI.HtmlControls.HtmlTableRow PriorityRow;
		protected System.Web.UI.WebControls.DropDownList Priority;
		protected System.Web.UI.HtmlControls.HtmlTableRow CreatePollRow;
		protected System.Web.UI.WebControls.LinkButton CreatePoll;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow1;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow2;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow3;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow4;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow5;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow6;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow7;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow8;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow9;
		protected System.Web.UI.HtmlControls.HtmlTableRow PollRow10;
		protected System.Web.UI.WebControls.TextBox PollChoice1;
		protected System.Web.UI.WebControls.TextBox PollChoice2;
		protected System.Web.UI.WebControls.TextBox PollChoice3;
		protected System.Web.UI.WebControls.TextBox PollChoice4;
		protected System.Web.UI.WebControls.TextBox PollChoice5;
		protected System.Web.UI.WebControls.TextBox PollChoice6;
		protected System.Web.UI.WebControls.TextBox PollChoice7;
		protected System.Web.UI.WebControls.TextBox PollChoice8;
		protected System.Web.UI.WebControls.TextBox PollChoice9;
		protected System.Web.UI.WebControls.Button Cancel;
		protected System.Web.UI.WebControls.TextBox Question;
		protected System.Web.UI.WebControls.TextBox From;
		protected System.Web.UI.HtmlControls.HtmlTableRow FromRow;
		protected System.Web.UI.HtmlControls.HtmlTableRow PreviewRow;
		protected System.Web.UI.HtmlControls.HtmlTableCell PreviewCell;
		protected System.Web.UI.WebControls.HyperLink ForumLink;
		protected System.Web.UI.WebControls.Repeater LastPosts;
		private int ForumID;
		private FormatMsg fmt;
		protected System.Web.UI.HtmlControls.HtmlTableRow UploadRow1, UploadRow2, UploadRow3;
		protected System.Web.UI.HtmlControls.HtmlInputFile File1, File2, File3;

		private void Page_Load(object sender, System.EventArgs e)
		{
			ForumID = PageForumID;
			DataRow msg = null;
			
			if(Request.QueryString["q"] != null)
				using(DataTable dt = DB.message_list(Request.QueryString["q"]))
					msg = dt.Rows[0];
			else if(Request.QueryString["m"] != null) {
				using(DataTable dt = DB.message_list(Request.QueryString["m"]))
					msg = dt.Rows[0];
			
				if(!ForumModeratorAccess && PageUserID != (int)msg["UserID"])
					Response.Redirect(BaseDir);
			}
	
			if(ForumID == 0)
				Data.AccessDenied();

			if(Request["t"]==null && !ForumPostAccess)
				Data.AccessDenied();
			if(Request["t"]!=null && !ForumReplyAccess)
				Data.AccessDenied();

			UploadRow1.Visible = ForumUploadAccess;
			UploadRow2.Visible = ForumUploadAccess;
			UploadRow3.Visible = ForumUploadAccess;

			Message.EnableRTE = IsAdmin;

			if(!IsPostBack) 
			{
				Priority.Items.Add(new ListItem(GetText("normal"),"0"));
				Priority.Items.Add(new ListItem(GetText("sticky"),"1"));
				Priority.Items.Add(new ListItem(GetText("announcement"),"2"));
				Priority.SelectedIndex = 0;

				Preview.Text = GetText("preview");
				PostReply.Text = GetText("Save");
				Cancel.Text = GetText("Cancel");
				CreatePoll.Text = GetText("createpoll");

				PriorityRow.Visible = ForumPriorityAccess;
				CreatePollRow.Visible = Request.QueryString["t"]==null && ForumPollAccess;

				HomeLink.Text = ForumName;
				HomeLink.NavigateUrl = BaseDir;
				CategoryLink.Text = PageCategoryName;
				CategoryLink.NavigateUrl = String.Format("default.aspx?c={0}",PageCategoryID);

				if(Request.QueryString["t"] != null) 
				{
					DataRow topic = DB.topic_info(Request.QueryString["t"]);
					if((bool)topic["IsLocked"])
						Response.Redirect(Request.UrlReferrer.ToString());
					SubjectRow.Visible = false;
					Title.Text = GetText("reply");

					// History (Last 10 posts)
					LastPosts.Visible = true;
					LastPosts.DataSource = DB.post_list_reverse10(Request.QueryString["t"]);
					LastPosts.DataBind();
				}

				if(Request.QueryString["q"] != null) {
					MSGFORMAT msgformat = Data.GetMsgFormat(msg["Format"]);
					if(Message.IsRTEBrowser) 
					{
						string body = msg["Message"].ToString();
						if(msgformat==MSGFORMAT.FORUM) 
						{
							FormatMsg fmt = new FormatMsg(this);
							body = fmt.FormatMessage(body);
						}
						Message.Text = String.Format("<br/><div class=\"quote\">{0} wrote:<div class=\"quoteinner\">{1}</div></div><br/>",msg["username"],Server.HtmlDecode(body));
					}
					else 
					{
						if(msgformat!=MSGFORMAT.FORUM)
							// TODO: Convert this html to forumcodes
							Message.Text = String.Format("[quote={0}]...[/quote]",msg["username"]);
						else
							Message.Text = String.Format("[quote={0}]{1}[/quote]",msg["username"],Server.HtmlDecode((string)msg["message"]));
					}
				} else if(Request.QueryString["m"] != null) {
					MSGFORMAT msgformat = Data.GetMsgFormat(msg["Format"]);
					string body = Server.HtmlDecode((string)msg["message"]);
					if(Message.IsRTEBrowser) 
					{
						if(msgformat==MSGFORMAT.FORUM) 
						{
							FormatMsg fmt = new FormatMsg(this);
							body = fmt.FormatMessage(body);
						}
					} 
					else 
					{
						if(msgformat!=MSGFORMAT.FORUM) 
						{
							throw new Exception("TODO: Convert this html message to forumcodes");
						}
					}
					Message.Text = body;
					
					Subject.Text = (string)msg["Topic"];
					Subject.Enabled = false;
					CreatePollRow.Visible = false;
					Priority.SelectedItem.Selected = false;
					Priority.Items.FindByValue(msg["Priority"].ToString()).Selected = true;
				}

				From.Text = PageUserName;
				if(User.Identity.IsAuthenticated)
					FromRow.Visible = false;
			}

			using(DataTable dt = DB.forum_list(ForumID)) 
			{
				foreach(DataRow forum in dt.Rows) 
				{
					ForumLink.Text = (string)forum["Name"];
					ForumLink.NavigateUrl = "topics.aspx?f=" + ForumID.ToString();
				}
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
			this.CreatePoll.Click += new System.EventHandler(this.CreatePoll_Click);
			this.Preview.Click += new System.EventHandler(this.Preview_Click);
			this.PostReply.Click += new System.EventHandler(this.PostReply_Click);
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void PostReply_Click(object sender, System.EventArgs e)
		{
			if(SubjectRow.Visible && Subject.Text.Length<=0) {
				AddLoadMessage(GetText("need_subject"));
				return;
			}

			try 
			{
				CheckValidFile(File1);
				CheckValidFile(File2);
				CheckValidFile(File3);
			}
			catch(Exception x) 
			{
				AddLoadMessage(x.Message);
				return;
			}

			// Must wait 30 seconds before posting again
			if(Session["lastpost"] != null && Request.QueryString["m"]==null) 
			{
				DateTime lastpost = DateTime.Parse(Session["lastpost"].ToString());
				lastpost += TimeSpan.FromSeconds(30);
				if(lastpost > DateTime.Now) {
					AddLoadMessage(String.Format(GetText("wait"),(lastpost - DateTime.Now).Seconds));
					return;
				}
			}

			MSGFORMAT fmt = MSGFORMAT.FORUM;
			if(Message.IsRTEBrowser)
				fmt = MSGFORMAT.HTML;

			long TopicID;
			long nMessageID = 0;
			string msg = Message.Text;
			if(fmt!=MSGFORMAT.HTML)
				msg = Server.HtmlEncode(msg);
			Session["lastpost"] = DateTime.Now;
			if(Request.QueryString["t"] != null) {
				if(!ForumReplyAccess)
					Data.AccessDenied();

				TopicID = long.Parse(Request.QueryString["t"]);
				if(!DB.message_save(TopicID,PageUserID,msg,From.Text,Request.UserHostAddress,fmt,ref nMessageID))
					TopicID = 0;
			} 
			else if(Request.QueryString["m"] != null) {
				if(!ForumEditAccess)
					Data.AccessDenied();

				DB.message_update(Request.QueryString["m"],Priority.SelectedValue,Message.Text);
				TopicID = PageTopicID;
				nMessageID = long.Parse(Request.QueryString["m"]);
			} 
			else {
				if(!ForumPostAccess)
					Data.AccessDenied();

				object PollID = null;
				if(PollRow1.Visible) {
					PollID = DB.poll_save(Question.Text,
						PollChoice1.Text,
						PollChoice2.Text,
						PollChoice3.Text,
						PollChoice4.Text,
						PollChoice5.Text,
						PollChoice6.Text,
						PollChoice7.Text,
						PollChoice8.Text,
						PollChoice9.Text);
				}

				string subject = Server.HtmlEncode(Subject.Text);
				TopicID = DB.topic_save(ForumID,subject,msg,PageUserID,Priority.SelectedValue,PollID,From.Text,Request.UserHostAddress,fmt,ref nMessageID);
			}

			SaveAttachment(nMessageID,File1);
			SaveAttachment(nMessageID,File2);
			SaveAttachment(nMessageID,File3);

			// Check if message is approved
			bool bApproved = false;
			using(DataTable dt = DB.message_list(nMessageID)) 
				foreach(DataRow row in dt.Rows) 
					bApproved = (bool)row["Approved"];

			// Create notification emails
			if(bApproved) 
			{
				Utils.CreateWatchEmail(this,nMessageID);
				Response.Redirect(String.Format("posts.aspx?m={0}&#{0}",nMessageID));
			} 
			else 
			{
				// Tell user that his message will have to be approved by a moderator
				//AddLoadMessage("Since you posted to a moderated forum, a forum moderator must approve your post before it will become visible.");
				string url = String.Format("topics.aspx?f={0}",PageForumID);
				Response.Redirect(String.Format("info.aspx?i=1&url={0}",Server.HtmlEncode(url)));
			}
		}

		private void CheckValidFile(HtmlInputFile file) 
		{
			if(file.PostedFile==null || file.PostedFile.FileName.Trim().Length==0 || file.PostedFile.ContentLength==0)
				return;

			string filename = file.PostedFile.FileName;
			int pos = filename.LastIndexOfAny(new char[]{'/','\\'});
			if(pos>=0)
				filename = filename.Substring(pos+1);
			pos = filename.LastIndexOf('.');
			if(pos>=0) 
			{
				switch(filename.Substring(pos+1).ToLower()) 
				{
					default:
						break;
					case "asp":
					case "aspx":
					case "ascx":
					case "config":
					case "php":
					case "php3":
					case "js":
					case "vb":
					case "vbs":
						throw new Exception(String.Format(GetText("fileerror"),filename));
				}
			}
		}

		private void SaveAttachment(long nMessageID,HtmlInputFile file) 
		{
			if(file.PostedFile==null || file.PostedFile.FileName.Trim().Length==0 || file.PostedFile.ContentLength==0)
				return;

			string sUpDir = Request.MapPath(System.Configuration.ConfigurationSettings.AppSettings["uploaddir"]);

			string filename = file.PostedFile.FileName;
			int pos = filename.LastIndexOfAny(new char[]{'/','\\'});
			if(pos>=0)
				filename = filename.Substring(pos+1);

			file.PostedFile.SaveAs(sUpDir + filename);
			DB.attachment_save(nMessageID,filename,file.PostedFile.ContentLength);
		}

		private void CreatePoll_Click(object sender, System.EventArgs e) {
			CreatePollRow.Visible = false;
			PollRow1.Visible = true;
			PollRow2.Visible = true;
			PollRow3.Visible = true;
			PollRow4.Visible = true;
			PollRow5.Visible = true;
			PollRow6.Visible = true;
			PollRow7.Visible = true;
			PollRow8.Visible = true;
			PollRow9.Visible = true;
			PollRow10.Visible = true;
		}

		private void Cancel_Click(object sender, System.EventArgs e) {
			Response.Redirect(String.Format("topics.aspx?f={0}",ForumID));
		}

		private void Preview_Click(object sender, System.EventArgs e) {
			PreviewRow.Visible = true;

			//string body = Server.HtmlEncode(Message.Text);
			string body = Message.Text;

			FormatMsg fmt = new FormatMsg(this);

			using(DataTable dt = DB.user_list(PageUserID,true)) 
			{
				if(!dt.Rows[0].IsNull("Signature"))
					body += "<br/><hr noshade/>" + fmt.FormatMessage(dt.Rows[0]["Signature"].ToString());
			}
			
			if(Message.IsRTEBrowser)
				PreviewCell.InnerHtml = body;
			else 
				PreviewCell.InnerHtml = fmt.FormatMessage(body);
		}

		protected string FormatBody(object o) 
		{
			if(fmt==null)
				fmt = new FormatMsg(this);

			DataRowView row = (DataRowView)o;
			string html = row["Message"].ToString();
			if(int.Parse(row["Format"].ToString())==(int)MSGFORMAT.FORUM)
				html = fmt.FormatMessage(html);

			if(row["Signature"].ToString().Length>0)
				html += "<br/><hr noshade/>" + fmt.FormatMessage(row["Signature"].ToString());

			return html;
		}
	}
}
