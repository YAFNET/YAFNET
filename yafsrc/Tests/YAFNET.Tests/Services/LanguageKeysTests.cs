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
using System.Text.Json;
using System.Text.RegularExpressions;

/// <summary>
/// Every text the code asks for with a literal page and tag exists in english.json
/// (otherwise "Missing Translation For PAGE.TAG" is logged and [PAGE.TAG] is shown).
/// </summary>
[TestFixture]
public partial class LanguageKeysTests
{
    [Test]
    [Description("All literal GetText / LocalizedText / localized-tag keys of the code exist in english.json")]
    public void All_Used_Texts_Exist_Test()
    {
        var root = FindSourceRoot();

        if (root is null)
        {
            Assert.Ignore("The source code is not available next to the tests.");
        }

        var (keys, tags) = ReadLanguageFile(Path.Combine(root, "YetAnotherForum.NET", "wwwroot", "languages", "english.json"));

        var missing = new SortedSet<string>();
        var checkedTexts = 0;

        foreach (var folder in new[] { "YAFNET.Core", "YAFNET.Web", Path.Combine("YetAnotherForum.NET", "Pages") })
        {
            foreach (var file in Directory.EnumerateFiles(Path.Combine(root, folder), "*.*", SearchOption.AllDirectories)
                         .Where(f => f.EndsWith(".cs") || f.EndsWith(".cshtml"))
                         .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                                     && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")))
            {
                var source = File.ReadAllText(file);

                var used = TextCall().Matches(source).Select(m => (m.Groups["page"].Value, m.Groups["tag"].Value))
                    .Concat(TagHelper().Matches(source).Select(m => (m.Groups["page"].Value, m.Groups["tag"].Value)))
                    .Concat(TagHelperReversed().Matches(source).Select(m => (m.Groups["page"].Value, m.Groups["tag"].Value)))
                    .Concat(SingleTagCall().Matches(source).Select(m => ("(current page)", m.Groups["tag"].Value)))
                    .Concat(InfoButtonLabels(source));

                foreach (var (page, tag) in used)
                {
                    checkedTexts++;

                    // the forum falls back to the same tag on any page
                    if (!keys.Contains($"{page.ToUpperInvariant()}.{tag.ToUpperInvariant()}") && !tags.Contains(tag.ToUpperInvariant()))
                    {
                        missing.Add($"{page}.{tag} ({Path.GetRelativePath(root, file)})");
                    }
                }
            }
        }

        checkedTexts.Should().BeGreaterThan(1000, "the scan finds the texts of the code");
        missing.Should().BeEmpty();
    }

    /// <summary>
    /// Labels with an info button show the text TAG_HELP of the page as tooltip.
    /// </summary>
    private static IEnumerable<(string Page, string Tag)> InfoButtonLabels(string source)
    {
        foreach (Match label in Label().Matches(source))
        {
            var attributes = label.Value;

            if (!attributes.Contains("info-button=\"true\""))
            {
                continue;
            }

            var tag = LocalizedTag().Match(attributes);
            var page = LocalizedPage().Match(attributes);

            if (tag.Success && page.Success)
            {
                yield return (page.Groups["page"].Value, $"{tag.Groups["tag"].Value}_HELP");
            }
        }
    }

    private static (HashSet<string> Keys, HashSet<string> Tags) ReadLanguageFile(string fileName)
    {
        using var json = JsonDocument.Parse(File.ReadAllText(fileName));

        var keys = new HashSet<string>();
        var tags = new HashSet<string>();

        foreach (var page in json.RootElement.GetProperty("Resources").GetProperty("page").EnumerateArray())
        {
            var name = page.GetProperty("@name").GetString()!.ToUpperInvariant();
            var resources = page.GetProperty("Resource");

            IEnumerable<JsonElement> list = resources.ValueKind == JsonValueKind.Array ? resources.EnumerateArray() : [resources];

            foreach (var resource in list)
            {
                var tag = resource.GetProperty("@tag").GetString()!.ToUpperInvariant();
                keys.Add($"{name}.{tag}");
                tags.Add(tag);
            }
        }

        return (keys, tags);
    }

    private static string FindSourceRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

        while (directory != null && !Directory.Exists(Path.Combine(directory.FullName, "YetAnotherForum.NET", "Pages")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName;
    }

    // GetText("PAGE", "TAG") and friends, the tag in upper case (GetTextFormatted("TAG", "value") is a single tag)
    [GeneratedRegex("""(?:GetText|GetAttributeText|LocalizedText(?:Async)?)\(\s*"(?<page>[A-Z0-9_]+)"\s*,\s*"(?<tag>[A-Za-z0-9_]+)"\s*\)""")]
    private static partial Regex TextCall();

    // GetText("TAG") and GetTextFormatted("TAG", ...) use the current page, any page has to have the tag
    [GeneratedRegex("""(?:\bGetText\(\s*"(?<tag>[A-Z][A-Z0-9_]+)"\s*\)|\bGetTextFormatted\(\s*"(?<tag>[A-Z][A-Z0-9_]+)"\s*,(?!\s*"[A-Z0-9_]+"\s*[,)]))""")]
    private static partial Regex SingleTagCall();

    [GeneratedRegex("""<label\b[^>]*>""")]
    private static partial Regex Label();

    [GeneratedRegex("""(?<![\w-])localized-tag="(?<tag>[A-Za-z0-9_]+)"(?:)""")]
    private static partial Regex LocalizedTag();

    [GeneratedRegex("""(?<![\w-])localized-page="(?<page>[A-Z0-9_]+)"(?:)""")]
    private static partial Regex LocalizedPage();

    [GeneratedRegex("""localized-page="(?<page>[A-Z0-9_]+)"[^>]*?localized-tag="(?<tag>[A-Za-z0-9_]+)"(?:)""")]
    private static partial Regex TagHelper();

    [GeneratedRegex("""localized-tag="(?<tag>[A-Za-z0-9_]+)"[^>]*?localized-page="(?<page>[A-Z0-9_]+)"(?:)""")]
    private static partial Regex TagHelperReversed();
}
