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
/// The medals of the admin: a medal is created, given to a role and removed again.
/// </summary>
[TestFixture]
public class MedalTests : ForumTestBase
{
    [Test]
    [Description("A medal is created and given to a role (twice: the settings are updated), an empty sort order is 0, the role and the medal are removed")]
    public async Task Medal_For_Role_Test()
    {
        var name = $"Medal {Guid.NewGuid():N}".Substring(0, 16);

        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/EditMedal");
        await this.Control("Name").FillAsync(name);
        await this.Control("Category").FillAsync("UI tests");
        await this.Control("Description").FillAsync("A medal of the UI test");
        await this.Control("Message").FillAsync("Thanks for testing");
        await this.Control("MedalImage").SelectOptionAsync(
            new SelectOptionValue { Index = 1 },
            new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("Save"));

        (await this.Page.InnerTextAsync("body")).Should().Contain(name);

        // the edit page of the medal
        var row = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = name });
        await this.ClickAndWaitAsync(row.Locator("[id$='_ThemeButtonEdit']"));

        // the role gets the medal, the sort order is empty
        await this.ClickAndWaitAsync(this.Control("AddGroup").Filter(new LocatorFilterOptions { Visible = true }).First);
        await this.Control("GroupEditDialog_AddGroupSave").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.Control("GroupEditDialog_AvailableGroupList").SelectOptionAsync(
            new SelectOptionValue { Label = "Registered Users" },
            new LocatorSelectOptionOptions { Force = true });
        await this.Control("GroupEditDialog_GroupSortOrder").EvaluateAsync("e => { e.removeAttribute('min'); e.value = ''; }");
        await this.ClickAndWaitAsync(this.Control("GroupEditDialog_AddGroupSave"));

        var groups = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = "Registered Users" });
        (await groups.CountAsync()).Should().Be(1, "the role has the medal");

        // the same role again: the settings are updated (it failed with a database error)
        await this.ClickAndWaitAsync(this.Control("AddGroup").Filter(new LocatorFilterOptions { Visible = true }).First);
        await this.Control("GroupEditDialog_AddGroupSave").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await this.Control("GroupEditDialog_AvailableGroupList").SelectOptionAsync(
            new SelectOptionValue { Label = "Registered Users" },
            new LocatorSelectOptionOptions { Force = true });
        await this.Control("GroupEditDialog_GroupMessage").FillAsync("For all members");
        await this.ClickAndWaitAsync(this.Control("GroupEditDialog_AddGroupSave"));

        (await groups.CountAsync()).Should().Be(1);
        (await groups.InnerTextAsync()).Should().Contain("For all members");

        // the role is removed
        await groups.Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = "Registered Users" }).CountAsync())
            .Should().Be(0);

        // the medal is deleted
        await this.GotoAsync("Admin/Medals");
        row = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = name });
        await row.Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await this.Page.InnerTextAsync("body")).Should().NotContain(name);
    }

    [Test]
    [Description("A medal is given to a user found with the user select of the dialog, the message is changed and the user is removed")]
    public async Task Medal_For_User_Test()
    {
        var name = $"Medal {Guid.NewGuid():N}".Substring(0, 16);
        var userName = $"Med{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/EditMedal");
        await this.Control("Name").FillAsync(name);
        await this.Control("Category").FillAsync("UI tests");
        await this.Control("Description").FillAsync("A medal of the UI test");
        await this.Control("Message").FillAsync("Thanks for testing");
        await this.Control("MedalImage").SelectOptionAsync(
            new SelectOptionValue { Index = 1 },
            new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("Save"));

        var row = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = name });
        await this.ClickAndWaitAsync(row.Locator("[id$='_ThemeButtonEdit']"));

        // the user is searched with the select of the dialog
        await this.ClickAndWaitAsync(this.Control("AddUser").Filter(new LocatorFilterOptions { Visible = true }).First);
        await this.Control("UserEditDialog_AddUserSave").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.Page.Locator("#UserEditDialog .choices").ClickAsync();
        await this.Page.Locator("#UserEditDialog .choices__input--cloned").PressSequentiallyAsync(userName);
        await this.Page.Locator("#UserEditDialog .choices__item--choice", new PageLocatorOptions { HasTextString = userName })
            .ClickAsync();

        await this.Control("UserEditDialog_UserMessage").FillAsync("For the user");
        await this.ClickAndWaitAsync(this.Control("UserEditDialog_AddUserSave"));

        var users = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = userName });
        (await users.CountAsync()).Should().Be(1);
        (await users.InnerTextAsync()).Should().Contain("For the user");

        // edit: the user can't be changed
        await this.ClickAndWaitAsync(users.Locator(".btn-group a", new LocatorLocatorOptions { HasTextString = "Edit" }));
        await this.Control("UserEditDialog_AddUserSave").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        (await this.Control("UserEditDialog_UserName").InputValueAsync()).Should().Be(userName);
        (await this.Control("UserEditDialog_UserName").IsDisabledAsync()).Should().BeTrue();

        await this.Control("UserEditDialog_UserMessage").FillAsync("Changed for the user");
        await this.ClickAndWaitAsync(this.Control("UserEditDialog_AddUserSave"));

        (await users.InnerTextAsync()).Should().Contain("Changed for the user");

        // the user is removed
        await this.ClickAndWaitAsync(users.Locator(".btn-group a", new LocatorLocatorOptions { HasTextString = "Remove" }));
        (await users.CountAsync()).Should().Be(0);

        this.BrowserErrors.Should().BeEmpty();
    }
}
