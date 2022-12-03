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

using System.Threading;

using YAF.Tests.Utils.Extensions;

/// <summary>
/// The login/log off user tester.
/// </summary>
[TestFixture]
public class LoginLogoutUserTests : TestBase
{
    /// <summary>
    /// Set Up
    /// </summary>
    [OneTimeSetUp]
    public void SetUp()
    {
        this.Driver = !TestConfig.UseExistingInstallation ? TestSetup.TestBase.ChromeDriver : new ChromeDriver();
    }

    /// <summary>
    /// Tears down.
    /// </summary>
    [OneTimeTearDown]
    public void TearDown()
    {
        this.Driver.Quit();
    }

    /// <summary>
    /// Login via Login Page User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public void Login_Page_User_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}Account/login");

        if (this.Driver.ElementExists(By.Id("forum_ctl01_LogOutButton")))
        {
            // Logout First
            this.Driver.FindElement(By.Id("forum_ctl01_LogOutButton")).Click();

            this.Driver.FindElement(By.Id("forum_ctl02_OkButton")).Click();
        }

        this.Driver.LoginUser(TestConfig.TestUserName, TestConfig.TestUserPassword);

        Assert.IsTrue(this.Driver.ElementExists(By.XPath("//*[contains(@id, '_LogOutButton')]")));
    }

    /// <summary>
    /// Login via Login Box User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public void Login_LoginBox_User_Test()
    {
        this.Driver.Navigate().GoToUrl(TestConfig.TestForumUrl);

        if (this.Driver.ElementExists(By.XPath("//*[contains(@id, '_LogOutButton')]")))
        {
            // Logout First
            this.Driver.FindElement(By.XPath("//*[contains(@id, '_LogOutButton')]")).Click();

            this.Driver.FindElement(By.XPath("//*[contains(@id, '_OkButton')]")).Click();
        }

        this.Driver.Navigate().GoToUrl(TestConfig.TestForumUrl);

        var js = (IJavaScriptExecutor)this.Driver;
        js.ExecuteScript("$('.LoginLink').click();");

        Thread.Sleep(400);

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//*[contains(@id, '_UserName')]")),
            "Login Box is disabled in Host Settings");

        this.Driver.FindElement(By.XPath("//*[contains(@id, '_UserName')]")).Clear();
        this.Driver.FindElement(By.XPath("//*[contains(@id, '_UserName')]")).SendKeys(TestConfig.TestUserName);
        this.Driver.FindElement(By.XPath("//*[contains(@id, '_Password')]")).Clear();
        this.Driver.FindElement(By.XPath("//*[contains(@id, '_Password')]")).SendKeys(TestConfig.TestUserPassword);

        this.Driver.FindElement(By.XPath("//*[contains(@id, '_LoginButton')]")).ClickAndWait();

        Assert.IsTrue(this.Driver.PageSource.Contains("Logout"), "Login failed");
    }

    /// <summary>
    /// Logout User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public void Logout_User_Test()
    {
        this.Driver.Navigate().GoToUrl(TestConfig.TestForumUrl);

        if (!this.Driver.ElementExists(By.XPath("//*[contains(@id, '_LogOutButton')]")))
        {
            // Login First
            this.Driver.LoginUser(TestConfig.TestUserName, TestConfig.TestUserPassword);
        }

        Assert.IsTrue(this.Driver.LogOutUser(), "Logout Failed");
    }
}