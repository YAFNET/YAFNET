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

namespace YAF.Tests.UserTests.UserSettings;

/// <summary>
/// The User Profile tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ProfileTests : TestBase
{
    /// <summary>
    /// Changes the forum language test.
    /// </summary>
    [Test]
    public async Task ChangeForumLanguageTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Settings"), "Edit Settings is not available for that User");

                    Assert.That(
                        pageSource, Does.Contain("What language do you want to use"),
                        "Changing the Language is disabled for Users");

                    // Switch Language to German
                    await page.Locator("//select[@id='Language']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'de-DE')]").ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    await page.ReloadAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Dein Account"), "Changing Language failed");

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    // Switch Language Back to English
                    await page.Locator("//select[@id='Language']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'en-US')]").First.ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    await page.ReloadAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Your Account"), "Changing Language failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Changes the forum theme test.
    /// </summary>
    [Test]
    public async Task ChangeForumThemeTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Settings"), "Edit Settings is not available for that User");

                    Assert.That(
                        pageSource, Does.Contain("Select your preferred theme"),
                        "Changing the Theme is disabled for Users");

                    // Switch Theme to "flatly"
                    await page.Locator("//select[@id='Theme']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'flatly')]").ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("themes/flatly/bootstrap-forum"), Is.True,
                        "Changing Forum Theme failed");

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    // Switch Theme back to the yaf theme
                    await page.Locator("//select[@id='Theme']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'yaf')]").ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("themes/yaf/bootstrap-forum"), Is.True,
                        "Changing Forum Theme failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Change the user email address test.
    /// </summary>
    [Test]
    public async Task ChangeUserEmailAddressTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Settings"), "Edit Settings is not available for that User");

                    Assert.That(
                        pageSource, Does.Contain("Change Email Address"),
                        "Changing the Email Address is disabled for Users");

                    // Switch Theme to "Black Grey"
                    var emailInput = page.Locator("//input[contains(@id,'Email')]");

                    var oldEmailAddress = await emailInput.GetAttributeAsync("value");
                    const string newEmailAddress = "testmail123@localhost.com";

                    Assert.That(oldEmailAddress, Is.Not.Null);

                    await emailInput.ClearAsync();

                    await emailInput.FillAsync(newEmailAddress);

                    // Save the Profile Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    Assert.That(
                        await emailInput.GetAttributeAsync("value"), Is.EqualTo(newEmailAddress),
                        $"Email Address should match {newEmailAddress}");

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    // Switch Email Address back
                    emailInput = page.Locator("//input[contains(@id,'Email')]");
                    await emailInput.ClearAsync();
                    await emailInput.FillAsync(oldEmailAddress);

                    // Save the Profile Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain(oldEmailAddress), "Email Address Changing back failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Set the user country and region test.
    /// </summary>
    [Test]
    public async Task SetUserCountryAndRegionTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditProfile");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Profile"), "Edit Profile is not available for that User");

                    Assert.That(pageSource, Does.Contain("Country"), "Changing the Country is disabled for Users");

                    // Switch Country to "Germany"
                    await page.Locator("//select[@id='Input_Country']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'DE')]").Last.ClickAsync();

                    // Switch Region to "Berlin"
                    await page.Locator("//select[@id='Input_Region']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'BER')]").Last.ClickAsync();

                    // Save the Profile Changes
                    await page.Locator("//button[contains(@formaction,'UpdateProfile')]").ClickAsync();

                    await page.GetByText("View Profile").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource.Contains("Germany") && pageSource.Contains("Berlin"), Is.True);
                },
            this.BrowserType);
    }
}