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

namespace YAF.Core.Middleware;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Options;

using YAF.Core.Helpers;

/// <summary>
/// Search engines and bots get no session when "Abandon Sessions for Don't Track Users" is on: a crawler
/// does not send the session cookie back, every request started a new session that was kept in the memory
/// (with the forum list of the index, the page data of a topic...) until it expired. A request of a crawler
/// without a session cookie gets a session that only lives for the request, nothing is stored and no session
/// cookie is sent (like the abandoned sessions of YAF.NET v3). A crawler that keeps the cookie has its session.
/// </summary>
public class CrawlerSession : IHaveServiceLocator
{
    /// <summary>
    /// The request delegate.
    /// </summary>
    private readonly RequestDelegate requestDelegate;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrawlerSession"/> class.
    /// </summary>
    /// <param name="next">The next.</param>
    /// <param name="serviceLocator">The service locator.</param>
    public CrawlerSession(RequestDelegate next, IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
        this.requestDelegate = next;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The invoke.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The <see cref="Task"/>.</returns>
    public Task InvokeAsync(HttpContext context)
    {
        if (this.IsNewCrawlerSession(context))
        {
            context.Features.Set<ISessionFeature>(new SessionFeature { Session = new RequestSession() });
        }

        return this.requestDelegate(context);
    }

    /// <summary>
    /// Is the request one of a crawler without a session (and its session is not kept)?
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>True if the request gets a session only for the request.</returns>
    private bool IsNewCrawlerSession(HttpContext context)
    {
        if (context.Features.Get<ISessionFeature>() is null)
        {
            return false;
        }

        try
        {
            if (!this.Get<BoardSettings>().AbandonSessionsForDontTrack)
            {
                return false;
            }
        }
        catch (Exception)
        {
            // the board is not installed yet
            return false;
        }

        var cookieName = this.Get<IOptions<SessionOptions>>().Value.Cookie.Name;

        return !context.Request.Cookies.ContainsKey(cookieName)
               && UserAgentHelper.SearchEngineSpiderName(context.Request.Headers.UserAgent.ToString());
    }

    /// <summary>
    /// A session that only lives for the request.
    /// </summary>
    public sealed class RequestSession : ISession
    {
        /// <summary>
        /// The values of the request.
        /// </summary>
        private readonly Dictionary<string, byte[]> values = new(StringComparer.Ordinal);

        /// <inheritdoc />
        public bool IsAvailable => true;

        /// <inheritdoc />
        public string Id { get; } = Guid.NewGuid().ToString();

        /// <inheritdoc />
        public IEnumerable<string> Keys => this.values.Keys;

        /// <inheritdoc />
        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public bool TryGetValue(string key, out byte[] value)
        {
            return this.values.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public void Set(string key, byte[] value)
        {
            this.values[key] = value;
        }

        /// <inheritdoc />
        public void Remove(string key)
        {
            this.values.Remove(key);
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.values.Clear();
        }
    }
}
