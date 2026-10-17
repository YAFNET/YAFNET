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
/// The delete page of a post: the replies to the post, cancel, delete and undelete.
/// </summary>
[TestFixture]
public class DeleteMessageTests : ForumTestBase
{
    [Test]
    [Description("The replies to a post are listed, cancel goes back, a reply is deleted and undeleted, the post is deleted with its replies")]
    public async Task Delete_And_Undelete_Test()
    {
        var subject = $"Delete page {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("The first post.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync($"PostMessage?t={await this.TopicIdAsync(subject)}&f=1");
        await this.Control("YafTextEditor").FillAsync("The second post.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        var secondId = (await this.MessageIdsAsync()).Max();

        // a member answers the second post
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync($"PostMessage?t={await this.TopicIdAsync(subject)}&f=1&q={secondId}");
        await this.Control("YafTextEditor").EvaluateAsync("e => e.value = e.value + ' The reply of the member.'");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        var replyId = (await this.MessageIdsAsync()).Max();

        // the admin sees the reply on the delete page of the second post
        await this.LoginAsAdminAsync();
        await this.GotoAsync($"DeleteMessage?m={secondId}&action=delete");
        (await this.Page.InnerTextAsync("body")).Should().Contain("The reply of the member.", "the reply is listed");

        await this.ClickAndWaitAsync(this.Control("Cancel"));
        this.Page.Url.Should().Contain("Topics");

        // the reply is deleted and undeleted
        await this.GotoAsync($"DeleteMessage?m={replyId}&action=delete");
        await this.Control("ReasonEditor").FillAsync("Deleted for a moment");
        await this.ClickAndWaitAsync(this.Control("Delete"));

        await this.GotoAsync($"DeleteMessage?m={replyId}&action=delete");
        await this.ClickAndWaitAsync(this.Control("Restore"));
        (await this.Page.InnerTextAsync("body")).Should().Contain("The reply of the member.");

        // the second post with its replies
        await this.GotoAsync($"DeleteMessage?m={secondId}&action=delete");
        await this.Control("DeleteAllPosts").CheckAsync();
        await this.Control("ReasonEditor").FillAsync("Deleted with the replies");
        await this.ClickAndWaitAsync(this.Control("Delete"));

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync($"Topics/1-Test Forum");
        var href = await this.Page.Locator("a[href*='/Posts/t']", new Microsoft.Playwright.PageLocatorOptions { HasTextString = subject })
                       .First.GetAttributeAsync("href");
        await this.GotoAsync(href.TrimStart('/'));

        var text = await this.Page.InnerTextAsync("body");
        text.Should().Contain("The first post.").And.NotContain("The reply of the member.");
    }

    private async Task<string> TopicIdAsync(string subject)
    {
        await this.GotoAsync("Topics/1-Test Forum");

        var href = await this.Page.Locator("a[href*='/Posts/t']", new Microsoft.Playwright.PageLocatorOptions { HasTextString = subject })
                       .First.GetAttributeAsync("href");

        return Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;
    }

    private async Task<int[]> MessageIdsAsync()
    {
        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");

        return [.. ids.Select(id => Regex.Match(id, @"^post(\d+)$")).Where(m => m.Success).Select(m => int.Parse(m.Groups[1].Value))];
    }
}
