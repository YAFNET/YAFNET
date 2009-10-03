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
using System.Data;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	/// <summary>
	/// Functions to send email via SMTP
	/// </summary>
	public class YafSendMail
	{
		/// <summary>
		/// Queues an e-mail messagage for asynchronous delivery
		/// </summary>
		/// <param name="fromEmail"></param>
		/// <param name="toEmail"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		public void Queue( string fromEmail, string toEmail, string subject, string body )
		{
			YAF.Classes.Data.DB.mail_create( fromEmail, null, toEmail, null, subject, body, null );
		}

		public void Send( string fromEmail, string toEmail, string subject, string body )
		{
			System.Diagnostics.Debug.Assert( !string.IsNullOrEmpty( toEmail ) );
			System.Diagnostics.Debug.Assert( !string.IsNullOrEmpty( fromEmail ) );
			Send( new System.Net.Mail.MailAddress( fromEmail ), new System.Net.Mail.MailAddress( toEmail ), subject, body );
		}

		public void Send( string fromEmail, string fromName, string toEmail, string toName, string subject, string body )
		{
			Send( new System.Net.Mail.MailAddress( fromEmail, fromName ), new System.Net.Mail.MailAddress( toEmail, toName ), subject, body );
		}

		public void Send( System.Net.Mail.MailAddress fromAddress, System.Net.Mail.MailAddress toAddress, string subject, string bodyText )
		{
			Send( fromAddress, toAddress, subject, bodyText, null );
		}

		public void Send( System.Net.Mail.MailAddress fromAddress, System.Net.Mail.MailAddress toAddress, string subject, string bodyText, string bodyHtml )
		{
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

				// Wes : Changed to use settings from configuration file's standard <smtp> section. 
				// Reason for change: The host smtp settings are redundant and
				// using them here couples this method to YafCache, which is dependant on a current HttpContext. 
				// Configuration settings are cached automatically.
				System.Net.Mail.SmtpClient smtpSend = new System.Net.Mail.SmtpClient();
				smtpSend.EnableSsl = Config.UseSMTPSSL;
				smtpSend.Send( emailMessage );
			}
		}
	}

	/// <summary>
	/// Separate class since SendThreaded isn't needed functionality
	/// for any instance except the HttpModule instance.
	/// </summary>
	public class YafSendMailThreaded : YafSendMail
	{
		public void SendThreaded(int uniqueId)
		{
			try
			{
				System.Diagnostics.Debug.WriteLine("Retrieving queued mail...");
				Thread.BeginCriticalRegion();

				using (DataTable dt = YAF.Classes.Data.DB.mail_list(uniqueId))
				{
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						string toEmail = dt.Rows[i]["ToUser"].ToString().Trim();
						string fromEmail = dt.Rows[i]["FromUser"].ToString().Trim();

						bool deleteEmail = true;

						// Build a MailMessage
						if (!String.IsNullOrEmpty(fromEmail) && !String.IsNullOrEmpty(toEmail))
						{
							System.Net.Mail.MailAddress toEmailAddress;

							MailAddress fromEmailAddress = !TypeHelper.DbStringIsNullOrEmpty(dt.Rows[i]["FromUserName"]) ? new System.Net.Mail.MailAddress(fromEmail, dt.Rows[i]["FromUserName"].ToString().Trim()) : new System.Net.Mail.MailAddress(fromEmail);

							// create the TO email address...
							if (!TypeHelper.DbStringIsNullOrEmpty(dt.Rows[i]["ToUserName"]))
							{
								toEmailAddress = new System.Net.Mail.MailAddress(toEmail, dt.Rows[i]["ToUserName"].ToString().Trim());
							}
							else
							{
								toEmailAddress = new System.Net.Mail.MailAddress(toEmail);
							}

							string subject = dt.Rows[i]["Subject"].ToString();
							string textBody = dt.Rows[i]["Body"].ToString();
							string htmlBody = dt.Rows[i]["BodyHtml"].ToString();

							try
							{
								// send the email message now...
								System.Diagnostics.Debug.WriteLine("Sending");
								Send(fromEmailAddress, toEmailAddress, subject, textBody, htmlBody);
								System.Diagnostics.Debug.WriteLine("Sent");
							}
							// Mek: Removed as possibly redundant
							//catch (System.Net.Mail.SmtpFailedRecipientException)
							//{
							//    // only try maximum of 5 times...
							//    if (Convert.ToInt32(dt.Rows[i]["SendTries"]) < 5) deleteEmail = false;
							//}
							catch (System.Net.Mail.SmtpException x)
							{
								// only try maximum of 5 times...
								if (Convert.ToInt32(dt.Rows[i]["SendTries"]) < 5)
									deleteEmail = false;
								else
									YAF.Classes.Data.DB.eventlog_create(1, "SendMailThread", x);
							}
						}

						// if all is well, delete this message...
						if (deleteEmail) YAF.Classes.Data.DB.mail_delete(dt.Rows[i]["MailID"]);
					}
				}
			}
			catch (Exception x)
			{
				// log the error...
				//YAF.Classes.Data.DB.eventlog_create(1, "SendMailThread", x);
			}
			finally
			{
				Thread.EndCriticalRegion();
			}

			System.Diagnostics.Debug.WriteLine("SendMailThread exiting");
		}
	}
}
