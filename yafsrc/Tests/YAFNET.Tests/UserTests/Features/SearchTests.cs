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

using YAF.Tests.Infrastructure;

namespace YAF.Tests.UserTests.Features;

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
