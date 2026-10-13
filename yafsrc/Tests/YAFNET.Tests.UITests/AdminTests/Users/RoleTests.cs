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
    /// The guest group can't be deleted, also not with a manipulated form (the list has no delete button for it).
    /// </summary>
    [Test]
    [Description("The guest group can't be deleted")]
    public async Task GuestGroupCanNotBeDeletedTest()
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

                    // the group without delete button
                    const string findGuestGroupId = """
                                                    () => {
                                                        for (const group of document.querySelectorAll('.btn-group')) {
                                                            const edit = group.querySelector('a[href*="EditGroup"]');
                                                            if (edit && !group.querySelector('button[formaction*="Delete"]')) {
                                                                return edit.getAttribute('href').match(/(\d+)\D*$/)[1];
                                                            }
                                                        }
                                                        return null;
                                                    }
                                                    """;

                    var guestGroupId = await page.EvaluateAsync<string>(findGuestGroupId);

                    Assert.That(guestGroupId, Is.Not.Null, "The guest group is not listed");

                    // the delete handler with the id of the guest group
                    var status = await page.EvaluateAsync<int>(
                        """
                        async url => {
                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                            const body = new URLSearchParams({ '__RequestVerificationToken': token });
                            const response = await fetch(url, { method: 'POST', body: body });
                            return response.status;
                        }
                        """,
                        $"{this.TestSettings.TestForumUrl}Admin/Groups?handler=Delete&id={guestGroupId}");

                    Assert.That(status, Is.EqualTo(200), "The delete handler failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Groups");

                    Assert.That(
                        await page.EvaluateAsync<string>(findGuestGroupId),
                        Is.EqualTo(guestGroupId),
                        "The guest group was deleted");
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
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();
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
    private static async Task<string> UserStyleAsync(IPage page, TestConfig testSettings, string userName)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Members");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();
        await page.Locator("#Input_UserSearchName").FillAsync(userName);
        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Search')]"));

        return await page.Locator("a", new PageLocatorOptions { HasTextString = userName }).First.GetAttributeAsync("style");
    }
}
