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
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The extension repository extensions.
    /// </summary>
    public static class FileExtensionRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="extension">
        /// The extension.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable List(this IRepository<FileExtension> repository, string extension = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.extension_list(BoardID: boardId ?? repository.BoardID, Extension: extension);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="extension">
        /// The extension.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<FileExtension> ListTyped(this IRepository<FileExtension> repository, string extension = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var functionSession = repository.DbFunction.CreateSession())
            {
                return functionSession.GetTyped<FileExtension>(r => r.extension_list(BoardID: boardId ?? repository.BoardID, Extension: extension));
            }
        }

        #endregion
    }
}