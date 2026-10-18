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
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The internal spam check of the posts of new members (spam words): logged, flagged or rejected.
/// </summary>
[TestFixture]
public class SpamTests : ForumTestBase
{
    [Test]
    [Description("A post with a spam word is logged in the spam log (do nothing), refused (reject) and flagged as unapproved")]
    public async Task Spam_Word_Post_Test()
    {
        var word = $"spamword{Guid.NewGuid():N}".Substring(0, 16);
        var member = $"Spm{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.GotoAsync("Admin/SpamWords");
        await this.ClickAndWaitAsync(
            this.Page.Locator("a", new PageLocatorOptions { HasTextString = "Add New Word" })
                .Filter(new LocatorFilterOptions { Visible = true }).First);
        await this.Page.Locator("[id$='_EditDialog_Save']").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await this.Control("EditDialog_spamword").FillAsync(word);
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        try
        {
            // do nothing: the post is saved and logged
            var logged = $"Logged {Guid.NewGuid():N}";
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.PostAsync(logged, $"Buy {word} now");

            await this.LoginAsAdminAsync();
            await this.GotoAsync("Topics/1-Test Forum");
            (await this.Page.InnerTextAsync("body")).Should().Contain(logged);

            await this.GotoAsync("Admin/SpamLog");
            (await this.Page.InnerTextAsync("body")).Should().Contain(member);

            // reject
            await this.SetHandlingAsync("2");

            var rejected = $"Rejected {Guid.NewGuid():N}";
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.PostAsync(rejected, $"Buy {word} now");
            (await this.Page.ContentAsync()).Should().Contain("toast-body");

            // flag as unapproved
            await this.LoginAsAdminAsync();
            await this.SetHandlingAsync("1");

            var flagged = $"Flagged {Guid.NewGuid():N}";
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.PostAsync(flagged, $"Buy {word} now");

            await this.LoginAsUserAsync(TestConfig.TestUserName2);
            await this.GotoAsync("Topics/1-Test Forum");
            var topics = await this.Page.InnerTextAsync("body");
            topics.Should().NotContain(rejected).And.NotContain(flagged);
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetHandlingAsync("0");
        }
    }

    [Test]
    [Description("A reply, a quick reply, an edit and a private message with a spam word for every handling (the last one deletes and bans the member)")]
    public async Task Spam_Word_Replies_Test()
    {
        var word = $"spamreply{Guid.NewGuid():N}".Substring(0, 16);
        var subject = $"Spam replies {Guid.NewGuid():N}".Substring(0, 24);

        await this.LoginAsAdminAsync();
        await this.AddSpamWordAsync(word);

        // the topic of the admin
        await this.PostAsync(subject, "A topic for the replies.");
        await this.GotoAsync("Topics/1-Test Forum");
        var href = (await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                        .GetAttributeAsync("href")).TrimStart('/');
        var topicId = System.Text.RegularExpressions.Regex.Match(href, @"Posts/t(\d+)").Groups[1].Value;

        try
        {
            foreach (var handling in new[] { "0", "1", "2", "3" })
            {
                var member = $"Spr{handling}{DateTime.UtcNow:HHmmssfff}";

                await this.RegisterAsync(member, TestConfig.TestUserPassword);

                // a post without spam to edit
                await this.LoginAsync(member, TestConfig.TestUserPassword);
                await this.GotoAsync($"PostMessage?t={topicId}&f=1");
                await this.Control("YafTextEditor").FillAsync("A clean reply.");
                await this.ClickAndWaitAsync(this.Control("PostReply"));

                var ids = await this.Page.Locator("a[id^='post']").EvaluateAllAsync<string[]>("es => es.map(e => e.id)");
                var messageId = ids.Select(id => System.Text.RegularExpressions.Regex.Match(id, @"^post(\d+)$"))
                    .Where(m => m.Success).Select(m => int.Parse(m.Groups[1].Value)).Max();

                await this.LoginAsAdminAsync();
                await this.SetHandlingAsync(handling);

                await this.LoginAsync(member, TestConfig.TestUserPassword);

                // the edit
                await this.GotoAsync($"EditMessage?m={messageId}");
                await this.Control("YafTextEditor").FillAsync($"The edit with {word}.");
                await this.ClickAndWaitAsync(this.Control("PostReply"));

                if (handling == "3")
                {
                    // the member is deleted and banned
                    await this.LoginAsync(member, TestConfig.TestUserPassword);
                    (await this.IsLoggedInAsAsync(member)).Should().BeFalse("the spammer is deleted");
                    continue;
                }

                // the reply
                await this.GotoAsync($"PostMessage?t={topicId}&f=1");
                await this.Control("YafTextEditor").FillAsync($"A reply with {word}.");
                await this.ClickAndWaitAsync(this.Control("PostReply"));

                // the quick reply
                await this.GotoAsync(href);
                await this.Page.Locator("[id*='_QuickReplyLink']").Filter(new LocatorFilterOptions { Visible = true }).First.ClickAsync();
                await this.Control("QuickReplyDialog_Reply").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
                await this.Control("QuickReplyDialog_YafTextEditor").FillAsync($"A quick reply with {word}.");
                await this.ClickAndWaitAsync(this.Control("QuickReplyDialog_Reply"));

                // the private message
                await this.GotoAsync("PostPrivateMessage");
                await this.Control("To").FillAsync(TestConfig.AdminUserName);
                await this.Control("PmSubjectTextBox").FillAsync($"Spam {handling}");
                await this.Control("YafTextEditor").FillAsync($"A message with {word}.");
                await this.ClickAndWaitAsync(this.Control("Save"));
            }
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetHandlingAsync("0");
        }
    }

