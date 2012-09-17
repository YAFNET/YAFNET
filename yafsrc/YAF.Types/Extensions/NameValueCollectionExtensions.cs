/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    using YAF.Types;

    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Flattens a <see cref="NameValueCollection"/> to a simple string <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="collection">
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static ILookup<string, string> ToLookup([NotNull] this NameValueCollection collection)
        {
            CodeContracts.ArgumentNotNull(collection, "collection");

            return collection.Cast<string>().ToLookup(key => key, key => collection[key]);
        }

        /// <summary>
        /// Gets the value as an <see cref="IEnumerable"/> handling splitting the string if needed.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>Does not return null.</returns>
        public static IEnumerable<string> GetValueList([NotNull] this NameValueCollection collection, [NotNull] string paramName)
        {
            CodeContracts.ArgumentNotNull(collection, "collection");
            CodeContracts.ArgumentNotNull(paramName, "paramName");

            return collection[paramName] == null ? Enumerable.Empty<string>() : collection[paramName].Split(',').AsEnumerable();
        }

        /// <summary>
        /// Gets the first value of <paramref name="paramName"/> in the collection or default (Null).
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string GetFirstOrDefault([NotNull] this NameValueCollection collection, [NotNull] string paramName)
        {
            CodeContracts.ArgumentNotNull(collection, "collection");
            CodeContracts.ArgumentNotNull(paramName, "paramName");

            return collection.ToLookup()[paramName].FirstOrDefault();
        }

        public static T GetFirstOrDefaultAs<T>([NotNull] this NameValueCollection collection, [NotNull] string paramName)
        {
            CodeContracts.ArgumentNotNull(collection, "collection");
            CodeContracts.ArgumentNotNull(paramName, "paramName");

            return collection.GetFirstOrDefault(paramName).ToType<T>();
        }
    }
}