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
using YAF.Types.Flags;
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

    private int nameCounter;

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
    /// Inserts an approved user with valid defaults for all required columns.
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
            PageSize = 20,
            Flags = (int)UserFlags.Flags.IsApproved
        };

        configure?.Invoke(user);

        return this.Insert(user);
    }

    /// <summary>
    /// Inserts a rank. Ladder ranks are what users get promoted through by post count.
    /// </summary>
    /// <returns>The new rank ID.</returns>
    public int AddRank(string name, int? minPosts = null, bool isLadder = false, Action<Rank> configure = null)
    {
        var rank = new Rank
        {
            BoardID = BoardId,
            Name = name,
            MinPosts = minPosts,
            SortOrder = 1,
            Flags = new RankFlags { IsLadder = isLadder }.BitValue
        };

        configure?.Invoke(rank);

        return this.Insert(rank);
    }

    /// <summary>
    /// Inserts a category on the test board (names must be unique per board, the default is).
    /// </summary>
    /// <returns>The new category ID.</returns>
    public int AddCategory(string name = null, Action<Category> configure = null)
    {
        var category = new Category
        {
            BoardID = BoardId, Name = name ?? $"Category {++this.nameCounter}", SortOrder = 1, Flags = 1
        };

        configure?.Invoke(category);

        return this.Insert(category);
    }

    /// <summary>
    /// Inserts a forum. Without a category ID a new category is created
    /// (names must be unique per category, the default is).
    /// </summary>
    /// <returns>The new forum ID.</returns>
    public int AddForum(string name = null, int? categoryId = null, Action<Forum> configure = null)
    {
        name ??= $"Forum {++this.nameCounter}";

        var forum = new Forum
        {
            CategoryID = categoryId ?? this.AddCategory(),
            Name = name,
            Description = $"{name} description",
            SortOrder = 1
        };

        configure?.Invoke(forum);

        return this.Insert(forum);
    }

    /// <summary>
    /// Inserts a topic row (without messages; use <see cref="AddMessage"/> to add posts).
    /// </summary>
    /// <returns>The new topic ID.</returns>
    public int AddTopic(int forumId, int userId, string subject = "Test Topic", Action<Topic> configure = null)
    {
        var topic = new Topic
        {
            ForumID = forumId,
            UserID = userId,
            TopicName = subject,
            Posted = DateTime.UtcNow,
            UserName = $"User{userId}",
            UserDisplayName = $"User {userId}"
        };

        configure?.Invoke(topic);

        return this.Insert(topic);
    }

    /// <summary>
    /// Inserts an approved message at the next position of the topic and updates the topic's
    /// post count and last post columns (like posting through the repository would).
    /// </summary>
    /// <returns>The new message ID.</returns>
    public int AddMessage(
        int topicId,
        int userId,
        string text = "Test message",
        DateTime? posted = null,
        int? replyTo = null,
        Action<Message> configure = null)
    {
        using var db = this.Open();

        var position = db.Scalar<Message, int?>(m => Sql.Max(m.Position), m => m.TopicID == topicId);

        var message = new Message
        {
            TopicID = topicId,
            UserID = userId,
            MessageText = text,
            Posted = posted ?? DateTime.UtcNow,
            UserName = $"User{userId}",
            UserDisplayName = $"User {userId}",
            IP = "127.0.0.1",
            ReplyTo = replyTo,
            Position = position.HasValue ? position.Value + 1 : 0,
            Flags = new MessageFlags { IsHtml = true, IsBBCode = true, IsSmilies = true, IsApproved = true }.BitValue,
            IsModeratorChanged = false
        };

        configure?.Invoke(message);

        var id = (int)db.Insert(message, selectIdentity: true);

        db.UpdateAdd(() => new Topic { NumPosts = 1 }, t => t.ID == topicId);
        db.UpdateOnly(
            () => new Topic
            {
                LastPosted = message.Posted,
                LastMessageID = id,
                LastUserID = userId,
                LastUserName = message.UserName,
                LastUserDisplayName = message.UserDisplayName,
                LastMessageFlags = message.Flags
            },
            t => t.ID == topicId);

        return id;
    }

    /// <summary>
    /// The access a normal member gets: read, post, reply, poll and vote.
    /// </summary>
    public const AccessFlags.Flags MemberAccess = AccessFlags.Flags.ReadAccess | AccessFlags.Flags.PostAccess
                                                  | AccessFlags.Flags.ReplyAccess | AccessFlags.Flags.PollAccess
                                                  | AccessFlags.Flags.VoteAccess;

    /// <summary>
    /// Sets up the permission chain the access views are built from: a new group (with the given
    /// group flags) the user is a member of, and an access mask granting <paramref name="access"/>
    /// to the forum for that group. This only writes the permission tables; the per-user
    /// ActiveAccess cache the topic lists read is filled by <see cref="TestBoardContext.GrantAccess"/>.
    /// </summary>
    /// <returns>The new group ID.</returns>
    public int AddForumAccess(
        int userId,
        int forumId,
        AccessFlags.Flags access = MemberAccess,
        GroupFlags.Flags groupFlags = GroupFlags.Flags.None)
    {
        var number = ++this.nameCounter;

        var groupId = this.Insert(new Group { BoardID = BoardId, Name = $"Group {number}", Flags = (int)groupFlags, SortOrder = 1 });
        var maskId = this.Insert(new AccessMask { BoardID = BoardId, Name = $"Mask {number}", Flags = (int)access, SortOrder = 1 });

        using var db = this.Open();

        db.Insert(new ForumAccess { GroupID = groupId, ForumID = forumId, AccessMaskID = maskId });
        db.Insert(new UserGroup { UserID = userId, GroupID = groupId });

        return groupId;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.keepAlive.Dispose();
    }

    private int Insert<T>(T entity)
    {
        using var db = this.Open();

        return (int)db.Insert(entity, selectIdentity: true);
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
        // the start rank new users get (like the installer creates)
        db.Insert(new Rank { ID = RankId, BoardID = BoardId, Name = "Member", SortOrder = 1, Flags = (int)RankFlags.Flags.IsStart });

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
