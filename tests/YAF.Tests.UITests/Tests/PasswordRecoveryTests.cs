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

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The forgotten password: the reset e-mail and the reset page, a not verified account gets the verification again.
/// </summary>
[TestFixture]
public class PasswordRecoveryTests : ForumTestBase
{
    [Test]
    [Description("An unknown user gets no mail, the user gets the reset link and sets a new password (a wrong e-mail or code is refused)")]
    public async Task Reset_Password_Test()
    {
        var userName = $"Fgt{DateTime.UtcNow:HHmmssfff}";
        var newPassword = $"{TestConfig.TestUserPassword}N2";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);
        await this.LogoutAsync();

        // unknown
        await this.RequestResetAsync($"Unknown{userName}");
        this.Page.Url.Should().Contain("ForgotPassword");

        var started = DateTime.UtcNow;

        // by e-mail
        await this.RequestResetAsync(EmailOf(userName));
        this.Page.Url.Should().NotContain("ForgotPassword");

        var link = (await TestMail.WaitForAsync(EmailOf(userName), started)).GetLink("ResetPassword");

        // a wrong e-mail and a wrong code
        await this.ResetAsync(link, EmailOf($"Unknown{userName}"), newPassword);
        this.Page.Url.Should().Contain("ResetPassword");

        await this.ResetAsync(link.Replace("code=", "code=x"), EmailOf(userName), newPassword);
        this.Page.Url.Should().Contain("ResetPassword");

        // the new password: the user is logged in
        await this.ResetAsync(link, EmailOf(userName), newPassword);
        (await this.IsLoggedInAsAsync(userName)).Should().BeTrue();

        await this.LoginAsync(userName, TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync(userName)).Should().BeFalse("the old password is changed");

        await this.LoginAsync(userName, newPassword);
        (await this.IsLoggedInAsAsync(userName)).Should().BeTrue();
    }

    [Test]
    [Description("A not verified account gets the verification e-mail again instead of the reset e-mail")]
    public async Task Not_Verified_Account_Test()
    {
        var userName = $"Fgn{DateTime.UtcNow:HHmmssfff}";

        await this.LogoutAsync();
        await this.GotoAsync("Account/Register");

        await this.Control("UserName").FillAsync(userName);
        await this.Control("Email").FillAsync(EmailOf(userName));
        await this.Control("Password").FillAsync(TestConfig.TestUserPassword);
        await this.Control("ConfirmPassword").FillAsync(TestConfig.TestUserPassword);
        await this.ClickAndWaitAsync(this.Control("CreateUser"));

        var started = DateTime.UtcNow;

        await this.RequestResetAsync(userName);

        var mail = await TestMail.WaitForAsync(EmailOf(userName), started);
        mail.GetLink("Approve").Should().NotBeNullOrEmpty();
    }

    private async Task RequestResetAsync(string userNameOrEmail)
    {
        await this.GotoAsync("Account/ForgotPassword");

        await this.Control("UserName").FillAsync(userNameOrEmail);
        await this.ClickAndWaitAsync(this.Control("Forgot"));
    }

    private async Task ResetAsync(string link, string email, string password)
    {
        await this.GotoAsync(link);

        await this.Control("Email").FillAsync(email);
        await this.Control("Password").FillAsync(password);
        await this.Control("ConfirmPassword").FillAsync(password);
        await this.ClickAndWaitAsync(this.Control("Forgot"));
    }
}
