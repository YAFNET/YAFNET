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
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Writing a private message: reply, quote, report, finding the recipients and the checks of the input.
/// </summary>
[TestFixture]
public class PostPrivateMessageTests : ForumTestBase
{
    [Test]
    [Description("The recipient replies to a message: the reply goes to the sender with the subject \"Re: ...\"")]
    public async Task Reply_Test()
    {
        var (recipient, subject) = await this.SendFromAdminAsync();

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.OpenInboxMessageAsync(subject);

        await this.ClickAndWaitAsync(this.Control("ReplyMessage").First);

        (await this.Control("PmSubjectTextBox").InputValueAsync()).Should().Be($"Re: {subject}");
        (await this.Control("To").InputValueAsync()).Should().Be(TestConfig.AdminUserName);
        (await this.Control("To").IsDisabledAsync()).Should().BeTrue();

        await this.Control("YafTextEditor").FillAsync("The answer.");
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.LoginAsAdminAsync();
        await this.GotoAsync("MyMessages");
        (await this.Page.InnerTextAsync("body")).Should().Contain($"Re: {subject}");
    }

    [Test]
    [Description("A quote has the message of the sender in a quote")]
    public async Task Quote_Test()
    {
        var (recipient, subject) = await this.SendFromAdminAsync();

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.OpenInboxMessageAsync(subject);

        await this.ClickAndWaitAsync(this.Control("QuoteMessage").First);

        (await this.Control("YafTextEditor").InputValueAsync()).Should()
            .Be($"[QUOTE={TestConfig.AdminUserName}]The message {subject}.[/QUOTE]");
    }

    [Test]
    [Description("A message is reported to the host with the message in a quote")]
    public async Task Report_Test()
    {
        var (recipient, subject) = await this.SendFromAdminAsync();

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.OpenInboxMessageAsync(subject);

        await this.ClickAndWaitAsync(this.Control("ReportMessage").First);

        (await this.Control("PmSubjectTextBox").InputValueAsync()).Should()
            .Be($"Report a Private Message from User: {TestConfig.AdminUserName}");
        (await this.Control("To").InputValueAsync()).Should().Be(TestConfig.AdminUserName, "the admin is the host");
        (await this.Control("YafTextEditor").InputValueAsync()).Should().Contain($"[QUOTE={TestConfig.AdminUserName}]The message {subject}.[/QUOTE]");
    }

    [Test]
    [Description("A member can not reply to the message of other users")]
    public async Task Reply_To_Other_Message_Is_Denied_Test()
    {
        var (recipient, subject) = await this.SendFromAdminAsync();

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        var id = await this.OpenInboxMessageAsync(subject);

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync($"PostPrivateMessage?p={id}&q=1");

        (await this.Page.InnerTextAsync("body")).Should().NotContain($"The message {subject}.");
    }

    [Test]
    [Description("The recipients are found by a part of the name, cleared and sent to")]
    public async Task Find_Users_Test()
    {
        var subject = $"Found {Guid.NewGuid():N}";

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("PostPrivateMessage");

        // at least two letters
        await this.Control("To").FillAsync("T");
        await this.ClickAndWaitAsync(this.Control("FindUsers"));
        (await this.Page.ContentAsync()).Should().Contain("at least, the first two letters");

        // nobody
        await this.Control("To").FillAsync("NobodyHasThisName");
        await this.ClickAndWaitAsync(this.Control("FindUsers"));
        (await this.Page.ContentAsync()).Should().Contain("The User you are looking for does not exist");

        // the found users, the own name is not in the list
        await this.Control("To").FillAsync("TestUser");
        await this.ClickAndWaitAsync(this.Control("FindUsers"));

        var names = await this.Control("ToList").Locator("option").AllInnerTextsAsync();
        names.Should().Contain(TestConfig.TestUserName2).And.NotContain(TestConfig.TestUserName);

        // the search is cleared
        await this.ClickAndWaitAsync(this.Control("Clear"));
        (await this.Control("To").IsVisibleAsync()).Should().BeTrue();
        (await this.Control("ToList").CountAsync()).Should().Be(0);

        await this.Control("To").FillAsync(TestConfig.TestUserName2.Substring(0, 8));
        await this.ClickAndWaitAsync(this.Control("FindUsers"));
        await this.Control("ToList").SelectOptionAsync(
            new SelectOptionValue { Label = TestConfig.TestUserName2 },
            new LocatorSelectOptionOptions { Force = true });

        await this.Control("PmSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("To the found user.");
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync("MyMessages");
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);
    }

