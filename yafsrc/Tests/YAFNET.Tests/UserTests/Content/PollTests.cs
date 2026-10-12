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
/// The topic poll tests.
/// </summary>
public class PollTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Creating a new topic with a Poll test.
    /// </summary>
    [Test]
    [Description("Creating a new topic with a Poll test.")]
    public async Task CreatePollTopicTest()
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
                await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: false);
            });
    }

    /// <summary>
    /// A poll can only be edited through its own topic (the moderator rights are the ones of the topic's forum).
    /// </summary>
    [Test]
    [Description("A poll can only be edited through its own topic")]
    public async Task EditPollNeedsItsTopicTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                var topicUrl = await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: false);

                await page.GotoAsync(topicUrl);

                var editLink = await page.Locator("a[href*='PollEdit']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "Edit poll link not found");

                var url = new Uri(new Uri(this.TestSettings.TestForumUrl), editLink.Replace("&amp;", "&"));

                // with the topic the poll can be edited
                await page.GotoAsync(url.ToString());

                Assert.That(
                    await page.Locator("//input[contains(@id,'Question')]").First.InputValueAsync(),
                    Is.Not.Empty,
                    "The poll can not be edited");

                // without the topic it is refused
                var query = System.Web.HttpUtility.ParseQueryString(url.Query);
                query.Remove("t");

                await page.GotoAsync(new UriBuilder(url) { Query = query.ToString() }.Uri.ToString());

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("You tried to enter an area where you didn't have access."),
                    "The poll can be edited without its topic");
            });
    }

    /// <summary>
    /// A poll can still be saved after a warning (the poll id was lost, the second save was refused).
    /// </summary>
    [Test]
    [Description("A poll can still be saved after a warning")]
    public async Task EditPollAfterWarningTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                var topicUrl = await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: false);

                await page.GotoAsync(topicUrl);

                var editLink = await page.Locator("a[href*='PollEdit']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "Edit poll link not found");

                await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), editLink.Replace("&amp;", "&")).ToString());

                // without question: the warning
                await page.Locator("//input[contains(@id,'Question')]").First.FillAsync(string.Empty);
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'SavePoll')]"));

                Assert.That(
                    await page.Locator("//button[contains(@formaction,'SavePoll')]").CountAsync(),
                    Is.EqualTo(1),
                    "The page is not shown again with the warning");

                var question = $"A changed question {Random.Shared.Next()}?";

                await page.Locator("//input[contains(@id,'Question')]").First.FillAsync(question);
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'SavePoll')]"));

                var content = await page.ContentAsync();

                Assert.That(
                    content,
                    Does.Not.Contain("You tried to enter an area where you didn't have access."),
                    "The poll can't be saved after the warning");
                Assert.That(content, Does.Contain(question), "The poll was not changed");
            });
    }

    /// <summary>
    /// The poll of a new topic in a moderated forum: after saving the poll the user is told that the
    /// topic must be approved (the user was sent to the topic, which is not shown yet).
    /// </summary>
    [Test]
    [Description("The poll of a topic in a moderated forum shows the approval info")]
    public async Task PollOfModeratedTopicShowsApprovalInfoTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var maskName = $"PollModerated{Random.Shared.Next()}";
                var forumName = $"Moderated Poll Forum {maskName}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a moderated forum where the members can create polls
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditAccessMask");

                await page.Locator("#Input_Name").FillAsync(maskName);

                foreach (var access in new[] { "Read", "Post", "Reply", "Poll", "Vote" })
                {
                    await page.Locator($"#Input_{access}Access").CheckAsync();
                }

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the moderated poll test");
                await page.Locator("#Input_SortOrder").FillAsync("99");

                await page.GetByLabel("Registered").First.SelectOptionAsync(new SelectOptionValue { Label = maskName });

                // all posts are moderated (the switch submits the form to show the options, set directly)
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

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                var forumLink = await page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasText = forumName })
                                    .First.GetAttributeAsync("href");

                Assert.That(forumLink, Is.Not.Null, "The forum is not shown to the user");

                var forumId = System.Text.RegularExpressions.Regex.Match(forumLink, @"/Topics/(\d+)/").Groups[1].Value;

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{forumId}");

                await page.Locator("//input[contains(@id,'_AddPoll')]").CheckAsync();

                await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Moderated poll test {Random.Shared.Next()}");

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A poll of a moderated topic");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(page.Url, Does.Contain("PollEdit"), $"The poll page is not shown ({page.Url})");

                await page.Locator("//input[contains(@id,'Question')]").First.FillAsync("A question of a moderated topic?");
                await page.Locator("//input[contains(@id,'Input_Choices_0__ChoiceName')]").FillAsync("Choice A");
                await page.Locator("//input[contains(@id,'Input_Choices_1__ChoiceName')]").FillAsync("Choice B");

                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'SavePoll')]"));

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("a forum moderator must approve your post"),
                    $"The approval info is not shown ({page.Url})");
            });
    }

    /// <summary>
    /// Create a new Poll in an existing topic test.
    /// </summary>
    [Test]
    [Description("Create a new Poll in an existing topic test.")]
    public async Task AddPollToTopicTest()
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
                            this.TestSettings.AdminUserName,
                            this.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    // First Creating a new test topic with the test user
                    Assert.That(
                        await page.CreateNewTestTopicAsync(this.TestSettings), Is.True,
                        "Topic Creating failed");
                });

                // Go to edit Page
                await page.GetByTitle("Tools").ClickAsync();
                await page.Locator("//a[contains(@href,'EditMessage')]").First.ClickAsync();

                Assert.That(
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Create Poll" })
                        .IsVisibleAsync(), Is.True,
                    "Editing not allowed for that user. Or the user is not allowed to create Polls");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Create Poll" }).ClickAsync();

                var pageSource = await page.ContentAsync();

                Assert.That(
                    pageSource, Does.Contain("Create Poll"),
                    "Creating Poll not possible, or the user has not access to it");

                // Enter Poll Question
                await page.Locator("//input[contains(@id,'Question')]").First.FillAsync("Is this a Test Question?");

                // Enter Poll Answer Choice 1
                await page.Locator("//input[contains(@id,'Input_Choices_0__ChoiceName')]").FillAsync("Option 1");

                // Enter Poll Answer Choice 2
                await page.Locator("//input[contains(@id,'Input_Choices_1__ChoiceName')]").FillAsync("Option 2");

                // Enter Poll Answer Choice 3
                await page.Locator("//input[contains(@id,'Input_Choices_2__ChoiceName')]").FillAsync("Option 3");

                // Enter Poll Answer Choice 4
                await page.Locator("//input[contains(@id,'Input_Choices_3__ChoiceName')]").FillAsync("Option 4");

                // Enter Poll Answer Choice 5
                await page.Locator("//input[contains(@id,'Input_Choices_4__ChoiceName')]").FillAsync("Option 5");

                await page.Locator("//input[contains(@id,'_ShowVotersCheckBox')]").ClickAsync();

                // Save Poll
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'SavePoll')]"));

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Is this a Test Question?"), "Poll Creating Failed");
            });
    }

    /// <summary>
    /// Vote in the test poll topic test.
    /// </summary>
    [Test]
    [Description("Vote in the test poll topic test.")]
    public async Task VotePollTest()
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
                var testPollTopicUrl = await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: false);

                // Now Login with Test User 2
                await page.LogOutUserAsync();

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName2,
                        this.TestSettings.TestUser2Password), Is.True,
                    "Login with test user 2 failed");

                // Go To New Test Topic Url
                await page.GotoAsync(testPollTopicUrl);

                var pageSource = await page.ContentAsync();

                Assert.That(!pageSource.Contains("You already voted"), Is.True, "User has already voted");

                // Vote for Option 3
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@id,'VoteButton')]").Nth(1));

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Thank you for your vote!"), "Voting failed");
            });
    }

    /// <summary>
    /// Multi Vote in the test poll topic test.
    /// </summary>
    [Test]
    [Description("Multi Vote in the test poll topic test.")]
    public async Task MultiVotePollTest()
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
                var testPollTopicUrl = await this.CreatePollTopicTestAsync(page, allowMultiVote: true, addPollImages: false);

                // Now Login with Test User 2
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName2,
                        this.TestSettings.TestUser2Password), Is.True,
                    "Login with test user 2 failed");

                // Go To New Test Topic Url
                await page.GotoAsync(testPollTopicUrl);

                var pageSource = await page.ContentAsync();

                Assert.That(!pageSource.Contains("You already voted"), Is.True, "User has already voted");

                // Vote for Option 3
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@id,'VoteButton')]").Nth(2));

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Thank you for your vote!"), "Voting failed");

                // Close info alert
                await page.Locator(".btn-close").ClickAsync();

                // Vote for Option 5
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@id,'VoteButton')]").Nth(2));

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Thank you for your vote!"), "Voting failed");
            });
    }

    /// <summary>
    /// Remove the test poll completely test.
    /// </summary>
    [Test]
    [Description("Remove the test poll completely test.")]
    public async Task Remove_Poll_Completely_Test()
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
                var testPollTopicUrl = await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: false);

                // Now Login with the Admin Test user accounts
                await page.LoginAdminUserAsync(
                    this.TestSettings);

                // Go To New Test Topic Url
                await page.GotoAsync(testPollTopicUrl);

                // Remove Poll
                Assert.That(
                    await page.Locator("//*[contains(@formaction,'RemovePoll')]").IsVisibleAsync(), Is.True,
                    "Editing not allowed for that user");

                await page.Locator("//*[contains(@formaction,'RemovePoll')]").ClickAsync();

                await page.SubmitAndWaitAsync(page.GetByText("Yes"));

                var pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Poll was removed!"), "Deleting Poll failed");
            });
    }

    /// <summary>
    /// Create a new Topic with Poll where the Poll Question
    /// and Answer Choices contains images test.
    /// </summary>
    [Test]
    [Description("Create a new Topic with Poll where the Poll Question and Answer Choices contains images test.")]
    public async Task CreatePollWithImageOptionsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Login with the Admin Test user accounts
                await page.LoginUserAsync(
                    this.TestSettings,
                    this.TestSettings.AdminUserName,
                    this.TestSettings.AdminPassword);

                var testPollTopicUrl = await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: true);

                await page.GotoAsync(testPollTopicUrl);

                Assert.That(
                    await page.Locator("//img[contains(@alt,'Option 2')]").IsVisibleAsync(), Is.True,
                    "Image Not Found");

                var choiceImage2 = await page.Locator("//img[contains(@alt,'Option 2')]").GetAttributeAsync("src");

                Assert.That(choiceImage2, Is.Not.Null);

                Assert.That(
                    choiceImage2, Is.EqualTo("https://yetanotherforum.net/assets/img/YAFLogo.svg"),
                    "Image Url does not match");

                // the image of the question
                Assert.That(
                    await page.Locator("//img[@alt='Is this a Test Question?']").GetAttributeAsync("src"),
                    Is.EqualTo("https://github.com/YAFNET/YAFNET/releases"),
                    "The image of the question is missing");
            });
    }

    /// <summary>
    /// A member can't add images to a poll with a manipulated form when image polls are only allowed
    /// for admins (the default), the images could be loaded from any site.
    /// </summary>
    [Test]
    public async Task PollImagesNeedPermissionTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var maskName = $"PollAccess{Random.Shared.Next()}";
                var forumName = $"Poll Forum {maskName}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a forum where the members can create polls
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditAccessMask");

                await page.Locator("#Input_Name").FillAsync(maskName);

                foreach (var access in new[] { "Read", "Post", "Reply", "Poll", "Vote" })
                {
                    await page.Locator($"#Input_{access}Access").CheckAsync();
                }

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the poll test");
                await page.Locator("#Input_SortOrder").FillAsync("99");

                await page.GetByLabel("Registered").First.SelectOptionAsync(new SelectOptionValue { Label = maskName });

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // a new user (the test user may just have posted, flood control)
                await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                var forumLink = await page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasText = forumName })
                                    .First.GetAttributeAsync("href");

                Assert.That(forumLink, Is.Not.Null, "The forum is not shown to the user");

                var forumId = System.Text.RegularExpressions.Regex.Match(forumLink, @"/Topics/(\d+)/").Groups[1].Value;

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{forumId}");

                Assert.That(await page.ContentAsync(), Does.Contain("Add Poll"), "The user has no poll access");

                await page.Locator("//input[contains(@id,'_AddPoll')]").CheckAsync();

                await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync($"Poll image test {Random.Shared.Next()}");

                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A poll without images");

                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(
                    await page.Locator("//input[contains(@id,'_QuestionObjectPath')]").CountAsync(),
                    Is.Zero,
                    "The poll images are allowed (the test needs the default setting)");

                await page.Locator("//input[contains(@id,'Question')]").First.FillAsync("A question without image?");
                await page.Locator("//input[contains(@id,'Input_Choices_0__ChoiceName')]").FillAsync("Choice A");
                await page.Locator("//input[contains(@id,'Input_Choices_1__ChoiceName')]").FillAsync("Choice B");

                // the image fields of the admins
                await page.EvaluateAsync(
                    """
                    () => {
                        const form = document.querySelector("button[formaction*='SavePoll']").form;
                        for (const name of ['Input.QuestionObjectPath', 'Input.Choices[0].ObjectPath']) {
                            const field = document.createElement('input');
                            field.type = 'hidden';
                            field.name = name;
                            field.value = 'https://example.com/tracker.png';
                            form.appendChild(field);
                        }
                    }
                    """);

                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'SavePoll')]"));

                Assert.That(await page.ContentAsync(), Does.Contain("Choice A"), "The poll was not created");

                Assert.That(
                    await page.Locator("img[src*='example.com']").CountAsync(),
                    Is.Zero,
                    "The poll shows images of another site");
            });
    }

    /// <summary>
    /// Creates the poll topic test.
    /// </summary>
    /// <param name="page">
    /// The Page Instance
    /// </param>
    /// <param name="allowMultiVote">if set to <c>true</c> [allow multi vote].</param>
    /// <param name="addPollImages">if set to <c>true</c> [add poll images].</param>
    private async Task<string> CreatePollTopicTestAsync(IPage page, bool allowMultiVote, bool addPollImages)
    {
        // Go to Post New Topic
        await page.GotoAsync(
            $"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");

        var pageSource = await page.ContentAsync();

        Assert.That(pageSource, Does.Contain("Post New Topic"), "Post New Topic not possible");

        Assert.That(pageSource, Does.Contain("Add Poll"), "User does doesn't have Poll Access");

        // Enable Add Poll Option
        await page.Locator("//input[contains(@id,'_AddPoll')]").CheckAsync();

        // Create New Poll TopicF
        await page.Locator("//input[contains(@id,'_TopicSubject')]")
            .FillAsync($"Auto Created Test Topic with Poll - {DateTime.UtcNow}");

        pageSource = await page.ContentAsync();

        if (pageSource.Contains("Description"))
        {
            await page.Locator("//input[contains(@id,'_TopicDescription')]").FillAsync("Poll Testing");
        }

        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").ClickAsync();
        await page.FrameLocator(".sceditor-container >> iframe").Locator("body")
            .FillAsync("This is a test topic to test the Poll Creating");

        // Post New Topic
        await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

        pageSource = await page.ContentAsync();

        Assert.That(pageSource, Does.Contain("Create Poll"), "Topic Creating failed");

        // Enter Poll Question
        await page.Locator("//input[contains(@id,'Question')]").First.FillAsync("Is this a Test Question?");

        // Enter Poll Question Image if exists
        if (await page.Locator("//input[contains(@id,'_QuestionObjectPath')]").IsVisibleAsync() && addPollImages)
        {
            await page.Locator("//input[contains(@id,'_QuestionObjectPath')]")
                .FillAsync("https://github.com/YAFNET/YAFNET/releases");
        }

        // Enter Poll Answer Choice 1
        await page.Locator("//input[contains(@id,'Input_Choices_0__ChoiceName')]").FillAsync("Option 1");

        // Enter Poll Question Image if exists
        if (await page.Locator("//input[contains(@id,'0__ObjectPath')]").IsVisibleAsync())
        {
            await page.Locator("//input[contains(@id,'0__ObjectPath')]")
                .FillAsync("https://yetanotherforum.net/images/Logos/YAFLogo.svg");
        }

        // Enter Poll Answer Choice 2
        await page.Locator("//input[contains(@id,'Input_Choices_1__ChoiceName')]").FillAsync("Option 2");

        // Enter Poll Question Image if exists
        if (await page.Locator("//input[contains(@id,'1__ObjectPath')]").IsVisibleAsync() && addPollImages)
        {
            await page.Locator("//input[contains(@id,'1__ObjectPath')]")
                .FillAsync("https://yetanotherforum.net/assets/img/YAFLogo.svg");
        }

        // Enter Poll Answer Choice 3
        await page.Locator("//input[contains(@id,'Input_Choices_2__ChoiceName')]").FillAsync("Option 3");

        // Enter Poll Question Image if exists
        if (await page.Locator("//input[contains(@id,'2__ObjectPath')]").IsVisibleAsync() && addPollImages)
        {
            await page.Locator("//input[contains(@id,'2__ObjectPath')]")
                .FillAsync("https://yetanotherforum.net/images/Logos/YAFLogo.svg");
        }

        // Enter Poll Answer Choice 4
        await page.Locator("//input[contains(@id,'Input_Choices_3__ChoiceName')]").FillAsync("Option 4");

        // Enter Poll Question Image if exists
        if (await page.Locator("//input[contains(@id,'3__ObjectPath')]").IsVisibleAsync() && addPollImages)
        {
            await page.Locator("//input[contains(@id,'3__ObjectPath')]")
                .FillAsync("https://yetanotherforum.net/images/Logos/YAFLogo.svg");
        }

        // Enter Poll Answer Choice 5
        await page.Locator("//input[contains(@id,'Input_Choices_4__ChoiceName')]").FillAsync("Option 5");

        // Enter Poll Question Image if exists
        if (await page.Locator("//input[contains(@id,'4__ObjectPath')]").IsVisibleAsync() && addPollImages)
        {
            await page.Locator("//input[contains(@id,'4__ObjectPath')]")
                .FillAsync("https://yetanotherforum.net/images/Logos/YAFLogo.svg");
        }

        if (allowMultiVote)
        {
            await page.Locator("//input[contains(@id,'_AllowMultipleChoicesCheckBox')]").CheckAsync();
        }

        await page.Locator("//input[contains(@id,'_ShowVotersCheckBox')]").CheckAsync();

        // Save Poll
        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'SavePoll')]"));

        pageSource = await page.ContentAsync();

        Assert.That(pageSource, Does.Contain("Poll Question:") | Does.Contain("Option 1"), "Poll Creating Failed");

        return page.Url;
    }
}