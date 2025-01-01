﻿/* Yet Another Forum.NET
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

namespace YAF.Types.Extensions;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The enumerable extensions.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Iterates through a generic list type
    /// </summary>
    /// <typeparam name="T">The typed generic list</typeparam>
    /// <param name="list"> </param>
    /// <param name="action"> </param>
    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
        list.ToList().ForEach(action);
    }

    /// <summary>
    ///     Iterates through a list with a isFirst flag.
    /// </summary>
    /// <typeparam name="T">
    /// The typed generic list
    /// </typeparam>
    /// <param name="list"> </param>
    /// <param name="action"> </param>
    public static void ForEachFirst<T>(this IEnumerable<T> list, Action<T, bool> action)
    {
        var isFirst = true;

        list.ToList().ForEach(
            item =>
                {
                    action(item, isFirst);
                    isFirst = false;
                });
    }

    /// <summary>
    ///     Iterates through a list with a index.
    /// </summary>
    /// <typeparam name="T">
    /// The typed generic list
    /// </typeparam>
    /// <param name="list"> </param>
    /// <param name="action"> </param>
    public static void ForEachIndex<T>(this IEnumerable<T> list, Action<T, int> action)
    {
        var i = 0;

        list.ToList().ForEach(item => action(item, i++));
    }

    /// <summary>
    ///     If the <paramref name="currentEnumerable" /> is <see langword="null" /> , an Empty IEnumerable of <typeparamref
    ///      name="T" /> is returned, else <paramref name="currentEnumerable" /> is returned.
    /// </summary>
    /// <param name="currentEnumerable"> The current enumerable. </param>
    /// <typeparam name="T">
    /// The typed generic list
    /// </typeparam>
    /// <returns> </returns>
    public static IEnumerable<T> IfNullEmpty<T>(this IEnumerable<T> currentEnumerable)
    {
        return currentEnumerable ?? Enumerable.Empty<T>();
    }

    /// <summary>
    /// The distinct by.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="keySelector">
    /// The key selector.
    /// </param>
    /// <typeparam name="TSource">
    /// </typeparam>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <returns>
    /// The <see cref="IEnumerable"/>.
    /// </returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        var knownKeys = new HashSet<TKey>();
        return source.Where(element => knownKeys.Add(keySelector(element)));
    }

    /// <summary>
    /// Checks if List is Null Or Empty
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <returns><c>true</c> if Null Or Empty, <c>false</c> otherwise.</returns>
    public static bool NullOrEmpty<T>(this IEnumerable<T> source)
    {
        if (source == null)
        {
            return true;
        }

        return !source.Any();
    }

    public static bool Exists<T>(this T[] array, Predicate<T> match)
    {
        return Array.Exists(array, match);
    }

    public static T Find<T>(this T[] array, Predicate<T> match)
    {
        return Array.Find(array, match);
    }
}