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
/// The Message tests.
/// </summary>
public class MessageTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Editing a message.
    /// </summary>
    [Test]
    [Description("Editing a message")]
    public async Task EditMessageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                 await page.LoginUserAsync(
                     this.TestSettings,
                     this.TestSettings.TestUserName,
                     this.TestSettings.TestUserPassword), Is.True,
                 "Login failed");

                // Do actual test

                // Go to Edit Message
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditMessage/{this.TestSettings.TestMessageId}");

                var pageSource = await page.ContentAsync();

                if (pageSource.Contains("You tried to enter an area where you didn't have access."))
                {
                    // Topic doesn't exist create a topic first
                    Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");

                    await page.GetByTitle("Tools").ClickAsync();
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit", Exact = true }).ClickAsync();
                }

                // Edit message
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                var editorContent = await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
                    .TextContentAsync();


                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(
                    $"{editorContent} This messsage was edited.");

                await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                pageSource = await page.ContentAsync();

                Assert.That(
                 pageSource, Does.Contain("This messsage was edited."),
                 "Edit Message failed");
            });
    }

    /// <summary>
    /// Create a new reply in a topic.
    /// </summary>
    [Test]
    [Description("Create a new reply in a topic.")]
    public async Task PostReplyTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    // Go to Post New Topic
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Posts/{this.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("You tried to enter an area where you didn't have access."))
                    {
                        // Topic doesn't exist create a topic first
                        Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");
                    }

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(
                        "This is a Test Reply in an Test Topic Created by an automated Unit Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("This is a Test Reply in an Test Topic Created by an automated Unit Test"),
                        "Reply Message failed");
                });
    }

    /// <summary>
    /// Post a reply with quoting a Message test.
    /// </summary>
    [Test]
    [Description("Post a reply with quoting a Message test.")]
    public async Task PostReplyWithQuoteTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    // Go to Post New Topic
                    await page.GotoAsync(
                        $"{this.TestSettings.TestForumUrl}Posts/{this.TestSettings.TestTopicId}/test");

                    var pageSource = await page.ContentAsync();

                    if (pageSource.Contains("You've passed an invalid value to the forum."))
                    {
                        // Topic doesn't exist create a topic first
                        Assert.That(
                            await page.CreateNewTestTopicAsync(this.TestSettings), Is.True,
                            "Topic Creating failed");
                    }

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Reply with Quote" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("  Quoting Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Quoting Test"), "Quoting Message Failed");
                });
    }

    /// <summary>
    /// Post 3 Replies and try to quote them with Multi Quoting via the "Multi Quote" Button test.
    /// </summary>
    [Test]
    [Description("Post 3 Replies and try to quote them with Multi Quoting via the \"Multi Quote\" Button test.")]
    public async Task PostReplyWithMultiQuoteTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    await Assert.MultipleAsync(async () =>
                    {
                        // Log user in first!
                        Assert.That(
                            await page.LoginUserAsync(
                                this.TestSettings,
                                this.TestSettings.TestUserName,
                                this.TestSettings.TestUserPassword), Is.True,
                            "Login failed");

                        // Do actual test

                        // Wait 35 seconds to avoid post flood
                        await Task.Delay(31000);

                        // First Creating a new test topic with the test user
                        Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");
                    });

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    // Post Replay A
                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    var url = page.Url;

                    await page.GotoAsync(url);

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply A
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("Test Reply A");

                    // Post New Message
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Test Reply A"), "Reply Message failed");

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    /////

                    // Post Replay B
                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync(); url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply B
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("Test Reply B");

                    // Post New Message
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Test Reply B"), "Reply Message failed");

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    /////

                    // Post Replay C
                    await page.Locator("//button[contains(@formaction,'ReplyLink')]").First.ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    // Create New Reply C
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("Test Reply C");

                    // Post New Message
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Test Reply C"), "Reply Message failed");

                    // Wait 35 seconds to avoid post flood
                    await Task.Delay(35000);

                    /////

                    // Find the MultiQuote Buttons for Post Replace A,B,C
                    await page.Locator("//input[contains(@id,'multiQuote')]").Nth(1).ClickAsync();
                    await Task.Delay(5000);
                    await page.Locator("//input[contains(@id,'multiQuote')]").Nth(2).ClickAsync();
                    await Task.Delay(5000);
                    await page.Locator("//input[contains(@id,'multiQuote')]").Nth(3).ClickAsync();
                    await Task.Delay(5000);

                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Reply with Quote" }).Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post a reply"), "Post Reply not possible");

                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    var editorContent = await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
                                            .TextContentAsync();

                    Assert.That(editorContent, Is.Not.Null, "Content is empty");

                    Assert.That(editorContent, Does.Contain("Test Reply A"), "Test Replay A quote not found");
                    Assert.That(editorContent, Does.Contain("Test Reply B"), "Test Replay B quote not found");
                    Assert.That(editorContent, Does.Contain("Test Reply C"), "Test Replay C quote not found");

                    // Create New Reply
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync($"{editorContent}  Multi Quoting Test");

                    // Post New Topic
                    await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

                    url = page.Url;

                    await page.GotoAsync(url);

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Multi Quoting Test"), "MultiQuoting Message Failed");
                });
    }

    /// <summary>
    /// A member can not edit the message of another user by opening the edit page directly.
    /// </summary>
    [Test]
    [Description("Editing the message of another user is denied")]
    public async Task EditMessageOfOtherUserIsDeniedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // the admin creates a topic
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");

                var editLink = await page.Locator("a[href*='EditMessage']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "Edit link of the admin message not found");

                // the test user opens the edit page of that message
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), editLink).ToString());

                var pageSource = await page.ContentAsync();

                Assert.That(
                    pageSource, Does.Contain("You tried to enter an area where you didn't have access."),
                    "The message of another user can be edited");
            });
    }

    /// <summary>
    /// The history of a deleted message (which contains its text) is only shown to moderators.
    /// </summary>
    [Test]
    [Description("The history of a deleted message is hidden from members")]
    public async Task MessageHistoryOfDeletedMessageIsHiddenTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var replyText = $"History test reply {Guid.NewGuid():N}";

                // the admin posts a reply and edits it, so it has a history
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");

                // the reply page (the topic page has its own quick reply editor)
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'ReplyLink')]").First);

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(replyText);

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(await page.ContentAsync(), Does.Contain(replyText), "Posting the reply failed");

                // after posting, the new message is shown (Post/{id}/... or Posts/{topic}/...?m={id})
                var match = System.Text.RegularExpressions.Regex.Match(page.Url, @"/Post/(\d+)/|[?&]m=(\d+)");
                var messageId = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;

                Assert.That(messageId, Is.Not.Empty, $"The id of the reply is not in {page.Url}");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditMessage/{messageId}");

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync($"{replyText} edited");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                // a member sees the history of the visible message
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName2,
                        this.TestSettings.TestUser2Password), Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MessageHistory?m={messageId}");

                // the original text and the current text (the texts are only shown when compared)
                Assert.That(
                    await page.Locator("input[type=checkbox][name$='.Selected']").CountAsync(),
                    Is.EqualTo(2),
                    "Message history is not shown");

                // compares the first two versions, sent with a token of the topic list
                async Task<string> CompareVersionsAsync()
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyTopics");

                    return await page.EvaluateAsync<string>(
                        """
                        async url => {
                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                            const body = new URLSearchParams({
                                '__RequestVerificationToken': token,
                                'RevisionsList[0].Selected': 'true',
                                'RevisionsList[1].Selected': 'true' });
                            const response = await fetch(url, { method: 'POST', body: body });
                            return await response.text();
                        }
                        """,
                        $"{this.TestSettings.TestForumUrl}MessageHistory/ShowDiff?m={messageId}");
                }

                Assert.That(await CompareVersionsAsync(), Does.Contain(replyText), "The versions can not be compared");

                // the admin deletes the reply
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}DeleteMessage/{messageId}");

                await page.Locator("#ReasonEditor").FillAsync("Deleted by a test");
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Delete']"));

                // now the member sees neither the history nor the report page of it
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName2,
                        this.TestSettings.TestUser2Password), Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MessageHistory?m={messageId}");

                Assert.That(
                    await page.Locator("input[type=checkbox][name$='.Selected']").CountAsync(),
                    Is.EqualTo(0),
                    "The history of the deleted message is shown");

                // also the compare of its versions (loaded from the database)
                Assert.That(await CompareVersionsAsync(), Does.Not.Contain(replyText), "The versions of the deleted message can be compared");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}ReportPost/{messageId}");

                Assert.That(
                    await page.ContentAsync(), Does.Not.Contain(replyText),
                    "The deleted message is shown on the report page");
            });
    }

    /// <summary>
    /// An older version of an edited message can be restored from the message history.
    /// </summary>
    [Test]
    [Description("Restore an older version of a message")]
    public async Task RestoreMessageVersionTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var replyText = $"Restore test reply {Guid.NewGuid():N}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");

                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'ReplyLink')]").First);

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(replyText);

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                var match = System.Text.RegularExpressions.Regex.Match(page.Url, @"/Post/(\d+)/|[?&]m=(\d+)");
                var messageId = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;

                Assert.That(messageId, Is.Not.Empty, $"The id of the reply is not in {page.Url}");

                // two edits: the history contains the original text and the first edit (saved with the time of the second edit)
                foreach (var edit in new[] { "first edit", "second edit" })
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditMessage/{messageId}");

                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync($"{replyText} {edit}");

                    await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));
                }

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MessageHistory?m={messageId}");

                var restoreButtons = page.Locator("button[formaction*='Restore']");

                // the original text and the first edit, not the current text
                Assert.That(await restoreButtons.CountAsync(), Is.EqualTo(2), "The restore buttons are missing");

                // restore the first edit (the confirm dialog is added by the script of the page)
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                var restoreAction = await restoreButtons.Nth(1).GetAttributeAsync("formaction");

                await restoreButtons.Nth(1).ClickAsync();

                // the confirm dialog sends the restore
                var restoreResponse = await page.RunAndWaitForResponseAsync(
                                          () => page.Locator(".modal.show")
                                              .GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Yes" }).ClickAsync(),
                                          r => r.Request.Method == "POST" && r.Url.Contains("Restore"));

                await page.WaitForLoadStateAsync(LoadState.Load);

                Assert.That(
                    restoreResponse.Status,
                    Is.EqualTo(200),
                    $"The restore failed ({restoreResponse.Status} {(restoreResponse.Headers.TryGetValue("location", out var location) ? location : string.Empty)}, {restoreAction})");

                // the editor contains the current text
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditMessage/{messageId}");

                var content = await page.ContentAsync();

                Assert.That(content, Does.Contain($"{replyText} first edit"), "The first edit was not restored");
                Assert.That(content, Does.Not.Contain($"{replyText} second edit"), "The first edit was not restored");
            });
    }

    /// <summary>
    /// A reply that quotes or mentions a name without user (a guest post, a deleted user or another
    /// display name) is posted without error (the notification of the quoted user failed).
    /// </summary>
    [Test]
    [Description("Reply with a quote and a mention of unknown users")]
    public async Task ReplyQuotingUnknownUserTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                var replyText = $"Reply with unknown quote {Guid.NewGuid():N}";

                Assert.That(
                    await page.CreateNewReplyInTestTopicAsync(
                        this.TestSettings,
                        $"{replyText} [quote=Nobody Here;1]An old quote[/quote] @[userlink]Nobody Either[/userlink]"),
                    Is.True,
                    $"The reply failed ({page.Url})");
            });
    }

    /// <summary>
    /// The message history open in two tabs: comparing the versions in the first tab shows the versions of
    /// its message (the page took the details of the versions from the session, which contained the list of
    /// the tab opened last: wrong editors and edit reasons, or an error with fewer versions).
    /// </summary>
    [Test]
    [Description("Compare versions with the history of another message open in a second tab")]
    public async Task MessageHistoryInTwoTabsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var id = Guid.NewGuid().ToString("N");

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");

                var topicUrl = page.Url;

                // posts a reply and edits it with the given edit reasons
                async Task<string> PostEditedReplyAsync(string text, params string[] reasons)
                {
                    await page.GotoAsync(topicUrl);
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'ReplyLink')]").First);

                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync(text);

                    await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                    var match = System.Text.RegularExpressions.Regex.Match(page.Url, @"/Post/(\d+)/|[?&]m=(\d+)");
                    var messageId = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;

                    Assert.That(messageId, Is.Not.Empty, $"The id of the reply is not in {page.Url}");

                    foreach (var reason in reasons)
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditMessage/{messageId}");

                        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync($"{text} {reason}");
                        await page.Locator("#Input_ReasonEditor").FillAsync(reason);

                        await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));
                    }

                    return messageId;
                }

                // the first message has 3 versions (original, first edit, current), the second 2
                var firstId = await PostEditedReplyAsync($"First history {id}", $"ReasonA1{id}", $"ReasonA2{id}");
                var secondId = await PostEditedReplyAsync($"Second history {id}", $"ReasonB{id}");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MessageHistory?m={firstId}");

                Assert.That(
                    await page.Locator("input[type=checkbox][name$='.Selected']").CountAsync(),
                    Is.EqualTo(3),
                    "The history of the first message is not shown");

                // the history of the second message in another tab (same session)
                var secondTab = await page.Context.NewPageAsync();

                try
                {
                    await secondTab.GotoAsync($"{this.TestSettings.TestForumUrl}MessageHistory?m={secondId}");

                    Assert.That(
                        await secondTab.ContentAsync(), Does.Contain($"ReasonB{id}"),
                        "The history of the second message is not shown");
                }
                finally
                {
                    await secondTab.CloseAsync();
                }

                // compare the first and the last version in the first tab
                var checkBoxes = page.Locator("input[type=checkbox][name$='.Selected']");

                await checkBoxes.Nth(0).CheckAsync();
                await checkBoxes.Nth(2).CheckAsync();

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator("button[formaction*='ShowDiff']").ClickAsync(),
                                   r => r.Request.Method == "POST" && r.Url.Contains("ShowDiff"));

                await page.WaitForLoadStateAsync(LoadState.Load);

                Assert.That(response.Status, Is.EqualTo(200), "Comparing the versions failed");

                var content = await page.ContentAsync();

                // a version shows the reason of the edit that replaced it
                Assert.That(content, Does.Contain($"ReasonA2{id}"), "The versions of the first message are not shown");
                Assert.That(content, Does.Not.Contain($"ReasonB{id}"), "The versions of the second message are shown");

                Assert.That(
                    await page.Locator("#diffContent").InnerTextAsync(),
                    Does.Contain($"First history {id}").And.Contain($"ReasonA2{id}"),
                    "The changes are not shown");
            });
    }

    /// <summary>
    /// A member can't delete the own post of a locked topic: the delete page and its handler are denied
    /// (the page was shown, deleting just showed it again without the message).
    /// </summary>
    [Test]
    [Description("Deleting the own post of a locked topic is denied")]
    public async Task DeleteMessageOfLockedTopicIsDeniedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");

                var topicUrl = page.Url;

                var deleteLink = await page.Locator("a[href*='DeleteMessage']").First.GetAttributeAsync("href");

                Assert.That(deleteLink, Is.Not.Null, "The delete link of the own post is missing");

                var deleteUrl = new Uri(new Uri(this.TestSettings.TestForumUrl), deleteLink).ToString();

                // an admin in an own browser locks the topic
                await using var otherContext = await this.Fixture.Context.Browser!.NewContextAsync(
                                                   new BrowserNewContextOptions
                                                       {
                                                           BaseURL = this.TestSettings.TestForumUrl,
                                                           IgnoreHTTPSErrors = true
                                                       });

                var adminPage = await otherContext.NewPageAsync();

                Assert.That(
                    await adminPage.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await adminPage.GotoAsync(topicUrl);

                await adminPage.RunAndWaitForNextPageAsync(
                    () => adminPage.EvaluateAsync("() => document.querySelector(\"button[formaction*='LockTopic']\").click()"));

                Assert.That(
                    await adminPage.Locator("button[formaction*='UnlockTopic']").CountAsync(),
                    Is.GreaterThan(0),
                    "The topic was not locked");

                // the member opens the delete page
                await page.GotoAsync(deleteUrl);

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("You tried to enter an area where you didn't have access."),
                    "The delete page of a post in a locked topic is shown");

                // the delete handler, sent with a valid antiforgery token of the topic page
                await page.GotoAsync(topicUrl);

                var url = await page.EvaluateAsync<string>(
                    """
                    async url => {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                        const body = new URLSearchParams({ '__RequestVerificationToken': token, 'ReasonEditor': 'test' });
                        const response = await fetch(url, { method: 'POST', body: body });
                        return response.url;
                    }
                    """,
                    $"{deleteUrl}{(deleteUrl.Contains('?') ? "&" : "?")}handler=Delete");

                Assert.That(url, Does.Not.Contain("DeleteMessage"), "The delete handler shows the page again");

                await page.GotoAsync(topicUrl);

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("This is a Test Message Created by an automated Unit Test"),
                    "The post of the locked topic was deleted");
            });
    }

    /// <summary>
    /// An edit reason with characters that are html encoded (apostrophes, umlauts) is saved: the encoded
    /// reason was longer than the column (100) and the edit failed, except on Sqlite.
    /// </summary>
    [Test]
    [Description("Edit reason with special characters up to the maximum length")]
    public async Task EditReasonWithSpecialCharactersTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var id = Guid.NewGuid().ToString("N")[..8];

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "Topic Creating failed");

                var editLink = await page.Locator("a[href*='EditMessage']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "The edit link is missing");

                // 100 characters (the length of the field), 150 html encoded
                var reason = $"Edit {id} {string.Concat(System.Linq.Enumerable.Repeat("it's ", 17))}".PadRight(100, 'x')[..100];

                await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), editLink).ToString());

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync($"Edited {id}");
                await page.Locator("#Input_ReasonEditor").FillAsync(reason);

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync(),
                                   r => r.Request.Method == "POST");

                await page.WaitForLoadStateAsync(LoadState.Load);

                Assert.That(response.Status, Is.LessThan(500), "Saving the edit failed");

                Assert.That(await page.Locator("body").InnerTextAsync(), Does.Contain($"Edited {id}"), "The edit was not saved");

                // the reason is shown in the history of the message
                var messageId = System.Text.RegularExpressions.Regex.Match(editLink, @"(\d+)\D*$").Groups[1].Value;

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MessageHistory?m={messageId}");

                Assert.That(
                    await page.Locator("body").InnerTextAsync(),
                    Does.Contain($"Edit {id} it"),
                    "The edit reason was not saved");
            });
    }
}
