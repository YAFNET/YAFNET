/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using YAF.ViewComponents;

namespace YAF.Tests.LighthouseTests;

/// <summary>
/// Unit TestBase.
/// </summary>
public class Setup
{
    /// <summary>
    /// Gets or sets the host factory.
    /// </summary>
    /// <value>The host factory.</value>
    private WebTestingHostFactory<CultureSwitcherViewComponent> HostFactory { get; set; }

    /// <summary>
    /// Gets or sets the test settings.
    /// </summary>
    /// <value>The test settings.</value>
    public TestConfig TestSettings { get; set; }

    /// <summary>
    /// starts the application and create playwright
    /// </summary>
    [OneTimeSetUp]
    public void SetUp()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath($"{Directory.GetCurrentDirectory()}..//..//..//.//..//")
            .AddJsonFile("appSettings.json", false, false)
            .AddEnvironmentVariables()
            .Build();

        this.TestSettings = configuration.GetSection("TestSettings").Get<TestConfig>();

        // Create the host factory with the App class as parameter and the
        // url we are going to use.
        this.HostFactory = new WebTestingHostFactory<CultureSwitcherViewComponent>();

        this.HostFactory
            // Override host configuration to mock stuff if required.
            .WithWebHostBuilder(
                builder =>
                {
                    // Setup the url to use.
                    builder.UseUrls(this.TestSettings.TestForumUrl);
                    // Replace or add services if needed.
                })
            // Create the host using the CreateDefaultClient method.
            .CreateDefaultClient();
    }

    /// <summary>
    /// The Tear Down
    /// </summary>
    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        await this.HostFactory.DisposeAsync();
    }
}