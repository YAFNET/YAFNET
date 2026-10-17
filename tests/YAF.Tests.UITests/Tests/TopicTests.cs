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

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// New topics, replies, quick replies, editing and deleting posts.
/// </summary>
[TestFixture]
public class TopicTests : ForumTestBase
{
    [OneTimeSetUp]
    public async Task LoginAsync()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
    }

    [Test]
    [Description("A member creates a new topic, it is shown with its text and in the topic list of the forum")]
    public async Task New_Topic_Test()
    {
        var subject = $"New topic {Guid.NewGuid():N}";

        await this.CreateTopicAsync(subject, "This is the first post of the topic.");

        (await this.Page.TitleAsync()).Should().Contain(subject);
        (await this.Page.InnerTextAsync("body")).Should().Contain("This is the first post of the topic.");

        await this.GotoAsync("Topics/1-Test Forum");

        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);
    }

    [Test]
    [Description("A reply is added to the topic")]
    public async Task Reply_Test()
    {
        var topicId = await this.CreateTopicAsync($"Reply topic {Guid.NewGuid():N}", "The first post.");

        await this.GotoAsync($"PostMessage?t={topicId}&f=1");

        await this.Control("YafTextEditor").FillAsync("This is the reply.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        (await this.Page.InnerTextAsync("body")).Should().Contain("This is the reply.");
        (await this.GetMessageIdsAsync()).Should().HaveCount(2);
    }

    [Test]
    [Description("A quick reply is added to the topic")]
    public async Task Quick_Reply_Test()
    {
        await this.CreateTopicAsync($"Quick reply topic {Guid.NewGuid():N}", "The first post.");

        // the visible button of the post (the context menu has a hidden one)
        await this.Page.Locator("[id*='_QuickReplyLink']").Filter(new () { Visible = true }).First.ClickAsync();

        await this.Control("QuickReplyDialog_YafTextEditor").FillAsync("This is the quick reply.");
        await this.ClickAndWaitAsync(this.Control("QuickReplyDialog_Reply"));

        (await this.Page.InnerTextAsync("body")).Should().Contain("This is the quick reply.");
    }

    [Test]
    [Description("The author edits the own post")]
    public async Task Edit_Message_Test()
    {
        await this.CreateTopicAsync($"Edit topic {Guid.NewGuid():N}", "The text before the edit.");

        var messageId = (await this.GetMessageIdsAsync()).Last();

        await this.GotoAsync($"EditMessage?m={messageId}");

        await this.Control("YafTextEditor").FillAsync("The text after the edit.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var text = await this.Page.InnerTextAsync("body");

        text.Should().Contain("The text after the edit.").And.NotContain("The text before the edit.");
    }

    [Test]
    [Description("The author deletes a reply, it is not shown anymore")]
    public async Task Delete_Message_Test()
    {
        var topicId = await this.CreateTopicAsync($"Delete topic {Guid.NewGuid():N}", "The first post.");

        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.Control("YafTextEditor").FillAsync("This reply is deleted.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var replyId = (await this.GetMessageIdsAsync()).Last();

        await this.GotoAsync($"DeleteMessage?m={replyId}&action=delete");
        await this.Control("ReasonEditor").FillAsync("Deleted by the UI test");
        await this.ClickAndWaitAsync(this.Control("Delete"));

        await this.GotoAsync($"PostMessage?t={topicId}&f=1");

        // the last posts of the topic are shown below the reply editor
        (await this.Page.InnerTextAsync("body")).Should().NotContain("This reply is deleted.");
    }

    [Test]
    [Description("Html in the subject and the text of a post is shown as text")]
    public async Task Html_Is_Shown_As_Text_Test()
    {
        var marker = Guid.NewGuid().ToString("N").Substring(0, 8);

        await this.CreateTopicAsync(
            $"<b>Bold</b> {marker} <script>window.subjectXss = 1;</script>",
            "<img src=x onerror=\"window.messageXss = 1\"> <script>window.messageXss = 1;</script>",
            marker);

        (await this.Page.EvaluateAsync<bool>("() => window.subjectXss === undefined && window.messageXss === undefined"))
            .Should().BeTrue();

        (await this.Page.Locator("b", new () { HasTextString = "Bold" }).CountAsync()).Should().Be(0);

        (await this.Page.TitleAsync()).Should().Contain(marker);
    }

    /// <summary>
    /// Creates a topic in the test forum, the topic page is shown afterwards.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <param name="text">The text of the first post.</param>
    /// <param name="lookup">A part of the subject to find the topic in the list (the subject by default).</param>
    /// <returns>Returns the topic id.</returns>
    private async Task<int> CreateTopicAsync(string subject, string text, string lookup = null)
    {
        await this.GotoAsync("PostTopic?f=1");

        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync(text);

        await this.ClickAndWaitAsync(this.Control("PostReply"));

        // the new post is shown (Posts/m{message id})
        this.Page.Url.Should().MatchRegex(@"/Posts/[tm]\d+", "the topic is shown after the post");

        // the topic id is taken from the link of the topic list of the forum
        await this.GotoAsync("Topics/1-Test Forum");

        var link = this.Page.Locator("a[href*='/Posts/t']", new () { HasTextString = lookup ?? subject }).First;
        var href = await link.GetAttributeAsync("href");

        var topicId = int.Parse(Regex.Match(href, @"/Posts/t(\d+)").Groups[1].Value);

        // the address of the topic needs the name part
        await this.GotoAsync(href.TrimStart('/'));

        return topicId;
    }

    /// <summary>
    /// Gets the ids of the posts of the shown topic page.
    /// </summary>
    /// <returns>Returns the message ids.</returns>
    private async Task<int[]> GetMessageIdsAsync()
    {
        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");

        return [.. ids.Select(id => Regex.Match(id, @"^post(\d+)$")).Where(m => m.Success).Select(m => int.Parse(m.Groups[1].Value))];
    }
}
