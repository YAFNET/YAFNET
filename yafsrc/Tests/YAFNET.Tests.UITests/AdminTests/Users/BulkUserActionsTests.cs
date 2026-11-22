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

 * https://www.apache.org/licenses/LICENSE-2.0

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
/// The bulk actions of the selected users of Admin > Users.
/// </summary>
public class BulkUserActionsTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The selected users that are not approved are approved, the bar shows the number of the selected users.
    /// </summary>
    [Test]
    public async Task BulkApproveTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                var prefix = await this.CreateUsersAsync(page, "BulkApprove");

                await SelectAllAsync(page, prefix);

                Assert.That(await page.Locator("#BulkBar").IsVisibleAsync(), Is.True, "The bar of the bulk actions is not shown");
                Assert.That(await page.Locator("#BulkBar [data-bulk-count]").InnerTextAsync(), Does.Contain("2"), "The number of the selected users");

                await page.SubmitAndWaitAsync(page.Locator("#BulkBar button[formaction$='/BulkApprove']"));

                Assert.That(await page.ContentAsync(), Does.Contain("2 users were approved."));
                Assert.That(await page.Locator(".user-row .badge.text-bg-danger").CountAsync(), Is.Zero, "A user is not approved");

                await DeleteUsersAsync(page, prefix);
            });
    }

    /// <summary>
    /// The selected users are suspended (a preset of the dialog) and the suspension is removed.
    /// </summary>
    [Test]
    public async Task BulkSuspendAndRemoveSuspensionTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                var prefix = await this.CreateUsersAsync(page, "BulkSuspend");

                await SelectAllAsync(page, prefix);

                await page.Locator("#BulkBar [data-bs-target='#BulkSuspendDialog']").ClickAsync();

                var dialog = page.Locator("#BulkSuspendDialog");

                await Assertions.Expect(dialog).ToBeVisibleAsync();

                await dialog.Locator("[data-set-values*='\"7\"']").ClickAsync();
                await dialog.Locator("#BulkSuspendReason").FillAsync("Suspended by a test");

                Assert.That(await page.Locator("#BulkSuspendCount").InputValueAsync(), Is.EqualTo("7"), "The preset was not set");

                await page.SubmitAndWaitAsync(dialog.Locator("button[formaction$='/BulkSuspend']"));

                Assert.That(await page.ContentAsync(), Does.Contain("2 users were suspended."));
                Assert.That(await page.Locator(".user-row .fa-user-clock").CountAsync(), Is.EqualTo(2), "The users are not suspended");

                await SelectAllAsync(page, prefix);
                await page.SubmitAndWaitAsync(page.Locator("#BulkBar button[formaction$='/BulkUnsuspend']"));

                Assert.That(await page.ContentAsync(), Does.Contain("The suspension of 2 users was removed."));
                Assert.That(await page.Locator(".user-row .fa-user-clock").CountAsync(), Is.Zero, "The users are still suspended");

                await DeleteUsersAsync(page, prefix);
            });
    }

    /// <summary>
    /// The selected users are added to a role and removed from it, the last role of a user is not removed.
    /// </summary>
    [Test]
    public async Task BulkRoleTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                var prefix = await this.CreateUsersAsync(page, "BulkRole");

                var message = await RunRoleActionAsync(page, prefix, "Administrators", "BulkAddRole");

                Assert.That(message, Does.Contain("2 users were added to the role Administrators."));

                message = await RunRoleActionAsync(page, prefix, "Administrators", "BulkRemoveRole");

                Assert.That(message, Does.Contain("2 users were removed from the role Administrators."));

                // the last role
                message = await RunRoleActionAsync(page, prefix, "Registered Users", "BulkRemoveRole");

                Assert.That(message, Does.Contain("0 users were removed").And.Contain("2 users were skipped"), "The last role was removed");

                await DeleteUsersAsync(page, prefix);
            });
    }

    /// <summary>
    /// The dialog lists the selected users, they are deleted.
    /// </summary>
    [Test]
    public async Task BulkDeleteTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                var prefix = await this.CreateUsersAsync(page, "BulkDelete");

                await SelectAllAsync(page, prefix);

                await page.Locator("#BulkBar [data-bs-target='#BulkDeleteDialog']").ClickAsync();

                var dialog = page.Locator("#BulkDeleteDialog");

                await Assertions.Expect(dialog).ToBeVisibleAsync();

                Assert.That(await dialog.Locator("[data-bulk-names] li").CountAsync(), Is.EqualTo(2), "The dialog doesn't list the users");
                Assert.That(await dialog.Locator("[data-bulk-names]").InnerTextAsync(), Does.Contain($"{prefix}1"));

                await page.SubmitAndWaitAsync(dialog.Locator("button[formaction$='/BulkDelete']"));

                Assert.That(await page.ContentAsync(), Does.Contain("2 users were deleted."));

                await FilterAsync(page, prefix);

                Assert.That(await page.Locator(".user-row").CountAsync(), Is.Zero, "The users were not deleted");
            });
    }

    /// <summary>
    /// The server checks the posted users: an admin is not deleted, the guest (no box, the id is posted by hand) is
    /// skipped.
    /// </summary>
    [Test]
    public async Task BulkDeleteSkipsAdminsAndGuestTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                var prefix = await this.CreateUsersAsync(page, "BulkAdmin", 1);

                await RunRoleActionAsync(page, prefix, "Administrators", "BulkAddRole");

                // the guest has no box, its id is posted with the admin
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                await page.OpenUserFilterAsync();
                await page.Locator("#Input_Since").SelectOptionAsync("9999");
                await page.RunAndWaitForNextPageAsync(() => page.Locator("#Input_Name").PressAsync("Enter"));

                var guestId = await page.Locator(".user-row", new PageLocatorOptions { HasText = "Guest" })
                                  .First.GetAttributeAsync("data-user-id");

                Assert.That(guestId, Is.Not.Null, "The guest is not listed");

                await SelectAllAsync(page, prefix);

                await page.Locator("form.admin-users").EvaluateAsync(
                    "(form, id) => form.insertAdjacentHTML('beforeend', `<input type=\"hidden\" name=\"SelectedUsers\" value=\"${id}\">`)",
                    guestId);

                await page.RunAndWaitForNextPageAsync(
                    () => page.Locator("#BulkDeleteDialog button[formaction$='/BulkDelete']")
                        .EvaluateAsync("b => b.form.requestSubmit(b)"));

                Assert.That(await page.ContentAsync(), Does.Contain("0 users were deleted.").And.Contain("2 users were skipped"));

                await FilterAsync(page, prefix);

                Assert.That(await page.Locator(".user-row").CountAsync(), Is.EqualTo(1), "The admin was deleted");

                // clean up
                await RunRoleActionAsync(page, prefix, "Administrators", "BulkRemoveRole");
                await DeleteUsersAsync(page, prefix);
            });
    }

    /// <summary>
    /// Creates users (not approved) with a new prefix.
    /// </summary>
    /// <returns>The prefix of the names.</returns>
    private async Task<string> CreateUsersAsync(IPage page, string name, int count = 2)
    {
        var prefix = $"{name}{Random.Shared.Next(100000, 999999)}u";

        for (var i = 1; i <= count; i++)
        {
            var userName = $"{prefix}{i}";

            await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/RegisterUser");
            await page.Locator("#Input_UserName").FillAsync(userName);
            await page.Locator("#Input_Email").FillAsync($"{userName.ToLower()}@test.com");
            await page.Locator("#Input_Password").FillAsync(this.TestSettings.TestUserPassword);
            await page.Locator("#Input_Password2").FillAsync(this.TestSettings.TestUserPassword);
            await page.SubmitAndWaitAsync(page.Locator("button[formaction*='ForumRegister']"));
        }

        return prefix;
    }

    /// <summary>
    /// Shows the users of the prefix.
    /// </summary>
    private static async Task FilterAsync(IPage page, string prefix)
    {
        await page.GotoAsync($"{page.Url.Split("/Admin/")[0]}/Admin/Users");
        await page.OpenUserFilterAsync();

        await page.Locator("#Input_Since").SelectOptionAsync("9999");

        var name = page.Locator("#Input_Name");

        await name.FillAsync(prefix);
        await page.RunAndWaitForNextPageAsync(() => name.PressAsync("Enter"));
    }

    /// <summary>
    /// Shows the users of the prefix and selects them all.
    /// </summary>
    private static async Task SelectAllAsync(IPage page, string prefix)
    {
        await FilterAsync(page, prefix);

        await page.Locator("#BulkSelectAll").CheckAsync();
    }

    /// <summary>
    /// Adds the users of the prefix to a role or removes them from it.
    /// </summary>
    /// <returns>The page content (the message).</returns>
    private static async Task<string> RunRoleActionAsync(IPage page, string prefix, string role, string handler)
    {
        await SelectAllAsync(page, prefix);

        await page.Locator("#BulkBar [data-bs-target='#BulkRoleDialog']").ClickAsync();

        var dialog = page.Locator("#BulkRoleDialog");

        await Assertions.Expect(dialog).ToBeVisibleAsync();

        await dialog.Locator("#BulkRole").SelectOptionAsync(new SelectOptionValue { Label = role });

        await page.SubmitAndWaitAsync(dialog.Locator($"button[formaction$='/{handler}']"));

        return await page.ContentAsync();
    }

    /// <summary>
    /// Deletes the users of the prefix.
    /// </summary>
    private static async Task DeleteUsersAsync(IPage page, string prefix)
    {
        await SelectAllAsync(page, prefix);

        await page.RunAndWaitForNextPageAsync(
            () => page.Locator("#BulkDeleteDialog button[formaction$='/BulkDelete']").EvaluateAsync("b => b.form.requestSubmit(b)"));
    }
}
