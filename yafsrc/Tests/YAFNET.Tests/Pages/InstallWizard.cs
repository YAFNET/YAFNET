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

using AwesomeAssertions;

using YAF.Tests.Infrastructure;

namespace YAF.Tests.Pages;

public class InstallWizard(DatabaseFixture fixture, TestConfig testSettings)
{
    public async Task InstallAsync()
    {
        await TestContext.Progress.WriteLineAsync($"🚀 [{fixture.Scenario}] Starting installing YAF.NET...");

        // Step 1: Welcome
        var welcome = new WelcomePage(fixture.Page);
        await welcome.NavigateAsync(fixture.BaseUrl);

        var heading = await welcome.GetWelcomeTextAsync();
        await TestContext.Progress.WriteLineAsync($"🚀 [{fixture.Scenario}] Welcome heading: {heading}");
        heading.Should().NotBeNullOrWhiteSpace();

        await welcome.ClickNextAsync();

        // Step 2: Database/mail test page
        var database = new DatabasePage(fixture.Page);
        var content = await fixture.Page.ContentAsync();

        content.Should().Contain("Connection Succeeded");

        await database.ClickNextAsync();

        // Step 3: Initialize Database
        var initDatabase = new InitDatabasePage(fixture.Page);

        heading = await welcome.GetWelcomeTextAsync();
        await TestContext.Progress.WriteLineAsync($"🚀 [{fixture.Scenario}] {heading}");

        content = await fixture.Page.ContentAsync();

        content.Should().Contain("Initialize Database");

        await initDatabase.ClickNextAsync();

        // Step 4: Create the new board
        var createBoard = new CreateBoardPage(fixture.Page);

        heading = await createBoard.GetWelcomeTextAsync();

        heading.Should().Contain("Create New Board");

        await TestContext.Progress.WriteLineAsync($"🚀 [{fixture.Scenario}] Create New Board");

        await createBoard.FillBoardDetailsAsync(testSettings.TestApplicationName, testSettings.TestForumMail,
            testSettings.AdminUserName, testSettings.AdminEmail, testSettings.AdminPassword);

        await createBoard.ClickNextAsync();

        // Step 5: Complete
        var complete = new CompletePage(fixture.Page);

        var successMsg = await complete.GetSuccessMessageAsync();
        await TestContext.Progress.WriteLineAsync($"🚀 [{fixture.Scenario}] Success: {successMsg}");
        successMsg.Should().NotBeNullOrWhiteSpace();

        // Run the new board
        await complete.ClickGoToAppAsync();

        content = await fixture.Page.ContentAsync();

        await TestContext.Progress.WriteLineAsync("🚀 board created");

        content.Should().Contain(testSettings.TestApplicationName);

        await TestContext.Progress.WriteLineAsync($"🆕 YAF.NET with {fixture.Scenario} has been installed!");
    }

    public async Task RegisterTestUserAsync(string userName, string password)
    {
        var result =
            await fixture.Page.RegisterUserAsync(
                testSettings,
                fixture.SmtpServer,
                userName,
                password);

        result.Should().BeTrue(
            "Registration failed");

        await TestContext.Progress.WriteLineAsync($"💫 {userName} has been created!");
    }
}