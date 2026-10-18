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

    [Test]
    [Description("The admin can't delete themselves or another admin, the since filter, the page size, reset and new user")]
    public async Task Delete_Refused_And_Filters_Test()
    {
        var member = $"Adu{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();
        var memberId = await this.OpenAdminEditUserAsync(member);

        // the member is an admin
        await this.SetAdminAsync(memberId, true);

        try
        {
            foreach (var (name, email) in new[] { (TestConfig.AdminUserName, TestConfig.AdminEmail), (member, EmailOf(member)) })
            {
                // found by the e-mail address (only one user)
                await this.SearchAsync(email);
                await this.Page.Locator("[id$='_ThemeButtonDelete']").First.ClickAsync();
                await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

                (await this.Page.Locator("[id$='_NameEdit']", new PageLocatorOptions { HasTextString = name }).CountAsync())
                    .Should().Be(1, $"{name} is not deleted");
            }
        }
        finally
        {
            await this.SetAdminAsync(memberId, false);
        }

        // the since filter: the last hour and since the last visit
        foreach (var since in new[] { "-1", "0" })
        {
            await this.GotoAsync("Admin/Users");
            await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
            await this.Control("Since").SelectOptionAsync(since, new LocatorSelectOptionOptions { Force = true });
            await this.ClickAndWaitAsync(this.Control("search"));
            await this.AssertNoServerErrorAsync();
        }

        await this.GotoAsync("Admin/Users");
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("Since").SelectOptionAsync("-1", new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("search"));
        (await this.Page.InnerTextAsync("body")).Should().Contain(member, "the member registered in the last hour");

        // the page size (posts back)
        await this.Page.RunAndWaitForResponseAsync(
            () => this.Control("PageSize").SelectOptionAsync("10"),
            response => response.Request.Method == "POST");
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await this.AssertNoServerErrorAsync();

        // reset and new user
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.ClickAndWaitAsync(this.Control("ResetUserSearch"));
        this.Page.Url.Should().Contain("Admin/Users");

        await this.ClickAndWaitAsync(this.Control("NewUser"));
        this.Page.Url.Should().Contain("Admin/RegisterUser");
    }

    [Test]
    [Description("The filters of the role, the rank and the suspended users")]
    public async Task Role_Rank_And_Suspended_Filters_Test()
    {
        var member = $"Auf{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        // the administrators: the admin, not the member
        await this.FilterAsync(page => this.Control("group").SelectOptionAsync(
                                   new SelectOptionValue { Label = "Administrators" },
                                   new LocatorSelectOptionOptions { Force = true }));
        var text = await this.Page.InnerTextAsync("body");
        text.Should().Contain(TestConfig.AdminUserName).And.NotContain(member);

        // the rank of new members (with the name, a full run has more members than a page shows)
        await this.FilterAsync(
            async page =>
                {
                    await this.Control("name").FillAsync(member);
                    await this.Control("rank").SelectOptionAsync(
                        new SelectOptionValue { Label = "Newbie" },
                        new LocatorSelectOptionOptions { Force = true });
                });
        (await this.Page.InnerTextAsync("body")).Should().Contain(member);

        // the member is not suspended
        await this.FilterAsync(page => this.Control("SuspendedOnly").CheckAsync());
        (await this.Page.InnerTextAsync("body")).Should().NotContain(member);
    }

    private async Task FilterAsync(Func<IPage, Task> filter)
    {
        await this.GotoAsync("Admin/Users");
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await filter(this.Page);
        await this.ClickAndWaitAsync(this.Control("search"));
        await this.AssertNoServerErrorAsync();
    }

    [Test]
    [Description("The admin changes the rank and the moderation of a member on the info tab, the values are saved")]
    public async Task Edit_User_Info_Test()
    {
        var member = $"Uin{DateTime.UtcNow:HHmmssfff}";
        var rank = $"Info rank {Guid.NewGuid():N}".Substring(0, 20);

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/EditRank");
        await this.Control("Name").FillAsync(rank);
        await this.Control("MinPosts").FillAsync("0");
        await this.Control("RankPriority").FillAsync("5");
        await this.Control("PMLimit").FillAsync("10");
        await this.Control("UsrSigChars").FillAsync("100");
        await this.Control("UsrAlbums").FillAsync("0");
        await this.Control("UsrAlbumImages").FillAsync("0");
        await this.ClickAndWaitAsync(this.Control("Save"));

        var userId = await this.OpenAdminEditUserAsync(member);

        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Control("QuickEditControl_RankID").SelectOptionAsync(
            new SelectOptionValue { Label = rank },
            new LocatorSelectOptionOptions { Force = true });
        await this.Control("QuickEditControl_IsExcludedFromActiveUsers").CheckAsync(new LocatorCheckOptions { Force = true });
        await this.Control("QuickEditControl_Moderated").CheckAsync(new LocatorCheckOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("QuickEditControl_Save"));

        await this.GotoAsync($"Admin/EditUser?u={userId}");
        (await this.Control("QuickEditControl_RankID").Locator("option:checked").InnerTextAsync()).Should().Be(rank);
        (await this.Control("QuickEditControl_IsExcludedFromActiveUsers").IsCheckedAsync()).Should().BeTrue();
        (await this.Control("QuickEditControl_Moderated").IsCheckedAsync()).Should().BeTrue();
    }

    private async Task SearchAsync(string email)
    {
        await this.GotoAsync("Admin/Users");
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("Email").FillAsync(email);
        await this.ClickAndWaitAsync(this.Control("search"));
    }

    private async Task SetAdminAsync(int userId, bool admin)
    {
        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a[href='#View2']").ClickAsync();

        await this.Page.Locator(".col-md-4", new PageLocatorOptions { HasTextString = "Administrators" })
            .Locator("input[type='checkbox']").SetCheckedAsync(admin);
        await this.ClickAndWaitAsync(this.Control("GroupEditControl_Save"));
    }
}
