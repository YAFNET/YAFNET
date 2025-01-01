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
/// The collection extensions.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// The add or update.
    /// </summary>
    /// <param name="dictionary">
    /// The dictionary.
    /// </param>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    public static void AddOrUpdate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }

    /// <summary>
    /// The add range.
    /// </summary>
    /// <param name="dictionaryFirst">
    /// The dictionary first.
    /// </param>
    /// <param name="dictionarySecondary">
    /// The dictionary secondary.
    /// </param>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    public static void AddRange<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionaryFirst,
        IDictionary<TKey, TValue> dictionarySecondary)
    {
        dictionarySecondary.ToList().ForEach(i => dictionaryFirst.AddOrUpdate(i.Key, i.Value));
    }
}