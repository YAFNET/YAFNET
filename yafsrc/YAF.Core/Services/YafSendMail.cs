/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Core.Services
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.Mail;
  using System.Threading;

  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Functions to send email via SMTP
  /// </summary>
  public class YafSendMail : ISendMail
  {
    #region Public Methods

    /// <summary>
    /// Sends all MailMessages via the SmtpClient. Doesn't handle any exceptions.
    /// </summary>
    /// <param name="messages">
    /// The messages.
    /// </param>
    public void SendAll([NotNull] IEnumerable<MailMessage> messages)
    {
      CodeContracts.ArgumentNotNull(messages, "messages");

      messages.ToList().ForEach(m => m.Send());
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
      CodeContracts.ArgumentNotNull(messages, "messages");

      foreach (var message in messages.ToList())
      {
        try
        {
          // send the message...
          message.Send();

          // sleep for a 1/20 of a second...
          Thread.Sleep(50);
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