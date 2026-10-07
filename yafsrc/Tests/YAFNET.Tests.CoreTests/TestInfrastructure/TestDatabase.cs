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

namespace YAF.Tests.CoreTests.TestInfrastructure;

using System.Data;
using System.Data.SQLite;

using ServiceStack.OrmLite;

using YAF.Configuration;
using YAF.Core.Events;
using YAF.Core.Migrations;
using YAF.Data.Sqlite;
using YAF.Types.Extensions.Data;
using YAF.Types.Models;

/// <summary>
/// A private in-memory SQLite database with the full YAF schema, built by the same
/// migration and view scripts the installer uses.
/// </summary>
/// <remarks>
/// The schema is created once per test run in a template database. Every instance gets
/// its own copy through the SQLite backup API, so tests are isolated and cheap.
/// <see cref="DbAccessBase"/> opens and closes a connection per operation, and an in-memory
/// database disappears with its last connection, so each instance keeps one connection open
/// until it is disposed.
/// </remarks>
public sealed class TestDatabase : IDisposable
{
    /// <summary>
    /// The ID of the board that every test database contains.
    /// </summary>
    public const int BoardId = 1;

    /// <summary>
    /// The ID of the rank that every test database contains.
    /// </summary>
    public const int RankId = 1;

    private static readonly Lazy<SQLiteConnection> Template = new(CreateTemplate);

    private readonly SQLiteConnection keepAlive;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestDatabase"/> class.
    /// </summary>
    public TestDatabase()
    {
        this.ConnectionString = CreateConnectionString();

        this.keepAlive = new SQLiteConnection(this.ConnectionString);
        this.keepAlive.Open();

        Template.Value.BackupDatabase(this.keepAlive, "main", "main", -1, null, 0);

        this.DbAccess = CreateDbAccess(this.ConnectionString);
    }

    /// <summary>
    /// Gets the connection string of this database.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Gets the <see cref="SqliteDbAccess"/> used by the repositories.
    /// </summary>
    public SqliteDbAccess DbAccess { get; }

    /// <summary>
    /// Opens a new connection, e.g. to seed or inspect data with OrmLite.
    /// </summary>
    public IDbConnection Open()
    {
        return this.DbAccess.CreateConnectionOpen();
    }

    /// <summary>
    /// Inserts a user with valid defaults for all required columns.
    /// </summary>
    /// <returns>The new user ID.</returns>
    public int AddUser(string name, Action<User> configure = null)
    {
        var user = new User
        {
            BoardID = BoardId,
            RankID = RankId,
            Name = name,
            DisplayName = name,
            Email = $"{name.ToLowerInvariant()}@example.com",
            Joined = DateTime.UtcNow,
            LastVisit = DateTime.UtcNow,
            PageSize = 20
        };

        configure?.Invoke(user);

        using var db = this.Open();

        return (int)db.Insert(user, selectIdentity: true);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.keepAlive.Dispose();
    }

    private static SQLiteConnection CreateTemplate()
    {
        // the values from appsettings-Sqlite.json, the naming strategy reads them for every table name
        Config.DatabaseObjectQualifier = "yaf_";
        Config.DatabaseOwner = "dbo";

        // what SetSqliteDialectEvent does at startup
        new SetSqliteDialectEvent().Handle(new InitDatabaseProviderEvent(SqliteDbAccess.ProviderTypeName, null));

        var connectionString = CreateConnectionString();

        // kept open for the whole test run
        var template = new SQLiteConnection(connectionString);
        template.Open();

        var dbAccess = CreateDbAccess(connectionString);

        new Migrator(dbAccess.ResolveDbFactory(), typeof(Migration01)).Run(throwIfError: true);

        dbAccess.Execute(dbCommand => dbAccess.Information.CreateViews(dbAccess, dbCommand));
        dbAccess.Execute(dbCommand => dbAccess.Information.CreateIndexViews(dbAccess, dbCommand));

        using var db = dbAccess.CreateConnectionOpen();

        db.Insert(new Board { ID = BoardId, Name = "Test Board", Description = "Unit test board" });
        db.Insert(new Rank { ID = RankId, BoardID = BoardId, Name = "Member", SortOrder = 1 });

        return template;
    }

    private static string CreateConnectionString()
    {
        return $"FullUri=file:yaf_test_{Guid.NewGuid():N}?mode=memory&cache=shared;";
    }

    private static SqliteDbAccess CreateDbAccess(string connectionString)
    {
        var dbAccess = new SqliteDbAccess(_ => SQLiteFactory.Instance);

        // the default reads IConfiguration through BoardContext
        dbAccess.Information.ConnectionString = () => connectionString;

        return dbAccess;
    }
}
