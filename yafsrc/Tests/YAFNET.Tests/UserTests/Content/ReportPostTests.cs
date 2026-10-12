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
/// The report post tests.
/// </summary>
public class ReportPostTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The moderators get an email for the first report of a user, reporting the same post again
    /// only counts the report (otherwise a user could flood the inboxes of the moderators).
    /// </summary>
    [Test]
    public async Task ReportingAgainSendsNoEmailTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(
                    await page.CreateTopicAsync(
                        this.TestSettings,
                        this.TestSettings.TestForumId,
                        $"Report test {Random.Shared.Next()}",
                        "A post for the report test"),
                    Is.Not.Null,
                    "The topic was not created");

                var messageId = await page.Locator(".message .selectionQuoteable").First.GetAttributeAsync("id");

                Assert.That(messageId, Is.Not.Null.And.Not.Empty, "The message id was not found");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // another user reports the post twice
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                this.Fixture.SmtpServer.ClearReceivedEmail();

                await ReportAsync(page, this.TestSettings, messageId, "first report");

                for (var wait = 0; wait < 20 && this.Fixture.SmtpServer.ReceivedEmailCount == 0; wait++)
                {
                    await Task.Delay(500);
                }

                var firstReportMails = this.Fixture.SmtpServer.ReceivedEmailCount;

                Assert.That(firstReportMails, Is.GreaterThan(0), "The moderators were not notified about the first report");

                await ReportAsync(page, this.TestSettings, messageId, "second report");

                await Task.Delay(3000);

                Assert.That(
                    this.Fixture.SmtpServer.ReceivedEmailCount,
                    Is.EqualTo(firstReportMails),
                    "The moderators were notified again");
            });
    }

    private static async Task ReportAsync(IPage page, TestConfig testSettings, string messageId, string text)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}ReportPost/{messageId}");

        await page.Locator("#Report").FillAsync(text);

        // the report is confirmed in a dialog
        await page.Locator("button[formaction*='Report']").ClickAsync();
        await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

        Assert.That(page.Url, Does.Not.Contain("ReportPost"), $"The report was not sent ({page.Url})");
    }

    /// <summary>
    /// The "view" of the reported posts only leads to the posts of the moderated forum (the link
    /// contains the topic name), an unknown post failed with an error.
    /// </summary>
    [Test]
    public async Task ReportedPostsViewOnlyForumPostsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Moderate/ReportedPosts/{this.TestSettings.TestForumId}");

                // the view button of an unknown post
                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.EvaluateAsync(
                                       """
                                       forumId => {
                                           const form = document.createElement('form');
                                           form.method = 'post';
                                           form.action = `/Moderate/ReportedPosts/${forumId}/View?messageId=999999`;
                                           form.appendChild(document.querySelector("input[name='__RequestVerificationToken']").cloneNode());
                                           document.body.appendChild(form);
                                           form.submit();
                                       }
                                       """,
                                       this.TestSettings.TestForumId),
                                   r => r.Request.Method == "POST" && r.Url.Contains("ReportedPosts"));

                Assert.That(response.Status, Is.LessThan(500), "The view of an unknown post failed");
                Assert.That(
                    response.Headers.TryGetValue("location", out var location) ? location : string.Empty,
                    Does.Not.Contain("/Post/"),
                    "The view leads to a post of another forum");
            });
    }
}
