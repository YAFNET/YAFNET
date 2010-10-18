namespace YAF.Classes.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Net.Mail;
  using System.Threading;

  using YAF.Classes.Data;
  using YAF.Classes.Utils;

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
        IEnumerable<TypedMailList> mailList = new List<TypedMailList>();

        try
        {
          Debug.WriteLine("Retrieving queued mail...");
          Thread.BeginCriticalRegion();

          mailList = DB.MailList(uniqueId);
        }
        finally
        {
          Thread.EndCriticalRegion();
        }

        var mailMessages = new Dictionary<MailMessage, TypedMailList>();

        // construcct mail message list...
        foreach (var mail in mailList)
        {
          // Build a MailMessage
          if (!mail.FromUser.IsSet() || !mail.ToUser.IsSet())
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
              DB.eventlog_create(null, "Invalid Email Address: {0}".FormatWith(ex.ToString()), ex.ToString());
#endif
            }
            else if (ex is SmtpException)
            {
#if (DEBUG)
              DB.eventlog_create(null, "SendMailThread SmtpException", ex.ToString());
#endif
              Debug.WriteLine("Send Exception: {0}".FormatWith(ex.ToString()));

              if (mailMessages.ContainsKey(message) && mailMessages[message].SendTries < 5)
              {
                // remove from the collection so it doesn't get deleted...
                mailMessages.Remove(message);
              }
              else
              {
                DB.eventlog_create(null, "SendMailThread Failed for the 5th time:", ex.ToString());
              }
            }
            else
            {
              // general exception...
              Debug.WriteLine("Exception Thrown in SendMail Thread: " + ex.ToString());
#if (DEBUG)
              DB.eventlog_create(null, "SendMailThread General Exception", ex.ToString());
#endif
            }
          });

        foreach (var message in mailMessages.Values)
        {
          // all is well, delete this message...
          Debug.WriteLine("Deleting email to {0} (ID: {1})".FormatWith(message.ToUser, message.MailID));
          DB.mail_delete(message.MailID);
        }
      }
      catch (Exception ex)
      {
        // general exception...
        Debug.WriteLine("Exception Thrown in SendMail Thread: " + ex.ToString());
#if (DEBUG)
        DB.eventlog_create(null, "SendMailThread General Exception", ex.ToString());
#endif
      }

      Debug.WriteLine("SendMailThread exiting");
    }

    #endregion
  }
}