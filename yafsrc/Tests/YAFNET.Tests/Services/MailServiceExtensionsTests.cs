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

namespace YAF.Tests.Services;

using System.Threading.Tasks;

using MimeKit;

using YAF.Core.Extensions;
using YAF.Tests.TestInfrastructure;

/// <summary>
/// The <see cref="IMailServiceExtensions"/> overloads that build the mail message.
/// </summary>
[TestFixture]
public class MailServiceExtensionsTests
{
    [Test]
    [Description("The address string overload sends a text mail from, to and with the sender")]
    public async Task Send_With_Addresses_Test()
    {
        var mail = new RecordingMailService();

        await mail.SendAsync("from@example.com", "to@example.com", "sender@example.com", "Subject", "Body text");

        var message = mail.Sent.Single();
        message.From.Mailboxes.Single().Address.Should().Be("from@example.com");
        message.To.Mailboxes.Single().Address.Should().Be("to@example.com");
        message.Sender.Address.Should().Be("sender@example.com");
        message.Subject.Should().Be("Subject");
        message.TextBody.Should().Contain("Body text");
        message.HtmlBody.Should().BeNull();
    }

    [Test]
    [Description("The overload with names sets the display names and sends text and html bodies")]
    public async Task Send_With_Names_And_Html_Test()
    {
        var mail = new RecordingMailService();

        await mail.SendAsync(
            "from@example.com",
            "Forum",
            "to@example.com",
            "Alice",
            "sender@example.com",
            "Sender",
            "Hi",
            "text",
            "<p>html</p>");

        var message = mail.Sent.Single();
        message.From.Mailboxes.Single().Name.Should().Be("Forum");
        message.To.Mailboxes.Single().Name.Should().Be("Alice");
        message.Sender.Name.Should().Be("Sender");
        message.TextBody.Should().Contain("text");
        message.HtmlBody.Should().Contain("<p>html</p>");
    }

    [Test]
    [Description("CreateMessage builds the message without sending it, missing addresses are rejected")]
    public async Task Create_Message_And_Validation_Test()
    {
        var mail = new RecordingMailService();
        var from = MailboxAddress.Parse("from@example.com");
        var to = MailboxAddress.Parse("to@example.com");

        var message = mail.CreateMessage(from, to, from, "Subject", "text", null);

        message.Subject.Should().Be("Subject");
        mail.Sent.Should().BeEmpty();

        await mail.SendAsync(from, to, from, "Plain", "text");
        mail.Sent.Single().Subject.Should().Be("Plain");

        await FluentActions.Awaiting(() => mail.SendAsync(null, "to@example.com", "s@example.com", "s", "b"))
            .Should().ThrowAsync<ArgumentNullException>();
        await FluentActions.Awaiting(() => mail.SendAsync("from@example.com", null, "s@example.com", "s", "b"))
            .Should().ThrowAsync<ArgumentNullException>();
        FluentActions.Invoking(() => mail.CreateMessage(from, null, from, "s", "b", null))
            .Should().Throw<ArgumentNullException>();
    }
}
