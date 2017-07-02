/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2017 Ingo Herbote
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

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The banned name repository extensions.
    /// </summary>
    public static class BannedNameRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mask">The mask.</param>
        /// <param name="id">The id.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="boardId">The board Id.</param>
        /// <returns>
        /// The <see cref="DataTable" /> .
        /// </returns>
        public static DataTable List(
            this IRepository<BannedName> repository,
            string mask = null,
            int? id = null,
            int? pageIndex = 0,
            int? pageSize = 1000000,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.bannedname_list(
                BoardID: boardId ?? repository.BoardID,
                Mask: mask,
                ID: id,
                PageIndex: pageIndex,
                PageSize: pageSize);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mask">The mask.</param>
        /// <param name="id">The id.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="boardId">The board id.</param>
        /// <returns>
        /// The <see cref="IList" />.
        /// </returns>
        public static IList<BannedName> ListTyped(
            this IRepository<BannedName> repository,
            string mask = null,
            int? id = null,
            int pageIndex = 0,
            int pageSize = 1000000,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return
                    session.GetTyped<BannedName>(
                        r =>
                        r.bannedname_list(
                            BoardID: boardId ?? repository.BoardID,
                            Mask: mask,
                            ID: id,
                            PageIndex: pageIndex,
                            PageSize: pageSize));
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="mask">
        /// The mask. 
        /// </param>
        /// <param name="reason">
        /// The reason. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        public static void Save(
            this IRepository<BannedName> repository,
            int? id,
            string mask,
            string reason,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.bannedname_save(
                ID: id,
                BoardID: boardId ?? repository.BoardID,
                Mask: mask,
                Reason: reason,
                UTCTIMESTAMP: DateTime.UtcNow);

            if (id.HasValue)
            {
                repository.FireUpdated(id);
            }
            else
            {
                repository.FireNew();
            }
        }

        #endregion
    }
}