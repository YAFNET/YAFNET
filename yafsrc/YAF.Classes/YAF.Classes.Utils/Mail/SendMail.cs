/* Yet Another Forum.net
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Threading;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Functions to send email via SMTP
	/// </summary>
	public static class SendMail
	{
		static public void Send( string fromEmail, string toEmail, string subject, string body )
		{
			Send( new System.Net.Mail.MailAddress( fromEmail ), new System.Net.Mail.MailAddress( toEmail ), subject, body );
		}
		static public void Send( string fromEmail, string fromName, string toEmail, string toName, string subject, string body )
		{
			Send( new System.Net.Mail.MailAddress( fromEmail, fromName ), new System.Net.Mail.MailAddress( toEmail, toName ), subject, body );
		}
		static public void Send( System.Net.Mail.MailAddress fromAddress, System.Net.Mail.MailAddress toAddress, string subject, string bodyText )
		{
			Send( fromAddress, toAddress, subject, bodyText, null );
		}
		static public void Send( System.Net.Mail.MailAddress fromAddress, System.Net.Mail.MailAddress toAddress, string subject, string bodyText, string bodyHtml )
		{
			System.Net.Mail.SmtpClient smtpSend = new System.Net.Mail.SmtpClient( YafContext.Current.BoardSettings.SmtpServer );

			if ( YafContext.Current.BoardSettings.SmtpUserName != null && YafContext.Current.BoardSettings.SmtpUserPass != null )
			{
				smtpSend.Credentials = new System.Net.NetworkCredential( YafContext.Current.BoardSettings.SmtpUserName, YafContext.Current.BoardSettings.SmtpUserPass );
			}

			// Ederon : 9/9/2007 - added port setting
			if ( YafContext.Current.BoardSettings.SmtpServerPort != null && General.IsValidInt( YafContext.Current.BoardSettings.SmtpServerPort ) )
			{
				smtpSend.Port = int.Parse( YafContext.Current.BoardSettings.SmtpServerPort );
			}

			// Ederon : 9/9/2007 - added SSL support through setting
			smtpSend.EnableSsl = YafContext.Current.BoardSettings.SmtpServerSsl;

			using ( System.Net.Mail.MailMessage emailMessage = new System.Net.Mail.MailMessage() )
			{
				emailMessage.To.Add( toAddress );
				emailMessage.From = fromAddress;
				emailMessage.Subject = subject;

				Encoding textEncoding = Encoding.UTF8;

				if ( !Regex.IsMatch( bodyText, @"^([0-9a-z!@#\$\%\^&\*\(\)\-=_\+])", RegexOptions.IgnoreCase ) ||
						!Regex.IsMatch( subject, @"^([0-9a-z!@#\$\%\^&\*\(\)\-=_\+])", RegexOptions.IgnoreCase ) )
				{
					textEncoding = Encoding.Unicode;
				}

				// add text view...
				emailMessage.AlternateViews.Add( System.Net.Mail.AlternateView.CreateAlternateViewFromString( bodyText, textEncoding, "text/plain" ) );

				// see if html alternative is also desired...
				if ( !String.IsNullOrEmpty( bodyHtml ) )
				{
					emailMessage.AlternateViews.Add( System.Net.Mail.AlternateView.CreateAlternateViewFromString( bodyHtml, Encoding.UTF8, "text/html" ) );
				}

				smtpSend.Send( emailMessage );
			}
		}
	}

	/// <summary>
	/// This class needs a little tweaking, but it works.
	/// No sure how it works when the DB isn't availabe.
	/// </summary>
	public class SendMailThread
	{
		Thread _mailThread = null;
		System.Web.HttpApplication _appContext;

		bool IsRunning
		{
			get
			{
				if ( _mailThread != null && _mailThread.IsAlive )
				{
					return true;
				}

				return false;
			}
		}

		public SendMailThread( System.Web.HttpApplication context )
		{
			if ( _mailThread == null )
			{
				_mailThread = new Thread( new ThreadStart( SendMailThreaded ) );
				_mailThread.Priority = ThreadPriority.BelowNormal;
			}

			_appContext = context;
		}

		public void StartThread()
		{
			if ( _mailThread != null && !_mailThread.IsAlive )
			{
				_mailThread.Start();
			}
		}

		public void StopThread()
		{
			if ( _mailThread != null && _mailThread.IsAlive )
			{
				_mailThread.Abort();
			}
		}

		protected void SendMailThreaded()
		{
			HttpContext.Current = _appContext.Context;

			while ( Thread.CurrentThread.ThreadState != ThreadState.Aborted )
			{
				// wait 10 seconds and start the e-mailing thread again...
				Thread.Sleep( 10000 );

				if ( Thread.CurrentThread.ThreadState == ThreadState.Aborted ) break;

				if ( HttpContext.Current == null )
				{
					// abort this thread immediately...
					break;
				}

				try
				{
					using ( DataTable dt = YAF.Classes.Data.DB.mail_list( Thread.CurrentThread.GetHashCode() ) )
					{
						for ( int i = 0; i < dt.Rows.Count; i++ )
						{
							string toEmail = dt.Rows [i] ["ToUser"].ToString().Trim();
							string fromEmail = dt.Rows [i] ["FromUser"].ToString().Trim();

							bool deleteEmail = true;

							// Build a MailMessage
							if ( !String.IsNullOrEmpty( fromEmail ) && !String.IsNullOrEmpty( toEmail ) )
							{
								System.Net.Mail.MailAddress fromEmailAddress, toEmailAddress;

								if ( !DbStringIsNullOrEmpty( dt.Rows [i] ["FromUserName"] ) )
								{
									// use display name from db
									fromEmailAddress = new System.Net.Mail.MailAddress( fromEmail, dt.Rows [i] ["FromUserName"].ToString().Trim() );
								}
								else
								{
									// no from display name
									fromEmailAddress = new System.Net.Mail.MailAddress( fromEmail );
								}

								// create the TO email address...
								if ( !DbStringIsNullOrEmpty( dt.Rows [i] ["ToUserName"] ) )
								{
									toEmailAddress = new System.Net.Mail.MailAddress( toEmail, dt.Rows [i] ["ToUserName"].ToString().Trim() );
								}
								else
								{
									toEmailAddress = new System.Net.Mail.MailAddress( toEmail );
								}

								string subject = dt.Rows [i] ["Subject"].ToString();
								string textBody = dt.Rows [i] ["Body"].ToString();
								string htmlBody = dt.Rows [i] ["BodyHtml"].ToString();

								try
								{
									// send the email message now...
									SendMail.Send( fromEmailAddress, toEmailAddress, subject, textBody, htmlBody );
								}
								catch ( System.Net.Mail.SmtpFailedRecipientException )
								{
									// only try maximum of 5 times...
									if ( Convert.ToInt32( dt.Rows [i] ["SendTries"] ) < 5 ) deleteEmail = false;
								}
								catch ( System.Net.Mail.SmtpException )
								{
									// only try maximum of 5 times...
									if ( Convert.ToInt32( dt.Rows [i] ["SendTries"] ) < 5 ) deleteEmail = false;
								}
								catch ( Exception x )
								{
									throw x;
								}
							}

							// if all is well, delete this message...
							if ( deleteEmail ) YAF.Classes.Data.DB.mail_delete( dt.Rows [i] ["MailID"] );
						}
					}
				}
				catch ( Exception x )
				{
					// log the error...
					YAF.Classes.Data.DB.eventlog_create( 1, "SendMailThread", x );
				}
			}
		}

		protected bool DbStringIsNullOrEmpty( object dbString )
		{
			if ( dbString == DBNull.Value ) return true;
			if ( String.IsNullOrEmpty( dbString.ToString().Trim() ) ) return true;
			return false;
		}
	}
}
