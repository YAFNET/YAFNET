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

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Topic list queries (forum view, announcements, My Topics, latest topics) against the
/// in-memory database, with access rights built through the real permission views.
/// </summary>
[TestFixture]
[NonParallelizable]
public class TopicListTests
{
    private static readonly DateTime Since = DateTimeHelper.SqlDbMinTime();

    private readonly DateTime now = DateTime.UtcNow;

    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int eve;

    private int forumA;

    private int forumB;

    private int hiddenForum;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.eve = this.db.AddUser("Eve");

        var category = this.db.AddCategory();
        this.forumA = this.db.AddForum("Forum A", category);
        this.forumB = this.db.AddForum("Forum B", category);
        this.hiddenForum = this.db.AddForum("Staff only", category);

        // Alice can read A and B, Bob can read everything, Eve nothing
        this.board.GrantAccess(this.alice, this.forumA);
        this.board.GrantAccess(this.alice, this.forumB);
        this.board.GrantAccess(this.bob, this.forumA);
        this.board.GrantAccess(this.bob, this.forumB);
        this.board.GrantAccess(this.bob, this.hiddenForum);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("GrantAccess builds ActiveAccess rows from the access views")]
    public void GrantAccess_Test()
    {
        var access = this.Repository<ActiveAccess>().GetSingle(a => a.UserID == this.alice && a.ForumID == this.forumA);

        access.ReadAccess.Should().BeTrue();
        access.PostAccess.Should().BeTrue();
        access.ReplyAccess.Should().BeTrue();
        access.ModeratorAccess.Should().BeFalse();
        access.IsAdmin.Should().BeFalse();

        this.Repository<ActiveAccess>().Exists(a => a.UserID == this.alice && a.ForumID == this.hiddenForum)
            .Should().BeFalse();
        this.Repository<ActiveAccess>().Exists(a => a.UserID == this.eve).Should().BeFalse();
    }

    [Test]
    [Description("Group flags and access masks end up in ActiveAccess")]
    public void GrantAccess_Admin_And_Moderator_Test()
    {
        this.board.GrantAccess(
            this.eve,
            this.forumA,
            TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess | AccessFlags.Flags.DeleteAccess,
            GroupFlags.Flags.IsAdmin);

        var access = this.Repository<ActiveAccess>().GetSingle(a => a.UserID == this.eve && a.ForumID == this.forumA);

        access.IsAdmin.Should().BeTrue();
        access.ModeratorAccess.Should().BeTrue();
        access.DeleteAccess.Should().BeTrue();
        access.EditAccess.Should().BeFalse();
    }

    [Test]
    [Description("The forum topic list shows readable, normal topics, newest first")]
    public void ListPaged_Forum_Test()
    {
        var older = this.AddTopic(this.forumA, this.bob, "Older", this.now.AddHours(-2), replies: 2);
        var newer = this.AddTopic(this.forumA, this.bob, "Newer", this.now.AddHours(-1));

        var deleted = this.AddTopic(this.forumA, this.bob, "Deleted", this.now.AddMinutes(-30));
        this.SetTopicFlags(deleted, new TopicFlags { IsDeleted = true });

        this.AddTopic(this.forumA, this.bob, "Announcement", this.now.AddMinutes(-20), priority: 1);
        this.AddTopic(this.forumB, this.bob, "Other forum", this.now.AddMinutes(-10));
        this.db.AddTopic(this.forumA, this.bob, "Without messages");

        var topics = this.Topics.ListPaged(this.forumA, this.alice, Since, 0, 20, false);

        topics.Select(t => t.TopicID).Should().Equal(newer, older);
        topics.Should().OnlyContain(t => t.TotalRows == 2);

        var olderRow = topics.Single(t => t.TopicID == older);
        olderRow.Subject.Should().Be("Older");
        olderRow.Replies.Should().Be(2);
        olderRow.ForumName.Should().Be("Forum A");
        olderRow.LastMessage.Should().Be("Older post 2");
        olderRow.LastUserName.Should().Be("Bob");
    }

    [Test]
    [Description("Users without read access see nothing")]
    public void ListPaged_No_Access_Test()
    {
        this.AddTopic(this.forumA, this.bob, "Visible to members", this.now.AddHours(-1));
        this.AddTopic(this.hiddenForum, this.bob, "Staff topic", this.now.AddHours(-1));

        this.Topics.ListPaged(this.forumA, this.eve, Since, 0, 20, false).Should().BeEmpty();
        this.Topics.ListPaged(this.hiddenForum, this.alice, Since, 0, 20, false).Should().BeEmpty();
        this.Topics.ListPaged(this.hiddenForum, this.bob, Since, 0, 20, false).Should().ContainSingle();
    }

    [Test]
    [Description("A mask without read access hides the forum even though the user has a group")]
    public void ListPaged_Mask_Without_Read_Test()
    {
        this.board.GrantAccess(this.eve, this.forumA, AccessFlags.Flags.PostAccess);
        this.AddTopic(this.forumA, this.bob, "Topic", this.now.AddHours(-1));

        this.Topics.ListPaged(this.forumA, this.eve, Since, 0, 20, false).Should().BeEmpty();
    }

    [Test]
    [Description("Paging returns the requested page and the total row count")]
    public void ListPaged_Paging_Test()
    {
        var ids = Enumerable.Range(1, 5)
            .Select(i => this.AddTopic(this.forumA, this.bob, $"Topic {i}", this.now.AddHours(-i)))
            .ToList();

        var page = this.Topics.ListPaged(this.forumA, this.alice, Since, 1, 2, false);

        // newest first: ids[0], ids[1] | ids[2], ids[3] | ids[4]
        page.Select(t => t.TopicID).Should().Equal(ids[2], ids[3]);
        page.Should().OnlyContain(t => t.TotalRows == 5);

        this.Topics.ListPaged(this.forumA, this.alice, Since, 2, 2, false).Select(t => t.TopicID)
            .Should().Equal(ids[4]);
    }

    [Test]
    [Description("The since date hides topics without newer posts")]
    public void ListPaged_SinceDate_Test()
    {
        var recent = this.AddTopic(this.forumA, this.bob, "Recent", this.now.AddDays(-1));
        this.AddTopic(this.forumA, this.bob, "Old", this.now.AddDays(-10));

        this.Topics.ListPaged(this.forumA, this.alice, this.now.AddDays(-7), 0, 20, false)
            .Select(t => t.TopicID).Should().Equal(recent);
    }

    [Test]
    [Description("Moved topic links are listed and point to the target topic")]
    public void ListPaged_Moved_Topic_Link_Test()
    {
        var target = this.AddTopic(this.forumB, this.bob, "Moved here", this.now.AddHours(-2));
        var link = this.db.AddTopic(
            this.forumA,
            this.bob,
            "Moved here",
            t =>
            {
                t.TopicMovedID = target;
                t.LastPosted = this.now.AddHours(-1);
            });

        var row = this.Topics.ListPaged(this.forumA, this.alice, Since, 0, 20, false).Single();

        row.TopicID.Should().Be(link);
        row.LinkTopicID.Should().Be(target);
        row.TopicMovedID.Should().Be(target);
    }

    [Test]
    [Description("NumPostsDeleted counts the user's own deleted posts")]
    public void ListPaged_NumPostsDeleted_Test()
    {
        var topic = this.AddTopic(this.forumA, this.bob, "Topic", this.now.AddHours(-1));
        var deleted = new MessageFlags { IsApproved = true, IsDeleted = true }.BitValue;
        this.db.AddMessage(topic, this.alice, posted: this.now.AddMinutes(-50), configure: m => m.Flags = deleted);
        this.db.AddMessage(topic, this.alice, posted: this.now.AddMinutes(-40), configure: m => m.Flags = deleted);
        this.db.AddMessage(topic, this.bob, posted: this.now.AddMinutes(-30), configure: m => m.Flags = deleted);

        this.Topics.ListPaged(this.forumA, this.alice, Since, 0, 20, false).Single().NumPostsDeleted.Should().Be(2);
    }

    [Test]
    [Description("findLastRead adds the topic and forum read tracking dates")]
    public void ListPaged_FindLastRead_Test()
    {
        var topic = this.AddTopic(this.forumA, this.bob, "Topic", this.now.AddHours(-1));
        var lastRead = this.now.AddMinutes(-5);

        using (var connection = this.db.Open())
        {
            connection.Insert(new TopicReadTracking { UserID = this.alice, TopicID = topic, LastAccessDate = lastRead });
        }

        var withTracking = this.Topics.ListPaged(this.forumA, this.alice, Since, 0, 20, true).Single();
        var withoutTracking = this.Topics.ListPaged(this.forumA, this.alice, Since, 0, 20, false).Single();

        withTracking.LastTopicAccess.Should().BeCloseTo(lastRead, TimeSpan.FromSeconds(1));
        withTracking.LastForumAccess.Should().BeNull();
        withoutTracking.LastTopicAccess.Should().BeNull();
    }

    [Test]
    [Description("Announcements are the topics with a priority")]
    public void ListAnnouncementsPaged_Test()
    {
        this.AddTopic(this.forumA, this.bob, "Normal", this.now.AddHours(-1));
        var sticky = this.AddTopic(this.forumA, this.bob, "Sticky", this.now.AddHours(-2), priority: 1);
        var announcement = this.AddTopic(this.forumA, this.bob, "Announcement", this.now.AddHours(-3), priority: 2);

        this.Topics.ListAnnouncementsPaged(this.forumA, this.alice, 0, 10, false).Select(t => t.TopicID)
            .Should().Equal(sticky, announcement);
        this.Topics.ListAnnouncementsPaged(this.forumA, this.eve, 0, 10, false).Should().BeEmpty();
    }

    [Test]
    [Description("Active topics span all readable forums within the date range")]
    public void ListActivePaged_Test()
    {
        var a = this.AddTopic(this.forumA, this.bob, "In A", this.now.AddHours(-1));
        var b = this.AddTopic(this.forumB, this.bob, "In B", this.now.AddHours(-2));
        this.AddTopic(this.hiddenForum, this.bob, "Hidden", this.now.AddHours(-1));
        this.AddTopic(this.forumA, this.bob, "Too old", this.now.AddDays(-30));

        var topics = this.Topics.ListActivePaged(this.alice, this.now.AddDays(-7), this.now.AddMinutes(1), 0, 20, false);

        topics.Select(t => t.TopicID).Should().Equal(a, b);
        topics.Should().OnlyContain(t => t.TotalRows == 2);
    }

    [Test]
    [Description("Unanswered topics are the ones with only the first post")]
    public void ListUnansweredPaged_Test()
    {
        var unanswered = this.AddTopic(this.forumA, this.bob, "No replies", this.now.AddHours(-1));
        this.AddTopic(this.forumB, this.bob, "With reply", this.now.AddHours(-2), replies: 1);

        this.Topics.ListUnansweredPaged(this.alice, Since, this.now.AddMinutes(1), 0, 20, false)
            .Select(t => t.TopicID).Should().Equal(unanswered);
    }

    [Test]
    [Description("My topics only lists topics the user started")]
    public void ListByUserPaged_Test()
    {
        var mine = this.AddTopic(this.forumA, this.alice, "Mine", this.now.AddHours(-1));
        var bobsTopic = this.AddTopic(this.forumA, this.bob, "Bob's", this.now.AddHours(-2));

        // replying to Bob's topic doesn't make it Alice's
        this.db.AddMessage(bobsTopic, this.alice, posted: this.now.AddMinutes(-90));

        this.Topics.ListByUserPaged(this.alice, Since, this.now.AddMinutes(1), 0, 20)
            .Select(t => t.TopicID).Should().Equal(mine);
    }

    [Test]
    [Description("Watched topics lists only the topics the user watches and still can read, with a matching total")]
    public void ListWatchedPaged_Test()
    {
        var watched1 = this.AddTopic(this.forumA, this.bob, "Watched 1", this.now.AddHours(-1));
        var watched2 = this.AddTopic(this.forumB, this.bob, "Watched 2", this.now.AddHours(-2));
        this.AddTopic(this.forumA, this.bob, "Not watched", this.now.AddHours(-3));
        this.AddTopic(this.forumB, this.bob, "Not watched either", this.now.AddHours(-4));

        // watched, but in a forum Alice can not read (anymore): the list would show its last post
        var hidden = this.AddTopic(this.hiddenForum, this.bob, "Hidden", this.now.AddMinutes(-30));

        using (var connection = this.db.Open())
        {
            connection.Insert(new WatchTopic { UserID = this.alice, TopicID = watched1, Created = this.now });
            connection.Insert(new WatchTopic { UserID = this.alice, TopicID = watched2, Created = this.now });
            connection.Insert(new WatchTopic { UserID = this.alice, TopicID = hidden, Created = this.now });
            connection.Insert(new WatchTopic { UserID = this.bob, TopicID = watched1, Created = this.now });
        }

        var topics = this.Topics.ListWatchedPaged(this.alice, Since, this.now.AddMinutes(1), 0, 20);

        topics.Select(t => t.TopicID).Should().Equal(watched1, watched2);
        topics.Should().OnlyContain(t => t.TotalRows == 2, "the pager on My Topics uses TotalRows");

        var firstPage = this.Topics.ListWatchedPaged(this.alice, Since, this.now.AddMinutes(1), 0, 1);

        firstPage.Should().ContainSingle().Which.TotalRows.Should().Be(2);
        this.Topics.ListWatchedPaged(this.bob, Since, this.now.AddMinutes(1), 0, 20).Should().ContainSingle()
            .Which.TotalRows.Should().Be(1);
    }

    [Test]
    [Description("Latest topics use the access view and skip test forums unless requested")]
    public void Latest_Test()
    {
        var testForum = this.db.AddForum("Test forum", configure: f => f.Flags = new ForumFlags { IsTest = true }.BitValue);
        this.board.GrantAccess(this.alice, testForum);

        var a = this.AddTopic(this.forumA, this.bob, "In A", this.now.AddHours(-1));
        var b = this.AddTopic(this.forumB, this.bob, "In B", this.now.AddHours(-2));
        var test = this.AddTopic(testForum, this.bob, "In test forum", this.now.AddMinutes(-30));
        this.AddTopic(this.hiddenForum, this.bob, "Hidden", this.now.AddMinutes(-10));

        this.Topics.Latest(TestDatabase.BoardId, 0, 10, this.alice, false, false)
            .Select(t => t.TopicID).Should().Equal(a, b);

        this.Topics.Latest(TestDatabase.BoardId, 0, 10, this.alice, true, false)
            .Select(t => t.TopicID).Should().Equal(test, a, b);

        this.Topics.Latest(TestDatabase.BoardId, 0, 1, this.alice, false, false)
            .Select(t => t.TopicID).Should().Equal(a);

        this.Topics.Latest(TestDatabase.BoardId, 0, 10, this.eve, true, false).Should().BeEmpty();
    }

    [Test]
    [Description("Latest topics can be filtered by category and sorted by views or posts")]
    public void Latest_Category_And_Sorting_Test()
    {
        var otherCategory = this.db.AddCategory();
        var otherForum = this.db.AddForum(categoryId: otherCategory);
        this.board.GrantAccess(this.alice, otherForum);

        var popular = this.AddTopic(this.forumA, this.bob, "Popular", this.now.AddHours(-3), views: 100);
        var busy = this.AddTopic(this.forumA, this.bob, "Busy", this.now.AddHours(-2), replies: 3);
        var recent = this.AddTopic(otherForum, this.bob, "Recent", this.now.AddHours(-1));

        var forumACategory = this.Repository<Forum>().GetById(this.forumA).CategoryID;

        this.Topics.Latest(TestDatabase.BoardId, forumACategory, 10, this.alice, false, false)
            .Select(t => t.TopicID).Should().Equal(busy, popular);

        this.Topics.Latest(TestDatabase.BoardId, 0, 10, this.alice, false, false, sortOrder: 1)
            .First().TopicID.Should().Be(popular);

        this.Topics.Latest(TestDatabase.BoardId, 0, 10, this.alice, false, false, sortOrder: 2)
            .First().TopicID.Should().Be(busy);

        this.Topics.Latest(TestDatabase.BoardId, 0, 10, this.alice, false, false)
            .First().TopicID.Should().Be(recent);
    }

    private IRepository<Topic> Topics => this.board.Current.GetRepository<Topic>();

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }

    /// <summary>
    /// Adds a topic with a first post at <paramref name="lastPosted"/> minus the replies, and
    /// <paramref name="replies"/> replies so the last post is at <paramref name="lastPosted"/>.
    /// </summary>
    private int AddTopic(
        int forumId,
        int userId,
        string subject,
        DateTime lastPosted,
        int replies = 0,
        short priority = 0,
        int views = 0)
    {
        var topic = this.db.AddTopic(
            forumId,
            userId,
            subject,
            t =>
            {
                t.Priority = priority;
                t.Views = views;
                t.UserName = this.Repository<User>().GetById(userId).Name;
            });

        for (var i = 0; i <= replies; i++)
        {
            this.db.AddMessage(
                topic,
                userId,
                i == 0 ? $"{subject} first post" : $"{subject} post {i}",
                posted: lastPosted.AddMinutes(i - replies),
                configure: m => m.UserName = this.Repository<User>().GetById(userId).Name);
        }

        return topic;
    }

    private void SetTopicFlags(int topicId, TopicFlags flags)
    {
        this.Topics.Lock(topicId, flags.BitValue);
    }
}
