/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

using System;
using System.Threading;

using YAF.Tests.Utils.Extensions;
using YAF.Types.Extensions;

/// <summary>
/// The topic poll tests.
/// </summary>
[TestFixture]
public class PollTests : TestBase
{
    /// <summary>
    /// Gets or sets Test Poll Topic Url.
    /// </summary>
    private string testPollTopicUrl = string.Empty;

    /// <summary>
    /// Login User Setup
    /// </summary>
    [OneTimeSetUp]
    public void SetUpTest()
    {
        this.Driver = !TestConfig.UseExistingInstallation ? TestSetup.TestBase.ChromeDriver : new ChromeDriver();

        Assert.IsTrue(this.LoginUser(), "Login failed");
    }

    /// <summary>
    /// Logout Test User
    /// </summary>
    [OneTimeTearDown]
    public void TearDownTest()
    {
        this.LogoutUser();
    }

    /// <summary>
    /// Creating a new topic with a Poll test.
    /// </summary>
    [Test]
    [Description("Creating a new topic with a Poll test.")]
    public void Create_Poll_Topic_Test()
    {
        this.CreatePollTopicTest(allowMultiVote: false, addPollImages: false);
    }

    /// <summary>
    /// Create a new Poll in an existing topic test.
    /// </summary>
    [Test]
    [Description("Create a new Poll in an existing topic test.")]
    public void Add_Poll_To_Topic_Test()
    {
        // First Creating a new test topic with the test user
        Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");

        // Go to edit Page
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_Edit_0')]")).ClickAndWait();

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//a[contains(@id,'_CreatePoll1')]")),
            "Editing not allowed for that user. Or the user is not allowed to create Polls");

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_CreatePoll1')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Edit Poll"), "Creating Poll not possible");

        // Enter Poll Question
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_Question')]"))
            .SendKeys("Is this a Test Question?");

        // Enter Poll Answer Choice 1
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_0')]")).SendKeys("Option 1");

        // Enter Poll Answer Choice 2
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_1')]")).SendKeys("Option 2");

        // Enter Poll Answer Choice 3
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_2')]")).SendKeys("Option 3");

        // Enter Poll Answer Choice 4
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_3')]")).SendKeys("Option 4");

        // Enter Poll Answer Choice 5
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_4')]")).SendKeys("Option 5");

        Driver.FindElement(By.XPath("//input[contains(@id,'_ShowVotersCheckBox')]")).Click();

        // Save Poll
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_SavePoll')]")).Click();

        Assert.IsTrue(
            this.Driver.FindElement(By.XPath("//span[contains(@id,'_QuestionLabel_0')]"))
                .Text.Contains("Is this a Test Question?"),
            "Poll Creating Failed");

