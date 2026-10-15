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

using System.Threading.Tasks;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// ForumRepositoryExtensions tests: saving, the forum index, forum jump lists, statistics,
/// moderation lists, deleting and moving forums.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ForumRepositoryTests
{
    private readonly DateTime now = DateTime.UtcNow;

    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int category;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.category = this.db.AddCategory("General");

        this.board.PageUser.ID = this.alice;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Save inserts a forum with flags, and updates it in place")]
    public void Save_Insert_And_Update_Test()
    {
        var id = this.Forums.Save(
            null, this.category, 0, "New forum", "desc", 3, true, false, true, false, null, false, "", null, null, null);

        var forum = this.Forums.GetById(id);
        forum.Name.Should().Be("New forum");
        forum.ParentID.Should().BeNull("a parent ID of 0 means no parent");
        forum.RemoteURL.Should().BeNull("an empty remote URL is stored as null");
        forum.ForumFlags.IsLocked.Should().BeTrue();
        forum.ForumFlags.IsTest.Should().BeTrue();
        forum.ForumFlags.IsHidden.Should().BeFalse();

        this.Forums.Save(
                id, this.category, null, "Renamed", "desc", 1, false, true, false, true, 5, true, "https://example.com", null, null, null)
            .Should().Be(id);

        forum = this.Forums.GetById(id);
        forum.Name.Should().Be("Renamed");
        forum.ForumFlags.IsHidden.Should().BeTrue();
        forum.ForumFlags.IsModerated.Should().BeTrue();
        forum.ModeratedPostCount.Should().Be(5);
        forum.IsModeratedNewTopicOnly.Should().BeTrue();
        forum.RemoteURL.Should().Be("https://example.com");
    }

    [Test]
    [Description("A forum moved to another category takes its sub forums (and their sub forums) along")]
    public void Save_Moves_Sub_Forums_To_The_New_Category_Test()
    {
        var parent = this.db.AddForum("Parent", this.category);
        var child = this.db.AddForum("Child", this.category, f => f.ParentID = parent);
        var grandChild = this.db.AddForum("Grand child", this.category, f => f.ParentID = child);
        var other = this.db.AddForum("Other", this.category);
        var newCategory = this.db.AddCategory("New category");

        this.Forums.Save(
            parent, newCategory, null, "Parent", "desc", 1, false, false, false, false, null, false, "", null, null, null);

        this.Forums.GetById(parent).CategoryID.Should().Be(newCategory);
        this.Forums.GetById(child).CategoryID.Should().Be(newCategory, "the sub forum is moved with its parent");
        this.Forums.GetById(grandChild).CategoryID.Should().Be(newCategory, "also the sub forum of the sub forum");
        this.Forums.GetById(other).CategoryID.Should().Be(this.category, "other forums stay");
    }

    [Test]
    [Description("IsParentsChecker detects forums with sub forums")]
    public void IsParentsChecker_Test()
    {
        var parent = this.db.AddForum("Parent", this.category);
        this.db.AddForum("Child", this.category, f => f.ParentID = parent);

        this.Forums.IsParentsChecker(parent).Should().BeTrue();
        this.Forums.IsParentsChecker(this.db.AddForum("Leaf", this.category)).Should().BeFalse();
    }

    [Test]
    [Description("A forum can't be moved below itself or one of its sub forums, a forum with sub forums can have a parent")]
    public void IsOwnSubForum_Test()
    {
        var top = this.db.AddForum("Top", this.category);
        var middle = this.db.AddForum("Middle", this.category, f => f.ParentID = top);
        var bottom = this.db.AddForum("Bottom", this.category, f => f.ParentID = middle);
        var other = this.db.AddForum("Other", this.category);

        this.Forums.IsOwnSubForum(middle, middle).Should().BeTrue("the forum itself");
        this.Forums.IsOwnSubForum(middle, bottom).Should().BeTrue("a sub forum");
        this.Forums.IsOwnSubForum(top, bottom).Should().BeTrue("a sub forum of a sub forum");

        this.Forums.IsOwnSubForum(middle, top).Should().BeFalse("the forum keeps its parent (and its sub forums)");
        this.Forums.IsOwnSubForum(middle, other).Should().BeFalse("another forum");
        this.Forums.IsOwnSubForum(other, bottom).Should().BeFalse();
    }

    [Test]
    [Description("ListAll returns every category, also without forums, sorted")]
    public void ListAll_Test()
    {
        var b = this.db.AddForum("B", this.category, f => f.SortOrder = 2);
        var a = this.db.AddForum("A", this.category, f => f.SortOrder = 1);
        var emptyCategory = this.db.AddCategory("Empty", c => c.SortOrder = 5);

        var list = this.Forums.ListAll(TestDatabase.BoardId);

        list.Where(t => t.Item1.ID == this.category).Select(t => t.Item2.ID).Should().Equal(a, b);
        list.Should().Contain(t => t.Item1.ID == emptyCategory);
    }

    [Test]
    [Description("ListAllWithAccess skips unreadable forums, link forums and inactive categories")]
    public void ListAllWithAccess_Test()
    {
        var readable = this.db.AddForum("Readable", this.category);
        var notGranted = this.db.AddForum("Not granted", this.category);
        var link = this.db.AddForum("Link", this.category, f => f.RemoteURL = "https://example.com");
        var inactiveCategory = this.db.AddCategory("Inactive", c => c.Flags = 0);
        var inInactive = this.db.AddForum("In inactive category", inactiveCategory);

        this.board.GrantAccess(this.alice, readable);
        this.board.GrantAccess(this.alice, link);
        this.board.GrantAccess(this.alice, inInactive);

        this.Forums.ListAllWithAccess(TestDatabase.BoardId, this.alice).Select(t => t.Item1.ID)
            .Should().Equal(readable).And.NotContain(notGranted);
    }

    [Test]
    [Description("The forum jump lists group forums by category and can search, filter and page")]
    public void ListAllSorted_Test()
    {
        var general = this.db.AddForum("General chat", this.category, f => f.SortOrder = 1);
        var parent = this.db.AddForum("Support", this.category, f => f.SortOrder = 2);
        var child = this.db.AddForum("Support <old>", this.category, f =>
        {
            f.ParentID = parent;
            f.SortOrder = 3;
        });

        this.board.GrantAccess(this.alice, general);
        this.board.GrantAccess(this.alice, parent);
        this.board.GrantAccess(this.alice, child);

        var groups = this.Forums.ListAllSorted(TestDatabase.BoardId, this.alice, "support");

        var group = groups.Should().ContainSingle().Subject;
        group.id.Should().Be(-this.category);
        group.text.Should().Be("General");
        group.children.Select(c => c.text).Should().Equal("Support", " - Support &lt;old&gt;");
        group.children.First().url.Should().Be($"/forum/{parent}");

        this.Forums.ListAllSorted(TestDatabase.BoardId, this.alice, general).Single().children.Single().id
            .Should().Be(general.ToString());

        this.Forums.ListAllSorted(TestDatabase.BoardId, this.alice, 1, 2, out var pager).Single().children.Single().id
            .Should().Be(child.ToString());
        pager.PageSize.Should().Be(2);
    }

    [Test]
    [Description("The forum jump shows the texts as html: the names of the categories are encoded like the names of the forums")]
    public void ListAllSorted_Encodes_Category_Name_Test()
    {
        var category = this.db.AddCategory("<img src=x onerror=alert(1)>");
        var forum = this.db.AddForum("Forum", category);

        this.board.GrantAccess(this.bob, forum);

        this.Forums.ListAllSorted(TestDatabase.BoardId, this.bob, 0, 20, out _).Single().text
            .Should().Be("&lt;img src=x onerror=alert(1)&gt;");
    }

    [Test]
    [Description("ListAllFromCategory returns the top level forums of the category")]
    public void ListAllFromCategory_Test()
    {
        var top = this.db.AddForum("Top", this.category);
        this.db.AddForum("Sub", this.category, f => f.ParentID = top);

        this.Forums.ListAllFromCategory(this.category).Select(f => f.ForumID).Should().Equal(top);
    }

    [Test]
    [Description("The forum index lists readable forums with their last post")]
    public async Task ListReadAsync_Test()
    {
        var withPosts = this.db.AddForum("With posts", this.category, f => f.SortOrder = 1);
        var empty = this.db.AddForum("Empty", this.category, f => f.SortOrder = 2);
        var hidden = this.db.AddForum("Hidden", this.category, f => f.SortOrder = 3);

        this.board.GrantAccess(this.alice, withPosts);
        this.board.GrantAccess(this.alice, empty);

        var topic = this.db.AddTopic(withPosts, this.bob, "Latest topic");
        this.db.AddMessage(topic, this.bob, posted: this.now.AddMinutes(-5));
        this.Forums.UpdateLastPost(withPosts);
        this.Forums.UpdateStats(withPosts);

        var forums = await this.Forums.ListReadAsync(TestDatabase.BoardId, this.alice, null, null, false, 0, 20);

        forums.Select(f => f.ForumID).Should().Equal(withPosts, empty).And.NotContain(hidden);

        var row = forums[0];
        row.Category.Should().Be("General");
        row.Forum.Should().Be("With posts");
        row.Topics.Should().Be(1);
        row.Posts.Should().Be(1);
        row.LastTopicID.Should().Be(topic);
        row.LastTopicName.Should().Be("Latest topic");
        row.LastUser.Should().Be("Bob");
        row.ReadAccess.Should().BeTrue();
        row.Total.Should().Be(2);

        forums[1].LastPosted.Should().BeNull();
    }

    [Test]
    [Description("The forum index can be filtered by category and parent forum")]
    public async Task ListReadAsync_Filter_Test()
    {
        var otherCategory = this.db.AddCategory("Other");
        var parent = this.db.AddForum("Parent", this.category);
        var child = this.db.AddForum("Child", this.category, f => f.ParentID = parent);
        var other = this.db.AddForum("Other forum", otherCategory);

        this.board.GrantAccess(this.alice, parent);
        this.board.GrantAccess(this.alice, child);
        this.board.GrantAccess(this.alice, other);

        (await this.Forums.ListReadAsync(TestDatabase.BoardId, this.alice, otherCategory, null, false, 0, 20))
            .Select(f => f.ForumID).Should().Equal(other);

        var children = await this.Forums.ListReadAsync(TestDatabase.BoardId, this.alice, null, parent, false, 0, 20);

        children.Select(f => f.ForumID).Should().Equal(child);

        (await this.Forums.ListReadAsync(TestDatabase.BoardId, this.alice, null, null, false, 0, 20))
            .Single(f => f.ForumID == parent).SubForums.Should().Be(1);
    }

    [Test]
    [Description("UpdateStats counts posts and topics of the forum and its sub forums, skipping deleted topics")]
    public void UpdateStats_Test()
    {
        var parent = this.db.AddForum("Parent", this.category);
        var child = this.db.AddForum("Child", this.category, f => f.ParentID = parent);

        var t1 = this.db.AddTopic(parent, this.bob);
        this.db.AddMessage(t1, this.bob);
        this.db.AddMessage(t1, this.alice);

        var t2 = this.db.AddTopic(child, this.bob);
        this.db.AddMessage(t2, this.bob);

        var deleted = this.db.AddTopic(parent, this.bob, configure: t => t.Flags = new TopicFlags { IsDeleted = true }.BitValue);
        this.db.AddMessage(deleted, this.bob);

        this.Forums.UpdateStats(parent);

        var forum = this.Forums.GetById(parent);
        forum.NumTopics.Should().Be(2);
        forum.NumPosts.Should().Be(3);
    }

    [Test]
    [Description("UpdateStats sets zero for a forum without topics")]
    public void UpdateStats_Empty_Forum_Test()
    {
        var forum = this.db.AddForum("Empty", this.category, f =>
        {
            f.NumPosts = 7;
            f.NumTopics = 2;
        });

        this.Forums.UpdateStats(forum);

        this.Forums.GetById(forum).NumPosts.Should().Be(0);
        this.Forums.GetById(forum).NumTopics.Should().Be(0);
    }

    [Test]
    [Description("UpdateLastPost picks the newest approved post and clears the columns when there is none")]
    public void UpdateLastPost_Test()
    {
        var forum = this.db.AddForum("Forum", this.category);
        var topic = this.db.AddTopic(forum, this.bob);
        var older = this.db.AddMessage(topic, this.bob, posted: this.now.AddMinutes(-10));
        this.db.AddMessage(
            topic,
            this.alice,
            posted: this.now.AddMinutes(-1),
            configure: m => m.Flags = new MessageFlags { IsApproved = false }.BitValue);

        this.Forums.UpdateLastPost(forum);

        var saved = this.Forums.GetById(forum);
        saved.LastMessageID.Should().Be(older);
        saved.LastTopicID.Should().Be(topic);
        saved.LastUserID.Should().Be(this.bob);

        this.Repository<Message>().UpdateOnly(() => new Message { Flags = 8 | 16 }, m => m.ID == older);

        this.Forums.UpdateLastPost(forum);

        this.Forums.GetById(forum).LastMessageID.Should().BeNull();
    }

    [Test]
    [Description("ModerateList lists forums with unapproved or reported posts the user moderates")]
    public void ModerateList_Test()
    {
        var withUnapproved = this.db.AddForum("Unapproved", this.category);
        var withReported = this.db.AddForum("Reported", this.category);
        var clean = this.db.AddForum("Clean", this.category);
        var notModerated = this.db.AddForum("Not moderated", this.category);

        var moderator = TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess;
        this.board.GrantAccess(this.alice, withUnapproved, moderator);
        this.board.GrantAccess(this.alice, withReported, moderator);
        this.board.GrantAccess(this.alice, clean, moderator);
        this.board.GrantAccess(this.alice, notModerated);

        var t1 = this.db.AddTopic(withUnapproved, this.bob);
        this.db.AddMessage(t1, this.bob, configure: m => m.Flags = new MessageFlags { IsApproved = false }.BitValue);
        this.db.AddMessage(t1, this.bob, configure: m => m.Flags = new MessageFlags { IsApproved = false }.BitValue);

        var t2 = this.db.AddTopic(withReported, this.bob);
        this.db.AddMessage(t2, this.bob, configure: m => m.Flags = new MessageFlags { IsApproved = true, IsReported = true }.BitValue);

        var t3 = this.db.AddTopic(clean, this.bob);
        this.db.AddMessage(t3, this.bob);

        var t4 = this.db.AddTopic(notModerated, this.bob);
        this.db.AddMessage(t4, this.bob, configure: m => m.Flags = new MessageFlags { IsApproved = false }.BitValue);

        var list = this.Forums.ModerateList(this.alice, TestDatabase.BoardId);

        list.Select(f => f.ForumID).Should().BeEquivalentTo([withUnapproved, withReported]);
        list.Single(f => f.ForumID == withUnapproved).MessageCount.Should().Be(2);
        list.Single(f => f.ForumID == withReported).ReportedCount.Should().Be(1);
    }

    [Test]
    [Description("Deleting a forum removes its topics and access rights and re-parents sub forums")]
    public async Task DeleteAsync_Test()
    {
        var forum = this.db.AddForum("To delete", this.category);
        var sub = this.db.AddForum("Sub", this.category, f => f.ParentID = forum);
        var other = this.db.AddForum("Other", this.category);
        this.board.GrantAccess(this.alice, forum);

        var topic = this.db.AddTopic(forum, this.bob);
        this.db.AddMessage(topic, this.bob);

        (await this.Forums.DeleteAsync(forum)).Should().BeTrue();

        (await this.Forums.GetByIdAsync(forum)).Should().BeNull();
        (await this.Forums.GetByIdAsync(sub)).ParentID.Should().BeNull();
        (await this.Forums.GetByIdAsync(other)).Should().NotBeNull();
        (await this.Repository<Topic>().GetByIdAsync(topic)).Should().BeNull();
        (await this.Repository<Message>().CountAsync(m => m.TopicID == topic)).Should().Be(0);
        (await this.Repository<ForumAccess>().CountAsync(a => a.ForumID == forum)).Should().Be(0);
    }

    [Test]
    [Description("Deleting a forum doesn't touch topics of other forums")]
    public async Task DeleteAsync_Leaves_Other_Topics_Alone_Test()
    {
        var other = this.db.AddForum("Other", this.category);
        var forum = this.db.AddForum("To delete", this.category);

        // a topic in the other forum that happens to have the same ID as the forum we delete
        var otherTopic = 0;
        while (otherTopic < forum)
        {
            otherTopic = this.db.AddTopic(other, this.bob);
        }

        otherTopic.Should().Be(forum);

        var lastMessage = this.db.AddMessage(otherTopic, this.bob);

        await this.Forums.DeleteAsync(forum);

        (await this.Repository<Topic>().GetByIdAsync(otherTopic)).LastMessageID.Should().Be(lastMessage);
    }

    [Test]
    [Description("Moving a forum moves its topics and user access, then removes it")]
    public async Task MoveAsync_Test()
    {
        var source = this.db.AddForum("Source", this.category);
        var target = this.db.AddForum("Target", this.category);

        var topic = this.db.AddTopic(source, this.bob);
        this.db.AddMessage(topic, this.bob);

        using (var connection = this.db.Open())
        {
            connection.Insert(new UserForum { UserID = this.alice, ForumID = source, AccessMaskID = 1, Accepted = true, Invited = this.now });
            connection.Insert(new WatchForum { UserID = this.alice, ForumID = source, Created = this.now });
        }

        (await this.Forums.MoveAsync(source, target)).Should().BeTrue();

        (await this.Forums.GetByIdAsync(source)).Should().BeNull();
        (await this.Repository<Topic>().GetByIdAsync(topic)).ForumID.Should().Be(target);
        (await this.Repository<UserForum>().GetSingleAsync(u => u.UserID == this.alice)).ForumID.Should().Be(target);
        (await this.Repository<WatchForum>().CountAsync(w => w.ForumID == source)).Should().Be(0);
    }

    [Test]
    [Description("The links of moved topics to topics of the new forum are removed, other links are moved")]
    public async Task MoveAsync_Moved_Topic_Links_Test()
    {
        var source = this.db.AddForum("Source", this.category);
        var target = this.db.AddForum("Target", this.category);
        var other = this.db.AddForum("Other", this.category);

        var inTarget = this.db.AddTopic(target, this.bob, "In target");
        var inOther = this.db.AddTopic(other, this.bob, "In other");

        var linkToTarget = this.db.AddTopic(source, this.bob, "In target", t => t.TopicMovedID = inTarget);
        var linkToOther = this.db.AddTopic(source, this.bob, "In other", t => t.TopicMovedID = inOther);

        (await this.Forums.MoveAsync(source, target)).Should().BeTrue();

        (await this.Repository<Topic>().GetByIdAsync(linkToTarget)).Should().BeNull("the link would be next to the topic");
        (await this.Repository<Topic>().GetByIdAsync(linkToOther)).ForumID.Should().Be(target);
        (await this.Repository<Topic>().GetByIdAsync(inTarget)).Should().NotBeNull();
    }

    [Test]
    [Description("A user with an own access mask in both forums keeps the one of the new forum")]
    public async Task MoveAsync_User_Access_In_Both_Forums_Test()
    {
        var source = this.db.AddForum("Source", this.category);
        var target = this.db.AddForum("Target", this.category);

        using (var connection = this.db.Open())
        {
            connection.Insert(new UserForum { UserID = this.alice, ForumID = source, AccessMaskID = 1, Accepted = true, Invited = this.now });
            connection.Insert(new UserForum { UserID = this.alice, ForumID = target, AccessMaskID = 2, Accepted = true, Invited = this.now });
            connection.Insert(new UserForum { UserID = this.bob, ForumID = source, AccessMaskID = 1, Accepted = true, Invited = this.now });
        }

        (await this.Forums.MoveAsync(source, target)).Should().BeTrue();

        (await this.Forums.GetByIdAsync(source)).Should().BeNull();
        (await this.Repository<UserForum>().GetSingleAsync(u => u.UserID == this.alice)).AccessMaskID.Should().Be(2);
        (await this.Repository<UserForum>().GetSingleAsync(u => u.UserID == this.bob)).ForumID.Should().Be(target);
    }

    [Test]
    [Description("ReOrderAllAscending and ReOrderAllDescending sort by name")]
    public void ReOrder_Test()
    {
        var c = this.db.AddForum("Charlie", this.category);
        var a = this.db.AddForum("Alpha", this.category);
        var b = this.db.AddForum("Bravo", this.category);

        var forums = this.Forums.Get(f => f.CategoryID == this.category);

        this.Forums.ReOrderAllAscending(forums);
        this.SortedNames().Should().Equal("Alpha", "Bravo", "Charlie");

        this.Forums.ReOrderAllDescending(forums);
        this.SortedNames().Should().Equal("Charlie", "Bravo", "Alpha");

        new[] { a, b, c }.Should().OnlyContain(id => this.Forums.GetById(id) != null);
    }

    private IRepository<Forum> Forums => this.board.Current.GetRepository<Forum>();

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }

    private List<string> SortedNames()
    {
        return [.. this.Forums.Get(f => f.CategoryID == this.category).OrderBy(f => f.SortOrder).Select(f => f.Name)];
    }
}
