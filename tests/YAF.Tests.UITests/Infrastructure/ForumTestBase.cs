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

namespace YAF.Tests.UITests.Infrastructure;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using NUnit.Framework.Interfaces;

/// <summary>
/// The base of the UI test fixtures: every fixture gets its own browser context (cookies, session) and page.
/// The server errors of the pages are collected, a failed test gets a screenshot.
/// </summary>
[NonParallelizable]
public abstract class ForumTestBase
{
    /// <summary>
    /// Gets the browser context of the fixture.
    /// </summary>
    protected IBrowserContext Context { get; private set; }

    /// <summary>
    /// Gets the page of the fixture.
    /// </summary>
    protected IPage Page { get; private set; }

    /// <summary>
    /// Gets the server errors (status 500) of the requests of the current test.
    /// </summary>
    protected List<string> ServerErrors { get; } = [];

    /// <summary>
    /// Gets the JavaScript errors of the pages of the current test.
    /// </summary>
    protected List<string> BrowserErrors { get; } = [];

    [OneTimeSetUp]
    public async Task CreateContextAsync()
    {
        this.Context = await UiTestSetup.Browser.NewContextAsync(
                           new BrowserNewContextOptions
                               {
                                   BaseURL = TestConfig.BaseUrl,
                                   ViewportSize = new ViewportSize { Width = 1280, Height = 900 }
                               });

        this.Context.SetDefaultTimeout(30000);
        this.Context.SetDefaultNavigationTimeout(120000);

        this.Context.Response += (_, response) =>
            {
                if (response.Status >= 500)
                {
                    this.ServerErrors.Add($"{response.Status} {response.Request.Method} {response.Url}");
                }
            };

        await this.AcceptCookiesAsync();

        this.Page = await this.Context.NewPageAsync();

        this.Page.PageError += (_, error) =>
            {
                TestContext.Progress.WriteLine($"[browser-error] {this.Page.Url}: {error}");
                this.BrowserErrors.Add($"{this.Page.Url}: {error}");
            };
    }

    [SetUp]
    public void ClearServerErrors()
    {
        this.ServerErrors.Clear();
        this.BrowserErrors.Clear();
    }

