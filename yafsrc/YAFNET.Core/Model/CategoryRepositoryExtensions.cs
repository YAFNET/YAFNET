/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Model;

using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The category repository extensions.
/// </summary>
public static class CategoryRepositoryExtensions
{
    /// <summary>
    /// Gets the highest sort order.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <returns>Returns the highest sort order.</returns>
    public static int GetHighestSortOrder(this IRepository<Category> repository)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Category>();

        expression.Where(x => x.BoardID == repository.BoardID)
            .Select(c => Sql.Max(c.SortOrder));

        return repository.DbAccess.Execute(
            db => db.Connection.Scalar<int>(expression));
    }

    /// <summary>
    /// List Categories
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="categoryId">
    /// The category id.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    public static List<Category> List(
        this IRepository<Category> repository,
        int? categoryId = null,
        int? boardId = null)
    {
        return categoryId.HasValue
                   ? [.. repository.Get(
                       category => category.BoardID == (boardId ?? repository.BoardID)
                                   && category.ID == categoryId.Value).OrderBy(o => o.SortOrder)]
                   : repository.Get(category => category.BoardID == (boardId ?? repository.BoardID));
    }

    /// <summary>
    /// Save a Category
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="categoryId">
    /// The category id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="categoryImage">
    /// The category image.
    /// </param>
    /// <param name="sortOrder">
    /// The sort order.
    /// </param>
    /// <param name="flags">
    /// The Category Flags
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <returns>
    /// Returns the Category ID of the Updated or new Category
    /// </returns>
    public static int Save(
        this IRepository<Category> repository,
        int? categoryId,
        string name,
        string categoryImage,
        short sortOrder,
        CategoryFlags flags,
        int? boardId = null)
    {
        return repository.Upsert(
            new Category
                {
                    BoardID = boardId ?? repository.BoardID,
                    ID = categoryId ?? 0,
                    Name = name,
                    SortOrder = sortOrder,
                    CategoryImage = categoryImage,
                    Flags = flags.BitValue
                });
    }

    /// <summary>
    /// Re-Order all Categories  By Name Ascending
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="categories">
    /// The categories to be sorted.
    /// </param>
    public static void ReOrderAllAscending(this IRepository<Category> repository, List<Category> categories)
    {
        short sortOrder = 0;

        categories.OrderBy(x => x.Name).ForEach(
            category =>
                {
                    category.SortOrder = sortOrder;

                    repository.Update(category);

                    sortOrder++;
                });
    }

    /// <summary>
    /// Re-Order all Categories By Name Descending
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="categories">
    /// The categories to be sorted.
    /// </param>
    public static void ReOrderAllDescending(this IRepository<Category> repository, List<Category> categories)
    {
        short sortOrder = 0;

        categories.OrderByDescending(x => x.Name).ForEach(
            category =>
                {
                    category.SortOrder = sortOrder;

                    repository.Update(category);

                    sortOrder++;
                });
    }
}