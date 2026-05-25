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

using YAF.Tests.Infrastructure;

namespace YAF.Tests.UserTests.Features;

/// <summary>
/// The user friend tests.
/// </summary>
public class FriendTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Approve the Friend request test.
    /// </summary>
    [Test]
    public async Task ApproveFriendRequestTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Add Friend Request first
                    await page.AddFriendRequestAsync(this.TestSettings);

                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("New Friend request"))
                    {
                        await page.Locator(".bootbox-accept").ClickAsync();
                    }
                    else
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyFriends");
                    }

                    Assert.That(
                        pageSource, Does.Contain("Friend List"),
                        "My Friends function is not available for that User, or is disabled for that Forum");

                    await page.GetByRole(AriaRole.Combobox, new PageGetByRoleOptions { Name = "friend mode" })
                        .SelectOptionAsync(["3"]);

                    // Select the First Request
                    await page.Locator("//button[contains(@formaction,'Approve')]").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You and "), "Approve Friend Failed");

                    // Remove Friend Request
                    await page.RemoveFriendRequestAsync(this.TestSettings, this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword);
                    await page.RemoveFriendRequestAsync(this.TestSettings, this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword);
                });
    }

    /// <summary>
    /// Deny the Newest Friend Request test.
    /// </summary>
    [Test]
    public async Task DenyFriendRequestTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Add Friend Request first
                    await page.AddFriendRequestAsync(this.TestSettings);

                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("New Friend request"))
                    {
                        await page.Locator(".bootbox-accept").ClickAsync();
                    }
                    else
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyFriends");
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
                });
    }

    /// <summary>
    /// Approve and add Friend request test.
    /// </summary>
    [Test]
    public async Task ApproveAndAddFriendRequestTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Add Friend Request first
                    await page.AddFriendRequestAsync(this.TestSettings);

                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("New Friend request"))
                    {
                        await page.Locator(".bootbox-accept").ClickAsync();
                    }
                    else
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyFriends");
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

                    // Remove Friend Request
                    await page.RemoveFriendRequestAsync(this.TestSettings, this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword);
                    await page.RemoveFriendRequestAsync(this.TestSettings, this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword);
                });
    }
}