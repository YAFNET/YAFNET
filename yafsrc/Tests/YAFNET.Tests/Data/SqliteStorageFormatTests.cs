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

using System.Data;

using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

using YAF.Tests.TestInfrastructure;

/// <summary>
/// How SQLite stores dates and Guids. Both providers (System.Data.SQLite and Microsoft.Data.Sqlite, see
/// <see cref="SqliteTestProvider"/>) write them as System.Data.SQLite does, so a database file can be used with both.
/// </summary>
[TestFixture]
public class SqliteStorageFormatTests
{
    [Test]
    [Description("A UTC date is stored as text with a Z (as System.Data.SQLite stores it) and read back as the same UTC date")]
    public void Date_Is_Stored_As_Text_With_Z_Test()
    {
        using var database = new TestDatabase();
        using var db = database.Open();

        db.CreateTable<StorageFormatRow>();

        var created = new DateTime(2026, 11, 28, 10, 5, 6, 789, DateTimeKind.Utc);
        var id = (int)db.Insert(new StorageFormatRow { Created = created, Key = Guid.NewGuid() }, selectIdentity: true);

        Scalar(db, $"SELECT Created FROM {db.GetTableName<StorageFormatRow>()} WHERE ID = {id}")
            .Should().Be("2026-11-28 10:05:06.789Z");

        var row = db.SingleById<StorageFormatRow>(id);
        row.Created.Should().Be(created);
        row.Created.Kind.Should().Be(DateTimeKind.Utc);

        db.Select<StorageFormatRow>(r => r.Created == created).Should().ContainSingle("a date parameter matches the stored text");
        db.Select<StorageFormatRow>(r => r.Created > created.AddSeconds(-1)).Should().ContainSingle();
    }

    [Test]
    [Description("A Guid is stored as a 16 byte blob (as System.Data.SQLite stores it), and a Guid parameter matches it")]
    public void Guid_Is_Stored_As_Blob_Test()
    {
        using var database = new TestDatabase();
        using var db = database.Open();

        db.CreateTable<StorageFormatRow>();

        var key = Guid.NewGuid();
        var id = (int)db.Insert(new StorageFormatRow { Created = DateTime.UtcNow, Key = key }, selectIdentity: true);

        Scalar(db, $"SELECT typeof(Key) || ':' || length(Key) FROM {db.GetTableName<StorageFormatRow>()} WHERE ID = {id}")
            .Should().Be("blob:16");

        db.SingleById<StorageFormatRow>(id).Key.Should().Be(key);
        db.Select<StorageFormatRow>(r => r.Key == key).Should().ContainSingle();
    }

    private static string Scalar(IDbConnection db, string sql)
    {
        using var command = db.CreateCommand();
        command.CommandText = sql;

        return command.ExecuteScalar()?.ToString();
    }

    /// <summary>
    /// A table for the tests.
    /// </summary>
    public class StorageFormatRow
    {
        [AutoIncrement]
        public int ID { get; set; }

        public DateTime Created { get; set; }

        public Guid Key { get; set; }
    }
}
