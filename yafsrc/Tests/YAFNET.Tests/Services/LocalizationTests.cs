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

using System.Globalization;
using System.IO;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using YAF.Core.Services;
using YAF.Core.Services.Localization;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;

/// <summary>
/// The real <see cref="Localization"/> service, reading language files from a temporary web root
/// (the real english.json plus a small German test file).
/// </summary>
[TestFixture]
[NonParallelizable]
public class LocalizationTests
{
    private const string GermanFile = "test-german.json";

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
                  { "@tag": "BOLD", "#text": "Das ist [b]fett[/b][br]und [noparse][b]roh[/b][/noparse]" },
                  { "@tag": "FORMAT", "#text": "{0} von {1}" },
                  { "@tag": "EMPTY", "#text": "" }
                ]
              },
              {
                "@name": "TOPICS",
                "Resource": [
                  { "@tag": "TITLE", "#text": "Themen" },
                  { "@tag": "WITH_QUOTE", "#text": "Sag \"hallo\" & <tschüss>" }
                ]
              }
            ]
          }
        }
        """;

    private string webRoot;

    private TestBoardContext board;

    private CultureInfo previousCulture;

    [SetUp]
    public void SetUp()
    {
        this.previousCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-lang-{Guid.NewGuid():N}");
        var languages = Path.Combine(this.webRoot, "languages");
        Directory.CreateDirectory(languages);

        File.Copy(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "languages", "english.json"),
            Path.Combine(languages, "english.json"));
        File.WriteAllText(Path.Combine(languages, GermanFile), GermanJson);

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(environment).As<IWebHostEnvironment>();

                // Localizer reads the json through ILocalization.LoadLanguageFile
                b.RegisterType<Localization>().As<ILocalization>();
            });

        this.board.BoardSettings.Language = "english.json";
        this.board.BoardSettings.Culture = "en-US";
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        CultureInfo.CurrentCulture = this.previousCulture;

        if (Directory.Exists(this.webRoot))
        {
            Directory.Delete(this.webRoot, true);
        }
    }

    [Test]
    [Description("Texts are read from the board language file")]
    public void GetText_Test()
    {
        var localization = new Localization("COMMON");

        localization.GetText("OK").Should().Be("OK");
        localization.GetText("COMMON", "CANCEL").Should().Be("Cancel");
        localization.TranslationLoaded.Should().BeTrue();
        localization.LanguageFileName.Should().Be("english.json");
        localization.LanguageCode.Should().Be("en");
    }

    [Test]
    [Description("Tags are case insensitive")]
    public void GetText_Case_Insensitive_Test()
    {
        new Localization(string.Empty).GetText("common", "ok").Should().Be("OK");
    }

    [Test]
    [Description("A tag that does not exist anywhere returns the [PAGE.TAG] marker")]
    public void GetText_Missing_Tag_Test()
    {
        new Localization("COMMON").GetText("COMMON", "DOES_NOT_EXIST").Should().Be("[COMMON.DOES_NOT_EXIST]");
    }

    [Test]
    [Description("A tag missing in its page is looked up in the other pages")]
    public void GetText_Other_Page_Fallback_Test()
    {
        this.UseGerman();

        new Localization(string.Empty).GetText("COMMON", "TITLE").Should().Be("Themen");
    }

    [Test]
    [Description("A tag missing in a translation falls back to the English text in brackets")]
    public void GetText_English_Fallback_Test()
    {
        this.UseGerman();

        var localization = new Localization("COMMON");

        localization.GetText("OK").Should().Be("Okay");
        localization.GetText("CANCEL").Should().Be("[Cancel]");
    }

    [Test]
    [Description("BBCode style markup in texts is converted, [noparse] keeps it literal")]
    public void GetText_Markup_Test()
    {
        this.UseGerman();

        new Localization(string.Empty).GetText("COMMON", "BOLD").Should().Be("Das ist <strong>fett</strong><br />und [b]roh[/b]");
    }

    [Test]
    [Description("GetText(text) needs a text and uses the TransPage")]
    public void GetText_Null_Test()
    {
        var localization = new Localization("COMMON");

        localization.Invoking(l => l.GetText(null)).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("GetTextExists is false for unknown and empty texts")]
    public void GetTextExists_Test()
    {
        this.UseGerman();

        var localization = new Localization(string.Empty);

        localization.GetTextExists("COMMON", "OK").Should().BeTrue();
        localization.GetTextExists("COMMON", "EMPTY").Should().BeFalse();
        localization.GetTextExists("COMMON", "DOES_NOT_EXIST").Should().BeFalse();
    }

    [Test]
    [Description("GetTextFormatted fills the arguments and marks missing ones")]
    public void GetTextFormatted_Test()
    {
        this.UseGerman();

        var localization = new Localization("COMMON");

        localization.GetTextFormatted("FORMAT", 1, 5).Should().Be("1 von 5");
        localization.GetTextFormatted("FORMAT", 1).Should().Be("1 von [INVALID: COMMON.FORMAT -- EMPTY PARAM #1]");
        localization.Invoking(l => l.GetTextFormatted("FORMAT", null)).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("Attribute texts are html attribute encoded")]
    public void GetAttributeText_Test()
    {
        this.UseGerman();

        new Localization("TOPICS").GetAttributeText("WITH_QUOTE").Should().Be("Sag &quot;hallo&quot; &amp; &lt;tschüss>");
    }

    [Test]
    [Description("A text can be read from another language file than the board language")]
    public void GetText_Other_Language_File_Test()
    {
        var localization = new Localization("COMMON");

        localization.GetText("COMMON", "OK", GermanFile).Should().Be("Okay");
        localization.GetText("COMMON", "OK", null).Should().Be("OK", "without a file the board language is used");
        localization.GetText("COMMON", "DOES_NOT_EXIST", null).Should().Be("[COMMON.DOES_NOT_EXIST]");
    }

    [Test]
    [Description("The culture comes from the language file, refined by the board culture")]
    public void Culture_Test()
    {
        this.UseGerman();

        var localization = new Localization(string.Empty);

        localization.Culture.Name.Should().Be("de-DE");
        localization.LanguageCode.Should().Be("de");
        localization.FormatString("{0:N1}", 1.5).Should().Be("1,5");
        localization.FormatDateTime("MMMM", new DateTime(2026, 3, 1)).Should().Be("März");
    }

    [Test]
    [Description("A board culture of another language does not override the language file culture")]
    public void Culture_Mismatch_Test()
    {
        this.UseGerman();
        this.board.BoardSettings.Culture = "fr-FR";

        new Localization(string.Empty).Culture.TwoLetterISOLanguageName.Should().Be("de");
    }

    [Test]
    [Description("GetNodesUsingQuery returns the matching resources of a page")]
    public void GetNodesUsingQuery_Test()
    {
        this.UseGerman();

        new Localization(string.Empty).GetNodesUsingQuery("TOPICS", r => r.Tag.StartsWith("TI")).Select(r => r.Text)
            .Should().Equal("Themen");
    }

    [Test]
    [Description("A localization without a TransPage reads texts like one with an empty page")]
    public void GetText_Without_TransPage_Test()
    {
        new Localization().GetText("COMMON", "OK").Should().Be("OK");
    }

    [Test]
    [Description("Loading a language file that does not exist fails")]
    public void LoadTranslation_Missing_File_Test()
    {
        new Localization(string.Empty).Invoking(l => l.LoadTranslation("missing.json")).Should().Throw<ArgumentException>();
    }

    [Test]
    [Description("A board language file that does not exist (anymore) falls back to english instead of breaking the pages")]
    public void Missing_Board_Language_File_Test()
    {
        this.board.BoardSettings.Language = "removed.json";

        var localization = new Localization("COMMON");

        localization.GetText("COMMON", "CANCEL").Should().Be("Cancel");
        localization.LanguageFileName.Should().Be("english.json");
    }

    [Test]
    [Description("A user language file that does not exist (anymore) falls back to the board language")]
    public void Missing_User_Language_File_Test()
    {
        this.board.BoardSettings.AllowUserLanguage = true;
        this.board.User.IsGuest = false;

        // an existing user language file is used instead of the board language
        this.board.PageUser.LanguageFile = GermanFile;
        new Localization("COMMON").GetText("COMMON", "OK").Should().Be("Okay");

        this.UseGerman();
        this.board.PageUser.LanguageFile = "removed.json";

        var localization = new Localization("COMMON");

        localization.GetText("COMMON", "OK").Should().Be("Okay");
        localization.LanguageFileName.Should().Be(GermanFile.ToLower());
    }

    private void UseGerman()
    {
        this.board.BoardSettings.Language = GermanFile;
        this.board.BoardSettings.Culture = "de-DE";
    }
}
