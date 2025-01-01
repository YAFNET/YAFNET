/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Tests.AdminTests.Settings;

/// <summary>
/// The  Host Settings Tests
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HostSettingsTests : TestBase
{
    /// <summary>
    /// Basic test to check if the Host Settings are correctly saved
    /// </summary>
    [Test]
    public async Task HostSettingsSavedCorrectlyTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.AdminUserName,
                            this.Base.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Admin/HostSettings");

                    await page.Locator("#image-tab").ClickAsync();

                    // Modify a random setting
                    var input = page.Locator("//input[contains(@id,'_ImageAttachmentResizeWidth')]");
                    var width = await input.GetAttributeAsync("value");

                    Assert.That(width, Is.Not.Null);

                    await input.ClearAsync();
                    await input.FillAsync("350");

                    await page.Locator("//button[contains(@formaction,'Save')]").First.ClickAsync();

                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Admin/HostSettings");

                    await page.Locator("#image-tab").ClickAsync();

                    input = page.Locator("//input[contains(@id,'_ImageAttachmentResizeWidth')]");

                    var value = await input.GetAttributeAsync("value");

                    Assert.That(value, Is.Not.Null);

                    Assert.That(value, Is.EqualTo("350"));

                    // Restore old Setting
                    await input.ClearAsync();
                    await input.FillAsync(width);

                    await page.Locator("//button[contains(@formaction,'Save')]").First.ClickAsync();

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Who is Online"));
                },
            this.BrowserType);
    }
}