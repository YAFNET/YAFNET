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

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.UserSettings;

/// <summary>
/// The Email Notification tests.
/// </summary>
public class EmailNotificationTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Add watch topic test.
    /// </summary>
    [Test]
    public async Task AddWatchTopicTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    if (this.TestSettings.NewInstall)
                    {
                        var source = await page.UpdateEmailSettingsAsync(this.TestSettings);
                        Assert.That(source.Contains("Notification for topics or forums you've selectively watched (listed below)"), Is.True);
                    }

                    // Go to Test Topic
                    await page.GotoAsync(
                        $"{this.TestSettings.TestForumUrl}Posts/{this.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("You've passed an invalid value to the forum."), Is.False,
                        "Test Topic Doesn't Exists");

                    // Get Topic Title
                    var topicTitle = await page.Locator(".breadcrumb-item.active").TextContentAsync();

                    Assert.That(topicTitle, Is.Not.Null);

                    // Open Topic Options Menu
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Tools" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Watch this topic"),
                        "Watch Topic is disabled, or User already Watches that Topic");

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'TrackTopic')]"));

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You will now be"), "Watch topic failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/Subscriptions");

                    pageSource = await page.ContentAsync();

                    await Assert.MultipleAsync(async () =>
                    {
                        Assert.That(
                            pageSource, Does.Contain("Notification Preferences"),
                            "Email Notification Preferences is not available for that User");

                        Assert.That(await page.GetByText(topicTitle).IsVisibleAsync(), Is.True);
                    });
                });
    }

    /// <summary>
    /// Delete watch topic test.
    /// </summary>
    [Test, DependsOnTest(nameof(AddWatchTopicTest))]
    public async Task DeleteWatchTopicTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    // Go to Test Topic
                    await page.GotoAsync(
                        $"{this.TestSettings.TestForumUrl}Posts/{this.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("You've passed an invalid value to the forum."), Is.False,
                        "Test Topic Doesn't Exists");

                    // Open Topic Options Menu
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Tools" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Unwatch this topic"),
                        "Watch Topic is disabled, or User doesn't watch this topic");

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UnTrackTopic')]"));

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You will no longer"), "Unwatch topic failed");
                });
    }

    /// <summary>
    /// Add watch forum test.
    /// </summary>
    [Test, DependsOnTest(nameof(DeleteWatchTopicTest))]
    public async Task AddWatchForumTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    if (this.TestSettings.NewInstall)
                    {
                        var source = await page.UpdateEmailSettingsAsync(this.TestSettings);
                        Assert.That(source.Contains("Notification for topics or forums you've selectively watched (listed below)"), Is.True);
                    }

                    // Do actual test
                    await page.GotoAsync(
                        $"{this.TestSettings.TestForumUrl}Topics/{this.TestSettings.TestForumId}/testForum");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("New Topic"), "Test Forum with that ID doesn't exists");

                    // Get Forum Title
                    var forumTitle = await page.Locator(".breadcrumb-item.active").TextContentAsync();

                    Assert.Multiple(() =>
                    {
                        Assert.That(forumTitle, Is.Not.Null);

                        Assert.That(
                            pageSource, Does.Contain("Watch Forum"),
                            "Watch Forum is disabled, or User already Watches that Forum");
                    });

                    // Watch the Test Forum
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'WatchForum')]"));

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("You will now be notified when new posts are made in this forum."),
                        "Watch form failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/Subscriptions");

                    pageSource = await page.ContentAsync();

                    await Assert.MultipleAsync(async () =>
                    {
                        Assert.That(
                            pageSource, Does.Contain("Notification Preferences"),
                            "Email Notification Preferences is not available for that User");

                        Assert.That(await page.GetByText(forumTitle).IsVisibleAsync(), Is.True);
                    });
                });
    }

    /// <summary>
    /// Delete watch forum test.
    /// </summary>
    [Test, DependsOnTest(nameof(AddWatchForumTest))]
    public async Task DeleteWatchForumTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync(
                        $"{this.TestSettings.TestForumUrl}Topics/{this.TestSettings.TestForumId}/testForum");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("New Topic"), "Test Forum with that ID doesn't exists");

                    Assert.That(
                        pageSource, Does.Contain("Unwatch Forum"),
                        "Watch Forum is disabled, or User doesn't watch that Forum");

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'WatchForum')]"));

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("You will no longer be notified when new posts are made in this forum."),
                        "Watch forum failed");
                });
    }

    /// <summary>
    /// Receive email on new post in Watched Topic test.
    /// </summary>
    [Test]
    public async Task ReceiveEmailOnNewPostTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    this.Fixture.SmtpServer.ClearReceivedEmail();

                    // Go to Test Topic
                    await page.GotoAsync(
                        $"{this.TestSettings.TestForumUrl}Posts/{this.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("You've passed an invalid value to the forum."), Is.False,
                        "Test Topic Doesn't Exists");

                    // Open Topic Options Menu
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Tools" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    // Subscribe if needed
                    if (pageSource.Contains("Watch this topic"))
                    {
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'TrackTopic')]"));

                        pageSource = await page.ContentAsync();

                        Assert.That(pageSource, Does.Contain("You will now be"), "Watch topic failed");
                    }

                    // Login as Admin and Post A Reply in the Test Topic
                    await page.LoginAdminUserAsync(this.TestSettings);

                    Assert.That(
                        await page.CreateNewReplyInTestTopicAsync(
                            this.TestSettings,
                            "Reply Message [hide-reply]Only for repliers[/hide-reply]"), Is.True,
                        "Reply Message as Admin failed");

                    // Check if an Email was received
                    while (this.Fixture.SmtpServer.ReceivedEmailCount.Equals(0))
                    {
                        await Task.Delay(5000);
                    }

                    var mail = this.Fixture.SmtpServer.ReceivedEmail[0];

                    Assert.That(
                        mail.ToAddresses[0].ToString(), Is.EqualTo($"{this.TestSettings.TestUserName.ToLower()}@test.com"),
                        "Receiver does not match");

                    Assert.That(
                        mail.FromAddress.Address, Is.EqualTo(this.TestSettings.TestForumMail),
                        "Sender does not match");

                    Assert.That(
                        mail.Headers["Subject"], Is.EqualTo($"Topic Subscription New Post Notification (From {this.TestSettings.TestApplicationName})"),
                        "Subject does not match");

                    Assert.That(
                        mail.MessageParts[0].BodyData.StartsWith("There's a new post in topic \""), Is.True,
                        "Body does not match");

                    // hidden content is only shown in the forum
                    Assert.That(
                        mail.MessageParts[0].BodyData, Does.Not.Contain("Only for repliers"),
                        "The mail contains hidden content");
                });
    }

    /// <summary>
    /// Send & receive email digest test
    /// </summary>
    [Test]
    public async Task SendReceiveEmailDigestTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Login as Admin and enable email digest
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Admin" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Settings" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Board Settings" })
                    .ClickAsync();

                await page.GetByRole(AriaRole.Checkbox,
                    new PageGetByRoleOptions { Name = "Allow Digest Email Sending" }).CheckAsync();

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).ClickAsync();

                // Login test user and enable digest
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/Subscriptions");

                await page.GetByText("Receive once daily digest/").ClickAsync();
                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).ClickAsync();

                // Login back as Admin make test post and enable email digest
                await page.LoginAdminUserAsync(this.TestSettings);

                // sign in test user and enable digest
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                this.Fixture.SmtpServer.ClearReceivedEmail();

                Assert.That(
                    await page.CreateNewReplyInTestTopicAsync(this.TestSettings, "Reply Message"), Is.True,
                    "Reply Message as Admin failed");

                // Login back as Admin make test post and enable email digest
                await page.LoginAdminUserAsync(this.TestSettings);

                // Force digest send
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Digest");

                await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email:" }).ClickAsync();
                await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email:" }).FillAsync($"{this.TestSettings.TestUserName.ToLower()}@test.com");
                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Send Test" }).ClickAsync();

                // Check if an Email was received
                while (this.Fixture.SmtpServer.ReceivedEmailCount.Equals(0))
                {
                    await Task.Delay(3000);
                }

                var mail = this.Fixture.SmtpServer.ReceivedEmail[0];

                Assert.That(
                    mail.ToAddresses[0].ToString(), Is.EqualTo($"{this.TestSettings.TestUserName.ToLower()}@test.com"),
                    "Receiver does not match");

                Assert.That(
                    mail.FromAddress.Address, Is.EqualTo(this.TestSettings.TestForumMail),
                    "Sender does not match");

                Assert.That(
                    mail.Headers["Subject"],
                    Is.EqualTo($"Active and New Topics - Daily Digest for {this.TestSettings.TestApplicationName}"),
                    "Subject does not match");

                Assert.That(
                    mail.MessageParts[1].BodyData
                        .Contains($"Active and New Topics - Daily Digest for {this.TestSettings.TestApplicationName}"),
                    Is.True,
                    "Body does not match");
            });
    }

    /// <summary>
    /// A notification type that is not in the list (manipulated form) is not saved and doesn't fail.
    /// </summary>
    [Test]
    public async Task NotificationTypeNotInListTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/Subscriptions");

                var select = page.Locator("select[name='Input.NotificationType']");

                var current = await select.InputValueAsync();

                foreach (var value in new[] { "x", "99" })
                {
                    await select.EvaluateAsync(
                        "(select, value) => { select.add(new Option(value, value, true, true)); select.value = value; }",
                        value);

                    var response = await page.RunAndWaitForResponseAsync(
                                       () => page.EvaluateAsync("() => document.querySelector(\"button[formaction*='/Save']\").click()"),
                                       r => r.Request.Method == "POST" && r.Url.Contains("Profile/Subscriptions"));

                    Assert.That(response.Status, Is.EqualTo(200), $"Saving the notification type {value} failed");

                    await page.WaitForURLAsync(url => url.EndsWith("/Save"));

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/Subscriptions");

                    Assert.That(await select.InputValueAsync(), Is.EqualTo(current), $"The notification type {value} was saved");
                }

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
            });
    }

    /// <summary>
    /// After "Force send" the digest page still shows when the digest was sent last (the field was empty).
    /// </summary>
    [Test]
    public async Task ForceDigestSendShowsLastSendTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                // the button is only enabled with the digest turned on
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");

                var digestSwitch = page.Locator("#Input_AllowDigestEmail");

                var digestWasEnabled = await digestSwitch.IsCheckedAsync();

                if (!digestWasEnabled)
                {
                    await digestSwitch.CheckAsync();
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
                }

                try
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Digest");

                    var lastSend = await page.Locator("#LastSend").InputValueAsync();

                    Assert.That(lastSend, Is.Not.Empty, "The last send time is not shown");

                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='ForceSend']"));

                    Assert.That(
                        await page.ContentAsync(), Does.Contain("Digest Send Scheduled for Immediate Sending"),
                        "The digest was not forced");

                    // not compared with the value before: the digest task can run in between
                    Assert.That(
                        await page.Locator("#LastSend").InputValueAsync(),
                        Is.Not.Empty,
                        "The last send time is not shown after forcing the digest");
                }
                finally
                {
                    if (!digestWasEnabled)
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");

                        await digestSwitch.UncheckAsync();
                        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
                    }
                }
            });
    }

    /// <summary>
    /// A topic is watched once: watching it again (e.g. from a second tab) failed with an error (the subscription
    /// is unique), and guests can't watch topics (the button is not shown to them).
    /// </summary>
    [Test]
    public async Task WatchTopicTwiceTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    var topicId = await page.CreateTopicAsync(
                        this.TestSettings,
                        this.TestSettings.TestForumId,
                        $"Watch twice test {Random.Shared.Next()}",
                        "A topic for the watch test");

                    Assert.That(topicId, Is.Not.Null, "The topic was not created");

                    // the url of the button (the page is not reloaded, like an outdated second tab)
                    var trackUrl = (await page.Locator("button[formaction*='TrackTopic']").First.GetAttributeAsync("formaction"))
                        ?.Replace("UnTrackTopic", "TrackTopic");

                    Assert.That(trackUrl, Does.Contain("TrackTopic"), "The watch button is missing");

                    const string post = """
                                        async url => {
                                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                                            const body = new URLSearchParams({ '__RequestVerificationToken': token });
                                            const response = await fetch(url, { method: 'POST', body: body });
                                            return [String(response.status), response.url];
                                        }
                                        """;

                    var first = await page.EvaluateAsync<string[]>(post, trackUrl);
                    var second = await page.EvaluateAsync<string[]>(post, trackUrl);

                    Assert.That(first[0], Is.EqualTo("200"), "Watching the topic failed");
                    Assert.That(second[0], Is.EqualTo("200"), "Watching the topic again failed");

                    // a guest is refused
                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Posts/{topicId}/topic");

                    var guest = await page.EvaluateAsync<string[]>(post, trackUrl);

                    Assert.That(guest[1], Does.Contain("Info"), $"The guest could watch the topic ({guest[0]} {guest[1]})");
                });
    }

    /// <summary>
    /// "Watch all forums" on the board index watched nothing (the forums were filtered by an empty category),
    /// and guests could send it.
    /// </summary>
    [Test]
    public async Task WatchAllForumsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    await page.GotoAsync(this.TestSettings.TestForumUrl);

                    var watchAllUrl = await page.Locator("button[formaction*='WatchAll']").GetAttributeAsync("formaction");

                    Assert.That(watchAllUrl, Does.Contain("WatchAll"), "The watch all button is missing");

                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='WatchAll']"));

                    await page.GotoAsync(
                        $"{this.TestSettings.TestForumUrl}Topics/{this.TestSettings.TestForumId}/testForum");

                    Assert.That(
                        await page.ContentAsync(), Does.Contain("Unwatch Forum"),
                        "The test forum is not watched after watch all");

                    // a guest is refused
                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    // a page with an antiforgery token
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/login");

                    const string post = """
                                        async url => {
                                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                                            const body = new URLSearchParams({ '__RequestVerificationToken': token });
                                            const response = await fetch(url, { method: 'POST', body: body });
                                            return [String(response.status), response.url];
                                        }
                                        """;

                    var guest = await page.EvaluateAsync<string[]>(post, watchAllUrl);

                    Assert.That(guest[1], Does.Contain("Info"), $"The guest could watch all forums ({guest[0]} {guest[1]})");
                });
    }
}
