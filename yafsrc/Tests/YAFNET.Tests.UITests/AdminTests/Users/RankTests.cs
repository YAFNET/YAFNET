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

namespace YAF.Tests.UITests.AdminTests.Users;

/// <summary>
/// The Rank Tests
/// </summary>
public class RankTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// A rank that users have can't be deleted: the delete failed with an error (the users reference the rank).
    /// </summary>
    [Test]
    [Description("A rank in use can't be deleted")]
    public async Task RankInUseCanNotBeDeletedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                    Assert.That(await page.LoginAdminUserAsync(this.TestSettings), Is.True, "Login failed");

                    var rankName = $"TestRank{Random.Shared.Next()}";

                    // a new rank
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditRank");
                    await page.Locator("#Input_Name").FillAsync(rankName);
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));

                    // the user gets the rank
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");
                    await page.OpenUserFilterAsync();
                    var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
                    await searchNameInput.FillAsync(userName);
                    await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));
                    await page.RunAndWaitForNextPageAsync(
                        () => page.Locator("//a[contains(@href,'EditUser')]", new PageLocatorOptions { HasText = userName }).First.ClickAsync());
                    await page.Locator("#Input_RankID").SelectOptionAsync(new SelectOptionValue { Label = rankName });
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UsersInfo') and contains(@formaction,'Save')]"));

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Ranks");

                    var rankId = await page.EvaluateAsync<string>(
                        """
                        name => {
                            for (const item of document.querySelectorAll('li.list-group-item')) {
                                const edit = item.querySelector('a[href*="EditRank"]');
                                if (edit && item.textContent.includes(name)) {
                                    return edit.getAttribute('href').match(/(\d+)\D*$/)[1];
                                }
                            }
                            return null;
                        }
                        """,
                        rankName);

                    Assert.That(rankId, Is.Not.Null, "The rank is not listed");

                    var response = await page.EvaluateAsync<string[]>(
                        """
                        async url => {
                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                            const body = new URLSearchParams({ '__RequestVerificationToken': token });
                            const response = await fetch(url, { method: 'POST', body: body });
                            return [String(response.status), await response.text()];
                        }
                        """,
                        $"{this.TestSettings.TestForumUrl}Admin/Ranks?handler=Delete&id={rankId}");

                    Assert.That(response[0], Is.EqualTo("200"), "The delete handler failed");
                    Assert.That(response[1], Does.Contain("You cannot delete this rank because it is in use."), "The warning is missing");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Ranks");

                    Assert.That(await page.Locator("body").InnerTextAsync(), Does.Contain(rankName), "The rank was deleted");
                });
    }

    /// <summary>
    /// A second rank with the name of an existing rank is refused with a warning: the database refused it (the
    /// name is unique in the board) and the page failed with an error.
    /// </summary>
    [Test]
    [Description("A rank name can't be used twice")]
    public async Task RankNameCanNotBeUsedTwiceTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(await page.LoginAdminUserAsync(this.TestSettings), Is.True, "Login failed");

                    var rankName = $"TwiceRank{Random.Shared.Next()}";

                    for (var i = 0; i < 2; i++)
                    {
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditRank");
                        await page.Locator("#Input_Name").FillAsync(rankName);
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));
                    }

                    Assert.That(await page.ContentAsync(), Does.Contain("A rank with such a name already exists."), "The warning is missing");
                    Assert.That(page.Url, Does.Contain("EditRank"), "The page is not shown again");
                });
    }

    /// <summary>
    /// The signature BBCodes of a rank (e.g. "B,I") can be entered and are kept when the rank is saved again: the
    /// field was a number field, the browser showed it empty and saving the rank removed the BBCodes.
    /// </summary>
    [Test]
    [Description("The signature BBCodes of a rank are kept")]
    public async Task RankSignatureBBCodesAreKeptTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(await page.LoginAdminUserAsync(this.TestSettings), Is.True, "Login failed");

                    var rankName = $"BBCodeRank{Random.Shared.Next()}";

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditRank");
                    await page.Locator("#Input_Name").FillAsync(rankName);
                    await page.Locator("#Input_UsrSigBBCodes").FillAsync("B,I");
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));

                    // the rank is opened and saved again without changes
                    var editLink = page.Locator("li.list-group-item", new PageLocatorOptions { HasTextString = rankName })
                        .Locator("a[href*='EditRank']").First;
                    var editUrl = new Uri(new Uri(page.Url), await editLink.GetAttributeAsync("href")).ToString();

                    await page.GotoAsync(editUrl);

                    Assert.That(await page.Locator("#Input_UsrSigBBCodes").InputValueAsync(), Is.EqualTo("B,I"), "The BBCodes are not shown");

                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));

                    await page.GotoAsync(editUrl);

                    Assert.That(await page.Locator("#Input_UsrSigBBCodes").InputValueAsync(), Is.EqualTo("B,I"), "The BBCodes were removed");
                });
    }

    /// <summary>
    /// The start rank of the new users (Newbie) is on the rank ladder: it was not, the users were never promoted to
    /// the next ranks (Member, Advanced Member).
    /// </summary>
    [Test]
    [Description("The start rank of a new board is on the ladder")]
    public async Task StartRankIsOnTheLadderTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                    await page.GotoAsync(await this.GetEditRankUrlAsync(page, "Newbie"));

                    Assert.That(await page.Locator("#Input_IsStart").IsCheckedAsync(), Is.True, "Newbie is not the start rank");
                    Assert.That(await page.Locator("#Input_IsLadder").IsCheckedAsync(), Is.True, "Newbie is not on the ladder");
                });
    }

    /// <summary>
    /// A board has one start rank: a new start rank kept the old start rank, the new users got one of them.
    /// </summary>
    [Test]
    [Description("Saving a rank as start rank removes the flag from the old start rank")]
    public async Task NewStartRankReplacesTheOldStartRankTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                    var rankName = $"StartRank{Random.Shared.Next()}";

                    try
                    {
                        // a new start rank
                        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditRank");
                        await page.Locator("#Input_Name").FillAsync(rankName);
                        await page.Locator("#Input_IsStart").CheckAsync();
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));

                        await page.GotoAsync(await this.GetEditRankUrlAsync(page, rankName));
                        Assert.That(await page.Locator("#Input_IsStart").IsCheckedAsync(), Is.True, "The new rank is not the start rank");

                        await page.GotoAsync(await this.GetEditRankUrlAsync(page, "Newbie"));
                        Assert.That(await page.Locator("#Input_IsStart").IsCheckedAsync(), Is.False, "The old start rank is still a start rank");
                    }
                    finally
                    {
                        // Newbie is the start rank again, the test rank is removed
                        await page.GotoAsync(await this.GetEditRankUrlAsync(page, "Newbie"));
                        await page.Locator("#Input_IsStart").CheckAsync();
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));

                        await this.DeleteRankAsync(page, rankName);
                    }
                });
    }

    /// <summary>
    /// The start rank can't lose its flag: without a start rank the registration of new users failed with an error.
    /// </summary>
    [Test]
    [Description("The start rank keeps its flag")]
    public async Task StartRankKeepsTheFlagTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                    var editUrl = await this.GetEditRankUrlAsync(page, "Newbie");

                    try
                    {
                        await page.GotoAsync(editUrl);
                        await page.Locator("#Input_IsStart").UncheckAsync();
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));

                        Assert.That(await page.ContentAsync(), Does.Contain("Mark another rank as start rank instead."), "The warning is missing");

                        await page.GotoAsync(editUrl);
                        Assert.That(await page.Locator("#Input_IsStart").IsCheckedAsync(), Is.True, "The start rank lost the flag");
                    }
                    finally
                    {
                        // Newbie is the start rank again
                        await page.GotoAsync(editUrl);
                        await page.Locator("#Input_IsStart").CheckAsync();
                        await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));
                    }
                });
    }

    /// <summary>
    /// The name of a rank is saved without the spaces around it and a name of spaces is refused: both were saved.
    /// </summary>
    [Test]
    [Description("A rank name is trimmed, a blank name is refused")]
    public async Task RankNameIsTrimmedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                    // a name of spaces
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditRank");
                    await page.Locator("#Input_Name").FillAsync("   ");
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));

                    Assert.That(await page.ContentAsync(), Does.Contain("You must define a rank name."), "The blank name was saved");

                    // the spaces around the name are removed
                    var rankName = $"TrimRank{Random.Shared.Next()}";

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditRank");
                    await page.Locator("#Input_Name").FillAsync($"  {rankName}  ");
                    await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'EditRank') and contains(@formaction,'Save')]"));

                    await page.GotoAsync(await this.GetEditRankUrlAsync(page, rankName));

                    Assert.That(await page.Locator("#Input_Name").InputValueAsync(), Is.EqualTo(rankName), "The spaces were saved");
                });
    }

    /// <summary>
    /// The ranks page shows the ladder by the minimum posts (Newbie first), the start rank can't be deleted and the
    /// users of a rank are listed on the users page.
    /// </summary>
    [Test]
    [Description("The ranks page shows the ladder")]
    public async Task RanksPageShowsTheLadderTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword), Is.True, "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Ranks");

                    var ladder = await page.Locator("#LadderRanks .rank-name").AllInnerTextsAsync();

                    Assert.That(System.Linq.Enumerable.Take(ladder, 3), Is.EqualTo(new[] { "Newbie", "Member", "Advanced Member" }), "The ladder is not sorted by the posts");

                    var newbie = page.Locator("#LadderRanks li.rank-row", new PageLocatorOptions { Has = page.Locator(".rank-name", new PageLocatorOptions { HasTextString = "Newbie" }) });

                    Assert.That(await newbie.Locator(".rank-actions button[formaction*='Delete']").CountAsync(), Is.Zero, "The start rank can be deleted");
                    Assert.That(await newbie.Locator(".rank-delete-info").EvaluateAsync<string>("e => e.getAttribute('title') || e.getAttribute('data-bs-original-title')"), Does.Contain("start rank"), "The reason is missing");

                    // the users of the rank
                    await page.RunAndWaitForNextPageAsync(() => newbie.Locator("a.rank-users").ClickAsync());

                    Assert.That(page.Url, Does.Contain("Admin/Users"), "The users page is not shown");
                    Assert.That(
                        await page.Locator("#Input_Rank").EvaluateAsync<string>("s => s.options[s.selectedIndex].text"),
                        Is.EqualTo("Newbie"),
                        "The users are not filtered by the rank");
                });
    }

    /// <summary>
    /// Deletes a rank.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="name">The name of the rank.</param>
    private async Task DeleteRankAsync(IPage page, string name)
    {
        var editUrl = await this.GetEditRankUrlAsync(page, name);
        var rankId = System.Text.RegularExpressions.Regex.Match(new Uri(editUrl).AbsolutePath, @"(\d+)\D*$").Groups[1].Value;

        await page.EvaluateAsync(
            """
            async url => {
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                const body = new URLSearchParams({ '__RequestVerificationToken': token });
                await fetch(url, { method: 'POST', body: body });
            }
            """,
            $"{this.TestSettings.TestForumUrl}Admin/Ranks?handler=Delete&id={rankId}");
    }

    /// <summary>
    /// Gets the url of the edit page of a rank.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <param name="name">The name of the rank.</param>
    /// <returns>Returns the url</returns>
    private async Task<string> GetEditRankUrlAsync(IPage page, string name)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Ranks");

        var href = await page.EvaluateAsync<string>(
            """
            name => {
                for (const item of document.querySelectorAll('li.rank-row')) {
                    const title = item.querySelector('.rank-name');
                    const edit = item.querySelector('a[href*="EditRank"]');
                    if (title && edit && title.textContent.trim() === name) {
                        return edit.getAttribute('href');
                    }
                }
                return null;
            }
            """,
            name);

        Assert.That(href, Is.Not.Null, $"The rank {name} is not listed");

        return new Uri(new Uri(page.Url), href).ToString();
    }
}
