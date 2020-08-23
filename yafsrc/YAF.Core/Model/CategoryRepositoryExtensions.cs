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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The category repository extensions.
    /// </summary>
    public static class CategoryRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the highest sort order.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <returns>Returns the highest sort order.</returns>
        public static int GetHighestSortOrder(this IRepository<Category> repository)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbAccess.Execute(
                cmd => cmd.Connection.Scalar<int>(
                    $"select MAX(SortOrder) from {repository.DbAccess.GetTableName<Category>()}"));
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="categoryId">The category id.</param>
        /// <param name="boardId">The board id.</param>
        /// <returns>
        /// The <see cref="DataTable" />.
        /// </returns>
        public static List<Category> List(
            this IRepository<Category> repository,
            int? categoryId = null,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository);

            return categoryId.HasValue
                       ? repository.Get(
                           category => category.BoardID == (boardId ?? repository.BoardID)
                                       && category.ID == categoryId.Value).OrderBy(o => o.SortOrder).ToList()
                       : repository.Get(category => category.BoardID == (boardId ?? repository.BoardID));
        }

        /// <summary>
        /// Get Categories as data table.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable ListAsDataTable(
            this IRepository<Category> repository,
            [CanBeNull] int? categoryId = null)
        {
            CodeContracts.VerifyNotNull(repository);

            return repository.DbFunction.GetAsDataTable(
                x => x.category_list(repository.BoardID, categoryId));
        }

        /// <summary>
        /// Save a Category
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="categoryId">The category id.</param>
        /// <param name="name">The name.</param>
        /// <param name="categoryImage">The category image.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="boardId">The board id.</param>
        public static void Save(
            this IRepository<Category> repository,
            int? categoryId,
            string name,
            string categoryImage,
            short sortOrder,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.Upsert(
                new Category
                {
                    BoardID = boardId ?? repository.BoardID,
                    ID = categoryId ?? 0,
                    Name = name,
                    SortOrder = sortOrder,
                    CategoryImage = categoryImage
                });
        }

        #endregion
    }
}