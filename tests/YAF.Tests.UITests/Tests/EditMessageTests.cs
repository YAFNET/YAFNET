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
