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
/// The history of a post: the old texts are listed, a moderator restores an old text.
/// </summary>
[TestFixture]
public class MessageHistoryTests : ForumTestBase
{
    [Test]
    [Description("The texts of an edited post are in the history, the first text is restored")]
    public async Task History_And_Restore_Test()
    {
        var first = $"First text {Guid.NewGuid():N}";
        var second = $"Second text {Guid.NewGuid():N}";
        var third = $"Third text {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync($"History {Guid.NewGuid():N}");
        await this.Control("YafTextEditor").FillAsync(first);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");
        var messageId = ids.Select(id => Regex.Match(id, @"^post(\d+)$")).Where(m => m.Success).Select(m => m.Groups[1].Value).First();

        foreach (var text in new[] { second, third })
        {
            await this.GotoAsync($"EditMessage?m={messageId}");
            await this.Control("YafTextEditor").FillAsync(text);
            await this.ClickAndWaitAsync(this.Control("PostReply"));
        }

        await this.GotoAsync($"MessageHistory?m={messageId}");
        var history = await this.Page.ContentAsync();
        history.Should().Contain(first).And.Contain(second).And.Contain(third);

        // the oldest text (the first restore button of the list)
        var restore = this.Page.Locator("[id$='_ThemeButtonEdit']");
        (await restore.CountAsync()).Should().BeGreaterThan(0);

        await restore.First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.GotoAsync($"Posts/m{messageId}-History");
        var post = await this.Page.InnerTextAsync("body");
        (post.Contains(first) || post.Contains(second)).Should().BeTrue("an old text is restored");
        post.Should().NotContain(third);
    }
}
