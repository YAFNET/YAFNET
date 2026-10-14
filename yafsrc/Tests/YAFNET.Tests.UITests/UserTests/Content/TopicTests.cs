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
using System.Text.RegularExpressions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Content;

/// <summary>
/// The topic tests.
/// </summary>
public class TopicTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A guest keeps the entered name when the page is shown again with a warning,
    /// and a guest without a name posts with the name of the guest user (the page failed).
    /// </summary>
    [Test]
    public async Task GuestTopicNameTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Guest Forum {Random.Shared.Next()}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a forum where guests can post
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the guest test");
                await page.Locator("#Input_SortOrder").FillAsync("99");

                await page.GetByLabel("Guest").First.SelectOptionAsync(new SelectOptionValue { Label = "Member Access" });

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                var forumLink = await page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasText = forumName })
                                    .First.GetAttributeAsync("href");

                Assert.That(forumLink, Is.Not.Null, "The forum was not created");

                var forumId = Regex.Match(forumLink, @"/Topics/(\d+)/").Groups[1].Value;

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{forumId}");

                Assert.That(await page.Locator("#Input_From").CountAsync(), Is.EqualTo(1), "The guest can't post");

                // without subject: the warning, the entered name is kept
                await page.Locator("#Input_From").FillAsync("Guest Tester");
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic of a guest");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(await page.Locator("#Input_From").InputValueAsync(), Is.EqualTo("Guest Tester"), "The name of the guest was replaced");

                // without name
                await page.Locator("#Input_From").FillAsync(string.Empty);
                await page.Locator("//input[contains(@id,'_TopicSubject')]").First.FillAsync($"Guest topic {Random.Shared.Next()}");
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic of a guest");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(
                    await page.Locator("h1", new PageLocatorOptions { HasText = "Forum Error" }).CountAsync(),
                    Is.Zero,
                    $"The post of a guest without name failed ({page.Url})");
                Assert.That(page.Url, Does.Not.Contain("PostTopic"), "The topic was not created");
            });
    }

    /// <summary>
    /// All guests are the same user: a guest can only edit and delete the guest posts of the own address
    /// (like the buttons of the post), the pages allowed every guest to change all guest posts.
    /// </summary>
    [Test]
    public async Task GuestCanOnlyChangeOwnGuestPostsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Guest Edit Forum {Random.Shared.Next()}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a forum where guests can post, edit, delete and add polls (the default masks have no poll
                // access without moderator access)
                var maskName = $"Guest Poll Access {Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditAccessMask");

                await page.Locator("#Input_Name").FillAsync(maskName);
                await page.Locator("#Input_SortOrder").FillAsync("9");

                foreach (var access in new[] { "Read", "Post", "Reply", "Poll", "Edit", "Delete" })
                {
                    await page.Locator($"#Input_{access}Access").CheckAsync();
                }

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the guest edit test");
                await page.Locator("#Input_SortOrder").FillAsync("99");

                await page.GetByLabel("Guest").First.SelectOptionAsync(new SelectOptionValue { Label = maskName });

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                var forumLink = await page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasText = forumName })
                                    .First.GetAttributeAsync("href");

                Assert.That(forumLink, Is.Not.Null, "The forum was not created");

                var forumId = Regex.Match(forumLink, @"/Topics/(\d+)/").Groups[1].Value;

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // the first guest
                await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string> { ["X-Real-IP"] = "10.20.30.40" });

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{forumId}");

                await page.Locator("#Input_From").FillAsync("First Guest");
                await page.Locator("//input[contains(@id,'_TopicSubject')]").First.FillAsync($"Guest edit topic {Random.Shared.Next()}");
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic of the first guest");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                var messageId = await page.Locator(".message .selectionQuoteable").First.GetAttributeAsync("id");

                Assert.That(messageId, Is.Not.Null.And.Not.Empty, $"The topic was not created ({page.Url})");

                var topicId = Regex.Match(page.Url, @"/Posts/(\d+)").Groups[1].Value;

                Assert.That(topicId, Is.Not.Empty, $"The topic id was not found ({page.Url})");

                const string denied = "You tried to enter an area where you didn't have access.";
                var pollQuestion = page.Locator("//input[contains(@id,'Question')]");

                // another guest
                await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string> { ["X-Real-IP"] = "10.50.60.70" });

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditMessage/{messageId}");
                Assert.That(await page.ContentAsync(), Does.Contain(denied), "Another guest can edit the post");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}DeleteMessage/{messageId}");
                Assert.That(await page.ContentAsync(), Does.Contain(denied), "Another guest can delete the post");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PollEdit?t={topicId}");
                Assert.That(await pollQuestion.CountAsync(), Is.Zero, "Another guest can add a poll to the topic");

                // the guest of the post
                await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string> { ["X-Real-IP"] = "10.20.30.40" });

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditMessage/{messageId}");
                Assert.That(await page.ContentAsync(), Does.Not.Contain(denied), "The guest can't edit the own post");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}DeleteMessage/{messageId}");
                Assert.That(await page.ContentAsync(), Does.Not.Contain(denied), "The guest can't delete the own post");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PollEdit?t={topicId}");
                Assert.That(await pollQuestion.CountAsync(), Is.GreaterThan(0), "The guest can't add a poll to the own topic");

                await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string>());

                // the flood delay (30 seconds) is checked per user: all guests have to wait for the next guest post
                // (the next guest test)
                await Task.Delay(TimeSpan.FromSeconds(31));
            });
    }

    /// <summary>
    /// A topic in a moderated forum: the user gets the approval info (posting showed an error page,
    /// the notification of the moderators failed), the topic is shown after the approval.
    /// </summary>
    [Test]
    public async Task ModeratedTopicApprovalTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Moderated Forum {Random.Shared.Next()}";
                var topicName = $"Moderated topic {Random.Shared.Next()}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a moderated forum (all posts, the switch submits the form to show the options, set directly)
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the moderation test");
                await page.Locator("#Input_SortOrder").FillAsync("99");

                await page.EvaluateAsync(
                    """
                    () => {
                        const moderated = document.querySelector('#Input_Moderated');
                        moderated.checked = true;
                        const all = document.createElement('input');
                        all.type = 'hidden';
                        all.name = 'Input.ModerateAllPosts';
                        all.value = 'true';
                        moderated.form.appendChild(all);
                    }
                    """);

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                var forumLink = await page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasText = forumName })
                                    .First.GetAttributeAsync("href");

                Assert.That(forumLink, Is.Not.Null, "The forum was not created");

                var forumId = Regex.Match(forumLink, @"/Topics/(\d+)/").Groups[1].Value;

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // a new user (no posts, not a moderator)
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{forumId}");

                await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync(topicName);
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic which needs the approval");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("a forum moderator must approve your post"),
                    $"The approval info is not shown ({page.Url})");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // not shown before the approval
                await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), forumLink).ToString());

                Assert.That(
                    await page.Locator("a[href*='/Posts/']", new PageLocatorOptions { HasText = topicName }).CountAsync(),
                    Is.Zero,
                    "The topic is shown before the approval");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Moderate/UnapprovedPosts/{forumId}");

                Assert.That(await page.ContentAsync(), Does.Contain(topicName), "The topic is not in the unapproved posts");

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Approve']").First);

                await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), forumLink).ToString());

                var topicLink = page.Locator("a[href*='/Posts/']", new PageLocatorOptions { HasText = topicName });

                Assert.That(await topicLink.CountAsync(), Is.GreaterThan(0), "The topic is not shown after the approval");

                await topicLink.First.ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("A topic which needs the approval"),
                    $"The message of the approved topic is not shown ({page.Url})");
            });
    }

    /// <summary>
    /// Create the new topic in forum test.
    /// </summary>
    [Test]
    public async Task CreateNewTopicTest()
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
                        $"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Post New Topic"), "Post New Topic not possible");

                    // Create New Topic
                    await page.Locator("//input[contains(@id,'_TopicSubject')]")
                        .FillAsync($"Auto Created Test Topic - {DateTime.UtcNow}");

                    if (pageSource.Contains("Description"))
                    {
                        await page.Locator("//input[contains(@id,'_TopicDescription')]").FillAsync("Test Description");
                    }

                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
                        .FillAsync("This is a Test Message Created by an automated Unit Test");

                    // Post New Topic
                    await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                    pageSource = await page.ContentAsync();

                    // members have to wait between two posts (flood control), the test before may just have posted;
                    // the form keeps the entered values, send it again
                    if (!pageSource.Contains("Next Topic"))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(31));

                        await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                        pageSource = await page.ContentAsync();
                    }

                    Assert.That(pageSource, Does.Contain("Next Topic"), "Topic Creating failed");
                });
    }

    /// <summary>
    /// Delete topic test
    /// </summary>
    [Test]
    [Description("Delete a newly created topic")]
    public async Task DeleteTopicTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                await page.LoginUserAsync(
                    this.TestSettings,
                    this.TestSettings.AdminUserName,
                    this.TestSettings.AdminPassword), Is.True,
                "Login failed");

                // Do actual test

                // Go to Post New Topic
                await page.GotoAsync(
                $"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

                var pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Post New Topic"), "Post New Topic not possible");

                var topicTitle = $"Auto Created Test Topic for deleting - {DateTime.UtcNow}";

                // Create New Topic
                await page.Locator("//input[contains(@id,'_TopicSubject')]")
                .FillAsync(topicTitle);

                if (pageSource.Contains("Description"))
                {
                    await page.Locator("//input[contains(@id,'_TopicDescription')]").FillAsync("Test Description");
                }

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
                .FillAsync("This is a Test Message Created by an automated Unit Test");

                // Post New Topic
                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Next Topic"), "Topic Creating failed");

                // Delete Topic
                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Manage Topic" }).First.ClickAsync();

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Delete Topic" }).ClickAsync();

                await page.SubmitAndWaitAsync(page.Locator(".btn-success"));

                pageSource = await page.ContentAsync();

                Assert.That(pageSource.Contains(topicTitle), Is.False, "Topic Deleting failed");
                ////
            });
    }

    /// <summary>
    /// "Next topic" on the newest topic of a forum shows a message on the topic.
    /// </summary>
    [Test]
    public async Task NextTopicOnNewestTopicTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // a new user, no flood delay
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    var subject = $"Newest topic {Random.Shared.Next()}";

                    var topicId = await page.CreateTopicAsync(
                                      this.TestSettings,
                                      this.TestSettings.TestForumId,
                                      subject,
                                      "The newest topic of the forum");

                    Assert.That(topicId, Is.Not.Null, "The topic was not created");

                    await page.SubmitAndWaitAsync(page.Locator("#NextTopic"));

                    var content = await page.ContentAsync();

                    Assert.That(content, Does.Contain(subject), $"The topic is not shown ({page.Url})");
                    Assert.That(content, Does.Contain("No more topics in this forum."), "The message is missing");
                });
    }

    /// <summary>
    /// A topic with a priority that is not in the list (manipulated form) is created as a normal topic.
    /// </summary>
    [Test]
    public async Task TopicWithInvalidPriorityTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

                var subject = $"Priority test {Random.Shared.Next()}";

                await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync(subject);

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic with a manipulated priority");

                await page.Locator("select[name='Input.Priority']").EvaluateAsync(
                    "select => { select.add(new Option('x', '70000', true, true)); select.value = '70000'; }");

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync(),
                                   r => r.Request.Method == "POST" && r.Url.Contains("PostTopic"));

                Assert.That(response.Status, Is.LessThan(400), $"The topic was not created ({response.Status})");

                await page.WaitForLoadStateAsync(LoadState.Load);

                Assert.That(page.Url, Does.Contain("/Posts/"), "The topic was not created");
            });
    }

    /// <summary>
    /// The "since" and the mode of the topic lists of My Topics are sent by the client, values that are
    /// not in the list failed with an error.
    /// </summary>
    [Test]
    public async Task MyTopicsWithInvalidFilterTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.TestUserName, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                foreach (var query in new[] { "mode=0&show=2147483647", "mode=0&show=-2147483648", "mode=99&show=0" })
                {
                    var response = await page.GotoAsync(
                                       $"{this.TestSettings.TestForumUrl}MyTopics/LoadMoreTopics?page=0&size=10&{query}");

                    Assert.That(response?.Status, Is.EqualTo(200), $"The topic list failed for {query}");
                }
            });
    }

    /// <summary>
    /// The tags of a new topic are saved also when the user has disabled the activity stream
    /// (they were only saved with it).
    /// </summary>
    [Test]
    public async Task TopicTagsWithoutActivityStreamTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                // disable the activity stream
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                await page.Locator("#Activity").UncheckAsync();

                await page.SubmitAndWaitAsync(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).Last);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                Assert.That(await page.Locator("#Activity").IsCheckedAsync(), Is.False, "The activity stream is still enabled");

                var tag = $"tagtest{Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

                await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Tag test {Random.Shared.Next()}");

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic with a tag");

                await page.Locator("#Input_TagsValue").EvaluateAsync("(input, tag) => input.value = tag", tag);

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(page.Url, Does.Contain("/Post"), "The topic was not created");

                Assert.That(await page.ContentAsync(), Does.Contain(tag), "The tag of the topic was not saved");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
            });
    }

    /// <summary>
    /// A long tag is saved (MySQL created the column with 255 characters,
    /// the topic was created but the user got the error page).
    /// </summary>
    [Test]
    public async Task LongTopicTagIsSavedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                var tag = $"longtag{Random.Shared.Next()}{new string('t', 300)}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

                await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Long tag test {Random.Shared.Next()}");

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic with a long tag");

                await page.Locator("#Input_TagsValue").EvaluateAsync("(input, tag) => input.value = tag", tag);

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(await page.ContentAsync(), Does.Not.Contain("Forum Error"), "The error page is shown");

                Assert.That(page.Url, Does.Contain("/Post"), "The topic was not created");

                Assert.That(await page.ContentAsync(), Does.Contain(tag), "The tag of the topic was not saved");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
            });
    }

    /// <summary>
    /// The users reading a topic are listed ("Users browsing this topic", only shown to admins): the flags of the
    /// user were read as the flags of the visit, an approved user was a guest and the list was not shown.
    /// </summary>
    [Test]
    public async Task TopicBrowsersAreListedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    // the new user creates and reads the topic
                    var topicId = await page.CreateTopicAsync(
                                      this.TestSettings,
                                      this.TestSettings.TestForumId,
                                      $"Browsed topic {Random.Shared.Next()}",
                                      "A topic of the active users test");

                    Assert.That(topicId, Is.Not.Null, $"The topic was not created ({page.Url})");

                    var topicUrl = page.Url;

                    // an admin in an own browser (the list is only shown to admins)
                    await using var otherContext = await this.Fixture.Context.Browser!.NewContextAsync(
                                                       new BrowserNewContextOptions
                                                           {
                                                               BaseURL = this.TestSettings.TestForumUrl,
                                                               IgnoreHTTPSErrors = true
                                                           });

                    var otherPage = await otherContext.NewPageAsync();

                    Assert.That(
                        await otherPage.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    await otherPage.GotoAsync(topicUrl);

                    Assert.That(otherPage.Url, Does.Contain("/Posts/"), $"The topic is not shown ({otherPage.Url})");

                    var browsers = otherPage.Locator(".card", new PageLocatorOptions { HasText = "Users browsing this topic" });

                    Assert.That(
                        await browsers.Last.InnerTextAsync(),
                        Does.Contain(userName),
                        "The user reading the topic is not listed");
                });
    }

    /// <summary>
    /// A plain url in a post becomes a link and the text before it on the line is kept (it was removed).
    /// </summary>
    [Test]
    public async Task PostWithPlainUrlKeepsTextTest()
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

                    var marker = $"Please read {Random.Shared.Next()}";

                    var topicId = await page.CreateTopicAsync(
                                      this.TestSettings,
                                      this.TestSettings.TestForumId,
                                      $"Plain url topic {Random.Shared.Next()}",
                                      $"{marker} https://example.com/page for the details");

                    Assert.That(topicId, Is.Not.Null, $"The topic was not created ({page.Url})");

                    var post = page.Locator(".message-container, .card-body", new PageLocatorOptions { HasText = "for the details" }).First;

                    Assert.That(await post.InnerTextAsync(), Does.Contain(marker), "The text before the url is missing");
                    Assert.That(
                        await post.Locator("a[href='https://example.com/page']").CountAsync(),
                        Is.GreaterThan(0),
                        "The url is no link");
                });
    }

    /// <summary>
    /// The style of a topic title (set by moderators) can't add attributes to the last topic link of the
    /// forum list (it was written there as html).
    /// </summary>
    [Test]
    public async Task TopicStyleCanNotAddAttributesTest()
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

                    await SetStyledTopicTitlesAsync(page, true);

                    try
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

                        await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Styled topic {Random.Shared.Next()}");
                        await page.Locator("#Input_TopicStyles").FillAsync("color:red\" onmouseover=\"window.styleXss=1\" data-x=\"");

                        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A styled topic");

                        await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                        Assert.That(page.Url, Does.Contain("/Posts/"), "The topic was not created");

                        await page.GotoAsync(this.TestSettings.TestForumUrl);

                        Assert.That(
                            await page.Locator("a[onmouseover]").CountAsync(),
                            Is.Zero,
                            "The style of the topic added an attribute to a link");
                    }
                    finally
                    {
                        await SetStyledTopicTitlesAsync(page, false);
                    }
                });

        async Task SetStyledTopicTitlesAsync(IPage page, bool enabled)
        {
            await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings");

            await page.Locator("#Input_UseStyledTopicTitles").EvaluateAsync($"input => input.checked = {(enabled ? "true" : "false")}");

            await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Save')]").First);
        }
    }
}
