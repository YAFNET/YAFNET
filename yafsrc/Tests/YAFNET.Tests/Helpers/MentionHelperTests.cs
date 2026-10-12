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

namespace YAF.Tests.Helpers;

using Autofac;

using NSubstitute;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// Tests for MentionHelper
/// </summary>
[TestFixture]
[NonParallelizable]
public class MentionHelperTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int carol;

    private int forum;

    private int topic;

    private int message;

    [SetUp]
    public void SetUp()
    {
        var displayName = Substitute.For<IUserDisplayName>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<ActivityStream>().As<IActivityStream>();
                b.RegisterInstance(displayName).As<IUserDisplayName>();
            },
            useDatabase: true);

        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.carol = this.db.AddUser("Carol");

        this.forum = this.db.AddForum();
        this.db.AddForumAccess(this.alice, this.forum);
        this.db.AddForumAccess(this.bob, this.forum);

        this.topic = this.db.AddTopic(this.forum, this.alice);
        this.message = this.db.AddMessage(this.topic, this.alice);

        var users = this.board.Current.GetRepository<User>();

        displayName.FindUserByName("Alice").Returns(_ => users.GetById(this.alice));
        displayName.FindUserByName("Bob").Returns(_ => users.GetById(this.bob));
        displayName.FindUserByName("Carol").Returns(_ => users.GetById(this.carol));
        displayName.FindUserByName("Nobody").Returns((User)null);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Mentioned and quoted users who can read the forum are notified, unknown names, the author and users without access are skipped")]
    public void NotifyMentionedAndQuotedUsers_Test()
    {
        var text = "@[userlink]Bob[/userlink] @[userlink]Nobody[/userlink] @[userlink]Carol[/userlink] @[userlink]Alice[/userlink] "
                   + "[quote=Bob;1]a quote[/quote] [quote=Nobody;2]an old quote[/quote]";

        MentionHelper.NotifyMentionedAndQuotedUsers(text, this.forum, this.topic, this.message, this.alice);

        var activities = this.board.Current.GetRepository<Activity>().Get(a => a.MessageID == this.message);

        activities.Should().OnlyContain(a => a.UserID == this.bob && a.FromUserID == this.alice);
        activities.Should().ContainSingle(a => new ActivityFlags(a.Flags).WasMentioned);
        activities.Should().ContainSingle(a => new ActivityFlags(a.Flags).WasQuoted);
    }
}
