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
namespace YAF.Classes.Core.Nntp
{
  #region Using

  using System;
  using System.Data;
  using System.Web;

  using YAF.Classes.Data;

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
      int guestUserId = DB.user_guest(boardID); // Use guests user-id
      string hostAddress = HttpContext.Current.Request.UserHostAddress;     
      DateTime dateTimeStart = DateTime.UtcNow;
      int articleCount = 0;

      string nntpHostName = string.Empty;
      int nntpPort = 119;

      var nntpConnection = new NntpConnection();

      try
      {
        // Only those not updated in the last 30 minutes
        using (DataTable forumsDataTable = DB.nntpforum_list(boardID, lastUpdate, null, true))
        {
          foreach (DataRow forumDataRow in forumsDataTable.Rows)
          {
            if (nntpHostName != forumDataRow["Address"].ToString().ToLower() || nntpPort != (int)forumDataRow["Port"])
            {
              if (nntpConnection != null)
              {
                nntpConnection.Disconnect();
              }

              nntpHostName = forumDataRow["Address"].ToString().ToLower();
              nntpPort = Convert.ToInt32(forumDataRow["Port"]);

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

            var lastMessageNo = (int)forumDataRow["LastMessageNo"];
            int currentMessage = lastMessageNo;

            // If this is first retrieve for this group, only fetch last 50
            if (currentMessage == 0)
            {
              currentMessage = group.High - 50;
            }

            currentMessage++;

            var forumID = (int)forumDataRow["ForumID"];

            for (; currentMessage <= group.High; currentMessage++)
            {
              try
              {
                Article article = nntpConnection.GetArticle(currentMessage);

                string body = article.Body.Text;
                string subject = article.Header.Subject;
                string fromName = article.Header.From;
                string thread = article.ArticleId.ToString();
                DateTime dateTime = article.Header.Date;

                if (dateTime.Year < 1950 || dateTime > DateTime.UtcNow)
                {
                  dateTime = DateTime.UtcNow;
                }

                body = String.Format("Date: {0}\r\n\r\n", article.Header.Date) + body;
                body = String.Format("Date parsed: {0}\r\n", dateTime) + body;

                if (createUsers)
                {
                  guestUserId = DB.user_nntp(boardID, fromName, string.Empty);
                }

                body = HttpContext.Current.Server.HtmlEncode(body);
                DB.nntptopic_savemessage(forumDataRow["NntpForumID"], subject, body, guestUserId, fromName, hostAddress, dateTime, thread);
                lastMessageNo = currentMessage;
                articleCount++;

                // We don't wanna retrieve articles forever...
                // Total time x seconds for all groups
                if ((DateTime.UtcNow - dateTimeStart).TotalSeconds > timeToRun)
                {
                  break;
                }
              }
              catch (NntpException)
              {
              }
            }

            DB.nntpforum_update(forumDataRow["NntpForumID"], lastMessageNo, guestUserId);

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
        if (nntpConnection != null)
        {
          nntpConnection.Disconnect();
        }
      }

      return articleCount;
    }

    #endregion
  }
}