/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Tests.Utils;

using System;

using netDumbster.smtp;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using YAF.Tests.Utils.Extensions;

/// <summary>
/// Unit TestBase.
/// </summary>
public class TestBase
{
    /// <summary>
    /// Gets or sets the chrome driver Instance
    /// </summary>
    protected ChromeDriver Driver { get; set; }

    /// <summary>
    /// Gets or sets test Mail Server.
    /// </summary>
    protected SimpleSmtpServer TestMailServer { get; set; }

    /// <summary>
    /// Logins the admin user.
    /// </summary>
    /// <returns>Returns if Successfully or not
    /// </returns>
    protected bool LoginAdminUser()
    {
        this.Driver.LoginUser(TestConfig.AdminUserName, TestConfig.AdminPassword);

        return this.Driver.FindElement(By.XPath("//a[contains(@id,'_LogOutButton')]")) != null;
    }

    /// <summary>
    /// Logins the user.
    /// </summary>
    /// <returns>
    /// Returns if Successfully or not
    /// </returns>
    protected bool LoginUser()
    {
        return this.LoginUser(TestConfig.TestUserName, TestConfig.TestUserPassword);
    }

    /// <summary>
    /// Logins the user.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns>
    /// Returns if Successfully or not
    /// </returns>
    protected bool LoginUser(string userName, string password)
    {
        // Login as Test User
        var loginSucceed = this.Driver.LoginUser(userName, password);

        if (!loginSucceed)
        {
            this.Driver.RegisterUser(this.TestMailServer, userName, password);
        }

        return this.Driver.FindElement(By.XPath("//a[contains(@id,'_LogOutButton')]")) != null;
    }

    /// <summary>
    /// lookouts the user.
    /// </summary>
    /// <param name="closeBrowser">
    /// The close Browser.
    /// </param>
    /// <returns>
    /// Returns if Successfully or not
    /// </returns>
    protected bool LogoutUser(bool closeBrowser = true)
    {
        this.Driver.Navigate().GoToUrl(TestConfig.TestForumUrl);

        try
        {
            var logOut = this.Driver.LogOutUser();

            if (closeBrowser)
            {
                this.Driver.Quit();
            }

            return logOut;
        }
        catch (NoSuchElementException)
        {
            if (closeBrowser)
            {
                this.Driver.Quit();
            }
        }

        return false;
    }

    /// <summary>
    /// Creates the new test topic.
    /// </summary>
    /// <returns>
    /// Returns if Creating of the 
    /// New Topic was successfully or not
    /// </returns>
    protected bool CreateNewTestTopic()
    {
        this.Driver.Navigate().GoToUrl(
            $"{TestConfig.TestForumUrl}postmessage.aspx?f={TestConfig.TestForumID}");

        if (!this.Driver.PageSource.Contains("Post New Topic"))
        {
            return false;
        }

        // Create New Topic
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_TopicSubjectTextBox')]")).SendKeys(
            $"Auto Created Test Topic - {DateTime.UtcNow}");

        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]")).SendKeys("This is a Test Message Created by an automated Unit Test");

        // Post New Topic
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReply')]")).Click();

        return this.Driver.PageSource.Contains("Next Topic");
    }

    /// <summary>
    /// Creates a new reply in the test topic.
    /// </summary>
    /// <param name="message">The Reply message.</param>
    /// <returns>
    /// Returns if Reply was Created or not
    /// </returns>
    protected bool CreateNewReplyInTestTopic(string message)
    {
        // Go to Post New Topic
        this.Driver.Navigate().GoToUrl($"{TestConfig.TestForumUrl}postst{TestConfig.TestTopicID}.aspx");

        if (this.Driver.PageSource.Contains("You've passed an invalid value to the forum."))
        {
            return false;
        }

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReplyLink1')]")).Click();

        if (!this.Driver.PageSource.Contains("Post a reply"))
        {
            return false;
        }

        // Create New Reply
        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]")).SendKeys(message);

        // Post New Topic
        this.Driver.FindElement(By.XPath("//a[contains(@id,'_PostReply')]")).ClickAndWait();

        return this.Driver.PageSource.Contains(message);
    }

    /// <summary>
    /// Sends a private message.
    /// </summary>
    /// <param name="testMessage">The test message.</param>
    /// <returns>If the Message was sent or not</returns>
    protected bool SendPrivateMessage(string testMessage)
    {
        this.Driver.Navigate().GoToUrl(
            $"{TestConfig.TestForumUrl}pmessage.aspx");

        // Send a Message to Myself
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_To')]")).SendKeys(TestConfig.TestUserName);

        this.Driver.FindElement(By.XPath("//input[contains(@id,'_PmSubjectTextBox')]")).SendKeys("Testmessage");
        this.Driver.FindElement(By.XPath("//textarea[contains(@id,'_YafTextEditor')]")).SendKeys(testMessage);

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_Save')]")).Click();

        // Check if MessageBox is Shown
        return this.Driver.PageSource.Contains("unread message(s)");
    }
}