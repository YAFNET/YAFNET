/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Tests.UserTests.UserSettings;

/// <summary>
/// The Email Notification tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class EmailNotificationTests : TestBase
{
    /// <summary>
    /// Add watch topic test.
    /// </summary>
    [Test, Order(1)]
    public async Task AddWatchTopicTest()
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

                    // Go to Test Topic
                    await page.GotoAsync(
                        $"{this.Base.TestSettings.TestForumUrl}Posts/{this.Base.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("You've passed an invalid value to the forum."), Is.False,
                        "Test Topic Doesn't Exists");

                    // Get Topic Title
                    var topicTitle = await page.Locator(".active").TextContentAsync();

                    Assert.That(topicTitle, Is.Not.Null);

                    // Open Topic Options Menu
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Tools" }).ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Watch this topic"),
                        "Watch Topic is disabled, or User already Watches that Topic");

                    await page.Locator("//button[contains(@formaction,'TrackTopic')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You will now be"), "Watch topic failed");

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/Subscriptions");

                    pageSource = await page.ContentAsync();

                    await Assert.MultipleAsync(async () =>
                    {
                        Assert.That(
                            pageSource, Does.Contain("Email Notification Preferences"),
                            "Email Notification Preferences is not available for that User");

                        Assert.That(await page.GetByText(topicTitle).IsVisibleAsync(), Is.True);
                    });
                },
            this.BrowserType);
    }

    /// <summary>
    /// Delete watch topic test.
    /// </summary>
    [Test, Order(2)]
    public async Task DeleteWatchTopicTest()
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

                    // Go to Test Topic
                    await page.GotoAsync(
                        $"{this.Base.TestSettings.TestForumUrl}Posts/{this.Base.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("You've passed an invalid value to the forum."), Is.False,
                        "Test Topic Doesn't Exists");

                    // Open Topic Options Menu
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Tools" }).ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Unwatch this topic"),
                        "Watch Topic is disabled, or User doesn't watch this topic");

                    await page.Locator("//button[contains(@formaction,'UnTrackTopic')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You will no longer"), "Unwatch topic failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Add watch forum test.
    /// </summary>
    [Test, Order(3)]
    public async Task AddWatchForumTest()
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
                    await page.GotoAsync(
                        $"{this.Base.TestSettings.TestForumUrl}Topics/{this.Base.TestSettings.TestForumId}/testForum");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("New Topic"), "Test Forum with that ID doesn't exists");

                    // Get Forum Title
                    var forumTitle = await page.Locator(".active").First.TextContentAsync();

                    Assert.Multiple(() =>
                    {
                        Assert.That(forumTitle, Is.Not.Null);

                        Assert.That(
                            pageSource, Does.Contain("Watch Forum"),
                            "Watch Forum is disabled, or User already Watches that Forum");
                    });

                    // Watch the Test Forum
                    await page.Locator("//button[contains(@formaction,'WatchForum')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("You will now be notified when new posts are made in this forum."),
                        "Watch form failed");

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/Subscriptions");

                    pageSource = await page.ContentAsync();

                    await Assert.MultipleAsync(async () =>
                    {
                        Assert.That(
                            pageSource, Does.Contain("Email Notification Preferences"),
                            "Email Notification Preferences is not available for that User");

                        Assert.That(await page.GetByText(forumTitle).IsVisibleAsync(), Is.True);
                    });
                },
            this.BrowserType);
    }

    /// <summary>
    /// Delete watch forum test.
    /// </summary>
    [Test, Order(4)]
    public async Task DeleteWatchForumTest()
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
                    await page.GotoAsync(
                        $"{this.Base.TestSettings.TestForumUrl}Topics/{this.Base.TestSettings.TestForumId}/testForum");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("New Topic"), "Test Forum with that ID doesn't exists");

                    Assert.That(
                        pageSource, Does.Contain("Unwatch Forum"),
                        "Watch Forum is disabled, or User doesn't watch that Forum");

                    await page.Locator("//button[contains(@formaction,'WatchForum')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("You will no longer be notified when new posts are made in this forum."),
                        "Watch forum failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Receive email on new post in Watched Topic test.
    /// </summary>
    [Test]
    public async Task ReceiveEmailOnNewPostTest()
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

                    // Go to Test Topic
                    await page.GotoAsync(
                        $"{this.Base.TestSettings.TestForumUrl}Posts/{this.Base.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("You've passed an invalid value to the forum."), Is.False,
                        "Test Topic Doesn't Exists");

                    // Open Topic Options Menu
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Tools" }).ClickAsync();

                    pageSource = await page.ContentAsync();

                    // Subscribe if needed
                    if (pageSource.Contains("Watch this topic"))
                    {
                        await page.Locator("//button[contains(@formaction,'TrackTopic')]").ClickAsync();

                        pageSource = await page.ContentAsync();

                        Assert.That(pageSource, Does.Contain("You will now be"), "Watch topic failed");
                    }

                    // Login as Admin and Post A Reply in the Test Topic
                    await page.LoginAdminUserAsync(this.Base.TestSettings);

                    Assert.That(
                        await page.CreateNewReplyInTestTopicAsync(this.Base.TestSettings, "Reply Message"), Is.True,
                        "Reply Message as Admin failed");

                    // Check if an Email was received
                    while (this.Base.SmtpServer.ReceivedEmailCount.Equals(0))
                    {
                        await Task.Delay(5000);
                    }

                    var mail = this.Base.SmtpServer.ReceivedEmail[0];

                    Assert.That(
                        mail.ToAddresses[0].ToString(), Is.EqualTo($"{this.Base.TestSettings.TestUserName.ToLower()}@test.com"),
                        "Receiver does not match");

                    Assert.That(
                        mail.FromAddress.Address, Is.EqualTo(this.Base.TestSettings.TestForumMail),
                        "Sender does not match");

                    Assert.That(
                        mail.Headers["Subject"], Is.EqualTo($"Topic Subscription New Post Notification (From {this.Base.TestSettings.TestApplicationName})"),
                        "Subject does not match");

                    Assert.That(
                        mail.MessageParts[0].BodyData.StartsWith("There's a new post in topic \""), Is.True,
                        "Body does not match");
                },
            this.BrowserType);
    }
}