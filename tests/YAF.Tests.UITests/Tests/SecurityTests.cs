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

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Access checks of pages that take ids or addresses from the url.
/// </summary>
[TestFixture]
public class SecurityTests : ForumTestBase
{
    [Test]
    [Description("A member can not open the edit page of a post of another user")]
    public async Task Edit_Other_Users_Post_Is_Denied_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        // the first post of the forum is a post of the admin
        await this.GotoAsync("EditMessage?m=1");

        this.Page.Url.Should().NotContain("EditMessage");
    }

    [Test]
    [Description("A form of another web site can not call the api with the login cookie of the user, the json requests of the forum pass")]
    public async Task Api_Refuses_Cross_Site_Form_Post_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        var forged = await this.Page.APIRequest.PostAsync(
            $"{TestConfig.BaseUrl}api/ThankYou/AddThanks/1",
            new APIRequestContextOptions { Form = this.Page.APIRequest.CreateFormData() });

        forged.Status.Should().Be(403);

        var json = await this.Page.APIRequest.PostAsync(
            $"{TestConfig.BaseUrl}api/ThankYou/GetThanks/1",
            new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json;charset=utf-8" },
                Data = "{}"
            });

        json.Status.Should().NotBe(403);
    }

    [Test]
    [Description("A member can not open the delete page of a post of another user")]
    public async Task Delete_Other_Users_Post_Is_Denied_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("DeleteMessage?m=1&action=delete");

        this.Page.Url.Should().NotContain("DeleteMessage");
    }

    [Test]
    [Description("Guests can not open the pages of members")]
    public async Task Guest_Can_Not_Open_Member_Pages_Test()
    {
        await this.LogoutAsync();

        foreach (var url in new[] { "PostPrivateMessage", "MyMessages", "Profile/EditProfile", "MyAccount" })
        {
            await this.GotoAsync(url);

            this.Page.Url.Should().NotEndWith(url, $"{url} is refused to guests");
        }
    }

    [TestCase("javascript:alert(1)")]
    [TestCase("//evil.example.com/")]
    [TestCase("https://evil.example.com/")]
    [Description("The continue link of the info page only goes to addresses of the forum")]
    public async Task Info_Page_Continue_Link_Test(string url)
    {
        await this.LogoutAsync();

        await this.GotoAsync($"Info?i=1&url={System.Uri.EscapeDataString(url)}");

        var links = await this.Page.Locator("a[href]").EvaluateAllAsync<string[]>("es => es.map(e => e.getAttribute('href'))");

        links.Should().NotContain(url);
    }

}
