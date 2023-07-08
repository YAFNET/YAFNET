/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
                    Assert.IsTrue(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword),
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Edit Settings"), "Edit Settings is not available for that User");

                    Assert.IsTrue(
                        pageSource.Contains("What language do you want to use"),
                        "Changing the Language is disabled for Users");

                    // Switch Language to German
                    await page.Locator("#select2-Language-container").ClickAsync();
                    await page.Locator("//li[contains(@id,'-de-DE')]").ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").ClickAsync();

                    await page.ReloadAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Dein Account"), "Changing Language failed");

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    // Switch Language Back to English
                    await page.Locator("#select2-Language-container").ClickAsync();
                    await page.Locator("//li[contains(@id,'-en-US')]").First.ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").ClickAsync();

                    await page.ReloadAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Your Account"), "Changing Language failed");
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
                    Assert.IsTrue(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword),
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Edit Settings"), "Edit Settings is not available for that User");

                    Assert.IsTrue(
                        pageSource.Contains("Select your preferred theme"),
                        "Changing the Theme is disabled for Users");

                    // Switch Theme to "flatly"
                    await page.Locator("#select2-Theme-container").ClickAsync();
                    await page.Locator("//li[contains(@id,'-flatly')]").ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").ClickAsync();

                    Assert.IsNotNull(
                        await page.Locator("//link[contains(@href,'Themes/flatly/bootstrap')]").IsVisibleAsync(),
                        "Changing Forum Theme failed");

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    // Switch Theme back to the yaf theme
                    await page.Locator("#select2-Theme-container").ClickAsync();
                    await page.Locator("//li[contains(@id,'-yaf')]").ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").ClickAsync();

                    Assert.IsNotNull(
                        await page.Locator("//link[contains(@href,'Themes/yaf/bootstrap')]").IsVisibleAsync(),
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
                    Assert.IsTrue(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword),
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Edit Settings"), "Edit Settings is not available for that User");

                    Assert.IsTrue(
                        pageSource.Contains("Change Email Address"),
                        "Changing the Email Address is disabled for Users");

                    // Switch Theme to "Black Grey"
                    var emailInput = page.Locator("//input[contains(@id,'Email')]");

                    var oldEmailAddress = await emailInput.GetAttributeAsync("value");
                    const string NewEmailAddress = "testmail123@localhost.com";

                    Assert.IsNotNull(oldEmailAddress);

                    await emailInput.ClearAsync();

                    await emailInput.FillAsync(NewEmailAddress);

                    // Save the Profile Changes
                    await page.Locator("//button[contains(@type,'submit')]").ClickAsync();

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    Assert.AreEqual(
                        NewEmailAddress,
                        await emailInput.GetAttributeAsync("value"),
                        $"Email Address should match {NewEmailAddress}");

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    // Switch Email Address back
                    emailInput = page.Locator("//input[contains(@id,'Email')]");
                    await emailInput.ClearAsync();
                    await emailInput.FillAsync(oldEmailAddress);

                    // Save the Profile Changes
                    await page.Locator("//button[contains(@type,'submit')]").ClickAsync();

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSettings");

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains(oldEmailAddress), "Email Address Changing back failed");
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
                    Assert.IsTrue(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword),
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditProfile");

                    var pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Edit Profile"), "Edit Profile is not available for that User");

                    Assert.IsTrue(pageSource.Contains("Country"), "Changing the Country is disabled for Users");

                    // Switch Country to "Germany"
                    await page.Locator("#select2-Input_Country-container").ClickAsync();
                    await page.GetByRole(AriaRole.Option, new() { Name = "Germany" }).ClickAsync();

                    // Switch Region to "Berlin"
                    await page.Locator("#select2-Input_Region-container").ClickAsync();
                    await page.GetByRole(AriaRole.Option, new() { Name = "Berlin" }).ClickAsync();

                    // Save the Profile Changes
                    await page.Locator("//button[contains(@formaction,'UpdateProfile')]").ClickAsync();

                    await page.GetByText("View Profile").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Germany") && pageSource.Contains("Berlin"));
                },
            this.BrowserType);
    }
}