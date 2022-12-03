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

namespace YAF.Tests.UserTests.UserSettings;

using System;
using System.Threading;

using netDumbster.smtp;

using YAF.Tests.Utils.Extensions;
using YAF.Types.Extensions;

/// <summary>
/// The Email Notification tests.
/// </summary>
[TestFixture]
public class EmailNotificationTests : TestBase
{
    /// <summary>
    /// Login User Setup
    /// </summary>
    [OneTimeSetUp]
    public void SetUpTest()
    {
        this.Driver = !TestConfig.UseExistingInstallation ? TestSetup.TestBase.ChromeDriver : new ChromeDriver();

        if (TestConfig.UseTestMailServer)
        {
            this.TestMailServer = SimpleSmtpServer.Start(TestConfig.TestMailPort.ToType<int>());
        }

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
    /// Add watch topic test.
    /// </summary>
    [Test]
    public void Add_Watch_Topic_Test()
    {
        // Go to Test Topic
        this.Driver.Navigate()
            .GoToUrl(
                $"{TestConfig.TestForumUrl}postst{TestConfig.TestTopicID}.aspx");

        Assert.IsFalse(
            this.Driver.PageSource.Contains("You've passed an invalid value to the forum."),
            "Test Topic Doesn't Exists");

        // Get Topic Title
        var topicTitle = this.Driver.FindElement(By.ClassName("currentPageLink")).Text;

        // Open Topic Options Menu
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_OptionsLink')]")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Watch this topic"),
            "Watch Topic is disabled, or User already Watches that Topic");

        this.Driver.FindElement(By.XPath("//li[contains(@title,'Watch this topic')]")).ClickAndWait();

        Assert.IsTrue(this.Driver.PageSource.Contains("You will now be"), "Watch topic failed");

        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_subscriptions.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Email Notification Preferences"),
            "Email Notification Preferences is not available for that User");

