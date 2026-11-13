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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Autofac.Features.Indexed;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Creates, lists and deletes the zipped database backups, with the native backup of the database type
/// (the <see cref="IDbBackupProvider"/> named with the provider name of the database access).
/// </summary>
public partial class DatabaseBackupService : IDatabaseBackupService, IHaveServiceLocator
{
    /// <summary>
    /// How long the result of the support check is cached.
    /// </summary>
    private static readonly TimeSpan SupportCacheDuration = TimeSpan.FromHours(1);

    /// <summary>
    /// Only one backup at a time (the nightly task and the "Backup now" button).
    /// </summary>
    private static readonly SemaphoreSlim BackupLock = new(1, 1);

    /// <summary>
    /// The cached result of the support check.
    /// </summary>
    private static (DbBackupSupport Support, DateTime ValidUntil)? cachedSupport;

    /// <summary>
    /// The backup providers, by the provider name.
    /// </summary>
    private readonly IIndex<string, IDbBackupProvider> backupProviders;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseBackupService"/> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    /// <param name="backupProviders">The backup providers.</param>
    public DatabaseBackupService(IServiceLocator serviceLocator, IIndex<string, IDbBackupProvider> backupProviders)
    {
        this.ServiceLocator = serviceLocator;
        this.backupProviders = backupProviders;
    }

    /// <summary>
    /// Gets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; }

    /// <summary>
    /// Gets the full path of the backup folder of the application.
    /// </summary>
    public string BackupFolder
    {
        get
        {
            var folder = this.Get<BoardConfiguration>().BackupFolder;

            if (folder.IsNotSet())
            {
                folder = Path.Combine("App_Data", "Backups");
            }

            return Path.GetFullPath(Path.IsPathRooted(folder) ? folder : Path.Combine(this.ContentRootPath, folder));
        }
    }

    /// <summary>
    /// Gets the content root path of the application.
    /// </summary>
    private string ContentRootPath =>
        this.ServiceLocator.TryGet(typeof(IWebHostEnvironment), out var environment)
            ? ((IWebHostEnvironment)environment).ContentRootPath
            : AppContext.BaseDirectory;

    /// <summary>
    /// Checks if a database backup can be made (the result is cached for an hour).
    /// </summary>
    /// <param name="refresh">Check again, without the cached result.</param>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    public async Task<DbBackupSupport> GetSupportAsync(bool refresh = false)
    {
        var cached = cachedSupport;

        if (!refresh && cached.HasValue && cached.Value.ValidUntil > DateTime.UtcNow)
        {
            return cached.Value.Support;
        }

        var support = await this.CheckSupportAsync();

        CacheSupport(support);

        return support;
    }

