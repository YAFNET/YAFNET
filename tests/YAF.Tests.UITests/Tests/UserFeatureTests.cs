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
/// Private messages, the profile, the signature and the password of a member.
/// </summary>
[TestFixture]
public class UserFeatureTests : ForumTestBase
{
    [Test]
    [Description("A member sends a private message, the recipient finds it in the inbox")]
    public async Task Private_Message_Test()
    {
        var subject = $"PM {Guid.NewGuid():N}";

        // the recipient exists
        await this.LoginAsUserAsync(TestConfig.TestUserName2);

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("PostPrivateMessage");

        await this.Control("To").FillAsync(TestConfig.TestUserName2);
        await this.Control("PmSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("Hello from the UI test.");

        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.LoginAsUserAsync(TestConfig.TestUserName2);

        await this.GotoAsync("MyMessages");

        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);
    }

    [Test]
    [Description("The profile of a member is saved and shown on the profile page")]
    public async Task Edit_Profile_Test()
    {
        var city = $"City{Guid.NewGuid():N}".Substring(0, 20);

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("Profile/EditProfile");

        await this.Control("ProfileEditor_Realname").FillAsync("Test User Real Name");
        await this.Control("ProfileEditor_City").FillAsync(city);

        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        await this.GotoAsync("Profile/EditProfile");

        (await this.Control("ProfileEditor_City").InputValueAsync()).Should().Be(city);
        (await this.Control("ProfileEditor_Realname").InputValueAsync()).Should().Be("Test User Real Name");
    }

    [Test]
    [Description("A profile homepage longer than its column is refused with a message (it failed with an error)")]
    public async Task Too_Long_Homepage_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("Profile/EditProfile");

        // the input has a maximum length, a manipulated form can send more
        await this.Page.EvaluateAsync(
            "() => { const e = document.querySelector(\"[id$='_ProfileEditor_HomePage']\"); e.removeAttribute('maxlength'); e.type = 'text'; }");
        await this.Control("ProfileEditor_HomePage").FillAsync($"https://example.com/{new string('a', 300)}");

        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        this.Page.Url.Should().Contain("Profile/EditProfile");
    }

    [Test]
    [Description("The signature of a member is saved")]
    public async Task Signature_Test()
    {
        var signature = $"My signature {Guid.NewGuid():N}";

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("Profile/EditSignature");

        await this.Control("SignatureEditor_YafTextEditor").FillAsync(signature);
        await this.ClickAndWaitAsync(this.Control("SignatureEditor_save"));

        await this.GotoAsync("Profile/EditSignature");

        (await this.Control("SignatureEditor_YafTextEditor").InputValueAsync()).Should().Be(signature);
    }

    [Test]
    [Description("A member changes the password, the new password works and the old one not anymore")]
    public async Task Change_Password_Test()
    {
        var userName = $"Pwd{DateTime.UtcNow:HHmmssfff}";
        const string NewPassword = "NewPassword1234!";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);
        await this.LoginAsync(userName, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/ChangePassword");

        await this.Control("CurrentPassword").FillAsync(TestConfig.TestUserPassword);
        await this.Control("NewPassword").FillAsync(NewPassword);
        await this.Control("ConfirmNewPassword").FillAsync(NewPassword);

        await this.ClickAndWaitAsync(this.Control("ChangePasswordPushButton"));

        await this.LoginAsync(userName, TestConfig.TestUserPassword);
        (await this.IsLoggedInAsAsync(userName)).Should().BeFalse("the old password does not work anymore");

        await this.LoginAsync(userName, NewPassword);
        (await this.IsLoggedInAsAsync(userName)).Should().BeTrue();
    }

    [Test]
    [Description("Changing the own e-mail address needs the password and the confirmation of the new address")]
    public async Task Change_Email_Test()
    {
        var userName = $"Mail{DateTime.UtcNow:HHmmssfff}";
        var newEmail = $"new.{userName.ToLowerInvariant()}@localhost.com";

        await this.RegisterAsync(userName, TestConfig.TestUserPassword);
        await this.LoginAsync(userName, TestConfig.TestUserPassword);

        // a wrong password is refused
        await this.GotoAsync("Profile/EditSettings");
        await this.Control("ProfileSettings_Email").FillAsync(newEmail);
        await this.Control("ProfileSettings_CurrentPassword").FillAsync("wrong password");
        await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));

        await this.GotoAsync("Profile/EditSettings");
        (await this.Control("ProfileSettings_Email").InputValueAsync()).Should().Be(EmailOf(userName));

        // with the password the confirmation link is sent to the new address
        var started = DateTime.UtcNow;

        await this.Control("ProfileSettings_Email").FillAsync(newEmail);
        await this.Control("ProfileSettings_CurrentPassword").FillAsync(TestConfig.TestUserPassword);
        await this.ClickAndWaitAsync(this.Control("ProfileSettings_UpdateProfile"));

        await this.GotoAsync("Profile/EditSettings");
        (await this.Control("ProfileSettings_Email").InputValueAsync()).Should().Be(
            EmailOf(userName),
            "the address is changed after the confirmation");

        var mail = await TestMail.WaitForAsync(newEmail, started);

        await this.GotoAsync(mail.GetLink("Approve"));

        await this.GotoAsync("Profile/EditSettings");
        (await this.Control("ProfileSettings_Email").InputValueAsync()).Should().Be(newEmail);
    }
}
