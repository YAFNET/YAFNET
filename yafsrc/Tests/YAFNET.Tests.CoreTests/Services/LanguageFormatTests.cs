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

namespace YAF.Tests.CoreTests.Services;

using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

/// <summary>
/// The translations of texts with placeholders ({0}) are valid format strings: a broken translation
/// (e.g. "{0 comments" or "{text") throws a FormatException when the text is shown.
/// </summary>
[TestFixture]
public partial class LanguageFormatTests
{
    [Test]
    [Description("The translations of texts with placeholders are valid format strings in all languages and both copies")]
    public void Translations_Are_Valid_Format_Strings_Test()
    {
        var root = FindSourceRoot();

        if (root is null)
        {
            Assert.Ignore("The source code is not available next to the tests.");
        }

        var errors = new List<string>();
        var arguments = Enumerable.Range(0, 10).Select(i => (object)$"arg{i}").ToArray();

        foreach (var folder in new[]
                                 {
                                     Path.Combine(root, "YetAnotherForum.NET", "wwwroot", "languages"),
                                     Path.Combine(root, "YAFNET.UI", "YAFNET.RazorPages", "wwwroot", "languages")
                                 }.Where(Directory.Exists))
        {
            var english = ReadTexts(Path.Combine(folder, "english.json"));

            // only the texts that are formatted (a placeholder in the english text that is a valid format string)
            var formatted = english.Where(e => Placeholder().IsMatch(e.Value) && IsValidFormat(e.Value, arguments))
                .Select(e => e.Key).ToHashSet();

            foreach (var file in Directory.EnumerateFiles(folder, "*.json"))
            {
                foreach (var (key, text) in ReadTexts(file))
                {
                    if (formatted.Contains(key) && !IsValidFormat(text, arguments))
                    {
                        errors.Add($"{Path.GetRelativePath(root, file)}: {key} \"{text}\"");
                    }
                }
            }
        }

        errors.Should().BeEmpty();
    }

    private static bool IsValidFormat(string text, object[] arguments)
    {
        try
        {
            _ = string.Format(text, arguments);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static Dictionary<string, string> ReadTexts(string fileName)
    {
        using var json = JsonDocument.Parse(File.ReadAllText(fileName));

        var texts = new Dictionary<string, string>();

        foreach (var page in json.RootElement.GetProperty("Resources").GetProperty("page").EnumerateArray())
        {
            var name = page.GetProperty("@name").GetString()!.ToUpperInvariant();
            var resources = page.GetProperty("Resource");

            IEnumerable<JsonElement> list = resources.ValueKind == JsonValueKind.Array ? resources.EnumerateArray() : [resources];

            foreach (var resource in list)
            {
                var tag = resource.GetProperty("@tag").GetString()!.ToUpperInvariant();

                texts[$"{name}.{tag}"] = resource.TryGetProperty("#text", out var text) ? text.GetString() ?? string.Empty : string.Empty;
            }
        }

        return texts;
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

    [GeneratedRegex(@"\{\d+")]
    private static partial Regex Placeholder();
}
