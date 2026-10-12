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

using YAF.Tests.Infrastructure;

namespace YAF.Tests.UserTests.Features;

/// <summary>
/// The email to a user tests.
/// </summary>
public class EmailUserTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A user can send a few emails to another user per hour,
    /// and can't send emails to a user who ignores them (like private messages).
    /// </summary>
    [Test]
    public async Task EmailUserLimitAndIgnoreTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // the recipient
                var recipient = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                var profileLink = await page.Locator("a", new PageLocatorOptions { HasText = "View Profile" }).First.GetAttributeAsync("href");
                var recipientId = Regex.Match(profileLink ?? string.Empty, @"/UserProfile/(\d+)/").Groups[1].Value;

                Assert.That(recipientId, Is.Not.Empty, $"The id of {recipient} was not found (link {profileLink}, page {page.Url})");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // the sender, with a post the recipient can ignore later
                var sender = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                var topicId = await page.CreateTopicAsync(
                                  this.TestSettings,
                                  this.TestSettings.TestForumId,
                                  $"Email user test {Random.Shared.Next()}",
                                  "A topic for the email user test");

                Assert.That(topicId, Is.Not.Null, "The topic was not created");

                // five emails per hour
                for (var i = 1; i <= 6; i++)
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Email/{recipientId}");

                    await page.Locator("#Input_Subject").FillAsync($"Hello {i}");
                    await page.Locator("#Input_Body").FillAsync("A message for the email user test");

                    await page.SubmitAndWaitAsync(page.Locator("form button[type='submit']").Last);

                    var tooMany = (await page.ContentAsync()).Contains("You have sent too many emails");

                    Assert.That(tooMany, Is.EqualTo(i > 5), $"Email {i}: the limit is wrong");

                    if (tooMany)
                    {
                        // the page is shown again with the name of the recipient
                        Assert.That(
                            await page.Locator("form .card-header").First.InnerTextAsync(),
                            Does.Contain(recipient),
                            "The name of the recipient is missing");
                    }
                }

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // the recipient ignores the sender
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, recipient, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Posts/{topicId}/topic");

                await page.RunAndWaitForNextPageAsync(
                    () => page.Locator("[formaction*='AddIgnoredUser']").First.EvaluateAsync("button => button.click()"));

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // the ignored sender can't open the email form
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, sender, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Email/{recipientId}");

                Assert.That(
                    await page.Locator("#Input_Subject").CountAsync(),
                    Is.Zero,
                    "An ignored user can send emails");
            });
    }
}
