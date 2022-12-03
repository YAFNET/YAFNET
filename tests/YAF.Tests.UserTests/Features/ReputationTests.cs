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

namespace YAF.Tests.UserTests.Features;

using YAF.Tests.Utils.Extensions;

/// <summary>
/// The user Reputation tests.
/// </summary>
[TestFixture]
[Description("The user Reputation tests.")]
public class ReputationTests : TestBase
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
    /// Add +1 Reputation from TestUser2 to TestUser Test.
    /// </summary>
    [Test]
    [Description("Add +1 Reputation from TestUser2 to TestUser Test.")]
    public void Add_Reputation_To_Test_User_Test()
    {
        // First Creating a new test topic with the test user
        Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");

        var newTestTopicUrl = this.Driver.Url;

        // Now Login with Test User 2
        Assert.IsTrue(
            this.LoginUser(TestConfig.TestUserName2, TestConfig.TestUser2Password),
            "Login with test user 2 failed");

        // Go To New Test Topic Url
        this.Driver.Navigate().GoToUrl(newTestTopicUrl);

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//a[contains(@id,'_AddReputation_0')]")),
            "Reputation is deactivated  in yaf or the user has already voted within the last 24 hours, or the user doesnt have enough points to be allowed to vote");

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_AddReputation_0')]")).Click();

        Assert.IsFalse(
            this.Driver.ElementExists(By.XPath("//a[contains(@id,'_AddReputation_0')]")),
            "Voting failed");
    }

    /// <summary>
    /// Add -1 Reputation from TestUser2 to TestUser Test
    /// </summary>
    [Test]
    [Description("Add -1 Reputation from TestUser2 to TestUser Test")]
    public void Remove_Reputation_From_Test_User_Test()
    {
        // First Creating a new test topic with the test user
        Assert.IsTrue(this.CreateNewTestTopic(), "Topic Creating failed");

        var newTestTopicUrl = this.Driver.Url;

        // Now Login with Test User 2
        Assert.IsTrue(
            this.LoginUser(TestConfig.TestUserName2, TestConfig.TestUser2Password),
            "Login with test user 2 failed");

        // Go To New Test Topic Url
        this.Driver.Navigate().GoToUrl(newTestTopicUrl);

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//a[contains(@id,'_RemoveReputation_0')]")),
            "Reputation is deactivated (Or negative Reputation) in yaf or the user has already voted within the last 24 hours, or the user doesnt have enough points to be allowed to vote");

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_RemoveReputation_0')]")).Click();

        Assert.IsTrue(this.Driver.ElementExists(By.XPath("//a[contains(@id,'_AddReputation_0')]")), "Voting failed");
    }
}