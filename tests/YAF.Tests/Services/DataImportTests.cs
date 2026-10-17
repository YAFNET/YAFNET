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

    [Test]
    [Description("Existing bans and duplicates in the file are skipped")]
    public void Existing_Bans_Are_Skipped_Test()
    {
        DataImport.BannedEmailAddressesImport(TestDatabase.BoardId, Lines("old@spam\\.example")).Should().Be(1);
        DataImport.BannedEmailAddressesImport(TestDatabase.BoardId, Lines("new@spam\\.example", "old@spam\\.example"))
            .Should().Be(1);

        DataImport.BannedIpAddressesImport(TestDatabase.BoardId, 2, Lines("10.0.0.1")).Should().Be(1);
        DataImport.BannedIpAddressesImport(TestDatabase.BoardId, 2, Lines("10.0.0.1", "10.0.0.2")).Should().Be(1);

        DataImport.BannedNamesImport(TestDatabase.BoardId, Lines("^casino", "^casino")).Should().Be(1);
        DataImport.BannedUserAgentsImport(TestDatabase.BoardId, Lines("EvilBot", "EvilBot")).Should().Be(1);

        this.Values<BannedEmail>(b => b.Mask).Should().BeEquivalentTo("old@spam\\.example", "new@spam\\.example");
        this.Values<BannedIP>(b => b.Mask).Should().BeEquivalentTo("10.0.0.1", "10.0.0.2");
        this.Values<BannedName>(b => b.Mask).Should().Equal("^casino");
        this.Values<BannedUserAgent>(b => b.UserAgent).Should().Equal("EvilBot");
    }

    [Test]
    [Description("Spam words are imported from the xml export, existing ones skipped, other files import nothing")]
    public void Spam_Words_Test()
    {
        this.board.Current.GetRepository<Spam_Words>().Insert(new Spam_Words { BoardID = TestDatabase.BoardId, SpamWord = "casino" });

        DataImport.SpamWordsImport(
                TestDatabase.BoardId,
                Text("<YafSpamWordsList><YafSpamWords><SpamWord>casino</SpamWord></YafSpamWords><YafSpamWords><SpamWord>pills</SpamWord></YafSpamWords></YafSpamWordsList>"))
            .Should().Be(1);

        this.Values<Spam_Words>(s => s.SpamWord).Should().BeEquivalentTo("casino", "pills");

        DataImport.SpamWordsImport(TestDatabase.BoardId, Text("<root><other>1</other></root>")).Should().Be(0);
    }

    [Test]
    [Description("BBCode extensions (in the format of the admin export): new ones are added, existing ones updated, other files are rejected")]
    public void BBCode_Test()
    {
        this.board.Current.GetRepository<BBCode>().Insert(
            new BBCode { BoardID = TestDatabase.BoardId, Name = "OLD", SearchRegex = "x", ReplaceRegex = "y", ExecOrder = 1 });

        const string Xml = """
            <YafBBCodeList>
              <YafBBCode><Name>OLD</Name><Description>updated</Description><SearchRegex>a</SearchRegex><ReplaceRegex>b</ReplaceRegex><ExecOrder>2</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>true</UseToolbar></YafBBCode>
              <YafBBCode><Name>NEW</Name><Description>new one</Description><SearchRegex>c</SearchRegex><ReplaceRegex>d</ReplaceRegex><ExecOrder>3</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>false</UseToolbar></YafBBCode>
            </YafBBCodeList>
            """;

        DataImport.BBCodeExtensionImport(TestDatabase.BoardId, Text(Xml)).Should().Be(1);

        var codes = this.board.Current.GetRepository<BBCode>().Get(_ => true);
        codes.Single(c => c.Name == "OLD").Description.Should().Be("updated");
        codes.Single(c => c.Name == "NEW").ExecOrder.Should().Be(3);

        FluentActions.Invoking(() => DataImport.BBCodeExtensionImport(TestDatabase.BoardId, Text("<root><x>1</x></root>")))
            .Should().Throw<Exception>();
    }

    [Test]
    [Description("BBCode extensions with an invalid search expression are not imported")]
    public void BBCode_Invalid_Regex_Test()
    {
        const string Xml = """
            <YafBBCodeList>
              <YafBBCode><Name>BAD</Name><Description /><SearchRegex>([</SearchRegex><ReplaceRegex>b</ReplaceRegex><ExecOrder>1</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>false</UseToolbar></YafBBCode>
              <YafBBCode><Name>GOOD</Name><Description /><SearchRegex>c</SearchRegex><ReplaceRegex>d</ReplaceRegex><ExecOrder>2</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>false</UseToolbar></YafBBCode>
            </YafBBCodeList>
            """;

        DataImport.BBCodeExtensionImport(TestDatabase.BoardId, Text(Xml)).Should().Be(1);

        this.Values<BBCode>(c => c.Name).Should().Equal("GOOD");
    }

    private static MemoryStream Text(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }

    private List<string> Values<T>(Func<T, string> selector)
        where T : class, YAF.Types.Interfaces.Data.IEntity, new()
    {
        return [.. this.board.Current.GetRepository<T>().Get(_ => true).Select(selector)];
    }

    private static MemoryStream Lines(params string[] lines)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(string.Join("\n", lines)));
    }
}
