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
/// A member suspends or deletes the own account.
/// </summary>
[TestFixture]
public class DeleteAccountTests : ForumTestBase
{
    [Test]
    [Description("A member suspends the own account (a wrong password is refused): the member is logged out and can not log in")]
    public async Task Suspend_Own_Account_Test()
    {
        var member = $"Sus{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        // the current password is needed
        await this.GotoAsync("Profile/DeleteAccount");
        await this.Page.Locator("input[type=radio][value='suspend']").CheckAsync();
        await this.ConfirmDeleteAsync("wrong password");
        (await this.IsLoggedInAsAsync(member)).Should().BeTrue("a wrong password is refused");

        await this.GotoAsync("Profile/DeleteAccount");
        await this.Page.Locator("input[type=radio][value='suspend']").CheckAsync();
        await this.ConfirmDeleteAsync(TestConfig.TestUserPassword);

        (await this.IsLoggedInAsAsync(member)).Should().BeFalse("the member is logged out");

        await this.LoginAsync(member, TestConfig.TestUserPassword);
        await this.GotoAsync(string.Empty);
        (await this.Page.ContentAsync()).Should().Contain("suspended");
    }

    [Test]
    [Description("A member deletes the own account: the posts are removed, the member can not log in")]
    public async Task Delete_Own_Account_Test()
    {
        var member = $"Del{DateTime.UtcNow:HHmmssfff}";
        var subject = $"Deleted account {Guid.NewGuid():N}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A post of the deleted account.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Profile/DeleteAccount");
        await this.Page.Locator("input[type=radio][value='delete']").CheckAsync();
        await this.ConfirmDeleteAsync(TestConfig.TestUserPassword);

        await this.LoginAsync(member, TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync(member)).Should().BeFalse("the account is deleted");

        await this.GotoAsync("Topics/1-Test Forum");
        (await this.Page.InnerTextAsync("body")).Should().NotContain(subject);
    }

    [Test]
    [Description("The host admin can not delete the own account")]
    public async Task Host_Admin_Can_Not_Delete_Test()
    {
        await this.LoginAsAdminAsync();

        await this.GotoAsync("Profile/DeleteAccount");

        (await this.Control("DeleteUser").CountAsync()).Should().Be(0);
    }

    private async Task ConfirmDeleteAsync(string password)
    {
        await this.Control("CurrentPassword").FillAsync(password);

        var button = this.Control("DeleteUser");

        if (await button.GetAttributeAsync("data-bs-toggle") == "confirm")
        {
            await button.ClickAsync();
            await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        }
        else
        {
            await this.ClickAndWaitAsync(button);
        }
    }
}
