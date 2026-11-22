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

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Content;

/// <summary>
/// The page "my topics" (the active, unanswered, watched topics and the topics of the user).
/// </summary>
public class MyTopicsTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The title of the page is "My Topics": it was the title of another page ("Reset Password", the page of the
    /// texts has no title).
    /// </summary>
    [Test]
    public async Task PageTitleTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyTopics");

                var title = await page.TitleAsync();

                Assert.That(title, Does.StartWith("My Topics"), $"The title is wrong ({title})");
            });
    }

    /// <summary>
    /// "Mark all as read" marks all topics of the list: only the topics of the first page were marked (the list
    /// shows the next pages when it is scrolled).
    /// </summary>
    [Test]
    public async Task MarkAllAsReadMarksAllTopicsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // new topics of another user (more than a page)
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var prefix = $"Mark all {Random.Shared.Next()}";

                for (var i = 1; i <= 6; i++)
                {
                    Assert.That(
                        await page.CreateTopicAsync(this.TestSettings, this.TestSettings.TestForumId, $"{prefix} {i}", "A new topic"),
                        Is.Not.Null,
                        "The topic was not created");
                }

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                var topics = page.Locator("#topicsContainer > *", new PageLocatorOptions { HasText = prefix });
                var unread = topics.Filter(new LocatorFilterOptions { HasText = "New Posts" });

                // the active topics of all time, all on one page: the new topics are not read
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyTopics");
                await page.RunAndWaitForNextPageAsync(() => page.EvaluateAsync(
                    """
                    () => {
                        document.querySelector('#Input_TopicModeValue').value = '0';
                        document.querySelector('#Input_SinceValue').value = '9999';
                        const size = document.querySelector('#Size');
                        size.value = '50';
                        size.form.submit();
                    }
                    """));

                Assert.That(await unread.CountAsync(), Is.EqualTo(6), "The new topics are not shown as new");

                // 5 per page
                await page.RunAndWaitForNextPageAsync(() => page.EvaluateAsync(
                    """
                    () => {
                        document.querySelector('#Input_TopicModeValue').value = '0';
                        document.querySelector('#Input_SinceValue').value = '9999';
                        const size = document.querySelector('#Size');
                        size.value = '5';
                        size.form.submit();
                    }
                    """));

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='MarkAll']"));

                // all topics on one page
                await page.RunAndWaitForNextPageAsync(() => page.EvaluateAsync(
                    """
                    () => {
                        const size = document.querySelector('#Size');
                        size.value = '50';
                        size.form.submit();
                    }
                    """));

                Assert.That(await topics.CountAsync(), Is.EqualTo(6), "The new topics are not listed");
                Assert.That(
                    await unread.CountAsync(),
                    Is.Zero,
                    "A topic of the next page is not marked as read");
            });
    }

    /// <summary>
    /// The tabs show the lists: the active topics since the last visit, the other lists all topics; a time that is
    /// selected after the tab is kept.
    /// </summary>
    [Test]
    public async Task TabsSetTheTimeTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyTopics");

                var since = page.Locator("#Input_SinceValue");
                var active = page.Locator(".my-topics-tabs .nav-link.active");

                Assert.That(await since.InputValueAsync(), Is.EqualTo("0"), "The active topics are not shown since the last visit");

                // the watched topics: all
                await page.SubmitAndWaitAsync(page.Locator("#MyTopicsTab2"));

                Assert.That(await active.GetAttributeAsync("data-mode"), Is.EqualTo("2"), "The tab is not active");
                Assert.That(await since.InputValueAsync(), Is.EqualTo("9999"), "The watched topics are not shown of all time");

                // another time is kept
                await page.RunAndWaitForNextPageAsync(() => since.SelectOptionAsync("7"));

                Assert.That(await since.InputValueAsync(), Is.EqualTo("7"), "The selected time was set back");
                Assert.That(await active.GetAttributeAsync("data-mode"), Is.EqualTo("2"), "The list was changed");

                // the active topics: since the last visit
                await page.SubmitAndWaitAsync(page.Locator("#MyTopicsTab0"));

                Assert.That(await since.InputValueAsync(), Is.EqualTo("0"), "The active topics are not shown since the last visit");
            });
    }
}
