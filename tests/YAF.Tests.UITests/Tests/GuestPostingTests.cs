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
/// The posts of guests in a forum where guests may post: the name of the guest is checked.
/// </summary>
[TestFixture]
public class GuestPostingTests : ForumTestBase
{
    [Test]
    [Description("A guest posts a topic and a reply: a too long name and the name of a member are refused")]
    public async Task Guest_Topic_And_Reply_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var forumName = $"Guests {id}";
        var subject = $"Guest topic {id}";

        // a forum where the guests are members
        await this.LoginAsAdminAsync();
        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(forumName);
        await this.Control("Description").FillAsync("A forum for the posts of guests");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync("70");

        // the access mask of the guests (the label of the list is the name of the role)
        await this.Page.GetByLabel("Guests", new PageGetByLabelOptions { Exact = true }).SelectOptionAsync(
            new SelectOptionValue { Label = "Member Access" },
            new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.GotoAsync(string.Empty);
        var forumHref = await this.Page.Locator("a[href*='/Topics/']", new PageLocatorOptions { HasTextString = forumName })
                            .First.GetAttributeAsync("href");
        var forumId = Regex.Match(forumHref, @"/Topics/(\d+)").Groups[1].Value;

        await this.LogoutAsync();

        // the name of the guest
        foreach (var name in new[] { new string('g', 101), TestConfig.AdminUserName })
        {
            await this.GotoAsync($"PostTopic?f={forumId}");
            await this.Control("From").EvaluateAsync("e => e.removeAttribute('maxlength')");
            await this.Control("From").FillAsync(name);
            await this.Control("TopicSubjectTextBox").FillAsync(subject);
            await this.Control("YafTextEditor").FillAsync("The topic of a guest.");
            await this.ClickAndWaitAsync(this.Control("PostReply"));

            this.Page.Url.Should().Contain("PostTopic", name);
        }

        await this.GotoAsync($"PostTopic?f={forumId}");
        await this.Control("From").FillAsync($"Visitor {id}");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("The topic of a guest.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        // the reply of a guest
        await this.GotoAsync(forumHref.TrimStart('/'));
        var topicHref = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                            .GetAttributeAsync("href");
        var topicId = Regex.Match(topicHref, @"Posts/t(\d+)").Groups[1].Value;

        foreach (var name in new[] { TestConfig.AdminUserName, string.Empty })
        {
            await this.GotoAsync($"PostMessage?t={topicId}&f={forumId}");
            await this.Control("From").FillAsync(name);
            await this.Control("YafTextEditor").FillAsync("The reply of a guest.");
            await this.ClickAndWaitAsync(this.Control("PostReply"));
        }

        await this.GotoAsync(topicHref.TrimStart('/'));
        (await this.Page.InnerTextAsync("body")).Should().Contain("The reply of a guest.");
    }
}
