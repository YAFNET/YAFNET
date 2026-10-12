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

namespace YAF.Tests.Services;

using System.Text.RegularExpressions;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Rewrite;

using ServiceStack.OrmLite;

using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Services;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Medal ribbons of a user, the collapsible panel state and the category urls.
/// </summary>
[TestFixture]
[NonParallelizable]
public class MedalPanelRouteTests
{
    private TestBoardContext board;

    private int alice;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<UserMedalService>().As<IUserMedalService>();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
            },
            useDatabase: true);

        this.alice = this.board.Database.AddUser("Alice");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The medal ribbon lists the user's medals, hidden medals are left out")]
    public void Medals_Test()
    {
        this.AwardMedal("Gold", MedalFlags.Flags.ShowMessage, "for helping", hide: false);
        this.AwardMedal("Secret", MedalFlags.Flags.AllowHiding, "x", hide: true);

        var html = this.board.Resolve<IUserMedalService>().GetUserMedals(this.alice);

        html.Should().StartWith("<ul").And.Contain("Gold: for helping").And.Contain("images/Medals/Gold.png");
        html.Should().NotContain("Secret");
    }

    [Test]
    [Description("A user without medals has no ribbon")]
    public void No_Medals_Test()
    {
        this.board.Resolve<IUserMedalService>().GetUserMedals(this.alice).Should().BeEmpty();
    }

    [Test]
    [Description("Quotes in a medal message do not break the html attributes")]
    public void Medal_Message_Encoded_Test()
    {
        this.AwardMedal("Gold", MedalFlags.Flags.ShowMessage, "He said \"thanks\" <3", hide: false);

        var html = this.board.Resolve<IUserMedalService>().GetUserMedals(this.alice);

        html.Should().Contain("&quot;thanks&quot;").And.Contain("&lt;3");
        html.Should().NotContain("\"thanks\"");
    }

    [Test]
    [Description("The panel state is read from the session, then from the cookie")]
    public void Panel_State_Test()
    {
        var panels = new PanelSessionState();

        panels["forum"].Should().Be(CollapsiblePanelState.None);

        panels["forum"] = CollapsiblePanelState.Collapsed;
        panels["forum"].Should().Be(CollapsiblePanelState.Collapsed);
        this.board.HttpContext.Response.Headers.SetCookie.ToString().Should().Contain("panelstate_forum=1");

        panels.TogglePanelState("forum", CollapsiblePanelState.Expanded);
        panels["forum"].Should().Be(CollapsiblePanelState.Expanded);

        panels.TogglePanelState("other", CollapsiblePanelState.Expanded);
        panels["other"].Should().Be(CollapsiblePanelState.Collapsed, "an unknown panel toggles from its default");
    }

    [Test]
    [Description("A panel state cookie is used when the session has none, a broken cookie is ignored")]
    [TestCase("1", CollapsiblePanelState.Collapsed)]
    [TestCase("abc", CollapsiblePanelState.None)]
    public void Panel_State_Cookie_Test(string cookie, CollapsiblePanelState expected)
    {
        this.board.HttpContext.Request.Headers.Cookie = $"panelstate_forum={cookie}";

        new PanelSessionState()["forum"].Should().Be(expected);
    }

    [Test]
    [Description("Category urls contain id and a cleaned name, with the route options")]
    public void Category_Url_Test()
    {
        CategoryRouteHelper.BuildUrl(null, 5, "General Talk").Should().Be("/Category/5/General-Talk");
        CategoryRouteHelper.BuildUrl("Forums", 5, "General Talk").Should().Be("/Forums/Category/5/General-Talk");
        CategoryRouteHelper.BuildUrl(null, 5, "General Talk", new RouteOptions { LowercaseUrls = true, AppendTrailingSlash = true })
            .Should().Be("/category/5/general-talk/");
    }

    [Test]
    [Description("Every category url can be routed, also when the name has no url characters")]
    [TestCase("General Talk")]
    [TestCase("日本語")]
    [TestCase("!!!")]
    public void Category_Url_Routes_Test(string name)
    {
        var url = CategoryRouteHelper.BuildUrl(null, 5, name);

        var rules = CategoryRouteHelper.AddCategoryRules(new RewriteOptions(), null);
        var context = new RewriteContext { HttpContext = new DefaultHttpContext() };
        context.HttpContext.Request.Path = url;

        foreach (var rule in rules.Rules)
        {
            rule.ApplyRule(context);
        }

        context.HttpContext.Request.Path.Value.Should().Be("/Index", $"{url} must be rewritten to the index page");
        context.HttpContext.Request.QueryString.Value.Should().Be("?c=5");
    }

    private void AwardMedal(string name, MedalFlags.Flags flags, string message, bool hide)
    {
        using var connection = this.board.Database.Open();

        var medal = (int)connection.Insert(
            new Medal
            {
                BoardID = TestDatabase.BoardId,
                Name = name,
                Message = message,
                Category = "c",
                MedalURL = $"{name}.png",
                Flags = (int)flags
            },
            selectIdentity: true);

        connection.Insert(new UserMedal { UserID = this.alice, MedalID = medal, Hide = hide, SortOrder = 1, DateAwarded = DateTime.UtcNow });
    }
}
