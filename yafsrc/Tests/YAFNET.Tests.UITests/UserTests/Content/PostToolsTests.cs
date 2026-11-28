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

using System.Text.RegularExpressions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Content;

/// <summary>
/// The tools menu of a post.
/// </summary>
public class PostToolsTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The tools menu is shown when only some of its items are available: Undelete for the author of
    /// a deleted post, the posts of the author for a guest.
    /// </summary>
    [Test]
    [Description("The tools menu of a post is shown for the undelete and the author items")]
    public async Task ToolsMenuWithUndeleteAndAuthorItemsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await SetShowDeletedMessagesToAllAsync(page, this.TestSettings, true);

                try
                {
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    var topicId = await page.CreateTopicAsync(
                                      this.TestSettings,
                                      this.TestSettings.TestForumId,
                                      $"Post tools {Random.Shared.Next()}",
                                      "The first post of the admin");

                    Assert.That(topicId, Is.Not.Null, "The topic was not created");

                    var topicUrl = Regex.Match(page.Url, @"^.*/topic/\d+/[^/?#]+").Value;

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    // a member (no moderator) deletes the own reply
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    await page.GotoAsync(topicUrl);
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'ReplyLink')]").First);
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("The reply of the member");
                    await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                    var reply = page.Locator(".message .selectionQuoteable", new PageLocatorOptions { HasText = "The reply of the member" });

                    Assert.That(await reply.CountAsync(), Is.EqualTo(1), $"The reply was not posted ({page.Url})");

                    var messageId = await reply.GetAttributeAsync("id");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}DeleteMessage/{messageId}");

                    var deleteButton = page.Locator("button[formaction$='/Delete']");

                    Assert.That(await deleteButton.CountAsync(), Is.EqualTo(1), $"The member can't delete the own reply ({page.Url})");

                    await page.Locator("#ReasonEditor").FillAsync("Deleted by the author");
                    await page.SubmitAndWaitAsync(deleteButton);

                    await page.GotoAsync(topicUrl);

                    var deletedPost = page.Locator($"[id='MessageRow{messageId}']");

                    Assert.That(
                        await deletedPost.Locator(".alert-danger").CountAsync(),
                        Is.GreaterThan(0),
                        "The deleted post is not shown");

                    await deletedPost.GetByTitle("Tools").ClickAsync();

                    Assert.That(
                        await deletedPost.Locator(".dropdown-menu.show a[href*='DeleteMessage']").IsVisibleAsync(),
                        Is.True,
                        "The tools menu of the deleted post has no Undelete");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    // a guest sees the posts of the author
                    await page.GotoAsync(topicUrl);

                    var firstPost = page.Locator("[id^='MessageRow']").First;

                    await firstPost.GetByTitle("Tools").ClickAsync();

                    Assert.That(
                        await firstPost.Locator(".dropdown-menu.show").GetByText("View All Posts by User").IsVisibleAsync(),
                        Is.True,
                        "The guest does not see the posts of the author");

                    Assert.That(
                        await firstPost.Locator(".dropdown-menu.show button[formaction*='IgnoredUser']").CountAsync(),
                        Is.EqualTo(0),
                        "The guest can ignore the author");
                }
                finally
                {
                    await SetShowDeletedMessagesToAllAsync(page, this.TestSettings, false);
                }
            });
    }

    /// <summary>
    /// The post of an ignored user is collapsed below a bar that names the author, the button of the bar shows it.
    /// </summary>
    [Test]
    [Description("The post of an ignored user is hidden behind a bar")]
    public async Task PostOfIgnoredUserIsHiddenTest()
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
                                  $"Ignored post {Random.Shared.Next()}",
                                  "The post of the ignored admin");

                Assert.That(topicId, Is.Not.Null, "The topic was not created");

                var topicUrl = Regex.Match(page.Url, @"^.*/topic/\d+/[^/?#]+").Value;

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                try
                {
                    await page.GotoAsync(topicUrl);

                    var post = page.Locator("[id^='MessageRow']").First;

                    Assert.That(await page.Locator(".post-ignored").CountAsync(), Is.EqualTo(0), "The post is hidden before ignoring");

                    // the tools menu of the post ignores the author
                    await post.GetByTitle("Tools").ClickAsync();
                    await page.SubmitAndWaitAsync(post.Locator(".dropdown-menu.show button[formaction*='AddIgnoredUser']"));

                    await page.GotoAsync(topicUrl);

                    var bar = page.Locator(".post-ignored").First;

                    Assert.That(await bar.InnerTextAsync(), Does.Contain(this.TestSettings.AdminUserName), "The bar does not name the author");

                    post = page.Locator("[id^='MessageRow']").First;

                    Assert.That(await post.IsVisibleAsync(), Is.False, "The post of the ignored user is shown");

                    await bar.Locator("button").ClickAsync();

                    await Assertions.Expect(post.GetByText("The post of the ignored admin")).ToBeVisibleAsync();

                    // shown again after removing the author from the ignore list
                    await post.GetByTitle("Tools").ClickAsync();
                    await page.SubmitAndWaitAsync(post.Locator(".dropdown-menu.show button[formaction*='RemoveIgnoredUser']"));
                }
                finally
                {
                    await page.GotoAsync(topicUrl);
                }

                Assert.That(await page.Locator(".post-ignored").CountAsync(), Is.EqualTo(0), "The post is still hidden");
            });
    }

    /// <summary>
    /// Switches the deleted posts for all users on or off (Host Settings > Display).
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="testSettings">The test settings.</param>
    /// <param name="enable">If the deleted posts are shown to all users.</param>
    private static async Task SetShowDeletedMessagesToAllAsync(IPage page, TestConfig testSettings, bool enable)
    {
        await page.GotoAsync(testSettings.TestForumUrl);

        if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
        {
            await page.LogOutUserAsync();
        }

        Assert.That(
            await page.LoginUserAsync(testSettings, testSettings.AdminUserName, testSettings.AdminPassword),
            Is.True,
            "Login failed");

        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/HostSettings/Display");

        await page.Locator("#Input_ShowDeletedMessagesToAll").SetCheckedAsync(enable);

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']:visible").First);

        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
    }
}
