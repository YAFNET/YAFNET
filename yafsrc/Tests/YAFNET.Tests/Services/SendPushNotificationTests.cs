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

using System.Text.RegularExpressions;

using Autofac;

using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The text of the push notifications of new topics and posts.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SendPushNotificationTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        var displayName = Substitute.For<IUserDisplayName>();
        displayName.GetNameById(7).Returns("Alice");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(displayName).As<IUserDisplayName>();
                b.Register(c => new BadWordReplace(this.board.Cache, NullLogger<BadWordReplace>.Instance, c.Resolve<IServiceLocator>()))
                    .As<IBadWordReplace>();
            });

        this.board.Localization
            .Add("DEFAULT", "WATCH_TOPIC_MSG", "{0} replied in {1}")
            .Add("DEFAULT", "WATCH_FORUM_MSG", "{0} started {1}");

        this.board.Cache[Constants.Cache.ReplaceWords] = new List<BadWordReplaceItem>
        {
            new("****", "darn", RegexOptions.IgnoreCase)
        };
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The notification is text: the stored (html encoded) topic name is decoded and the bad words are replaced")]
    public void Topic_Notification_Text_Test()
    {
        var message = new Message { UserID = 7, Topic = new Topic { TopicName = "Tom &amp; Jerry &lt;3 darn" } };

        var push = new SendPushNotification(this.board.Resolve<IServiceLocator>());

        push.GetTopicNotificationText(message, false).Should().Be("Alice replied in Tom & Jerry <3 ****");
        push.GetTopicNotificationText(message, true).Should().Be("Alice started Tom & Jerry <3 ****");
    }
}
