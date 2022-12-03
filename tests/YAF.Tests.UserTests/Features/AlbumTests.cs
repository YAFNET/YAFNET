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

using System.IO;

using YAF.Tests.Utils.Extensions;

/// <summary>
/// The user album tests.
/// </summary>
[TestFixture]
public class AlbumTests : TestBase
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
    /// Add new user album test.
    /// </summary>
    [Test]
    public void Add_New_User_Album_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_profile.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Edit Albums"),
            "Albums Feature is not available for that User");

        this.Driver.FindElement(By.LinkText("Edit Albums")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Album Images"),
            "Albums Feature is not available for that User");

        // Add New Album
        var addAlbumButton = this.Driver.FindElement(By.XPath("//input[contains(@id,'_AddAlbum')]"));

        Assert.IsNotNull(addAlbumButton, "User has already reached max. Album Limit");

        addAlbumButton.Click();

        // Album Title
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_txtTitle')]")).SendKeys("TestAlbum");
        this.Driver.FindElement(By.XPath("//input[contains(@id,'UpdateTitle')]")).Click();

        // Test Image
        var filePath = Path.GetFullPath(@"..\..\testfiles\avatar.png");

        var fileUpload = this.Driver.FindElement(By.XPath("//input[contains(@id,'_File')]"));
        fileUpload.Click();
        fileUpload.SendKeys(filePath);

        this.Driver.FindElement(By.XPath("//input[contains(@id,'_Upload')]")).Click();

        this.Driver.FindElement(By.XPath("//input[contains(@id,'_Back')]")).ClickAndWait();

        Assert.IsTrue(
            this.Driver.PageSource.Contains($"{TestConfig.TestUserName} Album: TestAlbum"),
            "New Album Creating Failed");
    }

    /// <summary>
    /// Delete the user album test.
    /// </summary>
    [Test]
    public void Delete_User_Album_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_profile.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Edit Albums"),
            "Albums Feature is not available for that User");

        this.Driver.FindElement(By.LinkText("Edit Albums")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Album Images"),
            "Albums Feature is not available for that User");

        // Add New Album
        var edit = this.Driver.FindElement(By.XPath("//input[contains(@id,'_Albums_Edit_0')]"));

        Assert.IsNotNull(edit, "Albums doesn't exists.");

        edit.Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Add/Edit Album"));

        this.Driver.FindElement(By.XPath("//input[contains(@id,'_Delete')]")).Click();

        this.Driver.SwitchTo().Alert().Accept();

        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_profile.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Edit Albums"),
            "Albums Feature is not available for that User");

        this.Driver.FindElement(By.LinkText("Edit Albums")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Album Images"), "Album deleting failed");
    }

    /// <summary>
    /// Add an additional image test.
    /// </summary>
    [Test]
    public void Add_Additional_Image_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_profile.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Edit Albums"),
            "Albums Feature is not available for that User");

        this.Driver.FindElement(By.LinkText("Edit Albums")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Album Images"),
            "Albums Feature is not available for that User");

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//input[contains(@id,'_Albums_Edit_0')]")),
            "Albums doesnt exists.");

        // Edit Album
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_Albums_Edit_0')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Add/Edit Album"));

        // Another Test Image
        var filePath = Path.GetFullPath(@"..\..\testfiles\testImage.jpg");
        var fileUpload = this.Driver.FindElement(By.XPath("//input[contains(@id,'_File')]"));
        fileUpload.Click();
        fileUpload.SendKeys(filePath);

        this.Driver.FindElement(By.XPath("//input[contains(@id,'_Upload')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("testImage.jpg"), "Image Adding Failed");
    }

    /// <summary>
    /// Deletes an image from album test.
    /// </summary>
    [Test]
    public void Delete_Image_From_Album_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_profile.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Edit Albums"),
            "Albums Feature is not available for that User");

        this.Driver.FindElement(By.LinkText("Edit Albums")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Album Images"),
            "Albums Feature is not available for that User");

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//input[contains(@id,'_Albums_Edit_0')]")),
            "Albums doesnt exists.");

        // Edit Album
        this.Driver.FindElement(By.XPath("//input[contains(@id,'_Albums_Edit_0')]")).Click();

        // Get The Images Count
        var textOld = this.Driver.FindElement(By.XPath("//span[contains(@id,'_imagesInfo')]")).Text;

        Assert.IsTrue(this.Driver.PageSource.Contains("Add/Edit Album"));

        this.Driver.FindElement(By.XPath("//a[contains(@id,'_List_ImageDelete_0')]")).Click();

        this.Driver.SwitchTo().Alert().Accept();

        this.Driver.Navigate().Refresh();

        var textNew = this.Driver.FindElement(By.XPath("//span[contains(@id,'_imagesInfo')]")).Text;

        Assert.AreNotEqual(textNew, textOld, "Image deleting failed");
    }

    /// <summary>
    /// Set the image as cover test.
    /// </summary>
    [Test]
    public void Set_Image_As_Cover_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_profile.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Edit Albums"),
            "Albums Feature is not available for that User");

        this.Driver.FindElement(By.LinkText("Edit Albums")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Album Images"),
            "Albums Feature is not available for that User");

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//input[contains(@id,'_Albums_Edit_0')]")),
            "Albums doesnt exists.");

        // View Album
        this.Driver.FindElement(By.XPath("//img[contains(@id,'_coverImage_0')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Album Images"));

        // Set First Album Image as Cover
        var setCoverButton = this.Driver.FindElement(By.XPath("//input[contains(@id,'_AlbumImages_SetCover_0')]"));

        Assert.AreEqual(
            "Set as Cover",
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_AlbumImages_SetCover_0')]"))
                .GetAttribute("value"),
            "Image is already Cover");

        setCoverButton.Click();

        Assert.AreEqual(
            "Remove Cover",
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_AlbumImages_SetCover_0')]"))
                .GetAttribute("value"),
            "Set as Cover Failed");
    }

    /// <summary>
    /// Remove the image as cover test.
    /// </summary>
    [Test]
    public void Remove_Image_As_Cover_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_profile.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Edit Albums"),
            "Albums Feature is not available for that User");

        this.Driver.FindElement(By.LinkText("Edit Albums")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Album Images"),
            "Albums Feature is not available for that User");

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//input[contains(@id,'_Albums_Edit_0')]")),
            "Albums doesnt exists.");

        // View Album
        this.Driver.FindElement(By.XPath("//img[contains(@id,'_coverImage_0')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Album Images"));

        // Remove Cover from first Album Image
        var setCoverButton = this.Driver.FindElement(By.XPath("//input[contains(@id,'_AlbumImages_SetCover_0')]"));

        Assert.AreEqual(
            "Remove Cover",
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_AlbumImages_SetCover_0')]"))
                .GetAttribute("value"),
            "Image is not set as Cover");

        setCoverButton.Click();

        Assert.AreEqual(
            "Set as Cover",
            this.Driver.FindElement(By.XPath("//input[contains(@id,'_AlbumImages_SetCover_0')]"))
                .GetAttribute("value"),
            "Remove as Cover Failed");
    }

    /// <summary>
    /// Edit the image caption test.
    /// </summary>
    [Test]
    public void Edit_Image_Caption_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_profile.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Edit Albums"),
            "Albums Feature is not available for that User");

        this.Driver.FindElement(By.LinkText("Edit Albums")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Album Images"),
            "Albums Feature is not available for that User");

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//input[contains(@id,'_Albums_Edit_0')]")),
            "Albums doesnt exists.");

        // View Album
        this.Driver.FindElement(By.XPath("//img[contains(@id,'_coverImage_0')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Album Images"));

        var imageTitleSpan =
            this.Driver.FindElement(
                By.XPath(
                    "//span[contains(@id,'AlbumImages_spnImageOwner_0')]/descendant::span[@class='albumtitle']"));
        imageTitleSpan.Click();

        var imageTitleInput = this.Driver.FindElement(By.Id(imageTitleSpan.GetAttribute("id").Replace("spn", "txt")));
        imageTitleInput.SendKeys("TestCaption");

        imageTitleInput.SendKeys(Keys.Enter);

        this.Driver.Navigate().Refresh();

        Assert.IsTrue(this.Driver.PageSource.Contains("TestCaption"), "Edit Caption Failed");
    }

    /// <summary>
    /// Edit the album name test.
    /// </summary>
    [Test]
    public void Edit_Album_Name_Test()
    {
        this.Driver.Navigate()
            .GoToUrl($"{TestConfig.TestForumUrl}cp_profile.aspx");

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Edit Albums"),
            "Albums Feature is not available for that User");

        this.Driver.FindElement(By.LinkText("Edit Albums")).Click();

        Assert.IsTrue(
            this.Driver.PageSource.Contains("Album Images"),
            "Albums Feature is not available for that User");

        Assert.IsTrue(
            this.Driver.ElementExists(By.XPath("//input[contains(@id,'_Albums_Edit_0')]")),
            "Albums doesnt exists.");

        // View Album
        this.Driver.FindElement(By.XPath("//img[contains(@id,'_coverImage_0')]")).Click();

        Assert.IsTrue(this.Driver.PageSource.Contains("Album Images"));

        var imageTitleSpan = this.Driver.FindElement(By.ClassName("albumtitle"));
        imageTitleSpan.Click();

        var imageTitleInput = this.Driver.FindElement(By.Id(imageTitleSpan.GetAttribute("id").Replace("spn", "txt")));
        imageTitleInput.Clear();
        imageTitleInput.SendKeys("TestAlbumNameRandom");

        imageTitleInput.SendKeys(Keys.Enter);

        this.Driver.Navigate().Refresh();

        Assert.IsTrue(
            this.Driver.FindElement(By.ClassName("albumtitle")).Text.Contains("TestAlbumNameRandom"),
            "Edit Caption Failed");
    }
}