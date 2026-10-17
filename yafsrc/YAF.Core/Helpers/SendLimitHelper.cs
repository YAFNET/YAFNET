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

namespace YAF.Core.Helpers;

using System;
using System.Collections.Generic;
using System.Runtime.Caching;

/// <summary>
/// Limits how often a user can do an action (e.g. send emails) in a time window.
/// </summary>
public static class SendLimitHelper
{
    /// <summary>
    /// The lock for the counters.
    /// </summary>
    private static readonly object CounterLock = new();

    /// <summary>
    /// Counts the action in the default memory cache, if the limit of the time window is not reached yet.
    /// </summary>
    /// <param name="key">The key of the action and the user.</param>
    /// <param name="maxCount">How often the action is allowed in the time window.</param>
    /// <param name="window">The time window.</param>
    /// <returns>Returns false if the limit is reached (the action is not counted then).</returns>
    public static bool TryCount(string key, int maxCount, TimeSpan window)
    {
        return TryCount(MemoryCache.Default, key, maxCount, window);
    }

    /// <summary>
    /// Counts the action, if the limit of the time window is not reached yet.
    /// </summary>
    /// <param name="cache">The memory cache that keeps the counters.</param>
    /// <param name="key">The key of the action and the user.</param>
    /// <param name="maxCount">How often the action is allowed in the time window.</param>
    /// <param name="window">The time window.</param>
    /// <returns>Returns false if the limit is reached (the action is not counted then).</returns>
    public static bool TryCount(ObjectCache cache, string key, int maxCount, TimeSpan window)
    {
        lock (CounterLock)
        {
            var now = DateTime.UtcNow;

            var times = cache.Get(key) as List<DateTime> ?? new List<DateTime>();

            times.RemoveAll(time => time <= now - window);

            if (times.Count >= maxCount)
            {
                return false;
            }

            times.Add(now);

            // kept until the newest action leaves the time window
            cache.Set(key, times, new DateTimeOffset(now + window));

            return true;
        }
    }
}
