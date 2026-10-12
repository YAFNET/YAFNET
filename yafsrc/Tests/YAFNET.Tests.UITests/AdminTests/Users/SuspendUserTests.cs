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

namespace YAF.Tests.UITests.AdminTests.Users;

/// <summary>
/// The suspend user tests.
/// </summary>
public class SuspendUserTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A suspended user sees the info page and can't change anything with the api (e.g. thank posts).
    /// </summary>
    [Test]
    public async Task SuspendedUserCanNotUseTheApiTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

                var href = await page.Locator(
                    "//a[contains(@href,'EditUser')]",
                    new PageLocatorOptions { HasText = userName }).First.GetAttributeAsync("href");

                Assert.That(href, Is.Not.Null, $"{userName} not found");

                var userId = href[(href.LastIndexOf('/') + 1)..];

                // suspend for one day
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");

                // the tab is loaded with ajax
                await page.Locator("//button[contains(@data-bs-target,'#View8')]").ClickAsync();
                var suspendCount = page.Locator("#View8 input[id$='_SuspendCount']");
                await suspendCount.WaitForAsync();
                await suspendCount.FillAsync("1");
                await page.SubmitAndWaitAsync(page.Locator("#View8 button[formaction*='handler=Suspend']"));

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword);

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("suspended"),
                    "The suspended user is not sent to the info page");

                // a request with a valid antiforgery token, the api has to deny it
                var status = await page.EvaluateAsync<int>(
                    """
                    async url => {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                        const response = await fetch(url, { method: 'POST', headers: { 'RequestVerificationToken': token } });
                        return response.status;
                    }
                    """,
                    $"{this.TestSettings.TestForumUrl}api/ThankYou/AddThanks/1");

                Assert.That(status, Is.EqualTo(403), "The suspended user could use the api");
            });
    }
}
