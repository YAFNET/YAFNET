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

using System.IO;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using NSubstitute;

using YAF.Core.Controllers;
using YAF.Core.Handlers;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Objects;

/// <summary>
/// The feed endpoints (who may read which feed) and the theme selection.
/// </summary>
[TestFixture]
[NonParallelizable]
public class FeedAndThemeTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private IPermissions permissions;

    private string webRoot;

    private int alice;

    private int forum;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-theme-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(this.webRoot, "css", "themes", "yaf"));
        Directory.CreateDirectory(Path.Combine(this.webRoot, "css", "themes", "dark"));

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        this.permissions = Substitute.For<IPermissions>();
        this.permissions.Check(Arg.Any<ViewPermissions>()).Returns(true);

        var format = Substitute.For<IFormatMessage>();
        format.FormatSyndicationMessageAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(c => Task.FromResult(c.ArgAt<string>(0)));

        var badWords = Substitute.For<IBadWordReplace>();
        badWords.Replace(Arg.Any<string>()).Returns(c => c.Arg<string>());
        badWords.ReplaceHtml(Arg.Any<string>()).Returns(c => c.Arg<string>());

        var urlHelper = Substitute.For<IUrlHelper>();
        urlHelper.Action(Arg.Any<UrlActionContext>()).Returns("/api/Feed/GetLatestPosts");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
                b.RegisterType<SyndicationFeeds>().AsSelf();
                b.RegisterInstance(this.permissions).As<IPermissions>();
                b.RegisterInstance(format).As<IFormatMessage>();
                b.RegisterInstance(badWords).As<IBadWordReplace>();
                b.RegisterInstance(urlHelper).As<IUrlHelper>();
                b.RegisterInstance(Substitute.For<IDateTimeService>()).As<IDateTimeService>();
                b.RegisterInstance(Substitute.For<IUserDisplayName>()).As<IUserDisplayName>();
            },
            useDatabase: true);

        this.board.LinkBuilder.GetAbsoluteLink(Arg.Any<ForumPages>(), Arg.Any<object>()).Returns("https://forum.example.com/post");
        this.board.LinkBuilder.GetUserProfileLink(Arg.Any<int>(), Arg.Any<string>()).Returns("https://forum.example.com/user");

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.forum = this.db.AddForum("General");
        this.topic = this.db.AddTopic(this.forum, this.alice, "Hello");
        this.db.AddMessage(this.topic, this.alice, "text");
        this.board.GrantAccess(this.alice, this.forum);
        this.board.LoginAs(this.alice);

        this.board.BoardSettings.ShowAtomLink = true;
        this.board.BoardSettings.ShowActiveDiscussions = true;
        this.board.PageLoad.ReadAccess = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Directory.Delete(this.webRoot, true);
    }

    [Test]
    [Description("The three feeds are served as atom xml")]
    public async Task Feeds_Served_Test()
    {
        (await ReadAsync(await this.Feed().GetLatestPosts())).Should().Contain("<feed").And.Contain("Hello");
        (await ReadAsync(await this.Feed().GetTopicsFeed(this.forum))).Should().Contain("Hello");

        this.board.SetPage(this.forum, this.topic);
        this.board.PageLoad.ReadAccess = true;
        (await ReadAsync(await this.Feed().GetPostsFeed(this.topic))).Should().Contain("text");
    }

    [Test]
    [Description("No feeds when atom links are disabled, for guests on login-only boards, or without permission")]
    public async Task Feeds_Denied_Test()
    {
        this.board.BoardSettings.ShowAtomLink = false;
        (await this.Feed().GetLatestPosts()).Should().BeOfType<NotFoundResult>();

        this.board.BoardSettings.ShowAtomLink = true;
        this.board.BoardSettings.RequireLogin = true;
        this.board.User.IsGuest = true;
        (await this.Feed().GetTopicsFeed(this.forum)).Should().BeOfType<NotFoundResult>();

        this.board.BoardSettings.RequireLogin = false;
        this.board.User.IsGuest = false;
        this.permissions.Check(Arg.Any<ViewPermissions>()).Returns(false);
        (await this.Feed().GetPostsFeed(this.topic)).Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("Topic and post feeds need read access to the forum")]
    public async Task Feeds_Read_Access_Test()
    {
        this.board.PageLoad.ReadAccess = false;

        (await this.Feed().GetTopicsFeed(this.forum)).Should().BeOfType<NotFoundResult>();
        (await this.Feed().GetPostsFeed(this.topic)).Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("The board theme is used, a user theme only when allowed, invalid themes fall back to yaf")]
    public void Theme_Test()
    {
        this.board.BoardSettings.Theme = "dark";
        new ThemeProvider().Theme.ThemeFile.Should().Be("dark");

        this.board.PageUser.ThemeFile = "yaf";
        new ThemeProvider().Theme.ThemeFile.Should().Be("dark", "user themes are not allowed");

        this.board.BoardSettings.AllowUserTheme = true;
        this.board.PageUser.ThemeFile = "dark";
        this.board.BoardSettings.Theme = "yaf";
        new ThemeProvider().Theme.ThemeFile.Should().Be("dark");

        this.board.PageUser.ThemeFile = "does-not-exist";
        new ThemeProvider().Theme.ThemeFile.Should().Be("yaf");
    }

    [Test]
    [Description("Theme names can not point outside the themes folder")]
    [TestCase("..")]
    [TestCase("../..")]
    [TestCase("dark/../yaf")]
    [TestCase("/")]
    public void Theme_Path_Traversal_Test(string theme)
    {
        Theme.IsValidTheme(theme).Should().BeFalse();
        Theme.IsValidTheme("dark").Should().BeTrue();
    }

    private Feed Feed()
    {
        return this.board.CreateController<Feed>();
    }

    private async static Task<string> ReadAsync(ActionResult result)
    {
        var file = result.Should().BeOfType<FileContentResult>().Subject;

        return await Task.FromResult(Encoding.UTF8.GetString(file.FileContents));
    }
}
