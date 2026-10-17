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
/// The login dialog of the forum pages (the buttons post back in an update panel).
/// </summary>
[TestFixture]
public class LoginBoxTests : ForumTestBase
{
    [Test]
    [Description("A wrong password is refused in the dialog, the right one logs in")]
    public async Task Login_With_Dialog_Test()
    {
        await this.LogoutAsync();
        await this.GotoAsync("Topics/1-Test Forum");

        await this.OpenDialogAsync();
        await this.Page.Locator("#LoginBox [id$='_UserName']").FillAsync(TestConfig.AdminUserName);
        await this.Page.Locator("#LoginBox [id$='_Password']").FillAsync("wrong password");

        // a partial post back (update panel)
        var response = await this.Page.RunAndWaitForResponseAsync(
            () => this.Page.Locator("#LoginBox [id$='_LoginButton']").ClickAsync(),
            r => r.Request.Method == "POST");
        response.Status.Should().Be(200);
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await this.GotoAsync("Topics/1-Test Forum");
        (await this.IsLoggedInAsAsync(TestConfig.AdminUserName)).Should().BeFalse();

        await this.GotoAsync("Topics/1-Test Forum");
        await this.OpenDialogAsync();
        await this.Page.Locator("#LoginBox [id$='_UserName']").FillAsync(TestConfig.AdminUserName);
        await this.Page.Locator("#LoginBox [id$='_Password']").FillAsync(TestConfig.AdminPassword);

        // the login redirects after the partial post back
        response = await this.Page.RunAndWaitForResponseAsync(
            () => this.Page.Locator("#LoginBox [id$='_LoginButton']").ClickAsync(),
            r => r.Request.Method == "POST");
        response.Status.Should().Be(200);
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await this.GotoAsync(string.Empty);
        (await this.IsLoggedInAsAsync(TestConfig.AdminUserName)).Should().BeTrue();
    }

    private async Task OpenDialogAsync()
    {
        await this.Page.Locator(".LoginLink").First.ClickAsync();
        await this.Page.Locator("#LoginBox").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }
}
