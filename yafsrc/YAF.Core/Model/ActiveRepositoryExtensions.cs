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

    using YAF.Types;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

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
        public static DataTable ListForum(this IRepository<Active> repository, int forumId, bool styledNicks)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

           /* var expression = OrmLiteConfig.DialectProvider.SqlExpression<Active>();

            expression.Join<User>((a, u) => u.ID == a.UserID).Where(a => a.ForumID == forumId).Select<Active, User>(
                (a, b) => new
                {
                    UserID = a.UserID,
                    UserName = b.Name,
                    UserDisplayName = b.DisplayName,
                    IsHidden = b.IsActiveExcluded,
                    IsCrawler = a.Flags & 8,
                    Style = styledNicks ? b.UserStyle : string.Empty,
                    b.Suspended,
                    UserCount = (SELECT COUNT(ac.UserID) from
                     [{ databaseOwner}].[{ objectQualifier}
         Active] ac where ac.UserID = a.UserID and ac.ForumID = @ForumID),
                    Browser = a.Browser
                }).GroupBy<Active, User>(
                (a, b) => new
                {
                    a.UserID,
                    b.DisplayName,
                    b.Name,
                    b.IsActiveExcluded,
                    b.ID,
                    b.UserStyle,
                    b.Suspended,
                    a.Flags,
                    a.Browser
                }).OrderBy<User>(u => u.Name);

            return repository.DbAccess.Execute(
                db => db.Connection
                    .Select<(int MedalID, string Name, string Message, string MedalURL, string RibbonURL, string
                        SmallMedalURL, string SmallRibbonURL, byte SortOrder, bool Hide, bool OnlyRibbon, int Flags,
                        DateTime DateAwarded)>(expression));*/

            return repository.DbFunction.GetData.active_listforum(ForumID: forumId, StyledNicks: styledNicks);
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
        public static DataTable ListAsDataTable(
            this IRepository<Active> repository,
            bool guests,
            bool showCrawlers,
            int activeTime,
            bool styledNicks,
            int? boardId = null)
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
        public static DataTable ListUserAsDataTable(
            this IRepository<Active> repository,
            int userID,
            bool guests,
            bool showCrawlers,
            int activeTime,
            bool styledNicks,
            int? boardId = null)
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

            return repository.DbFunction.GetAsDataTable(
                f => f.active_stats(BoardID: boardId ?? repository.BoardID)).GetFirstRow();
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

            return repository.DbFunction.GetData.active_updatemaxstats(
                BoardID: boardId ?? repository.BoardID,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        #endregion
    }
}