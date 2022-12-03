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
/// The user friend tests.
/// </summary>
[TestFixture]
public class FriendTests : TestBase
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
    /// Request Friend Test
    /// Login as Admin and add the TestUser as Friend
    /// </summary>
    [Test]
    public void Request_Friend_Test()
    {
        this.LoginAdminUser();

        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_editbuddies.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Pending Requests"),
            "My Friends function is not available for that User, or is disabled for that Forum");

        // Go to Members Page and Find the Test User 
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}members.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Search Members"),
            "Members List View Permissions needs to be disabled.");

        this.Driver.FindElement(By.XPath("//input[contains(@id,'_UserSearchName')]"))
            .SendKeys(TestConfig.TestUserName);

        this.Driver.FindElement(By.XPath("//input[contains(@id,'_SearchByUserName')]")).ClickAndWait();

        var userProfileLink = this.Driver.FindElement(By.LinkText(TestConfig.TestUserName));

        Assert.IsNotNull(userProfileLink, "User Profile Not Found");

        userProfileLink.Click();

        Assert.IsFalse(this.Driver.PageSource.Contains("Remove Friend"), "User is already a Friend");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Add as Friend"),
            "My Friends function is not available for that User, or is disabled for that Forum");

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_lnkBuddy')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Friend request sent."));
    }

    /// <summary>
    /// Approve the Friend request test.
    /// </summary>
    [Test]
    public void Approve_Friend_Request_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_editbuddies.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Pending Requests"),
            "My Friends function is not available for that User, or is disabled for that Forum");

        this.Driver.FindElement(By.LinkText("Pending Requests")).Click();

        // Select the First Request
        this.Driver.FindElement(By.LinkText("Approve")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("You have been added to "), "Approve Friend Failed");
    }

    /// <summary>
    /// Deny the Newest Friend Request test.
    /// </summary>
    [Test]
    public void Deny_Friend_Request_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_editbuddies.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Pending Requests"),
            "My Friends function is not available for that User, or is disabled for that Forum");

        this.Driver.FindElement(By.LinkText("Pending Requests")).Click();

        // Select the First Request
        this.Driver.FindElement(By.LinkText("Deny")).Click();

        this.Driver.SwitchTo().Alert().Accept();

        Assert.IsTrue(this.Driver.PageSource.Contains("Friend request denied."), "Deny Request Failed");
    }

    /// <summary>
    /// Approve and add Friend request test.
    /// </summary>
    [Test]
    public void Approve_and_Add_Friend_Request_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_editbuddies.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Pending Requests"),
            "My Friends function is not available for that User, or is disabled for that Forum");

        this.Driver.FindElement(By.LinkText("Pending Requests")).Click();

        // Select the First Request
        this.Driver.FindElement(By.LinkText("Approve and Add")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("You and "), "Approve and Add Friend Failed");
    }

    /// <summary>
    /// Remove a Friend test.
    /// </summary>
    [Test]
    public void Remove_Friend_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_editbuddies.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Pending Requests"),
            "My Friends function is not available for that User, or is disabled for that Forum");

        // Select the Newest Friend
        var delete = this.Driver.FindElement(By.XPath("//a[contains(@id,'_BuddyList1_rptBuddy_lnkRemove_0')]"));

        Assert.IsNotNull(delete, "Currently the Test User doesnt have any Friends");

        delete.Click();

        this.Driver.SwitchTo().Alert().Accept();
    }
}