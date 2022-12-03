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

using System.Threading;

using YAF.Tests.Utils.Extensions;

/// <summary>
/// The Message tests.
/// </summary>
[TestFixture]
public class MessageTests : TestBase
{
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
    /// Create a new reply in a topic.
    /// </summary>
    [Test]
    [Description("Create a new reply in a topic.")]
    public void Post_Reply_Test()
    {
        // Go to Post New Topic
        this.Driver.Navigate()
            .GoToUrl(
                $"{TestConfig.TestForumUrl}postst{TestConfig.TestTopicID}.aspx");

        if (this.Driver.PageSource.Contains("You've passed an invalid value to the forum."))
        {
            // Topic doesn't exist create a topic first
            Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");

            // Wait 60 seconds to avoid post flood
            Thread.Sleep(31000);
        }

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReplyLink1')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Post a reply"), "Post Reply not possible");

        // Create New Reply
        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]"))
            .SendKeys("This is a Test Reply in an Test Topic Created by an automated Unit Test");

        // Post New Topic
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReply')]")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains(
                "This is a Test Reply in an Test Topic Created by an automated Unit Test"),
            "Reply Message failed");
    }

    /// <summary>
    /// Post a reply with quoting a Message test.
    /// </summary>
    [Test]
    [Description("Post a reply with quoting a Message test.")]
    public void Post_Reply_With_Quote_Test()
    {
        // Go to Post New Topic
        this.Driver.Navigate()
            .GoToUrl(
                $"{TestConfig.TestForumUrl}postst{TestConfig.TestTopicID}.aspx");

        if (this.Driver.PageSource.Contains("You've passed an invalid value to the forum."))
        {
            // Topic doesn't exist create a topic first
            Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");

            // Wait 60 seconds to avoid post flood
            Thread.Sleep(60000);
        }

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_Quote_0')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Post a reply"), "Post Reply not possible");

        // Create New Reply
        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]")).SendKeys("  Quoting Test");

        // Post New Topic
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReply')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Quoting Test"), "Quoting Message Failed");
    }

    /// <summary>
    /// Post 3 Replies and try to quote them with Multi Quoting via the "Multi Quote" Button test.
    /// </summary>
    [Test]
    [Description("Post 3 Replies and try to quote them with Multi Quoting via the \"Multi Quote\" Button test.")]
    [Ignore("Doesnt work yet")]
    public void Post_Reply_With_Multi_Quote_Test()
    {
        // First Creating a new test topic with the test user
        Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");

        // Wait 30 seconds to avoid post flood
        Thread.Sleep(60000);

        // Post Replay A
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReplyLink1')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Post a reply"), "Post Reply not possible");

        // Create New Reply A
        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]")).SendKeys("Test Reply A");

        // Post New Message
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReply')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Test Reply A"), "Reply Message failed");

        // Wait 30 seconds to avoid post flood
        Thread.Sleep(60000);
        /////

        // Post Replay B
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReplyLink1')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Post a reply"), "Post Reply not possible");

        // Create New Reply B
        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]")).SendKeys("Test Reply B");

        // Post New Message
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReply')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Test Reply B"), "Reply Message failed");

        // Wait 30 seconds to avoid post flood
        Thread.Sleep(60000);
        /////

        // Post Replay C
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReplyLink1')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Post a reply"), "Post Reply not possible");

        // Create New Reply B
        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]")).SendKeys("Test Reply C");

        // Post New Message
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReply')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Test Reply C"), "Reply Message failed");

        // Wait 30 seconds to avoid post flood
        Thread.Sleep(60000);
        /////

        // Find the MultiQuote Buttons for Post Replace A,B,C
        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//input[contains(@id,'_MultiQuote_1')]")),
            "MultiQuote Button of Post Replay A doesnt exists, or Quoting is disabled or not allowed");

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//input[contains(@id,'_MultiQuote_2')]")),
            "MultiQuote Button of Post Replay A doesnt exists, or Quoting is disabled or not allowed");

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//input[contains(@id,'_MultiQuote_3')]")),
            "MultiQuote Button of Post Replay A doesnt exists, or Quoting is disabled or not allowed");

        this.Driver.FindElement(By.XPath("//input[contains(@id,'_MultiQuote_1')]")).Click();
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_MultiQuote_2')]")).Click();
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_MultiQuote_3')]")).Click();

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_Quote_3')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Post a reply"), "Post Reply not possible");

        var editorContent = this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]")).Text;

        Assert.IsTrue(editorContent.Contains("Test Reply A"), "Test Replay A quote not found");
        Assert.IsTrue(editorContent.Contains("Test Reply B"), "Test Replay B quote not found");
        Assert.IsTrue(editorContent.Contains("Test Reply C"), "Test Replay C quote not found");

        // Create New Reply
        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]"))
            .SendKeys("  Multi Quoting Test");

        // Post New Topic
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReply')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Multi Quoting Test"), "MultiQuoting Message Failed");
    }
}