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
/// The digest page of the admin: a test digest and the forced send.
/// </summary>
[TestFixture]
public class DigestTests : ForumTestBase
{
    [Test]
    [Description("A test digest needs an address and is sent to it, the digest send is forced")]
    public async Task Test_Digest_And_Force_Send_Test()
    {
        var address = $"digest{Guid.NewGuid():N}".Substring(0, 18) + "@example.com";

        await this.LoginAsAdminAsync();

        // no address
        await this.GotoAsync("Admin/Digest");
        await this.ClickAndWaitAsync(this.Control("TestSend"));
        await this.AssertNoServerErrorAsync();
        (await this.Page.ContentAsync()).Should().Contain("Please enter a valid email address to send a test.");

        var started = DateTime.UtcNow;

        await this.Control("TextSendEmail").FillAsync(address);
        await this.ClickAndWaitAsync(this.Control("TestSend"));
        await this.AssertNoServerErrorAsync();
        (await this.Page.ContentAsync()).Should().Contain("successfully");

        await TestMail.WaitForAsync(address, started);

        // the forced send
        await this.ClickAndWaitAsync(this.Page.Locator("a, button", new PageLocatorOptions { HasTextString = "Force Digest Send" }).First);
        await this.AssertNoServerErrorAsync();
        (await this.Page.ContentAsync()).Should().Contain("Digest Send Scheduled for Immediate Sending");
    }
}
