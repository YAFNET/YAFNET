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
/// The checks of the registration and the custom profile fields of the registration.
/// </summary>
[TestFixture]
public class RegisterTests : ForumTestBase
{
    [Test]
    [Description("A user name or e-mail address that is already registered (in any case) is refused")]
    public async Task Already_Registered_Test()
    {
        var userName = $"Dup{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);

        // the same name in upper case
        await this.FillRegistrationAsync(userName.ToUpperInvariant(), $"other.{EmailOf(userName)}");
        await this.ClickAndWaitAsync(this.Control("CreateUser"));
        (await this.Page.ContentAsync()).Should().Contain("is already taken");

        // the same e-mail address
        await this.FillRegistrationAsync($"{userName}X", EmailOf(userName).ToUpperInvariant());
        await this.ClickAndWaitAsync(this.Control("CreateUser"));
        (await this.Page.ContentAsync()).Should().Contain("is already taken");

        await this.LoginAsync($"{userName}X", TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync($"{userName}X")).Should().BeFalse();
    }

    [Test]
    [Description("A user name with a semicolon or shorter than the minimum length is refused")]
    public async Task Invalid_Names_Test()
    {
        var userName = $"Bad;{DateTime.UtcNow:HHmmssfff}";

        await this.FillRegistrationAsync(userName, $"bad{DateTime.UtcNow:HHmmssfff}@localhost.com");
        await this.ClickAndWaitAsync(this.Control("CreateUser"));
        this.Page.Url.Should().Contain("Account/Register");
        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        await this.FillRegistrationAsync("ab", $"short{DateTime.UtcNow:HHmmssfff}@localhost.com");
        await this.ClickAndWaitAsync(this.Control("CreateUser"));
        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        await this.LoginAsync("ab", TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync("ab")).Should().BeFalse();
    }

    [Test]
    [Description("A custom profile field of the registration is required and saved with the new user")]
    public async Task Custom_Profile_Field_Test()
    {
        var field = $"Hometown {Guid.NewGuid():N}".Substring(0, 17);
        var userName = $"Fld{DateTime.UtcNow:HHmmssfff}";

        await this.LoginAsAdminAsync();
        await this.GotoAsync("Admin/ProfileDefinitions");
        await this.ClickAndWaitAsync(
            this.Page.Locator("a", new PageLocatorOptions { HasTextString = "Add New Definition" })
                .Filter(new LocatorFilterOptions { Visible = true }).First);
        await this.Page.Locator("[id$='_EditDialog_Save']")
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await this.Control("EditDialog_Name").FillAsync(field);
        await this.Control("EditDialog_Length").FillAsync("30");
        await this.Control("EditDialog_ShowOnRegisterPage").CheckAsync();
        await this.Control("EditDialog_Required").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        try
        {
            // the field is required (the browser checks it first)
            await this.FillRegistrationAsync(userName, EmailOf(userName));
            await this.Page.GetByLabel(field, new PageGetByLabelOptions { Exact = true })
                .EvaluateAsync("e => e.removeAttribute('required')");
            await this.ClickAndWaitAsync(this.Control("CreateUser"));
            (await this.Page.ContentAsync()).Should().Contain($"{field} is required");

            await this.Page.GetByLabel(field, new PageGetByLabelOptions { Exact = true }).FillAsync("Springfield");
            await this.Control("Password").FillAsync(TestConfig.TestUserPassword);
            await this.Control("ConfirmPassword").FillAsync(TestConfig.TestUserPassword);

            var started = DateTime.UtcNow;
            await this.ClickAndWaitAsync(this.Control("CreateUser"));

            var mail = await TestMail.WaitForAsync(EmailOf(userName), started);
            await this.GotoAsync(mail.GetLink("Approve"));

            await this.LoginAsync(userName, TestConfig.TestUserPassword);
            await this.GotoAsync("Profile/EditProfile");
            (await this.Page.GetByLabel(field, new PageGetByLabelOptions { Exact = true }).InputValueAsync()).Should().Be("Springfield");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.GotoAsync("Admin/ProfileDefinitions");

            var row = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = field });

            if (await row.CountAsync() > 0)
            {
                await row.Locator("[id$='_ThemeButtonDelete']").ClickAsync();
                await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
            }
        }
    }

    private async Task FillRegistrationAsync(string userName, string email)
    {
        await this.LogoutAsync();
        await this.GotoAsync("Account/Register");

        await this.Control("UserName").FillAsync(userName);
        await this.Control("Email").FillAsync(email);
        await this.Control("Password").FillAsync(TestConfig.TestUserPassword);
        await this.Control("ConfirmPassword").FillAsync(TestConfig.TestUserPassword);
    }
}
