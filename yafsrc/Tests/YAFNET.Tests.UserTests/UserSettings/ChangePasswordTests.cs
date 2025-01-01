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
/// The user Change Password tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ChangePasswordTests : TestBase
{
    /// <summary>
    /// Change the user password test
    /// </summary>
    [Test]
    public async Task ChangeUserPasswordTest()
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
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/ChangePassword");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Change Password"),
                        "Change Password is not available for that User");

                    // Enter Old Password
                    await page.Locator("//input[contains(@id,'_Password')]")
                        .FillAsync(this.Base.TestSettings.TestUserPassword);

                    // Enter New Password
                    await page.Locator("//input[contains(@id,'_NewPassword')]")
                        .FillAsync($"{this.Base.TestSettings.TestUserPassword}ABCDEF");
                    await page.Locator("//input[contains(@id,'_ConfirmNewPassword')]")
                        .FillAsync($"{this.Base.TestSettings.TestUserPassword}ABCDEF");

                    // Submit
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Password has been successfully changed."),
                        "Changing Password Failed");

                    // Now Change Password Back to Default Password
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/ChangePassword");

                    // Enter Old Password
                    await page.Locator("//input[contains(@id,'_Password')]")
                        .FillAsync($"{this.Base.TestSettings.TestUserPassword}ABCDEF");

                    // Enter New Password
                    await page.Locator("//input[contains(@id,'_NewPassword')]")
                        .FillAsync(this.Base.TestSettings.TestUserPassword);
                    await page.Locator("//input[contains(@id,'_ConfirmNewPassword')]")
                        .FillAsync(this.Base.TestSettings.TestUserPassword);

                    // Submit
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Password has been successfully changed."),
                        "Changing Password Failed");
                },
            this.BrowserType);
    }
}