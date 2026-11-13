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

namespace YAF.Data.SqlServer;

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Types.Objects;

/// <summary>
/// The SQL Server database backup (BACKUP DATABASE ... TO DISK). SQL Server writes the file itself, on its own disk:
/// the backup folder must be shared by SQL Server and the application (configured as SqlServerBackupFolder).
/// </summary>
public class SqlServerDbBackupProvider : IDbBackupProvider
{
    /// <summary>
    /// The engine editions without a backup to a disk: Azure SQL Database, Azure Synapse, Azure SQL Managed Instance,
    /// Azure Synapse serverless and SQL database in Microsoft Fabric.
    /// </summary>
    private static readonly int[] CloudEditions = [5, 6, 8, 11, 12];

    /// <summary>
    /// The board configuration.
    /// </summary>
    private readonly BoardConfiguration boardConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerDbBackupProvider"/> class.
    /// </summary>
    /// <param name="boardConfiguration">The board configuration.</param>
    public SqlServerDbBackupProvider(IServiceLocator serviceLocator, BoardConfiguration boardConfiguration)
    {
        this.ServiceLocator = serviceLocator;
        this.boardConfiguration = boardConfiguration;
    }

    /// <summary>
    /// Gets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; }

    /// <summary>
    /// Gets the file extension of the backup file in the zip.
    /// </summary>
    public string FileExtension => ".bak";

    /// <summary>
    /// Gets the content root path of the application.
    /// </summary>
    private string ContentRootPath =>
        this.ServiceLocator.TryGet(typeof(IWebHostEnvironment), out var environment)
            ? ((IWebHostEnvironment)environment).ContentRootPath
            : AppContext.BaseDirectory;

    /// <summary>
    /// Gets the backup folder as SQL Server sees it. An absolute path is used as it is (SQL Server can run on another
    /// operating system, e.g. /var/opt/mssql/backup in a container), a relative path (SQL Server on the same machine)
    /// is relative to the content root of the application.
    /// </summary>
    public string SqlServerBackupFolder {
        get {
            var folder = this.boardConfiguration.SqlServerBackupFolder;

            if (folder.IsNotSet())
            {
                folder = Path.Combine("App_Data", "Backups");
            }

            if (Path.IsPathRooted(folder) || folder.StartsWith('/') || folder.StartsWith('\\'))
            {
                return folder;
            }

            return Path.GetFullPath(Path.Combine(this.ContentRootPath, folder));
        }
    }

    /// <summary>
    /// Checks the configuration, the SQL Server edition, the permission and if SQL Server sees the backup folder of the application.
    /// </summary>
    /// <param name="dbAccess">The database access.</param>
    /// <param name="backupFolder">The backup folder of the application.</param>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    public async Task<DbBackupSupport> CheckSupportAsync(IDbAccess dbAccess, string backupFolder)
    {
        var sqlServerFolder = this.SqlServerBackupFolder;

        if (sqlServerFolder.IsNotSet())
        {
            return DbBackupSupport.NotSupported(
                "SQL Server writes the backup on its own disk: the backup needs a folder that SQL Server and the application share, "
                + "configured as BoardConfiguration:SqlServerBackupFolder (the folder as SQL Server sees it) and BoardConfiguration:BackupFolder (as the application sees it).");
        }

        var edition = await ScalarAsync<int>(dbAccess, "SELECT CAST(SERVERPROPERTY('EngineEdition') AS int)");

        if (CloudEditions.Contains(edition))
        {
            return DbBackupSupport.NotSupported("Azure SQL doesn't support backups to a disk.");
        }

        if (await ScalarAsync<int>(dbAccess, "SELECT HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'BACKUP DATABASE')") != 1)
        {
            return DbBackupSupport.NotSupported("The database user is not allowed to back up the database (permission BACKUP DATABASE).");
        }

        // xp_fileexist only sees the files of SQL Server for a sysadmin
        if (await ScalarAsync<int>(dbAccess, "SELECT IS_SRVROLEMEMBER('sysadmin')") != 1)
        {
            return DbBackupSupport.Supported(
                $"It could not be checked if SQL Server writes to the backup folder of the application ({sqlServerFolder} for SQL Server): the first backup checks it.");
        }

        // the application writes a file into its folder, SQL Server has to see it in its folder
        var markerName = $"yaf-backup-check-{Guid.NewGuid():N}.tmp";
        var markerPath = Path.Combine(backupFolder, markerName);

        await File.WriteAllTextAsync(markerPath, string.Empty);

        try
        {
            var exists = await dbAccess.ExecuteAsync(
                async db =>
                {
                    await using var command = ((DbConnection)db.ToDbConnection()).CreateCommand();

                    command.CommandText = "EXEC master.dbo.xp_fileexist @path";
                    AddParameter(command, "@path", this.SqlServerPath(markerName));

                    await using var reader = await command.ExecuteReaderAsync();

                    // "File Exists" is a tinyint
                    return await reader.ReadAsync() && Convert.ToInt32(reader.GetValue(0), System.Globalization.CultureInfo.InvariantCulture) == 1;
                });

            return exists
                         ? DbBackupSupport.Supported()
                         : DbBackupSupport.NotSupported(
                             $"SQL Server doesn't see the backup folder of the application: {sqlServerFolder} (SQL Server) and {backupFolder} (application) are not the same folder.");

        }
        finally
        {
            File.Delete(markerPath);
        }
    }

    /// <summary>
    /// Creates the backup file.
    /// </summary>
    /// <param name="dbAccess">The database access.</param>
    /// <param name="filePath">The full path of the backup file (in the backup folder of the application).</param>
    /// <returns>The <see cref="Task"/>.</returns>
    public Task CreateBackupAsync(IDbAccess dbAccess, string filePath)
    {
        var sqlServerPath = this.SqlServerPath(Path.GetFileName(filePath));

        return dbAccess.ExecuteAsync(
            async db =>
            {
                await using var command = ((DbConnection)db.ToDbConnection()).CreateCommand();

                // COPY_ONLY: the backup doesn't change the backup chain of other (regular) backups of the database
                command.CommandText =
                    "DECLARE @database sysname = DB_NAME(); BACKUP DATABASE @database TO DISK = @path WITH COPY_ONLY, INIT, FORMAT, NAME = N'YAF.NET Database Backup'";
                command.CommandTimeout = this.boardConfiguration.SqlCommandTimeout;

                AddParameter(command, "@path", sqlServerPath);

                return await command.ExecuteNonQueryAsync();
            });
    }

    /// <summary>
    /// Gets the path of a file in the backup folder, as SQL Server sees it (SQL Server on Linux uses "/").
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>The path.</returns>
    private string SqlServerPath(string fileName)
    {
        var folder = this.SqlServerBackupFolder.TrimEnd('/', '\\');
        var separator = folder.Contains('/') ? '/' : '\\';

        return $"{folder}{separator}{fileName}";
    }

    /// <summary>
    /// Runs a query with a single value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="dbAccess">The database access.</param>
    /// <param name="sql">The query.</param>
    /// <returns>The value.</returns>
    private static Task<T> ScalarAsync<T>(IDbAccess dbAccess, string sql)
    {
        return dbAccess.ExecuteAsync(db => db.ScalarAsync<T>(sql));
    }

    /// <summary>
    /// Adds a string parameter.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    private static void AddParameter(DbCommand command, string name, string value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
