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


using Microsoft.AspNetCore.Hosting;

using netDumbster.smtp;

using NUnit.Framework.Interfaces;

using YAF.Tests.Pages;
using YAF.ViewComponents;

namespace YAF.Tests.Infrastructure;

public abstract class DatabaseFixture : IAsyncDisposable
{
    public abstract ComposeScenario Scenario { get; }

    private TestConfig TestSettings { get; set; }

    private IPlaywright playwright = null!;
    private IBrowser browser = null!;

    private DockerComposeFixture docker = null!;

    /// <summary>
    /// Called once per fixture class — starts the container
    /// </summary>
    /// <param name="testSettings">The test settings.</param>
    public async Task InitializeAsync(TestConfig testSettings)
    {
        this.TestSettings = testSettings;

        await this.LaunchBrowserAsync(this.Scenario);
    }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    async protected virtual Task DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            await this.CloseBrowserAsync(this.TestSettings);
        }
    }

    /// <summary>
    /// Gets or sets the host factory.
    /// </summary>
    /// <value>The host factory.</value>
    private WebTestingHostFactory<CultureSwitcherViewComponent> HostFactory { get; set; }

    public IBrowserContext Context { get; private set; } = null!;

    public IPage Page { get; private set; } = null!;

    /// <summary>
    /// the base URL from the Docker fixture
    /// </summary>
    /// <value>
    /// The base URL.
    /// </value>
    public string BaseUrl { get; private set; } = null!;

    /// <summary>
    /// Gets or sets the SMTP server.
    /// </summary>
    /// <value>
    /// The SMTP server.
    /// </value>
    public SimpleSmtpServer SmtpServer { get; set; }

    public async Task LaunchBrowserAsync(ComposeScenario scenario)
    {
        this.playwright = await Playwright.CreateAsync();
        this.browser = await this.playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        // Launch Mail Server
        this.SmtpServer = SimpleSmtpServer.Start(this.TestSettings.TestMailPort);

        if (this.TestSettings.UseDocker)
        {
            await this.SetUpForScenarioWithDockerAsync(scenario, this.TestSettings);
        }
        else
        {
            await this.SetUpForScenarioAsync(this.TestSettings);
        }
    }

    public async Task CloseBrowserAsync(TestConfig testSettings)
    {
        await this.TearDownScenarioAsync(testSettings);

        await this.browser.DisposeAsync();
        this.playwright.Dispose();
    }

    /// <summary>
    /// Call this at the start of every [SetUp] in the derived class,
    /// passing the scenario chosen by [TestCaseSource].
    /// </summary>
    async protected Task SetUpForScenarioWithDockerAsync(ComposeScenario scenario, TestConfig testSettings)
    {
        this.BaseUrl = scenario.BaseUrl;

        // Boot the compose stack for this scenario
        this.docker = new DockerComposeFixture(scenario.BaseUrl, scenario.ComposeFile);
        await this.docker.StartAsync();

        var videoDir = string.Empty;

        if (testSettings.RecordVideos)
        {
            videoDir = "videos/";
        }

        // Fresh browser context (isolated cookies, sessions, storage)
        this.Context = await this.browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = this.BaseUrl,
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize { Width = 1280, Height = 900 },
            RecordVideoDir = videoDir
        });

        this.Page = await this.Context.NewPageAsync();

        this.Page.Console += (_, msg) => TestContext.Progress.WriteLine($"[browser] {msg.Type}: {msg.Text}");
        this.Page.PageError += (_, err) => TestContext.Progress.WriteLine($"[browser-error] {err}");

        await this.InstallApplicationAsync(testSettings);
    }

    /// <summary>
    /// Call this at the start of every [SetUp] in the derived class,
    /// passing the scenario chosen by [TestCaseSource].
    /// </summary>
    async protected Task SetUpForScenarioAsync(TestConfig testSettings)
    {
        this.BaseUrl = testSettings.TestForumUrl;

        // Create the host factory with the App class as parameter and the
        // url we are going to use.
        this.HostFactory = new WebTestingHostFactory<CultureSwitcherViewComponent>();

        this.HostFactory
            // Override host configuration to mock stuff if required.
            .WithWebHostBuilder(
                builder =>
                {
                    // Setup the url to use.
                    builder.UseUrls(this.BaseUrl);
                    // Replace or add services if needed.
                })
            // Create the host using the CreateDefaultClient method.
            .CreateDefaultClient();

        var videoDir = string.Empty;

        if (testSettings.RecordVideos)
        {
            videoDir = "videos/";
        }

        // Fresh browser context (isolated cookies, sessions, storage)
        this.Context = await this.browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = this.BaseUrl,
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize { Width = 1280, Height = 900 },
            RecordVideoDir = videoDir
        });

        this.Page = await this.Context.NewPageAsync();

        this.Page.Console += (_, msg) => TestContext.Progress.WriteLine($"[browser] {msg.Type}: {msg.Text}");
        this.Page.PageError += (_, err) => TestContext.Progress.WriteLine($"[browser-error] {err}");

        await this.InstallApplicationAsync(testSettings);
    }

    /// <summary>
    /// Call this at the end of every [TearDown] in the derived class.
    /// </summary>
    async protected Task TearDownScenarioAsync(TestConfig testConfig)
    {
        // Screenshot on failure
        if (TestContext.CurrentContext.Result.Outcome.Status ==
            TestStatus.Failed)
        {
            var name = TestContext.CurrentContext.Test.MethodName ?? "unknown";
            await this.Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = $"screenshots/{name}.png",
                FullPage = true
            });
        }

        await this.Context.DisposeAsync();

        if (testConfig.UseDocker)
        {
            await this.docker.DisposeAsync();
        }

        // Stop Mail Server
        this.SmtpServer?.Stop();
    }

    private async Task InstallApplicationAsync(TestConfig testSettings)
    {
        if (!testSettings.NewInstall)
        {
            return;
        }

        // Install YAF.NET
        var installer = new InstallWizard(this, testSettings);
        await installer.InstallAsync();

        await this.Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close" })
            .Filter(new LocatorFilterOptions { HasText = "Close" }).ClickAsync();

        this.Page = await this.Context.NewPageAsync();

        // register test users
        await installer.RegisterTestUserAsync(testSettings.TestUserName, testSettings.TestUserPassword);

        this.SmtpServer.ClearReceivedEmail();

        await installer.RegisterTestUserAsync(testSettings.TestUserName2, testSettings.TestUser2Password);
    }
}

// One concrete fixture per DB — each gets its own container lifecycle
public class SqlServerFixture : DatabaseFixture
{
    public override ComposeScenario Scenario => ComposeScenario.SqlServer;
}

// ReSharper disable once InconsistentNaming
#pragma warning disable S101
public class PostgreSQLFixture : DatabaseFixture
#pragma warning restore S101
{
    public override ComposeScenario Scenario => ComposeScenario.PostgreSQL;
}

public class MySqlFixture : DatabaseFixture
{
    public override ComposeScenario Scenario => ComposeScenario.MySql;
}

public class SqliteFixture : DatabaseFixture
{
    public override ComposeScenario Scenario => ComposeScenario.Sqlite;
}