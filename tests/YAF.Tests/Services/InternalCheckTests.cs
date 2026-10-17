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

using System;

using YAF.Core.Extensions;
using YAF.Core.Services.CheckForSpam;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Models;

/// <summary>
/// The internal bot check (<see cref="InternalCheck"/>) with the banned e-mails, names and IPs of the board.
/// </summary>
[TestFixture]
[NonParallelizable]
public class InternalCheckTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("An invalid banned e-mail mask does not disable the other e-mail bans")]
    public void Invalid_Email_Mask_Does_Not_Disable_Other_Bans_Test()
    {
        var emails = this.board.Current.GetRepository<BannedEmail>();

        emails.Insert(NewBannedEmail("[invalid"));
        emails.Insert(NewBannedEmail("^spam@example\\.com$"));

        var check = new InternalCheck();

        check.IsBot(null, "spam@example.com", "Alice", out var response).Should().BeTrue();
        response.Should().Contain("spam@example.com");

        check.IsBot(null, "alice@example.com", "Alice", out _).Should().BeFalse();
    }

    [Test]
    [Description("Without an e-mail address and user name only the IP is checked")]
    public void Missing_Email_And_Name_Test()
    {
        this.board.Current.GetRepository<BannedEmail>().Insert(NewBannedEmail("^spam@example\\.com$"));
        this.board.Current.GetRepository<BannedName>().Insert(
            new BannedName { BoardID = TestDatabase.BoardId, Mask = "^Spammer$", Since = DateTime.UtcNow });

        new InternalCheck().IsBot(null, null, null, out _).Should().BeFalse();
    }

    private static BannedEmail NewBannedEmail(string mask)
    {
        return new BannedEmail { BoardID = TestDatabase.BoardId, Mask = mask, Since = DateTime.UtcNow };
    }
}
