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

namespace YAF.Tests.UserTests.Features;

/// <summary>
/// The user Reputation tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Description("The user Reputation tests.")]
public class ReputationTests : TestBase
{
    /// <summary>
    /// Add +1 Reputation from TestUser2 to TestUser Test.
    /// </summary>
    [Test]
    [Description("Add +1 Reputation from TestUser2 to TestUser Test.")]
    public async Task AddReputationToTestUserTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    await Assert.MultipleAsync(async () =>
                    {
                        // Log user in first!
                        Assert.That(
                            await page.LoginUserAsync(
                                this.Base.TestSettings,
                                this.Base.TestSettings.TestUserName,
                                this.Base.TestSettings.TestUserPassword), Is.True,
                            "Login failed");

                        // Do actual test

                        // First Creating a new test topic with the test user
                        Assert.That(
                            await page.CreateNewTestTopicAsync(this.Base.TestSettings), Is.True,
                            "Topic Creating failed");
                    });

                    var newTestTopicUrl = page.Url;

                    // Now Login with Test User 2
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName2,
                            this.Base.TestSettings.TestUser2Password), Is.True,
                        "Login with test user 2 failed");

                    // Go To New Test Topic Url
                    await page.GotoAsync(newTestTopicUrl);

                    Assert.That(
                        await page.Locator("//a[contains(@class,'AddReputation_')]").IsVisibleAsync(), Is.True,
                        "Reputation is deactivated  in yaf or the user has already voted within the last 24 hours, or the user doesn't have enough points to be allowed to vote");

                    await page.Locator("//a[contains(@class,'AddReputation_')]").ClickAsync();

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You have successfully given +1"), "Voting failed");
                },
            this.BrowserType);
    }

    /// <summary>
    /// Add -1 Reputation from TestUser2 to TestUser Test
    /// </summary>
    [Test]
    [Description("Add -1 Reputation from TestUser2 to TestUser Test")]
    public async Task RemoveReputationFromTestUserTest()
    {
        await this.Base.PlaywrightFixture.GotoPageAsync(
            this.Base.TestSettings.TestForumUrl,
            async page =>
                {
                    await Assert.MultipleAsync(async () =>
                    {
                        // Log user in first!
                        Assert.That(
                            await page.LoginUserAsync(
                                this.Base.TestSettings,
                                this.Base.TestSettings.TestUserName2,
                                this.Base.TestSettings.TestUser2Password), Is.True,
                            "Login failed");

                        // Do actual test

                        // First Creating a new test topic with the test user
                        Assert.That(
                            await page.CreateNewTestTopicAsync(this.Base.TestSettings), Is.True,
                            "Topic Creating failed");
                    });

                    var newTestTopicUrl = page.Url;

                    // Now Login with Test User 2
                    Assert.That(
                        await page.LoginUserAsync(
                            this.Base.TestSettings,
                            this.Base.TestSettings.TestUserName,
                            this.Base.TestSettings.TestUserPassword), Is.True,
                        "Login with test user 2 failed");

                    // Go To New Test Topic Url
                    await page.GotoAsync(newTestTopicUrl);

                    Assert.That(
                        await page.Locator("//a[contains(@class,'RemoveReputation_')]").IsVisibleAsync(), Is.True,
                        "Reputation is deactivated (Or negative Reputation) in yaf or the user has already voted within the last 24 hours, or the user doesn't have enough points to be allowed to vote");

                    await page.Locator("//a[contains(@class,'RemoveReputation_')]").ClickAsync();

                    var pageSource = await page.ContentAsync();

                    Assert.That(pageSource, Does.Contain("You have successfully removed -1"), "Voting failed");
                },
            this.BrowserType);
    }
}