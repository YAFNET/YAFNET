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

using YAF.Tests.Infrastructure;

namespace YAF.Tests.UserTests.UserSettings;

/// <summary>
/// The delete own account tests.
/// </summary>
public class DeleteAccountTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Deleting the own account needs the current password, afterwards the user can not log in anymore.
    /// </summary>
    [Test]
    public async Task DeleteAccountNeedsPasswordTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    // A wrong password is refused, the enter key also asks for the confirmation
                    await DeleteAccountAsync(page, "WrongPassword123!", true);

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("The password you entered is not correct."),
                        "Deleting with a wrong password was not refused");

                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                        Is.True,
                        "The account was deleted with a wrong password");

                    // the right password deletes the account and logs out
                    await DeleteAccountAsync(page, this.TestSettings.TestUserPassword, false);

                    Assert.That(
                        await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                        Is.False,
                        "The deleted account can still log in");
                });

        async Task DeleteAccountAsync(IPage page, string password, bool pressEnter)
        {
            await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/DeleteAccount");

            await page.Locator("input[name=Option][value=delete]").CheckAsync();
            await page.Locator("#CurrentPassword").FillAsync(password);

            if (pressEnter)
            {
                await page.Locator("#CurrentPassword").PressAsync("Enter");
            }
            else
            {
                await page.Locator("form:has(#CurrentPassword) button[type=submit]").ClickAsync();
            }

            // Are you sure? (the dialog sends the form by script)
            await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));
        }
    }
}
