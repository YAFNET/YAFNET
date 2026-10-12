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

namespace YAF.Tests.UITests.UserTests.UserSettings;

/// <summary>
/// The User Profile tests.
/// </summary>
public class ProfileTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Changes the forum language test.
    /// </summary>
    [Test]
    public async Task ChangeForumLanguageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Enable Allow User Change Language
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.AdminUserName,
                            this.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Enable avatar uploads in the Host Settings first.
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Host" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Host Settings" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Permission" }).ClickAsync();

                    if (!await page.GetByRole(AriaRole.Checkbox,
                            new PageGetByRoleOptions { Name = "Allow User Change Language:" }).IsCheckedAsync())
                    {
                        await page.GetByRole(AriaRole.Checkbox, new PageGetByRoleOptions { Name = "Allow User Change Language:" }).CheckAsync();
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
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Settings"), "Edit Settings is not available for that User");

                    Assert.That(
                        pageSource, Does.Contain("What language do you want to use"),
                        "Changing the Language is disabled for Users");

                    // Switch Language to German
                    await page.Locator("//select[@id='Language']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'de-DE')]").ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    await page.ReloadAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Dein Account"), "Changing Language failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    // Switch Language Back to English
                    await page.Locator("//select[@id='Language']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'en-US')]").First.ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    await page.ReloadAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Your Account"), "Changing Language failed");
                });
    }

    /// <summary>
    /// Changes the forum theme test.
    /// </summary>
    [Test]
    public async Task ChangeForumThemeTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Enable Allow User Change Language
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.AdminUserName,
                            this.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Enable avatar uploads in the Host Settings first.
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Host" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Host Settings" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Permission" }).ClickAsync();

                    if (!await page.GetByRole(AriaRole.Checkbox,
                            new PageGetByRoleOptions { Name = "Allow User Change Theme:" }).IsCheckedAsync())
                    {
                        await page.GetByRole(AriaRole.Checkbox,
                            new PageGetByRoleOptions { Name = "Allow User Change Theme:" }).CheckAsync();
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
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Settings"), "Edit Settings is not available for that User");

                    Assert.That(
                        pageSource, Does.Contain("Select your preferred theme"),
                        "Changing the Theme is disabled for Users");

                    // Switch Theme to "flatly"
                    await page.Locator("//select[@id='Theme']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'flatly')]").ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("themes/flatly/bootstrap-forum"), Is.True,
                        "Changing Forum Theme failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    // Switch Theme back to the yaf theme
                    await page.Locator("//select[@id='Theme']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'yaf')]").ClickAsync();

                    // Save the Changes
                    await page.Locator("//button[contains(@type,'submit')]").Last.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource.Contains("themes/yaf/bootstrap-forum"), Is.True,
                        "Changing Forum Theme failed");
                });
    }

    /// <summary>
    /// Change the user email address test. The new address needs the current password,
    /// and is only used after the confirmation link sent to it was opened.
    /// </summary>
    [Test]
    public async Task ChangeUserEmailAddressTest()
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
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Settings"), "Edit Settings is not available for that User");

                    Assert.That(
                        pageSource, Does.Contain("Change Email Address"),
                        "Changing the Email Address is disabled for Users");

                    var oldEmailAddress = await page.Locator("#Email").GetAttributeAsync("value");
                    const string newEmailAddress = "testmail123@localhost.com";

                    Assert.That(oldEmailAddress, Is.Not.Null);

                    // A wrong password changes nothing
                    pageSource = await this.SubmitEmailChangeAsync(page, newEmailAddress, "WrongPassword123!");

                    Assert.That(
                        pageSource, Does.Contain("The password you entered is not correct."),
                        "Wrong password was not refused");

                    Assert.That(
                        await this.GetCurrentEmailAsync(page), Is.EqualTo(oldEmailAddress),
                        "Email Address changed with a wrong password");

                    // Change the address, and confirm it
                    await this.ChangeAndConfirmEmailAsync(page, oldEmailAddress, newEmailAddress);

                    // Switch Email Address back
                    await this.ChangeAndConfirmEmailAsync(page, newEmailAddress, oldEmailAddress);
                });
    }

    /// <summary>
    /// Requests the email change with the right password, checks that the address only
    /// changes after the confirmation link is opened.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="currentEmail">The current email address.</param>
    /// <param name="newEmail">The new email address.</param>
    private async Task ChangeAndConfirmEmailAsync(IPage page, string currentEmail, string newEmail)
    {
        this.Fixture.SmtpServer.ClearReceivedEmail();

        var pageSource = await this.SubmitEmailChangeAsync(page, newEmail, this.TestSettings.TestUserPassword);

        Assert.That(
            pageSource, Does.Contain($"A confirmation link was sent to {newEmail}"),
            "Confirmation link was not sent");

        Assert.That(
            await this.GetCurrentEmailAsync(page), Is.EqualTo(currentEmail),
            "Email Address changed before it was confirmed");

        var confirmLink = await this.Fixture.SmtpServer.WaitForEmailLinkAsync(newEmail);

        Assert.That(confirmLink, Is.Not.Null, $"No confirmation mail was sent to {newEmail}");

        await page.GotoAsync(confirmLink);

        pageSource = await page.ContentAsync();

        Assert.That(
            pageSource, Does.Contain("Your new email address has been confirmed."),
            "Confirming the new Email Address failed");

        Assert.That(
            await this.GetCurrentEmailAsync(page), Is.EqualTo(newEmail),
            $"Email Address should match {newEmail}");
    }

    /// <summary>
    /// Enters the new email address and the password on the settings page and saves.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="newEmail">The new email address.</param>
    /// <param name="password">The current password.</param>
    /// <returns>The page source after saving.</returns>
    private async Task<string> SubmitEmailChangeAsync(IPage page, string newEmail, string password)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

        await page.Locator("#Email").FillAsync(newEmail);
        await page.Locator("#CurrentPassword").FillAsync(password);

        // Save the Settings
        await page.SubmitAndWaitAsync(page.Locator("form:has(#Email) button[type=submit]"));

        return await page.ContentAsync();
    }

    /// <summary>
    /// Gets the email address shown on the settings page.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <returns>The email address.</returns>
    private async Task<string> GetCurrentEmailAsync(IPage page)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

        return await page.Locator("#Email").GetAttributeAsync("value");
    }

    /// <summary>
    /// Set the user country and region test.
    /// </summary>
    [Test]
    public async Task SetUserCountryAndRegionTest()
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
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("Edit Profile"), "Edit Profile is not available for that User");

                    Assert.That(pageSource, Does.Contain("Country"), "Changing the Country is disabled for Users");

                    // Switch Country to "Germany"
                    await page.Locator("//select[@id='Input_Country']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'DE')]").Last.ClickAsync();

                    // Switch Region to "Berlin"
                    await page.Locator("//select[@id='Input_Region']/following::div[1]").ClickAsync();
                    await page.Locator("//div[contains(@data-value,'BER')]").Last.ClickAsync();

                    // Save the Profile Changes
                    await page.Locator("//button[contains(@formaction,'UpdateProfile')]").ClickAsync();

                    await page.GetByText("View Profile").First.ClickAsync();

                    pageSource = await page.ContentAsync();

                    Assert.That(pageSource.Contains("Germany") && pageSource.Contains("Berlin"), Is.True);
                });
    }

    /// <summary>
    /// Profile texts with quotes are shown as entered (not html encoded twice, "O&#39;Brien").
    /// </summary>
    [Test]
    public async Task ProfileTextWithQuotesTest()
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

                    const string city = "Martha's Vineyard";
                    const string interests = "Rock 'n' Roll & \"Jazz\"";

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                    await page.Locator("#Input_City").FillAsync(city);
                    await page.Locator("#Input_Interests").FillAsync(interests);

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateProfile')]"));

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                    Assert.That(await page.Locator("#Input_City").InputValueAsync(), Is.EqualTo(city), "The edit form shows the city encoded");

                    await page.SubmitAndWaitAsync(page.GetByText("View Profile").First);

                    var shown = await page.Locator("body").InnerTextAsync();

                    Assert.That(shown, Does.Contain(city), "The profile shows the city encoded");
                    Assert.That(shown, Does.Contain(interests), "The profile shows the interests encoded");
                });
    }

    /// <summary>
    /// A display name has the same rules as at the registration (not the guest name),
    /// an empty display name is too short (and no error page).
    /// </summary>
    [Test]
    public async Task DisplayNameRulesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    await page.SetDisplayNamesAsync(this.TestSettings, true);

                    try
                    {
                        Assert.That(
                            await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                            Is.True,
                            "Login failed");

                        // the guest name is not allowed
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");
                        await page.Locator("#Input_DisplayName").FillAsync("Guest");
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateProfile')]"));

                        Assert.That(
                            await page.ContentAsync(),
                            Does.Contain("Username cannot contain semi-colons"),
                            "The guest name was accepted");

                        // an empty name is too short
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");
                        await page.Locator("#Input_DisplayName").FillAsync(string.Empty);
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateProfile')]"));

                        Assert.That(
                            await page.ContentAsync(),
                            Does.Contain("must be more than"),
                            "The empty display name is not rejected as too short");

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                        Assert.That(
                            await page.Locator("#Input_DisplayName").InputValueAsync(),
                            Is.EqualTo(userName),
                            "The display name was changed");

                        // a normal name is saved
                        var displayName = $"Temp Display {userName[8..]}";

                        await page.Locator("#Input_DisplayName").FillAsync(displayName);
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateProfile')]"));

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                        Assert.That(
                            await page.Locator("#Input_DisplayName").InputValueAsync(),
                            Is.EqualTo(displayName),
                            "The display name was not saved");

                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
                    }
                    finally
                    {
                        // the other tests run without display names
                        await page.SetDisplayNamesAsync(this.TestSettings, false);
                    }
                });
    }

    /// <summary>
    /// The value of a custom number field must be a number (also when the form is manipulated),
    /// a field without a length limit can be filled.
    /// </summary>
    [Test]
    public async Task CustomNumberFieldTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var fieldName = $"Shoe Size {Random.Shared.Next(1000, 9999)}";

                    await this.LoginAdminAsync(page);

                    // a number field without a length limit
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ProfileDefinitions");

                    await page.Locator(".card-footer button[data-bs-toggle='ajax-modal']").ClickAsync();

                    var dialog = page.Locator(".modal.show");

                    await dialog.Locator("#Name").FillAsync(fieldName);
                    await dialog.Locator("#DataType").SelectOptionAsync("Number");
                    await dialog.Locator("#Length").FillAsync("0");

                    Assert.That(
                        (await dialog.Locator("button[data-bs-save='editModal']").InnerTextAsync()).Trim(),
                        Is.EqualTo("Add New Definition"),
                        "The add button has the wrong text");

                    var response = await page.RunAndWaitForResponseAsync(
                                       () => dialog.Locator("button[data-bs-save='editModal']").ClickAsync(),
                                       r => r.Url.Contains("ProfileDefinition/Edit"));

                    Assert.That(response.Status, Is.EqualTo(200), "The definition was not saved");

                    await page.Locator("h5", new PageLocatorOptions { HasText = fieldName }).WaitForAsync();

                    try
                    {
                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                        Assert.That(
                            await page.LoginUserAsync(
                                this.TestSettings,
                                this.TestSettings.TestUserName,
                                this.TestSettings.TestUserPassword),
                            Is.True,
                            "Login failed");

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                        var field = page.GetByLabel(fieldName);

                        Assert.That(await field.GetAttributeAsync("maxlength"), Is.Null, "The field has a length limit");

                        // the browser only allows numbers, a manipulated form sends text
                        await field.EvaluateAsync("input => input.type = 'text'");
                        await field.FillAsync("big");

                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateProfile')]"));

                        Assert.That(
                            await page.ContentAsync(),
                            Does.Contain($"{fieldName} must be a number"),
                            "The text was accepted as a number");

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                        await page.GetByLabel(fieldName).FillAsync("42");

                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateProfile')]"));

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                        Assert.That(
                            await page.GetByLabel(fieldName).InputValueAsync(),
                            Is.EqualTo("42"),
                            "The number was not saved");

                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
                    }
                    finally
                    {
                        await page.GotoAsync(this.TestSettings.TestForumUrl);

                        if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                        {
                            await page.LogOutUserAsync();
                        }

                        await this.LoginAdminAsync(page);

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ProfileDefinitions");

                        await page.Locator(".list-group-item", new PageLocatorOptions { HasText = fieldName })
                            .Locator(".btn-group button[formaction*='Delete']").ClickAsync();

                        await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                        await page.LogOutUserAsync();
                    }
                });
    }

    /// <summary>
    /// The name of a custom field (set by the admin of the board) is a text: the message of an invalid value
    /// showed it as html (the notification and the hint of required fields are html).
    /// </summary>
    [Test]
    public async Task CustomFieldNameIsTextTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var label = $"Size {Random.Shared.Next(1000, 9999)}";
                    var fieldName = $"{label} <img src=x onerror=window.__xss=1>";

                    await this.LoginAdminAsync(page);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ProfileDefinitions");

                    await page.Locator(".card-footer button[data-bs-toggle='ajax-modal']").ClickAsync();

                    var dialog = page.Locator(".modal.show");

                    await dialog.Locator("#Name").FillAsync(fieldName);
                    await dialog.Locator("#DataType").SelectOptionAsync("Number");
                    await dialog.Locator("#Length").FillAsync("0");

                    await page.RunAndWaitForResponseAsync(
                        () => dialog.Locator("button[data-bs-save='editModal']").ClickAsync(),
                        r => r.Url.Contains("ProfileDefinition/Edit"));

                    await page.Locator("h5", new PageLocatorOptions { HasText = label }).WaitForAsync();

                    try
                    {
                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                        Assert.That(
                            await page.LoginUserAsync(
                                this.TestSettings,
                                this.TestSettings.TestUserName,
                                this.TestSettings.TestUserPassword),
                            Is.True,
                            "Login failed");

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                        var field = page.GetByLabel(label);

                        // the browser only allows numbers, a manipulated form sends text
                        await field.EvaluateAsync("input => input.type = 'text'");
                        await field.FillAsync("big");

                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UpdateProfile')]"));

                        await page.Locator(".toast").First.WaitForAsync();
                        await page.WaitForTimeoutAsync(500);

                        Assert.That(await page.EvaluateAsync<bool>("() => window.__xss === 1"), Is.False, "The name of the field runs as script");
                        Assert.That(await page.Locator(".toast").First.InnerTextAsync(), Does.Contain(fieldName), "The name is not shown as text");

                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
                    }
                    finally
                    {
                        await page.GotoAsync(this.TestSettings.TestForumUrl);

                        if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                        {
                            await page.LogOutUserAsync();
                        }

                        await this.LoginAdminAsync(page);

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ProfileDefinitions");

                        await page.Locator(".list-group-item", new PageLocatorOptions { HasText = label })
                            .Locator(".btn-group button[formaction*='Delete']").ClickAsync();

                        await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                        await page.LogOutUserAsync();
                    }
                });
    }

    /// <summary>
    /// The name and the default value of a custom field are limited to their columns
    /// (MySQL creates them with 255 characters, a longer value was not saved).
    /// </summary>
    [Test]
    public async Task LongCustomFieldNameIsLimitedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var fieldName = $"Long field {Random.Shared.Next(1000, 9999)} ".PadRight(300, 'x');

                    await this.LoginAdminAsync(page);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ProfileDefinitions");

                    await page.Locator(".card-footer button[data-bs-toggle='ajax-modal']").ClickAsync();

                    var dialog = page.Locator(".modal.show");

                    await dialog.Locator("#Name").FillAsync(fieldName);
                    await dialog.Locator("#Length").FillAsync("0");
                    await dialog.Locator("#DefaultValue").FillAsync(new string('d', 300));

                    var response = await page.RunAndWaitForResponseAsync(
                                       () => dialog.Locator("button[data-bs-save='editModal']").ClickAsync(),
                                       r => r.Url.Contains("ProfileDefinition/Edit"));

                    Assert.That(response.Status, Is.EqualTo(200), "The definition was not saved");

                    var item = page.Locator(".list-group-item", new PageLocatorOptions { HasText = fieldName[..255] });

                    await item.Locator("h5").WaitForAsync();

                    await item.Locator(".btn-group button[formaction*='Delete']").ClickAsync();

                    await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                    await page.LogOutUserAsync();
                });
    }

    /// <summary>
    /// The settings only keep a language of the list and a page size of 1 - 100 (a manipulated form
    /// could break all pages of the user).
    /// </summary>
    [Test]
    public async Task SettingsWithInvalidValuesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    await page.EvaluateAsync(
                        """
                        () => {
                            const form = document.querySelector("select[name='Size']").form;
                            const set = (name, value) => {
                                let field = form.querySelector(`[name='${name}']`);
                                if (field && field.tagName === 'SELECT') {
                                    field.add(new Option(value, value, true, true));
                                    field.value = value;
                                } else {
                                    field = document.createElement('input');
                                    field.type = 'hidden';
                                    field.name = name;
                                    field.value = value;
                                    form.appendChild(field);
                                }
                            };
                            set('Language', 'x');
                            set('Size', '1000000');
                        }
                        """);

                    await page.SubmitAndWaitAsync(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).Last);

                    var response = await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    Assert.That(response?.Status, Is.EqualTo(200), "The pages of the user fail");

                    Assert.That(await page.Locator("select[name='Size']").InputValueAsync(), Is.EqualTo("5"), "The page size was saved");

                    response = await page.GotoAsync(this.TestSettings.TestForumUrl);

                    Assert.That(response?.Status, Is.EqualTo(200), "The pages of the user fail");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
                });
    }

    /// <summary>
    /// A user can't hide from the active users list with a manipulated settings form when
    /// "allow users to hide themselves" is disabled (the default).
    /// </summary>
    [Test]
    public async Task HideMeNeedsPermissionTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    Assert.That(
                        await page.Locator("input[name='HideMe'][type='checkbox']").CountAsync(),
                        Is.Zero,
                        "The hide me option is shown (the test needs it disabled)");

                    await page.EvaluateAsync(
                        """
                        () => {
                            const form = document.querySelector("select[name='Size']").form;
                            const field = document.createElement('input');
                            field.type = 'hidden';
                            field.name = 'HideMe';
                            field.value = 'true';
                            form.appendChild(field);
                        }
                        """);

                    await page.SubmitAndWaitAsync(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).Last);

                    // the own row of the active users list shows if the user is hidden
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}ActiveUsers/0");

                    Assert.That(
                        await page.Locator(".active-user-row .fa-user-secret").CountAsync(),
                        Is.Zero,
                        "The user is hidden from the active users list");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
                });
    }

    private async Task LoginAdminAsync(IPage page)
    {
        Assert.That(
            await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
            Is.True,
            "Login failed");
    }

    /// <summary>
    /// The birthday is the same day for every viewer (it was converted like a point in time: users west of the
    /// time zone of the user saw the day before).
    /// </summary>
    [Test]
    public async Task BirthdayIsShownAsDateTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    // a time zone east of the viewer (Tokyo, UTC+9)
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditSettings");

                    var timeZone = await page.Locator("select[name='TimeZone']").EvaluateAsync<string>(
                                       "select => { const o = [...select.options].find(o => o.text.includes('Tokyo')); select.value = o.value; return o.value; }");

                    Assert.That(timeZone, Is.Not.Empty, "The time zone was not found");

                    await page.SubmitAndWaitAsync(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Save" }).Last);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                    await page.Locator("#Input_Birthday").EvaluateAsync("input => input.value = '1990-05-10'");

                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='UpdateProfile']"));

                    var profileLink = await page.Locator("a", new PageLocatorOptions { HasText = "View Profile" }).First.GetAttributeAsync("href");

                    Assert.That(profileLink, Is.Not.Null, "The profile link is missing");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    // a viewer in another time zone
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.TestUserName, this.TestSettings.TestUserPassword),
                        Is.True,
                        "Login failed");

                    await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), profileLink).ToString());

                    var birthday = page.Locator(".profile-pill", new PageLocatorOptions { Has = page.Locator(".fa-cake-candles") });

                    var text = await birthday.InnerTextAsync();

                    Assert.That(text, Does.Contain("10").And.Contain("1990"), $"The birthday is not the 10th ({text})");
                });
    }
}
