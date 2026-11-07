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

namespace YAF.Tests.UITests.UserTests.Features;

/// <summary>
/// The search tests.
/// </summary>
public class SearchTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A topic title with (broken) html is shown as text in the search results (which are rendered with javascript),
    /// the html is not executed.
    /// </summary>
    [Test]
    public async Task SearchResultsShowTopicHtmlAsTextTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // a new user, no flood delay
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                var marker = $"XssCheck{Random.Shared.Next()}";

                // the tag is not closed, so it is not removed when the topic is saved
                var subject = $"{marker} <img src=x onerror=window.xssFired=1 x";

                Assert.That(
                    await page.CreateTopicAsync(this.TestSettings, this.TestSettings.TestForumId, subject, "A topic for the search test"),
                    Is.Not.Null,
                    "The topic was not created");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Search?search={marker}");

                var result = page.Locator("#SearchResultsPlaceholder").GetByText(marker).First;
                await result.WaitForAsync();

                Assert.That(
                    await page.EvaluateAsync<bool>("() => window.xssFired === 1"),
                    Is.False,
                    "The html of the topic title was executed");

                Assert.That(
                    await page.Locator("#SearchResultsPlaceholder img[onerror]").CountAsync(),
                    Is.Zero,
                    "The html of the topic title was rendered");

                Assert.That(
                    await page.Locator("#SearchResultsPlaceholder").InnerTextAsync(),
                    Does.Contain("<img src=x"),
                    "The topic title is not shown as text");
            });
    }

    /// <summary>
    /// "All Forums" is selected by default, the clear button resets the page, the search for forums shows
    /// the forums and disables the filters that are only for posts.
    /// </summary>
    [Test]
    public async Task SearchOptionsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Search?search=NothingToFind");

                var forumSelect = page.Locator(".choices-forum .choices__item--selectable").First;
                await forumSelect.WaitForAsync();

                Assert.That(await forumSelect.InnerTextAsync(), Does.Contain("All Forums"), "All Forums is not selected");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Clear" }).ClickAsync();
                await page.WaitForURLAsync(url => !url.Contains("NothingToFind"));

                Assert.That(await page.Locator("#searchInput").InputValueAsync(), Is.Empty, "The search text was not cleared");

                var forumName = await page.EvaluateAsync<string>(
                    """
                    async () => {
                        const response = await fetch('/api/Forum/GetForums', {
                            method: 'POST',
                            body: JSON.stringify({ PageSize: 0, Page: 0, SearchTerm: '' }),
                            headers: {
                                'Content-Type': 'application/json;charset=utf-8',
                                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                            }
                        });

                        return (await response.json()).results[0].children[0].text;
                    }
                    """);

                await page.Locator("#searchInput").FillAsync(forumName);
                await page.Locator("label[for='SearchType2']").ClickAsync();

                await page.Locator("#SearchResultsPlaceholder").GetByRole(AriaRole.Link, new LocatorGetByRoleOptions { Name = forumName })
                    .First.WaitForAsync();

                Assert.That(await page.Locator(".searchUserInput").IsDisabledAsync(), Is.True, "The user filter is not disabled");
                Assert.That(await page.Locator("#TitleOnly1").IsDisabledAsync(), Is.True, "The title filter is not disabled");
                Assert.That(await page.Locator("#ResultsPerPage5").IsCheckedAsync(), Is.True, "5 results per page is not selected");
                Assert.That(await page.Locator("#SearchWhat2").IsCheckedAsync(), Is.True, "Match exact phrase is not selected");
            });
    }

    /// <summary>
    /// Hidden content (here hide-reply) can't be found by the search and is not shown in the results
    /// (it was indexed with the post).
    /// </summary>
    [Test]
    public async Task SearchDoesNotFindHiddenContentTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                var visible = $"VisibleWord{Random.Shared.Next()}";
                var hidden = $"HiddenWord{Random.Shared.Next()}";

                Assert.That(
                    await page.CreateTopicAsync(
                        this.TestSettings,
                        this.TestSettings.TestForumId,
                        $"Hidden search topic {Random.Shared.Next()}",
                        $"{visible} [hide-reply]{hidden}[/hide-reply]"),
                    Is.Not.Null,
                    "The topic was not created");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.TestUserName, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                // the visible word finds the post, without the hidden word
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Search?search={visible}");

                var results = page.Locator("#SearchResultsPlaceholder");

                await results.GetByText(visible).First.WaitForAsync();

                Assert.That(await results.InnerTextAsync(), Does.Not.Contain(hidden), "The hidden content is shown in the result");

                // the hidden word finds nothing
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Search?search={hidden}");
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                Assert.That(
                    await page.Locator("#SearchResultsPlaceholder").InnerTextAsync(),
                    Does.Not.Contain("Hidden search topic"),
                    "The post was found by its hidden content");
            });
    }
}
