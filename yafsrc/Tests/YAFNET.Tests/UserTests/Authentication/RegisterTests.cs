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

using AwesomeAssertions;

using YAF.Tests.Infrastructure;

namespace YAF.Tests.UserTests.Authentication;

/// <summary>
/// The Register a test user Test.
/// </summary>
public class RegisterUser(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Register Random Test User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task RegisterRandomNewUser()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var result = await page.RegisterRandomTestUserAsync(
                            this.TestSettings,
                            this.Fixture.SmtpServer);

                        result.Should().BeTrue("Registration failed");
                });
    }

    /// <summary>
    /// Register Random Test User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    [Ignore("TODO")]
    public async Task RegisterBotUserTest()
    {
        const string username = "aqiuliqemi";
        const string email = "ikocec@coveryourpills.org";

        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // TODO : Setup spam protection first

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/Register");

                    var pageSource = await page.ContentAsync();

                    if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                    {
                        // Logout First
                        await page.LogOutUserAsync();

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/Register");
                    }

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

                    await page.Locator("//input[contains(@id, '_Password')]").FillAsync(this.TestSettings.TestUserPassword);
                    await page.Locator("//input[contains(@id, '_ConfirmPassword')]").FillAsync(this.TestSettings.TestUserPassword);
                    await page.Locator("//input[contains(@id, '_Email')]").FillAsync(email);

                    // Create User
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Sorry Spammers are not allowed in the Forum!"),
                        "Spam Check Failed");
                });
    }
}