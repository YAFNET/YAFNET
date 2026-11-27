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

using System.Data.SQLite;
using System.Net.Http.Json;

using YAF.Core.Identity;
using YAF.Tests.HostIntegration.TestInfrastructure;

/// <summary>
/// The forum inside of a web application, with the tests of every identity setup.
/// </summary>
public abstract class HostIntegrationTests(HostIdentityMode mode)
{
    /// <summary>
    /// Gets the running application with the installed forum.
    /// </summary>
    protected HostApplication Host { get; private set; }

    /// <summary>
    /// Gets the name of the user store the forum has to use.
    /// </summary>
    private string UserStoreName =>
        mode == HostIdentityMode.HostWithOwnTypes ? nameof(ForumUserStoreAdapter) : nameof(UserStore);

    [OneTimeSetUp]
    public async Task StartHost()
    {
        this.Host = await HostApplication.StartAsync(mode);

        using var browser = this.Host.CreateBrowser();

        using var response = await browser.PostAsync("/host/Install", null);

        await ShouldBeOkAsync(response);
    }

    [OneTimeTearDown]
    public async Task StopHost()
    {
        if (this.Host != null)
        {
            await this.Host.DisposeAsync();
        }
    }

    [Test]
    public async Task Guest_Opens_The_Board()
    {
        using var browser = this.Host.CreateBrowser();

        using var response = await browser.GetAsync("/");

        (await ShouldBeOkAsync(response)).Should().Contain(HostApplication.ForumName);
    }

    [Test]
    public async Task Signed_In_Admin_Opens_The_Admin_Pages()
    {
        using var browser = await this.SignInAsync(HostApplication.AdminName);

        var adminId = this.ScalarOfForumDatabase("select UserID from yaf_User where Name = 'HostAdmin'");

        await PagesShouldShowAsync(
            browser,
            [
                "/", "/Admin/Admin", "/Admin/Users", $"/Admin/EditUser/{adminId}", "/Admin/Groups", "/members",
                "/MyAccount", $"/user/{adminId}/hostadmin"
            ],
            HostApplication.AdminName);
    }

    [Test]
    public async Task Registered_User_Opens_The_Board()
    {
        await this.RegisterAsync("Member");

        using var browser = await this.SignInAsync("Member");

        var identity = await browser.GetFromJsonAsync<ForumIdentity>("/host/whoami");

        identity.Should().Be(new ForumIdentity("Member", "Member", false, false, this.UserStoreName));

        await PagesShouldShowAsync(browser, ["/", "/members", "/MyAccount", "/Profile/EditProfile"], "Member");
    }

    [Test]
    public async Task Forum_Knows_The_Signed_In_Admin()
    {
        using var browser = await this.SignInAsync(HostApplication.AdminName);

        var identity = await browser.GetFromJsonAsync<ForumIdentity>("/host/whoami");

        identity.Should().Be(
            new ForumIdentity(HostApplication.AdminName, HostApplication.AdminName, false, true, this.UserStoreName));
    }

    [Test]
    public async Task Forum_Knows_The_Guest()
    {
        using var browser = this.Host.CreateBrowser();

        var identity = await browser.GetFromJsonAsync<ForumIdentity>("/host/whoami");

        identity.Should().Be(new ForumIdentity(null, "Guest", true, false, this.UserStoreName));
    }

    [Test]
    public void Identities_Are_In_The_Stores_Of_The_Application()
    {
        var forumRows = this.ScalarOfForumDatabase(
            """
            select (select count(*) from yaf_AspNetUsers) + (select count(*) from yaf_AspNetRoles)
                 + (select count(*) from yaf_AspNetUserRoles) + (select count(*) from yaf_AspNetUserLogins)
            """);

        if (mode == HostIdentityMode.HostWithOwnTypes)
        {
            this.Host.HostUserNames.Should().Contain(HostApplication.AdminName);
            this.Host.HostRoleNames.Should().Contain(["Administrators", "Registered Users"]);

            forumRows.Should().Be(0);
        }
        else
        {
            this.Host.HostUserNames.Should().BeEmpty();

            this.ScalarOfForumDatabase("select count(*) from yaf_AspNetUsers where UserName = 'HostAdmin'")
                .Should().Be(1);
        }
    }

    protected async static Task<string> ShouldBeOkAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(
            HttpStatusCode.OK,
            $"{response.RequestMessage?.RequestUri} answered with {response.Headers.Location} {Shorten(body)}");

        return body;
    }

    protected async static Task PagesShouldShowAsync(HttpClient browser, IEnumerable<string> paths, string userName)
    {
        foreach (var path in paths)
        {
            using var response = await browser.GetAsync(path);

            (await ShouldBeOkAsync(response)).Should().Contain(userName, $"{path} is rendered for {userName}");
        }
    }

    protected async Task<HttpClient> SignInAsync(string name)
    {
        var browser = this.Host.CreateBrowser();

        using var response = await browser.PostAsync($"/host/login?name={name}", null);

        await ShouldBeOkAsync(response);

        return browser;
    }

    protected async Task RegisterAsync(string name)
    {
        using var browser = this.Host.CreateBrowser();

        using var response = await browser.PostAsync($"/host/register?name={name}", null);

        await ShouldBeOkAsync(response);
    }

    protected long ScalarOfForumDatabase(string sql)
    {
        using var connection = new SQLiteConnection($"Data Source={this.Host.DatabaseFile};Version=3;");

        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = sql;

        return Convert.ToInt64(command.ExecuteScalar());
    }

    private static string Shorten(string body)
    {
        return body.Length > 4000 ? body[..4000] : body;
    }
}
