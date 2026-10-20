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

using System.IO;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Core.Services.Import;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// The upgrade of an installed forum: the migrations since the installed version, the default BBCodes and spam
/// words of all boards and the new version.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UpgradeServiceTests
{
    private readonly static string WebProjectRoot = Path.GetFullPath(
        Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..", "..", "YetAnotherForum.NET", "wwwroot"));

    private string webRoot;

    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-upgrade-{Guid.NewGuid():N}");

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
        this.board?.Dispose();

        if (Directory.Exists(this.webRoot))
        {
            Directory.Delete(this.webRoot, true);
        }
    }

    [Test]
    [Description("The upgrade saves the new version, imports the defaults into every board and starts a new search index")]
    public async Task Upgrade_Test()
    {
        var upgrade = this.CreateUpgradeService();
        var secondBoard = this.AddBoard();

        this.Registry.Save("version", "1004");
        this.board.Cache[Constants.Cache.Version] = 1004;

        (await upgrade.UpgradeAsync()).Should().BeTrue();

        this.Global("version").Should().Be(this.board.Resolve<BoardInfo>().AppVersion.ToString());
        this.board.Cache[Constants.Cache.Version].Should().BeNull("the cached version is old");

        this.Global("forceupdatesearchindex").Should().Be("1", "there is no search index yet");
        this.Global("baseurlmask").Should().Be("https://forum.example.com/", "the address of the forum is taken from the request");

        this.board.Events.OfType<ImportStaticDataEvent>().Select(e => e.BoardID).Should()
            .BeEquivalentTo([TestDatabase.BoardId, secondBoard]);

        foreach (var boardId in new[] { TestDatabase.BoardId, secondBoard })
        {
            this.Repository<BBCode>().Count(b => b.BoardID == boardId).Should().BePositive();
            this.Repository<SpamWords>().Count(s => s.BoardID == boardId).Should().BePositive();
        }
    }

    [Test]
    [Description("The address of the forum and an existing search index are kept, upgrading twice imports the defaults once")]
    public async Task Upgrade_Keeps_Settings_Test()
    {
        var upgrade = this.CreateUpgradeService();

        this.Registry.Save("version", "1004");
        this.Registry.Save("baseurlmask", "https://old.example.com");
        this.Registry.Save("lastsearchindexupdated", "yesterday");

        await upgrade.UpgradeAsync();

        var bbCodes = this.Repository<BBCode>().Count(b => b.BoardID == TestDatabase.BoardId);
        var spamWords = this.Repository<SpamWords>().Count(s => s.BoardID == TestDatabase.BoardId);

        await upgrade.UpgradeAsync();

        this.Global("baseurlmask").Should().Be("https://old.example.com");
        this.Registry.Count(r => r.Name == "forceupdatesearchindex").Should().Be(0);

        this.Repository<BBCode>().Count(b => b.BoardID == TestDatabase.BoardId).Should().Be(bbCodes);
        this.Repository<SpamWords>().Count(s => s.BoardID == TestDatabase.BoardId).Should().Be(spamWords);
    }

    [Test]
    [Description("Missing tables are created, an upgrade from 4.0.0 imports the old private messages")]
    public async Task Upgrade_From_4_0_0_Test()
    {
        var upgrade = this.CreateUpgradeService();

        var alice = this.board.Database.AddUser("Alice");
        var bob = this.board.Database.AddUser("Bob");

        using (var db = this.board.Database.Open())
        {
            db.DropTable<Thanks>();

            db.CreateTable<PMessage>();
            db.CreateTable<UserPMessage>();

            var message = (int)db.Insert(
                new PMessage { FromUserID = alice, Created = DateTime.UtcNow, Subject = "Hi", Body = "old message" },
                selectIdentity: true);
            db.Insert(new UserPMessage { UserID = bob, PMessageID = message });
        }

        this.Registry.Save("version", "1000");

        await upgrade.UpgradeAsync();

        using (var db = this.board.Database.Open())
        {
            db.TableExists<Thanks>().Should().BeTrue();
            db.TableExists<PMessage>().Should().BeFalse();
            db.Select<PrivateMessage>().Should().ContainSingle(m => m.FromUserId == alice && m.ToUserId == bob);
        }
    }

    [Test]
    [Description("The upgrade is finished when the default BBCodes are imported")]
    public async Task Upgrade_Waits_For_BBCode_Import_Test()
    {
        var importer = Substitute.For<IDataImporter>();
        var import = new TaskCompletionSource<int>();
        importer.BBCodeExtensionImportAsync(Arg.Any<int>(), Arg.Any<Stream>()).Returns(import.Task);

        var upgrade = this.CreateUpgradeService(b => b.RegisterInstance(importer).As<IDataImporter>());

        this.Registry.Save("version", "1004");

        var upgrading = upgrade.UpgradeAsync();

        upgrading.IsCompleted.Should().BeFalse("the BBCodes are still imported");

        import.SetResult(1);
        (await upgrading).Should().BeTrue();
    }

    [Test]
    [Description("The old \"Registered\" role is renamed to \"Registered Users\", the name of the member group")]
    public async Task Upgrade_Renames_Registered_Role_Test()
    {
        var upgrade = this.CreateUpgradeService();

        this.InsertRoles(new AspNetRoles("r1", "Registered"));

        this.Registry.Save("version", "1004");

        await upgrade.UpgradeAsync();

        this.Repository<AspNetRoles>().Get(_ => true).Should().ContainSingle(r => r.Id == "r1" && r.Name == "Registered Users");
    }

    [Test]
    [Description("If both roles exist the members of \"Registered\" are moved to \"Registered Users\" and \"Registered\" is deleted")]
    public async Task Upgrade_Merges_Registered_Role_Test()
    {
        var upgrade = this.CreateUpgradeService();

        this.InsertRoles(new AspNetRoles("r1", "Registered"), new AspNetRoles("r2", "Registered Users"));

        using (var db = this.board.Database.Open())
        {
            db.Insert(new AspNetUserRoles { UserId = "alice", RoleId = "r1" });
            db.Insert(new AspNetUserRoles { UserId = "bob", RoleId = "r1" });
            db.Insert(new AspNetUserRoles { UserId = "bob", RoleId = "r2" });
        }

        this.Registry.Save("version", "1004");

        await upgrade.UpgradeAsync();

        this.Repository<AspNetRoles>().Get(_ => true).Select(r => r.Name).Should().Equal("Registered Users");
        this.Repository<AspNetUserRoles>().Get(_ => true).Select(r => $"{r.UserId}:{r.RoleId}").Should()
            .BeEquivalentTo(["alice:r2", "bob:r2"]);
    }

    private IRepository<Registry> Registry => this.Repository<Registry>();

    private void InsertRoles(params AspNetRoles[] roles)
    {
        using var db = this.board.Database.Open();

        foreach (var role in roles)
        {
            db.Insert(role);
        }
    }

    private string Global(string name)
    {
        return this.Registry.GetSingle(r => r.Name == name && r.BoardID == null)?.Value;
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }

    private int AddBoard()
    {
        using var db = this.board.Database.Open();

        return (int)db.Insert(new Board { Name = "Second Board" }, selectIdentity: true);
    }

    private UpgradeService CreateUpgradeService(Action<ContainerBuilder> configure = null)
    {
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<DataImporter>().As<IDataImporter>();

                configure?.Invoke(b);
            },
            useDatabase: true);

        this.board.HttpContext.Request.Scheme = "https";
        this.board.HttpContext.Request.Host = new Microsoft.AspNetCore.Http.HostString("forum.example.com");

        return new UpgradeService(
            this.board.Resolve<IServiceLocator>(),
            this.board.Events,
            this.board.Resolve<IDbAccess>());
    }
}
