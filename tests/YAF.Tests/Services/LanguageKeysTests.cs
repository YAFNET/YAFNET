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
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

/// <summary>
/// Every text the code asks for with a literal tag exists in english.json
/// (otherwise "Missing Translation For PAGE.TAG" is logged and [PAGE.TAG] is shown).
/// </summary>
[TestFixture]
public class LanguageKeysTests
{
    /// <summary>
    /// GetText("PAGE", "TAG") and GetAttributeText("PAGE", "TAG") (GetTextFormatted("TAG", "value") is a single tag).
    /// </summary>
    private static readonly Regex TextCall = new(
        @"\b(?:GetText|GetAttributeText)\(\s*""(?<page>[A-Z0-9_]+)""\s*,\s*""(?<tag>[A-Za-z0-9_]+)""\s*[,)]");

    /// <summary>
    /// GetText("TAG") and GetTextFormatted("TAG", ...) use the current page, any page has to have the tag.
    /// </summary>
    private static readonly Regex SingleTagCall = new(
        @"(?:\bGetText\(\s*""(?<tag>[A-Z][A-Z0-9_]+)""\s*\)|\bGetTextFormatted\(\s*""(?<tag>[A-Z][A-Z0-9_]+)""\s*,(?!\s*""[A-Z0-9_]+""\s*[,)]))");

    /// <summary>
    /// The localized controls of the markup (LocalizedLabel, ThemeButton, ...).
    /// </summary>
    private static readonly Regex MarkupTag = new(
        @"\b(?:LocalizedTag|TextLocalizedTag|TitleLocalizedTag)\s*=\s*""(?<tag>[A-Za-z0-9_]+)""");

    [Test]
    [Description("All literal GetText / localized control tags of the code exist in english.json")]
    public void All_Used_Texts_Exist_Test()
    {
        var root = FindSourceRoot();

        if (root is null)
        {
            Assert.Ignore("The source code is not available next to the tests.");
        }

        var tags = ReadTags(Path.Combine(root, "YetAnotherForum.NET", "languages", "english.json"));

        var missing = new SortedSet<string>();
        var checkedTexts = 0;

        foreach (var folder in new[] { "YAF.Core", "YAF.Web", "YAF.Configuration", "YetAnotherForum.NET" })
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(root, folder), "*.*", SearchOption.AllDirectories)
                         .Where(f => f.EndsWith(".cs") || f.EndsWith(".ascx") || f.EndsWith(".aspx") || f.EndsWith(".master"))
                         .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                                     && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                                     && !f.Contains($"{Path.DirectorySeparatorChar}node_modules{Path.DirectorySeparatorChar}")))
            {
                var source = File.ReadAllText(file);

                var used = TextCall.Matches(source).Cast<Match>().Select(m => $"{m.Groups["page"].Value}.{m.Groups["tag"].Value}")
                    .Concat(SingleTagCall.Matches(source).Cast<Match>().Select(m => m.Groups["tag"].Value))
                    .Concat(MarkupTag.Matches(source).Cast<Match>().Select(m => m.Groups["tag"].Value));

                foreach (var text in used)
                {
                    checkedTexts++;

                    // the forum falls back to the same tag on any page
                    var tag = text.Substring(text.IndexOf('.') + 1).ToUpperInvariant();

                    if (!tags.Contains(tag))
                    {
                        missing.Add($"{text} ({file.Substring(root.Length + 1)})");
                    }
                }
            }
        }

        checkedTexts.Should().BeGreaterThan(1000, "the scan finds the texts of the code");
        missing.Should().BeEmpty();
    }

    [Test]
    [Description("Every gender of the profile has a text in english.json")]
    public void All_Genders_Have_A_Text_Test()
    {
        var root = FindSourceRoot();

        if (root is null)
        {
            Assert.Ignore("The source code is not available next to the tests.");
        }

        var tags = ReadTags(Path.Combine(root, "YetAnotherForum.NET", "languages", "english.json"));

        EnumExtensions.GetAllItems<YAF.Types.Constants.Gender>().Select(g => g.ToString().ToUpperInvariant()).Where(g => !tags.Contains(g))
            .Should().BeEmpty();
    }

    private static HashSet<string> ReadTags(string fileName)
    {
        var json = JObject.Parse(File.ReadAllText(fileName));

        var tags = new HashSet<string>();

        foreach (var page in json["Resources"]!["page"]!)
        {
            var resources = page["Resource"];

            var list = resources is JArray array ? array.Cast<JToken>() : [resources];

            foreach (var resource in list)
            {
                tags.Add(resource!["@tag"]!.ToString().ToUpperInvariant());
            }
        }

        return tags;
    }

    private static string FindSourceRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

        while (directory != null && !Directory.Exists(Path.Combine(directory.FullName, "yafsrc", "YetAnotherForum.NET", "languages")))
        {
            directory = directory.Parent;
        }

        return directory == null ? null : Path.Combine(directory.FullName, "yafsrc");
    }
}
