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
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Editing, moving, reporting and the admin lists of MessageRepositoryExtensions.
/// </summary>
[TestFixture]
[NonParallelizable]
public class MessageEditTests
{
    private readonly DateTime start = DateTime.UtcNow.AddHours(-5);

    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int forum;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.forum = this.db.AddForum("Forum");
        this.topic = this.db.AddTopic(this.forum, this.alice, "Original subject");

        this.board.PageUser.ID = this.alice;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The first edit saves the original post in the history, later edits add the previous version")]
    public void UpdateAsync_History_Test()
    {
        var message = this.AddPost(this.alice, "version 1", 0);

        this.Edit(message, "version 2", "typo");
        this.Edit(message, "version 3", "more");

        var saved = this.Messages.GetById(message);
        saved.MessageText.Should().Be("version 3");
        saved.EditReason.Should().Be("more");
        saved.EditedBy.Should().Be(this.bob);
        saved.Edited.Should().NotBeNull();

        var history = (this.Repository<MessageHistory>().Get(h => h.MessageID == message)).OrderBy(h => h.Edited).ToList();

        history.Select(h => h.Message).Should().Equal("version 1", "version 2");
        history[0].EditedBy.Should().Be(this.alice, "the first entry is the original post");
        history[0].Edited.Should().BeCloseTo(this.start, TimeSpan.FromSeconds(1));
    }

    [Test]
    [Description("Edits in the same second get versions one second apart (the time is part of the key of a version, MySQL saves whole seconds)")]
    public void UpdateAsync_History_Edits_In_Same_Second_Test()
    {
        var message = this.AddPost(this.alice, "version 1", 0);

        this.Edit(message, "version 2", "first");
        this.Edit(message, "version 3", "second");
        this.Edit(message, "version 4", "third");

        var history = (this.Repository<MessageHistory>().Get(h => h.MessageID == message)).OrderBy(h => h.Edited).ToList();

        history.Select(h => h.Message).Should().Equal("version 1", "version 2", "version 3");
        (history[2].Edited - history[1].Edited).Should().BeGreaterThanOrEqualTo(TimeSpan.FromSeconds(1));
    }

    [Test]
    [Description("Editing updates the topic subject, priority and the search index")]
    public void UpdateAsync_Topic_And_Search_Test()
    {
        var message = this.AddPost(this.alice, "text", 0);

        this.Edit(message, "new text", "reason", subject: "New subject", priority: 2);

        var savedTopic = this.Repository<Topic>().GetById(this.topic);
        savedTopic.TopicName.Should().Be("New subject");
        savedTopic.Priority.Should().Be(2);

        this.board.Search.Indexed[message].Message.Should().Be("new text");
        this.board.Search.Indexed[message].Topic.Should().Be("New subject");
    }

    [Test]
    [Description("Editing keeps the topic tags in the search index")]
    public void UpdateAsync_Search_Keeps_Tags_Test()
    {
        var message = this.AddPost(this.alice, "text", 0);
        this.Repository<TopicTag>().AddTagsToTopic("astronomy", this.topic);

        this.Edit(message, "new text", "reason");

        this.board.Search.Indexed[message].TopicTags.Should().Be("astronomy");
    }

    [Test]
    [Description("Editing an unapproved post approves it in an unmoderated forum")]
    public void UpdateAsync_Approves_In_Unmoderated_Forum_Test()
    {
        var message = this.AddPost(this.alice, "held back", 0, approved: false);

        this.Edit(message, "edited", "reason");

        (this.Messages.GetById(message)).MessageFlags.IsApproved.Should().BeTrue();
        this.board.Search.Indexed[message].Flags.Should().Be((this.Messages.GetById(message)).Flags);
    }

    [Test]
    [Description("In a moderated forum an edit only approves the post with override approval")]
    public void UpdateAsync_Moderated_Forum_Test()
    {
        this.Repository<Forum>().UpdateOnly(
            () => new Forum { Flags = new ForumFlags { IsModerated = true }.BitValue },
            f => f.ID == this.forum);

        var message = this.AddPost(this.alice, "held back", 0, approved: false);

        this.Edit(message, "edited", "reason");

        (this.Messages.GetById(message)).MessageFlags.IsApproved.Should().BeFalse();
        this.board.Events.OfType<UpdateTopicLastPostEvent>().Should().ContainSingle();

        this.Edit(message, "edited by a moderator", "reason", overrideApproval: true);

        (this.Messages.GetById(message)).MessageFlags.IsApproved.Should().BeTrue();
    }

