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
/// Moderate > Forum: the selected topics are moved to another forum or deleted.
/// </summary>
[TestFixture]
public class ModerateForumTests : ForumTestBase
{
    [Test]
    [Description("A selected topic is moved to another forum, another one is deleted")]
    public async Task Move_And_Delete_Topics_Test()
    {
        var forum = $"Target {Guid.NewGuid():N}".Substring(0, 15);
        var moved = $"Moved {Guid.NewGuid():N}";
        var deleted = $"Deleted {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(forum);
        await this.Control("Description").FillAsync("The target of moved topics");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync("4");
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.GotoAsync(string.Empty);
        var forumHref = await this.Page.Locator("a", new PageLocatorOptions { HasTextString = forum }).First.GetAttributeAsync("href");
        var forumId = Regex.Match(forumHref, @"Topics/(\d+)").Groups[1].Value;

        await this.CreateTopicAsync(moved);
        await this.CreateTopicAsync(deleted);

        // move
        await this.GotoAsync("Moderate/Forums?f=1");
        await this.SelectTopicAsync(moved);
        await this.Control("ForumListSelected").EvaluateAsync("(e, id) => e.value = id", forumId);
        await this.Page.Locator("a.dropdown-toggle[aria-label='arrows-alt']").ClickAsync();
        await this.ClickAndWaitAsync(this.Control("Move"));

        await this.GotoAsync(forumHref.TrimStart('/'));
        (await this.Page.InnerTextAsync("body")).Should().Contain(moved);

        // delete
        await this.GotoAsync("Moderate/Forums?f=1");
        await this.SelectTopicAsync(deleted);
        await this.Control("DeleteTopic").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.GotoAsync("Topics/1-Test Forum");
        // the moved topic is a pointer to the new forum
        var topics = await this.Page.InnerTextAsync("body");
        topics.Should().NotContain(deleted);
    }

    [Test]
    [Description("Nothing selected: the moderator gets a message")]
    public async Task Nothing_Selected_Test()
    {
        await this.LoginAsAdminAsync();
        await this.CreateTopicAsync($"Not selected {Guid.NewGuid():N}");

        await this.GotoAsync("Moderate/Forums?f=1");
        await this.Control("DeleteTopic").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await this.Page.ContentAsync()).Should().Contain("toast-body");
    }

    [Test]
    [Description("The reported and the unapproved posts of an unknown forum (or of none) show the info page")]
    public async Task Unknown_Forum_Test()
    {
        await this.LoginAsAdminAsync();

        foreach (var url in new[]
                     {
                         "Moderate/ReportedPosts?f=999999", "Moderate/ReportedPosts", "Moderate/UnapprovedPosts?f=999999",
                         "Moderate/UnapprovedPosts"
                     })
        {
            await this.GotoAsync(url);
            this.Page.Url.Should().Contain("Info", url);
        }
    }

    private async Task CreateTopicAsync(string subject)
    {
        await this.GotoAsync("PostTopic?f=1");

        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A topic to moderate.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
    }

    /// <summary>
    /// Checks the check box of a topic of the moderate list (the newest topics are on the first page).
    /// </summary>
    private async Task SelectTopicAsync(string subject)
    {
        await this.Page.Locator(
                $"xpath=//a[contains(., '{subject}')]/ancestor::*[.//input[@type='checkbox']][1]//input[@type='checkbox']")
            .First.CheckAsync();
    }
}
