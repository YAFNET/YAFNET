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

namespace YAF.Tests.UITests.UserTests.UserSettings;

/// <summary>
/// The user Change Password tests.
/// </summary>
public class ChangePasswordTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Change the user password test
    /// </summary>
    [Test]
    public async Task ChangeUserPasswordTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/ChangePassword");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Change Password"),
                        "Change Password is not available for that User");

                    // Enter Old Password
                    await page.Locator("//input[contains(@id,'_Password')]")
                        .FillAsync(this.TestSettings.TestUserPassword);

                    // Enter New Password
                    await page.Locator("//input[contains(@id,'_NewPassword')]")
                        .FillAsync($"{this.TestSettings.TestUserPassword}ABCDEF");
                    await page.Locator("//input[contains(@id,'_ConfirmNewPassword')]")
                        .FillAsync($"{this.TestSettings.TestUserPassword}ABCDEF");

                    // Submit
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@type,'submit')]").Last);

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Password has been successfully changed."),
                        "Changing Password Failed");

                    // Now Change Password Back to Default Password
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/ChangePassword");

                    // Enter Old Password
                    await page.Locator("//input[contains(@id,'_Password')]")
                        .FillAsync($"{this.TestSettings.TestUserPassword}ABCDEF");

                    // Enter New Password
                    await page.Locator("//input[contains(@id,'_NewPassword')]")
                        .FillAsync(this.TestSettings.TestUserPassword);
                    await page.Locator("//input[contains(@id,'_ConfirmNewPassword')]")
                        .FillAsync(this.TestSettings.TestUserPassword);

                    // Submit
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@type,'submit')]").Last);

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Password has been successfully changed."),
                        "Changing Password Failed");
                });
    }

    /// <summary>
    /// A confirm password that differs is refused (the new password typed first was saved).
    /// </summary>
    [Test]
    public async Task DifferentConfirmPasswordIsRefusedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/ChangePassword");

                    await page.Locator("#Input_Password").FillAsync(this.TestSettings.TestUserPassword);
                    await page.Locator("#Input_NewPassword").FillAsync($"{this.TestSettings.TestUserPassword}New");
                    await page.Locator("#Input_ConfirmNewPassword").FillAsync($"{this.TestSettings.TestUserPassword}Typo");

                    Assert.That(
                        await page.Locator("#Input_ConfirmNewPassword").EvaluateAsync<bool>("e => e.validity.valid"),
                        Is.False,
                        "The browser sends the form with different passwords");

                    // without the browser check the server refuses it too
                    await page.RunAndWaitForNextPageAsync(
                        () => page.Locator("#Input_ConfirmNewPassword").EvaluateAsync("e => e.form.submit()"));

                    Assert.That(
                        await page.ContentAsync(),
                        Does.Contain("Password and Confirm Password do not match"),
                        "No message");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                        Is.True,
                        "The password was changed");
                });
    }
}