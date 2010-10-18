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

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.Mail;

  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// Functions to send email via SMTP
  /// </summary>
  public class YafSendMail
  {
    #region Public Methods

    /// <summary>
    /// The send all.
    /// </summary>
    /// <param name="messages">
    /// The messages.
    /// </param>
    public void SendAll([NotNull] IEnumerable<MailMessage> messages)
    {
      // Wes : Changed to use settings from configuration file's standard <smtp> section. 
      // Reason for change: The host smtp settings are redundant and
      // using them here couples this method to YafCache, which is dependant on a current HttpContext. 
      // Configuration settings are cached automatically.
      CodeContracts.ArgumentNotNull(messages, "messages");

      var smtpSend = new SmtpClient { EnableSsl = Config.UseSMTPSSL };

      // Tommy: solve random failure problem. Don't set this value to 1.
      // See this: http://stackoverflow.com...tem-net-mail-has-issues 
      smtpSend.ServicePoint.MaxIdleTime = 2;
      smtpSend.ServicePoint.ConnectionLimit = 1;

      foreach (var message in messages.ToList())
      {
        smtpSend.Send(message);
      }
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
    public void SendAllIsolated([NotNull] IEnumerable<MailMessage> messages, [CanBeNull] Action<MailMessage, Exception> handleExceptionAction)
    {
      // Wes : Changed to use settings from configuration file's standard <smtp> section. 
      // Reason for change: The host smtp settings are redundant and
      // using them here couples this method to YafCache, which is dependant on a current HttpContext. 
      // Configuration settings are cached automatically.
      CodeContracts.ArgumentNotNull(messages, "messages");

      var smtpSend = new SmtpClient { EnableSsl = Config.UseSMTPSSL };

      // Tommy: solve random failure problem. Don't set this value to 1.
      // See this: http://stackoverflow.com...tem-net-mail-has-issues 
      smtpSend.ServicePoint.MaxIdleTime = 2;
      smtpSend.ServicePoint.ConnectionLimit = 1;

      foreach (var message in messages.ToList())
      {
        try
        {
          smtpSend.Send(message);
        }
        catch (Exception ex)
        {
          if (handleExceptionAction != null)
          {
            handleExceptionAction(message, ex);
          }
        }
      }
    }

    #endregion
  }
}