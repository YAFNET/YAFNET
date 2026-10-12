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

namespace YAF.Tests.UITests.UserTests.Content;

/// <summary>
/// Texts of the users (names, posts) that the scripts of the pages show: they are shown as text, never as html.
/// </summary>
public class UserTextInScriptsTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The mention list of the editor (@name) shows a display name with html as text.
    /// </summary>
    [Test]
    public async Task MentionListShowsNameAsTextTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var prefix = $"Mx{Random.Shared.Next(100000, 999999)}";
                var displayName = $"{prefix}<img src=x onerror=window.xssFired=1>";

                var mentionedUser = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await page.SetDisplayNamesAsync(this.TestSettings, true);

                try
                {
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, mentionedUser, this.TestSettings.TestUserPassword),
                        Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");
                    await page.Locator("#Input_DisplayName").FillAsync(displayName);
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateProfile')]"));

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                    Assert.That(
                        await page.Locator("#Input_DisplayName").InputValueAsync(),
                        Is.EqualTo(displayName),
                        "The display name was not saved");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    // another user mentions the user in the quick reply of a topic
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    Assert.That(
                        await page.CreateTopicAsync(
                            this.TestSettings,
                            this.TestSettings.TestForumId,
                            $"Mention test {prefix}",
                            "A topic for the mention test"),
                        Is.Not.Null,
                        "The topic was not created");

                    var editor = page.Locator("#QuickReplyEditor");
                    await editor.ClickAsync();
                    await editor.PressSequentiallyAsync($"@{prefix}");

                    var mentionList = page.Locator(".dropdown-mentions");
                    await mentionList.Locator(".mention-li-nt").First.WaitForAsync();

                    Assert.That(
                        await mentionList.Locator("img[onerror]").CountAsync(),
                        Is.Zero,
                        "The html of the display name was rendered");

                    Assert.That(
                        await mentionList.InnerTextAsync(),
                        Does.Contain("<img src=x"),
                        "The display name is not shown as text");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
                }
                finally
                {
                    await page.SetDisplayNamesAsync(this.TestSettings, false);
                }
            });
    }

    /// <summary>
    /// The context menu of a post shows selected text with html as text.
    /// </summary>
    [Test]
    public async Task ContextMenuShowsSelectedTextAsTextTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                // the tag is not closed, so it is not removed when the post is saved
                const string text = "Don't select <img src=x onerror=window.xssFired=1 x";

                Assert.That(
                    await page.CreateTopicAsync(
                        this.TestSettings,
                        this.TestSettings.TestForumId,
                        $"Context menu test {Random.Shared.Next()}",
                        text),
                    Is.Not.Null,
                    "The topic was not created");

                // select the text of the post and open the context menu
                await page.EvaluateAsync(
                    """
                    () => {
                        const post = document.querySelector('.message .selectionQuoteable');
                        const range = document.createRange();
                        range.selectNodeContents(post);
                        const selection = window.getSelection();
                        selection.removeAllRanges();
                        selection.addRange(range);
                        post.dispatchEvent(new MouseEvent('contextmenu', { bubbles: true, cancelable: true }));
                    }
                    """);

                var contextMenu = page.Locator(".message .context-menu.show");
                await contextMenu.Locator(".item-search").First.WaitForAsync();

                Assert.That(
                    await contextMenu.Locator("img[onerror]").CountAsync(),
                    Is.Zero,
                    "The html of the selected text was rendered");

                Assert.That(
                    await contextMenu.InnerTextAsync(),
                    Does.Contain("Don't select <img src=x"),
                    "The selected text is not shown as text");

                // the own topic can be quoted
                Assert.That(
                    await contextMenu.Locator(".item-selected-quoting").CountAsync(),
                    Is.EqualTo(1),
                    "The quote item is missing");
            });
    }
}
