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

namespace YAF.Data.Sqlite;

using System.Threading.Tasks;

using YAF.Types.Objects;

/// <summary>
/// The SQLite database backup: a copy of the database file (VACUUM INTO, a consistent copy while the forum runs).
/// </summary>
public class SqliteDbBackupProvider : IDbBackupProvider
{
    /// <summary>
    /// Gets the file extension of the backup file in the zip.
    /// </summary>
    public string FileExtension => ".db";

    /// <summary>
    /// The database file is opened by the application, its backup is always possible.
    /// </summary>
    /// <param name="dbAccess">The database access.</param>
    /// <param name="backupFolder">The backup folder of the application.</param>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    public Task<DbBackupSupport> CheckSupportAsync(IDbAccess dbAccess, string backupFolder)
    {
        return Task.FromResult(DbBackupSupport.Supported());
    }

    /// <summary>
    /// Creates the backup file.
    /// </summary>
    /// <param name="dbAccess">The database access.</param>
    /// <param name="filePath">The full path of the backup file.</param>
    /// <returns>The <see cref="Task"/>.</returns>
    public Task CreateBackupAsync(IDbAccess dbAccess, string filePath)
    {
        return dbAccess.ExecuteAsync(
            async db =>
            {
                await using var command = ((DbConnection)db.ToDbConnection()).CreateCommand();

                command.CommandText = "VACUUM INTO @path";
                command.CommandTimeout = 0;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@path";
                parameter.Value = filePath;
                command.Parameters.Add(parameter);

                return await command.ExecuteNonQueryAsync();
            });
    }
}
