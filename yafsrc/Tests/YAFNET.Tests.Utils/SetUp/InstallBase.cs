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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Utils.SetUp;

using System.IO;

using Microsoft.Extensions.Configuration;

using netDumbster.smtp;

/// <summary>
/// The InstallBase Class
/// </summary>
public class InstallBase
{
    /// <summary>
    /// Gets or sets the SMTP server.
    /// </summary>
    /// <value>
    /// The SMTP server.
    /// </value>
    public SimpleSmtpServer SmtpServer { get; set; }

    /// <summary>
    /// Gets or sets the test settings.
    /// </summary>
    /// <value>The test settings.</value>
    public TestConfig TestSettings { get; set; }

    /// <summary>
    /// The playwright fixture
    /// </summary>
    public PlaywrightFixture PlaywrightFixture;

    /// <summary>
    /// Initializes the Install Base.
    /// </summary>
    /// <returns>Task.</returns>
    public Task InitializeAsync()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath($"{Directory.GetCurrentDirectory()}..//..//..//.//..//")
            .AddJsonFile("appSettings.json", false, false)
            .AddEnvironmentVariables()
            .Build();

        this.TestSettings = configuration.GetSection("TestSettings").Get<TestConfig>();

        // Launch Mail Server
        this.SmtpServer = SimpleSmtpServer.Start(this.TestSettings.TestMailPort);

        return this.StartClientAsync();
    }

    /// <summary>
    /// Starts the playwright fixture client asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public Task StartClientAsync()
    {
        this.PlaywrightFixture = new PlaywrightFixture();

        return this.PlaywrightFixture.InitializeAsync(this.TestSettings.TestForumUrl);
    }

    /// <summary>
    /// Tears down.
    /// </summary>
    public void TearDown()
    {
        // Stop Mail Server
        this.SmtpServer?.Stop();
    }
}