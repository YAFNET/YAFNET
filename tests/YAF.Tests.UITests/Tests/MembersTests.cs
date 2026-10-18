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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The filters and the sorting of the members list.
/// </summary>
[TestFixture]
public class MembersTests : ForumTestBase
{
    [Test]
    [Description("The number of posts is filtered with the selected comparison (equal, less or equal, more or equal), the rank filter")]
    public async Task Posts_Filter_Test()
    {
        var member = $"Mem{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        // the admin has a post (the posts of the test forum are not counted)
        var forum = $"Counted {Guid.NewGuid():N}".Substring(0, 16);
        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(forum);
        await this.Control("Description").FillAsync("The posts are counted");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync("3");
        await this.ClickAndWaitAsync(this.Control("Save"));

        var forumId = ForumId(forum);

        await this.GotoAsync($"PostTopic?f={forumId}");
        await this.Control("TopicSubjectTextBox").FillAsync($"Members {Guid.NewGuid():N}");
        await this.Control("YafTextEditor").FillAsync("A post of the admin.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        // equal 0: the new member without posts, not the admin
        await this.SearchAsync(posts: "0", comparison: "1");
        var names = await this.NamesAsync();
        names.Should().Contain(member).And.NotContain(TestConfig.AdminUserName);

        // more or equal 1: the admin, not the new member
        await this.SearchAsync(posts: "1", comparison: "3");
        names = await this.NamesAsync();
        names.Should().Contain(TestConfig.AdminUserName).And.NotContain(member);

        // the selected comparison is kept
        (await this.Control("NumPostDDL").InputValueAsync()).Should().Be("3");

        // less or equal 0: the new member, not the admin
        await this.SearchAsync(posts: "0", comparison: "2");
        names = await this.NamesAsync();
        names.Should().Contain(member).And.NotContain(TestConfig.AdminUserName);

        // the rank of the new members
        await this.SearchAsync(posts: string.Empty, comparison: "1", rank: "Newbie");
        names = await this.NamesAsync();
        names.Should().Contain(member);
    }

    [Test]
    [Description("A number of posts that is no number shows a message (no error)")]
    public async Task Invalid_Posts_Value_Test()
    {
        await this.LoginAsAdminAsync();
        await this.GotoAsync("Members");

        await this.OpenFilterAsync();
        await this.Control("NumPostsTB").EvaluateAsync("e => e.type = 'text'");
        await this.Control("NumPostsTB").FillAsync("99999999999");
        await this.ClickAndWaitAsync(this.Control("SearchByUserName"));

        (await this.Page.ContentAsync()).Should().Contain("toast-body");
    }

    [Test]
    [Description("The members are found by a part of the name, sorted by the join date and filtered by role")]
    public async Task Name_Sort_And_Role_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var first = $"Srt{id}A";
        var second = $"Srt{id}B";

        await this.RegisterAsync(first, TestConfig.TestUserPassword);
        await this.RegisterAsync(second, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        await this.GotoAsync("Members");
        await this.OpenFilterAsync();
        await this.Control("UserSearchName").FillAsync($"Srt{id}");
        await this.ClickAndWaitAsync(this.Control("SearchByUserName"));

        (await this.NamesAsync()).Should().Equal(first, second);

        // the newest first
        await this.Control("Sort").ClickAsync();
        await this.ClickAndWaitAsync(this.Control("SortJoinedDesc"));
        (await this.NamesAsync()).Should().Equal(second, first);

        // the most posts first
        await this.Control("Sort").ClickAsync();
        await this.ClickAndWaitAsync(this.Control("SortPostsDesc"));
        (await this.NamesAsync()).Should().HaveCount(2);

        // the administrators
        await this.GotoAsync("Members");
        await this.OpenFilterAsync();
        await this.Control("Group").SelectOptionAsync(
            new SelectOptionValue { Label = "Administrators" },
            new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("SearchByUserName"));

        (await this.NamesAsync()).Should().Contain(TestConfig.AdminUserName).And.NotContain(first);
    }

    [Test]
    [Description("The other sort orders of the members (name, rank, join date, posts, last visit) keep the search, reset clears it")]
    public async Task Sort_Orders_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var first = $"Ord{id}A";
        var second = $"Ord{id}B";

        await this.RegisterAsync(first, TestConfig.TestUserPassword);
        await this.RegisterAsync(second, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        await this.GotoAsync("Members");
        await this.OpenFilterAsync();
        await this.Control("UserSearchName").FillAsync($"Ord{id}");
        await this.ClickAndWaitAsync(this.Control("SearchByUserName"));

        var expected = new Dictionary<string, string[]>
                           {
                               ["SortUserNameDesc"] = [second, first],
                               ["SortUserNameAsc"] = [first, second],
                               ["SortJoinedAsc"] = [first, second],
                               ["SortRankAsc"] = null,
                               ["SortRankDesc"] = null,
                               ["SortPostsAsc"] = null,
                               ["SortLastVisitAsc"] = null,
                               ["SortLastVisitDesc"] = null
                           };

        foreach (var sort in expected)
        {
            await this.Control("Sort").ClickAsync();
            await this.ClickAndWaitAsync(this.Control(sort.Key));

            var names = await this.NamesAsync();

            if (sort.Value == null)
            {
                names.Should().HaveCount(2, sort.Key);
            }
            else
            {
                names.Should().Equal(sort.Value, sort.Key);
            }
        }

        // reset
        await this.OpenFilterAsync();
        await this.ClickAndWaitAsync(this.Control("ResetUserSearch"));
        (await this.NamesAsync()).Should().HaveCountGreaterThan(2);
    }

    private async Task SearchAsync(string posts, string comparison, string rank = null)
    {
        await this.GotoAsync("Members");

        // all members on one page
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("50", new LocatorSelectOptionOptions { Force = true }));

        await this.OpenFilterAsync();
        await this.Control("NumPostDDL").SelectOptionAsync(comparison, new LocatorSelectOptionOptions { Force = true });
        await this.Control("NumPostsTB").FillAsync(posts);

        if (rank != null)
        {
            await this.Control("Ranks").SelectOptionAsync(new SelectOptionValue { Label = rank }, new LocatorSelectOptionOptions { Force = true });
        }

        await this.ClickAndWaitAsync(this.Control("SearchByUserName"));
    }

    /// <summary>
    /// Opens the drop down with the filters.
    /// </summary>
    private async Task OpenFilterAsync()
    {
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
    }

    private async Task<string[]> NamesAsync()
    {
        var names = await this.Page.Locator("[id$='_UserProfileLink']").AllInnerTextsAsync();

        return [.. names.Select(n => n.Trim())];
    }
}
