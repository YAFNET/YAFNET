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

using System.IO;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Core.Services.Localization;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The select lists and language information of <see cref="StaticDataHelper"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class StaticDataHelperTests
{
    private const string GermanJson = """
        {
          "Resources": {
            "@language": "Deutsch",
            "@code": "de",
            "page": [
              { "@name": "COMMON", "Resource": [ { "@tag": "OK", "#text": "Okay" } ] },
              { "@name": "COUNTRY", "Resource": [ { "@tag": "DE", "#text": "Deutschland" }, { "@tag": "AT", "#text": "Österreich" } ] },
              { "@name": "REGION", "Resource": [ { "@tag": "RGN_DE_BY", "#text": "Bayern" }, { "@tag": "RGN_AT_W", "#text": "Wien" } ] }
            ]
          }
        }
        """;

    private string webRoot;

    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-static-{Guid.NewGuid():N}");
        var languages = Path.Combine(this.webRoot, "languages");
        Directory.CreateDirectory(languages);
        Directory.CreateDirectory(Path.Combine(this.webRoot, "resources"));
        Directory.CreateDirectory(Path.Combine(this.webRoot, "css", "themes", "yaf"));
        Directory.CreateDirectory(Path.Combine(this.webRoot, "css", "themes", "dark"));

        File.Copy(Path.Combine(TestContext.CurrentContext.TestDirectory, "languages", "english.json"), Path.Combine(languages, "english.json"));
        File.WriteAllText(Path.Combine(languages, "german.json"), GermanJson);
        File.WriteAllText(
            Path.Combine(this.webRoot, "resources", "helpMenu.json"),
            """[ { "HelpCategory": "General", "HelpPages": [ "Index", "Search" ] } ]""");

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
                b.RegisterType<Localization>().As<ILocalization>();
            });

        this.board.BoardSettings.Language = "german.json";
        this.board.BoardSettings.Culture = "de-DE";
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Directory.Delete(this.webRoot, true);
    }

    [Test]
    [Description("The language list shows every language file with its translation progress, also incomplete translations")]
    public void Language_Files_Test()
    {
        var files = StaticDataHelper.LanguageFiles();

        files.Select(f => f.CultureFile).Should().BeEquivalentTo(["english.json", "german.json"]);
        files.Single(f => f.CultureFile == "english.json").TranslatedPercentage.Should().Be(100);

        var german = files.Single(f => f.CultureFile == "german.json");
        german.CultureEnglishName.Should().Be("Deutsch");
        german.TranslatedPercentage.Should().BeInRange(0, 1, "only one text is translated");
    }

    [Test]
    [Description("The topic age filter offers all periods up to the last year")]
    public void Topic_Times_Test()
    {
        StaticDataHelper.TopicTimes().Select(t => t.Value).Should().Equal("0", "1", "2", "3", "4", "5", "6", "7", "8");
    }

    [Test]
    [Description("Countries and regions come from the language file, sorted, with a none entry first")]
    public void Countries_And_Regions_Test()
    {
        var countries = StaticDataHelper.Countries();
        countries.First().Value.Should().BeNull();
        countries.Skip(1).Select(c => c.Text).Should().Equal("Deutschland", "Österreich");

        StaticDataHelper.Regions("DE").Skip(1).Select(r => (r.Text, r.Value)).Should().Equal(("Bayern", "BY"));
    }

    [Test]
    [Description("Cultures and languages for the available language files")]
    public void Cultures_Test()
    {
        StaticDataHelper.Cultures().Should().Contain(c => c.CultureTag == "de-DE" && c.CultureFile == "german.json");
        StaticDataHelper.NeutralCultures().Should().Contain(c => c.CultureFile == "english.json");
        StaticDataHelper.Languages().Should().Contain(l => l.Value == "de-DE");

        StaticDataHelper.CultureDefaultFromFile("german.json").Should().StartWith("de-");
        StaticDataHelper.CultureDefaultFromFile(null).Should().Be("en-US");
        StaticDataHelper.CultureDefaultFromFile("missing.json").Should().BeNull();
    }

    [Test]
    [Description("Simple select lists: themes, time zones, page sizes, priorities, genders and list modes")]
    public void Select_Lists_Test()
    {
        StaticDataHelper.Themes().Select(t => t.Value).Should().BeEquivalentTo(["yaf", "dark"]);
        StaticDataHelper.TimeZones().Should().NotBeEmpty();
        StaticDataHelper.TimeZones([TimeZoneInfo.Utc]).Single().Value.Should().Be("UTC");
        StaticDataHelper.PageEntries().Select(p => p.Value).Should().Equal("5", "10", "20", "25", "50");
        StaticDataHelper.TopicPriorities().Select(p => p.Value).Should().Equal("0", "1", "2");
        StaticDataHelper.Gender().Should().NotBeEmpty();
        StaticDataHelper.FriendListModes().Should().HaveCount(3);
        StaticDataHelper.TopicListModes().Should().NotBeEmpty();
        StaticDataHelper.ImageFormats.Should().Contain("webp");
    }

    [Test]
    [Description("The help menu is read from the resources")]
    public void Help_Menu_Test()
    {
        var menu = StaticDataHelper.LoadHelpMenuJson();

        menu.Should().ContainSingle().Which.HelpPages.Should().Equal("Index", "Search");
    }

    [Test]
    [Description("The culture of a user: own culture when it fits the language, otherwise the language's culture")]
    public void Culture_By_User_Test()
    {
        // the user's culture fits the language of the file (de)
        CultureInfoHelper.GetCultureByUser(
            this.board.BoardSettings,
            new User { LanguageFile = "german.json", Culture = "de-AT" }).Name.Should().Be("de-AT");

        // a culture of another language falls back to the culture of the language file
        CultureInfoHelper.GetCultureByUser(
            this.board.BoardSettings,
            new User { LanguageFile = "german.json", Culture = "fr-FR" }).TwoLetterISOLanguageName.Should().Be("de");

        // without own settings the board language and culture are used
        CultureInfoHelper.GetCultureByUser(this.board.BoardSettings, new User()).Name.Should().Be("de-DE");
    }

    [Test]
    [Description("A language file that no longer exists (e.g. removed) falls back to the default culture")]
    public void Culture_By_User_Missing_Language_File_Test()
    {
        var culture = CultureInfoHelper.GetCultureByUser(
            this.board.BoardSettings,
            new User { LanguageFile = "removed.json", Culture = "fr-FR" });

        culture.Name.Should().Be("en-US");
    }
}
