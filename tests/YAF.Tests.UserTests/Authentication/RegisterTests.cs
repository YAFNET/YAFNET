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

namespace YAF.Tests.UserTests.Authentication;

using System;

using netDumbster.smtp;

using YAF.Tests.Utils.Extensions;
using YAF.Types.Extensions;

/// <summary>
/// The Register a test user Test.
/// </summary>
[TestFixture]
public class RegisterUser : TestBase
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
    }

    /// <summary>
    /// Logout New User
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this.Driver.Quit();
    }

    /// <summary>
    /// Register Random Test User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public void Register_Random_New_User_Test()
    {
        // Create New Random Test User
        var random = new Random();

        var userName = $"TestUser{random.Next()}";

        Assert.IsTrue(this.Driver.RegisterUser(this.TestMailServer, userName, TestConfig.TestUserPassword), "Registration failed");
    }

    /// <summary>
    /// Register Random Test User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public void Register_Bot_User_Test()
    {
        const string USERNAME = "aqiuliqemi";
        const string EMAIL = "ikocec@coveryourpills.org";

        this.Driver.Navigate().GoToUrl($"{TestConfig.TestForumUrl}Account/Register");

        // Check if Registrations are Disabled
        Assert.IsFalse(
            this.Driver.PageSource.Contains("You tried to enter an area where you didn't have access"),
            "Registrations are disabled");

        // Accept the Rules
        if (this.Driver.PageSource.Contains("Forum Rules"))
        {
            this.Driver.FindElement(By.Id("forum_ctl04_Login1_LoginButton")).Click();
            this.Driver.Navigate().Refresh();
        }

        Assert.IsFalse(
            this.Driver.PageSource.Contains("Security Image"),
            "Captchas needs to be disabled in order to run the tests");

        // Fill the Register Page
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_UserName')]")).SendKeys(USERNAME);

        if (this.Driver.PageSource.Contains("Display Name"))
        {
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_DisplayName')]")).SendKeys(USERNAME);
        }

        this.Driver.FindElement(
                By.XPath("//input[contains(@id,'_Password')]"))
            .SendKeys(TestConfig.TestUserPassword);
        this.Driver.FindElement(
                By.XPath("//input[contains(@id,'_ConfirmPassword')]"))
            .SendKeys(TestConfig.TestUserPassword);
        this.Driver.FindElement(
            By.XPath("//input[contains(@id,'_Email')]")).SendKeys(EMAIL);

        // Create User
        this.Driver.FindElement(
            By.XPath("//a[contains(@id,'_CreateUser')]")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Sorry Spammers are not allowed in the Forum!"),
            "Spam Check Failed");
    }
}