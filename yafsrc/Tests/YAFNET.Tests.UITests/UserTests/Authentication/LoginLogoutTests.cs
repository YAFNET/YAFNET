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

using HtmlProperties;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Authentication;

/// <summary>
/// The login/log off user tester.
/// </summary>
public class LoginLogoutUser(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Login via Login Page User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task LoginPageUser()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                {
                    // Logout First
                    await page.LogOutUserAsync();
                }

                Assert.That(await page.LoginUserAsync(
                    this.TestSettings,
                    this.TestSettings.TestUserName,
                    this.TestSettings.TestUserPassword), Is.True, "Login Failed");
            });
    }

    /// <summary>
    /// Login via Login Box User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task LoginLoginBoxUser()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                {
                    // Logout First
                    await page.LogOutUserAsync();

                    await page.GotoAsync(this.TestSettings.TestForumUrl);
                }

                if (await page.Locator(HtmlTag.Button).Filter(new LocatorFilterOptions { HasText = "Close" })
                        .IsVisibleAsync())
                {
                    await page.Locator(HtmlTag.Button).Filter(new LocatorFilterOptions { HasText = "Close" })
                        .ClickAsync();
                }

                await page.Locator("#navbarSupportedContent")
                    .GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Login" })
                    .ClickAsync();

                await page.Locator("//*[contains(@id, 'UserName')]").WaitForAsync();

                await page.Locator("//*[contains(@id, 'UserName')]").FillAsync(this.TestSettings.TestUserName);
                await page.Locator("//input[contains(@id, 'Password')]")
                    .FillAsync(this.TestSettings.TestUserPassword);

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Forum Login" }).ClickAsync();

                Assert.That(await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(), Is.True);
            });
    }

    /// <summary>
    /// The login box trims the user name like the login page (autocomplete can add a space).
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task LoginBoxUserNameWithSpaceTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                {
                    await page.LogOutUserAsync();

                    await page.GotoAsync(this.TestSettings.TestForumUrl);
                }

                if (await page.Locator(HtmlTag.Button).Filter(new LocatorFilterOptions { HasText = "Close" })
                        .IsVisibleAsync())
                {
                    await page.Locator(HtmlTag.Button).Filter(new LocatorFilterOptions { HasText = "Close" })
                        .ClickAsync();
                }

                await page.Locator("#navbarSupportedContent")
                    .GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Login" })
                    .ClickAsync();

                await page.Locator("//*[contains(@id, 'UserName')]").WaitForAsync();

                await page.Locator("//*[contains(@id, 'UserName')]").FillAsync($"{this.TestSettings.TestUserName} ");
                await page.Locator("//input[contains(@id, 'Password')]")
                    .FillAsync(this.TestSettings.TestUserPassword);

                await page.SubmitAndWaitAsync(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Forum Login" }));

                Assert.That(
                    await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(),
                    Is.True,
                    "The login with a space after the user name failed");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
            });
    }

    /// <summary>
    /// Logout User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task LogoutUser()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                if (!await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                {
                    // Login First
                    Assert.That(await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True, "Login Failed");
                }

                Assert.That(
                    await page.LogOutUserAsync(), Is.True,
                    "Logout Failed");
            });
    }

    /// <summary>
    /// Opening the logout link (e.g. an image on another site) does not log the user out, only the logout dialog does.
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task LogoutLinkDoesNotLogOutTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                if (!await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                {
                    Assert.That(await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True, "Login Failed");
                }

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/Logout");

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                Assert.That(
                    await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(),
                    Is.True,
                    "The logout link logged the user out");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout Failed");
            });
    }

    /// <summary>
    /// The login page has no register button when registrations are disabled (it led to an access denied page).
    /// </summary>
    [Test]
    public async Task NoRegisterButtonWhenRegistrationsAreDisabledTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    await SetRegistrationsDisabledAsync(page, this.TestSettings, true);

                    try
                    {
                        await using var guestContext = await this.Fixture.Context.Browser!.NewContextAsync(
                                                           new BrowserNewContextOptions
                                                               {
                                                                   BaseURL = this.TestSettings.TestForumUrl,
                                                                   IgnoreHTTPSErrors = true
                                                               });

                        var guest = await guestContext.NewPageAsync();

                        await guest.GotoAsync($"{this.TestSettings.TestForumUrl}Account/Login");

                        await Assert.MultipleAsync(
                            async () =>
                                {
                                    Assert.That(await guest.Locator("#Login").CountAsync(), Is.EqualTo(1), "No login page");
                                    Assert.That(
                                        await guest.Locator("button[formaction*='Register']").CountAsync(),
                                        Is.Zero,
                                        "The login page offers to register");
                                });
                    }
                    finally
                    {
                        await SetRegistrationsDisabledAsync(page, this.TestSettings, false);
                    }
                });
    }

    /// <summary>
    /// Turns the registrations off or on in the host settings.
    /// </summary>
    private async static Task SetRegistrationsDisabledAsync(IPage page, TestConfig testSettings, bool disabled)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/HostSettings");

        await page.Locator("#Input_DisableRegistrations").EvaluateAsync("(e, v) => e.checked = v", disabled);

        var save = page.Locator("button[formaction*='Save']").First;

        await page.RunAndWaitForNextPageAsync(() => save.EvaluateAsync("b => b.form.requestSubmit(b)"));

        await page.WaitForLoadStateAsync();
    }
}