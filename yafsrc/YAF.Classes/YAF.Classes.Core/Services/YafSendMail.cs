/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Threading;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
  /// <summary>
  /// Functions to send email via SMTP
  /// </summary>
  public class YafSendMail
  {
    /// <summary>
    /// Queues an e-mail message for asynchronous delivery
    /// </summary>
    /// <param name="fromEmail">
    /// The from Email.
    /// </param>
    /// <param name="fromName">
    /// The from Name.
    /// </param>
    /// <param name="toEmail">
    /// The to Email.
    /// </param>
    /// <param name="toName">
    /// The to Name.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="bodyText">
    /// The body Text.
    /// </param>
    /// <param name="bodyHtml">
    /// The body Html.
    /// </param>
    public void Queue(string fromEmail, string fromName, string toEmail, string toName, string subject, string bodyText, string bodyHtml)
    {
      DB.mail_create(fromEmail, fromName, toEmail, toName, subject, bodyText, bodyHtml);
    }

    /// <summary>
    /// Queues an e-mail messagage for asynchronous delivery
    /// </summary>
    /// <param name="fromEmail">
    /// </param>
    /// <param name="toEmail">
    /// </param>
    /// <param name="subject">
    /// </param>
    /// <param name="body">
    /// </param>
    public void Queue(string fromEmail, string toEmail, string subject, string body)
    {
      DB.mail_create(fromEmail, null, toEmail, null, subject, body, null);
    }

    /// <summary>
    /// The send.
    /// </summary>
    /// <param name="fromEmail">
    /// The from email.
    /// </param>
    /// <param name="toEmail">
    /// The to email.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="body">
    /// The body.
    /// </param>
    public void Send(string fromEmail, string toEmail, string subject, string body)
    {
      Debug.Assert(!string.IsNullOrEmpty(toEmail));
      Debug.Assert(!string.IsNullOrEmpty(fromEmail));
      Send(new MailAddress(fromEmail), new MailAddress(toEmail), subject, body);
    }

    /// <summary>
    /// The send.
    /// </summary>
    /// <param name="fromEmail">
    /// The from email.
    /// </param>
    /// <param name="fromName">
    /// The from name.
    /// </param>
    /// <param name="toEmail">
    /// The to email.
    /// </param>
    /// <param name="toName">
    /// The to name.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="body">
    /// The body.
    /// </param>
    public void Send(string fromEmail, string fromName, string toEmail, string toName, string subject, string body)
    {
      Send(new MailAddress(fromEmail, fromName), new MailAddress(toEmail, toName), subject, body);
    }

    /// <summary>
    /// The send.
    /// </summary>
    /// <param name="fromAddress">
    /// The from address.
    /// </param>
    /// <param name="toAddress">
    /// The to address.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="bodyText">
    /// The body text.
    /// </param>
    public void Send(MailAddress fromAddress, MailAddress toAddress, string subject, string bodyText)
    {
      Send(fromAddress, toAddress, subject, bodyText, null);
    }

    /// <summary>
    /// The send.
    /// </summary>
    /// <param name="fromAddress">
    /// The from address.
    /// </param>
    /// <param name="toAddress">
    /// The to address.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="bodyText">
    /// The body text.
    /// </param>
    /// <param name="bodyHtml">
    /// The body html.
    /// </param>
    public void Send(MailAddress fromAddress, MailAddress toAddress, string subject, string bodyText, string bodyHtml)
    {
      using (var emailMessage = new MailMessage())
      {
        emailMessage.To.Add(toAddress);
        emailMessage.From = fromAddress;
        emailMessage.Subject = subject;

        Encoding textEncoding = Encoding.UTF8;

        // TODO: Add code that figures out encoding...
        /*
        if ( !Regex.IsMatch( bodyText, @"^([0-9a-z!@#\$\%\^&\*\(\)\-=_\+])", RegexOptions.IgnoreCase ) ||
                !Regex.IsMatch( subject, @"^([0-9a-z!@#\$\%\^&\*\(\)\-=_\+])", RegexOptions.IgnoreCase ) )
        {
          textEncoding = Encoding.Unicode;
        }
        */

        // add text view...
        emailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, textEncoding, "text/plain"));

        // see if html alternative is also desired...
        if (bodyHtml.IsSet())
        {
          emailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHtml, Encoding.UTF8, "text/html"));
        }

        // Wes : Changed to use settings from configuration file's standard <smtp> section. 
        // Reason for change: The host smtp settings are redundant and
        // using them here couples this method to YafCache, which is dependant on a current HttpContext. 
        // Configuration settings are cached automatically.
        var smtpSend = new SmtpClient { EnableSsl = Config.UseSMTPSSL };
        smtpSend.Send(emailMessage);
      }
    }
  }

  /// <summary>
  /// Separate class since SendThreaded isn't needed functionality
  /// for any instance except the <see cref="HttpModule"/> instance.
  /// </summary>
  public class YafSendMailThreaded : YafSendMail
  {
    /// <summary>
    /// The send threaded.
    /// </summary>
    /// <param name="uniqueId">
    /// The unique id.
    /// </param>
    public void SendThreaded(int uniqueId)
    {
      try
      {
        Debug.WriteLine("Retrieving queued mail...");
        Thread.BeginCriticalRegion();

        using (DataTable dt = DB.mail_list(uniqueId))
        {
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            string toEmail = dt.Rows[i]["ToUser"].ToString().Trim();
            string fromEmail = dt.Rows[i]["FromUser"].ToString().Trim();

            bool deleteEmail = true;

            // Build a MailMessage
            if (fromEmail.IsSet() && toEmail.IsSet())
            {
              MailAddress toEmailAddress;

              MailAddress fromEmailAddress = !dt.Rows[i]["FromUserName"].IsNullOrEmptyDBField()
                                               ? new MailAddress(fromEmail, dt.Rows[i]["FromUserName"].ToString().Trim())
                                               : new MailAddress(fromEmail);

              // create the TO email address...
              if (!dt.Rows[i]["ToUserName"].IsNullOrEmptyDBField())
              {
                toEmailAddress = new MailAddress(toEmail, dt.Rows[i]["ToUserName"].ToString().Trim());
              }
              else
              {
                toEmailAddress = new MailAddress(toEmail);
              }

              string subject = dt.Rows[i]["Subject"].ToString();
              string textBody = dt.Rows[i]["Body"].ToString();
              string htmlBody = dt.Rows[i]["BodyHtml"].ToString();

              Exception exceptionThrown = null;

              try
              {
                // send the email message now...
                Debug.WriteLine("Sending");
                Send(fromEmailAddress, toEmailAddress, subject, textBody, htmlBody);
                Debug.WriteLine("Sent");
              }
              catch (System.Net.Mail.SmtpFailedRecipientException ex)
              {
                exceptionThrown = ex;
              }
              catch (System.FormatException ex)
              {
                exceptionThrown = ex;
              }
              catch (SmtpException ex)
              {
                exceptionThrown = ex;
              }

              if (exceptionThrown != null)
              {
                if (dt.Rows[i]["SendTries"].ToType<int>() < 5)
                {
                  deleteEmail = false;
                }
                else
                {
                  DB.eventlog_create(1, "SendMailThread", exceptionThrown);
                }
              }
            }

            // if all is well, delete this message...
            if (deleteEmail)
            {
              DB.mail_delete(dt.Rows[i]["MailID"]);
            }
          }
        }
      }
			catch (Exception e)
			{
        //YAF.Classes.Data.DB.eventlog_create(1, "SendMailThread", x);

				// debug the exception
				Debug.WriteLine("Exception Thrown in SendMail Thread: " + e.ToString());
			}
      finally
      {
        Thread.EndCriticalRegion();
      }

      Debug.WriteLine("SendMailThread exiting");
    }
  }
}