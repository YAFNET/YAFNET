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

using System.Text.RegularExpressions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.AdminTests;

/// <summary>
/// The parent forums of the forums.
/// </summary>
public class ForumHierarchyTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A forum with a parent and sub forums can still be saved, but a forum can't be moved below its own sub forum.
    /// </summary>
    [Test]
    public async Task ForumWithParentAndSubForumsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var suffix = Random.Shared.Next();

                var top = await this.SaveForumAsync(page, null, $"HierarchyTop{suffix}", null);
                var middle = await this.SaveForumAsync(page, null, $"HierarchyMiddle{suffix}", top);
                await this.SaveForumAsync(page, null, $"HierarchyBottom{suffix}", middle, parentOfNew: middle);

                // the index shows two levels, the third level is shown in its parent forum
                var bottom = await this.ForumIdAsync(page, $"HierarchyBottom{suffix}", middle);

                // the middle forum has a parent and a sub forum
                await this.SaveForumAsync(page, middle, $"HierarchyMiddle{suffix}", top, "changed description");

                Assert.That(page.Url, Does.Contain("Admin/Forums"), "The forum with parent and sub forums could not be saved");

                // the top forum can't be moved below its sub forum
                await this.SaveForumAsync(page, top, $"HierarchyTop{suffix}", bottom);

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("The chosen forum cannot be child forum"),
                    "A forum was moved below its own sub forum");
            });
    }

    /// <summary>
    /// The names of the sub forums in the forum list are shown as entered (not encoded twice).
    /// </summary>
    [Test]
    public async Task SubForumNameIsShownAsEnteredTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var suffix = Random.Shared.Next();
                var subForumName = $"Q & A {suffix}";

                var top = await this.SaveForumAsync(page, null, $"SubForumTop{suffix}", null);
                await this.SaveForumAsync(page, null, subForumName, top, parentOfNew: null);

                await page.GotoAsync(this.TestSettings.TestForumUrl);

                var subForumLink = page.Locator("a.card-link", new PageLocatorOptions { HasText = $"{suffix}" });

                Assert.That(
                    (await subForumLink.First.InnerTextAsync()).Trim(),
                    Is.EqualTo(subForumName),
                    "The name of the sub forum is not shown as entered");
            });
    }

    /// <summary>
    /// The access masks of the forums on the page of a group are saved with the group.
    /// </summary>
    [Test]
    public async Task GroupForumAccessMaskIsSavedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var forumName = $"GroupMaskForum{Random.Shared.Next()}";

                await this.SaveForumAsync(page, null, forumName, null);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Groups");

                var editLink = await page.Locator(".list-group-item", new PageLocatorOptions { HasText = "Registered Users" })
                                   .Locator("a[href*='/Admin/EditGroup/']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "The group was not found");

                var groupUrl = new Uri(new Uri(this.TestSettings.TestForumUrl), editLink).ToString();

                await page.GotoAsync(groupUrl);

                await page.GetByLabel(forumName).SelectOptionAsync(new SelectOptionValue { Label = "Read Only Access" });

                await page.SubmitAndWaitAsync(page.Locator("#Save"));

                await page.GotoAsync(groupUrl);

                Assert.That(
                    (await page.GetByLabel(forumName).Locator("option:checked").InnerTextAsync()).Trim(),
                    Is.EqualTo("Read Only Access"),
                    "The access mask of the forum was not saved");
            });
    }

    /// <summary>
    /// Creates (forumId null) or edits a forum, returns the id of the forum.
    /// </summary>
    private async Task<int> SaveForumAsync(IPage page, int? forumId, string name, int? parentId, string description = "A forum of the hierarchy test", int? parentOfNew = null)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false{(forumId.HasValue ? $"/{forumId}" : string.Empty)}");

        await page.Locator("#Input_Name").FillAsync(name);
        await page.Locator("#Input_Description").FillAsync(description);

        if (parentId.HasValue)
        {
            // the select is a Choices.js select (only the selected option is in the form)
            await page.EvaluateAsync(
                """
                id => {
                    const select = document.querySelector('#Input_ParentID');
                    select.add(new Option('', id, true, true));
                    select.value = id;
                }
                """,
                parentId.Value.ToString());
        }

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

        return forumId ?? await this.ForumIdAsync(page, name, parentOfNew);
    }

    private async Task<int> ForumIdAsync(IPage page, string name, int? parentId = null)
    {
        await page.GotoAsync(parentId.HasValue ? $"{this.TestSettings.TestForumUrl}Topics/{parentId}/forum" : this.TestSettings.TestForumUrl);

        var link = await page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasText = name }).First.GetAttributeAsync("href");

        Assert.That(link, Is.Not.Null, $"The forum {name} was not found");

        return int.Parse(Regex.Match(link, @"/Topics/(\d+)/").Groups[1].Value);
    }
}
