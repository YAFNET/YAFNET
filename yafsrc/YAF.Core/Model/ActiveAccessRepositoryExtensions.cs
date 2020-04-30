/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Model
{
    using System;
    using System.Data;

    using System.Data.SqlClient;

    using YAF.Types;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The active access repository extensions.
    /// </summary>
    public static class ActiveAccessRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The Page Load as Data Row
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="sessionID">
        /// The session id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userKey">
        /// The user key.
        /// </param>
        /// <param name="ip">
        /// The IP Address.
        /// </param>
        /// <param name="location">
        /// The location.
        /// </param>
        /// <param name="forumPage">
        /// The forum page name.
        /// </param>
        /// <param name="browser">
        /// The browser.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <param name="isCrawler">
        /// The is Crawler.
        /// </param>
        /// <param name="isMobileDevice">
        /// The browser is a mobile device.
        /// </param>
        /// <param name="dontTrack">
        /// The don't track.
        /// </param>
        /// <returns>
        /// Common User Info DataRow
        /// </returns>
        public static DataRow PageLoadAsDataRow(
            this IRepository<ActiveAccess> repository,
            [NotNull] object sessionID,
            [NotNull] object boardId,
            [NotNull] object userKey,
            [NotNull] object ip,
            [NotNull] object location,
            [NotNull] object forumPage,
            [NotNull] object browser,
            [NotNull] object platform,
            [NotNull] object categoryID,
            [NotNull] object forumID,
            [NotNull] object topicID,
            [NotNull] object messageID,
            [NotNull] object isCrawler,
            [NotNull] object isMobileDevice,
            [NotNull] object dontTrack)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var tries = 0;
            while (true)
            {
                try
                {
                     return repository.DbFunction.GetAsDataTable(
                          cdb => cdb.pageload(
                              SessionID: sessionID,
                              BoardID: boardId ?? repository.BoardID,
                              UserKey: userKey ?? DBNull.Value,
                              IP: ip,
                              Location: location,
                              ForumPage: forumPage,
                              Browser: browser,
                              Platform: platform,
                              CategoryID: categoryID,
                              ForumID: forumID,
                              TopicID: topicID,
                              MessageID: messageID,
                              IsCrawler: isCrawler,
                              IsMobileDevice: isMobileDevice,
                              DontTrack: dontTrack,
                              UTCTIMESTAMP: DateTime.UtcNow)).GetFirstRow();
                }
                catch (SqlException x)
                {
                    if (x.Number == 1205 && tries < 3)
                    {
                        // Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
                    }
                    else
                    {
                        throw new ApplicationException(
                            $"Sql Exception with error number {x.Number} (Tries={tries})",
                            x);
                    }
                }

                ++tries;
            }
        }

        /// <summary>
        /// Sets the Page Access for the specified user
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board identifier.
        /// </param>
        /// <param name="userId">
        /// The user identifier.
        /// </param>
        /// <param name="isGuest">
        /// The is guest.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable PageAccessAsDataTable(
            this IRepository<ActiveAccess> repository,
            object boardId,
            [NotNull] object userId,
            [NotNull] object isGuest)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetAsDataTable(
                cdb => cdb.pageaccess(
                    BoardID: boardId ?? repository.BoardID,
                    UserID: userId,
                    IsGuest: isGuest,
                    UTCTIMESTAMP: DateTime.UtcNow));
        }

        #endregion
    }
}