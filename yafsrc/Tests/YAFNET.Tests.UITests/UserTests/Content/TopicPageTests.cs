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

using System.Text.RegularExpressions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Content;

/// <summary>
/// The pages of a topic, the page number is in the path (/topic/1/name/2).
/// </summary>
public class TopicPageTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The text of the first post, it is on the first page.
    /// </summary>
    private const string FirstPostText = "Text of the first post";

    /// <summary>
    /// The text of the reply of the other user, it is on the second page.
    /// </summary>
    private const string OtherUserText = "Text of the other user";

    /// <summary>
    /// The number of replies of the topic owner: with the first post one page is filled (20 posts per page),
    /// the last one is on the second page.
    /// </summary>
    private const int OwnerReplies = 20;

    /// <summary>
    /// A logged in user stays on the second page of a topic: when browsing and after an action on a post.
    /// </summary>
    [Test]
    [Description("A logged in user stays on the second page of a topic")]
    public async Task ActionsOnSecondPageOfTopicTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var topicId = await page.CreateTopicAsync(
                                  this.TestSettings,
                                  this.TestSettings.TestForumId,
                                  $"Topic pages {Random.Shared.Next()}",
                                  FirstPostText);

                Assert.That(topicId, Is.Not.Null, "The topic was not created");

                var topicUrl = Regex.Match(page.Url, @"^.*/topic/\d+/[^/?#]+").Value;

                // admins have no flood delay
                for (var reply = 1; reply <= OwnerReplies; reply++)
                {
                    await ReplyAsync(page, topicUrl, $"Reply of the owner {reply}");
                }

                // only the post of another user can be marked as answer by the topic owner
                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await ReplyAsync(page, topicUrl, OtherUserText);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // the pager links to the page in the path
                await page.GotoAsync($"{topicUrl}/1");

                await AssertPageAsync(page, 1);

                await page.RunAndWaitForNavigationAsync(
                    () => page.Locator(".pagination a.page-link[href$='/2']").First.ClickAsync());

                Assert.That(page.Url, Is.EqualTo($"{topicUrl}/2"), "The pager links to the path");

                await AssertPageAsync(page, 2);

                // the page number of an old url moves to the path
                await page.GotoAsync($"{topicUrl}?p=2");

                Assert.That(page.Url, Is.EqualTo($"{topicUrl}/2"), "The old url is redirected");

                await AssertPageAsync(page, 2);

                // an action on a post of the second page returns to that page. The button is in a dropdown
                var response = await page.RunAndWaitForNavigationAsync(
                                   () => page.Locator("button[formaction*='MarkAsAnswer']").First
                                       .EvaluateAsync("button => button.click()"));

                Assert.That(response?.Status, Is.EqualTo(200), "Mark as answer failed");

                Assert.That(page.Url, Does.Contain("/post/"), "The post is shown after the action");

                await AssertPageAsync(page, 2);

                Assert.That(await page.ContentAsync(), Does.Contain("Remove As Answer"), "The post is not the answer");

                // a reply with a quote of a post of the second page
                await page.GotoAsync($"{topicUrl}/2");

                await page.SubmitAndWaitAsync(
                    page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Reply with Quote" }).Last);

                Assert.That(await page.ContentAsync(), Does.Contain("Post a reply"), "Reply with quote not possible");

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("Reply from the second page");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(
                    await page.GetByText("Reply from the second page").CountAsync(),
                    Is.GreaterThan(0),
                    "The reply is not shown");

                await AssertPageAsync(page, 2);
            });
    }

    /// <summary>
    /// Writes a reply in a topic.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="topicUrl">The url of the topic.</param>
    /// <param name="message">The message.</param>
    private static async Task ReplyAsync(IPage page, string topicUrl, string message)
    {
        await page.GotoAsync(topicUrl);

        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'ReplyLink')]").First);

        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(message);

        await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));
    }

    /// <summary>
    /// Checks that the pager and the posts belong to a page of the topic.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="pageNumber">The expected page number, 1 or 2.</param>
    private static async Task AssertPageAsync(IPage page, int pageNumber)
    {
        Assert.That(
            (await page.Locator(".pagination .page-item.active").First.InnerTextAsync()).Trim(),
            Is.EqualTo(pageNumber.ToString()),
            $"The pager is not on page {pageNumber} at {page.Url}");

        Assert.That(
            await page.GetByText(FirstPostText).CountAsync(),
            pageNumber == 1 ? Is.GreaterThan(0) : Is.EqualTo(0),
            $"The first post at {page.Url}");

        Assert.That(
            await page.GetByText(OtherUserText).CountAsync(),
            pageNumber == 2 ? Is.GreaterThan(0) : Is.EqualTo(0),
            $"The reply of the other user at {page.Url}");
    }
}
