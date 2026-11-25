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


using YAF.UI.Chat;

namespace YAF.Tests.Chat;

/// <summary>
/// Tests of the rules of the chat (YAFNET.UI.Chat, the file is linked into this project).
/// </summary>
[TestFixture]
public class ChatRulesTests
{
    /// <summary>
    /// Only members who are not suspended can send messages: the hub can be called
    /// directly, a guest message also broke the chat page (the guest has no ProviderUserKey).
    /// </summary>
    [Test]
    [Description("Only members who are not suspended can send messages.")]
    public void Only_Members_Not_Suspended_Can_Send_Messages_Test()
    {
        ChatRules.CanSendMessages(isGuest: false, isSuspendedNow: false).Should().BeTrue();
        ChatRules.CanSendMessages(isGuest: true, isSuspendedNow: false).Should().BeFalse();
        ChatRules.CanSendMessages(isGuest: false, isSuspendedNow: true).Should().BeFalse();
    }

    /// <summary>
    /// An empty message (or only spaces) is not saved, a message is saved without the spaces around it.
    /// </summary>
    [Test]
    [Description("An empty message is not saved.")]
    public void Empty_Message_Is_Not_Saved_Test()
    {
        ChatRules.NormalizeMessage(null).Should().BeNull();
        ChatRules.NormalizeMessage(string.Empty).Should().BeNull();
        ChatRules.NormalizeMessage("   \t\n ").Should().BeNull();
        ChatRules.NormalizeMessage("  Hello  ").Should().Be("Hello");
    }

    /// <summary>
    /// A message longer than the limit is cut (the hub can be called without the message field).
    /// </summary>
    [Test]
    [Description("A message is saved with at most MaxMessageLength characters.")]
    public void Long_Message_Is_Cut_Test()
    {
        var message = new string('a', ChatRules.MaxMessageLength + 50);

        ChatRules.NormalizeMessage(message).Should().HaveLength(ChatRules.MaxMessageLength);
        ChatRules.NormalizeMessage(new string('b', ChatRules.MaxMessageLength)).Should().HaveLength(ChatRules.MaxMessageLength);
    }

    /// <summary>
    /// Messages of the same user within the group time are shown as one group.
    /// </summary>
    [Test]
    [Description("Messages of the same user within a few minutes are one group.")]
    public void Messages_Of_The_Same_User_Are_Grouped_Test()
    {
        var first = new DateTime(2026, 11, 25, 10, 0, 0, DateTimeKind.Utc);

        ChatRules.ContinuesGroup(1, first, 1, first.AddMinutes(1)).Should().BeTrue();
        ChatRules.ContinuesGroup(1, first, 1, first + ChatRules.GroupTime).Should().BeTrue();

        // another user, too late, the first message, or an older message
        ChatRules.ContinuesGroup(2, first, 1, first.AddMinutes(1)).Should().BeFalse();
        ChatRules.ContinuesGroup(1, first, 1, first + ChatRules.GroupTime + TimeSpan.FromSeconds(1)).Should().BeFalse();
        ChatRules.ContinuesGroup(null, first, 1, first).Should().BeFalse();
        ChatRules.ContinuesGroup(1, first, 1, first.AddMinutes(-1)).Should().BeFalse();
    }
}
