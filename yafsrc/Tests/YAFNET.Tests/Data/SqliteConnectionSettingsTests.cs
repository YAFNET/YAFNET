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

using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

using YAF.Data.Sqlite;

/// <summary>
/// The connection settings of SQLite for parallel requests (<see cref="SqliteDbInformation.WithConcurrencySettings"/>).
/// </summary>
[TestFixture]
public class SqliteConnectionSettingsTests
{
    [Test]
    [Description("A busy timeout is added, so parallel requests wait for a lock instead of failing with 'database is locked'")]
    public void Busy_Timeout_Is_Added()
    {
        var builder = new SQLiteConnectionStringBuilder(
            SqliteDbInformation.WithConcurrencySettings("Data Source=yafnet.db;Version=3;"));

        builder.DataSource.Should().Be("yafnet.db");
        builder.BusyTimeout.Should().Be(5000);
        builder.Pooling.Should().BeTrue("pooled connections keep the WAL index open");
        builder.ContainsKey("journal mode").Should().BeFalse("the journal mode is set once, not by every connection");
    }

    [Test]
    [Description("Settings of the admin in the connection string are kept")]
    public void Own_Settings_Are_Kept()
    {
        var builder = new SQLiteConnectionStringBuilder(
            SqliteDbInformation.WithConcurrencySettings("Data Source=yafnet.db;BusyTimeout=100;Pooling=False"));

        builder.BusyTimeout.Should().Be(100);
        builder.Pooling.Should().BeFalse();

        SqliteDbInformation.WithConcurrencySettings(null).Should().BeNull();
    }

    [Test]
    [Description("The database file is switched to the WAL journal mode, unless the connection string sets a journal mode")]
    public void Wal_Journal_Mode_Is_Set_Once()
    {
        var file = Path.Combine(Path.GetTempPath(), $"yaf-wal-{Guid.NewGuid():N}.db");

        try
        {
            var connectionString = $"Data Source={file};Pooling=False";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SqliteDbInformation.EnableWalJournalMode(connection, connectionString).Should().BeTrue();
            }

            // the mode is stored in the file, a new connection has it without any setting
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA journal_mode;";
                command.ExecuteScalar().Should().Be("wal");

                SqliteDbInformation.EnableWalJournalMode(connection, $"{connectionString};Journal Mode=Delete")
                    .Should().BeFalse("the admin set an own journal mode");
            }
        }
        finally
        {
            SQLiteConnection.ClearAllPools();

            foreach (var name in new[] { file, $"{file}-wal", $"{file}-shm" })
            {
                if (File.Exists(name))
                {
                    File.Delete(name);
                }
            }
        }
    }

    [Test]
    [Description("The provider init (first use of the database) switches the database file to WAL")]
    public void Provider_Init_Sets_Wal_Test()
    {
        var file = Path.Combine(Path.GetTempPath(), $"yaf-wal-init-{Guid.NewGuid():N}.db");

        try
        {
            var connectionString = $"Data Source={file};Version=3;";

            var dbAccess = new SqliteDbAccess(_ => SQLiteFactory.Instance);
            dbAccess.Information.ConnectionString = () => SqliteDbInformation.WithConcurrencySettings(connectionString);

            new SetSqliteDialectEvent().Handle(
                new YAF.Core.Events.InitDatabaseProviderEvent(SqliteDbAccess.ProviderTypeName, dbAccess));

            using var connection = new SQLiteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode;";
            command.ExecuteScalar().Should().Be("wal");
        }
        finally
        {
            SQLiteConnection.ClearAllPools();

            foreach (var name in new[] { file, $"{file}-wal", $"{file}-shm" })
            {
                if (File.Exists(name))
                {
                    File.Delete(name);
                }
            }
        }
    }

    [Test]
    [Description("An operation that fails with 'database is locked' (SQLITE_BUSY) is tried again")]
    public async Task Busy_Operation_Is_Retried()
    {
        var dbAccess = CreateDbAccess();
        var calls = 0;

        var result = await dbAccess.ExecuteAsync(
                         _ =>
                             {
                                 calls++;

                                 return calls == 1
                                            ? throw new SQLiteException(SQLiteErrorCode.Busy, "database is locked")
                                            : Task.FromResult(42);
                             });

        result.Should().Be(42);
        calls.Should().Be(2);

        calls = 0;
        dbAccess.Execute(
                _ =>
                    {
                        calls++;

                        return calls == 1 ? throw new SQLiteException(SQLiteErrorCode.Busy, "database is locked") : 1;
                    })
            .Should().Be(1);
        calls.Should().Be(2);
    }

    [Test]
    [Description("A lasting lock and other errors are not retried forever")]
    public async Task Retry_Ends_Test()
    {
        var dbAccess = CreateDbAccess();
        var calls = 0;

        await FluentActions.Awaiting(
                () => dbAccess.ExecuteAsync<int>(
                    _ =>
                        {
                            calls++;
                            throw new SQLiteException(SQLiteErrorCode.Busy, "database is locked");
                        }))
            .Should().ThrowAsync<SQLiteException>();

        calls.Should().Be(4, "the first try and three retries");

        calls = 0;

        await FluentActions.Awaiting(
                () => dbAccess.ExecuteAsync<int>(
                    _ =>
                        {
                            calls++;
                            throw new SQLiteException(SQLiteErrorCode.Constraint, "constraint failed");
                        }))
            .Should().ThrowAsync<SQLiteException>();

        calls.Should().Be(1, "only a locked database is tried again");
    }

    private static SqliteDbAccess CreateDbAccess()
    {
        // the OrmLite dialect, as at the start of the application
        new SetSqliteDialectEvent().Handle(new YAF.Core.Events.InitDatabaseProviderEvent(SqliteDbAccess.ProviderTypeName, null));

        var dbAccess = new SqliteDbAccess(_ => SQLiteFactory.Instance);

        dbAccess.Information.ConnectionString = () => $"FullUri=file:yaf_retry_{Guid.NewGuid():N}?mode=memory&cache=shared;";

        return dbAccess;
    }
}
