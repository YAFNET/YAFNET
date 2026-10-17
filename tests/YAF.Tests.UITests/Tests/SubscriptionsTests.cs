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
/// The subscriptions of the profile: the watched forums and topics are removed, the notification settings and the
/// e-mail of a new private message.
/// </summary>
[TestFixture]
public class SubscriptionsTests : ForumTestBase
{
    [Test]
    [Description("A watched forum and topic are listed and removed (nothing selected gives a message), no notification hides the lists")]
    public async Task Unsubscribe_Test()
    {
        var member = $"Sub{DateTime.UtcNow:HHmmssfff}";
        var subject = $"Subscribed {Guid.NewGuid():N}".Substring(0, 21);

        await this.LoginAsAdminAsync();
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A topic to watch.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        var topicUrl = this.Page.Url;

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        // watch the forum and the topic
        await this.GotoAsync("Topics/1-Test Forum");
        await this.ClickAndWaitAsync(this.Control("WatchForum"));

        await this.GotoAsync(topicUrl);
        await this.Control("OptionsLink").ClickAsync();
        await this.ClickAndWaitAsync(
            this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = "Watch this topic" }).First);
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // the lists are shown for the selected forums and topics (new members have no notifications)
        await this.GotoAsync("Profile/Subscriptions");
        await this.PartialPostBackAsync(() => this.Control("rblNotificationType").Locator("input[value='0']").CheckAsync());
        await this.PartialPostBackAsync(() => this.Control("SaveUser").ClickAsync());

        await this.GotoAsync("Profile/Subscriptions");
        (await this.ForumRows().CountAsync()).Should().Be(1);
        (await this.TopicRows().CountAsync()).Should().Be(1);
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);

        // nothing selected
        await this.PartialPostBackAsync(() => this.Control("UnsubscribeForums").ClickAsync());
        await this.PartialPostBackAsync(() => this.Control("UnsubscribeTopics").ClickAsync());
        (await this.ForumRows().CountAsync()).Should().Be(1);
        (await this.TopicRows().CountAsync()).Should().Be(1);

        await this.ForumRows().First.CheckAsync();
        await this.PartialPostBackAsync(() => this.Control("UnsubscribeForums").ClickAsync());
        (await this.ForumRows().CountAsync()).Should().Be(0);

        await this.TopicRows().First.CheckAsync();
        await this.PartialPostBackAsync(() => this.Control("UnsubscribeTopics").ClickAsync());
        (await this.TopicRows().CountAsync()).Should().Be(0);

        // no notification: the lists are hidden
        await this.PartialPostBackAsync(() => this.Control("rblNotificationType").Locator("input[value='10']").CheckAsync());
        (await this.Control("UnsubscribeTopics").IsVisibleAsync()).Should().BeFalse();

        await this.PartialPostBackAsync(() => this.Control("SaveUser").ClickAsync());

        await this.GotoAsync("Profile/Subscriptions");
        (await this.Control("rblNotificationType").Locator("input[value='10']").IsCheckedAsync()).Should().BeTrue();
    }

    [Test]
    [Description("A member with the notification of private messages gets an e-mail with the link to the new message")]
    public async Task Private_Message_Notification_Test()
    {
        var member = $"Pmn{DateTime.UtcNow:HHmmssfff}";
        var subject = $"Notified {Guid.NewGuid():N}".Substring(0, 19);

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/Subscriptions");
        await this.Control("PMNotificationEnabled").CheckAsync();
        await this.PartialPostBackAsync(() => this.Control("SaveUser").ClickAsync());

        var started = DateTime.UtcNow;

        await this.LoginAsAdminAsync();
        await this.GotoAsync("PostPrivateMessage");
        await this.Control("To").FillAsync(member);
        await this.Control("PmSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A message with a notification.");
        await this.ClickAndWaitAsync(this.Control("Save"));

        var mail = await TestMail.WaitForAsync(EmailOf(member), started);
        var link = mail.GetLink("PrivateMessage");

        // the link opens the message
        await this.LoginAsync(member, TestConfig.TestUserPassword);
        await this.GotoAsync(link);
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);
    }

    /// <summary>
    /// The page is in update panels: the post back of a control only updates the panel.
    /// </summary>
    private async Task PartialPostBackAsync(Func<Task> action)
    {
        await this.Page.RunAndWaitForResponseAsync(
            action,
            response => response.Request.Method == "POST" && response.Url.Contains("Subscriptions"));
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await this.AssertNoServerErrorAsync();
    }

    private ILocator ForumRows()
    {
        return this.Page.Locator("[id*='_ForumList_'][id$='_unsubf']");
    }

    private ILocator TopicRows()
    {
        return this.Page.Locator("[id*='_TopicList_'][id$='_unsubx']");
    }
}
