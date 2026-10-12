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

using Autofac;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// Thanks, reputation, friends and the activity stream.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SocialServicesTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private IAvatars avatars;

    private IUserDisplayName displayName;

    private int alice;

    private int bob;

    private int carol;

    private int topic;

    private int message;

    [SetUp]
    public void SetUp()
    {
        this.avatars = Substitute.For<IAvatars>();
        this.avatars.GetAvatarUrlForUser(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<bool>())
            .Returns(c => $"/avatar/{c.ArgAt<int>(0)}");

        this.displayName = Substitute.For<IUserDisplayName>();
        this.displayName.GetNameById(Arg.Any<int>()).Returns(c => $"User {c.Arg<int>()}");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<ThankYou>().As<IThankYou>();
                b.RegisterType<Reputation>().As<IReputation>();
                b.RegisterType<Friends>().As<IFriends>();
                b.RegisterType<ActivityStream>().As<IActivityStream>();
                b.RegisterType<DateTimeService>().As<IDateTimeService>();
                b.RegisterInstance(this.avatars).As<IAvatars>();
                b.RegisterInstance(this.displayName).As<IUserDisplayName>();
            },
            useDatabase: true);

        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice", u => u.Activity = true);
        this.bob = this.db.AddUser("Bob <b>", u => u.Activity = true);
        this.carol = this.db.AddUser("Carol", u => u.Activity = false);

        this.topic = this.db.AddTopic(this.db.AddForum(), this.alice);
        this.message = this.db.AddMessage(this.topic, this.alice);

        this.LoginAs(this.alice);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Thanks are added, counted and removed per user and message")]
    public void Thanks_Repository_Test()
    {
        var thanks = this.Repository<Thanks>();

        thanks.AddMessageThanks(this.bob, this.alice, this.message);
        thanks.AddMessageThanks(this.carol, this.alice, this.message);

        thanks.ThankedMessage(this.message, this.bob).Should().BeTrue();
        thanks.ThankedMessage(this.message, this.alice).Should().BeFalse();
        thanks.ThanksFromUser(this.bob).Should().Be(1);
        thanks.ThanksToUser(this.alice).Posts.Should().Be(1);
        thanks.ThanksToUser(this.alice).ThanksReceived.Should().Be("2");
        thanks.MessageGetThanksList(this.message).Select(t => t.Item2.ID).Should().BeEquivalentTo([this.bob, this.carol]);

        thanks.RemoveMessageThanks(this.bob, this.message);

        thanks.ThankedMessage(this.message, this.bob).Should().BeFalse();
        thanks.ThankedMessage(this.message, this.carol).Should().BeTrue();
    }

    [Test]
    [Description("The thank you box lists the thankers with encoded names and avatars")]
    public void ThankYou_Info_Test()
    {
        var service = this.board.Resolve<IThankYou>();

        service.ThanksInfo("Alice", this.message).Should().Be("&nbsp;");

        this.Repository<Thanks>().AddMessageThanks(this.bob, this.alice, this.message);

        var html = service.ThanksInfo("Alice", this.message);

        html.Should().Contain($"src=\"/avatar/{this.bob}\"");
        html.Should().Contain("alt=\"Bob &lt;b&gt;\"").And.NotContain("Bob <b>");

        service.ThanksInfo("Alice", this.message).Should().StartWith("<span class=\"avatar-stack\">");

        this.board.Localization.Add("BUTTON", "BUTTON_THANKS", "Thanks").Add("BUTTON", "BUTTON_THANKS_TT", "Say thanks");

        var info = service.CreateThankYou("Alice", "BUTTON_THANKS", "BUTTON_THANKS_TT", this.message);
        info.MessageID.Should().Be(this.message);
        info.Text.Should().Be("Thanks");
        info.Title.Should().Be("Say thanks");

        service.GetThankYou("Alice", "BUTTON_THANKS", "BUTTON_THANKS_TT", this.message).ThanksInfo
            .Should().NotContain("fa-heart");
    }

    [Test]
    [Description("The thanks date is shown in the title when enabled")]
    public void ThankYou_Date_Test()
    {
        this.board.BoardSettings.ShowThanksDate = true;
        this.board.Localization.Add("DEFAULT", "ONDATE", "on {0}").Add("DEFAULT", "FORMAT_DATE_SHORT", "yyyy-MM-dd");
        this.Repository<Thanks>().AddMessageThanks(this.bob, this.alice, this.message);

        this.board.Resolve<IThankYou>().ThanksInfo("Alice", this.message).Should()
            .Contain($"title=\"Bob &lt;b&gt; on {DateTime.UtcNow:yyyy-MM-dd}\"");
    }

    [Test]
    [Description("Reputation voting is allowed once every 24 hours")]
    public void Reputation_Voting_Allowed_Test()
    {
        var reputation = this.board.Resolve<IReputation>();

        reputation.CheckIfAllowReputationVoting(null).Should().BeTrue();
        reputation.CheckIfAllowReputationVoting(DBNull.Value).Should().BeTrue();
        reputation.CheckIfAllowReputationVoting(DateTime.UtcNow.AddHours(-25)).Should().BeTrue();
        reputation.CheckIfAllowReputationVoting(DateTime.UtcNow.AddHours(-1)).Should().BeFalse();
    }

    [Test]
    [Description("Reputation percentage: 0 is neutral, the limits are 0% and 100%, more points never mean less")]
    public void Reputation_Percentage_Test()
    {
        var reputation = this.board.Resolve<IReputation>();

        // defaults: -100 .. 500, negative allowed
        reputation.ConvertPointsToPercentage(0).Should().Be(50);
        reputation.ConvertPointsToPercentage(500).Should().Be(100);
        reputation.ConvertPointsToPercentage(5000).Should().Be(100);
        reputation.ConvertPointsToPercentage(-100).Should().Be(0);
        reputation.ConvertPointsToPercentage(-1000).Should().Be(0);

        var values = Enumerable.Range(-120, 640).Select(reputation.ConvertPointsToPercentage).ToList();
        values.Should().BeInAscendingOrder().And.OnlyContain(v => v >= 0 && v <= 100);
    }

    [TestCase(0f, "HATED", " bg-danger")]
    [TestCase(10f, "HOSTILE", " bg-warning")]
    [TestCase(45f, "UNFRIENDLY", " bg-info")]
    [TestCase(55f, "NEUTRAL", " bg-info")]
    [TestCase(70f, "FRIENDLY", " bg-success")]
    [TestCase(85f, "HONORED", " bg-success")]
    [TestCase(100f, "EXALTED", " bg-success")]
    [Description("The reputation bar text and colour follow the percentage")]
    public void Reputation_Bar_Test(float percentage, string expectedText, string expectedColor)
    {
        var reputation = this.board.Resolve<IReputation>();

        reputation.GetReputationBarText(percentage).Should().Be($"[REPUTATION_VALUES.{expectedText}]");
        reputation.GetReputationBarColor(percentage).Trim().Should().Be(expectedColor.Trim());
    }

    [Test]
    [Description("The reputation bar renders the percentage with a dot as decimal separator")]
    public void Reputation_Bar_Html_Test()
    {
        var html = this.board.Resolve<IReputation>().GenerateReputationBar(0, this.alice);

        html.Should().Contain("style=\"width:50%;\"").And.Contain("[REPUTATION_VALUES.NEUTRAL]");
    }

    [Test]
    [Description("A friend request becomes a friendship once approved, in both directions")]
    public void Friends_Request_And_Approve_Test()
    {
        var friends = this.board.Resolve<IFriends>();

        friends.AddRequest(this.bob).Should().BeFalse("the request still has to be approved");
        friends.AddRequest(this.bob).Should().BeFalse("a second request is ignored");

        this.Buddies.GetSendRequests(this.alice).Select(b => b.UserID).Should().Equal(this.bob);
        this.Buddies.GetReceivedRequests(this.bob).Select(b => b.UserID).Should().Equal(this.alice);
        friends.IsBuddy(this.bob).Should().BeTrue("a sent request counts for IsBuddy");

        this.LoginAs(this.bob);
        friends.ApproveRequest(this.Buddies.GetReceivedRequests(this.bob).Single()).Should().BeTrue();

        this.Buddies.GetAllFriends(this.bob).Select(b => b.UserID).Should().Equal(this.alice);
        this.Buddies.GetAllFriends(this.alice).Select(b => b.UserID).Should().Equal(this.bob);
        this.Buddies.GetReceivedRequests(this.bob).Should().BeEmpty();

        var activities = this.Repository<Activity>().Get(a => (a.Flags & 32768) == 32768);
        activities.Select(a => (a.UserID, a.FromUserID)).Should().BeEquivalentTo([(this.bob, (int?)this.alice), (this.alice, (int?)this.bob)]);

        this.board.Events.OfType<UpdateUserEvent>().Should().NotBeEmpty();
    }

    [Test]
    [Description("A request to someone who already asked you makes you friends immediately")]
    public void Friends_Mutual_Request_Test()
    {
        var friends = this.board.Resolve<IFriends>();

        this.LoginAs(this.bob);
        friends.AddRequest(this.alice);

        this.LoginAs(this.alice);
        friends.AddRequest(this.bob).Should().BeTrue();

        this.Buddies.GetAllFriends(this.alice).Select(b => b.UserID).Should().Equal(this.bob);
        this.Buddies.GetAllFriends(this.bob).Select(b => b.UserID).Should().Equal(this.alice);
    }

    [Test]
    [Description("Denying removes the request; deny all only removes requests older than 14 days")]
    public void Friends_Deny_Test()
    {
        var friends = this.board.Resolve<IFriends>();

        using (var connection = this.db.Open())
        {
            connection.Insert(new Buddy { FromUserID = this.bob, ToUserID = this.alice, Requested = DateTime.UtcNow.AddDays(-20) });
            connection.Insert(new Buddy { FromUserID = this.carol, ToUserID = this.alice, Requested = DateTime.UtcNow.AddDays(-1) });
        }

        friends.DenyAllRequests();

        this.Buddies.GetReceivedRequests(this.alice).Select(b => b.UserID).Should().Equal(this.carol);

        friends.DenyRequest(this.carol);

        this.Buddies.GetReceivedRequests(this.alice).Should().BeEmpty();
    }

    [Test]
    [Description("Approve all approves every received request")]
    public void Friends_Approve_All_Test()
    {
        using (var connection = this.db.Open())
        {
            connection.Insert(new Buddy { FromUserID = this.bob, ToUserID = this.alice, Requested = DateTime.UtcNow });
            connection.Insert(new Buddy { FromUserID = this.carol, ToUserID = this.alice, Requested = DateTime.UtcNow });
        }

        this.board.Resolve<IFriends>().ApproveAllRequests();

        this.Buddies.GetAllFriends(this.alice).Select(b => b.UserID).Should().BeEquivalentTo([this.bob, this.carol]);

        // Carol has the activity stream disabled
        this.Repository<Activity>().Count(a => a.UserID == this.carol).Should().Be(0);
    }

    [Test]
    [Description("Removing a friend deletes the friendship from the current user's side")]
    public void Friends_Remove_Test()
    {
        var friends = this.board.Resolve<IFriends>();
        friends.AddRequest(this.bob);
        this.LoginAs(this.bob);
        friends.ApproveRequest(this.Buddies.GetReceivedRequests(this.bob).Single());

        this.LoginAs(this.alice);
        friends.Remove(this.bob).Should().Be($"User {this.bob}");

        this.Buddies.GetAllFriends(this.alice).Should().BeEmpty();
        friends.IsBuddy(this.bob).Should().BeFalse();
        friends.IsBuddy(this.alice).Should().BeTrue("you are always your own buddy");
    }

    [Test]
    [Description("Activity stream entries get the right flags, notification state and events")]
    public void ActivityStream_Test()
    {
        var stream = this.board.Resolve<IActivityStream>();
        var aliceUser = this.Repository<User>().GetById(this.alice);

        this.board.Cache[string.Format(Constants.Cache.ActiveUserLazyData, this.alice)] = "cached";

        stream.AddTopicToStream(aliceUser, this.topic, this.message, "Topic", "text");
        stream.AddReplyToStream(aliceUser, this.topic, this.message, "Topic", "text");
        stream.AddWatchTopicToStream(aliceUser, this.topic, this.message, "Topic", "text", this.bob);
        stream.AddWatchReplyToStream(aliceUser, this.topic, this.message, "Topic", "text", this.bob);
        stream.AddMentionToStream(this.alice, this.topic, this.message, this.bob);
        stream.AddQuotingToStream(this.alice, this.topic, this.message, this.bob);
        stream.AddThanksReceivedToStream(this.alice, this.topic, this.message, this.bob);
        stream.AddThanksGivenToStream(this.bob, this.topic, this.message, this.alice);
        stream.AddBecomeFriendsToStream(this.alice, this.bob);

        var activities = this.Repository<Activity>().GetAll().ToDictionary(a => new ActivityFlags(a.Flags).BitValue);

        activities[new ActivityFlags { CreatedTopic = true }.BitValue].Notification.Should().BeFalse();
        activities[new ActivityFlags { CreatedReply = true }.BitValue].Notification.Should().BeFalse();
        activities[new ActivityFlags { WatchForumReply = true }.BitValue].Notification.Should().BeTrue();
        activities[new ActivityFlags { WatchTopicReply = true }.BitValue].FromUserID.Should().Be(this.bob);
        activities[new ActivityFlags { WasMentioned = true }.BitValue].Notification.Should().BeTrue();
        activities[new ActivityFlags { WasQuoted = true }.BitValue].Notification.Should().BeTrue();
        activities[new ActivityFlags { ReceivedThanks = true }.BitValue].UserID.Should().Be(this.alice);
        activities[new ActivityFlags { GivenThanks = true }.BitValue].UserID.Should().Be(this.bob);
        activities[new ActivityFlags { BecomeFriends = true }.BitValue].MessageID.Should().BeNull();

        this.board.Events.OfType<ActivityNotificationEvent>().Should().HaveCount(7);
        this.board.Cache[string.Format(Constants.Cache.ActiveUserLazyData, this.alice)].Should()
            .BeNull("notifications clear the cached lazy data");
    }

    [Test]
    [Description("The notification counters see the activity stream entries")]
    public async System.Threading.Tasks.Task ActivityStream_LazyData_Counters_Test()
    {
        var stream = this.board.Resolve<IActivityStream>();

        stream.AddMentionToStream(this.alice, this.topic, this.message, this.bob);
        stream.AddQuotingToStream(this.alice, this.topic, this.message, this.bob);
        stream.AddQuotingToStream(this.alice, this.topic, this.message, this.carol);
        stream.AddThanksReceivedToStream(this.alice, this.topic, this.message, this.bob);

        var data = await this.Repository<User>().LazyDataAsync(this.alice, TestDatabase.BoardId, false, false, false);

        data.Mention.Should().Be(1);
        data.Quoted.Should().Be(2);
        data.ReceivedThanks.Should().Be(1);
    }

    private IRepository<Buddy> Buddies => this.board.Current.GetRepository<Buddy>();

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }

    private void LoginAs(int userId)
    {
        var user = this.Repository<User>().GetById(userId);

        this.board.PageUser.ID = user.ID;
        this.board.PageUser.Name = user.Name;
        this.board.PageUser.DisplayName = user.DisplayName;
        this.board.PageUser.Activity = user.Activity;
    }
}
