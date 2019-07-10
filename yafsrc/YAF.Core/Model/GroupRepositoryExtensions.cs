/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The group repository extensions.
    /// </summary>
    public static class GroupRepositoryExtensions
    {
        #region Public Methods and Operators

       public static IList<Group> List (this IRepository<Group> repository, int? groupId = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return groupId.HasValue
                       ? repository.Get(g => g.BoardID == boardId && g.ID == groupId.Value)
                       : repository.Get(g => g.BoardID == boardId).OrderBy(o => o.SortOrder)
                           .ToList();
        }

       /// <summary>
       /// The group_member.
       /// </summary>
       /// <param name="boardID">
       /// The board id.
       /// </param>
       /// <param name="userID">
       /// The user id.
       /// </param>
       /// <returns>
       /// </returns>
       public static DataTable MemberAsDataTable(
           this IRepository<Group> repository,
           [NotNull] object boardID,
           [NotNull] object userID)
       {
           return repository.DbFunction.GetData.group_member(BoardID: boardID, UserID: userID);

       }

        /// <summary>
        /// Returns info about all Groups and Rank styles.
        ///   Used in GroupRankStyles cache.
        ///   Usage: LegendID = 1 - Select Groups, LegendID = 2 - select Ranks by Name
        /// </summary>
        /// <param name="boardID">
        /// The board ID.
        /// </param>
        public static DataTable RankStyleAsDataTable(
            this IRepository<Group> repository, [NotNull] object boardID)
        {
            return repository.DbFunction.GetData.group_rank_style(BoardID: boardID);
        }

        /// <summary>
        /// The group_save.
        /// </summary>
        /// <param name="groupID">
        /// The group id.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isAdmin">
        /// The is admin.
        /// </param>
        /// <param name="isGuest">
        /// The is guest.
        /// </param>
        /// <param name="isStart">
        /// The is start.
        /// </param>
        /// <param name="isModerator">
        /// The is moderator.
        /// </param>
        /// <param name="accessMaskID">
        /// The access mask id.
        /// </param>
        /// <param name="pmLimit">
        /// The pm limit.
        /// </param>
        /// <param name="style">
        /// The style.
        /// </param>
        /// <param name="sortOrder">
        /// The sort order.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="usrSigChars">
        /// The usrSigChars defines number of allowed characters in user signature.
        /// </param>
        /// <param name="usrSigBBCodes">
        /// The UsrSigBBCodes.defines comma separated bbcodes allowed for a rank, i.e in a user signature
        /// </param>
        /// <param name="usrSigHTMLTags">
        /// The UsrSigHTMLTags defines comma separated tags allowed for a rank, i.e in a user signature
        /// </param>
        /// <param name="usrAlbums">
        /// The UsrAlbums defines allowed number of albums.
        /// </param>
        /// <param name="usrAlbumImages">
        /// The UsrAlbumImages defines number of images allowed for an album.
        /// </param>
        /// <returns>
        /// The group_save.
        /// </returns>
        public static long Save(
            this IRepository<Group> repository,
            [NotNull] object groupID,
            [NotNull] object boardID,
            [NotNull] object name,
            [NotNull] object isAdmin,
            [NotNull] object isGuest,
            [NotNull] object isStart,
            [NotNull] object isModerator,
            [NotNull] object accessMaskID,
            [NotNull] object pmLimit,
            [NotNull] object style,
            [NotNull] object sortOrder,
            [NotNull] object description,
            [NotNull] object usrSigChars,
            [NotNull] object usrSigBBCodes,
            [NotNull] object usrSigHTMLTags,
            [NotNull] object usrAlbums,
            [NotNull] object usrAlbumImages)
        {
            return repository.DbFunction.Scalar.group_save(
                GroupID: groupID,
                BoardID: boardID,
                Name: name,
                IsAdmin: isAdmin,
                IsGuest: isGuest,
                IsStart: isStart,
                IsModerator: isModerator,
                AccessMaskID: accessMaskID,
                PMLimit: pmLimit,
                Style: style,
                SortOrder: sortOrder,
                Description: description,
                UsrSigChars: usrSigChars,
                UsrSigBBCodes: usrSigBBCodes,
                UsrSigHTMLTags: usrSigHTMLTags,
                UsrAlbums: usrAlbums,
                UsrAlbumImages: usrAlbumImages);
        }

        /// <summary>
        /// Returns a list of access entries for a group.
        /// </summary>
        /// <param name="groupId">
        /// The group Id.
        /// </param>
        /// <param name="eventTypeId">
        /// The event Type Id.
        /// </param>
        /// <returns>Returns a list of access entries for a group.</returns>
        public static DataTable ListEventLogAccessAsTable(
            this IRepository<Group> repository,
            [NotNull] int? boardId)
        {
            return repository.DbFunction.GetData.group_eventlogaccesslist(BoardID: boardId);
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