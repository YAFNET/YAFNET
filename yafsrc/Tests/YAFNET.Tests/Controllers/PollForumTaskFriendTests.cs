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

namespace YAF.Tests.Controllers;

using System.Threading.Tasks;

using Autofac;

using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Controllers;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Core.Tasks;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// Removing polls, the forum delete task and the friend actions of the post list.
/// </summary>
[TestFixture]
[NonParallelizable]
public class PollForumTaskFriendTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private ITaskModuleManager tasks;

    private ISessionService session;

    private int alice;

    private int bob;

    private int forum;

    [SetUp]
    public void SetUp()
    {
        this.tasks = Substitute.For<ITaskModuleManager>();
        this.session = Substitute.For<ISessionService>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(this.tasks).As<ITaskModuleManager>();
                b.RegisterInstance(this.session).As<ISessionService>();
                b.RegisterType<Friends>().As<IFriends>();
                b.RegisterInstance(Substitute.For<IUserDisplayName>()).As<IUserDisplayName>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.forum = this.db.AddForum();

        this.board.LoginAs(this.bob);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The poll owner can remove the poll of a topic")]
    public void RemovePoll_Owner_Test()
    {
        var (poll, _) = this.AddPollTopic(this.forum, this.bob);

        this.board.CreateController<PollController>().RemovePoll(poll);

        this.Repository<Poll>().GetById(poll).Should().BeNull();
    }

    [Test]
    [Description("Other users can not remove a poll, unknown polls are rejected")]
    public void RemovePoll_Not_Owner_Test()
    {
        var (poll, _) = this.AddPollTopic(this.forum, this.alice);

        this.board.CreateController<PollController>().RemovePoll(poll);
        this.board.CreateController<PollController>().RemovePoll(99999);

        this.Repository<Poll>().GetById(poll).Should().NotBeNull();
    }

    [Test]
    [Description("A moderator of another forum can not remove polls of a forum they do not moderate")]
    public void RemovePoll_Moderator_Of_Other_Forum_Test()
    {
        var modForum = this.db.AddForum();
        this.board.GrantAccess(this.bob, modForum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        var (poll, _) = this.AddPollTopic(this.forum, this.alice);

        // the request names the forum bob moderates
        this.board.SetPage(modForum);
        this.board.PageLoad.ModeratorAccess = true;

        this.board.CreateController<PollController>().RemovePoll(poll);

        this.Repository<Poll>().GetById(poll).Should().NotBeNull();
    }

    [Test]
    [Description("A moderator of the topic's forum can remove its poll")]
    public void RemovePoll_Moderator_Test()
    {
        this.board.GrantAccess(this.bob, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);
        var (poll, _) = this.AddPollTopic(this.forum, this.alice);

        this.board.SetPage(this.forum);
        this.board.PageLoad.ModeratorAccess = true;

        this.board.CreateController<PollController>().RemovePoll(poll);

        this.Repository<Poll>().GetById(poll).Should().BeNull();
    }

    [Test]
    [Description("The forum delete task deletes the forum, or moves its topics to another forum first")]
    public async Task ForumDeleteTask_Test()
    {
        var target = this.db.AddForum();
        var moved = this.db.AddTopic(this.forum, this.alice);
        var other = this.db.AddForum();
        this.db.AddTopic(other, this.alice);

        await NewTask(this.forum, target).RunOnceAsync();

        (await this.Repository<Forum>().GetByIdAsync(this.forum)).Should().BeNull();
        (await this.Repository<Topic>().GetByIdAsync(moved)).ForumID.Should().Be(target);

        await NewTask(other, -1).RunOnceAsync();

        (await this.Repository<Forum>().GetByIdAsync(other)).Should().BeNull();
        (await this.Repository<Topic>().CountAsync(t => t.ForumID == other)).Should().Be(0);
    }

    [Test]
    [Description("Deleting a forum is refused while another forum delete is running, for both variants")]
    public void ForumDeleteTask_Blocking_Test()
    {
        this.tasks.AreTasksRunning(Arg.Any<string[]>()).Returns(false);
        this.tasks.StartTask(nameof(ForumDeleteTask), Arg.Any<Func<IBackgroundTask>>()).Returns(true);

        ForumDeleteTask.Start(TestDatabase.BoardId, this.forum, out _).Should().BeTrue();
        this.tasks.Received(1).StartTask(nameof(ForumDeleteTask), Arg.Any<Func<IBackgroundTask>>());

        // a finished delete that is not cleaned up yet: the task manager does not start the new one
        this.tasks.StartTask(nameof(ForumDeleteTask), Arg.Any<Func<IBackgroundTask>>()).Returns(false);
        ForumDeleteTask.Start(TestDatabase.BoardId, this.forum, out var notStarted).Should().BeFalse();
        notStarted.Should().Contain("ForumDeleteTask");
        ForumDeleteTask.Start(TestDatabase.BoardId, this.forum, this.db.AddForum(), out _).Should().BeFalse();

        this.tasks.ClearReceivedCalls();
        this.tasks.AreTasksRunning(Arg.Any<string[]>()).Returns(true);

        ForumDeleteTask.Start(TestDatabase.BoardId, this.forum, out var message).Should().BeFalse();
        message.Should().Contain("ForumDeleteTask");

        ForumDeleteTask.Start(TestDatabase.BoardId, this.forum, this.db.AddForum(), out _)
            .Should().BeFalse("moving the topics and deleting is blocked as well");

        this.tasks.DidNotReceive().StartTask(Arg.Any<string>(), Arg.Any<Func<IBackgroundTask>>());
    }

    [Test]
    [Description("Adding and removing the author of a shown post as friend")]
    public void Friend_From_Post_Test()
    {
        var post = this.db.AddMessage(this.db.AddTopic(this.forum, this.alice), this.alice);
        this.session.GetPageData<List<PagedMessage>>().Returns(
            [new PagedMessage { MessageID = post, UserID = this.alice, Topic = "Topic", UserName = "Alice", DisplayName = "Alice" }]);

        var controller = this.board.CreateController<FriendsController>();

        controller.AddFriend(post);
        this.Repository<Buddy>().Count(b => b.FromUserID == this.bob && b.ToUserID == this.alice).Should().Be(1);

        controller.RemoveFriend(post);
        this.Repository<Buddy>().Count(_ => true).Should().Be(0);

        controller.AddFriend(99999);
        this.Repository<Buddy>().Count(_ => true).Should().Be(0, "only shown posts");
    }

    private ForumDeleteTask NewTask(int forumId, int newForumId)
    {
        return new ForumDeleteTask
        {
            ServiceLocator = this.board.Resolve<IServiceLocator>(),
            Logger = NullLogger<BaseBackgroundTask>.Instance,
            Data = TestDatabase.BoardId,
            ForumId = forumId,
            ForumNewId = newForumId
        };
    }

    private (int Poll, int Topic) AddPollTopic(int forumId, int ownerId)
    {
        var poll = this.Repository<Poll>().Create(ownerId, "Q", null, false, false, false, null);
        var topic = this.db.AddTopic(forumId, ownerId, configure: t => t.PollID = poll);

        return (poll, topic);
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
