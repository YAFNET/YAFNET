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

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using NSubstitute;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// <see cref="LinkBuilder"/> (page links with cleaned names and the area) and the breadcrumb
/// helpers in <see cref="PageLinkExtensions"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class LinkBuilderTests
{
    private TestBoardContext board;

    private BoardConfiguration configuration;

    private ILinkBuilder links;

    [SetUp]
    public void SetUp()
    {
        this.configuration = new BoardConfiguration();

        var linkGenerator = Substitute.For<LinkGenerator>();
        linkGenerator.GetPathByAddress(
                Arg.Any<HttpContext>(),
                Arg.Any<RouteValuesAddress>(),
                Arg.Any<RouteValueDictionary>(),
                Arg.Any<RouteValueDictionary>(),
                Arg.Any<PathString?>(),
                Arg.Any<FragmentString>(),
                Arg.Any<LinkOptions>())
            .Returns(c => Path(c.ArgAt<RouteValueDictionary>(2)));
        linkGenerator.GetPathByAddress(
                Arg.Any<RouteValuesAddress>(),
                Arg.Any<RouteValueDictionary>(),
                Arg.Any<PathString>(),
                Arg.Any<FragmentString>(),
                Arg.Any<LinkOptions>())
            .Returns(c => Path(c.ArgAt<RouteValueDictionary>(1)));

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<LinkBuilder>().As<ILinkBuilder>();
                b.RegisterInstance(linkGenerator).As<LinkGenerator>();
                b.RegisterInstance(this.configuration);
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterInstance(Options.Create(new RouteOptions())).As<IOptions<RouteOptions>>();

                var badWords = Substitute.For<IBadWordReplace>();
                badWords.Replace(Arg.Any<string>()).Returns(c => c.Arg<string>().Replace("darn", "****"));
                b.RegisterInstance(badWords).As<IBadWordReplace>();
            },
            useDatabase: true);

        this.links = this.board.Resolve<ILinkBuilder>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Links go to the page with the ids and a url friendly name, bad words are replaced in topic names")]
    public void Links_Test()
    {
        this.links.GetLink(ForumPages.Index).Should().Be("/Index");
        this.links.GetForumLink(5, "General Talk").Should().Be("/Topics?f=5&name=General-Talk");
        this.links.GetForumLink(new Forum { ID = 5, Name = "General Talk" }).Should().Be("/Topics?f=5&name=General-Talk");
        this.links.GetTopicLink(7, "darn it").Should().StartWith("/Posts?t=7&name=").And.NotContain("darn");
        this.links.GetMessageLink("Hello", 9).Should().Be("/Post?name=Hello&m=9");
        this.links.GetUserProfileLink(new User { ID = 3, Name = "alice", DisplayName = "Alice A" })
            .Should().Contain("u=3");
        this.links.GetCategoryLink(2, "News").Should().Be("/Category/2/News");
    }

    [Test]
    [Description("Absolute links start with the forum base url")]
    public void Absolute_Link_Test()
    {
        this.links.GetAbsoluteLink(ForumPages.Index).Should().Be("https://forum.example.com/Index");
        this.links.ForumUrl.Should().Be("https://forum.example.com/Index");
    }

    [Test]
    [Description("The board area is added to every link")]
    public void Area_Test()
    {
        this.configuration.Area = "Forums";

        this.links.GetLink(ForumPages.Index).Should().Be("/Index?area=Forums");
        this.links.GetCategoryLink(2, "News").Should().Be("/Forums/Category/2/News");
    }

    [Test]
    [Description("A link with a missing name (e.g. a deleted user) does not fail")]
    public void Null_Name_Test()
    {
        this.links.Invoking(l => l.GetUserProfileLink(3, null)).Should().NotThrow();
        this.links.Invoking(l => l.GetForumLink(5, null)).Should().NotThrow();
    }

    [Test]
    [Description("The breadcrumb lists the board, category, all parent forums and the topic")]
    public void Breadcrumb_Test()
    {
        var db = this.board.Database;
        var category = db.AddCategory("News");
        var top = db.AddForum("Top", category);
        var middle = db.AddForum("Middle", category, f => f.ParentID = top);
        var bottom = db.AddForum("Bottom", category, f => f.ParentID = middle);
        this.board.SetPage(bottom);
        this.board.BoardSettings.Name = "My Board";

        var crumbs = new List<PageLink>()
            .AddRoot()
            .AddCategory(new Category { ID = category, Name = "News" })
            .AddForum(this.board.PageForum)
            .AddTopic(new Topic { ID = 1, TopicName = "Hello" });

        crumbs.Select(c => c.Title).Should().Equal("My Board", "News", "Top", "Middle", "Bottom", "Hello");
    }

    [Test]
    [Description("A parent loop in the forum data does not hang the breadcrumb")]
    public void Breadcrumb_Parent_Loop_Test()
    {
        var db = this.board.Database;
        var a = db.AddForum("A");
        var b = db.AddForum("B", configure: f => f.ParentID = a);
        this.board.Current.GetRepository<Forum>().UpdateOnly(() => new Forum { ParentID = b }, f => f.ID == a);
        this.board.SetPage(b);

        new List<PageLink>().AddForum(this.board.PageForum).Select(c => c.Title).Should().Equal("A", "B");
    }

    [Test]
    [Description("Breadcrumb helpers ignore missing objects and empty names")]
    public void Breadcrumb_Missing_Test()
    {
        var crumbs = new List<PageLink>().AddTopic(null).AddCategory(null).AddForum(null);
        crumbs.Should().BeEmpty();

        new List<PageLink>().Invoking(l => l.AddUser(3, null)).Should().NotThrow();
        new List<PageLink>().AddLink("Title", null).Single().URL.Should().BeNull();
    }

    [Test]
    [Description("The current forum can be shown without a link")]
    public void Breadcrumb_Forum_Without_Link_Test()
    {
        var forum = this.board.Database.AddForum("General");
        this.board.SetPage(forum);

        new List<PageLink>().AddForum(this.board.PageForum, noForumLink: true).Single().URL.Should().BeEmpty();
    }

    [Test]
    [Description("Redirects from a controller go to the page with the route values")]
    public void Redirect_From_Controller_Test()
    {
        this.board.Current.CurrentForumController = this.board.CreateController<YAF.Core.Controllers.TagsController>();

        var result = this.links.AccessDenied().Should().BeOfType<RedirectToPageResult>().Subject;

        result.PageName.Should().Be(ForumPages.Info.GetPageName());
        result.RouteValues["info"].Should().Be((int)InfoMessage.AccessDenied);

        this.links.Redirect(ForumPages.Index).Should().BeOfType<RedirectToPageResult>()
            .Which.PageName.Should().Be(ForumPages.Index.GetPageName());
    }

    [Test]
    [Description("Without a page or controller the response is redirected to the full link, including the values")]
    public void Redirect_Without_Page_Test()
    {
        this.links.RedirectInfoPage(InfoMessage.Failure).Should().BeOfType<EmptyResult>();

        this.board.HttpContext.Response.Headers.Location.ToString().Should()
            .Be($"/Info?info={(int)InfoMessage.Failure}");
    }

    [Test]
    [Description("Message links go to the post page with a url friendly topic name")]
    public void Message_Link_Test()
    {
        this.links.GetMessageLink(new Topic { ID = 3, TopicName = "Hello World" }, 42).Should()
            .Be("/Post?name=Hello-World&m=42");
    }

    private static string Path(RouteValueDictionary values)
    {
        var page = values["page"]?.ToString();
        var query = string.Join(
            "&",
            values.Where(v => v.Key != "page").Select(v => $"{v.Key}={v.Value}"));

        return query.Length > 0 ? $"{page}?{query}" : page;
    }
}
