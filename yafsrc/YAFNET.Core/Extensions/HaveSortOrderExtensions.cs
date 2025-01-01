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

using System.Collections.Generic;

/// <summary>
/// The have sort order extensions.
/// </summary>
public static class HaveSortOrderExtensions
{
    /// <summary>
    /// By the optional sort order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sortEnumerable">The sort enumerable.</param>
    /// <param name="defaultSortOrder">The default sort order.</param>
    /// <returns>IEnumerable&lt;T&gt;.</returns>
    public static IEnumerable<T> ByOptionalSortOrder<T>(
        this IEnumerable<T> sortEnumerable,
        int defaultSortOrder = 1000)
    {
        return sortEnumerable.Select(
            m =>
                {
                    var sortOrder = defaultSortOrder;

                    if (m is IHaveSortOrder haveSortOrder)
                    {
                        sortOrder = haveSortOrder.SortOrder;
                    }

                    return new KeyValuePair<int, T>(sortOrder, m);
                }).OrderByDescending(m => m.Key).Select(m => m.Value);
    }
}