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
using System.IO;
using System.Text;
using System.Web;

namespace yaf
{
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

		/// <summary>
		/// Reads a template from the templates directory
		/// </summary>
		/// <param name="name">Name of template (not including path)</param>
		/// <returns>The template</returns>
		static public string ReadTemplate(string name) 
		{
			string file;
			if(HttpContext.Current.Cache[name] != null && false) 
			{
				file = HttpContext.Current.Cache[name].ToString();
			} 
			else 
			{
				string templatefile = HttpContext.Current.Server.MapPath(String.Format("{0}templates/{1}",Data.ForumRoot,name));
				StreamReader sr = new StreamReader(templatefile,Encoding.ASCII);
				file = sr.ReadToEnd();
				sr.Close();
				HttpContext.Current.Cache[name] = file;
			}
			return file;
		}

		static public void SendMail(string from,string to,string subject,string body) 
		{
			// http://sourceforge.net/projects/opensmtp-net/
			OpenSmtp.Mail.SmtpConfig.VerifyAddresses = false;

			OpenSmtp.Mail.Smtp smtp = new OpenSmtp.Mail.Smtp(Config.BoardSettings.SmtpServer,25);
			if(Config.BoardSettings.SmtpUserName!=null && Config.BoardSettings.SmtpUserPass!=null) 
			{
				smtp.Username = Config.BoardSettings.SmtpUserName;
				smtp.Password = Config.BoardSettings.SmtpUserPass;
			}
			smtp.SendMail(from,to,subject,body);
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
		static public void LogToMail(Exception x) 
		{
			try 
			{
				string	config	= Config.ConfigSection["logtomail"];
				if(config==null)
					return;

				// Find mail info
				string	email	= "";
				string	server	= "";
				string	user	= "";
				string	pass	= "";

				foreach(string part in config.Split(';'))
				{
					string[] pair = part.Split('=');
					if(pair.Length!=2) continue;

					switch(pair[0].Trim().ToLower())
					{
						case "email":
							email = pair[1].Trim();
							break;
						case "server":
							server = pair[1].Trim();
							break;
						case "user":
							user = pair[1].Trim();
							break;
						case "pass":
							pass = pair[1].Trim();
							break;
					}
				}


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
				// Send mail
				// http://sourceforge.net/projects/opensmtp-net/
				OpenSmtp.Mail.SmtpConfig.VerifyAddresses = false;
				OpenSmtp.Mail.MailMessage mailMessage = new OpenSmtp.Mail.MailMessage(email, email);
				OpenSmtp.Mail.Smtp smtp = new OpenSmtp.Mail.Smtp(server,25);
				if(user.Length>0 && pass.Length>0) 
				{
					smtp.Username = user;
					smtp.Password = pass;
				}
				mailMessage.Subject = "Yet Another Forum.net Error Report";
				mailMessage.HtmlBody = msg.ToString();
				smtp.SendMail(mailMessage);
			}
			catch(Exception) 
			{
			}
		}
		static public void CreateWatchEmail(PageInfo basePage,object messageID) 
		{
			using(DataTable dt = DB.message_list(messageID)) 
			{
				foreach(DataRow row in dt.Rows) 
				{
					// Send track mails
					string subject = String.Format("Topic Subscription New Post Notification (From {0})",Config.BoardSettings.Name);

					string body = Utils.ReadTemplate("topicpost.txt");
					body = body.Replace("{forumname}",Config.BoardSettings.Name);
					body = body.Replace("{topic}",row["Topic"].ToString());
					body = body.Replace("{link}",String.Format("{0}{1}",basePage.ServerURL,Forum.GetLink(Pages.posts,"m={0}#{0}",messageID)));

					DB.mail_createwatch(row["TopicID"],Config.BoardSettings.ForumEmail,subject,body,row["UserID"]);
				}
			}
		}
	}
}
