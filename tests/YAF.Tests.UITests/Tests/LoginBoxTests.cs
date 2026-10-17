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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.UITests.Tests;

using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The login dialog of the forum pages (the buttons post back in an update panel).
/// </summary>
[TestFixture]
public class LoginBoxTests : ForumTestBase
{
    [Test]
    [Description("A wrong password is refused in the dialog, the right one logs in")]
    public async Task Login_With_Dialog_Test()
    {
        await this.LogoutAsync();
        await this.GotoAsync("Topics/1-Test Forum");

        await this.OpenDialogAsync();
        await this.Page.Locator("#LoginBox [id$='_UserName']").FillAsync(TestConfig.AdminUserName);
        await this.Page.Locator("#LoginBox [id$='_Password']").FillAsync("wrong password");

        // a partial post back (update panel)
        var response = await this.Page.RunAndWaitForResponseAsync(
            () => this.Page.Locator("#LoginBox [id$='_LoginButton']").ClickAsync(),
            r => r.Request.Method == "POST");
        response.Status.Should().Be(200);
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await this.GotoAsync("Topics/1-Test Forum");
        (await this.IsLoggedInAsAsync(TestConfig.AdminUserName)).Should().BeFalse();

        await this.GotoAsync("Topics/1-Test Forum");
        await this.OpenDialogAsync();
        await this.Page.Locator("#LoginBox [id$='_UserName']").FillAsync(TestConfig.AdminUserName);
        await this.Page.Locator("#LoginBox [id$='_Password']").FillAsync(TestConfig.AdminPassword);

        // the login redirects after the partial post back
        response = await this.Page.RunAndWaitForResponseAsync(
            () => this.Page.Locator("#LoginBox [id$='_LoginButton']").ClickAsync(),
            r => r.Request.Method == "POST");
        response.Status.Should().Be(200);
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await this.GotoAsync(string.Empty);
        (await this.IsLoggedInAsAsync(TestConfig.AdminUserName)).Should().BeTrue();
    }

    [Test]
    [Description("An unknown user is refused, a not verified user gets the verification again, 10 wrong passwords lock the account, the links open the pages")]
    public async Task Login_Checks_Test()
    {
        var unverified = $"Lbu{DateTime.UtcNow:HHmmssfff}";
        var locked = $"Lbl{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(locked, TestConfig.TestUserPassword);

        // a member that did not verify the e-mail address
        await this.LogoutAsync();
        await this.GotoAsync("Account/Register");
        await this.Control("UserName").FillAsync(unverified);
        await this.Control("Email").FillAsync(EmailOf(unverified));
        await this.Control("Password").FillAsync(TestConfig.TestUserPassword);
        await this.Control("ConfirmPassword").FillAsync(TestConfig.TestUserPassword);
        await this.ClickAndWaitAsync(this.Control("CreateUser"));

        // unknown
        await this.LoginWithDialogAsync($"Unknown{unverified}", TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync($"Unknown{unverified}")).Should().BeFalse();

        // not verified: the verification is sent again
        await this.LoginWithDialogAsync(unverified, TestConfig.TestUserPassword);
        var resend = this.Page.Locator("#LoginBox [id$='_ResendConfirm']");
        await resend.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        var started = DateTime.UtcNow;
        await this.Page.RunAndWaitForResponseAsync(() => resend.ClickAsync(), r => r.Request.Method == "POST");
        (await TestMail.WaitForAsync(EmailOf(unverified), started)).Should().NotBeNull();

        // ten wrong passwords lock the account
        for (var i = 0; i < 10; i++)
        {
            await this.LoginWithDialogAsync(locked, "wrong password");
        }

        await this.LoginWithDialogAsync(locked, TestConfig.TestUserPassword);
        await this.GotoAsync(string.Empty);
        (await this.IsLoggedInAsAsync(locked)).Should().BeFalse("the account is locked");

        // the links of the dialog
        foreach (var (link, page) in new[] { ("PasswordRecovery", "ForgotPassword"), ("RegisterLink", "Register") })
        {
            await this.GotoAsync("Topics/1-Test Forum");
            await this.OpenDialogAsync();
            await this.ClickAndWaitAsync(this.Page.Locator($"#LoginBox [id$='_{link}']"));
            this.Page.Url.Should().ContainAny(page, "RulesAndPrivacy");
        }
    }

    private async Task LoginWithDialogAsync(string userName, string password)
    {
        await this.GotoAsync("Topics/1-Test Forum");
        await this.OpenDialogAsync();
        await this.Page.Locator("#LoginBox [id$='_UserName']").FillAsync(userName);
        await this.Page.Locator("#LoginBox [id$='_Password']").FillAsync(password);

        await this.Page.RunAndWaitForResponseAsync(
            () => this.Page.Locator("#LoginBox [id$='_LoginButton']").ClickAsync(),
            r => r.Request.Method == "POST");
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    private async Task OpenDialogAsync()
    {
        await this.Page.Locator(".LoginLink").First.ClickAsync();
        await this.Page.Locator("#LoginBox").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }
}