    [Test]
    [Description("Moving a post to another topic renumbers both topics and fixes the post counts")]
    public void MoveAsync_Single_Post_Test()
    {
        var first = this.AddPost(this.alice, "first", 0);
        var moved = this.AddPost(this.bob, "moved", 10);
        var after = this.AddPost(this.alice, "after", 20);

        var target = this.db.AddTopic(this.forum, this.bob, "Target");
        var t1 = this.db.AddMessage(target, this.bob, "target 1", posted: this.start.AddMinutes(5));
        var t2 = this.db.AddMessage(target, this.bob, "target 2", posted: this.start.AddMinutes(15));

        this.Messages.Move(this.Repository<Topic>().GetById(this.topic), this.Messages.GetById(moved), target, false);

        var movedMessage = this.Messages.GetById(moved);
        movedMessage.TopicID.Should().Be(target);
        movedMessage.Position.Should().Be(1, "it sits between the target posts by date");
        movedMessage.ReplyTo.Should().Be(t1, "it replies to the first post of the new topic");

        (this.Messages.GetById(t2)).Position.Should().Be(2);
        (this.Messages.GetById(after)).Position.Should().Be(1, "the old topic closed the gap");
        (this.Messages.GetById(first)).Position.Should().Be(0);

        (this.Repository<Topic>().GetById(this.topic)).NumPosts.Should().Be(2);
        (this.Repository<Topic>().GetById(target)).NumPosts.Should().Be(3);

        this.board.Events.OfType<UpdateTopicLastPostEvent>().Should().HaveCount(2);
        this.board.Events.OfType<UpdateForumStatsEvent>().Should().NotBeEmpty();
    }

    [Test]
    [Description("The search index entry of a moved post gets the new topic and forum (the read access is checked with the forum)")]
    public void MoveAsync_Updates_Search_Index_Test()
    {
        this.AddPost(this.alice, "first", 0);
        var moved = this.AddPost(this.bob, "moved", 10);

        var otherForum = this.db.AddForum("Staff");
        var target = this.db.AddTopic(otherForum, this.bob, "Target");
        this.db.AddMessage(target, this.bob, "target", posted: this.start.AddMinutes(5));

        this.board.Search.Indexed[moved] = new SearchMessage { MessageId = moved, TopicId = this.topic, ForumId = this.forum };

        this.Messages.Move(this.Repository<Topic>().GetById(this.topic), this.Messages.GetById(moved), target, false);

        this.board.Search.Indexed[moved].TopicId.Should().Be(target);
        this.board.Search.Indexed[moved].ForumId.Should().Be(otherForum);
    }

    [Test]
    [Description("Moving with replies moves the whole reply tree, moving the last post removes the old topic")]
    public void MoveAsync_With_Replies_Test()
    {
        var root = this.AddPost(this.alice, "root", 0);
        var reply = this.AddPost(this.bob, "reply", 10, replyTo: root);
        var replyToReply = this.AddPost(this.alice, "reply to reply", 20, replyTo: reply);

        var target = this.db.AddTopic(this.forum, this.bob, "Target");
        this.db.AddMessage(target, this.bob, "target", posted: this.start.AddMinutes(-10));

        this.Messages.Move(this.Repository<Topic>().GetById(this.topic), this.Messages.GetById(root), target, true);

        new[] { root, reply, replyToReply }.Should().OnlyContain(id => this.Messages.GetById(id).TopicID == target);
        (this.Repository<Topic>().GetById(this.topic)).Should().BeNull("the empty topic is erased");
    }

    [Test]
    [Description("Create and move: the post becomes the first post of the new topic, its replies follow")]
    public void MoveAsync_To_New_Topic_Test()
    {
        var first = this.AddPost(this.alice, "first", 0);
        var moved = this.AddPost(this.bob, "moved", 10, replyTo: first);
        var reply = this.AddPost(this.alice, "reply", 20, replyTo: moved);

        var newTopic = (int)this.Repository<Topic>().CreateByMessage(moved, this.forum, "Split topic");

        this.Messages.Move(this.Repository<Topic>().GetById(this.topic), this.Messages.GetById(moved), newTopic, true);

        var movedMessage = this.Messages.GetById(moved);
        movedMessage.TopicID.Should().Be(newTopic);
        movedMessage.Position.Should().Be(0);
        movedMessage.ReplyTo.Should().BeNull("it is the first post of the new topic");

        var replyMessage = this.Messages.GetById(reply);
        replyMessage.TopicID.Should().Be(newTopic);
        replyMessage.Position.Should().Be(1);
        replyMessage.ReplyTo.Should().Be(moved);

        (this.Messages.GetById(first)).Position.Should().Be(0);
        (this.Repository<Topic>().GetById(this.topic)).NumPosts.Should().Be(1);
        (this.Repository<Topic>().GetById(newTopic)).NumPosts.Should().Be(2);
    }

