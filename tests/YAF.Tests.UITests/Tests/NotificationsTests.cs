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
    private ILocator ItemButtons => this.Page.Locator("[id*='_ActivityStream_'][id$='_MarkRead']");
}
