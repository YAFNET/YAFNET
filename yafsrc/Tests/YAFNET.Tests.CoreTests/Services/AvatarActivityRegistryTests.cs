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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Events;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// Avatar urls, the activity stream / notifications, the last visit of guests and the registry helpers.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AvatarActivityRegistryTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        var urlHelper = Substitute.For<IUrlHelper>();
        urlHelper.Action(Arg.Any<UrlActionContext>())
            .Returns(c => $"/api/Avatar/{c.Arg<UrlActionContext>().Action}");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<Avatars>().As<IAvatars>();
                b.RegisterType<SessionService>().As<ISessionService>();
                b.RegisterInstance(urlHelper).As<IUrlHelper>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Uploaded avatars, local avatar paths and the text avatar fallback")]
    public void Avatar_Url_Test()
    {
        var avatars = this.board.Resolve<IAvatars>();
        this.board.BoardSettings.AvatarUpload = true;

        avatars.GetAvatarUrlForUser(1, null, true).Should().Be("/api/Avatar/GetResponseLocalAvatar");
        avatars.GetAvatarUrlForUser(1, "/images/avatars/cat.png", false).Should().Be("/images/avatars/cat.png");
        avatars.GetAvatarUrlForUser(1, null, false).Should().Be("/api/Avatar/GetTextAvatar");

        this.board.BoardSettings.AvatarUpload = false;
        avatars.GetAvatarUrlForUser(1, "/images/avatars/cat.png", true).Should().Be("/images/avatars/cat.png", "uploads are disabled");

        avatars.Invoking(a => a.GetAvatarUrlForUser(null)).Should().Throw<ArgumentNullException>();
        avatars.GetAvatarUrlForCurrentUser().Should().Be("/api/Avatar/GetTextAvatar");
    }

    [Test]
    [Description("An avatar path to another site (//host) is not used")]
    public void Avatar_Other_Site_Test()
    {
        this.board.Resolve<IAvatars>().GetAvatarUrlForUser(1, "//tracker.example/pixel.png", false)
            .Should().Be("/api/Avatar/GetTextAvatar");
    }

    [Test]
    [Description("Notifications list the activities of other users, the timeline the user's own")]
    public void Notifications_And_Timeline_Test()
    {
        var forum = this.db.AddForum();
        this.db.AddForumAccess(this.alice, forum);

        var topic = this.db.AddTopic(forum, this.alice, "Thread");
        var message = this.db.AddMessage(topic, this.alice);

        this.AddActivity(this.alice, this.bob, ActivityFlags.Flags.WasMentioned, topic, message);
        this.AddActivity(this.alice, this.bob, ActivityFlags.Flags.WatchTopicReply, topic, message);
        this.AddActivity(this.alice, null, ActivityFlags.Flags.CreatedTopic, topic, message);

        var notifications = this.Activities.Notifications(this.alice);
        notifications.Should().HaveCount(2).And.OnlyContain(n => n.Item2.ID == this.bob && n.Item3.TopicName == "Thread");

        this.Activities.Timeline(this.alice).Select(t => t.Item1.ActivityFlags.CreatedTopic).Should().Contain(true);
        this.Activities.Timeline(this.alice).Should().NotContain(t => t.Item1.ActivityFlags.WatchTopicReply);

        this.Activities.GetUnreadWatchTopics(this.alice).Should().Be(1);
    }

    [Test]
    [Description("The notifications only show topics the user can still read (moved to a hidden forum, deleted)")]
    public void Notifications_Only_Readable_Topics_Test()
    {
        var readable = this.db.AddForum("Readable");
        var hidden = this.db.AddForum("Hidden");
        this.db.AddForumAccess(this.alice, readable);

        var visibleTopic = this.db.AddTopic(readable, this.alice, "Visible");
        var hiddenTopic = this.db.AddTopic(hidden, this.alice, "Moved to staff");
        var deletedTopic = this.db.AddTopic(
            readable,
            this.alice,
            "Deleted",
            t => t.Flags = new TopicFlags { IsDeleted = true }.BitValue);

        this.AddActivity(this.alice, this.bob, ActivityFlags.Flags.WasMentioned, visibleTopic, this.db.AddMessage(visibleTopic, this.bob));
        this.AddActivity(this.alice, this.bob, ActivityFlags.Flags.WasMentioned, hiddenTopic, this.db.AddMessage(hiddenTopic, this.bob));
        this.AddActivity(this.alice, this.bob, ActivityFlags.Flags.WasMentioned, deletedTopic, this.db.AddMessage(deletedTopic, this.bob));

        this.Activities.Notifications(this.alice).Select(n => n.Item3.TopicName).Should().Equal("Visible");
    }

    [Test]
    [Description("Reading a message or topic marks its notifications as read, MarkAllAsRead all of them")]
    public void Mark_As_Read_Test()
    {
        var topic = this.db.AddTopic(this.db.AddForum(), this.alice);
        var first = this.db.AddMessage(topic, this.alice);
        var second = this.db.AddMessage(topic, this.alice);
        var otherTopic = this.db.AddTopic(this.db.AddForum(), this.alice);
        var third = this.db.AddMessage(otherTopic, this.alice);

        this.AddActivity(this.alice, this.bob, ActivityFlags.Flags.WasMentioned, topic, first);
        this.AddActivity(this.alice, this.bob, ActivityFlags.Flags.WasQuoted, topic, second);
        this.AddActivity(this.alice, this.bob, ActivityFlags.Flags.WasQuoted, otherTopic, third);

        this.Activities.UpdateNotification(this.alice, first);
        this.Unread().Should().Be(2);
        this.board.Events.OfType<UpdateUserEvent>().Should().ContainSingle();

        this.Activities.UpdateTopicNotification(this.alice, topic);
        this.Unread().Should().Be(1);

        this.Activities.MarkAllAsRead(this.alice);
        this.Unread().Should().Be(0);
    }

    [Test]
    [Description("Members get their previous visit, guests the one of their cookie (a broken cookie counts as never)")]
    public void Last_Visit_Test()
    {
        var session = this.board.Resolve<ISessionService>();
        var handler = new LastVisitEventHandler(session, this.board.Resolve<Microsoft.AspNetCore.Http.IHttpContextAccessor>());

        var previous = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        this.board.PageLoad.PreviousVisit = previous;
        handler.Handle(new ForumPagePreLoadEvent());
        session.LastVisit.Should().Be(previous);

        session.LastVisit = null;
        this.board.User.IsGuest = true;
        this.board.HttpContext.Request.Headers.Cookie = $"PreviousVisit={Uri.EscapeDataString("01/02/2026 03:04:05")}";
        handler.Handle(new ForumPagePreLoadEvent());
        session.LastVisit.Should().Be(new DateTime(2026, 1, 2, 3, 4, 5));
        this.board.HttpContext.Response.Headers.SetCookie.ToString().Should().Contain("PreviousVisit=");

        this.board.HttpContext.Session.Clear();
        this.board.HttpContext.Request.Headers.Cookie = "PreviousVisit=not-a-date";
        handler.Handle(new ForumPagePreLoadEvent());
        session.LastVisit.Should().Be(DateTimeHelper.SqlDbMinTime());
    }

    [Test]
    [Description("The spam statistics counters are increased and stored for the board")]
    public void Registry_Counters_Test()
    {
        this.Registry.IncrementDeniedRegistrations();
        this.Registry.IncrementDeniedRegistrations();
        this.Registry.IncrementBannedUsers();
        this.Registry.IncrementReportedSpammers();

        this.board.BoardSettings.DeniedRegistrations.Should().Be(2);
        this.Value("deniedregistrations").Should().Be("2");
        this.Value("bannedusers").Should().Be("1");
        this.Value("reportedspammers").Should().Be("1");
    }

    [Test]
    [Description("The database version decides between new install, upgrade and current")]
    public void Registry_Version_Test()
    {
        this.Registry.Delete(r => r.Name == "version");
        this.board.Cache.Remove(Constants.Cache.Version);

        this.Registry.GetDbVersion().Should().Be(-1);
        this.Registry.ValidateVersion(100).Should().Be(DbVersionType.NewInstall);

        this.Registry.Save("version", 90);
        this.board.Cache.Remove(Constants.Cache.Version);

        this.Registry.GetDbVersion().Should().Be(90);
        this.Registry.ValidateVersion(100).Should().Be(DbVersionType.Upgrade);
        this.Registry.ValidateVersion(90).Should().Be(DbVersionType.Current);
    }

    [Test]
    [Description("Legacy settings are deleted (setting names are stored lower case)")]
    public void Registry_Delete_Legacy_Test()
    {
        this.Registry.Save("smtpserver", "mail.example.com");
        this.Registry.Save("enableTopicdescription", "true");
        this.Registry.Save("keepme", "1");

        this.Registry.DeleteLegacy();

        this.Registry.List().Select(r => r.Name).Should().Contain("keepme")
            .And.NotContain("smtpserver")
            .And.NotContain("enabletopicdescription");
    }

    private IRepository<Activity> Activities => this.board.Current.GetRepository<Activity>();

    private IRepository<Registry> Registry => this.board.Current.GetRepository<Registry>();

    private string Value(string name)
    {
        return this.Registry.GetSingle(r => r.Name == name && r.BoardID == TestDatabase.BoardId)?.Value;
    }

    private long Unread()
    {
        return this.Activities.Count(a => a.UserID == this.alice && a.Notification);
    }

    private void AddActivity(int userId, int? fromUserId, ActivityFlags.Flags flags, int topicId, int messageId)
    {
        using var connection = this.db.Open();

        connection.Insert(
            new Activity
            {
                UserID = userId,
                FromUserID = fromUserId,
                Flags = (int)flags,
                TopicID = topicId,
                MessageID = messageId,
                Notification = true,
                Created = DateTime.UtcNow
            });
    }
}
