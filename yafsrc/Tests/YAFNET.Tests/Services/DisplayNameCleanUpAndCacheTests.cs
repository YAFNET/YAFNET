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

using System.Runtime.Caching;
using System.Threading.Tasks;

using NSubstitute;

using YAF.Configuration.Pattern;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Core.Tasks;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The default user display name lookup, the user (un)suspend task and small helpers
/// (<see cref="MemoryCache"/> / <see cref="IDateTimeService"/> extensions, <see cref="TypeDictionary"/>).
/// </summary>
[TestFixture]
[NonParallelizable]
public class DisplayNameCleanUpAndCacheTests
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

    [Test]
    [Description("Users are found by user name, or by display name when display names are enabled")]
    public void Display_Name_Lookup_Test()
    {
        var alice = this.db.AddUser("alice", u => u.DisplayName = "Alice Wonder");
        this.db.AddUser("bob", u => u.DisplayName = "Bob Builder");
        this.db.AddUser("other-board", u => u.BoardID = 2);

        var names = new DefaultUserDisplayName(this.board.Current.ServiceLocator);

        this.board.BoardSettings.EnableDisplayName = false;
        names.FindUserByName("alice").ID.Should().Be(alice);
        names.FindUserByName("Alice Wonder").Should().BeNull();
        names.FindUserContainsName("o").Select(u => u.Name).Should().BeEquivalentTo(["bob"]);

        this.board.BoardSettings.EnableDisplayName = true;
        names.FindUserByName("Alice Wonder").ID.Should().Be(alice);
        names.FindUserByName("alice").Should().BeNull();
        names.FindUserContainsName("Wonder").Single().ID.Should().Be(alice);

        names.GetNameById(alice).Should().Be("Alice Wonder");
        names.GetNameById(9999).Should().BeNull();
    }

    [Test]
    [Description("The clean up task lifts expired suspensions and keeps the running ones")]
    public async Task User_Clean_Up_Task_Test()
    {
        var expired = this.db.AddUser("expired", u => u.Suspended = DateTime.UtcNow.AddHours(-1));
        var running = this.db.AddUser("running", u => u.Suspended = DateTime.UtcNow.AddDays(1));
        var normal = this.db.AddUser("normal");

        var users = this.board.Current.GetRepository<User>();
        await users.SuspendAsync(expired, DateTime.UtcNow.AddHours(-1), "spam", normal);

        var task = new UserCleanUpTask { ServiceLocator = this.board.Resolve<IServiceLocator>() };
        task.TaskName.Should().Be(nameof(UserCleanUpTask));
        task.TaskDescription.Should().NotBeNullOrEmpty();

        await task.RunOnceAsync();

        var lifted = await users.GetByIdAsync(expired);
        lifted.Suspended.Should().BeNull();
        lifted.SuspendedReason.Should().BeNull();
        lifted.SuspendedBy.Should().Be(0);

        (await users.GetByIdAsync(running)).Suspended.Should().NotBeNull();
        (await users.GetByIdAsync(normal)).Suspended.Should().BeNull();
    }

    [Test]
    [Description("MemoryCache.GetOrSet only calls the factory when the key is missing, Set overwrites")]
    public void Memory_Cache_Test()
    {
        using var cache = new MemoryCache($"test-{Guid.NewGuid():N}");
        var calls = 0;

        cache.GetOrSet("key", () => ++calls).Should().Be(1);
        cache.GetOrSet("key", () => ++calls).Should().Be(1);
        calls.Should().Be(1);

        cache.Set("key", 42);
        cache.GetOrSet("key", () => ++calls).Should().Be(42);

        cache.Invoking(c => c.GetOrSet<string>(null, () => "x")).Should().Throw<ArgumentNullException>();
        cache.Invoking(c => c.GetOrSet<string>("x", null)).Should().Throw<ArgumentNullException>();
        FluentActions.Invoking(() => ((MemoryCache)null).Set("x", 1)).Should().Throw<ArgumentNullException>();

        cache.Invoking(c => c.Set<string>("key", null)).Should().NotThrow("errors on set are ignored");
    }

    [Test]
    [Description("Format picks the formatter for the requested format, bad values give [error]")]
    public void Date_Time_Format_Test()
    {
        var service = Substitute.For<IDateTimeService>();
        var date = new DateTime(2026, 10, 9, 12, 30, 0);

        service.FormatDateTimeShort(date).Returns("short");
        service.FormatDateTimeTopic(date).Returns("topic");
        service.FormatDateLong(date).Returns("long");
        service.FormatDateShort(date).Returns("date");
        service.FormatTime(date).Returns("time");
        service.FormatDateTime(date).Returns("both");

        service.Format(DateTimeFormat.BothDateShort, date).Should().Be("short");
        service.Format(DateTimeFormat.BothTopic, date).Should().Be("topic");
        service.Format(DateTimeFormat.DateLong, date).Should().Be("long");
        service.Format(DateTimeFormat.DateShort, date).Should().Be("date");
        service.Format(DateTimeFormat.Time, date).Should().Be("time");
        service.Format(DateTimeFormat.Both, date).Should().Be("both");
        service.Format((DateTimeFormat)99, date).Should().Be("both");

        service.Format(DateTimeFormat.Both, "not a date").Should().Be("[error]");
        service.Invoking(s => s.Format(DateTimeFormat.Both, null)).Should().Throw<ArgumentNullException>();

        service.FormatDateTimeTopic((object)"2026-10-09 12:30:00").Should().Be("topic");
        service.FormatDateTimeTopic((object)"garbage").Should().Be("[error]");
    }

    [Test]
    [Description("TypeDictionary converts stored values and returns defaults or null for missing keys")]
    public void Type_Dictionary_Test()
    {
        var dictionary = new TypeDictionary { ["int"] = "42", ["bool"] = "true", ["text"] = 7 };

        dictionary.AsInt("int").Should().Be(42);
        dictionary.AsInt("missing").Should().BeNull();
        dictionary.AsBoolean("bool").Should().BeTrue();
        dictionary.AsBoolean("missing").Should().BeNull();
        dictionary.AsString("text").Should().Be("7");
        dictionary.AsString("missing").Should().BeNull();
        dictionary.AsType("missing", 5).Should().Be(5);
        dictionary.AsType("int", 5).Should().Be(42);
        dictionary.AsType<long>("int").Should().Be(42L);
        dictionary.Invoking(d => d.AsType<int>("missing")).Should().Throw<KeyNotFoundException>();
    }
}
