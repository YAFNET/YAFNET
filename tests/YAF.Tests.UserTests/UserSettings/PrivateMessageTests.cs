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
using System.IO;

using YAF.Tests.Utils.Extensions;

/// <summary>
/// The Private Message tests.
/// </summary>
[TestFixture]
public class PrivateMessageTests : TestBase
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
    /// Send a private message test.
    /// </summary>
    [Test]
    public void Send_Private_Message_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_pm.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Archive"),
            "Private Message Function is not available for that User, or is disabled");

        var testMessage = $"This is an automated Test Message generated at {DateTime.UtcNow}";

        Assert.IsTrue(this.SendPrivateMessage(testMessage), "Test Message Send Failed");

        // Read Message
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_pm.aspx");

        // Get First Message
        this.Driver.FindElement(By.LinkText("Testmessage")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains(testMessage), "Test Message Send Failed");
    }

    /// <summary>
    /// Archive a private message test.
    /// </summary>
    [Test]
    public void Archive_Private_Message_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_pm.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Archive"),
            "Private Message Function is not available for that User, or is disabled");

        // Select the First Message
        var element = this.Driver.FindElement(By.XPath("//input[contains(@id,'_ItemCheck_0')]"));
        var linkText = element.GetAttribute("href");
        element.Click();

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_MessagesView_ArchiveSelected')]")).Click();

        bool messageExists;

        try
        {
            messageExists =
                this.Driver.FindElement(By.XPath("//input[contains(@id,'_ItemCheck_0')]"))
                    .GetAttribute("href")
                    .Equals(linkText);
        }
        catch (Exception)
        {
            messageExists = false;
        }

        Assert.IsFalse(messageExists, "Message Archiving Failed");
    }

    /// <summary>
    /// Export a private message test.
    /// </summary>
    [Test]
    [Ignore("Doesnt work yet")]
    public void Export_Private_Message_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_pm.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Archive"),
            "Private Message Function is not available for that User, or is disabled");

        // Select the First Message
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_ItemCheck_0')]")).Click();

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_MessagesView_ExportSelected')]")).Click();

        // Switch to XML Export
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_ExportType_2')]")).Click();

        var filePath = Path.GetFullPath(@"..\..\testfiles\");

        Assert.Pass("Test Currently doesn't work in IE9");

        this.Driver.DownloadFile("", Path.Combine(filePath, "textExport.txt"));

        var file = new FileStream(Path.Combine(filePath, "textExport.txt"), FileMode.Open, FileAccess.Read);
        var sr = new StreamReader(file);
        var fileContent = sr.ReadToEnd();

        sr.Close();
        file.Close();

        Assert.IsTrue(
            fileContent.Contains("This is an automtated Test Message generated at ."),
            "Message Export Failed");
    }

    /// <summary>
    /// Delete a private message test.
    /// </summary>
    [Test]
    public void Delete_Private_Message_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_pm.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Archive"),
            "Private Message Function is not available for that User, or is disabled");

        // Select the First Message
        var element = this.Driver.FindElement(By.XPath("//input[contains(@id,'_ItemCheck_0')]"));
        var linkText = element.GetAttribute("href");
        element.Click();

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_MessagesView_DeleteSelected')]")).Click();

        this.Driver.SwitchTo().Alert().Accept();

        bool messageExists;

        try
        {
            messageExists =
                this.Driver.FindElement(By.XPath("//input[contains(@id,'_ItemCheck_0')]"))
                    .GetAttribute("href")
                    .Equals(linkText);
        }
        catch (Exception)
        {
            messageExists = false;
        }

        Assert.IsFalse(messageExists, "Message deleting Failed");
    }
}