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
    #region Using

    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    ///     The smiley repository extensions.
    /// </summary>
    public static class SmileyRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="smileyID">
        /// The smiley id. 
        /// </param>
        public static void Delete(this IRepository<Smiley> repository, int? smileyID = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.smiley_delete(SmileyID: smileyID);
            repository.FireDeleted(smileyID);
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="smileyID">
        /// The smiley id. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataTable List(this IRepository<Smiley> repository, int? smileyID = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.smiley_list(BoardID: boardId ?? repository.BoardID, SmileyID: smileyID);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="smileyID">
        /// The smiley id. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/> . 
        /// </returns>
        public static IList<Smiley> ListTyped(
            this IRepository<Smiley> repository, int? smileyID = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return smileyID.HasValue
                           ? session.GetTyped<Smiley>(
                               r => r.smiley_list(BoardID: boardId ?? repository.BoardID, SmileyID: smileyID.Value))
                           : repository.GetByBoardID(boardId);
            }
        }

        /// <summary>
        /// The list unique.
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
        public static DataTable ListUnique(this IRepository<Smiley> repository, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.smiley_listunique(BoardID: boardId ?? repository.BoardID);
        }

        /// <summary>
        /// The list unique typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<Smiley> ListUniqueTyped(this IRepository<Smiley> repository, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.ListTyped(boardId: boardId)
                .GroupBy(s => new { s.Icon, s.Emoticon })
                .Select(x => x.FirstOrDefault())
                .OrderBy(s => s.SortOrder)
                .ThenBy(s => s.Code)
                .ToList();
        }

        /// <summary>
        /// The resort.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="smileyID">
        /// The smiley id. 
        /// </param>
        /// <param name="move">
        /// The move. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        public static void Resort(this IRepository<Smiley> repository, int smileyID, int move, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.smiley_resort(BoardID: boardId ?? repository.BoardID, SmileyID: smileyID, Move: move);
            repository.FireUpdated(smileyID);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="smileyID">
        /// The smiley id. 
        /// </param>
        /// <param name="code">
        /// The code. 
        /// </param>
        /// <param name="icon">
        /// The icon. 
        /// </param>
        /// <param name="emoticon">
        /// The emoticon. 
        /// </param>
        /// <param name="sortOrder">
        /// The sort order. 
        /// </param>
        /// <param name="replace">
        /// The replace. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        public static void Save(
            this IRepository<Smiley> repository, 
            int? smileyID, 
            string code, 
            string icon, 
            string emoticon, 
            byte sortOrder, 
            short replace, 
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.smiley_save(
                SmileyID: smileyID, 
                BoardID: boardId ?? repository.BoardID, 
                Code: code, 
                Icon: icon, 
                Emoticon: emoticon, 
                SortOrder: sortOrder, 
                Replace: replace);

            if (smileyID.HasValue)
            {
                repository.FireUpdated(smileyID);
            }
            else
            {
                repository.FireNew();
            }
        }

        /// <summary>
        /// Save the instance of the smiley object.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="smiley">
        /// The smiley. 
        /// </param>
        public static void Save(this IRepository<Smiley> repository, Smiley smiley)
        {
            CodeContracts.VerifyNotNull(repository, "repository");
            CodeContracts.VerifyNotNull(smiley, "smiley");

            if (smiley.BoardID == 0)
            {
                smiley.BoardID = repository.BoardID;
            }

            repository.DbFunction.Query.smiley_save(smiley);

            if (smiley.ID > 0)
            {
                repository.FireUpdated(smiley.ID, smiley);
            }
            else
            {
                repository.FireNew(null, smiley);
            }
        }

        #endregion
    }
}