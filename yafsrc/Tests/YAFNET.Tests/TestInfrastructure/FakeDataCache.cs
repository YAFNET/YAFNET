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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.TestInfrastructure;

using System.Threading.Tasks;

using YAF.Types.Interfaces;

/// <summary>
/// A dictionary backed <see cref="IDataCache"/> for unit tests. Unlike HttpRuntimeCache it does not
/// use the process wide MemoryCache.Default, so nothing leaks between tests. Pre-fill it to keep
/// services from falling through to their repository.
/// </summary>
public class FakeDataCache : IDataCache
{
    private readonly Dictionary<string, object> items = new();

    public object this[string key]
    {
        get => this.Get(key);
        set => this.Set(key, value);
    }

    public object Get(string originalKey)
    {
        return this.items.GetValueOrDefault(originalKey);
    }

    public void Set(string key, object value)
    {
        this.items[key] = value;
    }

    public void Set(string key, object value, TimeSpan timeout)
    {
        this.Set(key, value);
    }

    public void Remove(string key)
    {
        this.items.Remove(key);
    }

    public IEnumerable<KeyValuePair<string, T>> GetAll<T>()
    {
        return this.items.Where(i => i.Value is T).Select(i => new KeyValuePair<string, T>(i.Key, (T)i.Value));
    }

    public T GetOrSet<T>(string key, Func<T> getValue)
    {
        if (this.items.TryGetValue(key, out var item) && item is T value)
        {
            return value;
        }

        value = getValue();
        this.items[key] = value;
        return value;
    }

    public T GetOrSet<T>(string key, Func<T> getValue, TimeSpan timeout)
    {
        return this.GetOrSet(key, getValue);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getValue)
    {
        if (this.items.TryGetValue(key, out var item) && item is T value)
        {
            return value;
        }

        value = await getValue();
        this.items[key] = value;
        return value;
    }

    public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getValue, TimeSpan timeout)
    {
        return this.GetOrSetAsync(key, getValue);
    }
}
