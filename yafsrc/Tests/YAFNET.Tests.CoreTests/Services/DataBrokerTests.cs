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

using System.Threading.Tasks;

using Autofac;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// <see cref="DataBroker"/>: the per-request page load (who is where, with which access) and cached lookups.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DataBrokerTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private DataBroker broker;

    private int guest;

    private int alice;

    private int forum;

    private int category;

    private int topic;

    private int message;

    [SetUp]
    public void SetUp()
    {
        var aspNetUsers = Substitute.For<IAspNetUsersHelper>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<DataBroker>().AsSelf();
                b.RegisterInstance(aspNetUsers).As<IAspNetUsersHelper>();
            },
            useDatabase: true);

        this.db = this.board.Database;

        this.guest = this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));
        this.alice = this.db.AddUser("Alice", u =>
        {
            u.ProviderUserKey = "alice-key";
            u.LastVisit = DateTime.UtcNow.AddDays(-3);
        });

        aspNetUsers.GuestUser(TestDatabase.BoardId).Returns(_ => this.Repository<User>().GetById(this.guest));

        this.category = this.db.AddCategory();
        this.forum = this.db.AddForum("General", this.category);
        this.topic = this.db.AddTopic(this.forum, this.alice, "Hello");
        this.message = this.db.AddMessage(this.topic, this.alice);

        this.db.AddForumAccess(this.guest, this.forum, AccessFlags.Flags.ReadAccess);
        this.db.AddForumAccess(this.alice, this.forum, AccessFlags.Flags.ReadAccess | AccessFlags.Flags.PostAccess | AccessFlags.Flags.ModeratorAccess);

        this.broker = this.board.Resolve<DataBroker>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A guest request records a guest session and returns the guest's forum access")]
    public void GetPageLoad_Guest_Test()
    {
        var (pageLoad, user, category, forum, topic, message) = this.Load("s1", null, forumId: this.forum);

        user.ID.Should().Be(this.guest);
        forum.ID.Should().Be(this.forum);
        category.ID.Should().Be(this.category);
        topic.Should().BeNull();
        message.Should().BeNull();

        pageLoad.UserID.Should().Be(this.guest);
        pageLoad.GuestUserID.Should().Be(this.guest);
        pageLoad.ReadAccess.Should().BeTrue();
        pageLoad.PostAccess.Should().BeFalse();
        pageLoad.IsCrawler.Should().BeFalse();

        var active = this.Repository<Active>().GetSingle(a => a.SessionID == "s1");
        active.UserID.Should().Be(this.guest);
        active.ForumID.Should().Be(this.forum);
        active.Flags.Should().Be((int)(ActiveFlags.Flags.IsActiveNow | ActiveFlags.Flags.IsGuest));
    }

    [Test]
    [Description("A logged-in request records a registered session, updates the last visit and returns the user's access")]
    public void GetPageLoad_Registered_Test()
    {
        var (pageLoad, user, _, _, _, _) = this.Load("s1", "alice-key", ip: "10.0.0.1");

        user.ID.Should().Be(this.alice);
        pageLoad.UserID.Should().Be(this.alice);
        pageLoad.PreviousVisit.Should().BeBefore(DateTime.UtcNow.AddDays(-2));
        pageLoad.IsModeratorAny.Should().BeTrue();

        var updated = this.Repository<User>().GetById(this.alice);
        updated.LastVisit.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        updated.IP.Should().Be("10.0.0.1");

        this.Repository<Active>().GetSingle(a => a.SessionID == "s1").Flags
            .Should().Be((int)(ActiveFlags.Flags.IsActiveNow | ActiveFlags.Flags.IsRegistered));
    }

    [Test]
    [Description("The access rows are cleared (by a change of the groups) while the page loads: they are created again, the page load data is not missing")]
    public void GetPageLoad_Access_Cleared_While_Loading_Test()
    {
        // the access rows exist from an earlier request
        this.Load("s1", "alice-key");

        // a group change in another request clears them after they were checked (when the new session is saved)
        this.board.Events.OnRaise = e =>
        {
            if (e is RepositoryEvent<Active>)
            {
                this.board.Events.OnRaise = null;
                this.Repository<ActiveAccess>().DeleteAll();
            }
        };

        var (pageLoad, user, _, _, _, _) = this.Load("s2", "alice-key", forumId: this.forum);

        this.board.Events.OnRaise.Should().BeNull("the access rows were cleared while loading");
        user.ID.Should().Be(this.alice);
        pageLoad.Should().NotBeNull();
        pageLoad.UserID.Should().Be(this.alice);
        pageLoad.ReadAccess.Should().BeTrue();
        pageLoad.PostAccess.Should().BeTrue();
    }

    [Test]
    [Description("An unknown user key is treated as a guest")]
    public void GetPageLoad_Unknown_User_Test()
    {
        var (pageLoad, user, _, _, _, _) = this.Load("s1", "nobody");

        user.ID.Should().Be(this.guest);
        pageLoad.UserID.Should().Be(this.guest);
    }

    [Test]
    [Description("A message id alone resolves topic, forum and category")]
    public void GetPageLoad_From_Message_Test()
    {
        var (_, _, category, forum, topic, message) = this.Load("s1", "alice-key", messageId: this.message);

        message.ID.Should().Be(this.message);
        topic.ID.Should().Be(this.topic);
        forum.ID.Should().Be(this.forum);
        category.ID.Should().Be(this.category);
    }

    [Test]
    [Description("A topic id alone resolves the topic, forum and category")]
    public void GetPageLoad_From_Topic_Test()
    {
        var (_, _, category, forum, topic, _) = this.Load("s1", "alice-key", topicId: this.topic);

        topic.ID.Should().Be(this.topic);
        topic.TopicName.Should().Be("Hello");
        forum.ID.Should().Be(this.forum);
        forum.Name.Should().Be("General");
        category.ID.Should().Be(this.category);

        this.Repository<Active>().GetSingle(a => a.SessionID == "s1").TopicID.Should().Be(this.topic);
    }

    [Test]
    [Description("A topic or message of a hidden forum sent with the ids of a readable forum gets the access of its own forum")]
    public void GetPageLoad_Topic_Of_Other_Forum_Test()
    {
        var hiddenForum = this.db.AddForum("Hidden", this.category);
        var hiddenTopic = this.db.AddTopic(hiddenForum, this.alice, "Secret");
        var hiddenMessage = this.db.AddMessage(hiddenTopic, this.alice);

        this.db.AddForumAccess(this.alice, hiddenForum, AccessFlags.Flags.None);

        var (pageLoad, _, _, forum, topic, _) =
            this.Load("s1", "alice-key", categoryId: this.category, forumId: this.forum, topicId: hiddenTopic);

        topic.ID.Should().Be(hiddenTopic);
        forum.ID.Should().Be(hiddenForum, "the forum of the topic is used, not the forum of the request");
        pageLoad.ReadAccess.Should().BeFalse();

        (pageLoad, _, _, forum, topic, var message) = this.Load(
            "s1",
            "alice-key",
            categoryId: this.category,
            forumId: this.forum,
            topicId: this.topic,
            messageId: hiddenMessage);

        message.ID.Should().Be(hiddenMessage);
        topic.ID.Should().Be(hiddenTopic, "the topic of the message is used");
        forum.ID.Should().Be(hiddenForum);
        pageLoad.ReadAccess.Should().BeFalse();
    }

    [Test]
    [Description("Ids that do not exist are dropped")]
    public void GetPageLoad_Invalid_Ids_Test()
    {
        var (pageLoad, _, category, forum, topic, message) =
            this.Load("s1", "alice-key", forumId: 9999, categoryId: 9999, topicId: 9999, messageId: 9999);

        category.Should().BeNull();
        forum.Should().BeNull();
        topic.Should().BeNull();
        message.Should().BeNull();
        pageLoad.Should().NotBeNull("the board level access is returned");

        var active = this.Repository<Active>().GetSingle(a => a.SessionID == "s1");
        active.ForumID.Should().BeNull();
        active.TopicID.Should().BeNull();
    }

    [Test]
    [Description("Visiting a forum does not update the last visit (only board level pages do)")]
    public void GetPageLoad_Forum_Keeps_LastVisit_Test()
    {
        var before = this.Repository<User>().GetById(this.alice).LastVisit;

        this.Load("s1", "alice-key", forumId: this.forum);

        this.Repository<User>().GetById(this.alice).LastVisit.Should().Be(before);
    }

    [Test]
    [Description("Do-not-track requests are not recorded as active")]
    public void GetPageLoad_DoNotTrack_Test()
    {
        var (pageLoad, _, _, _, _, _) = this.Load("s1", "alice-key", doNotTrack: true);

        pageLoad.UserID.Should().Be(this.alice);
        this.Repository<Active>().Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("The same session is updated, a new session of a logged-in user replaces the old one")]
    public void GetPageLoad_Session_Update_Test()
    {
        this.Load("s1", "alice-key");
        this.Load("s1", "alice-key", forumId: this.forum);

        this.Repository<Active>().GetSingle(a => a.SessionID == "s1").ForumID.Should().Be(this.forum);

        this.Load("s2", "alice-key");

        this.Repository<Active>().Get(a => a.UserID == this.alice).Select(a => a.SessionID).Should().Equal("s2");
    }

    [Test]
    [Description("Only the first request of a new logged-in session flags the active list for an update")]
    public void GetPageLoad_ActiveUpdate_Test()
    {
        this.Load("s1", "alice-key").Item1.ActiveUpdate.Should().BeTrue();
        this.Load("s1", "alice-key").Item1.ActiveUpdate.Should().BeFalse();
        this.Load("g1", null).Item1.ActiveUpdate.Should().BeFalse("guests do not update the active list");
    }

    [Test]
    [Description("A crawler is recorded once per browser and IP even when it sends a new session every time")]
    public void GetPageLoad_Crawler_Test()
    {
        this.Load("c1", null, browser: "Googlebot", isCrawler: true).Item1.IsCrawler.Should().BeTrue();
        this.Load("c2", null, browser: "Googlebot", isCrawler: true);

        var active = this.Repository<Active>().Get(_ => true).Should().ContainSingle().Subject;
        active.Flags.Should().Be((int)(ActiveFlags.Flags.IsActiveNow | ActiveFlags.Flags.IsGuest | ActiveFlags.Flags.IsCrawler));
    }

    [Test]
    [Description("A logged-in user is never a crawler")]
    public void GetPageLoad_Registered_Not_Crawler_Test()
    {
        this.Load("s1", "alice-key", isCrawler: true).Item1.IsCrawler.Should().BeFalse();
    }

    [Test]
    [Description("A crawler with a different IP is recorded as its own visitor")]
    public void GetPageLoad_Crawler_Other_IP_Test()
    {
        this.Load("c1", null, browser: "Googlebot", ip: "66.249.0.1", isCrawler: true);
        this.Load("c2", null, browser: "Googlebot", ip: "66.249.0.2", isCrawler: true);

        this.Repository<Active>().Get(_ => true).Select(a => a.IP).Should().BeEquivalentTo(["66.249.0.1", "66.249.0.2"]);
    }

    [Test]
    [Description("A visitor using the same browser as an active crawler still gets recorded")]
    public void GetPageLoad_Same_Browser_As_Crawler_Test()
    {
        this.Load("c1", null, browser: "Chrome", ip: "66.249.0.1", isCrawler: true);

        this.Load("s1", "alice-key", browser: "Chrome", ip: "10.0.0.1");

        this.Repository<Active>().Count(a => a.SessionID == "s1" && a.UserID == this.alice).Should().Be(1);
    }

    [Test]
    [Description("A crawler behind the same IP and browser as a person does not take over the person's session")]
    public void GetPageLoad_Crawler_Same_IP_As_User_Test()
    {
        this.Load("s1", "alice-key", browser: "Chrome", ip: "10.0.0.1");

        this.Load("c1", null, browser: "Chrome", ip: "10.0.0.1", isCrawler: true);

        this.Repository<Active>().GetSingle(a => a.SessionID == "s1").UserID.Should().Be(this.alice);
        this.Repository<Active>().Count(a => a.SessionID == "c1").Should().Be(1);
    }

    [Test]
    [Description("A new session updates the board's max online statistics (distinct IPs)")]
    public void GetPageLoad_MaxStats_Test()
    {
        this.Load("s1", "alice-key", ip: "10.0.0.1");
        this.Load("g1", null, ip: "10.0.0.2");

        this.Repository<Registry>().GetSingle(r => r.Name == "maxusers" && r.BoardID == TestDatabase.BoardId).Value
            .Should().Be("2");
    }

    [Test]
    [Description("BoardLayout returns the forums of the board, category 0 means all categories")]
    public async Task BoardLayout_Test()
    {
        this.board.RefreshActiveAccess(this.alice);

        var forums = await this.broker.BoardLayoutAsync(TestDatabase.BoardId, this.alice, 0, 10, 0, null);

        forums.Should().ContainSingle().Which.ForumID.Should().Be(this.forum);
    }

    [Test]
    [Description("BoardLayout returns an empty list for a user without access")]
    public async Task BoardLayout_No_Access_Test()
    {
        (await this.broker.BoardLayoutAsync(TestDatabase.BoardId, 9999, 0, 10, null, null)).Should().BeEmpty();
    }

    [Test]
    [Description("The user lazy data is cached per user")]
    public async Task ActiveUserLazyData_Cached_Test()
    {
        var first = await this.broker.ActiveUserLazyDataAsync(this.alice);
        var second = await this.broker.ActiveUserLazyDataAsync(this.alice);

        first.Should().NotBeNull();
        second.Should().BeSameAs(first);
    }

    [Test]
    [Description("The forum moderators list is cached")]
    public async Task GetModerators_Cached_Test()
    {
        var first = await this.broker.GetModeratorsAsync();

        (await this.broker.GetModeratorsAsync()).Should().BeSameAs(first);
    }

    [Test]
    [Description("Digest topics are the recent topics of the forums the user can read")]
    public void GetDigestTopics_Test()
    {
        var otherForum = this.db.AddForum();
        this.db.AddMessage(this.db.AddTopic(otherForum, this.alice, "Hidden"), this.alice);

        var oldTopic = this.db.AddTopic(this.forum, this.alice, "Old");
        this.db.AddMessage(oldTopic, this.alice, posted: DateTime.UtcNow.AddDays(-10));

        var topics = this.broker.GetDigestTopics(TestDatabase.BoardId, this.alice, DateTime.UtcNow.AddDays(-1), 10);

        topics.Select(t => t.Subject).Should().Equal("Hello");
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }

    private Tuple<PageLoad, User, Category, Forum, Topic, Message> Load(
        string sessionId,
        string userKey,
        int? categoryId = null,
        int? forumId = null,
        int? topicId = null,
        int? messageId = null,
        string browser = "Firefox",
        string ip = "127.0.0.1",
        bool isCrawler = false,
        bool doNotTrack = false)
    {
        return this.broker.GetPageLoad(
            sessionId,
            TestDatabase.BoardId,
            userKey,
            ip,
            "/path",
            null,
            "DE",
            "page",
            browser,
            "Windows",
            "UA",
            categoryId,
            forumId,
            topicId,
            messageId,
            isCrawler,
            doNotTrack);
    }
}
