/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The active repository extensions.
    /// </summary>
    public static class ActiveRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Lists the forum.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="forumID">The forum id.</param>
        /// <param name="styledNicks">The styled nicks.</param>
        /// <returns>
        /// The <see cref="DataTable" /> .
        /// </returns>
        public static DataTable ListForum(this IRepository<Active> repository, int forumID, bool styledNicks)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.active_listforum(ForumID: forumID, StyledNicks: styledNicks);
        }

        /// <summary>
        /// Lists the topic.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicID">The topic id.</param>
        /// <param name="styledNicks">The styled nicks.</param>
        /// <returns>
        /// The <see cref="DataTable" /> .
        /// </returns>
        public static DataTable ListTopic(this IRepository<Active> repository, int topicID, bool styledNicks)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.active_listtopic(TopicID: topicID, StyledNicks: styledNicks);
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="guests">
        /// The guests. 
        /// </param>
        /// <param name="showCrawlers">
        /// The show crawlers. 
        /// </param>
        /// <param name="activeTime">
        /// The active time. 
        /// </param>
        /// <param name="styledNicks">
        /// The styled nicks. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataTable List(
            this IRepository<Active> repository, bool guests, bool showCrawlers, int activeTime, bool styledNicks, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.active_list(
                BoardID: boardId ?? repository.BoardID,
                Guests: guests,
                ShowCrawlers: showCrawlers,
                ActiveTime: activeTime,
                StyledNicks: styledNicks,
                UTCTIMESTAMP: DateTime.UtcNow);
        }
        
        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="guests">
        /// The guests.
        /// </param>
        /// <param name="showCrawlers">
        /// The show crawlers.
        /// </param>
        /// <param name="activeTime">
        /// The active time.
        /// </param>
        /// <param name="styledNicks">
        /// The styled nicks.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<Active> ListTyped(
            this IRepository<Active> repository,
            bool guests,
            bool showCrawlers,
            int activeTime,
            bool styledNicks,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.SqlList(
                "active_list",
                new
                    {
                        BoardID = boardId ?? repository.BoardID,
                        Guests = guests,
                        ShowCrawlers = showCrawlers,
                        ActiveTime = activeTime,
                        StyledNicks = styledNicks,
                        UTCTIMESTAMP = DateTime.UtcNow
                    });
        }

        /// <summary>
        /// Lists the active user.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userID">The user id.</param>
        /// <param name="guests">The guests.</param>
        /// <param name="showCrawlers">The show crawlers.</param>
        /// <param name="activeTime">The active time.</param>
        /// <param name="styledNicks">The styled nicks.</param>
        /// <param name="boardId">The board Id.</param>
        /// <returns>
        /// The <see cref="DataTable" /> .
        /// </returns>
        public static DataTable ListUser(
            this IRepository<Active> repository, int userID, bool guests, bool showCrawlers, int activeTime, bool styledNicks, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.active_list_user(
                BoardID: boardId ?? repository.BoardID, 
                UserID: userID, 
                Guests: guests, 
                ShowCrawlers: showCrawlers, 
                ActiveTime: activeTime, 
                StyledNicks: styledNicks);
        }

        /// <summary>
        /// The stats.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataRow Stats(this IRepository<Active> repository, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return ((DataTable)repository.DbFunction.GetData.active_stats(BoardID: boardId ?? repository.BoardID)).Rows[0];
        }

        /// <summary>
        /// Updates the maximum stats.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="boardId">The board Id.</param>
        /// <returns>
        /// The <see cref="DataTable" /> .
        /// </returns>
        public static DataTable UpdateMaxStats(this IRepository<Active> repository, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.active_updatemaxstats(BoardID: boardId ?? repository.BoardID, UTCTIMESTAMP: DateTime.UtcNow);
        }

        #endregion
    }
}