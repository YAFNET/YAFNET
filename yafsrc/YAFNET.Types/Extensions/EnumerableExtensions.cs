/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
    /// <param name="list"> </param>
    /// <typeparam name="T"> </typeparam>
    extension<T>(IEnumerable<T> list)
    {
        /// <summary>
        ///     Iterates through a generic list type
        /// </summary>
        /// <param name="action"> </param>
        public void ForEach(Action<T> action)
        {
            list.ToList().ForEach(action);
        }

        /// <summary>
        ///     Iterates through a list with a isFirst flag.
        /// </summary>
        /// <param name="action"> </param>
        public void ForEachFirst(Action<T, bool> action)
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
        /// <param name="action"> </param>
        public void ForEachIndex(Action<T, int> action)
        {
            var i = 0;

            list.ToList().ForEach(item => action(item, i++));
        }

        /// <summary>
        ///     If the <paramref name="list" /> is <see langword="null" /> , an Empty IEnumerable of <typeparamref
        ///      name="T" /> is returned, else <paramref name="list" /> is returned.
        /// </summary>
        /// <returns> </returns>
        public IEnumerable<T> IfNullEmpty()
        {
            return list ?? [];
        }

        /// <summary>
        /// Checks if List is Null Or Empty
        /// </summary>
        /// <returns><c>true</c> if Null Or Empty, <c>false</c> otherwise.</returns>
        public bool NullOrEmpty()
        {
            if (list is null)
            {
                return true;
            }

            return !list.Any();
        }
    }

    /// <param name="array">The array.</param>
    /// <typeparam name="T"></typeparam>
    extension<T>(T[] array)
    {
        /// <summary>
        /// Existses the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Exists(Predicate<T> match)
        {
            return Array.Exists(array, match);
        }

        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>T.</returns>
        public T Find(Predicate<T> match)
        {
            return Array.Find(array, match);
        }
    }
}