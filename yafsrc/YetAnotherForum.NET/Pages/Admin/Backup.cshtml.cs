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

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using YAF.Configuration;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Tasks;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Admin Database Backup Page: the nightly backup and the zipped backups to download.
/// </summary>
public class BackupModel : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BackupModel"/> class.
    /// </summary>
    public BackupModel()
        : base("ADMIN_BACKUP", ForumPages.Admin_Backup)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether the nightly backup is enabled.
    /// </summary>
    [BindProperty]
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the number of backups to keep.
    /// </summary>
    [BindProperty]
    public int Keep { get; set; }

    /// <summary>
    /// Gets or sets the time of the last backup.
    /// </summary>
    public DateTime? LastBackup { get; set; }

    /// <summary>
    /// Gets or sets the backup folder.
    /// </summary>
    public string BackupFolder { get; set; }

    /// <summary>
    /// Gets or sets a note of the support check (e.g. a check that is made by the first backup).
    /// </summary>
    public string SupportNote { get; set; }

    /// <summary>
    /// Gets or sets the backups.
    /// </summary>
    public List<DbBackupFile> Backups { get; set; }

    /// <summary>
    /// Only the host admin can use this page (it works on the whole database), and only when a backup can be made.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        if (!this.PageBoardContext.PageUser.UserFlags.IsHostAdmin
            || !this.Get<IDatabaseBackupService>().GetSupport().IsSupported)
        {
            context.Result = this.Get<ILinkBuilder>().AccessDenied();
        }
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BACKUP", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Shows the settings and the backups.
    /// </summary>
    public void OnGet()
    {
        this.Enabled = this.PageBoardContext.BoardSettings.DatabaseBackupEnabled;
        this.Keep = this.PageBoardContext.BoardSettings.DatabaseBackupKeep;

        this.BindData();
    }

    /// <summary>
    /// Saves the settings of the nightly backup.
    /// </summary>
    /// <returns>The <see cref="IActionResult"/>.</returns>
    public IActionResult OnPostSave()
    {
        var boardSettings = this.PageBoardContext.BoardSettings;

        boardSettings.DatabaseBackupEnabled = this.Enabled;
        boardSettings.DatabaseBackupKeep = Math.Max(0, this.Keep);

        // host settings (for all boards)
        this.GetRepository<Registry>().Save(
            nameof(BoardSettings.DatabaseBackupEnabled),
            boardSettings.Registry[nameof(BoardSettings.DatabaseBackupEnabled)]);
        this.GetRepository<Registry>().Save(
            nameof(BoardSettings.DatabaseBackupKeep),
            boardSettings.Registry[nameof(BoardSettings.DatabaseBackupKeep)]);

        if (this.Enabled)
        {
            this.Get<ITaskModuleManager>().StartTask(nameof(DatabaseBackupTask), () => new DatabaseBackupTask());
        }

        this.Keep = boardSettings.DatabaseBackupKeep;

        this.BindData();

        return this.PageBoardContext.Notify(this.GetText("ADMIN_BACKUP", "MSG_SAVED"), MessageTypes.success);
    }

    /// <summary>
    /// Creates a backup now.
    /// </summary>
    /// <returns>The <see cref="IActionResult"/>.</returns>
    public async Task<IActionResult> OnPostBackupAsync()
    {
        try
        {
            var backup = await this.Get<IDatabaseBackupService>().CreateBackupAsync();

            this.ReadSettings();
            this.BindData();

            return this.PageBoardContext.Notify(
                this.GetTextFormatted("MSG_CREATED", backup.Name),
                MessageTypes.success);
        }
        catch (Exception ex)
        {
            this.Get<ILogger<BackupModel>>().Error(ex, "Database backup failed");

            this.ReadSettings();
            this.BindData();

            return this.PageBoardContext.Notify(this.GetTextFormatted("MSG_FAILED", ex.Message), MessageTypes.danger);
        }
    }

    /// <summary>
    /// Downloads a backup.
    /// </summary>
    /// <param name="name">The file name of the backup.</param>
    /// <returns>The <see cref="IActionResult"/>.</returns>
    public IActionResult OnPostDownload(string name)
    {
        var path = this.Get<IDatabaseBackupService>().GetBackupFilePath(name);

        if (path is null)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        return this.PhysicalFile(path, "application/zip", name);
    }

    /// <summary>
    /// Deletes a backup.
    /// </summary>
    /// <param name="name">The file name of the backup.</param>
    /// <returns>The <see cref="IActionResult"/>.</returns>
    public IActionResult OnPostDelete(string name)
    {
        var deleted = this.Get<IDatabaseBackupService>().DeleteBackup(name);

        this.ReadSettings();
        this.BindData();

        return deleted
                   ? this.PageBoardContext.Notify(this.GetText("ADMIN_BACKUP", "MSG_DELETED"), MessageTypes.success)
                   : this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
    }

    /// <summary>
    /// Reads the settings of the nightly backup.
    /// </summary>
    private void ReadSettings()
    {
        this.Enabled = this.PageBoardContext.BoardSettings.DatabaseBackupEnabled;
        this.Keep = this.PageBoardContext.BoardSettings.DatabaseBackupKeep;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        var backupService = this.Get<IDatabaseBackupService>();

        this.BackupFolder = backupService.BackupFolder;
        this.SupportNote = backupService.GetSupport().Reason;
        this.Backups = backupService.GetBackups();

        var lastBackup = this.PageBoardContext.BoardSettings.LastDatabaseBackup;

        this.LastBackup = lastBackup.IsSet()
                          && DateTime.TryParse(
                              lastBackup,
                              CultureInfo.InvariantCulture,
                              DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                              out var last)
                              ? last
                              : null;
    }
}
