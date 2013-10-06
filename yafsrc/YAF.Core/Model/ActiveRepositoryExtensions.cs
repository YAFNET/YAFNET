/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;

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
        /// The listforum.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="forumID">
        /// The forum id. 
        /// </param>
        /// <param name="styledNicks">
        /// The styled nicks. 
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataTable ListForum(this IRepository<Active> repository, int forumID, bool styledNicks)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.active_listforum(ForumID: forumID, StyledNicks: styledNicks);
        }

        /// <summary>
        /// The listtopic.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="topicID">
        /// The topic id. 
        /// </param>
        /// <param name="styledNicks">
        /// The styled nicks. 
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataTable ListTopic(this IRepository<Active> repository, int topicID, bool styledNicks)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.active_listtopic(TopicID: topicID, StyledNicks: styledNicks);
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
            this IRepository<Active> repository, bool guests, bool showCrawlers, int activeTime, bool styledNicks, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var functionSession = repository.DbFunction.CreateSession())
            {
                return functionSession.GetTyped<Active>(
                    r => r.active_list(
                        BoardID: boardId ?? repository.BoardID, 
                        Guests: guests, 
                        ShowCrawlers: showCrawlers, 
                        ActiveTime: activeTime, 
                        StyledNicks: styledNicks, 
                        UTCTIMESTAMP: DateTime.UtcNow));
            }
        }

        /// <summary>
        /// The list user.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="userID">
        /// The user id. 
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
        /// The updatemaxstats.
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
        public static DataTable UpdateMaxStats(this IRepository<Active> repository, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.active_updatemaxstats(BoardID: boardId ?? repository.BoardID, UTCTIMESTAMP: DateTime.UtcNow);
        }

        #endregion
    }
}