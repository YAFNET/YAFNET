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

using YAF.Core.Helpers;
using YAF.Types.Flags;
using YAF.Types.Models;

/// <summary>
/// Who can see the text of a single message (<see cref="MessageAccessHelper"/>).
/// </summary>
[TestFixture]
public class MessageAccessHelperTests
{
    private const int Author = 5;

    private const int OtherUser = 6;

    [Test]
    public void ApprovedMessageIsVisibleToEveryone()
    {
        MessageAccessHelper.CanViewMessage(Message(approved: true, deleted: false), OtherUser, false).Should().BeTrue();
    }

    [Test]
    public void NotApprovedMessageIsOnlyVisibleToTheAuthorAndModerators()
    {
        var message = Message(approved: false, deleted: false);

        MessageAccessHelper.CanViewMessage(message, OtherUser, false).Should().BeFalse("it waits for approval");
        MessageAccessHelper.CanViewMessage(message, Author, false).Should().BeTrue();
        MessageAccessHelper.CanViewMessage(message, OtherUser, true).Should().BeTrue();
    }

    [Test]
    public void DeletedMessageIsOnlyVisibleToModerators()
    {
        var message = Message(approved: true, deleted: true);

        MessageAccessHelper.CanViewMessage(message, OtherUser, false).Should().BeFalse();
        MessageAccessHelper.CanViewMessage(message, Author, false).Should().BeFalse("the topic only shows a placeholder");
        MessageAccessHelper.CanViewMessage(message, OtherUser, true).Should().BeTrue();
    }

    private static Message Message(bool approved, bool deleted)
    {
        return new Message
               {
                   UserID = Author,
                   MessageFlags = new MessageFlags { IsApproved = approved, IsDeleted = deleted }
               };
    }
}
