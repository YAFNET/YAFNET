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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace YAF.Core.Hubs;

/// <summary>
/// The session of a hub connection without session: the long polling transport copies the first request of
/// the connection without its session. It lives as long as the connection, its id is the connection id.
/// </summary>
public class HubConnectionSessionFeature : ISessionFeature
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HubConnectionSessionFeature"/> class.
    /// </summary>
    /// <param name="connectionId">The id of the hub connection.</param>
    public HubConnectionSessionFeature(string connectionId)
    {
        this.Session = new HubConnectionSession(connectionId);
    }

    /// <summary>
    /// Gets or sets the session.
    /// </summary>
    public ISession Session { get; set; }

    /// <summary>
    /// A session in memory, for the time of the connection.
    /// </summary>
    private sealed class HubConnectionSession : ISession
    {
        /// <summary>
        /// The values of the session.
        /// </summary>
        private readonly ConcurrentDictionary<string, byte[]> values = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="HubConnectionSession"/> class.
        /// </summary>
        /// <param name="id">The id of the session.</param>
        public HubConnectionSession(string id)
        {
            this.Id = id;
        }

        /// <inheritdoc />
        public bool IsAvailable => true;

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public IEnumerable<string> Keys => this.values.Keys;

        /// <inheritdoc />
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        /// <inheritdoc />
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        /// <inheritdoc />
        public bool TryGetValue(string key, out byte[] value) => this.values.TryGetValue(key, out value);

        /// <inheritdoc />
        public void Set(string key, byte[] value) => this.values[key] = value;

        /// <inheritdoc />
        public void Remove(string key) => this.values.TryRemove(key, out _);

        /// <inheritdoc />
        public void Clear() => this.values.Clear();
    }
}
