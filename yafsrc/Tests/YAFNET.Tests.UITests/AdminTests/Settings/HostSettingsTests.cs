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

    /// <summary>
    /// The block options of disabled features (not shown on the page) keep their value when the block options are
    /// saved: the friend requests were not blocked anymore after the user saved the page while the buddy list was
    /// turned off.
    /// </summary>
    [Test]
    public async Task HiddenBlockOptionsAreKeptTest()
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

                var buddyListEnabled = await page.Locator("#Input_EnableBuddyList").IsCheckedAsync();

                try
                {
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_EnableBuddyList", true);

                    // the friend requests are blocked
                    await SaveBlockOptionsAsync(page, this.TestSettings, ("#BlockFriendRequests", true), ("#BlockPMs", false));

                    // the buddy list is turned off, the option is not shown, the other options are saved
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_EnableBuddyList", false);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/BlockOptions");

                    Assert.That(await page.Locator("#BlockFriendRequests").CountAsync(), Is.Zero, "The option of the buddy list is shown");

                    await SaveBlockOptionsAsync(page, this.TestSettings, ("#BlockPMs", true));

                    // turned on again
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_EnableBuddyList", true);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/BlockOptions");

                    Assert.That(await page.Locator("#BlockPMs").IsCheckedAsync(), Is.True, "The shown option was not saved");
                    Assert.That(await page.Locator("#BlockFriendRequests").IsCheckedAsync(), Is.True, "The hidden option was reset");
                }
                finally
                {
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_EnableBuddyList", true);
                    await SaveBlockOptionsAsync(page, this.TestSettings, ("#BlockFriendRequests", false), ("#BlockPMs", false));

                    if (!buddyListEnabled)
                    {
                        await SetCheckBoxAsync(page, this.TestSettings, "#Input_EnableBuddyList", false);
                    }
                }
            });
    }

    /// <summary>
    /// The theme and the language of a user are kept when the settings are saved while the users can't choose them
    /// (the selects are not shown): they were reset to the board defaults.
    /// </summary>
    [Test]
    public async Task HiddenThemeAndLanguageAreKeptTest()
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

                var themeEnabled = await page.Locator("#Input_AllowUserTheme").IsCheckedAsync();
                var languageEnabled = await page.Locator("#Input_AllowUserLanguage").IsCheckedAsync();

                string oldTheme = null, oldLanguage = null;

                try
                {
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", true);
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserLanguage", true);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    oldTheme = await page.Locator("#Theme").InputValueAsync();
                    oldLanguage = await page.Locator("#Language").InputValueAsync();

                    // another theme and another english culture
                    var theme = await page.Locator("#Theme").EvaluateAsync<string>(
                                    "select => [...select.options].map(o => o.value).find(v => v && v !== select.value)");
                    var language = await page.Locator("#Language").EvaluateAsync<string>(
                                       "select => [...select.options].map(o => o.value).find(v => v.startsWith('en-') && v !== select.value)");

                    Assert.That(theme, Is.Not.Null, "There is no other theme");
                    Assert.That(language, Is.Not.Null, "There is no other language");

                    await SaveSettingsAsync(page, this.TestSettings, theme, language);

                    // the users can't choose them anymore, the other settings are saved
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", false);
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserLanguage", false);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    Assert.That(await page.Locator("#Theme").CountAsync(), Is.Zero, "The theme select is shown");

                    await SaveSettingsAsync(page, this.TestSettings, null, null);

                    // they can choose them again
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", true);
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserLanguage", true);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    Assert.That(await page.Locator("#Theme").InputValueAsync(), Is.EqualTo(theme), "The theme was reset");
                    Assert.That(await page.Locator("#Language").InputValueAsync(), Is.EqualTo(language), "The language was reset");
                }
                finally
                {
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", true);
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserLanguage", true);

                    if (oldTheme != null)
                    {
                        await SaveSettingsAsync(page, this.TestSettings, oldTheme, oldLanguage);
                    }

                    if (!themeEnabled)
                    {
                        await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", false);
                    }

                    if (!languageEnabled)
                    {
                        await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserLanguage", false);
                    }
                }
            });
    }

    /// <summary>
    /// The settings of a user can be saved by the admin while the users can't change their e-mail address or theme
    /// (the fields are not shown): the empty e-mail address was refused ("bad e-mail") and nothing was saved, and the
    /// theme of the user was reset.
    /// </summary>
    [Test]
    public async Task AdminSavesUserSettingsWithHiddenFieldsTest()
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

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                var profileLink = await page.Locator("a", new PageLocatorOptions { HasText = "View Profile" }).First.GetAttributeAsync("href");
                var userId = System.Text.RegularExpressions.Regex.Match(profileLink ?? string.Empty, @"/UserProfile/(\d+)/").Groups[1].Value;

                Assert.That(userId, Is.Not.Empty, "The id of the admin was not found");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings");

                var emailChangeEnabled = await page.Locator("#Input_AllowEmailChange").IsCheckedAsync();
                var themeEnabled = await page.Locator("#Input_AllowUserTheme").IsCheckedAsync();

                var settingsUrl = $"{this.TestSettings.TestForumUrl}Admin/EditUser/UsersSettings?userId={userId}";
                string oldTheme = null, oldSize = null;

                try
                {
                    // the admin has another theme
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", true);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    oldTheme = await page.Locator("#Theme").InputValueAsync();

                    var theme = await page.Locator("#Theme").EvaluateAsync<string>(
                                    "select => [...select.options].map(o => o.value).find(v => v && v !== select.value)");

                    await SaveSettingsAsync(page, this.TestSettings, theme, null);

                    // the e-mail address and the theme can't be changed, the admin changes the page size of the user
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowEmailChange", false);
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", false);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");
                    await page.GotoAsync(settingsUrl);

                    oldSize = await page.Locator("#Input_Size").InputValueAsync();

                    var size = await page.Locator("#Input_Size").EvaluateAsync<string>(
                                   """
                                   select => {
                                       const value = [...select.options].map(o => o.value).find(v => v && v !== select.value);
                                       select.value = value;
                                       return value;
                                   }
                                   """);

                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                    Assert.That(await page.ContentAsync(), Does.Not.Contain("illegal e-mail address"), "The e-mail address was refused");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");
                    await page.GotoAsync(settingsUrl);

                    Assert.That(await page.Locator("#Input_Size").InputValueAsync(), Is.EqualTo(size), "The settings were not saved");

                    // the theme is kept
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", true);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    Assert.That(await page.Locator("#Theme").InputValueAsync(), Is.EqualTo(theme), "The theme was reset");
                }
                finally
                {
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowEmailChange", true);
                    await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", true);

                    if (oldSize != null)
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");
                        await page.GotoAsync(settingsUrl);
                        await page.Locator("#Input_Size").EvaluateAsync("(select, value) => select.value = value", oldSize);
                        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
                    }

                    if (oldTheme != null)
                    {
                        await SaveSettingsAsync(page, this.TestSettings, oldTheme, null);
                    }

                    if (!emailChangeEnabled)
                    {
                        await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowEmailChange", false);
                    }

                    if (!themeEnabled)
                    {
                        await SetCheckBoxAsync(page, this.TestSettings, "#Input_AllowUserTheme", false);
                    }
                }
            });
    }

    /// <summary>
    /// Sets the theme and the language (when not null) of the logged-in user and saves the settings.
    /// </summary>
    private static async Task SaveSettingsAsync(IPage page, TestConfig testSettings, string theme, string language)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Profile/EditSettings");

        // the selects are replaced by script controls
        if (theme != null)
        {
            await page.Locator("#Theme").EvaluateAsync("(select, value) => select.value = value", theme);
        }

        if (language != null)
        {
            await page.Locator("#Language").EvaluateAsync("(select, value) => select.value = value", language);
        }

        await page.SubmitAndWaitAsync(page.Locator("button[formaction$='EditSettings']"));
    }

    /// <summary>
    /// Sets the block options of the logged-in user and saves them.
    /// </summary>
    private static async Task SaveBlockOptionsAsync(IPage page, TestConfig testSettings, params (string Selector, bool Value)[] options)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Profile/BlockOptions");

        foreach (var (selector, value) in options)
        {
            await page.Locator(selector).SetCheckedAsync(value);
        }

        await page.SubmitAndWaitAsync(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save", Exact = true }));
    }

    /// <summary>
    /// Sets a check box of the host settings and saves them.
    /// </summary>
    private static async Task SetCheckBoxAsync(IPage page, TestConfig testSettings, string selector, bool enabled)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/HostSettings");

        // the check box and the save button are on a hidden tab
        await page.RunAndWaitForNextPageAsync(
            () => page.EvaluateAsync(
                """
                ([selector, enabled]) => {
                    document.querySelector(selector).checked = enabled;
                    document.querySelector("button[formaction*='Save']").click();
                }
                """,
                new object[] { selector, enabled }));
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
