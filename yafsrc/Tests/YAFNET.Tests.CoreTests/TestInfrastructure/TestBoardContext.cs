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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using YAF.Configuration;
using YAF.Core;
using YAF.Core.Context;
using YAF.Core.Data;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Services;
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
/// Pass <c>useDatabase: true</c> to get a private in-memory SQLite database (see <see cref="TestDatabase"/>)
/// with the real <see cref="BasicRepository{T}"/>, so <c>GetRepository&lt;T&gt;()</c> runs real SQL.
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
    /// <param name="useDatabase">
    /// Register an in-memory SQLite <see cref="IDbAccess"/> and the real repositories.
    /// </param>
    public TestBoardContext(Action<ContainerBuilder> configure = null, bool useDatabase = false)
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
        builder.RegisterInstance(this.Events).As<IRaiseEvent>().As<IRaiseEventAsync>();
        builder.RegisterType<CurrentBoardId>().As<IHaveBoardID>().SingleInstance();
        builder.RegisterInstance(this.Search).As<ISearch>();
        builder.RegisterInstance(this.LinkBuilder).As<ILinkBuilder>();
        builder.RegisterInstance(this.Notifications).As<ISendNotification>();

        if (useDatabase)
        {
            this.Database = new TestDatabase();

            builder.RegisterInstance(this.Database.DbAccess).As<IDbAccess>();
            builder.RegisterGeneric(typeof(BasicRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();
        }

        configure?.Invoke(builder);

        this.container = builder.Build();

        this.previousContainer = GlobalContainer.AutoFacContainer;
        this.previousServiceProvider = ServiceLocatorAccess.CurrentServiceProvider;

        GlobalContainer.AutoFacContainer = this.container;
        ServiceLocatorAccess.CurrentServiceProvider = this.container.Resolve<IServiceLocator>();

        this.SetPageData();
    }

    /// <summary>
    /// Gets the board context that <see cref="BoardContext.Current"/> returns while this instance is alive.
    /// </summary>
    public BoardContext Current => BoardContext.Current;

    /// <summary>
    /// Gets the HTTP context the board context is bound to (with an in-memory session).
    /// </summary>
    public DefaultHttpContext HttpContext { get; } = CreateHttpContext();

    /// <summary>
    /// Gets the fake localization (returns <c>[PAGE.TAG]</c> for unknown texts).
    /// </summary>
    public FakeLocalization Localization { get; } = new();

    /// <summary>
    /// Gets the in-memory cache registered as <see cref="IDataCache"/> and <see cref="IObjectStore"/>.
    /// </summary>
    public FakeDataCache Cache { get; } = new();

    /// <summary>
    /// Gets the event raiser registered as <see cref="IRaiseEvent"/> and <see cref="IRaiseEventAsync"/>.
    /// No handlers run, raised events are recorded.
    /// </summary>
    public RecordingEventRaiser Events { get; } = new();

    /// <summary>
    /// Gets the fake search index registered as <see cref="ISearch"/>.
    /// </summary>
    public FakeSearch Search { get; } = new();

    /// <summary>
    /// Gets the NSubstitute <see cref="ILinkBuilder"/>; forum links are <c>/forum/{id}</c>.
    /// </summary>
    public ILinkBuilder LinkBuilder { get; } = CreateLinkBuilder();

    /// <summary>
    /// Gets the NSubstitute <see cref="ISendNotification"/> (verify calls with <c>Received()</c>).
    /// </summary>
    public ISendNotification Notifications { get; } = Substitute.For<ISendNotification>();

    /// <summary>
    /// Gets the in-memory database, or <c>null</c> when created without <c>useDatabase</c>.
    /// </summary>
    public TestDatabase Database { get; }

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
    public User PageUser { get; private set; } = new() { ID = 2, Name = "TestUser", DisplayName = "Test User" };

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
    /// Grants a user access to a forum the way the forum does it: writes the group / access mask
    /// permission chain (<see cref="TestDatabase.AddForumAccess"/>) and then rebuilds the user's
    /// ActiveAccess rows from the access views with the production <c>InsertPageAccess</c>.
    /// Requires <c>useDatabase: true</c>.
    /// </summary>
    public void GrantAccess(
        int userId,
        int forumId,
        AccessFlags.Flags access = TestDatabase.MemberAccess,
        GroupFlags.Flags groupFlags = GroupFlags.Flags.None)
    {
        if (this.Database is null)
        {
            throw new InvalidOperationException("GrantAccess needs a TestBoardContext created with useDatabase: true.");
        }

        this.Database.AddForumAccess(userId, forumId, access, groupFlags);
        this.RefreshActiveAccess(userId);
    }

    /// <summary>
    /// Rebuilds the user's ActiveAccess rows from the access views (what happens on the next page load).
    /// </summary>
    public void RefreshActiveAccess(int userId)
    {
        var repository = this.Current.GetRepository<ActiveAccess>();

        repository.Delete(a => a.UserID == userId);
        repository.InsertPageAccess(TestDatabase.BoardId, userId, false);
    }

    /// <summary>
    /// Gets the forum of the current page (see <see cref="SetPage"/>).
    /// </summary>
    public Forum PageForum { get; private set; } = new();

    /// <summary>
    /// Gets the topic of the current page (see <see cref="SetPage"/>), can be <c>null</c>.
    /// </summary>
    public Topic PageTopic { get; private set; } = new();

    /// <summary>
    /// Makes the forum / topic (loaded from the test database) the current page, like a request with
    /// <c>?f=..&amp;t=..</c> would. The access rights of the page are the <see cref="PageLoad"/> flags.
    /// </summary>
    public void SetPage(int forumId, int? topicId = null)
    {
        this.PageForum = this.Current.GetRepository<Forum>().GetById(forumId) ?? new Forum();
        this.PageTopic = topicId.HasValue ? this.Current.GetRepository<Topic>().GetById(topicId.Value) : null;

        this.Query.ForumID = this.PageForum.ID;
        this.Query.TopicID = this.PageTopic?.ID ?? 0;
        this.Query.CategoryID = this.PageForum.CategoryID;

        this.SetPageData();
    }

    /// <summary>
    /// Makes the user (loaded from the test database) the current, logged-in user.
    /// </summary>
    public void LoginAs(int userId)
    {
        this.PageUser = this.Current.GetRepository<User>().GetById(userId);
        this.PageLoad.UserID = userId;
        this.User.IsGuest = false;

        this.Current.MembershipUser = new Types.Models.Identity.AspNetUsers
        {
            Id = this.PageUser.ProviderUserKey ?? $"key-{userId}",
            UserName = this.PageUser.Name,
            Email = this.PageUser.Email
        };

        this.SetPageData();
    }

    /// <summary>
    /// Creates a controller bound to the test HTTP context.
    /// </summary>
    public T CreateController<T>()
        where T : Microsoft.AspNetCore.Mvc.Controller, new()
    {
        return new T
        {
            ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = this.HttpContext }
        };
    }

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
        this.Database?.Dispose();
    }

    private void SetPageData()
    {
        this.Current.PageData = Tuple.Create(
            this.Request,
            Tuple.Create(this.PageLoad, this.PageUser, new Category(), this.PageForum, this.PageTopic, new Message()),
            this.User,
            this.Query);
    }

    private static ILinkBuilder CreateLinkBuilder()
    {
        var linkBuilder = Substitute.For<ILinkBuilder>();

        linkBuilder.GetForumLink(Arg.Any<Forum>()).Returns(c => $"/forum/{c.Arg<Forum>().ID}");
        linkBuilder.GetForumLink(Arg.Any<int>(), Arg.Any<string>()).Returns(c => $"/forum/{c.ArgAt<int>(0)}");

        return linkBuilder;
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Features.Set<ISessionFeature>(new SessionFeature { Session = new FakeSession() });

        // a realistic request, used when absolute links are built
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("forum.example.com");
        context.Request.PathBase = string.Empty;
        context.Request.Path = "/";
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("203.0.113.10");
        return context;
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
