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
/// Editing a post: the checks of the input, the preview, the reason and cancel.
/// </summary>
[TestFixture]
public class EditMessageTests : ForumTestBase
{
    [Test]
    [Description("The text and the subject of the first post are required")]
    public async Task Input_Checks_Test()
    {
        await this.LoginAsAdminAsync();
        var messageId = await this.CreateTopicAsync($"Edit checks {Guid.NewGuid():N}");

        await this.GotoAsync($"EditMessage?m={messageId}");

        await this.Control("YafTextEditor").FillAsync(string.Empty);
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        (await this.Page.ContentAsync()).Should().Contain("Message body cannot be empty.");

        await this.Control("YafTextEditor").FillAsync("The text is back.");
        await this.Control("TopicSubjectTextBox").EvaluateAsync("e => e.removeAttribute('required')");
        await this.Control("TopicSubjectTextBox").FillAsync(string.Empty);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        this.Page.Url.Should().Contain("EditMessage", "the post is not saved");
    }

    [Test]
    [Description("The preview shows the changed text, the post is saved with a reason, cancel goes back to the topic")]
    public async Task Preview_Reason_And_Cancel_Test()
    {
        var text = $"Changed {Guid.NewGuid():N}";
        var reason = $"Typo {Guid.NewGuid():N}".Substring(0, 13);

        await this.LoginAsAdminAsync();
        var messageId = await this.CreateTopicAsync($"Edit preview {Guid.NewGuid():N}");

        await this.GotoAsync($"EditMessage?m={messageId}");

        await this.Control("YafTextEditor").FillAsync($"[b]{text}[/b]");
        await this.ClickAndWaitAsync(this.Control("Preview"));
        (await this.Page.Locator("strong", new PageLocatorOptions { HasTextString = text }).CountAsync()).Should().Be(1);

        await this.Control("ReasonEditor").FillAsync(reason);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var topic = await this.Page.InnerTextAsync("body");
        topic.Should().Contain(text);

        await this.GotoAsync($"EditMessage?m={messageId}");
        await this.ClickAndWaitAsync(this.Control("Cancel"));

        this.Page.Url.Should().Contain("/Posts/");
    }

    [Test]
    [Description("The tags of the topic are in the edit page, the subject of another topic is refused, the topic is watched and unwatched")]
    public async Task Tags_Duplicate_Subject_And_Watch_Test()
    {
        var first = $"First subject {Guid.NewGuid():N}".Substring(0, 24);
        var tag = $"uitag{DateTime.UtcNow:HHmmssfff}";

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.CreateTopicAsync(first);

        // a topic with a tag
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync($"Tagged {Guid.NewGuid():N}");
        await this.Control("YafTextEditor").FillAsync("The tagged post.");
        await this.Control("TagsValue").EvaluateAsync("(e, tag) => e.value = tag", tag);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");
        var messageId = ids.Select(id => Regex.Match(id, @"^post(\d+)$")).Where(m => m.Success).Select(m => m.Groups[1].Value).First();

        await this.GotoAsync($"EditMessage?m={messageId}");
        (await this.Control("TagsValue").InputValueAsync()).Should().Contain(tag);

        // the subject of the other topic
        await this.Control("TopicSubjectTextBox").FillAsync(first);
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        this.Page.Url.Should().Contain("EditMessage", "the subject exists");

        // watched, then not watched
        foreach (var watch in new[] { true, false })
        {
            await this.GotoAsync($"EditMessage?m={messageId}");
            await this.Control("TopicWatch").SetCheckedAsync(watch);
            await this.ClickAndWaitAsync(this.Control("PostReply"));
            this.Page.Url.Should().Contain("/Posts/");
        }
    }

    [Test]
    [Description("A member edits the own reply in the topic of another member: the subject can't be changed")]
    public async Task Reply_In_Topic_Of_Other_Member_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        var messageId = await this.CreateTopicAsync($"Topic of another {Guid.NewGuid():N}");

        var topicUrl = this.Page.Url;
        var topicId = Regex.Match(
            await this.Page.ContentAsync(),
            @"PostMessage\?t=(\d+)|[?&]t=(\d+)").Groups.Cast<Group>().Skip(1).First(g => g.Success).Value;

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.Control("YafTextEditor").FillAsync("The reply of the other member.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");
        var replyId = ids.Select(id => Regex.Match(id, @"^post(\d+)$")).Where(m => m.Success).Select(m => int.Parse(m.Groups[1].Value))
            .Max();

        replyId.Should().NotBe(messageId, topicUrl);

        await this.GotoAsync($"EditMessage?m={replyId}");
        (await this.Control("TopicSubjectTextBox").IsDisabledAsync()).Should().BeTrue();

        await this.Control("YafTextEditor").FillAsync("The changed reply of the other member.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        (await this.Page.InnerTextAsync("body")).Should().Contain("The changed reply of the other member.");
    }

    /// <summary>
    /// Creates a topic and returns the id of the first post.
    /// </summary>
    private async Task<int> CreateTopicAsync(string subject)
    {
        await this.GotoAsync("PostTopic?f=1");

        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("The first post.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");

        return ids.Select(id => Regex.Match(id, @"^post(\d+)$")).Where(m => m.Success).Select(m => int.Parse(m.Groups[1].Value)).First();
    }
}
