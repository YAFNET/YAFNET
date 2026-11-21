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

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.AdminTests;

/// <summary>
/// The tests of the host pages (languages, restart, database, backup).
/// </summary>
public class HostTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Save saves the texts of the shown page when another page is selected without loading it: it failed with an
    /// error (the texts were searched in the selected page).
    /// </summary>
    [Test]
    public async Task EditLanguageSaveAfterSelectingAnotherPageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                await LoadPageAsync(page, this.TestSettings, "ADMIN_TAGS");

                // another page is selected, but not loaded
                await page.Locator("#Input_Pages").SelectOptionAsync("ADMIN_PM");

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator("button[formaction*='Save']").ClickAsync(),
                                   r => r.Request.Method == "POST" && r.Url.Contains("/Save"));

                Assert.That(response.Status, Is.LessThan(400), $"The save failed ({response.Status})");

                await page.WaitForLoadStateAsync();

                Assert.That(await page.ContentAsync(), Does.Contain("The language file has been updated and saved!"), "Not saved");
            });
    }

    /// <summary>
    /// Loads a page of the vietnamese language file.
    /// </summary>
    /// <returns>The status of the response.</returns>
    private static async Task<int> LoadPageAsync(IPage page, TestConfig testSettings, string pageName)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/EditLanguage/vietnam.json");

        await page.Locator("#Input_Pages").SelectOptionAsync(pageName);

        var response = await page.RunAndWaitForResponseAsync(
                           () => page.Locator("button[formaction*='LoadPageLocalization']").ClickAsync(),
                           r => r.Request.Method == "POST" && r.Url.Contains("LoadPageLocalization"));

        await page.WaitForLoadStateAsync();

        return response.Status;
    }
}
