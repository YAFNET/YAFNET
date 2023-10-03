/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Tests.UserTests.Content;

/// <summary>
/// The Message tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class MessageTests : TestBase
{
    /// <summary>
    /// Create a new reply in a topic.
    /// </summary>
    [Test]
    [Description("Create a new reply in a topic.")]
    public async Task PostReplyTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.IsTrue(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword),
                        "Login failed");

                    // Do actual test

                    // Go to Post New Topic
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Posts?t={this.Base.TestSettings.TestTopicId}&name=test");

                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("You tried to enter an area where you didn't have access."))
                    {
                        // Topic doesn't exist create a topic first
                        Assert.IsTrue(await page.CreateNewTestTopicAsync(this.Base.TestSettings), "Topic Creating failed");

                        // Wait 60 seconds to avoid post flood
                        Thread.Sleep(31000);
                    }

                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Post a reply"), "Post Reply not possible");

                    // Create New Reply
                    await page.Locator(".BBCodeEditor").FillAsync(
                        "This is a Test Reply in an Test Topic Created by an automated Unit Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(
                        pageSource.Contains(
                            "This is a Test Reply in an Test Topic Created by an automated Unit Test"),
                        "Reply Message failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Post a reply with quoting a Message test.
    /// </summary>
    [Test]
    [Description("Post a reply with quoting a Message test.")]
    public async Task PostReplyWithQuoteTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.IsTrue(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword),
                        "Login failed");

                    // Do actual test

                    // Go to Post New Topic
                    await page.GotoAsync(
                        $"{this.Base.TestSettings.TestForumUrl}Posts?t={this.Base.TestSettings.TestTopicId}&name=test");

                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("You've passed an invalid value to the forum."))
                    {
                        // Topic doesn't exist create a topic first
                        Assert.IsTrue(
                            await page.CreateNewTestTopicAsync(this.Base.TestSettings),
                            "Topic Creating failed");

                        // Wait 60 seconds to avoid post flood
                        Thread.Sleep(60000);
                    }

                    await page.GetByRole(AriaRole.Button, new() { Name = " Reply with Quote" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Post a reply"), "Post Reply not possible");

                    // Create New Reply
                    await page.Locator(".BBCodeEditor").FillAsync("  Quoting Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Quoting Test"), "Quoting Message Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Post 3 Replies and try to quote them with Multi Quoting via the "Multi Quote" Button test.
    /// </summary>
    [Test]
    [Description("Post 3 Replies and try to quote them with Multi Quoting via the \"Multi Quote\" Button test.")]
    public async Task PostReplyWithMultiQuoteTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.IsTrue(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword),
                        "Login failed");

                    // Do actual test

                    // First Creating a new test topic with the test user
                    Assert.IsTrue(await page.CreateNewTestTopicAsync(this.Base.TestSettings), "Topic Creating failed");

                    // Wait 30 seconds to avoid post flood
                    Thread.Sleep(60000);

                    // Post Replay A
                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    var pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Post a reply"), "Post Reply not possible");

                    // Create New Reply A
                    await page.Locator(".BBCodeEditor").FillAsync("Test Reply A");

                    // Post New Message
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Test Reply A"), "Reply Message failed");

                    // Wait 30 seconds to avoid post flood
                    Thread.Sleep(60000);

                    /////

                    // Post Replay B
                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Post a reply"), "Post Reply not possible");

                    // Create New Reply B
                    await page.Locator(".BBCodeEditor").FillAsync("Test Reply B");

                    // Post New Message
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Test Reply B"), "Reply Message failed");

                    // Wait 30 seconds to avoid post flood
                    Thread.Sleep(60000);

                    /////

                    // Post Replay C
                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Post a reply"), "Post Reply not possible");

                    // Create New Reply B
                    await page.Locator(".BBCodeEditor").FillAsync("Test Reply C");

                    // Post New Message
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Test Reply C"), "Reply Message failed");

                    // Wait 30 seconds to avoid post flood
                    Thread.Sleep(60000);

                    /////

                    // Find the MultiQuote Buttons for Post Replace A,B,C
                    await page.Locator("//input[contains(@id,'multiQuote')]").Nth(1).CheckAsync();
                    await page.Locator("//input[contains(@id,'multiQuote')]").Nth(2).CheckAsync();
                    await page.Locator("//input[contains(@id,'multiQuote')]").Nth(3).CheckAsync();

                    await page.GetByRole(AriaRole.Button, new() { Name = " Reply" }).Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Post a reply"), "Post Reply not possible");

                    var editorContent = await page.Locator(".BBCodeEditor")
                                            .TextContentAsync();

                    Assert.IsNotNull(editorContent, "Content is empty");

                    Assert.IsTrue(editorContent.Contains("Test Reply A"), "Test Replay A quote not found");
                    Assert.IsTrue(editorContent.Contains("Test Reply B"), "Test Replay B quote not found");
                    Assert.IsTrue(editorContent.Contains("Test Reply C"), "Test Replay C quote not found");

                    // Create New Reply
                    await page.Locator(".BBCodeEditor").FillAsync($"{editorContent}  Multi Quoting Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Multi Quoting Test"), "MultiQuoting Message Failed");
                },
            this.BrowserType);
    }
}