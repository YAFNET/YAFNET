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

namespace YAF.Tests.Services;

using System.Globalization;
using System.Threading.Tasks;

using Autofac;

using YAF.Core.Events;
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
/// The achievements: the goals, the awards, the dialog and the achievements a user shows.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AchievementServiceTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private IAchievementService service;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<AchievementService>().As<IAchievementService>();
                b.RegisterType<ActivityStream>().As<IActivityStream>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.service = this.board.Resolve<IAchievementService>();

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");

        this.board.Localization.Add("ACHIEVEMENT_DEFAULTS", "POSTS_1_NAME", "First Words")
            .Add("ACHIEVEMENT_DEFAULTS", "POSTS_1_DESC", "Write your first post.")
            .Add("ACHIEVEMENTS", "GOAL_POSTS", "{0} posts")
            .Add("ACHIEVEMENTS", "GOAL_REACTIONSRECEIVED", "{0} reactions received")
            .Add("ACHIEVEMENTS", "GOAL_AND", "{0} and {1}");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("An achievement is awarded once when the goal is reached, the user is told about it")]
    public async Task Achievement_Is_Awarded_When_The_Goal_Is_Reached_Test()
    {
        var achievement = this.AddAchievement(AchievementType.Posts, 3);

        this.SetPosts(this.alice, 2);
        (await this.service.CheckUserAsync(this.alice)).Should().BeEmpty();

        this.SetPosts(this.alice, 3);
        (await this.service.CheckUserAsync(this.alice)).Select(a => a.ID).Should().Equal(achievement);

        // a second check doesn't award it again
        (await this.service.CheckUserAsync(this.alice)).Should().BeEmpty();

        (await this.Repository<UserAchievement>().CountAsync(u => u.UserID == this.alice)).Should().Be(1);

        var unlocked = this.board.Events.OfType<AchievementUnlockedEvent>().Should().ContainSingle().Subject;
        unlocked.UserName.Should().Be("Alice");
        unlocked.AchievementIds.Should().Equal(achievement);
    }

    [Test]
    [Description("An unlocked achievement is a notification, it is read when the user has seen the dialog")]
    public async Task Notification_Test()
    {
        var achievement = this.AddAchievement(AchievementType.Posts, 1);

        this.SetPosts(this.alice, 1);
        await this.service.CheckUserAsync(this.alice);

        var activity = await this.Repository<Activity>().GetSingleAsync(a => a.UserID == this.alice);
        activity.ActivityFlags.UnlockedAchievement.Should().BeTrue();
        activity.AchievementID.Should().Be(achievement);
        activity.Notification.Should().BeTrue();
        activity.FromUserID.Should().BeNull();

        this.board.Events.OfType<ActivityNotificationEvent>().Should().ContainSingle(e => e.UserName == "Alice");

        // the notification list has it (no other user), the timeline of the account not
        this.Repository<Activity>().Notifications(this.alice).Select(n => n.Item1.AchievementID).Should().Equal(achievement);
        this.Repository<Activity>().Timeline(this.alice).Should().BeEmpty();

        await this.service.MarkSeenAsync(this.alice, achievement);

        (await this.Repository<Activity>().GetSingleAsync(a => a.UserID == this.alice)).Notification.Should().BeFalse();

        // a deleted achievement is not listed
        await this.Repository<Achievement>().DeleteAsync(a => a.ID == achievement);
        this.Repository<Activity>().Notifications(this.alice).Should().BeEmpty();
    }

    [Test]
    [Description("An achievement with two goals needs both")]
    public async Task Both_Goals_Are_Needed_Test()
    {
        var achievement = this.AddAchievement(
            AchievementType.Posts,
            2,
            a =>
            {
                a.ExtraType = (int)AchievementType.ReactionsReceived;
                a.ExtraThreshold = 1;
            });

        this.SetPosts(this.alice, 5);
        (await this.service.CheckUserAsync(this.alice)).Should().BeEmpty();

        var message = this.db.AddMessage(this.db.AddTopic(this.db.AddForum(), this.alice), this.alice);
        this.Repository<Thanks>().AddMessageThanks(this.bob, this.alice, message);

        (await this.service.CheckUserAsync(this.alice)).Select(a => a.ID).Should().Equal(achievement);
    }

    [Test]
    [Description("A published post checks the achievements of its author")]
    public void Published_Post_Awards_The_Author_Test()
    {
        var achievement = this.AddAchievement(AchievementType.Posts, 1);

        var topic = this.db.AddTopic(this.db.AddForum(), this.alice);
        var message = this.db.AddMessage(topic, this.alice);
        this.SetPosts(this.alice, 1);

        new PostPublishedAchievements(this.board.Current.ServiceLocator).Handle(new PostPublishedEvent(1, topic, message, true));

        this.service.GetUserAchievements(this.alice).Select(u => u.AchievementID).Should().Equal(achievement);
    }

    [Test]
    [Description("The topics, the reactions received and given are counted")]
    public async Task User_Values_Test()
    {
        var forum = this.db.AddForum();
        var topic = this.db.AddTopic(forum, this.alice, configure: t => t.NumPosts = 1);
        var message = this.db.AddMessage(topic, this.alice);

        // not counted: a deleted topic and a topic that is not approved yet (no posts)
        this.db.AddTopic(forum, this.alice, configure: t => { t.NumPosts = 1; t.Flags = 8; });
        this.db.AddTopic(forum, this.alice, configure: t => t.NumPosts = 0);

        this.Repository<Thanks>().AddMessageThanks(this.bob, this.alice, message);

        var values = await this.service.GetUserValuesAsync(
                         this.alice,
                         [AchievementType.Topics, AchievementType.ReactionsReceived, AchievementType.ReactionsGiven, AchievementType.MembershipDays]);

        values[AchievementType.Topics].Should().Be(1);
        values[AchievementType.ReactionsReceived].Should().Be(1);
        values[AchievementType.ReactionsGiven].Should().Be(0);
        values[AchievementType.MembershipDays].Should().Be(0);

        (await this.service.GetUserValuesAsync(this.bob, [AchievementType.ReactionsGiven]))[AchievementType.ReactionsGiven]
            .Should().Be(1);
    }

    [Test]
    [Description("Inactive achievements, guests and users that are not approved get nothing, nor if the achievements are off")]
    public async Task No_Award_Test()
    {
        this.AddAchievement(AchievementType.Posts, 1, a => a.IsActive = false);

        this.SetPosts(this.alice, 10);
        (await this.service.CheckUserAsync(this.alice)).Should().BeEmpty();

        this.AddAchievement(AchievementType.Posts, 1);
        this.service.ClearBoardCache(TestDatabase.BoardId);

        var guest = this.db.AddUser("Guest", u => { u.Flags = (int)(UserFlags.Flags.IsGuest | UserFlags.Flags.IsApproved); u.NumPosts = 10; });
        var unapproved = this.db.AddUser("New", u => { u.Flags = 0; u.NumPosts = 10; });

        (await this.service.CheckUserAsync(guest)).Should().BeEmpty();
        (await this.service.CheckUserAsync(unapproved)).Should().BeEmpty();

        this.board.BoardSettings.EnableAchievements = false;
        (await this.service.CheckUserAsync(this.alice)).Should().BeEmpty();

        this.board.BoardSettings.EnableAchievements = true;
        (await this.service.CheckUserAsync(this.alice)).Should().ContainSingle();
    }

    [Test]
    [Description("The unseen achievements are listed for the dialog until they are marked as seen")]
    public async Task Unseen_Test()
    {
        var common = this.AddAchievement(AchievementType.Posts, 1, a => a.Rarity = (int)AchievementRarity.Common);
        var epic = this.AddAchievement(AchievementType.Posts, 2, a => a.Rarity = (int)AchievementRarity.Epic);

        this.SetPosts(this.alice, 2);
        await this.service.CheckUserAsync(this.alice);

        this.service.HasUnseen(this.alice).Should().BeTrue();

        // the most valuable first
        this.service.GetUnseen(this.alice).Select(a => a.Id).Should().Equal(epic, common);

        await this.service.MarkSeenAsync(this.alice, epic);
        await this.service.MarkSeenAsync(this.alice, common);

        this.service.HasUnseen(this.alice).Should().BeFalse();
        this.service.GetUnseen(this.alice).Should().BeEmpty();
    }

    [Test]
    [Description("A user shows at most three unlocked achievements")]
    public async Task Featured_Test()
    {
        var ids = Enumerable.Range(1, 4).Select(i => this.AddAchievement(AchievementType.Posts, i)).ToList();
        var locked = this.AddAchievement(AchievementType.Posts, 100);

        this.SetPosts(this.alice, 4);
        await this.service.CheckUserAsync(this.alice);

        (await this.service.SetFeaturedAsync(this.alice, locked, true)).Should().BeFalse("the achievement is not unlocked");

        (await this.service.SetFeaturedAsync(this.alice, ids[0], true)).Should().BeTrue();
        (await this.service.SetFeaturedAsync(this.alice, ids[1], true)).Should().BeTrue();
        (await this.service.SetFeaturedAsync(this.alice, ids[2], true)).Should().BeTrue();
        (await this.service.SetFeaturedAsync(this.alice, ids[3], true)).Should().BeFalse("three are shown already");

        // a shown one can be switched on again (no change) and off
        (await this.service.SetFeaturedAsync(this.alice, ids[0], true)).Should().BeTrue();
        (await this.service.SetFeaturedAsync(this.alice, ids[0], false)).Should().BeTrue();
        (await this.service.SetFeaturedAsync(this.alice, ids[3], true)).Should().BeTrue();

        this.service.GetFeatured(this.alice).Select(a => a.Id).Should().BeEquivalentTo([ids[1], ids[2], ids[3]]);
    }

    [Test]
    [Description("A default achievement has the localized texts until the administrator changes them")]
    public void Default_Texts_Test()
    {
        var achievement = new Achievement { TextKey = "POSTS_1", Type = (int)AchievementType.Posts, Threshold = 1 };

        this.service.GetName(achievement).Should().Be("First Words");
        this.service.GetDescription(achievement).Should().Be("Write your first post.");

        achievement.Name = "My name";
        achievement.Description = "My description";

        this.service.GetName(achievement).Should().Be("My name");
        this.service.GetDescription(achievement).Should().Be("My description");
    }

    [Test]
    [Description("The goals are formatted with the culture, two goals are joined")]
    public void Goal_Text_Test()
    {
        this.board.Localization.Culture = CultureInfo.GetCultureInfo("en-US");

        this.service.GetGoalText(new Achievement { Type = (int)AchievementType.Posts, Threshold = 1000 })
            .Should().Be("1,000 posts");

        this.service.GetGoalText(
                new Achievement
                {
                    Type = (int)AchievementType.Posts,
                    Threshold = 5,
                    ExtraType = (int)AchievementType.ReactionsReceived,
                    ExtraThreshold = 10
                })
            .Should().Be("5 posts and 10 reactions received");
    }

    [Test]
    [Description("The share of the members that have an achievement")]
    public void Percent_Test()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");

        AchievementService.FormatPercent(0, 100, culture).Should().Be("0%");
        AchievementService.FormatPercent(1, 0, culture).Should().Be("0%");
        AchievementService.FormatPercent(1, 2000, culture).Should().Be("<0.1%");
        AchievementService.FormatPercent(62, 1000, culture).Should().Be("6.2%");
        AchievementService.FormatPercent(5, 5, culture).Should().Be("100%");
        AchievementService.FormatPercent(1, 3, CultureInfo.GetCultureInfo("de-DE")).Should().Be("33,3%");
    }

    [Test]
    [Description("The infos have the progress of the locked achievements, a reached goal is not shown as done")]
    public async Task Progress_Test()
    {
        this.AddAchievement(AchievementType.Posts, 10);
        var unlocked = this.AddAchievement(AchievementType.Posts, 1);

        this.SetPosts(this.alice, 1);
        await this.service.CheckUserAsync(this.alice);

        this.SetPosts(this.alice, 4);

        var infos = await this.service.GetAchievementInfosAsync(TestDatabase.BoardId, this.alice, true);

        var done = infos.Single(i => i.Id == unlocked);
        done.Unlocked.Should().BeTrue();
        done.Progress.Should().Be(100);

        var open = infos.Single(i => i.Id != unlocked);
        open.Unlocked.Should().BeFalse();
        open.Progress.Should().Be(40);
        open.ProgressText.Should().Be("4 / 10");

        // a goal reached since the last check
        this.SetPosts(this.alice, 50);

        (await this.service.GetAchievementInfosAsync(TestDatabase.BoardId, this.alice, true))
            .Single(i => i.Id != unlocked).Progress.Should().Be(99);
    }

    [Test]
    [Description("All members of the board are checked, the points of the user are the sum of the achievements")]
    public async Task Check_Board_Test()
    {
        this.AddAchievement(AchievementType.Posts, 1);

        this.SetPosts(this.alice, 1);
        this.SetPosts(this.bob, 1);

        (await this.service.CheckBoardAsync(TestDatabase.BoardId)).Should().Be(2);
        (await this.service.CheckBoardAsync(TestDatabase.BoardId)).Should().Be(0);

        this.service.GetUserPoints(this.alice).Should().Be(5);
    }

    [Test]
    [Description("The default achievements have a text key, valid icons and texts in the language file")]
    public void Default_Achievements_Test()
    {
        var defaults = AchievementDefaults.Create(TestDatabase.BoardId);

        defaults.Should().NotBeEmpty();
        defaults.Select(a => a.TextKey).Should().OnlyHaveUniqueItems();
        defaults.Should().OnlyContain(a => AchievementIcons.IsValid(a.Icon) && a.IsActive && a.Threshold > 0);
        defaults.Should().OnlyContain(a => Enum.IsDefined(typeof(AchievementType), a.Type));
    }

    private int AddAchievement(AchievementType type, int threshold, Action<Achievement> configure = null)
    {
        var achievement = new Achievement
        {
            BoardID = TestDatabase.BoardId,
            Name = $"{type} {threshold}",
            Icon = "trophy",
            Type = (int)type,
            Threshold = threshold,
            Points = 5,
            IsActive = true
        };

        configure?.Invoke(achievement);

        var id = this.Repository<Achievement>().Insert(achievement);

        this.service.ClearBoardCache(TestDatabase.BoardId);

        return id;
    }

    private void SetPosts(int userId, int posts)
    {
        this.Repository<User>().UpdateOnly(() => new User { NumPosts = posts }, u => u.ID == userId);
    }

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }
}
