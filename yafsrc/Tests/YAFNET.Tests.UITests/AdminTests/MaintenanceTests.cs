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

namespace YAF.Tests.UITests.AdminTests;

/// <summary>
/// The tests of the maintenance pages (attachments, tags, prune, recycle bin, private messages, event log).
/// </summary>
public class MaintenanceTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The selected page size of the deleted posts is used: the posts were always listed with the page size of the user.
    /// </summary>
    [Test]
    public async Task DeletedPostsPageSizeIsUsedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                // six deleted posts
                for (var i = 0; i < 6; i++)
                {
                    var topicId = await page.CreateTopicAsync(
                                      this.TestSettings,
                                      this.TestSettings.TestForumId,
                                      $"Recycle bin paging {Random.Shared.Next()}",
                                      "A post of the recycle bin test");

                    Assert.That(topicId, Is.Not.Null, "The topic was not created");

                    var deleteUrl = await page.Locator("a[href*='DeleteMessage']").First.GetAttributeAsync("href");

                    await page.GotoAsync(new Uri(new Uri(page.Url), deleteUrl).ToString());
                    await page.Locator("#ReasonEditor").FillAsync("Deleted by a test");
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Delete']"));
                }

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Restore");

                // the deleted posts are on their own tab
                await page.Locator("#DeletedPostsTab").ClickAsync();

                // the page size of the user is selected, another size is chosen
                var select = page.Locator("#MessagesPageSize");
                var userSize = int.Parse(await select.InputValueAsync());
                var size = userSize > 5 ? 5 : 10;

                await page.RunAndWaitForNextPageAsync(() => select.SelectOptionAsync(size.ToString()));

                // the tab of the posts stays open
                Assert.That(await page.Locator("#DeletedPostsTab").GetAttributeAsync("class"), Does.Contain("active"), "The tab of the posts was closed");

                var rows = await page.Locator("#DeletedPosts .post-row").CountAsync();

                // the old code listed min(user size, deleted posts) rows, at least 6 posts are deleted
                if (size == 5)
                {
                    Assert.That(rows, Is.EqualTo(5), $"The posts are listed with the page size of the user ({userSize})");
                }
                else
                {
                    Assert.That(rows, Is.GreaterThan(userSize), $"The posts are listed with the page size of the user ({userSize})");
                }
            });
    }

    /// <summary>
    /// The search of the deleted topics starts on the first page: it searched on the shown page, a search with less
    /// results than that page showed nothing.
    /// </summary>
    [Test]
    public async Task DeletedTopicsSearchStartsOnTheFirstPageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                // the second page of the deleted topics
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Restore/2/1");

                var action = await page.Locator("button[formaction*='Refresh']").GetAttributeAsync("formaction");

                Assert.That(action, Does.Not.Contain("/2/"), $"The search is on the shown page ({action})");
            });
    }

    /// <summary>
    /// The filter of the event log starts on the first page: it was applied on the shown page, a filter with less
    /// entries than that page showed nothing.
    /// </summary>
    [Test]
    public async Task EventLogFilterStartsOnTheFirstPageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                // the second page of the event log
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EventLog/2");

                var action = await page.Locator("button[formaction*='ApplyButton']").GetAttributeAsync("formaction");

                Assert.That(action, Does.Not.Contain("/2/"), $"The filter is applied on the shown page ({action})");
            });
    }

    /// <summary>
    /// The number of the deleted private messages is shown after the delete (nothing was shown), the entered days
    /// are kept.
    /// </summary>
    [Test]
    public async Task DeletePrivateMessagesKeepsTheDaysTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Pm");

                // only messages older than 9999 days (none)
                await page.Locator("#Input_Days2").FillAsync("9999");

                var button = page.Locator("button[formaction*='Commit']");

                // the confirm dialog of the button is skipped
                await page.RunAndWaitForNextPageAsync(() => button.EvaluateAsync("b => b.form.requestSubmit(b)"));

                Assert.That(await page.Locator("#Input_Days2").InputValueAsync(), Is.EqualTo("9999"), "The days were reset");
                Assert.That(await page.ContentAsync(), Does.Contain("0 private messages were deleted."), "The number of the deleted messages is missing");
            });
    }

    /// <summary>
    /// A chip of the event types filters the event log by its type.
    /// </summary>
    [Test]
    public async Task EventLogTypeChipFiltersTheListTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EventLog");

                // the login of the test is logged, there is at least one type
                var chip = page.Locator(".event-type:not([data-type='-1'])").First;
                var type = await chip.GetAttributeAsync("data-type");

                await page.RunAndWaitForNextPageAsync(() => chip.ClickAsync());

                var types = await page.Locator(".event-row").EvaluateAllAsync<string[]>("rows => rows.map(r => r.dataset.type)");

                Assert.That(types, Is.Not.Empty, "The type has no entries");
                Assert.That(types, Is.All.EqualTo(type), "The list is not filtered by the type of the chip");
                Assert.That(
                    await page.Locator($".event-type[data-type='{type}']").GetAttributeAsync("class"),
                    Does.Not.Contain("btn-outline"),
                    "The chip of the type is not selected");
            });
    }

    /// <summary>
    /// Enter in the search field of the tags searches the tags of the board.
    /// </summary>
    [Test]
    public async Task TagsSearchTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Tags");

                // a text that no tag has
                await page.Locator("#Filter").FillAsync($"no tag {Random.Shared.Next()}");

                await page.RunAndWaitForNextPageAsync(() => page.Locator("#Filter").PressAsync("Enter"));

                Assert.That(page.Url, Does.Contain("Search"), "Enter did not search");
                Assert.That(await page.Locator(".tags-no-match").CountAsync(), Is.EqualTo(1), "The search did not filter the tags");
                Assert.That(await page.Locator(".tag-card").CountAsync(), Is.Zero, "Tags are listed that don't match");
            });
    }

    /// <summary>
    /// The usages of an attachment are the posts that show it, the button leads to them: they were the hits of the
    /// search index for "]id[" (the number in any post, only after the index was updated).
    /// </summary>
    [Test]
    public async Task AttachmentUsagesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                var image = Convert.ToBase64String(
                    await System.IO.File.ReadAllBytesAsync(System.IO.Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png")));
                var fileName = $"usage-{Random.Shared.Next()}.png";

                var attachmentId = await page.EvaluateAsync<int>(
                    """
                    async ([baseUrl, image, name]) => {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                        const bytes = Uint8Array.from(atob(image), c => c.charCodeAt(0));
                        const data = new FormData();
                        data.append('file', new File([bytes], name, { type: 'image/png' }));
                        const response = await fetch(baseUrl + 'api/FileUpload/Upload', {
                            method: 'POST', headers: { 'RequestVerificationToken': token }, body: data });
                        return (await response.json())[0].fileID;
                    }
                    """,
                    new object[] { this.TestSettings.TestForumUrl, image, fileName });

                Assert.That(attachmentId, Is.GreaterThan(0), "The attachment was not uploaded");

                var subject = $"Attachment usage {Random.Shared.Next()}";

                Assert.That(
                    await page.CreateTopicAsync(this.TestSettings, this.TestSettings.TestForumId, subject, $"[attach]{attachmentId}[/attach]"),
                    Is.Not.Null,
                    "The topic was not created");

                // a post with the number of the attachment is no usage
                Assert.That(
                    await page.CreateTopicAsync(this.TestSettings, this.TestSettings.TestForumId, $"Number {Random.Shared.Next()}", $"The number {attachmentId} is no attachment"),
                    Is.Not.Null,
                    "The topic was not created");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Attachments");

                var row = page.Locator(".attachment-row", new PageLocatorOptions { HasText = fileName });

                Assert.That(await row.CountAsync(), Is.EqualTo(1), "The attachment is not listed");
                Assert.That(
                    (await row.Locator(".attachment-meta .badge").InnerTextAsync()).Trim(),
                    Is.EqualTo("1"),
                    "The post of the attachment is not counted");

                await page.RunAndWaitForNextPageAsync(() => row.Locator(".attachment-posts").ClickAsync());

                Assert.That(await page.ContentAsync(), Does.Contain(subject), "The button doesn't lead to the post");
            });
    }
}
