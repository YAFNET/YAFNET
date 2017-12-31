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
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The access mask repository extensions.
    /// </summary>
    public static class AccessMaskRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="accessMaskId">The access mask id.</param>
        /// <param name="name">The name.</param>
        /// <param name="readAccess">The read access.</param>
        /// <param name="postAccess">The post access.</param>
        /// <param name="replyAccess">The reply access.</param>
        /// <param name="priorityAccess">The priority access.</param>
        /// <param name="pollAccess">The poll access.</param>
        /// <param name="voteAccess">The vote access.</param>
        /// <param name="moderatorAccess">The moderator access.</param>
        /// <param name="editAccess">The edit access.</param>
        /// <param name="deleteAccess">The delete access.</param>
        /// <param name="uploadAccess">The upload access.</param>
        /// <param name="downloadAccess">The download access.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="boardId">The board id.</param>
        public static void Save(
            this IRepository<AccessMask> repository,
            int? accessMaskId,
            string name,
            bool readAccess,
            bool postAccess,
            bool replyAccess,
            bool priorityAccess,
            bool pollAccess,
            bool voteAccess,
            bool moderatorAccess,
            bool editAccess,
            bool deleteAccess,
            bool uploadAccess,
            bool downloadAccess,
            short sortOrder,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var flags = new AccessFlags
                            {
                                ReadAccess = readAccess,
                                PostAccess = postAccess,
                                ReplyAccess = replyAccess,
                                PriorityAccess = priorityAccess,
                                PollAccess = pollAccess,
                                VoteAccess = voteAccess,
                                ModeratorAccess = moderatorAccess,
                                EditAccess = editAccess,
                                DeleteAccess = deleteAccess,
                                UploadAccess = uploadAccess,
                                DownloadAccess = downloadAccess
                            };

            repository.Upsert(
                new AccessMask
                    {
                        BoardID = boardId ?? repository.BoardID,
                        ID = accessMaskId ?? 0,
                        Name = name,
                        Flags = flags.BitValue,
                        SortOrder = sortOrder
                    });
        }

        #endregion
    }
}