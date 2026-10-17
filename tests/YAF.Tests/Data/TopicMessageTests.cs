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

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Posting, deleting and restoring topics and messages through the real repository code.
/// </summary>
[TestFixture]
[NonParallelizable]
public class TopicMessageTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int forumId;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.forumId = this.db.AddForum();

        this.LoginAs(this.alice);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A new topic gets its first message at position 0 and all counters are updated")]
    public void SaveNewTopic_Test()
    {
        var startPoints = (this.Users.GetById(this.alice)).Points;

        var (topic, message) = this.PostTopic("Hello World", "first post");

        message.Position.Should().Be(0);
        message.UserDisplayName.Should().Be("Alice");

        var savedTopic = this.Topics.GetById(topic.ID);
        savedTopic.TopicName.Should().Be("Hello World");
        savedTopic.NumPosts.Should().Be(1);
        savedTopic.LastMessageID.Should().Be(message.ID);
        savedTopic.LastUserID.Should().Be(this.alice);

        var forum = this.Forums.GetById(this.forumId);
        forum.LastMessageID.Should().Be(message.ID);
        forum.LastTopicID.Should().Be(topic.ID);

        var user = this.Users.GetById(this.alice);
        user.NumPosts.Should().Be(1);
        user.Points.Should().Be(startPoints + 3);

        (this.Messages.GetById(message.ID)).MessageFlags.IsApproved.Should().BeTrue();
        this.board.Search.Indexed.Should().ContainKey(message.ID).WhoseValue.Topic.Should().Be("Hello World");
        this.board.Events.OfType<UpdateForumStatsEvent>().Should().NotBeEmpty();
    }

    [Test]
    [Description("Unapproved posts don't update the post counters until they are approved")]
    public void Unapproved_Post_Then_Approve_Test()
    {
        var (topic, message) = this.PostTopic("Moderated", "needs approval", approved: false);

        (this.Messages.GetById(message.ID)).MessageFlags.IsApproved.Should().BeFalse();
        (this.Users.GetById(this.alice)).NumPosts.Should().Be(0);
        (this.Topics.GetById(topic.ID)).LastMessageID.Should().BeNull();

        this.Messages.Approve(message.ID, this.forumId);

        (this.Messages.GetById(message.ID)).MessageFlags.IsApproved.Should().BeTrue();
        new MessageFlags(this.board.Search.Indexed[message.ID].Flags).IsApproved.Should()
            .BeTrue("the search index gets the approved flags, so the post can be found by everybody");
        (this.Users.GetById(this.alice)).NumPosts.Should().Be(1);
        (this.Topics.GetById(topic.ID)).LastMessageID.Should().Be(message.ID);
    }

    [Test]
    [Description("Replies get the next position and become the last post")]
    public void Reply_Test()
    {
        var (topic, first) = this.PostTopic("Topic", "first");

        this.LoginAs(this.bob);
        var reply = this.Reply(topic, "second", replyTo: first.ID);

        reply.Position.Should().Be(1);
        reply.ReplyTo.Should().Be(first.ID);

        var savedTopic = this.Topics.GetById(topic.ID);
        savedTopic.NumPosts.Should().Be(2);
        savedTopic.LastMessageID.Should().Be(reply.ID);
        savedTopic.LastUserID.Should().Be(this.bob);

        this.Messages.RepliedTopic(topic.ID, this.bob).Should().BeTrue();
        this.Messages.Replies(first.ID).Should().ContainSingle().Which.ID.Should().Be(reply.ID);
    }

    [Test]
    [Description("Guests don't earn points and keep the name they entered")]
    public void Guest_Post_Test()
    {
        var guest = this.db.AddUser("Guest");
        var startPoints = (this.Users.GetById(guest)).Points;
        this.LoginAs(guest);
        this.board.User.IsGuest = true;

        var (_, message) = this.PostTopic("From a guest", "hi", userName: "Visitor");

        message.UserDisplayName.Should().Be("Visitor");
        (this.Users.GetById(guest)).Points.Should().Be(startPoints);
    }

    [Test]
    [Description("Posts in a test forum don't count towards the user's post count")]
    public void Test_Forum_Post_Count_Test()
    {
        this.forumId = this.db.AddForum("Testing", configure: f => f.Flags = new ForumFlags { IsTest = true }.BitValue);

        this.PostTopic("Test", "doesn't count");

        (this.Users.GetById(this.alice)).NumPosts.Should().Be(0);
    }

    [Test]
    [Description("Soft deleting a message flags it and updates the counters")]
    public void Delete_Message_Test()
    {
        var (topic, first) = this.PostTopic("Topic", "first");
        var reply = this.Reply(topic, "second");

        this.Delete(topic, reply, erase: false);

        (this.Messages.GetById(reply.ID)).MessageFlags.IsDeleted.Should().BeTrue();
        (this.Messages.GetById(reply.ID)).DeleteReason.Should().Be("test");
        (this.Users.GetById(this.alice)).NumPosts.Should().Be(1);
        (this.Topics.GetById(topic.ID)).NumPosts.Should().Be(1);
        this.board.Search.DeletedMessageIds.Should().Equal(reply.ID);

        // the deleted post was the last one, the last post columns are cleared for the UpdateTopicLastPostEvent handler
        (this.Topics.GetById(topic.ID)).LastMessageID.Should().BeNull();
        this.board.Events.OfType<UpdateTopicLastPostEvent>().Should().ContainSingle();

        this.Messages.LastPosts(topic.ID).Select(p => p.Item1.ID).Should().Equal(first.ID);
    }

    [Test]
    [Description("Restoring a deleted message gives the author the post back")]
    public void Restore_Message_Test()
    {
        var (topic, _) = this.PostTopic("Topic", "first");
        var reply = this.Reply(topic, "second");

        this.Delete(topic, reply, erase: false);
        (this.Users.GetById(this.alice)).NumPosts.Should().Be(1);

        this.Messages.Restore(this.forumId, topic.ID, this.Messages.GetById(reply.ID));

        var restored = this.Messages.GetById(reply.ID);
        restored.MessageFlags.IsDeleted.Should().BeFalse();
        restored.DeleteReason.Should().BeNull();
        (this.Topics.GetById(topic.ID)).NumPosts.Should().Be(2);
        (this.Users.GetById(this.alice)).NumPosts.Should().Be(2);

        // the deleted post was removed from the search index, the restored post is found again
        new MessageFlags(this.board.Search.Indexed[reply.ID].Flags).IsDeleted.Should().BeFalse();
    }

    [Test]
    [Description("Erasing a reply removes the row and moves later posts up")]
    public void Erase_Message_Test()
    {
        var now = DateTime.UtcNow;
        var (topic, _) = this.PostTopic("Topic", "first", posted: now.AddMinutes(-3));
        var second = this.Reply(topic, "second", posted: now.AddMinutes(-2));
        var third = this.Reply(topic, "third", posted: now.AddMinutes(-1));

        this.Delete(topic, second, erase: true);

        (this.Messages.GetById(second.ID)).Should().BeNull();
        (this.Messages.GetById(third.ID)).Position.Should().Be(1);
        (this.Topics.GetById(topic.ID)).Should().NotBeNull();
    }

    [Test]
    [Description("Erasing the only message erases the topic too")]
    public void Erase_Last_Message_Deletes_Topic_Test()
    {
        var (topic, message) = this.PostTopic("Topic", "only post");

        this.Delete(topic, message, erase: true);

        (this.Topics.GetById(topic.ID)).Should().BeNull();
        (this.Messages.Count(m => m.TopicID == topic.ID)).Should().Be(0);
    }

    [Test]
    [Description("Deleting with deleteLinked also deletes the replies")]
    public void Delete_Linked_Replies_Test()
    {
        var (topic, first) = this.PostTopic("Topic", "first");
        var reply = this.Reply(topic, "reply", replyTo: first.ID);
        var replyToReply = this.Reply(topic, "reply to reply", replyTo: reply.ID);
        var unrelated = this.Reply(topic, "unrelated");

        this.Messages.Delete(this.forumId, topic.ID, this.Messages.GetById(reply.ID), false, "test", true, false);

        (this.Messages.GetById(reply.ID)).MessageFlags.IsDeleted.Should().BeTrue();
        (this.Messages.GetById(replyToReply.ID)).MessageFlags.IsDeleted.Should().BeTrue();
        (this.Messages.GetById(unrelated.ID)).MessageFlags.IsDeleted.Should().BeFalse();
        (this.Messages.GetById(first.ID)).MessageFlags.IsDeleted.Should().BeFalse();
    }

    [Test]
    [Description("Without deleteLinked only the post itself is deleted, the replies (quotes) of others stay")]
    public void Delete_Without_Linked_Replies_Test()
    {
        var (topic, _) = this.PostTopic("Topic", "first");
        var post = this.Reply(topic, "own post");
        var quote = this.Reply(topic, "quote of the post", replyTo: post.ID);

        this.Messages.Delete(this.forumId, topic.ID, this.Messages.GetById(post.ID), false, "test", false, false);

        (this.Messages.GetById(post.ID)).MessageFlags.IsDeleted.Should().BeTrue();
        (this.Messages.GetById(quote.ID)).MessageFlags.IsDeleted.Should().BeFalse();
    }

    [Test]
    [Description("Restoring restores the deleted replies too, unless only the post itself is restored")]
    public void Restore_Linked_Replies_Test()
    {
        var (topic, _) = this.PostTopic("Topic", "first");
        var post = this.Reply(topic, "own post");
        var quote = this.Reply(topic, "spam quote", replyTo: post.ID);

        // a moderator deleted the quote, the author later deletes the post
        this.Messages.Delete(this.forumId, topic.ID, this.Messages.GetById(quote.ID), true, "spam", false, false);
        this.Messages.Delete(this.forumId, topic.ID, this.Messages.GetById(post.ID), false, "test", false, false);

        this.Messages.Restore(this.forumId, topic.ID, this.Messages.GetById(post.ID), false);

        (this.Messages.GetById(post.ID)).MessageFlags.IsDeleted.Should().BeFalse();
        (this.Messages.GetById(quote.ID)).MessageFlags.IsDeleted.Should().BeTrue("the moderator's delete stays");

        this.Messages.Restore(this.forumId, topic.ID, this.Messages.GetById(post.ID));
        (this.Messages.GetById(quote.ID)).MessageFlags.IsDeleted.Should().BeFalse("by default the replies are restored too");
    }

    [Test]
    [Description("The post count of the topic only changes when a post is really deleted or restored")]
    public void Topic_Post_Count_Delete_Restore_Test()
    {
        var (topic, _) = this.PostTopic("Topic", "first");
        var post = this.Reply(topic, "own post");
        var quote = this.Reply(topic, "quote", replyTo: post.ID);

        (this.Topics.GetById(topic.ID)).NumPosts.Should().Be(3);

        // the delete form sent twice
        this.Messages.Delete(this.forumId, topic.ID, this.Messages.GetById(quote.ID), true, "spam", false, false);
        this.Messages.Delete(this.forumId, topic.ID, this.Messages.GetById(quote.ID), true, "spam", false, false);

        (this.Topics.GetById(topic.ID)).NumPosts.Should().Be(2);

        // the post with its replies, the deleted quote is not counted again
        this.Messages.Delete(this.forumId, topic.ID, this.Messages.GetById(post.ID), true, "test", true, false);

        (this.Topics.GetById(topic.ID)).NumPosts.Should().Be(1);

        // the post with its replies, then again (nothing is deleted anymore)
        this.Messages.Restore(this.forumId, topic.ID, this.Messages.GetById(post.ID));
        this.Messages.Restore(this.forumId, topic.ID, this.Messages.GetById(post.ID));

        (this.Topics.GetById(topic.ID)).NumPosts.Should().Be(3);

        // a deleted post erased later (Admin > Restore)
        this.Messages.Delete(this.forumId, topic.ID, this.Messages.GetById(quote.ID), true, "spam", false, false);
        this.Messages.Delete(this.forumId, topic.ID, this.Messages.GetById(quote.ID), true, string.Empty, true, true);

        (this.Messages.GetById(quote.ID)).Should().BeNull();
        (this.Topics.GetById(topic.ID)).NumPosts.Should().Be(2);
    }

    [Test]
    [Description("A deleted topic restored with its first post (Admin > Restore) has the post count of before")]
    public void Restore_Topic_Post_Count_Test()
    {
        var (topic, first) = this.PostTopic("Topic", "first");
        this.Reply(topic, "second", replyTo: first.ID);
        this.Reply(topic, "third", replyTo: first.ID);

        (this.Topics.GetById(topic.ID)).NumPosts.Should().Be(3);

        this.Topics.Delete(this.forumId, topic.ID, false);

        this.Messages.Restore(this.forumId, topic.ID, this.Messages.GetById(first.ID));

        (this.Topics.GetById(topic.ID)).NumPosts.Should().Be(3);
    }

    [Test]
    [Description("Restoring a post counts the posts of the author like deleting, without the posts of test forums")]
    public void Restore_User_Post_Count_Without_Test_Forums_Test()
    {
        var normalForum = this.forumId;

        this.forumId = this.db.AddForum("Testing", configure: f => f.Flags = new ForumFlags { IsTest = true }.BitValue);

        this.PostTopic("Test", "doesn't count");

        this.forumId = normalForum;

        var (topic, _) = this.PostTopic("Topic", "first");
        var reply = this.Reply(topic, "second");

        this.Delete(topic, reply, erase: false);

        (this.Users.GetById(this.alice)).NumPosts.Should().Be(1);

        this.Messages.Restore(this.forumId, topic.ID, this.Messages.GetById(reply.ID));

        (this.Users.GetById(this.alice)).NumPosts.Should().Be(2);
    }

    [Test]
    [Description("Soft deleting a topic flags the topic and all of its messages")]
    public void Delete_Topic_Test()
    {
        var (topic, _) = this.PostTopic("Topic", "first");
        this.Reply(topic, "second");

        this.Topics.Delete(this.forumId, topic.ID);

        (this.Topics.GetById(topic.ID)).TopicFlags.IsDeleted.Should().BeTrue();
        (this.Messages.Get(m => m.TopicID == topic.ID)).Should().HaveCount(2)
            .And.OnlyContain(m => m.MessageFlags.IsDeleted);
        this.board.Search.DeletedTopicIds.Should().Equal(topic.ID);
        (this.Forums.GetById(this.forumId)).LastTopicID.Should().BeNull();
    }

    [Test]
    [Description("Erasing a topic removes the topic and its messages")]
    public void Erase_Topic_Test()
    {
        var (topic, _) = this.PostTopic("Topic", "first");
        this.Reply(topic, "second");
        var otherTopic = this.db.AddTopic(this.forumId, this.bob);
        this.db.AddMessage(otherTopic, this.bob);

        this.Topics.Delete(this.forumId, topic.ID, true);

        (this.Topics.GetById(topic.ID)).Should().BeNull();
        (this.Messages.Count(m => m.TopicID == topic.ID)).Should().Be(0);
        (this.Messages.Count(m => m.TopicID == otherTopic)).Should().Be(1);
    }

    [Test]
    [Description("A deleted topic doesn't count as an existing subject")]
    public void CheckForDuplicate_Deleted_Topic_Test()
    {
        var (topic, _) = this.PostTopic("Deleted subject", "first");

        this.Topics.CheckForDuplicate("Deleted subject").Should().BeTrue();

        this.Topics.Delete(this.forumId, topic.ID, false);

        this.Topics.CheckForDuplicate("Deleted subject").Should().BeFalse("the topic is deleted");
    }

    [Test]
    [Description("An edited topic doesn't count as an existing subject for itself, another topic does")]
    public void CheckForDuplicate_Edited_Topic_Test()
    {
        var (topic, _) = this.PostTopic("Edited subject", "first");

        this.Topics.CheckForDuplicate("edited subject", topic.ID).Should().BeFalse("it is the edited topic itself");

        var (other, _) = this.PostTopic("Other subject", "second");

        this.Topics.CheckForDuplicate("Edited subject", other.ID).Should().BeTrue("another topic has the subject");
    }

    [Test]
    [Description("Topic helpers: lock, answer, duplicates and lookups")]
    public void Topic_Helpers_Test()
    {
        var (topic, message) = this.PostTopic("How do I install YAF?", "question");

        this.Topics.Lock(topic.ID, new TopicFlags { IsLocked = true }.BitValue);
        (this.Topics.GetById(topic.ID)).TopicFlags.IsLocked.Should().BeTrue();

        this.Topics.SetAnswerMessage(topic.ID, message.ID);
        this.Topics.GetAnswerMessage(topic.ID).Should().Be(message.ID);
        this.Topics.RemoveAnswerMessage(topic.ID);
        this.Topics.GetAnswerMessage(topic.ID).Should().BeNull();
        this.Topics.GetAnswerMessage(-1).Should().BeNull();

        // the same subject (ignoring the case), not a part of it
        this.Topics.CheckForDuplicate("how do i install yaf? ").Should().BeTrue();
        this.Topics.CheckForDuplicate("install YAF").Should().BeFalse();
        this.Topics.CheckForDuplicate("something else").Should().BeFalse();

        (this.Topics.GetTopicFromMessage(message.ID)).ID.Should().Be(topic.ID);
        this.Topics.GetById(topic.ID).TopicName.Should().Be("How do I install YAF?");

        var tuple = this.Messages.GetMessageAsTuple(message.ID);
        tuple.Item1.ID.Should().Be(topic.ID);
        tuple.Item3.Name.Should().Be("Alice");
        tuple.Item4.ID.Should().Be(this.forumId);
    }

    [Test]
    [Description("FindNext and FindPrevious navigate by last post date and skip deleted topics")]
    public void Topic_Navigation_Test()
    {
        var now = DateTime.UtcNow;
        var oldest = this.AddTopicWithPost("Oldest", now.AddHours(-3));
        var deleted = this.AddTopicWithPost("Deleted", now.AddHours(-2));
        var middle = this.AddTopicWithPost("Middle", now.AddHours(-1));
        var newest = this.AddTopicWithPost("Newest", now);

        this.Topics.Lock(deleted, new TopicFlags { IsDeleted = true }.BitValue);

        var middleTopic = this.Topics.GetById(middle);

        this.Topics.FindNext(middleTopic).ID.Should().Be(newest);
        this.Topics.FindPrevious(middleTopic).ID.Should().Be(oldest);
        this.Topics.FindNext(this.Topics.GetById(newest)).Should().BeNull();
    }

    [Test]
    [Description("UpdateLastPostAsync picks the newest approved, not deleted message")]
    public void UpdateLastPost_Test()
    {
        var now = DateTime.UtcNow;
        var topic = this.db.AddTopic(this.forumId, this.alice);
        var older = this.db.AddMessage(topic, this.alice, posted: now.AddMinutes(-2));
        var newer = this.db.AddMessage(
            topic,
            this.bob,
            posted: now.AddMinutes(-1),
            configure: m => m.Flags = new MessageFlags { IsApproved = true, IsDeleted = true }.BitValue);

        (this.Topics.GetById(topic)).LastMessageID.Should().Be(newer);

        this.Topics.UpdateLastPost(topic);

        (this.Topics.GetById(topic)).LastMessageID.Should().Be(older);
    }

    [Test]
    [Description("GetUserLastPosted returns the date of the user's newest post")]
    public void GetUserLastPosted_Test()
    {
        var now = DateTime.UtcNow;
        var topic = this.db.AddTopic(this.forumId, this.alice);
        this.db.AddMessage(topic, this.alice, posted: now.AddDays(-2));
        this.db.AddMessage(topic, this.alice, posted: now.AddDays(-1));

        this.Messages.GetUserLastPosted(this.alice).Should().BeCloseTo(now.AddDays(-1), TimeSpan.FromSeconds(1));
        this.Messages.GetUserLastPosted(this.bob).Should().BeNull();
    }

    [Test]
    [Description("GetUserLastPosted with an address only returns the posts of that address (all guests are the same user)")]
    public void GetUserLastPosted_Of_Address_Test()
    {
        var now = DateTime.UtcNow;
        var topic = this.db.AddTopic(this.forumId, this.alice);
        this.db.AddMessage(topic, this.alice, posted: now.AddDays(-2), configure: m => m.IP = "10.0.0.1");
        this.db.AddMessage(topic, this.alice, posted: now.AddDays(-1), configure: m => m.IP = "10.0.0.2");

        this.Messages.GetUserLastPosted(this.alice, "10.0.0.1").Should().BeCloseTo(now.AddDays(-2), TimeSpan.FromSeconds(1));
        this.Messages.GetUserLastPosted(this.alice, "10.0.0.3").Should().BeNull();
        this.Messages.GetUserLastPosted(this.alice).Should().BeCloseTo(now.AddDays(-1), TimeSpan.FromSeconds(1));
    }

    private IRepository<Topic> Topics => this.board.Current.GetRepository<Topic>();

    private IRepository<Message> Messages => this.board.Current.GetRepository<Message>();

    private IRepository<Forum> Forums => this.board.Current.GetRepository<Forum>();

    private IRepository<User> Users => this.board.Current.GetRepository<User>();

    private void LoginAs(int userId)
    {
        var user = this.Users.GetById(userId);

        this.board.PageUser.ID = user.ID;
        this.board.PageUser.Name = user.Name;
        this.board.PageUser.DisplayName = user.DisplayName;
        this.board.User.IsGuest = false;
    }

    private Tuple<Topic, Message> PostTopic(
        string subject,
        string text,
        bool approved = true,
        string userName = null,
        DateTime? posted = null)
    {
        var user = this.Users.GetById(this.board.PageUser.ID);

        var topic = this.Topics.SaveNew(
            this.Forums.GetById(this.forumId),
            subject,
            string.Empty,
            string.Empty,
            string.Empty,
            text,
            user,
            0,
            userName ?? user.Name,
            user.DisplayName,
            "127.0.0.1",
            posted ?? DateTime.UtcNow,
            new MessageFlags { IsHtml = true, IsBBCode = true, IsApproved = approved },
            out var message);

        return Tuple.Create(topic, message);
    }

    private Message Reply(Topic topic, string text, int? replyTo = null, DateTime? posted = null)
    {
        var user = this.Users.GetById(this.board.PageUser.ID);

        return this.Messages.SaveNew(
            this.Forums.GetById(this.forumId),
            topic,
            user,
            text,
            user.Name,
            "127.0.0.1",
            posted ?? DateTime.UtcNow,
            replyTo,
            new MessageFlags { IsHtml = true, IsBBCode = true, IsApproved = true });
    }

    private void Delete(Topic topic, Message message, bool erase)
    {
        this.Messages.Delete(
            this.forumId,
            topic.ID,
            this.Messages.GetById(message.ID),
            false,
            "test",
            false,
            erase);
    }

    private int AddTopicWithPost(string subject, DateTime posted)
    {
        var topic = this.db.AddTopic(this.forumId, this.alice, subject);
        this.db.AddMessage(topic, this.alice, posted: posted);
        return topic;
    }
}
