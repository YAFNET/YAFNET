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

using System.Threading;

namespace YAF.Tests.Infrastructure;

/// <summary>
/// Keeps the docker stack of the current scenario running across all test classes.
/// All scenarios share the same port, so only one stack can run at a time. The stack is
/// only replaced when a test class for a different scenario starts, and the last one is
/// stopped by the <see cref="AssemblySetup"/> after all tests are done.
/// </summary>
public static class FixtureRegistry
{
    private readonly static SemaphoreSlim Lock = new(1, 1);

    private static DatabaseFixture activeFixture;

    public async static Task<DatabaseFixture> AcquireAsync(ComposeScenario scenario, TestConfig testSettings)
    {
        await Lock.WaitAsync();
        try
        {
            if (activeFixture is not null && activeFixture.Scenario.Name == scenario.Name)
            {
                return activeFixture;
            }

            // a different scenario needs the port, stop the running stack first
            await DisposeActiveFixtureAsync();

            var fixture = scenario.Name switch
            {
                "SqlServer" => (DatabaseFixture)new SqlServerFixture(),
                "PostgreSQL" => new PostgreSQLFixture(),
                "MySql" => new MySqlFixture(),
                "Sqlite" => new SqliteFixture(),
                _ => throw new ArgumentOutOfRangeException(nameof(scenario))
            };

            await fixture.InitializeAsync(testSettings);
            activeFixture = fixture;

            return activeFixture;
        }
        finally
        {
            Lock.Release();
        }
    }

    /// <summary>
    /// Stops the running stack. Called once after all tests are done.
    /// </summary>
    public async static Task ReleaseAllAsync()
    {
        await Lock.WaitAsync();
        try
        {
            await DisposeActiveFixtureAsync();
        }
        finally
        {
            Lock.Release();
        }
    }

    private async static Task DisposeActiveFixtureAsync()
    {
        if (activeFixture is null)
        {
            return;
        }

        var fixture = activeFixture;
        activeFixture = null;

        await fixture.DisposeAsync();
    }
}
