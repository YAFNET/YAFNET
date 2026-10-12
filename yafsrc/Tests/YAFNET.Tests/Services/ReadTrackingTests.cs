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

using Autofac;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects.Model;
using YAF.Types.Objects;

/// <summary>
/// Read tracking of the current user: in the database for members, in the session for guests
/// (or when database read tracking is off), and the <see cref="SessionService"/> it uses.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ReadTrackingTests
{
    private TestBoardContext board;

    private IReadTrackCurrentUser readTrack;

    private ISessionService session;

    private int alice;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<ReadTrackCurrentUser>().As<IReadTrackCurrentUser>();
                b.RegisterType<SessionService>().As<ISessionService>();
            },
            useDatabase: true);

        this.alice = this.board.Database.AddUser("Alice");
        this.board.LoginAs(this.alice);

        this.readTrack = this.board.Resolve<IReadTrackCurrentUser>();
        this.session = this.board.Resolve<ISessionService>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Members: forum and topic read times are stored in the database")]
    public void Database_Forum_Topic_Read_Test()
    {
        this.readTrack.GetForumRead(5, null).Should().Be(DateTimeHelper.SqlDbMinTime(), "never read");

        this.readTrack.SetForumRead(5);
        this.readTrack.SetTopicRead(7);

        this.readTrack.GetForumRead(5, null).Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        this.readTrack.GetTopicRead(7, null).Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        var overrideTime = new DateTime(2020, 1, 1);
        this.readTrack.GetForumRead(5, overrideTime).Should().Be(overrideTime);
        this.readTrack.GetTopicRead(7, overrideTime).Should().Be(overrideTime);

        this.Repository<ForumReadTracking>().Count(x => x.UserID == this.alice).Should().Be(1);
        this.Repository<TopicReadTracking>().Count(x => x.UserID == this.alice).Should().Be(1);
    }

    [Test]
    [Description("Reading again updates the stored time instead of adding a row")]
    public void Database_Read_Again_Test()
    {
        this.Insert(new TopicReadTracking { UserID = this.alice, TopicID = 7, LastAccessDate = DateTime.UtcNow.AddDays(-1) });

        this.readTrack.SetTopicRead(7);

        this.Repository<TopicReadTracking>().Get(x => x.UserID == this.alice).Should().ContainSingle()
            .Which.LastAccessDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Test]
    [Description("LastRead is the newest forum or topic read of the member")]
    public void Database_LastRead_Test()
    {
        this.readTrack.LastRead.Should().Be(DateTimeHelper.SqlDbMinTime());

        var forumRead = DateTime.UtcNow.AddDays(-2);
        var topicRead = DateTime.UtcNow.AddDays(-1);

        this.Insert(new ForumReadTracking { UserID = this.alice, ForumID = 5, LastAccessDate = forumRead });
        this.readTrack.LastRead.Should().BeCloseTo(forumRead, TimeSpan.FromSeconds(1));

        this.Insert(new TopicReadTracking { UserID = this.alice, TopicID = 7, LastAccessDate = topicRead });
        this.readTrack.LastRead.Should().BeCloseTo(topicRead, TimeSpan.FromSeconds(1), "the topic was read after the forum");
    }

    [Test]
    [Description("Guests: read times are kept in the session")]
    public void Session_Forum_Topic_Read_Test()
    {
        this.board.User.IsGuest = true;

        var lastVisit = DateTime.UtcNow.AddDays(-3);
        this.session.LastVisit = lastVisit;

        this.readTrack.GetForumRead(5, null).Should().BeCloseTo(lastVisit, TimeSpan.FromSeconds(1), "unread forums fall back to the last visit");

        this.readTrack.SetForumRead(5);
        this.readTrack.SetTopicRead(7);

        this.readTrack.GetForumRead(5, null).Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        this.readTrack.GetTopicRead(7, null).Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        this.readTrack.GetTopicRead(8, null).Should().BeCloseTo(lastVisit, TimeSpan.FromSeconds(1));

        this.Repository<ForumReadTracking>().Count(_ => true).Should().Be(0, "guests are not tracked in the database");
    }

    [Test]
    [Description("SessionService: forum and topic read times survive the session (json) round trip")]
    public void SessionService_Read_Times_Test()
    {
        var date = new DateTime(2026, 5, 4, 3, 2, 1, DateTimeKind.Utc);

        this.session.SetForumRead(5, date);
        this.session.SetTopicRead(7, date);
        this.session.SetForumRead(6, date.AddDays(1));

        this.session.GetForumRead(5).Should().Be(date);
        this.session.GetForumRead(6).Should().Be(date.AddDays(1));
        this.session.GetTopicRead(7).Should().Be(date);
    }

    [Test]
    [Description("SessionService: unread items fall back to the last visit, without one to the minimum date")]
    public void SessionService_Unread_Test()
    {
        this.session.GetForumRead(5).Should().Be(DateTimeHelper.SqlDbMinTime());

        var lastVisit = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        this.session.LastVisit = lastVisit;

        this.session.GetForumRead(5).Should().Be(lastVisit);
        this.session.GetTopicRead(7).Should().Be(lastVisit);

        this.session.LastVisit = DateTimeHelper.SqlDbMinTime();
        this.session.LastVisit.Should().BeNull("setting the minimum date clears the last visit");
    }

    [Test]
    [Description("SessionService: simple values and page data per user")]
    public void SessionService_Values_Test()
    {
        this.session.BoardForumsIndex = 3;
        this.session.InfoMessage = "hello";
        this.session.MultiQuoteIds = [new MultiQuote { MessageID = 1, TopicID = 2 }];
        this.session.LastPendingBuddies.Should().Be(DateTimeHelper.SqlDbMinTime());

        var pending = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc);
        this.session.LastPendingBuddies = pending;

        this.session.BoardForumsIndex.Should().Be(3);
        this.session.InfoMessage.Should().Be("hello");
        this.session.MultiQuoteIds.Should().ContainSingle().Which.MessageID.Should().Be(1);
        this.session.LastPendingBuddies.Should().Be(pending);

        this.session.SetPageData("data for alice");
        this.session.GetPageData<string>().Should().Be("data for alice");

        this.session.ClearPageData();
        this.session.GetPageData<string>().Should().BeNull();
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }

    private void Insert<T>(T entity)
    {
        using var connection = this.board.Database.Open();
        connection.Insert(entity);
    }
}
