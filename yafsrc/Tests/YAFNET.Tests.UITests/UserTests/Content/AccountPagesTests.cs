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

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Content;

/// <summary>
/// The account pages of the user (my account, the notifications).
/// </summary>
public class AccountPagesTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The notifications show the unread count, the breadcrumb leads to the account and names the page
    /// (the name of the user linked to the notifications itself).
    /// </summary>
    [Test]
    public async Task NotificationsHeaderAndBreadcrumbTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword),
                        Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Notification");

                    var crumbs = page.Locator(".breadcrumb li");

                    await Assert.MultipleAsync(
                        async () =>
                            {
                                Assert.That(await page.Locator("#UnreadCount").CountAsync(), Is.EqualTo(1), "The unread count is missing");
                                Assert.That(
                                    await crumbs.Nth(1).Locator("a").GetAttributeAsync("href"),
                                    Does.EndWith("/MyAccount").IgnoreCase,
                                    "The user does not link to the account");
                                Assert.That(
                                    await crumbs.Last.InnerTextAsync(),
                                    Is.EqualTo("Your activity notifications"),
                                    "The page is not named");
                            });
                });
    }
}
