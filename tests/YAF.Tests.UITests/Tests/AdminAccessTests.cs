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
/// The access of the admins that are not host admins (page access), the moderators of a forum, the users
/// registered by the admin and the database maintenance.
/// </summary>
[TestFixture]
public class AdminAccessTests : ForumTestBase
{
    [Test]
    [Description("An admin (not host admin) can use the granted admin pages only, never the host admin pages")]
    public async Task Page_Access_Test()
    {
        var userName = $"Pga{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        var userId = await this.OpenAdminEditUserAsync(userName);
        await this.AddToRoleAsync(userId, "Administrators");

        // the admin is listed
        await this.GotoAsync("Admin/PageAccessList");
        await this.ClickAndWaitAsync(
            this.Page.Locator("li", new PageLocatorOptions { HasTextString = userName }).Locator("[id$='_ThemeButtonEdit']"));
        this.Page.Url.Should().Contain("PageAccessEdit");

        // nothing granted
        await this.ClickAndWaitAsync(this.Control("RevokeAll"));

        await this.LoginAsync(userName, TestConfig.TestUserPassword);
        (await this.CanOpenAsync("Admin/BannedIps")).Should().BeFalse();

        // one page
        await this.LoginAsAdminAsync();
        await this.GotoAsync($"Admin/PageAccessEdit?u={userId}");
        await this.Page.GetByText("Admin_BannedIps", new PageGetByTextOptions { Exact = true }).CheckAsync();
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.LoginAsync(userName, TestConfig.TestUserPassword);
        (await this.CanOpenAsync("Admin/BannedIps")).Should().BeTrue();
        (await this.CanOpenAsync("Admin/SpamWords")).Should().BeFalse();

        // all pages, but not the host admin pages
        await this.LoginAsAdminAsync();
        await this.GotoAsync($"Admin/PageAccessEdit?u={userId}");
        (await this.Page.GetByText("Admin_BannedIps", new PageGetByTextOptions { Exact = true }).IsCheckedAsync()).Should()
            .BeTrue();
        await this.ClickAndWaitAsync(this.Control("GrantAll"));

        await this.LoginAsync(userName, TestConfig.TestUserPassword);
        (await this.CanOpenAsync("Admin/SpamWords")).Should().BeTrue();
        (await this.CanOpenAsync("Admin/HostSettings")).Should().BeFalse();
        (await this.CanOpenAsync("Admin/PageAccessList")).Should().BeFalse();

        // cancel
        await this.LoginAsAdminAsync();
        await this.GotoAsync($"Admin/PageAccessEdit?u={userId}");
        await this.ClickAndWaitAsync(this.Control("Cancel"));
        this.Page.Url.Should().Contain("PageAccessList");
    }

