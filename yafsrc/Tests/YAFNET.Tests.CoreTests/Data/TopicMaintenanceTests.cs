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

namespace YAF.Tests.CoreTests.Data;

using System.Threading.Tasks;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Topic maintenance: moving, pruning, deleted topics, RSS lists and subject fix-ups.
/// </summary>
[TestFixture]
[NonParallelizable]
public class TopicMaintenanceTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int forum;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");

        this.forum = this.db.AddForum("General");
        this.board.GrantAccess(this.alice, this.forum);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("AttachPoll links a poll to the topic")]
    public void AttachPoll_Test()
    {
        var topic = this.db.AddTopic(this.forum, this.alice);

        this.Topics.AttachPoll(topic, 42);

        this.Topics.GetById(topic).PollID.Should().Be(42);
    }

    [Test]
    [Description("Moving with showMoved leaves a link topic in the old forum and refreshes both forums")]
    public async Task Move_With_Link_Test()
    {
        var target = this.db.AddForum();
        var topic = this.db.AddTopic(this.forum, this.alice, "Moving");
        this.db.AddMessage(topic, this.alice);

        await this.Topics.MoveAsync(topic, this.forum, target, true, 7);

        this.Topics.GetById(topic).ForumID.Should().Be(target);

        var link = this.Topics.GetSingle(t => t.TopicMovedID == topic);
        link.ForumID.Should().Be(this.forum);
        link.TopicName.Should().Be("Moving");
        link.LinkDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromMinutes(1));

        this.board.Events.OfType<UpdateForumStatsEvent>().Select(e => e.ForumId).Should().BeEquivalentTo([target, this.forum]);
    }

    [Test]
    [Description("The days of the link (sent by the client) are at least one day, a huge value does not fail")]
    public async Task Move_Link_Days_Limited_Test()
    {
        var target = this.db.AddForum();
        var first = this.db.AddTopic(this.forum, this.alice, "First");
        var second = this.db.AddTopic(this.forum, this.alice, "Second");

        await this.Topics.MoveAsync(first, this.forum, target, true, -5);
        await this.Topics.MoveAsync(second, this.forum, target, true, int.MaxValue);

        this.Topics.GetSingle(t => t.TopicMovedID == first).LinkDate
            .Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromMinutes(1));
        this.Topics.GetSingle(t => t.TopicMovedID == second).LinkDate
            .Should().BeCloseTo(DateTime.UtcNow.AddDays(36500), TimeSpan.FromMinutes(1));
    }

    [Test]
    [Description("Moving again replaces the old link, moving without showMoved leaves no link")]
    public async Task Move_Replaces_Link_Test()
    {
        var second = this.db.AddForum();
        var third = this.db.AddForum();
        var topic = this.db.AddTopic(this.forum, this.alice);

        await this.Topics.MoveAsync(topic, this.forum, second, true, 7);
        await this.Topics.MoveAsync(topic, second, third, true, 7);

        this.Topics.Get(t => t.TopicMovedID == topic).Should().ContainSingle().Which.ForumID.Should().Be(second);

        var other = this.db.AddTopic(this.forum, this.alice);
        await this.Topics.MoveAsync(other, this.forum, second, false, 7);

        this.Topics.Count(t => t.TopicMovedID == other).Should().Be(0);
    }

    [Test]
    [Description("Splitting a post creates the new topic in the chosen forum, started by the author of the post")]
    public async Task Create_By_Message_Test()
    {
        var topic = this.db.AddTopic(this.forum, this.alice);
        var posted = DateTime.UtcNow.AddDays(-2);
        var message = this.db.AddMessage(topic, this.bob, posted: posted);
        var target = this.db.AddForum();

        var newTopic = await this.Topics.CreateByMessageAsync(message, target, "Split off");

        var created = this.Topics.GetById(newTopic);
        created.TopicName.Should().Be("Split off");
        created.ForumID.Should().Be(target);
        created.UserID.Should().Be(this.bob);
        created.Posted.Should().BeCloseTo(posted, TimeSpan.FromSeconds(1));
        created.NumPosts.Should().Be(0, "the post is moved into it afterwards");
    }

    [Test]
    [Description("Prune removes old, unpinned, non-persistent topics of the forum and returns how many")]
    public async Task Prune_Test()
    {
        var old = this.AddTopicLastPosted(this.forum, DateTime.UtcNow.AddDays(-60));
        var recent = this.AddTopicLastPosted(this.forum, DateTime.UtcNow.AddDays(-5));
        var sticky = this.AddTopicLastPosted(this.forum, DateTime.UtcNow.AddDays(-60), t => t.Priority = 1);
        var persistent = this.AddTopicLastPosted(
            this.forum,
            DateTime.UtcNow.AddDays(-60),
            t => t.Flags = (int)TopicFlags.Flags.IsPersistent);

        var otherForum = this.db.AddForum();
        var otherOld = this.AddTopicLastPosted(otherForum, DateTime.UtcNow.AddDays(-60));

        var count = await this.Topics.PruneAsync(TestDatabase.BoardId, this.forum, 30, false);

        count.Should().Be(1);
        this.Topics.GetById(old).TopicFlags.IsDeleted.Should().BeTrue("pruned topics are soft deleted");
        this.Topics.GetById(recent).TopicFlags.IsDeleted.Should().BeFalse();
        this.Topics.GetById(sticky).TopicFlags.IsDeleted.Should().BeFalse("sticky topics are never pruned");
        this.Topics.GetById(persistent).TopicFlags.IsDeleted.Should().BeFalse("persistent topics are never pruned");
        this.Topics.GetById(otherOld).TopicFlags.IsDeleted.Should().BeFalse("only the selected forum is pruned");
    }

    [Test]
    [Description("0 or a negative number of days does not prune all topics (at least one day is kept)")]
    public async Task Prune_Invalid_Days_Test()
    {
        var today = this.AddTopicLastPosted(this.forum, DateTime.UtcNow.AddHours(-1));

        (await this.Topics.PruneAsync(TestDatabase.BoardId, this.forum, -1, false)).Should().Be(0);
        (await this.Topics.PruneAsync(TestDatabase.BoardId, this.forum, 0, false)).Should().Be(0);

        this.Topics.GetById(today).TopicFlags.IsDeleted.Should().BeFalse();
    }

    [Test]
    [Description("Prune with forum 0 covers all forums, permDelete erases the topics")]
    public async Task Prune_All_Forums_PermDelete_Test()
    {
        var otherForum = this.db.AddForum();
        var a = this.AddTopicLastPosted(this.forum, DateTime.UtcNow.AddDays(-60));
        var b = this.AddTopicLastPosted(otherForum, DateTime.UtcNow.AddDays(-60));

        (await this.Topics.PruneAsync(TestDatabase.BoardId, 0, 30, true)).Should().Be(2);

        this.Topics.GetById(a).Should().BeNull();
        this.Topics.GetById(b).Should().BeNull();
        this.Repository<Message>().Count(m => m.TopicID == a || m.TopicID == b).Should().Be(0);
    }

    [Test]
    [Description("Already deleted topics are not pruned again, but permDelete erases them")]
    public async Task Prune_Already_Deleted_Test()
    {
        var deleted = this.AddTopicLastPosted(
            this.forum,
            DateTime.UtcNow.AddDays(-60),
            t => t.Flags = (int)TopicFlags.Flags.IsDeleted);

        (await this.Topics.PruneAsync(TestDatabase.BoardId, 0, 30, false)).Should().Be(0);
        this.Topics.GetById(deleted).Should().NotBeNull();

        (await this.Topics.PruneAsync(TestDatabase.BoardId, 0, 30, true)).Should().Be(1);
        this.Topics.GetById(deleted).Should().BeNull();
    }

    [Test]
    [Description("Prune on a board without old topics does nothing")]
    public async Task Prune_Nothing_Test()
    {
        this.AddTopicLastPosted(this.forum, DateTime.UtcNow);

        (await this.Topics.PruneAsync(TestDatabase.BoardId, 0, 30, false)).Should().Be(0);
    }

    [Test]
    [Description("Deleted topics are listed, optionally filtered by subject")]
    public async Task GetDeletedTopics_Test()
    {
        var deletedA = this.db.AddTopic(this.forum, this.alice, "Spam offer", t => t.Flags = (int)TopicFlags.Flags.IsDeleted);
        var deletedB = this.db.AddTopic(this.forum, this.alice, "Old news", t => t.Flags = (int)TopicFlags.Flags.IsDeleted);
        this.db.AddTopic(this.forum, this.alice, "Spam but visible");

        (await this.Topics.GetDeletedTopicsAsync(TestDatabase.BoardId, null)).Select(x => x.Item2.ID)
            .Should().BeEquivalentTo([deletedA, deletedB]);

        (await this.Topics.GetDeletedTopicsAsync(TestDatabase.BoardId, "Spam")).Should().ContainSingle()
            .Which.Item1.ID.Should().Be(this.forum);

        (await this.Topics.GetDeletedTopicsAsync(TestDatabase.BoardId + 1, null)).Should().BeEmpty();
    }

    [Test]
    [Description("Deleted topics paged returns the page and the total count")]
    public void GetDeletedTopicsPaged_Test()
    {
        for (var i = 0; i < 5; i++)
        {
            this.db.AddTopic(this.forum, this.alice, $"Deleted {i}", t => t.Flags = (int)TopicFlags.Flags.IsDeleted);
        }

        this.db.AddTopic(this.forum, this.alice, "Visible");

        var page = this.Topics.GetDeletedTopicsPaged(TestDatabase.BoardId, null, 1, 2);

        page.Should().HaveCount(2).And.OnlyContain(t => t.TotalRows == 5 && t.ForumName == "General");

        var filtered = this.Topics.GetDeletedTopicsPaged(TestDatabase.BoardId, "Deleted 3", 0, 10);
        filtered.Should().ContainSingle().Which.TotalRows.Should().Be(1);

        this.Topics.GetDeletedTopicsPaged(TestDatabase.BoardId, "nothing like this", 0, 10).Should().BeEmpty();
    }

    [Test]
    [Description("RssLatest returns the newest visible posts the user can read")]
    public void RssLatest_Test()
    {
        var topic = this.db.AddTopic(this.forum, this.alice, "Visible");
        this.db.AddMessage(topic, this.alice, "first", DateTime.UtcNow.AddMinutes(-2));

        var deleted = this.db.AddTopic(this.forum, this.alice, "Deleted", t => t.Flags = (int)TopicFlags.Flags.IsDeleted);
        this.db.AddMessage(deleted, this.alice);

        var hiddenForum = this.db.AddForum();
        this.db.AddMessage(this.db.AddTopic(hiddenForum, this.alice, "No access"), this.alice);

        var latest = this.Topics.RssLatest(TestDatabase.BoardId, 10, this.alice);

        latest.Should().ContainSingle();
        latest[0].Item2.TopicName.Should().Be("Visible");
        latest[0].Item1.MessageText.Should().Be("first");
        latest[0].Item3.ID.Should().Be(this.alice);

        this.Topics.RssLatest(TestDatabase.BoardId, 10, this.bob).Should().BeEmpty("bob has no access");
    }

    [Test]
    [Description("RssList returns the latest topics of a forum, limited and newest first")]
    public void RssList_Test()
    {
        for (var i = 0; i < 3; i++)
        {
            var topic = this.db.AddTopic(this.forum, this.alice, $"Topic {i}");
            this.db.AddMessage(topic, this.alice, $"text {i}", DateTime.UtcNow.AddMinutes(i));
        }

        var list = this.Topics.RssList(this.forum, this.alice, 2);

        list.Select(t => t.Topic).Should().Equal("Topic 2", "Topic 1");
        list[0].Forum.Should().Be("General");
        list[0].LastMessage.Should().Be("text 2");

        this.Topics.RssList(this.forum, this.bob, 10).Should().BeEmpty("bob has no access");
    }

    [Test]
    [Description("SimpleList returns a window of topics by id")]
    public void SimpleList_Test()
    {
        var ids = Enumerable.Range(0, 4).Select(i => this.db.AddTopic(this.forum, this.alice, $"T{i}")).ToList();

        this.Topics.SimpleList(ids[1], 2).Select(t => t.ID).Should().Equal(ids[1], ids[2]);
        this.Topics.SimpleList().Should().HaveCount(4);
    }

    [Test]
    [Description("UnEncodeAllTopicsSubjects updates changed subjects and ignores failing ones")]
    public void UnEncodeAllTopicsSubjects_Test()
    {
        var encoded = this.db.AddTopic(this.forum, this.alice, "Tom &amp; Jerry");
        var plain = this.db.AddTopic(this.forum, this.alice, "Plain");
        var broken = this.db.AddTopic(this.forum, this.alice, "throw");

        this.Topics.UnEncodeAllTopicsSubjects(
            s => s == "throw" ? throw new InvalidOperationException() : System.Net.WebUtility.HtmlDecode(s));

        this.Topics.GetById(encoded).TopicName.Should().Be("Tom & Jerry");
        this.Topics.GetById(plain).TopicName.Should().Be("Plain");
        this.Topics.GetById(broken).TopicName.Should().Be("throw");
    }

    [Test]
    [Description("Watched topics of a user are listed newest first with the topic loaded")]
    public async Task WatchTopic_List_Test()
    {
        var first = this.db.AddTopic(this.forum, this.alice, "First");
        var second = this.db.AddTopic(this.forum, this.alice, "Second");
        var watches = this.Repository<WatchTopic>();

        await watches.AddAsync(this.alice, first);
        await watches.AddAsync(this.alice, second);
        await watches.AddAsync(this.bob, first);

        var list = await watches.ListAsync(this.alice);
        list.Select(w => w.Topic.TopicName).Should().Equal("Second", "First");

        (await watches.ListAsync(this.alice, 1, 1)).Should().ContainSingle().Which.TopicID.Should().Be(first);
        (await watches.CheckAsync(this.bob, second)).Should().BeNull();
    }

    private IRepository<Topic> Topics => this.Repository<Topic>();

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }

    private int AddTopicLastPosted(int forumId, DateTime lastPosted, Action<Topic> configure = null)
    {
        var topic = this.db.AddTopic(forumId, this.alice, configure: configure);
        this.db.AddMessage(topic, this.alice, posted: lastPosted);

        return topic;
    }
}
