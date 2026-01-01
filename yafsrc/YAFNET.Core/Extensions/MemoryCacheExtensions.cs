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

namespace YAF.Core.Extensions;

using System;
using System.Runtime.Caching;

/// <summary>
/// The MemoryCache extensions.
/// </summary>
public static class MemoryCacheExtensions
{
    /// <param name="cache">
    /// The cache.
    /// </param>
    extension(MemoryCache cache)
    {
        /// <summary>
        /// The get or set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="getValue">
        /// The get value.
        /// </param>
        /// <typeparam name="T">
        /// The typed Parameter
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// The typed Parameter
        /// </returns>
        public T GetOrSet<T>(string key,
            Func<T> getValue)
        {
            ArgumentNullException.ThrowIfNull(cache);
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(getValue);

            var item = cache[key];

            if (!Equals(item, default(T)))
            {
                return (T)item;
            }

            item = cache[key];

            if (!Equals(item, default(T)))
            {
                return (T)item;
            }

            item = getValue();
            cache[key] = item;

            return (T)item;
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// The typed Parameter
        /// </typeparam>
        public void Set<T>(string key, T value)
        {
            ArgumentNullException.ThrowIfNull(cache);
            ArgumentNullException.ThrowIfNull(key);

            try
            {
                cache[key] = value;
            }
            catch (Exception)
            {
                // NOTE : Ignore if board settings is reset!
            }
        }
    }
}