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

namespace YAF.Tests.CoreTests.TestInfrastructure;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using YAF.Configuration;
using YAF.Core;
using YAF.Core.Context;
using YAF.Types;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// Builds a minimal Autofac container and wires it into <see cref="BoardContext.Current"/>
/// so code that uses the service locator can be unit tested without hosting the forum.
/// </summary>
/// <remarks>
/// <see cref="BoardContext.Current"/> and <see cref="ServiceLocatorAccess"/> are static, so
/// fixtures using this class must be <c>[NonParallelizable]</c>. Always dispose the instance
/// (typically in <c>[TearDown]</c>) to restore the previous container.
/// <code>
/// using var board = new TestBoardContext(b => b.RegisterInstance(myFake).As&lt;IMyService&gt;());
/// board.Localization.Add("RELATIVE_TIME", "NOW", "just now");
/// board.User.IsGuest = true;
/// </code>
/// </remarks>
public sealed class TestBoardContext : IDisposable
{
    private readonly ILifetimeScope previousContainer;

    private readonly IServiceLocator previousServiceProvider;

    private readonly IContainer container;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestBoardContext"/> class.
    /// </summary>
    /// <param name="configure">
    /// Additional registrations. They are applied last, so they override the defaults.
    /// </param>
    public TestBoardContext(Action<ContainerBuilder> configure = null)
    {
        var builder = new ContainerBuilder();

        // same registrations as GeneralModule uses in production
        builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().As<IInjectServices>()
            .InstancePerLifetimeScope();

        // BoardContextPageProvider caches a static instance when there is no HttpContext,
        // a dedicated HttpContext per test keeps contexts from leaking between tests
        builder.RegisterInstance(new FixedHttpContextAccessor(this.HttpContext)).As<IHttpContextAccessor>();
        builder.RegisterType<BoardContextPageProvider>().AsSelf().SingleInstance();
        builder.Register(c => c.Resolve<BoardContextPageProvider>().Instance).ExternallyOwned();

        builder.RegisterInstance(this.Localization).As<ILocalization>();
        builder.RegisterInstance(this.Cache).As<IDataCache>().As<IObjectStore>();
        builder.RegisterInstance(this.BoardSettings);
        builder.RegisterInstance(this.ControlSettings);
        builder.RegisterGeneric(typeof(NullLogger<>)).As(typeof(ILogger<>)).SingleInstance();
        builder.RegisterInstance(NullLoggerFactory.Instance).As<ILoggerFactory>();

        configure?.Invoke(builder);

        this.container = builder.Build();

        this.previousContainer = GlobalContainer.AutoFacContainer;
        this.previousServiceProvider = ServiceLocatorAccess.CurrentServiceProvider;

        GlobalContainer.AutoFacContainer = this.container;
        ServiceLocatorAccess.CurrentServiceProvider = this.container.Resolve<IServiceLocator>();

        this.Current.PageData = Tuple.Create(
            this.Request,
            Tuple.Create(this.PageLoad, this.PageUser, new Category(), new Forum(), new Topic(), new Message()),
            this.User,
            this.Query);
    }

    /// <summary>
    /// Gets the board context that <see cref="BoardContext.Current"/> returns while this instance is alive.
    /// </summary>
    public BoardContext Current => BoardContext.Current;

    /// <summary>
    /// Gets the HTTP context the board context is bound to.
    /// </summary>
    public DefaultHttpContext HttpContext { get; } = new();

    /// <summary>
    /// Gets the fake localization (returns <c>[PAGE.TAG]</c> for unknown texts).
    /// </summary>
    public FakeLocalization Localization { get; } = new();

    /// <summary>
    /// Gets the in-memory cache registered as <see cref="IDataCache"/> and <see cref="IObjectStore"/>.
    /// </summary>
    public FakeDataCache Cache { get; } = new();

    /// <summary>
    /// Gets the board settings.
    /// </summary>
    public BoardSettings BoardSettings { get; } = new();

    /// <summary>
    /// Gets the control settings.
    /// </summary>
    public ControlSettings ControlSettings { get; } = new();

    /// <summary>
    /// Gets the lazy user data of the current page (guest flag, time zone, ...).
    /// </summary>
    public UserLazyData User { get; } = new() { TimeZoneUser = "UTC" };

    /// <summary>
    /// Gets the current page user.
    /// </summary>
    public User PageUser { get; } = new() { ID = 2, Name = "TestUser", DisplayName = "Test User" };

    /// <summary>
    /// Gets the page load data (access rights of the current user).
    /// </summary>
    public PageLoad PageLoad { get; } = new() { UserID = 2, GuestUserID = 1, ReadAccess = true };

    /// <summary>
    /// Gets the request data of the current page.
    /// </summary>
    public UserRequestData Request { get; } = new();

    /// <summary>
    /// Gets the query data of the current page.
    /// </summary>
    public PageQueryData Query { get; } = new();

    /// <summary>
    /// Resolves a service from the test container.
    /// </summary>
    public T Resolve<T>()
    {
        return this.container.Resolve<T>();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GlobalContainer.AutoFacContainer = this.previousContainer;
        ServiceLocatorAccess.CurrentServiceProvider = this.previousServiceProvider;

        this.container.Dispose();
    }

    /// <summary>
    /// An <see cref="IHttpContextAccessor"/> with a plain field instead of the AsyncLocal storage
    /// of the default implementation, so the context survives NUnit's SetUp / Test thread hops.
    /// </summary>
    private sealed class FixedHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; } = httpContext;
    }
}