    [Test]
    [Description("A post older than all posts of the target topic becomes its first post, the old first post replies to it")]
    public void MoveAsync_Older_Than_Target_Posts_Test()
    {
        this.AddPost(this.alice, "first", 0);
        var moved = this.AddPost(this.bob, "moved", 10);

        var target = this.db.AddTopic(this.forum, this.bob, "Target");
        var t1 = this.db.AddMessage(target, this.bob, "target 1", posted: this.start.AddMinutes(30));
        var t2 = this.db.AddMessage(target, this.bob, "target 2", posted: this.start.AddMinutes(40), replyTo: t1);

        this.Messages.Move(this.Repository<Topic>().GetById(this.topic), this.Messages.GetById(moved), target, false);

        var movedMessage = this.Messages.GetById(moved);
        movedMessage.Position.Should().Be(0);
        movedMessage.ReplyTo.Should().BeNull();

        var oldFirst = this.Messages.GetById(t1);
        oldFirst.Position.Should().Be(1);
        oldFirst.ReplyTo.Should().Be(moved);

        var second = this.Messages.GetById(t2);
        second.Position.Should().Be(2);
        second.ReplyTo.Should().Be(t1, "the other replies stay");

        (this.Repository<Topic>().GetById(target)).NumPosts.Should().Be(3);
    }

    [Test]
    [Description("An erased answer is no answer of the topic anymore (the link to the answer showed an error)")]
    public void Erase_Answer_Test()
    {
        this.AddPost(this.alice, "question", 0);
        var answer = this.AddPost(this.bob, "answer", 10);

        this.Repository<Topic>().SetAnswerMessage(this.topic, answer);

        this.Messages.Delete(this.forum, this.topic, this.Messages.GetById(answer), true, "spam", false, true);

        (this.Repository<Topic>().GetById(this.topic)).AnswerMessageId.Should().BeNull();
    }

    [Test]
    [Description("A moved answer is no answer of the old topic and no answer in the new topic")]
    public void MoveAsync_Answer_Test()
    {
        this.AddPost(this.alice, "question", 0);
        var answer = this.AddPost(this.bob, "answer", 10);

        this.Repository<Topic>().SetAnswerMessage(this.topic, answer);
        this.Messages.UpdateFlags(
            answer,
            new MessageFlags((this.Messages.GetById(answer)).Flags) { IsAnswer = true }.BitValue);

        var target = this.db.AddTopic(this.forum, this.bob, "Target");
        this.db.AddMessage(target, this.bob, "target", posted: this.start.AddMinutes(-10));

        this.Messages.Move(this.Repository<Topic>().GetById(this.topic), this.Messages.GetById(answer), target, false);

        (this.Repository<Topic>().GetById(this.topic)).AnswerMessageId.Should().BeNull();
        (this.Messages.GetById(answer)).MessageFlags.IsAnswer.Should().BeFalse();
    }

    [Test]
    [Description("Resolving a report clears the reported flag")]
    public void ReportResolveAsync_Test()
    {
        var message = this.AddPost(this.alice, "reported", 0);
        this.Messages.UpdateOnly(
            () => new Message { Flags = new MessageFlags { IsApproved = true, IsReported = true }.BitValue },
            m => m.ID == message);

        using (var connection = this.db.Open())
        {
            connection.Insert(new MessageReported { ID = message, Message = "reported" });
        }

        this.Messages.ReportResolve(message, this.bob);

        (this.Messages.GetById(message)).MessageFlags.IsReported.Should().BeFalse();
        (this.Messages.GetById(message)).MessageFlags.IsApproved.Should().BeTrue();

        var report = this.Repository<MessageReported>().GetSingle(r => r.ID == message);
        report.Resolved.Should().BeTrue();
        report.ResolvedBy.Should().Be(this.bob);
    }

    [Test]
    [Description("Unapproved lists the posts waiting for approval in a forum")]
    public void UnapprovedAsync_Test()
    {
        this.AddPost(this.alice, "approved", 0);
        var waiting = this.AddPost(this.bob, "waiting", 10, approved: false);
        this.AddPost(this.bob, "deleted", 20, flags: new MessageFlags { IsApproved = false, IsDeleted = true });

        var list = this.Messages.Unapproved(this.forum);

        list.Select(t => t.Item2.ID).Should().Equal(waiting);
        list.Single().Item3.Name.Should().Be("Bob");
    }

