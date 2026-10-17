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
/// The checks of the signature of a member: allowed bb codes and the length.
/// </summary>
[TestFixture]
public class SignatureTests : ForumTestBase
{
    [Test]
    [Description("A bb code that is not allowed and a too long signature are refused, an empty signature removes it")]
    public async Task Signature_Checks_Test()
    {
        var member = $"Sig{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        // bold is not allowed (members may use url, img, spoiler and quote)
        await this.SaveAsync("[b]bold[/b]");
        (await this.Page.ContentAsync()).Should().Contain("Your signature contains forbidden");

        // more than 128 characters
        await this.SaveAsync(new string('x', 300));
        (await this.Page.ContentAsync()).Should().Contain("Your signature is too long");

        await this.SaveAsync("[url=https://example.com]My site[/url]");
        await this.GotoAsync("Profile/EditSignature");
        (await this.Control("SignatureEditor_YafTextEditor").InputValueAsync()).Should().Contain("My site");

        await this.SaveAsync(string.Empty);
        await this.GotoAsync("Profile/EditSignature");
        (await this.Control("SignatureEditor_YafTextEditor").InputValueAsync()).Should().BeEmpty();
    }

    private async Task SaveAsync(string signature)
    {
        await this.GotoAsync("Profile/EditSignature");

        await this.Control("SignatureEditor_YafTextEditor").EvaluateAsync("e => e.removeAttribute('maxlength')");
        await this.Control("SignatureEditor_YafTextEditor").FillAsync(signature);
        await this.ClickAndWaitAsync(this.Control("SignatureEditor_save"));
    }
}
