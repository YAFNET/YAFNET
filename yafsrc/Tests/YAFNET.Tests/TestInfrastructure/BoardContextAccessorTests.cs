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

namespace YAF.Tests.TestInfrastructure;

using YAF.Types.Constants;

/// <summary>
/// The page data accessors of the board context (access flags, user counters, suspension, page values).
/// </summary>
[TestFixture]
[NonParallelizable]
public class BoardContextAccessorTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Forum access flags come from the page load data")]
    public void Access_Flags_Test()
    {
        var load = this.board.PageLoad;
        load.PollAccess = true;
        load.PostAccess = true;
        load.PriorityAccess = true;
        load.VoteAccess = true;
        load.IsCrawler = true;
        load.IsModeratorAny = true;

        var current = this.board.Current;

        current.ForumPollAccess.Should().BeTrue();
        current.ForumPostAccess.Should().BeTrue();
        current.ForumPriorityAccess.Should().BeTrue();
        current.ForumVoteAccess.Should().BeTrue();
        current.IsCrawler.Should().BeTrue();
        current.IsModeratorInAnyForum.Should().BeTrue();
    }

    [Test]
    [Description("User counters and suspension come from the lazy user data")]
    public void User_Data_Test()
    {
        var user = this.board.User;
        user.PendingBuddies = 2;
        user.NumAlbums = 3;
        user.WatchTopic = 4;
        user.ReceivedThanks = 5;
        user.Mention = 6;
        user.Quoted = 7;
        user.UnreadPrivate = 8;
        user.ModeratePosts = 9;
        user.UserHasBuddies = true;
        user.UserHasPrivateConversations = true;
        user.LastPendingBuddies = new DateTime(2026, 1, 1);

        var current = this.board.Current;

        (current.PendingBuddies, current.NumAlbums, current.WatchTopic, current.ReceivedThanks, current.Mention,
                current.Quoted, current.UnreadPrivate, current.ModeratePosts)
            .Should().Be((2, 3, 4, 5, 6, 7, 8, 9));
        current.UserHasBuddies.Should().BeTrue();
        current.UserHasPrivateConversations.Should().BeTrue();
        current.LastPendingBuddies.Should().Be(new DateTime(2026, 1, 1));

        current.IsSuspended.Should().BeFalse();
        current.SuspendedReason.Should().BeEmpty();

        var until = DateTime.UtcNow.AddDays(3);
        user.Suspended = until;
        user.SuspendedReason = "spam";

        current.IsSuspended.Should().BeTrue();
        current.SuspendedUntil.Should().Be(until);
        current.SuspendedReason.Should().Be("spam");
    }

    [Test]
    [Description("The page index is zero based, page values and the variable store")]
    public void Page_Values_Test()
    {
        var current = this.board.Current;

        current.PageIndex.Should().Be(0);
        this.board.Query.PageIndex = 3;
        current.PageIndex.Should().Be(2, "the query p=3 is the third page");

        this.board.Query.CategoryID = 7;
        current.PageCategoryID.Should().Be(7);
        current.PageCategory.Should().NotBeNull();
        current.PageMessage.Should().NotBeNull();

        current["key"].Should().BeNull();
        current["key"] = "value";
        current["key"].Should().Be("value");
    }

    [Test]
    [Description("Session notifications are stored for the next page")]
    public void Session_Notify_Test()
    {
        this.board.Current.SessionNotify("saved", MessageTypes.success);

        this.board.Current.SessionMessageService.Should().NotBeNull();
    }
}
