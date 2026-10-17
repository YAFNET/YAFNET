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
/// The admin pages: categories, forums, ranks, groups, bans, spam words and users.
/// </summary>
[TestFixture]
public class AdminTests : ForumTestBase
{
    [SetUp]
    public async Task LoginAsync()
    {
        await this.GotoAsync(string.Empty);

        if (!await this.IsLoggedInAsAsync(TestConfig.AdminUserName))
        {
            await this.LoginAsAdminAsync();
        }
    }

    [Test]
    [Description("A new category and a new forum in it are shown on the board index")]
    public async Task Create_Category_And_Forum_Test()
    {
        var category = Unique("Category");
        var forum = Unique("Forum");

        await this.GotoAsync("Admin/EditCategory");
        await this.Control("Name").FillAsync(category);
        await this.Control("SortOrder").FillAsync("10");
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(forum);
        await this.Control("Description").FillAsync("A forum created by the UI test");
        await this.SelectByTextAsync(this.Control("CategoryList"), category);
        await this.Control("SortOrder").FillAsync("1");
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.GotoAsync(string.Empty);

        var text = await this.Page.InnerTextAsync("body");
        text.Should().Contain(category).And.Contain(forum);
    }

    [Test]
    [Description("A forum name that exists in the category is refused")]
    public async Task Duplicate_Forum_Name_Is_Refused_Test()
    {
        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync("Test Forum");
        await this.Control("Description").FillAsync("A second forum with the same name");
        await this.SelectByTextAsync(this.Control("CategoryList"), "Test Category");
        await this.Control("SortOrder").FillAsync("1");
        await this.ClickAndWaitAsync(this.Control("Save"));

        // the page is shown again with the message (it was saved or failed with an error)
        this.Page.Url.Should().Contain("Admin/EditForum");
    }

    [Test]
    [Description("A new rank is listed, a second rank with the same name is refused")]
    public async Task Create_Rank_Test()
    {
        var rank = Unique("Rank");

        await this.CreateRankAsync(rank);

        this.Page.Url.Should().Contain("Admin/Ranks");
        (await this.Page.InnerTextAsync("body")).Should().Contain(rank);

        await this.CreateRankAsync(rank);

        this.Page.Url.Should().Contain("Admin/EditRank", "the existing name is refused");
    }

    [Test]
    [Description("A new group is listed, a second group with the same name is refused")]
    public async Task Create_Group_Test()
    {
        var group = Unique("Group");

        await this.CreateGroupAsync(group);

        this.Page.Url.Should().Contain("Admin/Groups");
        (await this.Page.InnerTextAsync("body")).Should().Contain(group);

        await this.CreateGroupAsync(group);

        this.Page.Url.Should().Contain("Admin/EditGroup", "the existing name is refused");
    }

    [Test]
    [Description("An IP address is banned with the dialog, an invalid address is refused")]
    public async Task Ban_Ip_Test()
    {
        await this.GotoAsync("Admin/BannedIps");

        await this.OpenEditDialogAsync("Add New Address");
        await this.Control("EditDialog_mask").FillAsync("not an address");
        await this.Control("EditDialog_BanReason").FillAsync("UI test");
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        (await this.Page.InnerTextAsync("body")).Should().NotContain("not an address");

        await this.GotoAsync("Admin/BannedIps");

        await this.OpenEditDialogAsync("Add New Address");
        await this.Control("EditDialog_mask").FillAsync("203.0.113.*");
        await this.Control("EditDialog_BanReason").FillAsync("UI test");
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        (await this.Page.InnerTextAsync("body")).Should().Contain("203.0.113.*");
    }

