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

using YAF.Types.Interfaces.Events;

/// <summary>
/// An <see cref="IRaiseEvent"/> that runs no handlers and
/// records every raised event, so tests can assert e.g. cache invalidation events.
/// </summary>
public class RecordingEventRaiser : IRaiseEvent
{
    private readonly List<object> events = [];

    /// <summary>
    /// Gets all raised events, in order.
    /// </summary>
    public IReadOnlyList<object> Events => this.events;

    /// <summary>
    /// Gets or sets an action that runs for every raised event (e.g. to change data while the code under test runs).
    /// </summary>
    public Action<object> OnRaise { get; set; }

    /// <summary>
    /// Gets the raised events of the given type.
    /// </summary>
    public IEnumerable<T> OfType<T>()
    {
        return this.events.OfType<T>();
    }

    public void Raise<T>(T eventObject)
        where T : IAmEvent
    {
        this.events.Add(eventObject);
        this.OnRaise?.Invoke(eventObject);
    }

    public void RaiseIssolated<T>(T eventObject, Action<string, Exception> logExceptionAction)
        where T : IAmEvent
    {
        this.events.Add(eventObject);
        this.OnRaise?.Invoke(eventObject);
    }

}
