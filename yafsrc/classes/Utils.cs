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
using System.Data;
using System.Web;

namespace yaf
{
	public class User 
	{
		/// <summary>
		/// Can be called by other applications to validate a user. 
		/// </summary>
		/// <param name="username">The name of the user (can be the same as email).</param>
		/// <param name="email">The user's email address.</param>
		/// <returns>The UserID of the user, or 0 if validation failed.</returns>
		static public int ValidateUser(string username,string email) 
		{
			try 
			{
				return DB.user_extvalidate(username,email);
			}
			catch(Exception) 
			{
				return 0;
			}
		}
	}

	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils {
		static public ulong Str2IP(String[] ip) {
			if(ip.Length!=4)
				throw new Exception("Invalid ip address.");

			ulong num = 0;
			for(int i=0;i<ip.Length;i++) {
				num <<= 8;
				num |= ulong.Parse(ip[i]);
			}
			return num;
		}

		static public bool IsBanned(string ban,string chk) {
			String[] ipmask = ban.Split('.');
			String[] ip = ban.Split('.');
			for(int i=0;i<ipmask.Length;i++) {
				if(ipmask[i]=="*") {
					ipmask[i] = "0";
					ip[i] = "0";
				}
				else
					ipmask[i] = "255";
			}

			ulong banmask = Str2IP(ip);
			ulong banchk = Str2IP(ipmask);
			ulong ipchk = Str2IP(chk.Split('.'));
			
			return (ipchk & banchk) == banmask;
		}

		static public string Text2Html(string html) 
		{
			html = html.Replace("\n","<br/>");
			return html;
		}

		/// <summary>
		/// Send an error report by email. For this to work, 
		/// smtpserver and erroremail must be set in Web.config.
		/// </summary>
		/// <param name="x">The Exception object to report.</param>
		static public void ReportError(Exception x) 
		{
			try 
			{
				// Send email about the error
				string sErrorSmtp = Config.ConfigSection["smtpserver"];
				string sErrorEmail = Config.ConfigSection["erroremail"];
				if(sErrorEmail==null || sErrorEmail.Length==0 || sErrorSmtp==null || sErrorSmtp.Length==0)
					return;

				// Build body
				System.Text.StringBuilder msg = new System.Text.StringBuilder();
				msg.Append("<style>\n");
				msg.Append("body,td,th{font:8pt tahoma}\n");
				msg.Append("table{background-color:#C0C0C0}\n");
				msg.Append("th{font-weight:bold;text-align:left;background-color:#C0C0C0;padding:4px}\n");
				msg.Append("td{vertical-align:top;background-color:#FFFBF0;padding:4px}\n");
				msg.Append("</style>\n");
				msg.Append("<table cellpadding=1 cellspacing=1>\n");

				if(x!=null) 
				{
					msg.Append("<tr><th colspan=2>Exception</th></tr>");
					msg.AppendFormat("<tr><td>Message</td><td>{0}</td></tr>",Text2Html(x.Message));
					msg.AppendFormat("<tr><td>Source</td><td>{0}</td></tr>",Text2Html(x.Source));
					msg.AppendFormat("<tr><td>StackTrace</td><td>{0}</td></tr>",Text2Html(x.StackTrace));
					msg.AppendFormat("<tr><td>TargetSize</td><td>{0}</td></tr>",Text2Html(x.TargetSite.ToString()));
				}

				msg.Append("<tr><th colspan=2>QueryString</th></tr>");
				foreach(string key in HttpContext.Current.Request.QueryString.AllKeys) 
				{
					msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>",key,HttpContext.Current.Request.QueryString[key]);
				}
				msg.Append("<tr><th colspan=2>Form</th></tr>");
				foreach(string key in HttpContext.Current.Request.Form.AllKeys) 
				{
					msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>",key,HttpContext.Current.Request.Form[key]);
				}
				msg.Append("<tr><th colspan=2>ServerVariables</th></tr>");
				foreach(string key in HttpContext.Current.Request.ServerVariables.AllKeys)
				{
					msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>",key,HttpContext.Current.Request.ServerVariables[key]);
				}
				msg.Append("<tr><th colspan=2>Session</th></tr>");
				foreach(string key in HttpContext.Current.Session)
				{
					msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>",key,HttpContext.Current.Session[key]);
				}
				msg.Append("<tr><th colspan=2>Application</th></tr>");
				foreach(string key in HttpContext.Current.Application)
				{
					msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>",key,HttpContext.Current.Application[key]);
				}
				msg.Append("<tr><th colspan=2>Cookies</th></tr>");
				foreach(string key in HttpContext.Current.Request.Cookies.AllKeys)
				{
					msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>",key,HttpContext.Current.Request.Cookies[key].Value);
				}
				msg.Append("</table>");
#if  true
				// .NET
				System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
				mailMessage.From = sErrorEmail;
				mailMessage.To = sErrorEmail;
				mailMessage.Subject = "Yet Another Forum.net Error Report";
				mailMessage.BodyFormat = System.Web.Mail.MailFormat.Html;
				mailMessage.Body = msg.ToString();
				System.Web.Mail.SmtpMail.SmtpServer = sErrorSmtp;
				System.Web.Mail.SmtpMail.Send(mailMessage);
#else
			// http://sourceforge.net/projects/opensmtp-net/
			OpenSmtp.Mail.SmtpConfig.VerifyAddresses = false;
			OpenSmtp.Mail.MailMessage mailMessage = new OpenSmtp.Mail.MailMessage(sErrorEmail, sErrorEmail);
			OpenSmtp.Mail.Smtp smtp = new OpenSmtp.Mail.Smtp(sErrorSmtp,25);
			mailMessage.Subject = "Yet Another Forum.net Error Report";
			mailMessage.HtmlBody = msg.ToString();
			smtp.SendMail(mailMessage);
#endif	
			}
			catch(Exception) 
			{
			}
		}
		static public void CreateWatchEmail(yaf.pages.ForumPage basePage,object messageID) 
		{
			using(DataTable dt = DB.message_list(messageID)) 
			{
				foreach(DataRow row in dt.Rows) 
				{
					// Send track mails
					string subject = String.Format("Topic Subscription New Post Notification (From {0})",basePage.ForumName);

					string body = basePage.ReadTemplate("topicpost.txt");
					body = body.Replace("{forumname}",basePage.ForumName);
					body = body.Replace("{topic}",row["Topic"].ToString());
					body = body.Replace("{link}",String.Format("{0}{1}",basePage.ServerURL,Forum.GetLink(Pages.posts,"m={0}#{0}",messageID)));

					DB.mail_createwatch(row["TopicID"],basePage.ForumEmail,subject,body,row["UserID"]);
				}
			}
		}
	}
}
