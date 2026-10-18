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

namespace YAF.Tests.UITests.Tests;

using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The admin lists with an edit dialog, an export and an import (banned e-mails, ips, names and user agents, spam
/// words, replace words) and the bb code extensions.
/// </summary>
[TestFixture]
public class AdminListsTests : ForumTestBase
{
    [Test]
    [TestCase("BannedEmails", "Add New Email", "Import Email(s)", "mask", "BanReason", "ui{0}@example.com", "ui{0}x@example.com", "txt", true)]
    [TestCase("BannedIps", "Add New Address", "Import Address(es)", "mask", "BanReason", "198.51.{1}.*", "198.51.{1}.1", "txt", true)]
    [TestCase("BannedNames", "Add New User Name", "Import Name(s)", "mask", "BanReason", "uiname{0}", "uiname{0}x", "txt", true)]
    [TestCase("BannedUserAgents", "Add New UserAgent", "Import UserAgent(s)", "mask", null, "UiAgent{0}", "UiAgent{0}x", "txt", true)]
    [TestCase("SpamWords", "Add New Word", "Import from XML", "spamword", null, "uispam{0}", "uispam{0}x", "xml", false)]
    [TestCase("ReplaceWords", "Add New Word Replace", "Import from XML", "badword", "goodword", "uibad{0}", "uibad{0}x", "xml", false)]
    [Description("An entry is added (twice), found, edited, exported, deleted and imported again from the export, a file that is not text is refused")]
    public async Task Add_Edit_Export_Import_Test(
        string page,
        string addText,
        string importText,
        string field,
        string secondField,
        string valueFormat,
        string editedFormat,
        string exportType,
        bool hasDuplicateCheck)
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var octet = new Random().Next(1, 254);
        var value = string.Format(valueFormat, id, octet);
        var edited = string.Format(editedFormat, id, octet);

        await this.LoginAsAdminAsync();

        // add
        await this.GotoAsync($"Admin/{page}");
        await this.AddAsync(addText, field, secondField, value);

        if (hasDuplicateCheck)
        {
            // the same entry again is refused (with a warning)
            await this.AddAsync(addText, field, secondField, value);
        }

        await this.FindAsync(page, value);
        (await this.Rows(value).CountAsync()).Should().Be(1);

        // edit: the dialog shows the entry
        await this.ClickAndWaitAsync(this.Rows(value).Locator("[id$='_ThemeButtonEdit'], [id$='_btnEdit']").First);
        await this.WaitForVisibleAsync("[id$='_EditDialog_Save']");
        (await this.Control($"EditDialog_{field}").InputValueAsync()).Should().Be(value);

        await this.Control($"EditDialog_{field}").FillAsync(edited);
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        await this.FindAsync(page, edited);
        (await this.Rows(edited).CountAsync()).Should().Be(1);

        // export
        var download = await this.Page.RunAndWaitForDownloadAsync(
            () => this.Page.Locator(".card-footer [id$='_Linkbutton4']").ClickAsync());
        var export = File.ReadAllBytes(await download.PathAsync());
        System.Text.Encoding.UTF8.GetString(export).Should().Contain(edited);

        // delete
        await this.Rows(edited).Locator("[id$='_ThemeButtonDelete']").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.FindAsync(page, edited);
        (await this.Rows(edited).CountAsync()).Should().Be(0);

        // the export is imported
        await this.ImportAsync(
            importText,
            new FilePayload { Name = $"export.{exportType}", MimeType = $"text/{(exportType == "xml" ? "xml" : "plain")}", Buffer = export });

        await this.FindAsync(page, edited);
        (await this.Rows(edited).CountAsync()).Should().Be(1, "the entry is imported");

        // imported again: the existing entry is not added twice
        await this.ImportAsync(
            importText,
            new FilePayload { Name = $"export.{exportType}", MimeType = $"text/{(exportType == "xml" ? "xml" : "plain")}", Buffer = export });

        await this.FindAsync(page, edited);
        (await this.Rows(edited).CountAsync()).Should().Be(1, "the entry is not imported twice");

        // a broken xml file: the dialog is opened again
        if (exportType == "xml")
        {
            await this.ImportAsync(
                importText,
                new FilePayload { Name = "broken.xml", MimeType = "text/xml", Buffer = System.Text.Encoding.UTF8.GetBytes("<YafReplaceWords><broken") });

            await this.WaitForVisibleAsync("[id$='_ImportDialog_Import']");
            await this.GotoAsync($"Admin/{page}");
        }

        // a file that is not text: the dialog is opened again
        await this.ImportAsync(
            importText,
            new FilePayload { Name = "image.png", MimeType = "image/png", Buffer = [137, 80, 78, 71, 13, 10, 26, 10] });

        await this.WaitForVisibleAsync("[id$='_ImportDialog_Import']");
        this.BrowserErrors.Should().BeEmpty();
    }

    [Test]
    [Description("A bb code with an invalid search expression is refused, a valid one is saved, edited, exported, deleted and imported again")]
    public async Task BBCode_Test()
    {
        var name = $"UITEST{DateTime.UtcNow:HHmmssfff}";

        await this.LoginAsAdminAsync();

        await this.GotoAsync("Admin/BBCodes");
        await this.ClickAndWaitAsync(this.Page.Locator(".card-footer [id$='_Linkbutton3']"));

        await this.Control("txtName").FillAsync(name);
        await this.Control("txtDescription").FillAsync("A bb code of the UI test");
        await this.Control("txtSearchRegEx").FillAsync(@"\[uitest\](.*?)\[/uitest\]");
        await this.Control("txtReplaceRegEx").FillAsync("<span class=\"uitest\">$1</span>");

        // an invalid search expression
        await this.Control("txtSearchRegEx").FillAsync(@"[uitest(");
        await this.ClickAndWaitAsync(this.Control("Save"));
        this.Page.Url.Should().Contain("BBCode_Edit", "the invalid expression is refused");

        await this.Control("txtSearchRegEx").FillAsync(@"\[uitest\](.*?)\[/uitest\]");
        await this.ClickAndWaitAsync(this.Control("Save"));

        // edit
        await this.ShowAllBBCodesAsync();
        await this.ClickAndWaitAsync(this.Rows(name).Locator("[id$='_ThemeButtonEdit']"));
        (await this.Control("txtName").InputValueAsync()).Should().Be(name);
        await this.Control("txtDescription").FillAsync("The edited bb code of the UI test");
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.ShowAllBBCodesAsync();
        (await this.Rows(name).InnerTextAsync()).Should().Contain("The edited bb code");

        // nothing selected: no export
        await this.ClickAndWaitAsync(this.Page.Locator(".card-footer [id$='_Linkbutton4']"));

        // export of the selected bb code
        await this.ShowAllBBCodesAsync();
        await this.Rows(name).Locator("input[type='checkbox']").CheckAsync();
        var download = await this.Page.RunAndWaitForDownloadAsync(
            () => this.Page.Locator(".card-footer [id$='_Linkbutton4']").ClickAsync());
        var export = File.ReadAllBytes(await download.PathAsync());
        System.Text.Encoding.UTF8.GetString(export).Should().Contain(name).And.Contain("The edited bb code");

        // delete
        await this.ShowAllBBCodesAsync();
        await this.Rows(name).Locator("[id$='_ThemeButtonDelete']").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.ShowAllBBCodesAsync();
        (await this.Rows(name).CountAsync()).Should().Be(0);

        // import of the export
        await this.ImportAsync(
            "Import from XML",
            new FilePayload { Name = "BBCodeExport.xml", MimeType = "text/xml", Buffer = export });

        await this.ShowAllBBCodesAsync();
        (await this.Rows(name).CountAsync()).Should().Be(1, "the bb code is imported");

        // a file that is not text
        await this.ImportAsync(
            "Import from XML",
            new FilePayload { Name = "image.png", MimeType = "image/png", Buffer = [137, 80, 78, 71, 13, 10, 26, 10] });

        await this.WaitForVisibleAsync("[id$='_ImportDialog_Import']");
        this.BrowserErrors.Should().BeEmpty();
    }

    /// <summary>
    /// The rows of the list with the text.
    /// </summary>
    [Test]
    [Description("The tag of a topic is listed in the admin tags (another page size) and deleted")]
    public async Task Admin_Tags_Test()
    {
        var tag = $"uitag{Guid.NewGuid():N}".Substring(0, 14);

        await this.LoginAsAdminAsync();
        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync($"Tagged {tag}");
        await this.Control("YafTextEditor").FillAsync("A topic with a tag.");
        await this.Control("TagsValue").EvaluateAsync("(e, v) => e.value = v", tag);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Admin/Tags");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("50", new LocatorSelectOptionOptions { Force = true }));

        var row = this.Page.Locator("li.list-group-item", new PageLocatorOptions { HasTextString = tag });
        (await row.CountAsync()).Should().Be(1);

        await row.Locator(".btn-group a:has(i.fa-trash)").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await this.Page.Locator("li.list-group-item", new PageLocatorOptions { HasTextString = tag }).CountAsync()).Should().Be(0);
    }

    private ILocator Rows(string text)
    {
        return this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = text });
    }

    private async Task WaitForVisibleAsync(string selector)
    {
        await this.Page.Locator(selector).WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }

    /// <summary>
    /// Adds an entry with the edit dialog (opened by the add button of the list).
    /// </summary>
    private async Task AddAsync(string addText, string field, string secondField, string value)
    {
        await this.ClickAndWaitAsync(
            this.Page.Locator(".card-footer a", new PageLocatorOptions { HasTextString = addText }));
        await this.WaitForVisibleAsync("[id$='_EditDialog_Save']");

        await this.Control($"EditDialog_{field}").FillAsync(value);

        if (secondField != null)
        {
            await this.Control($"EditDialog_{secondField}").FillAsync("UI test");
        }

        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));
    }

    /// <summary>
    /// Shows the entry: the search of the list (the replace words have no search and no pages).
    /// </summary>
    private async Task FindAsync(string page, string value)
    {
        await this.GotoAsync($"Admin/{page}");

        if (await this.Control("SearchInput").CountAsync() == 0)
        {
            return;
        }

        await this.Page.Locator("a.dropdown-toggle[aria-label='filter']").ClickAsync();
        await this.Control("SearchInput").FillAsync(value);
        await this.ClickAndWaitAsync(this.Control("search"));
    }

    /// <summary>
    /// Imports the file with the import dialog of the list.
    /// </summary>
    private async Task ImportAsync(string importText, FilePayload file)
    {
        await this.Page.Locator(".card-footer a", new PageLocatorOptions { HasTextString = importText }).ClickAsync();
        await this.WaitForVisibleAsync("[id$='_ImportDialog_Import']");

        await this.Control("ImportDialog_importFile").SetInputFilesAsync(file);
        await this.ClickAndWaitAsync(this.Control("ImportDialog_Import"));
    }

    private async Task ShowAllBBCodesAsync()
    {
        await this.GotoAsync("Admin/BBCodes");

        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("50", new LocatorSelectOptionOptions { Force = true }));
    }
}
