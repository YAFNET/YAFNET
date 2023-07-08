/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AvatarTests : TestBase
{
    /// <summary>
    /// Select the avatar from collection test.
    /// </summary>
    [Test]
    public async Task SelectAvatarFromCollectionTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.IsTrue(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword),
                        "Login failed");

                    // Do actual test

                    // Go to Modify Avatar Page
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditAvatar");

                    var pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Modify Avatar"), "Modify Avatar is not available for that User");

                    // Select an Avatar from the Avatar Collection
                    Assert.IsTrue(
                        pageSource.Contains("Select your Avatar from our Collection"),
                        "Avatar Collection not available");

                    await page.Locator(".select2-selection").ClickAsync();

                    await page.GetByRole(AriaRole.Option, new() { Name = "SampleAvatar.gif SampleAvatar.gif" })
                        .ClickAsync();

                    await page.GetByRole(AriaRole.Button, new() { Name = " Update" }).ClickAsync();

                    // Check new avatar
                    var image = page.Locator(".img-thumbnail");

                    var src = await image.GetAttributeAsync("src");

                    Assert.IsNotNull(src);

                    Assert.IsTrue(src.Contains("SampleAvatar.gif"), "Modify Avatar Failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Upload the avatar from computer test.
    /// </summary>
    [Test]
    public async Task UploadAvatarFromComputerTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.IsTrue(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword),
                        "Login failed");

                    // Do actual test

                    // Go to Modify Avatar Page
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditAvatar");

                    var pageSource = await page.ContentAsync();

                    Assert.IsTrue(pageSource.Contains("Modify Avatar"), "Modify Avatar is not available for that User");

                    Assert.IsTrue(pageSource.Contains("Upload Avatar from Your Computer"), "Upload Avatars disabled");

                    var filePath = Path.GetFullPath(@"..\..\..\..\testfiles\avatar.png");

                    // Enter Test Avatar
                    await page.Locator("//input[contains(@id,'Upload')]").SetInputFilesAsync(filePath);

                    await page.Locator("//button[contains(@formaction,'UploadUpdate')]").ClickAsync();

                    // Check new avatar
                    var image = page.Locator(".img-thumbnail");

                    var src = await image.GetAttributeAsync("src");

                    Assert.IsNotNull(src);

                    Assert.IsTrue(src.Contains(".png"), "Modify Avatar Failed");
                },
            this.BrowserType);
    }
}