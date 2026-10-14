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
                    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();
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
}
