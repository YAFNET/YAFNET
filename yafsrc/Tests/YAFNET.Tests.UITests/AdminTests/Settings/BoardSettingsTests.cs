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

namespace YAF.Tests.UITests.AdminTests.Settings;

/// <summary>
/// The Board Settings Tests
/// </summary>
public class BoardSettingsTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A default page size of 0 is not saved (the lists of the new users were empty).
    /// </summary>
    [Test]
    public async Task DefaultPageSizeZeroIsNotSavedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");

                var before = await GetValueAsync(page, "Input.DefaultPageSize");

                await SubmitValueAsync(page, "Input.DefaultPageSize", "0");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");

                Assert.That(await GetValueAsync(page, "Input.DefaultPageSize"), Is.EqualTo(before), "The page size 0 was saved");
            });
    }

    /// <summary>
    /// The hours between the digests can't be 0 or less (the digest was sent on each run of the task).
    /// </summary>
    [Test]
    public async Task DigestHoursBelowOneAreNotSavedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");

                var before = await GetValueAsync(page, "Input.DigestSendEveryXHours");

                await SubmitValueAsync(page, "Input.DigestSendEveryXHours", "-5");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");

                Assert.That(await GetValueAsync(page, "Input.DigestSendEveryXHours"), Is.EqualTo(before), "The negative hours were saved");
            });
    }

    /// <summary>
    /// An empty board name is not saved (it was only checked in the browser).
    /// </summary>
    [Test]
    public async Task EmptyBoardNameIsNotSavedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await page.LoginAdminUserAsync(this.TestSettings);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");

                var before = await GetValueAsync(page, "Input.Name");

                Assert.That(before, Is.Not.Empty);

                await SubmitValueAsync(page, "Input.Name", string.Empty);

                // the settings are shown again with the message (and not an error page)
                Assert.That(page.Url, Does.Contain("Admin/Settings"), "The empty name was not refused");
                Assert.That(await page.ContentAsync(), Does.Contain("You must enter a name for the board."));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Settings");

                Assert.That(await GetValueAsync(page, "Input.Name"), Is.EqualTo(before), "The empty board name was saved");
            });
    }

    /// <summary>
    /// Gets the value of a field of the board settings.
    /// </summary>
    private static Task<string> GetValueAsync(IPage page, string name)
    {
        return page.EvaluateAsync<string>("n => document.querySelector(`[name=\"${n}\"]`).value", name);
    }

    /// <summary>
    /// Saves the board settings with a value the form doesn't offer (as a crafted request would do).
    /// </summary>
    private static async Task SubmitValueAsync(IPage page, string name, string value)
    {
        await page.EvaluateAsync(
            """
            ([n, v]) => {
                const field = document.querySelector(`[name="${n}"]`);
                ['required', 'min', 'max', 'minlength'].forEach(a => field.removeAttribute(a));
                if (field.tagName === 'SELECT') {
                    field.add(new Option(v, v));
                }
                field.value = v;
            }
            """,
            new[] { name, value });

        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
    }
}
