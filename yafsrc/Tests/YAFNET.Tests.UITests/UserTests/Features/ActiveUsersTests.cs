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

    /// <summary>
    /// A visitor coming from a page with a long address (the referer is saved
    /// in the active list, its column holds 500 characters) gets the forum page.
    /// </summary>
    [Test]
    public async Task LongRefererIsShownTest()
    {
        // a new session, so the visitor is inserted into the active list
        await using var guestContext = await this.Fixture.Context.Browser!.NewContextAsync(
                                           new BrowserNewContextOptions
                                               {
                                                   BaseURL = this.TestSettings.TestForumUrl,
                                                   IgnoreHTTPSErrors = true
                                               });

        var page = await guestContext.NewPageAsync();

        var response = await page.GotoAsync(
                           this.TestSettings.TestForumUrl,
                           new PageGotoOptions
                               {
                                   Referer = $"{this.TestSettings.TestForumUrl}Search?search={new string('a', 600)}"
                               });

        Assert.That(response, Is.Not.Null, "No response");

        Assert.That(response!.Ok, Is.True, $"The forum answered {response.Status}");

        Assert.That(await page.ContentAsync(), Does.Not.Contain("Forum Error"), "The error page is shown");
    }

    /// <summary>
    /// The pages of the active users list can be changed (the pager was outside of the form and did nothing).
    /// </summary>
    [Test]
    public async Task ActiveUsersSecondPageTest()
    {
        // more active sessions than one page of 5
        var guests = new System.Collections.Generic.List<IBrowserContext>();

        try
        {
            for (var i = 0; i < 6; i++)
            {
                var guestContext = await this.Fixture.Context.Browser!.NewContextAsync(
                                       new BrowserNewContextOptions
                                           {
                                               BaseURL = this.TestSettings.TestForumUrl,
                                               IgnoreHTTPSErrors = true
                                           });

                guests.Add(guestContext);

                await (await guestContext.NewPageAsync()).GotoAsync(this.TestSettings.TestForumUrl);
            }

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

                        await page.SubmitAndWaitAsync(
                            page.Locator(".pagination .page-link", new PageLocatorOptions { HasTextRegex = new System.Text.RegularExpressions.Regex(@"^\s*2\s*$") }).First);

                        Assert.That(await page.ContentAsync(), Does.Not.Contain("You tried to enter an area where you didn't have access."), "Page 2 was denied");

                        Assert.That(
                            (await page.Locator(".pagination .page-item.active").InnerTextAsync()).Trim(),
                            Is.EqualTo("2"),
                            "Page 2 is not shown");
                    });
        }
        finally
        {
            foreach (var guest in guests)
            {
                await guest.DisposeAsync();
            }
        }
    }
}
