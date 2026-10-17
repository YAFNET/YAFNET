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
/// Poll and category repositories.
/// </summary>
[TestFixture]
[NonParallelizable]
public class PollCategoryRepositoryTests
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
    [Description("Create and update a poll with its flags")]
    public void Poll_Create_Update_Test()
    {
        var closes = new DateTime(2030, 1, 1);
        var pollId = this.Polls.Create(this.alice, "Best color?", closes, true, false, true, "q.png");

        var poll = this.Polls.GetById(pollId);
        poll.Question.Should().Be("Best color?");
        poll.Closes.Should().Be(closes);
        poll.PollFlags.IsClosedBound.Should().BeTrue();
        poll.PollFlags.AllowMultipleChoice.Should().BeFalse();
        poll.PollFlags.ShowVoters.Should().BeTrue();

        this.Polls.Update(pollId, "Best colour?", null, false, true, false, null);

        poll = this.Polls.GetById(pollId);
        poll.Question.Should().Be("Best colour?");
        poll.Closes.Should().BeNull();
        poll.PollFlags.AllowMultipleChoice.Should().BeTrue();
        poll.PollFlags.IsClosedBound.Should().BeFalse();
    }

    [Test]
    [Description("A poll is loaded with its choices")]
    public void Poll_Choices_Test()
    {
        var pollId = this.Polls.Create(this.alice, "Q", null, false, false, false, null);
        this.AddChoice(pollId, "Red");
        this.AddChoice(pollId, "Blue");

        this.Polls.GetPollAndChoices(pollId).Select(x => x.Item2.ChoiceName).Should().BeEquivalentTo(["Red", "Blue"]);
        this.Polls.GetPollAndChoices(99999).Should().BeEmpty();
    }

    [Test]
    [Description("Removing a poll removes its choices and votes and detaches it from the topic")]
    public void Poll_Remove_Test()
    {
        var pollId = this.Polls.Create(this.alice, "Q", null, false, false, false, null);
        var otherPoll = this.Polls.Create(this.alice, "Other", null, false, false, false, null);
        var choice = this.AddChoice(pollId, "Red");
        this.AddChoice(otherPoll, "Green");

        using (var connection = this.db.Open())
        {
            connection.Insert(new PollVote { PollID = pollId, UserID = this.alice, ChoiceID = choice });
        }

        var topic = this.db.AddTopic(this.db.AddForum(), this.alice, configure: t => t.PollID = pollId);

        this.Polls.Remove(pollId);

        this.Polls.GetById(pollId).Should().BeNull();
        this.Repository<Choice>().Count(c => c.PollID == pollId).Should().Be(0);
        this.Repository<PollVote>().Count(v => v.PollID == pollId).Should().Be(0);
        this.Repository<Topic>().GetById(topic).PollID.Should().BeNull();

        this.Repository<Choice>().Count(c => c.PollID == otherPoll).Should().Be(1, "other polls are kept");
    }

    [Test]
    [Description("A vote is counted once per user and poll, and only for a choice of the poll")]
    public void Poll_Vote_Test()
    {
        var pollId = this.Polls.Create(this.alice, "Q", null, false, false, false, null);
        var otherPoll = this.Polls.Create(this.alice, "Other", null, false, false, false, null);
        var red = this.AddChoice(pollId, "Red");
        var blue = this.AddChoice(pollId, "Blue");
        var green = this.AddChoice(otherPoll, "Green");

        var votes = this.Repository<PollVote>();

        votes.Vote(green, this.alice, pollId).Should().BeFalse("the choice belongs to another poll");
        votes.Vote(red, this.alice, pollId).Should().BeTrue();
        votes.Vote(red, this.alice, pollId).Should().BeFalse("the user voted already");
        votes.Vote(blue, this.alice, pollId).Should().BeFalse("the user voted already");

        this.Repository<Choice>().GetById(red).Votes.Should().Be(1);
        this.Repository<Choice>().GetById(blue).Votes.Should().Be(0);
        this.Repository<Choice>().GetById(green).Votes.Should().Be(0);
        votes.Count(v => v.PollID == pollId).Should().Be(1);
    }

    [Test]
    [Description("Categories: save (insert and update), list and the highest sort order")]
    public void Category_Save_List_Test()
    {
        this.Categories.GetHighestSortOrder().Should().Be(0, "no categories yet");

        var id = this.Categories.Save(null, "News", "news.png", 5, new CategoryFlags { IsActive = true });
        this.Categories.Save(null, "Help", null, 2, new CategoryFlags { IsActive = true });

        this.Categories.GetHighestSortOrder().Should().Be(5);

        this.Categories.Save(id, "Latest News", "news.png", 7, new CategoryFlags { IsActive = false });

        var saved = this.Categories.GetById(id);
        saved.Name.Should().Be("Latest News");
        saved.CategoryFlags.IsActive.Should().BeFalse();

        this.Categories.List(id).Should().ContainSingle();
        this.Categories.List().Select(c => c.Name).Should().Equal("Help", "Latest News");
        this.Categories.List(boardId: TestDatabase.BoardId + 1).Should().BeEmpty();
    }

    [Test]
    [Description("Categories can be sorted by name, ascending and descending")]
    public void Category_ReOrder_Test()
    {
        this.db.AddCategory("Beta");
        this.db.AddCategory("Alpha");
        this.db.AddCategory("Gamma");

        this.Categories.ReOrderAllAscending(this.Categories.List());
        this.Categories.List().OrderBy(c => c.SortOrder).Select(c => c.Name).Should().Equal("Alpha", "Beta", "Gamma");

        this.Categories.ReOrderAllDescending(this.Categories.List());
        this.Categories.List().OrderBy(c => c.SortOrder).Select(c => c.Name).Should().Equal("Gamma", "Beta", "Alpha");
    }

    private IRepository<Poll> Polls => this.Repository<Poll>();

    private IRepository<Category> Categories => this.Repository<Category>();

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }

    private int AddChoice(int pollId, string name)
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(new Choice { PollID = pollId, ChoiceName = name, Votes = 0 }, selectIdentity: true);
    }
}
