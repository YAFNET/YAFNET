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
/// The rows of the topic list: the page links of long topics, announcements, styled titles and locked topics.
/// </summary>
[TestFixture]
public class TopicListTests : ForumTestBase
{
    [Test]
    [Description("A long topic shows links to the first and last pages, a shorter one to all pages; an announcement with a styled title and a locked topic")]
    public async Task Pages_Announcement_And_Locked_Test()
    {
        var id = Guid.NewGuid().ToString("N").Substring(0, 10);
        var longTopic = $"Long topic {id}";
        var shortTopic = $"Short topic {id}";

        await this.LoginAsAdminAsync();
        await this.SetHostSettingsAsync("2", true);

        try
        {
            // an announcement with a style and 9 posts (5 pages)
            await this.GotoAsync("PostTopic?f=1");
            await this.Control("TopicSubjectTextBox").FillAsync(longTopic);
            await this.Control("YafTextEditor").FillAsync("The first post.");
            await this.Control("Priority").SelectOptionAsync("2", new LocatorSelectOptionOptions { Force = true });
            await this.Control("TopicStylesTextBox").FillAsync("color: green;");
            await this.ClickAndWaitAsync(this.Control("PostReply"));
            await this.ReplyAsync(longTopic, 8);

            // a locked topic with 3 posts (2 pages)
            await this.GotoAsync("PostTopic?f=1");
            await this.Control("TopicSubjectTextBox").FillAsync(shortTopic);
            await this.Control("YafTextEditor").FillAsync("The first post.");
            await this.ClickAndWaitAsync(this.Control("PostReply"));
            await this.ReplyAsync(shortTopic, 2);

            await this.Control("Tools1").First.ClickAsync();
            await this.ClickAndWaitAsync(this.Control("LockTopic1").First);

            await this.GotoAsync("Topics/1-Test Forum");
            await this.AssertNoServerErrorAsync();

            var longRow = this.Page.Locator("[aria-label='topic pager']").Filter(
                new LocatorFilterOptions { Has = this.Page.Locator("a[href$='/page5']") });
            (await longRow.CountAsync()).Should().BeGreaterThan(0, "the last page of the long topic is linked");

            (await this.Page.Locator("a[href$='/page2']").CountAsync()).Should().BeGreaterThan(0);
            (await this.Page.InnerTextAsync("body")).Should().Contain(longTopic).And.Contain(shortTopic);
            (await this.Page.Locator("span[style*='green']", new PageLocatorOptions { HasTextString = longTopic }).CountAsync())
                .Should().BeGreaterThan(0, "the title is styled");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetHostSettingsAsync("20", false);
        }
    }

    private async Task ReplyAsync(string subject, int count)
    {
        await this.GotoAsync("Topics/1-Test Forum");
        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");
        var topicId = Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;

        for (var i = 1; i <= count; i++)
        {
            await this.GotoAsync($"PostMessage?t={topicId}&f=1");
            await this.Control("YafTextEditor").FillAsync($"Reply {i}.");
            await this.ClickAndWaitAsync(this.Control("PostReply"));
        }
    }

    private async Task SetHostSettingsAsync(string postsPerPage, bool styledTitles)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Control("PostsPerPage").EvaluateAsync("(e, v) => e.value = v", postsPerPage);
        await this.Control("UseStyledTopicTitles").EvaluateAsync("(e, v) => e.checked = v", styledTitles);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }
}
