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

using System;

namespace YAF.Tests.Utils.Extensions;

using Microsoft.Playwright;

using netDumbster.smtp;

/// <summary>
/// Test Helper Class
/// </summary>
public static class PageExtensions
{
    /// <param name="page">
    /// The Page Instance
    /// </param>
    extension(IPage page)
    {
        public async Task RemoveFriendRequestAsync(TestConfig testSettings, string userName, string password)
        {
            // Log user in first!
            Assert.That(
                await page.LoginUserAsync(
                    testSettings,
                    userName,
                    password), Is.True,
                "Login failed");

            // Do actual test
            await page.GotoAsync($"{testSettings.TestForumUrl}MyFriends");

            var pageSource = await page.ContentAsync();

            Assert.That(
                pageSource, Does.Contain("Friend List"),
                "My Friends function is not available for that User, or is disabled for that Forum");

            // Select the Newest Friend
            var delete = page.Locator("//button[contains(@formaction,'Remove')]");

            Assert.That(delete, Is.Not.Null, "Currently the Test User doesn't have any Friends");

            await delete.ClickAsync();

            await page.SubmitAndWaitAsync(page.Locator(".btn-success"));

            pageSource = await page.ContentAsync();

            Assert.That(pageSource, Does.Contain("has been removed from your Friend list."));
        }

        /// <summary>
        /// Adds the friend.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <returns></returns>
        public async Task AddFriendRequestAsync(TestConfig testSettings)
        {
            // Log user in first!
            Assert.That(
                await page.LoginUserAsync(
                    testSettings,
                    testSettings.AdminUserName,
                    testSettings.AdminPassword), Is.True,
                "Login failed");

            // Do actual test

            // Go to Members Page and Find the Test User
            await page.GotoAsync($"{testSettings.TestForumUrl}Members");

            var pageSource = await page.ContentAsync();

            Assert.That(pageSource, Does.Contain("Filter"), "Members List View Permissions needs to be enabled");

            await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

            await page.Locator("//input[contains(@id,'_UserSearchName')]")
                .FillAsync(testSettings.TestUserName);

            await page.Locator("//button[contains(@formaction,'Members/Search')]").ClickAsync();

            var userProfileLink = page.GetByText(testSettings.TestUserName).First;

            Assert.That(userProfileLink, Is.Not.Null, "User Profile Not Found");

            await userProfileLink.ClickAsync();

            pageSource = await page.ContentAsync();

            Assert.That(pageSource.Contains("Remove Friend"), Is.False, "User is already a Friend");

            await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'AddFriendRequest')]"));

            pageSource = await page.ContentAsync();

