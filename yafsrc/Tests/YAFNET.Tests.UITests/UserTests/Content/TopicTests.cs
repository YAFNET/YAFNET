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

                var forumId = await page.ForumIdOnIndexAsync(this.TestSettings, forumName);

                Assert.That(forumId, Is.Not.Null, "The forum was not created");

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
    /// An attachment is shown with a signed download link in the posts of its owner, the id alone is not enough to
    /// download it and another user can't show it in a post: any attachment could be downloaded by counting up the id.
    /// </summary>
    [Test]
    public async Task AttachmentNeedsSignedLinkTest()
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

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                // the admin uploads an image
                var image = Convert.ToBase64String(
                    await System.IO.File.ReadAllBytesAsync(System.IO.Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png")));

                var attachmentId = await page.EvaluateAsync<int>(
                    """
                    async ([baseUrl, image]) => {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                        const bytes = Uint8Array.from(atob(image), c => c.charCodeAt(0));
                        const data = new FormData();
                        data.append('file', new File([bytes], 'signed-' + Date.now() + '.png', { type: 'image/png' }));
                        const response = await fetch(baseUrl + 'api/FileUpload/Upload', {
                            method: 'POST', headers: { 'RequestVerificationToken': token }, body: data });
                        return (await response.json())[0].fileID;
                    }
                    """,
                    new object[] { this.TestSettings.TestForumUrl, image });

                Assert.That(attachmentId, Is.GreaterThan(0), "The attachment was not uploaded");

                var attachmentLinks = $"[href*='attachmentId={attachmentId}&'], [src*='attachmentId={attachmentId}&']";

                // the post of the owner shows the attachment with a signed link
                var topicId = await page.CreateTopicAsync(
                                  this.TestSettings,
                                  this.TestSettings.TestForumId,
                                  $"Attachment topic {Random.Shared.Next()}",
                                  $"[attach]{attachmentId}[/attach]");

                Assert.That(topicId, Is.Not.Null, "The topic was not created");

                var link = await page.Locator(attachmentLinks).First.EvaluateAsync<string>("e => e.href || e.src");

                Assert.That(link, Does.Contain("sig="), "The link is not signed");

                const string status = "async url => (await fetch(url)).status";

                Assert.That(await page.EvaluateAsync<int>(status, link), Is.EqualTo(200), "The signed link does not work");
                Assert.That(
                    await page.EvaluateAsync<int>(status, $"{this.TestSettings.TestForumUrl}api/Attachments/GetAttachment?attachmentId={attachmentId}&editor=false"),
                    Is.EqualTo(404),
                    "The attachment is served with its id alone");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // another user shows the attachment in a post
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                var otherTopicId = await page.CreateTopicAsync(
                                       this.TestSettings,
                                       this.TestSettings.TestForumId,
                                       $"Foreign attachment {Random.Shared.Next()}",
                                       $"Look: [attach]{attachmentId}[/attach]");

                Assert.That(otherTopicId, Is.Not.Null, "The topic was not created");
                Assert.That(await page.Locator(attachmentLinks).CountAsync(), Is.Zero, "The attachment of another user is shown");
            });
    }

    /// <summary>
    /// A guest can't post with the name of a member (also in other case): the post looked like a post of the member.
    /// </summary>
    [Test]
    public async Task GuestCanNotUseMemberNameTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Guest Name Forum {Random.Shared.Next()}";

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
                await page.Locator("#Input_Description").FillAsync("A forum of the guest name test");
                await page.Locator("#Input_SortOrder").FillAsync("99");

                await page.GetByLabel("Guest").First.SelectOptionAsync(new SelectOptionValue { Label = "Member Access" });

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

                var editLink = await page.Locator(".forum-row", new PageLocatorOptions { HasText = forumName })
                                   .Locator("a[href*='EditForum/false/']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "The forum was not created");

                var forumId = Regex.Match(editLink, @"EditForum/false/(\d+)").Groups[1].Value;

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // a guest address of its own (the flood delay of guests is checked per address, other tests post as guest)
                await page.SetExtraHTTPHeadersAsync(
                    new Dictionary<string, string> { ["X-Real-IP"] = $"10.66.{Random.Shared.Next(1, 255)}.{Random.Shared.Next(1, 255)}" });

                // the name of the admin (in other case)
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{forumId}");

                await page.Locator("#Input_From").FillAsync(this.TestSettings.AdminUserName.ToUpperInvariant());
                await page.Locator("//input[contains(@id,'_TopicSubject')]").First.FillAsync($"Guest name topic {Random.Shared.Next()}");
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic of a guest");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("This name belongs to a registered member, please enter another name."),
                    "The name of the member was accepted");
                Assert.That(page.Url, Does.Contain("PostTopic"), "The topic was created");

                // another name
                await page.Locator("#Input_From").FillAsync($"Guest Writer {Random.Shared.Next()}");
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic of a guest");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(page.Url, Does.Not.Contain("PostTopic"), "The topic with another name was not created");

                await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string>());
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

                var forumId = await page.ForumIdOnIndexAsync(this.TestSettings, forumName);

                Assert.That(forumId, Is.Not.Null, "The forum was not created");

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

                var topicId = Regex.Match(page.Url, @"/topic/(\d+)").Groups[1].Value;

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
            });
    }

    /// <summary>
    /// The flood delay of guests is checked per address: all guests are the same user, the post of one guest
    /// blocked all other guests.
    /// </summary>
    [Test]
    public async Task GuestFloodDelayPerAddressTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Guest Flood Forum {Random.Shared.Next()}";

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
                await page.Locator("#Input_Description").FillAsync("A forum of the guest flood test");
                await page.Locator("#Input_SortOrder").FillAsync("99");

                await page.GetByLabel("Guest").First.SelectOptionAsync(new SelectOptionValue { Label = "Member Access" });

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                var forumId = await page.ForumIdOnIndexAsync(this.TestSettings, forumName);

                Assert.That(forumId, Is.Not.Null, "The forum was not created");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                async Task PostAsGuestAsync(string ip)
                {
                    await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string> { ["X-Real-IP"] = ip });

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{forumId}");

                    await page.Locator("#Input_From").FillAsync($"Guest {ip}");
                    await page.Locator("//input[contains(@id,'_TopicSubject')]").First.FillAsync($"Guest flood topic {Random.Shared.Next()}");
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                    await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic of a guest");

                    await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));
                }

                await PostAsGuestAsync("10.80.1.1");
                Assert.That(page.Url, Does.Not.Contain("PostTopic"), $"The first guest can't post ({page.Url})");

                // another guest at once
                await PostAsGuestAsync("10.80.2.2");
                Assert.That(page.Url, Does.Not.Contain("PostTopic"), $"The other guest has to wait for the first guest ({page.Url})");

                // the first guest again
                await PostAsGuestAsync("10.80.1.1");
                Assert.That(await page.ContentAsync(), Does.Contain("You can't post in the next"), "The flood delay of the guest is not checked");

                await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string>());
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

                var forumId = await page.ForumIdOnIndexAsync(this.TestSettings, forumName);

                Assert.That(forumId, Is.Not.Null, "The forum was not created");

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
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}forum/{forumId}/forum");

                Assert.That(
                    await page.Locator("a[href*='/topic/']", new PageLocatorOptions { HasText = topicName }).CountAsync(),
                    Is.Zero,
                    "The topic is shown before the approval");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Moderate/UnapprovedPosts/{forumId}");

                Assert.That(await page.ContentAsync(), Does.Contain(topicName), "The topic is not in the unapproved posts");

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Approve']").First);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}forum/{forumId}/forum");

                var topicLink = page.Locator("a[href*='/topic/']", new PageLocatorOptions { HasText = topicName });

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
    /// A post is approved once: approving it again (a second moderator, the button sent twice) counted the post
    /// again for the user and sent the notifications again.
    /// </summary>
    [Test]
    public async Task ApprovedPostIsNotApprovedAgainTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Moderated Twice Forum {Random.Shared.Next()}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a moderated forum (all posts)
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the approval test");
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

                var forumId = await page.ForumIdOnIndexAsync(this.TestSettings, forumName);

                Assert.That(forumId, Is.Not.Null, "The forum was not created");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // a new user posts a topic that needs the approval
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{forumId}");

                await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Approve twice {Random.Shared.Next()}");
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic which needs the approval");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Moderate/UnapprovedPosts/{forumId}");

                var approve = page.Locator("button[formaction*='Approve']").First;
                var approveAction = await approve.GetAttributeAsync("formaction");

                await page.SubmitAndWaitAsync(approve);

                // the same approval again
                await page.GotoAsync(this.TestSettings.TestForumUrl);

                await page.RunAndWaitForNextPageAsync(
                    () => page.EvaluateAsync(
                        """
                        action => {
                            const form = document.createElement('form');
                            form.method = 'post';
                            form.action = action;
                            form.appendChild(document.querySelector("input[name='__RequestVerificationToken']").cloneNode());
                            document.body.appendChild(form);
                            form.submit();
                        }
                        """,
                        approveAction));

                // the user has one post
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}members");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();
                await page.Locator("#Input_UserSearchName").FillAsync(userName);
                await page.Locator("#Input_NumPosts").FillAsync("1");
                await page.EvaluateAsync("() => document.querySelector('#Input_NumPostValue').value = '1'");

                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Search')]"));

                Assert.That(
                    await page.Locator("li.member-row", new PageLocatorOptions { HasTextString = userName }).CountAsync(),
                    Is.EqualTo(1),
                    "The post was counted again");
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

                Assert.That(page.Url, Does.Contain("/topic/"), "The topic was not created");
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
                                       $"{this.TestSettings.TestForumUrl}MyTopics/LoadMoreTopics?pageIndex=0&size=10&{query}");

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

                Assert.That(page.Url, Does.Contain("/topic/"), "The topic was not created");

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

                Assert.That(page.Url, Does.Contain("/topic/"), "The topic was not created");

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

                    Assert.That(otherPage.Url, Does.Contain("/topic/"), $"The topic is not shown ({otherPage.Url})");

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

                        Assert.That(page.Url, Does.Contain("/topic/"), "The topic was not created");

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

    /// <summary>
    /// The dialog "move topic" of a topic shows the select of the forums: the functions of the dialogs were in a
    /// script module (not global), the dialog failed with "dialogFunctions is not a function".
    /// </summary>
    [Test]
    public async Task MoveTopicDialogShowsTheForumsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var errors = new List<string>();

                page.PageError += (_, error) => errors.Add(error);

                var topicId = await page.CreateTopicAsync(
                                  this.TestSettings,
                                  this.TestSettings.TestForumId,
                                  $"Move dialog {Random.Shared.Next()}",
                                  "A topic for the move dialog");

                Assert.That(topicId, Is.Not.Null, "The topic was not created");

                // the button is in the menu of the moderation (closed)
                await page.Locator("[data-url*='MoveTopic']").First.EvaluateAsync("b => b.click()");

                var dialog = page.Locator(".modal.show");

                await Assertions.Expect(dialog).ToBeVisibleAsync();
                await Assertions.Expect(dialog.Locator(".choices")).ToBeVisibleAsync();

                Assert.That(errors, Is.Empty, "The dialog failed with an error");
            });
    }

    /// <summary>
    /// The page size and the date filter of the topic list keep each other (a change of the page size showed
    /// all topics again, a change of the date filter reset the page size).
    /// </summary>
    [Test]
    public async Task TopicListFiltersKeepEachOtherTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}forum/{this.TestSettings.TestForumId}/test");

                // the date filter is a Choices.js select, the page size a select (both are in the hidden filter menu)
                const string Choose = """
                                      ([id, value]) => {
                                          const select = document.getElementById(id);
                                          if (![...select.options].some(o => o.value === value)) select.add(new Option(value, value));
                                          select.value = value;
                                          select.form.submit();
                                      }
                                      """;

                await page.RunAndWaitForNextPageAsync(() => page.EvaluateAsync(Choose, new[] { "ShowTopicListSelected", "3" }));
                await page.RunAndWaitForNextPageAsync(() => page.EvaluateAsync(Choose, new[] { "Size", "50" }));

                Assert.That(
                    await page.Locator("#ShowTopicListSelected").InputValueAsync(),
                    Is.EqualTo("3"),
                    "The date filter was reset by the page size");

                await page.RunAndWaitForNextPageAsync(() => page.EvaluateAsync(Choose, new[] { "ShowTopicListSelected", "2" }));

                Assert.That(
                    await page.Locator("#Size").InputValueAsync(),
                    Is.EqualTo("50"),
                    "The page size was reset by the date filter");
            });
    }

    /// <summary>
    /// The topic list of a forum uses the date filter that is set as default in the board settings
    /// (all topics were shown).
    /// </summary>
    [Test]
    public async Task TopicListUsesTheDefaultDateFilterTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                try
                {
                    await SetShowTopicsDefaultAsync(page, this.TestSettings, "3");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}forum/{this.TestSettings.TestForumId}/test");

                    Assert.That(
                        await page.Locator("#ShowTopicListSelected").InputValueAsync(),
                        Is.EqualTo("3"),
                        "The default date filter of the board is not used");
                }
                finally
                {
                    await SetShowTopicsDefaultAsync(page, this.TestSettings, "0");
                }
            });
    }

    /// <summary>
    /// Sets the default date filter of the topic lists in the board settings.
    /// </summary>
    private async static Task SetShowTopicsDefaultAsync(IPage page, TestConfig testSettings, string value)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/Settings");

        await page.Locator("#Input_ShowTopic").EvaluateAsync(
            """
            (select, value) => {
                if (![...select.options].some(o => o.value === value)) select.add(new Option(value, value));
                select.value = value;
            }
            """,
            value);

        var save = page.Locator("button[formaction*='Save']").First;

        await page.RunAndWaitForNextPageAsync(() => save.EvaluateAsync("b => b.form.requestSubmit(b)"));

        await page.WaitForLoadStateAsync();
    }

    /// <summary>
    /// A max post size of 0 is unlimited: the editor takes the typed text (every key was undone).
    /// </summary>
    [Test]
    public async Task EditorWithoutMaxPostSizeTakesTheTextTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await SetMaxPostSizeAsync(page, this.TestSettings, "0");

                try
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

                    var body = page.FrameLocator(".sceditor-container >> iframe").Locator("body");

                    await body.ClickAsync();
                    await page.Keyboard.TypeAsync("Unlimited text");

                    Assert.That(await body.InnerTextAsync(), Does.Contain("Unlimited text"), "The editor did not take the text");
                }
                finally
                {
                    await SetMaxPostSizeAsync(page, this.TestSettings, "32767");
                }
            });
    }

    /// <summary>
    /// Sets the max post size (0 is unlimited).
    /// </summary>
    private async static Task SetMaxPostSizeAsync(IPage page, TestConfig testSettings, string value)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/HostSettings");

        await page.Locator("#Input_MaxPostSize").EvaluateAsync("(input, value) => input.value = value", value);

        var save = page.Locator("button[formaction*='Save']").First;

        await page.RunAndWaitForNextPageAsync(() => save.EvaluateAsync("b => b.form.requestSubmit(b)"));

        await page.WaitForLoadStateAsync();
    }

    /// <summary>
    /// The subject of a new topic lists the topics with a similar title (a script error was shown instead).
    /// </summary>
    [Test]
    public async Task NewTopicListsSimilarTopicsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var word = $"Similar{Random.Shared.Next()}";

                var topicId = await page.CreateTopicAsync(this.TestSettings, this.TestSettings.TestForumId, $"{word} topic", "A topic of the similar topics test");

                Assert.That(topicId, Is.Not.Null, $"The topic was not created ({page.Url})");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

                await page.Locator(".searchSimilarTopics").PressSequentiallyAsync(word);

                var placeHolder = page.Locator("#SearchResultsPlaceholder");

                await Assertions.Expect(placeHolder.Locator("a", new LocatorLocatorOptions { HasText = word })).ToBeVisibleAsync();

                Assert.That(await placeHolder.InnerTextAsync(), Does.Not.Contain("Error"), "The similar topics show an error");
            });
    }

    /// <summary>
    /// Enter in the subject of a new topic posts the topic, Enter in the reason of an edit saves the edit
    /// (both showed the preview, the first button of the form).
    /// </summary>
    [Test]
    public async Task EnterInTheEditorFormsSavesTest()
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

                var body = page.FrameLocator(".sceditor-container >> iframe").Locator("body");

                await body.ClickAsync();
                await body.FillAsync("A topic of the enter test");

                var subject = page.Locator("//input[contains(@id,'_TopicSubject')]");

                await subject.FillAsync($"Enter test {Random.Shared.Next()}");

                await page.RunAndWaitForNextPageAsync(() => subject.PressAsync("Enter"));

                Assert.That(page.Url, Does.Contain("/topic/"), "Enter in the subject did not post the topic");

                var editLink = await page.Locator("a[href*='EditMessage']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "Edit link of the message not found");

                await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), editLink).ToString());

                var reason = page.Locator("#Input_ReasonEditor");

                await reason.FillAsync("Enter test");

                await page.RunAndWaitForNextPageAsync(() => reason.PressAsync("Enter"));

                Assert.That(page.Url, Does.Not.Contain("EditMessage"), "Enter in the reason did not save the edit");
            });
    }

    /// <summary>
    /// A new topic without subject gets the message of the topic subject, the preview has the preview title
    /// (the texts of the email topic page and the delete page were shown).
    /// </summary>
    [Test]
    public async Task NewTopicTextsTest()
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

                var body = page.FrameLocator(".sceditor-container >> iframe").Locator("body");

                await body.ClickAsync();
                await body.FillAsync("A topic of the texts test");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'Preview')]"));

                Assert.That(await page.ContentAsync(), Does.Contain("Preview:"), "The preview has the wrong title");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                var content = await page.ContentAsync();

                Assert.That(content, Does.Contain("Please enter a subject for the topic."), "The message of the missing subject is wrong");
                Assert.That(content, Does.Not.Contain("You must enter an email subject."), "The message of the email topic page is shown");
            });
    }

    /// <summary>
    /// The print page has the name of the topic as title (the page had the title "Reset Password").
    /// </summary>
    [Test]
    public async Task PrintTopicTitleTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var subject = $"Print title {Random.Shared.Next()}";

                var topicId = await page.CreateTopicAsync(
                                  this.TestSettings,
                                  this.TestSettings.TestForumId,
                                  subject,
                                  "A topic for the print page");

                Assert.That(topicId, Is.Not.Null, "The topic was not created");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PrintTopic/{topicId}");

                var title = await page.TitleAsync();

                Assert.That(title, Does.Contain(subject), "The print page has not the name of the topic as title");
                Assert.That(title, Does.Not.Contain("Reset Password"), "The print page has the title of another page");
            });
    }

    /// <summary>
    /// Scrolling down the topic list loads the next topics (the page index was sent as the query value "page",
    /// which is the route value of the razor pages, every request loaded the first page again and the list
    /// stopped after the first page).
    /// </summary>
    [Test]
    public async Task TopicListLoadsTheNextTopicsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var forumUrl = $"{this.TestSettings.TestForumUrl}forum/{this.TestSettings.TestForumId}/test";

                await page.GotoAsync(forumUrl);

                const string Choose = """
                                      ([id, value]) => {
                                          const select = document.getElementById(id);
                                          if (![...select.options].some(o => o.value === value)) select.add(new Option(value, value));
                                          select.value = value;
                                          select.form.submit();
                                      }
                                      """;

                // all topics, 5 per page
                await page.RunAndWaitForNextPageAsync(() => page.EvaluateAsync(Choose, new[] { "ShowTopicListSelected", "0" }));
                await page.RunAndWaitForNextPageAsync(() => page.EvaluateAsync(Choose, new[] { "Size", "5" }));

                try
                {
                    // the forum needs more topics than one page
                    var total = int.Parse(await page.Locator("#topicsContainer").GetAttributeAsync("data-total") ?? "0");

                    for (var i = total; i < 7; i++)
                    {
                        Assert.That(
                            await page.CreateTopicAsync(
                                this.TestSettings,
                                this.TestSettings.TestForumId,
                                $"Scroll topic {Random.Shared.Next()}",
                                "A topic for the next page of the list"),
                            Is.Not.Null,
                            "The topic was not created");
                    }

                    await page.GotoAsync(forumUrl);

                    var rows = page.Locator("#topicsContainer > [data-topic-id]");
                    var firstPage = await rows.CountAsync();

                    // the end of the list (the loader is removed when all topics are loaded)
                    await page.EvaluateAsync("() => window.scrollTo(0, document.body.scrollHeight)");

                    await Assertions.Expect(rows).Not.ToHaveCountAsync(firstPage);

                    var ids = await rows.EvaluateAllAsync<string[]>("rows => rows.map(r => r.dataset.topicId)");

                    Assert.That(ids, Is.Unique, "A topic of the first page was loaded again");
                }
                finally
                {
                    await page.GotoAsync(forumUrl);
                    await page.RunAndWaitForNextPageAsync(() => page.EvaluateAsync(Choose, new[] { "Size", "50" }));
                }
            });
    }

    /// <summary>
    /// The tags input of a new topic keeps the focus and the typed keys while the tags of the search are loaded
    /// (the third key loads the tags from the server, the input was disabled and lost the focus).
    /// </summary>
    [Test]
    public async Task TagsInputKeepsTheFocusWhileTheTagsLoadTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // the answer of the tag search is held back until the keys are typed
                var release = new TaskCompletionSource();

                await page.RouteAsync(
                    "**/api/Tags/GetBoardTags**",
                    async route =>
                    {
                        await release.Task;
                        await route.ContinueAsync();
                    });

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

                var input = page.Locator(".choices:has(#Tags) input.choices__input--cloned");

                await input.ClickAsync();

                var request = page.WaitForRequestAsync("**/api/Tags/GetBoardTags**");

                await input.PressSequentiallyAsync("tag");
                await request;

                // keys typed while the tags are loading
                await page.Keyboard.TypeAsync("ged");

                await Assertions.Expect(input).ToBeFocusedAsync();
                await Assertions.Expect(input).ToHaveValueAsync("tagged");

                release.SetResult();

                await Assertions.Expect(page.Locator(".choices:has(#Tags)")).Not.ToHaveClassAsync(new System.Text.RegularExpressions.Regex("is-loading"));
                await Assertions.Expect(input).ToBeFocusedAsync();
                await Assertions.Expect(input).ToHaveValueAsync("tagged");
            });
    }
}
