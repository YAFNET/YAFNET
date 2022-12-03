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

using System.IO;

/// <summary>
/// The user Avatar tests.
/// </summary>
[TestFixture]
public class AvatarTests : TestBase
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
    /// Select the avatar from collection test.
    /// </summary>
    [Test]
    public void Select_Avatar_From_Collection_Test()
    {
        // Go to Modify Avatar Page
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}Profile/EditAvatar");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Modify Avatar"),
            "Modify Avatar is not available for that User");

        // Select an Avatar from the Avatar Collection
        Assert.IsTrue(
            this.Driver.PageSource.Contains("Select your Avatar from our Collection"),
            "Avatar Collection not available");

        var select2Product = this.Driver.FindElement(By.CssSelector(".select2-selection"));
        select2Product.Click();

        var searchBox = this.Driver.FindElement(By.ClassName("select2-search__field"));
        searchBox.SendKeys("SampleAvatar.gif");

        var selectedItem = this.Driver.FindElement(By.ClassName("select2-results__option--highlighted"));
        selectedItem.Click();

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_UpdateGallery')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Modify Avatar"), "Modify Avatar Failed");
    }

    /// <summary>
    /// Upload the avatar from computer test.
    /// </summary>
    [Test]
    public void Upload_Avatar_From_Computer_Test()
    {
        // Go to Modify Avatar Page
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}Profile/EditAvatar");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Modify Avatar"),
            "Modify Avatar is not available for that User");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Upload Avatar from Your Computer"),
            "Upload Avatars disabled");

        var filePath = Path.GetFullPath(@"..\..\..\testfiles\avatar.png");

        // Enter Test Avatar
        var fileUpload = this.Driver.FindElement(By.XPath("//input[contains(@id,'_ProfileEditor_File')]"));
        fileUpload.SendKeys(filePath);

        this.Driver.FindElement(By.XPath("//*[contains(@id,'_UpdateUpload')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Modify Avatar"), "Modify Avatar Failed");
    }
}