/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Types.Extensions
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Types;

    #endregion

    public static class EnumerableExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Iterates through a generic list type
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="list"> </param>
        /// <param name="action"> </param>
        public static void ForEach<T>([NotNull] this IEnumerable<T> list, [NotNull] Action<T> action)
        {
            CodeContracts.VerifyNotNull(list, "list");
            CodeContracts.VerifyNotNull(action, "action");

            foreach (var item in list.ToList())
            {
                action(item);
            }
        }

        /// <summary>
        ///     Iterates through a list with a isFirst flag.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="list"> </param>
        /// <param name="action"> </param>
        public static void ForEachFirst<T>([NotNull] this IEnumerable<T> list, [NotNull] Action<T, bool> action)
        {
            CodeContracts.VerifyNotNull(list, "list");
            CodeContracts.VerifyNotNull(action, "action");

            bool isFirst = true;
            foreach (var item in list.ToList())
            {
                action(item, isFirst);
                isFirst = false;
            }
        }

        /// <summary>
        ///     Iterates through a list with a index.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="list"> </param>
        /// <param name="action"> </param>
        public static void ForEachIndex<T>([NotNull] this IEnumerable<T> list, [NotNull] Action<T, int> action)
        {
            CodeContracts.VerifyNotNull(list, "list");
            CodeContracts.VerifyNotNull(action, "action");

            int i = 0;
            foreach (var item in list.ToList())
            {
                action(item, i++);
            }
        }

        /// <summary>
        ///     If the <paramref name="currentEnumerable" /> is <see langword="null" /> , an Empty IEnumerable of <typeparamref
        ///      name="T" /> is returned, else <paramref name="currentEnumerable" /> is returned.
        /// </summary>
        /// <param name="currentEnumerable"> The current enumerable. </param>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        public static IEnumerable<T> IfNullEmpty<T>([CanBeNull] this IEnumerable<T> currentEnumerable)
        {
            if (currentEnumerable == null)
            {
                return Enumerable.Empty<T>();
            }

            return currentEnumerable;
        }

        /// <summary>
        ///     Creates an infinite IEnumerable from the <paramref name="currentEnumerable" /> padding it with default( <typeparamref
        ///      name="T" /> ).
        /// </summary>
        /// <param name="currentEnumerable"> The current enumerable. </param>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        public static IEnumerable<T> Infinite<T>([NotNull] this IEnumerable<T> currentEnumerable)
        {
            CodeContracts.VerifyNotNull(currentEnumerable, "currentEnumerable");

            foreach (var item in currentEnumerable)
            {
                yield return item;
            }

            while (true)
            {
                yield return default(T);
            }
        }

        /// <summary>
        ///     Converts an <see cref="IEnumerable" /> to a HashSet -- similar to ToList()
        /// </summary>
        /// <param name="list"> The list. </param>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        /// <exception cref="ArgumentNullException"></exception>
        [NotNull]
        public static HashSet<T> ToHashSet<T>([NotNull] this IEnumerable<T> list)
        {
            CodeContracts.VerifyNotNull(list, "list");

            return new HashSet<T>(list);
        }

        #endregion
    }
}