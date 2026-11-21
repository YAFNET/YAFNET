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

using System.Linq;
using System.Text.RegularExpressions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.AdminTests.Users;

/// <summary>
/// The Manage User Tests
/// </summary>
public class ManageUserTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Delete the random test user test.
    /// </summary>
    /// <summary>
    /// The admin can set and remove the signature of a user (a valid signature was not saved,
    /// an empty one or one with text failed with an error page).
    /// </summary>
    [Test]
    public async Task AdminEditsUserSignatureTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                await page.OpenUserFilterAsync();

                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

                var editLink = await page.Locator("//a[contains(@href,'EditUser')]", new PageLocatorOptions { HasText = userName }).First
                                   .GetAttributeAsync("href");
                var userId = editLink![(editLink.LastIndexOf('/') + 1)..];

                // the tab of the user editor is a page of its own (the editor loads the user first)
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");

                var signatureUrl = $"{this.TestSettings.TestForumUrl}Admin/EditUser/UsersSignature?userId={userId}";
                var signature = $"Signature of the admin {Random.Shared.Next()}";

                await page.GotoAsync(signatureUrl);
                await page.EvaluateAsync("text => { const t = document.querySelector('#Signature'); const e = window.sceditor && sceditor.instance(t); if (e) { e.val(text); e.updateOriginal(); } else { t.value = text; } }", signature);

                var response = await page.RunAndWaitForNextPageResponseAsync(
                                   () => page.Locator("button[formaction*='Save']").First.ClickAsync());

                Assert.That(response?.Status, Is.LessThan(500), "Saving the signature failed");

                await page.GotoAsync(signatureUrl);

                Assert.That(await page.EvaluateAsync<string>("() => { const t = document.querySelector('#Signature'); const e = window.sceditor && sceditor.instance(t); return e ? e.val() : t.value; }"), Is.EqualTo(signature), "The signature was not saved");

                // removed with an empty field
                await page.EvaluateAsync("text => { const t = document.querySelector('#Signature'); const e = window.sceditor && sceditor.instance(t); if (e) { e.val(text); e.updateOriginal(); } else { t.value = text; } }", string.Empty);

                response = await page.RunAndWaitForNextPageResponseAsync(
                               () => page.Locator("button[formaction*='Save']").First.ClickAsync());

                Assert.That(response?.Status, Is.LessThan(500), "Removing the signature failed");

                await page.GotoAsync(signatureUrl);

                Assert.That(await page.EvaluateAsync<string>("() => { const t = document.querySelector('#Signature'); const e = window.sceditor && sceditor.instance(t); return e ? e.val() : t.value; }"), Is.Empty, "The signature was not removed");
            });
    }

    [Test]
    [Description("Deletes the a Random Test User Account")]
    public async Task DeleteRandomTestUserTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // a new user, only this one is deleted (not one of the standard test users)
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // Do actual test

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.OpenUserFilterAsync();

                // Search for the new user
                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

                var editUserButton = page.Locator(
                    "//a[contains(@href,'EditUser')]",
                    new PageLocatorOptions { HasText = userName });

                Assert.That(await editUserButton.CountAsync(), Is.GreaterThan(0), $"{userName} not found");

                var href = await editUserButton.First.GetAttributeAsync("href");

                Assert.That(href, Is.Not.Null);

                var userId = href[(href.LastIndexOf('/') + 1)..];

                await page.Locator($".user-row[data-user-id='{userId}'] .user-actions [data-bs-target='#DeleteUserDialog']").ClickAsync();

                await page.SubmitAndWaitAsync(page.Locator("#DeleteUserDialog button[formaction*='Delete']"));

                // the search field still contains the name, the list must not
                Assert.That(await editUserButton.CountAsync(), Is.Zero, $"{userName} was not deleted");
            });
    }

    /// <summary>
    /// The Add User to Role Test.
    /// </summary>
    [Test]
    [Description("Assign the TestUser Account with the Moderator Role")]
    public async Task AddUserToTestRoleTest()
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
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                const string roleName = "TestRole";

                var pageSource = await page.CreateNewRole(this.TestSettings, roleName);

                Assert.That(pageSource, Does.Contain(roleName), "Test Role creating failed");

                // Do actual test

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.OpenUserFilterAsync();

                // Search for TestUser
                var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(this.TestSettings.TestUserName);
                await searchNameInput.PressAsync("Enter");

                var userProfileLink = page.Locator("//a[contains(@href,'EditUser')]").First;

                Assert.That(userProfileLink, Is.Not.Null, "Test User doesn't Exist");

                await userProfileLink.ClickAsync();

                // Go To Tab User Roles
                await page.Locator("//button[contains(@data-bs-target,'#View2')]").ClickAsync();

                // Add TestUser to Test Role
                await page.GetByLabel("TestRole").First.CheckAsync();

                // Save changes
                await page.Locator("//button[contains(@formaction,'UsersGroups?handler=Save')]").ClickAsync();

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.OpenUserFilterAsync();

                // Search for TestUser
                searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                await searchNameInput.FillAsync(this.TestSettings.TestUserName);
                await searchNameInput.PressAsync("Enter");

                userProfileLink = page.Locator("//a[contains(@href,'EditUser')]").First;

                Assert.That(userProfileLink, Is.Not.Null, "Test User doesn't Exist");

                await userProfileLink.ClickAsync();

                // Go To Tab User Roles
                await page.Locator("//button[contains(@data-bs-target,'#View2')]").ClickAsync();

                Assert.That(await page.GetByLabel("TestRole").First.IsCheckedAsync(), Is.True);

                await page.GetByLabel("TestRole").First.UncheckAsync();

                // Save changes
                await page.Locator("//button[contains(@formaction,'UsersGroups?handler=Save')]").ClickAsync();
            });
    }

    /// <summary>
    /// The users filter and "Lock inactive accounts" with values that are not in the range (manipulated form)
    /// don't fail.
    /// </summary>
    [Test]
    public async Task UsersFilterWithInvalidValuesTest()
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

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.EvaluateAsync(
                    """
                    () => {
                        const select = document.querySelector("select[name='Input.Since']");
                        select.add(new Option('x', 'x', true, true));
                        select.value = 'x';
                    }
                    """);

                // waits for the new page (the old page is loaded already when the response arrives)
                var response = await page.RunAndWaitForNextPageResponseAsync(
                                   () => page.EvaluateAsync("() => { const b = document.querySelector(\"button[formaction*='/Search']\"); b.form.requestSubmit(b); }"));

                Assert.That(response?.Status, Is.EqualTo(200), "The filter failed");

                await page.WaitForLoadStateAsync(LoadState.Load);

                Assert.That(
                    await page.Locator("//a[contains(@href,'EditUser')]").CountAsync(),
                    Is.GreaterThan(0),
                    "All users are listed");

                await page.Locator("input[name='Input.YearsOld']").EvaluateAsync("input => input.value = '1000000'");

                // waits for the new page, closing the page while it is loaded never ends
                response = await page.RunAndWaitForNextPageResponseAsync(
                               () => page.EvaluateAsync("() => { const b = document.querySelector(\"button[formaction*='/LockAccounts']\"); b.form.requestSubmit(b); }"));

                Assert.That(response?.Status, Is.EqualTo(200), "Lock inactive accounts failed");
            });
    }

    /// <summary>
    /// When the verification mail can not be sent the user is created anyway: the admin gets a message
    /// instead of an error page.
    /// </summary>
    [Test]
    public async Task AdminRegisterUserWithFailedMailTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var userName = $"TempUser{Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/RegisterUser");

                await page.Locator("#Input_UserName").FillAsync(userName);
                await page.Locator("#Input_Email").FillAsync($"{userName.ToLower()}@test.com");
                await page.Locator("#Input_Password").FillAsync(this.TestSettings.TestUserPassword);
                await page.Locator("#Input_Password2").FillAsync(this.TestSettings.TestUserPassword);

                this.Fixture.SmtpServer.Stop();

                try
                {
                    var response = await page.RunAndWaitForResponseAsync(
                                       () => page.Locator("button[formaction*='ForumRegister']").ClickAsync(),
                                       r => r.Request.Method == "POST" && r.Url.Contains("/Admin/RegisterUser"));

                    Assert.That(response.Status, Is.LessThan(400), $"Registering the user failed with an error page ({response.Status})");
                }
                finally
                {
                    this.Fixture.SmtpServer = TestSmtpServer.Start(this.TestSettings.TestMailPort);
                }

                await page.GetByText($"User {userName} Created Successfully.").WaitForAsync();
            });
    }

    /// <summary>
    /// "Import" of Admin > Users opens the import of users (it opened the import of the spam words).
    /// </summary>
    [Test]
    public async Task ImportButtonOpensTheUsersImportTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                await page.Locator("button[data-bs-toggle='ajax-modal'][data-url*='Import']").ClickAsync();

                var form = page.Locator(".modal.show form#formImport");

                await Assertions.Expect(form).ToBeVisibleAsync();

                Assert.That(await form.GetAttributeAsync("action"), Does.Contain("UsersImport"), "The import of the users is not opened");
            });
    }

    /// <summary>
    /// "Edit" in the context menu (right click) of a user opens the user (it sent the filter form to the edit page).
    /// </summary>
    [Test]
    public async Task ContextMenuEditOpensTheUserTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                // the list is paged (the users of the other tests come first)
                await page.OpenUserFilterAsync();
                await page.Locator("#Input_Name").FillAsync(this.TestSettings.AdminUserName);
                await page.RunAndWaitForNextPageAsync(() => page.Locator("#Input_Name").PressAsync("Enter"));

                var row = page.Locator(".list-group-item-menu", new PageLocatorOptions { HasText = this.TestSettings.AdminUserName }).First;

                await row.Locator(".user-meta").ClickAsync(new LocatorClickOptions { Button = MouseButton.Right, Position = new Position { X = 1, Y = 1 } });

                var edit = row.Locator(".context-menu .dropdown-item", new LocatorLocatorOptions { HasText = "Edit" }).First;

                await Assertions.Expect(edit).ToBeVisibleAsync();

                var response = await page.RunAndWaitForNextPageResponseAsync(() => edit.ClickAsync());

                Assert.That(response?.Status, Is.LessThan(400), $"The user editor failed ({response?.Status})");
                Assert.That(page.Url, Does.Contain("Admin/EditUser/"), "The user editor is not opened");
                await Assertions.Expect(page.Locator("#EditUserTabs")).ToBeVisibleAsync();
            });
    }

    /// <summary>
    /// The "since" filter of Admin > Users is localized and the first option shows the last visit
    /// (the texts were English text in the code, the last visit was shown without the date).
    /// </summary>
    [Test]
    public async Task SinceFilterIsLocalizedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                var options = await page.Locator("select[name='Input.Since'] option").AllTextContentsAsync();

                Assert.That(options[0], Does.Match(@"^last visit at .*\d"), "The last visit is not shown");
                Assert.That(
                    options.Skip(1).Select(o => o.Trim()),
                    Is.EqualTo(new[] { "last hour", "last two hours", "last day", "last two days", "last week", "last two weeks", "last month", "Show All" }),
                    "The options are not the texts of the language file");
            });
    }
    /// <summary>
    /// Enter in the filter of Admin > Users searches the users (the first button of the form was
    /// "lock inactive accounts": every Enter locked the accounts without a visit for 5 years).
    /// </summary>
    [Test]
    public async Task EnterInTheUserFilterSearchesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

                var name = page.Locator("#Input_Name");

                await page.OpenUserFilterAsync();

                await name.FillAsync(this.TestSettings.AdminUserName);

                var request = await page.RunAndWaitForRequestAsync(
                                  () => name.PressAsync("Enter"),
                                  r => r.Method == "POST" && r.Url.Contains("Admin/Users"));

                Assert.That(request.Url, Does.Not.Contain("LockAccounts"), "Enter locked the inactive accounts");
                Assert.That(request.Url, Does.Contain("Search"), "Enter did not search");
            });
    }
    /// <summary>
    /// A user is deleted with the delete dialog of the list: the dialog names the user, the list is shown
    /// again with a notification and without the user.
    /// </summary>
    [Test]
    public async Task DeleteUserWithTheDialogTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                await page.OpenUserFilterAsync();

                var searchNameInput = page.Locator("#Input_Name");
                await searchNameInput.FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

                var row = page.Locator(".user-row", new PageLocatorOptions { HasText = userName }).First;

                await row.Locator(".user-actions [data-bs-target='#DeleteUserDialog']").ClickAsync();

                var dialog = page.Locator("#DeleteUserDialog");

                await Assertions.Expect(dialog).ToBeVisibleAsync();
                await Assertions.Expect(dialog.Locator("[data-delete-user-name]")).ToHaveTextAsync(userName);

                await page.SubmitAndWaitAsync(dialog.Locator("button[formaction*='Delete']"));

                await Assertions.Expect(page.Locator(".toast").Last).ToContainTextAsync(userName);
                Assert.That(
                    await page.Locator(".user-row", new PageLocatorOptions { HasText = userName }).CountAsync(),
                    Is.Zero,
                    "The user was not deleted");
            });
    }
    /// <summary>
    /// Admin > Users lists the users that are not approved yet (the email is not confirmed) with the
    /// "not approved" badge (only the approved users were listed).
    /// </summary>
    [Test]
    public async Task NotApprovedUsersAreListedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a new user has to confirm the email first
                var userName = $"NotApproved{Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/RegisterUser");
                await page.Locator("#Input_UserName").FillAsync(userName);
                await page.Locator("#Input_Email").FillAsync($"{userName.ToLower()}@test.com");
                await page.Locator("#Input_Password").FillAsync(this.TestSettings.TestUserPassword);
                await page.Locator("#Input_Password2").FillAsync(this.TestSettings.TestUserPassword);
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='ForumRegister']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                await page.OpenUserFilterAsync();

                var searchNameInput = page.Locator("#Input_Name");
                await searchNameInput.FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

                var row = page.Locator(".user-row", new PageLocatorOptions { HasText = userName });

                Assert.That(await row.CountAsync(), Is.EqualTo(1), "The user that is not approved is not listed");
                await Assertions.Expect(row.Locator(".badge.text-bg-danger")).ToContainTextAsync("Not Approved");
            });
    }
    /// <summary>
    /// Enter in a field of a tab of the user editor sends the action of the field: the first button of the form
    /// was sent (points: set instead of add or remove, suspend: remove the suspension instead of suspend,
    /// profile: get the location instead of save).
    /// </summary>
    [Test]
    public async Task EnterInTheUserEditorTabsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a new user
                var userName = $"EnterTabs{Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/RegisterUser");
                await page.Locator("#Input_UserName").FillAsync(userName);
                await page.Locator("#Input_Email").FillAsync($"{userName.ToLower()}@test.com");
                await page.Locator("#Input_Password").FillAsync(this.TestSettings.TestUserPassword);
                await page.Locator("#Input_Password2").FillAsync(this.TestSettings.TestUserPassword);
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='ForumRegister']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                await page.OpenUserFilterAsync();
                await page.Locator("#Input_Name").FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => page.Locator("#Input_Name").PressAsync("Enter"));

                var userId = await page.Locator(".user-row", new PageLocatorOptions { HasText = userName }).GetAttributeAsync("data-user-id");

                // the tabs edit the user that the editor has loaded
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");

                async Task<string> EnterAsync(string tab, string field, string value = null)
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{tab}?userId={userId}");

                    var input = page.Locator(field);

                    if (value is not null)
                    {
                        await input.FillAsync(value);
                    }

                    var request = await page.RunAndWaitForRequestAsync(
                                      () => input.PressAsync("Enter"),
                                      r => r.Method == "POST");

                    await page.WaitForLoadStateAsync();

                    return Regex.Match(request.Url, @"handler=(\w+)").Groups[1].Value;
                }

                // all tabs, then one assert with the handlers that were sent
                var sent = new[]
                           {
                               "points add: " + await EnterAsync("UsersPoints", "#Input_AddPoints", "5"),
                               "points remove: " + await EnterAsync("UsersPoints", "#Input_RemovePoints", "2"),
                               "profile: " + await EnterAsync("UsersProfile", "#Input_Occupation", "Tester"),

                               // the second time the user is suspended (it removed the suspension)
                               "suspend: " + await EnterAsync("UsersSuspend", "#Input_SuspendCount", "2"),
                               "suspend again: " + await EnterAsync("UsersSuspend", "#Input_SuspendCount", "3")
                           };

                Assert.That(
                    sent,
                    Is.EqualTo(new[] { "points add: AddPoints", "points remove: RemovePoints", "profile: Save", "suspend: Suspend", "suspend again: Suspend" }),
                    "Enter sent another action");
            });
    }
    /// <summary>
    /// The admin changes the password of a user in the user editor: the editor is shown again with a message
    /// (the result was shown on the page of the tab, without the forum around it) and with the password tab
    /// (the editor showed the details after every save).
    /// </summary>
    [Test]
    public async Task AdminChangesPasswordOfUserTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var userName = $"ChangePass{Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/RegisterUser");
                await page.Locator("#Input_UserName").FillAsync(userName);
                await page.Locator("#Input_Email").FillAsync($"{userName.ToLower()}@test.com");
                await page.Locator("#Input_Password").FillAsync(this.TestSettings.TestUserPassword);
                await page.Locator("#Input_Password2").FillAsync(this.TestSettings.TestUserPassword);
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='ForumRegister']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                await page.OpenUserFilterAsync();
                await page.Locator("#Input_Name").FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => page.Locator("#Input_Name").PressAsync("Enter"));

                var userId = await page.Locator(".user-row", new PageLocatorOptions { HasText = userName }).GetAttributeAsync("data-user-id");

                async Task ChangeAsync(string password, string confirm)
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");
                    await page.Locator("button[data-bs-target='#View7']").ClickAsync();

                    var tab = page.Locator("#View7");

                    await tab.Locator("#Input_NewPassword").FillAsync(password);
                    await tab.Locator("#Input_NewPasswordConfirm").FillAsync(confirm);
                    await page.SubmitAndWaitAsync(tab.Locator("button[formaction*='ChangePassword']"));
                }

                // passwords that do not match: the message in the editor
                await ChangeAsync("New-Password-1", "Other-Password-2");

                Assert.That(page.Url, Does.Contain($"Admin/EditUser/{userId}"), "The editor is not shown after the error");
                await Assertions.Expect(page.Locator("#EditUserTabs")).ToBeVisibleAsync();
                await Assertions.Expect(page.Locator(".toast").Last).ToBeVisibleAsync();

                // the new password
                await ChangeAsync("New-Password-1", "New-Password-1");

                Assert.That(page.Url, Does.Contain($"Admin/EditUser/{userId}"), "The editor is not shown after the change");
                await Assertions.Expect(page.Locator("#EditUserTabs")).ToBeVisibleAsync();
                await Assertions.Expect(page.Locator(".toast").Last).ToBeVisibleAsync();

                // the editor shows the tab that was saved (it showed the details)
                await Assertions.Expect(page.Locator("button[data-bs-target='#View7']")).ToHaveClassAsync(new Regex(@"\bactive\b"));
            });
    }
    /// <summary>
    /// Saving the profile of a user without a birthday keeps it empty: the empty field showed the 1.1.1903 and
    /// saving it stored that date, the profile showed the birthday "January 1, 1903".
    /// </summary>
    [Test]
    public async Task ProfileWithoutBirthdayKeepsItEmptyTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var userName = $"Birthday{Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/RegisterUser");
                await page.Locator("#Input_UserName").FillAsync(userName);
                await page.Locator("#Input_Email").FillAsync($"{userName.ToLower()}@test.com");
                await page.Locator("#Input_Password").FillAsync(this.TestSettings.TestUserPassword);
                await page.Locator("#Input_Password2").FillAsync(this.TestSettings.TestUserPassword);
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='ForumRegister']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                await page.OpenUserFilterAsync();
                await page.Locator("#Input_Name").FillAsync(userName);
                await page.RunAndWaitForNextPageAsync(() => page.Locator("#Input_Name").PressAsync("Enter"));

                var userId = await page.Locator(".user-row", new PageLocatorOptions { HasText = userName }).GetAttributeAsync("data-user-id");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");

                // the profile is saved without changes
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/UsersProfile?userId={userId}");

                Assert.That(await page.Locator("#Input_Birthday").InputValueAsync(), Does.Not.Contain("1903"), "The empty birthday shows 1903");

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                // the profile of the user
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");

                var profileUrl = await page.Locator("a[href*='/user/']", new PageLocatorOptions { HasText = "View profile" }).GetAttributeAsync("href");

                await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), profileUrl).ToString());

                Assert.That(await page.ContentAsync(), Does.Not.Contain("1903"), "The profile shows the birthday 1903");
            });
    }
}
