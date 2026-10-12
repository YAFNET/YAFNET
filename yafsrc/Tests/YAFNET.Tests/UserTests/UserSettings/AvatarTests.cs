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

using System.IO;

using HtmlProperties;

using YAF.Tests.Infrastructure;

namespace YAF.Tests.UserTests.UserSettings;

/// <summary>
/// The user Avatar tests.
/// </summary>
public class AvatarTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Select the avatar from collection test.
    /// </summary>
    [Test]
    public async Task SelectAvatarFromCollectionTest()
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

                    // Go to Modify Avatar Page
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditAvatar");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Modify Avatar"), "Modify Avatar is not available for that User");

                    // Select an Avatar from the Avatar Collection
                    Assert.That(
                        pageSource, Does.Contain("Select your Avatar from our Collection"),
                        "Avatar Collection not available");

                    await page.Locator(".choices__list").First.ClickAsync();

                    await page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = "SampleAvatar.webp" })
                        .ClickAsync();

                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Update" }).ClickAsync();

                    // Check new avatar
                    var image = page.Locator(".img-thumbnail");

                    var src = await image.GetAttributeAsync(HtmlAttribute.Src);

                    Assert.That(src, Is.Not.Null);

                    Assert.That(src, Does.Contain("SampleAvatar.webp"), "Modify Avatar Failed");
                });
    }

    /// <summary>
    /// Upload the avatar from computer test.
    /// </summary>
    [Test]
    public async Task UploadAvatarFromComputerTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    if (this.TestSettings.NewInstall)
                    {
                        Assert.That(
                            await page.LoginUserAsync(
                                this.TestSettings,
                                this.TestSettings.AdminUserName,
                                this.TestSettings.AdminPassword), Is.True,
                            "Login failed");

                        // Enable avatar uploads in the Host Settings first.
                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Host" }).ClickAsync();
                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Host Settings" }).ClickAsync();
                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Avatars" }).ClickAsync();

                        await page.GetByRole(AriaRole.Checkbox,
                            new PageGetByRoleOptions { Name = "Allow User Avatar uploading:" }).CheckAsync();
                        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save Settings" })
                            .ClickAsync();
                    }

                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    // Go to Modify Avatar Page
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditAvatar");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Modify Avatar"), "Modify Avatar is not available for that User");

                    Assert.That(pageSource, Does.Contain("Upload Avatar from Your Computer"), "Upload Avatars disabled");

                    var filePath = Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png");

                    // Enter Test Avatar
                    await page.Locator("//input[contains(@id,'Upload')]").SetInputFilesAsync(filePath);

                    await page.Locator("//button[contains(@formaction,'UploadUpdate')]").ClickAsync();

                    // Check new avatar
                    var image = page.Locator(".img-thumbnail");

                    var src = await image.GetAttributeAsync(HtmlAttribute.Src);

                    Assert.That(src, Is.Not.Null);

                    Assert.That(src, Does.Contain(".webp"), "Modify Avatar Failed");
                });
    }

    /// <summary>
    /// An image file larger than the avatar size setting is not saved (the warning is shown on the avatar page).
    /// </summary>
    [Test]
    public async Task UploadedAvatarTooBigIsNotSavedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // the test image has 5480 bytes
                await this.SetAvatarSizeAsync(page, "1000");

                try
                {
                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                        Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditAvatar");

                    var filePath = Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png");

                    await page.Locator("//input[contains(@id,'Upload')]").SetInputFilesAsync(filePath);

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UploadUpdate')]"));

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource,
                        Does.Contain("The size of your image can't be more than 1000 bytes."),
                        "The warning is missing");

                    Assert.That(
                        pageSource,
                        Does.Contain("Select your Avatar from our Collection"),
                        "The avatar page is not shown completely");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditAvatar");

                    Assert.That(
                        await page.Locator(".img-thumbnail").First.GetAttributeAsync(HtmlAttribute.Src),
                        Does.Contain("GetTextAvatar"),
                        "The too big avatar was saved");
                }
                finally
                {
                    await this.SetAvatarSizeAsync(page, "50000");
                }
            });
    }

    /// <summary>
    /// The admin can select an avatar of the gallery and upload an avatar for a user (admin edit user).
    /// </summary>
    [Test]
    public async Task AdminChangesAvatarOfUserTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // allows uploads, logs in as the admin
                await this.SetAvatarSizeAsync(page, "50000");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

                var editUserUrl = await page.Locator("//a[contains(@href,'EditUser')]", new PageLocatorOptions { HasText = userName })
                                      .First.GetAttributeAsync("href");

                Assert.That(editUserUrl, Is.Not.Null, $"{userName} not found");

                editUserUrl = new Uri(new Uri(this.TestSettings.TestForumUrl), editUserUrl).ToString();

                // an avatar of the gallery
                await this.OpenAvatarTabAsync(page, editUserUrl);

                await page.Locator("#View4 select[name='AvatarGallery']").EvaluateAsync(
                    "select => select.value = [...select.options].find(o => o.text.includes('SampleAvatar')).value");

                await page.SubmitAndWaitAsync(page.Locator("#View4 button[formaction*='Gallery']"));

                await this.OpenAvatarTabAsync(page, editUserUrl);

                Assert.That(
                    await page.Locator("#View4 .img-thumbnail").GetAttributeAsync(HtmlAttribute.Src),
                    Does.Contain("SampleAvatar"),
                    "The avatar of the gallery was not saved");

                // an uploaded avatar
                var filePath = Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png");

                await page.Locator("#View4 input[type='file']").SetInputFilesAsync(filePath);

                await page.SubmitAndWaitAsync(page.Locator("#View4 button[formaction*='UploadUpdate']"));

                await this.OpenAvatarTabAsync(page, editUserUrl);

                var src = await page.Locator("#View4 .img-thumbnail").GetAttributeAsync(HtmlAttribute.Src);

                Assert.That(src, Does.Not.Contain("SampleAvatar").And.Not.Contain("GetTextAvatar"), "The uploaded avatar was not saved");
            });
    }

    /// <summary>
    /// Opens the avatar tab of the admin edit user page (it is loaded with ajax).
    /// </summary>
    private async Task OpenAvatarTabAsync(IPage page, string editUserUrl)
    {
        await page.GotoAsync(editUserUrl);

        await page.Locator("//button[contains(@data-bs-target,'#View4')]").ClickAsync();

        await page.Locator("#View4 .img-thumbnail").WaitForAsync();
    }

    /// <summary>
    /// Allows avatar uploads and sets the maximum file size of an avatar (logs in as the admin).
    /// </summary>
    private async Task SetAvatarSizeAsync(IPage page, string size)
    {
        await page.GotoAsync(this.TestSettings.TestForumUrl);

        if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
        {
            await page.LogOutUserAsync();
        }

        Assert.That(
            await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
            Is.True,
            "Login failed");

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings/Avatars");

        await page.Locator("#Input_AvatarUpload").CheckAsync(new LocatorCheckOptions { Force = true });
        await page.Locator("#Input_AvatarSize").FillAsync(size);

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']:visible").First);

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings/Avatars");

        Assert.That(
            await page.Locator("#Input_AvatarSize").InputValueAsync(),
            Is.EqualTo(size),
            "The avatar size was not saved");
    }
}