    [Test]
    [Description("The admin adds a moderator to a forum with the user select, changes the access mask and removes the moderator")]
    public async Task Forum_Moderator_Test()
    {
        var userName = $"Fmo{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        await this.GotoAsync("Moderate/Forums?f=1");
        await this.ClickAndWaitAsync(this.Control("AddUser"));
        await this.Control("ModForumUserDialog_Update").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // the user select (the access masks are a select of the same kind)
        var userSelect = this.Page.Locator("#ModForumUserDialog .choices:has(#UserSelect)");
        await userSelect.ClickAsync();
        await userSelect.Locator(".choices__input--cloned").FillAsync(userName);
        await userSelect.Locator(".choices__item--choice", new LocatorLocatorOptions { HasTextString = userName }).ClickAsync();

        await this.Control("ModForumUserDialog_AccessMaskID").SelectOptionAsync(
            new SelectOptionValue { Label = "Moderator Access" },
            new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("ModForumUserDialog_Update"));

        var row = this.Page.Locator("li", new PageLocatorOptions { HasTextString = userName });
        (await row.InnerTextAsync()).Should().Contain("Moderator Access");

        // the moderator has no dialog to add moderators
        await this.LoginAsync(userName, TestConfig.TestUserPassword);
        await this.GotoAsync("Moderate/Forums?f=1");
        (await this.Control("ModForumUserDialog_Update").CountAsync()).Should().Be(0);

        // edit: the user can't be changed
        await this.LoginAsAdminAsync();
        await this.GotoAsync("Moderate/Forums?f=1");
        await this.ClickAndWaitAsync(row.Locator("[id$='_ThemeButtonEdit']"));
        await this.Control("ModForumUserDialog_Update").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        (await this.Control("ModForumUserDialog_UserName").InputValueAsync()).Should().Be(userName);

        await this.Control("ModForumUserDialog_AccessMaskID").SelectOptionAsync(
            new SelectOptionValue { Label = "Member Access" },
            new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("ModForumUserDialog_Update"));

        (await row.InnerTextAsync()).Should().Contain("Member Access");

        // remove
        await row.Locator("[id$='_ThemeButtonRemove']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await row.CountAsync()).Should().Be(0);

        this.BrowserErrors.Should().BeEmpty();
    }

    [Test]
    [Description("The admin registers a user: an invalid e-mail and an existing name are refused, the new user gets the verification e-mail")]
    public async Task Register_User_Test()
    {
        var userName = $"Reg{DateTime.UtcNow:HHmmssfff}";

        await this.LoginAsAdminAsync();

        // an invalid e-mail (the browser check of the field is removed)
        await this.FillRegisterAsync(userName, "not-an-email");
        await this.ClickAndWaitAsync(this.Control("ForumRegister"));
        this.Page.Url.Should().Contain("RegisterUser");

        // an existing name
        await this.FillRegisterAsync(TestConfig.AdminUserName, EmailOf(userName));
        await this.ClickAndWaitAsync(this.Control("ForumRegister"));
        this.Page.Url.Should().Contain("RegisterUser");

        var started = DateTime.UtcNow;

        await this.FillRegisterAsync(userName, EmailOf(userName));
        await this.ClickAndWaitAsync(this.Control("ForumRegister"));
        this.Page.Url.Should().Contain("Admin/Users");

        var mail = await TestMail.WaitForAsync(EmailOf(userName), started);
        mail.Should().NotBeNull();

        // cancel
        await this.GotoAsync("Admin/RegisterUser");
        await this.ClickAndWaitAsync(this.Control("cancel"));
        this.Page.Url.Should().Contain("Admin/Users");
    }

    [Test]
    [Description("The database maintenance: the statistics, the recovery mode, the index rebuild and the shrink")]
    public async Task ReIndex_Test()
    {
        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/ReIndex");
        await this.ClickAndWaitAsync(this.Control("GetStats"));
        (await this.Control("txtIndexStatistics").InputValueAsync()).Should().NotBeNullOrEmpty();

        // simple
        await this.Control("RadioButtonList1").Locator("input").Nth(1).CheckAsync();
        await this.Control("RecoveryMode").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        foreach (var button in new[] { "Reindex", "Shrink" })
        {
            await this.Control(button).ClickAsync();
            await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        }

        (await this.Page.ContentAsync()).Should().Contain("toast-body");
    }

    /// <summary>
    /// Adds the user to the role with the admin edit user page.
    /// </summary>
    private async Task AddToRoleAsync(int userId, string role)
    {
        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a[href='#View2']").ClickAsync();

        await this.Page.Locator(".col-md-4", new PageLocatorOptions { HasTextString = role }).Locator("input[type='checkbox']")
            .CheckAsync();
        await this.ClickAndWaitAsync(this.Control("GroupEditControl_Save"));
    }

    /// <summary>
    /// Opens the page: false when the info page (no access) is shown instead.
    /// </summary>
    private async Task<bool> CanOpenAsync(string url)
    {
        await this.GotoAsync(url);

        return !this.Page.Url.Contains("/Info");
    }

    private async Task FillRegisterAsync(string userName, string email)
    {
        await this.GotoAsync("Admin/RegisterUser");

        await this.Control("UserName").FillAsync(userName);
        await this.Control("Email").EvaluateAsync("e => e.type = 'text'");
        await this.Control("Email").FillAsync(email);
        await this.Control("Password").FillAsync(TestConfig.TestUserPassword);
        await this.Control("Password2").FillAsync(TestConfig.TestUserPassword);
    }
}
