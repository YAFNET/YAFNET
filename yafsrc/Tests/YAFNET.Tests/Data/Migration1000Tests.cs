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
using YAF.Types.Models;

/// <summary>
/// The upgrade that removes the columns of the old private message limits and notification setting.
/// </summary>
[TestFixture]
[NonParallelizable]
public class Migration1000Tests
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
    [Description("The old columns are removed, also without a default constraint (only SQL Server has them as own objects): the columns of the ranks and groups were never removed")]
    public void Old_Columns_Are_Removed_Test()
    {
        using var connection = this.db.Open();

        connection.ExecuteSql($"ALTER TABLE {connection.GetQuotedTableName<User>()} ADD COLUMN PMNotification INTEGER NOT NULL DEFAULT 1");
        connection.ExecuteSql($"ALTER TABLE {connection.GetQuotedTableName<Rank>()} ADD COLUMN PMLimit INTEGER NOT NULL DEFAULT 0");
        connection.ExecuteSql($"ALTER TABLE {connection.GetQuotedTableName<Group>()} ADD COLUMN PMLimit INTEGER NOT NULL DEFAULT 0");

        new Migration1000 { Db = connection }.Up();

        connection.ColumnExists<User>("PMNotification").Should().BeFalse();
        connection.ColumnExists<Rank>("PMLimit").Should().BeFalse();
        connection.ColumnExists<Group>("PMLimit").Should().BeFalse();
    }

    [Test]
    [Description("A database without the old columns (a new install) is not changed")]
    public void Without_Old_Columns_Test()
    {
        using var connection = this.db.Open();

        FluentActions.Invoking(() => new Migration1000 { Db = connection }.Up()).Should().NotThrow();
    }
}
