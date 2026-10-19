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

using System.IO;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Services.Import;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The admin imports of banned e-mails / IPs / names / user agents, spam words and BBCode extensions.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DataImporterTests
{
    private const int Board = TestDatabase.BoardId;

    private TestBoardContext board;

    private IDataImporter importer;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(b => b.RegisterType<DataImporter>().As<IDataImporter>(), useDatabase: true);
        this.importer = this.board.Resolve<IDataImporter>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Banned e-mails: lines without @ and existing masks are skipped")]
    public async Task Banned_Emails_Test()
    {
        this.Insert(new BannedEmail { BoardID = Board, Mask = "old@spam.example", Since = DateTime.UtcNow });

        var count = await this.importer.BannedEmailAddressesImportAsync(
            Board,
            Text("new@spam.example\n\nno-at-sign\nold@spam.example\nother@spam.example"));

        count.Should().Be(2);
        this.Masks<BannedEmail>(b => b.Mask).Should().BeEquivalentTo(["old@spam.example", "new@spam.example", "other@spam.example"]);

        // a mask that is no valid regular expression is not imported
        (await this.importer.BannedEmailAddressesImportAsync(Board, Text("bad(@spam.example"))).Should().Be(0);
    }

    [Test]
    [Description("Banned IPs: valid addresses and wildcard masks are imported, existing ones skipped")]
    public async Task Banned_Ips_Test()
    {
        this.Insert(new BannedIP { BoardID = Board, Mask = "10.0.0.1", Since = DateTime.UtcNow });

        var count = await this.importer.BannedIpAddressesImportAsync(Board, 1, Text("10.0.0.1\n192.168.1.*\n2001:db8::1\n"));

        count.Should().Be(2);
        this.Masks<BannedIP>(b => b.Mask).Should().BeEquivalentTo(["10.0.0.1", "192.168.1.*", "2001:db8::1"]);
    }

    [Test]
    [Description("The ban date is stored in UTC like all other dates (it is shown converted from UTC to the time zone of the user)")]
    public async Task Ban_Date_Is_Utc_Test()
    {
        (await this.importer.BannedIpAddressesImportAsync(Board, 1, Text("10.0.0.1"))).Should().Be(1);
        (await this.importer.BannedEmailAddressesImportAsync(Board, Text("spam@example.com"))).Should().Be(1);
        (await this.importer.BannedNamesImportAsync(Board, Text("^Spammer$"))).Should().Be(1);

        this.board.Current.GetRepository<BannedIP>().Get(_ => true).Single().Since.Should()
            .BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        this.board.Current.GetRepository<BannedEmail>().Get(_ => true).Single().Since.Should()
            .BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        this.board.Current.GetRepository<BannedName>().Get(_ => true).Single().Since.Should()
            .BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Test]
    [Description("Banned IPs: lines that are no ip address or mask are not imported")]
    public async Task Banned_Ips_Invalid_Test()
    {
        var count = await this.importer.BannedIpAddressesImportAsync(Board, 1, Text("(\nnot an ip\n1.2.3\n300.1.1.1\n1.2.3.4"));

        count.Should().Be(1);
        this.Masks<BannedIP>(b => b.Mask).Should().Equal("1.2.3.4");
    }

    [Test]
    [Description("The AbuseIPDB black list only imports valid addresses")]
    public async Task Abuse_Ip_Db_Test()
    {
        var count = await this.importer.BannedIpAddressesImportAsync(
            Board,
            1,
            [
                new BlackListEntry { IpAddress = "203.0.113.5", AbuseConfidenceScore = 100 },
                new BlackListEntry { IpAddress = "invalid" },
                new BlackListEntry { IpAddress = "203.0.113.5" }
            ]);

        count.Should().Be(1);
        (await this.board.Current.GetRepository<BannedIP>().GetSingleAsync(_ => true)).Reason.Should().Contain("100");
    }

    [Test]
    [Description("Banned names and user agents: valid regular expressions are imported, invalid ones and duplicates skipped")]
    public async Task Banned_Names_And_User_Agents_Test()
    {
        (await this.importer.BannedNamesImportAsync(Board, Text("^casino\n([\n^casino"))).Should().Be(1);
        (await this.importer.BannedUserAgentsImportAsync(Board, Text("EvilBot\nBot(\nEvilBot"))).Should().Be(1);

        this.Masks<BannedName>(b => b.Mask).Should().Equal("^casino");
        this.Masks<BannedUserAgent>(b => b.UserAgent).Should().Equal("EvilBot");
    }

    [Test]
    [Description("Spam words are imported from the xml export, existing ones skipped, other files import nothing")]
    public void Spam_Words_Test()
    {
        this.Insert(new SpamWords { BoardID = Board, SpamWord = "casino" });

        this.importer.SpamWordsImport(
                Board,
                Text("<YafSpamWordsList><YafSpamWords><SpamWord>casino</SpamWord></YafSpamWords><YafSpamWords><SpamWord>pills</SpamWord></YafSpamWords></YafSpamWordsList>"))
            .Should().Be(1);

        this.Masks<SpamWords>(s => s.SpamWord).Should().BeEquivalentTo(["casino", "pills"]);

        this.importer.SpamWordsImport(Board, Text("<root><other>1</other></root>")).Should().Be(0);
    }

    [Test]
    [Description("BBCode extensions (in the format of the admin export): new ones are added, existing ones updated, other files are rejected")]
    public async Task BBCode_Test()
    {
        this.Insert(new BBCode { BoardID = Board, Name = "OLD", SearchRegex = "x", ReplaceRegex = "y", ExecOrder = 1 });

        const string Xml = """
            <YafBBCodeList>
              <YafBBCode><Name>OLD</Name><Description>updated</Description><SearchRegex>a</SearchRegex><ReplaceRegex>b</ReplaceRegex><ExecOrder>2</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>true</UseToolbar></YafBBCode>
              <YafBBCode><Name>NEW</Name><Description>new one</Description><SearchRegex>c</SearchRegex><ReplaceRegex>d</ReplaceRegex><ExecOrder>3</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>false</UseToolbar></YafBBCode>
            </YafBBCodeList>
            """;

        (await this.importer.BBCodeExtensionImportAsync(Board, Text(Xml))).Should().Be(1);

        var codes = await this.board.Current.GetRepository<BBCode>().GetAsync(_ => true);
        codes.Single(c => c.Name == "OLD").Description.Should().Be("updated");
        codes.Single(c => c.Name == "NEW").ExecOrder.Should().Be(3);

        await this.importer.Invoking(i => i.BBCodeExtensionImportAsync(Board, Text("<root><x>1</x></root>")))
            .Should().ThrowAsync<FormatException>();
    }

    [Test]
    [Description("BBCode extensions with an invalid search expression are not imported")]
    public async Task BBCode_Invalid_Regex_Test()
    {
        const string Xml = """
            <YafBBCodeList>
              <YafBBCode><Name>BAD</Name><Description /><SearchRegex>([</SearchRegex><ReplaceRegex>b</ReplaceRegex><ExecOrder>1</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>false</UseToolbar></YafBBCode>
              <YafBBCode><Name>GOOD</Name><Description /><SearchRegex>c</SearchRegex><ReplaceRegex>d</ReplaceRegex><ExecOrder>2</ExecOrder><OnClickJS /><DisplayJS /><EditJS /><DisplayCSS /><Variables /><ModuleClass /><UseModule>false</UseModule><UseToolbar>false</UseToolbar></YafBBCode>
            </YafBBCodeList>
            """;

        (await this.importer.BBCodeExtensionImportAsync(Board, Text(Xml))).Should().Be(1);

        this.Masks<BBCode>(c => c.Name).Should().Equal("GOOD");
    }

    private static MemoryStream Text(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }

    private void Insert<T>(T entity)
    {
        using var connection = this.board.Database.Open();
        connection.Insert(entity);
    }

    private List<string> Masks<T>(Func<T, string> selector)
        where T : class, YAF.Types.Interfaces.Data.IEntity, new()
    {
        return [.. this.board.Current.GetRepository<T>().Get(_ => true).Select(selector)];
    }
}
