/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

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
    using System.Linq;
    using System.Runtime.Caching;

    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
    using YAF.Types.Objects.Nntp;

    #endregion

    /// <summary>
    /// The on request delegate.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    public delegate void OnRequestDelegate(string message);

    /// <summary>
    /// The YAF NNTP.
    /// </summary>
    public class Nntp : INewsreader
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Nntp"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public Nntp([NotNull] ILoggerService logger)
        {
            this.Logger = logger;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Logger.
        /// </summary>
        public ILoggerService Logger { get; set; }

        #endregion

        #region Implemented Interfaces

        #region INewsreader

        /// <summary>
        /// The get nntp connection.
        /// </summary>
        /// <param name="nntpForum">
        /// The nntp forum.
        /// </param>
        /// <returns>
        /// The <see cref="NntpConnection"/>.
        /// </returns>
        [NotNull]
        public static NntpConnection GetNntpConnection([NotNull] Tuple<NntpForum, NntpServer, Forum> nntpForum)
        {
            CodeContracts.VerifyNotNull(nntpForum);

            var nntpConnection = new NntpConnection();

            // call connect server
            nntpConnection.ConnectServer(nntpForum.Item2.Address.ToLower(), nntpForum.Item2.Port ?? 119);

            // provide authentication if required...
            if (!nntpForum.Item2.UserName.IsSet() || !nntpForum.Item2.UserPass.IsSet())
            {
                return nntpConnection;
            }

            nntpConnection.ProvideIdentity(nntpForum.Item2.UserName, nntpForum.Item2.UserPass);
            nntpConnection.SendIdentity();

            return nntpConnection;
        }

        /// <summary>
        /// The read articles.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="lastUpdate">
        /// The last update.
        /// </param>
        /// <param name="timeToRun">
        /// The time to run.
        /// </param>
        /// <param name="createUsers">
        /// The create users.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ReadArticles(int boardId, int lastUpdate, int timeToRun, bool createUsers)
        {
            if (MemoryCache.Default["WorkingInYafNNTP"] != null)
            {
                return 0;
            }

            var guestUser = BoardContext.Current.GetRepository<User>().GetGuestUser(boardId); // Use guests user-id

            var dateTimeStart = DateTime.UtcNow;
            var articleCount = 0;
            var count = 0;

            try
            {
                MemoryCache.Default["WorkingInYafNNTP"] = true;

                // Only those not updated in the last 30 minutes
                foreach (var nntpForum in BoardContext.Current.GetRepository<NntpForum>()
                    .NntpForumList(boardId, true)
                    .Where(n => (n.Item1.LastUpdate - DateTime.UtcNow).Minutes > lastUpdate))
                {
                    using var nntpConnection = GetNntpConnection(nntpForum);
                    var group = nntpConnection.ConnectGroup(nntpForum.Item1.GroupName);

                    var lastMessageNo = nntpForum.Item1.LastMessageNo;

                    // start at the bottom...
                    var currentMessage = lastMessageNo == 0 ? group.Low : lastMessageNo + 1;
                    var cutOffDate = nntpForum.Item1.DateCutOff ?? DateTime.MinValue;

                    if (nntpForum.Item1.DateCutOff.HasValue)
                    {
                        var behindCutOff = true;

                        // advance if needed...
                        do
                        {
                            var list = nntpConnection.GetArticleList(
                                currentMessage,
                                Math.Min(currentMessage + 500, group.High));

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
                        BoardContext.Current.GetRepository<NntpForum>().Update(
                            nntpForum.Item1.ID,
                            currentMessage);
                    }

                    for (; currentMessage <= group.High; currentMessage++)
                    {
                        try
                        {
                            Article article;
                            try
                            {
                                article = nntpConnection.GetArticle(currentMessage);
                            }
                            catch (InvalidOperationException ex)
                            {
                                this.Logger.Error(ex, $"Error Downloading Message ID {currentMessage}");

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
                                this.Logger.Debug(
                                    $"Skipped message id {currentMessage} due to date being {dateTime}.");
                                continue;
                            }

                            if (fromName.IsSet() && fromName.Contains("<"))
                            {
                                fromName = fromName.Substring(0, fromName.LastIndexOf('<') - 1);
                                fromName = fromName.Replace("\"", string.Empty).Trim();
                            }
                            else if (fromName.IsSet() && fromName.Contains("("))
                            {
                                fromName = fromName.Substring(0, fromName.LastIndexOf('(') - 1).Trim();
                            }

                            if (fromName.IsNotSet())
                            {
                                fromName = originalName;
                            }

                            var externalMessageId = article.MessageId;

                            if (createUsers)
                            {
                                guestUser = BoardContext.Current.GetRepository<User>().UpdateNntpUser(
                                    boardId,
                                    fromName);
                            }

                            var body = ReplaceBody(article.Body.Text.Trim());

                            BoardContext.Current.GetRepository<NntpTopic>().SaveMessage(
                                nntpForum.Item1,
                                subject.Truncate(75),
                                body,
                                guestUser,
                                fromName.Truncate(100, string.Empty),
                                "NNTP",
                                dateTime,
                                externalMessageId.Truncate(255, string.Empty));

                            lastMessageNo = currentMessage;

                            articleCount++;

                            // We don't wanna retrieve articles forever...
                            // Total time x seconds for all groups
                            if ((DateTime.UtcNow - dateTimeStart).TotalSeconds > timeToRun)
                            {
                                break;
                            }

                            if (count++ <= 1000)
                            {
                                continue;
                            }

                            count = 0;
                            BoardContext.Current.GetRepository<NntpForum>().Update(
                                nntpForum.Item1.ID,
                                lastMessageNo);
                        }
                        catch (NntpException exception)
                        {
                            if (exception.ErrorCode >= 900)
                            {
                                throw;
                            }

                            if (exception.ErrorCode != 423)
                            {
                                this.Logger.Error(exception, "YafNntp");
                            }
                        }
                        catch (Exception exception)
                        {
                            this.Logger.Error(exception, "YafNntp DB Failure");
                        }
                    }

                    BoardContext.Current.GetRepository<NntpForum>().Update(
                        nntpForum.Item1.ID,
                        lastMessageNo);

                    // Total time x seconds for all groups
                    if ((DateTime.UtcNow - dateTimeStart).TotalSeconds > timeToRun)
                    {
                        break;
                    }
                }
            }
            finally
            {
                MemoryCache.Default["WorkingInYafNNTP"] = null;
            }

            return articleCount;
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
        /// The <see cref="string"/>.
        /// </returns>
        [NotNull]
        private static string ReplaceBody([NotNull] string body)
        {
            // Incorrect tags fixes which are common in nntp messages and cause display problems.
            // These are spotted ones.
            body = body.Replace("<br>", "<br />");
            body = body.Replace("<hr>", "<hr />");

            return body;
        }

        #endregion
    }
}