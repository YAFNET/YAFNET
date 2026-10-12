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

using System.Threading.Tasks;

using Autofac;

using ServiceStack.OrmLite;

using YAF.Core.Events;
using YAF.Core.Events.Cache;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;

/// <summary>
/// The real event raisers (<see cref="ServiceLocatorEventRaiser"/>, <see cref="ServiceLocatorAsyncEventRaiser"/>)
/// and <see cref="ClearCacheOnEvents"/>, which drops cached data and cached access rights when the data changes.
/// </summary>
[TestFixture]
[NonParallelizable]
public class EventsAndCacheClearingTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int forum;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<ServiceLocatorEventRaiser>().As<IRaiseEvent>();
                b.RegisterType<ServiceLocatorAsyncEventRaiser>().As<IRaiseEventAsync>();
                b.RegisterType<ClearCacheOnEvents>().AsImplementedInterfaces();
                b.RegisterType<CountingHandler>().AsSelf().AsImplementedInterfaces().SingleInstance();
                b.RegisterType<FailingHandler>().AsSelf().AsImplementedInterfaces().SingleInstance();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.forum = this.db.AddForum();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Raise calls every handler once, in order")]
    public void Raise_Test()
    {
        this.board.Resolve<IRaiseEvent>().Raise(new TestEvent());

        this.board.Resolve<CountingHandler>().Calls.Should().Be(1);
        this.board.Resolve<FailingHandler>().Calls.Should().Be(1);
    }

    [Test]
    [Description("RaiseIsolated calls every handler once, a failing handler is logged and does not stop the others")]
    public void RaiseIsolated_Test()
    {
        var failures = new List<string>();
        var failing = this.board.Resolve<FailingHandler>();
        failing.Fail = true;

        FluentActions.Invoking(
                () => this.board.Resolve<IRaiseEvent>().RaiseIsolated(new TestEvent(), (name, _) => failures.Add(name)))
            .Should().NotThrow();

        failing.Calls.Should().Be(1);
        this.board.Resolve<CountingHandler>().Calls.Should().Be(1, "handlers run once and after a failing one");
        failures.Should().Equal(nameof(FailingHandler));
    }

    [Test]
    [Description("RaiseIsolatedAsync calls every handler once, a failing handler is logged and does not stop the others")]
    public async Task RaiseIsolatedAsync_Test()
    {
        var failures = new List<string>();
        var failing = this.board.Resolve<FailingHandler>();
        failing.Fail = true;

        await FluentActions.Awaiting(
                () => this.board.Resolve<IRaiseEventAsync>()
                    .RaiseIsolatedAsync(new TestAsyncEvent(), (name, _) => failures.Add(name)))
            .Should().NotThrowAsync();

        failing.AsyncCalls.Should().Be(1);
        this.board.Resolve<CountingHandler>().AsyncCalls.Should().Be(1);
        failures.Should().Equal(nameof(FailingHandler));
    }

    [Test]
    [Description("RaiseAsync calls every handler once")]
    public async Task RaiseAsync_Test()
    {
        await this.board.Resolve<IRaiseEventAsync>().RaiseAsync(new TestAsyncEvent());

        this.board.Resolve<CountingHandler>().AsyncCalls.Should().Be(1);
    }

    [Test]
    [Description("Adding a user to a group drops the cached access rights")]
    public void UserGroup_Added_Clears_Access_Test()
    {
        var group = this.db.AddForumAccess(this.db.AddUser("Bob"), this.forum);
        this.board.RefreshActiveAccess(this.alice);

        this.board.Current.GetRepository<UserGroup>().AddOrRemove(this.alice, group, true);

        this.ActiveAccessCount().Should().Be(0);
    }

    [Test]
    [Description("Removing a user from a group drops the cached access rights, so the access is revoked at once")]
    public void UserGroup_Removed_Clears_Access_Test()
    {
        var group = this.db.AddForumAccess(this.alice, this.forum, groupFlags: GroupFlags.Flags.IsModerator);
        this.board.RefreshActiveAccess(this.alice);
        this.ActiveAccessCount().Should().BeGreaterThan(0);

        this.board.Current.GetRepository<UserGroup>().AddOrRemove(this.alice, group, false);

        this.ActiveAccessCount().Should().Be(0);
    }

    [Test]
    [Description("Changing the access mask of a group in a forum drops the cached access rights")]
    public async Task ForumAccess_Changed_Clears_Access_Test()
    {
        var group = this.db.AddForumAccess(this.alice, this.forum);
        this.board.RefreshActiveAccess(this.alice);

        var readOnly = this.board.Current.GetRepository<AccessMask>().Insert(
            new AccessMask { BoardID = TestDatabase.BoardId, Name = "Read only", Flags = (int)AccessFlags.Flags.ReadAccess, SortOrder = 1 });
        this.board.RefreshActiveAccess(this.alice);

        await this.board.Current.GetRepository<ForumAccess>().SaveAsync(this.forum, group, readOnly);

        this.ActiveAccessCount().Should().Be(0);
    }

    [Test]
    [Description("Changing an access mask drops the cached access rights")]
    public void AccessMask_Changed_Clears_Access_Test()
    {
        this.db.AddForumAccess(this.alice, this.forum);
        this.board.RefreshActiveAccess(this.alice);

        this.board.Current.GetRepository<AccessMask>().Insert(
            new AccessMask { BoardID = TestDatabase.BoardId, Name = "New", Flags = 0, SortOrder = 1 });

        this.ActiveAccessCount().Should().Be(0);
    }

    [Test]
    [Description("Ban, spam word and replace word changes drop their cached lists")]
    public void Ban_Lists_Test()
    {
        this.board.Cache.Set(Constants.Cache.BannedIP, "cached");
        this.board.Cache.Set(Constants.Cache.SpamWords, "cached");
        this.board.Cache.Set(Constants.Cache.ReplaceWords, "cached");

        this.board.Current.GetRepository<BannedIP>().Insert(
            new BannedIP { BoardID = TestDatabase.BoardId, Mask = "1.2.3.4", Since = DateTime.UtcNow });
        this.board.Current.GetRepository<SpamWords>().Insert(new SpamWords { BoardID = TestDatabase.BoardId, SpamWord = "x" });
        this.board.Current.GetRepository<ReplaceWords>().Insert(
            new ReplaceWords { BoardID = TestDatabase.BoardId, BadWord = "a", GoodWord = "b" });

        this.board.Cache.Get(Constants.Cache.BannedIP).Should().BeNull();
        this.board.Cache.Get(Constants.Cache.SpamWords).Should().BeNull();
        this.board.Cache.Get(Constants.Cache.ReplaceWords).Should().BeNull();
    }

    [Test]
    [Description("New posts drop the board statistics, deleting a user clears the whole cache")]
    public void Stats_And_User_Delete_Test()
    {
        this.board.Cache.Set(Constants.Cache.BoardStats, "cached");
        this.board.Cache.Set("SomethingElse", "cached");

        this.board.Current.GetRepository<Message>().Delete(m => m.ID == -1);
        this.board.Cache.Get(Constants.Cache.BoardStats).Should().BeNull();
        this.board.Cache.Get("SomethingElse").Should().NotBeNull();

        this.board.Current.GetRepository<User>().Delete(u => u.ID == -1);
        this.board.Cache.Get("SomethingElse").Should().BeNull();
    }

    [Test]
    [Description("Login and logout refresh the online lists and the user's cached data")]
    public void Login_Logout_Test()
    {
        var lazyKey = string.Format(Constants.Cache.ActiveUserLazyData, this.alice);

        this.board.Cache.Set(Constants.Cache.UsersOnlineStatus, "cached");
        this.board.Cache.Set(lazyKey, "cached");

        this.board.Resolve<IRaiseEvent>().Raise(new SuccessfulUserLoginEvent(this.alice));
        this.board.Cache.Get(Constants.Cache.UsersOnlineStatus).Should().BeNull();

        this.board.Resolve<IRaiseEvent>().Raise(new UserLogoutEvent(this.alice));
        this.board.Cache.Get(lazyKey).Should().BeNull();
    }

    private long ActiveAccessCount()
    {
        return this.board.Current.GetRepository<ActiveAccess>().Count(a => a.UserID == this.alice);
    }

    public class TestEvent : IAmEvent;

    public class TestAsyncEvent : IAmEventAsync;

    public class CountingHandler : IHandleEvent<TestEvent>, IHandleEventAsync<TestAsyncEvent>
    {
        public int Calls { get; private set; }

        public int AsyncCalls { get; private set; }

        public int Order => 20;

        public void Handle(TestEvent @event)
        {
            this.Calls++;
        }

        public Task HandleAsync(TestAsyncEvent @event)
        {
            this.AsyncCalls++;
            return Task.CompletedTask;
        }
    }

    public class FailingHandler : IHandleEvent<TestEvent>, IHandleEventAsync<TestAsyncEvent>
    {
        public bool Fail { get; set; }

        public int Calls { get; private set; }

        public int AsyncCalls { get; private set; }

        public int Order => 10;

        public void Handle(TestEvent @event)
        {
            this.Calls++;

            if (this.Fail)
            {
                throw new InvalidOperationException("handler failed");
            }
        }

        public Task HandleAsync(TestAsyncEvent @event)
        {
            this.AsyncCalls++;

            return this.Fail ? Task.FromException(new InvalidOperationException("handler failed")) : Task.CompletedTask;
        }
    }
}
