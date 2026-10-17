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
