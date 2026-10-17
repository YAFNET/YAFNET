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
/// The quick reply dialog of a topic: an empty reply and watching the topic.
/// </summary>
[TestFixture]
public class QuickReplyTests : ForumTestBase
{
    [Test]
    [Description("An empty quick reply is refused, a reply with \"watch topic\" subscribes the topic")]
    public async Task Empty_Reply_And_Watch_Test()
    {
        var subject = $"Quick {Guid.NewGuid():N}";
        var member = $"Qrp{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("The first post.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        var topicHref = new Uri(this.Page.Url).PathAndQuery.TrimStart('/');

        await this.LoginAsync(member, TestConfig.TestUserPassword);
        await this.GotoAsync(topicHref);

        // empty (the browser checks it first)
        await this.OpenAsync();
        await this.Control("QuickReplyDialog_YafTextEditor").EvaluateAsync("e => e.removeAttribute('required')");
        await this.Control("QuickReplyDialog_YafTextEditor").FillAsync(string.Empty);
        await this.ClickAndWaitAsync(this.Control("QuickReplyDialog_Reply"));
        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        // with watching the topic
        await this.GotoAsync(topicHref);
        await this.OpenAsync();
        await this.Control("QuickReplyDialog_YafTextEditor").FillAsync("A quick reply with watching.");
        await this.Control("QuickReplyDialog_TopicWatch").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("QuickReplyDialog_Reply"));

        (await this.Page.InnerTextAsync("body")).Should().Contain("A quick reply with watching.");

        // the topic is watched
        await this.Control("OptionsLink").ClickAsync();
        (await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Unwatch this topic" }).CountAsync())
            .Should().Be(1, $"the topic {subject} is watched");
    }

    private async Task OpenAsync()
    {
        // the visible button of the post (the context menu has a hidden one)
        await this.Page.Locator("[id*='_QuickReplyLink']").Filter(new LocatorFilterOptions { Visible = true }).First.ClickAsync();
        await this.Control("QuickReplyDialog_Reply").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }
}
