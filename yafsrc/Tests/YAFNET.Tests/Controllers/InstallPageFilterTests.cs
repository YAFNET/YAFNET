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

using Autofac;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;

using NSubstitute;

using YAF.Core.BasePages;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;

/// <summary>
/// The install steps can only be used while the forum is not installed (<see cref="InstallPage.OnPageHandlerExecuting"/>).
/// </summary>
[TestFixture]
[NonParallelizable]
public class InstallPageFilterTests
{
    private TestBoardContext board;

    private IActionResult indexRedirect;

    [SetUp]
    public void SetUp()
    {
        this.indexRedirect = new RedirectResult("/");

        this.board = new TestBoardContext(b => b.RegisterType<InstallService>().AsSelf(), useDatabase: true);

        this.board.LinkBuilder.Redirect(ForumPages.Index).Returns(this.indexRedirect);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("An installation that was not completed (the step stays in the cache) is ended: the steps (e.g. the one that creates an admin) can't be used on an installed forum")]
    public void Not_Completed_Installation_Is_Ended_Test()
    {
        this.board.Cache.Set("Install", "InstallFinished");

        this.Run("InstallFinished").Result.Should().BeSameAs(this.indexRedirect);
        this.board.Cache.Get("Install").Should().BeNull("all pages were sent to the install steps");

        this.board.Cache.Set("Install", "InstallFinished");

        this.Run(null).Result.Should().BeSameAs(this.indexRedirect, "the welcome step can't be started again");
        this.board.Cache.Get("Install").Should().BeNull();
    }

    [Test]
    [Description("The last step of the installation (complete) can be used, without an installation it is refused too")]
    public void Complete_Test()
    {
        this.board.Cache.Set("Install", "InstallFinished");

        this.Run("Complete").Result.Should().BeNull();

        this.board.Cache.Remove("Install");

        this.Run("Complete").Result.Should().BeSameAs(this.indexRedirect);
        this.Run("InstallCreateForum").Result.Should().BeSameAs(this.indexRedirect);
    }

    private PageHandlerExecutingContext Run(string handler)
    {
        var page = new TestInstallPage();

        var pageContext = new PageContext(
            new ActionContext(this.board.HttpContext, new RouteData(), new CompiledPageActionDescriptor()));
        var executing = new PageHandlerExecutingContext(
            pageContext,
            [],
            handler is null ? null : new HandlerMethodDescriptor { Name = handler, HttpMethod = "POST" },
            new Dictionary<string, object>(),
            page);

        page.OnPageHandlerExecuting(executing);

        return executing;
    }

    private sealed class TestInstallPage : InstallPage;
}