    [Test]
    [Description("The spam log is shown with another page size and the date filter, delete all only deletes the spam entries")]
    public async Task Spam_Log_Test()
    {
        var word = $"spamlog{Guid.NewGuid():N}".Substring(0, 16);
        var member = $"Spl{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.AddSpamWordAsync(word);

        // a spam entry (do nothing logs the post)
        await this.LoginAsync(member, TestConfig.TestUserPassword);
        await this.PostAsync($"Spam log {Guid.NewGuid():N}", $"Buy {word} now");

        // an entry that is no spam: the test data generator logs its report
        await this.LoginAsAdminAsync();
        await this.GotoAsync("Admin/TestData");
        await this.ClickAndWaitAsync(this.Control("LaunchGenerator"));

        await this.GotoAsync("Admin/SpamLog");
        (await this.Page.InnerTextAsync("body")).Should().Contain(member);

        // another page size (posts back)
        await this.Page.RunAndWaitForResponseAsync(
            () => this.Control("PageSize").SelectOptionAsync("10"),
            response => response.Request.Method == "POST");
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        (await this.Page.InnerTextAsync("body")).Should().Contain(member);

        // the date filter
        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.ClickAndWaitAsync(this.Control("ApplyButton"));
        (await this.Page.InnerTextAsync("body")).Should().Contain(member);

        // all spam entries are deleted
        await this.Page.Locator("[id$='_Footer'] a").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        (await this.Page.InnerTextAsync("body")).Should().NotContain(member);

        await this.GotoAsync("Admin/EventLog");
        (await this.Page.Locator("[id$='_Footer'] a").CountAsync()).Should().BeGreaterThan(0, "the other entries are kept");
    }

    private async Task AddSpamWordAsync(string word)
    {
        await this.GotoAsync("Admin/SpamWords");
        await this.ClickAndWaitAsync(
            this.Page.Locator("a", new PageLocatorOptions { HasTextString = "Add New Word" })
                .Filter(new LocatorFilterOptions { Visible = true }).First);
        await this.Page.Locator("[id$='_EditDialog_Save']").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await this.Control("EditDialog_spamword").FillAsync(word);
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));
    }

    private async Task PostAsync(string subject, string text)
    {
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync(text);
        await this.ClickAndWaitAsync(this.Control("PostReply"));
    }

    /// <summary>
    /// Sets the handling of spam messages in the host settings.
    /// </summary>
    private async Task SetHandlingAsync(string value)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Control("SpamMessageHandling").SelectOptionAsync(value, new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("Save"));
    }
}
