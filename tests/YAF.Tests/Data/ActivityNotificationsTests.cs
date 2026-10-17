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
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Models;

/// <summary>
/// The notifications of a user (<see cref="ActivityRepositoryExtensions.Notifications"/>).
/// </summary>
[TestFixture]
[NonParallelizable]
public class ActivityNotificationsTests
{
    private TestBoardContext board;

    private TestDatabase db;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The notifications only show topics the user can (still) read: not of hidden forums and not deleted")]
    public void Notifications_Only_Readable_Topics_Test()
    {
        var alice = this.db.AddUser("Alice");
        var bob = this.db.AddUser("Bob");

        var readable = this.db.AddForum("Readable");
        var hidden = this.db.AddForum("Hidden");

        this.db.AddForumAccess(alice, readable);

        var visibleTopic = this.db.AddTopic(readable, bob, "Visible");
        var hiddenTopic = this.db.AddTopic(hidden, bob, "Moved to a staff forum");
        var deletedTopic = this.db.AddTopic(
            readable,
            bob,
            "Deleted",
            t => t.Flags = new TopicFlags { IsDeleted = true }.BitValue);

        var activities = this.board.Current.GetRepository<Activity>();

        foreach (var topic in new[] { visibleTopic, hiddenTopic, deletedTopic })
        {
            activities.Insert(
                new Activity
                    {
                        UserID = alice,
                        FromUserID = bob,
                        TopicID = topic,
                        MessageID = this.db.AddMessage(topic, bob),
                        Flags = new ActivityFlags { WasMentioned = true }.BitValue,
                        Created = DateTime.UtcNow,
                        Notification = true
                    });
        }

        activities.Notifications(alice).Select(n => n.Item3.TopicName).Should().Equal("Visible");
    }
}
