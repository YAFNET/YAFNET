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
/// The quick reply below the posts and its dialog.
/// </summary>
public class QuickReplyTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A quick reply that is not saved keeps the page and the Post button: an empty text is not sent,
    /// a text of spaces and the flood delay show their message.
    /// </summary>
    [Test]
    [Description("A quick reply that is not saved shows why and the button can be used again")]
    public async Task QuickReplyNotSavedKeepsTheButtonTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var topicUrl = await CreateTopicAsync(page, this.TestSettings, "Quick reply");

                // a member has the flood delay
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync(topicUrl);

                var form = page.Locator("#quickReply form");
                var textarea = form.Locator("textarea[name='QuickReplyEditor']");
                var button = page.Locator("[data-bs-save='quickReply']");

                // an empty reply is not sent (the page is not loaded again)
                await page.EvaluateAsync("() => window.quickReplyMarker = 1");

                await button.ClickAsync();
                await page.WaitForTimeoutAsync(1500);

                Assert.That(
                    await page.EvaluateAsync<int?>("() => window.quickReplyMarker"),
                    Is.EqualTo(1),
                    "The empty reply loaded the page again");

                Assert.That(await form.GetAttributeAsync("class"), Does.Contain("was-validated"), "The empty text is not marked");

                // only spaces: the message of the server
                await textarea.FillAsync("   ");
                await button.ClickAsync();

                await Assertions.Expect(page.GetByText("You must enter something into your post.")).ToBeVisibleAsync();
                await AssertButtonUsableAsync(button);

                // the first reply is saved
                await textarea.FillAsync("The first quick reply");
                await page.RunAndWaitForNavigationAsync(() => button.ClickAsync());

                Assert.That(await page.ContentAsync(), Does.Contain("The first quick reply"), "The quick reply was not saved");

                // the second one right after it: the flood delay
                await page.Locator("#quickReply textarea[name='QuickReplyEditor']").FillAsync("The second quick reply");
                await page.Locator("[data-bs-save='quickReply']").ClickAsync();

                await Assertions.Expect(page.GetByText("You can't post in the next")).ToBeVisibleAsync();
                await AssertButtonUsableAsync(page.Locator("[data-bs-save='quickReply']"));
            });
    }

    /// <summary>
    /// Creates a topic as admin and logs out.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="testSettings">The test settings.</param>
    /// <param name="subject">The start of the subject.</param>
    /// <returns>The url of the topic.</returns>
    private static async Task<string> CreateTopicAsync(IPage page, TestConfig testSettings, string subject)
    {
        Assert.That(
            await page.LoginUserAsync(testSettings, testSettings.AdminUserName, testSettings.AdminPassword),
            Is.True,
            "Login failed");

        var topicId = await page.CreateTopicAsync(
                          testSettings,
                          testSettings.TestForumId,
                          $"{subject} {Random.Shared.Next()}",
                          "The first post of the topic");

        Assert.That(topicId, Is.Not.Null, "The topic was not created");

        var topicUrl = Regex.Match(page.Url, @"^.*/topic/\d+/[^/?#]+").Value;

        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

        return topicUrl;
    }

    /// <summary>
    /// The Post button shows its text and can be clicked.
    /// </summary>
    /// <param name="button">The button.</param>
    private static async Task AssertButtonUsableAsync(ILocator button)
    {
        Assert.That(await button.GetAttributeAsync("class"), Does.Not.Contain("disabled"), "The button is disabled");
        Assert.That(await button.InnerTextAsync(), Does.Not.Contain("Loading"), "The button still shows Loading");
    }
}
