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
/// The quotes of the reply page: the selected text of a post and several posts (multi-quote).
/// </summary>
[TestFixture]
public class QuoteTests : ForumTestBase
{
    [Test]
    [Description("A selected text of a post is quoted, several posts selected with multi-quote are quoted together")]
    public async Task Selected_Text_And_Multi_Quote_Test()
    {
        var id = Guid.NewGuid().ToString("N");
        var subject = $"Quotes {id}";

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync($"First post {id} with a long text.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Topics/1-Test Forum");
        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");
        var topicId = Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;

        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.Control("YafTextEditor").FillAsync($"Second post {id}.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var ids = (await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)"))
            .Select(i => Regex.Match(i, @"^post(\d+)$")).Where(m => m.Success).Select(m => int.Parse(m.Groups[1].Value))
            .OrderBy(i => i).ToArray();

        // the selected text (the script of the post sends it with the address)
        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync($"PostMessage?t={topicId}&f=1&q={ids[0]}&text={Uri.EscapeDataString($"selected {id}")}");
        var editor = await this.Control("YafTextEditor").InputValueAsync();
        editor.Should().Contain($"selected {id}").And.NotContain("with a long text");

        // multi-quote of both posts
        await this.GotoAsync(href.TrimStart('/'));

        foreach (var messageId in ids)
        {
            var box = this.Page.Locator($"#post{messageId}").Locator("xpath=ancestor::*[.//input[contains(@id,'_MultiQuote')]][1]")
                .Locator("input[id$='_MultiQuote']").First;

            await this.Page.RunAndWaitForResponseAsync(
                () => box.CheckAsync(new LocatorCheckOptions { Force = true }),
                response => response.Url.Contains("MultiQuote"));
        }

        await this.GotoAsync($"PostMessage?t={topicId}&f=1&q={ids[1]}");
        editor = await this.Control("YafTextEditor").InputValueAsync();
        editor.Should().Contain($"First post {id}").And.Contain($"Second post {id}");
    }
}
