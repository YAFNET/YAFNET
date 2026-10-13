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

namespace YAF.Tests.Controllers;

using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;

using NSubstitute;

using YAF.Core.BasePages;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Handlers;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The security checks every forum page runs before its handler (<see cref="ForumPage.OnPageHandlerExecutionAsync"/>).
/// </summary>
[TestFixture]
[NonParallelizable]
public class ForumPageFilterTests
{
    private TestBoardContext board;

    private IPermissions permissions;

    private IActionResult loginRedirect;

    private IActionResult accessDenied;

    private IActionResult infoRedirect;

    private int handlerCalls;

    [SetUp]
    public void SetUp()
    {
        this.permissions = Substitute.For<IPermissions>();
        this.loginRedirect = new RedirectResult("/Account/Login");
        this.accessDenied = new ForbidResult();
        this.infoRedirect = new RedirectResult("/Info");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<LocalizationProvider>().AsSelf().SingleInstance();
                b.RegisterInstance(this.permissions).As<IPermissions>();
                b.RegisterInstance(Substitute.For<IDateTimeService>()).As<IDateTimeService>();
            },
            useDatabase: true);

        this.board.Current.PageLinks = [];
        this.board.LinkBuilder.AccessDenied().Returns(this.accessDenied);
        this.board.LinkBuilder.RedirectInfoPage(Arg.Any<InfoMessage>()).Returns(this.infoRedirect);
        this.permissions.HandleRequest(ViewPermissions.RegisteredUsers).Returns(this.loginRedirect);

        var user = this.board.Database.AddUser("Alice");
        this.board.LoginAs(user);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A normal page runs its handler once and raises the page events")]
    public async Task Normal_Page_Test()
    {
        var context = await this.RunAsync(new TestPage(ForumPages.Index));

        context.Result.Should().BeNull();
        this.handlerCalls.Should().Be(1);
        this.board.Events.OfType<ForumPageInitEvent>().Should().ContainSingle();
        this.board.Events.OfType<ForumPagePreLoadEvent>().Should().ContainSingle();
        this.board.Events.OfType<ForumPagePostLoadEvent>().Should().ContainSingle();
    }

    [Test]
    [Description("Account pages (login, register...) skip the security checks and run their handler only once")]
    public async Task Account_Page_Runs_Handler_Once_Test()
    {
        this.board.BoardSettings.RequireLogin = true;
        this.board.User.IsGuest = true;

        var context = await this.RunAsync(new TestPage(ForumPages.Account_Login) { Account = true });

        this.handlerCalls.Should().Be(1, "the handler (e.g. a login attempt) must not run twice");
        context.Result.Should().BeNull();

        this.board.BoardSettings.RequireLogin = false;
        await this.RunAsync(new TestPage(ForumPages.Account_Register) { Account = true });
        this.handlerCalls.Should().Be(1, "also without the login requirement");
    }

    [Test]
    [Description("Without a database nothing runs")]
    public async Task No_Database_Test()
    {
        await this.RunAsync(new TestPage(ForumPages.Index) { WithoutDatabase = true });

        this.handlerCalls.Should().Be(0);
    }

    [Test]
    [Description("Guests are sent to the login when the board requires a login, unprotected pages stay open")]
    public async Task Require_Login_Test()
    {
        this.board.BoardSettings.RequireLogin = true;
        this.board.User.IsGuest = true;

        (await this.RunAsync(new TestPage(ForumPages.Index))).Result.Should().BeSameAs(this.loginRedirect);
        this.handlerCalls.Should().Be(0);

        (await this.RunAsync(new TestPage(ForumPages.Info) { Protected = false })).Result.Should().BeNull();
        this.handlerCalls.Should().Be(1);
    }

    [Test]
    [Description("Pages for registered users need a logged in user")]
    public async Task Registered_Page_Test()
    {
        this.board.Current.MembershipUser = null;

        (await this.RunAsync(new TestPage(ForumPages.MyTopics) { Registered = true })).Result.Should().BeSameAs(this.loginRedirect);

        this.board.Current.MembershipUser = new AspNetUsers { Id = "alice" };
        (await this.RunAsync(new TestPage(ForumPages.MyTopics) { Registered = true })).Result.Should().BeNull();
    }

    [Test]
    [Description("Admin pages need an admin, and an admin who is no host admin needs access to the page")]
    public async Task Admin_Page_Test()
    {
        (await this.RunAsync(new TestPage(ForumPages.Admin_Admin) { Admin = true })).Result.Should().BeSameAs(this.accessDenied);

        this.board.PageLoad.IsAdmin = true;
        (await this.RunAsync(new TestPage(ForumPages.Admin_Admin) { Admin = true })).Result.Should().BeSameAs(this.infoRedirect);

        this.board.Current.GetRepository<AdminPageUserAccess>().Insert(
            new AdminPageUserAccess { UserID = this.board.PageLoad.UserID, PageName = nameof(ForumPages.Admin_Admin) });

        (await this.RunAsync(new TestPage(ForumPages.Admin_Admin) { Admin = true })).Result.Should().BeNull();
    }

    [Test]
    [Description("The register page is closed when registrations are disabled")]
    public async Task Registrations_Disabled_Test()
    {
        this.board.BoardSettings.DisableRegistrations = true;

        (await this.RunAsync(new TestPage(ForumPages.Account_Register))).Result.Should().BeSameAs(this.accessDenied);
    }

    [Test]
    [Description("Members, active users, profiles and search use their view permission setting")]
    [TestCase(ForumPages.Members)]
    [TestCase(ForumPages.ActiveUsers)]
    [TestCase(ForumPages.UserProfile)]
    [TestCase(ForumPages.Albums)]
    [TestCase(ForumPages.Search)]
    public async Task View_Permissions_Test(ForumPages page)
    {
        this.permissions.HandleRequest(Arg.Any<ViewPermissions>()).Returns(this.accessDenied);

        (await this.RunAsync(new TestPage(page))).Result.Should().BeSameAs(this.accessDenied);
        this.handlerCalls.Should().Be(0);
    }

    [Test]
    [Description("A suspended user is sent to the info page, an expired suspension is lifted")]
    public async Task Suspended_User_Test()
    {
        var dateTime = this.board.Resolve<IDateTimeService>();
        dateTime.GetUserDateTime(Arg.Any<DateTime>()).Returns(c => c.Arg<DateTime>());

        this.board.User.Suspended = DateTime.UtcNow.AddDays(1);
        (await this.RunAsync(new TestPage(ForumPages.Index))).Result.Should().BeSameAs(this.infoRedirect);

        this.board.User.Suspended = DateTime.UtcNow.AddDays(-1);
        this.board.Current.GetRepository<User>().UpdateOnly(
            () => new User { Suspended = DateTime.UtcNow.AddDays(-1) },
            u => u.ID == this.board.PageLoad.UserID);

        (await this.RunAsync(new TestPage(ForumPages.Index))).Result.Should().BeNull();
        this.board.Current.GetRepository<User>().GetById(this.board.PageLoad.UserID).Suspended.Should().BeNull();
        await this.board.Notifications.Received(1).SendUserSuspensionEndedNotificationAsync(Arg.Any<User>());
    }

    [Test]
    [Description("A suspended user can't change anything with the controllers (POST), reading (GET) stays possible")]
    public async Task Suspended_User_Controller_Test()
    {
        var controller = this.board.CreateController<TestController>();

        (await RunActionAsync(controller, "POST")).Result.Should().BeNull("the user is not suspended");

        this.board.User.Suspended = DateTime.UtcNow.AddDays(1);

        (await RunActionAsync(controller, "POST")).Result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(403);
        this.handlerCalls.Should().Be(0, "the action must not run");

        (await RunActionAsync(controller, "GET")).Result.Should().BeNull();

        this.board.User.Suspended = DateTime.UtcNow.AddMinutes(-1);
        (await RunActionAsync(controller, "POST")).Result.Should().BeNull("the suspension has ended");
        this.handlerCalls.Should().Be(1);

        async Task<ActionExecutingContext> RunActionAsync(TestController c, string method)
        {
            this.handlerCalls = 0;
            this.board.HttpContext.Request.Method = method;

            var actionContext = new ActionContext(this.board.HttpContext, new RouteData(), new ActionDescriptor());
            var executing = new ActionExecutingContext(actionContext, [], new Dictionary<string, object>(), c);

            await c.OnActionExecutionAsync(
                executing,
                () =>
                {
                    this.handlerCalls++;
                    return Task.FromResult(new ActionExecutedContext(actionContext, [], c));
                });

            return executing;
        }
    }

    [Test]
    [Description("Moderator pages are only for moderators (of any forum) and admins")]
    public async Task Moderate_Page_Test()
    {
        (await this.RunAsync(new TestModeratePage())).Result.Should().BeSameAs(this.accessDenied);
        this.handlerCalls.Should().Be(0, "the handler must not run for normal users");

        this.board.PageLoad.IsAdmin = true;
        (await this.RunAsync(new TestModeratePage())).Result.Should().BeNull();
        this.handlerCalls.Should().Be(1);
    }

    [Test]
    [Description("The moderation of a forum needs moderator access to that forum, the overview to any forum")]
    public async Task Moderate_Forum_Page_Test()
    {
        // moderator of another forum
        this.board.PageLoad.IsModeratorAny = true;
        this.board.PageLoad.ModeratorAccess = false;

        (await this.RunAsync(new TestModeratePage())).Result.Should().BeNull("the overview is for every moderator");

        this.board.SetPage(this.board.Database.AddForum("Other"));
        (await this.RunAsync(new TestModeratePage())).Result.Should().BeSameAs(this.accessDenied);
        this.handlerCalls.Should().Be(0);

        this.board.PageLoad.ModeratorAccess = true;
        (await this.RunAsync(new TestModeratePage())).Result.Should().BeNull("a moderator of the forum");
    }

    private async Task<PageHandlerExecutingContext> RunAsync(ForumPage page)
    {
        this.handlerCalls = 0;

        var pageContext = new PageContext(
            new ActionContext(this.board.HttpContext, new RouteData(), new CompiledPageActionDescriptor()));
        var executing = new PageHandlerExecutingContext(pageContext, [], null, new Dictionary<string, object>(), page);

        await page.OnPageHandlerExecutionAsync(
            executing,
            () =>
            {
                this.handlerCalls++;
                return Task.FromResult(new PageHandlerExecutedContext(pageContext, [], null, page));
            });

        return executing;
    }

    private class TestController : ForumBaseController;

    private class TestModeratePage() : ModerateForumPage("MODERATE", ForumPages.Moderate_Moderate);

    private class TestPage : ForumPage
    {
        public TestPage(ForumPages page)
            : base("DEFAULT", page)
        {
        }

        public bool Account
        {
            set => this.IsAccountPage = value;
        }

        public bool Admin
        {
            set => this.IsAdminPage = value;
        }

        public bool Registered
        {
            set => this.IsRegisteredPage = value;
        }

        public bool Protected
        {
            set => this.IsProtected = value;
        }

        public bool WithoutDatabase
        {
            set => this.NoDataBase = value;
        }
    }
}
