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

namespace YAF.Types.Interfaces;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The object store extensions.
/// </summary>
public static class IObjectStoreExtensions
{
    /// <param name="objectStore">
    /// The object store.
    /// </param>
    extension(IObjectStore objectStore)
    {
        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="originalKey">
        /// The original key.
        /// </param>
        /// <typeparam name="T">
        /// The Typed Parameter
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Get<T>(string originalKey)
        {
            var item = objectStore.Get(originalKey);

            if (item is T item1)
            {
                return item1;
            }

            return default;
        }

        /// <summary>
        /// The remote all.
        /// </summary>
        /// <typeparam name="T">
        /// The Typed Parameter
        /// </typeparam>
        public void RemoveOf<T>()
        {
            objectStore.GetAll<T>().ToList().ForEach(i => objectStore.Remove(i.Key));
        }

        /// <summary>
        /// The remote all where.
        /// </summary>
        /// <param name="whereFunc">
        /// The where function.
        /// </param>
        /// <typeparam name="T">
        /// The Typed Parameter
        /// </typeparam>
        public void RemoveOf<T>(Func<KeyValuePair<string, T>, bool> whereFunc)
        {
            objectStore.GetAll<T>().Where(whereFunc).ToList().ForEach(i => objectStore.Remove(i.Key));
        }

        /// <summary>
        /// Clear the entire cache.
        /// </summary>
        public void Clear()
        {
            // remove all objects in the cache...
            objectStore.RemoveOf<object>();
        }

        /// <summary>
        /// Count of objects in the cache.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Count()
        {
            return objectStore.GetAll<object>().Count();
        }

        /// <summary>
        /// Count of T in the cache.
        /// </summary>
        /// <typeparam name="T">
        /// The Typed Parameter
        /// </typeparam>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CountOf<T>()
        {
            // remove all objects in the cache...
            return objectStore.GetAll<T>().Count();
        }

        /// <summary>
        /// The remote all where.
        /// </summary>
        /// <param name="whereFunc">
        /// The where function.
        /// </param>
        public void Remove(Func<string, bool> whereFunc)
        {
            objectStore.GetAll<object>().Where(k => whereFunc(k.Key)).ToList().ForEach(i => objectStore.Remove(i.Key));
        }
    }
}