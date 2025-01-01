/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Tests.AdminTests.Users;

/// <summary>
/// The Manage User Tests
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ManageUserTests : TestBase
{
    /// <summary>
    /// Delete the random test user test.
    /// </summary>
    [Test]
    [Description("Deletes the a Random Test User Account")]
    public async Task DeleteRandomTestUserTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.Base.TestSettings,
                        this.Base.TestSettings.AdminUserName,
                        this.Base.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // Do actual test

                await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Admin/Users");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Filter" }).ClickAsync();

                // Search for TestUser
                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync("TestUser");
                await searchNameInput.PressAsync("Enter");

                var editUserButton = page.Locator(
                    "//a[contains(@href,'EditUser')]",
                    new PageLocatorOptions { HasText = "TestUser" });

                await Assert.MultipleAsync(
                    async () =>
                    {
                        Assert.That(
                            editUserButton,
                            Is.Not.Null,
                            "Random Test User doesn't Exist, Please Run the Register_Random_New_User_Test before");

                        Assert.That(
                            await editUserButton.CountAsync() > 2,
                            Is.True,
                            "Random Test User doesn't Exist, Please Run the Register_Random_New_User_Test before");
                    });

                var href = await editUserButton.Nth(2).GetAttributeAsync("href");

                Assert.That(href, Is.Not.Null);

                var userId = href[(href.LastIndexOf('/') + 1)..];

                Assert.That(
                    await editUserButton.Nth(2).TextContentAsync(),
                    Is.Not.EqualTo(this.Base.TestSettings.TestUserName));

                await page.Locator($"//button[contains(@formaction,'Delete?id={userId}')]").First.ClickAsync();

                await page.Locator(".btn-success").ClickAsync();
            },
            this.BrowserType);
    }

    /// <summary>
    /// The Add User to Role Test.
    /// </summary>
    [Test]
    [Description("Assign the TestUser Account with the Moderator Role")]
    public async Task AddUserToTestRoleTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.Base.TestSettings,
                        this.Base.TestSettings.AdminUserName,
                        this.Base.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // Do actual test

                await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Admin/Users");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Filter" }).ClickAsync();

                // Search for TestUser
                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(this.Base.TestSettings.TestUserName);
                await searchNameInput.PressAsync("Enter");

                var userProfileLink = page.Locator("//a[contains(@href,'EditUser')]").First;

                Assert.That(userProfileLink, Is.Not.Null, "Test User doesn't Exist");

                await userProfileLink.ClickAsync();

                // Go To Tab User Roles
                await page.Locator("//button[contains(@data-bs-target,'#View2')]").ClickAsync();

                // Add TestUser to Test Role
                await page.GetByLabel("TestRole").First.CheckAsync();

                // Save changes
                await page.Locator("//button[contains(@formaction,'UsersGroups?handler=Save')]").ClickAsync();

                await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Admin/Users");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Filter" }).ClickAsync();

                // Search for TestUser
                searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(this.Base.TestSettings.TestUserName);
                await searchNameInput.PressAsync("Enter");

                userProfileLink = page.Locator("//a[contains(@href,'EditUser')]").First;

                Assert.That(userProfileLink, Is.Not.Null, "Test User doesn't Exist");

                await userProfileLink.ClickAsync();

                // Go To Tab User Roles
                await page.Locator("//button[contains(@data-bs-target,'#View2')]").ClickAsync();

                Assert.That(await page.GetByLabel("TestRole").First.IsCheckedAsync(), Is.True);

                await page.GetByLabel("TestRole").First.UncheckAsync();

                // Save changes
                await page.Locator("//button[contains(@formaction,'UsersGroups?handler=Save')]").ClickAsync();
            },
            this.BrowserType);
    }
}