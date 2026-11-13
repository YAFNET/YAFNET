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

namespace YAF.Core.Tasks;

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

/// <summary>
/// Creates the nightly database backup, when it is enabled (Admin > Database > Backup) and possible.
/// </summary>
public class DatabaseBackupTask : IntermittentBackgroundTask
{
    /// <summary>
    /// The first hour (server time) of the night in which the backup is made.
    /// </summary>
    public const int NightStartHour = 1;

    /// <summary>
    /// The hour (server time) when the night is over.
    /// </summary>
    public const int NightEndHour = 6;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseBackupTask"/> class.
    /// </summary>
    public DatabaseBackupTask()
    {
        // check every 15 minutes
        this.RunPeriodMs = 15 * 60 * 1000;
        this.StartDelayMs = 60 * 1000;
    }

    /// <summary>
    /// Gets the name of the task.
    /// </summary>
    public override string TaskName => nameof(DatabaseBackupTask);

    /// <summary>
    /// Gets the task description.
    /// </summary>
    public override string TaskDescription => "Creates the nightly database backup.";

    /// <summary>
    /// Checks if the backup of the night is due.
    /// </summary>
    /// <param name="enabled">If the nightly backup is enabled.</param>
    /// <param name="lastBackup">The time of the last backup (UTC, round-trip format), or empty.</param>
    /// <param name="now">The current server time.</param>
    /// <returns>If a backup is due.</returns>
    public static bool IsBackupDue(bool enabled, string lastBackup, DateTime now)
    {
        if (!enabled || now.Hour < NightStartHour || now.Hour >= NightEndHour)
        {
            return false;
        }

        if (lastBackup.IsNotSet()
            || !DateTime.TryParse(lastBackup, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var last))
        {
            return true;
        }

        // one backup per night
        return now.ToUniversalTime() - last.ToUniversalTime() > TimeSpan.FromHours(NightEndHour - NightStartHour + 1);
    }

    /// <summary>
    /// Creates the backup when it is due.
    /// </summary>
    public async override Task RunOnceAsync()
    {
        try
        {
            var boardSettings = this.Get<BoardSettings>();

            if (!IsBackupDue(boardSettings.DatabaseBackupEnabled, boardSettings.LastDatabaseBackup, DateTime.Now))
            {
                return;
            }

            var backupService = this.Get<IDatabaseBackupService>();

            if (!(await backupService.GetSupportAsync()).IsSupported)
            {
                return;
            }

            var backup = await backupService.CreateBackupAsync();

            this.Get<ILogger<DatabaseBackupTask>>().Info($"Database backup created: {backup.Name}");
        }
        catch (Exception ex)
        {
            this.Get<ILogger<DatabaseBackupTask>>().Error(ex, $"Error In {this.TaskName} Task");
        }
    }
}
