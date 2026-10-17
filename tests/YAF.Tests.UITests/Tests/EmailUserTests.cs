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
/// The e-mail page of a member and the password of a member set by an admin.
/// </summary>
[TestFixture]
public class EmailUserTests : ForumTestBase
{
    [Test]
    [Description("A member sends an e-mail to another member (subject and text are needed, at most 5 per hour), not to themselves, guests can't")]
    public async Task Email_Member_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var sender = $"Ems{id}";
        var recipient = $"Emr{id}";

        await this.RegisterAsync(recipient, TestConfig.TestUserPassword);
        await this.RegisterAsync(sender, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        var recipientId = await this.OpenAdminEditUserAsync(recipient);
        var senderId = await this.OpenAdminEditUserAsync(sender);

        await this.LoginAsync(sender, TestConfig.TestUserPassword);

        // the subject and the text are needed (the browser checks are removed)
        await this.GotoAsync($"Email?u={recipientId}");
        await this.Page.EvaluateAsync("() => document.querySelectorAll('[required]').forEach(e => e.removeAttribute('required'))");
        await this.ClickAndWaitAsync(this.Control("Send"));
        this.Page.Url.Should().Contain("Email?u=");

        await this.Page.EvaluateAsync("() => document.querySelectorAll('[required]').forEach(e => e.removeAttribute('required'))");
        await this.Control("Subject").FillAsync("Only a subject");
        await this.ClickAndWaitAsync(this.Control("Send"));
        this.Page.Url.Should().Contain("Email?u=");

        var started = DateTime.UtcNow;

        for (var i = 1; i <= 5; i++)
        {
            await this.GotoAsync($"Email?u={recipientId}");
            await this.Control("Subject").FillAsync($"Hello {id} {i}");
            await this.Control("Body").FillAsync("The text of the e-mail.");
            await this.ClickAndWaitAsync(this.Control("Send"));

            this.Page.Url.Should().Contain("UserProfile", i.ToString());
        }

        var mail = await TestMail.WaitForAsync(EmailOf(recipient), started);
        mail.Headers.Should().Contain($"Hello {id}");

        // the sixth e-mail of the hour is refused
        await this.GotoAsync($"Email?u={recipientId}");
        await this.Control("Subject").FillAsync($"Hello {id} 6");
        await this.Control("Body").FillAsync("The text of the e-mail.");
        await this.ClickAndWaitAsync(this.Control("Send"));
        this.Page.Url.Should().Contain("Email?u=");

        // not to themselves, an unknown member or as guest
        foreach (var userId in new[] { senderId, 999999 })
        {
            await this.GotoAsync($"Email?u={userId}");
            this.Page.Url.Should().NotContain("Email?u=", userId.ToString());
        }

        await this.LogoutAsync();
        await this.GotoAsync($"Email?u={recipientId}");
        this.Page.Url.Should().NotContain("Email?u=");
    }

    [Test]
    [Description("The admin sets the password of a member (with the e-mail to the member), the member logs in with it")]
    public async Task Admin_Reset_Password_Test()
    {
        var member = $"Rsp{DateTime.UtcNow:HHmmssfff}";
        const string NewPassword = "N3w!Passw0rd";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.OpenAdminEditUserAsync(member);

        // a too short password is refused
        await this.Page.Locator("a.nav-link[href='#View7']").ClickAsync();
        await this.Control("txtNewPassword").FillAsync("a");
        await this.Control("txtConfirmPassword").FillAsync("a");
        await this.ClickAndWaitAsync(this.Control("ChangePassword"));
        await this.AssertNoServerErrorAsync();

        var started = DateTime.UtcNow;

        await this.Page.Locator("a.nav-link[href='#View7']").ClickAsync();
        await this.Control("txtNewPassword").FillAsync(NewPassword);
        await this.Control("txtConfirmPassword").FillAsync(NewPassword);
        await this.Control("chkEmailNotify").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("ChangePassword"));

        var mail = await TestMail.WaitForAsync(EmailOf(member), started);
        mail.Body.Should().Contain(NewPassword);

        // without the e-mail
        await this.Page.Locator("a.nav-link[href='#View7']").ClickAsync();
        await this.Control("txtNewPassword").FillAsync(NewPassword);
        await this.Control("txtConfirmPassword").FillAsync(NewPassword);
        await this.ClickAndWaitAsync(this.Control("ChangePassword"));
        await this.AssertNoServerErrorAsync();

        await this.LoginAsync(member, NewPassword);
        (await this.IsLoggedInAsAsync(member)).Should().BeTrue();
    }
}
