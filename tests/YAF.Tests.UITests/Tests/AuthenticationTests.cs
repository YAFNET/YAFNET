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
/// Login, logout and registration.
/// </summary>
[TestFixture]
public class AuthenticationTests : ForumTestBase
{
    [Test]
    [Description("The admin created by the install wizard can log in")]
    public async Task Admin_Login_Test()
    {
        await this.LoginAsAdminAsync();

        (await this.IsLoggedInAsAsync(TestConfig.AdminUserName)).Should().BeTrue();
    }

    [Test]
    [Description("A wrong password is refused")]
    public async Task Wrong_Password_Is_Refused_Test()
    {
        await this.LoginAsync(TestConfig.AdminUserName, "wrong password");

        (await this.IsLoggedInAsAsync(TestConfig.AdminUserName)).Should().BeFalse();
        this.Page.Url.Should().Contain("Account/Login");
    }

    [Test]
    [Description("A new user registers, confirms the e-mail address with the link of the mail and can log in")]
    public async Task Register_And_Confirm_Test()
    {
        var userName = $"Reg{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);

        await this.LoginAsync(userName, TestConfig.TestUserPassword);

        (await this.IsLoggedInAsAsync(userName)).Should().BeTrue();
    }

    [Test]
    [Description("A member logs in with the e-mail address instead of the user name")]
    public async Task Login_With_Email_Test()
    {
        var userName = $"Eml{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);

        await this.LoginAsync(EmailOf(userName).ToUpperInvariant(), TestConfig.TestUserPassword);

        (await this.IsLoggedInAsAsync(userName)).Should().BeTrue();

        // not with a wrong password
        await this.LoginAsync(EmailOf(userName), "wrong password");

        (await this.IsLoggedInAsAsync(userName)).Should().BeFalse();
    }

    [Test]
    [Description("An account that was not confirmed yet can not log in")]
    public async Task Unconfirmed_Account_Can_Not_Login_Test()
    {
        var userName = $"Unc{DateTime.UtcNow:HHmmssfff}";

        await this.LogoutAsync();
        await this.GotoAsync("Account/Register");

        await this.Control("UserName").FillAsync(userName);
        await this.Control("Email").FillAsync(EmailOf(userName));
        await this.Control("Password").FillAsync(TestConfig.TestUserPassword);
        await this.Control("ConfirmPassword").FillAsync(TestConfig.TestUserPassword);

        await this.ClickAndWaitAsync(this.Control("CreateUser"));

        await this.LoginAsync(userName, TestConfig.TestUserPassword);

        (await this.IsLoggedInAsAsync(userName)).Should().BeFalse();
    }
}
