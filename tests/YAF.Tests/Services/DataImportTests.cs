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

using System.IO;
using System.Linq;
using System.Text;

using YAF.Core.Extensions;
using YAF.Core.Services.Import;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Models;

/// <summary>
/// The imports of the ban lists (<see cref="DataImport"/>).
/// </summary>
[TestFixture]
[NonParallelizable]
public class DataImportTests
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
    [Description("The ip import accepts ip addresses (v4 and v6) and ipv4 masks, no incomplete addresses")]
    public void BannedIpAddressesImport_Test()
    {
        var count = DataImport.BannedIpAddressesImport(
            TestDatabase.BoardId,
            2,
            Lines("10.0.0.1", " 192.168.1.* ", "2001:db8::1", "1.2.3", "300.1.1.1", "not an ip", string.Empty));

        count.Should().Be(3);

        this.board.Current.GetRepository<BannedIP>().Get(b => b.BoardID == TestDatabase.BoardId)
            .Select(b => b.Mask).Should().BeEquivalentTo("10.0.0.1", "192.168.1.*", "2001:db8::1");
    }

    [Test]
    [Description("The name, e-mail and user agent imports only accept valid regular expressions")]
    public void Regex_Imports_Skip_Invalid_Masks_Test()
    {
        DataImport.BannedNamesImport(TestDatabase.BoardId, Lines("^Spammer$", "[invalid")).Should().Be(1);
        DataImport.BannedEmailAddressesImport(TestDatabase.BoardId, Lines("spam@example\\.com", "(@invalid"))
            .Should().Be(1);
        DataImport.BannedUserAgentsImport(TestDatabase.BoardId, Lines("BadBot", "*invalid")).Should().Be(1);

        this.board.Current.GetRepository<BannedName>().Get(b => b.BoardID == TestDatabase.BoardId)
            .Should().ContainSingle().Which.Mask.Should().Be("^Spammer$");
        this.board.Current.GetRepository<BannedEmail>().Get(b => b.BoardID == TestDatabase.BoardId)
            .Should().ContainSingle().Which.Mask.Should().Be("spam@example\\.com");
        this.board.Current.GetRepository<BannedUserAgent>().Get(b => b.BoardID == TestDatabase.BoardId)
            .Should().ContainSingle().Which.UserAgent.Should().Be("BadBot");
    }

    private static MemoryStream Lines(params string[] lines)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(string.Join("\n", lines)));
    }
}
