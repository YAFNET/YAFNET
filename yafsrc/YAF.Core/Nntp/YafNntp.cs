/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Nntp
{
  #region Using

  using System;
  using System.Data;
  using System.Data.SqlClient;
  using System.Linq;
  using System.Web;

  using YAF.Classes.Data;
  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Types.Objects;
  using YAF.Utils;

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
  public class YafNntp : INewsreader
  {
    #region Constants and Fields

    /// <summary>
    /// The _application state base.
    /// </summary>
    private readonly HttpApplicationStateBase _applicationStateBase;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafNntp"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="applicationStateBase">
    /// The application state base.
    /// </param>
    public YafNntp([NotNull] ILogger logger, [NotNull] HttpApplicationStateBase applicationStateBase)
    {
      this._applicationStateBase = applicationStateBase;
      this.Logger = logger;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILogger Logger { get; set; }

    #endregion

    #region Implemented Interfaces

    #region INewsreader

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
    /// <exception cref="NntpException"><c>NntpException</c>.</exception>
    public int ReadArticles(int boardID, int lastUpdate, int timeToRun, bool createUsers)
    {
      if (this._applicationStateBase["WorkingInYafNNTP"] != null)
      {
        return 0;
      }

      var guestUserId = UserMembershipHelper.GuestUserId; // Use guests user-id

      // string hostAddress = YafContext.Current.Get<HttpRequestBase>().UserHostAddress;     
      var dateTimeStart = DateTime.UtcNow;
      var articleCount = 0;
      var count = 0;

      try
      {
        this._applicationStateBase["WorkingInYafNNTP"] = true;

        // Only those not updated in the last 30 minutes
        foreach (var nntpForum in LegacyDb.NntpForumList(boardID, lastUpdate, null, true))
        {
          using (var nntpConnection = GetNntpConnection(nntpForum))
          {
            var group = nntpConnection.ConnectGroup(nntpForum.GroupName);

            var lastMessageNo = nntpForum.LastMessageNo ?? 0;

            // start at the bottom...
            var currentMessage = lastMessageNo == 0 ? group.Low : lastMessageNo + 1;
            var nntpForumID = nntpForum.NntpForumID;
            var cutOffDate = nntpForum.DateCutOff ?? DateTime.MinValue;

            if (nntpForum.DateCutOff.HasValue)
            {
              var behindCutOff = true;

              // advance if needed...
              do
              {
                var list = nntpConnection.GetArticleList(currentMessage, Math.Min(currentMessage + 500, group.High));

                foreach (var article in list)
                {
                  if (article.Header.Date.Year < 1950 || article.Header.Date > DateTime.UtcNow)
                  {
                    article.Header.Date = DateTime.UtcNow;
                  }

                  if (article.Header.Date >= cutOffDate)
                  {
                    behindCutOff = false;
                    break;
                  }

                  currentMessage++;
                }
              }
              while (behindCutOff);

              // update the group lastMessage info...
              LegacyDb.nntpforum_update(nntpForum.NntpForumID, currentMessage, guestUserId);
            }

            for (; currentMessage <= group.High; currentMessage++)
            {
              Article article;

              try
              {
                try
                {
                  article = nntpConnection.GetArticle(currentMessage);
                }
                catch (InvalidOperationException ex)
                {
                    this.Logger.Error(ex, "Error Downloading Message ID {0}", currentMessage);

                  // just advance to the next message
                  currentMessage++;
                  continue;
                }

                var subject = article.Header.Subject.Trim();
                var originalName = article.Header.From.Trim();
                var fromName = originalName;
                var dateTime = article.Header.Date;

                if (dateTime.Year < 1950 || dateTime > DateTime.UtcNow)
                {
                  dateTime = DateTime.UtcNow;
                }

                if (dateTime < cutOffDate)
                {
                  this.Logger.Debug("Skipped message id {0} due to date being {1}.", currentMessage, dateTime);
                  continue;
                }

                if (fromName.IsSet() && fromName.LastIndexOf('<') > 0)
                {
                  fromName = fromName.Substring(0, fromName.LastIndexOf('<') - 1);
                  fromName = fromName.Replace("\"", string.Empty).Trim();
                }
                else if (fromName.IsSet() && fromName.LastIndexOf('(') > 0)
                {
                  fromName = fromName.Substring(0, fromName.LastIndexOf('(') - 1).Trim();
                }

                if (fromName.IsNotSet())
                {
                  fromName = originalName;
                }

                var externalMessageId = article.MessageId;

                var referenceId = article.Header.ReferenceIds.LastOrDefault();

                if (createUsers)
                {
                  guestUserId = LegacyDb.user_nntp(boardID, fromName, string.Empty, article.Header.TimeZoneOffset);
                }

                var body = this.ReplaceBody(article.Body.Text.Trim());

                LegacyDb.nntptopic_savemessage(
                  nntpForumID,
                  subject.Truncate(75),
                  body,
                  guestUserId,
                  fromName.Truncate(100, string.Empty),
                  "NNTP",
                  dateTime,
                  externalMessageId.Truncate(255, string.Empty),
                  referenceId.Truncate(255, string.Empty));

                lastMessageNo = currentMessage;

                articleCount++;

                // We don't wanna retrieve articles forever...
                // Total time x seconds for all groups
                if ((DateTime.UtcNow - dateTimeStart).TotalSeconds > timeToRun)
                {
                  break;
                }

                if (count++ > 1000)
                {
                  count = 0;
                  LegacyDb.nntpforum_update(nntpForum.NntpForumID, lastMessageNo, guestUserId);
                }
              }
              catch (NntpException exception)
              {
                if (exception.ErrorCode >= 900)
                {
                  throw;
                }
                else if (exception.ErrorCode != 423)
                {
                  this.Logger.Error(exception, "YafNntp");
                }
              }
              catch (SqlException exception)
              {
                this.Logger.Error(exception, "YafNntp DB Failure");
              }
            }

            LegacyDb.nntpforum_update(nntpForum.NntpForumID, lastMessageNo, guestUserId);

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
        this._applicationStateBase["WorkingInYafNNTP"] = null;
      }

      return articleCount;
    }

    [NotNull]
    public static NntpConnection GetNntpConnection([NotNull] TypedNntpForum nntpForum)
    {
      CodeContracts.VerifyNotNull(nntpForum, "nntpForum");

      var nntpConnection = new NntpConnection();

      // call connect server
      nntpConnection.ConnectServer(nntpForum.Address.ToLower(), nntpForum.Port ?? 119);

      // provide authentication if required...
      if (nntpForum.UserName.IsSet() && nntpForum.UserPass.IsSet())
      {
        nntpConnection.ProvideIdentity(nntpForum.UserName, nntpForum.UserPass);
        nntpConnection.SendIdentity();
      }

      return nntpConnection;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The replace body.
    /// </summary>
    /// <param name="body">
    /// The body.
    /// </param>
    /// <returns>
    /// The replace body.
    /// </returns>
    [NotNull]
    private string ReplaceBody([NotNull] string body)
    {
      // Incorrect tags fixes which are common in nntp messages and cause display problems.
      // These are spotted ones.
      body = body.Replace("<br>", "<br />");
      body = body.Replace("<hr>", "<hr />");

      // body = "Date: {0}\r\n\r\n".FormatWith(article.Header.Date) + body;
      // body = "Date parsed: {0}(UTC)\r\n".FormatWith(dateTime) + body;

      //// vzrus: various wrong NNTP tags replacements

      // body = body.Replace("&amp;lt;", "&lt;");
      // body = body.Replace("&amp;gt;", "&gt;");
      // body = body.Replace("&lt;br&gt;", "");
      // body = body.Replace("&lt;hr&gt;", "<hr />");

      // body = body.Replace("&amp;quot;", @"&#34;");

      // Innerquote class in yaf terms, should be replaced while displaying     
      // body = body.Replace("&lt;quote&gt;", @"[quote]");
      // body = body.Replace("&lt;/quote&gt;", @"[/quote]");
      return body;
    }

    #endregion
  }
}