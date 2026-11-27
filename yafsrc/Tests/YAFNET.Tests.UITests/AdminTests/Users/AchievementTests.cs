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

using System.Text.RegularExpressions;

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.AdminTests.Users;

/// <summary>
/// The achievements: the admin pages, the dialog, the notification, my achievements and the profile.
/// The test forum has the achievements switched off (the dialog would cover the pages of the other tests), the
/// tests switch them on and off again.
/// </summary>
public class AchievementTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The board has the default achievements, a new achievement is shown on the preview while it is edited and
    /// listed after it is saved.
    /// </summary>
    [Test]
    [Description("The default achievements are listed, a new achievement is saved")]
    public async Task AdminAchievementsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    await this.LoginAdminAsync(page);

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Achievements");

                    Assert.That(
                        await page.Locator(".achievement-row").CountAsync(),
                        Is.GreaterThanOrEqualTo(25),
                        "The default achievements are missing");
                    Assert.That(await page.Locator("body").InnerTextAsync(), Does.Contain("First Words"));

                    var name = $"Test Achievement {Random.Shared.Next()}";

                    await this.CreateAchievementAsync(page, name, 100);

                    Assert.That(
                        await page.Locator(".achievement-row", new PageLocatorOptions { HasText = name }).CountAsync(),
                        Is.EqualTo(1),
                        "The new achievement is not listed");

                    await this.DeleteAchievementAsync(page, name);
                });
    }

    /// <summary>
    /// A member who reaches the goal of an achievement sees the dialog, can show the achievement in the profile and
    /// gets a notification; the achievement is listed on my achievements.
    /// </summary>
    [Test]
    [Description("A member unlocks an achievement: dialog, notification, my achievements and profile")]
    public async Task UnlockAchievementTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var name = $"Legend {Random.Shared.Next()}";

                    try
                    {
                        await page.SetAchievementsAsync(this.TestSettings, true);

                        // an achievement for the first topic
                        await this.LoginAdminAsync(page);
                        await this.CreateAchievementAsync(page, name, 1);
                        Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                        var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                        Assert.That(await page.CreateNewTestTopicAsync(this.TestSettings), Is.True, "The topic was not created");

                        // the dialog (with the other new achievements, e.g. the first topic)
                        var dialog = page.Locator("#AchievementUnlockedModal");

                        await Assertions.Expect(dialog).ToBeVisibleAsync();

                        for (var i = 0; i < 10 && await page.Locator("#AchievementUnlockedName").InnerTextAsync() != name; i++)
                        {
                            Assert.That(await page.Locator("#AchievementUnlockedNext").IsEnabledAsync(), Is.True, "The achievement is not in the dialog");
                            await page.Locator("#AchievementUnlockedNext").ClickAsync();
                        }

                        await Assertions.Expect(page.Locator("#AchievementUnlockedName")).ToHaveTextAsync(name);
                        await Assertions.Expect(page.Locator("#AchievementUnlockedPoints")).ToHaveTextAsync("+250 points");
                        await Assertions.Expect(page.Locator("#AchievementUnlockedRarity")).ToHaveTextAsync("Legendary");

                        // show it in the profile
                        await page.Locator("#AchievementUnlockedFeature").ClickAsync();
                        await Assertions.Expect(page.Locator("#AchievementUnlockedFeature")).ToContainTextAsync("Shown in your profile");

                        await page.Locator("#AchievementUnlockedModal .btn-close").ClickAsync();
                        await Assertions.Expect(dialog).ToBeHiddenAsync();

                        // closing dismisses all new achievements (the requests are sent in the background), the
                        // dialog is not shown again
                        var unseen = await page.EvaluateAsync<int>(
                            """
                            async () => {
                                for (let i = 0; i < 20; i++) {
                                    const unseen = await (await fetch('/api/Achievement/Unseen')).json();
                                    if (unseen.length === 0) {
                                        return 0;
                                    }
                                    await new Promise(r => setTimeout(r, 250));
                                }
                                return (await (await fetch('/api/Achievement/Unseen')).json()).length;
                            }
                            """);

                        Assert.That(unseen, Is.Zero, "Closing the dialog did not dismiss the achievements");

                        await page.ReloadAsync();
                        await page.WaitForTimeoutAsync(1500);
                        Assert.That(await dialog.IsVisibleAsync(), Is.False, "The dialog was shown again");

                        // the notification
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Notification");

                        var notification = page.Locator(
                            "#notificationsContainer li.notification-row",
                            new PageLocatorOptions { HasText = name });

                        await Assertions.Expect(notification).ToHaveCountAsync(1);
                        await Assertions.Expect(notification).ToContainTextAsync("You unlocked the achievement");

                        // my achievements: unlocked and shown in the profile
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}MyAchievements");

                        var card = page.Locator(".achievement-card.unlocked", new PageLocatorOptions { HasText = name });

                        await Assertions.Expect(card).ToHaveCountAsync(1);
                        await Assertions.Expect(card.Locator("input[data-achievement-feature]")).ToBeCheckedAsync();

                        // the profile shows it
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                        var profileLink = await page.Locator("a", new PageLocatorOptions { HasText = "View Profile" }).First
                                              .GetAttributeAsync("href");

                        await page.GotoAsync(new Uri(new Uri(this.TestSettings.TestForumUrl), profileLink).ToString());

                        Assert.That(await page.Locator("body").InnerTextAsync(), Does.Contain(userName), "Not the profile of the user");

                        await Assertions.Expect(page.Locator("#ProfileShowcase [data-bs-toggle='tooltip']")).ToHaveCountAsync(1);
                        await Assertions.Expect(page.Locator("#ProfileAchievements")).ToBeVisibleAsync();
                    }
                    finally
                    {
                        // the other tests don't get the achievement and don't see the dialog
                        await this.LoginAdminAsync(page);
                        await this.DeleteAchievementAsync(page, name);

                        await page.SetAchievementsAsync(this.TestSettings, false);
                    }
                });
    }

    /// <summary>
    /// Logs the admin in (whoever is logged in), the achievements the admin has unlocked are dismissed (their dialog
    /// would cover the page).
    /// </summary>
    private async Task LoginAdminAsync(IPage page)
    {
        // the dialog of the user who is logged in would cover the logout
        await page.DismissAchievementsAsync(this.TestSettings);

        if (await page.Locator("li.dropdown-notify").IsVisibleAsync())
        {
            await page.LogOutUserAsync();
        }

        Assert.That(
            await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
            Is.True,
            "Login failed");

        await page.DismissAchievementsAsync(this.TestSettings);
    }

    /// <summary>
    /// Creates a legendary achievement for the topics (Admin > Edit Achievement), checks the preview.
    /// </summary>
    private async Task CreateAchievementAsync(IPage page, string name, int topics)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditAchievement");

        await page.Locator("#Input_Name").FillAsync(name);
        await page.Locator("#Input_Description").FillAsync("Created by a test");
        await page.Locator("#Input_Type").SelectOptionAsync("2");
        await page.Locator("#Input_Threshold").FillAsync(topics.ToString());
        await page.Locator("#Input_Points").FillAsync("250");
        await page.Locator("label[for='Rarity_4']").ClickAsync();
        await page.Locator("label[for='Icon_dragon']").ClickAsync();

        // the preview
        await Assertions.Expect(page.Locator("#AchievementPreviewName")).ToHaveTextAsync(name);
        await Assertions.Expect(page.Locator("#AchievementPreviewTrophy")).ToHaveClassAsync(new Regex("rarity-legendary"));
        await Assertions.Expect(page.Locator("#AchievementPreviewTrophy i")).ToHaveClassAsync(new Regex("fa-dragon"));
        await Assertions.Expect(page.Locator("#AchievementPreviewRarity")).ToHaveTextAsync("Legendary");

        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditAchievement') and contains(@formaction,'Save')]"));

        Assert.That(page.Url, Does.Contain("Admin/Achievements"), "The page did not go back to the list");
    }

    /// <summary>
    /// Deletes an achievement (Admin > Achievements).
    /// </summary>
    private async Task DeleteAchievementAsync(IPage page, string name)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Achievements");

        var row = page.Locator(".achievement-row", new PageLocatorOptions { HasText = name });

        if (await row.CountAsync() == 0)
        {
            return;
        }

        await row.Locator(".achievement-actions button[formaction*='Delete']").ClickAsync();

        // the confirm dialog
        await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

        await Assertions.Expect(page.Locator(".achievement-row", new PageLocatorOptions { HasText = name })).ToHaveCountAsync(0);
    }
}
