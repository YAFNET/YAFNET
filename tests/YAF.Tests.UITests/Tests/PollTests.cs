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
/// The polls of the topics: voting, editing and removing a poll.
/// </summary>
[TestFixture]
public class PollTests : ForumTestBase
{
    [Test]
    [Description("A member votes once, the admin changes the question and adds a choice, then removes the poll")]
    public async Task Vote_Edit_And_Remove_Test()
    {
        var question = $"Question {Guid.NewGuid():N}";
        var changed = $"Changed {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();
        var topicHref = await this.CreateTopicAsync($"Poll {Guid.NewGuid():N}");
        var topicId = Regex.Match(topicHref, @"Posts/t(\d+)").Groups[1].Value;

        await this.GotoAsync($"PollEdit?t={topicId}");
        await this.Control("Question").FillAsync(question);
        await this.Page.Locator("[id$='_ChoiceRepeater_ctl00_PollChoice']").FillAsync("Yes");
        await this.Page.Locator("[id$='_ChoiceRepeater_ctl01_PollChoice']").FillAsync("No");
        await this.ClickAndWaitAsync(this.Control("SavePoll"));

        // a member votes, then only the results are shown
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync(topicHref);

        (await this.Control("VoteButton").CountAsync()).Should().Be(2);
        await this.ClickAndWaitAsync(this.Control("VoteButton").First);

        await this.GotoAsync(topicHref);
        (await this.Control("VoteButton").First.GetAttributeAsync("class")).Should().Contain("disabled", "the member voted");
        (await this.Page.Locator(".progress").CountAsync()).Should().BeGreaterThan(0, "the results are shown");

        // the admin changes the poll
        await this.LoginAsAdminAsync();
        await this.GotoAsync(topicHref);
        await this.ClickAndWaitAsync(this.Control("EditPoll"));

        await this.Control("Question").FillAsync(changed);
        await this.Page.Locator("[id$='_ChoiceRepeater_ctl02_PollChoice']").FillAsync("Maybe");
        await this.ClickAndWaitAsync(this.Control("SavePoll"));

        await this.GotoAsync(topicHref);
        var text = await this.Page.InnerTextAsync("body");
        text.Should().Contain(changed).And.Contain("Maybe");

        // and removes it
        await this.Control("RemovePoll").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.GotoAsync(topicHref);
        (await this.Page.InnerTextAsync("body")).Should().NotContain(changed);
    }

    [Test]
    [Description("A poll needs a question and two choices")]
    public async Task Poll_Checks_Test()
    {
        await this.LoginAsAdminAsync();
        var topicHref = await this.CreateTopicAsync($"Poll checks {Guid.NewGuid():N}");
        var topicId = Regex.Match(topicHref, @"Posts/t(\d+)").Groups[1].Value;

        await this.GotoAsync($"PollEdit?t={topicId}");
        await this.Control("Question").EvaluateAsync("e => e.removeAttribute('required')");
        await this.ClickAndWaitAsync(this.Control("SavePoll"));
        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        await this.Control("Question").FillAsync("Only one choice?");
        await this.Page.Locator("[id$='_ChoiceRepeater_ctl00_PollChoice']").FillAsync("Yes");
        await this.ClickAndWaitAsync(this.Control("SavePoll"));
        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        this.Page.Url.Should().Contain("PollEdit", "the poll is not saved");
    }

    private async Task<string> CreateTopicAsync(string subject)
    {
        await this.GotoAsync("PostTopic?f=1");

        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A topic with a poll.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Topics/1-Test Forum");

        var href = await this.Page.Locator("a[href*='/Posts/t']", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");

        return href.TrimStart('/');
    }
}
