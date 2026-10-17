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

namespace YAF.Tests.Data;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Who-is-online (Active) repository.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ActiveRepositoryTests
{
    private const int Registered = (int)(ActiveFlags.Flags.IsActiveNow | ActiveFlags.Flags.IsRegistered);

    private const int Guest = (int)(ActiveFlags.Flags.IsActiveNow | ActiveFlags.Flags.IsGuest);

    private const int Crawler = Guest | (int)ActiveFlags.Flags.IsCrawler;

    private TestBoardContext board;

    private TestDatabase db;

    private int guest;

    private int alice;

    private int bob;

    private int forum;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.guest = this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");

        this.board.PageLoad.GuestUserID = this.guest;

        this.forum = this.db.AddForum("General");
        this.topic = this.db.AddTopic(this.forum, this.alice, "Hello");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("ListForum lists each user in the forum once, with the number of sessions")]
    public void ListForum_Test()
    {
        var otherForum = this.db.AddForum();

        this.AddActive(this.bob, "s1", Registered, this.forum);
        this.AddActive(this.alice, "s2", Registered, this.forum);
        this.AddActive(this.alice, "s3", Registered, this.forum);
        this.AddActive(this.guest, "s4", Guest, otherForum);

        var users = this.Active.ListForum(this.forum);

        users.Select(u => u.UserName).Should().Equal("Alice", "Bob");
        users.Single(u => u.UserID == this.alice).UserCount.Should().Be(2);
        users.Single(u => u.UserID == this.bob).UserCount.Should().Be(1);

        this.Active.ListForum(this.db.AddForum()).Should().BeEmpty();
    }

    [Test]
    [Description("ListTopic lists the users reading the topic")]
    public void ListTopic_Test()
    {
        this.AddActive(this.alice, "s1", Registered, this.forum, this.topic);
        this.AddActive(this.bob, "s2", Registered, this.forum);

        this.Active.ListTopic(this.topic).Should().ContainSingle().Which.UserID.Should().Be(this.alice);
    }

    [Test]
    [Description("The forum and topic lists mark hidden users and guests (the active flags, not the user flags)")]
    public void ListForum_ListTopic_Flags_Test()
    {
        // approved, hidden ("hide me") and excluded from the captcha (user flag 8 is the crawler flag of an active row)
        var carol = this.db.AddUser(
            "Carol",
            u => u.Flags |= (int)(UserFlags.Flags.IsActiveExcluded | UserFlags.Flags.IsCaptchaExcluded));

        this.AddActive(this.alice, "s1", Registered, this.forum, this.topic);
        this.AddActive(carol, "s2", Registered, this.forum, this.topic);
        this.AddActive(this.guest, "s3", Guest, this.forum, this.topic);

        foreach (var users in new[] { this.Active.ListForum(this.forum), this.Active.ListTopic(this.topic) })
        {
            var alice = users.Single(u => u.UserID == this.alice);
            alice.IsGuest.Should().BeFalse("an approved user is no guest");
            alice.IsActiveExcluded.Should().BeFalse();

            var hidden = users.Single(u => u.UserID == carol);
            hidden.IsActiveExcluded.Should().BeTrue("the user is hidden from the active lists");
            hidden.ActiveFlags.IsCrawler.Should().BeFalse("a user is no crawler");

            users.Single(u => u.UserID == this.guest).IsGuest.Should().BeTrue();
        }
    }

    [Test]
    [Description("List shows registered users with forum and topic names, crawlers only when asked")]
    public void List_Test()
    {
        this.AddActive(this.alice, "s1", Registered, this.forum, this.topic);
        this.AddActive(this.guest, "s2", Crawler);
        this.AddActive(this.guest, "s3", Guest);

        var members = this.Active.List(false, 15);

        var alice = members.Should().ContainSingle().Subject;
        alice.UserID.Should().Be(this.alice);
        alice.ForumName.Should().Be("General");
        alice.TopicName.Should().Be("Hello");
        alice.IsGuest.Should().BeFalse();

        this.Active.List(true, 15).Select(u => u.SessionID).Should().BeEquivalentTo(["s1", "s2"], "plain guests are never listed");
    }

    [Test]
    [Description("List removes sessions that were inactive for longer than activeTime")]
    public void List_Deletes_Inactive_Test()
    {
        this.AddActive(this.alice, "fresh", Registered, lastActive: DateTime.UtcNow.AddMinutes(-5));
        this.AddActive(this.bob, "stale", Registered, lastActive: DateTime.UtcNow.AddMinutes(-60));

        this.Active.List(false, 15).Select(u => u.SessionID).Should().Equal("fresh");
        this.Active.Count(a => a.SessionID == "stale").Should().Be(0);
    }

    [Test]
    [Description("List only shows sessions of the given board")]
    public void List_Other_Board_Test()
    {
        this.AddActive(this.alice, "s1", Registered);

        this.Active.List(true, 15, TestDatabase.BoardId + 1).Should().BeEmpty();
    }

    [Test]
    [Description("Stats counts members, guests and hidden users separately")]
    public void Stats_Test()
    {
        var hidden = this.db.AddUser("Hidden", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsActiveExcluded));

        this.AddActive(this.alice, "s1", Registered);
        this.AddActive(this.bob, "s2", Registered);
        this.AddActive(this.guest, "s3", Guest);
        this.AddActive(this.guest, "s4", Guest);
        this.AddActive(hidden, "s5", Registered);

        var stats = this.Active.Stats(TestDatabase.BoardId);

        stats.ActiveUsers.Should().Be(4, "hidden users are not counted as active");
        stats.ActiveMembers.Should().Be(2);
        stats.ActiveGuests.Should().Be(2);
        stats.ActiveHidden.Should().Be(1);
    }

    [Test]
    [Description("Stats of a board nobody is visiting are all zero")]
    public void Stats_Empty_Test()
    {
        this.Active.Stats(TestDatabase.BoardId).Should().Be((0, 0, 0, 0));
    }

    [Test]
    [Description("ListUsersPaged with guests lists members and guests in forums the viewer can read")]
    public void ListUsersPaged_With_Guests_Test()
    {
        this.board.GrantAccess(this.alice, this.forum);

        this.AddActive(this.bob, "s1", Registered, this.forum, lastActive: DateTime.UtcNow.AddMinutes(-2));
        this.AddActive(this.guest, "s2", Guest, this.forum, lastActive: DateTime.UtcNow.AddMinutes(-1));

        var users = this.Active.ListUsersPaged(this.alice, true, false, 0, 10);

        users.Select(u => u.SessionID).Should().Equal("s2", "s1");
        users.Should().OnlyContain(u => u.UserCount == 2 && u.HasForumAccess);
        users[0].IsGuest.Should().BeTrue();
    }

    [Test]
    [Description("ListUsersPaged without guests lists only members")]
    public void ListUsersPaged_Members_Only_Test()
    {
        this.board.GrantAccess(this.alice, this.forum);

        this.AddActive(this.bob, "s1", Registered, this.forum);
        this.AddActive(this.guest, "s2", Guest, this.forum);

        this.Active.ListUsersPaged(this.alice, false, false, 0, 10)
            .Should().ContainSingle().Which.UserID.Should().Be(this.bob);
    }

    [Test]
    [Description("ListUsersPaged shows crawlers only when asked, for every guests/crawlers combination")]
    [TestCase(true, true, new[] { "member", "guest", "crawler", "noflags" })]
    [TestCase(true, false, new[] { "member", "guest", "noflags" })]
    [TestCase(false, true, new[] { "member", "crawler" })]
    [TestCase(false, false, new[] { "member" })]
    public void ListUsersPaged_Crawlers_Test(bool showGuests, bool showCrawlers, string[] expected)
    {
        this.board.GrantAccess(this.alice, this.forum);

        this.AddActive(this.bob, "member", Registered, this.forum);
        this.AddActive(this.guest, "guest", Guest, this.forum);
        this.AddActive(this.guest, "crawler", Crawler, this.forum);

        // a captcha excluded member is not a crawler (user flag 8 used to be mistaken for one)
        var captchaExcluded = this.db.AddUser(
            "NoCaptcha",
            u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsCaptchaExcluded | UserFlags.Flags.IsGuest));
        this.AddActive(captchaExcluded, "noflags", 0, this.forum);

        using (var connection = this.db.Open())
        {
            connection.UpdateOnly(() => new Active { Flags = null }, a => a.SessionID == "noflags");
        }

        this.Active.ListUsersPaged(this.alice, showGuests, showCrawlers, 0, 10).Select(u => u.SessionID)
            .Should().BeEquivalentTo(expected);
    }

    [Test]
    [Description("ListUsersPaged pages the list, the total count stays the same")]
    public void ListUsersPaged_Paging_Test()
    {
        this.board.GrantAccess(this.alice, this.forum);

        for (var i = 0; i < 5; i++)
        {
            this.AddActive(this.bob, $"s{i}", Registered, this.forum, lastActive: DateTime.UtcNow.AddMinutes(-i));
        }

        var page2 = this.Active.ListUsersPaged(this.alice, true, false, 1, 2);

        page2.Select(u => u.SessionID).Should().Equal("s2", "s3");
        page2.Should().OnlyContain(u => u.UserCount == 5);
    }

    [Test]
    [Description("Users in forums the viewer has no access mask for are not listed")]
    public void ListUsersPaged_No_Access_Test()
    {
        this.board.GrantAccess(this.alice, this.forum);

        this.AddActive(this.bob, "s1", Registered, this.db.AddForum());

        this.Active.ListUsersPaged(this.alice, true, false, 0, 10).Should().BeEmpty();
    }

    [Test]
    [Description("Users outside of any forum (e.g. on the board index) are listed")]
    public void ListUsersPaged_Outside_Forum_Test()
    {
        this.board.GrantAccess(this.alice, this.forum);

        this.AddActive(this.bob, "index", Registered);

        this.Active.ListUsersPaged(this.alice, true, false, 0, 10)
            .Should().ContainSingle().Which.SessionID.Should().Be("index");
    }

    private IRepository<Active> Active => this.board.Current.GetRepository<Active>();

    private void AddActive(
        int userId,
        string sessionId,
        int flags,
        int? forumId = null,
        int? topicId = null,
        DateTime? lastActive = null)
    {
        using var connection = this.db.Open();

        var time = lastActive ?? DateTime.UtcNow;

        connection.Insert(
            new Active
            {
                BoardID = TestDatabase.BoardId,
                UserID = userId,
                SessionID = sessionId,
                Flags = flags,
                ForumID = forumId,
                TopicID = topicId,
                IP = "127.0.0.1",
                Login = time.AddMinutes(-10),
                LastActive = time,
                Browser = "Firefox",
                Platform = "Windows"
            });
    }
}
