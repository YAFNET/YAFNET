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
/// The notifications of a member (activity stream): received thanks, filters, mark as read.
/// </summary>
[TestFixture]
public class NotificationsTests : ForumTestBase
{
    [Test]
    [Description("A received thanks is a notification, it is filtered, marked as read one by one and all at once")]
    public async Task Received_Thanks_Test()
    {
        var member = $"Ntf{DateTime.UtcNow:HHmmssfff}";
        var subject = $"Thanked {Guid.NewGuid():N}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A post to thank.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        var topicId = Regex.Match(await this.Page.ContentAsync(), @"Posts/t(\d+)").Groups[1].Value;

        await this.GotoAsync($"PostMessage?t={topicId}&f=1");
        await this.Control("YafTextEditor").FillAsync("A reply to thank.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        var topicHref = new Uri(this.Page.Url).PathAndQuery.TrimStart('/');

        // two members thank the posts (one post each: the notifications of a post are read together)
        foreach (var (user, post) in new[] { (TestConfig.TestUserName, 0), (TestConfig.TestUserName2, 1) })
        {
            await this.LoginAsUserAsync(user);
            await this.GotoAsync(topicHref);
            await this.Page.Locator("a[href^='javascript:addThanks']").Nth(post).ClickAsync();
            await this.Page.Locator("a[href^='javascript:removeThanks']").First
                .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }

        await this.LoginAsync(member, TestConfig.TestUserPassword);
        await this.GotoAsync("Notification");

        (await this.ItemButtons.CountAsync()).Should().Be(2, "two thanks");

        // without the thanks
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("ReceivedThanks").UncheckAsync();
        await this.ClickAndWaitAsync(this.Control("Update"));
        (await this.ItemButtons.CountAsync()).Should().Be(0);

        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.ClickAndWaitAsync(this.Control("Reset"));
        (await this.ItemButtons.CountAsync()).Should().Be(2);

        // one is read, then all
        await this.ClickAndWaitAsync(this.ItemButtons.First);
        (await this.ItemButtons.CountAsync()).Should().Be(1);

        await this.ClickAndWaitAsync(this.Page.Locator(".card-footer [id$='_MarkAll']"));
        (await this.ItemButtons.CountAsync()).Should().Be(0);
    }

    /// <summary>
    /// Gets the "mark as read" buttons of the notifications of the list (not of the menu of the header).
    /// </summary>
    [Test]
    [Description("The notification menu (api) lists a thanks, a quote, a mention and a reply in a watched topic")]
    public async Task Notification_Menu_Test()
    {
        var member = $"Nmn{DateTime.UtcNow:HHmmssfff}";
        var subject = $"Notified {Guid.NewGuid():N}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        // a watched topic
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A post for the notifications.");
        await this.Control("TopicWatch").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");
        var messageId = ids.Select(id => Regex.Match(id, @"^post(\d+)$")).Where(m => m.Success).Select(m => m.Groups[1].Value).First();
        var topicHref = new Uri(this.Page.Url).PathAndQuery.TrimStart('/');

        await this.GotoAsync("Topics/1-Test Forum");
        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");
        var topicId = Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;

        // another member thanks, quotes and mentions the member
        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync(topicHref);
        await this.Page.Locator("a[href^='javascript:addThanks']").First.ClickAsync();
        await this.Page.Locator("a[href^='javascript:removeThanks']").First
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.GotoAsync($"PostMessage?t={topicId}&f=1&q={messageId}");
        await this.Control("YafTextEditor").EvaluateAsync(
            "(e, member) => e.value = e.value + ' Hello @[userlink]' + member + '[/userlink]'",
            member);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        // the menu of the member
        await this.LoginAsync(member, TestConfig.TestUserPassword);
        await this.GotoAsync(string.Empty);

        var response = await this.Page.APIRequest.PostAsync(
            $"{TestConfig.BaseUrl}api/Notify/GetNotifications",
            new APIRequestContextOptions
                {
                    Headers = new System.Collections.Generic.Dictionary<string, string>
                                  {
                                      ["Content-Type"] = "application/json;charset=utf-8"
                                  },
                    Data = "{\"PageNumber\":0,\"PageSize\":20}"
                });

        var json = await response.TextAsync();
        json.Should().Contain(subject).And.Contain("fa-heart").And.Contain("fa-at").And.Contain("fa-quote-left");

        // the notification page: three entries, the filter shows only the thanks, reset shows all again
        await this.GotoAsync("Notification");
        (await this.ItemButtons.CountAsync()).Should().Be(3);

        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("WasMentioned").UncheckAsync();
        await this.Control("WasQuoted").UncheckAsync();
        await this.ClickAndWaitAsync(this.Control("Update"));
        (await this.ItemButtons.CountAsync()).Should().Be(1, "only the thanks");

        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.ClickAndWaitAsync(this.Control("Reset"));
        (await this.ItemButtons.CountAsync()).Should().Be(3);
    }

    private ILocator ItemButtons => this.Page.Locator("[id*='_ActivityStream_'][id$='_MarkRead']");
}
