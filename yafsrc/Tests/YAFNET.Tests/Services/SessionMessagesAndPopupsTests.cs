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

using System.Net;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Session;

using NSubstitute;

using YAF.Core.BaseModules;
using YAF.Core.BasePages;
using YAF.Core.Handlers;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;

/// <summary>
/// The messages shown on the next page of a user (<see cref="SessionMessageService"/>), the page
/// notifications of the <see cref="YAF.Core.Context.BoardContext"/> and the unread messages / pending friends popups.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SessionMessagesAndPopupsTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<LocalizationProvider>().AsSelf().SingleInstance();
                b.RegisterType<SessionService>().As<ISessionService>();
            });

        this.board.Current.PageLinks = [];
        this.board.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.1");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A message is shown once on the next page of the same user")]
    public void Message_For_Next_Page_Test()
    {
        this.board.Current.SessionNotify("Saved", MessageTypes.success);
        this.board.Current.SessionNotify("Second", MessageTypes.info);

        var nextPage = new SessionMessageService();
        nextPage.GetMessage().Message.Should().Be("Saved");
        nextPage.GetMessage().MessageType.Should().Be(MessageTypes.success);
        nextPage.LoadStringList.Select(m => m.Message).Should().Equal("Saved", "Second");

        new SessionMessageService().GetMessage().Should().BeNull("the messages are only shown once");

        nextPage.Clear();
        nextPage.GetMessage().Should().BeNull();
    }

    [Test]
    [Description("Messages are kept per user session, other users with the same IP address do not see them")]
    public void Message_Not_Shared_By_IP_Address_Test()
    {
        this.board.Current.SessionNotify("Your password was changed", MessageTypes.success);

        // another user behind the same address (office, mobile network, proxy)
        var aliceSession = this.board.HttpContext.Features.Get<ISessionFeature>().Session;
        this.board.HttpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = new FakeSession() });

        new SessionMessageService().GetMessage().Should().BeNull();

        this.board.HttpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = aliceSession });
        new SessionMessageService().GetMessage().Message.Should().Be("Your password was changed");
    }

    [Test]
    [Description("Without a session the messages are dropped instead of failing")]
    public void No_Session_Test()
    {
        this.board.HttpContext.Features.Set<ISessionFeature>(null);

        FluentActions.Invoking(() => this.board.Current.SessionNotify("lost", MessageTypes.info)).Should().NotThrow();
        new SessionMessageService().GetMessage().Should().BeNull();
    }

    [Test]
    [Description("Page notifications, script blocks and confirm dialogs are passed to the page in the temp data")]
    public void Page_Notifications_Test()
    {
        var page = this.NewPage(ForumPages.Index);

        this.board.Current.Notify("Hello", MessageTypes.warning);
        page.TempData["_alert.type"].Should().Be("warning");
        page.TempData["_alert.body"].Should().Be("Hello");

        this.board.Current.RegisterJsBlock("alert(1)");
        page.TempData["_jsBlock"].Should().Be("alert(1)");

        this.board.Current.ShowConfirmModal("Title", "Text", "Yes", "No", "/link");
        page.TempData["_confirm.title"].Should().Be("Title");
        page.TempData["_confirm.link"].Should().Be("/link");

        var redirect = new RedirectResult("/Index");
        this.board.LinkBuilder.Redirect(ForumPages.Index).Returns(redirect);

        this.board.Current.SessionNotify("Done", MessageTypes.success, ForumPages.Index).Should().BeSameAs(redirect);
        new SessionMessageService().GetMessage().Message.Should().Be("Done");
    }

    [Test]
    [Description("Unread private messages are announced once (not on the messages page), a newer message again")]
    public void Unread_Messages_Popup_Test()
    {
        this.board.User.UnreadPrivate = 2;
        this.board.User.LastUnreadPrivate = DateTime.UtcNow.AddMinutes(-5);
        this.board.Localization.Add("DEFAULT", "UNREAD_MSG2", "You have {0} unread message(s)");

        var messages = this.NewPage(ForumPages.MyMessages);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        messages.TempData.ContainsKey("_confirm.text").Should().BeFalse("not on the messages page");

        var page = this.NewPage(ForumPages.Index);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        page.TempData["_confirm.text"].Should().Be("You have 2 unread message(s)");

        var again = this.NewPage(ForumPages.Index);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        again.TempData.ContainsKey("_confirm.text").Should().BeFalse("the messages were announced already");

        this.board.User.UnreadPrivate = 3;
        this.board.User.LastUnreadPrivate = DateTime.UtcNow;

        var newMessage = this.NewPage(ForumPages.Index);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        newMessage.TempData["_confirm.text"].Should().Be("You have 3 unread message(s)");
    }

    [Test]
    [Description("The unread messages popup comes first, the friends popup on the next page")]
    public void Messages_Before_Friends_Popup_Test()
    {
        this.board.User.UnreadPrivate = 1;
        this.board.User.LastUnreadPrivate = DateTime.UtcNow;
        this.board.User.PendingBuddies = 1;
        this.board.User.LastPendingBuddies = DateTime.UtcNow;
        this.board.Localization.Add("DEFAULT", "UNREAD_MSG2", "messages").Add("DEFAULT", "PENDINGBUDDIES2", "friends");

        var first = this.NewPage(ForumPages.Index);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        first.TempData["_confirm.text"].Should().Be("messages");

        var second = this.NewPage(ForumPages.Index);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        second.TempData["_confirm.text"].Should().Be("friends");
    }

    [Test]
    [Description("New friend requests are announced once, not on the friends page")]
    public void Pending_Friends_Popup_Test()
    {
        this.board.User.PendingBuddies = 1;
        this.board.User.LastPendingBuddies = DateTime.UtcNow;

        var friends = this.NewPage(ForumPages.MyFriends);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        friends.TempData.ContainsKey("_confirm.title").Should().BeFalse();

        var page = this.NewPage(ForumPages.Index);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        page.TempData.ContainsKey("_confirm.title").Should().BeTrue();

        var again = this.NewPage(ForumPages.Index);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        again.TempData.ContainsKey("_confirm.title").Should().BeFalse("the request was announced already");

        this.board.User.PendingBuddies = 0;
        var none = this.NewPage(ForumPages.Index);
        new PmFriendNotification().Handle(new ForumPagePostLoadEvent());
        none.TempData.ContainsKey("_confirm.title").Should().BeFalse();
    }

    private TestPage NewPage(ForumPages pageName)
    {
        return new TestPage(pageName)
        {
            TempData = new TempDataDictionary(this.board.HttpContext, Substitute.For<ITempDataProvider>())
        };
    }

    private class TestPage(ForumPages page) : ForumPage("DEFAULT", page);
}
