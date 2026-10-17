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
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The tools of the private message list (select, delete, export, mark as read, sort, page size).
/// </summary>
[TestFixture]
public class PrivateMessageListTests : ForumTestBase
{
    [Test]
    [Description("Delete selected deletes only the selected message of the inbox")]
    public async Task Delete_Selected_Test()
    {
        var (recipient, subjects) = await this.SendMessagesAsync(2);

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.GotoAsync("MyMessages");

        await this.SelectAsync(subjects[0]);
        await this.ConfirmAsync(this.Inbox("DeleteSelected"));

        var body = await this.Page.InnerTextAsync("body");
        body.Should().NotContain(subjects[0]);
        body.Should().Contain(subjects[1]);
    }

    [Test]
    [Description("Delete all deletes all messages of the inbox, the messages stay in the outbox of the sender")]
    public async Task Delete_All_Test()
    {
        var (recipient, subjects) = await this.SendMessagesAsync(2);

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.GotoAsync("MyMessages");

        await this.ConfirmAsync(this.Inbox("DeleteAll"));

        var body = await this.Page.InnerTextAsync("body");
        body.Should().NotContain(subjects[0]).And.NotContain(subjects[1]);

        await this.LoginAsAdminAsync();
        await this.GotoAsync("MyMessages?v=out");

        body = await this.Page.InnerTextAsync("body");
        body.Should().Contain(subjects[0]).And.Contain(subjects[1]);
    }

    [Test]
    [Description("Export selected exports the selected message as xml, export all all messages (also the ones of the other pages)")]
    public async Task Export_Test()
    {
        // a message to all users: the ids of the messages and of the messages of the users differ
        await this.LoginAsAdminAsync();
        await this.GotoAsync("PostPrivateMessage");
        await this.ClickAndWaitAsync(this.Control("AllUsers"));
        await this.Control("PmSubjectTextBox").FillAsync($"To all {Guid.NewGuid():N}");
        await this.Control("YafTextEditor").FillAsync("A message to all users.");
        await this.ClickAndWaitAsync(this.Control("Save"));

        var (recipient, subjects) = await this.SendMessagesAsync(6);

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.GotoAsync("MyMessages");

        await this.SelectAsync(subjects[5]);
        var selected = await this.DownloadAsync(this.Inbox("ExportSelected"));

        selected.Should().Contain(subjects[5]);
        subjects.Take(5).Should().AllSatisfy(s => selected.Should().NotContain(s));

        await this.GotoAsync("MyMessages");
        var all = await this.DownloadAsync(this.Inbox("ExportAll"));

        subjects.Should().AllSatisfy(s => all.Should().Contain(s), "the first page shows only 5 messages");
    }

    [Test]
    [Description("The sender deletes the sent message on the message page, the recipient still has it")]
    public async Task Sender_Deletes_Sent_Message_Test()
    {
        var (recipient, subjects) = await this.SendMessagesAsync(1);

        await this.LoginAsAdminAsync();
        await this.GotoAsync("MyMessages?v=out");

        var href = await this.Page.Locator("a", new PageLocatorOptions { HasTextString = subjects[0] }).First
                       .GetAttributeAsync("href");
        await this.GotoAsync(href.TrimStart('/'));

        await this.ConfirmAsync(this.Control("DeleteMessage").First);

        await this.GotoAsync("MyMessages?v=out");
        (await this.Page.InnerTextAsync("body")).Should().NotContain(subjects[0]);

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.GotoAsync("MyMessages");
        (await this.Page.InnerTextAsync("body")).Should().Contain(subjects[0]);
    }

    [Test]
    [Description("Mark all as read marks all messages of the inbox as read")]
    public async Task Mark_All_As_Read_Test()
    {
        var (recipient, _) = await this.SendMessagesAsync(2);

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.GotoAsync("MyMessages");

        (await this.Page.Locator("li.list-group-item i.fa-envelope").CountAsync()).Should().Be(2);

        await this.ClickAndWaitAsync(this.Inbox("MarkAsRead"));

        (await this.Page.Locator("li.list-group-item i.fa-envelope").CountAsync()).Should().Be(0);
        (await this.Page.Locator("li.list-group-item i.fa-envelope-open").CountAsync()).Should().Be(2);
    }

