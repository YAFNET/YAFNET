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
using YAF.Core.Controllers.Modals;
using YAF.Core.Events;
using YAF.Core.Events.Cache;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Services;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// Notification list, friend requests and IP bans.
/// </summary>
[TestFixture]
[NonParallelizable]
public class NotifyFriendsBanControllerTests
{
    private const string EvilName = "<img src=x onerror=alert(1)>";

    private TestBoardContext board;

    private TestDatabase db;

    private IUserDisplayName displayName;

    private int alice;

    private int mallory;

    private int topic;

    private int message;

    [SetUp]
    public void SetUp()
    {
        this.displayName = Substitute.For<IUserDisplayName>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(this.displayName).As<IUserDisplayName>();
                b.RegisterType<Friends>().As<IFriends>();
                b.RegisterInstance(Substitute.For<ISessionService>()).As<ISessionService>();
                b.RegisterType<ServiceLocatorEventRaiser>().As<IRaiseEvent>();
                b.RegisterType<ClearCacheOnEvents>().AsImplementedInterfaces();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.mallory = this.db.AddUser("Mallory", u => u.DisplayName = EvilName);

        this.displayName.GetNameById(this.mallory).Returns(EvilName);
        this.displayName.GetNameById(this.alice).Returns("Alice");

        var forum = this.db.AddForum();
        this.db.AddForumAccess(this.alice, forum);
        this.topic = this.db.AddTopic(forum, this.alice, "Thread");
        this.message = this.db.AddMessage(this.topic, this.alice);

        this.board.Localization.Add("DEFAULT", "RECEIVED_THANKS_MSG", "{0} thanked you for {1}");
        this.board.Localization.Add("DEFAULT", "WAS_MENTIONED_MSG", "{0} mentioned you in {1}");

        this.board.LinkBuilder.GetLink(Arg.Any<ForumPages>(), Arg.Any<object>()).Returns("/post/1");

        this.board.LoginAs(this.alice);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Notifications list who did what in which topic, with the topic icon")]
    public async Task Notifications_Test()
    {
        this.AddActivity(this.mallory, ActivityFlags.Flags.ReceivedThanks);
        this.AddActivity(this.mallory, ActivityFlags.Flags.WasMentioned, notification: false);

        var data = await this.GetNotificationsAsync();

        data.TotalRecords.Should().Be(1, "only unread notifications are listed");
        var html = data.AttachmentList.Should().ContainSingle().Subject.FileName;

        html.Should().Contain("thanked you for");
        html.Should().Contain("fa-heart");
        html.Should().Contain("<i class=\"fas fa-comment me-1\"></i>Thread", "the topic icon is markup, not text");
        html.Should().StartWith("<a href=").And.Contain("class=\"dropdown-item", "the whole item links to the post in the notify dropdown");
        html.IndexOf("<a ", 1, StringComparison.Ordinal).Should().Be(-1, "no nested links inside the dropdown item");
    }

    [Test]
    [Description("A display name with markup is encoded in the notification list (stored XSS)")]
    public async Task Notifications_Encode_Display_Name_Test()
    {
        this.AddActivity(this.mallory, ActivityFlags.Flags.WasMentioned);

        var html = (await this.GetNotificationsAsync()).AttachmentList.Single().FileName;

        html.Should().NotContain("<img");
        html.Should().Contain("&lt;img");
    }

    [Test]
    [Description("A topic name with markup is encoded in the notification list")]
    public async Task Notifications_Encode_Topic_Name_Test()
    {
        var evilForum = this.db.AddForum();
        this.db.AddForumAccess(this.alice, evilForum);
        var evilTopic = this.db.AddTopic(evilForum, this.mallory, "<script>alert(1)</script>");
        this.AddActivity(this.mallory, ActivityFlags.Flags.WasMentioned, topicId: evilTopic);

        var html = (await this.GetNotificationsAsync()).AttachmentList.Single().FileName;

        html.Should().NotContain("<script>");
    }

    [Test]
    [Description("A notification of a deleted topic does not break the list")]
    public async Task Notifications_Deleted_Topic_Test()
    {
        this.AddActivity(this.mallory, ActivityFlags.Flags.ReceivedThanks, topicId: 99999);
        this.AddActivity(this.mallory, ActivityFlags.Flags.ReceivedThanks);

        var data = await this.GetNotificationsAsync();

        data.AttachmentList.Should().ContainSingle();
    }

    [Test]
    [Description("A notification of a topic the user can't read anymore (moved to a hidden forum) is not listed")]
    public async Task Notifications_Hidden_Forum_Test()
    {
        var hiddenTopic = this.db.AddTopic(this.db.AddForum("Hidden"), this.mallory, "Staff only");
        this.AddActivity(this.mallory, ActivityFlags.Flags.WasMentioned, topicId: hiddenTopic);
        this.AddActivity(this.mallory, ActivityFlags.Flags.ReceivedThanks);

        var html = (await this.GetNotificationsAsync()).AttachmentList.Should().ContainSingle().Subject.FileName;

        html.Should().NotContain("Staff only");
    }

    [Test]
    [Description("A friend request is sent to another user")]
    public void AddFriendRequest_Test()
    {
        this.board.CreateController<FriendsController>().AddFriendRequest(this.mallory);

        this.Buddies().Should().ContainSingle().Which.ToUserID.Should().Be(this.mallory);
    }

    [Test]
    [Description("Users can not send a friend request to themselves")]
    public void AddFriendRequest_Self_Test()
    {
        this.board.CreateController<FriendsController>().AddFriendRequest(this.alice);

        this.Buddies().Should().BeEmpty();
    }

    [Test]
    [Description("Friend requests follow the rules of the button: not to users who block them, of another board or the guest, and not when friends are disabled")]
    public void AddFriendRequest_Refused_Test()
    {
        var blocking = this.db.AddUser("Blocking", u => u.BlockFlags = new UserBlockFlags { BlockFriendRequests = true }.BitValue);
        var otherBoard = this.db.AddUser("OtherBoard", u => u.BoardID = TestDatabase.BoardId + 1);
        var guest = this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));

