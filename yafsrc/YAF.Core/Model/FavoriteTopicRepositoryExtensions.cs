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

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
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

            return repository.Count(f => f.TopicID == topicId).ToType<int>();
        }

        /// <summary>
        /// The favorite remove.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static bool DeleteByUserAndTopic(this IRepository<FavoriteTopic> repository, int userId, int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var count = repository.DbAccess.Execute(
                db => db.Connection.Delete<FavoriteTopic>(x => x.UserID == userId && x.TopicID == topicId));
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

            return repository.DbAccess.Execute(cmd => cmd.Connection.Select<FavoriteTopic>(e => e.UserID == userID));
        }

        #endregion
    }
}