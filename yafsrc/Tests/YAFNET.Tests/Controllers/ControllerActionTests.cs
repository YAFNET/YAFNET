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

namespace YAF.Tests.Controllers;

using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Controllers;
using YAF.Core.Controllers.Modals;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The access checks and data changes inside the controller actions (quick reply, mark as answer,
/// poll votes, thanks).
/// </summary>
[TestFixture]
[NonParallelizable]
public class ControllerActionTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private ISpamCheck spamCheck;

    private ISessionService session;

    private IAspNetUsersHelper aspNetUsers;

    private int alice;

    private int bob;

    private int forum;

    private int topic;

    private int firstPost;

    [SetUp]
    public void SetUp()
    {
        this.spamCheck = Substitute.For<ISpamCheck>();
        this.session = Substitute.For<ISessionService>();
        this.aspNetUsers = Substitute.For<IAspNetUsersHelper>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(this.spamCheck).As<ISpamCheck>();
                b.RegisterInstance(this.session).As<ISessionService>();
                b.RegisterInstance(this.aspNetUsers).As<IAspNetUsersHelper>();
                b.RegisterInstance(Substitute.For<IUserDisplayName>()).As<IUserDisplayName>();
                b.RegisterType<ActivityStream>().As<IActivityStream>();
                b.RegisterType<Reactions>().As<IReactions>();
                b.RegisterType<PollService>().AsSelf();
                b.RegisterType<DateTimeService>().As<IDateTimeService>();
                b.RegisterInstance(Substitute.For<IAvatars>()).As<IAvatars>();
            },
            useDatabase: true);

        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.forum = this.db.AddForum("Forum");
        this.topic = this.db.AddTopic(this.forum, this.alice, "Alice's question");
        this.firstPost = this.db.AddMessage(this.topic, this.alice, "question");

        this.board.LinkBuilder.GetMessageLink(Arg.Any<Topic>(), Arg.Any<int>()).Returns(c => $"/post/{c.ArgAt<int>(1)}");

        this.board.LoginAs(this.bob);
        this.board.SetPage(this.forum, this.topic);
        this.board.PageLoad.ReadAccess = true;
        this.board.PageLoad.ReplyAccess = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A quick reply notifies the mentioned users who can read the forum (like the reply page)")]
    public async Task QuickReply_Mention_Test()
    {
        this.db.AddForumAccess(this.alice, this.forum);
        this.board.PageUser.Activity = true;

        var alice = await this.Repository<User>().GetByIdAsync(this.alice);
        this.board.Current.Get<IUserDisplayName>().FindUserByName("Alice").Returns(alice);

        await this.QuickReplyAsync("@[userlink]Alice[/userlink] did you see this?");

        (await this.Repository<Activity>().GetAsync(a => a.UserID == this.alice && a.FromUserID == this.bob))
            .Should().ContainSingle(a => new ActivityFlags(a.Flags).WasMentioned);
    }

    [Test]
    [Description("A member with reply access can quick-reply")]
    public async Task QuickReply_Test()
    {
        var result = await this.QuickReplyAsync("my answer");

        var replies = await this.Messages.GetAsync(m => m.TopicID == this.topic && m.UserID == this.bob);
        replies.Should().ContainSingle().Which.MessageText.Should().Be("my answer");
        replies.Single().MessageFlags.IsApproved.Should().BeTrue();

        ((OkObjectResult)result).Value.Should().Be($"/post/{replies.Single().ID}");
    }

    [Test]
    [Description("Quick reply is denied without reply access, also when the user can read the forum")]
    public async Task QuickReply_No_Reply_Access_Test()
    {
        this.board.PageLoad.ReplyAccess = false;

        var result = await this.QuickReplyAsync("sneaky reply");

        AssertDenied(result);
        (await this.Messages.CountAsync(m => m.UserID == this.bob)).Should().Be(0);
    }

    [Test]
    [Description("Quick reply into a locked topic or forum is only allowed for moderators")]
    public async Task QuickReply_Locked_Test()
    {
        this.Topics.Lock(this.topic, new TopicFlags { IsLocked = true }.BitValue);
        this.board.SetPage(this.forum, this.topic);

        AssertDenied(await this.QuickReplyAsync("locked topic"));

        this.board.PageLoad.ModeratorAccess = true;
        await this.QuickReplyAsync("moderator note");

        (await this.Messages.GetAsync(m => m.UserID == this.bob)).Select(m => m.MessageText).Should().Equal("moderator note");

        var lockedForum = this.db.AddForum("Locked", configure: f => f.Flags = new ForumFlags { IsLocked = true }.BitValue);
        var lockedForumTopic = this.db.AddTopic(lockedForum, this.alice);
        this.board.PageLoad.ModeratorAccess = false;
        this.board.SetPage(lockedForum, lockedForumTopic);

        AssertDenied(await this.QuickReplyAsync("locked forum"));
    }

    [Test]
    [Description("Quick replies in moderated forums need approval, unless the forum only moderates new topics, "
                 + "the user has enough posts or is a moderator; moderated users always need approval")]
    [TestCase(false, false, null, 0, false, false, true)]
    [TestCase(true, false, null, 0, false, false, false)]
    [TestCase(true, true, null, 0, false, false, true)]
    [TestCase(true, false, 5, 10, false, false, true)]
    [TestCase(true, false, 5, 2, false, false, false)]
    [TestCase(true, false, null, 0, true, false, true)]
    [TestCase(false, false, null, 0, false, true, false)]
    public async Task QuickReply_Moderated_Forum_Test(
        bool moderated,
        bool newTopicsOnly,
        int? moderatedPostCount,
        int userPosts,
        bool isModerator,
        bool userModerated,
        bool approved)
    {
        await this.board.Current.GetRepository<Forum>().UpdateOnlyAsync(
            () => new Forum
            {
                Flags = new ForumFlags { IsModerated = moderated }.BitValue,
                IsModeratedNewTopicOnly = newTopicsOnly,
                ModeratedPostCount = moderatedPostCount
            },
            f => f.ID == this.forum);
        this.board.SetPage(this.forum, this.topic);

        this.board.PageUser.NumPosts = userPosts;
        this.board.PageUser.Flags = userModerated
            ? this.board.PageUser.Flags | (int)UserFlags.Flags.Moderated
            : this.board.PageUser.Flags;
        this.board.PageLoad.ModeratorAccess = isModerator;

        await this.QuickReplyAsync("reply in a moderated forum");

        (await this.Messages.GetSingleAsync(m => m.UserID == this.bob)).MessageFlags.IsApproved.Should().Be(approved);
    }

    [Test]
    [Description("Quick reply needs an existing, not deleted topic of the current forum")]
    public async Task QuickReply_Invalid_Topic_Test()
    {
        this.board.SetPage(this.forum, null);
        AssertDenied(await this.QuickReplyAsync("no topic"));

        var otherForum = this.db.AddForum("Other");
        var otherTopic = this.db.AddTopic(otherForum, this.alice);
        this.board.SetPage(this.forum, otherTopic);
        this.Query.ForumID = this.forum;
        AssertDenied(await this.QuickReplyAsync("topic of another forum"));

        var deleted = this.db.AddTopic(this.forum, this.alice, configure: t => t.Flags = new TopicFlags { IsDeleted = true }.BitValue);
        this.board.SetPage(this.forum, deleted);
        AssertDenied(await this.QuickReplyAsync("deleted topic"));

        (await this.Messages.CountAsync(m => m.UserID == this.bob)).Should().Be(0);
    }

    [Test]
    [Description("Empty and too long replies and replies within the flood delay are rejected")]
    public async Task QuickReply_Validation_Test()
    {
        this.board.Localization.Add("DEFAULT", "EMPTY_MESSAGE", "empty").Add("DEFAULT", "ISEXCEEDED", "too long");

        Notification(await this.QuickReplyAsync("")).Message.Should().Be("empty");

        this.board.BoardSettings.MaxPostSize = 10;
        Notification(await this.QuickReplyAsync("this is far too long")).Message.Should().Be("too long");

        this.board.BoardSettings.MaxPostSize = 0;
        this.board.BoardSettings.PostFloodDelay = 60;
        this.board.PageLoad.LastPosted = DateTime.UtcNow.AddSeconds(-10);
        Notification(await this.QuickReplyAsync("again")).MessageType.Should().Be(nameof(MessageTypes.warning));

        // the whole remaining time (not only the seconds part of it: 90 seconds were 30)
        this.board.Localization.Add("DEFAULT", "WAIT", "wait {0} seconds");
        this.board.BoardSettings.PostFloodDelay = 120;
        this.board.PageLoad.LastPosted = DateTime.UtcNow.AddSeconds(-30);
        Notification(await this.QuickReplyAsync("again")).Message.Should().Be("wait 90 seconds");

        (await this.Messages.CountAsync(m => m.UserID == this.bob)).Should().Be(0);
    }

    [Test]
    [Description("Replies in a moderated forum wait for approval and notify the moderators")]
    public async Task QuickReply_Moderated_Forum_Test()
    {
        await this.Forums.UpdateOnlyAsync(() => new Forum { Flags = new ForumFlags { IsModerated = true }.BitValue }, f => f.ID == this.forum);
        this.board.SetPage(this.forum, this.topic);
        this.board.BoardSettings.EmailModeratorsOnModeratedPost = true;

        await this.QuickReplyAsync("please approve");

        (await this.Messages.GetSingleAsync(m => m.UserID == this.bob)).MessageFlags.IsApproved.Should().BeFalse();
        await this.board.Notifications.Received(1).ToModeratorsThatMessageNeedsApprovalAsync(this.forum, Arg.Any<int>(), false);

        // the info page needs the info route value ("moderated"), otherwise no link can be built
        this.board.LinkBuilder.Received(1).GetLink(
            ForumPages.Info,
            Arg.Is<object>(o => (int)o.GetType().GetProperty("info")!.GetValue(o) == (int)InfoMessage.Moderated));
    }

    [Test]
    [Description("A quick reply is saved like the other post forms: no html, the content of code blocks is encoded "
                 + "(the code highlighter decodes it once when the post is shown)")]
    public async Task QuickReply_Code_Block_Encoded_Test()
    {
        await this.QuickReplyAsync("[code]<b>bold</b>[/code] <i>italic</i>");

        (await this.Messages.GetSingleAsync(m => m.UserID == this.bob)).MessageText
            .Should().Be("[code]&lt;b&gt;bold&lt;/b&gt;[/code] italic");
    }

    [Test]
    [Description("The watch topic option of the quick reply subscribes and unsubscribes members and moderators")]
    public async Task QuickReply_Watch_Topic_Test()
    {
        await this.QuickReplyAsync("watch it", true);
        this.IsWatching(this.bob).Should().BeTrue();

        await this.Messages.DeleteAsync(m => m.UserID == this.bob);
        await this.QuickReplyAsync("stop watching");
        this.IsWatching(this.bob).Should().BeFalse();

        // moderators skip the spam check, the option still applies
        this.board.PageLoad.ModeratorAccess = true;
        await this.Messages.DeleteAsync(m => m.UserID == this.bob);
        await this.QuickReplyAsync("moderator watches", true);
        this.IsWatching(this.bob).Should().BeTrue();
    }

    [Test]
    [Description("Auto watch checks the watch option of the dialog, the option decides: a member with auto watch who unchecked it does not watch the topic")]
    public async Task QuickReply_Auto_Watch_Test()
    {
        await this.board.Current.GetRepository<User>().UpdateOnlyAsync(() => new User { AutoWatchTopics = true }, u => u.ID == this.bob);
        this.board.LoginAs(this.bob);
        this.board.SetPage(this.forum, this.topic);
        this.board.PageLoad.ReadAccess = true;
        this.board.PageLoad.ReplyAccess = true;

        await this.QuickReplyAsync("auto watched", true);

        this.IsWatching(this.bob).Should().BeTrue();

        // the member unchecked the option (it watched the topic anyway)
        await this.Messages.DeleteAsync(m => m.UserID == this.bob);
        await this.QuickReplyAsync("not watched");

        this.IsWatching(this.bob).Should().BeFalse();
    }

    [Test]
    [Description("Spam is only logged with 'do nothing', with 'delete and ban' the reply is not saved")]
    public async Task QuickReply_Spam_Log_And_Ban_Test()
    {
        this.spamCheck.CheckPostForSpam(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), out Arg.Any<string>())
            .Returns(
                c =>
                {
                    c[4] = "test";
                    return true;
                });

        this.board.BoardSettings.SpamMessageHandling = (int)SpamPostHandling.DoNothing;
        await this.QuickReplyAsync("logged spam");
        (await this.Messages.GetSingleAsync(m => m.UserID == this.bob)).MessageFlags.IsApproved.Should().BeTrue("the spam is only logged");

        await this.Messages.DeleteAsync(m => m.UserID == this.bob);
        this.board.BoardSettings.SpamMessageHandling = (int)SpamPostHandling.DeleteBanUser;
        (await this.QuickReplyAsync("ban me")).Should().BeOfType<OkResult>();
        (await this.Messages.CountAsync(m => m.UserID == this.bob)).Should().Be(0);
        await this.aspNetUsers.Received(1).DeleteAndBanUserAsync(
            Arg.Is<User>(u => u.ID == this.bob),
            Arg.Any<YAF.Types.Models.Identity.AspNetUsers>(),
            Arg.Any<string>());
    }

    [Test]
    [Description("Spam is rejected or held back depending on the board setting")]
    public async Task QuickReply_Spam_Test()
    {
        this.spamCheck.CheckPostForSpam(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), out Arg.Any<string>())
            .Returns(
                c =>
                {
                    c[4] = "test";
                    return true;
                });

        this.board.BoardSettings.SpamMessageHandling = (int)SpamPostHandling.RejectMessage;
        AssertDenied(await this.QuickReplyAsync("buy now"));
        (await this.Messages.CountAsync(m => m.UserID == this.bob)).Should().Be(0);

        this.board.BoardSettings.SpamMessageHandling = (int)SpamPostHandling.FlagMessageUnapproved;
        await this.QuickReplyAsync("buy now");
        (await this.Messages.GetSingleAsync(m => m.UserID == this.bob)).MessageFlags.IsApproved.Should().BeFalse();
    }

    [Test]
    [Description("Moving a topic to another forum")]
    public async Task MoveTopic_Test()
    {
        var target = this.db.AddForum("Target");
        this.board.Localization.Add("MODERATE", "MOVE_TO_DIFFERENT", "pick another forum");

        Notification(await this.CreateController<PostsModalController>().MoveTopic(new MoveTopicModal { ForumListSelected = this.forum }))
            .Message.Should().Be("pick another forum");

        // only to forums the user moderates (here: bob is moderator of the current forum only)
        this.board.Localization.Add("INFO", "TITLE_ACCESSDENIED", "access denied");
        Notification(await this.CreateController<PostsModalController>().MoveTopic(new MoveTopicModal { ForumListSelected = target }))
            .Message.Should().Be("access denied");
        (await this.Topics.GetByIdAsync(this.topic)).ForumID.Should().Be(this.forum);

        this.board.GrantAccess(this.bob, target, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        await this.CreateController<PostsModalController>().MoveTopic(new MoveTopicModal { ForumListSelected = target });

        (await this.Topics.GetByIdAsync(this.topic)).ForumID.Should().Be(target);
    }

    [Test]
    [Description("A topic can not be moved to a forum of another board, also not by an admin")]
    public async Task MoveTopic_Other_Board_Test()
    {
        this.board.PageLoad.IsAdmin = true;
        this.board.Localization.Add("INFO", "TITLE_ACCESSDENIED", "access denied");

        Notification(await this.CreateController<PostsModalController>().MoveTopic(new MoveTopicModal { ForumListSelected = 9999 }))
            .Message.Should().Be("access denied");
        (await this.Topics.GetByIdAsync(this.topic)).ForumID.Should().Be(this.forum);
    }

    [Test]
    [Description("The topic owner marks and unmarks an answer, a new answer replaces the old one")]
    public async Task MarkAsAnswer_Test()
    {
        var bobsReply = this.db.AddMessage(this.topic, this.bob, "answer 1");
        var carolsReply = this.db.AddMessage(this.topic, this.db.AddUser("Carol"), "answer 2");
        this.board.LoginAs(this.alice);
        this.SessionMessages(bobsReply, carolsReply);

        await this.CreateController<PostsController>().MarkAsAnswer(bobsReply);

        this.Topics.GetAnswerMessage(this.topic).Should().Be(bobsReply);
        (await this.Messages.GetByIdAsync(bobsReply)).MessageFlags.IsAnswer.Should().BeTrue();

        this.SessionMessages(bobsReply, carolsReply);
        await this.CreateController<PostsController>().MarkAsAnswer(carolsReply);

        this.Topics.GetAnswerMessage(this.topic).Should().Be(carolsReply);
        (await this.Messages.GetByIdAsync(bobsReply)).MessageFlags.IsAnswer.Should().BeFalse("the old answer is unmarked");

        this.SessionMessages(bobsReply, carolsReply);
        await this.CreateController<PostsController>().MarkAsAnswer(carolsReply);

        this.Topics.GetAnswerMessage(this.topic).Should().BeNull("marking the answer again removes it");
    }

    [Test]
    [Description("A new answer can be marked when the old answer was erased (the topic kept the id of the erased post and no new answer could be marked)")]
    public async Task MarkAsAnswer_Old_Answer_Erased_Test()
    {
        var bobsReply = this.db.AddMessage(this.topic, this.bob, "answer 1");
        var carolsReply = this.db.AddMessage(this.topic, this.db.AddUser("Carol"), "answer 2");

        // the topic points to an answer that does not exist anymore (erased before it was cleared)
        await this.Topics.SetAnswerMessageAsync(this.topic, 999999);

        this.board.LoginAs(this.alice);
        this.SessionMessages(bobsReply, carolsReply);

        await this.CreateController<PostsController>().MarkAsAnswer(carolsReply);

        this.Topics.GetAnswerMessage(this.topic).Should().Be(carolsReply);
        (await this.Messages.GetByIdAsync(carolsReply)).MessageFlags.IsAnswer.Should().BeTrue();
    }

    [Test]
    [Description("The flags of the page data can be outdated: a post deleted in the meantime is not undeleted by marking it as answer")]
    public async Task MarkAsAnswer_Outdated_Page_Data_Test()
    {
        var bobsReply = this.db.AddMessage(this.topic, this.bob, "answer");
        this.board.LoginAs(this.alice);

        // the topic page was loaded before a moderator deleted the post
        this.SessionMessages(bobsReply);

        var deleted = new MessageFlags((await this.Messages.GetByIdAsync(bobsReply)).Flags) { IsDeleted = true };
        await this.Messages.UpdateOnlyAsync(() => new Message { Flags = deleted.BitValue }, x => x.ID == bobsReply);

        await this.CreateController<PostsController>().MarkAsAnswer(bobsReply);

        var flags = (await this.Messages.GetByIdAsync(bobsReply)).MessageFlags;
        flags.IsDeleted.Should().BeTrue("the post stays deleted");
        flags.IsAnswer.Should().BeFalse();
        this.Topics.GetAnswerMessage(this.topic).Should().BeNull();
    }

    [Test]
    [Description("Only the topic owner can mark an answer, and not their own post")]
    public async Task MarkAsAnswer_Not_Owner_Test()
    {
        var bobsReply = this.db.AddMessage(this.topic, this.bob, "answer");
        var alicesReply = this.db.AddMessage(this.topic, this.alice, "own reply");

        // Bob isn't the topic owner
        this.SessionMessages(bobsReply, alicesReply);
        await this.CreateController<PostsController>().MarkAsAnswer(bobsReply);

        // Alice can't mark her own post
        this.board.LoginAs(this.alice);
        this.SessionMessages(bobsReply, alicesReply);
        await this.CreateController<PostsController>().MarkAsAnswer(alicesReply);

        this.Topics.GetAnswerMessage(this.topic).Should().BeNull();
    }

    [Test]
    [Description("A vote counts once per poll, or once per choice for multiple choice polls")]
    public async Task Poll_Vote_Once_Test()
    {
        var (pollId, choiceA, choiceB) = this.AddPoll(this.topic, multipleChoice: false);
        this.board.GrantAccess(this.bob, this.forum, TestDatabase.MemberAccess);

        await this.VoteAsync(choiceA, this.topic, pollId, this.forum);
        await this.VoteAsync(choiceB, this.topic, pollId, this.forum);
        await this.VoteAsync(choiceB, this.topic, pollId, this.forum);
        await this.VoteAsync(choiceA, this.topic, pollId, this.forum);

        this.Votes(choiceA).Should().Be(1);
        this.Votes(choiceB).Should().Be(0, "a single choice poll allows one vote");
        (await this.Repository<PollVote>().CountAsync(v => v.PollID == pollId)).Should().Be(1);

        var (multiPoll, multiA, multiB) = this.AddPoll(this.db.AddTopic(this.forum, this.alice), multipleChoice: true);
        var multiTopic = (await this.Topics.GetSingleAsync(t => t.PollID == multiPoll)).ID;

        await this.VoteAsync(multiA, multiTopic, multiPoll, this.forum);
        await this.VoteAsync(multiB, multiTopic, multiPoll, this.forum);
        await this.VoteAsync(multiB, multiTopic, multiPoll, this.forum);

        this.Votes(multiA).Should().Be(1);
        this.Votes(multiB).Should().Be(1, "each choice counts once");
    }

    [Test]
    [Description("Votes are not counted in locked topics and for closed polls")]
    public async Task Poll_Vote_Locked_And_Closed_Test()
    {
        var (pollId, choiceA, _) = this.AddPoll(this.topic, multipleChoice: false);
        this.board.GrantAccess(this.bob, this.forum, TestDatabase.MemberAccess);

        await this.Topics.UpdateOnlyAsync(() => new Topic { Flags = (int)TopicFlags.Flags.IsLocked }, t => t.ID == this.topic);
        await this.VoteAsync(choiceA, this.topic, pollId, this.forum);
        this.Votes(choiceA).Should().Be(0, "the topic is locked");

        await this.Topics.UpdateOnlyAsync(() => new Topic { Flags = 0 }, t => t.ID == this.topic);
        await this.Repository<Poll>().UpdateOnlyAsync(() => new Poll { Closes = DateTime.UtcNow.AddMinutes(-1) }, p => p.ID == pollId);
        await this.VoteAsync(choiceA, this.topic, pollId, this.forum);
        this.Votes(choiceA).Should().Be(0, "the poll is closed");

        await this.Repository<Poll>().UpdateOnlyAsync(() => new Poll { Closes = DateTime.UtcNow.AddDays(2) }, p => p.ID == pollId);
        await this.VoteAsync(choiceA, this.topic, pollId, this.forum);
        this.Votes(choiceA).Should().Be(1, "the poll is open again");
    }

    [Test]
    [Description("The ids of a vote must belong together and the vote access is checked for the topic's forum")]
    public async Task Poll_Vote_Ids_Test()
    {
        var (pollId, choiceA, _) = this.AddPoll(this.topic, multipleChoice: false);
        var otherTopic = this.db.AddTopic(this.forum, this.alice);
        var (otherPoll, otherChoice, _) = this.AddPoll(otherTopic, multipleChoice: false);

        // Bob may only vote in another forum
        var openForum = this.db.AddForum("Open");
        this.board.GrantAccess(this.bob, this.forum, AccessFlags.Flags.ReadAccess);
        this.board.GrantAccess(this.bob, openForum, TestDatabase.MemberAccess);

        (await this.VoteAsync(choiceA, this.topic, pollId, openForum)).Should().BeOfType<NotFoundResult>();
        await this.VoteAsync(choiceA, this.topic, pollId, this.forum);

        this.Board.GrantAccess(this.bob, this.forum, TestDatabase.MemberAccess);

        (await this.VoteAsync(otherChoice, this.topic, pollId, this.forum)).Should().BeOfType<NotFoundResult>("the choice belongs to another poll");
        (await this.VoteAsync(choiceA, otherTopic, pollId, this.forum)).Should().BeOfType<NotFoundResult>("the poll belongs to another topic");

        this.Votes(choiceA).Should().Be(0);
        this.Votes(otherChoice).Should().Be(0);
        (await this.Repository<PollVote>().CountAsync(v => true)).Should().Be(0);
        _ = otherPoll;
    }

    [Test]
    [Description("Reactions can't be given for unreadable or own posts, or with an unknown reaction")]
    public async Task Reaction_Access_Test()
    {
        var hidden = this.db.AddForum("Hidden");
        var hiddenPost = this.db.AddMessage(this.db.AddTopic(hidden, this.alice), this.alice, "secret");
        var ownPost = this.db.AddMessage(this.topic, this.bob, "mine");
        this.board.GrantAccess(this.bob, this.forum);

        (await this.CreateController<ReactionController>().React(hiddenPost, 1)).Result.Should().BeOfType<NotFoundResult>();
        (await this.CreateController<ReactionController>().React(ownPost, 1)).Result.Should().BeOfType<NotFoundResult>();
        (await this.CreateController<ReactionController>().React(int.MaxValue, 1)).Result.Should().BeOfType<NotFoundResult>();
        (await this.CreateController<ReactionController>().React(this.firstPost, 0)).Result.Should().BeOfType<NotFoundResult>();
        (await this.CreateController<ReactionController>().React(this.firstPost, 6)).Result.Should().BeOfType<NotFoundResult>();

        this.Repository<Thanks>().Count(t => true).Should().Be(0);
    }

    [Test]
    [Description("Removing a reaction of an unreadable post shows nothing (the answer lists the users who reacted)")]
    public async Task Reaction_Remove_Hidden_Post_Test()
    {
        var hidden = this.db.AddForum("Hidden");
        var hiddenPost = this.db.AddMessage(this.db.AddTopic(hidden, this.alice), this.alice, "secret");
        await this.Repository<Thanks>().InsertAsync(
            new Thanks { MessageID = hiddenPost, ThanksFromUserID = this.bob, ThanksToUserID = this.alice, ThanksDate = DateTime.UtcNow });

        this.board.GrantAccess(this.bob, this.forum);

        (await this.CreateController<ReactionController>().React(hiddenPost, 2)).Result.Should().BeOfType<NotFoundResult>();

        this.Repository<Thanks>().Count(t => t.MessageID == hiddenPost).Should().Be(1);
    }

    [Test]
    [Description("Reacting again with another reaction changes it, with the same reaction removes it and only the activities of this reaction")]
    public async Task Reaction_Change_And_Remove_Test()
    {
        var carol = this.db.AddUser("Carol");
        this.board.GrantAccess(this.bob, this.forum);
        this.board.GrantAccess(carol, this.forum);
        await this.Users.UpdateOnlyAsync(() => new User { Activity = true }, u => true);

        var info = (await this.CreateController<ReactionController>().React(this.firstPost, (int)ReactionType.Like)).Result
            .Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<ReactionInfo>().Subject;
        info.MessageID.Should().Be(this.firstPost);
        info.UserReaction.Should().Be((int)ReactionType.Like);
        info.ReactionsHtml.Should().Contain($"id=\"dvReactions{this.firstPost}\"");

        this.board.LoginAs(carol);
        await this.CreateController<ReactionController>().React(this.firstPost, (int)ReactionType.Wow);

        (await this.Repository<Activity>().CountAsync(a => a.MessageID == this.firstPost)).Should().Be(4);

        // another reaction replaces the reaction without new activities
        await this.CreateController<ReactionController>().React(this.firstPost, (int)ReactionType.Hooray);
        info = (ReactionInfo)((OkObjectResult)(await this.CreateController<ReactionController>().React(this.firstPost, (int)ReactionType.Laugh)).Result).Value;
        info.UserReaction.Should().Be((int)ReactionType.Laugh);

        (await this.Repository<Thanks>().GetAsync(t => t.MessageID == this.firstPost))
            .Select(t => (t.ThanksFromUserID, (ReactionType)t.Reaction))
            .Should().BeEquivalentTo([(this.bob, ReactionType.Like), (carol, ReactionType.Laugh)]);
        (await this.Repository<Activity>().CountAsync(a => a.MessageID == this.firstPost)).Should().Be(4);

        // the same reaction removes it
        await this.CreateController<ReactionController>().React(this.firstPost, (int)ReactionType.Laugh);

        (await this.Repository<Thanks>().GetAsync(t => t.MessageID == this.firstPost)).Select(t => t.ThanksFromUserID)
            .Should().Equal(this.bob);

        var activities = await this.Repository<Activity>().GetAsync(a => a.MessageID == this.firstPost);
        activities.Should().HaveCount(2, "Bob's reaction activities are kept");
        activities.Should().OnlyContain(a => a.UserID == this.bob || a.FromUserID == this.bob);
    }

    private TestBoardContext Board => this.board;

    private PageQueryData Query => this.board.Query;

    private IRepository<Message> Messages => this.board.Current.GetRepository<Message>();

    private IRepository<Topic> Topics => this.board.Current.GetRepository<Topic>();

    private IRepository<Forum> Forums => this.board.Current.GetRepository<Forum>();

    private IRepository<User> Users => this.board.Current.GetRepository<User>();

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }

    private T CreateController<T>()
        where T : Controller, new()
    {
        return this.board.CreateController<T>();
    }

    private Task<IActionResult> QuickReplyAsync(string text, bool watch = false)
    {
        return this.CreateController<PostsModalController>().Reply(new QuickReplyModal { QuickReplyEditor = text, TopicWatch = watch });
    }

    private bool IsWatching(int userId)
    {
        return this.board.Current.GetRepository<WatchTopic>().Exists(w => w.UserID == userId && w.TopicID == this.topic);
    }

    private Task<IActionResult> VoteAsync(int choiceId, int topicId, int pollId, int forumId)
    {
        return this.CreateController<PollController>().Vote(choiceId, topicId, pollId, forumId);
    }

    private static MessageModalNotification Notification(IActionResult result)
    {
        return result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<MessageModalNotification>().Subject;
    }

    private static void AssertDenied(IActionResult result)
    {
        Notification(result).MessageType.Should().Be(nameof(MessageTypes.danger));
    }

    private void SessionMessages(params int[] messageIds)
    {
        var topicRow = this.Topics.GetById(this.topic);

        var messages = messageIds.Select(
            id =>
            {
                var message = this.Messages.GetById(id);

                return new PagedMessage
                {
                    MessageID = id,
                    TopicID = this.topic,
                    Topic = topicRow.TopicName,
                    TopicOwnerID = topicRow.UserID,
                    UserID = message.UserID,
                    Flags = message.Flags
                };
            }).ToList();

        this.session.GetPageData<List<PagedMessage>>().Returns(messages);
    }

    private (int PollId, int ChoiceA, int ChoiceB) AddPoll(int topicId, bool multipleChoice)
    {
        using var connection = this.db.Open();

        var pollId = (int)connection.Insert(
            new Poll
            {
                Question = "Which one?",
                UserID = this.alice,
                Flags = new PollFlags { AllowMultipleChoice = multipleChoice }.BitValue
            },
            selectIdentity: true);

        var choiceA = (int)connection.Insert(new Choice { PollID = pollId, ChoiceName = "A" }, selectIdentity: true);
        var choiceB = (int)connection.Insert(new Choice { PollID = pollId, ChoiceName = "B" }, selectIdentity: true);

        connection.UpdateOnly(() => new Topic { PollID = pollId }, t => t.ID == topicId);

        return (pollId, choiceA, choiceB);
    }

    private int Votes(int choiceId)
    {
        return this.Repository<Choice>().GetById(choiceId).Votes;
    }
}
