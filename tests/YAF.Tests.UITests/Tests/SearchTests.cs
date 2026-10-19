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
/// The filters of the search page (user and tag) and a failed search.
/// </summary>
[TestFixture]
public class SearchTests : ForumTestBase
{
    [Test]
    [Description("The user and the tag filter together (without a search text) find only the topics of the user with the tag")]
    public async Task User_And_Tag_Filter_Test()
    {
        var tag = $"uisearch{Guid.NewGuid():N}".Substring(0, 16);

        await this.LoginAsAdminAsync();
        await this.PostTaggedTopicAsync($"Admin topic {tag}", tag);

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.PostTaggedTopicAsync($"Member topic {tag}", tag);

        await this.SearchAsync(string.Empty, TestConfig.TestUserName, tag);

        await this.WaitForResultAsync($"Member topic {tag}");
        (await this.ResultsTextAsync()).Should().NotContain($"Admin topic {tag}");

        await this.AssertNoServerErrorAsync();
    }

    [Test]
    [Description("A tag with a space is searched as phrase: a topic with only one of the words is not found")]
    public async Task Tag_Phrase_Test()
    {
        var word = $"uiphrase{Guid.NewGuid():N}".Substring(0, 16);

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.PostTaggedTopicAsync($"Phrase topic {word}", $"{word} first");
        await this.PostTaggedTopicAsync($"Other topic {word}", $"{word} second");

        await this.SearchAsync(string.Empty, string.Empty, $"{word} first");

        await this.WaitForResultAsync($"Phrase topic {word}");
        (await this.ResultsTextAsync()).Should().NotContain($"Other topic {word}");
    }

    [Test]
    [Description("A failed search closes the loading dialog and shows the error, the next search shows its results")]
    public async Task Failed_Search_Test()
    {
        var word = $"uifailed{Guid.NewGuid():N}".Substring(0, 16);

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.PostTaggedTopicAsync($"Failed search topic {word}", string.Empty);

        await this.Page.RouteAsync("**/api/Search/GetSearchResults", route => route.FulfillAsync(new RouteFulfillOptions { Status = 500, Body = "error" }));

        try
        {
            await this.SearchAsync(word, string.Empty, string.Empty);

            await this.Page.Locator("#loadModal").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden });
            await this.Page.Locator("#SearchResultsPlaceholder", new PageLocatorOptions { HasTextString = "SyntaxError" })
                .WaitForAsync();
        }
        finally
        {
            await this.Page.UnrouteAsync("**/api/Search/GetSearchResults");
        }

        await this.Control("GoSearch").ClickAsync();

        await this.WaitForResultAsync($"Failed search topic {word}");
        (await this.Page.Locator("#SearchResultsPlaceholder").IsVisibleAsync()).Should().BeTrue();
    }

    private async Task PostTaggedTopicAsync(string subject, string tags)
    {
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync($"The text of {subject}.");
        await this.Control("TagsValue").EvaluateAsync("(e, v) => e.value = v", tags);
        await this.ClickAndWaitAsync(this.Control("PostReply"));
    }

    private async Task SearchAsync(string text, string user, string tag)
    {
        await this.GotoAsync("Search");

        await this.Control("searchInput").FillAsync(text);

        // the filters are in the options drop down
        await this.Page.Locator(".searchUserInput").EvaluateAsync("(e, v) => e.value = v", user);
        await this.Page.Locator(".searchTagInput").EvaluateAsync("(e, v) => e.value = v", tag);

        // the results are loaded with script
        await this.Control("GoSearch").ClickAsync();
    }

    private Task WaitForResultAsync(string subject)
    {
        return this.Page.Locator("#SearchResultsPlaceholder .topicTitle", new PageLocatorOptions { HasTextString = subject })
            .WaitForAsync(new LocatorWaitForOptions { Timeout = 30000 });
    }

    private Task<string> ResultsTextAsync()
    {
        return this.Page.InnerTextAsync("#SearchResultsPlaceholder");
    }
}
