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
/// The move dialog of a topic.
/// </summary>
[TestFixture]
public class MoveTopicTests : ForumTestBase
{
    [Test]
    [Description("A topic is moved with a link (no forum and invalid link days are refused), the link stays in the old forum")]
    public async Task Move_With_Link_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var forumName = $"Move target {id}";
        var subject = $"Moved topic {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();

        // the target forum
        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(forumName);
        await this.Control("Description").FillAsync("The target of the moved topics");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync("75");
        await this.ClickAndWaitAsync(this.Control("Save"));

        var forumId = ForumId(forumName);

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A topic to move.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        var topicUrl = new Uri(this.Page.Url).PathAndQuery.TrimStart('/');

        // no forum and invalid link days
        foreach (var (forum, days) in new[] { ("0", "3"), (forumId, "abc"), (forumId, "-5") })
        {
            await this.OpenDialogAsync(topicUrl);
            await this.Control("MoveTopicDialog_ForumListSelected").EvaluateAsync("(e, v) => e.value = v", forum);
            await this.Control("MoveTopicDialog_LeavePointer").CheckAsync();
            await this.Control("MoveTopicDialog_LinkDays").EvaluateAsync("e => e.type = 'text'");
            await this.Control("MoveTopicDialog_LinkDays").FillAsync(days);
            await this.ClickAndWaitAsync(this.Control("MoveTopicDialog_Move"));

            await this.AssertNoServerErrorAsync();
            (await this.Page.ContentAsync()).Should().Contain("toast-body", $"{forum} {days}");
        }

        await this.OpenDialogAsync(topicUrl);
        await this.Control("MoveTopicDialog_ForumListSelected").EvaluateAsync("(e, v) => e.value = v", forumId);
        await this.Control("MoveTopicDialog_LeavePointer").CheckAsync();
        await this.Control("MoveTopicDialog_LinkDays").FillAsync("3");
        await this.ClickAndWaitAsync(this.Control("MoveTopicDialog_Move"));
        await this.AssertNoServerErrorAsync();

        await this.GotoAsync($"Topics/{forumId}-forum");
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject, "the topic is in the new forum");

        await this.GotoAsync("Topics/1-Test Forum");
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject, "the link stays in the old forum");
    }

    private async Task OpenDialogAsync(string topicUrl)
    {
        await this.GotoAsync(topicUrl);
        // the manage menu of the topic
        await this.Control("Tools1").First.ClickAsync();
        await this.Control("MoveTopic1").ClickAsync();
        await this.Control("MoveTopicDialog_Move").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }
}
