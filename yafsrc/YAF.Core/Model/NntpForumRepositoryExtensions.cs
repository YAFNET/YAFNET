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
    #region Using

    using System;
    using System.Collections.Generic;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    ///     The NntpForum repository extensions.
    /// </summary>
    public static class NntpForumRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="nntpForumId">
        /// The nntp forum id.
        /// </param>
        /// <param name="lastMessageNo">
        /// The last message no.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public static void Update(this IRepository<NntpForum> repository, [NotNull] object nntpForumId, [NotNull] object lastMessageNo, [NotNull] object userId)
        {

            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.nntpforum_update(
                NntpForumID: nntpForumId,
                LastMessageNo: lastMessageNo,
                UserID: userId,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// The nntpforum_list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="minutes">
        /// The minutes.
        /// </param>
        /// <param name="nntpForumID">
        /// The nntp forum id.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<TypedNntpForum> NntpForumList(this IRepository<NntpForum> repository, int boardId, int? minutes, int? nntpForumID, bool? active)
        {
            return repository.DbFunction
                .GetAsDataTable(
                    cdb => cdb.nntpforum_list(
                        BoardID: boardId,
                        Minutes: minutes,
                        NntpForumID: nntpForumID,
                        Active: active,
                        UTCTIMESTAMP: DateTime.UtcNow)).SelectTypedList(t => new TypedNntpForum(t));
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="nntpForumId">
        /// The nntp forum id.
        /// </param>
        /// <param name="nntpServerId">
        /// The nntp server id.
        /// </param>
        /// <param name="groupName">
        /// The group name.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <param name="datecutoff">
        /// The datecutoff.
        /// </param>
        public static void Save(
            this IRepository<NntpForum> repository,
            [NotNull] int? nntpForumId,
            [NotNull] int nntpServerId,
            [NotNull] string groupName,
            [NotNull] int forumID,
            [NotNull] bool active,
            [NotNull] DateTime? datecutoff)
        {
            if (nntpForumId.HasValue)
            {
                repository.UpdateOnly(
                    () => new NntpForum
                              {
                                  NntpServerID = nntpServerId,
                                  GroupName = groupName,
                                  ForumID = forumID,
                                  Active = active,
                                  DateCutOff = datecutoff
                              },
                    n => n.ID == nntpForumId);
            }
            else
            {
                var entity = new NntpForum
                                 {
                                     NntpServerID = nntpServerId,
                                     GroupName = groupName,
                                     ForumID = forumID,
                                     Active = active,
                                     DateCutOff = datecutoff,
                                     LastUpdate = DateTime.UtcNow,
                                     LastMessageNo = 0,
                                 };

                repository.Insert(entity);

                repository.FireNew(entity);
            }
        }

        #endregion
    }
}