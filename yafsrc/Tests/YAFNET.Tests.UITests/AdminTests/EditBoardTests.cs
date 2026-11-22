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

namespace YAF.Tests.UITests.AdminTests;

/// <summary>
/// The tests of the board editor (Admin > Boards > new or edit board).
/// </summary>
public class EditBoardTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The link "Boards" of the page links goes to the list of the boards: it went to a new board.
    /// </summary>
    [Test]
    public async Task BreadcrumbLinksToTheBoardsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditBoard");

                var link = page.Locator(".breadcrumb a", new PageLocatorOptions { HasText = "Boards" });

                Assert.That(await link.GetAttributeAsync("href"), Does.EndWith("Admin/Boards"), "The link doesn't go to the boards");
            });
    }

    /// <summary>
    /// Cancel goes back to the boards with empty fields: the form was checked and nothing happened.
    /// </summary>
    [Test]
    public async Task CancelWithEmptyFieldsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditBoard");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Cancel" })
                    .Or(page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Cancel" }))
                    .First.ClickAsync();

                await page.WaitForURLAsync("**/Admin/Boards", new PageWaitForURLOptions { Timeout = 10000 });

                Assert.That(page.Url, Does.EndWith("Admin/Boards"), "Cancel didn't go to the boards");
            });
    }

    /// <summary>
    /// The switch "create a new admin" keeps the cultures: the select was empty, a new board had no culture.
    /// </summary>
    [Test]
    public async Task CreateAdminUserKeepsTheCulturesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditBoard");

                await ShowAdminUserFieldsAsync(page);

                Assert.That(await page.Locator("#Input_UserPass1").CountAsync(), Is.EqualTo(1), "The fields of the new admin are not shown");
                Assert.That(await page.Locator("#Input_Culture option").CountAsync(), Is.GreaterThan(0), "The select of the cultures is empty");
            });
    }

    /// <summary>
    /// Turns the switch of the new admin on (the fields are shown).
    /// </summary>
    private static async Task ShowAdminUserFieldsAsync(IPage page)
    {
        var createAdmin = page.Locator("#Input_CreateAdminUser");

        // the switch sent the form (the fields were made on the server)
        if (await createAdmin.GetAttributeAsync("onchange") is not null)
        {
            await page.RunAndWaitForNextPageAsync(() => createAdmin.CheckAsync(new LocatorCheckOptions { Force = true }));
        }
        else
        {
            await createAdmin.CheckAsync(new LocatorCheckOptions { Force = true });
        }
    }
}
