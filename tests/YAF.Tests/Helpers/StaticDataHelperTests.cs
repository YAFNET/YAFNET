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
using System.Web;

using Autofac;

using NSubstitute;

using YAF.Tests.TestInfrastructure;

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
              { "@name": "COMMON", "Resource": [ { "@tag": "OK", "#text": "Okay" } ] }
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
        Directory.CreateDirectory(Path.Combine(this.webRoot, "Content", "Themes", "yaf"));
        Directory.CreateDirectory(Path.Combine(this.webRoot, "Content", "Themes", "dark"));

        File.Copy(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "languages", "english.json"),
            Path.Combine(languages, "english.json"));
        File.WriteAllText(Path.Combine(languages, "german.json"), GermanJson);

        // the folders of the forum are found with MapPath of the request
        var request = Substitute.For<HttpRequestBase>();
        request.MapPath(Arg.Any<string>()).Returns(
            c => Path.Combine(this.webRoot, c.Arg<string>().TrimStart('~', '/').Replace("//", "/").Replace('/', '\\')));

        this.board = new TestBoardContext(b => b.RegisterInstance(request).As<HttpRequestBase>());
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

        files.Select(f => f.CultureFile).Should().BeEquivalentTo("english.json", "german.json");
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
        this.board.Localization.Add("COUNTRY", "DE", "Deutschland").Add("COUNTRY", "AT", "Österreich")
            .Add("REGION", "RGN_DE_BY", "Bayern").Add("REGION", "RGN_AT_W", "Wien");

        var countries = StaticDataHelper.Country();
        countries.First().Value.Should().BeNull();
        countries.Skip(1).Select(c => c.Name).Should().Equal("Deutschland", "Österreich");

        StaticDataHelper.Region("DE").Skip(1).Select(r => (r.Name, r.Value)).Should().Equal(("Bayern", "BY"));
    }

    [Test]
    [Description("Cultures for the available language files and the culture of a language file")]
    public void Cultures_Test()
    {
        StaticDataHelper.Cultures().Should().Contain(c => c.CultureTag.StartsWith("de-") && c.CultureFile == "german.json");
        StaticDataHelper.NeutralCultures().Should().Contain(c => c.CultureFile == "english.json");

        StaticDataHelper.CultureDefaultFromFile("german.json").Should().StartWith("de-");
        StaticDataHelper.CultureDefaultFromFile(null).Should().Be("en-US");
        StaticDataHelper.CultureDefaultFromFile("missing.json").Should().BeNull();
    }

    [Test]
    [Description("Simple select lists: themes, time zones, page sizes, genders and list modes")]
    public void Select_Lists_Test()
    {
        StaticDataHelper.Themes().Should().BeEquivalentTo("yaf", "dark");
        StaticDataHelper.TimeZones().Should().NotBeEmpty();
        StaticDataHelper.TimeZones([TimeZoneInfo.Utc]).Single().Value.Should().Be("UTC");
        StaticDataHelper.PageEntries().Should().NotBeEmpty();
        StaticDataHelper.Gender().Should().NotBeEmpty();
        StaticDataHelper.TopicListModes().Should().NotBeEmpty();
        StaticDataHelper.ImageFormats().Should().Contain("png");
    }
}
