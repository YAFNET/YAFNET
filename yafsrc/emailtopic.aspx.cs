/* Copyright (C) 2003 Bjørnar Henden
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
	/// Summary description for emailtopic.
	/// </summary>
	public class emailtopic : BasePage
	{
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink CategoryLink;
		protected System.Web.UI.WebControls.HyperLink ForumLink;
		protected System.Web.UI.WebControls.HyperLink TopicLink;
		protected System.Web.UI.WebControls.TextBox EmailAddress;
		protected System.Web.UI.WebControls.TextBox Subject;
		protected System.Web.UI.WebControls.TextBox Message;
		protected System.Web.UI.WebControls.Button SendEmail;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.QueryString["t"] == null || !ForumReadAccess)
				Response.Redirect(BaseDir);

			if(!IsPostBack) {
				HomeLink.Text = ForumName;
				HomeLink.NavigateUrl = BaseDir;
				CategoryLink.Text = PageCategoryName;
				CategoryLink.NavigateUrl = String.Format("default.aspx?c={0}",PageCategoryID);
				ForumLink.Text = PageForumName;
				ForumLink.NavigateUrl = String.Format("topics.aspx?f={0}",PageForumID);
				TopicLink.Text = PageTopicName;
				TopicLink.NavigateUrl = String.Format("posts.aspx?t={0}",PageTopicID);

				Subject.Text = PageTopicName;
				Message.Text = String.Format("You might be interested in reading this:\r\nhttp://{0}{1}posts.aspx?t={2}\r\n\r\nFrom,\r\n\r\n{3}",Request.ServerVariables["SERVER_NAME"],BaseDir,PageTopicID,User.Identity.Name);
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
			this.SendEmail.Click += new System.EventHandler(this.SendEmail_Click);
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		private void SendEmail_Click(object sender, System.EventArgs e) {
			if(EmailAddress.Text.Length==0) {
				AddLoadMessage("You must enter a email address.");
				return;
			}

			try {
				string senderemail;
				using(SqlCommand cmd = new SqlCommand("yaf_user_list")) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@UserID",PageUserID);
					DataTable dt = DataManager.GetData(cmd);
					senderemail = (string)dt.Rows[0]["Email"];
				}

				//  Build a MailMessage
				System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
				mailMessage.From = senderemail;
				mailMessage.To = EmailAddress.Text;
				mailMessage.Subject = Subject.Text;
				mailMessage.BodyFormat = System.Web.Mail.MailFormat.Text;
				mailMessage.Body = Message.Text;

				System.Web.Mail.SmtpMail.SmtpServer = SmtpServer;
				System.Web.Mail.SmtpMail.Send(mailMessage);
				Response.Redirect(String.Format("posts.aspx?t={0}",PageTopicID));
			}
			catch(Exception x) {
				AddLoadMessage(String.Format("Failed to send email.\n\n{0}",x.Message));
			}
		}
	}
}
