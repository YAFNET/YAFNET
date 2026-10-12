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

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Features;

/// <summary>
/// The private message tests.
/// </summary>
public class PrivateMessageTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The conversation of the private messages page can only be opened with a member of the board
    /// (the page shows the name, avatar and last visit of the user): not with the guest account.
    /// </summary>
    [Test]
    public async Task ConversationOnlyWithMembersTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var adminId = await this.GetAdminIdAsync(page);

                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword),
                        Is.True,
                        "Login failed");

                    // a member can be selected
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyMessages?u={adminId}");

                    Assert.That(
                        await page.Locator($"li.chat-list-user[id='{adminId}']").CountAsync(),
                        Is.EqualTo(1),
                        "The conversation with a member can not be opened");

                    // the guest account (the first user of the board) not
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyMessages?u=1");

                    Assert.That(
                        await page.Locator("li.chat-list-user[id='1']").CountAsync(),
                        Is.Zero,
                        "A conversation with the guest account is opened");
                });
    }

    /// <summary>
    /// A sent message is loaded again with the conversation, a deleted conversation is not shown anymore
    /// (both failed on PostgreSQL).
    /// </summary>
    [Test]
    public async Task ConversationIsLoadedAndDeletedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var adminId = await this.GetAdminIdAsync(page);

                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword),
                        Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyMessages?u={adminId}");

                    var text = $"Chat message {Random.Shared.Next()}";
                    var message = page.Locator(".direct-chat-text", new PageLocatorOptions { HasText = text });

                    await page.Locator("#txtPrivateMessage").FillAsync(text);
                    await page.Locator("#btnSendMessage").ClickAsync();

                    await message.WaitForAsync();

                    // the conversation is loaded from the database
                    await page.ReloadAsync();

                    await message.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

                    // deleted for the test user (the page is reloaded)
                    await page.RunAndWaitForNextPageAsync(() => page.Locator("#deleteConversation").ClickAsync());

                    await page.Locator("#divMessage").WaitForAsync();
                    await page.WaitForTimeoutAsync(2000);

                    Assert.That(await message.CountAsync(), Is.Zero, "The deleted conversation is shown");
                });
    }

    /// <summary>
    /// Gets the id of the admin (from the user list of the admin, the members list can have several pages),
    /// the admin is logged out again.
    /// </summary>
    private async Task<string> GetAdminIdAsync(IPage page)
    {
        Assert.That(
            await page.LoginUserAsync(
                this.TestSettings,
                this.TestSettings.AdminUserName,
                this.TestSettings.AdminPassword),
            Is.True,
            "Login failed");

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

        var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
        await searchNameInput.FillAsync(this.TestSettings.AdminUserName);
        await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

        var adminLink = await page.Locator(
                            "//a[contains(@href,'EditUser')]",
                            new PageLocatorOptions { HasText = this.TestSettings.AdminUserName }).First
                            .GetAttributeAsync("href");

        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

        return adminLink![(adminLink.LastIndexOf('/') + 1)..];
    }
}
