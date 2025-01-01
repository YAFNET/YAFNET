/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Tests.UserTests.UserSettings;

/// <summary>
/// The Signature tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SignatureTests : TestBase
{
    /// <summary>
    /// Change the user signature test.
    /// </summary>
    [Test]
    public async Task ChangeUserSignatureTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login failed");

                    // Do actual test
                    await page.GotoAsync($"{this.Base.TestSettings.TestForumUrl}Profile/EditSignature");

                    var pageSource = await page.ContentAsync();

                    Assert.That(
                        pageSource, Does.Contain("Edit Signature"),
                        "Edit Signature is not available for that User");

                    await page.Locator(".BBCodeEditor").FillAsync("This is a Test Signature created by an Unit Test.");

                    await page.Locator("//*[contains(@formaction,'Preview')]").ClickAsync();

                    var previewCell = await page.Locator(".card-body").First.TextContentAsync();

                    Assert.That(
                        previewCell, Is.EqualTo("This is a Test Signature created by an Unit Test."),
                        "Signature changing failed");
                },
            this.BrowserType);
    }
}