namespace YAF.Classes.Core
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Diagnostics;
  using System.Net.Mail;
  using System.Threading;

  using YAF.Classes.Data;
  using YAF.Classes.Utils;

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

        foreach (var mail in mailList)
        {
          // Build a MailMessage
          if (mail.FromUser.IsSet() && mail.ToUser.IsSet())
          {
            try
            {
              MailAddress toEmailAddress = mail.ToUserName.IsSet() ? new MailAddress(mail.ToUser, mail.ToUserName) : new MailAddress(mail.ToUser);
              MailAddress fromEmailAddress = mail.FromUserName.IsSet() ? new MailAddress(mail.FromUser, mail.FromUserName) : new MailAddress(mail.FromUser);

              // send the email message now...
              Debug.WriteLine("Sending to {0}...".FormatWith(mail.ToUser));

              Send(fromEmailAddress, toEmailAddress, mail.Subject, mail.Body, mail.BodyHtml);

              Thread.Sleep(200);

              Debug.WriteLine("Sent to {0}.".FormatWith(mail.ToUser));
            }
            catch (System.FormatException ex)
            {
              // email address is no good -- delete this email...
              Debug.WriteLine("Invalid Email Address: {0}".FormatWith(ex.ToString()));

#if (DEBUG)
              DB.eventlog_create(null, "Invalid Email Address: {0}".FormatWith(ex.ToString()), ex.ToString());
#endif
            }
            catch (SmtpException ex)
            {
#if (DEBUG)
              DB.eventlog_create(null, "SendMailThread SmtpException", ex.ToString());
#endif
              Debug.WriteLine("Send Exception: {0}".FormatWith(ex.ToString()));

              if (mail.SendTries < 5)
              {
                continue;
              }
              else
              {
                DB.eventlog_create(null, "SendMailThread Failed for the 5th time:", ex.ToString());
              }
            }
          }

          // all is well, delete this message...
          Debug.WriteLine("Deleting email to {0} (ID: {1})".FormatWith(mail.ToUser, mail.MailID));
          DB.mail_delete(mail.MailID);
        }
      }
      catch (Exception e)
      {
        // debug the exception
        Debug.WriteLine("Exception Thrown in SendMail Thread: " + e.ToString());
#if (DEBUG)
        DB.eventlog_create(null, "SendMailThread General Exception", e.ToString());
#endif
      }

      Debug.WriteLine("SendMailThread exiting");
    }
  }
}