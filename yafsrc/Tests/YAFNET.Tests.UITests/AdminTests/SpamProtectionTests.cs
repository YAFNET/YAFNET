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
/// The tests of the spam protection pages (spam words, banned emails, ips, names and user agents).
/// </summary>
public class SpamProtectionTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A search started on a later page shows the matches, the pager counts the matches (not all entries).
    /// </summary>
    [Test]
    public async Task SearchOnALaterPageShowsTheMatchesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/SpamWords");

                // more entries than fit on one page
                var prefix = $"uitestsearch{Random.Shared.Next()}";

                for (var i = 0; i < 6; i++)
                {
                    await PostApiAsync(page, $"{this.TestSettings.TestForumUrl}api/SpamWords/Edit", new { id = 0, spamWord = $"{prefix}x{i}" });
                }

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/SpamWords/2");

                await SearchAsync(page, $"{prefix}x0");

                var content = await page.ContentAsync();

                Assert.That(content, Does.Contain($"{prefix}x0"), "The match is not shown");
                Assert.That(content, Does.Contain("Showing 1 to 1 of 1 entries"), "The pager counts all entries");
            });
    }

    /// <summary>
    /// A ban can't be changed to the mask of another ban: the dialog shows "already exists"
    /// (the banned names stopped with an error, the banned user agents saved the mask twice).
    /// </summary>
    [TestCase("BannedNames", "BannedName", "The Banned Name Already exists!")]
    [TestCase("BannedUserAgents", "BannedUserAgent", "The Banned UserAgent Already exists!")]
    public async Task EditingABanToAnExistingMaskIsRefusedTest(string listPage, string api, string message)
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/{listPage}");

                var first = $"uitestfirst{Random.Shared.Next()}";
                var second = $"uitestsecond{Random.Shared.Next()}";

                await PostApiAsync(page, $"{this.TestSettings.TestForumUrl}api/{api}/Edit", new { id = 0, mask = first, banReason = "test" });
                await PostApiAsync(page, $"{this.TestSettings.TestForumUrl}api/{api}/Edit", new { id = 0, mask = second, banReason = "test" });

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/{listPage}");
                await SearchAsync(page, second);

                var secondId = await page.Locator("li.list-group-item-menu input[id='fID']").First.GetAttributeAsync("value");

                // the second ban is changed to the mask of the first ban
                var response = await PostApiAsync(
                    page,
                    $"{this.TestSettings.TestForumUrl}api/{api}/Edit",
                    new { id = int.Parse(secondId!), mask = first, banReason = "test" });

                Assert.That(response, Does.Contain(message), "The dialog shows no message");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/{listPage}");
                await SearchAsync(page, first);

                Assert.That(await page.Locator("li.list-group-item-menu").CountAsync(), Is.EqualTo(1), "The mask was saved twice");
            });
    }

    /// <summary>
    /// A spam word is saved once (the same word was added again).
    /// </summary>
    [Test]
    public async Task SpamWordAddedTwiceIsSavedOnceTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/SpamWords");

                var word = $"uitesttwice{Random.Shared.Next()}";

                await PostApiAsync(page, $"{this.TestSettings.TestForumUrl}api/SpamWords/Edit", new { id = 0, spamWord = word });
                await PostApiAsync(page, $"{this.TestSettings.TestForumUrl}api/SpamWords/Edit", new { id = 0, spamWord = word });

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/SpamWords");
                await SearchAsync(page, word);

                Assert.That(await page.Locator("li.list-group-item-menu").CountAsync(), Is.EqualTo(1), "The spam word was saved twice");
            });
    }

    /// <summary>
    /// A spam word is saved without the spaces around it (it is checked without them).
    /// </summary>
    [Test]
    public async Task SpamWordIsSavedWithoutSpacesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/SpamWords");

                var word = $"uitesttrim{Random.Shared.Next()}";

                await PostApiAsync(page, $"{this.TestSettings.TestForumUrl}api/SpamWords/Edit", new { id = 0, spamWord = $"  {word} " });

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/SpamWords");
                await SearchAsync(page, word);

                Assert.That(await page.Locator("li.list-group-item-menu").CountAsync(), Is.EqualTo(1), "The spam word was saved with the spaces");
            });
    }

    /// <summary>
    /// Sends the json to the api of a dialog (with the antiforgery token of the page).
    /// </summary>
    private static Task<string> PostApiAsync(IPage page, string url, object data)
    {
        return page.EvaluateAsync<string>(
            """
            async ([url, data]) => {
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                const response = await fetch(url, {
                    method: 'POST',
                    headers: { 'RequestVerificationToken': token, 'Content-Type': 'application/json' },
                    body: JSON.stringify(data)
                });
                return await response.text();
            }
            """,
            new[] { url, data });
    }

    /// <summary>
    /// Searches the list with the filter of the page.
    /// </summary>
    private static async Task SearchAsync(IPage page, string text)
    {
        // the search field can be in a closed dropdown
        await page.Locator("#SearchInput").EvaluateAsync("(input, text) => input.value = text", text);

        await page.RunAndWaitForNavigationAsync(
            () => page.Locator("button[formaction*='Search']").First.EvaluateAsync("button => button.click()"));
    }

    /// <summary>
    /// Sets the api key of AbuseIPDB in the host settings.
    /// </summary>
    private static async Task SetAbuseIpDbApiKeyAsync(IPage page, TestConfig testSettings, string key)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/HostSettings");

        await page.Locator("#Input_AbuseIpDbApiKey").EvaluateAsync("(input, key) => input.value = key", key);

        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Save')]").First);
    }
}
