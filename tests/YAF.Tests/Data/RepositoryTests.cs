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


using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// Repository tests against an in-memory SQLite database with the real schema.
/// </summary>
[TestFixture]
[NonParallelizable]
public class RepositoryTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The schema is created and the test board exists")]
    public void Database_Has_Schema_And_Board_Test()
    {
        var boards = this.board.Current.GetRepository<Board>().GetAll();

        boards.Should().ContainSingle().Which.Name.Should().Be("Test Board");
        this.board.Current.GetRepository<Board>().BoardID.Should().Be(TestDatabase.BoardId);
    }

    [Test]
    [Description("Every test gets its own database")]
    public void Databases_Are_Isolated_Test()
    {
        this.Repository<BannedIP>().Insert(NewBannedIp("10.0.0.1"));

        using var other = new TestDatabase();
        using var db = other.Open();

        db.Count<BannedIP>().Should().Be(0);
        this.Repository<BannedIP>().Count(x => true).Should().Be(1);
    }

    [Test]
    [Description("Insert, GetById, Update and DeleteById round trip")]
    public void Crud_Round_Trip_Test()
    {
        var repository = this.Repository<BannedIP>();

        var id = repository.Insert(NewBannedIp("10.0.0.1"));

        id.Should().BePositive();
        repository.GetById(id).Mask.Should().Be("10.0.0.1");

        var entity = repository.GetById(id);
        entity.Reason = "changed";
        repository.Update(entity).Should().BeTrue();
        repository.GetById(id).Reason.Should().Be("changed");

        repository.DeleteById(id).Should().BeTrue();
        repository.GetById(id).Should().BeNull();
        repository.DeleteById(id).Should().BeFalse();

        // Insert and Update don't raise events in 3.x, only Upsert does
        this.board.Events.OfType<RepositoryEvent<BannedIP>>().Select(e => e.RepositoryEventType).Should()
            .Equal(RepositoryEventType.Delete);
    }

    [Test]
    [Description("Upsert inserts or updates and raises the repository events")]
    public void Upsert_Raises_Events_Test()
    {
        var repository = this.Repository<BannedIP>();

        var entity = NewBannedIp("10.0.0.2");
        var id = repository.Upsert(entity);

        entity.ID = id;
        entity.Reason = "changed";
        repository.Upsert(entity).Should().Be(id);

        repository.GetById(id).Reason.Should().Be("changed");
        this.board.Events.OfType<RepositoryEvent<BannedIP>>().Select(e => e.RepositoryEventType).Should()
            .Equal(RepositoryEventType.New, RepositoryEventType.Update);
    }

    [Test]
    [Description("Get, GetSingle, Exists, Count and GetByBoardId filter correctly")]
    public void Query_Methods_Test()
    {
        var repository = this.Repository<BannedIP>();

        repository.Insert(NewBannedIp("10.0.0.1"));
        repository.Insert(NewBannedIp("10.0.0.2"));
        repository.Insert(NewBannedIp("10.0.0.3", boardId: 2));

        repository.Get(x => x.Mask.StartsWith("10.0.0")).Should().HaveCount(3);
        repository.GetSingle(x => x.Mask == "10.0.0.2").Should().NotBeNull();
        repository.Exists(x => x.Mask == "10.0.0.3").Should().BeTrue();
        repository.Exists(x => x.Mask == "1.1.1.1").Should().BeFalse();
        repository.Count(x => x.BoardID == TestDatabase.BoardId).Should().Be(2);
        repository.GetByBoardId().Should().HaveCount(2);
        repository.GetByBoardId(2).Should().ContainSingle();
    }

    [Test]
    [Description("DeleteAll removes every row and reports success, also for more than one row")]
    public void DeleteAll_Test()
    {
        var repository = this.Repository<BannedIP>();

        repository.Insert(NewBannedIp("10.0.0.1"));
        repository.Insert(NewBannedIp("10.0.0.2"));
        repository.Insert(NewBannedIp("10.0.0.3"));

        repository.DeleteAll().Should().BeTrue();

        repository.Count(x => true).Should().Be(0);
        this.board.Events.OfType<RepositoryEvent<BannedIP>>().Should()
            .Contain(e => e.RepositoryEventType == RepositoryEventType.Delete);
    }

    [Test]
    [Description("BannedIP Save inserts new masks, rejects duplicates and updates by id")]
    public void BannedIp_Save_Test()
    {
        var repository = this.Repository<BannedIP>();

        repository.Save(null, "192.168.*.*", "spam", 2).Should().BeTrue();
        repository.Save(null, "192.168.*.*", "again", 2).Should().BeFalse();

        var id = repository.GetSingle(x => x.Mask == "192.168.*.*").ID;

        repository.Save(id, "192.168.1.*", "narrowed", 2).Should().BeTrue();

        var saved = repository.GetById(id);
        saved.Mask.Should().Be("192.168.1.*");
        saved.Reason.Should().Be("narrowed");
        repository.Count(x => true).Should().Be(1);
    }

    [Test]
    [Description("Registry Save upserts settings per board and for all boards")]
    public void Registry_Save_Test()
    {
        var repository = this.Repository<Registry>();

        repository.Save("TestSetting", 1, TestDatabase.BoardId);
        repository.Save("TestSetting", 2, TestDatabase.BoardId);
        repository.Save("TestSetting", "global");

        repository.Get(r => r.Name == "testsetting").Should().HaveCount(2);
        repository.GetSingle(r => r.Name == "testsetting" && r.BoardID == TestDatabase.BoardId).Value
            .Should().Be("2");
        repository.List().Should().ContainSingle(r => r.Name == "testsetting").Which.Value.Should().Be("global");
    }

    [Test]
    [Description("Registry version helpers read the stored version")]
    public void Registry_Version_Test()
    {
        var repository = this.Repository<Registry>();

        repository.GetDbVersion().Should().Be(-1);

        repository.Save("version", 90);

        repository.GetDbVersion().Should().Be(90);
        repository.ValidateVersion(100).Should().Be(DbVersionType.Upgrade);

        // GetCurrentVersion caches the value
        this.board.Cache.Remove(Constants.Cache.Version);
        repository.ValidateVersion(90).Should().Be(DbVersionType.Current);
    }

    [Test]
    [Description("Registry increment helpers persist the new counter")]
    public void Registry_Increment_Test()
    {
        var repository = this.Repository<Registry>();

        repository.IncrementBannedUsers();
        repository.IncrementBannedUsers();

        this.board.BoardSettings.BannedUsers.Should().Be(2);
        repository.GetSingle(r => r.Name == "bannedusers").Value.Should().Be("2");
    }

    [Test]
    [Description("BadWordReplace loads its word list from the database")]
    public void BadWordReplace_From_Database_Test()
    {
        this.Repository<Replace_Words>().Save(null, "darn", "****");

        var service = new BadWordReplace(
            this.board.Cache,
            Substitute.For<ILoggerService>(),
            this.board.Resolve<IServiceLocator>());

        service.Replace("darn it").Should().Be("**** it");
        this.board.Cache[Constants.Cache.ReplaceWords].Should().NotBeNull("the word list is cached");
    }

    [Test]
    [Description("SpamWordCheck loads only the spam words of the current board")]
    public void SpamWordCheck_From_Database_Test()
    {
        this.Repository<Spam_Words>().Save(null, "casino");
        this.Repository<Spam_Words>().Save(null, "viagra", boardId: 2);

        var service = new SpamWordCheck(
            this.board.Cache,
            Substitute.For<ILoggerService>(),
            this.board.Resolve<IServiceLocator>());

        service.CheckForSpamWord("best casino", out var spamWord).Should().BeTrue();
        spamWord.Should().Be("casino");
        service.CheckForSpamWord("cheap viagra", out _).Should().BeFalse("viagra belongs to another board");
    }

    private static BannedIP NewBannedIp(string mask, int boardId = TestDatabase.BoardId)
    {
        return new BannedIP { BoardID = boardId, Mask = mask, Reason = "test", Since = DateTime.UtcNow, UserID = 2 };
    }

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }
}