    [Test]
    [Description("The deleted message lists include only deleted posts and report the total")]
    public void Deleted_Messages_Test()
    {
        this.AddPost(this.alice, "kept", 0);
        var ids = Enumerable.Range(1, 5)
            .Select(i => this.AddPost(this.bob, $"deleted {i}", i, flags: new MessageFlags { IsApproved = true, IsDeleted = true }))
            .ToList();

        this.Messages.GetDeletedMessages(TestDatabase.BoardId).Select(t => t.Item3.ID).Should().BeEquivalentTo(ids);

        var page = this.Messages.GetDeletedMessagesPaged(TestDatabase.BoardId, 0, 2);

        page.Should().HaveCount(2);
        page.Should().OnlyContain(m => m.IsDeleted && m.TotalRows == 5, "the admin restore page uses TotalRows for its pager");
        this.Messages.GetDeletedMessagesPaged(TestDatabase.BoardId, 2, 2).Should().ContainSingle();
    }

    [Test]
    [Description("GetMessageWithAccess only returns messages the user may read")]
    public void GetMessageWithAccess_Test()
    {
        var message = this.AddPost(this.alice, "text", 0);
        this.board.GrantAccess(this.alice, this.forum);

        this.Messages.GetMessageWithAccess(message, this.alice).Item2.ID.Should().Be(message);
        this.Messages.GetMessageWithAccess(message, this.bob).Should().BeNull();
    }

    [Test]
    [Description("A user's posts: all of them, and the readable latest ones")]
    public void User_Messages_Test()
    {
        var hiddenForum = this.db.AddForum("Hidden");
        var hiddenTopic = this.db.AddTopic(hiddenForum, this.bob);

        var older = this.AddPost(this.bob, "older", 0);
        var newer = this.AddPost(this.bob, "newer", 10);
        this.db.AddMessage(hiddenTopic, this.bob, "hidden", posted: this.start.AddMinutes(20));
        this.AddPost(this.bob, "deleted", 30, flags: new MessageFlags { IsApproved = true, IsDeleted = true });

        this.board.GrantAccess(this.alice, this.forum);

        this.Messages.GetAllUserMessages(this.bob).Should().HaveCount(4).And.BeInDescendingOrder(m => m.Posted);

        this.Messages.GetAllUserMessagesWithAccess(TestDatabase.BoardId, this.bob, this.alice, 10)
            .Select(t => t.Item1.ID).Should().Equal(newer, older);

        this.Messages.GetAllUserMessagesWithAccess(TestDatabase.BoardId, this.bob, this.alice, 1)
            .Select(t => t.Item1.ID).Should().Equal(newer);
    }

    [Test]
    [Description("The search index source contains the visible posts of a forum")]
    public void GetAllSearchMessagesByForum_Test()
    {
        var visible = this.AddPost(this.alice, "searchable", 0);
        this.AddPost(this.alice, "unapproved", 10, approved: false);

        var link = this.db.AddTopic(this.forum, this.alice, configure: t => t.TopicMovedID = this.topic);
        this.db.AddMessage(link, this.alice, "link post");

        var items = this.Messages.GetAllSearchMessagesByForum(this.forum, []).ToList();

        items.Select(i => i.MessageId).Should().Equal(visible);
        items.Single().Topic.Should().Be("Original subject");
        items.Single().ForumName.Should().Be("Forum");
    }

    private IRepository<Message> Messages => this.board.Current.GetRepository<Message>();

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }

    private int AddPost(
        int userId,
        string text,
        int minutes,
        bool approved = true,
        int? replyTo = null,
        MessageFlags flags = null)
    {
        flags ??= new MessageFlags { IsHtml = true, IsBBCode = true, IsApproved = approved };

        return this.db.AddMessage(
            this.topic,
            userId,
            text,
            posted: this.start.AddMinutes(minutes),
            replyTo: replyTo,
            configure: m => m.Flags = flags.BitValue);
    }

    private void Edit(
        int messageId,
        string text,
        string reason,
        string subject = null,
        short? priority = null,
        bool overrideApproval = false)
    {
        var message = this.Messages.GetById(messageId);
        var topicEntity = this.Repository<Topic>().GetById(message.TopicID);

        this.Messages.Update(
            priority,
            text,
            null,
            null,
            null,
            subject,
            reason,
            false,
            overrideApproval,
            topicEntity,
            message,
            this.Repository<Forum>().GetById(topicEntity.ForumID),
            this.Repository<User>().GetById(message.UserID),
            this.bob);
    }
}
