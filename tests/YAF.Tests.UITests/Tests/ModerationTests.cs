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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Reported posts, moderated forums and the search.
/// </summary>
[TestFixture]
public class ModerationTests : ForumTestBase
{
    [Test]
    [Description("A member reports a post, the report is shown to the moderators")]
    public async Task Report_Post_Test()
    {
        var reason = $"Report {Guid.NewGuid():N}";

        await this.LoginAsUserAsync(TestConfig.TestUserName2);

        // the first post of the forum (by the admin)
        await this.GotoAsync("ReportPost?m=1");
        await this.Control("Report").FillAsync(reason);

        // the report is confirmed in a dialog
        await this.Control("btnReport").ClickAsync();
        await this.ClickAndWaitAsync(
            this.Page.Locator(".modal.show button", new PageLocatorOptions { HasTextString = "Yes" }));

        await this.LoginAsAdminAsync();
        await this.GotoAsync("Moderate/ReportedPosts?f=1");

        (await this.Page.InnerTextAsync("body")).Should().Contain(reason);
    }

    [Test]
    [Description("A report needs a reason of at most 128 characters, cancel goes back to the post, guests can't report")]
    public async Task Report_Checks_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName2);

        // no reason and a too long reason (the browser checks are removed)
        foreach (var reason in new[] { " ", new string('r', 129) })
        {
            await this.GotoAsync("ReportPost?m=1");
            await this.Page.EvaluateAsync("() => document.querySelectorAll('[required],[maxlength]').forEach(e => { e.removeAttribute('required'); e.removeAttribute('maxlength'); })");
            await this.Control("Report").FillAsync(reason);
            await this.Control("btnReport").ClickAsync();
            await this.ClickAndWaitAsync(
                this.Page.Locator(".modal.show button", new PageLocatorOptions { HasTextString = "Yes" }));

            this.Page.Url.Should().Contain("ReportPost", reason.Length.ToString());
        }

        // cancel
        await this.GotoAsync("ReportPost?m=1");
        await this.ClickAndWaitAsync(this.Control("btnCancel"));
        this.Page.Url.Should().Contain("Posts");

        // guests
        await this.LogoutAsync();
        await this.GotoAsync("ReportPost?m=1");
        this.Page.Url.Should().NotContain("ReportPost");
    }

    [Test]
    [Description("The moderator copies over the changed text of a reported post, views it and its history, resolves it and deletes another one")]
    public async Task Reported_Posts_Commands_Test()
    {
        var id = Guid.NewGuid().ToString("N");
        var subject = $"Reported topic {id}";

        // a member posts a topic with a reply
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync($"First reported {id}");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var firstId = (await this.MessageIdsAsync()).Min();
        await this.GotoAsync("Topics/1-Test Forum");
        var topicHref = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                            .GetAttributeAsync("href");
        var topicId = Regex.Match(topicHref, @"Posts/t(\d+)").Groups[1].Value;

        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.Control("YafTextEditor").FillAsync($"Second reported {id}");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        var secondId = (await this.MessageIdsAsync()).Max();

        // another member reports both posts
        await this.LoginAsUserAsync(TestConfig.TestUserName2);

        foreach (var messageId in new[] { firstId, secondId })
        {
            await this.GotoAsync($"ReportPost?m={messageId}");
            await this.Control("Report").FillAsync($"Report {id}");
            await this.Control("btnReport").ClickAsync();
            await this.ClickAndWaitAsync(
                this.Page.Locator(".modal.show button", new PageLocatorOptions { HasTextString = "Yes" }));
        }

        // the first post is changed after the report
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync($"EditMessage?m={firstId}");
        await this.Control("YafTextEditor").FillAsync($"First edited {id}");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.LoginAsAdminAsync();
        await this.GotoAsync("Moderate/ReportedPosts?f=1");

        ILocator Report(string text) =>
            this.Page.Locator(".card:has(> .card-footer)", new PageLocatorOptions { HasTextString = text });

        // the history of the changed post
        await this.ClickAndWaitAsync(Report($"First edited {id}").Locator("[id$='_ViewHistoryBtn']"));
        this.Page.Url.Should().Contain("MessageHistory");

        // the changed text is copied over (the post is not shown as changed any more)
        await this.GotoAsync("Moderate/ReportedPosts?f=1");
        await this.ClickAndWaitAsync(Report($"First edited {id}").Locator("[id$='_CopyOverBtn']"));
        (await Report($"First edited {id}").Locator("[id$='_ViewHistoryBtn']").CountAsync()).Should().Be(0);

        await this.ClickAndWaitAsync(Report($"First edited {id}").Locator("[id$='_ViewBtn']"));
        (await this.Page.InnerTextAsync("body")).Should().Contain($"First edited {id}");

        // resolved
        await this.GotoAsync("Moderate/ReportedPosts?f=1");
        await this.ClickAndWaitAsync(Report($"First edited {id}").Locator("[id$='_ResolveBtn']"));
        await this.AssertNoServerErrorAsync();
        (await Report($"First edited {id}").CountAsync()).Should().Be(0, "the resolved report is not listed");

        // the other post is deleted
        await this.GotoAsync("Moderate/ReportedPosts?f=1");
        await Report($"Second reported {id}").Locator("[id$='_DeleteBtn']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        await this.AssertNoServerErrorAsync();

        await this.GotoAsync($"Posts?t={topicId}");
        (await this.Page.InnerTextAsync("body")).Should().Contain($"First edited {id}").And.NotContain($"Second reported {id}");
    }

    [Test]
    [Description("A post in a moderated forum is only shown after a moderator approved it (the watching members are notified then), another one is deleted")]
    public async Task Moderated_Forum_Test()
    {
        var forumName = $"Moderated {Guid.NewGuid():N}".Substring(0, 22);
        var subject = $"Moderated topic {Guid.NewGuid():N}";
        var deleted = $"Deleted topic {Guid.NewGuid():N}";
        var watcher = $"Mwt{DateTime.UtcNow:HHmmssfff}";

        // a moderated forum (all posts need the approval)
        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(forumName);
        await this.Control("Description").FillAsync("A moderated forum created by the UI test");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync("5");

        // the option posts the page back
        await this.Page.RunAndWaitForNavigationAsync(() => this.Control("Moderated").CheckAsync());
        await this.ClickAndWaitAsync(this.Control("Save"));

        var forumId = ForumId(forumName);

        // a member watches the forum
        await this.RegisterAsync(watcher, TestConfig.TestUserPassword);
        await this.LoginAsync(watcher, TestConfig.TestUserPassword);
        await this.GotoAsync($"Topics/{forumId}-forum");
        await this.ClickAndWaitAsync(this.Control("WatchForum").Filter(new LocatorFilterOptions { Visible = true }).First);

        // the notifications of the watched topics and forums (new members get none)
        await this.GotoAsync("Profile/Subscriptions");
        await this.Page.RunAndWaitForResponseAsync(
            () => this.Control("rblNotificationType").Locator("input[value='0']").CheckAsync(),
            response => response.Request.Method == "POST");
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await this.Page.RunAndWaitForResponseAsync(
            () => this.Control("SaveUser").ClickAsync(),
            response => response.Request.Method == "POST");
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // a member posts a topic
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync($"PostTopic?f={forumId}");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("This topic waits for the approval.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        // other members don't see it
        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync($"Topics/{forumId}-forum");
        (await this.Page.InnerTextAsync("body")).Should().NotContain(subject);

        // the moderator approves it
        await this.LoginAsAdminAsync();
        await this.GotoAsync($"Moderate/UnapprovedPosts?f={forumId}");
        (await this.Page.InnerTextAsync("body")).Should().Contain("This topic waits for the approval.");

        var started = DateTime.UtcNow;

        // the last post to approve (the page goes back to the moderation index)
        await this.ClickAndWaitAsync(this.Control("ApproveBtn"));

        var mail = await TestMail.WaitForAsync(EmailOf(watcher), started);
        mail.Body.Should().Contain(subject, "the watching member is notified");

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync($"Topics/{forumId}-forum");
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);

        // another post is deleted
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync($"PostTopic?f={forumId}");
        await this.Control("TopicSubjectTextBox").FillAsync(deleted);
        await this.Control("YafTextEditor").FillAsync("This topic is deleted by the moderator.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.LoginAsAdminAsync();
        await this.GotoAsync($"Moderate/UnapprovedPosts?f={forumId}");
        await this.Control("DeleteBtn").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        await this.AssertNoServerErrorAsync();

        await this.GotoAsync($"Topics/{forumId}-forum");
        (await this.Page.InnerTextAsync("body")).Should().NotContain(deleted);
    }

    [Test]
    [Description("A new post is found by the search")]
    public async Task Search_Test()
    {
        var word = $"searchword{Guid.NewGuid():N}".Substring(0, 24);

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync($"Search topic {word}");
        await this.Control("YafTextEditor").FillAsync($"The text contains the word {word}.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Search");
        await this.Control("searchInput").FillAsync(word);

        // the results are loaded with script
        await this.Control("GoSearch").ClickAsync();

        await this.Page.Locator("body", new PageLocatorOptions { HasTextString = $"Search topic {word}" })
            .WaitForAsync(new LocatorWaitForOptions { Timeout = 30000 });

        await this.AssertNoServerErrorAsync();
    }

    private async Task<int[]> MessageIdsAsync()
    {
        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");

        return [.. ids.Select(i => Regex.Match(i, @"^post(\d+)$")).Where(m => m.Success).Select(m => int.Parse(m.Groups[1].Value))];
    }
}
