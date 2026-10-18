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
/// The menus and the topic navigation of the posts page.
/// </summary>
[TestFixture]
public class TopicMenuTests : ForumTestBase
{
    [Test]
    [Description("The tools menu prints the topic, the newest topic has no next topic, the previous topic is opened, guests can't e-mail the topic")]
    public async Task Tools_Next_Previous_And_Share_Test()
    {
        var subject = $"Menus {Guid.NewGuid():N}";

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("The newest topic.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        var topicUrl = this.Page.Url;

        // print (tools menu)
        await this.Control("OptionsLink").ClickAsync();
        await this.MenuItem("Print").ClickAsync();
        await this.Page.WaitForURLAsync(url => url.Contains("PrintTopic"));

        // the newest topic has no next topic
        await this.Page.GotoAsync(topicUrl);
        await this.ClickAndWaitAsync(this.Control("NextTopic"));
        await this.AssertNoServerErrorAsync();
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject, "the topic is still shown");

        // but a previous one
        await this.ClickAndWaitAsync(this.Control("PrevTopic"));
        await this.AssertNoServerErrorAsync();
        (await this.Control("TopicTitle").InnerTextAsync()).Should().NotContain(subject);

        // guests are asked to log in to send the topic (the share menu is shown to everyone)
        await this.LoginAsAdminAsync();
        await this.SetShareTopicToAsync("2");

        try
        {
            await this.LogoutAsync();
            await this.Page.GotoAsync(topicUrl);
            await this.Control("ShareLink").ClickAsync();
            await this.Page.RunAndWaitForResponseAsync(
                () => this.MenuItem("Email this topic").ClickAsync(),
                response => response.Request.Method == "POST");
            await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await this.AssertNoServerErrorAsync();
            this.Page.Url.Should().NotContain("EmailTopic");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetShareTopicToAsync("1");
        }
    }

    [Test]
    [Description("The ad is shown below the first post (also to members with the setting), the tools menu opens the atom feed of the topic")]
    public async Task Ad_And_Atom_Feed_Test()
    {
        var subject = $"Ads {Guid.NewGuid():N}";
        var ad = $"Advertisement {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();
        await this.SetAdAndAtomAsync(ad, true);

        try
        {
            await this.LoginAsUserAsync(TestConfig.TestUserName);
            await this.GotoAsync("PostTopic?f=1");
            await this.Control("TopicSubjectTextBox").FillAsync(subject);
            await this.Control("YafTextEditor").FillAsync("A topic with an ad.");
            await this.ClickAndWaitAsync(this.Control("PostReply"));

            (await this.Page.InnerTextAsync("body")).Should().Contain(ad);

            await this.Control("OptionsLink").ClickAsync();
            await this.MenuItem("Atom Feed").ClickAsync();
            await this.Page.WaitForURLAsync(url => url.Contains("Feed"));
            await this.AssertNoServerErrorAsync();
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetAdAndAtomAsync(string.Empty, false);
        }
    }

    private async Task SetAdAndAtomAsync(string ad, bool enabled)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Control("AdPost").EvaluateAsync("(e, v) => e.value = v", ad);
        await this.Control("ShowAdsToSignedInUsers").EvaluateAsync("(e, v) => e.checked = v", enabled);
        await this.Control("ShowAtomLink").EvaluateAsync("(e, v) => e.checked = v", enabled);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    private async Task SetShareTopicToAsync(string value)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Control("ShowShareTopicTo").SelectOptionAsync(value, new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    private ILocator MenuItem(string text)
    {
        return this.Page.Locator(".dropdown-menu.show a", new PageLocatorOptions { HasTextString = text }).First;
    }
}
