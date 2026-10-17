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
/// The friend requests and the friend lists of the members.
/// </summary>
[TestFixture]
public class FriendsTests : ForumTestBase
{
    [Test]
    [Description("A friend request is approved: both members are friends, a removed friend is not in the list anymore")]
    public async Task Request_Approve_And_Remove_Test()
    {
        var (alice, bob) = await this.RegisterTwoAsync();

        await this.SendRequestAsync(alice, bob);

        // the request of alice
        await this.GotoFriendsAsync("YourRequestsTab");
        (await this.Tab("YourRequestsTab").InnerTextAsync()).Should().Contain(bob);

        // bob approves it
        await this.LoginAsync(bob, TestConfig.TestUserPassword);
        await this.GotoFriendsAsync("PendingRequestsTab");
        await this.ClickAndWaitAsync(this.Tab("PendingRequestsTab").Locator("a", new LocatorLocatorOptions { HasTextString = "Approve" }).First);

        await this.GotoFriendsAsync("BuddyListTab");
        (await this.Tab("BuddyListTab").InnerTextAsync()).Should().Contain(alice);

        await this.LoginAsync(alice, TestConfig.TestUserPassword);
        await this.GotoFriendsAsync("BuddyListTab");
        (await this.Tab("BuddyListTab").InnerTextAsync()).Should().Contain(bob);

        // alice removes bob
        await this.Tab("BuddyListTab").Locator("[id$='_lnkRemove']").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.GotoFriendsAsync("BuddyListTab");
        (await this.Tab("BuddyListTab").InnerTextAsync()).Should().NotContain(bob);
    }

    [Test]
    [Description("A denied request and a removed own request are gone")]
    public async Task Deny_And_Remove_Request_Test()
    {
        var (alice, bob) = await this.RegisterTwoAsync();

        await this.SendRequestAsync(alice, bob);

        // bob denies it
        await this.LoginAsync(bob, TestConfig.TestUserPassword);
        await this.GotoFriendsAsync("PendingRequestsTab");
        await this.Tab("PendingRequestsTab").Locator("a", new LocatorLocatorOptions { HasTextString = "Deny" }).First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.GotoFriendsAsync("PendingRequestsTab");
        (await this.Tab("PendingRequestsTab").InnerTextAsync()).Should().NotContain(alice);

        // alice asks again and takes the request back
        await this.SendRequestAsync(alice, bob);

        await this.GotoFriendsAsync("YourRequestsTab");
        await this.ClickAndWaitAsync(this.Tab("YourRequestsTab").Locator("[id$='_RequestRemove']").First);

        await this.GotoFriendsAsync("YourRequestsTab");
        (await this.Tab("YourRequestsTab").InnerTextAsync()).Should().NotContain(bob);

        await this.LoginAsync(bob, TestConfig.TestUserPassword);
        await this.GotoFriendsAsync("PendingRequestsTab");
        (await this.Tab("PendingRequestsTab").InnerTextAsync()).Should().NotContain(alice);
    }

    [Test]
    [Description("Approve all approves all received requests")]
    public async Task Approve_All_Test()
    {
        var (alice, bob) = await this.RegisterTwoAsync();
        var carol = $"Carol{DateTime.UtcNow:HHmmssfff}";
        await this.RegisterAsync(carol, TestConfig.TestUserPassword);

        await this.SendRequestAsync(alice, carol);
        await this.SendRequestAsync(bob, carol);

        await this.LoginAsync(carol, TestConfig.TestUserPassword);
        await this.GotoFriendsAsync("PendingRequestsTab");
        await this.Tab("PendingRequestsTab").Locator("[id$='_Button3']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.GotoFriendsAsync("BuddyListTab");
        var friends = await this.Tab("BuddyListTab").InnerTextAsync();
        friends.Should().Contain(alice).And.Contain(bob);
    }

    private async Task<(string Alice, string Bob)> RegisterTwoAsync()
    {
        var alice = $"Alice{DateTime.UtcNow:HHmmssfff}";
        var bob = $"Bob{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(alice, TestConfig.TestUserPassword);
        await this.RegisterAsync(bob, TestConfig.TestUserPassword);

        return (alice, bob);
    }

    /// <summary>
    /// The first user asks the second user to be friends (on the profile of the second user).
    /// </summary>
    private async Task SendRequestAsync(string from, string to)
    {
        await this.LoginAsync(to, TestConfig.TestUserPassword);
        var href = await this.Page.Locator("a[href*='UserProfile']").First.GetAttributeAsync("href");
        var userId = Regex.Match(href, @"UserProfile/(\d+)").Groups[1].Value;

        await this.LoginAsync(from, TestConfig.TestUserPassword);
        await this.GotoAsync($"UserProfile/{userId}-{to}");

        await this.ClickAndWaitAsync(this.Control("lnkBuddy"));
    }

    private async Task GotoFriendsAsync(string tab)
    {
        await this.GotoAsync("Friends");
        await this.Page.Locator($"a.nav-link[href='#{tab}']").ClickAsync();
    }

    private ILocator Tab(string tab)
    {
        return this.Page.Locator($"#{tab}");
    }
}
