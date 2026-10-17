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
/// Admin > Event Log: an error with its details is shown, filtered and deleted.
/// </summary>
[TestFixture]
public class EventLogTests : ForumTestBase
{
    [Test]
    [Description("The error of a refused post is shown with its details (url, address, exception), filtered by type and deleted")]
    public async Task Error_Entry_Test()
    {
        await this.LoginAsAdminAsync();

        // the post of a button that is not on the page (the avatar upload is turned off) is an error
        await this.GotoAsync("Profile/EditAvatar");
        var back = await this.Control("Back").GetAttributeAsync("name");
        var prefix = back.Substring(0, back.Length - "Back".Length);

        await this.Page.EvaluateAsync(
            @"prefix => {
                const link = document.createElement('a');
                link.id = 'madeUpPost';
                link.href = `javascript:__doPostBack('${prefix}UpdateUpload','')`;
                link.textContent = 'upload';
                document.forms[0].appendChild(link);
            }",
            prefix);

        var response = await this.Page.RunAndWaitForResponseAsync(
            () => this.Page.Locator("#madeUpPost").ClickAsync(),
            r => r.Request.Method == "POST");
        response.Status.Should().Be(500);
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        this.ClearServerErrors();

        // the errors
        await this.GotoAsync("Admin/EventLog");
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("Types").SelectOptionAsync(
            new SelectOptionValue { Label = "Error" },
            new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("ApplyButton"));

        var entries = this.Page.Locator("li.list-group-item", new PageLocatorOptions { HasTextString = "EditAvatar" });
        (await entries.CountAsync()).Should().BeGreaterThan(0);

        // the details are collapsed
        var entry = await entries.First.TextContentAsync();
        entry.Should().Contain("HttpUnhandledException").And.Contain("EditAvatar");

        // the entry is deleted
        var before = await entries.CountAsync();
        await entries.First.Locator("a[data-bs-toggle='confirm']").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await this.Page.Locator("li.list-group-item", new PageLocatorOptions { HasTextString = "EditAvatar" }).CountAsync())
            .Should().BeLessThan(before);
    }
}