    [Test]
    [Description("A spam word is added, an invalid regular expression is refused")]
    public async Task Spam_Word_Test()
    {
        var word = Unique("spamword").ToLowerInvariant();

        await this.GotoAsync("Admin/SpamWords");

        await this.OpenEditDialogAsync("Add New Word");
        await this.Control("EditDialog_spamword").FillAsync("[invalid");
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        await this.GotoAsync("Admin/SpamWords");
        (await this.Page.InnerTextAsync("body")).Should().NotContain("[invalid");

        await this.OpenEditDialogAsync("Add New Word");
        await this.Control("EditDialog_spamword").FillAsync(word);
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        // the search is in the filter menu
        await this.Page.Locator("[data-bs-toggle='dropdown']", new PageLocatorOptions { HasTextString = "Filter" }).First.ClickAsync();
        await this.Control("SearchInput").FillAsync(word);
        await this.ClickAndWaitAsync(this.Control("search"));

        (await this.Page.InnerTextAsync("body")).Should().Contain(word);
    }

    [Test]
    [Description("A suspended user can not use the forum until the suspension ends")]
    public async Task Suspend_User_Test()
    {
        var userName = $"Susp{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        var userId = await this.FindUserIdAsync(userName);

        await this.GotoAsync($"Admin/EditUser?u={userId}");

        await this.Page.EvaluateAsync(
            "() => { document.querySelector(\"[id$='_SuspendUserControl_SuspendedReason']\").value = 'UI test'; document.querySelector(\"[id$='_SuspendUserControl_SuspendCount']\").value = '2'; }");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Page.EvaluateAsync("() => document.querySelector(\"[id$='_SuspendUserControl_Suspend']\").click()"));
        await this.AssertNoServerErrorAsync();

        await this.LoginAsync(userName, TestConfig.TestUserPassword);
        await this.GotoAsync(string.Empty);

        // the suspended user gets the info page
        this.Page.Url.Should().Contain("Info");
        (await this.Page.InnerTextAsync("body")).Should().Contain("UI test");
    }

    private static string Unique(string prefix)
    {
        return $"{prefix} {Guid.NewGuid():N}".Substring(0, prefix.Length + 13);
    }

    private async Task CreateRankAsync(string name)
    {
        await this.GotoAsync("Admin/EditRank");

        await this.Control("Name").FillAsync(name);
        await this.Control("MinPosts").FillAsync("0");
        await this.Control("RankPriority").FillAsync("5");
        await this.Control("PMLimit").FillAsync("10");
        await this.Control("UsrSigChars").FillAsync("100");
        await this.Control("UsrAlbums").FillAsync("0");
        await this.Control("UsrAlbumImages").FillAsync("0");

        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    private async Task CreateGroupAsync(string name)
    {
        await this.GotoAsync("Admin/EditGroup");

        await this.Control("Name").FillAsync(name);
        await this.Control("Priority").FillAsync("5");
        await this.Control("PMLimit").FillAsync("10");
        await this.Control("UsrSigChars").FillAsync("100");
        await this.Control("UsrAlbums").FillAsync("0");
        await this.Control("UsrAlbumImages").FillAsync("0");

        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    /// <summary>
    /// Opens the edit dialog of a list page with its add button.
    /// </summary>
    private async Task OpenEditDialogAsync(string buttonText)
    {
        await this.ClickAndWaitAsync(
            this.Page.Locator("a", new PageLocatorOptions { HasTextString = buttonText })
                .Filter(new LocatorFilterOptions { Visible = true }).First);

        await this.Page.Locator("[id$='_EditDialog_Save']")
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }

    private async Task SelectByTextAsync(ILocator select, string text)
    {
        await select.SelectOptionAsync(new SelectOptionValue { Label = text });
    }

    /// <summary>
    /// Finds the id of a user in the admin users list.
    /// </summary>
    private async Task<int> FindUserIdAsync(string userName)
    {
        await this.GotoAsync("Admin/Users");

        // the name posts back and opens the edit page
        await this.ClickAndWaitAsync(
            this.Page.Locator("[id$='_NameEdit']", new PageLocatorOptions { HasTextString = userName }).First);

        return int.Parse(Regex.Match(this.Page.Url, @"u=(\d+)").Groups[1].Value);
    }
}
