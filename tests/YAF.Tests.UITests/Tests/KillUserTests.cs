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
/// Admin > Edit User > Kill: the account of a spammer is removed with the posts, the name and the e-mail address are
/// banned.
/// </summary>
[TestFixture]
public class KillUserTests : ForumTestBase
{
    [Test]
    [Description("The spammer is deleted with the posts, name and e-mail are banned, the own address of the admin (the same as the one of the spammer) is not banned")]
    public async Task Kill_Spammer_Test()
    {
        var spammer = $"Spam{DateTime.UtcNow:HHmmssfff}";
        var subject = $"Buy now {Guid.NewGuid():N}";

        await this.RegisterAsync(spammer, TestConfig.TestUserPassword);
        await this.LoginAsync(spammer, TestConfig.TestUserPassword);

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("Spam spam spam.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.LoginAsAdminAsync();
        await this.OpenAdminEditUserAsync(spammer);
        await this.Page.Locator("a.nav-link[href='#View9']").ClickAsync();

        // ban the name, the e-mail address and the addresses (all checked), delete the account
        await this.Page.Locator("input[type=radio][value='delete']").CheckAsync();
        await this.Control("KillEdit1_Kill").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        // the forum still works for the admin (the same address as the spammer)
        await this.GotoAsync("Admin/BannedNames");
        (await this.Page.InnerTextAsync("body")).Should().Contain(spammer);

        await this.GotoAsync("Admin/BannedEmails");
        (await this.Page.InnerTextAsync("body")).Should().Contain(EmailOf(spammer).Split('@')[0], "the address is a regular expression");

        await this.GotoAsync("Admin/BannedIps");
        (await this.Page.InnerTextAsync("body")).Should().NotContain("::1").And.NotContain("127.0.0.1");

        await this.GotoAsync("Topics/1-Test Forum");
        (await this.Page.InnerTextAsync("body")).Should().NotContain(subject);

        await this.LoginAsync(spammer, TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync(spammer)).Should().BeFalse();
    }

    [Test]
    [Description("The suspend option suspends the user for years at once")]
    public async Task Suspend_Option_Test()
    {
        var spammer = $"Sus{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(spammer, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        // suspended (nothing banned)
        await this.OpenKillTabAsync(spammer);
        foreach (var ban in new[] { "BanName", "BanEmail", "BanIps" })
        {
            await this.Control($"KillEdit1_{ban}").SetCheckedAsync(false);
        }

        await this.Page.Locator("input[type=radio][value='suspend']").CheckAsync();
        await this.Control("KillEdit1_Kill").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.LoginAsync(spammer, TestConfig.TestUserPassword);
        await this.GotoAsync(string.Empty);
        this.Page.Url.Should().Contain("Info", "the user is suspended");
    }

    [Test]
    [Description("The admin can't kill themselves or another admin (nothing is banned)")]
    public async Task Kill_Refused_Test()
    {
        var admin = $"Kad{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(admin, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();
        var adminId = await this.OpenAdminEditUserAsync(admin);

        // the member is an admin
        await this.GotoAsync($"Admin/EditUser?u={adminId}");
        await this.Page.Locator("a[href='#View2']").ClickAsync();
        await this.Page.Locator(".col-md-4", new PageLocatorOptions { HasTextString = "Administrators" })
            .Locator("input[type='checkbox']").SetCheckedAsync(true);
        await this.ClickAndWaitAsync(this.Control("GroupEditControl_Save"));

        try
        {
            foreach (var name in new[] { TestConfig.AdminUserName, admin })
            {
                await this.OpenKillTabAsync(name);
                await this.Page.Locator("input[type=radio][value='suspend']").CheckAsync();
                await this.Control("KillEdit1_Kill").ClickAsync();
                await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
                await this.AssertNoServerErrorAsync();
            }

            await this.GotoAsync("Admin/BannedNames");
            (await this.Page.InnerTextAsync("body")).Should().NotContain(admin);

            await this.LoginAsync(admin, TestConfig.TestUserPassword);
            await this.GotoAsync(string.Empty);
            this.Page.Url.Should().NotContain("Info", "the admin is not suspended");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.GotoAsync($"Admin/EditUser?u={adminId}");
            await this.Page.Locator("a[href='#View2']").ClickAsync();
            await this.Page.Locator(".col-md-4", new PageLocatorOptions { HasTextString = "Administrators" })
                .Locator("input[type='checkbox']").SetCheckedAsync(false);
            await this.ClickAndWaitAsync(this.Control("GroupEditControl_Save"));
        }
    }

    private async Task OpenKillTabAsync(string userName)
    {
        await this.OpenAdminEditUserAsync(userName);
        await this.Page.Locator("a.nav-link[href='#View9']").ClickAsync();
    }
}
