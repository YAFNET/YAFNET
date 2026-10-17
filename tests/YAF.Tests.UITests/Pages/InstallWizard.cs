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

namespace YAF.Tests.UITests.Pages;

using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The install wizard (install/default.aspx): welcome, permissions, database connection, test settings,
/// database setup, create forum (board and admin) and finished.
/// </summary>
public class InstallWizard(IPage page)
{
    /// <summary>
    /// The maximum time of a step (the database setup creates all tables).
    /// </summary>
    private const float StepTimeout = 300000;

    /// <summary>
    /// Runs all steps of the wizard.
    /// </summary>
    public async Task InstallAsync()
    {
        page.SetDefaultTimeout(StepTimeout);
        page.SetDefaultNavigationTimeout(StepTimeout);

        await page.GotoAsync("install/default.aspx");

        for (var step = 0; step < 15; step++)
        {
            // the wizard is a dialog that the script of the page shows after the page is loaded
            await page.Locator("[id$='StartNextButton'], [id$='StepNextButton'], [id$='FinishButton']").First
                .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            await this.ThrowOnErrorAsync();

            var heading = page.Locator("h4").First;

            TestContext.Progress.WriteLine(
                $"[install] step {step}: {(await heading.CountAsync() > 0 ? (await heading.InnerTextAsync()).Trim() : "-")}");


            var finish = page.Locator("[id$='FinishButton']");

            if (await finish.IsVisibleAsync())
            {
                await page.RunAndWaitForNavigationAsync(() => finish.ClickAsync());

                return;
            }

            if (await page.Locator("[id$='TheForumName']").IsVisibleAsync())
            {
                await this.FillCreateForumAsync();
            }

            var next = page.Locator("[id$='StartNextButton'], [id$='StepNextButton']").First;

            // the buttons post back with __doPostBack: the next step is checked after the new page is loaded
            await page.RunAndWaitForNavigationAsync(() => next.ClickAsync());
        }

        throw new InvalidOperationException($"The install wizard did not finish, the last page was {page.Url}.");
    }

    /// <summary>
    /// The board and the admin of the forum.
    /// </summary>
    private async Task FillCreateForumAsync()
    {
        await page.Locator("[id$='TheForumName']").FillAsync(TestConfig.BoardName);
        await page.Locator("[id$='ForumEmailAddress']").FillAsync(TestConfig.ForumEmail);
        await page.Locator("[id$='ForumBaseUrlMask']").FillAsync(TestConfig.BaseUrl);
        await page.Locator("[id$='UserName']").FillAsync(TestConfig.AdminUserName);
        await page.Locator("[id$='AdminEmail']").FillAsync(TestConfig.AdminEmail);
        await page.Locator("[id$='Password1']").FillAsync(TestConfig.AdminPassword);
        await page.Locator("[id$='Password2']").FillAsync(TestConfig.AdminPassword);
    }

    /// <summary>
    /// The errors of the wizard are shown in an alert.
    /// </summary>
    private async Task ThrowOnErrorAsync()
    {
        var error = page.Locator(".alert-warning, .alert-danger").Filter(new LocatorFilterOptions { Visible = true });

        if (await error.CountAsync() > 0)
        {
            var text = (await error.First.InnerTextAsync()).Trim();

            if (text.Length > 0)
            {
                throw new InvalidOperationException($"The install wizard showed an error: {text}");
            }
        }

        var content = await page.ContentAsync();

        if (content.Contains("Server Error in '/' Application"))
        {
            throw new InvalidOperationException($"The install wizard failed: {await page.InnerTextAsync("body")}");
        }
    }
}
