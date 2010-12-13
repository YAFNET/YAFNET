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

namespace YAF.Classes.Core
{
  /// <summary>
  /// The create mail.
  /// </summary>
  public static class CreateMail
  {
    /// <summary>
    /// Send an error report by email. For this to work, 
    /// smtpserver and erroremail must be set in Web.config.
    /// </summary>
    /// <param name="x">
    /// The Exception object to report.
    /// </param>
    [Obsolete("Not used anymore -- remove calls")]
    public static void CreateLogEmail(Exception x)
    {
      //try
      //{
      //  string config = Config.LogToMail;
      //  if (config == null)
      //  {
      //    return;
      //  }

      //  // Find mail info
      //  string email = string.Empty;
      //  string server = string.Empty;
      //  string SmtpUserName = string.Empty;
      //  string SmtpPassword = string.Empty;

      //  foreach (string part in config.Split(';'))
      //  {
      //    string[] pair = part.Split('=');
      //    if (pair.Length != 2)
      //    {
      //      continue;
      //    }

      //    switch (pair[0].Trim().ToLower())
      //    {
      //      case "email":
      //        email = pair[1].Trim();
      //        break;
      //      case "server":
      //        server = pair[1].Trim();
      //        break;
      //      case "user":
      //        SmtpUserName = pair[1].Trim();
      //        break;
      //      case "pass":
      //        SmtpPassword = pair[1].Trim();
      //        break;
      //    }
      //  }

      //  // Build body
      //  var msg = new StringBuilder();
      //  msg.Append("<style>\r\n");
      //  msg.Append("body,td,th{font:8pt tahoma}\r\n");
      //  msg.Append("table{background-color:#C0C0C0}\r\n");
      //  msg.Append("th{font-weight:bold;text-align:left;background-color:#C0C0C0;padding:4px}\r\n");
      //  msg.Append("td{vertical-align:top;background-color:#FFFBF0;padding:4px}\r\n");
      //  msg.Append("</style>\r\n");
      //  msg.Append("<table cellpadding=1 cellspacing=1>\r\n");

      //  if (x != null)
      //  {
      //    msg.Append("<tr><th colspan=2>Exception</th></tr>");
      //    msg.AppendFormat("<tr><td>Exception</td><td><pre>{0}</pre></td></tr>", x);
      //    msg.AppendFormat("<tr><td>Message</td><td>{0}</td></tr>", Text2Html(x.Message));
      //    msg.AppendFormat("<tr><td>Source</td><td>{0}</td></tr>", Text2Html(x.Source));
      //    msg.AppendFormat("<tr><td>StackTrace</td><td>{0}</td></tr>", Text2Html(x.StackTrace));
      //    msg.AppendFormat("<tr><td>TargetSize</td><td>{0}</td></tr>", Text2Html(x.TargetSite.ToString()));
      //  }

      //  msg.Append("<tr><th colspan=2>QueryString</th></tr>");
      //  foreach (string key in YafContext.Current.Get<HttpRequestBase>().QueryString.AllKeys)
      //  {
      //    msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.QueryString[key]);
      //  }

      //  msg.Append("<tr><th colspan=2>Form</th></tr>");
      //  foreach (string key in HttpContext.Current.Request.Form.AllKeys)
      //  {
      //    msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.Form[key]);
      //  }

      //  msg.Append("<tr><th colspan=2>ServerVariables</th></tr>");
      //  foreach (string key in HttpContext.Current.Request.ServerVariables.AllKeys)
      //  {
      //    msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.ServerVariables[key]);
      //  }

      //  msg.Append("<tr><th colspan=2>Session</th></tr>");
      //  foreach (string key in YafContext.Current.Get<HttpSessionStateBase>())
      //  {
      //    msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, YafContext.Current.Get<HttpSessionStateBase>()[key]);
      //  }

      //  msg.Append("<tr><th colspan=2>Application</th></tr>");
      //  foreach (string key in HttpContext.Current.Application)
      //  {
      //    msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Application[key]);
      //  }

      //  msg.Append("<tr><th colspan=2>Cookies</th></tr>");
      //  foreach (string key in HttpContext.Current.Request.Cookies.AllKeys)
      //  {
      //    msg.AppendFormat("<tr><td>{0}</td><td>{1}&nbsp;</td></tr>", key, HttpContext.Current.Request.Cookies[key].Value);
      //  }

      //  msg.Append("</table>");

      //  var smtpSend = new SmtpClient(server);

      //  if (SmtpUserName.Length > 0 && SmtpPassword.Length > 0)
      //  {
      //    smtpSend.Credentials = new NetworkCredential(SmtpUserName, SmtpPassword);
      //  }

      //  using (var emailMessage = new MailMessage())
      //  {
      //    emailMessage.To.Add(email);
      //    emailMessage.From = new MailAddress(email);
      //    emailMessage.Subject = "Yet Another Forum.net Error Report";
      //    emailMessage.Body = msg.ToString();
      //    emailMessage.IsBodyHtml = true;

      //    smtpSend.Send(emailMessage);
      //  }
      //}
      //catch (Exception)
      //{
      //}
    }
  }
}