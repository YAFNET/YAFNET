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

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using NSubstitute;

using YAF.Core.Events;
using YAF.Core.Extensions;
using YAF.Core.Hubs;
using YAF.Core.Identity;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Flags;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// The real-time updates: a connection joins the groups of the forums the user (or guest) can read,
/// a published post is sent to the group of its forum.
/// </summary>
[TestFixture]
[NonParallelizable]
public class NotificationHubTests
{
    private TestBoardContext board;

    private IGroupManager groups;

    private INotificationClient forumClients;

    private int alice;

    private int guest;

    private int publicForum;

    private int privateForum;

    [SetUp]
    public void SetUp()
    {
        this.groups = Substitute.For<IGroupManager>();
        this.forumClients = Substitute.For<INotificationClient>();

        var hubContext = Substitute.For<IHubContext<NotificationHub, INotificationClient>>();
        hubContext.Clients.Group(Arg.Any<string>()).Returns(this.forumClients);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(new NotificationClient(hubContext)).AsSelf();
                b.Register(
                    c => new UserManager<AspNetUsers>(
                        new UserStore(c.Resolve<IServiceLocator>()),
                        Options.Create(new IdentityOptions()),
                        null,
                        [],
                        [],
                        new UpperInvariantLookupNormalizer(),
                        new IdentityErrorDescriber(),
                        null,
                        NullLogger<UserManager<AspNetUsers>>.Instance)).AsSelf();
            },
            useDatabase: true);

        var db = this.board.Database;

        this.alice = db.AddUser("Alice", u => u.ProviderUserKey = "alice-key");

        // the user name of the identity, not the name of the forum user
        this.board.Current.GetRepository<AspNetUsers>().Insert(
            new AspNetUsers { Id = "alice-key", UserName = "alice.identity", Email = "alice@example.com", IsApproved = true });
        this.guest = db.AddUser("Guest", u => u.Flags |= (int)UserFlags.Flags.IsGuest);

        this.publicForum = db.AddForum("Public");
        this.privateForum = db.AddForum("Private");

        this.board.GrantAccess(this.alice, this.publicForum);
        this.board.GrantAccess(this.alice, this.privateForum);
        this.board.GrantAccess(this.guest, this.publicForum);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A signed-in user joins the groups of all forums the user can read")]
    public async Task Subscribe_User_Test()
    {
        await this.Hub(SignedIn("alice-key")).SubscribeAsync();

        await this.groups.Received(1).AddToGroupAsync("connection", NotificationHub.ForumGroup(this.publicForum), Arg.Any<CancellationToken>());
        await this.groups.Received(1).AddToGroupAsync("connection", NotificationHub.ForumGroup(this.privateForum), Arg.Any<CancellationToken>());
    }

    [Test]
    [Description("A signed-in user without a user id claim is found by the user name of the identity")]
    public async Task Subscribe_User_By_Name_Test()
    {
        await this.Hub(new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "alice.identity")], "Test"))).SubscribeAsync();

        await this.groups.Received(1).AddToGroupAsync("connection", NotificationHub.ForumGroup(this.publicForum), Arg.Any<CancellationToken>());
        await this.groups.Received(1).AddToGroupAsync("connection", NotificationHub.ForumGroup(this.privateForum), Arg.Any<CancellationToken>());
    }

    [Test]
    [Description("A guest joins only the groups of the forums the guest user can read (no signal of posts in other forums)")]
    public async Task Subscribe_Guest_Test()
    {
        await this.Hub(new ClaimsPrincipal(new ClaimsIdentity())).SubscribeAsync();

        await this.groups.Received(1).AddToGroupAsync("connection", NotificationHub.ForumGroup(this.publicForum), Arg.Any<CancellationToken>());
        await this.groups.DidNotReceive().AddToGroupAsync("connection", NotificationHub.ForumGroup(this.privateForum), Arg.Any<CancellationToken>());
    }

    [Test]
    [Description("A signed-in user that is not a user of the board joins no group")]
    public async Task Subscribe_Unknown_User_Test()
    {
        await this.Hub(SignedIn("other-key")).SubscribeAsync();

        await this.groups.DidNotReceiveWithAnyArgs().AddToGroupAsync(default, default, default);
    }

    [Test]
    [Description("With the real-time updates switched off nobody joins a group")]
    public async Task Subscribe_Disabled_Test()
    {
        this.board.BoardSettings.EnableLiveUpdates = false;

        await this.Hub(SignedIn("alice-key")).SubscribeAsync();

        await this.groups.DidNotReceiveWithAnyArgs().AddToGroupAsync(default, default, default);
    }

    [Test]
    [Description("A published post is sent (ids only) to the group of its forum and clears the cached active discussions of the guests")]
    public void Published_Post_Is_Sent_To_Forum_Group_Test()
    {
        this.board.Cache.Set(Constants.Cache.ActiveDiscussions, "cached");

        this.Handler().Handle(new PostPublishedEvent(this.publicForum, 10, 20, true));

        this.forumClients.Received(1).PostPublishedAsync(
            Arg.Is<PostPublishedInfo>(i => i.ForumId == this.publicForum && i.TopicId == 10 && i.MessageId == 20 && i.IsNewTopic));

        this.board.Cache.Get(Constants.Cache.ActiveDiscussions).Should().BeNull();
    }

    [Test]
    [Description("With the real-time updates switched off a published post is not sent")]
    public void Published_Post_Disabled_Test()
    {
        this.board.BoardSettings.EnableLiveUpdates = false;

        this.Handler().Handle(new PostPublishedEvent(this.publicForum, 10, 20, false));

        this.forumClients.DidNotReceiveWithAnyArgs().PostPublishedAsync(default);
    }

    [Test]
    [Description("A failing send does not fail the post")]
    public void Published_Post_Send_Failure_Test()
    {
        this.forumClients.PostPublishedAsync(Arg.Any<PostPublishedInfo>())
            .Returns(Task.FromException(new InvalidOperationException("no connection")));

        var handle = () => this.Handler().Handle(new PostPublishedEvent(this.publicForum, 10, 20, false));

        handle.Should().NotThrow();
    }

    private static ClaimsPrincipal SignedIn(string providerUserKey)
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, providerUserKey)], "Test"));
    }

    private PostPublishedNotification Handler()
    {
        return new PostPublishedNotification(this.board.Current.ServiceLocator);
    }

    private NotificationHub Hub(ClaimsPrincipal user)
    {
        var context = Substitute.For<HubCallerContext>();
        context.ConnectionId.Returns("connection");
        context.User.Returns(user);

        return new NotificationHub(this.board.Current.ServiceLocator) { Context = context, Groups = this.groups };
    }
}
