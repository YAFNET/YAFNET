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
/// Board repository: creating, saving, statistics and deleting boards.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BoardRepositoryTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Creating a board sets up ranks, access masks, groups, guest and admin user, a forum and a welcome topic")]
    public void Create_Test()
    {
        var boardId = this.CreateBoard(isHostAdmin: false, rolePrefix: "B2 ");

        (this.Repository<Board>().GetById(boardId)).Name.Should().Be("Second");

        (this.Repository<Rank>().Get(r => r.BoardID == boardId)).Select(r => r.Name).Should()
            .BeEquivalentTo(["Administration", "Guest", "Newbie", "Member", "Advanced Member"]);
        (this.Repository<AccessMask>().Count(m => m.BoardID == boardId)).Should().Be(5);
        (this.Repository<Group>().Get(g => g.BoardID == boardId)).Select(g => g.Name).Should()
            .BeEquivalentTo(["B2 Administrators", "Guests", "B2 Registered Users"]);

        var users = this.Repository<User>().Get(u => u.BoardID == boardId);
        var guest = users.Single(u => u.Name == "Guest");
        guest.UserFlags.IsGuest.Should().BeTrue();

        var admin = users.Single(u => u.Name == "admin2");
        admin.ProviderUserKey.Should().Be("key-2");
        admin.UserFlags.IsHostAdmin.Should().BeFalse();
        admin.UserFlags.IsApproved.Should().BeTrue();

        var adminGroup = this.Repository<Group>().GetSingle(g => g.Name == "B2 Administrators");
        (this.Repository<UserGroup>().Get(x => x.UserID == admin.ID)).Select(x => x.GroupID).Should().Equal(adminGroup.ID);

        var forum = this.Repository<Forum>().GetSingle(f => f.Name == "Test Forum");
        (this.Repository<ForumAccess>().Count(a => a.ForumID == forum.ID)).Should().Be(3);

        var welcome = this.Repository<Topic>().GetSingle(t => t.ForumID == forum.ID);
        welcome.TopicName.Should().Contain("Hello from YAF.NET");
        welcome.UserID.Should().Be(admin.ID);
        (this.Repository<Message>().Count(m => m.TopicID == welcome.ID)).Should().Be(1);

        this.board.Events.OfType<UpdateUserStylesEvent>().Should().ContainSingle();
    }

    [Test]
    [Description("A host admin creating a board makes the new admin a host admin too")]
    public void Create_HostAdmin_Test()
    {
        var boardId = this.CreateBoard(isHostAdmin: true);

        (this.Repository<User>().GetSingle(u => u.BoardID == boardId && u.Name == "admin2")).UserFlags.IsHostAdmin
            .Should().BeTrue();
    }

    [Test]
    [Description("The culture and language of a new board belong to that board, not to every board")]
    public void Create_Saves_Culture_For_New_Board_Test()
    {
        var registry = this.Repository<Registry>();
        registry.Save("culture", "en-US");
        registry.Save("language", "english.json");

        var boardId = this.CreateBoard(culture: "de-DE", language: "german.json");

        (registry.GetSingle(r => r.Name == "culture" && r.BoardID == null)).Value
            .Should().Be("en-US", "creating a board must not change the culture of the other boards");
        (registry.GetSingle(r => r.Name == "language" && r.BoardID == null)).Value.Should().Be("english.json");

        (registry.GetSingle(r => r.Name == "culture" && r.BoardID == boardId)).Value.Should().Be("de-DE");
        (registry.GetSingle(r => r.Name == "language" && r.BoardID == boardId)).Value.Should().Be("german.json");
    }

    [Test]
    [Description("Save updates name, description and the board's own culture and language")]
    public void Save_Test()
    {
        this.Repository<Board>().Save(TestDatabase.BoardId, "Renamed", "german.json", "de-DE");

        var saved = this.Repository<Board>().GetById(TestDatabase.BoardId);
        saved.Name.Should().Be("Renamed");

        this.Repository<Registry>().GetSingle(r => r.Name == "culture" && r.BoardID == TestDatabase.BoardId).Value
            .Should().Be("de-DE");
        this.Repository<Registry>().Count(r => r.BoardID == null && r.Name == "culture").Should().Be(0);
    }

    [Test]
    [Description("Stats counts categories, forums, visible topics and posts, and approved members")]
    public void Stats_Test()
    {
        var forum = this.db.AddForum();
        var topic = this.db.AddTopic(forum, this.alice);
        this.db.AddMessage(topic, this.alice);
        this.db.AddMessage(topic, this.alice, configure: m => m.Flags |= (int)MessageFlags.Flags.IsDeleted);

        var deletedTopic = this.db.AddTopic(forum, this.alice, configure: t => t.Flags = (int)TopicFlags.Flags.IsDeleted);
        this.db.AddMessage(deletedTopic, this.alice);

        this.db.AddTopic(forum, this.alice, "empty");

        this.db.AddUser("Unapproved", u => u.Flags = 0);
        this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));
        this.db.AddUser("Deleted", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsDeleted));

        var stats = this.Repository<Board>().Stats(TestDatabase.BoardId);

        stats.Categories.Should().Be(1);
        stats.Forums.Should().Be(1);
        stats.Topics.Should().Be(1, "deleted topics and topics without posts are not counted");
        stats.Posts.Should().Be(1, "deleted messages and messages in deleted topics are not counted");
        stats.Users.Should().Be(1, "only approved, not deleted members count");
    }

    [Test]
    [Description("PostStats counts the posts of the board, other boards and inactive categories are ignored")]
    public void PostStats_Test()
    {
        var forum = this.db.AddForum();
        var topic = this.db.AddTopic(forum, this.alice);
        this.db.AddMessage(topic, this.alice);
        this.db.AddMessage(topic, this.alice);

        var inactiveCategory = this.db.AddCategory(configure: c => c.Flags = 0);
        var hiddenTopic = this.db.AddTopic(this.db.AddForum(categoryId: inactiveCategory), this.alice);
        this.db.AddMessage(hiddenTopic, this.alice);

        var stats = this.Repository<Board>().PostStats(TestDatabase.BoardId, true);

        stats.Posts.Should().Be(2);
        stats.Topics.Should().Be(1);
        stats.Forums.Should().Be(2);
    }

    [Test]
    [Description("PostStats (the public statistics box) does not count deleted posts and topics, like Stats")]
    public void PostStats_Excludes_Deleted_Test()
    {
        var forum = this.db.AddForum();
        var topic = this.db.AddTopic(forum, this.alice);
        this.db.AddMessage(topic, this.alice);
        this.db.AddMessage(topic, this.alice, configure: m => m.Flags |= (int)MessageFlags.Flags.IsDeleted);

        var deletedTopic = this.db.AddTopic(forum, this.alice, configure: t => t.Flags = (int)TopicFlags.Flags.IsDeleted);
        this.db.AddMessage(deletedTopic, this.alice);

        var stats = this.Repository<Board>().PostStats(TestDatabase.BoardId, true);

        stats.Posts.Should().Be(1);
        stats.Topics.Should().Be(1);

        var adminStats = this.Repository<Board>().Stats(TestDatabase.BoardId);
        (adminStats.Posts, adminStats.Topics).Should().Be((stats.Posts, stats.Topics), "the public and admin statistics agree");
    }

    [Test]
    [Description("PostStats without countable posts falls back to Stats")]
    public void PostStats_No_Posts_Test()
    {
        var noCountForum = this.db.AddForum(configure: f => f.Flags = (int)ForumFlags.Flags.IsTest);
        this.db.AddMessage(this.db.AddTopic(noCountForum, this.alice), this.alice);

        var stats = this.Repository<Board>().PostStats(TestDatabase.BoardId, false);

        stats.Should().NotBeNull();
        stats.Users.Should().Be(1, "the fallback statistics include the user count");
    }

    [Test]
    [Description("Deleting a board removes all of its data and leaves the other boards alone")]
    public void DeleteBoard_Test()
    {
        var keptForum = this.db.AddForum();
        this.db.AddMessage(this.db.AddTopic(keptForum, this.alice), this.alice);

        var boardId = this.CreateBoard();
        this.Repository<Registry>().Save("theme", "dark", boardId);

        // the board's data with a foreign key to the board (the delete failed on PostgreSQL, SQL Server and MySQL)
        var boardUser = (this.Repository<User>().GetSingle(u => u.BoardID == boardId)).ID;
        this.Repository<Tag>().Insert(new Tag { BoardID = boardId, TagName = "tag" });
        this.Repository<BannedUserAgent>().Insert(new BannedUserAgent { BoardID = boardId, UserAgent = "bot" });
        var definition = (int)this.Repository<ProfileDefinition>().Insert(
            new ProfileDefinition { BoardID = boardId, Name = "Field", DataType = "Text", DefaultValue = string.Empty },
            true);
        this.Repository<ProfileCustom>().Insert(new ProfileCustom { ProfileDefinitionID = definition, UserID = boardUser, Value = "v" });

        this.Repository<Board>().DeleteBoard(boardId);

        (this.Repository<Tag>().Count(t => t.BoardID == boardId)).Should().Be(0);
        (this.Repository<BannedUserAgent>().Count(b => b.BoardID == boardId)).Should().Be(0);
        (this.Repository<ProfileDefinition>().Count(p => p.BoardID == boardId)).Should().Be(0);

        (this.Repository<Board>().GetById(boardId)).Should().BeNull();
        (this.Repository<User>().Count(u => u.BoardID == boardId)).Should().Be(0);
        (this.Repository<Group>().Count(g => g.BoardID == boardId)).Should().Be(0);
        (this.Repository<Rank>().Count(r => r.BoardID == boardId)).Should().Be(0);
        (this.Repository<AccessMask>().Count(m => m.BoardID == boardId)).Should().Be(0);
        (this.Repository<Category>().Count(c => c.BoardID == boardId)).Should().Be(0);
        (this.Repository<Registry>().Count(r => r.BoardID == boardId)).Should().Be(0);
        (this.Repository<Forum>().Count(f => f.Name == "Test Forum")).Should().Be(0);

        (this.Repository<User>().GetById(this.alice)).Should().NotBeNull();
        (this.Repository<Forum>().GetById(keptForum)).Should().NotBeNull();
        (this.Repository<Message>().Count(m => m.UserID == this.alice)).Should().Be(1);
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }

    private int CreateBoard(
        bool isHostAdmin = false,
        string rolePrefix = "",
        string culture = "en-US",
        string language = "english.json")
    {
        return this.Repository<Board>().Create(
            "Second",
            "board@example.com",
            culture,
            language,
            "admin2",
            "admin2@example.com",
            "key-2",
            isHostAdmin,
            rolePrefix);
    }
}
