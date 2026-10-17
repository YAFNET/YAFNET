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
/// The admin start page: the not approved users are approved, sent the verification again or deleted.
/// </summary>
[TestFixture]
public class AdminDashboardTests : ForumTestBase
{
    [Test]
    [Description("A not approved user is approved (and can log in), another one gets the verification again, a third one is deleted")]
    public async Task Unverified_Users_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var approved = $"Apr{id}";
        var resent = $"Res{id}";
        var deleted = $"Del{id}";

        foreach (var name in new[] { approved, resent, deleted })
        {
            await this.RegisterWithoutConfirmAsync(name);
        }

        await this.LoginAsAdminAsync();

        // approve
        await this.UserActionAsync(approved, "Approve user");

        await this.LoginAsync(approved, TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync(approved)).Should().BeTrue("the user is approved");

        // resend the verification
        await this.LoginAsAdminAsync();
        var started = DateTime.UtcNow;
        await this.UserActionAsync(resent, "Resend Email Verification");
        var mail = await TestMail.WaitForAsync(EmailOf(resent), started);
        mail.Should().NotBeNull();

        // delete
        await this.UserActionAsync(deleted, "Delete user", confirm: true);

        await this.GotoAsync("Admin/Admin");
        var list = await this.Page.InnerTextAsync("body");
        list.Should().NotContain(deleted).And.NotContain(approved).And.Contain(resent);
    }

    private async Task RegisterWithoutConfirmAsync(string userName)
    {
        await this.LogoutAsync();
        await this.GotoAsync("Account/Register");

        await this.Control("UserName").FillAsync(userName);
        await this.Control("Email").FillAsync(EmailOf(userName));
        await this.Control("Password").FillAsync(TestConfig.TestUserPassword);
        await this.Control("ConfirmPassword").FillAsync(TestConfig.TestUserPassword);

        await this.ClickAndWaitAsync(this.Control("CreateUser"));
    }

    /// <summary>
    /// Clicks an action of the manage menu of a not approved user of the admin start page.
    /// </summary>
    private async Task UserActionAsync(string userName, string action, bool confirm = false)
    {
        await this.GotoAsync("Admin/Admin");

        // all not approved users on one page
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSizeUnverified").SelectOptionAsync("50", new LocatorSelectOptionOptions { Force = true }));

        var row = this.Page.Locator("li", new PageLocatorOptions { HasTextString = userName }).Last;

        await row.Locator("[id$='_Manage']").ClickAsync();

        var item = row.Locator(".dropdown-menu a", new LocatorLocatorOptions { HasTextString = action });

        if (confirm)
        {
            await item.ClickAsync();
            await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        }
        else
        {
            await this.ClickAndWaitAsync(item);
        }
    }
}
