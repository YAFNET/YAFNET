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

/// <summary>
/// Interface to the cache system.
/// </summary>
public interface IDataCache : IObjectStore
{
    /// <summary>
    /// Gets the cache value if it's in the cache or sets it if it doesn't exist or is expired.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="getValue">
    /// The get value.
    /// </param>
    /// <param name="timeout">
    /// The timeout.
    /// </param>
    /// <returns>
    /// </returns>
    T GetOrSet<T>(string key, Func<T> getValue, TimeSpan timeout);

    /// <summary>
    /// Sets a cache value with a timeout.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="timeout">
    /// The timeout.
    /// </param>
    void Set(string key, object value, TimeSpan timeout);
}