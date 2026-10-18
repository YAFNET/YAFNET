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
/// The reply page: an empty reply, the preview, cancel, a quote, watching the topic, the flood delay and a reply in
/// a moderated forum.
/// </summary>
[TestFixture]
public class PostMessageTests : ForumTestBase
{
    [Test]
    [Description("An empty reply is refused, the preview shows the text, cancel goes back, a quoted message is in the editor, the topic is watched and unwatched")]
    public async Task Reply_Options_Test()
    {
        var subject = $"Reply {Guid.NewGuid():N}".Substring(0, 16);

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        var (topicId, messageId) = await this.CreateTopicAsync("1", subject);

        // empty
        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        this.Page.Url.Should().Contain("PostMessage");

        // preview
        await this.Control("YafTextEditor").FillAsync("The text of the preview.");
        await this.ClickAndWaitAsync(this.Control("Preview"));
        (await this.Page.Locator(".card-body .alert", new PageLocatorOptions { HasTextString = "The text of the preview." }).CountAsync())
            .Should().Be(1, "the preview shows the text");

        // cancel
        await this.ClickAndWaitAsync(this.Control("Cancel"));
        this.Page.Url.Should().Contain("Posts");

        // a quote
        await this.GotoAsync($"PostMessage?t={topicId}&f=1&q={messageId}");
        (await this.Control("YafTextEditor").InputValueAsync()).Should().Contain("[quote");

        // a reply that watches the topic, then one that doesn't
        foreach (var watch in new[] { true, false })
        {
            await this.GotoAsync($"PostMessage?t={topicId}&f=1");
            await this.Control("YafTextEditor").FillAsync($"A reply, watched: {watch}.");
            await this.Control("TopicWatch").SetCheckedAsync(watch);
            await this.ClickAndWaitAsync(this.Control("PostReply"));
            this.Page.Url.Should().Contain("Posts");
        }

        await this.Control("OptionsLink").ClickAsync();
        (await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Unwatch this topic" }).CountAsync())
            .Should().Be(0, "the last reply removed the watch");
    }

    [Test]
    [Description("A second reply within the flood delay is refused with the remaining seconds")]
    public async Task Flood_Delay_Test()
    {
        var subject = $"Flood {Guid.NewGuid():N}".Substring(0, 16);

        await this.LoginAsAdminAsync();
        var delay = await this.SetFloodDelayAsync("60");

        try
        {
            // a new member (the last post of another member could be within the delay)
            var member = $"Fld{DateTime.UtcNow:HHmmssfff}";
            await this.RegisterAsync(member, TestConfig.TestUserPassword);
            await this.LoginAsync(member, TestConfig.TestUserPassword);

            var (topicId, _) = await this.CreateTopicAsync("1", subject);

            await this.GotoAsync($"PostMessage?t={topicId}&f=1");
            await this.Control("YafTextEditor").FillAsync("A reply too fast.");
            await this.ClickAndWaitAsync(this.Control("PostReply"));

            this.Page.Url.Should().Contain("PostMessage", "the reply is refused");
            (await this.Page.ContentAsync()).Should().Contain("toast-body");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetFloodDelayAsync(delay);
        }
    }

    [Test]
    [Description("A reply of a member in a moderated forum waits for the approval (info page)")]
    public async Task Moderated_Reply_Test()
    {
        var forumName = $"ModReply {Guid.NewGuid():N}".Substring(0, 20);
        var subject = $"Moderated reply {Guid.NewGuid():N}".Substring(0, 26);

        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(forumName);
        await this.Control("Description").FillAsync("A moderated forum for replies");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync("6");
        await this.Page.RunAndWaitForNavigationAsync(() => this.Control("Moderated").CheckAsync());
        await this.ClickAndWaitAsync(this.Control("Save"));

        var forumId = ForumId(forumName);

        // the topic of the admin is approved
        var (topicId, _) = await this.CreateTopicAsync(forumId, subject);

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync($"PostMessage?t={topicId}&f={forumId}");
        await this.Control("YafTextEditor").FillAsync("This reply waits for the approval.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        this.Page.Url.Should().Contain("Info");
    }

    /// <summary>
    /// Creates a topic and returns the ids of the topic and of its first message.
    /// </summary>
    [Test]
    [Description("Members can't reply in a locked topic or a locked forum, a post of another topic can't be quoted")]
    public async Task Locked_And_Foreign_Quote_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");

        await this.LoginAsAdminAsync();

        // a locked topic
        var locked = await this.CreateTopicAsync("1", $"Locked reply {id}");
        await this.GotoAsync($"Posts?t={locked.TopicId}");
        await this.Control("Tools1").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Control("LockTopic1").First);

        var open = await this.CreateTopicAsync("1", $"Open reply {id}");

        // a locked forum (the admin can still post)
        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync($"Locked forum {id}");
        await this.Control("Description").FillAsync("A locked forum of the reply test");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync("87");
        await this.Control("Locked").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("Save"));

        var forumId = ForumId($"Locked forum {id}");
        var inLockedForum = await this.CreateTopicAsync(forumId, $"In locked forum {id}");

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        foreach (var url in new[]
                     {
                         $"PostMessage?t={locked.TopicId}&f=1", $"PostMessage?t={inLockedForum.TopicId}&f={forumId}",
                         $"PostMessage?t={open.TopicId}&f=1&q={locked.MessageId}"
                     })
        {
            await this.GotoAsync(url);
            await this.AssertNoServerErrorAsync();
            (await this.Control("PostReply").CountAsync()).Should().Be(0, url);
        }
    }

    private async Task<(string TopicId, string MessageId)> CreateTopicAsync(string forumId, string subject)
    {
        await this.GotoAsync($"PostTopic?f={forumId}");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("The first post of the topic.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");
        var messageId = ids.Select(id => Regex.Match(id, @"^post(\d+)$")).Where(m => m.Success).Select(m => m.Groups[1].Value)
            .First();

        // the id of the topic: the link of the topic list
        await this.GotoAsync($"Topics/{forumId}-forum");
        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");
        var topicId = Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;

        return (topicId, messageId);
    }

    /// <summary>
    /// Sets the flood delay of the host settings and returns the old value.
    /// </summary>
    private async Task<string> SetFloodDelayAsync(string value)
    {
        await this.GotoAsync("Admin/HostSettings");

        var old = await this.Control("PostFloodDelay").InputValueAsync();

        await this.Control("PostFloodDelay").EvaluateAsync("(e, v) => e.value = v", value);
        await this.ClickAndWaitAsync(this.Control("Save"));

        return old;
    }
}
