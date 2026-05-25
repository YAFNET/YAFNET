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

namespace YAF.Tests.UserTests.Content;

/// <summary>
/// The Message tests.
/// </summary>
public class MessageTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Editing a message.
    /// </summary>
    [Test]
    [Description("Editing a message")]
    public async Task EditMessageTest()
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

                // Go to Edit Message
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditMessage/{this.TestSettings.TestMessageId}");

                var pageSource = await page.ContentAsync();

                if (pageSource.Contains("You tried to enter an area where you didn't have access."))
                {
                    // Topic doesn't exist create a topic first
                    Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");

                    await page.GetByTitle("Tools").ClickAsync();
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit", Exact = true }).ClickAsync();
                }

                // Edit message
                var editorContent = await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
                    .TextContentAsync();


                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(
                    $"{editorContent} This messsage was edited.");

                await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                pageSource = await page.ContentAsync();

                Assert.That(
                 pageSource, Does.Contain("This messsage was edited."),
                 "Edit Message failed");
            });
    }

    /// <summary>
    /// Create a new reply in a topic.
    /// </summary>
    [Test]
    [Description("Create a new reply in a topic.")]
    public async Task PostReplyTest()
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

                    // Go to Post New Topic
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Posts/{this.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("You tried to enter an area where you didn't have access."))
                    {
                        // Topic doesn't exist create a topic first
                        Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");
                    }

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(
                        "This is a Test Reply in an Test Topic Created by an automated Unit Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("This is a Test Reply in an Test Topic Created by an automated Unit Test"),
                        "Reply Message failed");
                });
    }

    /// <summary>
    /// Post a reply with quoting a Message test.
    /// </summary>
    [Test]
    [Description("Post a reply with quoting a Message test.")]
    public async Task PostReplyWithQuoteTest()
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

                    // Go to Post New Topic
                    await page.GotoAsync(
                        $"{this.TestSettings.TestForumUrl}Posts/{this.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("You've passed an invalid value to the forum."))
                    {
                        // Topic doesn't exist create a topic first
                        Assert.That(
                            await page.CreateNewTestTopicAsync(this.TestSettings), Is.True,
                            "Topic Creating failed");
                    }

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Reply with Quote" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("  Quoting Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Quoting Test"), "Quoting Message Failed");
                });
    }

    /// <summary>
    /// Post 3 Replies and try to quote them with Multi Quoting via the "Multi Quote" Button test.
    /// </summary>
    [Test]
    [Description("Post 3 Replies and try to quote them with Multi Quoting via the \"Multi Quote\" Button test.")]
    public async Task PostReplyWithMultiQuoteTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    await Assert.MultipleAsync(async () =>
                    {
                        // Log user in first!
                        Assert.That(
                            await page.LoginUserAsync(
                                this.TestSettings,
                                this.TestSettings.TestUserName,
                                this.TestSettings.TestUserPassword), Is.True,
                            "Login failed");

                        // Do actual test

                        // Wait 35 seconds to avoid post flood
                        await Task.Delay(31000);

                        // First Creating a new test topic with the test user
                        Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");
                    });

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    // Post Replay A
                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    var url = page.Url;

                    await page.GotoAsync(url);

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply A
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("Test Reply A");

                    // Post New Message
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Test Reply A"), "Reply Message failed");

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    /////

                    // Post Replay B
                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync(); url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply B
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("Test Reply B");

                    // Post New Message
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Test Reply B"), "Reply Message failed");

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    /////

                    // Post Replay C
                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply C
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("Test Reply C");

                    // Post New Message
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Test Reply C"), "Reply Message failed");

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    /////

                    // Find the MultiQuote Buttons for Post Replace A,B,C
                    await page.Locator("//input[contains(@id,'multiQuote')]").Nth(1).ClickAsync();
                    await Task.Delay(5000);
                    await page.Locator("//input[contains(@id,'multiQuote')]").Nth(2).ClickAsync();
                    await Task.Delay(5000);
                    await page.Locator("//input[contains(@id,'multiQuote')]").Nth(3).ClickAsync();
                    await Task.Delay(5000);

                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Reply with Quote" }).Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    var editorContent = await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
                                            .TextContentAsync();

                    Assert.That(editorContent, Is.Not.Null, "Content is empty");

                    Assert.That(editorContent, Does.Contain("Test Reply A"), "Test Replay A quote not found");
                    Assert.That(editorContent, Does.Contain("Test Reply B"), "Test Replay B quote not found");
                    Assert.That(editorContent, Does.Contain("Test Reply C"), "Test Replay C quote not found");

                    // Create New Reply
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync($"{editorContent}  Multi Quoting Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Multi Quoting Test"), "MultiQuoting Message Failed");
                });
    }
}