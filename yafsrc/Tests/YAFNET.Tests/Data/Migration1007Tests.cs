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
using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// The upgrade adding the board to the event log entries.
/// </summary>
[TestFixture]
[NonParallelizable]
public class Migration1007Tests
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
    [Description("A new database (the board column exists) is not changed by the upgrade")]
    public void New_Database_Test()
    {
        using var connection = this.db.Open();

        FluentActions.Invoking(() => new Migration1007 { Db = connection }.Up()).Should().NotThrow();

        connection.ColumnExists<EventLog>(x => x.BoardID).Should().BeTrue();
    }

    [Test]
    [Description("The board column is added, the existing entries get the board of their user, the ones without a user none")]
    public void Existing_Entries_Get_The_Board_Of_Their_User_Test()
    {
        var alice = this.db.AddUser("Alice");
        var other = this.db.AddUser("Other", u => u.BoardID = TestDatabase.BoardId + 1);

        using var connection = this.db.Open();

        connection.DropColumn<EventLog>("BoardID");

        var table = connection.GetQuotedTableName<EventLog>();

        foreach (var user in new[] { alice.ToString(), other.ToString(), "NULL" })
        {
            connection.ExecuteSql(
                $"INSERT INTO {table} (EventTime, UserID, Source, Description, Type) VALUES ('2026-11-01 10:00:00', {user}, 'test', 'd', 0)");
        }

        new Migration1007 { Db = connection }.Up();

        connection.ColumnExists<EventLog>(x => x.BoardID).Should().BeTrue();
        connection.Single<EventLog>(e => e.UserID == alice).BoardID.Should().Be(TestDatabase.BoardId);
        connection.Single<EventLog>(e => e.UserID == other).BoardID.Should().Be(TestDatabase.BoardId + 1);
        connection.Single<EventLog>(e => e.UserID == null).BoardID.Should().BeNull();
    }

    [Test]
    [Description("The tables are created and every board gets the default achievements once")]
    public void Tables_And_Default_Achievements_Test()
    {
        using var connection = this.db.Open();

        connection.DropTable<UserAchievement>();
        connection.DropTable<Achievement>();

        connection.Insert(new Board { ID = TestDatabase.BoardId + 1, Name = "Second Board" });

        new Migration1007 { Db = connection }.Up();

        connection.TableExists<Achievement>().Should().BeTrue();
        connection.TableExists<UserAchievement>().Should().BeTrue();

        var count = AchievementDefaults.Create(TestDatabase.BoardId).Count;

        connection.Count<Achievement>(a => a.BoardID == TestDatabase.BoardId).Should().Be(count);
        connection.Count<Achievement>(a => a.BoardID == TestDatabase.BoardId + 1).Should().Be(count);

        // a second run (e.g. an upgrade that is continued) doesn't add them again
        new Migration1007 { Db = connection }.Up();

        connection.Count<Achievement>().Should().Be(count * 2);
    }

    [Test]
    [Description("A board with achievements (e.g. created by the installer) keeps them")]
    public void Board_With_Achievements_Is_Not_Changed_Test()
    {
        using var connection = this.db.Open();

        connection.Insert(
            new Achievement { BoardID = TestDatabase.BoardId, Name = "Own", Icon = "trophy", Type = 1, Threshold = 1 });

        new Migration1007 { Db = connection }.Up();

        connection.Select<Achievement>().Select(a => a.Name).Should().Equal("Own");
    }
}
