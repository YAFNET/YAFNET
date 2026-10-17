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
/// The durations of the admin: the board announcement and the suspension of a user (a huge number is limited).
/// </summary>
[TestFixture]
public class DurationTests : ForumTestBase
{
    [Test]
    [Description("A board announcement with a huge or a negative duration is saved and shown, then deleted")]
    public async Task Board_Announcement_Test()
    {
        var text = $"Announcement {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();

        foreach (var (count, unit) in new[] { ("99999999", "3"), ("-5", "1") })
        {
            await this.GotoAsync("Admin/BoardAnnouncement");
            await this.Page.Locator("textarea[id$='_Message']").FillAsync(text);
            await this.Control("BoardAnnouncementUntil").EvaluateAsync("e => e.removeAttribute('min')");
            await this.Control("BoardAnnouncementUntil").FillAsync(count);
            await this.Page.Locator($"[id$='_BoardAnnouncementUntilUnit'] input[value='{unit}']").CheckAsync();
            await this.ClickAndWaitAsync(this.Control("SaveAnnouncement"));

            await this.GotoAsync(string.Empty);
            (await this.Page.InnerTextAsync("body")).Should().Contain(text, $"{count} is a valid duration");
        }

        await this.GotoAsync("Admin/BoardAnnouncement");
        await this.ClickAndWaitAsync(this.Page.Locator("a[aria-label='trash']").First);

        await this.GotoAsync(string.Empty);
        (await this.Page.InnerTextAsync("body")).Should().NotContain(text);
    }

    [Test]
    [Description("A suspension with a huge number of days is limited, the suspension is removed again")]
    public async Task Suspend_Huge_Duration_Test()
    {
        var member = $"Dur{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        var userId = await this.OpenAdminEditUserAsync(member);
        await this.Page.Locator("a.nav-link[href='#View8']").ClickAsync();

        await this.Control("SuspendUserControl_SuspendCount").FillAsync("99999999");
        await this.Page.Locator("[id$='_SuspendUserControl_SuspendUnit'] input[value='1']").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("SuspendUserControl_Suspend"));

        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a.nav-link[href='#View8']").ClickAsync();
        (await this.Control("SuspendUserControl_RemoveSuspension").CountAsync()).Should().Be(1, "the user is suspended");

        await this.ClickAndWaitAsync(this.Control("SuspendUserControl_RemoveSuspension"));

        await this.LoginAsync(member, TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync(member)).Should().BeTrue();
    }
}
