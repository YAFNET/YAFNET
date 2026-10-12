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

using System.Linq;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Authentication;

/// <summary>
/// The login and account recovery security tests.
/// </summary>
public class AccountSecurityTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The login only tells that an account is not approved yet when the password is right,
    /// otherwise anyone could find out about the account.
    /// </summary>
    [Test]
    public async Task UnapprovedAccountStateNeedsPasswordTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = $"TempUser{Random.Shared.Next()}";

                    // register, but don't open the verification link
                    Assert.That(
                        await page.SubmitRegistrationAsync(
                            this.TestSettings,
                            userName,
                            $"{userName.ToLower()}@test.com",
                            this.TestSettings.TestUserPassword),
                        Is.True,
                        "Registrations are disabled");

                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, userName, "WrongPassword123!"), Is.False,
                        "Login with a wrong password");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("The user name or password entered is incorrect."),
                        "Wrong password message is missing");

                    Assert.That(
                        pageSource, Does.Not.Contain("Your account is not approved yet"),
                        "The account state is shown without the password");

                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                        Is.False,
                        "Login of a not approved account");

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Your account is not approved yet"),
                        "The account state is not shown with the right password");

                    // the verification mail can be sent again, and its link approves the account
                    this.Fixture.SmtpServer.ClearReceivedEmail();

                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='ResendConfirm']"));

                    var confirmLink = await this.Fixture.SmtpServer.WaitForEmailLinkAsync($"{userName.ToLower()}@test.com");

                    Assert.That(confirmLink, Is.Not.Null, "The verification mail was not sent again");

                    await page.GotoAsync(confirmLink);
                    await page.GotoAsync(this.TestSettings.TestForumUrl);

                    Assert.That(
                        await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(), Is.True,
                        "Verifying the account with the resent link failed");
                });
    }

    /// <summary>
    /// A password reset link can only be used once.
    /// </summary>
    [Test]
    public async Task PasswordResetLinkWorksOnlyOnceTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);
                    var email = $"{userName.ToLower()}@test.com";
                    var newPassword = $"{this.TestSettings.TestUserPassword}New";

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    this.Fixture.SmtpServer.ClearReceivedEmail();

                    // Request the reset link
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/ForgotPassword");

                    await page.Locator("#Input_UserName").FillAsync(userName);
                    await page.SubmitAndWaitAsync(page.Locator("form:has(#Input_UserName) button[type=submit]"));

                    var resetLink = await this.Fixture.SmtpServer.WaitForEmailLinkAsync(email);

                    Assert.That(resetLink, Is.Not.Null, "No reset mail was sent");

                    // the first use sets the new password
                    await ResetPasswordAsync(page, resetLink, email, newPassword);

                    Assert.That(
                        await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(), Is.True,
                        "Resetting the password failed");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    // the second use is refused
                    await ResetPasswordAsync(page, resetLink, email, $"{newPassword}Again");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Invalid token."), "The reset link worked twice");

                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, userName, newPassword), Is.True,
                        "Login with the new password failed");
                });

        static async Task ResetPasswordAsync(IPage page, string resetLink, string email, string password)
        {
            await page.GotoAsync(resetLink);

            await page.Locator("#Input_Email").FillAsync(email);
            await page.Locator("#Input_Password").FillAsync(password);
            await page.Locator("#Input_ConfirmPassword").FillAsync(password);

            await page.SubmitAndWaitAsync(page.Locator("form:has(#Input_Email) button[type=submit]"));
        }
    }

    /// <summary>
    /// The password reset page can be used by anybody: an account gets only a few emails per hour,
    /// further requests show the same message without sending an email.
    /// </summary>
    [Test]
    public async Task PasswordResetEmailsAreLimitedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);
                    var email = $"{userName.ToLower()}@test.com";

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    this.Fixture.SmtpServer.ClearReceivedEmail();

                    for (var i = 0; i < 4; i++)
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/ForgotPassword");

                        await page.Locator("#Input_UserName").FillAsync(userName);
                        await page.SubmitAndWaitAsync(page.Locator("form:has(#Input_UserName) button[type=submit]"));
                    }

                    int CountMails() =>
                        this.Fixture.SmtpServer.ReceivedEmail.Count(
                            m => m.ToAddresses.Any(a => string.Equals(a.Address, email, StringComparison.OrdinalIgnoreCase)));

                    // the mails are sent while the requests are handled, give the smtp server a moment
                    for (var wait = 0; wait < 20 && CountMails() < 3; wait++)
                    {
                        await Task.Delay(500);
                    }

                    await Task.Delay(2000);

                    Assert.That(CountMails(), Is.EqualTo(3), "The number of reset emails is not limited");
                });
    }

    /// <summary>
    /// The password reset page gives the same answer for unknown and existing accounts
    /// (nobody can find out which user names or email addresses are registered).
    /// </summary>
    [Test]
    public async Task PasswordResetDoesNotRevealAccountsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    async Task<string> RequestResetAsync(string name)
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/ForgotPassword");

                        await page.Locator("#Input_UserName").FillAsync(name);
                        await page.SubmitAndWaitAsync(page.Locator("form:has(#Input_UserName) button[type=submit]"));

                        return await page.ContentAsync();
                    }

                    const string answer = "If an account with this user name or email address exists";

                    Assert.That(await RequestResetAsync($"NoSuchUser{Random.Shared.Next()}"), Does.Contain(answer), "Unknown user name");
                    Assert.That(await RequestResetAsync($"nobody{Random.Shared.Next()}@test.com"), Does.Contain(answer), "Unknown email address");
                    Assert.That(await RequestResetAsync(userName), Does.Contain(answer), "Existing user name");
                });
    }

    /// <summary>
    /// After the installation the install steps can't be posted anymore (they put the forum into the
    /// install mode, create admins and send e-mails).
    /// </summary>
    [Test]
    public async Task InstallPagesAreClosedAfterInstallTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    await page.GotoAsync(this.TestSettings.TestForumUrl);

                    if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                    {
                        await page.LogOutUserAsync();
                    }

                    try
                    {
                        // a guest posts the first install step with the token of any page
                        Assert.That(await page.Locator("input[name='__RequestVerificationToken']").CountAsync(), Is.GreaterThan(0), "No token on the page");

                        await PostAsync(page, "/Install/Install/InstallInitDatabase");

                        await page.GotoAsync(this.TestSettings.TestForumUrl);

                        Assert.That(page.Url, Does.Not.Contain("/Install"), "The forum was put into the install mode");
                    }
                    finally
                    {
                        // leave the install mode again (the old code)
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Install/Install");
                        await PostAsync(page, "/Install/Install/Complete");
                    }
                });

        static Task PostAsync(IPage page, string action)
        {
            return page.RunAndWaitForNextPageAsync(
                () => page.EvaluateAsync(
                    """
                    action => {
                        const form = document.createElement('form');
                        form.method = 'post';
                        form.action = action;
                        const token = document.querySelector("input[name='__RequestVerificationToken']");
                        if (token) {
                            form.appendChild(token.cloneNode());
                        }
                        document.body.appendChild(form);
                        form.submit();
                    }
                    """,
                    action));
        }
    }

    /// <summary>
    /// The admin sends the verification mail of a not approved account again (admin dashboard), the link of
    /// the new mail approves the account (the link contained the stored key, which is a hash now).
    /// </summary>
    [Test]
    public async Task AdminResendsVerificationMailTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = $"TempUser{Random.Shared.Next()}";
                    var email = $"{userName.ToLower()}@test.com";

                    // register, but don't open the verification link
                    Assert.That(
                        await page.SubmitRegistrationAsync(
                            this.TestSettings,
                            userName,
                            email,
                            this.TestSettings.TestUserPassword),
                        Is.True,
                        "Registrations are disabled");

                    Assert.That(
                        await this.Fixture.SmtpServer.WaitForEmailLinkAsync(email),
                        Is.Not.Null,
                        "The verification mail was not sent");

                    // nobody is logged in after the registration
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Admin");

                    var resendButton = page.Locator("li.list-group-item", new PageLocatorOptions { HasText = email })
                        .Locator("button[formaction*='ResendEmail']").First;

                    Assert.That(await resendButton.CountAsync(), Is.EqualTo(1), "The account is not listed as not approved");

                    this.Fixture.SmtpServer.ClearReceivedEmail();

                    await page.RunAndWaitForNextPageAsync(() => resendButton.EvaluateAsync("b => b.click()"));

                    var confirmLink = await this.Fixture.SmtpServer.WaitForEmailLinkAsync(email);

                    Assert.That(confirmLink, Is.Not.Null, "The verification mail was not sent again");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    // the link approves the account and logs the user in
                    await page.GotoAsync(confirmLink);
                    await page.GotoAsync(this.TestSettings.TestForumUrl);

                    Assert.That(
                        await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync(), Is.True,
                        "Verifying the account with the resent link failed");
                });
    }
}
