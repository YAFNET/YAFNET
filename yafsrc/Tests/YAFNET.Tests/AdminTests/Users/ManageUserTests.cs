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

namespace YAF.Tests.AdminTests.Users;

/// <summary>
/// The Manage User Tests
/// </summary>
public class ManageUserTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Delete the random test user test.
    /// </summary>
    /// <summary>
    /// The admin can set and remove the signature of a user (a valid signature was not saved,
    /// an empty one or one with text failed with an error page).
    /// </summary>
    [Test]
    public async Task AdminEditsUserSignatureTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

                var editLink = await page.Locator("//a[contains(@href,'EditUser')]", new PageLocatorOptions { HasText = userName }).First
                                   .GetAttributeAsync("href");
                var userId = editLink![(editLink.LastIndexOf('/') + 1)..];

                // the tab of the user editor is a page of its own (the editor loads the user first)
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");

                var signatureUrl = $"{this.TestSettings.TestForumUrl}Admin/EditUser/UsersSignature?userId={userId}";
                var signature = $"Signature of the admin {Random.Shared.Next()}";

                await page.GotoAsync(signatureUrl);
                await page.EvaluateAsync("text => { const t = document.querySelector('#Signature'); const e = window.sceditor && sceditor.instance(t); if (e) { e.val(text); e.updateOriginal(); } else { t.value = text; } }", signature);

                var response = await page.RunAndWaitForNavigationAsync(
                                   () => page.Locator("button[formaction*='Save']").First.ClickAsync());

                Assert.That(response?.Status, Is.LessThan(500), "Saving the signature failed");

                await page.GotoAsync(signatureUrl);

                Assert.That(await page.EvaluateAsync<string>("() => { const t = document.querySelector('#Signature'); const e = window.sceditor && sceditor.instance(t); return e ? e.val() : t.value; }"), Is.EqualTo(signature), "The signature was not saved");

                // removed with an empty field
                await page.EvaluateAsync("text => { const t = document.querySelector('#Signature'); const e = window.sceditor && sceditor.instance(t); if (e) { e.val(text); e.updateOriginal(); } else { t.value = text; } }", string.Empty);

                response = await page.RunAndWaitForNavigationAsync(
                               () => page.Locator("button[formaction*='Save']").First.ClickAsync());

                Assert.That(response?.Status, Is.LessThan(500), "Removing the signature failed");

                await page.GotoAsync(signatureUrl);

                Assert.That(await page.EvaluateAsync<string>("() => { const t = document.querySelector('#Signature'); const e = window.sceditor && sceditor.instance(t); return e ? e.val() : t.value; }"), Is.Empty, "The signature was not removed");
            });
    }

    [Test]
    [Description("Deletes the a Random Test User Account")]
    public async Task DeleteRandomTestUserTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // a new user, only this one is deleted (not one of the standard test users)
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // Do actual test

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

                // Search for the new user
                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

                var editUserButton = page.Locator(
                    "//a[contains(@href,'EditUser')]",
                    new PageLocatorOptions { HasText = userName });

                Assert.That(await editUserButton.CountAsync(), Is.GreaterThan(0), $"{userName} not found");

                var href = await editUserButton.First.GetAttributeAsync("href");

                Assert.That(href, Is.Not.Null);

                var userId = href[(href.LastIndexOf('/') + 1)..];

                await page.Locator($"//button[contains(@formaction,'Delete?id={userId}')]").First.ClickAsync();

                await page.SubmitAndWaitAsync(page.Locator(".btn-success"));

                // the search field still contains the name, the list must not
                Assert.That(await editUserButton.CountAsync(), Is.Zero, $"{userName} was not deleted");
            });
    }

    /// <summary>
    /// The Add User to Role Test.
    /// </summary>
    [Test]
    [Description("Assign the TestUser Account with the Moderator Role")]
    public async Task AddUserToTestRoleTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                const string roleName = "TestRole";

                var pageSource = await page.CreateNewRole(this.TestSettings, roleName);

                Assert.That(pageSource, Does.Contain(roleName), "Test Role creating failed");

                // Do actual test

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

                // Search for TestUser
                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(this.TestSettings.TestUserName);
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

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

                // Search for TestUser
                searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(this.TestSettings.TestUserName);
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
            });
    }

    /// <summary>
    /// The users filter and "Lock inactive accounts" with values that are not in the range (manipulated form)
    /// don't fail.
    /// </summary>
    [Test]
    public async Task UsersFilterWithInvalidValuesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.EvaluateAsync(
                    """
                    () => {
                        const select = document.querySelector("select[name='Input.Since']");
                        select.add(new Option('x', 'x', true, true));
                        select.value = 'x';
                    }
                    """);

                // waits for the new page (the old page is loaded already when the response arrives)
                var response = await page.RunAndWaitForNavigationAsync(
                                   () => page.EvaluateAsync("() => document.querySelector(\"button[formaction*='/Search']\").click()"));

                Assert.That(response?.Status, Is.EqualTo(200), "The filter failed");

                await page.WaitForLoadStateAsync(LoadState.Load);

                Assert.That(
                    await page.Locator("//a[contains(@href,'EditUser')]").CountAsync(),
                    Is.GreaterThan(0),
                    "All users are listed");

                await page.Locator("input[name='Input.YearsOld']").EvaluateAsync("input => input.value = '1000000'");

                response = await page.RunAndWaitForResponseAsync(
                               () => page.EvaluateAsync("() => document.querySelector(\"button[formaction*='/LockAccounts']\").click()"),
                               r => r.Request.Method == "POST" && r.Url.Contains("Admin/Users"));

                Assert.That(response.Status, Is.EqualTo(200), "Lock inactive accounts failed");
            });
    }
}
