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
/// The settings of the profile: the e-mail address checks, the change of the address by an admin and the theme and
/// language of the user.
/// </summary>
[TestFixture]
public class EditSettingsTests : ForumTestBase
{
    [Test]
    [Description("An invalid address and the address of another user are refused, cancel goes back to the account")]
    public async Task Email_Checks_Test()
    {
        var member = $"Set{DateTime.UtcNow:HHmmssfff}";
        var other = $"Sot{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(other, TestConfig.TestUserPassword);
        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        foreach (var email in new[] { "not-an-address", EmailOf(other) })
        {
            await this.GotoAsync("Profile/EditSettings");
            await this.Control("ProfileSettings_Email").EvaluateAsync("e => e.type = 'text'");
            await this.Control("ProfileSettings_Email").FillAsync(email);
            await this.Control("ProfileSettings_CurrentPassword").FillAsync(TestConfig.TestUserPassword);
            await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));

            this.Page.Url.Should().Contain("EditSettings", email);
        }

        // the address of the other member is in use
        (await this.Page.ContentAsync()).Should().Contain("The e-mail address you have entered is already in use.");

        await this.GotoAsync("Profile/EditSettings");
        (await this.Control("ProfileSettings_Email").InputValueAsync()).Should().Be(EmailOf(member));

        await this.ClickAndWaitAsync(this.Control("ProfileSettings_Cancel"));
        this.Page.Url.Should().Contain("MyAccount");
    }

    [Test]
    [Description("An admin changes the address of a user at once (without a confirmation)")]
    public async Task Admin_Changes_Email_Test()
    {
        var member = $"Sem{DateTime.UtcNow:HHmmssfff}";
        var newEmail = $"changed.{member.ToLowerInvariant()}@localhost.com";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        var userId = await this.OpenAdminEditUserAsync(member);

        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a[href='#View10']").ClickAsync();
        await this.Control("ProfileSettings_Email").FillAsync(newEmail);
        await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));

        await this.GotoAsync($"Admin/EditUser?u={userId}");
        (await this.Control("ProfileSettings_Email").InputValueAsync()).Should().Be(newEmail);
    }

    [Test]
    [Description("The admin saves the settings of the guest user (the time zone of the guests)")]
    public async Task Guest_Settings_Test()
    {
        await this.LoginAsAdminAsync();
        var guestId = await this.OpenAdminEditUserAsync("Guest");

        await this.GotoAsync($"Admin/EditUser?u={guestId}");
        await this.Page.Locator("a[href='#View10']").ClickAsync();
        await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));
        await this.AssertNoServerErrorAsync();
    }

    [Test]
    [Description("When the users may choose the theme and the language, a member chooses German and the page is German")]
    public async Task Theme_And_Language_Test()
    {
        var member = $"Lng{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.SetUserChoicesAsync(true);

        try
        {
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.GotoAsync("Profile/EditSettings");

            (await this.Control("ProfileSettings_Theme").Locator("option").CountAsync()).Should().BeGreaterThan(0);

            var german = await this.Control("ProfileSettings_Culture").EvaluateAsync<string>(
                "e => Array.from(e.options).find(o => o.value.startsWith('de')).value");
            await this.Control("ProfileSettings_Culture").SelectOptionAsync(german, new LocatorSelectOptionOptions { Force = true });
            await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));

            (await this.Page.GetAttributeAsync("html", "lang")).Should().StartWith("de");

            // back to english
            await this.GotoAsync("Profile/EditSettings");
            var english = await this.Control("ProfileSettings_Culture").EvaluateAsync<string>(
                "e => Array.from(e.options).find(o => o.value.startsWith('en')).value");
            await this.Control("ProfileSettings_Culture").SelectOptionAsync(english, new LocatorSelectOptionOptions { Force = true });
            await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));

            (await this.Page.GetAttributeAsync("html", "lang")).Should().StartWith("en");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetUserChoicesAsync(false);
        }
    }

    /// <summary>
    /// Allows or forbids the users to choose the theme and the language (host settings).
    /// </summary>
    private async Task SetUserChoicesAsync(bool allowed)
    {
        await this.GotoAsync("Admin/HostSettings");

        await this.Page.EvaluateAsync(
            "allowed => ['AllowUserTheme', 'AllowUserLanguage'].forEach(id => document.querySelector(`[id$='_${id}']`).checked = allowed)",
            allowed);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }
}
