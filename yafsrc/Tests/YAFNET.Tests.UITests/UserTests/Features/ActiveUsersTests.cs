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

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.UserTests.Features;

/// <summary>
/// The active users list tests.
/// </summary>
public class ActiveUsersTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A member can change the page size of the active users list
    /// (it was refused for users without upload rights).
    /// </summary>
    [Test]
    public async Task ChangeActiveUsersPageSizeTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.TestUserName,
                            this.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}ActiveUsers/0");

                    Assert.That(await page.ContentAsync(), Does.Contain("Active Users"), "Active users list is not shown");

                    var size = page.Locator("select#Size");
                    var other = await size.Locator("option:not(:checked)").First.GetAttributeAsync("value");

                    // the form is sent by the onchange of the select
                    await page.EvaluateAsync("() => window.yafTestOldPage = true");
                    await size.SelectOptionAsync([other]);
                    await page.WaitForFunctionAsync("() => !window.yafTestOldPage && document.readyState === 'complete'");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Not.Contain("You tried to enter an area where you didn't have access."),
                        "Changing the page size was denied");

                    Assert.That(
                        await page.Locator("select#Size").InputValueAsync(), Is.EqualTo(other),
                        "The page size was not changed");
                });
    }
}
