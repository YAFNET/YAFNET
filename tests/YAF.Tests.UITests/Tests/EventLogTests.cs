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

using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Admin > Event Log: an error with its details is shown, filtered and deleted.
/// </summary>
[TestFixture]
public class EventLogTests : ForumTestBase
{
    [Test]
    [Description("The error of a refused post is shown with its details (url, address, exception), filtered by type and date and deleted")]
    public async Task Error_Entry_Test()
    {
        await this.LoginAsAdminAsync();

        // the post of a button that is not on the page (the avatar upload is turned off) is an error
        await this.GotoAsync("Profile/EditAvatar");
        var back = await this.Control("Back").GetAttributeAsync("name");
        var prefix = back.Substring(0, back.Length - "Back".Length);

        await this.Page.EvaluateAsync(
            @"prefix => {
                const link = document.createElement('a');
                link.id = 'madeUpPost';
                link.href = `javascript:__doPostBack('${prefix}UpdateUpload','')`;
                link.textContent = 'upload';
                document.forms[0].appendChild(link);
            }",
            prefix);

        var response = await this.Page.RunAndWaitForResponseAsync(
            () => this.Page.Locator("#madeUpPost").ClickAsync(),
            r => r.Request.Method == "POST");
        response.Status.Should().Be(500);
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        this.ClearServerErrors();

        // the errors
        await this.GotoAsync("Admin/EventLog");
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("Types").SelectOptionAsync(
            new SelectOptionValue { Label = "Error" },
            new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("ApplyButton"));

        var entries = this.Page.Locator("li.list-group-item", new PageLocatorOptions { HasTextString = "EditAvatar" });
        (await entries.CountAsync()).Should().BeGreaterThan(0);

        // the details are collapsed
        var entry = await entries.First.TextContentAsync();
        entry.Should().Contain("HttpUnhandledException").And.Contain("EditAvatar");

        // a date filter that is no date keeps the default (all entries of the last days)
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("SinceDate").EvaluateAsync("e => { e.type = 'text'; e.value = 'no date'; }");
        await this.Control("ToDate").EvaluateAsync("e => { e.type = 'text'; e.value = 'no date'; }");
        await this.ClickAndWaitAsync(this.Control("ApplyButton"));
        (await entries.CountAsync()).Should().BeGreaterThan(0);

        // the entry is deleted
        var before = await entries.CountAsync();
        await entries.First.Locator("a[data-bs-toggle='confirm']").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await this.Page.Locator("li.list-group-item", new PageLocatorOptions { HasTextString = "EditAvatar" }).CountAsync())
            .Should().BeLessThan(before);
    }

    [Test]
    [Description("All entries of the board are deleted")]
    public async Task Delete_All_Test()
    {
        await this.LoginAsAdminAsync();

        // an entry: the test data generator logs its report (also when nothing is created)
        await this.GotoAsync("Admin/TestData");
        await this.ClickAndWaitAsync(this.Control("LaunchGenerator"));

        await this.GotoAsync("Admin/EventLog");
        (await this.Page.Locator("[id$='_Footer'] a").CountAsync()).Should().BeGreaterThan(0);

        await this.Page.Locator("[id$='_Footer'] a").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await this.Page.Locator("[id$='_Footer'] a").CountAsync()).Should().Be(0, "the list is empty");
    }

    [Test]
    [Description("The entries of a failed login, a suspension and its removal (only with the setting) and a deleted account are shown with their icons")]
    public async Task Entry_Icons_Test()
    {
        var member = $"Evi{System.DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        // a failed login
        await this.LoginAsync(member, "wrong password");

        await this.LoginAsAdminAsync();
        var userId = await this.OpenAdminEditUserAsync(member);

        // without the setting the suspension and its removal are not logged
        await this.SuspendAndRemoveAsync(userId);
        (await this.EventLogTextAsync()).Should().NotContain($"User {member} was suspended")
            .And.NotContain($"User {member} was un-suspended");

        await this.SetLogSuspensionsAsync(true);

        try
        {
            await this.SuspendAndRemoveAsync(userId);

            // the member deletes the own account
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.GotoAsync("Profile/DeleteAccount");
            await this.Page.Locator("input[type=radio][value='delete']").CheckAsync();
            await this.Control("CurrentPassword").FillAsync(TestConfig.TestUserPassword);
            await this.Control("DeleteUser").ClickAsync();
            await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

            await this.LoginAsAdminAsync();
            (await this.EventLogTextAsync()).Should().Contain($"User {member} was suspended")
                .And.Contain($"User {member} was un-suspended");

            foreach (var icon in new[] { "user-injured", "user-clock", "user-check", "user-alt-slash" })
            {
                (await this.Page.Locator($"li.list-group-item i.fa-{icon}").CountAsync()).Should().BeGreaterThan(0, icon);
            }
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetLogSuspensionsAsync(false);
        }
    }

    private async Task SuspendAndRemoveAsync(int userId)
    {
        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.EvaluateAsync(
            "() => { document.querySelector(\"[id$='_SuspendUserControl_SuspendedReason']\").value = 'Event log test'; document.querySelector(\"[id$='_SuspendUserControl_SuspendCount']\").value = '2'; }");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Page.EvaluateAsync("() => document.querySelector(\"[id$='_SuspendUserControl_Suspend']\").click()"));

        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Page.EvaluateAsync("() => document.querySelector(\"[id$='_SuspendUserControl_RemoveSuspension']\").click()"));
        await this.AssertNoServerErrorAsync();
    }

    /// <summary>
    /// The text of the event log (50 entries, the details are collapsed).
    /// </summary>
    private async Task<string> EventLogTextAsync()
    {
        await this.GotoAsync("Admin/EventLog");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("50", new LocatorSelectOptionOptions { Force = true }));

        return await this.Page.Locator("body").TextContentAsync();
    }

    private async Task SetLogSuspensionsAsync(bool enabled)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Control("LogUserSuspendedUnsuspended").EvaluateAsync("(e, v) => e.checked = v", enabled);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }
}
