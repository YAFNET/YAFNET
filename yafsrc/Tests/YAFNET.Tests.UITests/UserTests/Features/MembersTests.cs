﻿/* Yet Another Forum.NET
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

namespace YAF.Tests.UITests.UserTests.Features;

/// <summary>
/// The members list tests.
/// </summary>
public class MembersTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The filter by number of posts uses the selected comparison (equal, less or equal, more or equal):
    /// the page always replaced it with "more or equal".
    /// </summary>
    [Test]
    public async Task NumberOfPostsFilterUsesSelectedComparisonTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // a new user has no posts
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                var userRow = page.Locator("li.member-row", new PageLocatorOptions { HasTextString = userName });

                // "more or equal 5": the user is not listed
                await SearchAsync(page, this.TestSettings, userName, "3", "5");

                Assert.That(await userRow.CountAsync(), Is.Zero, "A user without posts is listed for \"more or equal 5 posts\"");

                // "less or equal 5": the user is listed
                await SearchAsync(page, this.TestSettings, userName, "2", "5");

                Assert.That(
                    await page.Locator("#Input_NumPostValue").InputValueAsync(),
                    Is.EqualTo("2"),
                    "The selected comparison was replaced");
                Assert.That(await userRow.CountAsync(), Is.EqualTo(1), "A user without posts is not listed for \"less or equal 5 posts\"");
            });
    }

    private async static Task SearchAsync(IPage page, TestConfig testSettings, string userName, string comparison, string numberOfPosts)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}members");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();
        await page.Locator("#Input_UserSearchName").FillAsync(userName);
        await page.Locator("#Input_NumPosts").FillAsync(numberOfPosts);

        // the select is replaced by a script control
        await page.EvaluateAsync("value => document.querySelector('#Input_NumPostValue').value = value", comparison);

        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'Search')]"));
    }

    /// <summary>
    /// A sort of the members keeps the selected letter (all members were listed again).
    /// </summary>
    [Test]
    public async Task SortKeepsTheSelectedLetterTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}members?letter=T");

                var sort = page.Locator("button[formaction*='UserNameDesc']");

                await page.RunAndWaitForNextPageAsync(() => sort.EvaluateAsync("b => b.form.requestSubmit(b)"));

                var names = await page.Locator(".member-row h5").AllInnerTextsAsync();

                Assert.That(names, Is.Not.Empty, "No members are listed");
                Assert.That(
                    names,
                    Has.All.Matches<string>(n => n.Trim().Split((char)10)[^1].Trim().StartsWith('T') || n.Trim().Split((char)10)[^1].Trim().StartsWith('t')),
                    "The sort lost the selected letter");
            });
    }

    /// <summary>
    /// A change of the page size on a later page shows the first page (the page stayed, it was empty
    /// and had no pager to go back).
    /// </summary>
    [Test]
    public async Task PageSizeOnALaterPageShowsMembersTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}members?p=2");

                var size = page.Locator("select#Size");

                await page.RunAndWaitForNextPageAsync(
                    () => size.EvaluateAsync("select => { select.value = '50'; select.form.submit(); }"));

                Assert.That(await page.Locator(".member-row").CountAsync(), Is.GreaterThan(0), "No members are listed");
            });
    }

    /// <summary>
    /// A guest who can view the profiles (profile view permission "everyone") gets no "PM" and "Email" buttons
    /// (they led to "access denied").
    /// </summary>
    [Test]
    public async Task GuestSeesNoContactButtonsOnAProfileTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}members?letter=T");

                var profileLink = await page.Locator(".member-row a", new PageLocatorOptions { HasText = this.TestSettings.TestUserName })
                                      .First.GetAttributeAsync("href");

                Assert.That(profileLink, Is.Not.Null, "The profile link of the test user was not found");

                var profileUrl = new Uri(new Uri(this.TestSettings.TestForumUrl), profileLink).ToString();

                await SetHostSettingAsync(page, this.TestSettings, "ProfileViewPermissions", "2");

                int contactButtons;

                try
                {
                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    await page.GotoAsync(profileUrl);

                    Assert.That(await page.ContentAsync(), Does.Contain(this.TestSettings.TestUserName), "The profile is not shown");

                    contactButtons = await page.Locator("a[href*='MyMessages'], a[href*='/Email']").CountAsync();
                }
                finally
                {
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword);

                    await SetHostSettingAsync(page, this.TestSettings, "ProfileViewPermissions", "1");
                }

                Assert.That(contactButtons, Is.Zero, "The guest gets the PM and Email buttons");
            });
    }

    /// <summary>
    /// Sets a select or a text box of the host settings.
    /// </summary>
    private async static Task SetHostSettingAsync(IPage page, TestConfig testSettings, string name, string value)
    {
        await page.GotoAsync($"{testSettings.TestForumUrl}Admin/HostSettings");

        await page.Locator($"#Input_{name}").EvaluateAsync("(input, value) => input.value = value", value);

        var save = page.Locator("button[formaction*='Save']").First;

        await page.RunAndWaitForNavigationAsync(() => save.EvaluateAsync("b => b.form.requestSubmit(b)"));

        await page.WaitForLoadStateAsync();
    }
}
