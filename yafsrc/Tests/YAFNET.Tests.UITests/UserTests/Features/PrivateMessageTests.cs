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

                    // Enter sends the message
                    await page.Locator("#txtPrivateMessage").FillAsync(text);
                    await page.Locator("#txtPrivateMessage").PressAsync("Enter");

                    await message.WaitForAsync();

                    Assert.That(
                        await page.Locator("#txtPrivateMessage").InputValueAsync(),
                        Is.Empty,
                        "The message is not sent by the enter key");

                    // the conversation is loaded from the database
                    await page.ReloadAsync();

                    await message.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

                    // the conversation list shows the last message
                    Assert.That(
                        await page.Locator($"li.chat-list-user[id='{adminId}'] .chat-preview").TextContentAsync(),
                        Does.Contain(text),
                        "The last message is not shown in the conversation list");

                    // deleted for the test user after the confirm dialog (the page is reloaded)
                    await page.Locator("#deleteConversation").ClickAsync();
                    await page.RunAndWaitForNextPageAsync(() => page.Locator(".bootbox .btn-success").ClickAsync());

                    await page.Locator("#divMessage").WaitForAsync();
                    await page.WaitForTimeoutAsync(2000);

                    Assert.That(await message.CountAsync(), Is.Zero, "The deleted conversation is shown");
                });
    }

    /// <summary>
    /// A new conversation is started with a user found by the user search of the private messages page.
    /// </summary>
    [Test]
    public async Task NewConversationTest()
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

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyMessages");

                    await page.Locator("[data-bs-target='#NewConversation']").First.ClickAsync();

                    await page.Locator("#FindUser").FillAsync(this.TestSettings.AdminUserName[..3]);

                    var userLink = page.Locator($"#FindUserResults a[href*='u={adminId}']");

                    await userLink.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

                    await page.RunAndWaitForNextPageAsync(() => userLink.ClickAsync());

                    Assert.That(
                        await page.Locator($"li.chat-list-user.active[id='{adminId}']").CountAsync(),
                        Is.EqualTo(1),
                        "The new conversation is not opened");
                });
    }

    /// <summary>
    /// The conversation that the page opens can be clicked again before it was loaded: the messages are shown once
    /// (they were shown twice).
    /// </summary>
    [Test]
    public async Task ConversationIsShownOnceTest()
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

                    var text = $"Once {Random.Shared.Next()}";
                    var message = page.Locator(".direct-chat-text", new PageLocatorOptions { HasText = text });

                    await page.Locator("#txtPrivateMessage").FillAsync(text);
                    await page.Locator("#txtPrivateMessage").PressAsync("Enter");
                    await message.WaitForAsync();

                    // the page opens the conversation, it is clicked right away
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyMessages?u={adminId}");
                    await page.Locator($"li.chat-list-user[id='{adminId}']").ClickAsync();

                    await message.First.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
                    await page.WaitForTimeoutAsync(2000);

                    Assert.That(await message.CountAsync(), Is.EqualTo(1), "The messages are shown twice");
                });
    }

    /// <summary>
    /// A message to a user that does not accept private messages (anymore) is not sent: the sender gets a message
    /// and keeps the text (it was removed without a message).
    /// </summary>
    [Test]
    public async Task MessageToUserThatBlocksIsNotLostTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var testUserId = await this.GetUserIdAsync(page, this.TestSettings.TestUserName);

                    // a new user writes to the test user, then blocks private messages
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyMessages?u={testUserId}");

                    var hello = $"Hello {Random.Shared.Next()}";

                    await page.Locator("#txtPrivateMessage").FillAsync(hello);
                    await page.Locator("#txtPrivateMessage").PressAsync("Enter");
                    await page.Locator(".direct-chat-text", new PageLocatorOptions { HasText = hello }).WaitForAsync();

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/BlockOptions");
                    await page.Locator("#BlockPMs").CheckAsync();
                    await page.SubmitAndWaitAsync(page.Locator("button[type='submit']", new PageLocatorOptions { HasText = "Save" }));

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    // the test user answers
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword),
                        Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyMessages");
                    await page.Locator($"li.chat-list-user[data-name='{userName}']").ClickAsync();
                    await page.Locator(".direct-chat-text", new PageLocatorOptions { HasText = hello }).WaitForAsync();

                    var answer = $"Answer {Random.Shared.Next()}";

                    await page.Locator("#txtPrivateMessage").FillAsync(answer);
                    await page.Locator("#txtPrivateMessage").PressAsync("Enter");

                    var notSent = page.Locator("#privateMessageNotSent");

                    await notSent.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

                    Assert.Multiple(
                        async () =>
                            {
                                Assert.That(await notSent.IsVisibleAsync(), Is.True, "No message that the text was not sent");
                                Assert.That(
                                    await page.Locator("#txtPrivateMessage").InputValueAsync(),
                                    Is.EqualTo(answer),
                                    "The text that was not sent is lost");
                                Assert.That(
                                    await page.Locator(".direct-chat-text", new PageLocatorOptions { HasText = answer }).CountAsync(),
                                    Is.Zero,
                                    "The message is shown as sent");
                            });
                });
    }

    /// <summary>
    /// Gets the id of a user (from the user list of the admin), the admin is logged out again.
    /// </summary>
    private async Task<string> GetUserIdAsync(IPage page, string userName)
    {
        Assert.That(
            await page.LoginUserAsync(
                this.TestSettings,
                this.TestSettings.AdminUserName,
                this.TestSettings.AdminPassword),
            Is.True,
            "Login failed");

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
        await page.OpenUserFilterAsync();

        var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
        await searchNameInput.FillAsync(userName);
        await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

        var userLink = await page.Locator(
                            "//a[contains(@href,'EditUser')]",
                            new PageLocatorOptions { HasText = userName }).First
                            .GetAttributeAsync("href");

        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

        return userLink![(userLink.LastIndexOf('/') + 1)..];
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
        await page.OpenUserFilterAsync();

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
