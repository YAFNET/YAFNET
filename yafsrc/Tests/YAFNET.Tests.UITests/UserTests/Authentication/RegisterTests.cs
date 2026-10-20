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

using AwesomeAssertions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Authentication;

/// <summary>
/// The Register a test user Test.
/// </summary>
public class RegisterUser(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Register Random Test User Test
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task RegisterRandomNewUser()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var result = await page.RegisterRandomTestUserAsync(
                            this.TestSettings,
                            this.Fixture.SmtpServer);

                        result.Should().BeTrue("Registration failed");
                });
    }

    /// <summary>
    /// The user name is saved trimmed (like it is checked and like the login uses it), the password
    /// as typed (like the login uses it).
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task RegisterWithSpacesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = $"TempUser{Random.Shared.Next()}";
                    var email = $"{userName.ToLower()}@test.com";
                    var password = $" {this.TestSettings.TestUserPassword} ";

                    (await page.SubmitRegistrationAsync(this.TestSettings, $"{userName} ", email, password))
                        .Should().BeTrue("Registrations are disabled");

                    var confirmUrl = await this.Fixture.SmtpServer.WaitForEmailLinkAsync(email);

                    confirmUrl.Should().NotBeNull("the account was not created");

                    await page.GotoAsync(confirmUrl);

                    await page.GotoAsync(this.TestSettings.TestForumUrl);

                    if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                    {
                        await page.LogOutUserAsync();
                    }

                    (await page.LoginUserAsync(this.TestSettings, userName, password))
                        .Should().BeTrue("the login with the password as typed failed");

                    (await page.LogOutUserAsync()).Should().BeTrue("Logout failed");
                });
    }

    /// <summary>
    /// A required custom profile field that is shown on the register page must be filled in to register,
    /// the value is saved in the profile of the new user. A required field that is not shown on the register
    /// page doesn't block the registration (the register page showed the required fields instead of the fields
    /// with "show on register page").
    /// </summary>
    [Category("Authentication")]
    [Test]
    public async Task RegisterWithRequiredCustomFieldTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var fieldName = $"Register field {Random.Shared.Next(1000, 9999)}";
                    var hiddenFieldName = $"Profile field {Random.Shared.Next(1000, 9999)}";

                    async Task AddDefinitionAsync(string name, bool showOnRegisterPage)
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ProfileDefinitions");

                        await page.Locator(".card-footer button[data-bs-toggle='ajax-modal']").ClickAsync();

                        var dialog = page.Locator(".modal.show");

                        await dialog.Locator("#Name").FillAsync(name);
                        await dialog.Locator("#DataType").SelectOptionAsync("Text");
                        await dialog.Locator("#Length").FillAsync("50");
                        await dialog.Locator("#ShowOnRegisterPage").SetCheckedAsync(showOnRegisterPage);
                        await dialog.Locator("#Required").CheckAsync();

                        var response = await page.RunAndWaitForResponseAsync(
                                           () => dialog.Locator("button[data-bs-save='editModal']").ClickAsync(),
                                           r => r.Url.Contains("ProfileDefinition/Edit"));

                        Assert.That(response.Status, Is.EqualTo(200), "The definition was not saved");

                        await page.Locator("h5", new PageLocatorOptions { HasText = name }).WaitForAsync();
                    }

                    async Task<bool> LoginAdminAsync()
                    {
                        await page.GotoAsync(this.TestSettings.TestForumUrl);

                        if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                        {
                            await page.LogOutUserAsync();
                        }

                        return await page.LoginUserAsync(
                                   this.TestSettings,
                                   this.TestSettings.AdminUserName,
                                   this.TestSettings.AdminPassword);
                    }

                    Assert.That(await LoginAdminAsync(), Is.True, "Login failed");

                    try
                    {
                        await AddDefinitionAsync(fieldName, true);
                        await AddDefinitionAsync(hiddenFieldName, false);

                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                        var userName = $"TempUser{Random.Shared.Next()}";
                        var email = $"{userName.ToLower()}@test.com";

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Account/Register");

                        // Accept the Rules
                        if ((await page.ContentAsync()).Contains("Forum Rules"))
                        {
                            await page.ReloadAsync();
                        }

                        var field = page.GetByLabel(fieldName);

                        Assert.That(await field.IsVisibleAsync(), Is.True, "The field is not shown on the register page");
                        Assert.That(await field.GetAttributeAsync("required"), Is.Not.Null, "The field is not marked as required");
                        Assert.That(
                            await page.GetByLabel(hiddenFieldName).CountAsync(),
                            Is.Zero,
                            "The field without \"show on register page\" is shown");

                        await page.Locator("//input[contains(@id, '_UserName')]").FillAsync(userName);

                        if (await page.Locator("//input[contains(@id, '_DisplayName')]").IsVisibleAsync())
                        {
                            await page.Locator("//input[contains(@id, '_DisplayName')]").FillAsync(userName);
                        }

                        await page.Locator("//input[contains(@id, '_Password')]").FillAsync(this.TestSettings.TestUserPassword);
                        await page.Locator("//input[contains(@id, '_ConfirmPassword')]").FillAsync(this.TestSettings.TestUserPassword);
                        await page.Locator("//input[contains(@id, '_Email')]").FillAsync(email);

                        var submit = page.Locator("//button[contains(@type,'submit')]").Last;

                        // the browser doesn't send the form without the field
                        await submit.ClickAsync();

                        Assert.That(
                            await field.EvaluateAsync<bool>("input => input.matches(':invalid')"),
                            Is.True,
                            "The empty field is valid");
                        Assert.That(page.Url, Does.Contain("Account/Register"), "The form was sent without the field");

                        // a manipulated form without the required attribute is refused by the server
                        await field.EvaluateAsync("input => input.removeAttribute('required')");

                        await page.SubmitAndWaitAsync(submit);

                        // the text is also in the page as the hint of the field, the notification is checked
                        await page.Locator(".toast").First.WaitForAsync();

                        Assert.That(
                            await page.Locator(".toast").First.InnerTextAsync(),
                            Does.Contain($"{fieldName} is required"),
                            "The registration without the required field was accepted");

                        // the passwords are not sent back
                        await page.Locator("//input[contains(@id, '_Password')]").FillAsync(this.TestSettings.TestUserPassword);
                        await page.Locator("//input[contains(@id, '_ConfirmPassword')]").FillAsync(this.TestSettings.TestUserPassword);
                        await page.GetByLabel(fieldName).FillAsync("my value");

                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@type,'submit')]").Last);

                        var confirmUrl = await this.Fixture.SmtpServer.WaitForEmailLinkAsync(email);

                        Assert.That(confirmUrl, Is.Not.Null, "The account was not created");

                        await page.GotoAsync(confirmUrl);

                        await page.GotoAsync(this.TestSettings.TestForumUrl);

                        if (!await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
                        {
                            Assert.That(
                                await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                                Is.True,
                                "Login of the new user failed");
                        }

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                        Assert.That(
                            await page.GetByLabel(fieldName).InputValueAsync(),
                            Is.EqualTo("my value"),
                            "The value of the field was not saved");

                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
                    }
                    finally
                    {
                        // other registrations would need the field
                        await LoginAdminAsync();

                        foreach (var name in new[] { fieldName, hiddenFieldName })
                        {
                            await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ProfileDefinitions");

                            var item = page.Locator(".list-group-item", new PageLocatorOptions { HasText = name });

                            if (await item.CountAsync() == 0)
                            {
                                continue;
                            }

                            await item.Locator(".btn-group button[formaction*='Delete']").ClickAsync();

                            await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));
                        }

                        await page.LogOutUserAsync();
                    }
                });
    }
}
