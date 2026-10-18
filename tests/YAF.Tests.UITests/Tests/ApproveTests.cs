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

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The verification page of the e-mail addresses.
/// </summary>
[TestFixture]
public class ApproveTests : ForumTestBase
{
    [Test]
    [Description("No key and an unknown key (in the address and in the form) fail")]
    public async Task Invalid_Key_Test()
    {
        await this.LogoutAsync();

        foreach (var url in new[] { "Account/Approve", "Account/Approve?code=unknown" })
        {
            await this.GotoAsync(url);
            await this.AssertNoServerErrorAsync();
            (await this.Page.InnerTextAsync("body")).Should().Contain("Failed to verify email address.", url);
        }

        await this.Control("key").FillAsync("another unknown key");
        await this.ClickAndWaitAsync(this.Control("ValidateKey"));
        await this.AssertNoServerErrorAsync();
        (await this.Page.InnerTextAsync("body")).Should().Contain("Failed to verify email address.");
    }

    [Test]
    [Description("The new address of a member is taken by another account before it is confirmed: the confirmation is refused")]
    public async Task New_Address_Taken_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var member = $"Apm{id}";
        var other = $"Apo{id}";
        var address = $"taken.{id}@localhost.com";

        await this.RegisterAsync(other, TestConfig.TestUserPassword);
        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        // the member changes the address (a confirmation mail to the new address)
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        var started = DateTime.UtcNow;

        await this.GotoAsync("Profile/EditSettings");
        await this.Control("ProfileSettings_Email").FillAsync(address);
        await this.Control("ProfileSettings_CurrentPassword").FillAsync(TestConfig.TestUserPassword);
        await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));

        var mail = await TestMail.WaitForAsync(address, started);

        // the admin gives the address to the other account
        await this.LoginAsAdminAsync();
        var otherId = await this.OpenAdminEditUserAsync(other);
        await this.GotoAsync($"Admin/EditUser?u={otherId}");
        await this.Page.Locator("a[href='#View10']").ClickAsync();
        await this.Control("ProfileSettings_Email").FillAsync(address);
        await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));

        await this.LoginAsync(member, TestConfig.TestUserPassword);
        await this.GotoAsync(mail.GetLink("Approve"));
        await this.AssertNoServerErrorAsync();
        (await this.Page.InnerTextAsync("body")).Should().Contain("The e-mail address you have entered is already in use.");

        await this.GotoAsync("Profile/EditSettings");
        (await this.Control("ProfileSettings_Email").InputValueAsync()).Should().Be(EmailOf(member));
    }
}
