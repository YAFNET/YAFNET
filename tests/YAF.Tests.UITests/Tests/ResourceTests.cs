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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The resources of the forum (resource.ashx): the user card info and the text avatar.
/// </summary>
[TestFixture]
public class ResourceTests : ForumTestBase
{
    [Test]
    [Description("The user card has the encoded name, the location and the online state of the user, an unknown user is an error text")]
    public async Task User_Info_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        var userId = await this.OwnUserIdAsync();

        var response = await this.Page.APIRequest.GetAsync($"resource.ashx?userinfo={userId}&boardId=1&type=json");
        response.Status.Should().Be(200);
        response.Headers["content-type"].Should().StartWith("application/json");

        var json = await response.TextAsync();
        json.Should().Contain(TestConfig.TestUserName);
        json.ToLowerInvariant().Should().Contain("\"online\"");

        var unknown = await this.Page.APIRequest.GetAsync("resource.ashx?userinfo=999999999&boardId=1&type=json");
        (await unknown.TextAsync()).Should().StartWith("Error:");
    }

    [Test]
    [Description("The text avatar of a user is an svg image with the letters of the name, an unknown id is an error text")]
    public async Task Text_Avatar_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        var userId = await this.OwnUserIdAsync();

        var response = await this.Page.APIRequest.GetAsync($"resource.ashx?avatar={userId}");
        response.Status.Should().Be(200);
        response.Headers["content-type"].Should().StartWith("image/svg+xml");
        (await response.TextAsync()).Should().Contain("<svg");

        var invalid = await this.Page.APIRequest.GetAsync("resource.ashx?avatar=abc");
        (await invalid.TextAsync()).Should().StartWith("Error:");
    }

    [Test]
    [Description("The list of the custom bb codes (for the editor) is json without the album image and attachment codes")]
    public async Task BBCode_List_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync(string.Empty);

        var response = await this.Page.APIRequest.GetAsync("resource.ashx?bbcodelist=1");
        response.Headers["content-type"].Should().StartWith("application/json");

        var json = await response.TextAsync();
        json.Should().StartWith("[").And.Contain("\"name\"").And.NotContain("\"ATTACH\"").And.NotContain("\"ALBUMIMG\"");
    }

    [Test]
    [Description("The avatar image of a user without an image in the database is not found, an invalid id is empty")]
    public async Task Database_Avatar_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        var userId = await this.OwnUserIdAsync();

        var response = await this.Page.APIRequest.GetAsync($"resource.ashx?u={userId}");
        response.Status.Should().Be(404);

        var invalid = await this.Page.APIRequest.GetAsync("resource.ashx?u=abc");
        (await invalid.BodyAsync()).Should().BeEmpty();
    }

    [Test]
    [Description("The user card shows the region of the country of the user")]
    public async Task User_Info_Region_Test()
    {
        var member = $"Rgn{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/EditProfile");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("ProfileEditor_Country").SelectOptionAsync(
                "US",
                new Microsoft.Playwright.LocatorSelectOptionOptions { Force = true }));
        await this.Control("ProfileEditor_Region").SelectOptionAsync(
            "CA",
            new Microsoft.Playwright.LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        var userId = await this.OwnUserIdAsync();

        var response = await this.Page.APIRequest.GetAsync($"resource.ashx?userinfo={userId}&boardId=1&type=json");
        (await response.TextAsync()).Should().Contain("California");
    }

    private async Task<int> OwnUserIdAsync()
    {
        await this.GotoAsync(string.Empty);

        var href = await this.Page.Locator("a[href*='UserProfile']").First.GetAttributeAsync("href");

        return int.Parse(Regex.Match(href, @"UserProfile/(\d+)").Groups[1].Value);
    }
}
