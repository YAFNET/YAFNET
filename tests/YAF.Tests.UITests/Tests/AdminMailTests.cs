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
/// The mail page of the admin: an e-mail to the members of a role and the test mail.
/// </summary>
[TestFixture]
public class AdminMailTests : ForumTestBase
{
    [Test]
    [Description("An e-mail is sent to the members of a role and a test mail to an address")]
    public async Task Mail_To_Role_And_Test_Mail_Test()
    {
        var id = Guid.NewGuid().ToString("N").Substring(0, 12);
        var address = $"smtptest{id}@example.com";

        await this.LoginAsAdminAsync();

        // the members of the administrators role
        var started = DateTime.UtcNow;

        await this.GotoAsync("Admin/Mail");
        await this.Control("ToList").SelectOptionAsync(
            new SelectOptionValue { Label = "Administrators" },
            new LocatorSelectOptionOptions { Force = true });
        await this.Control("Subject").FillAsync($"Newsletter {id}");
        await this.Control("Body").FillAsync("The text of the newsletter.");
        // confirmed in a dialog
        await this.Control("Send").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        await this.AssertNoServerErrorAsync();

        (await this.Control("Subject").InputValueAsync()).Should().BeEmpty("the form is cleared after the e-mail was queued");

        var mail = await TestMail.WaitForAsync(TestConfig.AdminEmail, started);
        mail.Headers.Should().Contain($"Newsletter {id}");

        // the test mail
        started = DateTime.UtcNow;

        await this.Control("TestToEmail").FillAsync(address);
        await this.ClickAndWaitAsync(this.Control("TestSmtp"));
        await this.AssertNoServerErrorAsync();

        await TestMail.WaitForAsync(address, started);
    }
}
