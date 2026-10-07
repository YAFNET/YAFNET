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

namespace YAF.Tests.CoreTests.Data;

using System.IO;
using System.Threading.Tasks;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// UserRepositoryExtensions tests: registration, approval, ranks, points, profile updates,
/// member lists, notifications, LazyData and deleting users.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserRepositoryTests
{
    private readonly DateTime now = DateTime.UtcNow;

    private TestBoardContext board;

    private TestDatabase db;

    private int guest;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;

        this.guest = this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsGuest | UserFlags.Flags.IsApproved));
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");

        this.board.PageLoad.GuestUserID = this.guest;
        this.board.PageUser.ID = this.alice;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("AspNetAsync creates a new user with the start rank")]
    public async Task AspNet_New_User_Test()
    {
        var id = await this.Users.AspNetAsync(TestDatabase.BoardId, "carol", null, "carol@example.com", "key-carol", 15, true);

        var user = this.Users.GetById(id);
        user.Name.Should().Be("carol");
        user.DisplayName.Should().Be("carol", "the user name is the fallback display name");
        user.RankID.Should().Be(TestDatabase.RankId);
        user.UserFlags.IsApproved.Should().BeTrue();
        user.PageSize.Should().Be(15);
        user.ProviderUserKey.Should().Be("key-carol");
    }

    [Test]
    [Description("AspNetAsync finds existing users by provider key or name and approves them")]
    public async Task AspNet_Existing_User_Test()
    {
        var dave = this.db.AddUser("Dave", u =>
        {
            u.ProviderUserKey = "key-dave";
            u.Flags = 0;
        });

        (await this.Users.AspNetAsync(TestDatabase.BoardId, "dave-renamed", "", "new@example.com", "key-dave", 20, true))
            .Should().Be(dave);

        var user = this.Users.GetById(dave);
        user.UserFlags.IsApproved.Should().BeTrue();
        user.Email.Should().Be("new@example.com");
        user.DisplayName.Should().Be("Dave");

        (await this.Users.AspNetAsync(TestDatabase.BoardId, "Bob", "Bob", "bob@example.com", "other-key", 20, false))
            .Should().Be(this.bob);

        (await this.Users.GetUserByProviderKeyAsync(TestDatabase.BoardId, "key-dave")).ID.Should().Be(dave);
        (await this.Users.GetUserByProviderKeyAsync(null, "key-dave")).ID.Should().Be(dave);
        (await this.Users.GetUserByProviderKeyAsync(2, "key-dave")).Should().BeNull();
    }

    [Test]
    [Description("Approving a user sets the flag and sends the welcome notification once")]
    public async Task ApproveAsync_Test()
    {
        var pending = this.db.AddUser("Pending", u => u.Flags = 0);

        await this.Users.ApproveAsync(pending);
        await this.Users.ApproveAsync(pending);

        this.Users.GetById(pending).UserFlags.IsApproved.Should().BeTrue();
        await this.board.Notifications.Received(1).SendUserWelcomeNotificationAsync(Arg.Is<User>(u => u.ID == pending));
    }

    [Test]
    [Description("Users on a rank ladder get promoted to the highest rank their post count allows")]
    public async Task PromoteAsync_Ladder_Test()
    {
        var newbie = this.db.AddRank("Newbie", 0, isLadder: true);
        var member = this.db.AddRank("Regular", 10, isLadder: true);
        var veteran = this.db.AddRank("Veteran", 50, isLadder: true);

        var user = this.db.AddUser("Climber", u =>
        {
            u.RankID = newbie;
            u.NumPosts = 12;
        });

        await this.Users.PromoteAsync(user);

        this.Users.GetById(user).RankID.Should().Be(member);

        this.Users.UpdateOnly(() => new User { NumPosts = 60 }, u => u.ID == user);
        await this.Users.PromoteAsync(user);

        this.Users.GetById(user).RankID.Should().Be(veteran);
    }

    [Test]
    [Description("Users don't get promoted without enough posts or outside a ladder rank")]
    public async Task PromoteAsync_No_Promotion_Test()
    {
        var newbie = this.db.AddRank("Newbie", 0, isLadder: true);
        this.db.AddRank("Regular", 10, isLadder: true);
        var special = this.db.AddRank("Special", 0);

        var fewPosts = this.db.AddUser("Few posts", u =>
        {
            u.RankID = newbie;
            u.NumPosts = 3;
        });
        var notOnLadder = this.db.AddUser("Special user", u =>
        {
            u.RankID = special;
            u.NumPosts = 100;
        });

        await this.Users.PromoteAsync(fewPosts);
        await this.Users.PromoteAsync(notOnLadder);

        this.Users.GetById(fewPosts).RankID.Should().Be(newbie);
        this.Users.GetById(notOnLadder).RankID.Should().Be(special);
    }

    [Test]
    [Description("Member counts and lists skip guests, deleted and unapproved users")]
    public async Task Member_Lists_Test()
    {
        var pending = this.db.AddUser("Pending", u => u.Flags = 0);
        this.db.AddUser("Deleted", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsDeleted));
        var oldMember = this.db.AddUser("Old member", u => u.Joined = this.now.AddYears(-3));

        (await this.Users.BoardMembersAsync(TestDatabase.BoardId)).Should().Be(3, "Alice, Bob and the old member");

        (await this.Users.GetUnApprovedUsersAsync(TestDatabase.BoardId)).Select(u => u.ID).Should().Equal(pending);

        (await this.Users.LatestAsync(TestDatabase.BoardId)).Select(u => u.ID).Should()
            .Contain([this.alice, this.bob]).And.NotContain([oldMember, pending, this.guest]);

        (await this.Users.GetRegisteredUsersByMonthAsync(TestDatabase.BoardId)).Should()
            .NotContain(u => u.ID == oldMember);
    }

    [Test]
    [Description("Most active users are the top posters of the period")]
    public async Task LastActiveAsync_Test()
    {
        var forum = this.db.AddForum();
        var topic = this.db.AddTopic(forum, this.alice);
        var carol = this.db.AddUser("Carol");

        this.AddPosts(topic, this.alice, 1);
        this.AddPosts(topic, this.bob, 5);
        this.AddPosts(topic, carol, 3);
        this.AddPosts(topic, this.guest, 9);
        this.db.AddMessage(topic, this.alice, posted: this.now.AddDays(-30));

        var top2 = await this.Users.LastActiveAsync(TestDatabase.BoardId, this.guest, this.now.AddDays(-7), 2);

        top2.Select(u => u.ID).Should().Equal(this.bob, carol);
        top2[0].NumOfPosts.Should().Be(5);

        var all = await this.Users.LastActiveAsync(TestDatabase.BoardId, this.guest, this.now.AddDays(-7), 10);

        all.Select(u => u.ID).Should().Equal(this.bob, carol, this.alice);
        all.Last().NumOfPosts.Should().Be(1, "the post older than the start date doesn't count");
    }

    [Test]
    [Description("Reputation points are added and removed and the vote is recorded")]
    public async Task Points_And_Reputation_Vote_Test()
    {
        var start = this.Users.GetById(this.bob).Points;

        await this.Users.AddPointsAsync(this.bob, this.alice, 5);
        this.Users.GetById(this.bob).Points.Should().Be(start + 5);

        this.Repository<ReputationVote>()
            .GetSingle(v => v.ReputationFromUserID == this.alice && v.ReputationToUserID == this.bob)
            .Should().NotBeNull("the vote date is used to limit voting to once a day");

        await this.Users.RemovePointsAsync(this.bob, this.alice, 2);
        this.Users.GetById(this.bob).Points.Should().Be(start + 3);
        this.Repository<ReputationVote>().Count(v => v.ReputationToUserID == this.bob).Should().Be(1);

        await this.Users.AddPointsAsync(this.bob, null, 1);
        await this.Users.SetPointsAsync(this.alice, 42);

        this.Users.GetById(this.bob).Points.Should().Be(start + 4);
        this.Users.GetById(this.alice).Points.Should().Be(42);
    }

    [Test]
    [Description("Profile updates write the expected columns")]
    public async Task Profile_Updates_Test()
    {
        await this.Users.SaveAsync(this.alice, new UserFlags { IsApproved = true }, "UTC", "german.json", "de-DE", "dark.css", true, false, 50);
        await this.Users.SaveSignatureAsync(this.alice, "[b]sig[/b]");
        await this.Users.SaveNotificationAsync(this.alice, true, 2, true);
        await this.Users.UpdateBlockFlagsAsync(this.alice, 3);
        await this.Users.SuspendAsync(this.alice, this.now.AddDays(1), "spam", this.bob);

        var user = this.Users.GetById(this.alice);
        user.UserFlags.IsDirty.Should().BeTrue();
        user.UserFlags.IsActiveExcluded.Should().BeTrue();
        user.UserFlags.IsApproved.Should().BeTrue();
        user.TimeZone.Should().Be("UTC");
        user.LanguageFile.Should().Be("german.json");
        user.Culture.Should().Be("de-DE");
        user.ThemeFile.Should().Be("dark.css");
        user.Activity.Should().BeFalse();
        user.PageSize.Should().Be(50);
        user.Signature.Should().Be("[b]sig[/b]");
        user.AutoWatchTopics.Should().BeTrue();
        user.NotificationType.Should().Be(2);
        user.DailyDigest.Should().BeTrue();
        user.BlockFlags.Should().Be(3);
        user.SuspendedReason.Should().Be("spam");
        user.SuspendedBy.Should().Be(this.bob);

        await this.Users.SuspendAsync(this.alice);
        this.Users.GetById(this.alice).Suspended.Should().BeNull();

        await this.Users.AdminSaveAsync(TestDatabase.BoardId, this.alice, (int)UserFlags.Flags.IsHostAdmin, TestDatabase.RankId);
        this.Users.GetById(this.alice).UserFlags.IsHostAdmin.Should().BeTrue();
    }

    [Test]
    [Description("Avatars can be uploaded, linked and removed")]
    public async Task Avatar_Test()
    {
        using (var stream = new MemoryStream([1, 2, 3]))
        {
            await this.Users.SaveAvatarAsync(this.alice, null, stream, "image/png");
        }

        var user = this.Users.GetById(this.alice);
        user.AvatarImage.Should().Equal(1, 2, 3);
        user.AvatarImageType.Should().Be("image/png");

        await this.Users.SaveAvatarAsync(this.alice, "https://example.com/a.png", null, null);

        user = this.Users.GetById(this.alice);
        user.Avatar.Should().Be("https://example.com/a.png");
        user.AvatarImage.Should().BeNull();

        await this.Users.DeleteAvatarAsync(this.alice);
        this.Users.GetById(this.alice).Avatar.Should().BeNull();
    }

    [Test]
    [Description("Changing the display name updates it on the user's posts and last post columns")]
    public async Task UpdateDisplayNameAsync_Test()
    {
        var forum = this.db.AddForum();
        var topic = this.db.AddTopic(forum, this.alice, configure: t => t.UserDisplayName = "Alice");
        var message = this.db.AddMessage(topic, this.alice, configure: m => m.UserDisplayName = "Alice");
        var custom = this.db.AddMessage(topic, this.alice, configure: m => m.UserDisplayName = "Guest name");
        this.Repository<Topic>().UpdateOnly(() => new Topic { LastUserDisplayName = "Alice" }, t => t.ID == topic);

        await this.Users.UpdateDisplayNameAsync(this.Users.GetById(this.alice), "Alice Smith");

        this.Users.GetById(this.alice).DisplayName.Should().Be("Alice Smith");
        this.Repository<Topic>().GetById(topic).UserDisplayName.Should().Be("Alice Smith");
        this.Repository<Topic>().GetById(topic).LastUserDisplayName.Should().Be("Alice Smith");
        this.Repository<Message>().GetById(message).UserDisplayName.Should().Be("Alice Smith");
        this.Repository<Message>().GetById(custom).UserDisplayName.Should().Be("Guest name");

        await this.Users.UpdateDisplayNameAsync(this.Users.GetById(this.alice), "");
        this.Users.GetById(this.alice).DisplayName.Should().Be("Alice Smith");
    }

    [Test]
    [Description("Deleting a user moves their posts to the guest and removes their data")]
    public async Task DeleteAsync_Test()
    {
        var forum = this.db.AddForum();
        var topic = this.db.AddTopic(forum, this.bob);
        var message = this.db.AddMessage(topic, this.bob);
        await this.Repository<IgnoreUser>().AddIgnoredUserAsync(this.bob, this.alice);
        this.board.GrantAccess(this.bob, forum);

        await this.Users.DeleteAsync(this.Users.GetById(this.bob));

        this.Users.GetById(this.bob).Should().BeNull();

        var post = this.Repository<Message>().GetById(message);
        post.UserID.Should().Be(this.guest);
        post.UserName.Should().Be("Bob");
        this.Repository<Topic>().GetById(topic).UserID.Should().Be(this.guest);
        this.Repository<Topic>().GetById(topic).LastUserID.Should().Be(this.guest);
        this.Repository<IgnoreUser>().Count(i => i.UserID == this.bob).Should().Be(0);
        this.Repository<UserGroup>().Count(g => g.UserID == this.bob).Should().Be(0);

        await this.Users.DeleteAsync(this.Users.GetById(this.guest));
        this.Users.GetById(this.guest).Should().NotBeNull("the guest user can't be deleted");
    }

    [Test]
    [Description("Watch notifications go to topic and forum watchers, once each, never to the poster")]
    public async Task WatchMailListAsync_Test()
    {
        var forum = this.db.AddForum();
        var topic = this.db.AddTopic(forum, this.alice);
        var carol = this.db.AddUser("Carol");
        var noMails = this.db.AddUser("No mails", u => u.NotificationType = 10);
        var pending = this.db.AddUser("Pending", u =>
        {
            u.Flags = 0;
            u.NotificationType = 0;
        });

        // TopicsISubscribeTo
        this.Users.UpdateOnly(() => new User { NotificationType = 0 }, u => u.Name != "No mails" && u.Name != "Pending");

        using (var connection = this.db.Open())
        {
            foreach (var user in new[] { this.alice, this.bob, noMails, pending })
            {
                connection.Insert(new WatchTopic { UserID = user, TopicID = topic, Created = this.now });
            }

            connection.Insert(new WatchForum { UserID = this.bob, ForumID = forum, Created = this.now });
            connection.Insert(new WatchForum { UserID = carol, ForumID = forum, Created = this.now });
        }

        var list = await this.Users.WatchMailListAsync(topic, this.alice);

        list.Select(u => u.ID).Should().BeEquivalentTo([this.bob, carol]);
    }

    [Test]
    [Description("Group e-mails, admins and moderators come from the group and access tables")]
    public async Task Groups_Admins_And_Moderators_Test()
    {
        var forum = this.db.AddForum("Moderated forum");
        var adminGroup = this.db.AddForumAccess(this.alice, forum, groupFlags: GroupFlags.Flags.IsAdmin);
        this.db.AddForumAccess(this.bob, forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);
        var guestGroup = this.db.AddForumAccess(this.guest, forum, groupFlags: GroupFlags.Flags.IsGuest);

        (await this.Users.GroupEmailsAsync(adminGroup)).Should().Equal("alice@example.com");
        (await this.Users.GroupEmailsAsync(guestGroup)).Should().BeEmpty("guest groups don't get e-mails");

        (await this.Users.ListAdminsAsync()).Select(u => u.ID).Should().Equal(this.alice);

        var moderators = await this.Users.GetForumModeratorsAsync();

        moderators.Should().ContainSingle(m => m.ModeratorID == this.bob).Which.ForumName.Should().Be("Moderated forum");
    }

    [Test]
    [Description("Recent users are approved members who visited within 30 days")]
    public async Task GetRecentUsersAsync_Test()
    {
        this.db.AddUser("Long gone", u => u.LastVisit = this.now.AddDays(-60));

        var recent = await this.Users.GetRecentUsersAsync();

        recent.Select(u => u.UserID).Should().BeEquivalentTo([this.alice, this.bob]);
        recent.Should().OnlyContain(u => !u.IsGuest);
    }

    [Test]
    [Description("LazyData returns the notification counters of the user")]
    public async Task LazyDataAsync_Test()
    {
        using (var connection = this.db.Open())
        {
            this.AddActivity(connection, new ActivityFlags { ReceivedThanks = true });
            this.AddActivity(connection, new ActivityFlags { WasMentioned = true });
            this.AddActivity(connection, new ActivityFlags { WasMentioned = true });
            this.AddActivity(connection, new ActivityFlags { WasQuoted = true });
            this.AddActivity(connection, new ActivityFlags { WasQuoted = true });
            this.AddActivity(connection, new ActivityFlags { WasQuoted = true });
            this.AddActivity(connection, new ActivityFlags { WatchTopicReply = true });
            this.AddActivity(connection, new ActivityFlags { WasQuoted = true }, notification: false);
        }

        this.Users.UpdateOnly(() => new User { TimeZone = "Europe/Berlin" }, u => u.ID == this.alice);

        var data = await this.Users.LazyDataAsync(this.alice, TestDatabase.BoardId, false, false, false);

        data.TimeZoneUser.Should().Be("Europe/Berlin");
        data.ReceivedThanks.Should().Be(1);
        data.Mention.Should().Be(2);
        data.Quoted.Should().Be(3);
        data.WatchTopic.Should().Be(1);
        data.UserHasBuddies.Should().BeFalse();
    }

    [Test]
    [Description("LazyData counts posts waiting for the moderator")]
    public async Task LazyDataAsync_ModeratePosts_Test()
    {
        var forum = this.db.AddForum();
        this.board.GrantAccess(this.alice, forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        var topic = this.db.AddTopic(forum, this.bob);
        this.db.AddMessage(topic, this.bob, configure: m => m.Flags = new MessageFlags { IsApproved = false }.BitValue);
        this.db.AddMessage(topic, this.bob, configure: m => m.Flags = new MessageFlags { IsApproved = true, IsReported = true }.BitValue);
        this.db.AddMessage(topic, this.bob);

        (await this.Users.LazyDataAsync(this.alice, TestDatabase.BoardId, false, false, false)).ModeratePosts.Should().Be(2);
        (await this.Users.LazyDataAsync(this.bob, TestDatabase.BoardId, false, false, false)).ModeratePosts.Should().Be(0);
    }

    [Test]
    [Description("Group styles win over rank styles")]
    public async Task UpdateStyleAsync_Test()
    {
        var styledRank = this.db.AddRank("Styled", configure: r => r.Style = "color: blue");
        this.Users.UpdateOnly(() => new User { RankID = styledRank }, u => u.ID == this.bob);
        this.Users.UpdateOnly(() => new User { RankID = styledRank }, u => u.ID == this.alice);

        var groupId = this.db.AddForumAccess(this.alice, this.db.AddForum());
        this.Repository<Group>().UpdateOnly(() => new Group { Style = "color: red" }, g => g.ID == groupId);

        await this.Users.UpdateStylesAsync(TestDatabase.BoardId);

        this.Users.GetById(this.alice).UserStyle.Should().Be("color: red");
        this.Users.GetById(this.bob).UserStyle.Should().Be("color: blue");
    }

    private IRepository<User> Users => this.board.Current.GetRepository<User>();

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }

    private void AddPosts(int topic, int userId, int count)
    {
        for (var i = 0; i < count; i++)
        {
            this.db.AddMessage(topic, userId, posted: this.now.AddHours(-1 - i));
        }
    }

    private void AddActivity(System.Data.IDbConnection connection, ActivityFlags flags, bool notification = true)
    {
        connection.Insert(
            new Activity
            {
                UserID = this.alice, FromUserID = this.bob, Flags = flags.BitValue, Notification = notification, Created = this.now
            });
    }
}
