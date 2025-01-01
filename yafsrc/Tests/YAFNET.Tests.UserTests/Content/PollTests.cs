/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Tests.UserTests.Content;

/// <summary>
/// The topic poll tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PollTests : TestBase
{
    /// <summary>
    /// Gets or sets Test Poll Topic Url.
    /// </summary>
    private string testPollTopicUrl = string.Empty;

    /// <summary>
    /// Creating a new topic with a Poll test.
    /// </summary>
    [Test]
    [Description("Creating a new topic with a Poll test.")]
    public async Task CreatePollTopicTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.AdminUserName,
                            this.Base.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: false);
                },
            this.BrowserType);
    }

    /// <summary>
    /// Create a new Poll in an existing topic test.
    /// </summary>
    [Test]
    [Description("Create a new Poll in an existing topic test.")]
    public async Task AddPollToTopicTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    await Assert.MultipleAsync(async () =>
                    {
                        // Log user in first!
                        Assert.That(
                            await page.LoginUserAsync(
                                this.Base.TestSettings,
                                this.Base.TestSettings.AdminUserName,
                                this.Base.TestSettings.AdminPassword), Is.True,
                            "Login failed");

                        // Do actual test

                        // First Creating a new test topic with the test user
                        Assert.That(
                            await page.CreateNewTestTopicAsync(this.Base.TestSettings), Is.True,
                            "Topic Creating failed");
                    });

                    // Go to edit Page
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "" }).First.ClickAsync();
                    await page.Locator("//a[contains(@href,'EditMessage')]").First.ClickAsync();

                    Assert.That(
                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Create Poll" }).IsVisibleAsync(), Is.True,
                        "Editing not allowed for that user. Or the user is not allowed to create Polls");

                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = " Create Poll" }).ClickAsync();

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
                    await page.Locator("//button[contains(@formaction,'SavePoll')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Is this a Test Question?"), "Poll Creating Failed");

                    this.testPollTopicUrl = page.Url;
                },
            this.BrowserType);
    }

    /// <summary>
    /// Vote in the test poll topic test.
    /// </summary>
    [Test]
    [Description("Vote in the test poll topic test.")]
    public async Task VotePollTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.AdminUserName,
                            this.Base.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    if (this.testPollTopicUrl.IsNotSet())
                    {
                        await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: false);
                    }

                    // Now Login with Test User 2
                    await page.LogOutUserAsync();

                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName2,
                            this.Base.TestSettings.TestUser2Password), Is.True,
                        "Login with test user 2 failed");

                    // Go To New Test Topic Url
                    await page.GotoAsync(this.testPollTopicUrl);

                    var pageSource = await page.ContentAsync();

                    Assert.That(!pageSource.Contains("You already voted"), Is.True, "User has already voted");

                    // Vote for Option 3
                    await page.Locator("//button[contains(@id,'VoteButton')]").Nth(2).ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Thank you for your vote!"), "Voting failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Multi Vote in the test poll topic test.
    /// </summary>
    [Test]
    [Description("Multi Vote in the test poll topic test.")]
    public async Task MultiVotePollTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.AdminUserName,
                            this.Base.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    if (this.testPollTopicUrl.IsNotSet())
                    {
                        await this.CreatePollTopicTestAsync(page, allowMultiVote: true, addPollImages: false);
                    }

                    // Now Login with Test User 2
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName2,
                            this.Base.TestSettings.TestUser2Password), Is.True,
                        "Login with test user 2 failed");

                    // Go To New Test Topic Url
                    await page.GotoAsync(this.testPollTopicUrl);

                    var pageSource = await page.ContentAsync();

                    Assert.That(!pageSource.Contains("You already voted"), Is.True, "User has already voted");

                    // Vote for Option 3
                    await page.Locator("//button[contains(@id,'VoteButton')]").Nth(2).ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Thank you for your vote!"), "Voting failed");

                    // Close info alert
                    await page.Locator(".btn-close").ClickAsync();

                    // Vote for Option 5
                    await page.Locator("//button[contains(@id,'VoteButton')]").Nth(3).ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Thank you for your vote!"), "Voting failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Remove the test poll completely test.
    /// </summary>
    [Test]
    [Description("Remove the test poll completely test.")]
    public async Task Remove_Poll_Completely_Test()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    if (this.testPollTopicUrl.IsNotSet())
                    {
                        await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: false);
                    }

                    // Now Login with the Admin Test user accounts
                    await page.LoginAdminUserAsync(
                        this.Base.TestSettings);

                    // Go To New Test Topic Url
                    await page.GotoAsync(this.testPollTopicUrl);

                    // Remove Poll
                    Assert.That(
                        await page.Locator("//*[contains(@formaction,'RemovePoll')]").IsVisibleAsync(), Is.True,
                        "Editing not allowed for that user");

                    await page.Locator("//*[contains(@formaction,'RemovePoll')]").ClickAsync();

                    await page.GetByText("Yes").ClickAsync();

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Poll was removed!"), "Deleting Poll failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Create a new Topic with Poll where the Poll Question
    /// and Answer Choices contains images test.
    /// </summary>
    [Test]
    [Description("Create a new Topic with Poll where the Poll Question and Answer Choices contains images test.")]
    public async Task CreatePollWithImageOptionsTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Login with the Admin Test user accounts
                    await page.LoginUserAsync(
                        this.Base.TestSettings,
                        this.Base.TestSettings.AdminUserName,
                        this.Base.TestSettings.AdminPassword);

                    await this.CreatePollTopicTestAsync(page, allowMultiVote: false, addPollImages: true);

                    await page.GotoAsync(this.testPollTopicUrl);

                    Assert.That(
                        await page.Locator("//img[contains(@alt,'Option 2')]").IsVisibleAsync(), Is.True,
                        "Image Not Found");

                    var choiceImage2 = await page.Locator("//img[contains(@alt,'Option 2')]").GetAttributeAsync("src");

                    Assert.That(choiceImage2, Is.Not.Null);

                    Assert.That(
                        choiceImage2, Is.EqualTo("https://yetanotherforum.net/assets/img/YAFLogo.svg"),
                        "Image Url does not match");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Creates the poll topic test.
    /// </summary>
    /// <param name="page">
    /// The Page Instance
    /// </param>
    /// <param name="allowMultiVote">if set to <c>true</c> [allow multi vote].</param>
    /// <param name="addPollImages">if set to <c>true</c> [add poll images].</param>
    private async Task CreatePollTopicTestAsync(IPage page, bool allowMultiVote, bool addPollImages)
    {
        // Go to Post New Topic
        await page.GotoAsync(
            $"{this.Base.TestSettings.TestForumUrl}PostTopic/{this.Base.TestSettings.TestForumId}");

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

        await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("This is a test topic to test the Poll Creating");

        // Post New Topic
        await page.Locator("//*[contains(@formaction,'PostReply')]").ClickAsync();

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
                .FillAsync("https://yetanotherforum.net/assets/img/YAFLogo.svg");
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
                .FillAsync("https://yetanotherforum.net/assets/img/YAFLogo.svg");
        }

        // Enter Poll Answer Choice 4
        await page.Locator("//input[contains(@id,'Input_Choices_3__ChoiceName')]").FillAsync("Option 4");

        // Enter Poll Question Image if exists
        if (await page.Locator("//input[contains(@id,'3__ObjectPath')]").IsVisibleAsync() && addPollImages)
        {
            await page.Locator("//input[contains(@id,'3__ObjectPath')]")
                .FillAsync("https://yetanotherforum.net/assets/img/YAFLogo.svg");
        }

        // Enter Poll Answer Choice 5
        await page.Locator("//input[contains(@id,'Input_Choices_4__ChoiceName')]").FillAsync("Option 5");

        // Enter Poll Question Image if exists
        if (await page.Locator("//input[contains(@id,'4__ObjectPath')]").IsVisibleAsync() && addPollImages)
        {
            await page.Locator("//input[contains(@id,'4__ObjectPath')]")
                .FillAsync("https://yetanotherforum.net/assets/img/YAFLogo.svg");
        }

        if (allowMultiVote)
        {
            await page.Locator("//input[contains(@id,'_AllowMultipleChoicesCheckBox')]").CheckAsync();
        }

        await page.Locator("//input[contains(@id,'_ShowVotersCheckBox')]").CheckAsync();

        // Save Poll
        await page.Locator("//button[contains(@formaction,'SavePoll')]").ClickAsync();

        pageSource = await page.ContentAsync();

        Assert.That(pageSource, Does.Contain("Poll Question:"), "Poll Creating Failed");

        this.testPollTopicUrl = page.Url;
    }
}