        var controller = this.board.CreateController<FriendsController>();

        controller.AddFriendRequest(blocking);
        controller.AddFriendRequest(otherBoard);
        controller.AddFriendRequest(guest);

        this.Buddies().Should().BeEmpty();

        this.board.BoardSettings.EnableBuddyList = false;
        controller.AddFriendRequest(this.mallory);

        this.Buddies().Should().BeEmpty("friends are disabled");
    }

    [Test]
    [Description("A friend request from a post follows the rules of the button too")]
    public void AddFriend_From_Post_Refused_Test()
    {
        var blocking = this.db.AddUser("Blocking", u => u.BlockFlags = new UserBlockFlags { BlockFriendRequests = true }.BitValue);
        this.ShowPost(blocking, "Blocking");

        this.board.CreateController<FriendsController>().AddFriend(this.message);

        this.Buddies().Should().BeEmpty();

        this.ShowPost(this.mallory, "Mallory");
        this.board.CreateController<FriendsController>().AddFriend(this.message);

        this.Buddies().Should().ContainSingle().Which.ToUserID.Should().Be(this.mallory);
    }

    [Test]
    [Description("A friend request to an unknown user is ignored")]
    public void AddFriendRequest_Unknown_Test()
    {
        this.board.CreateController<FriendsController>().AddFriendRequest(99999);

        this.Buddies().Should().BeEmpty();
    }

    [Test]
    [Description("Removing a friend removes the friendship")]
    public void RemoveBuddy_Test()
    {
        this.db.Open().Insert(new Buddy { FromUserID = this.alice, ToUserID = this.mallory, Approved = true, Requested = DateTime.UtcNow });

        this.board.CreateController<FriendsController>().RemoveBuddy(this.mallory);

        this.Buddies().Should().BeEmpty();
    }

    [Test]
    [Description("Becoming friends from a post or a profile shows the encoded name of the friend (the message is shown as html)")]
    public void Mutual_Friends_Notification_Encoded_Test()
    {
        this.board.BoardSettings.EnableDisplayName = true;
        this.board.Localization.Add("COMMON", "NOTIFICATION_BUDDYAPPROVED_MUTUAL", "You and {0} are friends");
        this.board.Localization.Add("COMMON", "NOTIFICATION_BUDDYREQUEST", "request sent");

        // Mallory asked first, adding Mallory makes both friends
        this.db.Open().Insert(new Buddy { FromUserID = this.mallory, ToUserID = this.alice, Approved = false, Requested = DateTime.UtcNow });
        this.ShowPost(this.mallory, EvilName);

        this.board.CreateController<FriendsController>().AddFriend(this.message);

        this.LastNotification().Should().Be("You and &lt;img src=x onerror=alert(1)&gt; are friends");
        this.Buddies().Should().OnlyContain(b => b.Approved);

        this.db.Open().Delete<Buddy>(_ => true);
        this.db.Open().Insert(new Buddy { FromUserID = this.mallory, ToUserID = this.alice, Approved = false, Requested = DateTime.UtcNow });

        this.board.CreateController<FriendsController>().AddFriendRequest(this.mallory);

        this.LastNotification().Should().Be("You and &lt;img src=x onerror=alert(1)&gt; are friends");
    }

    [Test]
    [Description("Removing a friend from a post or a profile shows the encoded name")]
    public void Remove_Friend_Notification_Encoded_Test()
    {
        this.board.Localization.Add("COMMON", "REMOVEBUDDY_NOTIFICATION", "{0} removed");
        this.ShowPost(this.mallory, EvilName);

        this.db.Open().Insert(new Buddy { FromUserID = this.alice, ToUserID = this.mallory, Approved = true, Requested = DateTime.UtcNow });
        this.board.CreateController<FriendsController>().RemoveFriend(this.message);

        this.Buddies().Should().BeEmpty();
        this.LastNotification().Should().Be("&lt;img src=x onerror=alert(1)&gt; removed");

        this.db.Open().Insert(new Buddy { FromUserID = this.alice, ToUserID = this.mallory, Approved = true, Requested = DateTime.UtcNow });
        this.board.CreateController<FriendsController>().RemoveBuddy(this.mallory);

        this.LastNotification().Should().Be("&lt;img src=x onerror=alert(1)&gt; removed");
    }

    [Test]
    [Description("Friend actions for a post that is not shown on the page do nothing")]
    public void Friend_Unknown_Post_Test()
    {
        this.ShowPost(this.mallory, EvilName);

        this.board.CreateController<FriendsController>().AddFriend(99999);
        this.board.CreateController<FriendsController>().RemoveFriend(99999);
        this.board.CreateController<FriendsController>().RemoveBuddy(99999);

        this.Buddies().Should().BeEmpty();
    }

    [Test]
    [Description("A new IP ban is saved, a duplicate is rejected")]
    public void BannedIp_Add_Test()
    {
        var controller = this.board.CreateController<BannedIpController>();

        controller.Edit(new BannedEmailEditModal { Mask = " 203.0.113.* ", BanReason = "spam" }).Should().BeOfType<OkResult>();

        this.Bans().Should().ContainSingle().Which.Mask.Should().Be("203.0.113.*");

        controller.Edit(new BannedEmailEditModal { Mask = "203.0.113.*" })
            .Should().BeOfType<OkObjectResult>("a duplicate returns a warning");
        this.Bans().Should().ContainSingle();
    }

    [Test]
    [Description("Invalid IP masks are not saved")]
    [TestCase("1.2.3")]
    [TestCase("1.2.3.4.5")]
    [TestCase("1.2.3.256")]
    [TestCase("1.2.x.4")]
    [TestCase("1..3.4")]
    public void BannedIp_Invalid_Test(string mask)
    {
        this.board.CreateController<BannedIpController>().Edit(new BannedEmailEditModal { Mask = mask });

        this.Bans().Should().BeEmpty();
    }

    [Test]
    [Description("Adding and editing an IP ban drops the cached ban list, so the change is enforced at once")]
    public void BannedIp_Edit_Clears_Cache_Test()
    {
        var controller = this.board.CreateController<BannedIpController>();
        controller.Edit(new BannedEmailEditModal { Mask = "203.0.113.10" });
        var id = this.Bans().Single().ID;

        this.board.Cache.Set(Constants.Cache.BannedIP, new List<string> { "203.0.113.10" });

        controller.Edit(new BannedEmailEditModal { Id = id, Mask = "198.51.100.7" });

        this.Bans().Single().Mask.Should().Be("198.51.100.7");
        this.board.Cache.Get(Constants.Cache.BannedIP).Should().BeNull("the old cached list must not stay active");
    }

    private async Task<GridDataSet> GetNotificationsAsync()
    {
        var result = await this.board.CreateController<NotifyController>()
            .GetNotifications(new PagedResults { PageNumber = 0, PageSize = 10 });

        return (GridDataSet)((OkObjectResult)result.Result).Value;
    }

    private void AddActivity(int fromUserId, ActivityFlags.Flags flags, bool notification = true, int? topicId = null)
    {
        using var connection = this.db.Open();

        connection.Insert(
            new Activity
            {
                UserID = this.alice,
                FromUserID = fromUserId,
                TopicID = topicId ?? this.topic,
                MessageID = this.message,
                Flags = (int)flags,
                Notification = notification,
                Created = DateTime.UtcNow
            });
    }

    private void ShowPost(int userId, string displayName)
    {
        this.board.Resolve<ISessionService>().GetPageData<List<PagedMessage>>().Returns(
        [
            new PagedMessage
            {
                MessageID = this.message, UserID = userId, DisplayName = displayName, UserName = "mallory", Topic = "Thread"
            }
        ]);
    }

    private string LastNotification()
    {
        // the messages are shown on the next page
        return new SessionMessageService().LoadStringList.LastOrDefault()?.Message;
    }

    private List<Buddy> Buddies()
    {
        return this.board.Current.GetRepository<Buddy>().Get(_ => true);
    }

    private List<BannedIP> Bans()
    {
        return this.board.Current.GetRepository<BannedIP>().Get(_ => true);
    }
}
