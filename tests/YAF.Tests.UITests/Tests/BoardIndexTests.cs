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
/// The board index: watch all forums, mark all forums as read and the statistics of the users.
/// </summary>
[TestFixture]
public class BoardIndexTests : ForumTestBase
{
    [Test]
    [Description("A member watches all forums (they are in the subscriptions) and marks all forums as read")]
    public async Task Watch_All_And_Mark_All_Test()
    {
        var member = $"Wal{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        // the list of the categories is an update panel
        await this.GotoAsync(string.Empty);
        await this.PartialPostBackAsync(this.Control("WatchForum").Filter(new LocatorFilterOptions { Visible = true }).First);

        await this.PartialPostBackAsync(this.Control("MarkAll").Filter(new LocatorFilterOptions { Visible = true }).First);

        // the watched forums are listed (the list is shown for the selected forums and topics)
        await this.GotoAsync("Profile/Subscriptions");
        await this.Page.RunAndWaitForResponseAsync(
            () => this.Control("rblNotificationType").Locator("input[value='0']").CheckAsync(),
            response => response.Request.Method == "POST");
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        (await this.Page.Locator("[id*='_ForumList_'][id$='_unsubf']").CountAsync()).Should().BeGreaterThan(0);
    }

    [Test]
    [Description("The statistics show the recent users and the active guests (host settings)")]
    public async Task Recent_Users_And_Guests_Test()
    {
        await this.LoginAsAdminAsync();
        await this.SetStatisticsAsync(true);

        try
        {
            // a guest is active
            await this.LogoutAsync();
            await this.GotoAsync("Topics/1-Test Forum");

            await this.LoginAsAdminAsync();
            await this.GotoAsync(string.Empty);

            (await this.Control("RecentUsersCount").CountAsync()).Should().BeGreaterThan(0, "the recent users are shown");
            (await this.Page.Locator("a[href*='ActiveUsers']").CountAsync()).Should().BeGreaterThan(0);
        }
        finally
        {
            await this.SetStatisticsAsync(false);
        }
    }

    [Test]
    [Description("A hidden member that is online is counted as hidden for admins, the link lists the hidden users")]
    public async Task Hidden_Active_User_Test()
    {
        var member = $"Hid{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        // the admin hides the member from the active users
        await this.LoginAsAdminAsync();
        var userId = await this.OpenAdminEditUserAsync(member);
        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a[href='#View10']").ClickAsync();
        await this.Control("ProfileSettings_HideMe").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));

        // the member is online
        await this.LoginAsync(member, TestConfig.TestUserPassword);
        await this.GotoAsync(string.Empty);

        await this.LoginAsAdminAsync();
        await this.GotoAsync(string.Empty);

        var hidden = this.Page.Locator("a[href*='ActiveUsers']", new PageLocatorOptions { HasTextString = "hidden" });
        (await hidden.CountAsync()).Should().BeGreaterThan(0, "the hidden member is counted");

        await this.ClickAndWaitAsync(hidden.First);
        (await this.Page.InnerTextAsync("body")).Should().Contain(member);
    }

    private async Task PartialPostBackAsync(ILocator button)
    {
        await this.Page.RunAndWaitForResponseAsync(() => button.ClickAsync(), response => response.Request.Method == "POST");
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await this.AssertNoServerErrorAsync();
    }

    private async Task SetStatisticsAsync(bool enabled)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Page.EvaluateAsync(
            "enabled => ['ShowRecentUsers', 'ShowGuestsInDetailedActiveList'].forEach(id => document.querySelector(`[id$='_${id}']`).checked = enabled)",
            enabled);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }
}
