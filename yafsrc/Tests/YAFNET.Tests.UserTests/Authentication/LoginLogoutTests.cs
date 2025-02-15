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

using HtmlProperties;

namespace YAF.Tests.UserTests.Authentication;

/// <summary>
/// The login/log off user tester.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LoginLogoutUser : TestBase
{
    /// <summary>
    /// Login via Login Page User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task LoginPageUser()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                    {
                        // Logout First
                        await page.LogOutUserAsync();
                    }

                    Assert.That(await page.LoginUserAsync(
                                      this.Base.TestSettings,
                                      this.Base.TestSettings.TestUserName,
                                      this.Base.TestSettings.TestUserPassword), Is.True, "Login Failed");
                }, this.BrowserType);
    }

    /// <summary>
    /// Login via Login Box User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task LoginLoginBoxUser()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                    {
                        // Logout First
                        await page.LogOutUserAsync();

                        await page.GotoAsync(this.Base.TestSettings.TestForumUrl);
                    }

                    if (await page.Locator(HtmlTag.Button).Filter(new LocatorFilterOptions { HasText = "Close" }).IsVisibleAsync())
                    {
                        await page.Locator(HtmlTag.Button).Filter(new LocatorFilterOptions { HasText = "Close" }).ClickAsync();
                    }

                    await page.Locator("#navbarSupportedContent").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Login" })
                        .ClickAsync();

                    await page.Locator("//*[contains(@id, 'UserName')]").WaitForAsync();

                    await page.Locator("//*[contains(@id, 'UserName')]").FillAsync(this.Base.TestSettings.TestUserName);
                    await page.Locator("//input[contains(@id, 'Password')]")
                        .FillAsync(this.Base.TestSettings.TestUserPassword);

                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Forum Login" }).ClickAsync();

                    Assert.That( await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(), Is.True);
                }, this.BrowserType);
    }

    /// <summary>
    /// Logout User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task LogoutUser()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    if (!await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                    {
                        // Login First
                        Assert.That(await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True, "Login Failed");
                    }

                    Assert.That(
                        await page.LogOutUserAsync(), Is.True,
                        "Logout Failed");
                },
            this.BrowserType);
    }
}