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
/// The tests of the maintenance pages (attachments, tags, prune, recycle bin, private messages, event log).
/// </summary>
public class MaintenanceTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The selected page size of the deleted posts is used: the posts were always listed with the page size of the user.
    /// </summary>
    [Test]
    public async Task DeletedPostsPageSizeIsUsedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                // six deleted posts
                for (var i = 0; i < 6; i++)
                {
                    var topicId = await page.CreateTopicAsync(
                                      this.TestSettings,
                                      this.TestSettings.TestForumId,
                                      $"Recycle bin paging {Random.Shared.Next()}",
                                      "A post of the recycle bin test");

                    Assert.That(topicId, Is.Not.Null, "The topic was not created");

                    var deleteUrl = await page.Locator("a[href*='DeleteMessage']").First.GetAttributeAsync("href");

                    await page.GotoAsync(new Uri(new Uri(page.Url), deleteUrl).ToString());
                    await page.Locator("#ReasonEditor").FillAsync("Deleted by a test");
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Delete']"));
                }

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Restore");

                // the page size of the user is selected, another size is chosen
                var select = page.Locator("#MessagesPageSize");
                var userSize = int.Parse(await select.InputValueAsync());
                var size = userSize > 5 ? 5 : 10;

                await page.RunAndWaitForNavigationAsync(() => select.SelectOptionAsync(size.ToString()));

                var rows = await page.Locator("button[formaction*='RestorePost']").CountAsync();

                // the old code listed min(user size, deleted posts) rows, at least 6 posts are deleted
                if (size == 5)
                {
                    Assert.That(rows, Is.EqualTo(5), $"The posts are listed with the page size of the user ({userSize})");
                }
                else
                {
                    Assert.That(rows, Is.GreaterThan(userSize), $"The posts are listed with the page size of the user ({userSize})");
                }
            });
    }

    /// <summary>
    /// The search of the deleted topics starts on the first page: it searched on the shown page, a search with less
    /// results than that page showed nothing.
    /// </summary>
    [Test]
    public async Task DeletedTopicsSearchStartsOnTheFirstPageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                // the second page of the deleted topics
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Restore/2/1");

                var action = await page.Locator("button[formaction*='Refresh']").GetAttributeAsync("formaction");

                Assert.That(action, Does.Not.Contain("/2/"), $"The search is on the shown page ({action})");
            });
    }

    /// <summary>
    /// The filter of the event log starts on the first page: it was applied on the shown page, a filter with less
    /// entries than that page showed nothing.
    /// </summary>
    [Test]
    public async Task EventLogFilterStartsOnTheFirstPageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                // the second page of the event log
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EventLog/2");

                var action = await page.Locator("button[formaction*='ApplyButton']").GetAttributeAsync("formaction");

                Assert.That(action, Does.Not.Contain("/2/"), $"The filter is applied on the shown page ({action})");
            });
    }
}
