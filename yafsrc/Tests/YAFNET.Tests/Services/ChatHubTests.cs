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

using System.Threading;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.SignalR;

using NSubstitute;

using YAF.Core.Extensions;
using YAF.Core.Hubs;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The private message chat (SignalR hub): who can receive messages, which avatar is sent and when messages are read.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ChatHubTests
{
    private const string EvilAvatar = "x\" onerror=\"alert(1)";

    private readonly List<string> connections = [];

    private TestBoardContext board;

    private Dictionary<string, ISingleClientProxy> clients;

    private int alice;

    private int bob;

    private int carol;

    [SetUp]
    public void SetUp()
    {
        var avatars = Substitute.For<IAvatars>();
        avatars.GetAvatarUrlForUser(Arg.Any<User>()).Returns(c => $"/avatar/{c.Arg<User>().ID}?a=1&b=2");

        var format = Substitute.For<IFormatMessage>();
        format.FormatMessageWithAllBBCodesAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<DateTime?>())
            .Returns(c => Task.FromResult($"<p>{c.ArgAt<string>(0)}</p>"));

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(avatars).As<IAvatars>();
                b.RegisterInstance(format).As<IFormatMessage>();
                b.RegisterInstance(Substitute.For<IDateTimeService>()).As<IDateTimeService>();
            },
            useDatabase: true);

        this.clients = [];

        var db = this.board.Database;
        this.alice = db.AddUser("Alice");
        this.bob = db.AddUser("Bob");
        this.carol = db.AddUser("Carol");
        this.board.PageLoad.GuestUserID = 9999;
    }

    [TearDown]
    public async Task TearDown()
    {
        // the connected users are kept in a static list
        foreach (var connection in this.connections)
        {
            await this.Hub(connection).OnDisconnectedAsync(null);
        }

        this.connections.Clear();
        this.board?.Dispose();
    }

    [Test]
    [Description("A message to an online user with the conversation open is delivered, read and has the encoded avatar of the sender")]
    public async Task Message_To_Open_Conversation_Test()
    {
        await this.ConnectAsync("bob-tab", this.bob, this.alice);
        await this.ConnectAsync("alice-tab", this.alice, this.bob, EvilAvatar);

        await this.Hub("alice-tab").SendPrivateMessageAsync(this.bob, "Hello Bob");

        var message = await this.Messages.GetSingleAsync(m => m.FromUserId == this.alice);
        message.Body.Should().Be("Hello Bob");
        message.PrivateMessageFlags.IsRead.Should().BeTrue();

        var avatar = $"/avatar/{this.alice}?a=1&amp;b=2";

        await this.clients["bob-tab"].Received(1).SendCoreAsync(
            "sendPrivateMessage",
            Arg.Is<object[]>(a => (int)a[0] == this.alice && (string)a[1] == "<p>Hello Bob</p>" && (string)a[2] == avatar),
            Arg.Any<CancellationToken>());
    }

    [Test]
    [Description("A message to a user who has another conversation open (or is offline) stays unread")]
    public async Task Message_To_Other_Conversation_Test()
    {
        await this.ConnectAsync("bob-tab", this.bob, this.carol);
        await this.ConnectAsync("alice-tab", this.alice, this.bob);

        await this.Hub("alice-tab").SendPrivateMessageAsync(this.bob, "Hello Bob");

        (await this.Messages.GetSingleAsync(m => m.FromUserId == this.alice)).PrivateMessageFlags.IsRead.Should().BeFalse();
        await this.clients["bob-tab"].DidNotReceive().SendCoreAsync("sendPrivateMessage", Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Test]
    [Description("No message to users who block private messages or ignore the sender, to yourself, unknown users and the guest")]
    public async Task Blocked_Recipients_Test()
    {
        await this.ConnectAsync("alice-tab", this.alice, this.bob);
        var hub = this.Hub("alice-tab");

        var users = this.board.Current.GetRepository<User>();
        await users.UpdateOnlyAsync(() => new User { BlockFlags = (int)UserBlockFlags.Flags.BlockPMs }, u => u.ID == this.bob);
        await hub.SendPrivateMessageAsync(this.bob, "blocked");

        await this.board.Current.GetRepository<IgnoreUser>().InsertAsync(new IgnoreUser { UserID = this.carol, IgnoredUserID = this.alice });
        await hub.SendPrivateMessageAsync(this.carol, "ignored");

        await hub.SendPrivateMessageAsync(this.alice, "myself");
        await hub.SendPrivateMessageAsync(99999, "nobody");

        var guest = this.board.Database.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));
        await hub.SendPrivateMessageAsync(guest, "guest");

        var otherBoard = this.board.Database.AddUser("Other", u => u.BoardID = 2);
        await hub.SendPrivateMessageAsync(otherBoard, "other board");

        (await this.Messages.CountAsync(_ => true)).Should().Be(0);
    }

    [Test]
    [Description("Admins and users in the friend list of the recipient are excluded from the blocking, a friend request is not enough")]
    public async Task Blocking_Exceptions_Test()
    {
        await this.board.Current.GetRepository<User>().UpdateOnlyAsync(
            () => new User { BlockFlags = (int)UserBlockFlags.Flags.BlockPMs },
            u => u.ID == this.bob);

        await this.ConnectAsync("alice-tab", this.alice, this.bob);

        // only Alice asked to be friends
        await this.board.Current.GetRepository<Buddy>().InsertAsync(
            new Buddy { FromUserID = this.alice, ToUserID = this.bob, Approved = false, Requested = DateTime.UtcNow });
        await this.Hub("alice-tab").SendPrivateMessageAsync(this.bob, "request only");
        (await this.Messages.CountAsync(_ => true)).Should().Be(0);

        // Bob has Alice in the friend list
        await this.board.Current.GetRepository<Buddy>().InsertAsync(
            new Buddy { FromUserID = this.bob, ToUserID = this.alice, Approved = true, Requested = DateTime.UtcNow });
        await this.Hub("alice-tab").SendPrivateMessageAsync(this.bob, "friend");
        (await this.Messages.GetSingleAsync(m => m.FromUserId == this.alice)).Body.Should().Be("friend");

        // admins
        await this.ConnectAsync("carol-tab", this.carol, this.bob);
        this.board.PageLoad.IsAdmin = true;
        await this.Hub("carol-tab").SendPrivateMessageAsync(this.bob, "admin");
        (await this.Messages.GetSingleAsync(m => m.FromUserId == this.carol)).Body.Should().Be("admin");
    }

    [Test]
    [Description("With private messages turned off nothing is sent, also messages without a connection are ignored")]
    public async Task Private_Messages_Disabled_Test()
    {
        await this.ConnectAsync("alice-tab", this.alice, this.bob);

        // not connected
        await this.Hub("unknown-tab").SendPrivateMessageAsync(this.bob, "who am I");

        this.board.BoardSettings.AllowPrivateMessages = false;
        await this.Hub("alice-tab").SendPrivateMessageAsync(this.bob, "disabled");

        (await this.Messages.CountAsync(_ => true)).Should().Be(0);
    }

    [Test]
    [Description("A suspended user can't send messages until the suspension has ended")]
    public async Task Suspended_User_Test()
    {
        await this.ConnectAsync("alice-tab", this.alice, this.bob);

        this.board.User.Suspended = DateTime.UtcNow.AddDays(1);
        await this.Hub("alice-tab").SendPrivateMessageAsync(this.bob, "while suspended");

        (await this.Messages.CountAsync(_ => true)).Should().Be(0);

        this.board.User.Suspended = DateTime.UtcNow.AddMinutes(-1);
        await this.Hub("alice-tab").SendPrivateMessageAsync(this.bob, "suspension ended");

        (await this.Messages.CountAsync(_ => true)).Should().Be(1);
    }

    [Test]
    [Description("Connecting loads the conversation, a deleted conversation is only gone for the user who deleted it")]
    public async Task Conversation_Test()
    {
        await this.ConnectAsync("alice-tab", this.alice, this.bob);
        await this.Hub("alice-tab").SendPrivateMessageAsync(this.bob, "first");

        // Bob opens the conversation
        await this.ConnectAsync("bob-tab", this.bob, this.alice);

        await this.clients["bob-tab"].Received(1).SendCoreAsync(
            "onConnected",
            Arg.Is<object[]>(a => (int)a[0] == this.alice && ((List<PrivateMessage>)a[1]).Single().Body == "<p>first</p>"),
            Arg.Any<CancellationToken>());

        (await this.Messages.GetSingleAsync(m => m.FromUserId == this.alice)).PrivateMessageFlags.IsRead.Should().BeTrue("Bob has read it");

        await this.Hub("bob-tab").DeleteConversationAsync(this.alice);

        (await this.Messages.GetSingleAsync(m => m.FromUserId == this.alice)).Should().NotBeNull("Alice still has the message");
    }

    private IRepository<PrivateMessage> Messages => this.board.Current.GetRepository<PrivateMessage>();

    private async Task ConnectAsync(string connectionId, int userId, int toUserId, string avatar = "/my-avatar.png")
    {
        this.board.LoginAs(userId);
        this.connections.Add(connectionId);

        await this.Hub(connectionId).ConnectAsync(avatar, toUserId);
    }

    private ChatHub Hub(string connectionId)
    {
        if (!this.clients.TryGetValue(connectionId, out var proxy))
        {
            proxy = Substitute.For<ISingleClientProxy>();
            this.clients[connectionId] = proxy;
        }

        var context = Substitute.For<HubCallerContext>();
        context.ConnectionId.Returns(connectionId);

        var hubClients = Substitute.For<IHubCallerClients>();
        hubClients.Client(Arg.Any<string>()).Returns(c => this.clients.TryGetValue(c.Arg<string>(), out var p) ? p : Substitute.For<ISingleClientProxy>());
        hubClients.Caller.Returns(proxy);

        return new ChatHub(this.board.Current.ServiceLocator) { Context = context, Clients = hubClients };
    }
}