        this.testPollTopicUrl = this.Driver.Url;
    }

    /// <summary>
    /// Vote in the test poll topic test.
    /// </summary>
    [Test]
    [Description("Vote in the test poll topic test.")]
    public void Vote_Poll_Test()
    {
        if (this.testPollTopicUrl.IsNotSet())
        {
            this.CreatePollTopicTest(allowMultiVote: false, addPollImages: false);
        }

        // Now Login with Test User 2
        Assert.IsTrue(
            this.LoginUser(TestConfig.TestUserName2, TestConfig.TestUser2Password),
            "Login with test user 2 failed");

        // Go To New Test Topic Url
        this.Driver.Navigate().GoToUrl(this.testPollTopicUrl);

        Assert.IsTrue(!this.Driver.PageSource.Contains("You already voted"), "User has already voted");

        // Vote for Option 3
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_MyLinkButton1_2')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Thank you for your vote!"), "Voting failed");
    }

    /// <summary>
    /// Multi Vote in the test poll topic test.
    /// </summary>
    [Test]
    [Description("Multi Vote in the test poll topic test.")]
    public void Multi_Vote_Poll_Test()
    {
        if (this.testPollTopicUrl.IsNotSet())
        {
            this.CreatePollTopicTest(allowMultiVote: true, addPollImages: false);
        }

        // Now Login with Test User 2
        Assert.IsTrue(
            this.LoginUser(TestConfig.TestUserName2, TestConfig.TestUser2Password),
            "Login with test user 2 failed");

        // Go To New Test Topic Url
        this.Driver.Navigate().GoToUrl(this.testPollTopicUrl);

        Assert.IsTrue(!this.Driver.PageSource.Contains("You already voted"), "User has already voted");

        // Vote for Option 3
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_MyLinkButton1_2')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Thank you for your vote!"), "Voting failed");

        Thread.Sleep(60000);

        // Vote for Option 5
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_MyLinkButton1_4')]"));

        Assert.IsTrue(this.Driver.PageSource.Contains("Thank you for your vote!"), "Voting failed");
    }

    /// <summary>
    /// Remove the test poll completely test.
    /// </summary>
    [Test]
    [Description("Remove the test poll completely test.")]
    public void Remove_Poll_Completely_Test()
    {
        if (this.testPollTopicUrl.IsNotSet())
        {
            this.CreatePollTopicTest(allowMultiVote: false, addPollImages: false);
        }

        // Now Login with the Admin Test user accounts
        this.LoginAdminUser();

        // Go To New Test Topic Url
        this.Driver.Navigate().GoToUrl(this.testPollTopicUrl);

        // Go to edit Page
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_Edit_0')]")).Click();

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//a[contains(@id,'_RemovePollAll_0')]")),
            "Editing not allowed for that user");

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_RemovePollAll_0')]")).Click();

        this.Driver.SwitchTo().Alert().Accept();

        Assert.IsTrue(this.Driver.PageSource.Contains("Create Poll"), "Deleting Poll failed");
    }

    /// <summary>
    /// Create a new Topic with Poll where the Poll Question 
    /// and Answer Choices contains images test.
    /// </summary>
    [Test]
    [Description("Create a new Topic with Poll where the Poll Question and Answer Choices contains images test.")]
    public void Create_Poll_With_Image_Options_Test()
    {
        // Now Login with the Admin Test user accounts
        this.LoginAdminUser();

        this.CreatePollTopicTest(allowMultiVote: false, addPollImages: true);

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//img[contains(@id,'_ChoiceImage_2')]")),
            "Image Not Found");

        var choiceImage2 = Driver.FindElement(By.XPath("//img[contains(@id,'_ChoiceImage_2')]"))
            .GetAttribute("src");

        Assert.IsTrue(
            choiceImage2.Equals("http://yetanotherforum.net/forum/images/YafLogoSmall.png"),
            "Image Url does not match");
    }

    /// <summary>
    /// Creates the poll topic test.
    /// </summary>
    /// <param name="allowMultiVote">if set to <c>true</c> [allow multi vote].</param>
    /// <param name="addPollImages">if set to <c>true</c> [add poll images].</param>
    private void CreatePollTopicTest(bool allowMultiVote, bool addPollImages)
    {
        // Go to Post New Topic
        this.Driver.Navigate()
            .GoToUrl(
                $"{TestConfig.TestForumUrl}postmessage.aspx?f={TestConfig.TestForumID}");

        Assert.IsTrue(this.Driver.PageSource.Contains("Post New Topic"), "Post New Topic not possible");

        Assert.IsTrue(this.Driver.PageSource.Contains("Add Poll?"), "User does doesnt have Poll Access");

        // Enable Add Poll Option
        Driver.FindElement(By.XPath("//input[contains(@id,'_AddPollCheckBox')]")).Click();

        // Create New Poll TopicF
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_TopicSubjectTextBox')]"))
            .SendKeys($"Auto Created Test Topic with Poll - {DateTime.UtcNow}");

        if (this.Driver.PageSource.Contains("Description"))
        {
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_TopicDescriptionTextBox')]"))
                .SendKeys("Poll Testing");
        }

        if (this.Driver.PageSource.Contains("Status"))
        {
            this.Driver.SelectDropDownByValue(By.XPath("//select[contains(@id,'_TopicStatus')]"), "QUESTION");
        }

        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]"))
            .SendKeys("This is a test topic to test the Poll Creating");

        // Post New Topic
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReply')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Edit Poll"), "Topic Creating failed");

        // Enter Poll Question
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_Question')]"))
            .SendKeys("Is this a Test Question?");

        // Enter Poll Question Image if exists
        if (this.Driver.ElementExists(By.XPath("//input[contains(@id,'_QuestionObjectPath')]")) && addPollImages)
        {
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_QuestionObjectPath')]"))
                .SendKeys("http://download.codeplex.com/Download?ProjectName=yafnet&DownloadId=167900&Build=18559");
        }

        // Enter Poll Answer Choice 1
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_0')]")).SendKeys("Option 1");

        // Enter Poll Question Image if exists
        if (this.Driver.ElementExists(By.XPath("//input[contains(@id,'_ObjectPath_0')]")))
        {
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_ObjectPath_0')]"))
                .SendKeys("http://yetanotherforum.net/forum/images/YafLogoSmall.png");
        }

        // Enter Poll Answer Choice 2
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_1')]")).SendKeys("Option 2");

        // Enter Poll Question Image if exists
        if (this.Driver.ElementExists(By.XPath("//input[contains(@id,'_ObjectPath_1')]")) && addPollImages)
        {
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_ObjectPath_1')]"))
                .SendKeys("http://yetanotherforum.net/forum/images/YafLogoSmall.png");
        }

        // Enter Poll Answer Choice 3
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_2')]")).SendKeys("Option 3");

        // Enter Poll Question Image if exists
        if (this.Driver.ElementExists(By.XPath("//input[contains(@id,'_ObjectPath_2')]")) && addPollImages)
        {
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_ObjectPath_2')]"))
                .SendKeys("http://yetanotherforum.net/forum/images/YafLogoSmall.png");
        }

        // Enter Poll Answer Choice 4
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_3')]")).SendKeys("Option 4");

        // Enter Poll Question Image if exists
        if (this.Driver.ElementExists(By.XPath("//input[contains(@id,'_ObjectPath_3')]")) && addPollImages)
        {
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_ObjectPath_3')]"))
                .SendKeys("http://yetanotherforum.net/forum/images/YafLogoSmall.png");
        }

        // Enter Poll Answer Choice 5
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PollChoice_4')]")).SendKeys("Option 5");

        // Enter Poll Question Image if exists
        if (this.Driver.ElementExists(By.XPath("//input[contains(@id,'_ObjectPath_4')]")) && addPollImages)
        {
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_ObjectPath_4')]"))
                .SendKeys("http://yetanotherforum.net/forum/images/YafLogoSmall.png");
        }

        if (allowMultiVote)
        {
            Driver.FindElement(By.XPath("//input[contains(@id,'_AllowMultipleChoicesCheckBox')]")).Click();
        }

        Driver.FindElement(By.XPath("//input[contains(@id,'_ShowVotersCheckBox')]")).Click();

        // Save Poll
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_SavePoll')]")).Click();

        Assert.IsTrue(
            this.Driver.FindElement(By.XPath("//span[contains(@id,'_QuestionLabel_0')]"))
                .Text.Contains("Is this a Test Question?"),
            "Poll Creating Failed");

        this.testPollTopicUrl = this.Driver.Url;
    }
}