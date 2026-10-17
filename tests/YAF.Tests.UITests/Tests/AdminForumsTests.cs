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

namespace YAF.Tests.UITests.Tests;

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Admin > Forums: a category with forums is not deleted, a forum is copied and deleted (background task), then
/// the empty category is deleted.
/// </summary>
[TestFixture]
public class AdminForumsTests : ForumTestBase
{
    [Test]
    [Description("A category with a forum can not be deleted, the forum is deleted (background task), then the empty category is deleted")]
    public async Task Category_And_Forum_Delete_Test()
    {
        var category = $"DelCat {Guid.NewGuid():N}".Substring(0, 15);
        var forum = $"DelForum {Guid.NewGuid():N}".Substring(0, 17);

        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/EditCategory");
        await this.Control("Name").FillAsync(category);
        await this.Control("SortOrder").FillAsync("20");
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(forum);
        await this.Control("Description").FillAsync("A forum to delete");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = category });
        await this.Control("SortOrder").FillAsync("1");
        await this.ClickAndWaitAsync(this.Control("Save"));

        // the category with the forum has no delete button
        await this.GotoAsync("Admin/Forums");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("50", new LocatorSelectOptionOptions { Force = true }));
        var categoryRow = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = category }).First;
        (await categoryRow.Locator("[id*='_CategoryList_'][id$='_ThemeButtonDelete']").CountAsync()).Should().Be(0);

        // the forum is deleted by the background task, the page goes back to the list when it is done
        await this.GotoAsync(string.Empty);
        var forumHref = await this.Page.Locator("a", new PageLocatorOptions { HasTextString = forum }).First.GetAttributeAsync("href");
        var forumId = Regex.Match(forumHref, @"Topics/(\d+)").Groups[1].Value;

        await this.GotoAsync($"Admin/DeleteForum?fa={forumId}");
        await this.Control("Delete").ClickAsync();
        await this.Page.Locator(".bootbox .btn-success").ClickAsync();
        await this.Page.WaitForURLAsync(url => url.Contains("Admin/Forums"), new PageWaitForURLOptions { Timeout = 60000 });

        await this.GotoAsync(string.Empty);
        (await this.Page.InnerTextAsync("body")).Should().NotContain(forum);

        // the empty category
        await this.DeleteCategoryAsync(category);
        (await this.Page.InnerTextAsync("body")).Should().NotContain(category);
    }

    [Test]
    [Description("Sorting the categories by name sorts all categories of the board, not only the ones of the current page")]
    public async Task Sort_All_Categories_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var first = $"AAA sort {id}";

        await this.LoginAsAdminAsync();

        // more entries than one page (a category with forums), the category of the test is the last one
        var other = $"MMM sort {id}";
        await this.CreateCategoryAsync(other, "90");

        for (var i = 1; i <= 5; i++)
        {
            await this.GotoAsync("Admin/EditForum");
            await this.Control("Name").FillAsync($"Sort forum {id} {i}");
            await this.Control("Description").FillAsync("A forum to sort");
            await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = other });
            await this.Control("SortOrder").FillAsync(i.ToString());
            await this.ClickAndWaitAsync(this.Control("Save"));
        }

        await this.CreateCategoryAsync(first, "99");

        // five entries per page: the new category is not on the first page
        await this.GotoAsync("Admin/Forums");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("5", new LocatorSelectOptionOptions { Force = true }));
        (await this.Page.InnerTextAsync("body")).Should().NotContain(first);

        await this.Page.Locator("a.dropdown-toggle[aria-label='tools']").ClickAsync();
        await this.Control("SortCategoriesAsc").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.Page.Locator("a.dropdown-toggle[aria-label='tools']").ClickAsync();
        await this.Control("SortForumsDesc").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        // the first category by name is on the first page now
        await this.GotoAsync("Admin/Forums");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("5", new LocatorSelectOptionOptions { Force = true }));
        (await this.Page.InnerTextAsync("body")).Should().Contain(first);
    }

    [Test]
    [Description("A too big sort order is refused, a forum can't be its own parent or the parent of its parent, a moderated forum with a post count, cancel")]
    public async Task Edit_Forum_Checks_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var parent = $"Parent {id}";
        var child = $"Child {id}";

        await this.LoginAsAdminAsync();

        // a too big sort order of a category and of a forum
        await this.GotoAsync("Admin/EditCategory");
        await this.Control("Name").FillAsync($"Big sort {id}");
        await this.Control("SortOrder").FillAsync("99999");
        await this.ClickAndWaitAsync(this.Control("Save"));
        this.Page.Url.Should().Contain("EditCategory");

        await this.GotoAsync("Admin/EditForum");
        await this.FillForumAsync(parent, "99999");
        await this.ClickAndWaitAsync(this.Control("Save"));
        this.Page.Url.Should().Contain("EditForum");

        // the parent and its child, moderated after 3 posts
        await this.GotoAsync("Admin/EditForum");
        await this.FillForumAsync(parent, "60");
        await this.Page.RunAndWaitForNavigationAsync(() => this.Control("Moderated").CheckAsync());
        await this.Page.RunAndWaitForNavigationAsync(() => this.Control("ModerateAllPosts").UncheckAsync());
        await this.Control("ModeratedPostCount").FillAsync("3");
        await this.ClickAndWaitAsync(this.Control("Save"));

        var parentId = await this.ForumIdAsync(parent);

        await this.GotoAsync("Admin/EditForum");
        await this.FillForumAsync(child, "61");
        await this.Control("ParentList").SelectOptionAsync(parentId, new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("Save"));

        var childId = await this.ForumIdAsync(child);

        // the post count of the moderated forum is shown
        await this.GotoAsync($"Admin/EditForum?fa={parentId}");
        (await this.Control("ModeratedPostCount").InputValueAsync()).Should().Be("3");

        // its own parent and the parent of its parent (when the list offers them)
        foreach (var newParent in new[] { parentId, childId })
        {
            await this.GotoAsync($"Admin/EditForum?fa={parentId}");

            if (await this.Control("ParentList").Locator($"option[value='{newParent}']").CountAsync() == 0)
            {
                continue;
            }

            await this.Control("ParentList").SelectOptionAsync(newParent, new LocatorSelectOptionOptions { Force = true });
            await this.ClickAndWaitAsync(this.Control("Save"));
            this.Page.Url.Should().Contain("EditForum", newParent);
        }

        await this.GotoAsync($"Admin/EditForum?fa={parentId}");
        await this.ClickAndWaitAsync(this.Control("Cancel"));
        this.Page.Url.Should().Contain("Admin/Forums");
    }

    private async Task FillForumAsync(string name, string sortOrder)
    {
        await this.Control("Name").FillAsync(name);
        await this.Control("Description").FillAsync("A forum of the edit checks");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync(sortOrder);
    }

    private async Task<string> ForumIdAsync(string name)
    {
        await this.GotoAsync(string.Empty);

        var href = await this.Page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasTextString = name }).First
                       .GetAttributeAsync("href");

        return Regex.Match(href, @"/Topics/(\d+)").Groups[1].Value;
    }

    [Test]
    [Description("The buttons of the list open the edit pages (new, edit, copy, delete), the other sort orders")]
    public async Task List_Buttons_Test()
    {
        await this.LoginAsAdminAsync();

        foreach (var (button, page) in new[]
                     {
                         ("[id$='_NewCategory']", "EditCategory"), ("[id$='_NewForum']", "EditForum"),
                         ("[id*='_CategoryList_'][id$='_ThemeButtonEdit']", "EditCategory?c="),
                         ("[id$='_btnEdit']", "EditForum?fa="), ("[id$='_btnDuplicate']", "EditForum?copy="),
                         ("[id$='_btnDelete']", "DeleteForum?fa=")
                     })
        {
            await this.GotoAsync("Admin/Forums");
            await this.ClickAndWaitAsync(
                this.Page.Locator(button).Filter(new LocatorFilterOptions { Visible = true }).First);
            this.Page.Url.Should().Contain(page, button);
        }

        // sort the categories descending and the forums ascending (and back)
        foreach (var sort in new[] { "SortCategoriesDesc", "SortForumsAsc", "SortCategoriesAsc" })
        {
            await this.GotoAsync("Admin/Forums");
            await this.Page.Locator("a.dropdown-toggle[aria-label='tools']").ClickAsync();
            await this.Control(sort).ClickAsync();
            await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        }
    }

    private async Task CreateCategoryAsync(string name, string sortOrder)
    {
        await this.GotoAsync("Admin/EditCategory");
        await this.Control("Name").FillAsync(name);
        await this.Control("SortOrder").FillAsync(sortOrder);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    private async Task DeleteCategoryAsync(string category)
    {
        await this.GotoAsync("Admin/Forums");

        // all categories on one page
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("50", new LocatorSelectOptionOptions { Force = true }));

        var row = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = category }).First;

        await row.Locator("[id$='_ThemeButtonDelete']").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
    }
}
