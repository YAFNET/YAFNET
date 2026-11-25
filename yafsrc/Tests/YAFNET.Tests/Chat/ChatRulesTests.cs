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
}
