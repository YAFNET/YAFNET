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

namespace YAF.Tests.AdminTests.Settings
{
    using NUnit.Framework;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;

    using YAF.Tests.Utils;
    using YAF.Tests.Utils.Extensions;

    /// <summary>
    /// The  Host Settings Tests
    /// </summary>
    [TestFixture]
    public class HostSettingsTests : TestBase
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
        /// Basic test to check if the Host Settings are correctly saved
        /// </summary>
        [Test]
        [Ignore("needs to be updated")]
        public void HostSettings_Saved_Correctly_Test()
        {
            this.Driver.Navigate()
                .GoToUrl($"{TestConfig.TestForumUrl}admin_hostsettings.aspx");

            // Modify a random setting
            var input = this.Driver.FindElement(By.XPath("//input[contains(@id,'_ImageAttachmentResizeWidth')]"));
            var width = input.GetAttribute("value");

            input.Clear();
            input.SendKeys("350");

            this.Driver.FindElement(By.XPath("//input[contains(@id,'_Save')]")).Click();

            this.Driver.Navigate()
                .GoToUrl($"{TestConfig.TestForumUrl}admin_hostsettings.aspx");

            input = this.Driver.FindElement(By.XPath("//input[contains(@id,'_ImageAttachmentResizeWidth')]"));

            Assert.IsTrue(input.GetAttribute("value").Equals("350"));

            // Restore old Setting
            input.Clear();
            input.SendKeys(width);

            this.Driver.FindElement(By.XPath("//input[contains(@id,'_Save')]")).ClickAndWait();

            Assert.IsTrue(this.Driver.PageSource.Contains("Who is Online"));
        }
    }
}