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
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Controllers;
using YAF.Core.Extensions;
using YAF.Core.Controllers.Modals;
using YAF.Core.Events;
using YAF.Core.Events.Cache;
using YAF.Core.Middleware;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Events;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// Banned user agents (middleware and admin dialog) and the avatar images.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserAgentAndAvatarTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private ILogger<AvatarController> avatarLogger;

    [SetUp]
    public void SetUp()
    {
        this.avatarLogger = Substitute.For<ILogger<AvatarController>>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<ServiceLocatorEventRaiser>().As<IRaiseEvent>();
                b.RegisterType<ClearCacheOnEvents>().AsImplementedInterfaces();
                b.RegisterInstance(this.avatarLogger).As<ILogger<AvatarController>>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.board.PageLoad.IsAdmin = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Requests with a banned user agent (regex, case insensitive) get 403, others pass")]
    [TestCase("EvilBot/1.0", false)]
    [TestCase("mozilla/5.0 evilbot", false)]
    [TestCase("Mozilla/5.0 Firefox", true)]
    public async Task Banned_UserAgent_Test(string userAgent, bool allowed)
    {
        this.Ban("EvilBot");

        var (next, status) = await this.RunMiddlewareAsync(userAgent);

        next.Should().Be(allowed);
        status.Should().Be(allowed ? 200 : 403);
    }

    [Test]
    [Description("Requests without a user agent are blocked")]
    public async Task Empty_UserAgent_Test()
    {
        (await this.RunMiddlewareAsync(string.Empty)).Status.Should().Be(403);
    }

    [Test]
    [Description("An invalid banned pattern does not break every request, the valid patterns still apply")]
    public async Task Invalid_Pattern_Test()
    {
        this.Ban("Bot(");
        this.Ban("EvilBot");

        (await this.RunMiddlewareAsync("Mozilla/5.0 Firefox")).Next.Should().BeTrue();
        (await this.RunMiddlewareAsync("EvilBot/1.0")).Status.Should().Be(403);
    }

    [Test]
    [Description("The admin dialog saves a pattern, rejects duplicates and invalid regular expressions")]
    public void Admin_Edit_Test()
    {
        var controller = this.board.CreateController<BannedUserAgentController>();

        controller.Edit(new BannedEmailEditModal { Mask = " EvilBot " }).Should().BeOfType<OkResult>();
        controller.Edit(new BannedEmailEditModal { Mask = "EvilBot" }).Should().BeOfType<OkObjectResult>("a duplicate returns a warning");
        controller.Edit(new BannedEmailEditModal { Mask = "Bot(" });

        this.board.Current.GetRepository<BannedUserAgent>().Get(_ => true).Select(b => b.UserAgent).Should().Equal("EvilBot");
    }

    [Test]
    [Description("Editing a banned user agent drops the cached list")]
    public void Admin_Edit_Clears_Cache_Test()
    {
        var controller = this.board.CreateController<BannedUserAgentController>();
        controller.Edit(new BannedEmailEditModal { Mask = "EvilBot" });
        var id = this.board.Current.GetRepository<BannedUserAgent>().GetSingle(_ => true).ID;

        this.board.Cache.Set(Constants.Cache.BannedUserAgent, new List<string> { "EvilBot" });

        controller.Edit(new BannedEmailEditModal { Id = id, Mask = "OtherBot" });

        this.board.Cache.Get(Constants.Cache.BannedUserAgent).Should().BeNull();
    }

    [Test]
    [Description("The text avatar is an svg with the initials of the user")]
    public async Task Text_Avatar_Test()
    {
        var user = this.db.AddUser("Alice Smith", u => u.ProviderUserKey = "abcdef12-0000-0000-0000-000000000000");
        var guest = this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));

        var svg = await ReadAsync(this.board.CreateController<AvatarController>().GetTextAvatar(user));
        svg.Should().Contain("<svg").And.Contain("#abcdef").And.Contain("AS");

        (await ReadAsync(this.board.CreateController<AvatarController>().GetTextAvatar(guest))).Should().Contain("#0c7333");

        this.board.CreateController<AvatarController>().GetTextAvatar(99999).Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("Text avatars are valid xml for every name, users without provider key get a color too")]
    public async Task Text_Avatar_Names_Test()
    {
        var andName = this.db.AddUser("Tom & Jerry", u => u.ProviderUserKey = "0a0b0c-key");
        var spaces = this.db.AddUser("Alice  Wonder", u => u.ProviderUserKey = null);
        var emoji = this.db.AddUser("😀😀", u => u.ProviderUserKey = "xyz");

        foreach (var user in new[] { andName, spaces, emoji })
        {
            var svg = await ReadAsync(this.board.CreateController<AvatarController>().GetTextAvatar(user));

            FluentActions.Invoking(() => System.Xml.Linq.XDocument.Parse(svg)).Should().NotThrow($"the avatar of user {user} is valid xml");
            svg.Should().MatchRegex("fill=\"#[0-9a-f]{6}\"");
        }

        (await ReadAsync(this.board.CreateController<AvatarController>().GetTextAvatar(andName))).Should()
            .Contain("T&amp;").And.Contain("#0a0b0c");
        (await ReadAsync(this.board.CreateController<AvatarController>().GetTextAvatar(spaces))).Should().Contain("AW");
    }

    [Test]
    [Description("Uploaded avatars are served with their image type")]
    public async Task Local_Avatar_Test()
    {
        var user = this.db.AddUser("Alice", u =>
        {
            u.AvatarImage = [1, 2, 3];
            u.AvatarImageType = "image/png";
        });

        var result = (FileStreamResult)this.board.CreateController<AvatarController>().GetResponseLocalAvatar(user);
        result.ContentType.Should().Be("image/png");
        (await ReadAsync(result)).Length.Should().Be(3);

        this.board.CreateController<AvatarController>().GetResponseLocalAvatar(this.db.AddUser("NoAvatar"))
            .Should().BeOfType<NotFoundResult>();

        // anyone can request it, every request wrote an error (exception) to the event log
        this.avatarLogger.ReceivedCalls().Should().BeEmpty("a user without uploaded avatar is no error");
    }

    [Test]
    [Description("An avatar is never served as html or another non image type, whatever the upload claimed")]
    [TestCase("text/html")]
    [TestCase("image/svg+xml")]
    [TestCase("application/javascript")]
    public void Local_Avatar_Content_Type_Test(string storedType)
    {
        var user = this.db.AddUser("Mallory", u =>
        {
            u.AvatarImage = [1, 2, 3];
            u.AvatarImageType = storedType;
        });

        var result = (FileStreamResult)this.board.CreateController<AvatarController>().GetResponseLocalAvatar(user);

        result.ContentType.Should().BeOneOf("image/png", "image/jpeg", "image/gif", "image/webp", "image/bmp");
    }

    [Test]
    [Description("Users of the old membership (numeric provider key) get their own colors, all of them were black")]
    public async Task Text_Avatar_Numeric_Key_Test()
    {
        var first = this.db.AddUser("Old Member", u => u.ProviderUserKey = "42");
        var second = this.db.AddUser("Other Member", u => u.ProviderUserKey = "43");
        var longKey = this.db.AddUser("Long Member", u => u.ProviderUserKey = "1234567");

        var colors = new List<string>();

        foreach (var user in new[] { first, second })
        {
            var svg = await ReadAsync(this.board.CreateController<AvatarController>().GetTextAvatar(user));
            colors.Add(System.Text.RegularExpressions.Regex.Match(svg, "<rect fill=\"(#[0-9a-f]{6})\"").Groups[1].Value);
        }

        colors.Should().NotContain("#000000").And.OnlyHaveUniqueItems();

        (await ReadAsync(this.board.CreateController<AvatarController>().GetTextAvatar(longKey))).Should()
            .Contain("fill=\"#123456\"");
    }

    [TestCase("image/jpg", "image/jpeg")]
    [TestCase("image/pjpeg", "image/jpeg")]
    [TestCase(" IMAGE/GIF ", "image/gif")]
    [TestCase("", "image/jpeg")]
    [TestCase(null, "image/jpeg")]
    [Description("Old and unusual names of the image types are served with the standard type")]
    public void Local_Avatar_Type_Names_Test(string storedType, string expected)
    {
        var user = this.db.AddUser("Alice", u =>
        {
            u.AvatarImage = [1, 2, 3];
            u.AvatarImageType = storedType;
        });

        ((FileStreamResult)this.board.CreateController<AvatarController>().GetResponseLocalAvatar(user)).ContentType
            .Should().Be(expected);
    }

    [Test]
    [Description("The avatar of an unknown user is not found")]
    public void Local_Avatar_Unknown_User_Test()
    {
        this.board.CreateController<AvatarController>().GetResponseLocalAvatar(99999).Should().BeOfType<NotFoundResult>();
        this.avatarLogger.ReceivedCalls().Should().BeEmpty();
    }

    private void Ban(string pattern)
    {
        using var connection = this.db.Open();
        connection.Insert(new BannedUserAgent { BoardID = TestDatabase.BoardId, UserAgent = pattern });
    }

    private async Task<(bool Next, int Status)> RunMiddlewareAsync(string userAgent)
    {
        this.board.Cache.Remove(Constants.Cache.BannedUserAgent);

        var context = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        context.Request.Headers.UserAgent = userAgent;

        var nextCalled = false;

        await new CheckBannedUserAgents(
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            this.board.Resolve<IServiceLocator>()).InvokeAsync(context);

        return (nextCalled, context.Response.StatusCode);
    }

    private static async Task<string> ReadAsync(ActionResult result)
    {
        var file = (FileStreamResult)result;
        using var reader = new StreamReader(file.FileStream);
        return await reader.ReadToEndAsync();
    }
}
