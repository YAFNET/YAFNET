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

namespace YAF.Tests.Services;

using NSubstitute;

using YAF.Core.Extensions;
using YAF.Core.Tasks;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The background tasks of the admin: deleting a forum (with or without moving its topics) and pruning topics.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ForumTasksTests
{
    private TestBoardContext board;

    private TestDatabase db;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;
        this.board.BoardSettings.BoardId = TestDatabase.BoardId;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    /// <summary>
    /// A logger that fails the test with the exception of the task (the task catches it).
    /// </summary>
    private ILoggerService Logger()
    {
        var logger = Substitute.For<ILoggerService>();

        logger.When(l => l.Log(Arg.Any<string>(), Arg.Any<EventLogTypes>(), Arg.Any<int?>(), Arg.Any<string>(), Arg.Any<Exception>()))
            .Do(c => c.ArgAt<Exception>(4).Should().BeNull(c.ArgAt<string>(0)));

        return logger;
    }

    [Test]
    [Description("The forum delete task deletes the forum with its topics")]
    public void Forum_Delete_Task_Test()
    {
        var user = this.db.AddUser("Author");
        var forum = this.db.AddForum("Old");
        var topic = this.db.AddTopic(forum, user);
        this.db.AddMessage(topic, user);

        new ForumDeleteTask
            {
                ServiceLocator = this.board.Resolve<IServiceLocator>(),
                Logger = this.Logger(),
                Data = TestDatabase.BoardId,
                ForumId = forum,
                ForumNewId = -1
            }.RunOnce();

        this.board.Current.GetRepository<Forum>().GetById(forum).Should().BeNull();
        this.board.Current.GetRepository<Topic>().GetById(topic).Should().BeNull();
    }

    [Test]
    [Description("The forum delete task moves the topics to the new forum before the forum is deleted")]
    public void Forum_Delete_Task_Move_Test()
    {
        var user = this.db.AddUser("Author");
        var forum = this.db.AddForum("Old");
        var target = this.db.AddForum("New");
        var topic = this.db.AddTopic(forum, user);
        this.db.AddMessage(topic, user);

        new ForumDeleteTask
            {
                ServiceLocator = this.board.Resolve<IServiceLocator>(),
                Logger = this.Logger(),
                Data = TestDatabase.BoardId,
                ForumId = forum,
                ForumNewId = target
            }.RunOnce();

        this.board.Current.GetRepository<Forum>().GetById(forum).Should().BeNull();
        this.board.Current.GetRepository<Topic>().GetById(topic).ForumID.Should().Be(target);
    }

    [Test]
    [Description("The prune task removes the old topics of the forum")]
    public void Prune_Topic_Task_Test()
    {
        var user = this.db.AddUser("Author");
        var forum = this.db.AddForum("Old");
        var old = this.db.AddTopic(forum, user, configure: t => t.LastPosted = DateTime.UtcNow.AddDays(-100));
        var recent = this.db.AddTopic(forum, user);

        new PruneTopicTask
            {
                ServiceLocator = this.board.Resolve<IServiceLocator>(),
                Logger = this.Logger(),
                Data = TestDatabase.BoardId,
                ForumId = forum,
                Days = 30,
                PermDelete = true
            }.RunOnce();

        this.board.Current.GetRepository<Topic>().GetById(old).Should().BeNull();
        this.board.Current.GetRepository<Topic>().GetById(recent).Should().NotBeNull();
    }
}
