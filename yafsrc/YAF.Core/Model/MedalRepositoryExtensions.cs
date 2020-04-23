/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Model
{
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
        /// Lists medal(s) assigned to the group
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="groupID">
        /// ID of group of which to list medals.
        /// </param>
        /// <param name="medalID">
        /// ID of medal to list.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable GroupMedalListAsDataTable(
            this IRepository<Medal> repository,
            [NotNull] object groupID,
            [NotNull] object medalID)
        {
            return repository.DbFunction.GetData.group_medal_list(GroupID: groupID, MedalID: medalID);
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

            if (success <= 0)
            {
                return false;
            }

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

        #endregion
    }
}