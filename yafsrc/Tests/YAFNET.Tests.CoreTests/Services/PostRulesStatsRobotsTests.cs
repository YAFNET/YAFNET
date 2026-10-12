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

using System.IO;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Controllers;
using YAF.Core.Helpers;
using YAF.Core.Middleware;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The post action rules of the post list (<see cref="PostDataHelperWrapper"/>), the admin user statistics
/// and robots.txt.
/// </summary>
[TestFixture]
[NonParallelizable]
public class PostRulesStatsRobotsTests
{
    private const int Me = 2;

    private const int Other = 3;

    private TestBoardContext board;

    private string webRoot;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-robots-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(this.webRoot, "resources"));

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        var geoIp = Substitute.For<IGeoIpCountryService>();
        geoIp.DatabaseExists().Returns(true);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
                b.RegisterInstance(geoIp).As<IGeoIpCountryService>();
            },
            useDatabase: true);

        this.board.PageLoad.UserID = Me;
        this.board.PageLoad.EditAccess = true;
        this.board.PageLoad.DeleteAccess = true;
        this.board.PageLoad.ReplyAccess = true;
        this.board.PageLoad.UploadAccess = true;
        this.board.BoardSettings.LockPosts = 0;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Directory.Delete(this.webRoot, true);
    }

    [Test]
    [Description("Authors can edit, delete and attach to their own posts, not to the posts of others")]
    public void Own_Post_Test()
    {
        var own = Post(Me);
        var other = Post(Other);

        own.CanEditPost.Should().BeTrue();
        own.CanDeletePost.Should().BeTrue();
        own.CanAttach.Should().BeTrue();
        own.CanThankPost.Should().BeFalse("nobody thanks their own post");

        other.CanEditPost.Should().BeFalse();
        other.CanDeletePost.Should().BeFalse();
        other.CanAttach.Should().BeFalse();
        other.CanThankPost.Should().BeTrue();
        other.CanReply.Should().BeTrue();
    }

    [Test]
    [Description("Without edit / delete / upload / reply access nothing is allowed, also on own posts")]
    public void No_Access_Test()
    {
        this.board.PageLoad.EditAccess = false;
        this.board.PageLoad.DeleteAccess = false;
        this.board.PageLoad.UploadAccess = false;
        this.board.PageLoad.ReplyAccess = false;

        var own = Post(Me);

        own.CanEditPost.Should().BeFalse();
        own.CanDeletePost.Should().BeFalse();
        own.CanAttach.Should().BeFalse();
        own.CanReply.Should().BeFalse();
    }

    [Test]
    [Description("Locked posts, topics and forums can not be edited by the author, moderators still can")]
    [TestCase("message")]
    [TestCase("topic")]
    [TestCase("forum")]
    public void Locked_Test(string what)
    {
        var post = Post(
            Me,
            messageFlags: what == "message" ? (int)MessageFlags.Flags.IsLocked : 0,
            topicFlags: what == "topic" ? (int)TopicFlags.Flags.IsLocked : 0,
            forumFlags: what == "forum" ? (int)ForumFlags.Flags.IsLocked : 0);

        post.CanEditPost.Should().BeFalse();
        post.CanDeletePost.Should().BeFalse();
        post.CanReply.Should().BeFalse();

        this.board.PageLoad.ModeratorAccess = true;

        post.CanEditPost.Should().BeTrue();
        post.CanReply.Should().BeTrue();
    }

    [Test]
    [Description("Posts are locked automatically after the lock period, not for admins")]
    public void Auto_Lock_Test()
    {
        this.board.BoardSettings.LockPosts = 7;

        Post(Me, edited: DateTime.UtcNow.AddDays(-3)).PostLocked.Should().BeFalse();
        Post(Me, edited: DateTime.UtcNow.AddDays(-10)).PostLocked.Should().BeTrue();
        Post(Me, edited: DateTime.UtcNow.AddDays(-10)).CanEditPost.Should().BeFalse();

        this.board.PageLoad.IsAdmin = true;
        Post(Me, edited: DateTime.UtcNow.AddDays(-10)).PostLocked.Should().BeFalse();
    }

    [Test]
    [Description("Editing a post does not extend the lock period, it starts when the post was written")]
    public void Auto_Lock_From_Posted_Test()
    {
        this.board.BoardSettings.LockPosts = 7;

        var post = new PostDataHelperWrapper(
            new PagedMessage
            {
                UserID = Me,
                Posted = DateTime.UtcNow.AddDays(-10),
                Edited = DateTime.UtcNow.AddDays(-1)
            });

        post.PostLocked.Should().BeTrue();
    }

    [Test]
    [Description("Guest posts can be edited from the same IP address only")]
    public void Guest_Post_Test()
    {
        Post(Other, isGuest: true, ip: "203.0.113.10").CanEditPost.Should().BeTrue("the request comes from that address");
        Post(Other, isGuest: true, ip: "198.51.100.1").CanEditPost.Should().BeFalse();
    }

    [Test]
    [Description("Deleted posts can be restored by who may delete them")]
    public void Deleted_Test()
    {
        var post = Post(Me, messageFlags: (int)MessageFlags.Flags.IsDeleted);

        post.PostDeleted.Should().BeTrue();
        post.CanUnDeletePost.Should().BeTrue();
        Post(Me).CanUnDeletePost.Should().BeFalse("it is not deleted");

        var deletedByModerator = Post(Me, messageFlags: (int)MessageFlags.Flags.IsDeleted);
        deletedByModerator.DataRow.IsModeratorChanged = true;

        deletedByModerator.CanUnDeletePost.Should().BeFalse("a moderator deleted it");

        this.board.PageLoad.ModeratorAccess = true;

        deletedByModerator.CanUnDeletePost.Should().BeTrue();

        new PostDataHelperWrapper().MessageId.Should().Be(0);
    }

    [Test]
    [Description("The admin statistics group the active users and the registrations by month")]
    public async Task Stats_Test()
    {
        this.AddActive("Firefox", "Windows", "DE");
        this.AddActive("Firefox", "Linux", "--");
        this.AddActive("Chrome", "Windows", null);

        this.board.Database.AddUser("Recent", u => u.Joined = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1));

        var stats = await this.GetStatsAsync();

        stats.Browsers.Should().BeEquivalentTo([new StatsData { Label = "Firefox", Data = 2 }, new StatsData { Label = "Chrome", Data = 1 }]);
        stats.Platforms.Single(p => p.Label == "Windows").Data.Should().Be(2);
        stats.Countries.Select(c => c.Label).Should().Contain("--").And.HaveCount(2);
        stats.Registrations.Should().ContainSingle();
    }

    [Test]
    [Description("Month names shorter than three characters (e.g. Chinese) do not break the statistics")]
    public async Task Stats_Short_Month_Names_Test()
    {
        for (var month = 1; month <= 12; month++)
        {
            this.board.Localization.Add("CALENDER", month.ToString(), "一月");
        }

        this.board.Database.AddUser("Recent", u => u.Joined = DateTime.UtcNow.AddDays(-1));

        var stats = await this.GetStatsAsync();

        stats.Should().NotBeNull();
        stats.Registrations.Single().Label.Should().StartWith("一月");
    }

    [Test]
    [Description("Without a robots.txt file a valid default is served")]
    public async Task Robots_Default_Test()
    {
        var body = await this.GetRobotsAsync();

        body.Split('\n').Select(l => l.Trim()).Should().Equal("User-Agent: *", "Allow: /");
    }

    [Test]
    [Description("The environment robots.txt wins over the general one, the sitemap url is the forum's")]
    public async Task Robots_File_Test()
    {
        var resources = Path.Combine(this.webRoot, "resources");
        await File.WriteAllTextAsync(Path.Combine(resources, "robots.txt"), "User-Agent: *\nDisallow: /general");

        (await this.GetRobotsAsync()).Should().Contain("/general");

        await File.WriteAllTextAsync(
            Path.Combine(resources, "robots.Production.txt"),
            "User-Agent: *\nSitemap: https://www.mydomain.com/sitemap.xml");

        (await this.GetRobotsAsync()).Should().Contain("Sitemap: https://forum.example.com/sitemap.xml");
    }

    [Test]
    [Description("Other requests pass through")]
    public async Task Robots_Other_Path_Test()
    {
        var next = false;
        var context = new DefaultHttpContext();
        context.Request.Path = "/forum";

        await new RobotsTxtMiddleware(_ => { next = true; return Task.CompletedTask; }, "Production", this.board.Resolve<IServiceLocator>())
            .InvokeAsync(context);

        next.Should().BeTrue();
    }

    private static PostDataHelperWrapper Post(
        int userId,
        int messageFlags = 0,
        int topicFlags = 0,
        int forumFlags = 0,
        bool isGuest = false,
        string ip = "127.0.0.1",
        DateTime? edited = null)
    {
        return new PostDataHelperWrapper(
            new PagedMessage
            {
                MessageID = 1,
                TopicID = 1,
                UserID = userId,
                Flags = messageFlags,
                TopicFlags = topicFlags,
                ForumFlags = forumFlags,
                IsGuest = isGuest,
                IP = ip,
                Posted = edited ?? DateTime.UtcNow,
                Edited = edited ?? DateTime.UtcNow
            });
    }

    private void AddActive(string browser, string platform, string country)
    {
        using var connection = this.board.Database.Open();

        connection.Insert(
            new Active
            {
                BoardID = TestDatabase.BoardId,
                UserID = 1,
                SessionID = Guid.NewGuid().ToString(),
                Browser = browser,
                Platform = platform,
                Country = country,
                IP = "127.0.0.1",
                Login = DateTime.UtcNow,
                LastActive = DateTime.UtcNow
            });
    }

    private async Task<UserStats> GetStatsAsync()
    {
        this.board.BoardSettings.BoardId = TestDatabase.BoardId;

        var result = await this.board.CreateController<Stats>().GetUserStats();

        return (UserStats)((OkObjectResult)result.Result)?.Value;
    }

    private async Task<string> GetRobotsAsync()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/robots.txt";
        context.Response.Body = new MemoryStream();

        await new RobotsTxtMiddleware(_ => Task.CompletedTask, "Production", this.board.Resolve<IServiceLocator>())
            .InvokeAsync(context);

        context.Response.Body.Position = 0;
        return await new StreamReader(context.Response.Body).ReadToEndAsync();
    }
}
