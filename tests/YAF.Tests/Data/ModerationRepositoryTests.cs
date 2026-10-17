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

using System.Threading.Tasks;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Reported posts, message history and event log repositories.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ModerationRepositoryTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int guest;

    private int alice;

    private int bob;

    private int forum;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.guest = this.db.AddUser("Guest");
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");

        this.board.PageLoad.GuestUserID = this.guest;

        this.forum = this.db.AddForum();
        this.topic = this.db.AddTopic(this.forum, this.alice);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Reporting a post flags it, stores the original text and one audit row per reporter")]
    public void Report_Creates_Report_And_Audit_Test()
    {
        var messageId = this.db.AddMessage(this.topic, this.alice, "spam spam");

        this.Reported.Report(this.Reportable(messageId), this.bob, DateTime.UtcNow, "is spam");

        this.Message(messageId).MessageFlags.IsReported.Should().BeTrue();
        (this.Reported.GetById(messageId)).Message.Should().Be("spam spam");

        var audit = (this.Audits.Get(a => a.MessageID == messageId)).Should().ContainSingle().Subject;
        audit.UserID.Should().Be(this.bob);
        audit.ReportedNumber.Should().Be(1);
        audit.ReportText.Should().EndWith("??is spam");
    }

    [Test]
    [Description("The same user reporting again increments the counter instead of adding an audit row")]
    public void Report_Twice_By_Same_User_Increments_Test()
    {
        var messageId = this.db.AddMessage(this.topic, this.alice);

        this.Reported.Report(this.Reportable(messageId), this.bob, DateTime.UtcNow, "first").Should()
            .BeTrue("the moderators are notified about the first report of the user");
        this.Reported.Report(this.Reportable(messageId), this.bob, DateTime.UtcNow, null).Should()
            .BeFalse("a further report of the user is only counted");

        (this.Reported.Count(r => r.ID == messageId)).Should().Be(1, "the report row is shared by all reporters");

        var audit = (this.Audits.Get(a => a.MessageID == messageId)).Should().ContainSingle().Subject;
        audit.ReportedNumber.Should().Be(2);
        audit.ReportText.Should().Contain("??first|").And.EndWith("??", "a null report text is stored as empty");
    }

    [Test]
    [Description("Different reporters each get their own audit row and the list counts them")]
    public void ListReported_Counts_Reporters_Test()
    {
        var messageId = this.db.AddMessage(this.topic, this.alice, "bad");

        this.Reported.Report(this.Reportable(messageId), this.bob, DateTime.UtcNow, "a");
        this.Reported.Report(this.Reportable(messageId), this.guest, DateTime.UtcNow, "b");

        var reported = (this.Reported.ListReported(this.forum)).Should().ContainSingle().Subject;

        reported.MessageID.Should().Be(messageId);
        reported.NumberOfReports.Should().Be(2);
        reported.OriginalMessage.Should().Be("bad");
        reported.TopicID.Should().Be(this.topic);
    }

    [Test]
    [Description("Deleted posts, posts in deleted topics, other forums and unflagged posts are not listed")]
    public void ListReported_Excludes_Test()
    {
        var deletedPost = this.db.AddMessage(this.topic, this.alice);
        this.Reported.Report(this.Reportable(deletedPost), this.bob, DateTime.UtcNow, "x");
        this.SetMessageFlag(deletedPost, f => f.IsDeleted = true);

        var deletedTopic = this.db.AddTopic(this.forum, this.alice, configure: t => t.Flags = 8);
        var inDeletedTopic = this.db.AddMessage(deletedTopic, this.alice);
        this.Reported.Report(this.Reportable(inDeletedTopic), this.bob, DateTime.UtcNow, "x");

        var otherTopic = this.db.AddTopic(this.db.AddForum(), this.alice);
        var otherForum = this.db.AddMessage(otherTopic, this.alice);
        this.Reported.Report(this.Reportable(otherForum), this.bob, DateTime.UtcNow, "x");

        var resolved = this.db.AddMessage(this.topic, this.alice);
        this.Reported.Report(this.Reportable(resolved), this.bob, DateTime.UtcNow, "x");
        this.SetMessageFlag(resolved, f => f.IsReported = false);

        (this.Reported.ListReported(this.forum)).Should().BeEmpty();
    }

    [Test]
    [Description("CopyOver replaces the stored original text with the current message text")]
    public void ReportCopyOver_Test()
    {
        var messageId = this.db.AddMessage(this.topic, this.alice, "before");
        this.Reported.Report(this.Reportable(messageId), this.bob, DateTime.UtcNow, "x");

        var message = this.Message(messageId);
        message.MessageText = "after";

        this.Reported.ReportCopyOver(message);

        (this.Reported.GetById(messageId)).Message.Should().Be("after");
    }

    [Test]
    [Description("History lists old revisions oldest first, then the current message, and prunes old revisions")]
    public void MessageHistory_List_Test()
    {
        var edited = DateTime.UtcNow;
        var messageId = this.db.AddMessage(
            this.topic,
            this.alice,
            "v3",
            configure: m =>
            {
                m.Edited = edited;
                m.EditReason = "typo";
            });

        this.AddHistory(messageId, "v2", edited.AddDays(-1));
        this.AddHistory(messageId, "v1", edited.AddDays(-2));
        this.AddHistory(messageId, "ancient", edited.AddDays(-30));

        var list = this.Repository<MessageHistory>().List(messageId, 10);

        list.Select(h => h.Message).Should().Equal("v1", "v2", "v3");
        list[^1].EditReason.Should().Be("typo");
        list[^1].TopicID.Should().Be(this.topic);
        list[0].Name.Should().Be("Bob");

        this.Repository<MessageHistory>().Count(h => h.MessageID == messageId).Should().Be(2, "revisions older than daysToClean are deleted");
    }

    [Test]
    [Description("A message that was never edited has no history, only the current message")]
    public void MessageHistory_List_Unedited_Message_Test()
    {
        var messageId = this.db.AddMessage(this.topic, this.alice, "original");

        var list = this.Repository<MessageHistory>().List(messageId, 10);

        var current = list.Should().ContainSingle().Subject;
        current.Message.Should().Be("original");
        current.Edited.Should().Be(current.Posted, "the page shows an unedited post as the original message");
    }

    [Test]
    [Description("History of a message that does not exist is empty")]
    public void MessageHistory_List_Missing_Message_Test()
    {
        this.Repository<MessageHistory>().List(int.MaxValue, 10).Should().BeEmpty();
    }

    [Test]
    [Description("The event log is filtered by date range and type, newest first, with the total row count")]
    public void EventLog_ListPaged_Filters_Test()
    {
        var now = DateTime.UtcNow;

        this.AddEvent(now.AddDays(-1), EventLogTypes.Error, this.alice);
        this.AddEvent(now.AddHours(-1), EventLogTypes.Warning, this.bob);
        this.AddEvent(now.AddMinutes(-1), EventLogTypes.Error, null);
        this.AddEvent(now.AddDays(-10), EventLogTypes.Error, this.alice);

        var all = this.EventLog.ListPaged(TestDatabase.BoardId, 1050, 365, 0, 10, now.AddDays(-2), now, null);
        all.Select(e => e.Type).Should().Equal((int)EventLogTypes.Error, (int)EventLogTypes.Warning, (int)EventLogTypes.Error);
        all.Should().OnlyContain(e => e.TotalRows == 3);
        all[0].Name.Should().Be("Guest", "events without a user are shown as the guest user");

        var errors = this.EventLog.ListPaged(TestDatabase.BoardId, 1050, 365, 0, 10, now.AddDays(-2), now, (int)EventLogTypes.Error);
        errors.Should().HaveCount(2).And.OnlyContain(e => e.TotalRows == 2);

        var page2 = this.EventLog.ListPaged(TestDatabase.BoardId, 1050, 365, 1, 2, now.AddDays(-2), now, null);
        page2.Should().ContainSingle().Which.Type.Should().Be((int)EventLogTypes.Error);

        this.EventLog.ListPaged(TestDatabase.BoardId + 1, 1050, 365, 0, 10, now.AddDays(-2), now, null)
            .Should().BeEmpty("events of users on another board are not listed");
    }

    [Test]
    [Description("spamOnly keeps only the spam event types")]
    public void EventLog_ListPaged_SpamOnly_Test()
    {
        var now = DateTime.UtcNow;

        this.AddEvent(now.AddMinutes(-3), EventLogTypes.Error, this.alice);
        this.AddEvent(now.AddMinutes(-2), EventLogTypes.SpamBotDetected, this.alice);
        this.AddEvent(now.AddMinutes(-1), EventLogTypes.SpamMessageDetected, this.alice);

        this.EventLog.ListPaged(TestDatabase.BoardId, 1050, 365, 0, 10, now.AddDays(-1), now, null, spamOnly: true)
            .Select(e => e.Type)
            .Should().BeEquivalentTo([(int)EventLogTypes.SpamMessageDetected, (int)EventLogTypes.SpamBotDetected]);
    }

    [Test]
    [Description("The delete of the event log and the spam log only deletes the entries of the board (and the spam log only spam entries)")]
    public void EventLog_DeleteBoardEntries_Test()
    {
        var now = DateTime.UtcNow;
        var otherBoardUser = this.db.AddUser("Other", u => u.BoardID = TestDatabase.BoardId + 1);

        this.AddEvent(now, EventLogTypes.Error, this.alice);
        this.AddEvent(now, EventLogTypes.SpamBotDetected, this.alice);
        this.AddEvent(now, EventLogTypes.Error, null);
        this.AddEvent(now, EventLogTypes.Error, otherBoardUser);
        this.AddEvent(now, EventLogTypes.SpamBotDetected, otherBoardUser);

        var otherBoardEntry = (this.EventLog.GetSingle(e => e.UserID == otherBoardUser && e.Type == (int)EventLogTypes.Error)).ID;

        // a single entry of another board is not deleted
        (this.EventLog.DeleteBoardEntries(TestDatabase.BoardId, otherBoardEntry)).Should().Be(0);

        // the spam log deletes only spam entries of the board
        (this.EventLog.DeleteBoardEntries(TestDatabase.BoardId, spamOnly: true)).Should().Be(1);
        (this.EventLog.Count(e => e.Type == (int)EventLogTypes.Error)).Should().Be(3);

        // the event log deletes the entries of the board and the ones without a user
        (this.EventLog.DeleteBoardEntries(TestDatabase.BoardId)).Should().Be(2);

        (this.EventLog.Get(_ => true)).Select(e => e.UserID).Should().OnlyContain(id => id == otherBoardUser).And.HaveCount(2);
    }

    [Test]
    [Description("DeleteOld removes entries older than maxDays and keeps newer ones")]
    public void EventLog_DeleteOld_By_Age_Test()
    {
        this.AddEvent(DateTime.Today.AddDays(-40), EventLogTypes.Error, this.alice);
        this.AddEvent(DateTime.Today.AddDays(-5), EventLogTypes.Error, this.alice);

        this.EventLog.DeleteOld(1050, 30);

        this.EventLog.Count(_ => true).Should().Be(1);
    }

    [Test]
    [Description("DeleteOld leaves the log alone while it is below maxRows")]
    public void EventLog_DeleteOld_Below_MaxRows_Test()
    {
        this.AddEvents(20);

        this.EventLog.DeleteOld(50, 365);

        this.EventLog.Count(_ => true).Should().Be(20);
    }

    [Test]
    [Description("DeleteOld trims the oldest entries once the log grows past maxRows (default board setting 1050)")]
    public void EventLog_DeleteOld_Above_MaxRows_Test()
    {
        this.AddEvents(1200);

        this.EventLog.DeleteOld(1050, 365);

        this.EventLog.Count(_ => true).Should().Be(1050, "the log is above maxRows + 50 and is trimmed back to maxRows");
        this.EventLog.Count(e => e.Description == "event 149").Should().Be(0, "the oldest entries are deleted");
        this.EventLog.Count(e => e.Description == "event 150").Should().Be(1);
    }

    private IRepository<MessageReported> Reported => this.Repository<MessageReported>();

    private IRepository<MessageReportedAudit> Audits => this.Repository<MessageReportedAudit>();

    private IRepository<EventLog> EventLog => this.Repository<EventLog>();

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }

    private Message Message(int id)
    {
        return this.Repository<Message>().GetById(id);
    }

    private void SetMessageFlag(int messageId, Action<MessageFlags> change)
    {
        var flags = this.Message(messageId).MessageFlags;
        change(flags);
        this.Repository<Message>().UpdateOnly(() => new Message { Flags = flags.BitValue }, m => m.ID == messageId);
    }

    private void AddHistory(int messageId, string text, DateTime edited)
    {
        using var connection = this.db.Open();

        connection.Insert(
            new MessageHistory
            {
                MessageID = messageId,
                Message = text,
                IP = "127.0.0.1",
                Edited = edited,
                EditedBy = this.bob,
                Flags = 0
            });
    }

    private void AddEvent(DateTime time, EventLogTypes type, int? userId)
    {
        using var connection = this.db.Open();

        connection.Insert(
            new EventLog { EventTime = time, Type = (int)type, UserID = userId, Source = "test", Description = "d" });
    }

    private void AddEvents(int count)
    {
        using var connection = this.db.Open();

        var start = DateTime.UtcNow.AddHours(-count);

        connection.InsertAll(
            Enumerable.Range(0, count).Select(
                i => new EventLog
                {
                    EventTime = start.AddHours(i),
                    Type = (int)EventLogTypes.Information,
                    UserID = this.alice,
                    Source = "test",
                    Description = $"event {i}"
                }));
    }

    /// <summary>
    /// The message in the form the report takes it (topic, message, user and forum, only the message is used).
    /// </summary>
    private Tuple<Topic, Message, User, Forum> Reportable(int messageId)
    {
        return Tuple.Create<Topic, Message, User, Forum>(null, this.Message(messageId), null, null);
    }
}
