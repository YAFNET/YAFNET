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
using System.Drawing;
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
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.TextBox Message;
		protected System.Web.UI.WebControls.Label Label1;
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
		private int ForumID;

		private void Page_Load(object sender, System.EventArgs e)
		{
			ForumID = PageForumID;
			DataRow msg = null;
			
			if(Request.QueryString["q"] != null)
				msg = Data.GetMessageInfo(int.Parse(Request.QueryString["q"]));
			else if(Request.QueryString["m"] != null) {
				msg = Data.GetMessageInfo(int.Parse(Request.QueryString["m"]));
			
				if(!ForumModeratorAccess && PageUserID != (int)msg["UserID"])
					Response.Redirect(BaseDir);
			}
	
			if(ForumID == 0)
				Response.Redirect(BaseDir);

			if(!ForumPostAccess)
				Response.Redirect(BaseDir);

			if(!IsPostBack) 
			{
				PriorityRow.Visible = ForumPriorityAccess;
				CreatePollRow.Visible = Request.QueryString["t"]==null && ForumPollAccess;

				HomeLink.Text = ForumName;
				HomeLink.NavigateUrl = BaseDir;
				CategoryLink.Text = PageCategoryName;
				CategoryLink.NavigateUrl = String.Format("default.aspx?c={0}",PageCategoryID);

				if(Request.QueryString["t"] != null) 
				{
					DataRow topic = Data.TopicInfo(int.Parse(Request.QueryString["t"]));
					if((bool)topic["IsLocked"])
						Response.Redirect(Request.UrlReferrer.ToString());
					SubjectRow.Visible = false;
					Title.Text = "Post a reply";
				}

				if(Request.QueryString["q"] != null) {
					Message.Text = String.Format("[quote={0}]{1}[/quote]",msg["username"],Server.HtmlDecode((string)msg["message"]));
				} else if(Request.QueryString["m"] != null) {
					Message.Text = Server.HtmlDecode((string)msg["message"]);
					Subject.Text = (string)msg["Topic"];
					Subject.Enabled = false;
					CreatePollRow.Visible = false;
					Priority.SelectedItem.Selected = false;
					Priority.Items.FindByValue(msg["Priority"].ToString()).Selected = true;
				}

				From.Text = (string)pageinfo["username"];
				if(User.Identity.IsAuthenticated)
					FromRow.Visible = false;
			}

			DataRow forum = Data.ForumInfo(ForumID);
			ForumLink.Text = (string)forum["Name"];
			ForumLink.NavigateUrl = "topics.aspx?f=" + ForumID.ToString();

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
				AddLoadMessage("Please enter a subject of the message.");
				return;
			}

			// Must wait 30 seconds before posting again
			if(Session["lastpost"] != null) {
				DateTime lastpost = DateTime.Parse(Session["lastpost"].ToString());
				lastpost += TimeSpan.FromSeconds(30);
				if(lastpost > DateTime.Now) {
					AddLoadMessage(String.Format("You can't post in the next {0:N0} seconds.",(lastpost - DateTime.Now).Seconds));
					return;
				}
			}

			int TopicID;
			string msg = Server.HtmlEncode(Message.Text);
			Session["lastpost"] = DateTime.Now;
			if(Request.QueryString["t"] != null) {
				if(!ForumReplyAccess)
					return;

				TopicID = int.Parse(Request.QueryString["t"]);
				if(!Data.PostReply(TopicID,User.Identity.Name,msg,From.Text,Request.UserHostAddress))
					TopicID = 0;
			} 
			else if(Request.QueryString["m"] != null) {
				using(SqlCommand cmd = new SqlCommand("yaf_message_update")) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@MessageID",Request.QueryString["m"]);
					cmd.Parameters.Add("@Priority",Priority.SelectedItem.Value);
					cmd.Parameters.Add("@Message",Message.Text);
					DataManager.ExecuteNonQuery(cmd);
				}
				TopicID = PageTopicID;
			} 
			else {
				if(!ForumPostAccess)
					return;

				int PollID = 0;
				if(PollRow1.Visible) {
					using(SqlCommand cmd = new SqlCommand("yaf_poll_save")) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@Question",Question.Text);
						cmd.Parameters.Add("@Choice1",PollChoice1.Text);
						cmd.Parameters.Add("@Choice2",PollChoice2.Text);
						cmd.Parameters.Add("@Choice3",PollChoice3.Text);
						cmd.Parameters.Add("@Choice4",PollChoice4.Text);
						cmd.Parameters.Add("@Choice5",PollChoice5.Text);
						cmd.Parameters.Add("@Choice6",PollChoice6.Text);
						cmd.Parameters.Add("@Choice7",PollChoice7.Text);
						cmd.Parameters.Add("@Choice8",PollChoice8.Text);
						cmd.Parameters.Add("@Choice9",PollChoice9.Text);
						PollID = (int)DataManager.ExecuteScalar(cmd);
					}
				}

				string subject = Server.HtmlEncode(Subject.Text);
				TopicID = Data.PostMessage(ForumID,subject,msg,User.Identity.Name,int.Parse(Priority.SelectedItem.Value),PollID,From.Text,Request.UserHostAddress);
			}
			if(TopicID>0) {
				// Get Topic Info
				DataRow topic = Data.TopicInfo(TopicID);

				// Send track mails
				using(SqlCommand cmd = new SqlCommand("yaf_mail_createwatch")) {
					string subject = String.Format("Topic Subscription New Post Notification (From {0})",ForumName);

					string body = ReadTemplate("topicpost.txt");
					body = body.Replace("{forumname}",ForumName);
					body = body.Replace("{topic}",(string)topic["Topic"]);
					body = body.Replace("{link}",String.Format("http://{0}{1}posts.aspx?t={2}",Request.ServerVariables["SERVER_NAME"],BaseDir,TopicID));

					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@TopicID",TopicID);
					cmd.Parameters.Add("@From",ForumEmail);
					cmd.Parameters.Add("@Subject",subject);
					cmd.Parameters.Add("@Body",body);
					DataManager.ExecuteNonQuery(cmd);
				}

				Response.Redirect("posts.aspx?t=" + TopicID);
			}
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

			string body = Server.HtmlEncode(Message.Text);

			using(SqlCommand cmd = new SqlCommand("yaf_user_list")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",PageUserID);
				DataTable dt = DataManager.GetData(cmd);
				if(!dt.Rows[0].IsNull("Signature"))
					body += "\r\n\r\n-- \r\n" + dt.Rows[0]["Signature"].ToString();
			}
			
			FormatMsg fmt = new FormatMsg(this);
			PreviewCell.InnerHtml = fmt.FormatMessage(this,body);
		}

	}
}
