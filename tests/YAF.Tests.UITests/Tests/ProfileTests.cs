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
/// The profile of a member: the custom profile fields of the admin, the checks of the input, the country and region.
/// </summary>
[TestFixture]
public class ProfileTests : ForumTestBase
{
    [Test]
    [Description("The custom profile fields of the admin are filled in by a member and shown on the profile page")]
    public async Task Custom_Profile_Fields_Test()
    {
        var id = Guid.NewGuid().ToString("N").Substring(0, 6);
        var number = $"Shoe size {id}";
        var check = $"Likes cats {id}";
        var text = $"Motto {id}";
        var member = $"Prof{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.AddDefinitionAsync(number, "Number", required: true);
        await this.AddDefinitionAsync(check, "Check", required: false);
        await this.AddDefinitionAsync(text, "Text", required: false);

        try
        {
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.GotoAsync("Profile/EditProfile");

            // the required field (also checked by the server, the browser checks it first)
            await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true })
                .EvaluateAsync("e => e.removeAttribute('required')");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
            (await this.Page.ContentAsync()).Should().Contain($"{number} is required");

            // no number (the browser only accepts numbers)
            await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true })
                .EvaluateAsync("e => e.type = 'text'");
            await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true }).FillAsync("big");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
            (await this.Page.ContentAsync()).Should().Contain($"{number} must be a number");

            await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true }).FillAsync("42");
            await this.Page.GetByLabel(check, new PageGetByLabelOptions { Exact = true }).CheckAsync();
            await this.Page.GetByLabel(text, new PageGetByLabelOptions { Exact = true }).FillAsync("Hello <b>world</b>");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

            // the values are kept
            await this.GotoAsync("Profile/EditProfile");
            (await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true }).InputValueAsync()).Should().Be("42");
            (await this.Page.GetByLabel(check, new PageGetByLabelOptions { Exact = true }).IsCheckedAsync()).Should().BeTrue();

            // and shown on the profile page
            await this.OpenOwnProfileAsync();

            var profile = await this.Page.InnerTextAsync("body");
            profile.Should().Contain($"{number}:").And.Contain("42");
            profile.Should().Contain("Hello <b>world</b>", "the value is shown as text");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.DeleteDefinitionAsync(number);
            await this.DeleteDefinitionAsync(check);
            await this.DeleteDefinitionAsync(text);
        }
    }

    [Test]
    [Description("The addresses of the profile are checked, a home page without a scheme gets https")]
    public async Task Address_Checks_Test()
    {
        var member = $"Addr{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/EditProfile");

        await this.Control("ProfileEditor_Weblog").EvaluateAsync("e => e.type = 'text'");
        await this.Control("ProfileEditor_Weblog").FillAsync("not a blog");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
        (await this.Page.ContentAsync()).Should().Contain("invalid address for your Blog");

        await this.Control("ProfileEditor_Weblog").FillAsync(string.Empty);
        await this.Control("ProfileEditor_Xmpp").FillAsync("no xmpp");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
        (await this.Page.ContentAsync()).Should().Contain("invalid name for your XMPP account");

        await this.Control("ProfileEditor_Xmpp").FillAsync(string.Empty);
        await this.Control("ProfileEditor_Facebook").EvaluateAsync("e => e.type = 'text'");
        await this.Control("ProfileEditor_Facebook").FillAsync("not a profile");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
        (await this.Page.ContentAsync()).Should().Contain("incorrect URL as Facebook Profile");

        await this.Control("ProfileEditor_Facebook").FillAsync(string.Empty);
        await this.Control("ProfileEditor_HomePage").EvaluateAsync("e => e.type = 'text'");
        await this.Control("ProfileEditor_HomePage").FillAsync("www.example.com");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        await this.GotoAsync("Profile/EditProfile");
        (await this.Control("ProfileEditor_HomePage").InputValueAsync()).Should().Be("https://www.example.com");
    }

    [Test]
    [Description("A country with regions shows the regions, the region of a newly selected country is saved, a country without regions has no region list")]
    public async Task Country_And_Region_Test()
    {
        var member = $"Ctry{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/EditProfile");

        (await this.Control("ProfileEditor_Region").CountAsync()).Should().Be(0, "no country is selected");

        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("ProfileEditor_Country").SelectOptionAsync(
                "US",
                new LocatorSelectOptionOptions { Force = true }));
        await this.AssertNoServerErrorAsync();

        await this.Control("ProfileEditor_Region").SelectOptionAsync("CA", new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        await this.GotoAsync("Profile/EditProfile");
        (await this.Control("ProfileEditor_Country").InputValueAsync()).Should().Be("US");
        (await this.Control("ProfileEditor_Region").InputValueAsync()).Should().Be("CA");

        // a country without regions
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("ProfileEditor_Country").SelectOptionAsync(
                "FR",
                new LocatorSelectOptionOptions { Force = true }));
        await this.AssertNoServerErrorAsync();

        (await this.Control("ProfileEditor_Region").CountAsync()).Should().Be(0, "France has no regions");

        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        await this.GotoAsync("Profile/EditProfile");
        (await this.Control("ProfileEditor_Country").InputValueAsync()).Should().Be("FR");
    }

    private async Task AddDefinitionAsync(string name, string dataType, bool required)
    {
        await this.GotoAsync("Admin/ProfileDefinitions");

        await this.ClickAndWaitAsync(
            this.Page.Locator("a", new PageLocatorOptions { HasTextString = "Add New Definition" })
                .Filter(new LocatorFilterOptions { Visible = true }).First);

        await this.Page.Locator("[id$='_EditDialog_Save']")
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.Control("EditDialog_Name").FillAsync(name);
        await this.Control("EditDialog_DataTypes").SelectOptionAsync(dataType, new LocatorSelectOptionOptions { Force = true });
        await this.Control("EditDialog_Length").FillAsync("20");

        if (required)
        {
            await this.Control("EditDialog_Required").CheckAsync();
        }

        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        (await this.Page.InnerTextAsync("body")).Should().Contain(name);
    }

    private async Task DeleteDefinitionAsync(string name)
    {
        await this.GotoAsync("Admin/ProfileDefinitions");

        var row = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = name });

        if (await row.CountAsync() == 0)
        {
            return;
        }

        await row.Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
    }

    private async Task OpenOwnProfileAsync()
    {
        var href = await this.Page.Locator("a[href*='UserProfile']").First.GetAttributeAsync("href");

        await this.GotoAsync(href.TrimStart('/'));
    }
}