            Assert.That(pageSource, Does.Contain("Friend request sent."));
        }

        /// <summary>
        /// Updates the email settings asynchronous.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <returns></returns>
        public async Task<string> UpdateEmailSettingsAsync(TestConfig testSettings)
        {
            await page.GotoAsync($"{testSettings.TestForumUrl}Profile/Subscriptions");

            await page.GetByLabel("Select email notification").SelectOptionAsync(["0"]);
            await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).ClickAsync();
            await page.GotoAsync($"{testSettings.TestForumUrl}Profile/Subscriptions");

            return await page.ContentAsync();
        }

        /// <summary>
        /// Creates the new role.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>the Page source</returns>
        public async Task<string> CreateNewRole(TestConfig testSettings, string roleName)
        {
            await page.GotoAsync($"{testSettings.TestForumUrl}Admin/Groups");

            await page.Locator("//a[contains(@href,'EditGroup')]").Last.ClickAsync();

            await page.Locator("#Input_Name").FillAsync(roleName);
            await page.Locator("#Input_Description").FillAsync("Test Role for testing");

            await page.Locator("#Input_NewAccessMaskID").SelectOptionAsync(["3"]);

            // wait for the groups page (the content was read while the page was loaded)
            await page.SubmitAndWaitAsync(page.Locator("#Save"));

            return await page.ContentAsync();
        }

        /// <summary>
        /// Log Current user Out
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <summary>
        /// Turns the display names on or off (as admin, logged out afterwards).
        /// The other tests run without display names, turn them off again in a finally block.
        /// </summary>
        /// <param name="testSettings">The test settings.</param>
        /// <param name="enable">If display names are enabled.</param>
        public async Task SetDisplayNamesAsync(TestConfig testSettings, bool enable)
        {
            // e.g. a test user is still logged in after a failed assert
            await page.GotoAsync(testSettings.TestForumUrl);

            if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
            {
                await page.LogOutUserAsync();
            }

            Assert.That(
                await page.LoginUserAsync(testSettings, testSettings.AdminUserName, testSettings.AdminPassword),
                Is.True,
                "Login failed");

            await page.GotoAsync($"{testSettings.TestForumUrl}Admin/HostSettings/Features");

            await page.Locator("#Input_EnableDisplayName").SetCheckedAsync(enable);

            await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']:visible").First);

            Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
        }

        public async Task<bool> LogOutUserAsync()
        {
            await page.Locator(".nav-link-user-dropdown").ClickAsync();
            await page.Locator("//a[contains(@aria-label, 'sign-out-alt')]").ClickAsync();

            await page.SubmitAndWaitAsync(page.Locator(".bootbox-accept"));

            return await page.Locator("#navbarSupportedContent").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Login" }).IsVisibleAsync();
        }

        public Task<bool> RegisterRandomTestUserAsync(TestConfig testSettings,
            SimpleSmtpServer simpleSmtpServer)
        {
            // Create New Random Test User (not "TestUser…", other tests look up TestUser1 and TestUser2 by name)
            var userName = $"TempUser{Random.Shared.Next()}";

            return page.RegisterUserAsync(
                            testSettings,
                            simpleSmtpServer,
                            userName,
                            testSettings.TestUserPassword);
        }

        /// <summary>
        /// Clicks a submit button and waits until the next page is completely loaded.
        /// (The page content read right after a click can still be the half loaded new page.)
        /// </summary>
        /// <param name="button">The button.</param>
        public Task SubmitAndWaitAsync(ILocator button)
        {
            return page.RunAndWaitForNextPageAsync(() => button.ClickAsync());
        }

        /// <summary>
        /// Runs an action that loads a new page (a click, a select that sends its form, ...)
        /// and waits until the new page is completely loaded.
        /// </summary>
        /// <param name="action">The action.</param>
        public async Task RunAndWaitForNextPageAsync(Func<Task> action)
        {
            // a page that is still loading (e.g. from the step before) can not be marked
            await page.WaitForLoadStateAsync();

            // mark the current document, the mark is gone on the new page
            await page.EvaluateAsync("() => window.yafTestOldPage = true");

            await action();

            await page.WaitForFunctionAsync("() => !window.yafTestOldPage && document.readyState === 'complete'");
        }

        /// <summary>
        /// Registers a new user with a random name, for tests that change the account in a way
        /// the other tests must not see (2FA, delete account, ...). The user is logged in afterwards.
        /// </summary>
        /// <param name="testSettings">The test Settings</param>
        /// <param name="simpleSmtpServer">The test mail server.</param>
        /// <returns>The user name, the password is the test user password.</returns>
        public async Task<string> RegisterNewTestUserAsync(TestConfig testSettings, SimpleSmtpServer simpleSmtpServer)
        {
            var userName = $"TempUser{Random.Shared.Next()}";

            Assert.That(
                await page.RegisterUserAsync(testSettings, simpleSmtpServer, userName, testSettings.TestUserPassword),
                Is.True,
                $"Registering {userName} failed");

            return userName;
        }

        /// <summary>
        /// Registers the standard test user.
        /// </summary>
        /// <param name="testSettings">
        /// The test Settings
        /// </param>
        /// <param name="simpleSmtpServer">
        /// </param>
        /// <param name="userName">
        /// Name of the user.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// If User was Registered or not
        /// </returns>
        public Task<bool> RegisterUserAsync(TestConfig testSettings,
            SimpleSmtpServer simpleSmtpServer,
            string userName,
            string password)
        {
            return page.RegisterUserAsync(testSettings,
                simpleSmtpServer,
                userName,
                $"{userName.ToLower()}@test.com",
                password);
        }

        /// <summary>
        /// Logins the admin user.
        /// </summary>
        /// <param name="testSettings">
        /// The test Settings
        /// </param>
        /// <returns>Returns if Successfully or not
        /// </returns>
        public async Task<bool> LoginAdminUserAsync(TestConfig testSettings)
        {
            await page.LogOutUserAsync();

            return await page.LoginUserAsync(testSettings, testSettings.AdminUserName, testSettings.AdminPassword);
        }

        /// <summary>
        /// Login test user
        /// </summary>
        /// <param name="testSettings">
        /// The test Settings
        /// </param>
        /// <returns>If User login was successfully or not</returns>
        public Task<bool> LoginUserAsync(TestConfig testSettings)
        {
            return page.LoginUserAsync(testSettings, testSettings.TestUserName, testSettings.TestUserPassword);
        }

        /// <summary>
        /// Logins the user.
        /// </summary>
        /// <param name="testSettings">The test Settings</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userPassword">The user password.</param>
        /// <returns>If User login was successfully or not</returns>
        public async Task<bool> LoginUserAsync(TestConfig testSettings, string userName, string userPassword)
        {
            await page.GotoAsync($"{testSettings.TestForumUrl}Account/login");

            await page.Locator("//input[contains(@id, '_UserName')]").FillAsync(userName);

            await page.Locator("//input[contains(@id, '_Password')]").FillAsync(userPassword);

            // wait for the page after the login, otherwise the menu of the user logged in before is found
            await page.SubmitAndWaitAsync(page.Locator("#Login"));

            var loggedIn = await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync();

            if (!loggedIn)
            {
                // the reason (wrong password, locked out, not approved, ...) for the test output
                var message = await page.EvaluateAsync<string>(
                    "() => [...document.querySelectorAll('.alert, [data-notify], .toast')].map(e => e.innerText.trim()).join(' | ')");

                await TestContext.Progress.WriteLineAsync($"[login] {userName} failed on {page.Url}: {message}");
            }

            return loggedIn;
        }

        /// <summary>
        /// Fills and sends the register form, without opening the verification link.
        /// Logs out the current user first.
        /// </summary>
        /// <param name="testSettings">The test Settings</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// False if registrations are disabled
        /// </returns>
        public async Task<bool> SubmitRegistrationAsync(TestConfig testSettings, string userName, string email, string password)
        {
            await page.GotoAsync($"{testSettings.TestForumUrl}Account/Register");

            if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
            {
                // Logout First
                await page.LogOutUserAsync();

                await page.GotoAsync($"{testSettings.TestForumUrl}Account/Register");
            }

            var pageSource = await page.ContentAsync();

            // Check if Registrations are Disabled
            if (pageSource.Contains("You tried to enter an area where you didn't have access"))
            {
                return false;
            }

            // Accept the Rules
            if (pageSource.Contains("Forum Rules"))
            {
                await page.ReloadAsync();
            }

            pageSource = await page.ContentAsync();

            // Fill the Register Page
            await page.Locator("//input[contains(@id, '_UserName')]").FillAsync(userName);

            if (pageSource.Contains("Display Name"))
            {
                await page.Locator("//input[contains(@id, '_DisplayName')]").FillAsync(userName);
            }

            await page.Locator("//input[contains(@id, '_Password')]").FillAsync(password);
            await page.Locator("//input[contains(@id, '_ConfirmPassword')]").FillAsync(password);
            await page.Locator("//input[contains(@id, '_Email')]").FillAsync(email);

            // Create User
            await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

            return true;
        }

        /// <summary>
        /// Registers the standard test user.
        /// </summary>
        /// <param name="testSettings">
        /// The test Settings
        /// </param>
        /// <param name="simpleSmtpServer">
        /// The simple Smtp Server.
        /// </param>
        /// <param name="userName">
        /// Name of the user.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// If User was Registered or not
        /// </returns>
        private async Task<bool> RegisterUserAsync(TestConfig testSettings, SimpleSmtpServer simpleSmtpServer, string userName, string email, string password)
        {
            if (!await page.SubmitRegistrationAsync(testSettings, userName, email, password))
            {
                return false;
            }

            var confirmUrl = await simpleSmtpServer.WaitForEmailLinkAsync(email);

            confirmUrl.Should().NotBeNull("the verification mail should arrive");

            if (confirmUrl != null)
            {
                await page.GotoAsync(confirmUrl);
            }
            else
            {
                return false;
            }

            await page.GotoAsync(testSettings.TestForumUrl);

            return await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync();
        }
    }
}