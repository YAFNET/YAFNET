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

using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Core.Services.Cache;
using YAF.Core.Services.Import;
using YAF.Data.Sqlite;
using YAF.Tests.TestInfrastructure;
using YAF.Types.EventProxies;
using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The installer: creating the database and the first board with its default BBCodes and spam words.
/// </summary>
[TestFixture]
[NonParallelizable]
public class InstallServiceTests
{
    private const string GermanJson = """
        { "Resources": { "@language": "Deutsch", "@code": "de", "page": [] } }
        """;

    private readonly static string WebProjectRoot = Path.GetFullPath(
        Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..", "..", "YetAnotherForum.NET", "wwwroot"));

    private string webRoot;

    private TestBoardContext board;

    private SQLiteConnection keepAlive;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-install-{Guid.NewGuid():N}");

        var languages = Path.Combine(this.webRoot, "languages");
        Directory.CreateDirectory(languages);

        File.Copy(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "languages", "english.json"),
            Path.Combine(languages, "english.json"));
        File.WriteAllText(Path.Combine(languages, "german.json"), GermanJson);

        var resources = Path.Combine(this.webRoot, "resources");
        Directory.CreateDirectory(resources);

        foreach (var file in new[] { "BBCodeExtensions.xml", "SpamWords.xml" })
        {
            File.Copy(Path.Combine(WebProjectRoot, "resources", file), Path.Combine(resources, file));
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (this.board != null)
        {
            this.board.Resolve<CurrentBoardSettings>().Reset();
            this.board.Dispose();
        }

        this.keepAlive?.Dispose();

        if (Directory.Exists(this.webRoot))
        {
            Directory.Delete(this.webRoot, true);
        }
    }

    [Test]
    [Description("The forum is installed when a board exists")]
    public void IsForumInstalled_Test()
    {
        var install = this.CreateInstallService();

        install.IsForumInstalled.Should().BeTrue("the test database has a board");

        this.board.Current.GetRepository<Board>().Delete(b => b.ID > 0);

        install.IsForumInstalled.Should().BeFalse("the tables exist, but there is no board");
    }

    [Test]
    [Description("A new database has no forum, the installer creates the tables, the views and saves the version")]
    public void InitializeDatabase_Test()
    {
        var install = this.CreateInstallService(emptyDatabase: true);

        install.IsForumInstalled.Should().BeFalse("there are no tables");

        install.InitializeDatabase().Should().BeTrue();

        install.IsForumInstalled.Should().BeFalse("the tables exist, but there is no board");

        var registry = this.board.Current.GetRepository<Registry>();
        registry.GetSingle(r => r.Name == "version" && r.BoardID == null).Value.Should()
            .Be(new BoardInfo(this.board.Resolve<IServiceLocator>()).AppVersion.ToString());

        // the views are created too
        using var db = this.board.Resolve<IDbAccess>().CreateConnectionOpen();
        db.SqlScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type = 'view'").Should().BePositive();
    }

    [Test]
    [Description("The connection test returns the error of a database that can not be opened")]
    public void TestDatabaseConnection_Test()
    {
        var install = this.CreateInstallService();

        install.TestDatabaseConnection(out var message).Should().BeTrue();
        message.Should().BeEmpty();

        var missing = new SqliteDbAccess(_ => SQLiteFactory.Instance);
        missing.Information.ConnectionString =
            () => $"Data Source={Path.Combine(this.webRoot, "missing", "forum.db")};FailIfMissing=True";

        install.DbAccess = missing;

        install.TestDatabaseConnection(out message).Should().BeFalse();
        message.Should().NotBeEmpty();
    }

    [Test]
    [Description("Installing creates the board with the language of its culture, the settings and imports the defaults")]
    public async Task InitializeForum_Test()
    {
        var install = this.CreateInstallService();
        var applicationId = Guid.NewGuid();

        await this.InitializeForumAsync(install, applicationId, culture: "de-DE");

        var newBoard = this.board.Current.GetRepository<Board>().GetSingle(b => b.Name == "My Forum");
        newBoard.Description.Should().Be("The description");

        var registry = this.board.Current.GetRepository<Registry>();
        string Global(string name) => registry.GetSingle(r => r.Name == name && r.BoardID == null).Value;

        Global("applicationid").Should().Be(applicationId.ToString());
        Global("forumemail").Should().Be("forum@example.com");
        Global("forumlogo").Should().Be("logo.svg");
        Global("baseurlmask").Should().Be("https://forum.example.com/");

        registry.GetSingle(r => r.Name == "culture" && r.BoardID == newBoard.ID).Value.Should().Be("de-DE");
        registry.GetSingle(r => r.Name == "language" && r.BoardID == newBoard.ID).Value.Should().Be("german.json");

        var admin = this.board.Current.GetRepository<User>().GetSingle(u => u.BoardID == newBoard.ID && u.Name == "admin");
        admin.Email.Should().Be("admin@example.com");
        admin.ProviderUserKey.Should().Be("admin-key");
        admin.UserFlags.IsHostAdmin.Should().BeTrue();

        // the settings of the new board are loaded
        this.board.Resolve<CurrentBoardSettings>().Instance.BoardId.Should().Be(newBoard.ID);

        Directory.Exists(Path.Combine(this.webRoot, "uploads")).Should().BeTrue();

        this.board.Events.OfType<ImportStaticDataEvent>().Should().ContainSingle(e => e.BoardID == newBoard.ID);

        this.board.Current.GetRepository<BBCode>().Count(b => b.BoardID == newBoard.ID).Should().BePositive();
        this.board.Current.GetRepository<SpamWords>().Count(s => s.BoardID == newBoard.ID).Should().BePositive();
    }

