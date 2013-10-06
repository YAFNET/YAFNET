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

    /// <summary>
    ///     The medal repository extensions.
    /// </summary>
    public static class MedalRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="medalID">
        /// The medal id. 
        /// </param>
        /// <param name="category">
        /// The category. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        public static void Delete(this IRepository<Medal> repository, int medalID, string category = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.medal_delete(BoardID: boardId ?? repository.BoardID, MedalID: medalID, Category: category);
            repository.FireDeleted(medalID);
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="medalID">
        /// The medal id. 
        /// </param>
        /// <param name="category">
        /// The category. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataTable List(this IRepository<Medal> repository, int? medalID = null, string category = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.medal_list(BoardID: boardId ?? repository.BoardID, MedalID: medalID, Category: category);
        }

        /// <summary>
        /// A list of Medals.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="medalID">
        /// The medal id.
        /// </param>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<Medal> ListTyped(this IRepository<Medal> repository, int? medalID = null, string category = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var functionSession = repository.DbFunction.CreateSession())
            {
                return functionSession.GetTyped<Medal>(
                    r => r.medal_list(BoardID: boardId ?? repository.BoardID, MedalID: medalID, Category: category));
            }
        }

        /// <summary>
        /// The listusers.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="medalID">
        /// The medal id. 
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataTable ListUsers(this IRepository<Medal> repository, int medalID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.medal_listusers(MedalID: medalID);
        }

        /// <summary>
        /// The resort.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="medalID">
        /// The medal id. 
        /// </param>
        /// <param name="move">
        /// The move. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        public static void Resort(this IRepository<Medal> repository, int medalID, int move, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.medal_resort(BoardID: boardId ?? repository.BoardID, MedalID: medalID, Move: move);
            repository.FireUpdated(medalID);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="medalID">
        /// The medal id. 
        /// </param>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="description">
        /// The description. 
        /// </param>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="category">
        /// The category. 
        /// </param>
        /// <param name="medalURL">
        /// The medal url. 
        /// </param>
        /// <param name="ribbonURL">
        /// The ribbon url. 
        /// </param>
        /// <param name="smallMedalURL">
        /// The small medal url. 
        /// </param>
        /// <param name="smallRibbonURL">
        /// The small ribbon url. 
        /// </param>
        /// <param name="smallMedalWidth">
        /// The small medal width. 
        /// </param>
        /// <param name="smallMedalHeight">
        /// The small medal height. 
        /// </param>
        /// <param name="smallRibbonWidth">
        /// The small ribbon width. 
        /// </param>
        /// <param name="smallRibbonHeight">
        /// The small ribbon height. 
        /// </param>
        /// <param name="sortOrder">
        /// The sort order. 
        /// </param>
        /// <param name="flags">
        /// The flags. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Save(
            this IRepository<Medal> repository, 
            int? medalID, 
            string name, 
            string description, 
            string message, 
            string category, 
            string medalURL, 
            string ribbonURL, 
            string smallMedalURL, 
            string smallRibbonURL, 
            short smallMedalWidth, 
            short smallMedalHeight, 
            short? smallRibbonWidth, 
            short? smallRibbonHeight, 
            byte sortOrder, 
            int flags, 
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = (int)repository.DbFunction.Scalar.medal_save(
                BoardID: boardId ?? repository.BoardID, 
                MedalID: medalID, 
                Name: name, 
                Description: description, 
                Message: message, 
                Category: category, 
                MedalURL: medalURL, 
                RibbonURL: ribbonURL, 
                SmallMedalURL: smallMedalURL, 
                SmallRibbonURL: smallRibbonURL, 
                SmallMedalWidth: smallMedalWidth, 
                SmallMedalHeight: smallMedalHeight, 
                SmallRibbonWidth: smallRibbonWidth, 
                SmallRibbonHeight: smallRibbonHeight, 
                SortOrder: sortOrder, 
                Flags: flags);

            if (success > 0)
            {
                if (medalID.HasValue)
                {
                    repository.FireUpdated(medalID);
                }
                else
                {
                    repository.FireNew();
                }

                return true;
            }

            return false;
        }

        #endregion
    }
}