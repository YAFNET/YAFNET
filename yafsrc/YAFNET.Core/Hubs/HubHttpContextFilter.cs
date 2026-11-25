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

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

using YAF.Core.Context;

namespace YAF.Core.Hubs;

/// <summary>
/// Gives the hub methods the http context of their connection.
/// With WebSockets a hub method runs in the request of the connection. With the long polling transport
/// (a browser, proxy or network without WebSockets) every call comes with its own short request and the hub
/// method ran without http context: BoardContext.Current failed (without http context it is one instance for
/// all callers), so private messages, notifications and chat messages could not be sent.
/// The filter uses the copy of the first request that SignalR keeps for such a connection.
/// </summary>
public class HubHttpContextFilter : IHubFilter
{
    /// <summary>
    /// The http context accessor.
    /// </summary>
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="HubHttpContextFilter"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The http context accessor.</param>
    public HubHttpContextFilter(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Runs a hub method with the http context of its connection.
    /// </summary>
    /// <param name="invocationContext">The invocation context.</param>
    /// <param name="next">The hub method.</param>
    /// <returns>The result of the hub method.</returns>
    public ValueTask<object> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object>> next)
    {
        this.SetHttpContext(invocationContext.Context);

        return next(invocationContext);
    }

    /// <summary>
    /// Runs OnConnectedAsync of the hub with the http context of its connection.
    /// </summary>
    /// <param name="context">The hub lifetime context.</param>
    /// <param name="next">The next filter or the hub.</param>
    /// <returns>A task.</returns>
    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        this.SetHttpContext(context.Context);

        return next(context);
    }

    /// <summary>
    /// Runs OnDisconnectedAsync of the hub with the http context of its connection and releases the
    /// board context of a long polling connection (the copied request never ends, nothing else releases it).
    /// </summary>
    /// <param name="context">The hub lifetime context.</param>
    /// <param name="exception">The exception of the disconnect.</param>
    /// <param name="next">The next filter or the hub.</param>
    /// <returns>A task.</returns>
    public async Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception exception,
        Func<HubLifetimeContext, Exception, Task> next)
    {
        var httpContext = this.SetHttpContext(context.Context);

        try
        {
            await next(context, exception);
        }
        finally
        {
            if (httpContext?.Features.Get<ISessionFeature>() is HubConnectionSessionFeature
                && httpContext.Items.TryGetValue(BoardContextPageProvider.PageBoardContextName, out var item)
                && item is IDisposable boardContext)
            {
                httpContext.Items.Remove(BoardContextPageProvider.PageBoardContextName);
                boardContext.Dispose();
            }
        }
    }

    /// <summary>
    /// Sets the http context of the connection if the hub runs without one (long polling).
    /// </summary>
    /// <param name="callerContext">The hub caller context.</param>
    /// <returns>The http context of the connection that was set, or null if nothing was set.</returns>
    private HttpContext SetHttpContext(HubCallerContext callerContext)
    {
        if (this.httpContextAccessor.HttpContext is not null)
        {
            // WebSockets: the hub runs in the request of the connection
            return null;
        }

        var httpContext = callerContext.GetHttpContext();

        if (httpContext is null)
        {
            return null;
        }

        // the first call of the connection
        if (httpContext.Features.Get<ISessionFeature>()?.Session is null)
        {
            // the copied request has no session, the forum uses the session id for the active users
            httpContext.Features.Set<ISessionFeature>(new HubConnectionSessionFeature(callerContext.ConnectionId));

            // the copy has the items of the first request, its board context was released with that request
            httpContext.Items.Remove(BoardContextPageProvider.PageBoardContextName);
        }

        this.httpContextAccessor.HttpContext = httpContext;

        return httpContext;
    }
}
