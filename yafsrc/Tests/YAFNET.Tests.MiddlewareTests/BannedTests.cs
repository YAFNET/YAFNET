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

namespace YAF.Tests.MiddlewareTests;

/// <summary>
/// Checks the Banned IP/UserAgent Middleware functionality.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BannedTests : Setup
{
    /// <summary>
    /// Checks if the IP address is banned.
    /// </summary>
    [Test]
    public async Task IpAddressIsBannedTest()
    {
        var client = WebTestingHostFactoryUtils.CreateClient(this.TestSettings, "113.240.110.90");

        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

        var response = await client.GetAsync(this.TestSettings.TestForumUrl);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    /// <summary>
    /// Checks if the UserAgent is banned.
    /// </summary>
    [Test]
    public async Task UserAgentIsBannedTest()
    {
        var client = WebTestingHostFactoryUtils.CreateClient(this.TestSettings, "127.0.0.1");

        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; TestBot/1.0)");

        var response = await client.GetAsync(this.TestSettings.TestForumUrl);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}