        Assert.IsTrue(this.Driver.ElementExists(By.LinkText(topicTitle)));
    }

    /// <summary>
    /// Delete watch topic test.
    /// </summary>
    [Test]
    public void Delete_Watch_Topic_Test()
    {
        // Go to Test Topic
        this.Driver.Navigate()
            .GoToUrl(
                $"{TestConfig.TestForumUrl}postst{TestConfig.TestTopicID}.aspx");

        Assert.IsFalse(
            this.Driver.PageSource.Contains("You've passed an invalid value to the forum."),
            "Test Topic Doesn't Exists");

        // Open Topic Options Menu
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_OptionsLink')]")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Unwatch this topic"),
            "Watch Topic is disabled, or User doesn't watch this topic");

        this.Driver.FindElement(By.XPath("//li[contains(@title,'Unwatch this topic')]")).ClickAndWait();

        Assert.IsTrue(this.Driver.PageSource.Contains("You will no longer"), "Unwatch topic failed");
    }

    /// <summary>
    /// Add watch forum test.
    /// </summary>
    [Test]
    public void Add_Watch_Forum_Test()
    {
        this.Driver.Navigate()
            .GoToUrl(
                $"{TestConfig.TestForumUrl}topics{TestConfig.TestForumID}.aspx");

        Assert.IsTrue(this.Driver.PageSource.Contains("New Topic"), "Test Forum with that ID doesn't exists");

        // Get Forum Title
        var forumTitle = this.Driver.FindElement(By.ClassName("currentPageLink")).Text;

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Watch Forum"),
            "Watch Forum is disabled, or User already Watches that Forum");

        // Watch the Test Forum
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_WatchForum')]")).ClickAndWait();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("You will now be notified when new posts are made in this forum."),
            "Watch form failed");

        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_subscriptions.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Email Notification Preferences"),
            "Email Notification Preferences is not available for that User");

        Assert.IsTrue(this.Driver.ElementExists(By.LinkText(forumTitle)));
    }

    /// <summary>
    /// Delete watch forum test.
    /// </summary>
    [Test]
    public void Delete_Watch_Forum_Test()
    {
        this.Driver.Navigate()
            .GoToUrl(
                $"{TestConfig.TestForumUrl}topics{TestConfig.TestForumID}.aspx");

        Assert.IsTrue(this.Driver.PageSource.Contains("New Topic"), "Test Forum with that ID doesn't exists");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Unwatch Forum"),
            "Watch Forum is disabled, or User doesn't watch that Forum");

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_WatchForum')]")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("You will no longer be notified when new posts are made in this forum."),
            "Watch forum failed");
    }

    /// <summary>
    /// Receive email on new post in Watched Topic test.
    /// </summary>
    [Test]
    public void Receive_Email_On_New_Post_Test()
    {
        /*Assert.IsTrue(TestConfig.UseTestMailServer, "This Test only works with the Test Mail Server Enabled");

        // Go to Test Topic
        this.Driver.Navigate().GoToUrl(
            "{0}{2}postst{1}.aspx".FormatWith(
                TestConfig.TestForumUrl,
                TestConfig.TestTopicID,
                TestConfig.ForumUrlRewritingPrefix));

        Assert.IsFalse(
            this.Driver.PageSource.Contains("You've passed an invalid value to the forum."),
            "Test Topic Doesn't Exists");

        // Open Topic Options Menu
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_OptionsLink')]")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Watch this topic"),
            "Watch Topic is disabled, or User already Watches that Topic");

        this.Driver.FindElement(By.XPath("//li[contains(@title,'Watch this topic')]")).ClickAndWait();

        Assert.IsTrue(this.Driver.PageSource.Contains("You will now be"), "Watch topic failed");

        this.LogoutUser(false);*/

        // Login as Admin and Post A Reply in the Test Topic
        this.LoginAdminUser();

        Assert.IsTrue(this.CreateNewReplyInTestTopic("Reply Message"), "Reply Message as Admin failed");

        // Check if an Email was received
        while (this.TestMailServer.ReceivedEmailCount.Equals(0))
        {
            Thread.Sleep(5000);
        }

        var mail = this.TestMailServer.ReceivedEmail[0];

        Assert.AreEqual(
            $"{TestConfig.TestUserName.ToLower()}@test.com",
            mail.ToAddresses[0].ToString(),
            "Receiver does not match");

        Assert.IsTrue(mail.FromAddress.ToString().Contains(TestConfig.TestForumMail), "Sender does not match");

        Assert.AreEqual(
            $"Topic Subscription New Post Notification (From {TestConfig.TestApplicationName})",
            mail.Headers["Subject"],
            "Subject does not match");

        Assert.IsTrue(
            mail.MessageParts[0].BodyData.StartsWith("There's a new post in topic \""),
            "Body does not match");
    }

    /// <summary>
    /// Receive email on private message test.
    /// </summary>
    [Test]
    public void Receive_Email_On_Private_Message_Test()
    {
        Assert.IsTrue(TestConfig.UseTestMailServer, "This Test only works with the Test Mail Server Enabled");

        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_subscriptions.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Email Notification Preferences"),
            "Email Notification Preferences is not available for that User");

        // Make sure the option Receive an email notification when you get a new private message?
        var pmEmail = this.Driver.FindElement(By.XPath("//input[contains(@id,'_PMNotificationEnabled')]"));

        if (!pmEmail.Selected)
        {
            pmEmail.Click();

            this.Driver.FindElement(By.XPath("//input[contains(@id,'_SaveUser')]")).Click();
        }

        var testMessage = $"This is an automated Test Message generated at {DateTime.UtcNow}";

        Assert.IsTrue(this.SendPrivateMessage(testMessage), "Test Message Send Failed");

        // Check if an Email was received
        var mail = this.TestMailServer.ReceivedEmail[0];

        Assert.AreEqual(
            $"{TestConfig.TestUserName.ToLower()}@test.com",
            mail.ToAddresses[0].ToString(),
            "Receiver does not match");

        Assert.IsTrue(mail.FromAddress.ToString().Contains(TestConfig.TestForumMail), "Sender does not match");

        Assert.AreEqual(
            $"New Private Message From {TestConfig.TestUserName} at {TestConfig.TestApplicationName}",
            mail.Headers["Subject"],
            "Subject does not match");

        Assert.IsTrue(
            mail.MessageParts[0].BodyData.StartsWith(
                $"A new Private Message from {TestConfig.TestUserName} about {"Testmessage"} was send to you at {TestConfig.TestApplicationName}."),
            "Body does not match");
    }

    /// <summary>
    /// TODO : Receive the email digest test.
    /// </summary>
    [Test]
    [Ignore("Doesnt work yet")]
    public void Receive_Email_Digest_Test()
    {
        Assert.IsTrue(TestConfig.UseTestMailServer, "This Test only works with the Test Mail Server Enabled");

        // Check if Digest is available and enabled
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_subscriptions.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Email Notification Preferences"),
            "Email Notification Preferences is not available for that User");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Receive once daily digest/summary of activity?"),
            "Email Digest is not enabled in that Forum.");
    }
}