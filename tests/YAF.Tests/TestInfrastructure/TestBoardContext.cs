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

using System.IO;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

using Autofac;

using NSubstitute;

using YAF.Configuration;
using YAF.Core;
using YAF.Core.Context;
using YAF.Core.Data;
using YAF.Core.Model;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Core.Services.Cache;
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
/// <see cref="GlobalContainer.Container"/>, <see cref="ServiceLocatorAccess"/> and
/// <see cref="HttpContext.Current"/> are static, so fixtures using this class must be
/// <c>[NonParallelizable]</c>. Always dispose the instance (typically in <c>[TearDown]</c>)
/// to remove the test container again.
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
    private static readonly FieldInfo MembershipUserField =
        typeof(BoardContext).GetField("membershipUser", BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// The board context of code without http context (e.g. the thread pool threads of the sync identity methods),
    /// it must not survive the container of a test.
    /// </summary>
    private static readonly FieldInfo GlobalBoardContextField =
        typeof(BoardContextPageProvider).GetField("_globalInstance", BindingFlags.Static | BindingFlags.NonPublic);

    private readonly IServiceLocator previousServiceProvider;

    private readonly HttpContext previousHttpContext;

    private readonly IContainer container;

    private string webRoot;

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
        // the board context is stored in the items of the current request, a dedicated
        // HttpContext per test keeps contexts from leaking between tests
        this.previousHttpContext = HttpContext.Current;
        HttpContext.Current = this.HttpContext;

        var builder = new ContainerBuilder();

        // same registrations as GeneralModule uses in production
        builder.RegisterType<AutoFacServiceLocatorProvider>().AsSelf().As<IServiceLocator>().As<IInjectServices>()
            .InstancePerLifetimeScope();
        builder.RegisterType<BoardContextPageProvider>().AsSelf().As<IReadOnlyProvider<BoardContext>>()
            .SingleInstance();
        builder.Register(k => k.Resolve<IComponentContext>().Resolve<BoardContextPageProvider>().Instance)
            .ExternallyOwned();

        builder.Register(_ => new TestHttpContextWrapper(this.HttpContext, this.MapPath)).As<HttpContextBase>();
        builder.Register(c => c.Resolve<HttpContextBase>().Request).As<HttpRequestBase>();
        builder.Register(c => c.Resolve<HttpContextBase>().Response).As<HttpResponseBase>();
        builder.Register(c => c.Resolve<HttpContextBase>().Server).As<HttpServerUtilityBase>();
        builder.Register(c => c.Resolve<HttpContextBase>().Session).As<HttpSessionStateBase>();
        builder.RegisterType<Session>().As<ISession>();

        builder.RegisterInstance(this.Localization).As<ILocalization>();
        builder.RegisterInstance(this.Cache).As<IDataCache>().As<IObjectStore>();
        builder.RegisterInstance(this.BoardSettings);
        builder.RegisterInstance(this.ControlSettings);
        builder.RegisterInstance(new BoardFolders());
        builder.RegisterInstance(this.Logger).As<ILoggerService>();
        builder.RegisterInstance(this.Events).As<IRaiseEvent>();
        builder.RegisterType<CurrentBoardId>().As<IHaveBoardID>().SingleInstance();
        builder.RegisterType<TreatCacheKeyWithBoard>().As<ITreatCacheKey>();
        builder.RegisterInstance(this.Search).As<ISearch>();
        builder.RegisterInstance(this.UrlBuilder).As<IUrlBuilder>();
        builder.RegisterType<LinkBuilder>().AsSelf();
        builder.RegisterInstance(this.Notifications).As<ISendNotification>();

        if (useDatabase)
        {
            this.Database = new TestDatabase();

            builder.RegisterInstance(this.Database.DbAccess).As<IDbAccess>();
            builder.RegisterGeneric(typeof(BasicRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();
        }

        configure?.Invoke(builder);

        this.container = builder.Build();

        // not the getter of the previous container, it would build the production container
        this.previousServiceProvider = ServiceLocatorAccess.CurrentServiceProvider;

        GlobalContainer.Container = this.container;
        GlobalBoardContextField.SetValue(null, null);
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
    public HttpContext HttpContext { get; } = CreateHttpContext();

    /// <summary>
    /// Gets the folder of the forum files (<c>wwwroot</c> in a new temporary folder, deleted on dispose).
    /// <c>MapPath</c> of the request and the server utility map into this folder.
    /// </summary>
    public string WebRoot
    {
        get
        {
            if (this.webRoot is null)
            {
                this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-test-{Guid.NewGuid():N}", "wwwroot");
                Directory.CreateDirectory(this.webRoot);
            }

            return this.webRoot;
        }
    }

    /// <summary>
    /// Gets the folder that contains the <see cref="WebRoot"/> (e.g. for files outside the forum).
    /// </summary>
    public string ContentRoot => Path.GetDirectoryName(this.WebRoot);

    /// <summary>
    /// Gets the fake localization (returns <c>[PAGE.TAG]</c> for unknown texts).
    /// </summary>
    public FakeLocalization Localization { get; } = new();

    /// <summary>
    /// Gets the in-memory cache registered as <see cref="IDataCache"/> and <see cref="IObjectStore"/>.
    /// </summary>
    public FakeDataCache Cache { get; } = new();

    /// <summary>
    /// Gets the event raiser registered as <see cref="IRaiseEvent"/>.
    /// No handlers run, raised events are recorded.
    /// </summary>
    public RecordingEventRaiser Events { get; } = new();

    /// <summary>
    /// Gets the fake search index registered as <see cref="ISearch"/>.
    /// </summary>
    public FakeSearch Search { get; } = new();

    /// <summary>
    /// Gets the NSubstitute <see cref="ILoggerService"/> (verify calls with <c>Received()</c>).
    /// </summary>
    public ILoggerService Logger { get; } = Substitute.For<ILoggerService>();

    /// <summary>
    /// Gets the NSubstitute <see cref="IUrlBuilder"/>; urls are returned as they are,
    /// full urls get <c>https://forum.example.com/</c> in front.
    /// </summary>
    public IUrlBuilder UrlBuilder { get; } = CreateUrlBuilder();

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
    public BoardSettings BoardSettings { get; } = new() { BoardId = TestDatabase.BoardId };

    /// <summary>
    /// Gets the control settings.
    /// </summary>
    public ControlSettings ControlSettings { get; } = new() { BoardID = TestDatabase.BoardId };

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
    /// Gets the forum of the current page (see <see cref="SetPage"/>).
    /// </summary>
    public Forum PageForum { get; private set; } = new();

    /// <summary>
    /// Gets the topic of the current page (see <see cref="SetPage"/>), can be <c>null</c>.
    /// </summary>
    public Topic PageTopic { get; private set; } = new();

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

        MembershipUserField.SetValue(
            this.Current,
            new Types.Models.Identity.AspNetUsers
            {
                Id = this.PageUser.ProviderUserKey ?? $"key-{userId}",
                UserName = this.PageUser.Name,
                Email = this.PageUser.Email
            });

        this.SetPageData();
    }

    /// <summary>
    /// Maps a virtual path (e.g. <c>~/uploads/a.png</c> or <c>/uploads</c>) into the <see cref="WebRoot"/>.
    /// </summary>
    public string MapPath(string virtualPath)
    {
        var relative = (virtualPath ?? string.Empty).TrimStart('~').Replace('\\', '/').Trim('/');

        while (relative.Contains("//"))
        {
            relative = relative.Replace("//", "/");
        }

        return relative.Length == 0 ? this.WebRoot : Path.Combine(this.WebRoot, relative.Replace('/', '\\'));
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
        GlobalContainer.Container = null;
        GlobalBoardContextField.SetValue(null, null);
        ServiceLocatorAccess.CurrentServiceProvider = this.previousServiceProvider;
        HttpContext.Current = this.previousHttpContext;

        this.container.Dispose();
        this.Database?.Dispose();

        if (this.webRoot != null && Directory.Exists(this.ContentRoot))
        {
            Directory.Delete(this.ContentRoot, true);
        }
    }

    private void SetPageData()
    {
        this.Current.PageData = Tuple.Create(
            this.Request,
            Tuple.Create(this.PageLoad, this.PageUser, new Category(), this.PageForum, this.PageTopic, new Message()),
            this.User,
            this.Query);

        this.Current.UserPageDataLoaded = true;
    }

    /// <summary>
    /// The HttpContextBase of the tests: MapPath of the request and the server map into the web root folder
    /// (the real ones need a hosted application).
    /// </summary>
    private sealed class TestHttpContextWrapper(HttpContext httpContext, Func<string, string> mapPath)
        : HttpContextWrapper(httpContext)
    {
        private readonly HttpRequestBase request = new TestHttpRequestWrapper(httpContext.Request, mapPath);

        private readonly HttpServerUtilityBase server = new TestHttpServerUtilityWrapper(httpContext.Server, mapPath);

        public override HttpRequestBase Request => this.request;

        public override HttpServerUtilityBase Server => this.server;
    }

    private sealed class TestHttpRequestWrapper(HttpRequest request, Func<string, string> mapPath)
        : HttpRequestWrapper(request)
    {
        public override string MapPath(string virtualPath)
        {
            return mapPath(virtualPath);
        }
    }

    private sealed class TestHttpServerUtilityWrapper(HttpServerUtility server, Func<string, string> mapPath)
        : HttpServerUtilityWrapper(server)
    {
        public override string MapPath(string path)
        {
            return mapPath(path);
        }
    }

    private static IUrlBuilder CreateUrlBuilder()
    {
        var urlBuilder = Substitute.For<IUrlBuilder>();

        urlBuilder.BuildUrl(Arg.Any<string>()).Returns(c => c.Arg<string>());
        urlBuilder.BuildUrlFull(Arg.Any<string>()).Returns(c => $"https://forum.example.com/{c.Arg<string>()}");

        return urlBuilder;
    }

    private static HttpContext CreateHttpContext()
    {
        // a realistic request, used when absolute links are built
        var context = new HttpContext(
            new HttpRequest(string.Empty, "https://forum.example.com/", string.Empty),
            new HttpResponse(TextWriter.Null));

        SessionStateUtility.AddHttpSessionStateToContext(
            context,
            new HttpSessionStateContainer(
                Guid.NewGuid().ToString("N"),
                new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                20,
                true,
                HttpCookieMode.UseCookies,
                SessionStateMode.InProc,
                false));

        return context;
    }
}