    /// <summary>
    /// Checks if a database backup can be made (the result is cached for an hour).
    /// </summary>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    public DbBackupSupport GetSupport()
    {
        var cached = cachedSupport;

        return cached.HasValue && cached.Value.ValidUntil > DateTime.UtcNow
                   ? cached.Value.Support
                   : this.GetSupportAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Creates a zipped backup of the database, deletes the oldest backups (the number of backups to keep of the host settings)
    /// and saves the time of the last backup.
    /// </summary>
    /// <returns>The new backup file.</returns>
    public async Task<DbBackupFile> CreateBackupAsync()
    {
        var support = await this.GetSupportAsync();

        if (!support.IsSupported)
        {
            throw new InvalidOperationException(support.Reason);
        }

        if (!await BackupLock.WaitAsync(0))
        {
            throw new InvalidOperationException("A database backup is already running.");
        }

        try
        {
            var dbAccess = this.Get<IDbAccess>();
            var provider = this.backupProviders[dbAccess.Information.ProviderName];
            var folder = this.BackupFolder;

            Directory.CreateDirectory(folder);

            var created = DateTime.UtcNow;
            var name = $"yaf-backup-{created:yyyyMMdd-HHmmss}";
            var backupPath = Path.Combine(folder, $"{name}{provider.FileExtension}");
            var zipPath = Path.Combine(folder, $"{name}.zip");
            var tempZipPath = $"{zipPath}.tmp";

            try
            {
                await provider.CreateBackupAsync(dbAccess, backupPath);

                if (!File.Exists(backupPath))
                {
                    // e.g. SQL Server wrote the file into a folder that is not shared with the application
                    var reason = $"The backup file was not created in the backup folder of the application ({folder}).";

                    CacheSupport(DbBackupSupport.NotSupported(reason));

                    throw new InvalidOperationException(reason);
                }

                // the zip is only visible (and downloadable) when it is complete
                await using (var zipStream = File.Create(tempZipPath))
                {
                    await using var zip = new ZipArchive(zipStream, ZipArchiveMode.Create);

                    await zip.CreateEntryFromFileAsync(backupPath, Path.GetFileName(backupPath), CompressionLevel.Optimal);
                }

                File.Move(tempZipPath, zipPath, true);
            }
            finally
            {
                this.DeleteFile(backupPath);
                this.DeleteFile(tempZipPath);
            }

            this.SaveLastBackup(created);

            this.DeleteOldBackups(this.Get<BoardSettings>().DatabaseBackupKeep);

            return new DbBackupFile { Name = Path.GetFileName(zipPath), Size = new FileInfo(zipPath).Length, Created = created };
        }
        finally
        {
            BackupLock.Release();
        }
    }

    /// <summary>
    /// Gets the backups, the newest first.
    /// </summary>
    /// <returns>The backups.</returns>
    public List<DbBackupFile> GetBackups()
    {
        var folder = this.BackupFolder;

        if (!Directory.Exists(folder))
        {
            return [];
        }

        return [.. new DirectoryInfo(folder).EnumerateFiles("yaf-backup-*.zip")
            .Select(file => (File: file, Match: BackupFileName().Match(file.Name)))
            .Where(x => x.Match.Success)
            .Select(x => new DbBackupFile
            {
                Name = x.File.Name,
                Size = x.File.Length,
                Created = DateTime.ParseExact(
                    x.Match.Groups["time"].Value,
                    "yyyyMMdd-HHmmss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)
            })
            .OrderByDescending(x => x.Created)];
    }

    /// <summary>
    /// Gets the full path of a backup.
    /// </summary>
    /// <param name="name">The file name of the backup.</param>
    /// <returns>The full path, or null when there is no backup with this name.</returns>
    public string GetBackupFilePath(string name)
    {
        // only the names of the backups, no paths (e.g. "../")
        if (name.IsNotSet() || !BackupFileName().IsMatch(name))
        {
            return null;
        }

        var path = Path.Combine(this.BackupFolder, name);

        return File.Exists(path) ? path : null;
    }

    /// <summary>
    /// Deletes a backup.
    /// </summary>
    /// <param name="name">The file name of the backup.</param>
    /// <returns>If the backup was deleted.</returns>
    public bool DeleteBackup(string name)
    {
        var path = this.GetBackupFilePath(name);

        if (path is null)
        {
            return false;
        }

        File.Delete(path);

        return true;
    }

    /// <summary>
    /// Caches the result of the support check.
    /// </summary>
    /// <param name="support">The support.</param>
    private static void CacheSupport(DbBackupSupport support)
    {
        cachedSupport = (support, DateTime.UtcNow.Add(SupportCacheDuration));
    }

    /// <summary>
    /// The file name of a backup: yaf-backup-yyyyMMdd-HHmmss.zip
    /// </summary>
    [GeneratedRegex(@"^yaf-backup-(?<time>\d{8}-\d{6})\.zip$")]
    private static partial Regex BackupFileName();

    /// <summary>
    /// Checks if a database backup can be made.
    /// </summary>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    private async Task<DbBackupSupport> CheckSupportAsync()
    {
        try
        {
            var dbAccess = this.Get<IDbAccess>();

            if (!this.backupProviders.TryGetValue(dbAccess.Information.ProviderName, out var provider))
            {
                return DbBackupSupport.NotSupported($"There is no database backup for {dbAccess.Information.ProviderName}.");
            }

            var folder = this.BackupFolder;

            // the backups must not be downloadable as static files
            if (this.ServiceLocator.TryGet(typeof(IWebHostEnvironment), out var environment)
                && ((IWebHostEnvironment)environment).WebRootPath is { } webRoot
                && IsInFolder(folder, webRoot))
            {
                return DbBackupSupport.NotSupported($"The backup folder ({folder}) must not be in wwwroot.");
            }

            try
            {
                Directory.CreateDirectory(folder);

                var probe = Path.Combine(folder, $"yaf-write-check-{Guid.NewGuid():N}.tmp");

                await File.WriteAllTextAsync(probe, string.Empty);
                File.Delete(probe);
            }
            catch (Exception ex)
            {
                return DbBackupSupport.NotSupported($"The application can't write to the backup folder ({folder}): {ex.Message}");
            }

            return await provider.CheckSupportAsync(dbAccess, folder);
        }
        catch (Exception ex)
        {
            this.Get<ILogger<DatabaseBackupService>>().Error(ex, "Database backup support check failed");

            return DbBackupSupport.NotSupported(ex.Message);
        }
    }

    /// <summary>
    /// Checks if a folder is the same or a sub folder of another folder.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="parent">The parent folder.</param>
    /// <returns>If the folder is in the parent folder.</returns>
    private static bool IsInFolder(string folder, string parent)
    {
        var full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder)) + Path.DirectorySeparatorChar;
        var fullParent = Path.TrimEndingDirectorySeparator(Path.GetFullPath(parent)) + Path.DirectorySeparatorChar;

        return full.StartsWith(fullParent, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Saves the time of the last backup (host setting).
    /// </summary>
    /// <param name="created">The time of the backup.</param>
    private void SaveLastBackup(DateTime created)
    {
        var value = created.ToString("o", CultureInfo.InvariantCulture);

        this.Get<BoardSettings>().LastDatabaseBackup = value;

        this.GetRepository<Registry>().Save(nameof(BoardSettings.LastDatabaseBackup), value);
    }

    /// <summary>
    /// Deletes the oldest backups.
    /// </summary>
    /// <param name="keep">The number of backups to keep (0 = all).</param>
    private void DeleteOldBackups(int keep)
    {
        if (keep <= 0)
        {
            return;
        }

        this.GetBackups().Skip(keep).ForEach(backup => this.DeleteFile(Path.Combine(this.BackupFolder, backup.Name)));
    }

    /// <summary>
    /// Deletes a file, a failure is only logged.
    /// </summary>
    /// <param name="path">The path.</param>
    private void DeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            this.Get<ILogger<DatabaseBackupService>>().Warn("Could not delete {0}: {1}", path, ex.Message);
        }
    }
}