    [Test]
    [Description("A culture without language file installs the english language, missing default files are skipped")]
    public async Task InitializeForum_Defaults_Test()
    {
        var install = this.CreateInstallService();

        File.Delete(Path.Combine(this.webRoot, "resources", "BBCodeExtensions.xml"));
        File.Delete(Path.Combine(this.webRoot, "resources", "SpamWords.xml"));
        Directory.CreateDirectory(Path.Combine(this.webRoot, "uploads"));

        await this.InitializeForumAsync(install, Guid.NewGuid(), culture: "xx-XX", forumEmail: string.Empty);

        var newBoard = this.board.Current.GetRepository<Board>().GetSingle(b => b.Name == "My Forum");

        var registry = this.board.Current.GetRepository<Registry>();
        registry.GetSingle(r => r.Name == "language" && r.BoardID == newBoard.ID).Value.Should().Be("english.json");
        registry.Count(r => r.Name == "forumemail").Should().Be(0, "an empty forum email is not saved");

        this.board.Current.GetRepository<BBCode>().Count(b => b.BoardID == newBoard.ID).Should().Be(0);
        this.board.Current.GetRepository<SpamWords>().Count(s => s.BoardID == newBoard.ID).Should().Be(0);
    }

    [Test]
    [Description("The installation is finished when the default BBCodes are imported")]
    public async Task InitializeForum_Waits_For_BBCode_Import_Test()
    {
        var importer = Substitute.For<IDataImporter>();
        var import = new TaskCompletionSource<int>();
        importer.BBCodeExtensionImportAsync(Arg.Any<int>(), Arg.Any<Stream>()).Returns(import.Task);

        var install = this.CreateInstallService(configure: b => b.RegisterInstance(importer).As<IDataImporter>());

        var installation = this.InitializeForumAsync(install, Guid.NewGuid());

        installation.IsCompleted.Should().BeFalse("the BBCodes are still imported");

        import.SetResult(1);
        await installation;
    }

    [Test]
    [Description("All required values are checked")]
    public async Task InitializeForum_Requires_Values_Test()
    {
        var install = this.CreateInstallService();

        await FluentActions.Awaiting(() => this.InitializeForumAsync(install, Guid.NewGuid(), culture: null))
            .Should().ThrowAsync<ArgumentNullException>();
        await FluentActions.Awaiting(() => this.InitializeForumAsync(install, Guid.NewGuid(), forumEmail: null))
            .Should().ThrowAsync<ArgumentNullException>();

        this.board.Current.GetRepository<Board>().Count(b => b.Name == "My Forum").Should().Be(0);
    }

    private Task InitializeForumAsync(
        InstallService install,
        Guid applicationId,
        string culture = "en-US",
        string forumEmail = "forum@example.com")
    {
        return install.InitializeForumAsync(
            applicationId,
            "My Forum",
            "The description",
            culture,
            forumEmail,
            "logo.svg",
            "https://forum.example.com/",
            "admin",
            "admin@example.com",
            "admin-key");
    }

    private InstallService CreateInstallService(bool emptyDatabase = false, Action<ContainerBuilder> configure = null)
    {
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        SqliteDbAccess emptyAccess = null;

        if (emptyDatabase)
        {
            var connectionString = $"FullUri=file:yaf_install_{Guid.NewGuid():N}?mode=memory&cache=shared;";

            // the in-memory database lives as long as a connection is open
            this.keepAlive = new SQLiteConnection(connectionString);
            this.keepAlive.Open();

            emptyAccess = new SqliteDbAccess(_ => SQLiteFactory.Instance);
            emptyAccess.Information.ConnectionString = () => connectionString;
        }

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardSettingsService>().AsSelf();
                b.RegisterType<CurrentBoardSettings>().AsSelf();
                b.RegisterType<TreatCacheKeyWithBoard>().As<ITreatCacheKey>();
                b.RegisterType<DataImporter>().As<IDataImporter>();

                if (emptyAccess != null)
                {
                    b.RegisterInstance(emptyAccess).As<IDbAccess>();
                }

                configure?.Invoke(b);
            },
            useDatabase: true);

        return new InstallService(
            this.board.Resolve<IServiceLocator>(),
            this.board.Events,
            this.board.Resolve<IDbAccess>());
    }
}