    [Test]
    [Description("A message needs a recipient, a subject and a text, the recipients must exist, a member sends to one recipient")]
    public async Task Input_Checks_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("PostPrivateMessage");

        await this.ClickAndWaitAsync(this.Control("Save"));
        (await this.Page.ContentAsync()).Should().Contain("Please specify the Recipient");

        await this.Control("To").FillAsync(TestConfig.TestUserName2);
        await this.ClickAndWaitAsync(this.Control("Save"));
        (await this.Page.ContentAsync()).Should().Contain("Please specify the Subject");

        await this.Control("PmSubjectTextBox").FillAsync("A subject");
        await this.ClickAndWaitAsync(this.Control("Save"));
        (await this.Page.ContentAsync()).Should().Contain("Please specify a message");

        await this.Control("YafTextEditor").FillAsync("The text.");

        // a member sends to one recipient
        await this.Control("To").FillAsync($"{TestConfig.TestUserName2}; {TestConfig.AdminUserName}");
        await this.ClickAndWaitAsync(this.Control("Save"));
        (await this.Page.ContentAsync()).Should().Contain("Too many recipients. Maximum allowed is 1.");

        await this.Control("To").FillAsync(" NobodyHasThisName ;");
        await this.ClickAndWaitAsync(this.Control("Save"));
        (await this.Page.ContentAsync()).Should().Contain("Sorry, user NobodyHasThisName is not a registered");

