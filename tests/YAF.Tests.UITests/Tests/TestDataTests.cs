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
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The test data generator of the host admin: users, a board, categories, forums, topics (sticky, with poll),
/// messages and private messages.
/// </summary>
[TestFixture]
public class TestDataTests : ForumTestBase
{
    [Test]
    [Description("One item of each kind is generated, the topics of the topics tab get the selected priority or a poll")]
    public async Task Generate_Test()
    {
        var prefix = $"td{DateTime.UtcNow:HHmmssfff}-";

        await this.LoginAsAdminAsync();

        await this.GenerateAsync(
            new Dictionary<string, string>
                {
                    ["UsersNumber"] = "1",
                    ["UserPrefixTB"] = prefix,
                    ["BoardNumber"] = "1",
                    ["BoardsUsersNumber"] = "1",
                    ["BoardPrefixTB"] = prefix,
                    ["CategoriesNumber"] = "1",
                    ["CategoriesForumsNumber"] = "1",
                    ["CategoriesTopicsNumber"] = "1",
                    ["CategoriesMessagesNumber"] = "1",
                    ["CategoryPrefixTB"] = prefix,
                    ["ForumsNumber"] = "1",
                    ["ForumsTopicsNumber"] = "1",
                    ["ForumsMessagesNumber"] = "1",
                    ["ForumPrefixTB"] = prefix,
                    ["TopicsNumber"] = "2",
                    ["TopicsMessagesNumber"] = "1",
                    ["TopicsPriorityList"] = "1",
                    ["TopicPrefixTB"] = $"{prefix}s",
                    ["PostsNumber"] = "2",
                    ["PMessagesNumber"] = "2",
                    ["MarkRead"] = "true"
                });

        var report = await this.Page.ContentAsync();
        report.Should().Contain("1 Boards").And.Contain("1 Categories").And.Contain("1 Forums").And.Contain("2 Topics")
            .And.Contain("2 Messages").And.Contain("2 Private Messages");

        // a topic with a poll
        await this.GenerateAsync(
            new Dictionary<string, string>
                {
                    ["TopicsNumber"] = "1", ["PollCreate"] = "true", ["TopicPrefixTB"] = $"{prefix}p"
                });

        // the topics of the topics tab
        await this.GotoAsync("Topics/1-Test Forum");

        var sticky = this.Page.Locator(".row", new PageLocatorOptions { HasTextString = $"{prefix}s" }).Last;
        (await sticky.Locator(".fa-thumbtack").CountAsync()).Should().BeGreaterThan(0, "the topic is sticky");

        var poll = this.Page.Locator(".row", new PageLocatorOptions { HasTextString = $"{prefix}p" }).Last;
        (await poll.Locator(".fa-poll-h").CountAsync()).Should().BeGreaterThan(0, "the topic has a poll");

        // nothing selected
        await this.GotoAsync("Admin/TestData");
        await this.Page.EvaluateAsync("() => document.querySelector(\"[id$='_PostsNumber']\").value = '1'");
        await this.ClickAndWaitAsync(this.Control("LaunchGenerator"));

        (await this.Page.ContentAsync()).Should().Contain("Test Data Generator reports");
    }

    /// <summary>
    /// Generates the data: the topics and the messages in the test forum, the other values are set in the fields
    /// (they are in hidden tabs).
    /// </summary>
    private async Task GenerateAsync(Dictionary<string, string> values)
    {
        await this.GotoAsync("Admin/TestData");

        // the lists that load the next list
        await this.SelectAsync("ForumsCategory", "Test Category", true);
        await this.SelectAsync("TopicsCategory", "Test Category", true);
        await this.SelectAsync("TopicsForum", "Test Forum", false);
        await this.SelectAsync("PostsCategory", "Test Category", true);
        await this.SelectAsync("PostsForum", "Test Forum", true);

        await this.Page.EvaluateAsync(
            """
            values => {
                for (const [id, value] of Object.entries(values)) {
                    const e = document.querySelector(`[id$='_${id}']`);
                    if (e.type === 'checkbox') { e.checked = value === 'true'; } else { e.value = value; }
                }
            }
            """,
            values);

        await this.ClickAndWaitAsync(this.Control("LaunchGenerator"));
    }

    /// <summary>
    /// Selects the option with the text (a list in a hidden tab, the list may post back).
    /// </summary>
    private async Task SelectAsync(string id, string text, bool postBack)
    {
        var select = this.Control(id);

        var value = await select.EvaluateAsync<string>(
            "(e, text) => Array.from(e.options).find(o => o.text.includes(text)).value",
            text);

        if (postBack)
        {
            await this.Page.RunAndWaitForNavigationAsync(
                () => select.SelectOptionAsync(value, new LocatorSelectOptionOptions { Force = true }));
        }
        else
        {
            await select.SelectOptionAsync(value, new LocatorSelectOptionOptions { Force = true });
        }
    }
}
