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

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.AdminTests.Settings;

/// <summary>
/// The Host Settings Tests
/// </summary>
public class HostSettingsTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Basic test to check if the Host Settings are correctly saved
    /// </summary>
    [Test]
    public async Task HostSettingsSavedCorrectlyTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.AdminUserName,
                            this.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings");

                    await page.Locator("#image-tab").ClickAsync();

                    // Modify a random setting
                    var input = page.Locator("//input[contains(@id,'_ImageAttachmentResizeWidth')]");
                    var width = await input.GetAttributeAsync("value");

                    Assert.That(width, Is.Not.Null);

                    await input.ClearAsync();
                    await input.FillAsync("350");

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Save')]").First);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings");

                    await page.Locator("#image-tab").ClickAsync();

                    input = page.Locator("//input[contains(@id,'_ImageAttachmentResizeWidth')]");

                    var value = await input.GetAttributeAsync("value");

                    Assert.That(value, Is.Not.Null);

                    Assert.That(value, Is.EqualTo("350"));

                    // Restore old Setting
                    await input.ClearAsync();
                    await input.FillAsync(width);

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Save')]").First);

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Who is Online"));
                });
    }

    /// <summary>
    /// The cached user data (unread private messages, pending friends, albums) only contains the counts of the
    /// enabled features: after a feature was enabled the counts stayed empty until the cache expired (the menu
    /// entry "My albums" was missing for up to 10 minutes).
    /// </summary>
    [Test]
    public async Task EnabledAlbumsAreShownAtOnceTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings");

                var albumsEnabled = await page.Locator("#Input_EnableAlbum").IsCheckedAsync();

                try
                {
                    await SetAlbumsAsync(page, this.TestSettings, true);

                    // an album of the admin
                    await page.GotoAsync(this.TestSettings.TestForumUrl);

                    await page.RunAndWaitForNextPageAsync(
                        () => page.EvaluateAsync(
                            """
                            () => {
                                const form = document.createElement('form');
                                form.method = 'post';
                                form.enctype = 'multipart/form-data';
                                form.action = '/EditAlbumImages/new/Upload';
                                form.appendChild(document.querySelector("input[name='__RequestVerificationToken']").cloneNode());

                                const title = document.createElement('input');
                                title.name = 'AlbumTitle';
                                title.value = 'Album of the admin';
                                form.appendChild(title);

                                const file = document.createElement('input');
                                file.type = 'file';
                                file.name = 'ImageFiles';
                                const png = Uint8Array.from(atob('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg=='), c => c.charCodeAt(0));
                                const files = new DataTransfer();
                                files.items.add(new File([png], 'album.png', { type: 'image/png' }));
                                file.files = files.files;
                                form.appendChild(file);

                                document.body.appendChild(form);
                                form.submit();
                            }
                            """));

                    // with the albums disabled the user data is loaded (and cached) without the albums
                    await SetAlbumsAsync(page, this.TestSettings, false);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings");
                    await page.RunAndWaitForNextPageAsync(
                        () => page.EvaluateAsync(
                            "() => document.querySelector(\"button[formaction*='UserLazyDataCacheReset']\").click()"));

                    await page.GotoAsync(this.TestSettings.TestForumUrl);

                    await SetAlbumsAsync(page, this.TestSettings, true);

                    await page.GotoAsync(this.TestSettings.TestForumUrl);

                    Assert.That(
                        await page.Locator("a.dropdown-item[href*='/Albums/']").CountAsync(),
                        Is.GreaterThan(0),
                        "The albums of the user are not shown after the albums were enabled");
                }
                finally
                {
                    await SetAlbumsAsync(page, this.TestSettings, albumsEnabled);
                }
            });
    }

    private static async Task SetAlbumsAsync(IPage page, TestConfig testSettings, bool enabled)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/HostSettings");

        // the check box and the save button are on a hidden tab
        await page.RunAndWaitForNextPageAsync(
            () => page.EvaluateAsync(
                """
                enabled => {
                    document.querySelector('#Input_EnableAlbum').checked = enabled;
                    document.querySelector("button[formaction*='Save']").click();
                }
                """,
                enabled));
    }
}
