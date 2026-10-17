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
/// The boards of the host admin: a board is created, renamed and deleted.
/// </summary>
[TestFixture]
public class BoardTests : ForumTestBase
{
    [Test]
    [Description("A board with a new admin whose name is taken is not created (with a message), a board of the host admin is created, renamed and deleted")]
    public async Task Create_Rename_And_Delete_Board_Test()
    {
        var name = $"Board {Guid.NewGuid():N}".Substring(0, 16);
        var renamed = $"Renamed {Guid.NewGuid():N}".Substring(0, 16);

        await this.LoginAsAdminAsync();

        // a new admin with the name of an existing user
        await this.GotoAsync("Admin/EditBoard");
        await this.Control("Name").FillAsync(name);
        await this.Page.RunAndWaitForNavigationAsync(() => this.Control("CreateAdminUser").CheckAsync());
        await this.Control("UserName").FillAsync(TestConfig.AdminUserName);
        await this.Control("UserEmail").FillAsync($"other.{EmailOf(TestConfig.AdminUserName)}");
        await this.Control("UserPass1").FillAsync(TestConfig.TestUserPassword);
        await this.Control("UserPass2").FillAsync(TestConfig.TestUserPassword);
        await this.ClickAndWaitAsync(this.Control("Save"));

        this.Page.Url.Should().Contain("Admin/EditBoard", "the board is not created");
        (await this.Page.ContentAsync()).Should().Contain("Create User Failed");

        // the host admin is the admin of the new board
        await this.GotoAsync("Admin/EditBoard");
        await this.Control("Name").FillAsync(name);
        await this.ClickAndWaitAsync(this.Control("Save"));

        (await this.Page.InnerTextAsync("body")).Should().Contain(name);

        var row = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = name });
        await this.ClickAndWaitAsync(row.Locator("[id$='_ThemeButtonEdit']"));

        await this.Control("Name").FillAsync(renamed);
        await this.ClickAndWaitAsync(this.Control("Save"));

        (await this.Page.InnerTextAsync("body")).Should().Contain(renamed).And.NotContain(name);

        row = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = renamed });
        await row.Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await this.Page.InnerTextAsync("body")).Should().NotContain(renamed);
    }
}
