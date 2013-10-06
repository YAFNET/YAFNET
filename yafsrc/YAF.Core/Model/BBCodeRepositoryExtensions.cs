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
    ///     The bb code repository extensions.
    /// </summary>
    public static class BBCodeRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="bBCodeID">
        /// The b b code id. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> . 
        /// </returns>
        public static DataTable List(this IRepository<BBCode> repository, int? bBCodeID = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.bbcode_list(BoardID: boardId ?? repository.BoardID, BBCodeID: bBCodeID);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="bBCodeID">
        /// The b b code id.
        /// </param>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<BBCode> ListTyped(this IRepository<BBCode> repository, int? bBCodeID = null, int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return session.GetTyped<BBCode>(r => r.bbcode_list(BoardID: boardId ?? repository.BoardID, BBCodeID: bBCodeID));
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="repository">
        /// The repository. 
        /// </param>
        /// <param name="bBCodeID">
        /// The b b code id. 
        /// </param>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="description">
        /// The description. 
        /// </param>
        /// <param name="onClickJS">
        /// The on click js. 
        /// </param>
        /// <param name="displayJS">
        /// The display js. 
        /// </param>
        /// <param name="editJS">
        /// The edit js. 
        /// </param>
        /// <param name="displayCSS">
        /// The display css. 
        /// </param>
        /// <param name="searchRegEx">
        /// The search reg ex. 
        /// </param>
        /// <param name="replaceRegEx">
        /// The replace reg ex. 
        /// </param>
        /// <param name="variables">
        /// The variables. 
        /// </param>
        /// <param name="useModule">
        /// The use module. 
        /// </param>
        /// <param name="moduleClass">
        /// The module class. 
        /// </param>
        /// <param name="execOrder">
        /// The exec order. 
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        public static void Save(
            this IRepository<BBCode> repository, 
            int? bBCodeID, 
            string name, 
            string description, 
            string onClickJS, 
            string displayJS, 
            string editJS, 
            string displayCSS, 
            string searchRegEx, 
            string replaceRegEx, 
            string variables, 
            bool? useModule, 
            string moduleClass, 
            int execOrder, 
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.bbcode_save(
                BBCodeID: bBCodeID, 
                BoardID: boardId ?? repository.BoardID, 
                Name: name, 
                Description: description, 
                OnClickJS: onClickJS, 
                DisplayJS: displayJS, 
                EditJS: editJS, 
                DisplayCSS: displayCSS, 
                SearchRegEx: searchRegEx, 
                ReplaceRegEx: replaceRegEx, 
                Variables: variables, 
                UseModule: useModule, 
                ModuleClass: moduleClass, 
                ExecOrder: execOrder);

            if (bBCodeID.HasValue)
            {
                repository.FireUpdated(bBCodeID);
            }
            else
            {
                repository.FireNew();
            }
        }

        #endregion
    }
}