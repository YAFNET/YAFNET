/* Yet Another Forum.NET
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
namespace YAF.Core.Services
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Net.Mail;
  using System.Threading;

  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;
  using YAF.Types.Objects;

  #endregion

  /// <summary>
  /// Separate class since SendThreaded isn't needed functionality
  ///   for any instance except the <see cref="HttpModule"/> instance.
  /// </summary>
  public class YafSendMailThreaded : YafSendMail
  {
    #region Public Methods

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
        IEnumerable<TypedMailList> mailList;
        var mailMessages = new Dictionary<MailMessage, TypedMailList>();

        try
        {
          Debug.WriteLine("Retrieving queued mail...");
          Thread.BeginCriticalRegion();

          mailList = LegacyDb.MailList(uniqueId);

          Debug.WriteLine("Got {0} Messages...".FormatWith(mailList.Count()));
        }
        finally
        {
          Thread.EndCriticalRegion();
        }

        // construct mail message list...
        foreach (var mail in mailList)
        {
          // Build a MailMessage
          if (mail.FromUser.IsNotSet() || mail.ToUser.IsNotSet())
          {
            continue;
          }

          MailAddress toEmailAddress = mail.ToUserName.IsSet()
                                         ? new MailAddress(mail.ToUser, mail.ToUserName)
                                         : new MailAddress(mail.ToUser);
          MailAddress fromEmailAddress = mail.FromUserName.IsSet()
                                           ? new MailAddress(mail.FromUser, mail.FromUserName)
                                           : new MailAddress(mail.FromUser);

          var newMessage = new MailMessage();
          newMessage.Populate(fromEmailAddress, toEmailAddress, mail.Subject, mail.Body, mail.BodyHtml);
          mailMessages.Add(newMessage, mail);
        }

        this.SendAllIsolated(
          mailMessages.Select(x => x.Key),
          (message, ex) =>
          {
            if (ex is FormatException)
            {
              // email address is no good -- delete this email...
              Debug.WriteLine("Invalid Email Address: {0}".FormatWith(ex.ToString()));
#if (DEBUG)
              LegacyDb.eventlog_create(null, "Invalid Email Address: {0}".FormatWith(ex.ToString()), ex.ToString());
#endif
            }
            else if (ex is SmtpException)
            {
#if (DEBUG)
              LegacyDb.eventlog_create(null, "SendMailThread SmtpException", ex.ToString());
#endif
              Debug.WriteLine("Send Exception: {0}".FormatWith(ex.ToString()));

              if (mailMessages.ContainsKey(message) && mailMessages[message].SendTries < 2)
              {
                // remove from the collection so it doesn't get deleted...
                mailMessages.Remove(message);
              }
              else
              {
                LegacyDb.eventlog_create(null, "SendMailThread Failed for the 2nd time:", ex.ToString());
              }
            }
            else
            {
              // general exception...
              Debug.WriteLine("Exception Thrown in SendMail Thread: " + ex.ToString());
#if (DEBUG)
              LegacyDb.eventlog_create(null, "SendMailThread General Exception", ex.ToString());
#endif
            }
          });

        foreach (var message in mailMessages.Values)
        {
          // all is well, delete this message...
          Debug.WriteLine("Deleting email to {0} (ID: {1})".FormatWith(message.ToUser, message.MailID));
          LegacyDb.mail_delete(message.MailID);
        }
      }
      catch (Exception ex)
      {
        // general exception...
        Debug.WriteLine("Exception Thrown in SendMail Thread: " + ex.ToString());
#if (DEBUG)
        LegacyDb.eventlog_create(null, "SendMailThread General Exception", ex.ToString());
#endif
      }

      Debug.WriteLine("SendMailThread exiting");
    }

    #endregion
  }
}