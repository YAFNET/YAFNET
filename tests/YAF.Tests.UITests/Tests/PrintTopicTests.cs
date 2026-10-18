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
/// The print page of a topic.
/// </summary>
[TestFixture]
public class PrintTopicTests : ForumTestBase
{
    [Test]
    [Description("The posts of a topic are printed (without hidden content), an unknown topic and no topic are refused")]
    public async Task Print_Topic_Test()
    {
        var id = Guid.NewGuid().ToString("N");
        var subject = $"Print {id}";

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync($"[b]Printed {id}[/b] [hide]Hidden {id}[/hide]");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Topics/1-Test Forum");
        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");
        var topicId = Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync($"PrintTopic?t={topicId}");
        await this.AssertNoServerErrorAsync();

        var text = await this.Page.InnerTextAsync("body");
        text.Should().Contain($"Printed {id}").And.Contain(TestConfig.TestUserName).And.NotContain($"Hidden {id}");

        foreach (var url in new[] { "PrintTopic?t=999999", "PrintTopic" })
        {
            await this.GotoAsync(url);
            await this.AssertNoServerErrorAsync();
            this.Page.Url.Should().NotContain("PrintTopic", url);
        }
    }
}
