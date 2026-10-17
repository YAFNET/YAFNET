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

using YAF.Core.Services.Localization;
using YAF.Tests.TestInfrastructure;

/// <summary>
/// The <see cref="Localizer"/> of a language file (a small German test file):
/// texts of a page, the fallback to other pages, the culture and the queries.
/// </summary>
[TestFixture]
[NonParallelizable]
public class LocalizerTests
{
    private const string GermanJson = """
        {
          "Resources": {
            "@language": "Deutsch",
            "@code": "de",
            "page": [
              {
                "@name": "COMMON",
                "Resource": [
                  { "@tag": "OK", "#text": "Okay" },
                  { "@tag": "FORMAT", "#text": "{0} von {1}" },
                  { "@tag": "EMPTY", "#text": "" }
                ]
              },
              {
                "@name": "TOPICS",
                "Resource": [
                  { "@tag": "TITLE", "#text": "Themen" },
                  { "@tag": "TOPICS_COUNT", "#text": "Anzahl" }
                ]
              }
            ]
          }
        }
        """;

    private TestBoardContext board;

    private string folder;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();

        this.folder = Path.Combine(Path.GetTempPath(), $"yaf-lang-{Guid.NewGuid():N}");
        Directory.CreateDirectory(this.folder);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Directory.Delete(this.folder, true);
    }

    [Test]
    [Description("A text of a page is found, page and tag are case insensitive")]
    public void GetText_Test()
    {
        var localizer = this.CreateLocalizer(GermanJson);

        localizer.GetText("COMMON", "OK").Should().Be("Okay");
        localizer.GetText("common", "ok").Should().Be("Okay");
        localizer.GetText("TOPICS", "TITLE").Should().Be("Themen");
    }

    [Test]
    [Description("A tag that is not on the page is taken from any other page")]
    public void GetText_Other_Page_Fallback_Test()
    {
        var localizer = this.CreateLocalizer(GermanJson);

        localizer.GetText("TOPICS", "OK").Should().Be("Okay");
        localizer.GetText("UNKNOWN_PAGE", "TITLE").Should().Be("Themen");
    }

    [Test]
    [Description("A missing or empty text is null (the english text is used then)")]
    public void GetText_Missing_Test()
    {
        var localizer = this.CreateLocalizer(GermanJson);

        localizer.GetText("COMMON", "NOT_THERE").Should().BeNull();
        localizer.GetText("COMMON", "EMPTY").Should().BeNull();

        localizer.GetText("COMMON", "NOT_THERE", out var text);
        text.Should().BeNull();
    }

    [Test]
    [Description("The page of a text call does not stick: the next call uses its own page")]
    public void GetText_Page_Switch_Test()
    {
        var localizer = this.CreateLocalizer(GermanJson);

        localizer.SetPage("TOPICS");
        localizer.GetText("TITLE", out var title);
        title.Should().Be("Themen");

        localizer.GetText("COMMON", "FORMAT").Should().Be("{0} von {1}");

        // an empty page is the default page
        localizer.SetPage(string.Empty);
        localizer.GetText("OK", out var ok);
        ok.Should().Be("Okay");
    }

    [Test]
    [Description("The culture comes from the code of the language file, a long code is shortened to the language")]
    [TestCase("de", "de")]
    [TestCase("de-DE", "de-DE")]
    [TestCase("de-DE-1996", "de")]
    [TestCase("", "en-US")]
    public void Culture_Test(string code, string expected)
    {
        var localizer = this.CreateLocalizer(GermanJson.Replace("\"@code\": \"de\"", $"\"@code\": \"{code}\""));

        localizer.CurrentCulture.Name.Should().Be(expected);
    }

    [Test]
    [Description("The texts of a page can be queried, an unknown page queries all pages")]
    public void GetNodesUsingQuery_Test()
    {
        var localizer = this.CreateLocalizer(GermanJson);

        localizer.GetNodesUsingQuery("TOPICS", r => r.Tag.StartsWith("TOPICS_")).Select(r => r.Text).Should()
            .Equal("Anzahl");

        localizer.GetNodesUsingQuery("UNKNOWN_PAGE", r => r.Tag.StartsWith("T")).Select(r => r.Tag).Should()
            .BeEquivalentTo("TITLE", "TOPICS_COUNT");
    }

    [Test]
    [Description("A missing language file is refused")]
    public void Missing_File_Test()
    {
        FluentActions.Invoking(() => new Localizer(Path.Combine(this.folder, "missing.json"), false)).Should()
            .Throw<ArgumentException>();
    }

    [Test]
    [Description("A language file with the old .xml name is read from the .json file")]
    public void Xml_File_Name_Test()
    {
        var localizer = this.CreateLocalizer(GermanJson, "german.xml");

        localizer.GetText("COMMON", "OK").Should().Be("Okay");
    }

    [Test]
    [Description("The english language file of the forum can be read")]
    public void English_File_Test()
    {
        var english = Path.Combine(FindLanguagesFolder(), "english.json");

        if (!File.Exists(english))
        {
            Assert.Ignore("The source code is not available next to the tests.");
        }

        var localizer = new Localizer(english, false);

        localizer.GetText("DEFAULT", "FORUM").Should().NotBeNullOrEmpty();
        localizer.CurrentCulture.TwoLetterISOLanguageName.Should().Be("en");
    }

    private static string FindLanguagesFolder()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

        while (directory != null
               && !Directory.Exists(Path.Combine(directory.FullName, "yafsrc", "YetAnotherForum.NET", "languages")))
        {
            directory = directory.Parent;
        }

        return directory == null
                   ? string.Empty
                   : Path.Combine(directory.FullName, "yafsrc", "YetAnotherForum.NET", "languages");
    }

    private Localizer CreateLocalizer(string json, string fileName = "german.json")
    {
        File.WriteAllText(Path.Combine(this.folder, Path.ChangeExtension(fileName, ".json")), json);

        return new Localizer(Path.Combine(this.folder, fileName), false);
    }
}
