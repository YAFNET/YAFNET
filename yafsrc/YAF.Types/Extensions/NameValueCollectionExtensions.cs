/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Types.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// The name value collection extensions.
    /// </summary>
    public static class NameValueCollectionExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the first value of <paramref name="paramName"/> in the collection or default (Null).
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="paramName">
        /// The parameter Name.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFirstOrDefault(
            [NotNull] this NameValueCollection collection, [NotNull] string paramName, IEqualityComparer<string> comparer = null)
        {
            CodeContracts.VerifyNotNull(collection, "collection");
            CodeContracts.VerifyNotNull(paramName, "paramName");

            return collection.ToLookup(comparer)[paramName].FirstOrDefault();
        }

        /// <summary>
        /// Gets the first value of <paramref name="paramName"/> in the collection as T or default (Null).
        /// </summary>
        /// <typeparam name="T">
        /// The typed parameter.
        /// </typeparam>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="paramName">
        /// The parameter Name.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetFirstOrDefaultAs<T>(
            [NotNull] this NameValueCollection collection, [NotNull] string paramName, IEqualityComparer<string> comparer = null)
        {
            CodeContracts.VerifyNotNull(collection, "collection");
            CodeContracts.VerifyNotNull(paramName, "paramName");

            return collection.GetFirstOrDefault(paramName, comparer).ToType<T>();
        }

        /// <summary>
        /// Get Parameter as integer.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="paramName">
        /// The parameter name.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <returns>
        /// Returns the integer Value
        /// </returns>
        public static int? GetFirstOrDefaultAsInt(
            [NotNull] this NameValueCollection collection, [NotNull] string paramName, IEqualityComparer<string> comparer = null)
        {
            CodeContracts.VerifyNotNull(collection, "collection");
            CodeContracts.VerifyNotNull(paramName, "paramName");

            if (collection.GetFirstOrDefault(paramName, comparer) != null && int.TryParse(
                    collection.GetFirstOrDefault(paramName),
                    out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Gets the value as an <see cref="IEnumerable"/> handling splitting the string if needed.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="paramName">
        /// The parameter Name.
        /// </param>
        /// <returns>
        /// Does not return null.
        /// </returns>
        public static IEnumerable<string> GetValueList([NotNull] this NameValueCollection collection, [NotNull] string paramName)
        {
            CodeContracts.VerifyNotNull(collection, "collection");
            CodeContracts.VerifyNotNull(paramName, "paramName");

            return collection[paramName] == null ? Enumerable.Empty<string>() : collection[paramName].Split(',').AsEnumerable();
        }

        /// <summary>
        /// Flattens a <see cref="NameValueCollection"/> to a simple string <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        /// <returns>
        /// The <see cref="ILookup"/>.
        /// </returns>
        [NotNull]
        public static ILookup<string, string> ToLookup([NotNull] this NameValueCollection collection, IEqualityComparer<string> comparer = null)
        {
            CodeContracts.VerifyNotNull(collection, "collection");

            return collection.Cast<string>().ToLookup(key => key, key => collection[key], comparer ?? StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if Element Exists in the collection
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="paramName">
        /// The parameter name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Exists([NotNull] this NameValueCollection collection, [NotNull] string paramName)
        {
            CodeContracts.VerifyNotNull(collection, "collection");
            CodeContracts.VerifyNotNull(paramName, "paramName");

            return collection[paramName] != null;
        }

        #endregion
    }
}