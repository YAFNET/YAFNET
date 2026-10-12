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

namespace YAF.Tests.CoreTests.Extensions;

using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// Computed members of the <see cref="User"/> and <see cref="Message"/> models.
/// </summary>
[TestFixture]
public class UserMessageModelTests
{
    [Test]
    [Description("New users start with one point, the default page size and activity notifications")]
    public void User_Defaults_Test()
    {
        var user = new User();

        user.Points.Should().Be(1);
        user.PageSize.Should().Be(5);
        user.Activity.Should().BeTrue();
    }

    [Test]
    [Description("Time zone ids are resolved, legacy offsets and unknown ids fall back to the server time zone")]
    [TestCase("Tokyo Standard Time", "Tokyo Standard Time")]
    [TestCase("60", null)]
    [TestCase("-120", null)]
    [TestCase("Not a time zone", null)]
    [TestCase(null, null)]
    public void User_Time_Zone_Test(string timeZone, string expectedId)
    {
        var user = new User { TimeZone = timeZone };

        user.TimeZoneInfo.Id.Should().Be(expectedId ?? TimeZoneInfo.Local.Id);
    }

    [Test]
    [Description("The notification type maps to the notification setting, missing means the default")]
    public void User_Notification_Setting_Test()
    {
        new User().NotificationSetting.Should().Be((UserNotificationSetting)0);
        new User { NotificationType = (int)UserNotificationSetting.TopicsIPostToOrSubscribeTo }.NotificationSetting.Should()
            .Be(UserNotificationSetting.TopicsIPostToOrSubscribeTo);
    }

    [Test]
    [Description("The user and block flags are stored in their bit values")]
    public void User_Flags_Test()
    {
        var user = new User
        {
            UserFlags = new UserFlags { IsApproved = true, IsGuest = true },
            Block = new UserBlockFlags { BlockPMs = true, BlockEmails = true }
        };

        user.Flags.Should().Be((int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));
        user.BlockFlags.Should().Be((int)(UserBlockFlags.Flags.BlockPMs | UserBlockFlags.Flags.BlockEmails));
        user.Block.BlockFriendRequests.Should().BeFalse();
        user.UserFlags.IsApproved.Should().BeTrue();
    }

    [Test]
    [Description("A message can be created from a paged message row")]
    public void Message_From_Paged_Message_Test()
    {
        var posted = new DateTime(2026, 1, 2, 3, 4, 5);
        var edited = posted.AddHours(1);

        var row = new PagedMessage
        {
            MessageID = 7,
            UserID = 3,
            UserName = "alice",
            Message = "text",
            TopicID = 5,
            Topic = "Topic",
            Posted = posted,
            Flags = (int)(MessageFlags.Flags.IsApproved | MessageFlags.Flags.IsBBCode),
            Edited = edited,
            EditReason = "typo",
            Position = 4,
            IsModeratorChanged = true,
            DeleteReason = "spam",
            IP = "10.0.0.1",
            ExternalMessageId = "ext",
            AnswerMessageId = 9,
            Signature = "sig"
        };

        var message = new Message(row);

        message.ID.Should().Be(7);
        message.UserID.Should().Be(3);
        message.UserName.Should().Be("alice");
        message.MessageText.Should().Be("text");
        message.TopicID.Should().Be(5);
        message.TopicName.Should().Be("Topic");
        message.Posted.Should().Be(posted);
        message.Edited.Should().Be(edited);
        message.EditReason.Should().Be("typo");
        message.Position.Should().Be(4);
        message.IsModeratorChanged.Should().BeTrue();
        message.DeleteReason.Should().Be("spam");
        message.IP.Should().Be("10.0.0.1");
        message.ExternalMessageId.Should().Be("ext");
        message.AnswerMessageId.Should().Be(9);
        message.Signature.Should().Be("sig");

        message.MessageFlags.IsApproved.Should().BeTrue();
        message.MessageFlags.IsBBCode.Should().BeTrue();
        message.MessageFlags = new MessageFlags(0) { IsDeleted = true };
        message.Flags.Should().Be((int)MessageFlags.Flags.IsDeleted);
    }
}
