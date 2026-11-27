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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.HostIntegration;

using YAF.Tests.HostIntegration.TestInfrastructure;

/// <summary>
/// The same application without an identity setup of its own: the forum sets up identity and stores it in its database.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ForumOwnedIdentityTests() : HostIntegrationTests(HostIdentityMode.Forum)
{
    [Test]
    public async Task Guest_Opens_The_Account_Pages_Of_The_Forum()
    {
        using var browser = this.Host.CreateBrowser();

        foreach (var path in new[] { "/search", "/Account/Login", "/Account/Register" })
        {
            using var response = await browser.GetAsync(path);

            await ShouldBeOkAsync(response);
        }
    }

    [Test]
    public async Task Signed_In_Admin_Opens_The_Password_Page_Of_The_Forum()
    {
        using var browser = await this.SignInAsync(HostApplication.AdminName);

        await PagesShouldShowAsync(browser, ["/Profile/ChangePassword"], HostApplication.AdminName);
    }
}
