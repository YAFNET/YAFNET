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
/// Admin > Restore: deleted posts and topics are restored or deleted for good.
/// </summary>
[TestFixture]
public class RestoreTests : ForumTestBase
{
    [Test]
    [Description("A deleted reply is restored and shown in the topic again")]
    public async Task Restore_Message_Test()
    {
        var text = $"Restored reply {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();

        var topicId = await this.CreateTopicAsync($"Restore {Guid.NewGuid():N}");

        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.Control("YafTextEditor").FillAsync(text);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var replyId = (await this.GetMessageIdsAsync()).Last();
        await this.DeleteAsync(replyId);

        await this.GotoAsync("Admin/Restore");
        var row = this.Page.Locator("li", new PageLocatorOptions { HasTextString = text }).Last;
        await this.ClickAndWaitAsync(row.Locator("[id$='_Restore']"));

        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        (await this.Page.InnerTextAsync("body")).Should().Contain(text);
    }

    [Test]
    [Description("A deleted topic is restored, another one is deleted for good")]
    public async Task Restore_And_Delete_Topic_Test()
    {
        var restored = $"Restored topic {Guid.NewGuid():N}";
        var deleted = $"Deleted topic {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();

        foreach (var subject in new[] { restored, deleted })
        {
            await this.CreateTopicAsync(subject);
            await this.DeleteAsync((await this.GetMessageIdsAsync()).First());
        }

        await this.GotoAsync("Topics/1-Test Forum");
        (await this.Page.InnerTextAsync("body")).Should().NotContain(restored);

        await this.GotoAsync("Admin/Restore");
        await this.ClickAndWaitAsync(
            this.Page.Locator("li", new PageLocatorOptions { HasTextString = restored }).Last.Locator("[id$='_Restore']"));

        await this.ClickAndWaitAsync(
            this.Page.Locator("li", new PageLocatorOptions { HasTextString = deleted }).Last.Locator("[id$='_Delete']"));

        await this.GotoAsync("Admin/Restore");
        var list = await this.Page.InnerTextAsync("body");
        list.Should().NotContain(restored).And.NotContain(deleted);

        await this.GotoAsync("Topics/1-Test Forum");
        var topics = await this.Page.InnerTextAsync("body");
        topics.Should().Contain(restored).And.NotContain(deleted);
    }

    private async Task<int> CreateTopicAsync(string subject)
    {
        await this.GotoAsync("PostTopic?f=1");

        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("The first post.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Topics/1-Test Forum");

        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");

        await this.GotoAsync(href.TrimStart('/'));

        return int.Parse(Regex.Match(href, @"/Posts/t(\d+)").Groups[1].Value);
    }

    private async Task DeleteAsync(int messageId)
    {
        await this.GotoAsync($"DeleteMessage?m={messageId}&action=delete");
        await this.Control("ReasonEditor").FillAsync("Deleted by the UI test");
        await this.ClickAndWaitAsync(this.Control("Delete"));
    }

    /// <summary>
    /// Gets the ids of the posts of the shown topic page.
    /// </summary>
    private async Task<int[]> GetMessageIdsAsync()
    {
        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");

        return [.. ids.Select(id => Regex.Match(id, @"^post(\d+)$")).Where(m => m.Success).Select(m => int.Parse(m.Groups[1].Value))];
    }
}
