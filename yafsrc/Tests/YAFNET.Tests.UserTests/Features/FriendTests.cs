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

namespace YAF.Tests.UserTests.Features;

/// <summary>
/// The user friend tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class FriendTests : TestBase
{
    /// <summary>
    /// Request Add Friend Test
    /// Login as Admin and add the TestUser as Friend
    /// </summary>
    [Test, Order(1)]
    public async Task AddRequestFriendTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.AdminUserName,
                            this.Base.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    // Go to Members Page and Find the Test User
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Members");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Filter"), "Members List View Permissions needs to be enabled");

                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

                    await page.Locator("//input[contains(@id,'_UserSearchName')]")
                        .FillAsync(this.Base.TestSettings.TestUserName);

                    await page.Locator("//button[contains(@formaction,'Members/Search')]").ClickAsync();

                    var userProfileLink = page.GetByText(this.Base.TestSettings.TestUserName).First;

                    Assert.That(userProfileLink, Is.Not.Null, "User Profile Not Found");

                    await userProfileLink.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource.Contains("Remove Friend"), Is.False, "User is already a Friend");

                    Assert.That(
                        pageSource, Does.Contain("Add as Friend"),
                        "My Friends function is not available for that User, or is disabled for that Forum");

                    await page.Locator("//a[contains(@href,'AddFriendRequest')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Friend request sent."));
                },
            this.BrowserType);
    }

    /// <summary>
    /// Approve the Friend request test.
    /// </summary>
    [Test, Order(2)]
    public async Task ApproveFriendRequestTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("New Friend request"))
                    {
                        await page.Locator(".bootbox-accept").ClickAsync();
                    }
                    else
                    {
                        await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyFriends");
                    }

                    Assert.That(
                        pageSource, Does.Contain("Friend List"),
                        "My Friends function is not available for that User, or is disabled for that Forum");

                    await page.GetByRole(AriaRole.Combobox, new PageGetByRoleOptions { Name = "friend mode" })
                        .SelectOptionAsync(["3"]);

                    // Select the First Request
                    await page.Locator("//button[contains(@formaction,'Approve')]").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You have been added to "), "Approve Friend Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Deny the Newest Friend Request test.
    /// </summary>
    [Test]
    public async Task DenyFriendRequestTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("New Friend request"))
                    {
                        await page.Locator(".bootbox-accept").ClickAsync();
                    }
                    else
                    {
                        await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyFriends");
                    }

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Friend List"),
                        "My Friends function is not available for that User, or is disabled for that Forum");

                    await page.GetByRole(AriaRole.Combobox, new PageGetByRoleOptions { Name = "friend mode" })
                        .SelectOptionAsync(["3"]);

                    // Select the First Request
                    await page.Locator("//button[contains(@formaction,'Deny')]").First.ClickAsync();

                    await page.GetByText("Yes").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Friend request denied."), "Deny Request Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Approve and add Friend request test.
    /// </summary>
    [Test]
    public async Task ApproveAndAddFriendRequestTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("New Friend request"))
                    {
                        await page.Locator(".bootbox-accept").ClickAsync();
                    }
                    else
                    {
                        await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyFriends");
                    }

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Friend List"),
                        "My Friends function is not available for that User, or is disabled for that Forum");

                    await page.GetByRole(AriaRole.Combobox, new PageGetByRoleOptions { Name = "friend mode" })
                        .SelectOptionAsync(["3"]);

                    // Select the First Request
                    await page.Locator("//button[contains(@formaction,'ApproveAdd')]").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You and "), "Approve and Add Friend Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Remove a Friend test.
    /// </summary>
    [Test, Order(3)]
    public async Task RemoveFriendTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyFriends");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Friend List"),
                        "My Friends function is not available for that User, or is disabled for that Forum");

                    // Select the Newest Friend
                    var delete = page.Locator("//button[contains(@formaction,'Remove')]");

                    Assert.That(delete, Is.Not.Null, "Currently the Test User doesn't have any Friends");

                    await delete.ClickAsync();

                    await page.Locator(".btn-success").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("has been removed from your Friend list."));
                },
            this.BrowserType);
    }
}