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

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Content;

/// <summary>
/// The forum access tests.
/// </summary>
public class ForumAccessTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A user who can start topics in a forum, but not reply (access mask with post, without reply access),
    /// can't reply by opening the reply page directly.
    /// </summary>
    [Test]
    public async Task PostAccessWithoutReplyAccessCanNotReplyTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var maskName = $"PostNoReply{Random.Shared.Next()}";
                var forumName = $"Post Only Forum {maskName}";

                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // the access mask: read and post, no reply
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditAccessMask");

                await page.Locator("#Input_Name").FillAsync(maskName);
                await page.Locator("#Input_ReadAccess").CheckAsync();
                await page.Locator("#Input_PostAccess").CheckAsync();
                await page.Locator("#Input_ReplyAccess").UncheckAsync();

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                // a new forum with this access mask for the registered users
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the forum access test");
                await page.Locator("#Input_SortOrder").FillAsync("99");

                await page.GetByLabel("Registered").First.SelectOptionAsync(new SelectOptionValue { Label = maskName });

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                // the user starts a topic in the forum
                var forumLink = await page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasText = forumName })
                                    .First.GetAttributeAsync("href");

                Assert.That(forumLink, Is.Not.Null, "The forum is not shown to the user");

                var forumId = Regex.Match(forumLink, @"/Topics/(\d+)/").Groups[1].Value;

                var topicId = await page.CreateTopicAsync(
                                  this.TestSettings,
                                  int.Parse(forumId),
                                  "Topic without replies",
                                  "Users of this forum can start topics, but not reply");

                Assert.That(topicId, Is.Not.Null, $"The topic was not created ({page.Url})");

                // the reply page is denied
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostMessage?t={topicId}");

                Assert.That(
                    await page.Locator(".sceditor-container").CountAsync(),
                    Is.Zero,
                    "The reply page is shown without reply access");
            });
    }

    /// <summary>
    /// The post handlers of the topics page (page size, show list, ...) don't show a forum the user
    /// can't read (its name, description, moderators and users of a hidden forum).
    /// </summary>
    [Test]
    public async Task TopicsPostHandlersNeedReadAccessTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Hidden Topics Forum {Random.Shared.Next()}";
                var topicName = $"Hidden topic {Random.Shared.Next()}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a forum only for admins
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the forum access test");
                await page.Locator("#Input_SortOrder").FillAsync("99");
                await page.Locator("#Input_HideNoAccess").CheckAsync();

                await page.GetByLabel("Administrators").First.SelectOptionAsync(new SelectOptionValue { Label = "Admin Access" });
                await page.GetByLabel("Registered").First.SelectOptionAsync(new SelectOptionValue { Label = "No Access" });
                await page.GetByLabel("Guest").First.SelectOptionAsync(new SelectOptionValue { Label = "No Access" });

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                var forumLink = await page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasText = forumName })
                                    .First.GetAttributeAsync("href");

                Assert.That(forumLink, Is.Not.Null, "The forum is not shown to the admin");

                var forumId = Regex.Match(forumLink, @"/Topics/(\d+)/").Groups[1].Value;

                var topicId = await page.CreateTopicAsync(
                                  this.TestSettings,
                                  int.Parse(forumId),
                                  topicName,
                                  "A topic only admins can read");

                Assert.That(topicId, Is.Not.Null, $"The topic was not created ({page.Url})");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                // the page size form of a readable forum, sent to the hidden forum
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Topics/{this.TestSettings.TestForumId}/testForum");

                await page.RunAndWaitForNextPageAsync(
                    () => page.EvaluateAsync(
                        "forumId => { const form = document.querySelector('select#Size').form; form.action = `/Topics/${forumId}/x/1/PageSize`; form.submit(); }",
                        forumId));

                var pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Not.Contain(forumName), "The hidden forum is shown");
                Assert.That(pageSource, Does.Not.Contain(topicName), "The topics of a forum without read access are listed");
                Assert.That(
                    pageSource,
                    Does.Contain("You tried to enter an area where you didn't have access."),
                    "The access is not denied");
            });
    }

    /// <summary>
    /// A topic of a hidden forum can't be read with the forum and category ids of a readable forum in the
    /// address (the access was checked for the forum of the address, not for the forum of the topic).
    /// </summary>
    [Test]
    public async Task TopicWithIdsOfOtherForumNeedsReadAccessTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Hidden Print Forum {Random.Shared.Next()}";
                var topicName = $"Hidden print topic {Random.Shared.Next()}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a forum only for admins
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the forum access test");
                await page.Locator("#Input_SortOrder").FillAsync("99");
                await page.Locator("#Input_HideNoAccess").CheckAsync();

                await page.GetByLabel("Administrators").First.SelectOptionAsync(new SelectOptionValue { Label = "Admin Access" });
                await page.GetByLabel("Registered").First.SelectOptionAsync(new SelectOptionValue { Label = "No Access" });
                await page.GetByLabel("Guest").First.SelectOptionAsync(new SelectOptionValue { Label = "No Access" });

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                var forumLink = await page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasText = forumName })
                                    .First.GetAttributeAsync("href");

                Assert.That(forumLink, Is.Not.Null, "The forum is not shown to the admin");

                var forumId = Regex.Match(forumLink, @"/Topics/(\d+)/").Groups[1].Value;

                var topicId = await page.CreateTopicAsync(
                                  this.TestSettings,
                                  int.Parse(forumId),
                                  topicName,
                                  "A secret text only admins can read");

                Assert.That(topicId, Is.Not.Null, $"The topic was not created ({page.Url})");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                // the print page of the hidden topic with the ids of the readable test forum
                await page.GotoAsync(
                    $"{this.TestSettings.TestForumUrl}PrintTopic/{topicId}?f={this.TestSettings.TestForumId}&c=1");

                var pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Not.Contain("A secret text only admins can read"), "The hidden topic is printed");
                Assert.That(pageSource, Does.Not.Contain(topicName), "The hidden topic is shown");
            });
    }

    /// <summary>
    /// With the albums disabled (the default) the album images can't be uploaded with a post
    /// (only the get of the page was denied).
    /// </summary>
    [Test]
    public async Task AlbumUploadNeedsEnabledAlbumsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditAlbumImages/new");

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("You tried to enter an area where you didn't have access."),
                    "The album page is shown (the test needs the albums disabled)");

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                // the upload form of the album page with the token of the user
                await page.RunAndWaitForNextPageAsync(
                    () => page.EvaluateAsync(
                        """
                        () => {
                            const form = document.createElement('form');
                            form.method = 'post';
                            form.enctype = 'multipart/form-data';
                            form.action = '/EditAlbumImages/new/Upload';
                            form.appendChild(document.querySelector("input[name='__RequestVerificationToken']").cloneNode());

                            const title = document.createElement('input');
                            title.name = 'AlbumTitle';
                            title.value = 'Album while disabled';
                            form.appendChild(title);

                            const file = document.createElement('input');
                            file.type = 'file';
                            file.name = 'ImageFiles';
                            const png = Uint8Array.from(atob('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg=='), c => c.charCodeAt(0));
                            const files = new DataTransfer();
                            files.items.add(new File([png], 'album.png', { type: 'image/png' }));
                            file.files = files.files;
                            form.appendChild(file);

                            document.body.appendChild(form);
                            form.submit();
                        }
                        """));

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("You tried to enter an area where you didn't have access."),
                    "An album image was uploaded with the albums disabled");
            });
    }

    /// <summary>
    /// With the avatar upload disabled (the default) an avatar can't be uploaded with a post
    /// (only the form was hidden).
    /// </summary>
    [Test]
    public async Task AvatarUploadNeedsEnabledUploadTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditAvatar");

                Assert.That(
                    await page.Locator("input[type='file']").CountAsync(),
                    Is.Zero,
                    "The avatar upload is shown (the test needs the upload disabled)");

                // the upload form with the token of the user
                await page.RunAndWaitForNextPageAsync(
                    () => page.EvaluateAsync(
                        """
                        () => {
                            const form = document.createElement('form');
                            form.method = 'post';
                            form.enctype = 'multipart/form-data';
                            form.action = '/Profile/EditAvatar/UploadUpdate';
                            form.appendChild(document.querySelector("input[name='__RequestVerificationToken']").cloneNode());

                            const file = document.createElement('input');
                            file.type = 'file';
                            file.name = 'Upload';
                            const png = Uint8Array.from(atob('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg=='), c => c.charCodeAt(0));
                            const files = new DataTransfer();
                            files.items.add(new File([png], 'avatar.png', { type: 'image/png' }));
                            file.files = files.files;
                            form.appendChild(file);

                            document.body.appendChild(form);
                            form.submit();
                        }
                        """));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditAvatar");

                Assert.That(
                    await page.Locator("img.img-thumbnail").First.GetAttributeAsync("src"),
                    Does.Not.Contain("/uploads/"),
                    "An avatar was uploaded with the avatar upload disabled");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
            });
    }
}
