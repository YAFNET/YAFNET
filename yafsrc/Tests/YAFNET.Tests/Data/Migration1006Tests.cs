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
/// The upgrade of the thanks to the reactions.
/// </summary>
[TestFixture]
[NonParallelizable]
public class Migration1006Tests
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
    [Description("A new database (the reaction column exists) is not changed by the upgrade")]
    public void New_Database_Test()
    {
        using var connection = this.db.Open();

        FluentActions.Invoking(() => new Migration1006 { Db = connection }.Up()).Should().NotThrow();

        connection.ColumnExists<Thanks>(x => x.Reaction).Should().BeTrue();
    }

    [Test]
    [Description("The reaction column is added, the thanks given before are a love")]
    public void Existing_Thanks_Are_Love_Test()
    {
        var alice = this.db.AddUser("Alice");
        var bob = this.db.AddUser("Bob");

        using var connection = this.db.Open();

        connection.DropColumn<Thanks>("Reaction");
        connection.ExecuteSql(
            $"INSERT INTO {connection.GetQuotedTableName<Thanks>()} (ThanksFromUserID, ThanksToUserID, MessageID, ThanksDate) VALUES ({bob}, {alice}, 1, '2020-05-01 10:00:00')");

        new Migration1006 { Db = connection }.Up();

        connection.ColumnExists<Thanks>(x => x.Reaction).Should().BeTrue();
        connection.Single<Thanks>(t => t.ThanksFromUserID == bob).Reaction.Should().Be((int)ReactionType.Love);
    }
}
