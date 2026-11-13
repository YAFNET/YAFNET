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

namespace YAF.Data.PostgreSQL;

using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using YAF.Types.Extensions;
using YAF.Types.Objects;

/// <summary>
/// The PostgreSQL database backup: a SQL dump made by pg_dump (it must be installed, e.g. the package postgresql-client,
/// and be the same or a newer version than the PostgreSQL server).
/// </summary>
#pragma warning disable S101
// ReSharper disable once InconsistentNaming
public partial class PostgreSQLDbBackupProvider : IDbBackupProvider
#pragma warning restore S101
{
    /// <summary>
    /// The board configuration.
    /// </summary>
    private readonly BoardConfiguration boardConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSQLDbBackupProvider"/> class.
    /// </summary>
    /// <param name="boardConfiguration">The board configuration.</param>
    public PostgreSQLDbBackupProvider(BoardConfiguration boardConfiguration)
    {
        this.boardConfiguration = boardConfiguration;
    }

    /// <summary>
    /// Gets the file extension of the backup file in the zip.
    /// </summary>
    public string FileExtension => ".sql";

    /// <summary>
    /// Gets the pg_dump program.
    /// </summary>
    private string PgDump => this.boardConfiguration.PgDumpPath.IsSet() ? this.boardConfiguration.PgDumpPath : "pg_dump";

    /// <summary>
    /// Checks if pg_dump is installed and not older than the PostgreSQL server.
    /// </summary>
    /// <param name="dbAccess">The database access.</param>
    /// <param name="backupFolder">The backup folder of the application.</param>
    /// <returns>The <see cref="DbBackupSupport"/>.</returns>
    public async Task<DbBackupSupport> CheckSupportAsync(IDbAccess dbAccess, string backupFolder)
    {
        string version;

        try
        {
            var result = await this.RunPgDumpAsync(["--version"], null, TimeSpan.FromSeconds(30));

            version = result.Output;
        }
        catch (Exception ex)
        {
            return DbBackupSupport.NotSupported(
                $"pg_dump was not found ({this.PgDump}): install the PostgreSQL client tools, or configure the path as BoardConfiguration:PgDumpPath. {ex.Message}");
        }

        // e.g. "pg_dump (PostgreSQL) 17.2 (Debian 17.2-1.pgdg120+1)"
        var match = PgDumpVersion().Match(version);

        if (!match.Success)
        {
            return DbBackupSupport.NotSupported($"The version of pg_dump is unknown: {version.Trim()}");
        }

        var pgDumpMajor = int.Parse(match.Groups["major"].Value, CultureInfo.InvariantCulture);

        // e.g. 170002
        var serverVersion = await dbAccess.ExecuteAsync(db => db.ScalarAsync<string>("SHOW server_version_num"));
        var serverMajor = int.Parse(serverVersion, CultureInfo.InvariantCulture) / 10000;

        return pgDumpMajor < serverMajor
                   ? DbBackupSupport.NotSupported(
                       $"pg_dump {pgDumpMajor} is older than the PostgreSQL server {serverMajor}: pg_dump must be the same or a newer version.")
                   : DbBackupSupport.Supported();
    }

    /// <summary>
    /// Creates the backup file.
    /// </summary>
    /// <param name="dbAccess">The database access.</param>
    /// <param name="filePath">The full path of the backup file.</param>
    /// <returns>The <see cref="Task"/>.</returns>
    public async Task CreateBackupAsync(IDbAccess dbAccess, string filePath)
    {
        var connection = new NpgsqlConnectionStringBuilder(dbAccess.Information.ConnectionString());

        List<string> arguments =
        [
            "--format=plain", "--no-owner", "--encoding=UTF8", $"--file={filePath}",
            $"--port={connection.Port}", $"--dbname={connection.Database}"
        ];

        if (connection.Host.IsSet())
        {
            arguments.Add($"--host={connection.Host}");
        }

        if (connection.Username.IsSet())
        {
            arguments.Add($"--username={connection.Username}");
        }

        // never ask for a password
        arguments.Add("--no-password");

        var environment = new Dictionary<string, string>();

        if (connection.Password.IsSet())
        {
            environment["PGPASSWORD"] = connection.Password;
        }

        environment["PGSSLMODE"] = connection.SslMode switch
        {
            SslMode.Disable => "disable",
            SslMode.Allow => "allow",
            SslMode.Require => "require",
            SslMode.VerifyCA => "verify-ca",
            SslMode.VerifyFull => "verify-full",
            _ => "prefer"
        };

        var timeout = this.boardConfiguration.SqlCommandTimeout > 0
                          ? TimeSpan.FromSeconds(this.boardConfiguration.SqlCommandTimeout)
                          : Timeout.InfiniteTimeSpan;

        var result = await this.RunPgDumpAsync(arguments, environment, timeout);

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException($"pg_dump failed ({result.ExitCode}): {result.Error.Trim()}");
        }
    }

    /// <summary>
    /// The version of pg_dump.
    /// </summary>
    [GeneratedRegex(@"\(PostgreSQL\)\s+(?<major>\d+)")]
    private static partial Regex PgDumpVersion();

    /// <summary>
    /// Runs pg_dump.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <param name="environment">The environment variables.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The exit code and the output.</returns>
    private async Task<(int ExitCode, string Output, string Error)> RunPgDumpAsync(
        IEnumerable<string> arguments,
        Dictionary<string, string> environment,
        TimeSpan timeout)
    {
        var startInfo = new ProcessStartInfo(this.PgDump)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        if (environment is not null)
        {
            foreach (var (key, value) in environment)
            {
                startInfo.Environment[key] = value;
            }
        }

        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException($"{this.PgDump} could not be started.");

        using var cancellation = new CancellationTokenSource(timeout);

        var output = process.StandardOutput.ReadToEndAsync(cancellation.Token);
        var error = process.StandardError.ReadToEndAsync(cancellation.Token);

        try
        {
            await process.WaitForExitAsync(cancellation.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(true);

            throw new TimeoutException($"pg_dump didn't finish in {timeout}.");
        }

        return (process.ExitCode, await output, await error);
    }
}
