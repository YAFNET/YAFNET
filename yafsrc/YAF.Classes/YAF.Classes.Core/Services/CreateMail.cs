/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net.Mail;
using System.Web;
using System.Data;
using System.Web.Security;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	public static class CreateMail
	{
		/// <summary>
		/// Send an error report by email. For this to work, 
		/// smtpserver and erroremail must be set in Web.config.
		/// </summary>
		/// <param name="x">The Exception object to report.</param>
		static public void CreateLogEmail( Exception x )
		{
			try
			{
				string config = YAF.Classes.Config.LogToMail;
				if ( config == null )
					return;

				// Find mail info
				string email = "";
				string server = "";
				string SmtpUserName = "";
				string SmtpPassword = "";

				foreach ( string part in config.Split( ';' ) )
				{
					string[] pair = part.Split( '=' );
					if ( pair.Length != 2 ) continue;

					switch ( pair[0].Trim().ToLower() )
					{
						case "email":
							email = pair[1].Trim();
							break;
						case "server":
							server = pair[1].Trim();
							break;
						case "user":
							SmtpUserName = pair[1].Trim();
							break;
						case "pass":
							SmtpPassword = pair[1].Trim();
							break;
					}
				}

				// Build body
				System.Text.StringBuilder msg = new System.Text.StringBuilder();
				msg.Append( "<style>\r\n" );
				msg.Append( "body,td,th{font:8pt tahoma}\r\n" );
				msg.Append( "table{background-color:#C0C0C0}\r\n" );
				msg.Append( "th{font-weight:bold;text-align:left;background-color:#C0C0C0;padding:4px}\r\n" );
				msg.Append( "td{vertical-align:top;background-color:#FFFBF0;padding:4px}\r\n" );
				msg.Append( "</style>\r\n" );
				msg.Append( "<table cellpadding=1 cellspacing=1>\r\n" );

				if ( x != null )
				{
					msg.Append( "<tr><th colspan=2>Exception</th></tr>" );
					msg.AppendFormat( "<tr><td>Exception</td><td><pre>{0}</pre></td></tr>", x );
					msg.AppendFormat( "<tr><td>Message</td><td>{0}</td></tr>", Text2Html( x.Message ) );
					msg.AppendFormat( "<tr><td>Source</td><td>{0}</td></tr>", Text2Html( x.Source ) );
					msg.AppendFormat( "<tr><td>StackTrace</td><td>{0}</td></tr>", Text2Html( x.StackTrace ) );
					msg.AppendFormat( "<tr><td>TargetSize</td><td>{0}</td></tr>", Text2Html( x.TargetSite.ToString() ) );
				}

				msg.Append( "<tr><th colspan=2>QueryString</th></tr>" );
				foreach ( string key in HttpContext.Current.Request.QueryString.AllKeys )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.QueryString[key] );
				}
				msg.Append( "<tr><th colspan=2>Form</th></tr>" );
				foreach ( string key in HttpContext.Current.Request.Form.AllKeys )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.Form[key] );
				}
				msg.Append( "<tr><th colspan=2>ServerVariables</th></tr>" );
				foreach ( string key in HttpContext.Current.Request.ServerVariables.AllKeys )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.ServerVariables[key] );
				}
				msg.Append( "<tr><th colspan=2>Session</th></tr>" );
				foreach ( string key in HttpContext.Current.Session )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Session[key] );
				}
				msg.Append( "<tr><th colspan=2>Application</th></tr>" );
				foreach ( string key in HttpContext.Current.Application )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Application[key] );
				}
				msg.Append( "<tr><th colspan=2>Cookies</th></tr>" );
				foreach ( string key in HttpContext.Current.Request.Cookies.AllKeys )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.Cookies[key].Value );
				}
				msg.Append( "</table>" );

				System.Net.Mail.SmtpClient smtpSend = new System.Net.Mail.SmtpClient( server );

				if ( SmtpUserName.Length > 0 && SmtpPassword.Length > 0 )
				{
					smtpSend.Credentials = new System.Net.NetworkCredential( SmtpUserName, SmtpPassword );
				}

				using ( System.Net.Mail.MailMessage emailMessage = new System.Net.Mail.MailMessage() )
				{
					emailMessage.To.Add( email );
					emailMessage.From = new System.Net.Mail.MailAddress( email );
					emailMessage.Subject = "Yet Another Forum.net Error Report";
					emailMessage.Body = msg.ToString();
					emailMessage.IsBodyHtml = true;

					smtpSend.Send( emailMessage );
				}
			}
			catch ( Exception )
			{
			}
		}

		static public string Text2Html( string html )
		{
			html = html.Replace( "\r\n", "<br/>" );
			html = html.Replace( "\n", "<br/>" );
			return html;
		}

		static public void WatchEmail( object messageID )
		{
			using ( DataTable dt = YAF.Classes.Data.DB.message_list( messageID ) )
			{
				foreach ( DataRow row in dt.Rows )
				{
					int userId = Convert.ToInt32( row["UserID"] );

					YafTemplateEmail watchEmail = new YafTemplateEmail( "TOPICPOST" );

					watchEmail.TemplateLanguageFile = UserHelper.GetUserLanguageFile( userId );

					// cleaned body as text...
					string bodyText = StringHelper.RemoveMultipleWhitespace( BBCodeHelper.StripBBCode( HtmlHelper.StripHtml( HtmlHelper.CleanHtmlString( row["Message"].ToString() ) ) ) );

					// Send track mails
					string subject = String.Format( YafContext.Current.Localization.GetText( "COMMON", "TOPIC_NOTIFICATION_SUBJECT" ), YafContext.Current.BoardSettings.Name );

					watchEmail.TemplateParams["{forumname}"] = YafContext.Current.BoardSettings.Name;
					watchEmail.TemplateParams["{topic}"] = row["Topic"].ToString();
					watchEmail.TemplateParams["{body}"] = bodyText;
					watchEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped( ForumPages.posts, true, "m={0}#post{0}",
					                                                                      messageID );

					watchEmail.CreateWatch( Convert.ToInt32( row["TopicID"] ), userId, new MailAddress( YafContext.Current.BoardSettings.ForumEmail, YafContext.Current.BoardSettings.Name ), subject );
				}
			}
		}

		/// <summary>
		/// Sends notification about new PM in user's inbox.
		/// </summary>
		/// <param name="toUserID">User supposed to receive notification about new PM.</param>
		/// <param name="subject">Subject of PM user is notified about.</param>
		static public void PmNotification( int toUserID, string subject )
		{
			try
			{
				// user's PM notification setting
				bool pmNotificationAllowed;
				// user's email
				string toEMail;

				// read user's info from DB
				using ( DataTable dt = DB.user_list( YafContext.Current.PageBoardID, toUserID, true ) )
				{
					pmNotificationAllowed = (bool)dt.Rows[0]["PMNotification"];
					toEMail = (string)dt.Rows[0]["EMail"];
				}

				if ( pmNotificationAllowed )
				{
					// user has PM notification set on

					int userPMessageID;
					//string senderEmail;

					// get the PM ID
					// Ederon : 11/21/2007 - PageBoardID as parameter of DB.pmessage_list?
					// using (DataTable dt = DB.pmessage_list(toUserID, PageContext.PageBoardID, null))
					using ( DataTable dt = DB.pmessage_list( toUserID, null, null ) )
						userPMessageID = (int)dt.Rows[0]["UserPMessageID"];

					// get the sender e-mail -- DISABLED: too much information...
					//using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
					//	senderEmail = ( string ) dt.Rows [0] ["Email"];

					// send this user a PM notification e-mail
					YafTemplateEmail pmNotification = new YafTemplateEmail( "PMNOTIFICATION" );

					pmNotification.TemplateLanguageFile = UserHelper.GetUserLanguageFile( toUserID );

					// fill the template with relevant info
					pmNotification.TemplateParams["{fromuser}"] = YafContext.Current.PageUserName;
					pmNotification.TemplateParams["{link}"] = String.Format( "{0}\r\n\r\n", YafBuildLink.GetLinkNotEscaped( ForumPages.cp_message, true, "pm={0}", userPMessageID ) );
					pmNotification.TemplateParams["{forumname}"] = YafContext.Current.BoardSettings.Name;
					pmNotification.TemplateParams["{subject}"] = subject;

					// create notification email subject
					string emailSubject = string.Format( YafContext.Current.Localization.GetText( "COMMON", "PM_NOTIFICATION_SUBJECT" ), YafContext.Current.PageUserName, YafContext.Current.BoardSettings.Name, subject );

					// send email
					pmNotification.SendEmail( new System.Net.Mail.MailAddress( toEMail ), subject, true );
				}
			}
			catch ( Exception x )
			{
				// report exception to the forum's event log
				DB.eventlog_create( YafContext.Current.PageUserID, "SendPmNotification", x );
				// tell user about failure
				YafContext.Current.AddLoadMessage( YafContext.Current.Localization.GetTextFormatted( "Failed", x.Message ) );
			}
		}
	}
}
