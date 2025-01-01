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

namespace YAF.Tests.UserTests.Authentication;

/// <summary>
/// The Register a test user Test.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RegisterUser : TestBase
{
    /// <summary>
    /// Register Random Test User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task RegisterRandomNewUser()
    {
        // Create New Random Test User
        var random = new Random();

        var userName = $"TestUser{random.Next()}";

        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.RegisterUserAsync(
                            this.Base.TestSettings,
                            this.Base.SmtpServer,
                            userName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Registration failed");
                }, this.BrowserType);
    }

    /// <summary>
    /// Register Random Test User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task RegisterBotUserTest()
    {
        const string username = "aqiuliqemi";
        const string email = "ikocec@coveryourpills.org";

        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Account/Register");

                    var pageSource = await page.ContentAsync();

                    // Check if Registrations are Disabled
                    Assert.That(
                        pageSource.Contains("You tried to enter an area where you didn't have access"), Is.False,
                        "Registrations are disabled");

                    // Accept the Rules
                    if (pageSource.Contains("Forum Rules"))
                    {
                        await page.ReloadAsync();
                    }

                    // Fill the Register Page
                    await page.Locator("//input[contains(@id, '_UserName')]").FillAsync(username);

                    if (pageSource.Contains("Display Name") || pageSource.Contains("Anzeigename"))
                    {
                        await page.Locator("//input[contains(@id, '_DisplayName')]").FillAsync(username);
                    }

                    await page.Locator("//input[contains(@id, '_Password')]").FillAsync(this.Base.TestSettings.TestUserPassword);
                    await page.Locator("//input[contains(@id, '_ConfirmPassword')]").FillAsync(this.Base.TestSettings.TestUserPassword);
                    await page.Locator("//input[contains(@id, '_Email')]").FillAsync(email);

                    // Create User
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Register" }).ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Sorry Spammers are not allowed in the Forum!"),
                        "Spam Check Failed");
                },
            this.BrowserType);
    }
}