    [TearDown]
    public async Task ScreenshotOnFailureAsync()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed || this.Page == null)
        {
            return;
        }

        var folder = Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshots");
        Directory.CreateDirectory(folder);

        var name = string.Join("_", TestContext.CurrentContext.Test.Name.Split(Path.GetInvalidFileNameChars()));
        var file = Path.Combine(folder, $"{name}.png");

        try
        {
            await this.Page.ScreenshotAsync(new PageScreenshotOptions { Path = file, FullPage = true });
            TestContext.AddTestAttachment(file);
        }
        catch (PlaywrightException)
        {
            // the page is closed
        }
    }

    [OneTimeTearDown]
    public async Task CloseContextAsync()
    {
        if (this.Context != null)
        {
            await this.Context.CloseAsync();
        }
    }

    /// <summary>
    /// Opens a page of the forum and checks that the server had no error.
    /// </summary>
    /// <param name="url">The address (relative to the forum).</param>
    /// <returns>Returns the response.</returns>
    protected async Task<IResponse> GotoAsync(string url)
    {
        var response = await this.Page.GotoAsync(url);

        await this.AssertNoServerErrorAsync();

        return response;
    }

    /// <summary>
    /// Clicks a button or link that posts the page back (the WebForms buttons use __doPostBack) and waits for the
    /// new page.
    /// </summary>
    /// <param name="selector">The selector of the button.</param>
    protected async Task ClickAndWaitAsync(string selector)
    {
        await this.ClickAndWaitAsync(this.Page.Locator(selector).First);
    }

    /// <summary>
    /// Clicks a button or link that posts the page back and waits for the new page.
    /// </summary>
    /// <param name="button">The button.</param>
    protected async Task ClickAndWaitAsync(ILocator button)
    {
        await this.Page.RunAndWaitForNavigationAsync(() => button.ClickAsync());

        await this.AssertNoServerErrorAsync();
    }

    /// <summary>
    /// Gets a control of the page by its server id (the client id of a WebForms control ends with it).
    /// </summary>
    /// <param name="id">The id of the control.</param>
    /// <returns>Returns the locator.</returns>
    protected ILocator Control(string id)
    {
        return this.Page.Locator($"[id$='_{id}'], [id='{id}']");
    }

    /// <summary>
    /// Logs in with the login page (a logged in user is logged out first).
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <param name="password">The password.</param>
    protected async Task LoginAsync(string userName, string password)
    {
        await this.LogoutAsync();

        await this.GotoAsync("Account/Login");

        await this.Control("UserName").FillAsync(userName);
        await this.Control("Password").FillAsync(password);

        await this.ClickAndWaitAsync(this.Control("LoginButton"));
    }

    /// <summary>
    /// Logs in as the admin of the forum.
    /// </summary>
    protected Task LoginAsAdminAsync()
    {
        return this.LoginAsync(TestConfig.AdminUserName, TestConfig.AdminPassword);
    }

    /// <summary>
    /// Gets the e-mail address of a test user.
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <returns>Returns the address.</returns>
    protected static string EmailOf(string userName)
    {
        return $"{userName.ToLowerInvariant()}@localhost.com";
    }

    /// <summary>
    /// Registers a user with the register page and confirms the e-mail address with the link of the
    /// verification mail (the user is logged in afterwards).
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <param name="password">The password.</param>
    protected async Task RegisterAsync(string userName, string password)
    {
        await this.LogoutAsync();

        var started = DateTime.UtcNow;

        await this.GotoAsync("Account/Register");

        await this.Control("UserName").FillAsync(userName);
        await this.Control("Email").FillAsync(EmailOf(userName));
        await this.Control("Password").FillAsync(password);
        await this.Control("ConfirmPassword").FillAsync(password);

        await this.ClickAndWaitAsync(this.Control("CreateUser"));

        var mail = await TestMail.WaitForAsync(EmailOf(userName), started);

        await this.GotoAsync(mail.GetLink("Approve"));

        lock (RegisteredUsers)
        {
            RegisteredUsers.Add(userName);
        }
    }

    /// <summary>
    /// Registers the user once for all tests and logs in with it.
    /// </summary>
    /// <param name="userName">The user name.</param>
    protected async Task LoginAsUserAsync(string userName)
    {
        bool registered;

        lock (RegisteredUsers)
        {
            registered = RegisteredUsers.Contains(userName);
        }

        if (!registered)
        {
            await this.RegisterAsync(userName, TestConfig.TestUserPassword);
        }

        await this.LoginAsync(userName, TestConfig.TestUserPassword);
    }

    /// <summary>
    /// The users registered by the tests of this run.
    /// </summary>
    private static readonly HashSet<string> RegisteredUsers = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a value indicating whether a user is logged in (the user menu is shown).
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <returns>Returns if the user is logged in.</returns>
    protected async Task<bool> IsLoggedInAsAsync(string userName)
    {
        var userMenu = this.Page.Locator("[id$='_UserDropDown']");

        // the name and the number of the unread messages and notifications
        return await userMenu.CountAsync() > 0 && Regex.IsMatch(
                   (await userMenu.First.InnerTextAsync()).Trim(),
                   $@"^{Regex.Escape(userName)}(\s+\d+)?$",
                   RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Opens the admin edit page of a user (the logged in user is an admin), the user is found with the name filter
    /// of the users list (the list has many pages after a while).
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <returns>Returns the id of the user.</returns>
    protected async Task<int> OpenAdminEditUserAsync(string userName)
    {
        await this.GotoAsync("Admin/Users");

        // the filter is in a drop down
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("name").FillAsync(userName);
        await this.ClickAndWaitAsync(this.Control("search"));

        // the name posts back and opens the edit page
        await this.ClickAndWaitAsync(
            this.Page.Locator("[id$='_NameEdit']", new PageLocatorOptions { HasTextString = userName }).First);

        return int.Parse(Regex.Match(this.Page.Url, @"u=(\d+)").Groups[1].Value);
    }

    /// <summary>
    /// Gets the id of the last forum with the name from the database (the board index lists the first 20 forums
    /// only, the forums of a full run are not all on it).
    /// </summary>
    /// <param name="name">The name of the forum.</param>
    /// <returns>Returns the id of the forum.</returns>
    protected static string ForumId(string name)
    {
        using var connection = new SqlConnection(TestSite.ConnectionString);
        connection.Open();

        using var command = new SqlCommand(
            "SELECT TOP 1 [ForumID] FROM [dbo].[yaf_Forum] WHERE [Name] = @name ORDER BY [ForumID] DESC",
            connection);
        command.Parameters.AddWithValue("@name", name);

        var id = command.ExecuteScalar();

        id.Should().NotBeNull($"the forum {name} exists");

        return id.ToString();
    }

    /// <summary>
    /// Logs out (the login cookie is removed).
    /// </summary>
    protected async Task LogoutAsync()
    {
        await this.Context.ClearCookiesAsync();

        await this.AcceptCookiesAsync();
    }

    /// <summary>
    /// The cookie notice is accepted (it covers the buttons at the bottom of the pages).
    /// </summary>
    private Task AcceptCookiesAsync()
    {
        return this.Context.AddCookiesAsync(
            [new Cookie { Name = "YAF-AcceptCookies", Value = "true", Url = TestConfig.BaseUrl }]);
    }

    /// <summary>
    /// Checks that the requests of the page had no server error and the page is not an error page.
    /// </summary>
    protected async Task AssertNoServerErrorAsync()
    {
        this.ServerErrors.Should().BeEmpty();
        this.BrowserErrors.Should().BeEmpty();

        var content = await this.Page.ContentAsync();

        content.Should().NotContain("Server Error in '/' Application");
    }
}
