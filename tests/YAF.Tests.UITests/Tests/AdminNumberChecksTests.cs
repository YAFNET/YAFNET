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
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The number fields of the admin pages: a number that is too big is refused (it was a server error).
/// </summary>
[TestFixture]
public class AdminNumberChecksTests : ForumTestBase
{
    private const string TooBig = "99999999999";

    [Test]
    [Description("A too big priority of a role and a too big minimum of posts or priority of a rank are refused")]
    public async Task Role_And_Rank_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");

        await this.LoginAsAdminAsync();

        await this.SubmitAsync(
            "Admin/EditGroup",
            new Dictionary<string, string>
                {
                    ["Name"] = $"Big role {id}", ["Priority"] = "99999", ["PMLimit"] = "10", ["UsrSigChars"] = "100",
                    ["UsrAlbums"] = "0", ["UsrAlbumImages"] = "0"
                },
            "Save");
        this.Page.Url.Should().Contain("EditGroup");

        foreach (var (minPosts, priority) in new[] { (TooBig, "5"), ("1", "99999") })
        {
            await this.SubmitAsync(
                "Admin/EditRank",
                new Dictionary<string, string>
                    {
                        ["Name"] = $"Big rank {id}", ["MinPosts"] = minPosts, ["RankPriority"] = priority, ["PMLimit"] = "10",
                        ["UsrSigChars"] = "100", ["UsrAlbums"] = "0", ["UsrAlbumImages"] = "0"
                    },
                "Save");
            this.Page.Url.Should().Contain("EditRank", minPosts);
        }
    }

    [Test]
    [Description("Too big numbers of days of the prune pages, of the board settings and of the bb code order are refused")]
    public async Task Prune_Settings_And_BBCode_Test()
    {
        await this.LoginAsAdminAsync();

        await this.SubmitAsync("Admin/PM", new Dictionary<string, string> { ["Days1"] = TooBig, ["Days2"] = "1" }, "commit", confirm: true);
        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        await this.SubmitAsync("Admin/Prune", new Dictionary<string, string> { ["days"] = TooBig }, "commit", confirm: true);
        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        // the setting is kept
        await this.SubmitAsync(
            "Admin/Settings",
            new Dictionary<string, string> { ["DefaultPageSize"] = TooBig, ["DigestSendEveryXHours"] = TooBig },
            "Save");

        await this.SubmitAsync(
            "Admin/BBCode_Edit",
            new Dictionary<string, string>
                {
                    ["txtName"] = $"BIG{DateTime.UtcNow:HHmmssfff}", ["txtDescription"] = "Too big order", ["txtExecOrder"] = "99999"
                },
            "Save");
        this.Page.Url.Should().Contain("BBCode_Edit");
    }

    [Test]
    [Description("A too big length of a profile definition and too big points of a user are refused")]
    public async Task Profile_Definition_And_Points_Test()
    {
        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/ProfileDefinitions");
        await this.ClickAndWaitAsync(
            this.Page.Locator("a", new PageLocatorOptions { HasTextString = "Add New Definition" })
                .Filter(new LocatorFilterOptions { Visible = true }).First);
        await this.Page.Locator("[id$='_EditDialog_Save']").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await this.Control("EditDialog_Name").FillAsync($"Big length {DateTime.UtcNow:HHmmssfff}");
        await this.SetValuesAsync(new Dictionary<string, string> { ["EditDialog_Length"] = TooBig });
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));
        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        var userId = await this.OpenAdminEditUserAsync(TestConfig.AdminUserName);
        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a.nav-link[href='#View6']").ClickAsync();

        foreach (var (field, button) in new[]
                     {
                         ("txtAddPoints", "btnAddPoints"), ("txtRemovePoints", "Button1"), ("txtUserPoints", "btnUserPoints")
                     })
        {
            await this.GotoAsync($"Admin/EditUser?u={userId}");
            await this.Page.Locator("a.nav-link[href='#View6']").ClickAsync();
            await this.SetValuesAsync(new Dictionary<string, string> { [field] = TooBig });
            await this.ClickAndWaitAsync(this.Control(button));
            (await this.Page.ContentAsync()).Should().Contain("toast-body", field);
        }
    }

    /// <summary>
    /// Opens the page, sets the values (also of number fields) and clicks the button.
    /// </summary>
    private async Task SubmitAsync(string url, Dictionary<string, string> values, string button, bool confirm = false)
    {
        await this.GotoAsync(url);

        await this.SetValuesAsync(values);

        if (confirm)
        {
            await this.Control(button).ClickAsync();
            await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        }
        else
        {
            await this.ClickAndWaitAsync(this.Control(button));
        }
    }

    private async Task SetValuesAsync(Dictionary<string, string> values)
    {
        await this.Page.EvaluateAsync(
            """
            values => {
                for (const [id, value] of Object.entries(values)) {
                    const e = document.querySelector(`[id$='_${id}']`);
                    e.removeAttribute('max');
                    e.removeAttribute('maxlength');
                    e.type = 'text';
                    e.value = value;
                }
            }
            """,
            values);
    }
}
