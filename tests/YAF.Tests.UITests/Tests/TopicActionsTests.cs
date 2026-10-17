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
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The actions of the topic page: lock, watch, previous and next topic, delete.
/// </summary>
[TestFixture]
public class TopicActionsTests : ForumTestBase
{
    [Test]
    [Description("A locked topic can not be answered by members, unlocked it can")]
    public async Task Lock_And_Unlock_Test()
    {
        await this.LoginAsAdminAsync();
        var href = await this.CreateTopicAsync($"Locked {Guid.NewGuid():N}");

        await this.ManageAsync("LockTopic1");
        (await this.Control("UnlockTopic1").CountAsync()).Should().Be(1);

        // a member can not answer (the reply buttons are not shown)
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync(href);
        (await this.Page.Locator("[id*='_QuickReplyLink']").Filter(new LocatorFilterOptions { Visible = true }).CountAsync())
            .Should().Be(0);

        await this.LoginAsAdminAsync();
        await this.GotoAsync(href);
        await this.ManageAsync("UnlockTopic1");
        (await this.Control("LockTopic1").CountAsync()).Should().Be(1);
    }

    [Test]
    [Description("A member watches a topic (it is in the subscriptions) and stops watching it")]
    public async Task Watch_And_Unwatch_Test()
    {
        var subject = $"Watched {Guid.NewGuid():N}";

        // a topic of the admin
        await this.LoginAsAdminAsync();
        var href = await this.CreateTopicAsync(subject);

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync(href);

        await this.OptionAsync("Watch this topic");
        await this.Control("OptionsLink").ClickAsync();
        (await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Unwatch this topic" }).CountAsync())
            .Should().Be(1, "the topic is watched");

        await this.GotoAsync(href);
        await this.OptionAsync("Unwatch this topic");

        await this.Control("OptionsLink").ClickAsync();
        (await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Unwatch this topic" }).CountAsync())
            .Should().Be(0, "the topic is not watched anymore");
    }

    [Test]
    [Description("The previous and the next topic of the forum are opened")]
    public async Task Previous_And_Next_Topic_Test()
    {
        var first = $"First {Guid.NewGuid():N}";
        var second = $"Second {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();
        await this.CreateTopicAsync(first);
        await this.CreateTopicAsync(second);

        await this.ClickAndWaitAsync(this.Control("PrevTopic"));
        (await this.Page.TitleAsync()).Should().Contain(first);

        await this.ClickAndWaitAsync(this.Control("NextTopic"));
        (await this.Page.TitleAsync()).Should().Contain(second);
    }

    [Test]
    [Description("A moderator deletes the topic, it is not in the forum anymore")]
    public async Task Delete_Topic_Test()
    {
        var subject = $"Deleted {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();
        await this.CreateTopicAsync(subject);

        await this.Page.Locator("a[id$='_Tools1'][aria-label='cogs']").ClickAsync();
        await this.Control("DeleteTopic1").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        this.Page.Url.Should().Contain("Topics");
        (await this.Page.InnerTextAsync("body")).Should().NotContain(subject);
    }

    /// <summary>
    /// Creates a topic, the topic is shown, returns its address.
    /// </summary>
    private async Task<string> CreateTopicAsync(string subject)
    {
        await this.GotoAsync("PostTopic?f=1");

        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("The first post.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Topics/1-Test Forum");

        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");

        href = href.TrimStart('/');

        await this.GotoAsync(href);

        return href;
    }

    /// <summary>
    /// Clicks an item of the manage menu of the topic.
    /// </summary>
    private async Task ManageAsync(string id)
    {
        await this.Page.Locator("a[id$='_Tools1'][aria-label='cogs']").ClickAsync();

        await this.ClickAndWaitAsync(this.Control(id));
    }

    /// <summary>
    /// Clicks an item of the options menu of the topic.
    /// </summary>
    private async Task OptionAsync(string text)
    {
        await this.Control("OptionsLink").ClickAsync();

        await this.ClickAndWaitAsync(
            this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = text }).First);
    }
}
