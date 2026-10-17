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

namespace YAF.Tests.Data;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Private messages: sending, the conversation of a message and the read state of the recipients.
/// </summary>
[TestFixture]
[NonParallelizable]
public class PrivateMessageTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int carol;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.carol = this.db.AddUser("Carol");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Marking a message as read changes only the message of that recipient and keeps its other flags")]
    public void Mark_As_Read_Test()
    {
        // a message to all users: one message of every recipient
        this.board.LoginAs(this.alice);
        this.PMessages.SendMessage(this.alice, 0, "To all", "Hello", new MessageFlags { IsHtml = false }.BitValue, null);

        var bobsMessage = this.UserMessages.GetSingle(u => u.UserID == this.bob);
        var carolsMessage = this.UserMessages.GetSingle(u => u.UserID == this.carol);

        // carol deleted it
        this.UserMessages.UpdateOnly(
            () => new UserPMessage { Flags = new PMessageFlags { IsInOutbox = true, IsDeleted = true }.BitValue },
            u => u.ID == carolsMessage.ID);

        this.UserMessages.MarkAsRead(bobsMessage.ID, bobsMessage.PMessageFlags);

        var bobs = this.UserMessages.GetById(bobsMessage.ID).PMessageFlags;
        bobs.IsRead.Should().BeTrue();
        bobs.IsInOutbox.Should().BeTrue("the sender still has it in the outbox");
        bobs.IsArchived.Should().BeFalse();

        var carols = this.UserMessages.GetById(carolsMessage.ID).PMessageFlags;
        carols.IsRead.Should().BeFalse("carol did not read it");
        carols.IsDeleted.Should().BeTrue("carol's deleted message stays deleted");
    }

    [Test]
    [Description("The conversation of a message has the replies with the flags of the recipient")]
    public void Conversation_Test()
    {
        this.board.LoginAs(this.alice);
        this.PMessages.SendMessage(this.alice, this.bob, "Question", "How are you?", 6, null);
        var question = this.UserMessages.GetSingle(u => u.UserID == this.bob);

        this.board.LoginAs(this.bob);
        this.PMessages.SendMessage(this.bob, this.alice, "Re: Question", "Fine", 6, question.ID);

        this.board.LoginAs(this.alice);
        var answer = this.UserMessages.GetSingle(u => u.UserID == this.alice);

        var conversation = this.PMessages.List(answer.ID, true);

        conversation.Select(m => m.Subject).Should().Equal("Question", "Re: Question");
        conversation.Single(m => m.UserPMessageID == answer.ID).UserPMFlags.Should()
            .Be(new PMessageFlags { IsInOutbox = true }.BitValue);
    }

    [Test]
    [Description("The first message of a conversation shows the replies to it, also when the ids of the messages and of the messages of the users differ (e.g. after a message to all users)")]
    public void Conversation_Of_The_First_Message_Test()
    {
        // a message to all users: from now on the message ids and the ids of the user messages differ
        this.board.LoginAs(this.carol);
        this.PMessages.SendMessage(this.carol, 0, "To all", "Hello", 6, null);

        this.board.LoginAs(this.alice);
        this.PMessages.SendMessage(this.alice, this.bob, "Question", "How are you?", 6, null);
        var question = this.UserMessages.Get(u => u.UserID == this.bob).OrderBy(u => u.ID).Last();

        this.board.LoginAs(this.bob);
        this.PMessages.SendMessage(this.bob, this.alice, "Re: Question", "Fine", 6, question.ID);
        var answer = this.UserMessages.Get(u => u.UserID == this.alice).OrderBy(u => u.ID).Last();

        // alice answers the answer (a reply of a reply belongs to the first message)
        this.board.LoginAs(this.alice);
        this.PMessages.SendMessage(this.alice, this.bob, "Re: Re: Question", "Great", 6, this.PMessages.GetMessage(answer.ID).ReplyTo);

        this.board.LoginAs(this.bob);
        this.PMessages.List(question.ID, true).Select(m => m.Subject).Should().Equal("Question", "Re: Re: Question");
    }

    [Test]
    [Description("The inbox lists the received messages, the outbox the sent ones, a message the recipient deleted stays in the outbox")]
    public void Inbox_Outbox_Test()
    {
        this.board.LoginAs(this.alice);
        this.PMessages.SendMessage(this.alice, this.bob, "First", "1", 6, null);
        this.PMessages.SendMessage(this.alice, this.bob, "Second", "2", 6, null);

        var second = this.UserMessages.Get(u => u.UserID == this.bob).Single(u => u.ID == this.UserMessages.Get(x => x.UserID == this.bob).Max(x => x.ID));
        this.UserMessages.UpdateOnly(
            () => new UserPMessage { Flags = new PMessageFlags { IsInOutbox = true, IsDeleted = true }.BitValue },
            u => u.ID == second.ID);

        this.PMessages.List(this.bob, PmView.Inbox, "Created", true).Select(m => m.Subject).Should().Equal("First");
        this.PMessages.List(this.alice, PmView.Outbox, "Created", true).Select(m => m.Subject).Should().Equal("First", "Second");
        this.PMessages.List(this.alice, PmView.Inbox, "Created", true).Should().BeEmpty();

        var count = this.PMessages.UserMessageCount(this.bob);
        count.InboxCount.Should().Be(1);

        this.UserMessages.List(this.bob, PmView.Inbox).Select(m => m.ID).Should().NotContain(second.ID);
        this.UserMessages.List(this.alice, PmView.Outbox).Should().HaveCount(2);
        this.PMessages.UserMessageCount(this.alice).OutBoxCount.Should().Be(2);
    }

    [Test]
    [Description("The recipient deletes the message from the inbox: the sender still has it in the outbox")]
    public void Recipient_Deletes_Test()
    {
        this.board.LoginAs(this.alice);
        this.PMessages.SendMessage(this.alice, this.bob, "Hello", "1", 6, null);
        var message = this.UserMessages.GetSingle(u => u.UserID == this.bob);

        this.UserMessages.Delete(message.ID, false);

        this.PMessages.List(this.bob, PmView.Inbox, "Created", true).Should().BeEmpty();
        this.PMessages.List(this.alice, PmView.Outbox, "Created", true).Select(m => m.Subject).Should().Equal("Hello");
    }

    [Test]
    [Description("The sender deletes the message from the outbox: the recipient still has it in the inbox")]
    public void Sender_Deletes_Test()
    {
        this.board.LoginAs(this.alice);
        this.PMessages.SendMessage(this.alice, this.bob, "Hello", "1", 6, null);
        var message = this.UserMessages.GetSingle(u => u.UserID == this.bob);

        this.UserMessages.Delete(message.ID, true);

        this.PMessages.List(this.alice, PmView.Outbox, "Created", true).Should().BeEmpty();
        this.PMessages.List(this.bob, PmView.Inbox, "Created", true).Select(m => m.Subject).Should().Equal("Hello");
    }

    [Test]
    [Description("The message is removed when the sender and the recipient deleted it")]
    public void Sender_And_Recipient_Delete_Test()
    {
        this.board.LoginAs(this.alice);
        this.PMessages.SendMessage(this.alice, this.bob, "Hello", "1", 6, null);
        var message = this.UserMessages.GetSingle(u => u.UserID == this.bob);

        this.UserMessages.Delete(message.ID, true);
        this.UserMessages.Delete(message.ID, false).Should().Be(1);

        this.UserMessages.Count(u => u.ID == message.ID).Should().Be(0);
        this.PMessages.Count(p => p.ID == message.PMessageID).Should().Be(0);
    }

    [Test]
    [Description("A message to all users stays with the other recipients when one recipient and the sender deleted it")]
    public void Message_To_All_Delete_Test()
    {
        this.board.LoginAs(this.alice);
        this.PMessages.SendMessage(this.alice, 0, "To all", "Hello", 6, null);
        var bobs = this.UserMessages.GetSingle(u => u.UserID == this.bob);

        this.UserMessages.Delete(bobs.ID, true);
        this.UserMessages.Delete(bobs.ID, false);

        this.PMessages.List(this.carol, PmView.Inbox, "Created", true).Select(m => m.Subject).Should().Equal("To all");
        this.PMessages.Count(p => p.ID == bobs.PMessageID).Should().Be(1);
    }

    [TestCase(0, 30, true)]
    [TestCase(29, 30, true)]
    [TestCase(30, 30, false)]
    [TestCase(0, 0, false)]
    [Description("The mailbox receives messages until the limit is reached (the last message fits)")]
    public void Can_Receive_Message_Test(int messages, int allowed, bool expected)
    {
        new PMessageCount { NumberTotal = messages, Allowed = allowed }.CanReceiveMessage.Should().Be(expected);
    }

    private IRepository<PMessage> PMessages => this.board.Current.GetRepository<PMessage>();

    private IRepository<UserPMessage> UserMessages => this.board.Current.GetRepository<UserPMessage>();
}
