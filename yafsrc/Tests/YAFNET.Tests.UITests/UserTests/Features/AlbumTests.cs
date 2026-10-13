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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Features;

/// <summary>
/// The user album tests.
/// </summary>
public class AlbumTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Add new user album test.
    /// </summary>
    [Test]
    public async Task AddNewUserAlbumTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    if (this.TestSettings.NewInstall)
                    {
                        // Enable Album Feature first
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.AdminUserName,
                            this.TestSettings.AdminPassword);

                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Host" }).ClickAsync();
                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Host Settings" }).ClickAsync();
                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Host Setup" }).ClickAsync();
                        await page.GetByRole(AriaRole.Tab, new PageGetByRoleOptions { Name = "Album Settings" }).ClickAsync();
                        await page.GetByRole(AriaRole.Checkbox, new PageGetByRoleOptions { Name = "Enable Album Feature:" }).CheckAsync();
                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save Settings" }).ClickAsync();
                    }

                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");

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

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Upload')]"));

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Back')]"));

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain($"{this.TestSettings.TestUserName} Album: TestAlbum"),
                        "New Album Creating Failed");
                });
    }

    /// <summary>
    /// Add additional image test.
    /// </summary>
    [Test, DependsOnTest(nameof(AddNewUserAlbumTest))]
    public async Task AddAdditionalImageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");

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

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Upload')]"));

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("testImage.jpg"), "Image Adding Failed");
                });
    }

    /// <summary>
    /// Set the image as cover test.
    /// </summary>
    [Test, DependsOnTest(nameof(AddAdditionalImageTest))]
    public async Task SetImageAsCoverTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");

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
                });
    }

    /// <summary>
    /// Remove the image as cover test.
    /// </summary>
    [Test, DependsOnTest(nameof(SetImageAsCoverTest))]
    public async Task RemoveImageAsCoverTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");

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
                });
    }

    /// <summary>
    /// Edit the image caption test.
    /// </summary>
    [Test, DependsOnTest(nameof(RemoveImageAsCoverTest))]
    public async Task EditImageCaptionTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");

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
                });
    }

    /// <summary>
    /// Edit the album title test.
    /// </summary>
    [Test, DependsOnTest(nameof(EditImageCaptionTest))]
    public async Task EditAlbumTitleTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                // Do actual test
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");

                var pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                await page.GetByText("Edit Albums").First.ClickAsync();

                pageSource = await page.ContentAsync();

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(pageSource, Does.Contain("albums"), "Albums Feature is not available for that User");

                    Assert.That(await page.GetByText("Add New Album").IsVisibleAsync(), Is.True, "Albums doesn't exists.");
                });

                await page.Locator(".dark-editable-element").First.ClickAsync();

                var captionTextBox = page.GetByRole(AriaRole.Textbox);

                await captionTextBox.ClickAsync();

                await captionTextBox.ClearAsync();

                await captionTextBox.FillAsync("Test Album");

                await captionTextBox.PressAsync("Enter");

                await Task.Delay(15000);

                await page.ReloadAsync();

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Test Album"), "Edit Title Failed");
            });
    }

    /// <summary>
    /// Deletes an image from album test.
    /// </summary>
    [Test, DependsOnTest(nameof(EditAlbumTitleTest))]
    public async Task DeleteImageFromAlbumTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                // Do actual test
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");

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

                var textNew = await page.GetByText("Delete", new PageGetByTextOptions { Exact = true }).CountAsync();

                Assert.That(textOld, Is.Not.EqualTo(textNew), "Image deleting failed");
            });
    }

    /// <summary>
    /// Delete the user album test.
    /// </summary>
    [Test, DependsOnTest(nameof(DeleteImageFromAlbumTest))]
    public async Task DeleteUserAlbumTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword), Is.True,
                    "Login failed");

                // Do actual test
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");

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

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");

                pageSource = await page.ContentAsync();

                Assert.That(pageSource, Does.Contain("Edit Albums"), "Albums Feature is not available for that User");

                await page.GetByText("Edit Albums").First.ClickAsync();

                pageSource = await page.ContentAsync();

                Assert.That(pageSource.Contains(albumTitle), Is.False, "Album deleting failed");
            });
    }

    /// <summary>
    /// An album title with special characters is shown as entered (not html encoded twice).
    /// </summary>
    [Test]
    public async Task AlbumTitleWithSpecialCharactersTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAccount");
                    await page.SubmitAndWaitAsync(page.GetByText("Edit Albums").First);

                    var albumsUrl = page.Url;

                    Assert.That(
                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit" }).CountAsync(),
                        Is.GreaterThan(0),
                        "The user has no album");

                    await page.SubmitAndWaitAsync(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Edit" }).First);

                    var editUrl = page.Url;
                    var oldTitle = await page.Locator("#AlbumTitle").InputValueAsync();
                    const string title = "Tom & Jerry <3";

                    await page.Locator("#AlbumTitle").FillAsync(title);
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateTitle')]"));

                    // a new visit of the form (right after the post it shows the posted value)
                    await page.GotoAsync(editUrl);
                    var shownInForm = await page.Locator("#AlbumTitle").InputValueAsync();

                    await page.GotoAsync(albumsUrl);

                    var shownInList = await page.Locator("body").InnerTextAsync();

                    // back to the old title for the other tests
                    await page.GotoAsync(editUrl);
                    await page.Locator("#AlbumTitle").FillAsync(System.Net.WebUtility.HtmlDecode(oldTitle));
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateTitle')]"));

                    Assert.That(shownInForm, Is.EqualTo(title), "The edit form shows the title encoded");
                    Assert.That(shownInList, Does.Contain(title), "The album list shows the title encoded");
                });
    }

    /// <summary>
    /// The limit of images per album also applies to an upload of several images at once.
    /// </summary>
    [Test, DependsOnTest(nameof(AddNewUserAlbumTest))]
    public async Task UploadManyImagesRespectsLimitTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // a new user without albums
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    const int fileCount = 40;

                    var folder = Path.Combine(Path.GetTempPath(), $"yaf-album-{Guid.NewGuid():N}");
                    Directory.CreateDirectory(folder);

                    try
                    {
                        var source = Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png");

                        var files = Enumerable.Range(1, fileCount).Select(
                            i =>
                                {
                                    var file = Path.Combine(folder, $"image{i}.png");
                                    File.Copy(source, file);
                                    return file;
                                }).ToArray();

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditAlbumImages/new");

                        await page.Locator("#AlbumTitle").FillAsync("Many images");
                        await page.Locator("#ImageFiles").SetInputFilesAsync(files);

                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Upload')]"));

                        var info = Regex.Match(await page.Locator("body").InnerTextAsync(), @"allowed to have (\d+) images");

                        Assert.That(info.Success, Is.True, "The image limit is not shown");

                        var allowed = int.Parse(info.Groups[1].Value);

                        Assert.That(allowed, Is.LessThan(fileCount), "The test needs more files than allowed");

                        Assert.That(
                            await page.Locator("button[formaction*='DeleteImage']").CountAsync(),
                            Is.EqualTo(allowed),
                            "More images than allowed were saved");
                    }
                    finally
                    {
                        Directory.Delete(folder, true);
                    }
                });
    }

    /// <summary>
    /// The active users page shows the locations of visitors at albums and profiles: an album that does not
    /// exist (anymore) and the albums of a user (/Albums/{id}, without a name) failed the page, the name of
    /// a profile location was the text of the address.
    /// </summary>
    [Test]
    public async Task ActiveUsersWithAlbumAndProfileLocationsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var spoofedName = $"Spoofed-Name-{Random.Shared.Next()}";

                    // guests at these addresses, they don't follow the redirects (the location stays)
                    var contexts = new List<IBrowserContext>();

                    foreach (var path in new[] { "Album/1/999999", "Albums/1", $"UserProfile/1/{spoofedName}" })
                    {
                        var guestContext = await this.Fixture.Context.Browser!.NewContextAsync(
                                               new BrowserNewContextOptions
                                                   {
                                                       BaseURL = this.TestSettings.TestForumUrl,
                                                       IgnoreHTTPSErrors = true
                                                   });

                        contexts.Add(guestContext);

                        await guestContext.APIRequest.GetAsync(
                            $"{this.TestSettings.TestForumUrl}{path}",
                            new APIRequestContextOptions { MaxRedirects = 0 });
                    }

                    try
                    {
                        Assert.That(
                            await page.LoginUserAsync(
                                this.TestSettings,
                                this.TestSettings.AdminUserName,
                                this.TestSettings.AdminPassword),
                            Is.True,
                            "Login failed");

                        var response = await page.GotoAsync($"{this.TestSettings.TestForumUrl}ActiveUsers/2");

                        Assert.That(response?.Status, Is.EqualTo(200), "The active users page failed");
                        Assert.That(page.Url, Does.Contain("ActiveUsers"), $"The active users page is not shown ({page.Url})");

                        Assert.That(
                            await page.ContentAsync(),
                            Does.Not.Contain(spoofedName),
                            "The text of the address is shown as the name of the user");
                    }
                    finally
                    {
                        foreach (var context in contexts)
                        {
                            await context.DisposeAsync();
                        }
                    }
                });
    }

    /// <summary>
    /// A long image caption with special characters is saved (it is stored html encoded, the encoded text
    /// was longer than the column and the database refused it, except Sqlite).
    /// </summary>
    [Test, DependsOnTest(nameof(AddNewUserAlbumTest))]
    public async Task LongImageCaptionWithSpecialCharactersTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // an own album (the other tests delete albums)
                    var albumTitle = $"Caption album {Random.Shared.Next()}";

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditAlbumImages/new");
                    await page.Locator("#AlbumTitle").FillAsync(albumTitle);
                    await page.Locator("#ImageFiles").SetInputFilesAsync(Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png"));
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Upload')]"));
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Back')]"));

                    // "Back" shows the new album
                    var albumUrl = page.Url;
                    var albumId = Regex.Match(albumUrl, @"/Album/\d+/(\d+)").Groups[1].Value;

                    Assert.That(albumId, Is.Not.Empty, $"The album was not created ({albumUrl})");

                    // 237 characters, 301 html encoded
                    var caption = $"Caption {Guid.NewGuid():N} {string.Concat(Enumerable.Repeat("Tom & Jerry ", 16))}".Trim();

                    await page.Locator(".dark-editable-element").First.ClickAsync();

                    var captionTextBox = page.GetByRole(AriaRole.Textbox);

                    await captionTextBox.FillAsync(caption);

                    var response = await page.RunAndWaitForResponseAsync(
                                       () => captionTextBox.PressAsync("Enter"),
                                       r => r.Url.Contains("ChangeImageCaption"));

                    Assert.That(response.Status, Is.EqualTo(200), "Saving the caption failed");

                    await page.GotoAsync(albumUrl);

                    var shown = (await page.Locator(".album-image-caption").First.InnerTextAsync()).Trim();

                    Assert.That(shown, Does.StartWith($"Caption "), "The caption was not saved");
                    Assert.That(caption, Does.StartWith(shown), "The caption is not shown as entered");
                    Assert.That(
                        System.Net.WebUtility.HtmlEncode(shown).Length,
                        Is.LessThanOrEqualTo(255),
                        "The stored caption is longer than the column");

                    // remove the album again (album limit)
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditAlbumImages/{albumId}");
                    await page.GetByText("Delete Album").ClickAsync();
                    await page.SubmitAndWaitAsync(page.Locator(".btn-success"));
                });
    }

    /// <summary>
    /// Two images with the same file name (a phone names every photo "image.jpg") are both kept: the second
    /// upload overwrote the file of the first image and deleting one image removed the file of the other.
    /// </summary>
    [Test, DependsOnTest(nameof(AddNewUserAlbumTest))]
    public async Task ImagesWithSameFileNameTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // a new user without albums
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    var first = Path.Combine(Path.GetTempPath(), $"yaf-album-{Guid.NewGuid():N}");
                    var second = Path.Combine(Path.GetTempPath(), $"yaf-album-{Guid.NewGuid():N}");
                    Directory.CreateDirectory(first);
                    Directory.CreateDirectory(second);

                    try
                    {
                        var firstFile = Path.Combine(first, "image.png");
                        var secondFile = Path.Combine(second, "image.png");
                        File.Copy(Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png"), firstFile);
                        File.Copy(Path.GetFullPath(@"..\..\..\..\testfiles\testImage.jpg"), secondFile);

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}EditAlbumImages/new");

                        await page.Locator("#AlbumTitle").FillAsync("Same names");
                        await page.Locator("#ImageFiles").SetInputFilesAsync(firstFile);
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Upload')]"));

                        await page.Locator("#ImageFiles").SetInputFilesAsync(secondFile);
                        await page.SubmitAndWaitAsync(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Upload", Exact = true }));

                        var previews = page.Locator("img.album-image-thumb");

                        Assert.That(await previews.CountAsync(), Is.EqualTo(2), "Both images are listed");

                        var urls = new List<string>();
                        var shown = new List<byte[]>();

                        for (var i = 0; i < 2; i++)
                        {
                            urls.Add(new Uri(new Uri(page.Url), await previews.Nth(i).GetAttributeAsync("src")).ToString());
                            shown.Add(await (await page.APIRequest.GetAsync(urls[i])).BodyAsync());
                        }

                        Assert.That(
                            shown,
                            Is.EquivalentTo(new[] { await File.ReadAllBytesAsync(firstFile), await File.ReadAllBytesAsync(secondFile) }),
                            "One image shows the file of the other image");

                        // delete the first listed image, the other one is still there
                        await page.GetByText("Delete", new PageGetByTextOptions { Exact = true }).First.ClickAsync();
                        await page.SubmitAndWaitAsync(page.Locator(".btn-success"));

                        var response = await page.APIRequest.GetAsync(urls[1]);

                        Assert.That(response.Status, Is.EqualTo(200), "Deleting one image removed the file of the other image");
                        Assert.That(await response.BodyAsync(), Is.EqualTo(shown[1]));
                    }
                    finally
                    {
                        Directory.Delete(first, true);
                        Directory.Delete(second, true);
                    }
                });
    }
}
