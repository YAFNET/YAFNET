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

using AwesomeAssertions;

using YAF.Tests.UITests.Infrastructure;
using YAF.Types.Constants;

namespace YAF.Tests.UITests.AdminTests;

/// <summary>
/// The admin pages tests
/// </summary>
public class AdminPagesTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Basic test to check if all admin pages load without error
    /// </summary>
    [Test]
    public Task AdminPagesLoadTest()
    {
        return this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");


                foreach (var pageName in Enum.GetNames<ForumPages>().Where(x =>
                             x.StartsWith("Admin_") && !x.Equals("Admin_PageAccessEdit") &&
                             !x.Equals("Admin_EditForum") && !x.Equals("Admin_DeleteForum") &&
                             !x.Equals("Admin_EditLanguage")
                             && !x.Equals("Admin_EditUser") && !x.Equals("Admin_TestData")
                         ))
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}{pageName.Replace("_", "/")}");

                    await Task.Delay(5000);

                    var pageSource = await page.ContentAsync();

                    page.Console += (_, msg) =>
                    {
                        Assert.That(msg.Type, Does.Not.Match("error"), $"Error on page: {pageName} - error: {msg.Text}");
                    };

                    Assert.That(pageSource, Does.Contain("Administration"), $"Error on page: {pageName}");
                }
            });
    }

    /// <summary>
    /// test to check if the bbcode extensions are installed
    /// </summary>
    [Test]
    public Task BBCodeExtensionsInstalledTest()
    {
        return this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                // Log user in first!
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");


                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/BBCodes/");

                var pageSource = await page.ContentAsync();


                pageSource.Should().Contain("ALBUMIMG");
            });
    }

    /// <summary>
    /// The language editor only opens files of the languages folder (the file name comes from the address).
    /// </summary>
    [Test]
    public async Task EditLanguageOnlyLanguageFilesTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditLanguage/english.json");

                Assert.That(await page.ContentAsync(), Does.Contain("Language Translator"), "The language file can not be edited");

                // a file name from the form is used before the one of the address
                await page.EvaluateAsync(
                    """
                    () => {
                        const input = document.createElement('input');
                        input.type = 'hidden';
                        input.name = 'x';
                        input.value = '../../appsettings.json';
                        document.querySelector('button[formaction*="Save"]').form.appendChild(input);
                    }
                    """);

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("You've passed an invalid value to the forum."),
                    "A file outside the languages folder was saved");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditLanguage/missing.json");

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("You've passed an invalid value to the forum."),
                    "A missing language file was opened");
            });
    }

    /// <summary>
    /// The page numbers of the lists on the admin start page come from the url, 0 or a page after the
    /// last one (e.g. a bookmark when fewer users are online) failed with an error.
    /// </summary>
    [Test]
    public Task AdminStartPageWithInvalidPageNumbersTest()
    {
        return this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                Assert.That(
                    await page.LoginUserAsync(
                        this.TestSettings,
                        this.TestSettings.AdminUserName,
                        this.TestSettings.AdminPassword), Is.True,
                    "Login failed");

                foreach (var pages in new[] { "0/0", "2/2", "2147483647/2147483647" })
                {
                    var response = await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Admin/{pages}");

                    Assert.That(response?.Status, Is.EqualTo(200), $"The admin start page failed for the pages {pages}");
                }
            });
    }
}
