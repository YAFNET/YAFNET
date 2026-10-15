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

namespace YAF.Tests.UITests.UserTests.Features;

/// <summary>
/// The team page tests.
/// </summary>
public class TeamTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The forums of a moderator on the team page are only the forums the user can read
    /// (the names of hidden forums are not shown).
    /// </summary>
    [Test]
    public async Task TeamPageHidesForumsWithoutAccessTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Hidden Team Forum {Random.Shared.Next()}";

                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // by default only admins see the team page
                await this.SetShowTeamToAsync(page, "1");

                try
                {
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.AdminUserName,
                            this.TestSettings.AdminPassword),
                        Is.True,
                        "Login failed");

                    // a forum only for admins, hidden for the other users
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                    await page.Locator("#Input_Name").FillAsync(forumName);
                    await page.Locator("#Input_Description").FillAsync("A forum of the team page test");
                    await page.Locator("#Input_SortOrder").FillAsync("99");
                    await page.Locator("#Input_HideNoAccess").CheckAsync();

                    // a new forum gets the default access mask, the admins moderate this one
                    await page.GetByLabel("Administrators").First.SelectOptionAsync(new SelectOptionValue { Label = "Admin Access" });
                    await page.GetByLabel("Registered").First.SelectOptionAsync(new SelectOptionValue { Label = "No Access" });
                    await page.GetByLabel("Guest").First.SelectOptionAsync(new SelectOptionValue { Label = "No Access" });

                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                    // the admin moderates the forum and sees it on the team page
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Team");

                    Assert.That(
                        await page.ContentAsync(),
                        Does.Contain(forumName),
                        "The admin does not see the new forum on the team page (the test needs a fresh moderator list)");

                    var forumId = await page.Locator("select[name$='SelectedForumId'] option", new PageLocatorOptions { HasText = forumName })
                                      .First.GetAttributeAsync("value");

                    Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                        Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Team");

                    Assert.That(
                        await page.Locator("body").InnerTextAsync(),
                        Does.Contain("Moderators"),
                        $"The team page is not shown ({page.Url})");

                    Assert.That(
                        await page.ContentAsync(),
                        Does.Not.Contain(forumName),
                        "The name of a hidden forum is shown on the team page");

                    // the forum select of the team page only leads to forums the user can read
                    // (the link contains the name), the id is sent by the client
                    foreach (var value in new[] { forumId, "x" })
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Team");

                        IResponse response = null;

                        await page.RunAndWaitForNavigationAsync(
                            async () => response = await page.RunAndWaitForResponseAsync(
                                            () => page.Locator("select[name$='SelectedForumId']").First.EvaluateAsync(
                                                "(select, value) => { select.add(new Option(value, value, true, true)); select.value = value; select.form.submit(); }",
                                                value),
                                            r => r.Request.Method == "POST" && r.Url.Contains("Team")));

                        Assert.That(response.Status, Is.LessThan(400), $"The forum select failed for {value}");
                        Assert.That(
                            response.Headers.TryGetValue("location", out var location) ? location : string.Empty,
                            Does.Not.Contain($"/{forumId}/"),
                            "The link to the hidden forum (with its name) is shown");
                    }
                }
                finally
                {
                    await this.SetShowTeamToAsync(page, "0");
                }
            });
    }

    /// <summary>
    /// The names of the moderated forums are shown once encoded in the forum select of the team page
    /// ("Tom &amp; Jerry" was shown for "Tom & Jerry").
    /// </summary>
    [Test]
    public async Task TeamPageForumNameEncodingTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var forumName = $"Tom & Jerry's \"Team\" {Random.Shared.Next()}";

                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                // a forum moderated by the admins
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");

                await page.Locator("#Input_Name").FillAsync(forumName);
                await page.Locator("#Input_Description").FillAsync("A forum of the team page test");
                await page.Locator("#Input_SortOrder").FillAsync("98");

                await page.GetByLabel("Administrators").First.SelectOptionAsync(new SelectOptionValue { Label = "Admin Access" });

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Team");

                // the choices of the forum select (shown by the select script)
                var choices = await page.Locator(".choices__item--choice").EvaluateAllAsync<string[]>(
                                  "items => items.map(i => i.textContent.trim())");

                Assert.That(
                    choices,
                    Has.Some.Contains(forumName),
                    $"The forum is not shown with its name: {string.Join(" | ", System.Linq.Enumerable.Where(choices, c => c.Contains("Jerry")))}");
            });
    }

    /// <summary>
    /// The post of the team page (forum select) is denied like the page when the team page is only
    /// for admins (the default), it showed the team.
    /// </summary>
    [Test]
    public async Task TeamPagePostNeedsPermissionTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.TestUserName,
                        this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Team");

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("You tried to enter an area where you didn't have access."),
                    "The team page is shown to a member (the test needs the default setting)");

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                // a form to the team page with the token of the user
                await page.RunAndWaitForNextPageAsync(
                    () => page.EvaluateAsync(
                        """
                        () => {
                            const form = document.createElement('form');
                            form.method = 'post';
                            form.action = '/Team';
                            form.appendChild(document.querySelector("input[name='__RequestVerificationToken']").cloneNode());
                            document.body.appendChild(form);
                            form.submit();
                        }
                        """));

                var pageSource = await page.ContentAsync();

                Assert.That(
                    pageSource,
                    Does.Contain("You tried to enter an area where you didn't have access."),
                    "The team is shown on the post");
            });
    }

    /// <summary>
    /// Sets who can see the team page (as admin, logged out afterwards).
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="value">The view permission (0 nobody/admins, 1 registered users).</param>
    private async Task SetShowTeamToAsync(IPage page, string value)
    {
        await page.GotoAsync(this.TestSettings.TestForumUrl);

        if (await page.Locator("//li[contains(@class,'dropdown-notify')]").IsVisibleAsync())
        {
            await page.LogOutUserAsync();
        }

        Assert.That(
            await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
            Is.True,
            "Login failed");

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings/Display");

        await page.Locator("#Input_ShowTeamTo").SelectOptionAsync(value);

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']:visible").First);

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/HostSettings/Display");

        Assert.That(
            await page.Locator("#Input_ShowTeamTo").InputValueAsync(),
            Is.EqualTo(value),
            "The team page setting was not saved");

        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");
    }
}
