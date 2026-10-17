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
using System.IO;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Admin > Users: the search by e-mail, the export of the users, deleting a user and locking inactive accounts.
/// </summary>
[TestFixture]
public class AdminUsersTests : ForumTestBase
{
    [Test]
    [Description("A user is found by the e-mail address and deleted, the users are exported as xml")]
    public async Task Search_Delete_And_Export_Test()
    {
        var member = $"Usr{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        // the export has all users
        await this.GotoAsync("Admin/Users");
        var download = await this.Page.RunAndWaitForDownloadAsync(
            () => this.Page.Locator("a[aria-label='download']").First.ClickAsync());
        File.ReadAllText(await download.PathAsync()).Should().Contain(member);

        // search by e-mail
        await this.GotoAsync("Admin/Users");
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("Email").FillAsync(EmailOf(member));
        await this.ClickAndWaitAsync(this.Control("search"));

        var rows = this.Page.Locator("[id$='_NameEdit']");
        (await rows.CountAsync()).Should().Be(1);
        (await rows.First.InnerTextAsync()).Should().Contain(member);

        // delete
        await this.Page.Locator("[id$='_ThemeButtonDelete']").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.LoginAsync(member, TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync(member)).Should().BeFalse("the user is deleted");
    }

    [Test]
    [Description("Locking the accounts inactive for years locks nobody of the test (all are active), an invalid number does nothing")]
    public async Task Lock_Inactive_Accounts_Test()
    {
        await this.LoginAsAdminAsync();

        foreach (var years in new[] { "abc", "-1", "5" })
        {
            await this.GotoAsync("Admin/Users");
            await this.Page.Locator("a.dropdown-toggle[aria-label='tools']").ClickAsync();
            await this.Control("YearsOld").EvaluateAsync("e => e.type = 'text'");
            await this.Control("YearsOld").FillAsync(years);
            await this.ClickAndWaitAsync(this.Page.Locator("a[aria-label='trash']", new PageLocatorOptions { HasTextString = "Lock" }).First);
        }

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        (await this.IsLoggedInAsAsync(TestConfig.TestUserName)).Should().BeTrue();
    }
}
