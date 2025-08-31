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

using System.Diagnostics.CodeAnalysis;

using YAF.ViewComponents;

namespace YAF.Tests.Utils;

using System;
using System.Net;
using System.Net.Sockets;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;

/// <summary>
/// Playwright fixture implementing an asynchronous life cycle.
/// </summary>
public class PlaywrightFixture
{
    /// <summary>
    /// Playwright module.
    /// </summary>
    public IPlaywright Playwright { get; private set; }

    /// <summary>
    /// Chromium lazy initializer.
    /// </summary>
    public Lazy<Task<IBrowser>> ChromiumBrowser { get; private set; }

    /// <summary>
    /// Firefox lazy initializer.
    /// </summary>
    public Lazy<Task<IBrowser>> FirefoxBrowser { get; private set; }

    /// <summary>
    /// Webkit lazy initializer.
    /// </summary>
    public Lazy<Task<IBrowser>> WebkitBrowser { get; private set; }

    /// <summary>
    /// Gets or sets the host factory.
    /// </summary>
    /// <value>The host factory.</value>
    private WebTestingHostFactory<CultureSwitcherViewComponent> HostFactory { get; set; }

    /// <summary>
    /// Initialize the Playwright fixture.
    /// </summary>
    public async Task InitializeAsync([StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var launchOptions = new BrowserTypeLaunchOptions { Headless = true };

        // Install Playwright and its dependencies.
        InstallPlaywright();

        this.Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        this.ChromiumBrowser = new Lazy<Task<IBrowser>>(this.Playwright.Chromium.LaunchAsync(launchOptions));
        this.FirefoxBrowser = new Lazy<Task<IBrowser>>(this.Playwright.Firefox.LaunchAsync(launchOptions));
        this.WebkitBrowser = new Lazy<Task<IBrowser>>(this.Playwright.Webkit.LaunchAsync(launchOptions));

        // Create the host factory with the App class as parameter and the
        // url we are going to use.
        this.HostFactory = new WebTestingHostFactory<CultureSwitcherViewComponent>();

        this.HostFactory
            // Override host configuration to mock stuff if required.
            .WithWebHostBuilder(
                builder =>
                {
                    // Setup the url to use.
                    builder.UseUrls(url);
                    // Replace or add services if needed.
                })
            // Create the host using the CreateDefaultClient method.
            .CreateDefaultClient();
    }

    /// <summary>
    /// Dispose all Playwright module resources.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (this.Playwright != null)
        {
            if (this.ChromiumBrowser is { IsValueCreated: true })
            {
                var browser = await this.ChromiumBrowser.Value;
                await browser.DisposeAsync();
            }

            if (this.FirefoxBrowser is { IsValueCreated: true })
            {
                var browser = await this.FirefoxBrowser.Value;
                await browser.DisposeAsync();
            }

            if (this.WebkitBrowser is { IsValueCreated: true })
            {
                var browser = await this.WebkitBrowser.Value;
                await browser.DisposeAsync();
            }

            this.Playwright.Dispose();
            this.Playwright = null;

            await this.HostFactory.DisposeAsync();
        }
    }

    /// <summary>
    /// Install and deploy all binaries Playwright may need.
    /// </summary>
    private static void InstallPlaywright()
    {
        var exitCode = Program.Main(["install-deps"]);
        if (exitCode != 0)
        {
            throw new Exception($"Playwright exited with code {exitCode} on install-deps");
        }

        exitCode = Program.Main(["install"]);
        if (exitCode != 0)
        {
            throw new Exception($"Playwright exited with code {exitCode} on install");
        }
    }

    /// <summary>
    /// Open a Browser page and navigate to the given URL before
    /// applying the given test handler.
    /// </summary>
    /// <param name="url">URL to navigate to.</param>
    /// <param name="testHandler">Test handler to apply on the page.
    /// </param>
    /// <param name="browserType">The Browser to use to open the page.
    /// </param>
    /// <returns>The GotoPage task.</returns>
    public async Task GotoPageAsync([StringSyntax(StringSyntaxAttribute.Uri)] string url, Func<IPage, Task> testHandler, Browser browserType)
    {
        // select and launch the browser.
        var browser = await this.SelectBrowserAsync(browserType);

        // Open a new page with an option to ignore HTTPS errors
        await using var context = await browser.NewContextAsync(
                                      new BrowserNewContextOptions
                                          {
                                              RecordVideoDir = "videos/",
                                              IgnoreHTTPSErrors = true
                                          }).ConfigureAwait(false);

        // Start tracing before creating the page.
        await context.Tracing.StartAsync(
            new TracingStartOptions { Screenshots = true, Snapshots = true, Sources = true });

        var page = await context.NewPageAsync().ConfigureAwait(false);

        Assert.That(page, Is.Not.Null);

        try
        {
            // Navigate to the given URL and wait until loading
            // network activity is done.
            var gotoResult = await page.GotoAsync(url);

            Assert.That(gotoResult, Is.Not.Null);

            await gotoResult.FinishedAsync();

            Assert.That(gotoResult.Ok, Is.True);

            await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Close" })
                .Filter(new LocatorFilterOptions { HasText = "Close" }).ClickAsync();

            // Run the actual test logic.
            await testHandler(page);
        }
        finally
        {
            // Make sure the page is closed
            await page.CloseAsync();

            // Stop tracing and save data into a zip archive.
            await context.Tracing.StopAsync(new TracingStopOptions { Path = "trace.zip" });

            await context.CloseAsync();
        }
    }

    /// <summary>
    /// Select the IBrowser instance depending on the given browser
    /// enumeration value.
    /// </summary>
    /// <param name="browser">The browser to select.</param>
    /// <returns>The selected IBrowser instance.</returns>
    private Task<IBrowser> SelectBrowserAsync(Browser browser)
    {
        return browser switch
            {
                Browser.Chromium => this.ChromiumBrowser.Value,
                Browser.Firefox => this.FirefoxBrowser.Value,
                Browser.Webkit => this.WebkitBrowser.Value,
                _ => throw new NotImplementedException()
            };
    }

    /// <summary>
    /// Gets the random unused port.
    /// </summary>
    /// <returns>System.Int32.</returns>
    public static int GetRandomUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Any, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}