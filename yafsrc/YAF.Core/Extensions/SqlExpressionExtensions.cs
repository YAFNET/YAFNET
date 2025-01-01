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

namespace YAF.Core.Extensions;

using System;

/// <summary>
/// The SQL Expression Extensions
/// </summary>
public static class SqlExpressionExtensions
{
    /// <summary>
    /// Pages the specified page.
    /// </summary>
    /// <typeparam name="T">The type parameter</typeparam>
    /// <param name="expression">The expression.</param>
    /// <param name="page">The page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <returns>Adds the Limit to the Sql Expression</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// page - Page must be a number greater than 0.
    /// or
    /// pageSize - PageSize must be a number greater than 0.
    /// </exception>
    public static SqlExpression<T> Page<T>(this SqlExpression<T> expression, int? page, int? pageSize)
    {
        if (!page.HasValue || !pageSize.HasValue)
        {
            return expression;
        }

        if (page <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be a number greater than 0.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be a number greater than 0.");
        }

        var skip = (page.Value - 1) * pageSize.Value;
        var take = pageSize.Value;

        return expression.Limit(skip, take);
    }
}