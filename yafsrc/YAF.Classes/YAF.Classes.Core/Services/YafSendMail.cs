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

namespace YAF.Classes.Core
{
  #region Using

  using System.Net.Mail;
  using System.Text;

  using YAF.Classes.Data;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Functions to send email via SMTP
  /// </summary>
  public class YafSendMail
  {
    #region Public Methods

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
    public void Queue([NotNull] string fromEmail, [NotNull] string fromName, [NotNull] string toEmail, [NotNull] string toName, [NotNull] string subject, [NotNull] string bodyText, [NotNull] string bodyHtml)
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
    public void Queue([NotNull] string fromEmail, [NotNull] string toEmail, [NotNull] string subject, [NotNull] string body)
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
    public void Send(
      [NotNull] string fromEmail, [NotNull] string toEmail, [CanBeNull] string subject, [CanBeNull] string body)
    {
      CodeContracts.ArgumentNotNull(fromEmail, "fromEmail");
      CodeContracts.ArgumentNotNull(toEmail, "toEmail");

      this.Send(new MailAddress(fromEmail), new MailAddress(toEmail), subject, body);
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
    public void Send([NotNull] string fromEmail, [NotNull] string fromName, [NotNull] string toEmail, [NotNull] string toName, [NotNull] string subject, [NotNull] string body)
    {
      this.Send(new MailAddress(fromEmail, fromName), new MailAddress(toEmail, toName), subject, body);
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
    public void Send([NotNull] MailAddress fromAddress, [NotNull] MailAddress toAddress, [NotNull] string subject, [NotNull] string bodyText)
    {
      this.Send(fromAddress, toAddress, subject, bodyText, null);
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
    public void Send(
      [NotNull] MailAddress fromAddress, 
      [NotNull] MailAddress toAddress, 
      [CanBeNull] string subject, 
      [CanBeNull] string bodyText, 
      [CanBeNull] string bodyHtml)
    {
      CodeContracts.ArgumentNotNull(fromAddress, "fromAddress");
      CodeContracts.ArgumentNotNull(toAddress, "toAddress");

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
        emailMessage.AlternateViews.Add(
          AlternateView.CreateAlternateViewFromString(bodyText, textEncoding, "text/plain"));

        // see if html alternative is also desired...
        if (bodyHtml.IsSet())
        {
          emailMessage.AlternateViews.Add(
            AlternateView.CreateAlternateViewFromString(bodyHtml, Encoding.UTF8, "text/html"));
        }

        // Wes : Changed to use settings from configuration file's standard <smtp> section. 
        // Reason for change: The host smtp settings are redundant and
        // using them here couples this method to YafCache, which is dependant on a current HttpContext. 
        // Configuration settings are cached automatically.
        var smtpSend = new SmtpClient { EnableSsl = Config.UseSMTPSSL };
        smtpSend.Send(emailMessage);
      }
    }

    #endregion
  }
}