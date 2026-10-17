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
    [Description("A medal is created and given to a role, an empty sort order is 0, the role and the medal are removed")]
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
        await this.ClickAndWaitAsync(this.Control("AddGroup").First);
        await this.Control("GroupEditDialog_AddGroupSave").WaitForAsync(
            new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.Control("GroupEditDialog_AvailableGroupList").SelectOptionAsync(
            new SelectOptionValue { Label = "Registered Users" },
            new LocatorSelectOptionOptions { Force = true });
        await this.Control("GroupEditDialog_GroupSortOrder").EvaluateAsync("e => { e.removeAttribute('min'); e.value = ''; }");
        await this.ClickAndWaitAsync(this.Control("GroupEditDialog_AddGroupSave"));

        var groups = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = "Registered Users" });
        (await groups.CountAsync()).Should().Be(1, "the role has the medal");

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
}
