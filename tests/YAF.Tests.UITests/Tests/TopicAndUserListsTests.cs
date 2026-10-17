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
/// The topic lists of the member (active, unanswered, watched, own topics) and the lists of the active users.
/// </summary>
[TestFixture]
public class TopicAndUserListsTests : ForumTestBase
{
    [Test]
    [Description("Every mode of the topic list with every time filter, the own topic is in the list of the own topics, all are marked as read")]
    public async Task Topic_List_Modes_Test()
    {
        var subject = $"My topic {Guid.NewGuid():N}".Substring(0, 19);

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A topic for the topic lists.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("MyTopics");

        var modes = await this.Control("TopicMode").Locator("option").CountAsync();
        var times = await this.Control("Since").Locator("option").CountAsync();

        for (var mode = 0; mode < modes; mode++)
        {
            await this.SelectAsync("TopicMode", mode);

            for (var time = 0; time < times; time++)
            {
                await this.SelectAsync("Since", time);
            }
        }

        // the own topics of all time
        await this.SelectAsync("TopicMode", modes - 1);
        await this.SelectAsync("Since", times - 1);
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);

        await this.ClickAndWaitAsync(this.Page.Locator(".card-footer [id$='_MarkAll']"));
    }

    [Test]
    [Description("The active users, the active members, the guests and (for admins only) the hidden users")]
    public async Task Active_Users_Lists_Test()
    {
        await this.LoginAsAdminAsync();

        foreach (var view in new[] { 0, 1, 2, 3 })
        {
            await this.GotoAsync($"ActiveUsers?v={view}");
            this.Page.Url.Should().Contain("ActiveUsers", $"view {view}");
        }

        // the hidden users are for admins only
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("ActiveUsers?v=3");
        this.Page.Url.Should().Contain("Info");
    }

    [Test]
    [Description("A second admin sees the other admin in the team with the buttons for a private message and an e-mail")]
    public async Task Team_Test()
    {
        var admin = $"Tad{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(admin, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        var userId = await this.OpenAdminEditUserAsync(admin);
        await this.SetRoleAsync(userId, "Administrators", true);

        try
        {
            await this.LoginAsync(admin, TestConfig.TestUserPassword);
            await this.GotoAsync("Team");

            (await this.Page.InnerTextAsync("body")).Should().Contain(TestConfig.AdminUserName).And.Contain(admin);

            (await this.Page.Locator("[id$='_PM'][href*='PostPrivateMessage']").CountAsync()).Should()
                .BeGreaterThan(0, "a private message to the other admin");
            (await this.Page.Locator("[id$='_Email'][href*='Email']").CountAsync()).Should()
                .BeGreaterThan(0, "an e-mail to the other admin");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetRoleAsync(userId, "Administrators", false);
        }
    }

    /// <summary>
    /// Adds the user to the role or removes the user from it (admin edit user page).
    /// </summary>
    private async Task SetRoleAsync(int userId, string role, bool member)
    {
        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a[href='#View2']").ClickAsync();

        await this.Page.Locator(".col-md-4", new PageLocatorOptions { HasTextString = role }).Locator("input[type='checkbox']")
            .SetCheckedAsync(member);
        await this.ClickAndWaitAsync(this.Control("GroupEditControl_Save"));
    }

    /// <summary>
    /// Selects the option of a list that posts back.
    /// </summary>
    private async Task SelectAsync(string id, int index)
    {
        var select = this.Control(id);

        if (await select.EvaluateAsync<int>("e => e.selectedIndex") == index)
        {
            return;
        }

        await this.Page.RunAndWaitForNavigationAsync(
            () => select.SelectOptionAsync(new SelectOptionValue { Index = index }, new LocatorSelectOptionOptions { Force = true }));
        await this.AssertNoServerErrorAsync();
    }
}
