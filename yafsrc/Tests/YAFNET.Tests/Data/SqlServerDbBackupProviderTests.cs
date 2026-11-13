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

namespace YAF.Tests.Data;

using System.IO;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using YAF.Data.SqlServer;
using YAF.Types.Interfaces;
using YAF.Types.Objects;

/// <summary>
/// The backup folder of the SQL Server database backup: the path SQL Server gets for BACKUP DATABASE and xp_fileexist.
/// </summary>
[TestFixture]
public class SqlServerDbBackupProviderTests
{
    [Test]
    [Description("A relative SqlServerBackupFolder (SQL Server on the same machine) is passed to SQL Server as a full path in the content root of the application, not relative (SQL Server would resolve it in its own folder)")]
    public void Relative_Folder_Is_Resolved_In_The_Content_Root_Test()
    {
        var contentRoot = Path.Combine(Path.GetTempPath(), "yaf-content-root");

        var provider = CreateProvider("App_Data/Backups", contentRoot);

        var expectedFolder = Path.GetFullPath(Path.Combine(contentRoot, "App_Data", "Backups"));

        provider.SqlServerBackupFolder.Should().Be(expectedFolder);

        var path = provider.SqlServerPath("yaf-backup-20261114-010000.bak");

        Path.IsPathRooted(path).Should().BeTrue();
        path.Should().Be(Path.Combine(expectedFolder, "yaf-backup-20261114-010000.bak"));
    }

    /// <summary>
    /// Creates the provider with the backup folder and the content root of the application.
    /// </summary>
    /// <param name="sqlServerBackupFolder">The SqlServerBackupFolder of the board configuration.</param>
    /// <param name="contentRoot">The content root of the application.</param>
    /// <returns>The <see cref="SqlServerDbBackupProvider"/>.</returns>
    private static SqlServerDbBackupProvider CreateProvider(string sqlServerBackupFolder, string contentRoot)
    {
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.ContentRootPath.Returns(contentRoot);

        var serviceLocator = Substitute.For<IServiceLocator>();
        serviceLocator.TryGet(typeof(IWebHostEnvironment), out Arg.Any<object>()).Returns(
            x =>
            {
                x[1] = environment;
                return true;
            });

        return new SqlServerDbBackupProvider(
            serviceLocator,
            new BoardConfiguration { SqlServerBackupFolder = sqlServerBackupFolder });
    }
}
