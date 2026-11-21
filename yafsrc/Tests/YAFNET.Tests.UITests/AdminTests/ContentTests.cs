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

using System.Text.RegularExpressions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.AdminTests;

/// <summary>
/// The tests of the content pages (board announcement, replace words, bb codes).
/// </summary>
public class ContentTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The end of the announcement is shown as a date of the user (it was the stored invariant text, e.g. 11/22/2026 15:33:00).
    /// </summary>
    [Test]
    public async Task AnnouncementEndIsShownAsDateTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/BoardAnnouncement");

                try
                {
                    await page.Locator("#Input_Message").FillAsync("UI test announcement");
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='SaveAnnouncement']"));

                    var content = await page.ContentAsync();

                    Assert.That(content, Does.Contain("UI test announcement"), "The announcement was not saved");
                    Assert.That(
                        Regex.IsMatch(content, @"\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}"),
                        Is.False,
                        "The end of the announcement is shown as stored text");
                }
                finally
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/BoardAnnouncement");

                    var delete = page.Locator("button[formaction*='Delete']");

                    if (await delete.CountAsync() > 0)
                    {
                        page.Dialog += (_, dialog) => dialog.AcceptAsync();
                        await page.SubmitAndWaitAsync(delete.First);
                    }
                }
            });
    }

    /// <summary>
    /// A replace word is saved without the spaces around it (it is checked without them).
    /// </summary>
    [Test]
    public async Task ReplaceWordIsSavedWithoutSpacesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ReplaceWords");

                var word = $"uitestbad{Random.Shared.Next()}";

                await PostApiAsync(page, $"{this.TestSettings.TestForumUrl}api/ReplaceWords/Edit", new { id = 0, badWord = $"  {word} ", goodWord = "good" });

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ReplaceWords");

                Assert.That(await GetBadWordOfTheDialogAsync(page, word), Is.EqualTo(word), "The replace word was saved with the spaces");
            });
    }

    /// <summary>
    /// A bad word can be added once (the same word was added again).
    /// </summary>
    [Test]
    public async Task ReplaceWordAddedTwiceIsRefusedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ReplaceWords");

                var word = $"uitesttwice{Random.Shared.Next()}";

                await PostApiAsync(page, $"{this.TestSettings.TestForumUrl}api/ReplaceWords/Edit", new { id = 0, badWord = word, goodWord = "one" });
                var response = await PostApiAsync(page, $"{this.TestSettings.TestForumUrl}api/ReplaceWords/Edit", new { id = 0, badWord = word, goodWord = "two" });

                Assert.That(response, Does.Contain("already exists"), "The dialog shows no message");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ReplaceWords");

                Assert.That(await page.Locator($"li:has-text('{word}')").CountAsync(), Is.EqualTo(1), "The bad word was saved twice");
            });
    }

    /// <summary>
    /// Cancel of a new bb code shows the bb codes (it was a submit button, the empty required fields stopped it).
    /// </summary>
    [Test]
    public async Task CancelOfANewBBCodeShowsTheBBCodesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditBBCode");

                await page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Cancel" })
                    .Or(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Cancel" })).First.ClickAsync();

                try
                {
                    await page.WaitForURLAsync(url => url.Contains("Admin/BBCodes"), new PageWaitForURLOptions { Timeout = 5000 });
                }
                catch (TimeoutException)
                {
                    // checked below
                }

                Assert.That(page.Url, Does.Contain("Admin/BBCodes"), "Cancel didn't show the bb codes");
            });
    }

    /// <summary>
    /// A bb code without a name shows a message (the save stopped with an error).
    /// </summary>
    [Test]
    public async Task BBCodeWithoutNameShowsAMessageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditBBCode");

                // as a crafted request: the name is left out, the check of the browser is removed
                await page.EvaluateAsync(
                    """
                    () => {
                        document.querySelectorAll('[required]').forEach(e => e.removeAttribute('required'));
                        document.querySelector('#Input_Name').value = '';
                        document.querySelector('#Input_Description').value = 'UI test';
                    }
                    """);

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Add']"));

                Assert.That(await page.ContentAsync(), Does.Contain("You must enter a name."), "The empty name showed no message");
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
    /// Gets the bad word as the edit dialog of the list shows it.
    /// </summary>
    private static Task<string> GetBadWordOfTheDialogAsync(IPage page, string word)
    {
        return page.EvaluateAsync<string>(
            """
            async word => {
                const row = [...document.querySelectorAll('li')].find(li => li.textContent.includes(word));
                const url = row.querySelector('[data-url*="Edit"]').getAttribute('data-url');
                const html = await (await fetch(url)).text();
                const dialog = new DOMParser().parseFromString(html, 'text/html');
                return dialog.querySelector('#BadWord').value;
            }
            """,
            word);
    }
}
