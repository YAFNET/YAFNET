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
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// <see cref="LoadPageFromDatabase"/>: the init page load handler that resolves the request into
/// the page data (user, access, forum / topic) through the real <see cref="DataBroker"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class LoadPageFromDatabaseTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private IAspNetRolesHelper roles;

    private IGeoIpCountryService geoIp;

    private int guest;

    private int alice;

    private int forum;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        var aspNetUsers = Substitute.For<IAspNetUsersHelper>();
        this.roles = Substitute.For<IAspNetRolesHelper>();
        this.geoIp = Substitute.For<IGeoIpCountryService>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<DataBroker>().AsSelf();
                b.RegisterInstance(aspNetUsers).As<IAspNetUsersHelper>();
                b.RegisterInstance(this.roles).As<IAspNetRolesHelper>();
                b.RegisterInstance(this.geoIp).As<IGeoIpCountryService>();
            },
            useDatabase: true);

        this.db = this.board.Database;

        this.guest = this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));
        this.alice = this.db.AddUser("Alice", u => u.ProviderUserKey = "alice-key");

        aspNetUsers.GuestUser(TestDatabase.BoardId).Returns(_ => this.board.Current.GetRepository<User>().GetById(this.guest));

        var category = this.db.AddCategory();
        this.forum = this.db.AddForum("General", category);
        this.topic = this.db.AddTopic(this.forum, this.alice, "Hello");

        this.db.AddForumAccess(this.guest, this.forum, AccessFlags.Flags.ReadAccess);
        this.db.AddForumAccess(this.alice, this.forum, AccessFlags.Flags.ReadAccess | AccessFlags.Flags.PostAccess);

        this.board.HttpContext.Request.Path = "/";
        this.board.HttpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A guest request loads the guest page data and resolves the forum of the topic")]
    public void Guest_Test()
    {
        var @event = this.NewEvent(topicId: this.topic);

        this.Handler().Handle(@event);

        var (pageLoad, user, category, forum, topic, _) = @event.PageLoadData;
        user.ID.Should().Be(this.guest);
        pageLoad.ReadAccess.Should().BeTrue();
        pageLoad.PostAccess.Should().BeFalse();
        topic.ID.Should().Be(this.topic);
        forum.ID.Should().Be(this.forum);

        @event.PageQueryData.TopicID.Should().Be(this.topic);
        @event.PageQueryData.ForumID.Should().Be(this.forum);
        @event.PageQueryData.CategoryID.Should().Be(category.ID);
        user.DarkMode.Should().BeFalse();
    }

    [Test]
    [Description("A logged in request loads the user's page data")]
    public void Registered_Test()
    {
        this.LoginAlice();
        var @event = this.NewEvent(forumId: this.forum);

        this.Handler().Handle(@event);

        @event.PageLoadData.Item2.ID.Should().Be(this.alice);
        @event.PageLoadData.Item1.PostAccess.Should().BeTrue();
        this.roles.DidNotReceiveWithAnyArgs().SetupUserRolesAsync(default, default);
    }

    [Test]
    [Description("A logged in user without any permissions gets the guest page data")]
    public void Registered_Without_Access_Falls_Back_To_Guest_Test()
    {
        var bob = this.db.AddUser("Bob", u => u.ProviderUserKey = "bob-key");
        this.board.LoginAs(bob);

        var @event = this.NewEvent();

        this.Handler().Handle(@event);

        @event.PageLoadData.Item2.ID.Should().Be(this.guest);
        @event.PageLoadData.Item1.UserID.Should().Be(this.guest);

        // the forum user roles are repaired before falling back
        this.roles.ReceivedWithAnyArgs().SetupUserRolesAsync(default, default);
    }

    [Test]
    [Description("Without a guest user the page load fails")]
    public void No_Guest_Access_Test()
    {
        this.board.Current.GetRepository<UserGroup>().Delete(g => g.UserID == this.guest);

        var handler = this.Handler();

        handler.Invoking(h => h.Handle(this.NewEvent())).Should().Throw<ArgumentException>()
            .WithMessage("Unable to find the Guest User!");
    }

    [Test]
    [Description("The theme cookie switches the dark mode")]
    [TestCase("dark", true)]
    [TestCase("auto", true)]
    [TestCase("light", false)]
    [TestCase("", false)]
    public void Dark_Mode_Cookie_Test(string cookie, bool darkMode)
    {
        this.board.HttpContext.Request.Headers.Cookie = $"YAF-Theme={cookie}";
        var @event = this.NewEvent();

        this.Handler().Handle(@event);

        @event.PageLoadData.Item2.DarkMode.Should().Be(darkMode);
    }

    [Test]
    [Description("The page name comes from the matching endpoint, the country from the GeoIP database")]
    public void Page_Name_And_Country_Test()
    {
        this.geoIp.DatabaseExists().Returns(true);
        this.geoIp.GetCountry(Arg.Any<string>()).Returns(new GeoIpData { CountryCode = "DE" });
        this.board.HttpContext.Request.Path = "/Members";

        var endpoints = new DefaultEndpointDataSource(
            new Endpoint(_ => System.Threading.Tasks.Task.CompletedTask, EndpointMetadataCollection.Empty, "Page: /Members"));

        this.Handler([endpoints]).Handle(this.NewEvent());

        var active = this.board.Current.GetRepository<Active>().GetSingle(a => a.SessionID == this.board.HttpContext.Session.Id);
        active.ForumPage.Should().Be("Members");
        active.Country.Should().Be("DE");
        active.Path.Should().Be("/Members");
    }

    [Test]
    [Description("Paths that are not a forum page record an empty page name")]
    public void Unknown_Page_Name_Test()
    {
        this.board.HttpContext.Request.Path = "/not/a/page";

        this.Handler().Handle(this.NewEvent());

        this.board.Current.GetRepository<Active>().GetSingle(a => a.SessionID == this.board.HttpContext.Session.Id)
            .ForumPage.Should().BeNullOrEmpty();
    }

    private LoadPageFromDatabase Handler(IEnumerable<EndpointDataSource> endpoints = null)
    {
        return new LoadPageFromDatabase(
            this.board.Current.ServiceLocator,
            NullLogger<LoadPageFromDatabase>.Instance,
            this.board.Cache,
            endpoints ?? []);
    }

    private InitPageLoadEvent NewEvent(int? forumId = null, int? topicId = null)
    {
        var @event = new InitPageLoadEvent();
        @event.UserRequestData.Browser = "Firefox";
        @event.UserRequestData.Platform = "Windows";
        @event.UserRequestData.UserAgent = "UA";
        @event.PageQueryData.ForumID = forumId ?? 0;
        @event.PageQueryData.TopicID = topicId ?? 0;

        return @event;
    }

    private void LoginAlice()
    {
        this.board.LoginAs(this.alice);
        this.board.Current.MembershipUser = new AspNetUsers { Id = "alice-key", UserName = "Alice" };
    }
}