    [Test]
    [Description("The messages are sorted by subject")]
    public async Task Sort_By_Subject_Test()
    {
        var (recipient, subjects) = await this.SendMessagesAsync(3);

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.GotoAsync("MyMessages");

        await this.Inbox("Sort").ClickAsync();
        await this.ClickAndWaitAsync(this.Inbox("SortSubjectDesc"));

        (await this.SubjectsAsync()).Should().Equal(subjects.OrderByDescending(s => s, StringComparer.Ordinal));

        await this.Inbox("Sort").ClickAsync();
        await this.ClickAndWaitAsync(this.Inbox("SortSubjectAsc"));

        (await this.SubjectsAsync()).Should().Equal(subjects.OrderBy(s => s, StringComparer.Ordinal));
    }

    [Test]
    [Description("The number of messages per page is changed")]
    public async Task Page_Size_Test()
    {
        var (recipient, _) = await this.SendMessagesAsync(6);

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.GotoAsync("MyMessages");

        (await this.SubjectsAsync()).Should().HaveCount(5, "the page size of a new user");

        await this.Page.RunAndWaitForNavigationAsync(() => this.Inbox("PageSize").SelectOptionAsync("10"));
        await this.AssertNoServerErrorAsync();

        (await this.SubjectsAsync()).Should().HaveCount(6);
        (await this.Inbox("PageSize").InputValueAsync()).Should().Be("10");
    }

    [Test]
    [Description("A delete button asks for the confirmation wherever it is clicked (also on its border)")]
    public async Task Delete_Asks_For_Confirmation_Test()
    {
        var (recipient, subjects) = await this.SendMessagesAsync(1);

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.GotoAsync("MyMessages");

        // the corner of the button (not the icon or the text)
        await this.Inbox("DeleteAll").ClickAsync(new LocatorClickOptions { Position = new Position { X = 2, Y = 2 } });

        await this.Page.Locator(".bootbox .btn-danger").ClickAsync();

        await this.GotoAsync("MyMessages");
        (await this.Page.InnerTextAsync("body")).Should().Contain(subjects[0], "the delete was cancelled");
    }

    /// <summary>
    /// Sends messages from the admin to a new member.
    /// </summary>
    private async Task<(string Recipient, string[] Subjects)> SendMessagesAsync(int count)
    {
        var recipient = $"Pm{DateTime.UtcNow:HHmmssfff}";
        var id = Guid.NewGuid().ToString("N").Substring(0, 8);

        await this.RegisterAsync(recipient, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();

        var subjects = Enumerable.Range(1, count).Select(i => $"PM {id} {(char)('A' + i)}").ToArray();

        foreach (var subject in subjects)
        {
            await this.GotoAsync("PostPrivateMessage");
            await this.Control("To").FillAsync(recipient);
            await this.Control("PmSubjectTextBox").FillAsync(subject);
            await this.Control("YafTextEditor").FillAsync($"The message {subject}.");
            await this.ClickAndWaitAsync(this.Control("Save"));
        }

        return (recipient, subjects);
    }

    /// <summary>
    /// A control of the inbox list (the page has the lists of the inbox and the outbox).
    /// </summary>
    private ILocator Inbox(string id)
    {
        return this.Page.Locator($"[id$='_InboxPMList_{id}']");
    }

    private async Task SelectAsync(string subject)
    {
        var row = this.Page.Locator("li.list-group-item", new PageLocatorOptions { HasTextString = subject });

        await row.Locator("input[type=checkbox]").CheckAsync();
    }

    private async Task ConfirmAsync(ILocator button)
    {
        await button.ClickAsync();

        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
    }

    private async Task<string> DownloadAsync(ILocator button)
    {
        var download = await this.Page.RunAndWaitForDownloadAsync(() => button.ClickAsync());

        return File.ReadAllText(await download.PathAsync());
    }

    private async Task<string[]> SubjectsAsync()
    {
        var subjects = await this.Page.Locator("li.list-group-item a[href*='PrivateMessage']").AllInnerTextsAsync();

        return [.. subjects.Select(s => s.Trim())];
    }
}
