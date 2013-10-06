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
    using System.Collections.Generic;
    using System.Data;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    public static class GroupRepositoryExtensions
    {
        #region Public Methods and Operators

        public static void Delete(this IRepository<Group> repository, int groupID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.group_delete(GroupID: groupID);

            repository.FireDeleted(groupID);
        }

        public static DataTable List(this IRepository<Group> repository, int? groupID = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.group_list(BoardID: boardId ?? repository.BoardID, GroupID: groupID);
        }

        public static IList<Group> ListTyped(this IRepository<Group> repository, int? groupID = null, int? boardId = null)
        {
            using (var session = repository.DbFunction.CreateSession())
            {
                return
                    session.GetTyped<Group>(
                        r => r.group_list(BoardID: boardId ?? repository.BoardID, GroupID: groupID));
            }
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