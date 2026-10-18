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
/// The access masks of the admin: create, edit and delete.
/// </summary>
[TestFixture]
public class AccessMaskTests : ForumTestBase
{
    [Test]
    [Description("An access mask is created (the sort order is checked), edited, cancel goes back, a used mask is not deleted, the new one is deleted")]
    public async Task Create_Edit_And_Delete_Test()
    {
        var id = Guid.NewGuid().ToString("N").Substring(0, 10);
        var name = $"Mask {id}";
        var renamed = $"Renamed {id}";

        await this.LoginAsAdminAsync();

        // the new button and a sort order that is no number or too big
        await this.GotoAsync("Admin/AccessMasks");
        await this.ClickAndWaitAsync(this.Control("New").Filter(new LocatorFilterOptions { Visible = true }).First);
        this.Page.Url.Should().Contain("Admin/EditAccessMask");

        foreach (var sortOrder in new[] { "abc", "99999" })
        {
            await this.Control("Name").FillAsync(name);
            await this.Control("SortOrder").EvaluateAsync("e => e.type = 'text'");
            await this.Control("SortOrder").FillAsync(sortOrder);
            await this.ClickAndWaitAsync(this.Control("Save"));

            this.Page.Url.Should().Contain("Admin/EditAccessMask", sortOrder);
        }

        await this.Control("Name").FillAsync(name);
        await this.Control("SortOrder").FillAsync("50");
        await this.Control("ReadAccess").CheckAsync();
        await this.Control("PostAccess").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("Save"));

        this.Page.Url.Should().Contain("Admin/AccessMasks");
        (await this.Row(name).CountAsync()).Should().Be(1);

        // edit
        await this.ClickAndWaitAsync(this.Row(name).Locator("[id$='_ThemeButtonEdit']"));
        (await this.Control("ReadAccess").IsCheckedAsync()).Should().BeTrue();
        (await this.Control("PostAccess").IsCheckedAsync()).Should().BeTrue();

        await this.Control("Name").FillAsync(renamed);
        await this.Control("PostAccess").UncheckAsync();
        await this.ClickAndWaitAsync(this.Control("Save"));
        (await this.Row(renamed).CountAsync()).Should().Be(1);

        // cancel
        await this.ClickAndWaitAsync(this.Row(renamed).Locator("[id$='_ThemeButtonEdit']"));
        (await this.Control("PostAccess").IsCheckedAsync()).Should().BeFalse();
        await this.ClickAndWaitAsync(this.Control("Cancel"));
        this.Page.Url.Should().Contain("Admin/AccessMasks");

        // a mask in use is not deleted
        await this.Row("Member Access").Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        (await this.Row("Member Access").CountAsync()).Should().Be(1);

        await this.Row(renamed).Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        (await this.Row(renamed).CountAsync()).Should().Be(0);

        // an unknown mask
        await this.GotoAsync("Admin/EditAccessMask?i=999999");
        this.Page.Url.Should().Contain("Info");
    }

    private ILocator Row(string name)
    {
        return this.Page.Locator("li.list-group-item")
            .Filter(new LocatorFilterOptions { Has = this.Page.Locator("h5", new PageLocatorOptions { HasTextString = name }) });
    }
}
