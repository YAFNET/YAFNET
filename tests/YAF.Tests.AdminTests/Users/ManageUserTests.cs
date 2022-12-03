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

namespace YAF.Tests.AdminTests.Users
{
    using System.Threading;

    using NUnit.Framework;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;

    using YAF.Tests.Utils;
    using YAF.Tests.Utils.Extensions;

    /// <summary>
    /// The Manage User Tests
    /// </summary>
    [TestFixture]
    public class ManageUserTests : TestBase
    {
        /// <summary>
        /// Login User Setup
        /// </summary>
        [OneTimeSetUp]
        public void SetUpTest()
        {
            this.Driver = !TestConfig.UseExistingInstallation ? TestSetup.TestBase.ChromeDriver : new ChromeDriver();

            Assert.IsTrue(this.LoginAdminUser(), "Login failed");
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
        /// Delete the random test user test.
        /// </summary>
        [Test]
        [Description("Deletes the a Random Test User Account")]
        public void Delete_Random_Test_User_Test()
        {
            this.Driver.Navigate()
                .GoToUrl($"{TestConfig.TestForumUrl}admin_users.aspx");

            // Search for TestUser
            var searchNameInput = this.Driver.FindElement(By.XPath("//input[contains(@id,'_name')]"));
            searchNameInput.SendKeys(TestConfig.TestUserName);
            searchNameInput.SendKeys(Keys.Enter);

            Thread.Sleep(5000);

            Assert.IsTrue(
                this.Driver.ElementExists(By.XPath("//a[contains(@id,'_NameEdit_2')]")),
                "Random Test User doesn't Exist, Please Run the Register_Random_New_User_Test before");

            this.Driver.FindElement(By.XPath("//a[contains(@id,'_ThemeButtonDelete_2')]")).Click();

            this.Driver.SwitchTo().Alert().Accept();
        }

        /// <summary>
        /// The Add User to Role Test.
        /// </summary>
        [Test]
        [Description("Assign the TestUser Account with the Moderator Role")]
        public void Add_User_To_Test_Role_Test()
        {
            this.Driver.Navigate()
                .GoToUrl($"{TestConfig.TestForumUrl}admin_users.aspx");

            // Search for TestUser
            var searchNameInput = this.Driver.FindElement(By.XPath("//input[contains(@id,'_name')]"));
            searchNameInput.SendKeys(TestConfig.TestUserName);
            searchNameInput.SendKeys(Keys.Enter);

            var userProfileLink = this.Driver.FindElement(By.XPath("//a[contains(@id,'_NameEdit_0')]"));

            Assert.IsNotNull(userProfileLink, "Test User doesn't Exist");

            userProfileLink.Click();

            // Go To Tab User Roles
            this.Driver.FindElement(By.Id("ui-id-2")).Click();

            // Add TestUser to Moderator User Role
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_UserGroups_GroupMember_2')]")).Click();

            // Save changes
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_GroupEditControl_Save')]")).Click();

            this.Driver.Navigate()
                .GoToUrl($"{TestConfig.TestForumUrl}admin_users.aspx");

            // Search for TestUser
            searchNameInput = this.Driver.FindElement(By.XPath("//input[contains(@id,'_name')]"));
            searchNameInput.SendKeys(TestConfig.TestUserName);
            searchNameInput.SendKeys(Keys.Enter);

            userProfileLink = this.Driver.FindElement(By.XPath("//a[contains(@id,'_NameEdit_0')]"));

            Assert.IsNotNull(userProfileLink, "Test User doesn't Exist");

            userProfileLink.Click();

            // Go To Tab User Roles
            this.Driver.FindElement(By.Id("ui-id-2")).Click();

            Assert.IsTrue(
                this.Driver.FindElement(By.XPath("//input[contains(@id,'_UserGroups_GroupMember_2')]")).Selected);

            this.Driver.FindElement(By.XPath("//input[contains(@id,'_UserGroups_GroupMember_2')]")).Click();

            // Save changes
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_GroupEditControl_Save')]")).Click();
        }
    }
}