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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    public static class GroupRepositoryExtensions
    {
        #region Public Methods and Operators

        public static void Delete(this IRepository<Group> repository, int groupId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.group_delete(GroupID: groupId);

            repository.FireDeleted(groupId);
        }

        public static DataTable ListAsTable(this IRepository<Group> repository, int? groupId = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.group_list(BoardID: boardId ?? repository.BoardID, GroupID: groupId);
        }


        public static IList<Group> List (this IRepository<Group> repository, int? groupId = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return groupId.HasValue
                       ? repository.Get(g => g.BoardID == boardId && g.ID == groupId.Value)
                       : repository.Get(g => g.BoardID == boardId).OrderBy(o => o.SortOrder)
                           .ToList();
        }

        public static DataTable Member(this IRepository<Group> repository, int userID, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.group_member(BoardID: boardId ?? repository.BoardID, UserID: userID);
        }

        public static DataTable RankStyle(this IRepository<Group> repository, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.group_rank_style(BoardID: boardId ?? repository.BoardID);
        }

        public static void Save(
            this IRepository<Group> repository,
            int groupID,
            string name,
            bool isAdmin,
            bool isStart,
            bool isModerator,
            bool isGuest,
            int accessMaskID,
            int pMLimit,
            string style,
            short sortOrder,
            string description,
            int usrSigChars,
            string usrSigBBCodes,
            string usrSigHTMLTags,
            int usrAlbums,
            int usrAlbumImages,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.group_save(
                GroupID: groupID,
                BoardID: boardId ?? repository.BoardID,
                Name: name,
                IsAdmin: isAdmin,
                IsStart: isStart,
                IsModerator: isModerator,
                IsGuest: isGuest,
                AccessMaskID: accessMaskID,
                PMLimit: pMLimit,
                Style: style,
                SortOrder: sortOrder,
                Description: description,
                UsrSigChars: usrSigChars,
                UsrSigBBCodes: usrSigBBCodes,
                UsrSigHTMLTags: usrSigHTMLTags,
                UsrAlbums: usrAlbums,
                UsrAlbumImages: usrAlbumImages);
        }

        #endregion
    }
}