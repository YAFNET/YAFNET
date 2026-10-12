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

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Controllers;
using YAF.Core.Helpers;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// The user lookups: the profile hover card (UserInfo), the admin user picker and the @mention list.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserControllersTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private IPermissions permissions;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        this.permissions = Substitute.For<IPermissions>();
        this.permissions.Check(Arg.Any<ViewPermissions>()).Returns(true);

        var dateTime = Substitute.For<IDateTimeService>();
        dateTime.FormatDateLong(Arg.Any<DateTime>()).Returns("1 January 2026");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<AspNetUsersHelper>().As<IAspNetUsersHelper>();
                b.RegisterInstance(this.permissions).As<IPermissions>();
                b.RegisterInstance(dateTime).As<IDateTimeService>();
                b.RegisterInstance(Substitute.For<IAvatars>()).As<IAvatars>();
                b.RegisterInstance(Substitute.For<IUserMedalService>()).As<IUserMedalService>();
                b.RegisterInstance(Substitute.For<IUrlHelper>()).As<IUrlHelper>();
                var ignored = Substitute.For<IUserIgnored>();
                ignored.UserIgnoredListAsync(Arg.Any<int>()).Returns([]);
                b.RegisterInstance(ignored).As<IUserIgnored>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.board.PageLoad.GuestUserID = 9999;

        this.alice = this.AddMember("alice", "Alice", "https://alice.example.com");
        this.bob = this.AddMember("bob", "Bob <b>", "https://x.example.com/\"><img src=x onerror=alert(1)>");

        this.board.LoginAs(this.alice);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The hover card shows the public profile, texts are encoded")]
    public async Task UserInfo_Test()
    {
        this.board.BoardSettings.EnableDisplayName = true;

        var info = await this.GetUserInfoAsync(this.alice);

        info.Name.Should().Be("Alice");
        info.HomePage.Should().Be("https://alice.example.com");
        info.Joined.Should().Contain("1 January 2026");

        (await this.GetUserInfoAsync(this.bob)).Name.Should().Be("Bob &lt;b&gt;");
    }

    [Test]
    [Description("A deleted (\"delete my account\") or not approved user has no hover card")]
    public async Task UserInfo_Deleted_User_Test()
    {
        using (var connection = this.db.Open())
        {
            connection.UpdateOnly(
                () => new YAF.Types.Models.User { Flags = (int)UserFlags.Flags.IsDeleted },
                u => u.ID == this.bob);
        }

        (await this.board.CreateController<UserInfo>().GetUserInfo(this.bob)).Result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("A home page url can not break out of the hover card link (stored XSS)")]
    public async Task UserInfo_HomePage_Encoded_Test()
    {
        var info = await this.GetUserInfoAsync(this.bob);

        info.HomePage.Should().NotContain("\"").And.NotContain("<");
    }

    [Test]
    [Description("A country, region or location sent with markup is encoded in the hover card (stored XSS)")]
    public async Task UserInfo_Location_Encoded_Test()
    {
        this.board.Localization.Add("COUNTRY", "DE", "Germany & Co");

        using (var connection = this.db.Open())
        {
            connection.UpdateOnly(
                () => new AspNetUsers { Profile_Country = "<img src=x onerror=alert(1)>", Profile_Region = "<b>" },
                u => u.Id == "bob-key");
            connection.UpdateOnly(
                () => new AspNetUsers { Profile_Country = "DE", Profile_Region = "<svg onload=alert(1)>" },
                u => u.Id == "alice-key");
        }

        (await this.GetUserInfoAsync(this.bob)).Location.Should().NotContain("<");

        var location = (await this.GetUserInfoAsync(this.alice)).Location;
        location.Should().StartWith("Germany &amp; Co, ").And.NotContain("<");

        using (var connection = this.db.Open())
        {
            connection.UpdateOnly(
                () => new AspNetUsers { Profile_Country = null, Profile_Region = null, Profile_Location = "<i>Berlin</i>" },
                u => u.Id == "alice-key");
        }

        (await this.GetUserInfoAsync(this.alice)).Location.Should().Be("&lt;i&gt;Berlin&lt;/i&gt;");
    }

    [Test]
    [Description("A url with quotes or angle brackets is not a valid home page")]
    public void IsValidUrl_Rejects_Markup_Test()
    {
        ValidationHelper.IsValidUrl("https://example.com/path?q=1").Should().BeTrue();
        ValidationHelper.IsValidUrl("https://x.example.com/\"><img src=x onerror=alert(1)>").Should().BeFalse();
        ValidationHelper.IsValidUrl("not a url").Should().BeFalse();
    }

    [Test]
    [Description("Unknown users and users without profile view permission get nothing")]
    public async Task UserInfo_Denied_Test()
    {
        (await this.board.CreateController<UserInfo>().GetUserInfo(99999)).Result.Should().BeOfType<NotFoundResult>();

        this.permissions.Check(Arg.Any<ViewPermissions>()).Returns(false);
        (await this.board.CreateController<UserInfo>().GetUserInfo(this.alice)).Result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("The admin user picker is only for admins and moderators")]
    public void GetUsers_Test()
    {
        var controller = this.board.CreateController<UserController>();

        controller.GetUsers(new SearchTopic { Page = 0, SearchTerm = "ali" }).Should().BeOfType<NotFoundResult>();

        this.board.PageLoad.IsAdmin = true;
        var result = (OkObjectResult)this.board.CreateController<UserController>().GetUsers(new SearchTopic { Page = 0, SearchTerm = "ali" });

        var options = (SelectPagedOptions)result.Value;
        options.Total.Should().Be(1);
        options.Results.Should().ContainSingle().Which.id.Should().Be(this.alice.ToString());
    }

    [Test]
    [Description("The mention list finds approved members of the board by name, not the guest")]
    public async Task GetMentionUsers_Test()
    {
        this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));
        this.db.AddUser("Alfred", u => u.Flags = 0);

        var result = (OkObjectResult)await this.board.CreateController<UserController>().GetMentionUsers("ali");

        Names(result).Should().Equal("alice");

        result = (OkObjectResult)await this.board.CreateController<UserController>().GetMentionUsers("Alf");
        Names(result).Should().BeEmpty("not approved users can not be mentioned");

        result = (OkObjectResult)await this.board.CreateController<UserController>().GetMentionUsers("Gue");
        Names(result).Should().BeEmpty("the guest user can not be mentioned");

        result = (OkObjectResult)await this.board.CreateController<UserController>().GetMentionUsers("al");
        Names(result).Should().BeEmpty("the editor asks after 3 characters, shorter searches would list (almost) all users");
    }

    [Test]
    [Description("The mention list returns at most 20 users")]
    public async Task GetMentionUsers_Limit_Test()
    {
        for (var i = 0; i < 25; i++)
        {
            this.db.AddUser($"Member{i}");
        }

        var result = (OkObjectResult)await this.board.CreateController<UserController>().GetMentionUsers("Mem");

        Names(result).Should().HaveCount(20);
    }

    private static List<string> Names(OkObjectResult result)
    {
        return [.. ((IEnumerable<object>)result.Value).Select(u => (string)u.GetType().GetProperty("name")!.GetValue(u))];
    }

    private async Task<ForumUserInfo> GetUserInfoAsync(int userId)
    {
        var result = await this.board.CreateController<UserInfo>().GetUserInfo(userId);

        return (ForumUserInfo)((OkObjectResult)result.Result).Value;
    }

    private int AddMember(string name, string displayName, string homePage)
    {
        using (var connection = this.db.Open())
        {
            connection.Insert(
                new AspNetUsers
                {
                    Id = $"{name}-key",
                    UserName = name,
                    Email = $"{name}@example.com",
                    ApplicationId = Guid.NewGuid(),
                    IsApproved = true,
                    Profile_Homepage = homePage
                });
        }

        return this.db.AddUser(
            name,
            u =>
            {
                u.ProviderUserKey = $"{name}-key";
                u.DisplayName = displayName;
            });
    }
}
