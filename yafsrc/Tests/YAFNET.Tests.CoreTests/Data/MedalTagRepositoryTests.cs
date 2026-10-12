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
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Medal and topic tag repositories.
/// </summary>
[TestFixture]
[NonParallelizable]
public class MedalTagRepositoryTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.topic = this.db.AddTopic(this.db.AddForum(), this.alice);

        this.board.Localization.Add("COMMON", "COMMON_WORDS", "the,and");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Save inserts a medal for null or 0 and updates an existing one")]
    public void Medal_Save_Test()
    {
        this.Medals.Save(null, "Gold", "desc", "msg", "cat", "gold.png", 1);
        this.Medals.Save(0, "Silver", "desc", "msg", "cat", "silver.png", 1);

        this.Medals.Count(_ => true).Should().Be(2, "an id of 0 means a new medal");

        var gold = this.Medals.GetSingle(m => m.Name == "Gold");
        gold.BoardID.Should().Be(TestDatabase.BoardId);

        this.Medals.Save(gold.ID, "Platinum", "new desc", "msg", "cat", "p.png", 3);

        var updated = this.Medals.GetById(gold.ID);
        updated.Name.Should().Be("Platinum");
        updated.Flags.Should().Be(3);
        this.Medals.Count(_ => true).Should().Be(2);
    }

    [Test]
    [Description("A user's medals include personal and group medals, the award message overrides the medal message")]
    public void Medal_ListUserMedals_Test()
    {
        var personal = this.AddMedal("Personal", MedalFlags.Flags.AllowHiding);
        var groupMedal = this.AddMedal("Group", MedalFlags.Flags.None);
        var groupId = this.db.AddForumAccess(this.alice, this.db.AddForum());

        using (var connection = this.db.Open())
        {
            connection.Insert(new UserMedal { UserID = this.alice, MedalID = personal, Message = "well done", Hide = true, SortOrder = 1, DateAwarded = DateTime.UtcNow });
            connection.Insert(new GroupMedal { GroupID = groupId, MedalID = groupMedal, Hide = true, SortOrder = 2 });
        }

        var medals = this.Medals.ListUserMedals(this.alice);

        medals.Should().HaveCount(2);

        var p = medals.Single(m => m.MedalID == personal);
        p.Message.Should().Be("well done");
        p.Hide.Should().BeTrue("the medal allows hiding");

        var g = medals.Single(m => m.MedalID == groupMedal);
        g.Message.Should().Be("Group message", "without an award message the medal message is used");
        g.Hide.Should().BeFalse("a medal without AllowHiding can never be hidden");
    }

    [Test]
    [Description("A user without medals gets an empty list")]
    public void Medal_ListUserMedals_None_Test()
    {
        this.AddMedal("Unawarded", MedalFlags.Flags.None);

        this.Medals.ListUserMedals(this.alice).Should().BeEmpty();
    }

    [Test]
    [Description("Tags are created once per board and reused, html is stripped")]
    public async Task AddTagsToTopic_Test()
    {
        await this.TopicTags.AddTagsToTopicAsync("csharp,<b>sql</b>", this.topic);

        var otherTopic = this.db.AddTopic(this.db.AddForum(), this.alice);
        await this.TopicTags.AddTagsToTopicAsync("csharp", otherTopic);

        this.Repository<Tag>().Get(t => true).Select(t => t.TagName).Should().BeEquivalentTo(["csharp", "sql"]);
        this.TopicTags.ListAsDelimitedString(this.topic).Split(',').Should().BeEquivalentTo(["csharp", "sql"]);
        this.TopicTags.List(otherTopic).Should().ContainSingle().Which.Item2.TagName.Should().Be("csharp");
    }

    [Test]
    [Description("Empty input, html-only tags and common words add nothing")]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("<br/>")]
    [TestCase("the,and")]
    public async Task AddTagsToTopic_Ignored_Test(string tags)
    {
        await this.TopicTags.AddTagsToTopicAsync(tags, this.topic);

        this.Repository<Tag>().Count(_ => true).Should().Be(0);
        this.TopicTags.Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("ListAll groups the tags of every topic")]
    public async Task ListAll_Test()
    {
        var otherTopic = this.db.AddTopic(this.db.AddForum(), this.alice);
        await this.TopicTags.AddTagsToTopicAsync("a,b", this.topic);
        await this.TopicTags.AddTagsToTopicAsync("c", otherTopic);

        var all = this.TopicTags.ListAll();

        all.Should().HaveCount(2);
        all.Single(t => t.ID == this.topic).TagName.Split(',').Should().BeEquivalentTo(["a", "b"]);
        all.Single(t => t.ID == otherTopic).TagName.Should().Be("c");
    }

    [Test]
    [Description("A topic without tags has an empty tag string")]
    public void ListAsDelimitedString_No_Tags_Test()
    {
        this.TopicTags.ListAsDelimitedString(this.topic).Should().BeEmpty();
    }

    private IRepository<Medal> Medals => this.Repository<Medal>();

    private IRepository<TopicTag> TopicTags => this.Repository<TopicTag>();

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }

    private int AddMedal(string name, MedalFlags.Flags flags)
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(
            new Medal
            {
                BoardID = TestDatabase.BoardId,
                Name = name,
                Message = $"{name} message",
                Category = "cat",
                MedalURL = $"{name}.png",
                Flags = (int)flags
            },
            selectIdentity: true);
    }
}
