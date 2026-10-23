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

using System.Collections.Generic;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Features;

/// <summary>
/// The reactions of the posts (they replace the thanks).
/// </summary>
[Description("The reactions of the posts.")]
public class ReactionTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A member reacts to the post of another member with the picker, changes the reaction and removes it
    /// by clicking it again; the author can't react to the own post and guests only see the counts.
    /// </summary>
    [Test]
    [Description("A member reacts to a post, changes and removes the reaction, guests only see the counts.")]
    public async Task ReactChangeAndRemoveTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var scriptErrors = new List<string>();

                page.PageError += (_, error) => scriptErrors.Add(error);
                page.Console += (_, msg) =>
                {
                    if (msg.Type == "error")
                    {
                        scriptErrors.Add(msg.Text);
                    }
                };

                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                // only the errors of the topic pages
                scriptErrors.Clear();

                Assert.That(
                    await page.CreateTopicAsync(
                        this.TestSettings,
                        this.TestSettings.TestForumId,
                        $"Reaction test {Random.Shared.Next()}",
                        "A post for the reaction test"),
                    Is.Not.Null,
                    "The topic was not created");

                var topicUrl = page.Url;
                var messageId = await page.Locator(".message .selectionQuoteable").First.GetAttributeAsync("id");

                Assert.That(messageId, Is.Not.Null.And.Not.Empty, "The message id was not found");

                var reactions = page.Locator($"#dvReactions{messageId}");

                Assert.That(
                    await page.Locator(".reaction-picker").CountAsync(),
                    Is.Zero,
                    "The author can react to the own post");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                scriptErrors.Clear();

                await page.GotoAsync(topicUrl);

                // the picker pops up with all reactions
                await reactions.Locator(".reaction-picker").ClickAsync();

                var menu = reactions.Locator(".reaction-picker-menu.show");

                await Assertions.Expect(menu).ToBeVisibleAsync();
                Assert.That(await menu.Locator(".reaction-emoji").CountAsync(), Is.EqualTo(5), "Reactions are missing in the picker");

                await ReactAsync(page, menu.Locator($".reaction-emoji[onclick='setReaction({messageId}, 1);']"));

                // the like of the user, highlighted, with the name in the tooltip
                var chips = reactions.Locator("button.rounded-pill");

                await Assertions.Expect(chips).ToHaveCountAsync(1);
                await Assertions.Expect(chips.First).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("btn-primary"));
                await Assertions.Expect(chips.First).ToContainTextAsync("1");
                Assert.That(await ChipTitleAsync(chips.First), Does.Contain("Like").And.Contain(userName), "The tooltip doesn't show the user");

                // another reaction replaces the like
                await reactions.Locator(".reaction-picker").ClickAsync();
                await ReactAsync(page, menu.Locator($".reaction-emoji[onclick='setReaction({messageId}, 2);']"));

                await Assertions.Expect(chips).ToHaveCountAsync(1);
                await Assertions.Expect(chips.First).ToHaveAttributeAsync("onclick", $"setReaction({messageId}, 2);");
                Assert.That(await ChipTitleAsync(chips.First), Does.Contain("Love"), "The reaction was not changed");

                // the reaction is saved
                await page.GotoAsync(topicUrl);

                await Assertions.Expect(chips).ToHaveCountAsync(1);
                await Assertions.Expect(chips.First).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("btn-primary"));

                // guests see the count without the names, and can't react
                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await page.GotoAsync(topicUrl);

                var badge = reactions.Locator(".badge");

                await Assertions.Expect(badge).ToHaveCountAsync(1);
                await Assertions.Expect(badge).ToContainTextAsync("1");
                Assert.That(await ChipTitleAsync(badge), Does.Contain("Love").And.Not.Contain(userName), "Guests see the names");
                Assert.That(await reactions.Locator(".reaction-picker").CountAsync(), Is.Zero, "Guests can react");

                // clicking the own reaction again removes it
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync(topicUrl);
                await ReactAsync(page, chips.First);

                await Assertions.Expect(chips).ToHaveCountAsync(0);
                await Assertions.Expect(reactions.Locator(".reaction-picker")).ToBeVisibleAsync();

                Assert.That(scriptErrors, Is.Empty, "Script errors on the page");
            });
    }

    /// <summary>
    /// Clicks a reaction and waits for the answer of the api.
    /// </summary>
    private static async Task ReactAsync(IPage page, ILocator reaction)
    {
        var response = await page.RunAndWaitForResponseAsync(
                           () => reaction.ClickAsync(),
                           r => r.Url.Contains("api/Reaction/React"));

        Assert.That(response.Status, Is.EqualTo(200), "The reaction was not saved");
    }

    /// <summary>
    /// Gets the tooltip text of a reaction (bootstrap moves the title into its own attribute).
    /// </summary>
    private static async Task<string> ChipTitleAsync(ILocator chip)
    {
        return await chip.GetAttributeAsync("data-bs-original-title") ?? await chip.GetAttributeAsync("title");
    }
}
