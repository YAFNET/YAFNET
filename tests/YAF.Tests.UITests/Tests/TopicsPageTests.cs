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
/// The topic list of a forum: the search in the forum, mark as read, the time filter, watching the forum,
/// sub forums and a forum with a remote address.
/// </summary>
[TestFixture]
public class TopicsPageTests : ForumTestBase
{
    [Test]
    [Description("The search of the forum opens the search page, the forum is marked as read, the time filter keeps the list")]
    public async Task Search_Read_And_Time_Filter_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        // an empty search stays on the page
        await this.GotoAsync("Topics/1-Test Forum");
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("forumSearchOK").ClickAsync();
        this.Page.Url.Should().Contain("Topics/1");

        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("forumSearch").FillAsync("Hello");
        await this.ClickAndWaitAsync(this.Control("forumSearchOK"));
        this.Page.Url.Should().Contain("Search").And.Contain("forum=1");

        await this.GotoAsync("Topics/1-Test Forum");
        await this.ClickAndWaitAsync(this.Page.Locator(".btn-group[aria-label='Tools'] [id$='_MarkRead']"));

        // the topics of the last day
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("ShowList").SelectOptionAsync(new SelectOptionValue { Index = 1 }, new LocatorSelectOptionOptions { Force = true }));
        await this.AssertNoServerErrorAsync();
    }

    [Test]
    [Description("A member watches and unwatches the forum, a guest gets a message")]
    public async Task Watch_Forum_Test()
    {
        var member = $"Wtf{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Topics/1-Test Forum");
        await this.ClickAndWaitAsync(this.Control("WatchForum"));
        (await this.Control("WatchForum").InnerTextAsync()).Should().Contain("Unwatch");

        await this.ClickAndWaitAsync(this.Control("WatchForum"));
        (await this.Control("WatchForum").InnerTextAsync()).Should().NotContain("Unwatch");

        await this.LogoutAsync();
        await this.GotoAsync("Topics/1-Test Forum");

        if (await this.Control("WatchForum").IsVisibleAsync())
        {
            await this.ClickAndWaitAsync(this.Control("WatchForum"));
            (await this.Page.ContentAsync()).Should().Contain("toast-body");
        }
    }

    [Test]
    [Description("A sub forum is listed in its parent forum, a forum with a remote address opens the address")]
    public async Task Sub_Forum_And_Remote_Forum_Test()
    {
        var sub = $"Sub {Guid.NewGuid():N}".Substring(0, 14);
        var remote = $"Remote {Guid.NewGuid():N}".Substring(0, 17);

        await this.LoginAsAdminAsync();

        await this.CreateForumAsync(sub, "Test Forum", null);
        await this.CreateForumAsync(remote, null, $"{TestConfig.BaseUrl}Members");

        await this.GotoAsync("Topics/1-Test Forum");
        (await this.Page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasTextString = sub }).CountAsync()).Should()
            .BeGreaterThan(0, "the sub forum is listed");

        // the remote forum
        await this.GotoAsync(string.Empty);
        var href = await this.Page.Locator("a", new PageLocatorOptions { HasTextString = remote }).First.GetAttributeAsync("href");

        var forumId = Regex.Match(href, @"Topics/(\d+)");

        if (forumId.Success)
        {
            await this.GotoAsync($"Topics/{forumId.Groups[1].Value}");
        }
        else
        {
            await this.GotoAsync(href);
        }

        this.Page.Url.Should().Contain("Members");
    }

    private async Task CreateForumAsync(string name, string parent, string remoteUrl)
    {
        await this.GotoAsync("Admin/EditForum");

        await this.Control("Name").FillAsync(name);
        await this.Control("Description").FillAsync("A forum of the UI test");
        await this.Control("CategoryList").SelectOptionAsync(
            new SelectOptionValue { Label = "Test Category" },
            new LocatorSelectOptionOptions { Force = true });
        await this.Control("SortOrder").FillAsync("50");

        if (parent != null)
        {
            var value = await this.Control("ParentList").EvaluateAsync<string>(
                "(e, text) => Array.from(e.options).find(o => o.text.includes(text)).value",
                parent);
            await this.Control("ParentList").SelectOptionAsync(value, new LocatorSelectOptionOptions { Force = true });
        }

        if (remoteUrl != null)
        {
            await this.Control("remoteurl").FillAsync(remoteUrl);
        }

        await this.ClickAndWaitAsync(this.Control("Save"));
    }
}
