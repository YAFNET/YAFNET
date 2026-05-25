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

using System.Collections.Concurrent;
using System.Threading;

namespace YAF.Tests.Infrastructure;

public static class FixtureRegistry
{
    // One live fixture per DB type, shared across all test classes
    private readonly static ConcurrentDictionary<ComposeScenario, DatabaseFixture> Fixtures = new();
    private readonly static ConcurrentDictionary<ComposeScenario, int> ConsumerCount = new();
    private readonly static SemaphoreSlim Lock = new(1, 1);

    public async static Task<DatabaseFixture> AcquireAsync(ComposeScenario scenario, TestConfig testSettings)
    {
        await Lock.WaitAsync();
        try
        {
            ConsumerCount.AddOrUpdate(scenario, 1, (_, count) => count + 1);

            if (Fixtures.TryGetValue(scenario, out var fixtureFromDictionary))
            {
                return fixtureFromDictionary;
            }

            var fixture = scenario.Name switch
            {
                "SqlServer" => (DatabaseFixture)new SqlServerFixture(),
                "PostgreSQL" => new PostgreSQLFixture(),
                "MySql" => new MySqlFixture(),
                "Sqlite" => new SqliteFixture(),
                _ => throw new ArgumentOutOfRangeException(nameof(scenario))
            };

            await fixture.InitializeAsync(testSettings);
            Fixtures[scenario] = fixture;

            return Fixtures[scenario];
        }
        finally
        {
            Lock.Release();
        }
    }

    public async static Task ReleaseAsync(ComposeScenario scenario, TestConfig testSettings)
    {
        await Lock.WaitAsync();
        try
        {
            var remaining = ConsumerCount.AddOrUpdate(scenario, 0, (_, count) => count - 1);

            // Only tear down the container when the last test class is done
            if (remaining == 0 && Fixtures.TryRemove(scenario, out var fixture))
            {
                await fixture.DisposeAsync();
            }
        }
        finally
        {
            Lock.Release();
        }
    }
}