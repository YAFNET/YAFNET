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
/// The tools of a post: mark as answer and hiding the posts of a user.
/// </summary>
[TestFixture]
public class PostActionsTests : ForumTestBase
{
    [Test]
    [Description("The author of the topic marks a reply as answer and removes the mark again")]
    public async Task Mark_As_Answer_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        var href = await this.CreateTopicAsync($"Question {Guid.NewGuid():N}");

        var topicId = Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;

        await this.LoginAsAdminAsync();
        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.Control("YafTextEditor").FillAsync("The answer of the admin.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync(href);

        await this.ToolAsync(post: 1, "Mark As Answer");
        await this.Page.Locator("[aria-label='ellipsis-h']").Nth(1).ClickAsync();
        (await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Remove As Answer" }).CountAsync())
            .Should().Be(1, "the reply is the answer");

        await this.GotoAsync(href);
        await this.ToolAsync(post: 1, "Remove As Answer");
        await this.Page.Locator("[aria-label='ellipsis-h']").Nth(1).ClickAsync();
        (await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Mark As Answer" }).CountAsync())
            .Should().Be(1);
    }

    [Test]
    [Description("A member hides the posts of another member and shows them again")]
    public async Task Hide_And_Show_User_Posts_Test()
    {
        var text = $"A post to hide {Guid.NewGuid():N}";

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        var href = await this.CreateTopicAsync($"Hidden {Guid.NewGuid():N}", text);

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync(href);

        await this.ToolAsync(post: 0, "Hide User Posts");
        (await this.Control("btnTogglePost").CountAsync()).Should().BeGreaterThan(0, "the post is collapsed");

        // the collapsed post is opened first
        await this.Control("btnTogglePost").First.ClickAsync();
        await this.ToolAsync(post: 0, "Show User Posts");
        (await this.Control("btnTogglePost").CountAsync()).Should().Be(0);
    }

    [Test]
    [Description("A second reply marked as answer replaces the first answer")]
    public async Task Second_Answer_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        var href = await this.CreateTopicAsync($"Two answers {Guid.NewGuid():N}");

        var topicId = Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;

        await this.LoginAsAdminAsync();

        foreach (var text in new[] { "The first answer.", "The second answer." })
        {
            await this.GotoAsync($"PostMessage?t={topicId}&f=1");
            await this.Control("YafTextEditor").FillAsync(text);
            await this.ClickAndWaitAsync(this.Control("PostReply"));
        }

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync(href);
        await this.ToolAsync(post: 1, "Mark As Answer");

        await this.GotoAsync(href);
        await this.ToolAsync(post: 2, "Mark As Answer");

        await this.GotoAsync(href);
        await this.Page.Locator("[aria-label='ellipsis-h']").Nth(1).ClickAsync();
        (await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Mark As Answer" }).CountAsync())
            .Should().Be(1, "the first reply is no answer anymore");
    }

    [Test]
    [Description("With the reputation a member votes for the author of a post (once), the admin votes against (no minimum points)")]
    public async Task Reputation_Test()
    {
        await this.LoginAsAdminAsync();
        await this.SetReputationAsync(true);

        try
        {
            await this.LoginAsUserAsync(TestConfig.TestUserName);
            var href = await this.CreateTopicAsync($"Reputation {Guid.NewGuid():N}");

            await this.LoginAsUserAsync(TestConfig.TestUserName2);
            await this.GotoAsync(href);

            await this.ClickAndWaitAsync(this.Page.Locator("[id$='_AddReputation']").First);
            (await this.Page.ContentAsync()).Should().Contain("toast-body");

            await this.GotoAsync(href);
            (await this.Page.Locator("[id$='_AddReputation']").CountAsync()).Should().Be(0, "the member voted");

            await this.LoginAsAdminAsync();
            await this.GotoAsync(href);

            // the admin may vote down (no minimum of points), the author has points now
            await this.ClickAndWaitAsync(this.Page.Locator("[id$='_RemoveReputation']").First);
            (await this.Page.ContentAsync()).Should().Contain("toast-body");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetReputationAsync(false);
        }
    }

    [Test]
    [Description("A member asks the author of a post to be friends with the menu of the post, the friend is removed with the menu of a post")]
    public async Task Friend_Request_From_Post_Test()
    {
        var author = $"Fra{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(author, TestConfig.TestUserPassword);
        await this.LoginAsync(author, TestConfig.TestUserPassword);
        var href = await this.CreateTopicAsync($"Friend {Guid.NewGuid():N}");

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync(href);
        await this.ToolAsync(post: 0, "Add as Friend");

        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        // the member replies, the author approves the request
        var topicId = Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;
        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.Control("YafTextEditor").FillAsync("The reply of the friend.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.LoginAsync(author, TestConfig.TestUserPassword);
        await this.GotoAsync("Friends");
        await this.Page.Locator("a.nav-link[href='#PendingRequestsTab']").ClickAsync();
        await this.ClickAndWaitAsync(
            this.Page.Locator("#PendingRequestsTab a", new PageLocatorOptions { HasTextString = "Approve" }).First);

        // and removes the friend with the menu of the post
        await this.GotoAsync(href);
        await this.Page.Locator("[aria-label='ellipsis-h']").Nth(1).ClickAsync();
        await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Remove Friend" }).First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        await this.AssertNoServerErrorAsync();

        await this.GotoAsync(href);
        await this.Page.Locator("[aria-label='ellipsis-h']").Nth(1).ClickAsync();
        (await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Add as Friend" }).CountAsync())
            .Should().Be(1, "the friend is removed");
    }

    private async Task SetReputationAsync(bool enabled)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Control("EnableUserReputation").EvaluateAsync("(e, v) => e.checked = v", enabled);
        await this.Control("ReputationMinDownVoting").EvaluateAsync("(e, v) => e.value = v", enabled ? "0" : "100");
        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    private async Task<string> CreateTopicAsync(string subject, string text = "The first post.")
    {
        await this.GotoAsync("PostTopic?f=1");

        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync(text);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Topics/1-Test Forum");

        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");

        return href.TrimStart('/');
    }

    /// <summary>
    /// Clicks an item of the tools menu of a post.
    /// </summary>
    private async Task ToolAsync(int post, string text)
    {
        await this.Page.Locator("[aria-label='ellipsis-h']").Nth(post).ClickAsync();

        await this.ClickAndWaitAsync(
            this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = text }).First);
    }
}
