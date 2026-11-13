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

namespace YAF.Types.Interfaces.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using YAF.Types.Objects;

/// <summary>
/// Creates, lists and deletes the zipped database backups (Admin > Database > Backup).
/// </summary>
public interface IDatabaseBackupService
{
    /// <summary>
    /// Gets the full path of the backup folder of the application.
    /// </summary>
    string BackupFolder { get; }

    /// <summary>
    /// Checks if a database backup can be made (the result is cached for an hour).
    /// </summary>
    /// <param name="refresh">Check again, without the cached result.</param>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    Task<DbBackupSupport> GetSupportAsync(bool refresh = false);

    /// <summary>
    /// Checks if a database backup can be made (the result is cached for an hour).
    /// </summary>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    DbBackupSupport GetSupport();

    /// <summary>
    /// Creates a zipped backup of the database, deletes the oldest backups (the number of backups to keep of the host settings)
    /// and saves the time of the last backup.
    /// </summary>
    /// <returns>The new backup file.</returns>
    Task<DbBackupFile> CreateBackupAsync();

    /// <summary>
    /// Gets the backups, the newest first.
    /// </summary>
    /// <returns>The backups.</returns>
    List<DbBackupFile> GetBackups();

    /// <summary>
    /// Gets the full path of a backup.
    /// </summary>
    /// <param name="name">The file name of the backup.</param>
    /// <returns>The full path, or null when there is no backup with this name.</returns>
    string GetBackupFilePath(string name);

    /// <summary>
    /// Deletes a backup.
    /// </summary>
    /// <param name="name">The file name of the backup.</param>
    /// <returns>If the backup was deleted.</returns>
    bool DeleteBackup(string name);
}
