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
    using System.Data;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The category repository extensions.
    /// </summary>
    public static class CategoryRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        public static bool Delete(this IRepository<Category> repository, int categoryID)
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            int success = repository.DbFunction.Scalar.category_delete(CategoryID: categoryID);

            if (success != 0)
            {
                repository.FireDeleted(categoryID);

                return true;
            }

            return false;
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable List(this IRepository<Category> repository, int? categoryID = null, int? boardId = null)
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            return repository.DbFunction.GetData.category_list(BoardID: boardId ?? repository.BoardID, CategoryID: categoryID);
        }

        /// <summary>
        /// The listread.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable Listread(this IRepository<Category> repository, int userID, int? categoryID, int? boardId = null)
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            return repository.DbFunction.GetData.category_listread(BoardID: boardId ?? repository.BoardID, UserID: userID, CategoryID: categoryID);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="categoryID">
        /// The category id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sortOrder">
        /// The sort order.
        /// </param>
        /// <param name="categoryImage">
        /// The category image.
        /// </param>
        public static void Save(
            this IRepository<Category> repository, int? categoryID, string name, string categoryImage, short sortOrder, int? boardId = null)
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            int newId = repository.DbFunction.Scalar.category_save(
                BoardID: boardId ?? repository.BoardID, CategoryID: categoryID ?? 0, Name: name, SortOrder: sortOrder, CategoryImage: categoryImage);

            if (categoryID.HasValue)
            {
                repository.FireUpdated(categoryID);
            }
            else
            {
                repository.FireNew(newId);
            }
        }

        /// <summary>
        /// The simplelist.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="startID">
        /// The start id.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable Simplelist(this IRepository<Category> repository, int startID, int limit)
        {
            CodeContracts.ArgumentNotNull(repository, "repository");

            return repository.DbFunction.GetData.category_simplelist(StartID: startID, Limit: limit);
        }

        #endregion
    }
}