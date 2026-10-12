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

using Google.Authenticator;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.UserSettings;

/// <summary>
/// The two-factor authentication tests.
/// </summary>
public class TwoFactorTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Enables 2FA (needs the password and a valid code), logs in with it,
    /// and disables it again (needs the code of the stored key).
    /// </summary>
    [Test]
    public async Task EnableLoginAndDisableTwoFactorTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // a new user, a failed test can not lock out the standard test users
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Enable Two Factor Authentication"), "2FA is not available");

                    // the key of the setup (the QR code), it is kept when the form is shown again
                    var twoFactorKey = await page.Locator("form:has(#InputCode) input[name=TwoFactorKey]")
                        .GetAttributeAsync("value");

                    Assert.That(twoFactorKey, Is.Not.Null.And.Not.Empty, "2FA setup key is missing");

                    var authenticator = new TwoFactorAuthenticator();

                    // A wrong password is refused, even with the right code
                    await page.Locator("#InputCode").FillAsync(authenticator.GetCurrentPIN(twoFactorKey));
                    await page.Locator("#TwoFactorPassword").FillAsync("WrongPassword123!");
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='EnableTwoFa']"));

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("The password you entered is not correct."),
                        "Enabling 2FA with a wrong password was not refused");

                    Assert.That(
                        await page.Locator("form:has(#InputCode) input[name=TwoFactorKey]").GetAttributeAsync("value"),
                        Is.EqualTo(twoFactorKey),
                        "The setup key changed, the authenticator would not match anymore");

                    // the right password and code
                    await page.Locator("#InputCode").FillAsync(authenticator.GetCurrentPIN(twoFactorKey));
                    await page.Locator("#TwoFactorPassword").FillAsync(this.TestSettings.TestUserPassword);
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='EnableTwoFa']"));

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Disable Two Factor Authentication"), "Enabling 2FA failed");

                    // the stored key is never sent to the browser
                    Assert.That(
                        pageSource, Does.Not.Contain(twoFactorKey.Split('+')[0]),
                        "The 2FA key is shown in the page");

                    // Login needs the code now
                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                        Is.False,
                        "Login without the 2FA code");

                    // the code is asked
                    await page.WaitForURLAsync("**/Account/Authorize**");

                    await page.Locator("#ValidationCode").FillAsync("12345");
                    await page.SubmitAndWaitAsync(page.Locator("form:has(#ValidationCode) button[type=submit]"));

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You have entered the wrong 2FA Code!"), "Wrong code was accepted");

                    await page.Locator("#ValidationCode").FillAsync(authenticator.GetCurrentPIN(twoFactorKey));
                    await page.SubmitAndWaitAsync(page.Locator("form:has(#ValidationCode) button[type=submit]"));

                    Assert.That(
                        await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(), Is.True,
                        "Login with the 2FA code failed");

                    // Disable it again, a wrong code is refused
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    await page.Locator("#InputCode").FillAsync("12345");
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='DisableTwoFa']"));

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You have entered the wrong 2FA Code!"), "Wrong code disabled 2FA");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    await page.Locator("#InputCode").FillAsync(authenticator.GetCurrentPIN(twoFactorKey));
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='DisableTwoFa']"));

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Enable Two Factor Authentication"), "Disabling 2FA failed");
                });
    }

    /// <summary>
    /// The login with the two factor code keeps the "remember me" choice of the login
    /// (it always kept the login for a month).
    /// </summary>
    [Test]
    public async Task TwoFactorLoginWithoutRememberMeTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    var twoFactorKey = await page.Locator("form:has(#InputCode) input[name=TwoFactorKey]")
                        .GetAttributeAsync("value");

                    var authenticator = new TwoFactorAuthenticator();

                    await page.Locator("#InputCode").FillAsync(authenticator.GetCurrentPIN(twoFactorKey));
                    await page.Locator("#TwoFactorPassword").FillAsync(this.TestSettings.TestUserPassword);
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='EnableTwoFa']"));

                    Assert.That(await page.ContentAsync(), Does.Contain("Disable Two Factor Authentication"), "Enabling 2FA failed");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    // the login without "remember me"
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/login");

                    await page.Locator("//input[contains(@id, '_UserName')]").FillAsync(userName);
                    await page.Locator("//input[contains(@id, '_Password')]").FillAsync(this.TestSettings.TestUserPassword);
                    await page.Locator("#Input_RememberMe").UncheckAsync();

                    await page.SubmitAndWaitAsync(page.Locator("#Login"));

                    await page.WaitForURLAsync("**/Account/Authorize**");

                    await page.Locator("#ValidationCode").FillAsync(authenticator.GetCurrentPIN(twoFactorKey));
                    await page.SubmitAndWaitAsync(page.Locator("form:has(#ValidationCode) button[type=submit]"));

                    Assert.That(
                        await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(), Is.True,
                        "Login with the 2FA code failed");

                    var authCookie = System.Linq.Enumerable.FirstOrDefault(await page.Context.CookiesAsync(), 
                        c => c.Name.StartsWith(".AspNetCore.Identity.Application"));

                    Assert.That(authCookie, Is.Not.Null, "The login cookie is missing");
                    Assert.That(authCookie.Expires, Is.EqualTo(-1), "The login is kept although \"remember me\" was not checked");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
                });
    }

    /// <summary>
    /// A password reset (the link of the e-mail) does not log in a user with two factor authentication
    /// without the code, the e-mail alone must not be enough.
    /// </summary>
    [Test]
    public async Task PasswordResetNeedsTwoFactorCodeTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);
                    var email = $"{userName.ToLower()}@test.com";
                    var newPassword = $"{this.TestSettings.TestUserPassword}New";

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    var twoFactorKey = await page.Locator("form:has(#InputCode) input[name=TwoFactorKey]")
                        .GetAttributeAsync("value");

                    var authenticator = new TwoFactorAuthenticator();

                    await page.Locator("#InputCode").FillAsync(authenticator.GetCurrentPIN(twoFactorKey));
                    await page.Locator("#TwoFactorPassword").FillAsync(this.TestSettings.TestUserPassword);
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='EnableTwoFa']"));

                    Assert.That(await page.ContentAsync(), Does.Contain("Disable Two Factor Authentication"), "Enabling 2FA failed");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    this.Fixture.SmtpServer.ClearReceivedEmail();

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/ForgotPassword");

                    await page.Locator("#Input_UserName").FillAsync(userName);
                    await page.SubmitAndWaitAsync(page.Locator("form:has(#Input_UserName) button[type=submit]"));

                    var resetLink = await this.Fixture.SmtpServer.WaitForEmailLinkAsync(email);

                    Assert.That(resetLink, Is.Not.Null, "No reset mail was sent");

                    await page.GotoAsync(resetLink);

                    await page.Locator("#Input_Email").FillAsync(email);
                    await page.Locator("#Input_Password").FillAsync(newPassword);
                    await page.Locator("#Input_ConfirmPassword").FillAsync(newPassword);

                    await page.SubmitAndWaitAsync(page.Locator("form:has(#Input_Email) button[type=submit]"));

                    Assert.That(
                        await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(), Is.False,
                        "The password reset logged in without the 2FA code");

                    // the code is asked
                    Assert.That(page.Url, Does.Contain("Account/Authorize"), "The 2FA code is not asked");

                    await page.Locator("#ValidationCode").FillAsync(authenticator.GetCurrentPIN(twoFactorKey));
                    await page.SubmitAndWaitAsync(page.Locator("form:has(#ValidationCode) button[type=submit]"));

                    Assert.That(
                        await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(), Is.True,
                        "Login with the 2FA code failed");
                });
    }
}
