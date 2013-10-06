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

    using ServiceStack.OrmLite;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The favorite topic repository extensions.
    /// </summary>
    public static class FavoriteTopicRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The count.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Count(this IRepository<FavoriteTopic> repository, int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return
                repository.DbAccess.Execute(
                    cmd =>
                    cmd.GetScalar<int>("SELECT COUNT(*) FROM " + repository.DbAccess.GetTableName<FavoriteTopic>() + "  WHERE topicId = {0}", topicId));
        }

        /// <summary>
        /// The favorite remove.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static bool DeleteByUserAndTopic(this IRepository<FavoriteTopic> repository, int userID, int topicID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var count = repository.DbAccess.Execute(db => db.Delete<FavoriteTopic>(x => x.UserID == userID && x.TopicID == topicID));
            if (count > 0)
            {
                repository.FireDeleted();
            }

            return count > 0;
        }

        /// <summary>
        /// The favorite details.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <param name="pageUserID">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since date.
        /// </param>
        /// <param name="toDate">
        /// The to date.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="styledNicks">
        /// The styled nicks.
        /// </param>
        /// <param name="findLastRead">
        /// The find last read.
        /// </param>
        /// <param name="boardId">
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable Details(
            this IRepository<FavoriteTopic> repository, 
            int? categoryID, 
            int pageUserID, 
            DateTime sinceDate, 
            DateTime toDate, 
            int pageIndex, 
            int pageSize, 
            bool styledNicks, 
            bool findLastRead, 
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.topic_favorite_details(
                BoardID: boardId ?? repository.BoardID, 
                CategoryID: categoryID, 
                PageUserID: pageUserID, 
                SinceDate: sinceDate, 
                ToDate: toDate, 
                PageIndex: pageIndex, 
                PageSize: pageSize, 
                StyledNicks: styledNicks, 
                FindLastRead: findLastRead);
        }

        /// <summary>
        /// The favorite list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static IList<FavoriteTopic> ListTyped(this IRepository<FavoriteTopic> repository, int userID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbAccess.Execute(cmd => cmd.Select<FavoriteTopic>(e => e.UserID == userID));
        }

        #endregion
    }
}