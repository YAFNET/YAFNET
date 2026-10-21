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

using NSubstitute;

using YAF.Core.Services.Cache;
using YAF.Tests.TestInfrastructure;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Events;

/// <summary>
/// The production <see cref="HttpRuntimeCache"/> (MemoryCache.Default, keys per board).
/// MemoryCache.Default is process wide, so every test uses its own key prefix.
/// </summary>
[TestFixture]
[NonParallelizable]
public class HttpRuntimeCacheTests
{
    private RecordingEventRaiser events;

    private HttpRuntimeCache cache;

    private string prefix;

    [SetUp]
    public void SetUp()
    {
        this.events = new RecordingEventRaiser();
        this.cache = CreateCache(1, this.events);
        this.prefix = $"t{Guid.NewGuid():N}";
    }

    [TearDown]
    public void TearDown()
    {
        this.cache.Remove(k => k.StartsWith(this.prefix));
    }

    [Test]
    [Description("Set, get, indexer and remove")]
    public void Set_Get_Remove_Test()
    {
        var key = this.Key("a");

        this.cache.Get(key).Should().BeNull();

        this.cache.Set(key, "value");
        this.cache.Get(key).Should().Be("value");
        this.cache[key].Should().Be("value");
        this.cache.Get<string>(key).Should().Be("value");
        this.cache.Get<int>(key).Should().Be(0, "a value of another type is not returned");

        this.cache[key] = "other";
        this.cache.Get(key).Should().Be("other");

        this.cache.Remove(key);
        this.cache.Get(key).Should().BeNull();
    }

    [Test]
    [Description("GetOrSet computes once and returns the cached value afterwards")]
    public void GetOrSet_Test()
    {
        var calls = 0;

        this.cache.GetOrSet(this.Key("a"), () => ++calls).Should().Be(1);
        this.cache.GetOrSet(this.Key("a"), () => ++calls).Should().Be(1);
        this.cache.GetOrSet(this.Key("b"), () => ++calls, TimeSpan.FromMinutes(1)).Should().Be(2);
        this.cache.GetOrSet(this.Key("b"), () => ++calls, TimeSpan.FromMinutes(1)).Should().Be(2);

        calls.Should().Be(2);
    }

    [Test]
    [Description("Null results are not cached")]
    public void GetOrSet_Null_Not_Cached_Test()
    {
        var calls = 0;

        this.cache.GetOrSet<string>(this.Key("a"), () => { calls++; return null; });
        this.cache.GetOrSet<string>(this.Key("a"), () => { calls++; return null; });

        calls.Should().Be(2);
    }

    [Test]
    [Description("GetOrSetAsync computes once")]
    public async Task GetOrSetAsync_Test()
    {
        var calls = 0;

        (await this.cache.GetOrSetAsync(this.Key("a"), () => Task.FromResult(++calls))).Should().Be(1);
        (await this.cache.GetOrSetAsync(this.Key("a"), () => Task.FromResult(++calls))).Should().Be(1);
        (await this.cache.GetOrSetAsync(this.Key("b"), () => Task.FromResult(++calls), TimeSpan.FromMinutes(1))).Should().Be(2);

        calls.Should().Be(2);
    }

    [Test]
    [Description("A failed load is not cached, the next call loads again")]
    [TestCase(false)]
    [TestCase(true)]
    public async Task GetOrSetAsync_Failure_Not_Cached_Test(bool withTimeout)
    {
        var key = this.Key("a");
        var fail = true;

        Task<string> Load() => fail ? Task.FromException<string>(new InvalidOperationException("db down")) : Task.FromResult("ok");

        Task<string> Get() => withTimeout
            ? this.cache.GetOrSetAsync(key, Load, TimeSpan.FromHours(1))
            : this.cache.GetOrSetAsync(key, Load);

        await FluentActions.Awaiting(Get).Should().ThrowAsync<InvalidOperationException>();

        fail = false;

        (await Get()).Should().Be("ok", "the failure must not stay in the cache");
    }

    [Test]
    [Description("Set with a timeout replaces an existing value")]
    public void Set_With_Timeout_Test()
    {
        var key = this.Key("a");

        this.cache.Set(key, "first", TimeSpan.FromMinutes(1));
        this.cache.Set(key, "second", TimeSpan.FromMinutes(1));

        this.cache.Get(key).Should().Be("second");
    }

    [Test]
    [Description("Removing a cached item raises CacheItemRemovedEvent")]
    public void Remove_Raises_Event_Test()
    {
        var key = this.Key("a");
        this.cache.GetOrSet(key, () => "value");

        this.cache.Remove(key);

        this.events.OfType<CacheItemRemovedEvent>().Should().NotBeEmpty();
    }

    [Test]
    [Description("Keys are per board")]
    public void Keys_Per_Board_Test()
    {
        var key = this.Key("a");
        var otherBoard = CreateCache(2, new RecordingEventRaiser());

        this.cache.Set(key, "board 1");

        otherBoard.Get(key).Should().BeNull();

        otherBoard.Set(key, "board 2");
        this.cache.Get(key).Should().Be("board 1");

        otherBoard.Remove(key);
    }

    [Test]
    [Description("A $ in the key does not break the key format")]
    public void Dollar_In_Key_Test()
    {
        var key = this.Key("a$b");

        this.cache.Set(key, "value");

        this.cache.Get(key).Should().Be("value");
        this.cache.GetAll<string>().Should().Contain(kv => kv.Key == this.Key("a.b"));
    }

    [Test]
    [Description("GetAll, CountOf, RemoveOf and Remove(where)")]
    public void GetAll_Test()
    {
        this.cache.Set(this.Key("s1"), "x");
        this.cache.Set(this.Key("s2"), "y");
        this.cache.Set(this.Key("n1"), 42);

        this.cache.GetAll<string>().Where(kv => kv.Key.StartsWith(this.prefix)).Select(kv => kv.Value)
            .Should().BeEquivalentTo("x", "y");
        this.cache.GetAll<int>().Should().Contain(kv => kv.Key == this.Key("n1") && kv.Value == 42);

        this.cache.Remove(k => k == this.Key("s1"));
        this.cache.Get(this.Key("s1")).Should().BeNull();
        this.cache.Get(this.Key("s2")).Should().Be("y");
    }

    [Test]
    [Description("Null keys and factories are rejected")]
    public void Null_Arguments_Test()
    {
        this.cache.Invoking(c => c.Get(null)).Should().Throw<ArgumentNullException>();
        this.cache.Invoking(c => c.GetOrSet<string>(null, () => "x")).Should().Throw<ArgumentNullException>();
        this.cache.Invoking(c => c.GetOrSet<string>("k", null)).Should().Throw<ArgumentNullException>();
        this.cache.Invoking(c => c.Set(null, "x", TimeSpan.FromMinutes(1))).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("The lock object of a key is stable")]
    public void StaticLockObject_Test()
    {
        var locks = new StaticLockObject();

        locks.Get("a").Should().BeSameAs(locks.Get("a"));
        locks.Invoking(l => l.Get(null)).Should().Throw<ArgumentNullException>();
    }

    private static HttpRuntimeCache CreateCache(int boardId, IRaiseEvent events)
    {
        var haveBoardId = Substitute.For<IHaveBoardID>();
        haveBoardId.BoardID.Returns(boardId);

        return new HttpRuntimeCache(events, new StaticLockObject(), new TreatCacheKeyWithBoard(haveBoardId));
    }

    private string Key(string name)
    {
        return $"{this.prefix}{name}";
    }
}
