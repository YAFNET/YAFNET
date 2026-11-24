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

using YAF.Core.Helpers;
using YAF.Types.Constants;

/// <summary>
/// The help texts name the labels of the forum with tokens ([[PAGE.TAG]], [[help:topic]], [[page:ForumPage]]).
/// </summary>
[TestFixture]
public class HelpTextTests
{
    [Test]
    [Description("The tokens are replaced by the label (bold, encoded, without a colon at the end) and the links")]
    public void ReplaceTokens_Test()
    {
        var text = HelpTextHelper.ReplaceTokens(
            "<p>Click [[TOOLBAR.INBOX]], see <a href=\"[[help:messenger]]\">x</a> and <a href=\"[[page:Account_Login]]\">y</a> [[page:Nothing]]</p>",
            (page, tag) => $"{page}/{tag} & <b>more</b>:",
            topic => $"/Help/{topic}",
            page => page == "Account_Login" ? "/Account/Login" : null);

        text.Should().Be(
            "<p>Click <strong>TOOLBAR/INBOX &amp; more</strong>, see <a href=\"/Help/messenger\">x</a> and <a href=\"/Account/Login\">y</a> #</p>");
    }

    [TestCase("Subject:", "Subject")]
    [TestCase("Add file(s)...", "Add file(s)")]
    [TestCase("Reconnexion automatique&nbsp;:", "Reconnexion automatique")]
    [TestCase("Show &amp; tell", "Show &amp; tell")]
    [Description("A label in the running text has no colon or ellipsis at its end and is encoded")]
    public void FormatLabel_Test(string label, string expected)
    {
        HelpTextHelper.FormatLabel(label).Should().Be(expected);
    }

    [Test]
    [Description("A text without tokens is not changed")]
    public void ReplaceTokens_Without_Tokens_Test()
    {
        HelpTextHelper.ReplaceTokens("<p>[b]bold[/b] [list]</p>", (_, _) => "x", _ => "x", _ => "x")
            .Should().Be("<p>[b]bold[/b] [list]</p>");
    }

    [Test]
    [Description("Every token of the help in every language names an existing text, help topic or forum page")]
    public void All_Help_Tokens_Exist_Test()
    {
        var root = FindSourceRoot();

        if (root is null)
        {
            Assert.Ignore("The source code is not available next to the tests.");
        }

        var folder = Path.Combine(root, "YAFNET.UI", "YAFNET.RazorPages", "wwwroot", "languages");
        var english = ReadTexts(Path.Combine(folder, "english.json"));

        using var menu = JsonDocument.Parse(
            File.ReadAllText(Path.Combine(root, "YAFNET.UI", "YAFNET.RazorPages", "wwwroot", "resources", "helpMenu.json")));
        var topics = menu.RootElement.EnumerateArray()
            .SelectMany(c => c.GetProperty("HelpPages").EnumerateArray().Select(p => p.GetString()!.ToLowerInvariant()))
            .ToHashSet();

        var errors = new List<string>();

        foreach (var file in Directory.EnumerateFiles(folder, "*.json"))
        {
            var texts = ReadTexts(file);

            foreach (var topic in topics)
            {
                if (!texts.TryGetValue($"HELP_INDEX.{topic.ToUpperInvariant()}CONTENT", out var content)
                    || !texts.ContainsKey($"HELP_INDEX.{topic.ToUpperInvariant()}TITLE"))
                {
                    errors.Add($"{Path.GetFileName(file)}: help topic {topic} is missing");
                    continue;
                }

                foreach (var token in HelpTextHelper.Token().Matches(content).Select(m => m.Groups))
                {
                    var kind = token["kind"].Value;
                    var value = token["value"].Value;

                    var valid = kind switch
                    {
                        "help" => topics.Contains(value.ToLowerInvariant()),
                        "page" => Enum.TryParse<ForumPages>(value, out _),
                        _ => english.ContainsKey($"{kind}.{value}")
                    };

                    if (!valid)
                    {
                        errors.Add($"{Path.GetFileName(file)}: {topic} {token[0].Value}");
                    }
                }
            }
        }

        errors.Should().BeEmpty();
    }

    private static Dictionary<string, string> ReadTexts(string fileName)
    {
        using var json = JsonDocument.Parse(File.ReadAllText(fileName));

        var texts = new Dictionary<string, string>();

        foreach (var page in json.RootElement.GetProperty("Resources").GetProperty("page").EnumerateArray())
        {
            foreach (var resource in page.GetProperty("Resource").EnumerateArray())
            {
                texts.TryAdd(
                    $"{page.GetProperty("@name").GetString()}.{resource.GetProperty("@tag").GetString()}",
                    resource.GetProperty("#text").GetString() ?? string.Empty);
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
}
