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

namespace YAF.Tests.UITests.AdminTests;

/// <summary>
/// The buttons of the admin pages call existing handlers (private messages, user points, access masks, forum sorting).
/// </summary>
public class AdminMaintenanceTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// The delete button of the private message maintenance runs the delete.
    /// </summary>
    [Test]
    public async Task PrivateMessagesDeleteButtonWorksTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Pm");

                await page.Locator("#Input_Days2").FillAsync("1");

                // the delete is confirmed in a dialog
                await page.Locator("button[formaction*='Commit']").ClickAsync();

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator(".bootbox .btn-success").ClickAsync(),
                                   r => r.Request.Method == "POST" && r.Url.Contains("/Admin/Pm"));

                Assert.That(response.Status, Is.LessThan(400), $"The delete failed ({response.Status} {response.Url})");
            });
    }

    /// <summary>
    /// The database maintenance buttons work on every database: the re-index and shrink of MySQL (and on PostgreSQL without
    /// the rights) failed with an error page instead of a message, on MySQL both run OPTIMIZE TABLE.
    /// </summary>
    [Test]
    public async Task DatabaseMaintenanceButtonsWorkTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                foreach (var (handler, confirm) in new[] { ("GetStats", false), ("RecoveryMode", true), ("ReIndex", true), ("Shrink", true) })
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ReIndex");

                    var button = page.Locator($"button[formaction$='/{handler}']");

                    IResponse response = null;

                    await page.RunAndWaitForNextPageAsync(
                        async () => response = await page.RunAndWaitForResponseAsync(
                                        async () =>
                                        {
                                            await button.ClickAsync();

                                            // the change is confirmed in a dialog
                                            if (confirm)
                                            {
                                                await page.Locator(".bootbox .btn-success").ClickAsync();
                                            }
                                        },
                                        r => r.Request.Method == "POST" && r.Url.Contains($"/Admin/ReIndex/{handler}")));

                    Assert.That(response.Status, Is.LessThan(400), $"{handler} failed ({response.Status})");
                    var content = await page.ContentAsync();

                    Assert.That(content, Does.Not.Contain("Forum Error").And.Not.Contain("Something went wrong"), $"{handler} failed");

                    // (MySQL has no recovery mode)
                    if (handler is "ReIndex" or "Shrink")
                    {
                        Assert.That(content, Does.Not.Contain("Not Supported by MySQL"), $"{handler} is not supported");
                    }
                }
            });
    }

    /// <summary>
    /// The points of a user can be set, added and removed.
    /// </summary>
    [Test]
    public async Task UserPointsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await this.LoginAdminAsync(page);

                var userId = await this.FindUserIdAsync(page, userName);

                await this.ChangePointsAsync(page, userId, "UserPoints", "SetUserPoints", "10");
                Assert.That(await this.PointsAsync(page), Is.EqualTo("10"), "Setting the points failed");

                await this.ChangePointsAsync(page, userId, "AddPoints", "AddPoints", "5");
                Assert.That(await this.PointsAsync(page), Is.EqualTo("15"), "Adding points failed");

                await this.ChangePointsAsync(page, userId, "RemovePoints", "RemovePoints", "3");
                Assert.That(await this.PointsAsync(page), Is.EqualTo("12"), "Removing points failed");
            });
    }

    /// <summary>
    /// The "new access mask" item of the context menu opens the page for a new access mask.
    /// </summary>
    [Test]
    public async Task NewAccessMaskFromContextMenuTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/AccessMasks");

                // the item of the (hidden) context menu
                await page.RunAndWaitForNextPageAsync(
                    () => page.Locator("button[formaction*='New']").First.EvaluateAsync("button => button.click()"));

                Assert.That(page.Url, Does.Contain("EditAccessMask"), "The page for a new access mask was not opened");
            });
    }

    /// <summary>
    /// The export of the bb codes exports the codes selected on the shown page: the pages are changed by a post
    /// of the form, the ids and the selection of the posted page were shown (page 1 was exported).
    /// </summary>
    [Test]
    public async Task ExportBBCodeOfSecondPageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/BBCodes");

                // 5 codes per page (the select sends the form)
                await page.EvaluateAsync("() => window.yafTestOldPage = true");
                await page.Locator("select#Size").SelectOptionAsync(["5"]);
                await page.WaitForFunctionAsync("() => !window.yafTestOldPage && document.readyState === 'complete'");

                var checkBoxes = page.Locator("input[type='checkbox'][id$='__Selected']");

                Assert.That(await checkBoxes.CountAsync(), Is.EqualTo(5), "The page size was not changed");

                await checkBoxes.First.CheckAsync();

                await page.SubmitAndWaitAsync(page.Locator(".pagination .page-link", new PageLocatorOptions { HasTextRegex = new System.Text.RegularExpressions.Regex(@"^\s*2\s*$") }).First);

                Assert.That(await checkBoxes.First.IsCheckedAsync(), Is.False, "The selection of page 1 is shown on page 2");

                await checkBoxes.Nth(1).CheckAsync();

                var name = (await page.Locator($"label[for='{await checkBoxes.Nth(1).GetAttributeAsync("id")}']").InnerTextAsync()).Trim();

                var download = await page.RunAndWaitForDownloadAsync(
                                   () => page.Locator(".card-footer button[formaction*='Export']").ClickAsync());

                var xml = await System.IO.File.ReadAllTextAsync(await download.PathAsync());

                Assert.That(xml, Does.Contain($"<Name>{name}</Name>"), $"The selected code {name} was not exported");

                Assert.That(
                    xml.Split("<Name>").Length - 1,
                    Is.EqualTo(1),
                    $"Other codes were exported: {xml}");
            });
    }

    /// <summary>
    /// The selection of the topics of the moderated forum is not taken to the next page (the pages are changed by
    /// a post of the form, the topics at the same places of the next page were selected and could be deleted).
    /// </summary>
    [Test]
    public async Task ModerateSelectionNotOnNextPageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                // more topics than one page of 5
                for (var i = 0; i < 6; i++)
                {
                    Assert.That(
                        await page.CreateTopicAsync(
                            this.TestSettings,
                            this.TestSettings.TestForumId,
                            $"Moderate paging {Random.Shared.Next()}",
                            "A topic of the paging test"),
                        Is.Not.Null,
                        "The topic was not created");
                }

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Moderate/Forums/{this.TestSettings.TestForumId}");

                await page.EvaluateAsync("() => window.yafTestOldPage = true");
                await page.Locator("select#Size").SelectOptionAsync(["5"]);
                await page.WaitForFunctionAsync("() => !window.yafTestOldPage && document.readyState === 'complete'");

                var checkBoxes = page.Locator("input[type='checkbox'][id^='TopicList_']");

                Assert.That(await checkBoxes.CountAsync(), Is.EqualTo(5), "The page size was not changed");

                await checkBoxes.First.CheckAsync();

                await page.SubmitAndWaitAsync(
                    page.Locator(".pagination .page-link", new PageLocatorOptions { HasTextRegex = new System.Text.RegularExpressions.Regex(@"^\s*2\s*$") }).First);

                Assert.That(await checkBoxes.First.IsCheckedAsync(), Is.False, "The selection of page 1 is shown on page 2");
            });
    }

    /// <summary>
    /// A name longer than its column (access mask: 50 characters) is limited by the input,
    /// saving does not fail on SQL Server, MySQL and PostgreSQL.
    /// </summary>
    [Test]
    public async Task LongAccessMaskNameIsLimitedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditAccessMask");

                var name = $"Long mask {Random.Shared.Next()} ".PadRight(60, 'x');

                await page.Locator("#Input_Name").FillAsync(name);
                await page.Locator("#Input_SortOrder").FillAsync("9");

                await page.SubmitAndWaitAsync(page.Locator("button[formaction$='/Save'], button[formaction*='handler=Save']").First);

                Assert.That(page.Url, Does.EndWith("Admin/AccessMasks"), "The access mask was not saved");
                Assert.That(await page.ContentAsync(), Does.Contain(name[..50]), "The access mask is not listed");
            });
    }

    /// <summary>
    /// The sorting tools of the forums page re-order the categories (the board of the tests has one category,
    /// the order of the forums is not changed).
    /// </summary>
    [Test]
    public async Task SortCategoriesToolWorksTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Forums");

                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Tools" }).First.ClickAsync();
                await page.Locator("button[formaction*='SortCategoriesAscending']").ClickAsync();

                // the sorting is confirmed in a dialog
                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("All Categories have been re-ordered."),
                    "The categories were not re-ordered");

                // the re-ordered categories start with 0, a category with that sort order can be saved
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditCategory/1");

                await page.Locator("#Input_SortOrder").FillAsync("0");
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                Assert.That(page.Url, Does.EndWith("Admin/Forums"), "The category with the sort order 0 was not saved");
            });
    }

    /// <summary>
    /// A medal without image shows the warning (the page is shown again with its lists).
    /// </summary>
    [Test]
    public async Task EditMedalWithoutImageShowsWarningTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditMedal");

                await page.Locator("#Input_Name").FillAsync("Medal without image");
                await page.Locator("#Input_Category").FillAsync("Tests");
                await page.Locator("#Input_Description").FillAsync("A medal of the tests");
                await page.Locator("#Input_Message").FillAsync("A medal of the tests");

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator("button[formaction*='Save']").ClickAsync(),
                                   r => r.Request.Method == "POST" && r.Url.Contains("EditMedal"));

                Assert.That(response.Status, Is.LessThan(400), $"The page failed ({response.Status})");

                await page.WaitForLoadStateAsync(LoadState.Load);

                Assert.That(await page.ContentAsync(), Does.Contain("Medal image must be specified!"), "The warning is missing");
            });
    }

    /// <summary>
    /// Only the images of the medals folder can be the image of a medal: any value of the form was saved and shown as
    /// html on the medals page.
    /// </summary>
    [Test]
    public async Task EditMedalRefusesOtherImageTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                var id = Random.Shared.Next();
                var name = $"Image medal {id}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditMedal");

                await page.Locator("#Input_Name").FillAsync(name);

                // a value that is not an image of the list (the select is a script control)
                await page.Locator("#Input_MedalImage").EvaluateAsync(
                    "(select, value) => { select.add(new Option(value, value, true, true)); select.value = value; }",
                    $"medal.webp\" id=\"medalimage{id}");

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                Assert.That(await page.ContentAsync(), Does.Contain("Medal image must be specified!"), "The image was not refused");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Medals");

                Assert.That(await page.Locator($"#medalimage{id}").CountAsync(), Is.Zero, "The image is html on the medals page");
                Assert.That(await page.ContentAsync(), Does.Not.Contain(name), "The medal was saved");
            });
    }

    /// <summary>
    /// A new category with the name of an existing category shows the warning
    /// (the page is shown again with the list of the images).
    /// </summary>
    [Test]
    public async Task EditCategoryWithExistingNameShowsWarningTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditCategory/1");

                var name = await page.Locator("#Input_Name").InputValueAsync();

                var newPage = await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditCategory");

                // the options of the image list sent by the server (Choices.js changes the select)
                var images = (await newPage!.TextAsync()).Split("<option").Length;

                await page.Locator("#Input_Name").FillAsync(name);

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator("button[formaction*='Save']").ClickAsync(),
                                   r => r.Request.Method == "POST" && r.Url.Contains("EditCategory"));

                Assert.That(response.Status, Is.LessThan(400), $"The page failed ({response.Status})");

                await page.WaitForLoadStateAsync(LoadState.Load);

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("A Category with such a name already exists."),
                    "The warning is missing");
                Assert.That((await response.TextAsync()).Split("<option").Length, Is.EqualTo(images), "The image list is missing");
            });
    }

    /// <summary>
    /// The admin can't delete the own account, the message is shown with the list of the users.
    /// </summary>
    [Test]
    public async Task AdminCantDeleteHimselfTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                var userId = await this.FindUserIdAsync(page, this.TestSettings.AdminUserName);

                await page.Locator($"button[formaction$='/Delete?id={userId}']:visible").First.ClickAsync();

                // the delete is confirmed in a dialog
                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                var content = await page.ContentAsync();

                Assert.That(content, Does.Contain("You can&#x27;t delete yourself.").Or.Contain("You can't delete yourself."), "The message is missing");
                Assert.That(
                    await page.Locator("//a[contains(@href,'EditUser')]", new PageLocatorOptions { HasText = this.TestSettings.AdminUserName }).CountAsync(),
                    Is.GreaterThan(0),
                    "The list of the users is missing");
            });
    }

    /// <summary>
    /// An access mask that is in use can't be deleted, the message is shown with the list of the masks.
    /// </summary>
    [Test]
    public async Task DeleteAccessMaskInUseShowsWarningTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/AccessMasks");

                var masks = await page.Locator("button[formaction*='/Edit?maskId=']").CountAsync();

                // the first mask (admin access) is used by the forums of the test board
                await page.Locator("button[formaction$='/Delete?maskId=1']:visible").First.ClickAsync();

                // the delete is confirmed in a dialog
                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("You cannot delete this access mask because it is in use."),
                    "The warning is missing");
                Assert.That(
                    await page.Locator("button[formaction*='/Edit?maskId=']").CountAsync(),
                    Is.EqualTo(masks),
                    "The list of the access masks is missing");
            });
    }

    /// <summary>
    /// A failed test mail shows the error, the page is shown again with the list of the groups.
    /// </summary>
    [Test]
    public async Task FailedTestMailShowsErrorTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Mail");

                var groups = await page.Locator("#Input_ToListItem option").CountAsync();

                Assert.That(groups, Is.GreaterThan(0), "The groups are missing");

                // an invalid sender, the mail can't be sent (the browser would not send it as e-mail field)
                await page.Locator("#Input_TestFromEmail").EvaluateAsync("input => input.type = 'text'");
                await page.Locator("#Input_TestFromEmail").FillAsync("@@@");
                await page.Locator("#Input_TestToEmail").FillAsync("test@test.com");
                await page.Locator("#Input_TestSubject").FillAsync("Test");
                await page.Locator("#Input_TestBody").FillAsync("Test");

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator("button[formaction*='TestSmtp']").ClickAsync(),
                                   r => r.Request.Method == "POST" && r.Url.Contains("TestSmtp"));

                Assert.That(response.Status, Is.LessThan(400), $"The page failed ({response.Status})");

                await page.WaitForLoadStateAsync(LoadState.Load);

                Assert.That(await page.ContentAsync(), Does.Not.Contain("Mail Sent."), "The mail was sent");
                Assert.That(
                    await page.Locator("#Input_ToListItem option").CountAsync(),
                    Is.EqualTo(groups),
                    "The list of the groups is missing");
            });
    }

    /// <summary>
    /// A group mail that can't be sent (the mail server is not reachable) shows the error instead of an error page.
    /// </summary>
    [Test]
    public async Task FailedGroupMailShowsErrorTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Mail");

                await page.Locator("#Input_Subject").FillAsync("Group mail test");
                await page.Locator("#Input_Body").FillAsync("A mail to the group");

                this.Fixture.SmtpServer.Stop();

                try
                {
                    IResponse response = null;

                    // the mail is confirmed in a dialog
                    await page.Locator("#Send").ClickAsync();

                    await page.RunAndWaitForNextPageAsync(
                        async () => response = await page.RunAndWaitForResponseAsync(
                                        () => page.Locator(".bootbox .btn-success").ClickAsync(),
                                        r => r.Request.Method == "POST" && r.Url.Contains("/Admin/Mail")));

                    Assert.That(response.Status, Is.LessThan(400), $"The group mail failed with an error page ({response.Status})");
                    Assert.That(await page.ContentAsync(), Does.Not.Contain("Forum Error"), "The group mail failed with an error page");
                }
                finally
                {
                    this.Fixture.SmtpServer = netDumbster.smtp.SimpleSmtpServer.Start(this.TestSettings.TestMailPort);
                }

                // with the mail server the group gets the mail
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Mail");

                await page.Locator("#Input_Subject").FillAsync("Group mail test");
                await page.Locator("#Input_Body").FillAsync("A mail to the group");

                await page.Locator("#Send").ClickAsync();
                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                for (var wait = 0; wait < 20 && this.Fixture.SmtpServer.ReceivedEmailCount == 0; wait++)
                {
                    await Task.Delay(500);
                }

                Assert.That(this.Fixture.SmtpServer.ReceivedEmailCount, Is.GreaterThan(0), "The group mail was not sent");
            });
    }

    /// <summary>
    /// The execution order of a bb code is shown and saved (the field always showed 1,
    /// saving a bb code changed its order).
    /// </summary>
    [Test]
    public async Task EditBBCodeKeepsExecOrderTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/BBCodes");

                var editUrl = await page.Locator("a[href*='/EditBBCode/']").First.GetAttributeAsync("href");

                Assert.That(editUrl, Is.Not.Null, "No bb code found");

                await page.GotoAsync(editUrl);

                var original = await page.Locator("#Input_ExecOrder").InputValueAsync();

                await page.Locator("#Input_ExecOrder").FillAsync("7");
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Add']"));

                await page.GotoAsync(editUrl);

                var saved = await page.Locator("#Input_ExecOrder").InputValueAsync();

                // the old value again
                await page.Locator("#Input_ExecOrder").FillAsync(original);
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Add']"));

                Assert.That(saved, Is.EqualTo("7"), "The execution order is not shown");
            });
    }

    /// <summary>
    /// A new custom BBCode works in the posts at once (the cached rule engines, which contain the
    /// custom BBCodes, were not cleared when a BBCode was added or changed).
    /// </summary>
    [Test]
    public async Task NewCustomBBCodeIsUsedAtOnceTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var id = Guid.NewGuid().ToString("N")[..8];
                var tag = $"xt{id}";

                await this.LoginAdminAsync(page);

                // the post is shown before the BBCode exists, the rule engine is cached
                var topicId = await page.CreateTopicAsync(
                                  this.TestSettings,
                                  this.TestSettings.TestForumId,
                                  $"Custom BBCode {id}",
                                  $"[{tag}]custom {id}[/{tag}]");

                Assert.That(topicId, Is.Not.Null, "The topic was not created");
                Assert.That(await page.Locator($"span.{tag}").CountAsync(), Is.Zero, "The BBCode already exists");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditBBCode/0");

                await page.Locator("#Input_Name").FillAsync(tag.ToUpperInvariant());
                await page.Locator("#Input_Description").FillAsync("Test BBCode");
                await page.Locator("#Input_SearchRegEx").FillAsync($@"\[{tag}\](?<inner>(.*?))\[/{tag}\]");
                await page.Locator("#Input_ReplaceRegEx").FillAsync($"<span class=\"{tag}\">${{inner}}</span>");
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Add']"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Posts/{topicId}/topic");

                Assert.That(
                    await page.Locator($"span.{tag}").CountAsync(), Is.EqualTo(1),
                    "The new BBCode is not used in the post");
            });
    }

    /// <summary>
    /// A changed or deleted medal is shown at once in the profiles (the medals of the users are cached).
    /// </summary>
    [Test]
    public async Task ChangedMedalIsShownAtOnceTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var id = Guid.NewGuid().ToString("N")[..8];

                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Profile/EditProfile");

                var profileLink = await page.Locator("a", new PageLocatorOptions { HasText = "View Profile" }).First.GetAttributeAsync("href");
                var userId = System.Text.RegularExpressions.Regex.Match(profileLink ?? string.Empty, @"/UserProfile/(\d+)/").Groups[1].Value;
                var profileUrl = new Uri(new Uri(this.TestSettings.TestForumUrl), profileLink).ToString();

                Assert.That(userId, Is.Not.Empty, "The id of the admin was not found");

                // a new medal
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditMedal");
                await this.SaveMedalAsync(page, $"Medal {id}");

                var medalId = await page.EvaluateAsync<string>(
                                  """
                                  name => {
                                      const node = [...document.querySelectorAll("a[href*='medalId=']")]
                                          .find(a => a.closest('li')?.textContent.includes(name));
                                      return node ? node.getAttribute('href').match(/medalId=(\d+)/)[1] : null;
                                  }
                                  """,
                                  $"Medal {id}");

                Assert.That(medalId, Is.Not.Null, "The medal was not created");

                // awarded to the admin (this clears the cached medals of the user)
                var award = await page.EvaluateAsync<string>(
                                """
                                async ([medalId, userId]) => {
                                    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                                    const response = await fetch('/api/Medal/EditUserMedal', {
                                        method: 'POST',
                                        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                                        body: JSON.stringify({ medalId: Number(medalId), userID: Number(userId), medalName: 'x' })
                                    });
                                    return String(response.status);
                                }
                                """,
                                new[] { medalId, userId });

                Assert.That(award, Is.EqualTo("200"), "The medal was not awarded");

                await page.GotoAsync(profileUrl);

                // the tooltip moves the title, the alt text is the same
                Assert.That(await page.Locator($"img[alt='Medal {id}']").CountAsync(), Is.EqualTo(1), "The awarded medal is not shown");

                // renamed
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditMedal?medalId={medalId}");
                await this.SaveMedalAsync(page, $"Renamed {id}");

                await page.GotoAsync(profileUrl);

                var renamed = await page.Locator($"img[alt='Renamed {id}']").CountAsync();

                // deleted
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Medals");

                var deleteUrl = await page.Locator($"button[formaction*='medalId={medalId}'][formaction*='Delete']").First.GetAttributeAsync("formaction");

                var deleteStatus = await page.EvaluateAsync<string>(
                    """
                    async url => {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                        const response = await fetch(url, { method: 'POST', body: new URLSearchParams({ '__RequestVerificationToken': token }) });
                        return String(response.status) + ' ' + response.url + ' ' + (await response.text()).includes('Renamed');
                    }
                    """,
                    deleteUrl);

                Assert.That(deleteStatus, Does.StartWith("200"), $"The medal was not deleted {deleteUrl} {deleteStatus}");
                Assert.That(deleteStatus, Does.EndWith("false"), $"The medal was not deleted {deleteUrl} {deleteStatus}");

                await page.GotoAsync(profileUrl);

                var deleted = await page.Locator($"img[alt*='{id}']").CountAsync();

                Assert.That(renamed, Is.EqualTo(1), "The renamed medal is not shown");
                Assert.That(deleted, Is.Zero, "The deleted medal is still shown");
            });
    }

    /// <summary>
    /// A medal can be deleted (every post of the medals page failed, the medal list was bound).
    /// </summary>
    [Test]
    public async Task MedalCanBeDeletedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var name = $"Delete medal {Guid.NewGuid().ToString("N")[..8]}";

                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditMedal");
                await this.SaveMedalAsync(page, name);

                var item = page.Locator("li.list-group-item", new PageLocatorOptions { HasText = name });

                Assert.That(await item.CountAsync(), Is.EqualTo(1), "The medal was not created");

                // the confirm dialog of the button is skipped
                await item.Locator("button[formaction*='Delete']").First.EvaluateAsync("b => b.removeAttribute('onclick')");

                IResponse response = null;

                await page.RunAndWaitForNextPageAsync(
                    async () => response = await page.RunAndWaitForResponseAsync(
                                    () => item.Locator("button[formaction*='Delete']").First.EvaluateAsync("b => b.form.requestSubmit(b)"),
                                    r => r.Request.Method == "POST"));

                Assert.That(response.Status, Is.LessThan(500), "Deleting the medal failed");
                Assert.That(
                    await page.Locator("li.list-group-item", new PageLocatorOptions { HasText = name }).CountAsync(),
                    Is.Zero,
                    "The medal was not deleted");
            });
    }

    /// <summary>
    /// Saves the medal form with the name and the first image.
    /// </summary>
    private async Task SaveMedalAsync(IPage page, string name)
    {
        await page.Locator("#Input_Name").FillAsync(name);
        await page.EvaluateAsync(
            """
            () => {
                const select = document.querySelector('#Input_MedalImage');
                if (select.selectedIndex < 1) {
                    select.selectedIndex = 1;
                }
            }
            """);
        await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));
    }

    /// <summary>
    /// After saving a language file the texts are shown again.
    /// </summary>
    [Test]
    public async Task EditLanguageSaveShowsTextsTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditLanguage/german-du.json");

                var texts = await page.Locator("textarea[id$='LocalizedResourceText']").CountAsync();

                Assert.That(texts, Is.GreaterThan(0), "The language file shows no texts");

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                Assert.That(await page.ContentAsync(), Does.Contain("The language file has been updated and saved!"), "Not saved");

                Assert.That(
                    await page.Locator("textarea[id$='LocalizedResourceText']").CountAsync(),
                    Is.EqualTo(texts),
                    "The texts are not shown after saving");
            });
    }

    /// <summary>
    /// A topic deleted in the moderation of a forum (soft deleted) is not listed anymore (failed on PostgreSQL).
    /// </summary>
    [Test]
    public async Task ModerateDeleteTopicTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                var subject = $"Moderate delete {Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}PostTopic/{this.TestSettings.TestForumId}");
                await page.Locator("//input[contains(@id,'_TopicSubject')]").FillAsync(subject);
                await page.FrameLocator(".sceditor-container >> iframe").Locator("body").FillAsync("A topic to delete");
                await page.SubmitAndWaitAsync(page.Locator("//*[contains(@formaction,'PostReply')]"));

                Assert.That(await page.ContentAsync(), Does.Contain(subject), "The topic was not created");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Moderate/Forums/{this.TestSettings.TestForumId}");

                var topic = page.Locator("li.list-group-item", new PageLocatorOptions { HasText = subject });

                await topic.Locator("input[type=checkbox]").CheckAsync();

                // confirmed in a dialog
                await page.Locator("button[formaction*='DeleteTopics']").ClickAsync();
                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                Assert.That(await page.ContentAsync(), Does.Contain("Topic(s) deleted."), "The topic was not deleted");
                Assert.That(await topic.CountAsync(), Is.Zero, "The deleted topic is listed");
            });
    }

    /// <summary>
    /// The moderation of a forum shows the topics again with the warning "nothing selected".
    /// </summary>
    [Test]
    public async Task ModerateDeleteNothingSelectedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Moderate/Forums/{this.TestSettings.TestForumId}");

                var topics = await page.Locator("input[type=hidden][id$='TopicID']").CountAsync();

                Assert.That(topics, Is.GreaterThan(0), "The test forum has no topics");

                // delete without a selected topic (confirmed in a dialog)
                await page.Locator("button[formaction*='DeleteTopics']").ClickAsync();
                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                Assert.That(
                    await page.ContentAsync(),
                    Does.Contain("You need to select at least one topic"),
                    "The warning is missing");

                Assert.That(
                    await page.Locator("input[type=hidden][id$='TopicID']").CountAsync(),
                    Is.EqualTo(topics),
                    "The topics are not shown with the warning");

                Assert.That(
                    await page.Locator("a[href*='CleanStringForUrl']").CountAsync(),
                    Is.Zero,
                    "The topic links contain code");
            });
    }

    /// <summary>
    /// "Run SQL", "ReIndex", the languages, "Restart App", the board editor and the test data generator work on the whole
    /// database or application: only the host admin can use them, they can't be granted to the admin
    /// of a board (also not with a manipulated form).
    /// </summary>
    [Test]
    public async Task DatabasePagesAreHostOnlyTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await this.LoginAdminAsync(page);

                var userId = await this.FindUserIdAsync(page, userName);

                // the user becomes an admin of the board
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");
                await page.Locator("//button[contains(@data-bs-target,'#View2')]").ClickAsync();
                await page.GetByLabel("Administrators").First.CheckAsync();
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UsersGroups?handler=Save')]"));

                // the host grants the pages of the board admin
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/PageAccessEdit/{userId}");

                var pages = await page.Locator("input[type=hidden][id$='PageName']").EvaluateAllAsync<string[]>("inputs => inputs.map(i => i.value)");

                Assert.That(pages, Does.Contain("Admin_Forums"), "The page list is missing");
                Assert.That(pages, Does.Not.Contain("Admin_RunSql").And.Not.Contain("Admin_ReIndex"), "The database pages can be granted");
                Assert.That(pages, Does.Not.Contain("Admin_Languages").And.Not.Contain("Admin_EditLanguage"), "The language pages (used by all boards) can be granted");
                Assert.That(pages, Does.Not.Contain("Admin_RestartApp").And.Not.Contain("Admin_TestData").And.Not.Contain("Admin_EditBoard"), "Restart App, the test data generator or the board editor can be granted");

                // a manipulated form grants them anyway
                await page.EvaluateAsync(
                    """
                    () => {
                        const form = document.querySelector("input[id$='PageName']").form;
                        ['Admin_RunSql', 'Admin_ReIndex', 'Admin_Languages', 'Admin_EditLanguage', 'Admin_RestartApp', 'Admin_TestData', 'Admin_EditBoard'].forEach((name, i) => {
                            const index = 900 + i;
                            form.insertAdjacentHTML('beforeend',
                                `<input type="hidden" name="AccessList[${index}].PageName" value="${name}" />` +
                                `<input type="hidden" name="AccessList[${index}].ReadAccess" value="true" />`);
                        });
                    }
                    """);

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']").First);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                foreach (var adminPage in new[] { "RunSql", "ReIndex", "Languages", "EditLanguage/english.json", "RestartApp", "TestData", "EditBoard" })
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/{adminPage}");

                    Assert.That(page.Url, Does.Not.Contain($"Admin/{adminPage}"), $"The board admin can open {adminPage}");
                }
            });
    }

    /// <summary>
    /// A board admin (not the host admin) sees and changes the "moderated" setting of a user: the setting was only
    /// shown to the host admin, saving the user info (e.g. the rank) as board admin removed it.
    /// </summary>
    [Test]
    public async Task BoardAdminChangesModeratedSettingTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var boardAdmin = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);
                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                var moderatedUser = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);
                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await this.LoginAdminAsync(page);

                var boardAdminId = await this.FindUserIdAsync(page, boardAdmin);
                var moderatedUserId = await this.FindUserIdAsync(page, moderatedUser);

                // the user becomes an admin of the board with all pages
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{boardAdminId}");
                await page.Locator("//button[contains(@data-bs-target,'#View2')]").ClickAsync();
                await page.GetByLabel("Administrators").First.CheckAsync();
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UsersGroups?handler=Save')]"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/PageAccessEdit/{boardAdminId}");
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='GrantAll']").First);

                // the host admin moderates the other user
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{moderatedUserId}");
                await page.Locator("#Input_Moderated").CheckAsync();
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UsersInfo') and contains(@formaction,'Save')]"));

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                // the board admin saves the user info
                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, boardAdmin, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{moderatedUserId}");

                // the tab is loaded with ajax
                await page.Locator("#Input_RankID").WaitForAsync();

                Assert.That(await page.Locator("#Input_Moderated").CountAsync(), Is.EqualTo(1), "The setting is not shown to the board admin");
                Assert.That(await page.Locator("#Input_Moderated").IsCheckedAsync(), Is.True, "The setting is not shown as saved");
                Assert.That(await page.Locator("#Input_IsHostAdminX").CountAsync(), Is.Zero, "The host admin setting is shown to the board admin");

                // saved unchanged: the user stays moderated
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UsersInfo') and contains(@formaction,'Save')]"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{moderatedUserId}");

                Assert.That(await page.Locator("#Input_Moderated").IsCheckedAsync(), Is.True, "The user is not moderated anymore");

                // the board admin ends the moderation
                await page.Locator("#Input_Moderated").UncheckAsync();
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UsersInfo') and contains(@formaction,'Save')]"));

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{moderatedUserId}");

                Assert.That(await page.Locator("#Input_Moderated").IsCheckedAsync(), Is.False, "The board admin can't end the moderation");
            });
    }

    /// <summary>
    /// The good word of a replace word is text: html is removed when it is saved (and encoded in the pages), it was
    /// added as html to the topic subject (the topic page and the topic list) and the message.
    /// </summary>
    [Test]
    public async Task ReplaceWordGoodWordIsEncodedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                var id = Random.Shared.Next();
                var badWord = $"zzbad{id}";
                var marker = $"goodword{id}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ReplaceWords");

                var status = await page.EvaluateAsync<string>(
                                 """
                                 async ([badWord, goodWord]) => {
                                     const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                                     const response = await fetch('/api/ReplaceWords/Edit', {
                                         method: 'POST',
                                         headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
                                         body: JSON.stringify({ id: 0, badWord: badWord, goodWord: goodWord })
                                     });
                                     return String(response.status);
                                 }
                                 """,
                                 new[] { badWord, $"<b id=\"{marker}\">nice</b>" });

                Assert.That(status, Is.EqualTo("200"), "The replace word was not saved");

                try
                {
                    var topicId = await page.CreateTopicAsync(
                                      this.TestSettings,
                                      this.TestSettings.TestForumId,
                                      $"Replace {badWord} subject",
                                      $"Replace {badWord} message");

                    Assert.That(topicId, Is.Not.Null, "The topic was not created");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Posts/{topicId}/topic");

                    Assert.That(await page.Locator($"#{marker}").CountAsync(), Is.Zero, "The good word is html on the topic page");
                    Assert.That(await page.Locator("body").InnerTextAsync(), Does.Contain("Replace nice subject"), "The good word is not shown");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Topics/{this.TestSettings.TestForumId}/testForum");

                    Assert.That(await page.Locator($"#{marker}").CountAsync(), Is.Zero, "The good word is html in the topic list");
                }
                finally
                {
                    // the replace word is removed again
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/ReplaceWords");

                    var deleteUrl = await page.Locator("li.list-group-item", new PageLocatorOptions { HasText = badWord })
                                        .Locator("button[formaction*='Delete']").First.GetAttributeAsync("formaction");

                    await page.EvaluateAsync(
                        """
                        async url => {
                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                            await fetch(url, { method: 'POST', body: new URLSearchParams({ '__RequestVerificationToken': token }) });
                        }
                        """,
                        deleteUrl);
                }
            });
    }

    /// <summary>
    /// The edit page of a board shows the language of that board (not the one of the current board),
    /// otherwise every save changed the language of the board; a board without a description can be saved.
    /// </summary>
    [Test]
    public async Task EditBoardShowsItsCultureTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                var boardName = $"Second Board {Random.Shared.Next()}";

                // a board in german, without a description and without a new admin
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditBoard");
                await page.Locator("#Input_Name").FillAsync(boardName);

                if (await page.Locator("#Input_CreateAdminUser").IsCheckedAsync())
                {
                    await page.Locator("#Input_CreateAdminUser").UncheckAsync();
                }

                // the select is a Choices.js select (only the selected option is in the form)
                await page.EvaluateAsync(
                    """
                    () => {
                        const select = document.querySelector('#Input_Culture');
                        select.add(new Option('de-DE', 'de-DE', true, true));
                        select.value = 'de-DE';
                    }
                    """);

                await page.SubmitAndWaitAsync(page.Locator("button[formaction$='/Save'], button[formaction*='handler=Save']").First);

                var editLink = await page.Locator(".list-group-item", new PageLocatorOptions { HasText = boardName })
                                   .Locator("a[href*='EditBoard/']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "The board was not created");

                var editUrl = new Uri(new Uri(this.TestSettings.TestForumUrl), editLink).ToString();

                await page.GotoAsync(editUrl);

                Assert.That(
                    await page.Locator("#Input_Culture").InputValueAsync(),
                    Is.EqualTo("de-DE"),
                    "The edit page shows the language of the current board");

                // saved without a description
                var response = await page.RunAndWaitForNavigationAsync(
                                   () => page.Locator("button[formaction$='/Save'], button[formaction*='handler=Save']").First.ClickAsync());

                Assert.That(response?.Status, Is.LessThan(500), "The board without a description can not be saved");
                Assert.That(page.Url, Does.Contain("Admin/Boards"), "The board was not saved");
            });
    }

    /// <summary>
    /// The test data generator is not available in release builds: also its "create" handler is refused
    /// (it created boards, users and private messages).
    /// </summary>
    [Test]
    public async Task TestDataGeneratorIsDisabledTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Admin");

                // the handler, sent with a valid antiforgery token of another page
                var url = await page.EvaluateAsync<string>(
                    """
                    async url => {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                        const body = new URLSearchParams({ '__RequestVerificationToken': token, 'Input.UsersNumber': '1' });
                        const response = await fetch(url, { method: 'POST', body: body });
                        return response.url;
                    }
                    """,
                    $"{this.TestSettings.TestForumUrl}Admin/TestData?handler=CreateTestData");

                Assert.That(url, Does.Not.Contain("Admin/TestData"), "The test data generator can be used");
            });
    }

    /// <summary>
    /// An admin of the board without the access to an admin page can't use the api of its dialogs
    /// (e.g. the banned ip dialog).
    /// </summary>
    [Test]
    public async Task AdminApiNeedsPageAccessTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                var userName = await page.RegisterNewTestUserAsync(this.TestSettings, this.Fixture.SmtpServer);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                await this.LoginAdminAsync(page);

                var userId = await this.FindUserIdAsync(page, userName);

                // the user becomes an admin of the board
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");
                await page.Locator("//button[contains(@data-bs-target,'#View2')]").ClickAsync();
                await page.GetByLabel("Administrators").First.CheckAsync();
                await page.SubmitAndWaitAsync(page.Locator("//button[contains(@formaction,'UsersGroups?handler=Save')]"));

                // the host grants only the admin start page
                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/PageAccessEdit/{userId}");
                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']").First);

                Assert.That(await page.LogOutUserAsync(), Is.True, "Logout failed");

                Assert.That(
                    await page.LoginUserAsync(this.TestSettings, userName, this.TestSettings.TestUserPassword),
                    Is.True,
                    "Login failed");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/BannedIps");

                Assert.That(page.Url, Does.Not.Contain("Admin/BannedIps"), "The board admin can open the banned ips page");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Admin");

                Assert.That(page.Url, Does.Contain("Admin/Admin"), "The board admin can't open the admin start page");

                // the dialog of the banned ips page, sent with a valid antiforgery token
                var status = await page.EvaluateAsync<int>(
                    """
                    async url => {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                        const response = await fetch(url, {
                            method: 'POST',
                            headers: { 'RequestVerificationToken': token, 'Content-Type': 'application/json' },
                            body: JSON.stringify({ mask: '203.0.113.77', banReason: 'Admin api test' })
                        });
                        return response.status;
                    }
                    """,
                    $"{this.TestSettings.TestForumUrl}api/BannedIp/Edit");

                Assert.That(status, Is.EqualTo(403), "The board admin could use the api of the banned ips page");
            });
    }

    /// <summary>
    /// A medal awarded to a group can be edited (the group of the award is sent with the dialog).
    /// </summary>
    [Test]
    public async Task GroupMedalCanBeEditedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                var medalName = $"Group Medal {Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditMedal");

                await page.Locator("#Input_Name").FillAsync(medalName);
                await page.Locator("#Input_Category").FillAsync("Tests");
                await page.Locator("#Input_Description").FillAsync("A medal of the tests");
                await page.Locator("#Input_Message").FillAsync("A medal of the tests");

                // the image select is replaced by a script control
                await page.Locator("#Input_MedalImage").EvaluateAsync("select => select.value = 'medal.webp'");

                await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Save']"));

                var editLink = await page.Locator(".list-group-item", new PageLocatorOptions { HasText = medalName })
                                   .Locator("a[href*='EditMedal']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "The medal was not saved");

                var medalUrl = new Uri(new Uri(this.TestSettings.TestForumUrl), editLink).ToString();

                // award the medal to a group
                await page.GotoAsync(medalUrl);

                await page.Locator(".card-footer button[data-bs-toggle='ajax-modal']").First.ClickAsync();

                var dialog = page.Locator(".modal.show");

                await dialog.Locator("select[name='GroupId']").EvaluateAsync(
                    "select => select.value = [...select.options].find(o => o.text.trim() === 'Registered Users').value");
                await dialog.Locator("#GroupMessage").FillAsync("First message");

                await this.SaveDialogAsync(page, dialog, "MedalTestFirst");

                Assert.That(await page.ContentAsync(), Does.Contain("First message"), "The medal was not awarded");

                // edit the award
                await page.Locator(".list-group-item", new PageLocatorOptions { HasText = "First message" })
                    .Locator(".btn-group button[data-bs-toggle='ajax-modal']").ClickAsync();

                dialog = page.Locator(".modal.show");

                await dialog.Locator("#GroupMessage").FillAsync("Second message");

                await this.SaveDialogAsync(page, dialog, "MedalTestSecond");

                var content = await page.ContentAsync();

                Assert.That(content, Does.Contain("Second message"), "The award was not changed");
                Assert.That(content, Does.Not.Contain("First message"), "The award was not changed");

                // the remove button of the context menu asks with the text (not the missing tag)
                Assert.That(
                    await page.Locator(".context-menu button[formaction*='RemoveGroup']").First.GetAttributeAsync("data-title"),
                    Is.EqualTo("Remove medal from this group?"),
                    "The confirm text of the context menu is missing");

                // remove the award
                await page.Locator(".list-group-item", new PageLocatorOptions { HasText = "Second message" })
                    .Locator(".btn-group button[formaction*='RemoveGroup']").ClickAsync();

                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                Assert.That(await page.ContentAsync(), Does.Not.Contain("Second message"), "The award was not removed");

                // award the medal to a user
                var userId = await this.FindUserIdAsync(page, this.TestSettings.TestUserName);

                await page.GotoAsync(medalUrl);

                await page.Locator(".card-footer button[data-bs-toggle='ajax-modal']").Last.ClickAsync();

                dialog = page.Locator(".modal.show");

                // the user select is replaced by a script control which sets the user id
                await dialog.Locator("input[name='UserID']").EvaluateAsync($"input => input.value = '{userId}'");
                await dialog.Locator("#UserMessage").FillAsync("User message");

                await this.SaveDialogAsync(page, dialog, "MedalTestUser");

                Assert.That(await page.ContentAsync(), Does.Contain("User message"), "The medal was not awarded to the user");

                // the remove buttons of a user award ask before the removal
                Assert.That(
                    await page.Locator(".context-menu button[formaction*='RemoveUser']").First.GetAttributeAsync("data-title"),
                    Is.EqualTo("Remove medal from this user?"),
                    "The context menu removes the award without asking");

                await page.Locator(".list-group-item", new PageLocatorOptions { HasText = "User message" })
                    .Locator(".btn-group button[formaction*='RemoveUser']").ClickAsync();

                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                Assert.That(await page.ContentAsync(), Does.Not.Contain("User message"), "The award was not removed");
            });
    }

    /// <summary>
    /// After an award of a medal is removed the other awards are still listed: the lists of the page were empty
    /// after the removal.
    /// </summary>
    [Test]
    public async Task MedalAwardsAreListedAfterRemovalTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                var medalName = $"Removal Medal {Random.Shared.Next()}";

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditMedal");
                await this.SaveMedalAsync(page, medalName);

                var editLink = await page.Locator(".list-group-item", new PageLocatorOptions { HasText = medalName })
                                   .Locator("a[href*='EditMedal']").First.GetAttributeAsync("href");

                Assert.That(editLink, Is.Not.Null, "The medal was not saved");

                var medalUrl = new Uri(new Uri(this.TestSettings.TestForumUrl), editLink).ToString();

                // awarded to two groups
                foreach (var (group, message) in new[] { ("Registered Users", "Registered award"), ("Administrators", "Administrators award") })
                {
                    await page.GotoAsync(medalUrl);
                    await page.Locator(".card-footer button[data-bs-toggle='ajax-modal']").First.ClickAsync();

                    var dialog = page.Locator(".modal.show");

                    await dialog.Locator("select[name='GroupId']").EvaluateAsync(
                        "(select, name) => select.value = [...select.options].find(o => o.text.trim() === name).value",
                        group);
                    await dialog.Locator("#GroupMessage").FillAsync(message);

                    await this.SaveDialogAsync(page, dialog, "MedalRemovalGroup");
                }

                // awarded to two users
                foreach (var (user, message) in new[] { (this.TestSettings.TestUserName, "First user award"), (this.TestSettings.AdminUserName, "Second user award") })
                {
                    var userId = await this.FindUserIdAsync(page, user);

                    await page.GotoAsync(medalUrl);
                    await page.Locator(".card-footer button[data-bs-toggle='ajax-modal']").Last.ClickAsync();

                    var dialog = page.Locator(".modal.show");

                    await dialog.Locator("input[name='UserID']").EvaluateAsync($"input => input.value = '{userId}'");
                    await dialog.Locator("#UserMessage").FillAsync(message);

                    await this.SaveDialogAsync(page, dialog, "MedalRemovalUser");
                }

                await page.GotoAsync(medalUrl);

                var content = await page.ContentAsync();

                Assert.That(content, Does.Contain("Administrators award"), "The medal was not awarded to the group");
                Assert.That(content, Does.Contain("Second user award"), "The medal was not awarded to the user");

                // one group award is removed
                await page.Locator(".list-group-item", new PageLocatorOptions { HasText = "Registered award" })
                    .Locator(".btn-group button[formaction*='RemoveGroup']").ClickAsync();
                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                content = await page.ContentAsync();

                Assert.That(content, Does.Not.Contain("Registered award"), "The group award was not removed");
                Assert.That(content, Does.Contain("Administrators award"), "The other group award is not listed");
                Assert.That(content, Does.Contain("Second user award"), "The user awards are not listed");

                // one user award is removed
                await page.Locator(".list-group-item", new PageLocatorOptions { HasText = "First user award" })
                    .Locator(".btn-group button[formaction*='RemoveUser']").ClickAsync();
                await page.SubmitAndWaitAsync(page.Locator(".bootbox .btn-success"));

                content = await page.ContentAsync();

                Assert.That(content, Does.Not.Contain("First user award"), "The user award was not removed");
                Assert.That(content, Does.Contain("Second user award"), "The other user award is not listed");
                Assert.That(content, Does.Contain("Administrators award"), "The group awards are not listed");
                Assert.That(await page.Locator("#Input_Name").InputValueAsync(), Is.EqualTo(medalName), "The medal is not shown");
            });
    }

    /// <summary>
    /// Clicks the save button of a modal dialog and waits until the page is loaded again.
    /// </summary>
    private async Task SaveDialogAsync(IPage page, ILocator dialog, string marker)
    {
        // the page is loaded again after the save, the marker is gone then
        await page.EvaluateAsync($"() => window.{marker} = true");

        var response = await page.RunAndWaitForResponseAsync(
                           () => dialog.Locator("button[data-bs-save='editModal']").ClickAsync(),
                           r => r.Request.Method == "POST" && r.Url.Contains("/api/"));

        Assert.That(response.Status, Is.EqualTo(200), "The dialog was not saved");

        await page.WaitForFunctionAsync($"() => !window.{marker}");
        await page.WaitForLoadStateAsync(LoadState.Load);
    }

    /// <summary>
    /// The attachments tab of the admin edit user page is shown for the edited user.
    /// </summary>
    [Test]
    public async Task UserAttachmentsTabIsShownTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                var userId = await this.FindUserIdAsync(page, this.TestSettings.TestUserName);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");

                // the tab is loaded with ajax
                await page.Locator("//button[contains(@data-bs-target,'#View11')]").ClickAsync();

                await page.Locator("#View11 input[name='Input.UserId']").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached });

                Assert.That(
                    await page.Locator("#View11 input[name='Input.UserId']").InputValueAsync(),
                    Is.EqualTo(userId),
                    "The attachments of the user are not shown");
            });
    }

    /// <summary>
    /// A board announcement with a huge duration is saved (limited to 100 years) and shown.
    /// </summary>
    [Test]
    public async Task BoardAnnouncementWithHugeDurationTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/BoardAnnouncement");

                var message = $"Test announcement {Random.Shared.Next()}";

                await page.Locator("#Input_Message").FillAsync(message);
                await page.Locator("#Input_BoardAnnouncementUntil").FillAsync("999999999");

                var response = await page.RunAndWaitForResponseAsync(
                                   () => page.Locator("button[formaction*='SaveAnnouncement']").ClickAsync(),
                                   r => r.Request.Method == "POST" && r.Url.Contains("BoardAnnouncement"));

                Assert.That(response.Status, Is.LessThan(400), $"The announcement was not saved ({response.Status})");

                try
                {
                    await page.GotoAsync(this.TestSettings.TestForumUrl);

                    Assert.That(await page.ContentAsync(), Does.Contain(message), "The announcement is not shown");
                }
                finally
                {
                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/BoardAnnouncement");

                    await page.SubmitAndWaitAsync(page.Locator("button[formaction*='Delete']"));
                }
            });
    }

    /// <summary>
    /// The board the forum runs on can not be deleted (the forum was broken afterwards):
    /// no delete button for it, and the delete request is refused.
    /// </summary>
    [Test]
    public async Task CurrentBoardCanNotBeDeletedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
            {
                await this.LoginAdminAsync(page);

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Boards");

                var current = page.Locator("li.list-group-item.active");

                var idText = await current.Locator("small").First.InnerTextAsync();
                var boardId = idText[(idText.LastIndexOf(':') + 1)..].Trim();

                Assert.That(
                    await current.Locator("button[formaction*='Delete']").CountAsync(),
                    Is.Zero,
                    "The current board has a delete button");

                var deleteStatus = await page.EvaluateAsync<int>(
                    """
                    async url => {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                        const response = await fetch(url, { method: 'POST', body: new URLSearchParams({ '__RequestVerificationToken': token }) });
                        return response.status;
                    }
                    """,
                    $"{this.TestSettings.TestForumUrl}Admin/Boards/Delete?id={boardId}");

                await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Boards");

                Assert.That(
                    await page.Locator("li.list-group-item.active").CountAsync(),
                    Is.EqualTo(1),
                    $"The current board was deleted ({deleteStatus})");
            });
    }

    private async Task LoginAdminAsync(IPage page)
    {
        Assert.That(
            await page.LoginUserAsync(this.TestSettings, this.TestSettings.AdminUserName, this.TestSettings.AdminPassword),
            Is.True,
            "Login failed");
    }

    private async Task<string> FindUserIdAsync(IPage page, string userName)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Users");

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Filter" }).ClickAsync();

        var searchNameInput = page.Locator("//input[contains(@id,'_Name')]");
        await searchNameInput.FillAsync(userName);
        await page.RunAndWaitForNextPageAsync(() => searchNameInput.PressAsync("Enter"));

        var href = await page.Locator("//a[contains(@href,'EditUser')]", new PageLocatorOptions { HasText = userName })
                       .First.GetAttributeAsync("href");

        Assert.That(href, Is.Not.Null, $"{userName} not found");

        return href[(href.LastIndexOf('/') + 1)..];
    }

    private async Task ChangePointsAsync(IPage page, string userId, string field, string handler, string value)
    {
        await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/EditUser/{userId}");

        // the tab is loaded with ajax
        await page.Locator("//button[contains(@data-bs-target,'#View6')]").ClickAsync();

        var input = page.Locator($"#View6 #Input_{field}");
        await input.WaitForAsync();
        await input.FillAsync(value);

        await page.SubmitAndWaitAsync(page.Locator($"#View6 button[formaction*='{handler}']"));
    }

    private async Task<string> PointsAsync(IPage page)
    {
        await page.Locator("//button[contains(@data-bs-target,'#View6')]").ClickAsync();

        var badge = page.Locator("#View6 h6 .badge");
        await badge.WaitForAsync();

        return (await badge.InnerTextAsync()).Trim();
    }
}
