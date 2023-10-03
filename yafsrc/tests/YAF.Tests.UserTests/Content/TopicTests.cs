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
/// The topic tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TopicTests : TestBase
{
    /// <summary>
    /// Create the new topic in forum test.
    /// </summary>
    [Test]
    public async Task CreateNewTopicTest()
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
                        $"{this.Base.TestSettings.TestForumUrl}PostTopic/{this.Base.TestSettings.TestForumId}");

                    var pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Post New Topic"), "Post New Topic not possible");

                    // Create New Topic
                    await page.Locator("//input[contains(@id,'_TopicSubject')]")
                        .FillAsync($"Auto Created Test Topic - {DateTime.UtcNow}");

                    if (pageSource.Contains("Description"))
                    {
                        await page.Locator("//input[contains(@id,'_TopicDescription')]").FillAsync("Test Description");
                    }

                    await page.Locator(".BBCodeEditor")
                        .FillAsync("This is a Test Message Created by an automated Unit Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Next Topic"), "Topic Creating failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Delete topic test
    /// </summary>
    [Test]
    [Description("Delete a newly created topic")]
    public async Task DeleteTopicTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.IsTrue(
                await page.LoginUserAsync(
                    this.Base.TestSettings,
                    this.Base.TestSettings.AdminUserName,
                    this.Base.TestSettings.AdminPassword),
                "Login failed");

                // Do actual test

                // Go to Post New Topic
                await page.GotoAsync(
                $"{this.Base.TestSettings.TestForumUrl}PostTopic/{this.Base.TestSettings.TestForumId}");

                var pageSource = await page.ContentAsync();

                Assert.IsTrue(pageSource.Contains("Post New Topic"), "Post New Topic not possible");

                var topicTitle = $"Auto Created Test Topic for deleting - {DateTime.UtcNow}";

                // Create New Topic
                await page.Locator("//input[contains(@id,'_TopicSubject')]")
                .FillAsync(topicTitle);

                if (pageSource.Contains("Description"))
                {
                    await page.Locator("//input[contains(@id,'_TopicDescription')]").FillAsync("Test Description");
                }

                await page.Locator(".BBCodeEditor")
                .FillAsync("This is a Test Message Created by an automated Unit Test");

                // Post New Topic
                await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                pageSource = await page.ContentAsync();

                Assert.IsTrue(pageSource.Contains("Next Topic"), "Topic Creating failed");

                // Delete Topic
                await page.GetByRole(AriaRole.Button, new() { Name = " Manage Topic" }).First.ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { Name = " Delete Topic" }).ClickAsync();

                await page.Locator(".btn-success").ClickAsync();

                pageSource = await page.ContentAsync();

                Assert.IsFalse(pageSource.Contains(topicTitle), "Topic Deleting failed");
                ////
            },
            this.BrowserType);
    }
}