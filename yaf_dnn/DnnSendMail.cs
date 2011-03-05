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

namespace YAF.DotNetNuke
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.Mail;
  using System.Threading;

  using global::DotNetNuke.Entities.Host;
  using global::DotNetNuke.Entities.Portals;
  using global::DotNetNuke.Services.Mail;
  using global::DotNetNuke.Services.Messaging;

  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;

  using MailPriority = global::DotNetNuke.Services.Mail.MailPriority;

  #endregion

  /// <summary>
  /// Functions to send email via SMTP
  /// </summary>
  [ExportService(ServiceLifetimeScope.Singleton)]
  public class DnnSendMail : ISendMail
  {
    #region Properties

    /// <summary>
    ///   Gets CurrentPortalSettings.
    /// </summary>
    private PortalSettings CurrentPortalSettings
    {
      get
      {
        return PortalController.GetCurrentPortalSettings();
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a SmtpClient and sends a MailMessage.
    /// </summary>
    /// <param name="mailMessage">
    /// The message.
    /// </param>
    public void Send([NotNull] MailMessage mailMessage)
    {
      CodeContracts.ArgumentNotNull(mailMessage, "mailMessage");

      var settings = Host.GetHostSettingsDictionary();

      Mail.SendMail(
        mailMessage.From.ToString(), 
        mailMessage.To.ToString(), 
        string.Empty, 
        string.Empty, 
        MailPriority.Normal, 
        mailMessage.Subject, 
        mailMessage.IsBodyHtml ? MailFormat.Html : MailFormat.Text, 
        mailMessage.BodyEncoding, 
        mailMessage.Body, 
        string.Empty, 
        settings["SMTPServer"], 
        settings["SMTPAuthentication"], 
        settings["SMTPUsername"], 
        settings["SMTPPassword"]);
    }

    #endregion

    #region Implemented Interfaces

    #region ISendMail

    /// <summary>
    /// Sends all MailMessages via the SmtpClient. Doesn't handle any exceptions.
    /// </summary>
    /// <param name="messages">
    /// The messages.
    /// </param>
    public void SendAll([NotNull] IEnumerable<MailMessage> messages)
    {
      CodeContracts.ArgumentNotNull(messages, "messages");

      foreach (var mailMessage in messages)
      {
        this.Send(mailMessage);
      }

      /*  messages.ToList().ForEach(
                m => m.Send());*/
    }

    /// <summary>
    /// The send all isolated.
    /// </summary>
    /// <param name="messages">
    /// The messages.
    /// </param>
    /// <param name="handleExceptionAction">
    /// The handle exception action.
    /// </param>
    public void SendAllIsolated(
      [NotNull] IEnumerable<MailMessage> messages, [CanBeNull] Action<MailMessage, Exception> handleExceptionAction)
    {
      CodeContracts.ArgumentNotNull(messages, "messages");

      foreach (var mailMessage in messages.ToList())
      {
        try
        {
          // send the message...
          // message.Send();
          this.Send(mailMessage);

          // sleep for a 1/20 of a second...
          Thread.Sleep(50);
        }
        catch (Exception ex)
        {
          if (handleExceptionAction != null)
          {
            handleExceptionAction(mailMessage, ex);
          }
        }
      }
    }

    #endregion

    #endregion
  }
}