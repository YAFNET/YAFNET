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

using ServiceStack.OrmLite;

using YAF.Core.Migrations;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Models;

/// <summary>
/// The upgrades of the 4.0 versions: new columns and tables, the import of the old private messages and the
/// removal of old settings.
/// </summary>
[TestFixture]
[NonParallelizable]
public class Migration1001To1005Tests
{
    private TestDatabase db;

    [SetUp]
    public void SetUp()
    {
        this.db = new TestDatabase();
    }

    [TearDown]
    public void TearDown()
    {
        this.db?.Dispose();
    }

    [Test]
    [Description("A new database (all columns and tables exist) is not changed by the upgrades")]
    public void New_Database_Test()
    {
        using var connection = this.db.Open();

        FluentActions.Invoking(
            () =>
            {
                new Migration1001 { Db = connection }.Up();
                new Migration1002 { Db = connection }.Up();
                new Migration1003 { Db = connection }.Up();
                new Migration1004 { Db = connection }.Up();
                new Migration1005 { Db = connection }.Up();
            }).Should().NotThrow();

        connection.TableExists<PMessage>().Should().BeFalse();
    }

    [Test]
    [Description("The private messages of YAF 3 are imported (one per recipient, read) and the old tables are removed")]
    public void Import_Old_Private_Messages_Test()
    {
        var alice = this.db.AddUser("Alice");
        var bob = this.db.AddUser("Bob");
        var carol = this.db.AddUser("Carol");

        using var connection = this.db.Open();

        connection.CreateTable<PMessage>();
        connection.CreateTable<UserPMessage>();

        var created = new DateTime(2020, 5, 1, 10, 0, 0, DateTimeKind.Utc);

        var toBoth = (int)connection.Insert(
            new PMessage { FromUserID = alice, Created = created, Subject = "Hello", Body = "hello you two", Flags = 0 },
            selectIdentity: true);
        var toAlice = (int)connection.Insert(
            new PMessage { FromUserID = bob, Created = created.AddDays(1), Subject = "Re", Body = "hi alice", Flags = 0 },
            selectIdentity: true);

        connection.Insert(new UserPMessage { UserID = bob, PMessageID = toBoth });
        connection.Insert(new UserPMessage { UserID = carol, PMessageID = toBoth });
        connection.Insert(new UserPMessage { UserID = alice, PMessageID = toAlice });

        new Migration1001 { Db = connection }.Up();

        var messages = connection.Select<PrivateMessage>().OrderBy(m => m.ToUserId).ToList();

        messages.Select(m => (m.FromUserId, m.ToUserId, m.Body)).Should().BeEquivalentTo(
            [(bob, alice, "hi alice"), (alice, bob, "hello you two"), (alice, carol, "hello you two")]);
        messages.Should().OnlyContain(m => new PrivateMessageFlags(m.Flags).IsRead);
        messages.Single(m => m.ToUserId == bob).Created.Should().Be(created);

        connection.TableExists<PMessage>().Should().BeFalse();
        connection.TableExists<UserPMessage>().Should().BeFalse();
    }

    [Test]
    [Description("The location column of the active users is replaced by the path")]
    public void Active_Path_Test()
    {
        using var connection = this.db.Open();

        connection.DropColumn<Active>("Path");
        connection.ExecuteSql($"ALTER TABLE {connection.GetQuotedTableName<Active>()} ADD COLUMN Location TEXT NULL");

        new Migration1002 { Db = connection }.Up();

        connection.ColumnExists<Active>(x => x.Path).Should().BeTrue();
        connection.ColumnExists<Active>("Location").Should().BeFalse();
    }

    [Test]
    [Description("The device subscriptions and banned countries tables are created when missing")]
    public void New_Tables_Test()
    {
        using var connection = this.db.Open();

        connection.DropTable<DeviceSubscription>();
        connection.DropTable<BannedCountry>();

        new Migration1003 { Db = connection }.Up();

        connection.TableExists<DeviceSubscription>().Should().BeTrue();
        connection.TableExists<BannedCountry>().Should().BeTrue();
    }

    [Test]
    [Description("The old board settings are removed, the others are kept")]
    public void Old_Settings_Test()
    {
        using var connection = this.db.Open();

        foreach (var name in new[] { "versionname", "lastiplistimport", "showmoderatorlist", "cdvversion", "keepme" })
        {
            connection.Insert(new Registry { Name = name, Value = "x", BoardID = TestDatabase.BoardId });
        }

        new Migration1004 { Db = connection }.Up();

        connection.Select<Registry>(r => r.Value == "x").Select(r => r.Name).Should().Equal("keepme");
    }
}
