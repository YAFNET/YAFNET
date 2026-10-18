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
/// The roles of the admin: the number checks, renaming a role with members and deleting it.
/// </summary>
[TestFixture]
public class AdminRolesTests : ForumTestBase
{
    [Test]
    [Description("Invalid numbers are refused, a renamed role keeps its members, cancel goes back, the role is deleted")]
    public async Task Rename_And_Delete_Role_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var role = $"Role {id}";
        var renamed = $"Renamed <b>{id}</b>";
        var member = $"Rol{id}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        // invalid numbers
        foreach (var field in new[] { "PMLimit", "UsrAlbums", "UsrSigChars", "UsrAlbumImages" })
        {
            var values = this.RoleValues(role);
            values[field] = "abc";

            await this.SaveRoleAsync("Admin/EditGroup", values);
            this.Page.Url.Should().Contain("EditGroup", field);
        }

        await this.SaveRoleAsync("Admin/EditGroup", this.RoleValues(role));

        // the member gets the role
        var userId = await this.OpenAdminEditUserAsync(member);
        await this.SetRoleAsync(userId, role, true);

        // renamed (the users of the role are moved to the new name)
        await this.GotoAsync("Admin/Groups");
        await this.ClickAndWaitAsync(this.RoleRow(role).Locator("[id$='_ThemeButtonEdit']"));
        var editUrl = this.Page.Url;

        await this.SaveRoleAsync(editUrl, new Dictionary<string, string> { ["Name"] = renamed });

        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a[href='#View2']").ClickAsync();
        (await this.Page.Locator(".col-md-4", new PageLocatorOptions { HasTextString = renamed })
             .Locator("input[type='checkbox']").IsCheckedAsync()).Should().BeTrue("the member is in the renamed role");

        // the role on the profile of the member (the name is a text)
        await this.GotoAsync($"UserProfile/{userId}-{member}");
        (await this.Page.InnerTextAsync("body")).Should().Contain(renamed);

        // cancel
        await this.GotoAsync(editUrl);
        await this.ClickAndWaitAsync(this.Control("Cancel"));
        this.Page.Url.Should().Contain("Admin/Groups");

        // delete
        await this.RoleRow(renamed).Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        (await this.RoleRow(renamed).CountAsync()).Should().Be(0);

        // the role of the provider stays: it is added as a group again
        await this.ClickAndWaitAsync(this.ProviderRoleRow(renamed).Locator("[id$='_ThemeButtonAdd']"));
        this.Page.Url.Should().Contain("Admin/EditGroup?i=");

        await this.GotoAsync("Admin/Groups");
        (await this.RoleRow(renamed).CountAsync()).Should().Be(1);
        await this.RoleRow(renamed).Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        // the role of the provider is deleted
        await this.ProviderRoleRow(renamed).Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        (await this.ProviderRoleRow(renamed).CountAsync()).Should().Be(0);

        // the new button
        await this.ClickAndWaitAsync(this.Control("NewGroup").Filter(new LocatorFilterOptions { Visible = true }).First);
        this.Page.Url.Should().Contain("Admin/EditGroup");
    }

    private ILocator ProviderRoleRow(string name)
    {
        return this.Page.Locator("li", new PageLocatorOptions { HasTextString = name })
            .Filter(new LocatorFilterOptions { Has = this.Page.Locator("[id*='_RoleListNet_']") });
    }

    private Dictionary<string, string> RoleValues(string name)
    {
        return new Dictionary<string, string>
                   {
                       ["Name"] = name, ["Priority"] = "5", ["PMLimit"] = "10", ["UsrSigChars"] = "100", ["UsrAlbums"] = "0",
                       ["UsrAlbumImages"] = "0"
                   };
    }

    private ILocator RoleRow(string name)
    {
        return this.Page.Locator("li", new PageLocatorOptions { HasTextString = name })
            .Filter(new LocatorFilterOptions { Has = this.Page.Locator("[id*='_RoleListYaf_']") });
    }

    private async Task SaveRoleAsync(string url, Dictionary<string, string> values)
    {
        await this.GotoAsync(url.StartsWith("http") ? new Uri(url).PathAndQuery.TrimStart('/') : url);

        await this.Page.EvaluateAsync(
            """
            values => {
                for (const [id, value] of Object.entries(values)) {
                    const e = document.querySelector(`[id$='_${id}']`);
                    e.type = 'text';
                    e.value = value;
                }
            }
            """,
            values);

        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    private async Task SetRoleAsync(int userId, string role, bool member)
    {
        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a[href='#View2']").ClickAsync();

        await this.Page.Locator(".col-md-4", new PageLocatorOptions { HasTextString = role }).Locator("input[type='checkbox']")
            .SetCheckedAsync(member);
        await this.ClickAndWaitAsync(this.Control("GroupEditControl_Save"));
    }
}
