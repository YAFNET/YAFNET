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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.HostIntegration;

using System.Net.Http.Json;
using System.Text.RegularExpressions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using YAF.Tests.HostIntegration.TestInfrastructure;

/// <summary>
/// The forum inside of an application that manages the accounts with its own ASP.NET Core Identity.
/// </summary>
[TestFixture(HostIdentityMode.HostWithForumTypes)]
[TestFixture(HostIdentityMode.HostWithOwnTypes)]
[NonParallelizable]
public class HostOwnedIdentityTests(HostIdentityMode mode) : HostIntegrationTests(mode)
{
    [Test]
    public async Task Forum_Links_To_The_Login_Of_The_Host()
    {
        using var browser = this.Host.CreateBrowser();

        using var response = await browser.GetAsync("/");

        (await ShouldBeOkAsync(response)).Should().Contain("href=\"/Account/Login?ReturnUrl=%2F\"");
    }

    [Test]
    public async Task Forum_Has_No_Login_Box()
    {
        using var browser = this.Host.CreateBrowser();

        using var index = await browser.GetAsync("/");

        using var response = await browser.PostAsync(
            "/api/LoginBox/SignIn",
            new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["UserName"] = HostApplication.AdminName,
                    ["Password"] = HostApplication.Password,
                    ["__RequestVerificationToken"] = ReadAntiForgeryToken(await ShouldBeOkAsync(index))
                }));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var identity = await browser.GetFromJsonAsync<ForumIdentity>("/host/whoami");

        identity.IsGuest.Should().BeTrue();
    }

    [Test]
    public async Task Forum_Does_Not_Change_Passwords()
    {
        using var browser = await this.SignInAsync(HostApplication.AdminName);

        var adminId = this.ScalarOfForumDatabase("select UserID from yaf_User where Name = 'HostAdmin'");

        foreach (var path in new[] { "/Profile/ChangePassword", $"/Admin/EditUser/UsersChangePass?userId={adminId}" })
        {
            using var response = await browser.GetAsync(path);

            response.StatusCode.Should().Be(HttpStatusCode.Redirect, $"{path} is not available");
            response.Headers.Location?.OriginalString.Should().Contain("/Info/", $"{path} is denied");
        }
    }

    [Test]
    public async Task Roles_Of_The_Forum_Are_Roles_Of_The_Host()
    {
        await this.RegisterAsync("RoleUser");

        // the forum added the role of its group for new users
        using var browser = await this.SignInAsync("RoleUser");

        var account = await browser.GetFromJsonAsync<HostAccount>("/host/account");

        account.Should().Be(new HostAccount("RoleUser", true));
    }

    [Test]
    public void Identity_Options_Stay_Those_Of_The_Host()
    {
        var options = this.Host.App.Services.GetRequiredService<IOptions<IdentityOptions>>().Value;

        options.Password.RequiredLength.Should().Be(HostApplication.RequiredPasswordLength);
    }

    private static string ReadAntiForgeryToken(string html)
    {
        var match = Regex.Match(html, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"");

        match.Success.Should().BeTrue("the page has a form with an anti forgery token");

        return match.Groups[1].Value;
    }
}
