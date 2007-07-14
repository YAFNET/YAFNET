/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Summary description for General Utils.
	/// </summary>
	public static class General
	{
		static public ulong Str2IP( String [] ip )
		{
			if ( ip.Length != 4 )
				throw new Exception( "Invalid ip address." );

			ulong num = 0;
			for ( int i = 0; i < ip.Length; i++ )
			{
				num <<= 8;
				num |= ulong.Parse( ip [i] );
			}
			return num;
		}

		static public ulong IPStrToLong( string IPAddress )
		{
			string [] ip = IPAddress.Split( '.' );
			return Str2IP( ip );
		}

		static public bool IsBanned( string ban, string chk )
		{
			String [] ipmask = ban.Split( '.' );
			String [] ip = ban.Split( '.' );
			for ( int i = 0; i < ipmask.Length; i++ )
			{
				if ( ipmask [i] == "*" )
				{
					ipmask [i] = "0";
					ip [i] = "0";
				}
				else
					ipmask [i] = "255";
			}

			ulong banmask = Str2IP( ip );
			ulong banchk = Str2IP( ipmask );
			ulong ipchk = Str2IP( chk.Split( '.' ) );

			return ( ipchk & banchk ) == banmask;
		}

		static public string GetSafeRawUrl()
		{
			string tProcessedRaw = System.Web.HttpContext.Current.Request.RawUrl;
			tProcessedRaw = tProcessedRaw.Replace( "\"", string.Empty );
			tProcessedRaw = tProcessedRaw.Replace( "<", "%3C" );
			tProcessedRaw = tProcessedRaw.Replace( ">", "%3E" );
			tProcessedRaw = tProcessedRaw.Replace( "&", "%26" );
			return tProcessedRaw.Replace( "'", string.Empty );
		}

		/// <summary>
		/// Reads a template from the templates directory
		/// </summary>
		/// <param name="name">Name of template (not including path)</param>
		/// <returns>The template</returns>
		static public string ReadTemplate( string name )
		{
			string file;

			if ( HttpContext.Current.Cache [name] != null )
			{
				file = HttpContext.Current.Cache [name].ToString();
			}
			else
			{
				string templatefile = HttpContext.Current.Server.MapPath( String.Format( "{0}templates/{1}", yaf_ForumInfo.ForumRoot, name ) );
				StreamReader sr = new StreamReader( templatefile, Encoding.ASCII );
				file = sr.ReadToEnd();
				sr.Close();
				HttpContext.Current.Cache [name] = file;
			}

			return file;
		}

		static public string CreateEmailFromTemplate( string templateName, ref StringDictionary templateParameters )
		{
			string email = ReadTemplate( templateName );

			foreach ( string key in templateParameters.Keys )
			{
				email = email.Replace( key, templateParameters [key] );
			}

			return email;
		}

		static public void SendMail( string fromEmail, string toEmail, string subject, string body )
		{
			SendMail( new System.Net.Mail.MailAddress( fromEmail ), new System.Net.Mail.MailAddress( toEmail ), subject, body );
		}

		static public void SendMail( string fromEmail, string fromName, string toEmail, string toName, string subject, string body )
		{
			SendMail( new System.Net.Mail.MailAddress( fromEmail, fromName ), new System.Net.Mail.MailAddress( toEmail, toName ), subject, body );
		}

		static public void SendMail( System.Net.Mail.MailAddress fromAddress, System.Net.Mail.MailAddress toAddress, string subject, string body )
		{
			System.Net.Mail.SmtpClient smtpSend = new System.Net.Mail.SmtpClient( yaf_Context.Current.BoardSettings.SmtpServer );

			if ( yaf_Context.Current.BoardSettings.SmtpUserName != null && yaf_Context.Current.BoardSettings.SmtpUserPass != null )
			{
				smtpSend.Credentials = new System.Net.NetworkCredential( yaf_Context.Current.BoardSettings.SmtpUserName, yaf_Context.Current.BoardSettings.SmtpUserPass );
			}

			using ( System.Net.Mail.MailMessage emailMessage = new System.Net.Mail.MailMessage() )
			{
				emailMessage.To.Add( toAddress );
				emailMessage.From = fromAddress;
				emailMessage.Subject = subject;
				emailMessage.Body = body;

				if ( !Regex.IsMatch( emailMessage.Body, @"^([0-9a-z!@#\$\%\^&\*\(\)\-=_\+])", RegexOptions.IgnoreCase ) ||
						!Regex.IsMatch( emailMessage.Subject, @"^([0-9a-z!@#\$\%\^&\*\(\)\-=_\+])", RegexOptions.IgnoreCase ) )
				{
					emailMessage.BodyEncoding = Encoding.UTF8;
				}

				smtpSend.Send( emailMessage );
			}
		}

		static public string Text2Html( string html )
		{
			html = html.Replace( "\r\n", "<br/>" );
			html = html.Replace( "\n", "<br/>" );
			return html;
		}

		/// <summary>
		/// Send an error report by email. For this to work, 
		/// smtpserver and erroremail must be set in Web.config.
		/// </summary>
		/// <param name="x">The Exception object to report.</param>
		static public void LogToMail( Exception x )
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
					string [] pair = part.Split( '=' );
					if ( pair.Length != 2 ) continue;

					switch ( pair [0].Trim().ToLower() )
					{
						case "email":
							email = pair [1].Trim();
							break;
						case "server":
							server = pair [1].Trim();
							break;
						case "user":
							SmtpUserName = pair [1].Trim();
							break;
						case "pass":
							SmtpPassword = pair [1].Trim();
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
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.QueryString [key] );
				}
				msg.Append( "<tr><th colspan=2>Form</th></tr>" );
				foreach ( string key in HttpContext.Current.Request.Form.AllKeys )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.Form [key] );
				}
				msg.Append( "<tr><th colspan=2>ServerVariables</th></tr>" );
				foreach ( string key in HttpContext.Current.Request.ServerVariables.AllKeys )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.ServerVariables [key] );
				}
				msg.Append( "<tr><th colspan=2>Session</th></tr>" );
				foreach ( string key in HttpContext.Current.Session )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Session [key] );
				}
				msg.Append( "<tr><th colspan=2>Application</th></tr>" );
				foreach ( string key in HttpContext.Current.Application )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Application [key] );
				}
				msg.Append( "<tr><th colspan=2>Cookies</th></tr>" );
				foreach ( string key in HttpContext.Current.Request.Cookies.AllKeys )
				{
					msg.AppendFormat( "<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.Cookies [key].Value );
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
					emailMessage.From = new System.Net.Mail.MailAddress(email);
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
		static public void CreateWatchEmail( object messageID )
		{
			using ( DataTable dt = YAF.Classes.Data.DB.message_list( messageID ) )
			{
				foreach ( DataRow row in dt.Rows )
				{
					// Send track mails
					string subject = String.Format( yaf_Context.Current.Localization.GetText( "COMMON", "TOPIC_NOTIFICATION_SUBJECT" ), yaf_Context.Current.BoardSettings.Name );

					StringDictionary emailParams = new StringDictionary();

					emailParams["{forumname}"] = yaf_Context.Current.BoardSettings.Name;
					emailParams["{topic}"] = row ["Topic"].ToString();
					emailParams ["{link}"] = String.Format( "{0}{1}", yaf_ForumInfo.ServerURL, yaf_BuildLink.GetLink( ForumPages.posts, "m={0}#{0}", messageID ) );

					string body = General.CreateEmailFromTemplate( "topicpost.txt", ref emailParams );

					YAF.Classes.Data.DB.mail_createwatch( row ["TopicID"], yaf_Context.Current.BoardSettings.ForumEmail, subject, body, row ["UserID"] );
				}
			}
		}
		static public bool IsValidEmail( string email )
		{
			return Regex.IsMatch( email, @"^([0-9a-z]+[-._+&])*[0-9a-z]+@([-0-9a-z]+[.])+[a-z]{2,6}$", RegexOptions.IgnoreCase );
		}
		static public bool IsValidURL( string url )
		{
			return Regex.IsMatch( url, @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&%\$#\=~])*[^\.\,\)\(\s]$" );
		}
		static public bool IsValidInt( string val )
		{
			return Regex.IsMatch( val, @"^[1-9]\d*\.?[0]*$" );
		}
		/// <summary>
		/// Searches through SearchText and replaces "bad words" with "good words"
		/// as defined in the database.
		/// </summary>
		/// <param name="SearchText">The string to search through.</param>
		static public string BadWordReplace( string SearchText )
		{
			string strReturn = SearchText;
			RegexOptions options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;

			// rico : run word replacement from database table names yaf_replacewords
			DataTable dt = ( DataTable ) HttpContext.Current.Cache ["replacewords"];
			if ( dt == null )
			{
				dt = YAF.Classes.Data.DB.replace_words_list();
				HttpContext.Current.Cache.Insert( "replacewords", dt, null, DateTime.Now.AddMinutes( 15 ), TimeSpan.Zero );
			}
			foreach ( DataRow rwords in dt.Rows )
			{
				try
				{
					strReturn = Regex.Replace( strReturn, Convert.ToString( rwords ["badword"] ), Convert.ToString( rwords ["goodword"] ), options );
				}
#if DEBUG
				catch ( Exception e )
				{
					throw new Exception( "Regular Expression Failed: " + e.Message );
				}
#else
				catch (Exception x)
				{
					YAF.Classes.Data.DB.eventlog_create(null,"BadWordReplace",x,EventLogTypes.Warning);
				}
#endif
			}

			return strReturn;
		}

		static public string traceResources()
		{
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();

			// get a list of resource names from the manifest
			string [] resNames = a.GetManifestResourceNames();

			// populate the textbox with information about our resources
			// also look for images and put them in our arraylist
			string txtInfo = "";

			txtInfo += String.Format( "Found {0} resources\r\n", resNames.Length );
			txtInfo += "----------\r\n";
			foreach ( string s in resNames )
			{
				txtInfo += s + "\r\n";
			}
			txtInfo += "----------\r\n";

			return txtInfo;
		}

		/* Ederon - 7/1/2007 start */

		static public void AddStyleAttributeSize(System.Web.UI.WebControls.WebControl control, string width, string height)
		{
			control.Attributes.Add("style", String.Format("width: {0}; height: {1};", width, height));
		}

		static public void AddStyleAttributeWidth(System.Web.UI.WebControls.WebControl control, string width)
		{
			control.Attributes.Add("style", String.Format("width: {0};", width));
		}

		static public void AddStyleAttributeHeight(System.Web.UI.WebControls.WebControl control, string height)
		{
			control.Attributes.Add("style", String.Format("height: {0};", height));
		}

		/* Ederon - 7/1/2007 end */
	}
}
