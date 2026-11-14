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
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using YAF.Configuration;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Core.Tasks;
using YAF.Data.Sqlite;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The database backup (Admin > Database > Backup): the zipped SQLite backup, the list, download path and delete,
/// the number of backups to keep, the support check and the nightly schedule.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DatabaseBackupTests
{
    private string folder;

    private TestBoardContext board;

    private IDatabaseBackupService service;

    [SetUp]
    public async Task SetUp()
    {
        this.folder = Path.Combine(Path.GetTempPath(), $"yaf-backup-tests-{Guid.NewGuid():N}");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(new BoardConfiguration { BackupFolder = this.folder });
                b.RegisterType<SqliteDbBackupProvider>().Named<IDbBackupProvider>(SqliteDbAccess.ProviderTypeName);
                b.RegisterType<DatabaseBackupService>().As<IDatabaseBackupService>();
            },
            useDatabase: true);

        this.service = this.board.Resolve<IDatabaseBackupService>();

        // the result of the check is cached (static) for an hour
        await this.service.GetSupportAsync(true);
    }

    [TearDown]
    public void TearDown()
    {
        this.board.Dispose();

        if (Directory.Exists(this.folder))
        {
            Directory.Delete(this.folder, true);
        }
    }

    [Test]
    [Description("A backup is a zip in the backup folder with a copy of the SQLite database, and the time of the backup is saved")]
    public async Task Backup_Creates_Zip_With_Database_Test()
    {
        var backup = await this.service.CreateBackupAsync();

        backup.Name.Should().MatchRegex(@"^yaf-backup-\d{8}-\d{6}\.zip$");

        var zipPath = Path.Combine(this.folder, backup.Name);
        File.Exists(zipPath).Should().BeTrue();
        new FileInfo(zipPath).Length.Should().Be(backup.Size);

        // only the zip is left in the folder
        Directory.GetFiles(this.folder).Should().ContainSingle();

        var extracted = Path.Combine(this.folder, "extracted.db");

        await using (var zip = await ZipFile.OpenReadAsync(zipPath))
        {
            zip.Entries.Should().ContainSingle().Which.Name.Should().Be(Path.ChangeExtension(backup.Name, ".db"));
            await zip.Entries[0].ExtractToFileAsync(extracted);
        }

        // the copy is a database with the forum tables and data
        await using (var connection = new SQLiteConnection($"Data Source={extracted};Pooling=False"))
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM yaf_Board";
            Convert.ToInt32(await command.ExecuteScalarAsync(), CultureInfo.InvariantCulture).Should().Be(1);
        }

        File.Delete(extracted);

        var saved = await this.board.Current.GetRepository<Registry>()
            .GetSingleAsync(r => r.Name == "lastdatabasebackup" && r.BoardID == null);

        saved.Should().NotBeNull();
        DateTime.Parse(saved.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            .Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        this.board.BoardSettings.LastDatabaseBackup.Should().Be(saved.Value);
    }

    [Test]
    [Description("After a backup only the newest backups are kept (the number of the setting), 0 keeps all")]
    public async Task Backup_Deletes_Oldest_Backups_Test()
    {
        Directory.CreateDirectory(this.folder);

        foreach (var name in new[] { "yaf-backup-20260101-010000.zip", "yaf-backup-20260102-010000.zip", "yaf-backup-20260103-010000.zip" })
        {
            await File.WriteAllTextAsync(Path.Combine(this.folder, name), "old");
        }

        this.board.BoardSettings.DatabaseBackupKeep = 2;

        var backup = await this.service.CreateBackupAsync();

        this.service.GetBackups().Select(b => b.Name).Should()
            .Equal(backup.Name, "yaf-backup-20260103-010000.zip");

        this.board.BoardSettings.DatabaseBackupKeep = 0;

        await File.WriteAllTextAsync(Path.Combine(this.folder, "yaf-backup-20250101-010000.zip"), "old");

        // a backup in the same second has the same name
        await Task.Delay(1100);
        await this.service.CreateBackupAsync();

        this.service.GetBackups().Should().HaveCount(4);
    }

    [Test]
    [Description("The list has only the backups (newest first), other files in the folder are ignored")]
    public async Task List_Only_Backups_Newest_First_Test()
    {
        Directory.CreateDirectory(this.folder);

        await File.WriteAllTextAsync(Path.Combine(this.folder, "yaf-backup-20260101-010000.zip"), "a");
        await File.WriteAllTextAsync(Path.Combine(this.folder, "yaf-backup-20260301-010000.zip"), "bb");
        await File.WriteAllTextAsync(Path.Combine(this.folder, "yaf-backup-20260201-010000.zip.tmp"), "c");
        await File.WriteAllTextAsync(Path.Combine(this.folder, "other.zip"), "d");

        var backups = this.service.GetBackups();

        backups.Select(b => b.Name).Should().Equal("yaf-backup-20260301-010000.zip", "yaf-backup-20260101-010000.zip");
        backups[0].Size.Should().Be(2);
        backups[0].Created.Should().Be(new DateTime(2026, 3, 1, 1, 0, 0, DateTimeKind.Utc));
        backups[0].Created.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    [Description("Download and delete only accept the names of existing backups, no paths")]
    [TestCase("../yaf-backup-20260101-010000.zip")]
    [TestCase("..\\yaf-backup-20260101-010000.zip")]
    [TestCase("yaf-backup-20260101-010000.zip/../../secret.txt")]
    [TestCase("other.zip")]
    [TestCase("yaf-backup-20260102-010000.zip")]
    [TestCase("")]
    [TestCase(null)]
    public async Task Invalid_Backup_Names_Test(string name)
    {
        Directory.CreateDirectory(this.folder);
        await File.WriteAllTextAsync(Path.Combine(this.folder, "yaf-backup-20260101-010000.zip"), "a");
        await File.WriteAllTextAsync(Path.Combine(this.folder, "other.zip"), "b");

        this.service.GetBackupFilePath(name).Should().BeNull();
        this.service.DeleteBackup(name).Should().BeFalse();

        Directory.GetFiles(this.folder).Should().HaveCount(2);
    }

    [Test]
    [Description("A backup can be downloaded and deleted by its name")]
    public async Task Download_Path_And_Delete_Test()
    {
        Directory.CreateDirectory(this.folder);

        const string name = "yaf-backup-20260101-010000.zip";

        await File.WriteAllTextAsync(Path.Combine(this.folder, name), "a");

        this.service.GetBackupFilePath(name).Should().Be(Path.Combine(Path.GetFullPath(this.folder), name));

        this.service.DeleteBackup(name).Should().BeTrue();

        File.Exists(Path.Combine(this.folder, name)).Should().BeFalse();
        this.service.GetBackupFilePath(name).Should().BeNull();
    }

    [Test]
    [Description("A relative backup folder is in the application folder (content root)")]
    public void Relative_Backup_Folder_Test()
    {
        using var testBoard = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(new BoardConfiguration { BackupFolder = "App_Data/Backups" });
                b.RegisterInstance(Environment("C:/app", "C:/app/wwwroot")).As<IWebHostEnvironment>();
                b.RegisterType<DatabaseBackupService>().As<IDatabaseBackupService>();
            });

        testBoard.Resolve<IDatabaseBackupService>().BackupFolder.Should()
            .Be(Path.GetFullPath(Path.Combine("C:/app", "App_Data", "Backups")));
    }

    [Test]
    [Description("A backup folder in wwwroot is not supported: the backups could be downloaded by everybody")]
    public async Task Backup_Folder_In_WebRoot_Not_Supported_Test()
    {
        using var testBoard = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(new BoardConfiguration { BackupFolder = Path.Combine(this.folder, "wwwroot", "backups") });
                b.RegisterInstance(Environment(this.folder, Path.Combine(this.folder, "wwwroot"))).As<IWebHostEnvironment>();
                b.RegisterType<SqliteDbBackupProvider>().Named<IDbBackupProvider>(SqliteDbAccess.ProviderTypeName);
                b.RegisterType<DatabaseBackupService>().As<IDatabaseBackupService>();
            },
            useDatabase: true);

        var backupService = testBoard.Resolve<IDatabaseBackupService>();

        var support = await backupService.GetSupportAsync(true);

        support.IsSupported.Should().BeFalse();
        support.Reason.Should().Contain("wwwroot");

        await FluentActions.Awaiting(() => backupService.CreateBackupAsync())
            .Should().ThrowAsync<InvalidOperationException>();

        // the not supported result is cached for the menu (GetSupport)
        #pragma warning disable S6966 // the synchronous method is tested
        backupService.GetSupport().IsSupported.Should().BeFalse();
        #pragma warning restore S6966
    }

    [Test]
    [Description("Without a backup provider for the database type the backup is not supported")]
    public async Task No_Provider_Not_Supported_Test()
    {
        using var testBoard = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(new BoardConfiguration { BackupFolder = this.folder });
                b.RegisterType<DatabaseBackupService>().As<IDatabaseBackupService>();
            },
            useDatabase: true);

        var support = await testBoard.Resolve<IDatabaseBackupService>().GetSupportAsync(true);

        support.IsSupported.Should().BeFalse();
        support.Reason.Should().Contain(SqliteDbAccess.ProviderTypeName);
    }

    [Test]
    [Description("When the backup file is not in the folder (e.g. SQL Server wrote it into a folder the application doesn't share), the backup fails and is not supported anymore")]
    public async Task Backup_File_Not_In_Folder_Test()
    {
        var provider = Substitute.For<IDbBackupProvider>();
        provider.FileExtension.Returns(".bak");
        provider.CheckSupportAsync(Arg.Any<IDbAccess>(), Arg.Any<string>()).Returns(DbBackupSupport.Supported());

        using var testBoard = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(new BoardConfiguration { BackupFolder = this.folder });
                b.RegisterInstance(provider).Named<IDbBackupProvider>(SqliteDbAccess.ProviderTypeName);
                b.RegisterType<DatabaseBackupService>().As<IDatabaseBackupService>();
            },
            useDatabase: true);

        var backupService = testBoard.Resolve<IDatabaseBackupService>();

        (await backupService.GetSupportAsync(true)).IsSupported.Should().BeTrue();

        await FluentActions.Awaiting(() => backupService.CreateBackupAsync())
            .Should().ThrowAsync<InvalidOperationException>().WithMessage("*not created in the backup folder*");

        (await backupService.GetSupportAsync()).IsSupported.Should().BeFalse();
        backupService.GetBackups().Should().BeEmpty();
    }

    [Test]
    [Description("The backup contains the uploads and images of wwwroot (in the folder wwwroot of the zip), not the other files of wwwroot")]
    public async Task Backup_Contains_Uploads_And_Images_Test()
    {
        var webRoot = Path.Combine(this.folder, "site", "wwwroot");
        var backupFolder = Path.Combine(this.folder, "backups");

        Directory.CreateDirectory(Path.Combine(webRoot, "uploads"));
        Directory.CreateDirectory(Path.Combine(webRoot, "images", "Avatars"));
        Directory.CreateDirectory(Path.Combine(webRoot, "js"));

        await File.WriteAllTextAsync(Path.Combine(webRoot, "uploads", "attachment.yafupload"), "upload");
        await File.WriteAllTextAsync(Path.Combine(webRoot, "images", "Avatars", "avatar.png"), "avatar");
        await File.WriteAllTextAsync(Path.Combine(webRoot, "js", "forum.js"), "script");

        using var testBoard = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(new BoardConfiguration { BackupFolder = backupFolder });
                b.RegisterInstance(Environment(Path.Combine(this.folder, "site"), webRoot)).As<IWebHostEnvironment>();
                b.RegisterType<SqliteDbBackupProvider>().Named<IDbBackupProvider>(SqliteDbAccess.ProviderTypeName);
                b.RegisterType<DatabaseBackupService>().As<IDatabaseBackupService>();
            },
            useDatabase: true);

        var backupService = testBoard.Resolve<IDatabaseBackupService>();

        (await backupService.GetSupportAsync(true)).IsSupported.Should().BeTrue();

        var backup = await backupService.CreateBackupAsync();

        await using var zip = await ZipFile.OpenReadAsync(Path.Combine(backupFolder, backup.Name));

        zip.Entries.Select(e => e.FullName).Should().BeEquivalentTo(
            Path.ChangeExtension(backup.Name, ".db"),
            "wwwroot/uploads/attachment.yafupload",
            "wwwroot/images/Avatars/avatar.png");

        using var reader = new StreamReader(await zip.GetEntry("wwwroot/images/Avatars/avatar.png")!.OpenAsync());
        (await reader.ReadToEndAsync()).Should().Be("avatar");
    }

    [Test]
    [Description("The folders of the files in the backup: uploads and images, in the static files root, of each board with the board folders")]
    public void File_Folders_Test()
    {
        DatabaseBackupService.GetFileFolders(new BoardConfiguration(), [1, 2]).Should()
            .Equal("uploads", "images");

        DatabaseBackupService.GetFileFolders(new BoardConfiguration { StaticFilesRoot = "Forums/" }, [1]).Should()
            .Equal("Forums/uploads", "Forums/images");

        DatabaseBackupService.GetFileFolders(
                new BoardConfiguration { StaticFilesRoot = "Forums/", MultiBoardFolders = true },
                [1, 2]).Should()
            .Equal("Forums/1/uploads", "Forums/1/images", "Forums/2/uploads", "Forums/2/images");
    }

    [Test]
    [Description("The nightly backup runs once a night between 1 and 6 o'clock (server time), only when it is enabled")]
    [TestCase(false, null, "2026-11-13T03:00:00", false, TestName = "Disabled")]
    [TestCase(true, null, "2026-11-13T00:59:00", false, TestName = "Before the night")]
    [TestCase(true, null, "2026-11-13T06:00:00", false, TestName = "After the night")]
    [TestCase(true, null, "2026-11-13T01:00:00", true, TestName = "Never made")]
    [TestCase(true, "invalid", "2026-11-13T03:00:00", true, TestName = "Invalid last backup")]
    [TestCase(true, "2026-11-12T01:30:00.0000000Z", "2026-11-13T01:15:00", true, TestName = "Last night")]
    [TestCase(true, "2026-11-13T01:30:00.0000000Z", "2026-11-13T05:45:00", false, TestName = "This night")]
    public void Backup_Due_Test(bool enabled, string lastBackup, string now, bool due)
    {
        // the server time of the test is UTC
        var serverTime = DateTime.SpecifyKind(DateTime.Parse(now, CultureInfo.InvariantCulture), DateTimeKind.Utc);

        DatabaseBackupTask.IsBackupDue(enabled, lastBackup, serverTime).Should().Be(due);
    }

    [Test]
    [Description("The backup page is a host admin page (it can't be granted to the admins of a board)")]
    public void Backup_Page_Is_Host_Page_Test()
    {
        ForumPages.Admin_Backup.IsHostAdminPage().Should().BeTrue();
    }

    private static IWebHostEnvironment Environment(string contentRoot, string webRoot)
    {
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.ContentRootPath.Returns(contentRoot);
        environment.WebRootPath.Returns(webRoot);
        return environment;
    }
}
