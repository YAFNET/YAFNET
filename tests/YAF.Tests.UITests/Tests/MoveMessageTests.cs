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
/// The move page of a post: move it to another topic or split it into a new topic.
/// </summary>
[TestFixture]
public class MoveMessageTests : ForumTestBase
{
    [Test]
    [Description("A post is moved to another topic (not to no topic or its own topic) and split into a new topic (a subject is needed)")]
    public async Task Move_And_Split_Test()
    {
        var id = Guid.NewGuid().ToString("N");
        var source = $"Move source {id}";
        var target = $"Move target {id}";
        var split = $"Move split {id}";

        await this.LoginAsAdminAsync();

        var sourceId = await this.CreateTopicAsync(source, "The first post of the source.");
        await this.GotoAsync($"PostMessage?t={sourceId}&f=1");
        await this.Control("YafTextEditor").FillAsync($"The moved post {id}.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        var messageId = (await this.MessageIdsAsync()).Max();

        var targetId = await this.CreateTopicAsync(target, "The first post of the target.");

        // no topic and the own topic are refused
        foreach (var topicId in new[] { "0", sourceId })
        {
            await this.GotoAsync($"MoveMessage?m={messageId}");
            await this.Control("TopicListSelected").EvaluateAsync("(e, v) => e.value = v", topicId);
            await this.ClickAndWaitAsync(this.Control("Move"));

            this.Page.Url.Should().Contain("MoveMessage", topicId);
        }

        // moved to the target topic
        await this.GotoAsync($"MoveMessage?m={messageId}");
        await this.Control("TopicListSelected").EvaluateAsync("(e, v) => e.value = v", targetId);
        await this.ClickAndWaitAsync(this.Control("Move"));
        await this.AssertNoServerErrorAsync();

        await this.GotoAsync($"Posts?t={targetId}");
        (await this.Page.InnerTextAsync("body")).Should().Contain($"The moved post {id}.");

        // split into a new topic: without a subject it is refused
        await this.GotoAsync($"MoveMessage?m={messageId}");
        await this.Control("ForumListSelected").EvaluateAsync("e => e.value = '1'");
        await this.ClickAndWaitAsync(this.Control("CreateAndMove"));
        this.Page.Url.Should().Contain("MoveMessage");

        await this.Control("ForumListSelected").EvaluateAsync("e => e.value = '1'");
        await this.Control("TopicSubject").FillAsync(split);
        await this.ClickAndWaitAsync(this.Control("CreateAndMove"));
        await this.AssertNoServerErrorAsync();

        var splitId = await this.TopicIdAsync(split);
        await this.GotoAsync($"Posts?t={splitId}");
        (await this.Page.InnerTextAsync("body")).Should().Contain($"The moved post {id}.");

        await this.GotoAsync($"Posts?t={targetId}");
        (await this.Page.InnerTextAsync("body")).Should().NotContain($"The moved post {id}.");
    }

    private async Task<string> CreateTopicAsync(string subject, string text)
    {
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync(text);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        return await this.TopicIdAsync(subject);
    }

    private async Task<string> TopicIdAsync(string subject)
    {
        await this.GotoAsync("Topics/1-Test Forum");

        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject })
                       .First.GetAttributeAsync("href");

        return Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;
    }

    private async Task<int[]> MessageIdsAsync()
    {
        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");

        return [.. ids.Select(i => Regex.Match(i, @"^post(\d+)$")).Where(m => m.Success).Select(m => int.Parse(m.Groups[1].Value))];
    }
}
