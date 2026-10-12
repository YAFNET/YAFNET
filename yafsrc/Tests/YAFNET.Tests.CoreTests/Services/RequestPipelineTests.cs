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

namespace YAF.Tests.CoreTests.Services;

using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using UAParser;
using UAParser.Interfaces;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Middleware;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Objects;

/// <summary>
/// Request pipeline pieces: the security headers, the page query / request information handlers
/// of the init page load event and the view permission check.
/// </summary>
[TestFixture]
[NonParallelizable]
public class RequestPipelineTests
{
    private TestBoardContext board;

    private BoardConfiguration configuration;

    [SetUp]
    public void SetUp()
    {
        this.configuration = new BoardConfiguration();

        this.board = new TestBoardContext(b => b.RegisterInstance(this.configuration));
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Pages get the configured security headers, the CSP is extended with the own host")]
    public async Task Security_Headers_Test()
    {
        this.configuration.ContentSecurityPolicy = "default-src 'self'";
        var called = false;

        var middleware = new SecurityHeaderMiddleware(
            _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            this.configuration);

        var context = NewContext("/Posts/t5/hello");
        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
        context.Response.Headers["X-Frame-Options"].ToString().Should().Be("SAMEORIGIN");
        context.Response.Headers["X-Content-Type-Options"].ToString().Should().Be("nosniff");
        context.Response.Headers["Referrer-Policy"].ToString().Should().Be("no-referrer");
        context.Response.Headers.ContentSecurityPolicy.ToString().Should().Be("default-src 'self' forum.example:8080;");
    }

    [Test]
    [Description("Without a configured CSP only the other headers are sent, files get no headers")]
    public async Task Security_Headers_Without_Csp_And_For_Files_Test()
    {
        var middleware = new SecurityHeaderMiddleware(_ => Task.CompletedTask, this.configuration);

        var page = NewContext("/Topics/1/general");
        await middleware.InvokeAsync(page);
        page.Response.Headers.ContainsKey("Content-Security-Policy").Should().BeFalse();
        page.Response.Headers.ContainsKey("X-Frame-Options").Should().BeTrue();

        var file = NewContext("/css/forum.min.css");
        await middleware.InvokeAsync(file);
        file.Response.Headers.Should().BeEmpty();

        FluentActions.Invoking(() => new SecurityHeaderMiddleware(null, this.configuration)).Should()
            .Throw<ArgumentNullException>();
    }

    [Test]
    [Description("The page ids are read from the query, invalid values become 0, the control category wins")]
    public void Page_Variables_From_Query_Test()
    {
        this.board.HttpContext.Request.QueryString = new QueryString("?c=1&f=2&t=3&m=4&p=5");
        var @event = new InitPageLoadEvent();

        new LoadPageVariablesFromQuery(this.board.Current.ServiceLocator).Handle(@event);

        @event.PageQueryData.CategoryID.Should().Be(1);
        @event.PageQueryData.ForumID.Should().Be(2);
        @event.PageQueryData.TopicID.Should().Be(3);
        @event.PageQueryData.MessageID.Should().Be(4);
        @event.PageQueryData.PageIndex.Should().Be(5);

        this.board.HttpContext.Request.QueryString = new QueryString("?f=abc");
        this.board.ControlSettings.CategoryID = 9;
        @event = new InitPageLoadEvent();

        new LoadPageVariablesFromQuery(this.board.Current.ServiceLocator).Handle(@event);

        @event.PageQueryData.ForumID.Should().Be(0);
        @event.PageQueryData.TopicID.Should().Be(0);
        @event.PageQueryData.CategoryID.Should().Be(9);

        FluentActions.Invoking(() => new LoadPageVariablesFromQuery(null)).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("Query values win over route values, invalid or too large numbers are treated as missing")]
    public void Query_Or_Route_Value_Test()
    {
        var request = NewContext("/").Request;
        request.QueryString = new QueryString("?t=12&f=abc&m=99999999999&name=hello");
        request.RouteValues["t"] = "5";
        request.RouteValues["c"] = "3";

        request.GetQueryOrRouteValue<int>("t").Should().Be(12);
        request.GetQueryOrRouteValue<int>("c").Should().Be(3);
        request.GetQueryOrRouteValue<int>("f").Should().Be(0);
        request.GetQueryOrRouteValue<int>("m").Should().Be(0);
        request.GetQueryOrRouteValue<int>("missing").Should().Be(0);
        request.GetQueryOrRouteValue<string>("name").Should().Be("hello");
        request.GetQueryOrRouteValue<long>("m").Should().Be(99999999999);
    }

    [Test]
    [Description("Only the request decides the topic, a board-wide cached topic id is not used (it would leak between users)")]
    public void Page_Variables_Ignore_Cached_Topic_Test()
    {
        this.board.Cache.Set("TopicID", 7);

        var withoutTopic = new InitPageLoadEvent();
        new LoadPageVariablesFromQuery(this.board.Current.ServiceLocator).Handle(withoutTopic);
        withoutTopic.PageQueryData.TopicID.Should().Be(0);

        this.board.HttpContext.Request.QueryString = new QueryString("?t=3");
        var withTopic = new InitPageLoadEvent();
        new LoadPageVariablesFromQuery(this.board.Current.ServiceLocator).Handle(withTopic);
        withTopic.PageQueryData.TopicID.Should().Be(3);
    }

    [Test]
    [Description("Browser, platform and crawler detection; crawlers are not tracked unless they are shown")]
    [TestCase("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36", false, false, false)]
    [TestCase("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)", false, true, true)]
    [TestCase("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)", true, true, false)]
    public void Request_Information_Test(string userAgent, bool showCrawlers, bool isSearchEngine, bool dontTrack)
    {
        this.board.BoardSettings.ShowCrawlersInActiveList = showCrawlers;
        this.board.HttpContext.Request.Headers.UserAgent = userAgent;

        var parser = Substitute.For<IUserAgentParser>();
        parser.ClientInfo.Returns(Parser.GetDefault().Parse(userAgent));

        var accessor = new HttpContextAccessor { HttpContext = this.board.HttpContext };
        var @event = new InitPageLoadEvent();

        new LoadPageRequestInformation(this.board.Current.ServiceLocator, accessor, parser).Handle(@event);

        @event.UserRequestData.UserAgent.Should().Be(userAgent);
        @event.UserRequestData.IsSearchEngine.Should().Be(isSearchEngine);
        @event.UserRequestData.DontTrack.Should().Be(dontTrack);
        @event.UserRequestData.Browser.Should().NotBeNullOrWhiteSpace();
        @event.UserRequestData.Platform.Should().NotBeNullOrWhiteSpace();
        this.board.Current.Vars["DontTrack"].Should().Be(dontTrack);

        if (!isSearchEngine)
        {
            @event.UserRequestData.Browser.Should().StartWith("Chrome 120");
            @event.UserRequestData.Platform.Should().StartWith("Windows");
        }
    }

    [Test]
    [Description("Without parsed client info the request data stays empty")]
    public void Request_Information_Without_Parser_Output_Test()
    {
        var parser = Substitute.For<IUserAgentParser>();
        parser.ClientInfo.Returns((IUAParserOutput)null);
        var @event = new InitPageLoadEvent();

        new LoadPageRequestInformation(this.board.Current.ServiceLocator, null, parser).Handle(@event);

        @event.UserRequestData.Browser.Should().BeNull();
        @event.UserRequestData.UserAgent.Should().BeNull();
    }

    [Test]
    [Description("View permissions: everyone, registered users (guests are sent to the login) and admins")]
    public void Permissions_Test()
    {
        var permissions = new Permissions(this.board.Current.ServiceLocator);
        var denied = new ForbidResult();
        var login = new RedirectResult("/Account/Login");
        this.board.LinkBuilder.AccessDenied().Returns(denied);
        this.board.LinkBuilder.Redirect(ForumPages.Account_Login).Returns(login);

        this.board.User.IsGuest = true;
        permissions.Check(ViewPermissions.Everyone).Should().BeTrue();
        permissions.Check(ViewPermissions.RegisteredUsers).Should().BeFalse();
        permissions.Check(ViewPermissions.Nobody).Should().BeFalse();

        permissions.HandleRequest(ViewPermissions.Everyone).Should().BeNull();
        permissions.HandleRequest(ViewPermissions.RegisteredUsers).Should().BeSameAs(login);
        permissions.HandleRequest(ViewPermissions.Nobody).Should().BeSameAs(denied);

        this.configuration.AllowLoginAndLogoff = false;
        permissions.HandleRequest(ViewPermissions.RegisteredUsers).Should().BeSameAs(denied);

        this.board.User.IsGuest = false;
        permissions.HandleRequest(ViewPermissions.RegisteredUsers).Should().BeNull();
        permissions.Check(ViewPermissions.Nobody).Should().BeFalse();

        this.board.PageLoad.IsAdmin = true;
        permissions.HandleRequest(ViewPermissions.Nobody).Should().BeNull();
    }

    private static DefaultHttpContext NewContext(string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("forum.example", 8080);
        context.Request.Path = path;

        return context;
    }
}
