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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The rights of a moderator of one forum (a role with the moderator access of the forum).
/// </summary>
[TestFixture]
public class ModeratorAccessTests : ForumTestBase
{
    [Test]
    [Description("The answer to a report shows the report only to the moderators of the forum of the reported post")]
    public async Task Report_Answer_Of_Other_Forum_Test()
    {
        var reason = $"Report {Guid.NewGuid():N}";
        var reporter = $"Rep{DateTime.UtcNow:HHmmssfff}";
        var moderator = $"Mod{DateTime.UtcNow:HHmmssfff}";
        var group = $"Mods {Guid.NewGuid():N}".Substring(0, 13);
        var forum = $"Moderated {Guid.NewGuid():N}".Substring(0, 18);

        await this.RegisterAsync(reporter, TestConfig.TestUserPassword);
        await this.RegisterAsync(moderator, TestConfig.TestUserPassword);

        // the report of the first post of the first forum
        await this.LoginAsync(reporter, TestConfig.TestUserPassword);
        await this.GotoAsync("ReportPost?m=1");
        await this.Control("Report").FillAsync(reason);
        await this.Control("btnReport").ClickAsync();
        await this.ClickAndWaitAsync(
            this.Page.Locator(".modal.show button", new PageLocatorOptions { HasTextString = "Yes" }));

        await this.LoginAsAdminAsync();
        var reporterId = await this.FindUserIdAsync(reporter);

        // a role that can read the forums and moderates a new forum
        await this.GotoAsync("Admin/EditGroup");
        await this.Control("Name").FillAsync(group);
        await this.Control("Priority").FillAsync("5");
        await this.Control("PMLimit").FillAsync("10");
        await this.Control("UsrSigChars").FillAsync("100");
        await this.Control("UsrAlbums").FillAsync("0");
        await this.Control("UsrAlbumImages").FillAsync("0");
        await this.Control("AccessMaskID").First.SelectOptionAsync(new SelectOptionValue { Label = "Read Only Access" });
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(forum);
        await this.Control("Description").FillAsync("A forum with its own moderators");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync("2");
        await this.Page.GetByLabel(group, new PageGetByLabelOptions { Exact = true })
            .SelectOptionAsync(new SelectOptionValue { Label = "Moderator Access" });
        await this.ClickAndWaitAsync(this.Control("Save"));

        // the moderator is in the role
        await this.FindUserIdAsync(moderator);
        await this.Page.Locator("a.nav-link[href='#View2']").ClickAsync();
        await this.Page.GetByLabel(group, new PageGetByLabelOptions { Exact = true }).CheckAsync();
        await this.ClickAndWaitAsync(this.Control("GroupEditControl_Save"));

        // the moderator of the new forum
        await this.LoginAsync(moderator, TestConfig.TestUserPassword);
        await this.GotoAsync(string.Empty);
        (await this.Page.Locator("a[href*='Moderate']").CountAsync()).Should().BeGreaterThan(0, "the user moderates the new forum");

        await this.GotoAsync($"PostPrivateMessage?r=1&u={reporterId}");
        (await this.Control("YafTextEditor").InputValueAsync()).Should().NotContain(reason);

        // a moderator of the forum of the post
        await this.LoginAsAdminAsync();
        await this.GotoAsync($"PostPrivateMessage?r=1&u={reporterId}");
        (await this.Control("YafTextEditor").InputValueAsync()).Should().Contain(reason);
        (await this.Control("To").InputValueAsync()).Should().Be(reporter);
    }

    /// <summary>
    /// Finds the id of a user in the admin users list and opens the edit page of the user.
    /// </summary>
    private async Task<int> FindUserIdAsync(string userName)
    {
        await this.GotoAsync("Admin/Users");

        // the name posts back and opens the edit page
        await this.ClickAndWaitAsync(
            this.Page.Locator("[id$='_NameEdit']", new PageLocatorOptions { HasTextString = userName }).First);

        return int.Parse(Regex.Match(this.Page.Url, @"u=(\d+)").Groups[1].Value);
    }
}
