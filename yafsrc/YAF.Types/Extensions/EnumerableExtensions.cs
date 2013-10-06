/* Yet Another Forum.net
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