        this.Page.Url.Should().Contain("PostPrivateMessage", "the message is not sent");
    }

    [Test]
    [Description("A message to several recipients is sent to each of them")]
    public async Task Several_Recipients_Test()
    {
        var subject = $"Several {Guid.NewGuid():N}";

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.LoginAsAdminAsync();
        await this.GotoAsync("PostPrivateMessage");

        await this.Control("To").FillAsync($";{TestConfig.TestUserName} ;; {TestConfig.TestUserName2};");
        await this.Control("PmSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("To both.");
        await this.ClickAndWaitAsync(this.Control("Save"));

        foreach (var user in new[] { TestConfig.TestUserName, TestConfig.TestUserName2 })
        {
            await this.LoginAsUserAsync(user);
            await this.GotoAsync("MyMessages");
            (await this.Page.InnerTextAsync("body")).Should().Contain(subject, user);
        }
    }

    [Test]
    [Description("The preview shows the message, cancel goes back to the messages")]
    public async Task Preview_And_Cancel_Test()
    {
        var text = $"Preview {Guid.NewGuid():N}";

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("PostPrivateMessage");

        await this.Control("YafTextEditor").FillAsync($"[b]{text}[/b]");
        await this.ClickAndWaitAsync(this.Control("Preview"));

        (await this.Page.Locator("strong", new PageLocatorOptions { HasTextString = text }).CountAsync()).Should().Be(1);

        await this.ClickAndWaitAsync(this.Control("Cancel"));
        this.Page.Url.Should().Contain("MyMessages");
    }

    [Test]
    [Description("A member sends a message to all friends (the friends are the recipients)")]
    public async Task All_Friends_Test()
    {
        var alice = $"Afa{DateTime.UtcNow:HHmmssfff}";
        var bob = $"Afb{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(alice, TestConfig.TestUserPassword);
        await this.RegisterAsync(bob, TestConfig.TestUserPassword);

        // alice asks bob, bob approves
        var bobId = await this.UserIdAsync(bob);

        await this.LoginAsync(alice, TestConfig.TestUserPassword);
        await this.GotoAsync($"UserProfile/{bobId}-{bob}");
        await this.ClickAndWaitAsync(this.Control("lnkBuddy"));

        await this.LoginAsync(bob, TestConfig.TestUserPassword);
        await this.GotoAsync("Friends");
        await this.Page.Locator("a.nav-link[href='#PendingRequestsTab']").ClickAsync();
        await this.ClickAndWaitAsync(
            this.Page.Locator("#PendingRequestsTab a", new PageLocatorOptions { HasTextString = "Approve" }).First);

        await this.LoginAsync(alice, TestConfig.TestUserPassword);
        await this.GotoAsync("PostPrivateMessage");
        await this.ClickAndWaitAsync(this.Control("AllBuddies"));
        (await this.Control("To").InputValueAsync()).Should().Contain(bob);

        await this.Control("PmSubjectTextBox").FillAsync("To all friends");
        await this.Control("YafTextEditor").FillAsync("A message to all friends.");
        await this.ClickAndWaitAsync(this.Control("Save"));
        this.Page.Url.Should().Contain("MyMessages");
    }

    [Test]
    [Description("A member who blocks private messages gets none from other members")]
    public async Task Blocked_Recipient_Test()
    {
        var blocker = $"Blk{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(blocker, TestConfig.TestUserPassword);
        await this.LoginAsync(blocker, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/BlockOptions");
        await this.Control("BlockPMs").CheckAsync();
        await this.ClickAndWaitAsync(this.Control("SaveUser"));

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("PostPrivateMessage");
        await this.Control("To").FillAsync(blocker);
        await this.Control("PmSubjectTextBox").FillAsync("Blocked");
        await this.Control("YafTextEditor").FillAsync("A message that is not allowed.");
        await this.ClickAndWaitAsync(this.Control("Save"));

        this.Page.Url.Should().Contain("PostPrivateMessage", "the recipient blocks the messages");
    }

    [Test]
    [Description("A message of a new member with a spam word is checked (logged, the message is sent)")]
    public async Task Spam_Word_Message_Test()
    {
        var word = $"spampm{Guid.NewGuid():N}".Substring(0, 14);
        var sender = $"Sps{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(sender, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.GotoAsync("Admin/SpamWords");
        await this.ClickAndWaitAsync(
            this.Page.Locator("a", new PageLocatorOptions { HasTextString = "Add New Word" })
                .Filter(new LocatorFilterOptions { Visible = true }).First);
        await this.Page.Locator("[id$='_EditDialog_Save']").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await this.Control("EditDialog_spamword").FillAsync(word);
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        await this.LoginAsync(sender, TestConfig.TestUserPassword);
        await this.GotoAsync("PostPrivateMessage");
        await this.Control("To").FillAsync(TestConfig.AdminUserName);
        await this.Control("PmSubjectTextBox").FillAsync("Spam check");
        await this.Control("YafTextEditor").FillAsync($"A message with {word} in it.");
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.AssertNoServerErrorAsync();
    }

    /// <summary>
    /// The id of a user (admin users list).
    /// </summary>
    [Test]
    [Description("The recipient deletes the message on its page, the inbox shows the message that it was deleted")]
    public async Task Delete_On_Message_Page_Test()
    {
        var (recipient, subject) = await this.SendFromAdminAsync();

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.OpenInboxMessageAsync(subject);

        await this.Control("DeleteMessage").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        this.Page.Url.Should().Contain("MyMessages");
        (await this.Page.ContentAsync()).Should().Contain("Message was deleted.");
        (await this.Page.InnerTextAsync("body")).Should().NotContain(subject);
    }

    [Test]
    [Description("A message to the guest is refused, a full mailbox of the recipient and the own full mailbox are refused (limit of the role)")]
    public async Task Full_Mailbox_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var alice = $"Pfa{id}";
        var bob = $"Pfb{id}";
        var carol = $"Pfc{id}";

        foreach (var name in new[] { alice, bob, carol })
        {
            await this.RegisterAsync(name, TestConfig.TestUserPassword);
        }

        await this.LoginAsAdminAsync();
        var oldLimit = await this.SetRegisteredUsersPmLimitAsync("1");

        try
        {
            // not to the guest
            await this.LoginAsync(alice, TestConfig.TestUserPassword);
            await this.SendAsync("Guest", $"To the guest {id}");
            this.Page.Url.Should().Contain("PostPrivateMessage", "the guest gets no messages");

            // bob gets one message, his mailbox is full then
            await this.SendAsync(bob, $"First {id}");
            this.Page.Url.Should().NotContain("PostPrivateMessage");

            await this.LoginAsync(carol, TestConfig.TestUserPassword);
            await this.SendAsync(bob, $"Second {id}");
            this.Page.Url.Should().Contain("PostPrivateMessage", "the mailbox of the recipient is full");

            // the mailbox of alice is full (the sent message)
            await this.LoginAsync(alice, TestConfig.TestUserPassword);
            await this.SendAsync(carol, $"Third {id}");
            this.Page.Url.Should().Contain("PostPrivateMessage", "the own mailbox is full");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetRegisteredUsersPmLimitAsync(oldLimit);
        }
    }

    [Test]
    [Description("The sender answers the reply (the answer of a reply is sent), a member who ignores the sender gets no message")]
    public async Task Reply_To_Reply_And_Ignored_Sender_Test()
    {
        var (recipient, subject) = await this.SendFromAdminAsync();

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.OpenInboxMessageAsync(subject);
        await this.ClickAndWaitAsync(this.Control("ReplyMessage").First);
        await this.Control("YafTextEditor").FillAsync("The first answer.");
        await this.ClickAndWaitAsync(this.Control("Save"));

        // the admin answers the reply
        await this.LoginAsAdminAsync();
        await this.OpenInboxMessageAsync($"Re: {subject}");
        await this.ClickAndWaitAsync(this.Control("ReplyMessage").First);
        await this.Control("YafTextEditor").FillAsync("The answer of the answer.");
        await this.ClickAndWaitAsync(this.Control("Save"));
        await this.AssertNoServerErrorAsync();
        this.Page.Url.Should().Contain("MyMessages");

        await this.LoginAsync(recipient, TestConfig.TestUserPassword);
        await this.GotoAsync("MyMessages");
        (await this.Page.Locator("a", new PageLocatorOptions { HasTextString = $"Re: {subject}" }).CountAsync())
            .Should().BeGreaterThan(0, "the answer of the reply is in the inbox");

        // the recipient ignores the member
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        ExecuteSql(
            $"INSERT INTO [dbo].[yaf_IgnoreUser] ([UserID], [IgnoredUserID]) SELECT r.[UserID], s.[UserID] FROM [dbo].[yaf_User] r, [dbo].[yaf_User] s WHERE r.[Name] = @name AND s.[Name] = '{TestConfig.TestUserName}'",
            recipient);

        await this.SendAsync(recipient, $"Ignored {Guid.NewGuid():N}");
        this.Page.Url.Should().Contain("PostPrivateMessage", "the recipient ignores the sender");
    }

    private async Task SendAsync(string to, string subject)
    {
        await this.GotoAsync("PostPrivateMessage");
        await this.Control("To").FillAsync(to);
        await this.Control("PmSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync($"The message {subject}.");
        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    /// <summary>
    /// Sets the private message limit of the registered users, returns the old limit.
    /// </summary>
    private async Task<string> SetRegisteredUsersPmLimitAsync(string limit)
    {
        await this.GotoAsync("Admin/Groups");
        await this.ClickAndWaitAsync(
            this.Page.Locator("li", new PageLocatorOptions { HasTextString = "Registered Users" })
                .Filter(new LocatorFilterOptions { Has = this.Page.Locator("[id*='_RoleListYaf_']") })
                .Locator("[id$='_ThemeButtonEdit']").First);

        var old = await this.Control("PMLimit").InputValueAsync();

        await this.Control("PMLimit").EvaluateAsync("(e, v) => { e.type = 'text'; e.value = v; }", limit);
        await this.ClickAndWaitAsync(this.Control("Save"));

        return old;
    }

    private async Task<int> UserIdAsync(string userName)
    {
        await this.LoginAsAdminAsync();

        return await this.OpenAdminEditUserAsync(userName);
    }

    /// <summary>
    /// Sends a message from the admin to a new member.
    /// </summary>
    private async Task<(string Recipient, string Subject)> SendFromAdminAsync()
    {
        var recipient = $"Ppm{DateTime.UtcNow:HHmmssfff}";
        var subject = $"Message {Guid.NewGuid():N}";

        await this.RegisterAsync(recipient, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.GotoAsync("PostPrivateMessage");
        await this.Control("To").FillAsync(recipient);
        await this.Control("PmSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync($"The message {subject}.");
        await this.ClickAndWaitAsync(this.Control("Save"));

        return (recipient, subject);
    }

    /// <summary>
    /// Opens a message of the inbox, returns the id of the message of the user.
    /// </summary>
    private async Task<string> OpenInboxMessageAsync(string subject)
    {
        await this.GotoAsync("MyMessages");

        var href = await this.Page.Locator("a", new PageLocatorOptions { HasTextString = subject }).First
                       .GetAttributeAsync("href");

        await this.GotoAsync(href.TrimStart('/'));

        return new Uri(this.Page.Url).Query.Split('&')[0].Split('=')[1];
    }
}
