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

using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

/// <summary>
/// A dictionary backed <see cref="ISession"/> for unit tests.
/// </summary>
public class FakeSession : ISession
{
    private readonly Dictionary<string, byte[]> items = new();

    public bool IsAvailable => true;

    public string Id { get; } = Guid.NewGuid().ToString("N");

    public IEnumerable<string> Keys => this.items.Keys;

    public void Clear()
    {
        this.items.Clear();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        this.items.Remove(key);
    }

    public void Set(string key, byte[] value)
    {
        this.items[key] = value;
    }

    public bool TryGetValue(string key, out byte[] value)
    {
        return this.items.TryGetValue(key, out value);
    }
}
