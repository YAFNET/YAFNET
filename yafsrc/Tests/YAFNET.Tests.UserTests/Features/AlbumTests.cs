/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

/// <summary>
/// The user album tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AlbumTests : TestBase
{
    /// <summary>
    /// Add new user album test.
    /// </summary>
    [Test, Order(1)]
    public async Task AddNewUserAlbumTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyAccount");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                    await page.GetByText("Edit Albums").First.ClickAsync();

                    // Add New Album
                    var addAlbumButton = page.GetByText("Add New Album");

                    Assert.That(addAlbumButton, Is.Not.Null, "User has already reached max. Album Limit");

                    await addAlbumButton.ClickAsync();

                    // Album Title
                    await page.Locator("#AlbumTitle").FillAsync("TestAlbum");

                    // Test Image
                    var filePath = Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png");

                    // Enter Test Avatar
                    await page.Locator("#ImageFiles").SetInputFilesAsync(filePath);

                    await page.Locator("//button[contains(@formaction,'Upload')]").ClickAsync();

                    await page.Locator("//button[contains(@formaction,'Back')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain($"{this.Base.TestSettings.TestUserName} Album: TestAlbum"),
                        "New Album Creating Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Add an additional image test.
    /// </summary>
    [Test, Order(2)]
    public async Task AddAdditionalImageTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyAccount");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                    await page.GetByText("Edit Albums").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    await Assert.MultipleAsync(async () =>
                    {
                        Assert.That(pageSource, Does.Contain("albums"), "Albums Feature is not available for that User");

                        Assert.That(await page.GetByText("Add New Album").IsVisibleAsync(), Is.True, "Albums doesn't exists.");
                    });

                    // Edit Album
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Add/Edit Album"));

                    // Another Test Image
                    var filePath = Path.GetFullPath(@"..\..\..\..\testfiles\testImage.jpg");
                    await page.Locator("#ImageFiles").SetInputFilesAsync(filePath);

                    await page.Locator("//button[contains(@formaction,'Upload')]").ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("testImage.jpg"), "Image Adding Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Set the image as cover test.
    /// </summary>
    [Test, Order(3)]
    public async Task SetImageAsCoverTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyAccount");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                    await page.GetByText("Edit Albums").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    await Assert.MultipleAsync(async () =>
                    {
                        Assert.That(pageSource, Does.Contain("albums"), "Albums Feature is not available for that User");

                        Assert.That(await page.GetByText("Add New Album").IsVisibleAsync(), Is.True, "Albums doesn't exists.");
                    });

                    // View Album
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "View" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Album Images"));

                    // Set First Album Image as Cover
                    var setCoverButton = page.Locator(".btn-cover").First;

                    Assert.That(await setCoverButton.TextContentAsync(), Is.EqualTo("Set as Cover"), "Image is already Cover");

                    await setCoverButton.ClickAsync();

                    setCoverButton = page.Locator(".btn-cover").First;

                    Assert.That(await setCoverButton.TextContentAsync(), Is.EqualTo("Remove Cover"), "Set as Cover Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Remove the image as cover test.
    /// </summary>
    [Test, Order(4)]
    public async Task RemoveImageAsCoverTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyAccount");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                    await page.GetByText("Edit Albums").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    await Assert.MultipleAsync(async () =>
                    {
                        Assert.That(pageSource, Does.Contain("albums"), "Albums Feature is not available for that User");

                        Assert.That(await page.GetByText("Add New Album").IsVisibleAsync(), Is.True, "Albums doesn't exists.");
                    });

                    // View Album
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "View" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Album Images"));

                    // Remove Cover from first Album Image
                    var setCoverButton = page.Locator(".btn-cover").First;

                    Assert.That(
                        await setCoverButton.TextContentAsync(), Is.EqualTo("Remove Cover"),
                        "Image is not set as Cover");

                    await setCoverButton.ClickAsync();

                    setCoverButton = page.Locator(".btn-cover").First;

                    Assert.That(await setCoverButton.TextContentAsync(), Is.EqualTo("Set as Cover"), "Remove as Cover Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Edit the image caption test.
    /// </summary>
    [Test, Order(5)]
    public async Task EditImageCaptionTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyAccount");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                    await page.GetByText("Edit Albums").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    await Assert.MultipleAsync(async () =>
                    {
                        Assert.That(pageSource, Does.Contain("albums"), "Albums Feature is not available for that User");

                        Assert.That(await page.GetByText("Add New Album").IsVisibleAsync(), Is.True, "Albums doesn't exists.");
                    });

                    // View Album
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "View" }).First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Album Images"));

                    await page.Locator(".dark-editable-element").First.ClickAsync();

                    var captionTextBox = page.GetByRole(AriaRole.Textbox);

                    await captionTextBox.ClickAsync();

                    await captionTextBox.ClearAsync();

                    var testCaption = $"TestCaption{DateTime.UtcNow.Ticks}";

                    await captionTextBox.FillAsync(testCaption);

                    await captionTextBox.PressAsync("Enter");

                    await Task.Delay(15000);

                    await page.ReloadAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain(testCaption), "Edit Caption Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Edit the album title test.
    /// </summary>
    [Test, Order(6)]
    public async Task EditAlbumTitleTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.Base.TestSettings,
                        this.Base.TestSettings.TestUserName,
                        this.Base.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                // Do actual test
                await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyAccount");

                var pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                await page.GetByText("Edit Albums").First.ClickAsync();

                pageSource = await page.ContentAsync();

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(pageSource, Does.Contain("albums"), "Albums Feature is not available for that User");

                    Assert.That(await page.GetByText("Add New Album").IsVisibleAsync(), Is.True, "Albums doesn't exists.");
                });

                await page.Locator(".album-caption").First.ClickAsync();

                var captionTextBox = page.GetByRole(AriaRole.Textbox);

                await captionTextBox.ClickAsync();

                await captionTextBox.ClearAsync();

                await captionTextBox.FillAsync("Test Album");

                await captionTextBox.PressAsync("Enter");

                await page.ReloadAsync();

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Test Album"), "Edit Title Failed");
            },
            this.BrowserType);
    }

    /// <summary>
    /// Deletes an image from album test.
    /// </summary>
    [Test, Order(7)]
    public async Task DeleteImageFromAlbumTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.Base.TestSettings,
                        this.Base.TestSettings.TestUserName,
                        this.Base.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                // Do actual test
                await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyAccount");

                var pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                await page.GetByText("Edit Albums").First.ClickAsync();

                pageSource = await page.ContentAsync();

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(pageSource, Does.Contain("albums"), "Albums Feature is not available for that User");

                    Assert.That(await page.GetByText("Add New Album").IsVisibleAsync(), Is.True, "Albums doesn't exists.");
                });

                // Edit Album
                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit" }).First.ClickAsync();

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Add/Edit Album"));

                // Get The Images Count
                var textOld = await page.GetByText("Delete", new PageGetByTextOptions { Exact = true }).CountAsync();

                await page.GetByText("Delete", new PageGetByTextOptions { Exact = true }).First.ClickAsync();

                await page.Locator(".btn-success").ClickAsync();

                await page.ReloadAsync();

                var textNew = await page.GetByText("Delete", new PageGetByTextOptions { Exact = true }).CountAsync();

                Assert.That(textOld, Is.Not.EqualTo(textNew), "Image deleting failed");
            },
            this.BrowserType);
    }

    /// <summary>
    /// Delete the user album test.
    /// </summary>
    [Test, Order(8)]
    public async Task DeleteUserAlbumTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.Base.TestSettings,
                        this.Base.TestSettings.TestUserName,
                        this.Base.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                // Do actual test
                await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyAccount");

                var pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                await page.GetByText("Edit Albums").First.ClickAsync();

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit" }).First.ClickAsync();

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Add/Edit Album"));

                var albumTitle = await page.Locator("#AlbumTitle").GetAttributeAsync("Value");

                Assert.That(albumTitle, Is.Not.Null);

                await page.GetByText("Delete Album").ClickAsync();

                await page.Locator(".btn-success").ClickAsync();

                await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}MyAccount");

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                await page.GetByText("Edit Albums").First.ClickAsync();

                pageSource = await page.ContentAsync();

                Assert.That(pageSource.Contains(albumTitle), Is.False, "Album deleting failed");
            },
            this.BrowserType);
    }
}