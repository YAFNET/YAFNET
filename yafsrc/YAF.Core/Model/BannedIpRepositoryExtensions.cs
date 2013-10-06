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
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The banned ip repository extensions.
    /// </summary>
    public static class BannedIpRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index. 
        /// </param>
        /// <param name="pageSize">
        /// The page size. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataTable List(
            this IRepository<BannedIP> repository, int? id = null, int? pageIndex = 0, int? pageSize = 1000000, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.bannedip_list(
                BoardID: boardId ?? repository.BoardID, ID: id, PageIndex: pageIndex, PageSize: pageSize);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<BannedIP> ListTyped(
            this IRepository<BannedIP> repository, int? id = null, int pageIndex = 0, int pageSize = 1000000, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return
                    session.GetTyped<BannedIP>(
                        r => r.bannedip_list(BoardID: boardId ?? repository.BoardID, ID: id, PageIndex: pageIndex, PageSize: pageSize));
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
        /// <param name="userID">
        /// The user id. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        public static void Save(this IRepository<BannedIP> repository, int? id, string mask, string reason, int userID, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.bannedip_save(
                ID: id, BoardID: boardId ?? repository.BoardID, Mask: mask, Reason: reason, UserID: userID, UTCTIMESTAMP: DateTime.UtcNow);

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