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
/// The email topic (send a page to a friend) tests.
/// </summary>
public class EmailTopicTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Only the recipient can be chosen, the subject and text are fixed,
    /// and a user can only send 5 emails per hour.
    /// </summary>
    [Test]
    public async Task EmailTopicHasFixedTextAndLimitTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // a new user, the limit of the standard test users stays untouched
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    // a list of addresses is refused
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}EmailTopic/{this.TestSettings.TestTopicId}");

                    Assert.That(
                        await page.Locator("#EmailBody").GetAttributeAsync("readonly"), Is.Not.Null,
                        "The text of the email can be changed");

                    // the browser already refuses it in an email field, the server must check it too (crafted request)
                    await page.Locator("#Input_Email").EvaluateAsync("input => input.type = 'text'");
                    await page.Locator("#Input_Email").FillAsync("friend1@test.com, friend2@test.com");
                    await page.SubmitAndWaitAsync(page.Locator("form:has(#Input_Email) button[type=submit]"));

                    Assert.That(
                        await page.ContentAsync(), Does.Contain("You have entered an illegal e-mail address."),
                        "A list of addresses was accepted");

                    // a text posted with the form is ignored
                    this.Fixture.SmtpServer.ClearReceivedEmail();

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}EmailTopic/{this.TestSettings.TestTopicId}");

                    var expectedText = await page.Locator("#EmailBody").InputValueAsync();

                    await page.Locator("#Input_Email").FillAsync("friend@test.com");
                    await page.EvaluateAsync(
                        """
                        () => {
                            const form = document.querySelector('#Input_Email').form;
                            for (const name of ['Input.Body', 'Input.Subject']) {
                                const input = document.createElement('input');
                                input.type = 'hidden';
                                input.name = name;
                                input.value = 'Buy cheap pills';
                                form.appendChild(input);
                            }
                        }
                        """);
                    await page.SubmitAndWaitAsync(page.Locator("form:has(#Input_Email) button[type=submit]"));

                    var mail = await this.Fixture.SmtpServer.WaitForEmailAsync("friend@test.com");

                    Assert.That(mail, Is.Not.Null, "The email was not sent");

                    var body = mail.MessageParts[0].BodyData;

                    Assert.That(body, Does.Not.Contain("Buy cheap pills"), "The posted text was sent");
                    Assert.That(mail.Headers["Subject"], Does.Not.Contain("Buy cheap pills"), "The posted subject was sent");
                    Assert.That(body, Does.Contain($"Posts/{this.TestSettings.TestTopicId}/"), "The topic link is missing");
                    Assert.That(
                        body.Replace("\r", string.Empty),
                        Does.Contain(expectedText.Replace("\r", string.Empty).Trim()),
                        "The email is not the shown text");

                    // 4 more are allowed, the 6th within the hour is refused
                    for (var i = 2; i <= 6; i++)
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}EmailTopic/{this.TestSettings.TestTopicId}");

                        await page.Locator("#Input_Email").FillAsync($"friend{i}@test.com");
                        await page.SubmitAndWaitAsync(page.Locator("form:has(#Input_Email) button[type=submit]"));

                        var tooMany = (await page.ContentAsync()).Contains("You have sent too many emails");

                        Assert.That(tooMany, Is.EqualTo(i == 6), $"Email {i} {(i == 6 ? "was not refused" : "was refused")}");
                    }
                });
    }
}
