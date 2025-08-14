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

namespace YAF.Types.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// The i object store.
/// </summary>
public interface IObjectStore : IReadValue<object>, IWriteValue<object>, IRemoveValue
{
    /// <summary>
    ///   The this.
    /// </summary>
    /// <param name = "key">
    ///   The key.
    /// </param>
    object this[string key] { get; set; }

    /// <summary>
    /// Gets all the cache elements of type T as a KeyValuePair Enumerable. If T is object, all object types should be returned.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    IEnumerable<KeyValuePair<string, T>> GetAll<T>();

    /// <summary>
    /// Gets the cache value if it's in the cache or sets it if it doesn't exist or is expired.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <returns>
    /// </returns>
    T GetOrSet<T>(string key, Func<T> getValue);

    /// <summary>
    /// Gets the cache value if it's in the cache or sets it if it doesn't exist or is expired.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <returns>
    /// </returns>
    public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getValue);
}