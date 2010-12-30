/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Text.RegularExpressions;

namespace YAF.Core.Nntp
{
  #region Using

  using System;
  using System.Data;
  using System.Web;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The on request delegate.
  /// </summary>
  /// <param name="msg">
  /// The msg.
  /// </param>
  public delegate void OnRequestDelegate(string msg);

  /// <summary>
  /// The yaf nntp.
  /// </summary>
  public static class YafNntp
  {
    #region Public Methods

    /// <summary>
    /// The read articles.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="lastUpdate">
    /// The n last update.
    /// </param>
    /// <param name="timeToRun">
    /// The n time to run.
    /// </param>
    /// <param name="createUsers">
    /// The b create users.
    /// </param>
    /// <returns>
    /// The read articles.
    /// </returns>
    public static int ReadArticles(object boardID, int lastUpdate, int timeToRun, bool createUsers)
    {
      int guestUserId = UserMembershipHelper.GuestUserId; // Use guests user-id
      string hostAddress = YafContext.Current.Get<HttpRequestBase>().UserHostAddress;     
      DateTime dateTimeStart = DateTime.UtcNow;
      int articleCount = 0;

      string nntpHostName = string.Empty;
      int nntpPort = 119;

      var nntpConnection = new NntpConnection();

      try
      {
        // Only those not updated in the last 30 minutes
        using (DataTable forumsDataTable = LegacyDb.nntpforum_list(boardID, lastUpdate, null, true))
        {
          foreach (DataRow forumDataRow in forumsDataTable.Rows)
          {
            if (nntpHostName != forumDataRow["Address"].ToString().ToLower() || nntpPort != (int)forumDataRow["Port"])
            {
              nntpConnection.Disconnect();

              nntpHostName = forumDataRow["Address"].ToString().ToLower();
              nntpPort = forumDataRow["Port"].ToType<int>();

              // call connect server
              nntpConnection.ConnectServer(nntpHostName, nntpPort);

              // provide authentication if required...
              if (forumDataRow["UserName"] != DBNull.Value && forumDataRow["UserPass"] != DBNull.Value)
              {
                nntpConnection.ProvideIdentity(forumDataRow["UserName"].ToString(), forumDataRow["UserPass"].ToString());
                nntpConnection.SendIdentity();
              }
            }

            Newsgroup group = nntpConnection.ConnectGroup(forumDataRow["GroupName"].ToString());

            int currentMessage;
            int lastMessageNo = (int)forumDataRow["LastMessageNo"];
            
            // If this is first retrieve for this group, only fetch last 50
            if (lastMessageNo == 0)
            {
              currentMessage = Math.Max(group.High - 50, 1);
            }
            else
            {
              currentMessage = lastMessageNo + 1;
            }

            var forumID = (int)forumDataRow["ForumID"];

            for (; currentMessage <= group.High; currentMessage++)
            {
              try
              {
                Article article = nntpConnection.GetArticle(currentMessage);

                string body = article.Body.Text.Trim();
                string subject = article.Header.Subject.Trim();
                string fromName = article.Header.From.Trim();
                string thread = article.ArticleId.ToString();
                DateTime dateTime = article.Header.Date;

                if (dateTime.Year < 1950 || dateTime > DateTime.UtcNow)
                {
                  dateTime = DateTime.UtcNow;
                }

                if (createUsers)
                {
                  guestUserId = LegacyDb.user_nntp(boardID, fromName, string.Empty, article.Header.TimeZoneOffset);
                }

                // Incorrect tags fixes which are common in nntp messages and cause display problems.
                // These are spotted ones.
                body = body.Replace("<br>", "<br />");
                body = body.Replace("<hr>", "<hr />");

                //body = "Date: {0}\r\n\r\n".FormatWith(article.Header.Date) + body;
                //body = "Date parsed: {0}(UTC)\r\n".FormatWith(dateTime) + body;

                //// vzrus: various wrong NNTP tags replacements

                //body = body.Replace("&amp;lt;", "&lt;");
                //body = body.Replace("&amp;gt;", "&gt;");
                //body = body.Replace("&lt;br&gt;", "");
                //body = body.Replace("&lt;hr&gt;", "<hr />");

                //body = body.Replace("&amp;quot;", @"&#34;");
                 
                // Innerquote class in yaf terms, should be replaced while displaying     
                //body = body.Replace("&lt;quote&gt;", @"[quote]");
                //body = body.Replace("&lt;/quote&gt;", @"[/quote]");

                LegacyDb.nntptopic_savemessage(forumDataRow["NntpForumID"], subject, body, guestUserId, fromName, hostAddress, dateTime, thread);
                lastMessageNo = currentMessage;
                articleCount++;

                // We don't wanna retrieve articles forever...
                // Total time x seconds for all groups
                if ((DateTime.UtcNow - dateTimeStart).TotalSeconds > timeToRun)
                {
                  break;
                }
              }
              catch (NntpException exception)
              {
#if (DEBUG)
                YafContext.Current.AddLoadMessage("Exception: " + exception.ToString());
#endif
              }
            }

            LegacyDb.nntpforum_update(forumDataRow["NntpForumID"], lastMessageNo, guestUserId);

            // Total time x seconds for all groups
            if ((DateTime.UtcNow - dateTimeStart).TotalSeconds > timeToRun)
            {
              break;
            }
          }
        }
      }
      finally
      {
        nntpConnection.Disconnect();
      }

      return articleCount;
    }

    #endregion
  }
}