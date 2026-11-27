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

using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

using NSubstitute;

using YAF.Core.Context;
using YAF.Core.Hubs;

namespace YAF.Tests.Hubs;

/// <summary>
/// Tests of the <see cref="HubHttpContextFilter"/>: the hub methods of a long polling connection ran without
/// http context, BoardContext.Current failed (private messages, notifications and chat messages were not sent).
/// </summary>
[TestFixture]
public class HubHttpContextFilterTests
{
    /// <summary>
    /// The http context accessor (its value is shared by all accessors of the async flow).
    /// </summary>
    private HttpContextAccessor accessor;

    /// <summary>
    /// Starts every test without http context.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.accessor = new HttpContextAccessor { HttpContext = null };
    }

    /// <summary>
    /// Long polling: the hub method runs without http context and gets the context of its connection with a session.
    /// </summary>
    [Test]
    [Description("A hub method of a long polling connection gets the http context of its connection, with a session")]
    public async Task Long_Polling_Hub_Method_Gets_The_Connection_Context_Test()
    {
        var connectionContext = new DefaultHttpContext();
        var filter = new HubHttpContextFilter(this.accessor);

        HttpContext seen = null;

        await filter.InvokeMethodAsync(
            Invocation(Caller(connectionContext, "connection-1")),
            _ =>
            {
                seen = this.accessor.HttpContext;
                return ValueTask.FromResult<object>(null);
            });

        seen.Should().BeSameAs(connectionContext);

        // the forum uses the session id for the active users, the copied request has no session
        seen!.Features.Get<ISessionFeature>().Should().BeOfType<HubConnectionSessionFeature>();
        seen.Session.Id.Should().Be("connection-1");
    }

    /// <summary>
    /// Long polling: the copied request has the board context of the first request, which was released with
    /// that request; the connection gets its own (it failed with ObjectDisposedException), and keeps it.
    /// </summary>
    [Test]
    [Description("A long polling connection does not use the released board context of its first request")]
    public async Task Long_Polling_Does_Not_Use_The_Released_Board_Context_Test()
    {
        // like the copy of SignalR: a plain dictionary, it throws for a missing key
        var connectionContext = new DefaultHttpContext { Items = new Dictionary<object, object>() };
        var released = new object();
        var own = new object();
        connectionContext.Items[BoardContextPageProvider.PageBoardContextName] = released;

        var caller = Caller(connectionContext, "connection-4");
        var filter = new HubHttpContextFilter(this.accessor);

        object first = null;

        await filter.InvokeMethodAsync(
            Invocation(caller),
            _ =>
            {
                this.accessor.HttpContext!.Items.TryGetValue(BoardContextPageProvider.PageBoardContextName, out first);

                // BoardContext.Current creates the board context of the connection
                this.accessor.HttpContext.Items[BoardContextPageProvider.PageBoardContextName] = own;
                return ValueTask.FromResult<object>(null);
            });

        this.accessor.HttpContext = null;

        object second = null;

        await filter.InvokeMethodAsync(
            Invocation(caller),
            _ =>
            {
                this.accessor.HttpContext!.Items.TryGetValue(BoardContextPageProvider.PageBoardContextName, out second);
                return ValueTask.FromResult<object>(null);
            });

        first.Should().BeNull();
        second.Should().BeSameAs(own);
    }

    /// <summary>
    /// WebSockets: the hub method runs in the request of the connection, the filter changes nothing.
    /// </summary>
    [Test]
    [Description("A hub method that runs in a request (WebSockets) keeps it")]
    public async Task WebSockets_Hub_Method_Keeps_Its_Request_Test()
    {
        var request = new DefaultHttpContext();
        var connectionContext = new DefaultHttpContext();
        this.accessor.HttpContext = request;

        var filter = new HubHttpContextFilter(this.accessor);

        HttpContext seen = null;

        await filter.InvokeMethodAsync(
            Invocation(Caller(connectionContext, "connection-2")),
            _ =>
            {
                seen = this.accessor.HttpContext;
                return ValueTask.FromResult<object>(null);
            });

        seen.Should().BeSameAs(request);
        connectionContext.Features.Get<ISessionFeature>().Should().BeNull();
    }

    /// <summary>
    /// Long polling: the board context of the connection is released when it disconnects
    /// (the copied request never ends, nothing else released it).
    /// </summary>
    [Test]
    [Description("The board context of a long polling connection is released when the connection ends")]
    public async Task Long_Polling_Board_Context_Is_Released_On_Disconnect_Test()
    {
        var connectionContext = new DefaultHttpContext();
        var boardContext = Substitute.For<IDisposable>();
        var caller = Caller(connectionContext, "connection-3");
        var filter = new HubHttpContextFilter(this.accessor);

        await filter.InvokeMethodAsync(
            Invocation(caller),
            _ =>
            {
                // the first use of BoardContext.Current puts it into the items of the context
                this.accessor.HttpContext!.Items[BoardContextPageProvider.PageBoardContextName] = boardContext;
                return ValueTask.FromResult<object>(null);
            });

        this.accessor.HttpContext = null;

        await filter.OnDisconnectedAsync(
            new HubLifetimeContext(caller, Substitute.For<IServiceProvider>(), Substitute.For<Hub>()),
            null,
            (_, _) => Task.CompletedTask);

        boardContext.Received(1).Dispose();
        connectionContext.Items.ContainsKey(BoardContextPageProvider.PageBoardContextName).Should().BeFalse();
    }

    /// <summary>
    /// A hub caller of a connection with the given http context.
    /// </summary>
    private static HubCallerContext Caller(HttpContext httpContext, string connectionId)
    {
        var features = new FeatureCollection();
        features.Set<IHttpContextFeature>(new HttpContextFeature { HttpContext = httpContext });

        var caller = Substitute.For<HubCallerContext>();
        caller.Features.Returns(features);
        caller.ConnectionId.Returns(connectionId);

        return caller;
    }

    /// <summary>
    /// The invocation of a hub method.
    /// </summary>
    private static HubInvocationContext Invocation(HubCallerContext caller)
    {
        return new HubInvocationContext(
            caller,
            Substitute.For<IServiceProvider>(),
            Substitute.For<Hub>(),
            typeof(Hub).GetMethod(nameof(Hub.OnConnectedAsync))!,
            []);
    }

    /// <summary>
    /// The http context feature of a connection.
    /// </summary>
    private sealed class HttpContextFeature : IHttpContextFeature
    {
        /// <inheritdoc />
        public HttpContext HttpContext { get; set; }
    }
}
