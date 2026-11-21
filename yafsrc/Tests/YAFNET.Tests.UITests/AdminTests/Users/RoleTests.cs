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

namespace YAF.Tests.UITests.AdminTests.Users;

/// <summary>
/// The Role (Group) Tests
/// </summary>
public class RoleTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Create a new Test Group (Role)
    /// </summary>
    [Test]
    [Description("Create a new Test Group (Role)")]
    public async Task CreateNewGroupTest()
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

                    var random = new Random();

                    var roleName = $"TestRole{random.Next()}";

                    var pageSource = await page.CreateNewRole(this.TestSettings, roleName);

                    Assert.That(pageSource, Does.Contain(roleName), "Test Role creating failed");
                });
    }

    /// <summary>
    /// A second group with the name of an existing group is refused with a warning: the database refused it (the
    /// name is unique in the board) and the page failed with an error.
    /// </summary>
    [Test]
    [Description("A group name can't be used twice")]
    public async Task GroupNameCanNotBeUsedTwiceTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(await page.LoginAdminUserAsync(this.TestSettings), Is.True, "Login failed");

                    var roleName = $"TwiceRole{Random.Shared.Next()}";

                    Assert.That(await page.CreateNewRole(this.TestSettings, roleName), Does.Contain(roleName), "The group was not created");

                    var pageSource = await page.CreateNewRole(this.TestSettings, roleName);

                    Assert.That(pageSource, Does.Contain("A group with such a name already exists."), "The warning is missing");
                    Assert.That(page.Url, Does.Contain("EditGroup"), "The page is not shown again");
                });
    }

    /// <summary>
    /// The guest group and the administrators group can't be deleted, also not with a manipulated form (the list
    /// has no delete button for them, without the administrators group the board has no admins).
    /// </summary>
    [TestCase("Guests")]
    [TestCase("Administrators")]
    [Description("The guest group and the administrators group can't be deleted")]
    public async Task ProtectedGroupCanNotBeDeletedTest(string groupName)
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.AdminUserName,
                            this.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Groups");

                    // the id of the group and if it has a delete button
                    const string findGroup = """
                                             name => {
                                                 for (const item of document.querySelectorAll('li.list-group-item')) {
                                                     const title = item.querySelector('.group-name');
                                                     const edit = item.querySelector('a[href*="EditGroup"]');
                                                     if (title && edit && title.textContent.replace(/\s+/g, ' ').trim() === name) {
                                                         const id = edit.getAttribute('href').match(/(\d+)\D*$/)[1];
                                                         return item.querySelector('button[formaction*="Delete"]') ? 'delete ' + id : id;
                                                     }
                                                 }
                                                 return null;
                                             }
                                             """;

                    var groupId = await page.EvaluateAsync<string>(findGroup, groupName);

                    Assert.That(groupId, Is.Not.Null, "The group is not listed");
                    Assert.That(groupId, Does.Not.StartWith("delete"), "The group has a delete button");

                    // the delete handler with the id of the group
                    var status = await page.EvaluateAsync<int>(
                        """
                        async url => {
                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                            const body = new URLSearchParams({ '__RequestVerificationToken': token });
                            const response = await fetch(url, { method: 'POST', body: body });
                            return response.status;
                        }
                        """,
                        $"{this.TestSettings.TestForumUrl}Admin/Groups?handler=Delete&id={groupId}");

                    Assert.That(status, Is.EqualTo(200), "The delete handler failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Groups");

                    Assert.That(
                        await page.EvaluateAsync<string>(findGroup, groupName),
                        Is.EqualTo(groupId),
                        "The group was deleted");
                });
    }

    /// <summary>
    /// The members of a deleted group lose the name style of the group: the style was only updated when a style
    /// was set, it was never removed.
    /// </summary>
    [Test]
    [Description("A deleted group's style is removed from its members")]
    public async Task DeletedGroupStyleIsRemovedFromMembersTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    Assert.That(await page.LoginAdminUserAsync(this.TestSettings), Is.True, "Login failed");

                    var roleName = $"StyledRole{Random.Shared.Next()}";

                    // a group with a style
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditGroup");
                    await page.Locator("#Input_Name").FillAsync(roleName);
                    await page.Locator("#Input_Description").FillAsync("Styled test role");
                    await page.Locator("#Input_Style").FillAsync("color: #123456");
                    await page.Locator("#Input_NewAccessMaskID").SelectOptionAsync(["3"]);
                    await page.SubmitAndWaitAsync(page.Locator("#Save"));

                    // the user is added to the group
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                    await page.OpenUserFilterAsync();
                    var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                    await searchNameInput.FillAsync(userName);
                    await searchNameInput.PressAsync("Enter");
                    await page.Locator("//a[contains(@href,'EditUser')]").First.ClickAsync();
                    await page.Locator("//button[contains(@data-bs-target,'#View2')]").ClickAsync();
                    await page.GetByLabel(roleName).First.CheckAsync();
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UsersGroups?handler=Save')]"));

                    Assert.That(await UserStyleAsync(page, this.TestSettings, userName), Does.Contain("#123456"), "The user has not the group style");

                    // the group is deleted
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Groups");

                    var groupId = await page.EvaluateAsync<string>(
                        """
                        name => {
                            for (const item of document.querySelectorAll('li.list-group-item')) {
                                const edit = item.querySelector('a[href*="EditGroup"]');
                                if (edit && item.textContent.includes(name)) {
                                    return edit.getAttribute('href').match(/(\d+)\D*$/)[1];
                                }
                            }
                            return null;
                        }
                        """,
                        roleName);

                    Assert.That(groupId, Is.Not.Null, "The group is not listed");

                    var status = await page.EvaluateAsync<int>(
                        """
                        async url => {
                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                            const body = new URLSearchParams({ '__RequestVerificationToken': token });
                            const response = await fetch(url, { method: 'POST', body: body });
                            return response.status;
                        }
                        """,
                        $"{this.TestSettings.TestForumUrl}Admin/Groups?handler=Delete&id={groupId}");

                    Assert.That(status, Is.EqualTo(200), "The delete handler failed");

                    Assert.That(await UserStyleAsync(page, this.TestSettings, userName), Is.Null.Or.Not.Contain("#123456"), "The user still has the style of the deleted group");
                });
    }

    /// <summary>
    /// Gets the style of the user link in the members list.
    /// </summary>
    private async static Task<string> UserStyleAsync(IPage page, TestConfig testSettings, string userName)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}members");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();
        await page.Locator("#Input_UserSearchName").FillAsync(userName);
        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Search')]"));

        return await page.Locator("a", new PageLocatorOptions { HasTextString = userName }).First.GetAttributeAsync("style");
    }

    /// <summary>
    /// The guest flag of the guest group was a switch of the editor: without it the board had no guest group
    /// (and the flag was saved from the form, also for other groups).
    /// </summary>
    [Test]
    public async Task GuestGroupKeepsTheGuestFlagTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    var groupId = await GetGroupIdAsync(page, this.TestSettings, "Guests");

                    try
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditGroup/{groupId}");

                        // the flag is sent as "false"
                        await page.EvaluateAsync(
                            "() => document.querySelectorAll('input[name=\"Input.IsGuestX\"]').forEach(i => { if (i.type === 'checkbox') { i.disabled = false; i.checked = false; } else { i.value = 'false'; } })");

                        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                        // a guest group has no access masks of the forums
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditGroup/{groupId}");

                        Assert.That(await page.Locator("select.accessMasks").CountAsync(), Is.Zero, "The guest group lost the guest flag");
                    }
                    finally
                    {
                        if (await page.Locator("select.accessMasks").CountAsync() > 0)
                        {
                            // the flag is set again (the first value of the form is used)
                            await page.EvaluateAsync(
                                """
                                () => {
                                    const input = document.createElement('input');
                                    input.type = 'hidden';
                                    input.name = 'Input.IsGuestX';
                                    input.value = 'true';
                                    document.querySelector('form.needs-validation').prepend(input);
                                }
                                """);

                            await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
                        }
                    }
                });
    }

    /// <summary>
    /// The admin flag of the only admin group could be removed: the board had no admins anymore (the group can't
    /// be deleted for the same reason).
    /// </summary>
    [Test]
    public async Task LastAdminGroupKeepsTheAdminFlagTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    var groupId = await GetGroupIdAsync(page, this.TestSettings, "Administrators");

                    try
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditGroup/{groupId}");

                        await page.Locator("#Input_IsAdminX").UncheckAsync();
                        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                        Assert.That(await page.ContentAsync(), Does.Contain("the board has no admins"), "The removed admin flag has no message");

                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditGroup/{groupId}");

                        Assert.That(await page.Locator("#Input_IsAdminX").IsCheckedAsync(), Is.True, "The only admin group lost the admin flag");
                    }
                    finally
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditGroup/{groupId}");

                        if (!await page.Locator("#Input_IsAdminX").IsCheckedAsync())
                        {
                            await page.Locator("#Input_IsAdminX").CheckAsync();
                            await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
                        }
                    }
                });
    }

    /// <summary>
    /// "Cancel" of the access mask editor was a submit button: the empty name of a new mask stopped it, else the
    /// access masks were shown without the masks (the list was only loaded by a get).
    /// </summary>
    [Test]
    public async Task CancelOfTheAccessMaskEditorShowsTheMasksTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditAccessMask");

                    await page.SubmitAndWaitAsync(
                        page.Locator("a, button", new PageLocatorOptions { HasTextString = "Cancel" }).Last);

                    Assert.That(page.Url, Does.Contain("Admin/AccessMasks"), "Cancel did not open the access masks");
                    Assert.That(await page.ContentAsync(), Does.Contain("Member Access"), "The access masks are not listed");
                });
    }

    /// <summary>
    /// The member count of a group opens the users filtered by the group.
    /// </summary>
    [Test]
    public async Task GroupMembersOpenTheFilteredUsersTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    var groupId = await GetGroupIdAsync(page, this.TestSettings, "Administrators");

                    await page.SubmitAndWaitAsync(page.Locator($".group-row[data-group-id='{groupId}'] .group-members"));

                    Assert.That(page.Url, Does.Contain($"Admin/Users?g={groupId}"), "The members are not opened");
                    Assert.That(await page.Locator("#Input_Group").InputValueAsync(), Is.EqualTo(groupId), "The users are not filtered by the group");
                    await Assertions.Expect(page.Locator(".user-row", new PageLocatorOptions { HasText = this.TestSettings.AdminUserName }).First).ToBeVisibleAsync();
                    Assert.That(
                        await page.Locator(".user-row", new PageLocatorOptions { HasText = this.TestSettings.TestUserName2 }).CountAsync(),
                        Is.Zero,
                        "A user of another group is listed");
                });
    }

    /// <summary>
    /// The id of a group of the groups page.
    /// </summary>
    private static async Task<string> GetGroupIdAsync(IPage page, TestConfig testSettings, string groupName)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/Groups");

        var groupId = await page.EvaluateAsync<string>(
                          """
                          name => {
                              for (const edit of document.querySelectorAll('a[href*="EditGroup/"]')) {
                                  const item = edit.closest('li, .group-row');
                                  const title = item?.querySelector('.group-name');
                                  if (title && title.textContent.replace(/\s+/g, ' ').trim() === name) {
                                      return edit.getAttribute('href').match(/(\d+)\D*$/)[1];
                                  }
                              }
                              return null;
                          }
                          """,
                          groupName);

        Assert.That(groupId, Is.Not.Null, $"The group {groupName} is not listed");

        return groupId;
    }
}
