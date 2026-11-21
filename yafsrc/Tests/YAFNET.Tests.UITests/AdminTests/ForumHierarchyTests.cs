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
    /// Copying a forum creates a new forum: the save updated the copied forum with the values of the copy.
    /// </summary>
    [Test]
    public async Task CopyForumCreatesNewForumTest()
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

                var source = await this.SaveForumAsync(page, null, $"CopySource{suffix}", null);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/true/{source}");
                await page.Locator("#Input_Name").FillAsync($"CopyTarget{suffix}");
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                var copy = await this.ForumIdAsync(page, $"CopyTarget{suffix}");

                Assert.That(copy, Is.Not.EqualTo(source), "The copied forum was changed instead of a new forum");
                Assert.That(await this.ForumIdAsync(page, $"CopySource{suffix}"), Is.EqualTo(source), "The copied forum is missing");
            });
    }

    /// <summary>
    /// "Copy" of a forum in Admin > Forums (button and context menu) copies that forum
    /// (the link of the context menu had no forum id and opened an empty new forum).
    /// </summary>
    [Test]
    public async Task CopyButtonCopiesTheForumTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var name = $"CopyButton{Random.Shared.Next()}";

                var forumId = await this.SaveForumAsync(page, null, name, null);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

                var copyLink = await page.Locator($".forum-row[data-forum-id='{forumId}'] .forum-actions a[href*='/EditForum/true']").GetAttributeAsync("href");

                Assert.That(copyLink, Does.EndWith($"/EditForum/true/{forumId}"), "The copy link has no forum");

                // the context menu (right click) has the same link
                Assert.That(
                    await page.Locator($".forum-row[data-forum-id='{forumId}'] .context-menu a[href*='/EditForum/true']").GetAttributeAsync("href"),
                    Is.EqualTo(copyLink),
                    "The copy link of the context menu has no forum");

                await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), copyLink).ToString());

                Assert.That(
                    await page.Locator("#Input_Description").InputValueAsync(),
                    Is.EqualTo("A forum of the hierarchy test"),
                    "The copy does not show the values of the forum");
            });
    }

    /// <summary>
    /// Admin > Forums shows each category once with all its forums (the rows of categories and forums were paged,
    /// a category with more forums than the page size was shown on several pages, each time with all its forums).
    /// </summary>
    [Test]
    public async Task ForumListShowsEachCategoryOnceTest()
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
                var categoryName = $"ListCategory{suffix}";

                var categoryId = await this.CreateCategoryAsync(page, categoryName);

                for (var i = 1; i <= 6; i++)
                {
                    await this.SaveForumInCategoryAsync(page, $"List{suffix}_{i:00}", categoryId, i, null);
                }

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

                Assert.That(
                    await page.Locator($"[data-category-id='{categoryId}']").CountAsync(),
                    Is.EqualTo(1),
                    "The category is not shown once");
                Assert.That(
                    await page.Locator($"[data-category-id='{categoryId}'] .forum-row").CountAsync(),
                    Is.EqualTo(6),
                    "The forums of the category are not shown with the category");
            });
    }

    /// <summary>
    /// Admin > Forums shows the forums of a category as a tree: a sub forum follows its parent, also when its
    /// sort order is lower than the one of the parent, and every level is indented (a sub forum was shown above
    /// its parent, the third level like the second).
    /// </summary>
    [Test]
    public async Task ForumListShowsSubForumsBelowTheirParentTest()
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
                var categoryId = await this.CreateCategoryAsync(page, $"TreeCategory{suffix}");

                await this.SaveForumInCategoryAsync(page, $"TreeTop{suffix}", categoryId, 5, null);
                var top = await this.AdminForumIdAsync(page, $"TreeTop{suffix}");

                await this.SaveForumInCategoryAsync(page, $"TreeMiddle{suffix}", categoryId, 1, top);
                var middle = await this.AdminForumIdAsync(page, $"TreeMiddle{suffix}");

                await this.SaveForumInCategoryAsync(page, $"TreeBottom{suffix}", categoryId, 1, middle);
                await this.SaveForumInCategoryAsync(page, $"TreeOther{suffix}", categoryId, 3, null);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

                // the forums of the category in the order of the list, with their depth
                var rows = await page.EvaluateAsync<string[]>(
                               """
                               suffix => [...document.querySelectorAll('.forum-row')]
                                   .map(row => [row.querySelector('.forum-name').textContent.trim(), row.dataset.depth])
                                   .filter(([name]) => name.includes(suffix))
                                   .map(([name, depth]) => name.replace(suffix, '') + ':' + depth)
                               """,
                               suffix.ToString());

                Assert.That(
                    rows,
                    Is.EqualTo(new[] { "TreeOther:0", "TreeTop:0", "TreeMiddle:1", "TreeBottom:2" }),
                    "The forums are not shown as a tree");
            });
    }

    /// <summary>
    /// The order of the forums of one level is changed with drag and drop and saved right away,
    /// the server only saves the order of forums of one level (the same category and parent).
    /// </summary>
    [Test]
    public async Task DragAndDropSavesTheForumOrderTest()
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
                var categoryId = await this.CreateCategoryAsync(page, $"DragCategory{suffix}");

                await this.SaveForumInCategoryAsync(page, $"DragA{suffix}", categoryId, 1, null);
                await this.SaveForumInCategoryAsync(page, $"DragB{suffix}", categoryId, 2, null);
                await this.SaveForumInCategoryAsync(page, $"DragC{suffix}", categoryId, 3, null);

                var other = await this.SaveForumAsync(page, null, $"DragOther{suffix}", null);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

                var card = page.Locator($"[data-category-id='{categoryId}']");

                // C is dragged above A
                var response = await page.RunAndWaitForResponseAsync(
                                   () => card.Locator(".forum-row", new LocatorLocatorOptions { HasText = $"DragC{suffix}" })
                                       .Locator(".forum-drag-handle")
                                       .DragToAsync(card.Locator(".forum-row", new LocatorLocatorOptions { HasText = $"DragA{suffix}" }).Locator(".forum-drag-handle")),
                                   r => r.Url.Contains("SaveOrder"));

                Assert.That(response.Status, Is.EqualTo(200), "The new order was not saved");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

                var names = await card.Locator(".forum-name").AllInnerTextsAsync();

                Assert.That(
                    names.Select(n => n.Trim().Replace(suffix.ToString(), string.Empty)),
                    Is.EqualTo(new[] { "DragC", "DragA", "DragB" }),
                    "The order was not saved");

                // forums of different levels (categories) are refused
                var ids = await card.Locator(".forum-row").EvaluateAllAsync<int[]>("rows => rows.map(r => Number(r.dataset.forumId))");

                var status = await page.EvaluateAsync<int>(
                                 """
                                 async ([url, ids]) => {
                                     const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                                     const response = await fetch(url, {
                                         method: 'POST',
                                         headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                                         body: JSON.stringify({ type: 'forums', ids: ids })
                                     });
                                     return response.status;
                                 }
                                 """,
                                 new object[] { await page.Locator(".admin-forums").GetAttributeAsync("data-save-url"), ids.Append(other).ToArray() });

                Assert.That(status, Is.EqualTo(400), "Forums of different levels were sorted together");
            });
    }

    /// <summary>
    /// A forum is deleted with its topics only when its name is typed in (also checked by the server),
    /// or its topics are moved to another forum; the sub forums are kept without parent.
    /// </summary>
    [Test]
    public async Task DeleteForumNeedsTheNameOrATargetTest()
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
                var name = $"Delete <{suffix}>";

                var forumId = await this.SaveForumAsync(page, null, name, null);
                var subForumId = await this.SaveForumAsync(page, null, $"DeleteSub{suffix}", forumId, parentOfNew: forumId);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/DeleteForum/{forumId}");

                var submit = page.Locator("[data-delete-submit]");
                var confirmName = page.Locator("#ConfirmName");

                Assert.That(await page.Locator(".admin-delete-forum").TextContentAsync(), Does.Contain("1"), "The sub forum count is missing");
                await Assertions.Expect(submit).ToBeDisabledAsync();

                await confirmName.FillAsync(name[..^1]);
                await Assertions.Expect(submit).ToBeDisabledAsync();

                // a wrong name is refused by the server (without the check of the page)
                await confirmName.FillAsync("wrong name");
                await submit.EvaluateAsync("b => { b.disabled = false; b.form.requestSubmit(b); }");
                await page.WaitForLoadStateAsync();

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

                Assert.That(
                    await page.Locator($".forum-row[data-forum-id='{forumId}']").CountAsync(),
                    Is.EqualTo(1),
                    "The forum was deleted with a wrong name");

                // the right name
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/DeleteForum/{forumId}");
                await confirmName.FillAsync(name);
                await Assertions.Expect(submit).ToBeEnabledAsync();

                await page.SubmitAndWaitAsync(submit);

                Assert.That(page.Url, Does.Contain("Admin/Forums"), "The list of forums is not shown after the delete");
                Assert.That(
                    await page.Locator($".forum-row[data-forum-id='{forumId}']").CountAsync(),
                    Is.Zero,
                    "The forum was not deleted");
                Assert.That(
                    await page.Locator($".forum-row[data-forum-id='{subForumId}']").GetAttributeAsync("data-depth"),
                    Is.EqualTo("0"),
                    "The sub forum was not kept as a forum without parent");
                await Assertions.Expect(page.Locator(".toast").Last).ToContainTextAsync(name);
            });
    }

    /// <summary>
    /// The forum select (e.g. "move topics to" when a forum is deleted) lists a sub forum below its parent,
    /// indented per level (a sub forum with a lower sort order was listed above its parent, every level alike).
    /// </summary>
    [Test]
    public async Task ForumSelectListsSubForumsBelowTheirParentTest()
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
                var categoryId = await this.CreateCategoryAsync(page, $"SelectCategory{suffix}");

                await this.SaveForumInCategoryAsync(page, $"SelectTop{suffix}", categoryId, 5, null);
                var top = await this.AdminForumIdAsync(page, $"SelectTop{suffix}");

                await this.SaveForumInCategoryAsync(page, $"SelectMiddle{suffix}", categoryId, 1, top);
                var middle = await this.AdminForumIdAsync(page, $"SelectMiddle{suffix}");

                await this.SaveForumInCategoryAsync(page, $"SelectBottom{suffix}", categoryId, 1, middle);
                await this.SaveForumInCategoryAsync(page, $"SelectOther{suffix}", categoryId, 3, null);

                // the forums of the select (searched by the suffix), the indent is shown as "." per space
                var options = await page.EvaluateAsync<string[]>(
                                  """
                                  async suffix => {
                                      const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                                      const response = await fetch('/api/Forum/GetForums', {
                                          method: 'POST',
                                          headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                                          body: JSON.stringify({ searchTerm: suffix, page: 0 })
                                      });
                                      const json = await response.json();
                                      return json.results.flatMap(group => group.children)
                                          .map(option => option.text.replaceAll('&nbsp;', '.').replace(suffix, ''));
                                  }
                                  """,
                                  suffix.ToString());

                Assert.That(
                    options,
                    Is.EqualTo(new[] { "SelectOther", "SelectTop", " - SelectMiddle", "... - SelectBottom" }),
                    "The forums of the select are not listed as a tree");
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
    /// "Mark all forums read" on the board index marks all forums, not only the first forums the index
    /// has loaded (20, the others are loaded with "show more").
    /// </summary>
    [Test]
    public async Task MarkAllMarksForumsNotLoadedYetTest()
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
                var categoryName = $"MarkAllCategory{suffix}";

                // a category with more forums than the index loads at once, shown after all other categories
                var categoryId = await this.CreateCategoryAsync(page, categoryName);


                for (var i = 1; i <= 21; i++)
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");
                    await page.Locator("#Input_Name").FillAsync($"MarkAll{suffix}_{i:00}");
                    await page.Locator("#Input_Description").FillAsync("A forum of the mark all test");
                    await page.Locator("#Input_SortOrder").FillAsync(i.ToString());
                    await page.EvaluateAsync(
                        """
                        id => {
                            const select = document.querySelector('#Input_CategoryID');
                            select.add(new Option('', id, true, true));
                            select.value = id;
                        }
                        """,
                        categoryId);
                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
                }

                // the last forum of the category is only loaded with "show more"
                const string lastForumRow = """
                                            async ([url, name]) => {
                                                await fetch(url);
                                                const response = await fetch(url + '&handler=ShowMore&index=1');
                                                const html = new DOMParser().parseFromString(await response.text(), 'text/html');
                                                const link = [...html.querySelectorAll("a[href*='/forum/']")]
                                                    .find(a => a.textContent.trim() === name);
                                                if (!link) {
                                                    return null;
                                                }
                                                const row = link.closest('.forum-row');
                                                return [link.getAttribute('href'), String(row.querySelector('.fa-comments.text-success') !== null)];
                                            }
                                            """;

                var categoryUrl = $"{this.TestSettings.TestForumUrl}?c={categoryId}";
                var lastForum = $"MarkAll{suffix}_21";

                var row = await page.EvaluateAsync<string[]>(lastForumRow, new[] { categoryUrl, lastForum });

                Assert.That(row, Is.Not.Null, "The last forum is not loaded with show more");

                var forumId = int.Parse(Regex.Match(row[0], @"/forum/(\d+)/").Groups[1].Value);

                Assert.That(
                    await page.CreateTopicAsync(this.TestSettings, forumId, $"Mark all {suffix}", "a new post"),
                    Is.Not.Null,
                    "The topic was not created");

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.TestUserName, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                row = await page.EvaluateAsync<string[]>(lastForumRow, new[] { categoryUrl, lastForum });

                Assert.That(row[1], Is.EqualTo("true"), "The forum has no new posts for the user");

                await page.GotoAsync(this.TestSettings.TestForumUrl);
                await page.SubmitAndWaitAsync(page.Locator("button[formaction$='/MarkAll']"));

                row = await page.EvaluateAsync<string[]>(lastForumRow, new[] { categoryUrl, lastForum });

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // the forums of the category are hidden again for the other tests
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditCategory/{categoryId}");
                await page.Locator("#Input_Active").UncheckAsync();
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                Assert.That(row[1], Is.EqualTo("false"), "Mark all did not mark the forum that was not loaded yet");
            });
    }

    /// <summary>
    /// A deleted category is removed from the forum list (the list of the page is kept in the session,
    /// the deleted category was still shown).
    /// </summary>
    [Test]
    public async Task DeletedCategoryIsRemovedFromListTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
                    Is.True,
                    "Login failed");

                var categoryName = $"DeleteCategory{Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditCategory/0");
                await page.Locator("#Input_Name").FillAsync(categoryName);
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                var item = page.Locator(".category-card", new PageLocatorOptions { HasText = categoryName });

                Assert.That(await item.CountAsync(), Is.GreaterThan(0), "The category was not created");

                // the confirm dialog of the button is skipped
                IResponse response = null;

                await page.RunAndWaitForNextPageAsync(
                    async () => response = await page.RunAndWaitForResponseAsync(
                                    () => item.Locator("button[formaction*='DeleteCategory']").First.EvaluateAsync("b => b.form.requestSubmit(b)"),
                                    r => r.Request.Method == "POST"));

                Assert.That(response.Status, Is.LessThan(500), "Deleting the category failed");

                Assert.That(
                    await page.Locator(".category-card", new PageLocatorOptions { HasText = categoryName }).CountAsync(),
                    Is.Zero,
                    "The deleted category is still shown");
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

    /// <summary>
    /// Creates a category (shown after the other categories) and returns its id.
    /// </summary>
    private async Task<string> CreateCategoryAsync(IPage page, string name)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditCategory/0");
        await page.Locator("#Input_Name").FillAsync(name);
        await page.Locator("#Input_SortOrder").FillAsync("999");
        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

        var categoryId = await page.EvaluateAsync<string>(
                             """
                             name => [...document.querySelectorAll('[data-category-id]')]
                                 .find(card => card.querySelector('.category-name')?.textContent.trim() === name)?.dataset.categoryId ?? null
                             """,
                             name);

        Assert.That(categoryId, Is.Not.Null, $"The category {name} was not created");

        return categoryId;
    }

    /// <summary>
    /// Saves a new forum in a category with a sort order and an optional parent forum.
    /// </summary>
    private async Task SaveForumInCategoryAsync(IPage page, string name, string categoryId, int sortOrder, int? parentId)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditForum/false");
        await page.Locator("#Input_Name").FillAsync(name);
        await page.Locator("#Input_Description").FillAsync("A forum of the tree test");
        await page.Locator("#Input_SortOrder").FillAsync(sortOrder.ToString());

        // the selects are Choices.js selects (only the selected option is in the form)
        await page.EvaluateAsync(
            """
            ([categoryId, parentId]) => {
                const set = (id, value) => {
                    const select = document.querySelector(id);
                    select.add(new Option('', value, true, true));
                    select.value = value;
                };
                set('#Input_CategoryID', categoryId);
                if (parentId) {
                    set('#Input_ParentID', parentId);
                }
            }
            """,
            new[] { categoryId, parentId?.ToString() });

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
    }

    /// <summary>
    /// Finds the id of a forum in Admin > Forums (the page that is shown after a forum is saved).
    /// </summary>
    private async Task<int> AdminForumIdAsync(IPage page, string name)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

        var forumId = await page.EvaluateAsync<string>(
                          """
                          name => [...document.querySelectorAll('.forum-row')]
                              .find(row => row.querySelector('.forum-name')?.textContent.trim() === name)?.dataset.forumId ?? null
                          """,
                          name);

        Assert.That(forumId, Is.Not.Null, $"The forum {name} was not found");

        return int.Parse(forumId);
    }

    private async Task<int> ForumIdAsync(IPage page, string name, int? parentId = null)
    {
        await page.GotoAsync(parentId.HasValue ? $"{this.TestSettings.TestForumUrl}forum/{parentId}/forum" : this.TestSettings.TestForumUrl);

        var link = await page.Locator("a[href*='/forum/']", new PageLocatorOptions { HasText = name }).First.GetAttributeAsync("href");

        Assert.That(link, Is.Not.Null, $"The forum {name} was not found");

        return int.Parse(Regex.Match(link, @"/forum/(\d+)/").Groups[1].Value);
    }
}
