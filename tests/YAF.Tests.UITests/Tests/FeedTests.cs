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

using System.Threading.Tasks;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The atom feeds of the forum: the latest posts, the posts of a topic and the topics of a forum.
/// </summary>
[TestFixture]
public class FeedTests : ForumTestBase
{
    [Test]
    [Description("With the atom links the feeds are atom xml (an invalid feed is the latest posts), the posts feed without a topic is the info page")]
    public async Task Atom_Feeds_Test()
    {
        await this.LoginAsAdminAsync();
        await this.SetAtomLinkAsync(true);

        try
        {
            await this.LoginAsUserAsync(TestConfig.TestUserName);
            await this.GotoAsync(string.Empty);

            foreach (var url in new[] { "Feed?feed=0", "Feed?feed=1&t=1", "Feed?feed=2&f=1", "Feed?feed=notafeed" })
            {
                var response = await this.Page.APIRequest.GetAsync($"{TestConfig.BaseUrl}{url}");

                response.Headers["content-type"].Should().StartWith("application/atom+xml", url);
                (await response.TextAsync()).Should().Contain("<feed", url);
            }

            await this.GotoAsync("Feed?feed=1");
            this.Page.Url.Should().Contain("Info");

            // the atom links of a forum and of a topic (the address of the feed is rewritten)
            await this.GotoAsync("Topics/1-Test-Forum");
            await this.AssertFeedLinkAsync("the forum", "-f1");

            var topic = await this.Page.Locator("a[href*='/Posts/t']").First.GetAttributeAsync("href");
            await this.GotoAsync(topic.TrimStart('/'));
            await this.AssertFeedLinkAsync("the topic", "-t");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetAtomLinkAsync(false);
        }

        // turned off
        await this.GotoAsync("Feed?feed=0");
        this.Page.Url.Should().Contain("Info");
    }

    /// <summary>
    /// The atom link of the page opens an atom feed.
    /// </summary>
    private async Task AssertFeedLinkAsync(string page, string feedOf)
    {
        var href = await this.Page.Locator($"a[href*='Feed'][href*='{feedOf}']").First.GetAttributeAsync("href");
        href.Should().NotBeNull($"{page} has an atom link of {page}");

        var response = await this.Page.APIRequest.GetAsync(new System.Uri(new System.Uri(TestConfig.BaseUrl), href).ToString());

        response.Headers["content-type"].Should().StartWith("application/atom+xml", $"the link {href} of {page}");
        (await response.TextAsync()).Should().Contain("<entry", $"the link {href} of {page}");
    }

    private async Task SetAtomLinkAsync(bool enabled)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Control("ShowAtomLink").EvaluateAsync("(e, v) => e.checked = v", enabled);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }
}
