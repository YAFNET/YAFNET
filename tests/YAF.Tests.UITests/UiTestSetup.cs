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

namespace YAF.Tests.UITests;

using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;
using YAF.Tests.UITests.Pages;

/// <summary>
/// Starts the forum (IIS Express with a LocalDB database) and the browser once for all UI tests, and installs
/// the forum with the install wizard.
/// </summary>
[SetUpFixture]
public class UiTestSetup
{
    /// <summary>
    /// Gets the running forum.
    /// </summary>
    public static TestSite Site { get; private set; }

    /// <summary>
    /// Gets the browser.
    /// </summary>
    public static IBrowser Browser { get; private set; }

    private static IPlaywright playwright;

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        Site = await TestSite.StartAsync();

        playwright = await Playwright.CreateAsync();

        Browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = !TestConfig.Headed });

        // the forum is installed once with the install wizard (the first test of the forum)
        var context = await Browser.NewContextAsync(new BrowserNewContextOptions { BaseURL = TestConfig.BaseUrl });

        try
        {
            var page = await context.NewPageAsync();

            await new InstallWizard(page).InstallAsync();

            await ConfigureForumAsync(page);
        }
        finally
        {
            await context.CloseAsync();
        }
    }

    /// <summary>
    /// The settings for the tests: no flood delay between two posts (the tests post several times in a row), the
    /// albums are turned on.
    /// </summary>
    /// <param name="page">The page.</param>
    private static async Task ConfigureForumAsync(IPage page)
    {
        page.SetDefaultTimeout(60000);
        page.SetDefaultNavigationTimeout(60000);

        await page.Context.AddCookiesAsync(
            [new Cookie { Name = "YAF-AcceptCookies", Value = "true", Url = TestConfig.BaseUrl }]);

        await page.GotoAsync("Account/Login");
        await page.Locator("[id$='_UserName']").FillAsync(TestConfig.AdminUserName);
        await page.Locator("[id$='_Password']").FillAsync(TestConfig.AdminPassword);
        await page.RunAndWaitForNavigationAsync(() => page.Locator("[id$='_LoginButton']").ClickAsync());

        await page.GotoAsync("Admin/HostSettings");

        // the fields and the save buttons are in tabs, they are changed with script
        await page.EvaluateAsync("() => document.querySelector(\"[id$='_PostFloodDelay']\").value = '0'");
        await page.EvaluateAsync("() => document.querySelector(\"[id$='_EnableAlbum']\").checked = true");
        await page.RunAndWaitForNavigationAsync(
            () => page.EvaluateAsync("() => document.querySelector(\"[id$='_Save']\").click()"));
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        if (Browser != null)
        {
            await Browser.CloseAsync();
        }

        playwright?.Dispose();

        Site?.Dispose();
    }
}
