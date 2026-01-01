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

using System.Linq;

using YAF.Types.Constants;

namespace YAF.Tests.AdminTests;

/// <summary>
/// The admin pages tests
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AdminPagesTests : TestBase
{
    /// <summary>
    /// Basic test to check if all admin pages load without error
    /// </summary>
    [Test]
    public Task AdminPagesLoadTest()
    {
        return this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.Base.TestSettings,
                        this.Base.TestSettings.AdminUserName,
                        this.Base.TestSettings.AdminPassword), Is.True,
                    "Login failed");


                foreach (var pageName in Enum.GetNames<ForumPages>().Where(x =>
                             x.StartsWith("Admin_") && !x.Equals("Admin_PageAccessEdit") &&
                             !x.Equals("Admin_EditForum") && !x.Equals("Admin_DeleteForum") &&
                             !x.Equals("Admin_EditLanguage")
                             && !x.Equals("Admin_EditUser")
                         ))
                {
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}{pageName.Replace("_", "/")}");

                    var pageSource = await page.ContentAsync();

                    page.Console += (_, msg) =>
                    {
                        Assert.That(msg.Type, Does.Not.Match("error"), $"Error on page: {pageName}");
                    };

                    Assert.That(pageSource, Does.Contain("Administration"), $"Error on page: {pageName}");
                }
            },
            this.BrowserType);
    }
}