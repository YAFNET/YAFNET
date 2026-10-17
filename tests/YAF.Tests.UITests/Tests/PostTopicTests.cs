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
/// The new topic page: the checks of the input, the preview, cancel, the poll option, watching and the flood delay.
/// </summary>
[TestFixture]
public class PostTopicTests : ForumTestBase
{
    [Test]
    [Description("An empty text, a missing and a taken subject are refused, the preview shows the text, cancel goes back to the forum")]
    public async Task Input_Checks_Preview_And_Cancel_Test()
    {
        var subject = $"Taken subject {Guid.NewGuid():N}".Substring(0, 24);

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.PostTopicAsync(subject, "The first topic with the subject.");
        this.Page.Url.Should().Contain("Posts");

        // empty text
        await this.PostTopicAsync($"Empty {Guid.NewGuid():N}", " ");
        this.Page.Url.Should().Contain("PostTopic");

        // no subject
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").EvaluateAsync("e => e.removeAttribute('required')");
        await this.Control("YafTextEditor").FillAsync("A topic without a subject.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));
        this.Page.Url.Should().Contain("PostTopic");

        // a taken subject
        await this.PostTopicAsync(subject, "The second topic with the subject.");
        this.Page.Url.Should().Contain("PostTopic");

        // preview
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("YafTextEditor").FillAsync("The text of the preview.");
        await this.ClickAndWaitAsync(this.Control("Preview"));
        (await this.Page.InnerTextAsync("body")).Should().Contain("The text of the preview.", "the preview shows the text");

        // cancel
        await this.ClickAndWaitAsync(this.Control("Cancel"));
        this.Page.Url.Should().Contain("Topics");
    }

    [Test]
    [Description("A topic with the poll option opens the poll editor (the option is for users with the poll access)")]
    public async Task Poll_Option_Test()
    {
        await this.LoginAsAdminAsync();

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync($"With poll {Guid.NewGuid():N}");
        await this.Control("YafTextEditor").FillAsync("A topic with a poll.");
        await this.Control("AddPollCheckBox").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        this.Page.Url.Should().Contain("PollEdit");
    }

    [Test]
    [Description("A second topic within the flood delay is refused")]
    public async Task Flood_Delay_Test()
    {
        await this.LoginAsAdminAsync();
        var delay = await this.SetFloodDelayAsync("60");

        try
        {
            // a new member (the last post of another member could be within the delay)
            var member = $"Fld{DateTime.UtcNow:HHmmssfff}";
            await this.RegisterAsync(member, TestConfig.TestUserPassword);
            await this.LoginAsync(member, TestConfig.TestUserPassword);

            await this.PostTopicAsync($"Flood first {Guid.NewGuid():N}", "The first topic.");
            this.Page.Url.Should().Contain("Posts");

            await this.PostTopicAsync($"Flood second {Guid.NewGuid():N}", "The second topic too fast.");
            this.Page.Url.Should().Contain("PostTopic", "the topic is refused");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetFloodDelayAsync(delay);
        }
    }

    private async Task PostTopicAsync(string subject, string text)
    {
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync(text);
        await this.ClickAndWaitAsync(this.Control("PostReply"));
    }

    /// <summary>
    /// Sets the flood delay of the host settings and returns the old value.
    /// </summary>
    private async Task<string> SetFloodDelayAsync(string value)
    {
        await this.GotoAsync("Admin/HostSettings");

        var old = await this.Control("PostFloodDelay").InputValueAsync();

        await this.Control("PostFloodDelay").EvaluateAsync("(e, v) => e.value = v", value);
        await this.ClickAndWaitAsync(this.Control("Save"));

        return old;
    }
}
