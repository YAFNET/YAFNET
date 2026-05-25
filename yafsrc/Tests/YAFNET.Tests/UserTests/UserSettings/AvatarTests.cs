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
}