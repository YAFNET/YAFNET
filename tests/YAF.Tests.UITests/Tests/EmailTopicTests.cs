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
/// A topic is sent by e-mail: the subject and the text are fixed, only the address is entered.
/// </summary>
[TestFixture]
public class EmailTopicTests : ForumTestBase
{
    [Test]
    [Description("The topic is sent to an address (an invalid address is refused), guests and an unknown topic get the info page")]
    public async Task Email_Topic_Test()
    {
        var subject = $"Mailed {Guid.NewGuid():N}".Substring(0, 17);
        var address = $"friend{DateTime.UtcNow:HHmmssfff}@example.com";

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A topic to send by e-mail.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        // the id of the topic: the link of the topic list
        await this.GotoAsync("Topics/1-Test Forum");
        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");
        var topicId = Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;

        // the share menu of the topic opens the page
        await this.GotoAsync(href.TrimStart('/'));
        await this.Control("ShareLink").ClickAsync();

        // the item is a link to # that posts the page back
        await this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Email this topic" }).First
            .ClickAsync();
        await this.Page.WaitForURLAsync(url => url.Contains($"EmailTopic?t={topicId}"));

        (await this.Control("Subject").InputValueAsync()).Should().Be(subject);

        // an invalid address (the browser check of the field is removed)
        await this.Control("EmailAddress").EvaluateAsync("e => e.type = 'text'");
        await this.Control("EmailAddress").FillAsync("not-an-address");
        await this.ClickAndWaitAsync(this.Control("SendEmail"));
        this.Page.Url.Should().Contain("EmailTopic");

        var started = DateTime.UtcNow;

        await this.Control("EmailAddress").FillAsync(address);
        await this.ClickAndWaitAsync(this.Control("SendEmail"));
        this.Page.Url.Should().Contain($"Posts/t{topicId}");

        var mail = await TestMail.WaitForAsync(address, started);
        mail.Headers.Should().Contain(subject);

        // an unknown topic and none
        await this.GotoAsync("EmailTopic?t=999999");
        this.Page.Url.Should().Contain("Info");

        await this.GotoAsync("EmailTopic");
        this.Page.Url.Should().Contain("Info");

        // guests
        await this.LogoutAsync();
        await this.GotoAsync($"EmailTopic?t={topicId}");
        this.Page.Url.Should().NotContain("EmailTopic?");
    }
}
