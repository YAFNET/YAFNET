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

namespace YAF.Tests.Utils.Extensions;

using System.Linq;

using Microsoft.Playwright;

using netDumbster.smtp;

/// <summary>
/// Test Helper Class
/// </summary>
public static class RegisterLoginExtensions
{
    /// <param name="page">
    /// The Page Instance
    /// </param>
    extension(IPage page)
    {
        /// <summary>
        /// Log Current user Out
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public async Task<bool> LogOutUserAsync()
        {
            await page.Locator(".nav-link-user-dropdown").ClickAsync();
            await page.Locator("//a[contains(@aria-label, 'sign-out-alt')]").ClickAsync();

            await page.Locator(".bootbox-accept").ClickAsync();

            return await page.Locator("#navbarSupportedContent").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Login" }).IsVisibleAsync();
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
            return RegisterUserAsync(
                page,
                testSettings,
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

            await page.Locator("#Login").ClickAsync();

            return await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync();
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

            var confirmEmail = simpleSmtpServer.ReceivedEmail.FirstOrDefault();

            if (confirmEmail is null)
            {
                return false;
            }

            var confirmUrl = StringUtils.GetLinks(confirmEmail.MessageParts[0].BodyData).FirstOrDefault();

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