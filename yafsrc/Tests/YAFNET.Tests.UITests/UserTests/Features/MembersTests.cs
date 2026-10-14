﻿/* Yet Another Forum.NET
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
/// The members list tests.
/// </summary>
public class MembersTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The filter by number of posts uses the selected comparison (equal, less or equal, more or equal):
    /// the page always replaced it with "more or equal".
    /// </summary>
    [Test]
    public async Task NumberOfPostsFilterUsesSelectedComparisonTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // a new user has no posts
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                var userRow = page.Locator("li.member-row", new PageLocatorOptions { HasTextString = userName });

                // "more or equal 5": the user is not listed
                await SearchAsync(page, this.TestSettings, userName, "3", "5");

                Assert.That(await userRow.CountAsync(), Is.Zero, "A user without posts is listed for \"more or equal 5 posts\"");

                // "less or equal 5": the user is listed
                await SearchAsync(page, this.TestSettings, userName, "2", "5");

                Assert.That(
                    await page.Locator("#Input_NumPostValue").InputValueAsync(),
                    Is.EqualTo("2"),
                    "The selected comparison was replaced");
                Assert.That(await userRow.CountAsync(), Is.EqualTo(1), "A user without posts is not listed for \"less or equal 5 posts\"");
            });
    }

    private static async Task SearchAsync(IPage page, TestConfig testSettings, string userName, string comparison, string numberOfPosts)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Members");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();
        await page.Locator("#Input_UserSearchName").FillAsync(userName);
        await page.Locator("#Input_NumPosts").FillAsync(numberOfPosts);

        // the select is replaced by a script control
        await page.EvaluateAsync("value => document.querySelector('#Input_NumPostValue').value = value", comparison);

        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Search')]"));
    }
}
