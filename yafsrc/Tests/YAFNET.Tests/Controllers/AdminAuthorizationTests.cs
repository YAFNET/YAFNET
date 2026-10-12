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

using System.Reflection;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

using YAF.Core.Controllers.Modals;
using YAF.Core.Extensions;
using YAF.Core.Filters;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Models;

/// <summary>
/// The admin api (the dialogs of the admin pages) needs the access to the admin page like the page itself.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AdminAuthorizationTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);

        this.board.LoginAs(this.board.Database.AddUser("Alice"));
        this.board.PageLoad.IsAdmin = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Any admin can use an api without admin page")]
    public void Admin_Without_Page_Test()
    {
        this.Authorize(new AdminAuthorizationAttribute()).Should().BeNull();

        this.board.PageLoad.IsAdmin = false;
        this.Authorize(new AdminAuthorizationAttribute()).Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(403);
    }

    [Test]
    [Description("An admin who is no host admin needs the access to the admin page")]
    public void Admin_Page_Access_Test()
    {
        var attribute = new AdminAuthorizationAttribute(ForumPages.Admin_BannedIps);

        this.Authorize(attribute).Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(403, "the admin has no access to the banned ips page");

        this.board.Current.GetRepository<AdminPageUserAccess>().Insert(
            new AdminPageUserAccess { UserID = this.board.PageLoad.UserID, PageName = nameof(ForumPages.Admin_BannedIps) });

        this.Authorize(attribute).Should().BeNull();

        this.Authorize(new AdminAuthorizationAttribute(ForumPages.Admin_SpamWords)).Should().BeOfType<StatusCodeResult>(
            "the access to one page is no access to the others");
    }

    [Test]
    [Description("The host admin can use all admin apis")]
    public void Host_Admin_Test()
    {
        this.board.LoginAs(this.board.Database.AddUser("Host", u => u.Flags |= (int)UserFlags.Flags.IsHostAdmin));
        this.board.PageLoad.IsAdmin = true;

        this.Authorize(new AdminAuthorizationAttribute(ForumPages.Admin_BannedIps)).Should().BeNull();
    }

    [Test]
    [Description("The admin dialogs use the access of their admin page")]
    public void Admin_Dialogs_Have_Page_Test()
    {
        var adminControllers = typeof(MedalController).Assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<AdminAuthorizationAttribute>() != null)
            .ToList();

        adminControllers.Should().NotBeEmpty();

        // the user access of a forum is managed on the moderate page (no admin page)
        adminControllers.Where(t => t != typeof(ModForumUserController)).Should().OnlyContain(
            t => t.GetCustomAttribute<AdminAuthorizationAttribute>().Page.HasValue,
            "an admin without access to the page must not use its dialogs");

        typeof(BannedIpController).GetCustomAttribute<AdminAuthorizationAttribute>().Page.Should().Be(ForumPages.Admin_BannedIps);
        typeof(UsersImportController).GetCustomAttribute<AdminAuthorizationAttribute>().Page.Should().Be(ForumPages.Admin_Users);
    }

    private IActionResult Authorize(AdminAuthorizationAttribute attribute)
    {
        var httpContext = new DefaultHttpContext
                              {
                                  User = new ClaimsPrincipal(
                                      new ClaimsIdentity([new Claim(ClaimTypes.Name, "Alice")], "Test"))
                              };

        var context = new AuthorizationFilterContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            []);

        attribute.OnAuthorization(context);

        return context.Result;
    }
}
