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

                // another page is posted with the texts of the shown page (the page select was not loaded)
                await page.Locator("form.admin-edit-language").EvaluateAsync(
                    "form => form.insertAdjacentHTML('beforeend', '<input type=\"hidden\" name=\"Input.Pages\" value=\"ADMIN_PM\">')");

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator("button[formaction*='Save']").ClickAsync(),
                                   r => r.Request.Method == "POST" && r.Url.Contains("/Save"));

                Assert.That(response.Status, Is.LessThan(400), $"The save failed ({response.Status})");

                await page.WaitForLoadStateAsync();

                Assert.That(await page.ContentAsync(), Does.Contain("The language file has been updated and saved!"), "Not saved");
            });
    }

    /// <summary>
    /// An emptied text is saved as the english text: null was written to the file and the page failed with an error
    /// every time it was shown.
    /// </summary>
    [Test]
    public async Task EditLanguageEmptyTextTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                await LoadPageAsync(page, this.TestSettings, "ADMIN_TAGS");

                var text = page.Locator("#Locals_0__LocalizedResourceText");
                var translated = await text.InputValueAsync();
                var original = await page.Locator("input#Locals_0__OriginalResourceText").InputValueAsync();

                try
                {
                    await text.FillAsync(string.Empty);

                    var response = await page.RunAndWaitForResponseAsync(
                                       () => page.Locator("button[formaction*='Save']").ClickAsync(),
                                       r => r.Request.Method == "POST" && r.Url.Contains("/Save"));

                    Assert.That(response.Status, Is.LessThan(400), $"The save failed ({response.Status})");

                    // the page is shown again
                    var status = await LoadPageAsync(page, this.TestSettings, "ADMIN_TAGS");

                    Assert.That(status, Is.LessThan(400), $"The page of the language fails ({status})");
                    Assert.That(await text.InputValueAsync(), Is.EqualTo(original), "The english text is not used for the empty text");
                }
                finally
                {
                    // the translation is set back
                    if (await text.CountAsync() > 0)
                    {
                        await text.FillAsync(translated);
                        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
                    }
                }
            });
    }

    /// <summary>
    /// A page of the list loads its texts, the switch shows only the texts that are not translated.
    /// </summary>
    [Test]
    public async Task EditLanguagePageListAndFilterTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                var status = await LoadPageAsync(page, this.TestSettings, "ADMIN_TAGS");

                Assert.That(status, Is.LessThan(400), $"The page of the language fails ({status})");
                Assert.That(
                    await page.Locator("#LanguagePages button.active").GetAttributeAsync("value"),
                    Is.EqualTo("ADMIN_TAGS"),
                    "The loaded page is not marked");

                var rows = page.Locator(".translation-row");
                var missing = await page.Locator(".translation-row.is-missing").CountAsync();

                Assert.That(await rows.CountAsync(), Is.GreaterThan(0), "No texts of the page");

                await page.Locator("#UntranslatedOnly").CheckAsync();

                Assert.That(
                    await page.Locator(".translation-row:visible").CountAsync(),
                    Is.EqualTo(missing),
                    "The switch doesn't show only the texts that are not translated");
            });
    }

    /// <summary>
    /// A number of backups to keep that is too large keeps the setting: it was saved as 0 (all backups are kept).
    /// </summary>
    [Test]
    public async Task BackupKeepTooLargeKeepsTheSettingTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Backup");

                var keep = page.Locator("#Keep");

                if (await keep.CountAsync() == 0)
                {
                    Assert.Ignore("The database has no backup");
                }

                var value = await keep.InputValueAsync();

                try
                {
                    // a number of backups that is not 0
                    await keep.FillAsync("7");
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                    // the number is larger than an int
                    await keep.EvaluateAsync("e => e.value = '99999999999'");
                    await page.RunAndWaitForNavigationAsync(
                        () => page.Locator("button[formaction*='Save']").EvaluateAsync("b => b.form.requestSubmit(b)"));

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Backup");

                    Assert.That(await keep.InputValueAsync(), Is.EqualTo("7"), "The number was saved as 0 (all backups)");
                }
                finally
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Backup");
                    await keep.FillAsync(value);
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
                }
            });
    }

    /// <summary>
    /// Loads a page of the vietnamese language file.
    /// </summary>
    /// <returns>The status of the response.</returns>
    private static async Task<int> LoadPageAsync(IPage page, TestConfig testSettings, string pageName)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/EditLanguage/vietnam.json");

        var response = await page.RunAndWaitForResponseAsync(
                           () => page.Locator($"#LanguagePages button[value='{pageName}']").ClickAsync(),
                           r => r.Request.Method == "POST" && r.Url.Contains("LoadPageLocalization"));

        await page.WaitForLoadStateAsync();

        return response.Status;
    }
}
