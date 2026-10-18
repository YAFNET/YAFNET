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

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The XMPP (Jabber) page of a member.
/// </summary>
[TestFixture]
public class JabberTests : ForumTestBase
{
    [Test]
    [Description("The XMPP page of a member: for a member without an address, the same and another server, the own page, guests can't")]
    public async Task Jabber_Page_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var he = $"Jbh{id}";
        var me = $"Jbm{id}";

        await this.RegisterAsync(he, TestConfig.TestUserPassword);
        await this.SetXmppAsync($"{he.ToLowerInvariant()}@jabber.example.org");

        await this.RegisterAsync(me, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        var heId = await this.OpenAdminEditUserAsync(he);
        var meId = await this.OpenAdminEditUserAsync(me);

        // the current member has no address (another server)
        await this.LoginAsync(me, TestConfig.TestUserPassword);
        await this.GotoAsync($"Jabber?u={heId}");
        await this.AssertNoServerErrorAsync();
        (await this.Page.InnerTextAsync("body")).Should().Contain("http://jabber.example.org");

        // the member without an address
        await this.LoginAsync(he, TestConfig.TestUserPassword);
        await this.GotoAsync($"Jabber?u={meId}");
        await this.AssertNoServerErrorAsync();
        this.Page.Url.Should().NotContain("Jabber?u=");

        // the own page
        await this.GotoAsync($"Jabber?u={heId}");
        await this.AssertNoServerErrorAsync();
        this.Page.Url.Should().Contain("Jabber?u=");

        // the same server
        await this.LoginAsync(me, TestConfig.TestUserPassword);
        await this.SetXmppAsync($"{me.ToLowerInvariant()}@jabber.example.org");
        await this.GotoAsync($"Jabber?u={heId}");
        (await this.Page.InnerTextAsync("body")).Should().Contain($"{he.ToLowerInvariant()}@jabber.example.org");

        // an unknown member and guests
        await this.GotoAsync("Jabber?u=999999");
        this.Page.Url.Should().NotContain("Jabber?u=");

        await this.LogoutAsync();
        await this.GotoAsync($"Jabber?u={heId}");
        this.Page.Url.Should().NotContain("Jabber?u=");
    }

    private async Task SetXmppAsync(string address)
    {
        await this.GotoAsync("Profile/EditProfile");
        await this.Control("Xmpp").FillAsync(address);
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
    }
}
