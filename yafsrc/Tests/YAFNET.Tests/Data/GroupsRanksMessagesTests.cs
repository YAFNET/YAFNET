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
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Rank, group, user group (roles) and private message repositories.
/// </summary>
[TestFixture]
[NonParallelizable]
public class GroupsRanksMessagesTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int carol;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.carol = this.db.AddUser("Carol");

        this.board.PageUser.ID = this.alice;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Saving a rank only keeps MinPosts for ladder ranks and refreshes styles when needed")]
    public async Task Rank_Save_Test()
    {
        var ranks = this.Repository<Rank>();

        await ranks.SaveAsync(null, TestDatabase.BoardId, "Ladder", new RankFlags { IsLadder = true }, null, null, 2, "d", 100, "b", 1, 2);
        await ranks.SaveAsync(null, TestDatabase.BoardId, "Fixed", new RankFlags(), 50, "color: red", 3, "d", 100, "b", 1, 2);

        ranks.GetSingle(r => r.Name == "Ladder").MinPosts.Should().Be(0, "a ladder rank without MinPosts starts at 0");
        ranks.GetSingle(r => r.Name == "Fixed").MinPosts.Should().BeNull("only ladder ranks use MinPosts");
        this.board.Events.OfType<UpdateUserStylesEvent>().Should().ContainSingle("only the styled rank refreshes styles");

        var id = ranks.GetSingle(r => r.Name == "Ladder").ID;
        await ranks.SaveAsync(id, TestDatabase.BoardId, "Ladder 2", new RankFlags { IsLadder = true }, 25, null, 2, "d", 200, "b", 1, 2);

        var updated = ranks.GetById(id);
        updated.Name.Should().Be("Ladder 2");
        updated.MinPosts.Should().Be(25);
        updated.UsrSigChars.Should().Be(200);
    }

    [Test]
    [Description("Rank lookups for a user")]
    public async Task Rank_User_Lookups_Test()
    {
        var styled = this.db.AddRank("Styled", configure: r => r.Style = "color: blue");
        this.Users.UpdateOnly(() => new User { RankID = styled }, u => u.ID == this.bob);

        var (user, rank) = await this.Repository<Rank>().GetUserAndRankAsync(this.bob);
        user.ID.Should().Be(this.bob);
        rank.Name.Should().Be("Styled");

        (await this.Repository<Rank>().GetRankStyleForUserAsync(this.bob)).Should().Be("color: blue");
        (await this.Repository<Rank>().GetRankStyleForUserAsync(this.alice)).Should().BeNull();
    }

    [Test]
    [Description("A new group gets the given access mask on every forum of the board")]
    public async Task Group_Save_Test()
    {
        var forumA = this.db.AddForum();
        var forumB = this.db.AddForum();
        var maskId = this.InsertMask("Read only", AccessFlags.Flags.ReadAccess);

        var groupId = await this.Repository<Group>().SaveAsync(
            null, TestDatabase.BoardId, "Readers", new GroupFlags(), maskId, null, 5, "desc", 0, null, 0, 0);

        this.Repository<ForumAccess>().Get(a => a.GroupID == groupId).Select(a => (a.ForumID, a.AccessMaskID))
            .Should().BeEquivalentTo([(forumA, maskId), (forumB, maskId)]);

        await this.Repository<Group>().SaveAsync(
            groupId, TestDatabase.BoardId, "Readers", new GroupFlags { IsModerator = true }, maskId, "color: green", 5, "desc", 0, null, 0, 0);

        this.Repository<Group>().GetById(groupId).GroupFlags.IsModerator.Should().BeTrue();
        this.board.Events.OfType<UpdateUserStylesEvent>().Should().ContainSingle();

        this.Repository<Group>().List(groupId, TestDatabase.BoardId).Single().Style.Should().Be("color: green");
        this.Repository<Group>().List(boardId: TestDatabase.BoardId).Should().Contain(g => g.ID == groupId);
    }

    [Test]
    [Description("Group membership: Member lists every group with the user's membership")]
    public void Group_Membership_Test()
    {
        var forum = this.db.AddForum();
        var editors = this.db.AddForumAccess(this.bob, forum);
        var viewers = this.db.AddForumAccess(this.carol, forum);
        var userGroups = this.Repository<UserGroup>();

        userGroups.AddOrRemove(this.alice, editors, true);
        userGroups.AddOrRemove(this.alice, editors, true);

        userGroups.List(this.alice).Select(g => g.ID).Should().Equal(editors);

        var members = this.Repository<Group>().Member(TestDatabase.BoardId, this.alice);
        members.Single(m => m.GroupID == editors).UserID.Should().Be(this.alice);
        members.Single(m => m.GroupID == viewers).UserID.Should().BeNull();

        userGroups.AddOrRemove(this.alice, editors, false);
        userGroups.List(this.alice).Should().BeEmpty();
    }

    [Test]
    [Description("SetRole adds the user to an existing group with the role's name")]
    public void SetRole_Existing_Group_Test()
    {
        var forum = this.db.AddForum();
        var groupId = this.db.AddForumAccess(this.bob, forum);
        var groupName = this.Repository<Group>().GetById(groupId).Name;

        this.Repository<UserGroup>().SetRole(TestDatabase.BoardId, this.alice, groupName);
        this.Repository<UserGroup>().SetRole(TestDatabase.BoardId, this.alice, groupName);

        this.Repository<UserGroup>().List(this.alice).Select(g => g.ID).Should().Equal(groupId);

        this.Repository<UserGroup>().SetRole(TestDatabase.BoardId, this.alice, null);
        this.Repository<UserGroup>().List(this.alice).Should().BeEmpty("an empty role removes all memberships");
    }

    [Test]
    [Description("A role without a group creates one with the same forum access as the start group")]
    public void SetRole_New_Group_Copies_Start_Group_Access_Test()
    {
        var publicForum = this.db.AddForum("Public");
        var staffForum = this.db.AddForum("Staff");

        var readMask = this.InsertMask("Read", AccessFlags.Flags.ReadAccess);
        var fullMask = this.InsertMask("Full", TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        var startGroup = this.Insert(new Group { BoardID = TestDatabase.BoardId, Name = "Registered", Flags = (int)GroupFlags.Flags.IsStart, SortOrder = 1 });
        var staffGroup = this.Insert(new Group { BoardID = TestDatabase.BoardId, Name = "Staff", SortOrder = 2 });

        using (var connection = this.db.Open())
        {
            connection.Insert(new ForumAccess { GroupID = startGroup, ForumID = publicForum, AccessMaskID = readMask });
            connection.Insert(new ForumAccess { GroupID = staffGroup, ForumID = staffForum, AccessMaskID = fullMask });
        }

        this.Repository<UserGroup>().SetRole(TestDatabase.BoardId, this.alice, "Imported Role");

        var newGroup = this.Repository<Group>().GetSingle(g => g.Name == "Imported Role");
        newGroup.Should().NotBeNull();

        this.Repository<UserGroup>().List(this.alice).Select(g => g.ID).Should().Equal(newGroup.ID);

        this.Repository<ForumAccess>().Get(a => a.GroupID == newGroup.ID).Select(a => (a.ForumID, a.AccessMaskID))
            .Should().BeEquivalentTo([(publicForum, readMask)], "the new group must not get access to the staff forum");
    }

    [Test]
    [Description("The group style with the lowest sort order wins")]
    public async Task Group_Style_Test()
    {
        var forum = this.db.AddForum();
        var low = this.db.AddForumAccess(this.alice, forum);
        var high = this.db.AddForumAccess(this.alice, forum);
        this.Repository<Group>().UpdateOnly(() => new Group { Style = "color: red", SortOrder = 1 }, g => g.ID == low);
        this.Repository<Group>().UpdateOnly(() => new Group { Style = "color: blue", SortOrder = 9 }, g => g.ID == high);

        (await this.Repository<UserGroup>().GetGroupStyleForUserAsync(this.alice)).Should().Be("color: red");
        (await this.Repository<UserGroup>().GetGroupStyleForUserAsync(this.bob)).Should().BeNull();
    }

    [Test]
    [Description("A conversation contains both directions, oldest first, and marks received messages read")]
    public async Task PrivateMessage_Conversation_Test()
    {
        var m1 = this.AddPm(this.bob, this.alice, "Hi Alice");
        var m2 = this.AddPm(this.alice, this.bob, "Hi Bob");
        var m3 = this.AddPm(this.bob, this.alice, "How are you?");
        this.AddPm(this.carol, this.alice, "Unrelated");

        var conversation = await this.Messages.GetConversationAsync(this.alice, this.bob);

        conversation.Select(m => m.ID).Should().Equal(m1, m2, m3);
        this.Messages.Get(m => m.ToUserId == this.alice && m.FromUserId == this.bob)
            .Should().OnlyContain(m => (m.Flags & 1) == 1, "received messages are now read");
        this.Messages.GetById(m2).Flags.Should().Be(0, "sent messages are untouched");
        this.board.Events.OfType<UpdateUserEvent>().Should().NotBeEmpty();
    }

    [Test]
    [Description("Deleting a conversation hides it for one side and removes it once both sides deleted it")]
    public async Task PrivateMessage_Delete_Conversation_Test()
    {
        this.AddPm(this.bob, this.alice, "Hi Alice");
        this.AddPm(this.alice, this.bob, "Hi Bob");

        await this.Messages.DeleteConversationAsync(this.alice, this.bob);

        (await this.Messages.GetConversationAsync(this.alice, this.bob)).Should().BeEmpty();
        (await this.Messages.GetConversationAsync(this.bob, this.alice)).Should().HaveCount(2, "Bob still sees it");

        await this.Messages.DeleteConversationAsync(this.bob, this.alice);

        this.Messages.Count(m => true).Should().Be(0);
    }

    [Test]
    [Description("The conversation list contains every partner once, the latest unread sender is found")]
    public async Task PrivateMessage_User_List_Test()
    {
        this.AddPm(this.bob, this.alice, "1");
        this.AddPm(this.alice, this.bob, "2");
        this.AddPm(this.alice, this.carol, "3");
        this.AddPm(this.carol, this.alice, "4");
        this.AddPm(this.bob, this.carol, "not mine");

        (await this.Messages.GetUserListAsync(this.alice)).Select(u => u.ID).Should().Equal(this.bob, this.carol);

        (await this.Messages.GetLatestConversationUserAsync(this.alice)).ID.Should().Be(this.carol);

        await this.Messages.GetConversationAsync(this.alice, this.carol);
        (await this.Messages.GetLatestConversationUserAsync(this.alice)).ID.Should().Be(this.bob);
    }

    [Test]
    [Description("Pruning removes private messages older than the given days")]
    public async Task PrivateMessage_Prune_Test()
    {
        this.AddPm(this.bob, this.alice, "old", DateTime.UtcNow.AddDays(-40));
        var recent = this.AddPm(this.bob, this.alice, "recent", DateTime.UtcNow.AddDays(-1));

        // a message of another board stays
        var otherBoardUser = this.db.AddUser("OtherBoardUser", u => u.BoardID = TestDatabase.BoardId + 1);
        var otherBoard = this.AddPm(otherBoardUser, otherBoardUser, "other board", DateTime.UtcNow.AddDays(-40));

        this.Messages.CountBoardMessages(TestDatabase.BoardId).Should().Be(2);

        (await this.Messages.PruneAllAsync(TestDatabase.BoardId, 30)).Should().Be(1);

        this.Messages.GetAll().Select(m => m.ID).Should().BeEquivalentTo([recent, otherBoard]);

        // 0 days (or less) would delete everything, at least one day is kept
        (await this.Messages.PruneAllAsync(TestDatabase.BoardId, 0)).Should().Be(0);
        this.Messages.GetAll().Select(m => m.ID).Should().Contain(recent);
    }

    private IRepository<User> Users => this.board.Current.GetRepository<User>();

    private IRepository<PrivateMessage> Messages => this.board.Current.GetRepository<PrivateMessage>();

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }

    private int InsertMask(string name, AccessFlags.Flags flags)
    {
        return this.Insert(new AccessMask { BoardID = TestDatabase.BoardId, Name = name, Flags = (int)flags, SortOrder = 1 });
    }

    private int Insert<T>(T entity)
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(entity, selectIdentity: true);
    }

    private int AddPm(int from, int to, string body, DateTime? created = null)
    {
        return this.Insert(new PrivateMessage { FromUserId = from, ToUserId = to, Body = body, Created = created ?? DateTime.UtcNow });
    }
}
